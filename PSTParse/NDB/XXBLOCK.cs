using System;

namespace PSTParse.NDB
{
    public class XXBLOCK : IBLOCK
    {
        public byte Type { get; private set; }
        public byte CLevel { get; private set; }
        public UInt16 TotalChildren { get; private set; }
        public uint TotalBytes { get; private set; }
        public BlockDataDTO Block { get; private set; }

        public ulong[] XBlockBIDs { get; private set; }

        public XXBLOCK(bool unicode, BlockDataDTO block)
        {
            Block = block;
            Type = block.Data[0];
            CLevel = block.Data[1];
            TotalChildren = BitConverter.ToUInt16(block.Data, 2);
            TotalBytes = BitConverter.ToUInt32(block.Data, 4);
            XBlockBIDs = new ulong[this.TotalChildren];
            for (var i = 0; i < TotalChildren; i++)
                XBlockBIDs[i] = unicode
                    ? BitConverter.ToUInt64(block.Data, 8 + 8 * i)
                    : BitConverter.ToUInt32(block.Data, 8 + 4 * i);
        }
    }
}
