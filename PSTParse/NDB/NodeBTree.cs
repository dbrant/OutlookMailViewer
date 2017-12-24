namespace PSTParse.NDB
{
    public class NodeBTree
    {
        public BREF RootLocation { get; private set; }
        public BTPage Root { get; set; }

        public NodeBTree(BREF root)
        {
            RootLocation = root;
        }
    }
}
