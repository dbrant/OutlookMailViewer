using System;

namespace PSTParse.NDB
{
    public class BREF
    {
        public UInt64 BID { get; private set; }
        public UInt64 IB { get; private set; }

        public bool IsInternal
        {
            get { return (BID & 0x02) > 0; }
        }

        public BREF(bool unicode, byte[] bref, int offset = 0)
        {
            BID = unicode ? BitConverter.ToUInt64(bref, offset) : BitConverter.ToUInt32(bref, offset);
            IB = unicode ? BitConverter.ToUInt64(bref, offset + 8) : BitConverter.ToUInt32(bref, offset + 4);
            BID = BID & 0xfffffffffffffffe;
        }
    }
}
