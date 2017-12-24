
namespace PSTParse.NDB
{
    public class PSTBTree
    {
        public BTPage Root { get; private set; }

        public PSTBTree(bool unicode, BREF bref, PSTFile pst)
        {
            using (var viewer = pst.PSTMMF.CreateViewAccessor((long)bref.IB, 512))
            {
                var data = new byte[512];
                viewer.ReadArray(0, data, 0, 512);
                Root = new BTPage(unicode, data, bref, pst);
            }
        }
    }
}
