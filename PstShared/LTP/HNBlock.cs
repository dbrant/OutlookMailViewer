using System;
using System.IO;
using MiscParseUtilities;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class HNBlock
    {
        public HNHDR Header { get; private set; }
        public HNPAGEHDR PageHeader { get; private set; }
        public HNBITMAPHDR BitMapPageHeader { get; private set; }

        public HNPAGEMAP PageMap { get; private set; }

        public UInt16 PageMapOffset { get; private set; }

        private BlockDataDTO _bytes;

        public HNBlock(int blockIndex, BlockDataDTO bytes)
        {
            _bytes = bytes;
            if (_bytes.Data.Length < 2)
            {
                // missing or unreadable block
                PageMap = new HNPAGEMAP(_bytes.Data, -1);
                return;
            }

            PageMapOffset = BitConverter.ToUInt16(_bytes.Data, 0);
            PageMap = new HNPAGEMAP(_bytes.Data, PageMapOffset);
            if (blockIndex == 0)
            {
                if (_bytes.Data.Length < 12)
                    return;
                Header = new HNHDR(_bytes.Data);
            } else if (blockIndex % 128 == 8)
            {
                if (_bytes.Data.Length < 66)
                    return;
                BitMapPageHeader = new HNBITMAPHDR(ref _bytes.Data);
            } else
            {
                PageHeader = new HNPAGEHDR(ref _bytes.Data);
            }
        }

        public HNDataDTO GetAllocation(HID hid)
        {
            if (hid.hidIndex == 0 || (int)hid.hidIndex >= PageMap.AllocationTable.Count)
                throw new InvalidDataException("Heap allocation " + hid.hidIndex + " not found.");
            var begOffset = PageMap.AllocationTable[(int) hid.hidIndex - 1];
            var endOffset = PageMap.AllocationTable[(int) hid.hidIndex];
            if (endOffset < begOffset || endOffset > _bytes.Data.Length)
                throw new InvalidDataException("Heap allocation " + hid.hidIndex + " is invalid.");
            return new HNDataDTO
                       {
                           Data = _bytes.Data.RangeSubset(begOffset, endOffset - begOffset),
                           BlockOffset = begOffset,
                           Parent = _bytes
                       };
        }

        public int GetOffset()
        {
            if (Header != null)
                return 12;
            if (PageHeader != null)
                return 2;
            return 66;
        }
    }
}
