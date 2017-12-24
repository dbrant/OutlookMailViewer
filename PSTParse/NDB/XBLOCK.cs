using System;

namespace PSTParse.NDB
{
    public class XBLOCK : IBLOCK
    {
        public BlockDataDTO Block { get; private set; }
        public uint BlockType { get; private set; }
        public uint HeaderLevel { get; private set; }
        public uint BIDEntryCount { get; private set; }
        public uint TotalBytes { get; private set; }

        public ulong[] BIDEntries { get; private set; }

        public XBLOCK(bool unicode, BlockDataDTO block)
        {
            Block = block;
            BlockType = block.Data[0];
            HeaderLevel = block.Data[1];
            BIDEntryCount = BitConverter.ToUInt16(block.Data, 2);
            TotalBytes = BitConverter.ToUInt32(block.Data, 4);
            BIDEntries = new ulong[BIDEntryCount];
            for (int i = 0; i < BIDEntryCount; i++)
                BIDEntries[i] = unicode
                    ? BitConverter.ToUInt64(block.Data, 8 + i * 8)
                    : BitConverter.ToUInt32(block.Data, 8 + i * 4);
        }
    }
}
