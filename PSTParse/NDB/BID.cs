using System;

namespace PSTParse.NDB
{
    public class BID
    {
        public ulong BlockID { get; private set; }

        public BID(bool unicode, byte[] bytes, int offset = 0)
        {
            BlockID = unicode ? BitConverter.ToUInt64(bytes, offset) : BitConverter.ToUInt32(bytes, offset);
            BlockID &= 0xfffffffffffffffe;
        }
    }
}
