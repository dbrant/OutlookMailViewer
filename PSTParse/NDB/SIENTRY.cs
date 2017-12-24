using System;

namespace PSTParse.NDB
{
    public class SIENTRY
    {
        public ulong NextChildNID;
        public ulong SLBlockBID;

        public SIENTRY(bool unicode, byte[] bytes)
        {
            this.NextChildNID = unicode ? BitConverter.ToUInt64(bytes, 0) : BitConverter.ToUInt32(bytes, 0);
            this.SLBlockBID = unicode ? BitConverter.ToUInt64(bytes, 8) : BitConverter.ToUInt32(bytes, 4);
        }
    }
}
