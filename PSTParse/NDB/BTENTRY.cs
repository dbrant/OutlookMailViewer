using System;
using System.Linq;

namespace PSTParse.NDB
{
    public class BTENTRY : BTPAGEENTRY
    {
        private ulong _btkey;
        public BREF BREF;

        public BTENTRY(bool unicode, byte[] bytes)
        {
            if (unicode)
            {
                _btkey = BitConverter.ToUInt64(bytes, 0);
                BREF = new BREF(unicode, bytes.Skip(8).Take(16).ToArray());
            }
            else
            {
                _btkey = BitConverter.ToUInt32(bytes, 0);
                BREF = new BREF(unicode, bytes.Skip(4).Take(8).ToArray());
            }
        }

        public ulong Key
        {
            get { return _btkey; }
        }
    }
}
