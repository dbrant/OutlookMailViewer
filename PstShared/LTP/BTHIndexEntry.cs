using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class BTHIndexEntry
    {
        public byte[] Key { get; private set; }
        public HID HID { get; private set; }

        public BTHIndexEntry(byte[] bytes, int offset, BTHHEADER header)
        {
            Key = bytes.RangeSubset(offset,(int)header.KeySize);
            var temp = offset + (int) header.KeySize;
            HID = new HID(bytes.RangeSubset(temp, 4));

        }
    }
}
