using System;

namespace PSTParse.NDB
{
    public class SIENTRY
    {
        public ulong NextChildNID { get; private set; }
        public ulong SLBlockBID { get; private set; }

        public SIENTRY(bool unicode, byte[] bytes)
        {
            NextChildNID = unicode ? BitConverter.ToUInt64(bytes, 0) : BitConverter.ToUInt32(bytes, 0);
            SLBlockBID = unicode ? BitConverter.ToUInt64(bytes, 8) : BitConverter.ToUInt32(bytes, 4);
        }
    }
}
