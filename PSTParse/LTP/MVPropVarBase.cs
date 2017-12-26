using System;
using System.Linq;
using System.Collections.Generic;

namespace PSTParse.LTP
{
    public class MVPropVarBase
    {
        public UInt32 PropCount { get; private set; }
        private List<ulong> PropOffsets;
        private List<byte[]> PropDataItems;

        public MVPropVarBase(byte[] bytes)
        {
            PropCount = BitConverter.ToUInt32(bytes, 0);
            PropOffsets = new List<ulong>();

            for(int i= 0;i < PropCount; i++)
                PropOffsets.Add(BitConverter.ToUInt64(bytes, 4 + i*8));

            PropDataItems = new List<byte[]>();
            for(int i = 0;i < PropCount; i++)
            {
                if (i < PropCount-1)
                {
                    PropDataItems.Add(
                        bytes.Skip((int) PropOffsets[i]).Take((int) (PropOffsets[i + 1] - PropOffsets[i]))
                            .ToArray());
                }
            }
        }
    }
}
