using System;

namespace PSTParse.NDB
{
    public class SLENTRY
    {
        public ulong SubNodeNID;
        public ulong SubNodeBID;
        public ulong SubSubNodeBID;

        public SLENTRY(bool unicode, byte[] bytes)
        {
            this.SubNodeNID = unicode ? BitConverter.ToUInt64(bytes, 0) : BitConverter.ToUInt32(bytes, 0);
            this.SubNodeBID = unicode ? BitConverter.ToUInt64(bytes, 8) : BitConverter.ToUInt32(bytes, 4);
            this.SubSubNodeBID = unicode ? BitConverter.ToUInt64(bytes, 16) : BitConverter.ToUInt32(bytes, 8);
        }
    }
}
