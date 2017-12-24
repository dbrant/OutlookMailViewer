﻿
namespace PSTParse.NDB
{
    public class BlockDataDTO
    {
        public BlockDataDTO Parent;
        public byte[] Data;
        public ulong PstOffset;
        public uint CRC32;
        public uint CRCOffset;
        public BBTENTRY BBTEntry;
    }
}
