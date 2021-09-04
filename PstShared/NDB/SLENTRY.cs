using System;

namespace PSTParse.NDB
{
    public class SLENTRY
    {
        public ulong SubNodeNID { get; private set; }
        public ulong SubNodeBID { get; private set; }
        public ulong SubSubNodeBID { get; private set; }

        public SLENTRY(bool unicode, byte[] bytes)
        {
            SubNodeNID = unicode ? BitConverter.ToUInt64(bytes, 0) : BitConverter.ToUInt32(bytes, 0);
            SubNodeBID = unicode ? BitConverter.ToUInt64(bytes, 8) : BitConverter.ToUInt32(bytes, 4);
            SubSubNodeBID = unicode ? BitConverter.ToUInt64(bytes, 16) : BitConverter.ToUInt32(bytes, 8);
        }
    }
}
