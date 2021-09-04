using System;

namespace PSTParse.NDB
{
    public class BlockTrailer
    {
        public uint DataSize { get; private set; }
        public uint WSig { get; private set; }
        public uint CRC { get; private set; }
        public ulong BID_raw { get; private set; }

        public BlockTrailer(bool unicode, byte[] bytes, int offset)
        {
            DataSize = BitConverter.ToUInt16(bytes, offset);
            WSig = BitConverter.ToUInt16(bytes, 2 + offset);
            CRC = unicode ? BitConverter.ToUInt32(bytes, 4 + offset) : BitConverter.ToUInt32(bytes, 8 + offset);
            BID_raw = unicode ? BitConverter.ToUInt64(bytes, 8 + offset) : BitConverter.ToUInt32(bytes, 4 + offset);
        }
    }
}
