using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class BTHDataEntry
    {
        public byte[] Key { get; private set; }
        public byte[] Data { get; private set; }

        public ulong DataOffset { get; private set; }
        public ulong DataBlockOffset { get; private set; }
        public BTH ParentTree { get; private set; }

        public BTHDataEntry(HNDataDTO data, int offset, BTH tree)
        {
            Key = data.Data.RangeSubset(offset, (int) tree.Header.KeySize);
            //Key = bytes.Skip(offset).Take((int)tree.Header.KeySize).ToArray();
            Data = data.Data.RangeSubset(offset + (int)tree.Header.KeySize, (int)tree.Header.DataSize);
            DataOffset = (ulong) offset + tree.Header.KeySize;
            ParentTree = tree;
        }
    }
}
