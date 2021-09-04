using System;
using System.Linq;

namespace PSTParse.LTP
{
    public class HNBITMAPHDR
    {
        public uint HNPageMapOffset { get; private set; }
        public byte[] FillLevel { get; private set; }

        public HNBITMAPHDR(ref byte[] bytes)
        {
            HNPageMapOffset = BitConverter.ToUInt16(bytes, 0);
            FillLevel = bytes.Skip(2).Take(64).ToArray();
        }
    }
}
