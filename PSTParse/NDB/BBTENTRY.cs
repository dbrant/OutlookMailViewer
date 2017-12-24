using System;

namespace PSTParse.NDB
{
    public class BBTENTRY : BTPAGEENTRY
    {
        public BREF BREF;
        public bool Internal;
        public UInt16 BlockByteCount;
        public UInt16 RefCount;

        public BBTENTRY(bool unicode, byte[] bytes)
        {
            BREF = new BREF(unicode, bytes);
            Internal = BREF.IsInternal;
            if (unicode)
            {
                BlockByteCount = BitConverter.ToUInt16(bytes, 16);
                RefCount = BitConverter.ToUInt16(bytes, 18);
            }
            else
            {
                BlockByteCount = BitConverter.ToUInt16(bytes, 8);
                RefCount = BitConverter.ToUInt16(bytes, 10);
            }
        }

        public ulong Key
        {
            get { return BREF.BID; }
        }
    }
}
