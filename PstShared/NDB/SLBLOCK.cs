using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    public class SLBLOCK : IBLOCK
    {
        public BlockDataDTO BlockData { get; private set; }
        public UInt16 EntryCount { get; private set; }
        public List<SLENTRY> Entries { get; private set; }

        public SLBLOCK(bool unicode, BlockDataDTO blockData)
        {
            BlockData = blockData;
            var type = blockData.Data[0];
            var clevel = blockData.Data[1];
            EntryCount = BitConverter.ToUInt16(blockData.Data, 2);
            Entries = new List<SLENTRY>();
            for (int i = 0; i < EntryCount; i++)
                Entries.Add(unicode
                            ? new SLENTRY(unicode, blockData.Data.RangeSubset(8 + 24 * i, 24))
                            : new SLENTRY(unicode, blockData.Data.RangeSubset(4 + 12 * i, 12)));
        }
    }
}
