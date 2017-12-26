using System;

namespace PSTParse.LTP
{
    public class HID
    {
        public ulong HID_Type { get; private set; }
        //the index in the allocations for the specific heap block.
        public ulong hidIndex { get; private set; }
        //the index in the block array for this heap
        public ulong hidBlockIndex { get; private set; }

        public HID(byte[] bytes, int offset = 0)
        {
            var temp = BitConverter.ToUInt32(bytes, offset);
            this.HID_Type = temp & 0x1F;
            this.hidIndex = (temp >> 5) & 0x7FF;
            this.hidBlockIndex = BitConverter.ToUInt16(bytes, offset + 2);
        }
    }
}
