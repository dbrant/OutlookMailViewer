using System;
using System.Linq;

namespace PSTParse.LTP
{
    public class BTHDataRecord
    {
        public uint Key { get; private set; }
        public uint Value { get; private set; }

        public BTHDataRecord(byte[] bytes, BTHHEADER header)
        {
            var keySize = (int)header.KeySize;
            var dataSize = (int)header.DataSize;

            Key = BitConverter.ToUInt16(bytes.Take(keySize).ToArray(), 0);
            Value = BitConverter.ToUInt32(bytes.Skip(keySize).Take(dataSize).ToArray(), 0);
        }
    }
}
