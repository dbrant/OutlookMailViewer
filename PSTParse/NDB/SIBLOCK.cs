using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    public class SIBLOCK : IBLOCK
    {
        public BlockDataDTO DataBlock { get; private set; }
        public UInt16 EntryCount { get; private set; }
        public List<SIENTRY> Entries { get; private set; }

        public SIBLOCK(bool unicode, BlockDataDTO dataBlock)
        {
            DataBlock = dataBlock;
            var type = dataBlock.Data[0];
            var cLevel = dataBlock.Data[1];
            EntryCount = BitConverter.ToUInt16(dataBlock.Data, 2);
            Entries = new List<SIENTRY>();
            for (int i = 0; i < EntryCount; i++)
                Entries.Add(unicode
                            ? new SIENTRY(unicode, dataBlock.Data.RangeSubset(8 + 16 * i, 16))
                            : new SIENTRY(unicode, dataBlock.Data.RangeSubset(8 + 8 * i, 8)));
        }
    }
}
