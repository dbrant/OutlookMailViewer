using System;

namespace PSTParse.NDB
{
    public class BREF
    {
        public UInt64 BID;
        public UInt64 IB;

        public bool IsInternal
        {
            get { return (BID & 0x02) > 0; }
        }

        public BREF(bool unicode, byte[] bref, int offset = 0)
        {
            if (unicode)
            {
                BID = BitConverter.ToUInt64(bref, offset);
                IB = BitConverter.ToUInt64(bref, offset + 8);
            } else
            {
                BID = BitConverter.ToUInt32(bref, offset);
                IB = BitConverter.ToUInt32(bref, offset + 4);
            }
            BID = BID & 0xfffffffffffffffe;
        }
    }
}
