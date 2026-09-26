using System;
using System.Collections.Generic;
using System.IO;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    /// <summary>
    /// Locates blocks by scanning the raw file for block trailers whose CRC matches the preceding data,
    /// independently of the Block B-tree. This allows recovery of blocks whose BBT pages are damaged,
    /// or whose data has been displaced from its expected offset (e.g. in an imperfect disk image).
    /// </summary>
    public static class BlockScanner
    {
        private const int ChunkSize = 16 * 1024 * 1024;
        private const int MaxBlockSize = 8192;

        public static Dictionary<ulong, BBTENTRY> Scan(string path, bool unicode, ulong maxBid)
        {
            var found = new Dictionary<ulong, BBTENTRY>();
            var crc = new CRC32();
            int trailerLen = unicode ? 16 : 12;
            int maxDataSize = MaxBlockSize - trailerLen;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            long fileLen = stream.Length;
            var buffer = new byte[ChunkSize + MaxBlockSize];
            long bufStart = 0;

            while (bufStart < fileLen)
            {
                stream.Seek(bufStart, SeekOrigin.Begin);
                int bufLen = 0;
                while (bufLen < buffer.Length)
                {
                    int n = stream.Read(buffer, bufLen, buffer.Length - bufLen);
                    if (n <= 0) break;
                    bufLen += n;
                }

                // Trailers located in the first MaxBlockSize bytes of a chunk were already examined as part
                // of the previous chunk (which overlaps this one), except for the very first chunk.
                int first = bufStart == 0 ? 0 : MaxBlockSize - trailerLen + 1;
                int last = bufLen - trailerLen;
                for (int t = first; t <= last; t++)
                {
                    int cb = BitConverter.ToUInt16(buffer, t);
                    if (cb == 0 || cb > maxDataSize)
                        continue;
                    ulong bid = unicode ? BitConverter.ToUInt64(buffer, t + 8) : BitConverter.ToUInt32(buffer, t + 4);
                    bid &= 0xfffffffffffffffe;
                    if (bid == 0 || bid > maxBid)
                        continue;

                    int blockSize = (cb + trailerLen + 63) / 64 * 64;
                    int start = t + trailerLen - blockSize;
                    if (start < 0)
                        continue;

                    uint storedCrc = unicode ? BitConverter.ToUInt32(buffer, t + 4) : BitConverter.ToUInt32(buffer, t + 8);
                    if (crc.ComputeCRC(0, buffer, start, (uint)cb) != storedCrc)
                        continue;

                    long absStart = bufStart + start;
                    // If multiple copies are found, prefer one that is properly aligned.
                    if (!found.TryGetValue(bid, out var existing) || ((long)existing.BREF.IB % 64 != 0 && absStart % 64 == 0))
                        found[bid] = new BBTENTRY(bid, (ulong)absStart, (ushort)cb);
                }

                if (bufStart + bufLen >= fileLen)
                    break;
                bufStart += last + 1 - (MaxBlockSize - trailerLen + 1);
            }
            return found;
        }
    }
}
