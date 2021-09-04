using System.Collections.Generic;

namespace PSTParse.LTP
{
    public class BTHIndexNode
    {
        public HID HID { get; private set; }
        public int Level { get; private set; }

        public List<BTHIndexEntry> Entries { get; private set; }
        public List<BTHIndexNode> Children { get; private set; }
        public BTHDataNode Data { get; private set; }

        public BTHIndexNode(HID hid, BTH tree, int level)
        {
            Level = level;
            HID = hid;
            if (hid.hidBlockIndex == 0 && hid.hidIndex == 0)
                return;
            
            Entries = new List<BTHIndexEntry>();

            if (level == 0)
            {
                Data = new BTHDataNode(hid, tree);
                /*
                for (int i = 0; i < bytes.Length; i += (int)tree.Header.KeySize + 4)
                    Entries.Add(new BTHIndexEntry(bytes, i, tree.Header));
                DataChildren = new List<BTHDataNode>();
                foreach(var entry in Entries)
                    DataChildren.Add(new BTHDataNode(entry.HID, tree));*/
            } else
            {
                var bytes = tree.GetHIDBytes(hid);
                for (int i = 0; i < bytes.Data.Length; i += (int)tree.Header.KeySize + 4)
                    Entries.Add(new BTHIndexEntry(bytes.Data, i, tree.Header));
                Children = new List<BTHIndexNode>();
                foreach(var entry in Entries)
                    Children.Add(new BTHIndexNode(entry.HID, tree, level - 1));
            }

        }

        public bool BlankPassword(PSTFile pst)
        {
            if (Data != null)
                return Data.BlankPassword(pst);

            foreach (var child in Children)
                child.BlankPassword(pst);
                /*if (child.BlankPassword(Data) != null)
                    return child.BlankPassword(Data);*/

            return false;
        }
    }
}
