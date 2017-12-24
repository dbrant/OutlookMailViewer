namespace PSTParse.NDB
{
    public class NodeBTree
    {
        public BREF RootLocation;
        public BTPage Root;

        public NodeBTree(BREF root)
        {
            RootLocation = root;
        }
    }
}
