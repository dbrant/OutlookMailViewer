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
            AllocationsCount = BitConverter.ToUInt16(bytes, offset);
            FreeItemsCount = BitConverter.ToUInt16(bytes, offset+2);
            AllocationTable = new List<UInt16>();

            for(int i= 0;i < AllocationsCount+1;i++)
                AllocationTable.Add(BitConverter.ToUInt16(bytes,offset+4+i*2));
        }
    }
}
