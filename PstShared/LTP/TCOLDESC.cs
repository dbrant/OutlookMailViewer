using System;

namespace PSTParse.LTP
{
    public class TCOLDESC
    {
        public uint Tag { get; private set; }
        public ushort DataOffset { get; private set; }
        public ushort DataSize { get; private set; }
        public ushort CEBIndex { get; private set; }

        public TCOLDESC(byte[] bytes, int offset = 0)
        {
            Tag = BitConverter.ToUInt32(bytes, offset);
            DataOffset = BitConverter.ToUInt16(bytes, offset + 4);
            DataSize = bytes[offset + 6];
            CEBIndex = bytes[offset + 7];
        }
    }
}
