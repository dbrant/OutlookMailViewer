using System;
using System.Text;
using PSTParse.NDB;

namespace PSTParse
{
    public class PSTHeader
    {
        public string DWMagic { get; private set; }
        public bool isUnicode { get; private set; }

        public NDB.PSTBTree NodeBT { get; private set; }
        public NDB.PSTBTree BlockBT { get; private set; }

        public BlockEncoding EncodingAlgotihm;
        public enum BlockEncoding
        {
            NONE=0,
            PERMUTE=1,
            CYCLIC=2
        }

        public PSTHeader(PSTFile pst)
        {
            var tempBytes = new byte[684];
            using (var mmfView = pst.PSTMMF.CreateViewAccessor(0, tempBytes.Length))
            {
                mmfView.ReadArray(0, tempBytes, 0, tempBytes.Length);
                DWMagic = Encoding.ASCII.GetString(tempBytes, 0, 4);

                var ver = BitConverter.ToUInt16(tempBytes, 10);

                bool ansi = (ver == 14 || ver == 15);
                isUnicode = !ansi;

                //root.PSTSize = ByteReverse.ReverseULong(root.PSTSize);
                var sentinel = tempBytes[isUnicode ? 512 : 460];
                EncodingAlgotihm = (BlockEncoding) tempBytes[isUnicode ? 513 : 461];

                var nbt_bref = new BREF(isUnicode, tempBytes, isUnicode ? 216 : 184);
                var bbt_bref = new BREF(isUnicode, tempBytes, isUnicode ? 232 : 192);

                NodeBT = new NDB.PSTBTree(isUnicode, nbt_bref, pst);
                BlockBT = new NDB.PSTBTree(isUnicode, bbt_bref, pst);
            }
        }
    }
}
