using System;

namespace PSTParse.LTP
{
    public class HNID
    {
        public ulong HNID_Type { get; private set; }
        public ulong hnidIndex { get; private set; }
        public ulong hnidBlockIndex { get; private set; }

        public HNID(byte[] bytes)
        {
            var temp = BitConverter.ToUInt64(bytes, 0);
            HNID_Type = temp & 0x1F;
            hnidIndex = (temp >> 5) & 0x4FF;
            hnidBlockIndex = temp >> 16;
        }
    }
}
