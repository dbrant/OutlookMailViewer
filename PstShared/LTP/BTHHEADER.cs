using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class BTHHEADER
    {
        public uint BType { get; private set; }
        //must be 2,4,8,16
        public uint KeySize { get; private set; }
        //must be >0 <=32
        public uint DataSize { get; private set; }
        public uint NumLevels { get; private set; }
        public HID BTreeRoot { get; private set; }

        public BTHHEADER(HNDataDTO block)
        {
            var bytes = block.Data;
            BType = bytes[0];
            KeySize = bytes[1];
            DataSize = bytes[2];
            NumLevels = bytes[3];
            BTreeRoot = new HID(bytes.RangeSubset(4, 4));
        }
    }
}
