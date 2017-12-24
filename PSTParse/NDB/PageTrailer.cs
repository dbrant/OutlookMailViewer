﻿using System;

namespace PSTParse.NDB
{
    public enum PageType
    {
        //blockBTree
        BBT = 0x80,
        //nodeBTree
        NBT = 0x81,
        FreeMap = 0x82,
        PageMap = 0x83,
        AMap = 0x84,
        FreePageMap = 0x85,
        DensityList = 0x86
    }
    public class PageTrailer
    {
        public PageType PageType { get; set; }
        public ulong BID { get; set; }

        public PageTrailer(bool unicode, byte[] trailer)
        {
            this.PageType = (PageType) trailer[0];
            this.BID = unicode ? BitConverter.ToUInt64(trailer, 8) : BitConverter.ToUInt32(trailer, 4);
        }
    }

}
