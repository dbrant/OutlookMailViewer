using System;
using MiscParseUtilities;

namespace PSTParse.Message_Layer
{
    public class NAMEID
    {
        public UInt32 PropertyID { get; private set; }
        public bool PropertyIDStringOffset { get; private set; }
        public Guid Guid { get; private set; }
        public UInt16 PropIndex { get; private set; }

        public NAMEID(byte[] bytes, int offset, NamedToPropertyLookup lookup)
        {
            PropertyID = BitConverter.ToUInt32(bytes, offset);
            PropertyIDStringOffset = (bytes[offset + 4] & 0x1) == 1;
            var guidType = BitConverter.ToUInt16(bytes, offset + 4) >>1;
            if (guidType == 1)
            {
                Guid = new Guid("00020328-0000-0000-C000-000000000046");//PS-MAPI
            } else if (guidType == 2)
            {
                Guid = new Guid("00020329-0000-0000-C000-000000000046");//PS_PUBLIC_STRINGS
            } else
            {
                Guid = new Guid(lookup._GUIDs.RangeSubset((guidType - 3)*16, 16));
            }

            PropIndex = (UInt16)(0x8000 + BitConverter.ToUInt16(bytes, offset + 6));
        }
    }
}
