using System;
using System.Linq;
using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class PCBTHRecord
    {
        public UInt16 PropID { get; private set; }
        public UInt16 PropType { get; private set; }
        //public ExchangeProperty PropertyValue { get; private set; }

        public PCBTHRecord(byte[] bytes)
        {
            PropID = BitConverter.ToUInt16(bytes.Take(2).ToArray(), 0);
            PropType = BitConverter.ToUInt16(bytes.Skip(2).Take(2).ToArray(), 0);
            //var prop= PropertyValue = ExchangeProperty.PropertyLookupByTypeID[PropType];
            //if (!prop.MultiValue)
            //{
            //    if (!prop.Variable)
            //    {
            //        if (prop.ByteCount <= 4 && prop.ByteCount != 0)
            //        {
            //            PropertyValue.Data = bytes.RangeSubset(4, (int) prop.ByteCount);
            //        }
            //        else
            //        {
            //            
            //        }
            //    }
            //}
            //HNID = new HNID(bytes.Skip(4).ToArray());
        }
    }
}
