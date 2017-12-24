using System;

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
        public PageType PageType { get; private set; }
        public ulong BID { get; private set; }

        public PageTrailer(bool unicode, byte[] trailer)
        {
            PageType = (PageType) trailer[0];
            BID = unicode ? BitConverter.ToUInt64(trailer, 8) : BitConverter.ToUInt32(trailer, 4);
        }
    }
}
