using System;
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

            PageMapOffset = BitConverter.ToUInt16(_bytes.Data, 0);
            PageMap = new HNPAGEMAP(_bytes.Data, PageMapOffset);
            if (blockIndex == 0)
            {
                Header = new HNHDR(_bytes.Data);
            } else if (blockIndex % 128 == 8)
            {
                BitMapPageHeader = new HNBITMAPHDR(ref _bytes.Data);
            } else
            {
                PageHeader = new HNPAGEHDR(ref _bytes.Data);
            }
        }

        public HNDataDTO GetAllocation(HID hid)
        {
            var begOffset = PageMap.AllocationTable[(int) hid.hidIndex - 1];
            var endOffset = PageMap.AllocationTable[(int) hid.hidIndex];
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
