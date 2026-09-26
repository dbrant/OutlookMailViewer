using System;
using System.Collections;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class TCRowMatrixData : IEnumerable<ExchangeProperty>
    {
        public Dictionary<uint, byte[]> ColumnXREF { get; private set; }
        private BTH _heap;

        public TCRowMatrixData(byte[] bytes, TableContext context, BTH heap, int offset = 0)
        {
            ColumnXREF = new Dictionary<uint, byte[]>();
            _heap = heap;

            //todo: cell existence test
            //var rowSize = context.TCHeader.EndOffsetCEB;
            foreach (var col in context.TCHeader.ColumnsDescriptors)
            {
                ColumnXREF.Add(col.Tag, bytes.RangeSubset(offset + col.DataOffset, col.DataSize));
            }
        }


        public IEnumerator<ExchangeProperty> GetEnumerator()
        {
            foreach(var col in ColumnXREF)
            {
                ExchangeProperty prop;
                try
                {
                    prop = new ExchangeProperty((UInt16) (col.Key >> 16), (UInt16) (col.Key & 0xFFFF), _heap, col.Value);
                }
                catch (Exception)
                {
                    // skip a property whose data is damaged.
                    continue;
                }
                yield return prop;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
