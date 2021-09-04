using System;
using System.Collections.Generic;

namespace PSTParse.LTP
{
    public class TCINFOHEADER
    {
        public byte Type;
        public ushort ColumnCount;
        public ushort EndOffset48;
        public ushort EndOffset2;
        public ushort EndOffset1;
        public ushort EndOffsetCEB;
        public HID RowIndexLocation;
        public ulong RowMatrixLocation;

        public List<TCOLDESC> ColumnsDescriptors;

        public TCINFOHEADER(byte[] bytes)
        {
            Type = bytes[0];
            ColumnCount = bytes[1];
            EndOffset48 = BitConverter.ToUInt16(bytes, 2);
            EndOffset2 = BitConverter.ToUInt16(bytes, 4);
            EndOffset1 = BitConverter.ToUInt16(bytes, 6);
            EndOffsetCEB = BitConverter.ToUInt16(bytes, 8);
            RowIndexLocation = new HID(bytes, 10);
            RowMatrixLocation = BitConverter.ToUInt32(bytes, 14);
            ColumnsDescriptors = new List<TCOLDESC>();
            for (var i = 0; i < ColumnCount; i++)
            {
                ColumnsDescriptors.Add(new TCOLDESC(bytes, 22 + i * 8));
            }
        }
    }
}
