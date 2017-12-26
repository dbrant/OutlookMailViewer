using System;

namespace PSTParse.LTP
{
    public class HNPAGEHDR
    {
        public UInt16 HNPageMapOffset { get; private set; }

        public HNPAGEHDR(ref byte[] bytes)
        {
            HNPageMapOffset = BitConverter.ToUInt16(bytes, 0);
        }
    }
}
