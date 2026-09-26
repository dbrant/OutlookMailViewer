using System;

namespace PSTParse.NDB
{
    public class BBTENTRY : BTPAGEENTRY
    {
        public BREF BREF { get; private set; }
        public bool Internal { get; private set; }
        public UInt16 BlockByteCount { get; private set; }
        public UInt16 RefCount { get; private set; }

        public BBTENTRY(bool unicode, byte[] bytes)
        {
            BREF = new BREF(unicode, bytes);
            Internal = BREF.IsInternal;
            BlockByteCount = unicode ? BitConverter.ToUInt16(bytes, 16) : BitConverter.ToUInt16(bytes, 8);
            RefCount = unicode ? BitConverter.ToUInt16(bytes, 18) : BitConverter.ToUInt16(bytes, 10);
        }

        // Constructs an entry for a block located by scanning the file, rather than through the BBT.
        public BBTENTRY(ulong bid, ulong ib, UInt16 byteCount)
        {
            BREF = new BREF(bid, ib);
            Internal = BREF.IsInternal;
            BlockByteCount = byteCount;
        }

        public ulong Key
        {
            get { return BREF.BID; }
        }
    }
}
