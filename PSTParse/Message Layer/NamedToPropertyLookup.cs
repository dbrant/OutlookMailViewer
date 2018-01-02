using System.Collections.Generic;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class NamedToPropertyLookup
    {
        private static ulong NODE_ID = 0x61;

        public PropertyContext PC { get; private set; }
        public Dictionary<ushort, NAMEID> Lookup { get; private set; }

        internal byte[] _GUIDs;
        internal byte[] _entries;
        internal byte[] _string;

        public NamedToPropertyLookup(PSTFile pst)
        {
            
            PC = new PropertyContext(NODE_ID, pst);
            _GUIDs = PC.Properties[MessageProperty.GuidList].Data;
            _entries = PC.Properties[MessageProperty.EntryList].Data;
            _string = PC.Properties[MessageProperty.StringList].Data;

            Lookup = new Dictionary<ushort, NAMEID>();
            for (int i = 0; i < _entries.Length; i += 8)
            {
                var cur = new NAMEID(_entries, i, this);
                Lookup.Add(cur.PropIndex, cur);
            }
        }
    }
}
