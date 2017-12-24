using System;

namespace PSTParse.NDB
{
    public class NBTENTRY : BTPAGEENTRY
    {
        public ulong NID { get; private set; }
        public ulong BID_Data { get; private set; }
        public ulong BID_SUB { get; private set; }
        public ulong NID_TYPE { get; private set; }
        public uint NID_Parent { get; private set; }

        public NBTENTRY(bool unicode, byte[] curEntryBytes)
        {
            if (unicode)
            {
                NID = BitConverter.ToUInt64(curEntryBytes, 0);
                BID_Data = BitConverter.ToUInt64(curEntryBytes, 8);
                BID_SUB = BitConverter.ToUInt64(curEntryBytes, 16);
                NID_Parent = BitConverter.ToUInt32(curEntryBytes, 24);
            }
            else
            {
                NID = BitConverter.ToUInt32(curEntryBytes, 0);
                BID_Data = BitConverter.ToUInt32(curEntryBytes, 4);
                BID_SUB = BitConverter.ToUInt32(curEntryBytes, 8);
                NID_Parent = BitConverter.ToUInt32(curEntryBytes, 12);
            }
            NID_TYPE = NID & 0x1f;
        }
    }
}
