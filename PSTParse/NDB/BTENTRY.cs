using System;
using System.Linq;

namespace PSTParse.NDB
{
    public class BTENTRY : BTPAGEENTRY
    {
        public ulong Key { get; private set; }
        public BREF BREF { get; private set; }

        public BTENTRY(bool unicode, byte[] bytes)
        {
            Key = unicode ? BitConverter.ToUInt64(bytes, 0) : BitConverter.ToUInt32(bytes, 0);
            BREF = unicode ? new BREF(unicode, bytes.Skip(8).Take(16).ToArray()) : new BREF(unicode, bytes.Skip(4).Take(8).ToArray());
        }
    }
}
