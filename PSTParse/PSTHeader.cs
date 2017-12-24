using System;
using System.Text;
using PSTParse.NDB;

namespace PSTParse
{
    public class PSTHeader
    {
        public string DWMagic { get; set; }
        public bool isUnicode { get; private set; }

        public NDB.PSTBTree NodeBT { get; set; }
        public NDB.PSTBTree BlockBT { get; set; }

        public BlockEncoding EncodingAlgotihm;
        public enum BlockEncoding
        {
            NONE=0,
            PERMUTE=1,
            CYCLIC=2
        }

        public PSTHeader(PSTFile pst)
        {
            using (var mmfView = pst.PSTMMF.CreateViewAccessor(0, 684))
            {
                var temp = new byte[4];
                mmfView.ReadArray(0, temp, 0, 4);
                DWMagic = Encoding.ASCII.GetString(temp);

                var ver = mmfView.ReadInt16(10);

                bool ansi = (ver == 14 || ver == 15);
                isUnicode = !ansi;
                
                //root.PSTSize = ByteReverse.ReverseULong(root.PSTSize);
                var sentinel = isUnicode ? mmfView.ReadByte(512) : mmfView.ReadByte(460);
                var cryptMethod = (uint)(isUnicode ? mmfView.ReadByte(513) : mmfView.ReadByte(461));
                EncodingAlgotihm = (BlockEncoding) cryptMethod;

                var bytes = new byte[16];
                if (isUnicode) { mmfView.ReadArray(216, bytes, 0, 16); }
                else { mmfView.ReadArray(184, bytes, 0, 8); }
                var nbt_bref = new BREF(isUnicode, bytes);

                if (isUnicode) { mmfView.ReadArray(232, bytes, 0, 16); }
                else { mmfView.ReadArray(192, bytes, 0, 8); }
                var bbt_bref = new BREF(isUnicode, bytes);

                NodeBT = new NDB.PSTBTree(isUnicode, nbt_bref, pst);
                BlockBT = new NDB.PSTBTree(isUnicode, bbt_bref, pst);
            }
        }
    }
}
