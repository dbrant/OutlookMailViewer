using System;
using System.Collections.Generic;

namespace PSTParse.LTP
{
    public class HNPAGEMAP
    {
        public uint AllocationsCount { get; private set; }
        public uint FreeItemsCount { get; private set; }
        public List<UInt16> AllocationTable { get; private set; }

        public HNPAGEMAP(byte[] bytes, int offset)
        {
            AllocationTable = new List<UInt16>();
            if (offset < 0 || offset + 4 > bytes.Length)
                return;

            AllocationsCount = BitConverter.ToUInt16(bytes, offset);
            FreeItemsCount = BitConverter.ToUInt16(bytes, offset+2);

            // If the page map is damaged, treat the heap block as having no allocations.
            if (offset + 4 + (AllocationsCount + 1) * 2 > bytes.Length)
                return;

            for (int i = 0; i < AllocationsCount + 1; i++)
                AllocationTable.Add(BitConverter.ToUInt16(bytes, offset + 4 + i * 2));
        }
    }
}
