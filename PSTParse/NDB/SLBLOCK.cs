using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    public class SLBLOCK : IBLOCK
    {
        public BlockDataDTO BlockData;
        public UInt16 EntryCount;
        public List<SLENTRY> Entries;

        public SLBLOCK(bool unicode, BlockDataDTO blockData)
        {
            this.BlockData = blockData;
            var type = blockData.Data[0];
            var clevel = blockData.Data[1];
            this.EntryCount = BitConverter.ToUInt16(blockData.Data, 2);
            this.Entries = new List<SLENTRY>();
            for(int i= 0; i  < EntryCount; i++)
                Entries.Add(unicode
                    ? new SLENTRY(unicode, blockData.Data.RangeSubset(8 + 24 * i, 24))
                    : new SLENTRY(unicode, blockData.Data.RangeSubset(4 + 12 * i, 12)));
        }

    }
}
