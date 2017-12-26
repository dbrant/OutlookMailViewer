using System;
using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class EntryID
    {
        public uint Flags { get; private set; }
        public byte[] PSTUID { get; private set; }
        public ulong NID { get; private set; }

        public EntryID(byte[] bytes, int offset = 0)
        {
            Flags = BitConverter.ToUInt32(bytes, offset);
            PSTUID = bytes.RangeSubset(4 + offset, 16);
            NID = BitConverter.ToUInt32(bytes, offset + 20);
        }
    }
}
