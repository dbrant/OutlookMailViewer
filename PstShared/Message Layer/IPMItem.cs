using System;
using System.Text;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public class IPMItem
    {
        private uint _nid;
        public String MessageClass { get; protected set; }
        public PropertyContext PC { get; protected set; }

        public IPMItem(PSTFile pst, uint nid)
        {
            _nid = nid;
            PC = new PropertyContext(nid, pst);
            GetMessageClass(pst);
        }

        public IPMItem(PSTFile pst, Tuple<ulong, ulong> nodeBIDs)
        {
            PC = new PropertyContext(BlockBO.GetNodeData(nodeBIDs, pst));
            GetMessageClass(pst);
        }

        private void GetMessageClass(PSTFile pst)
        {
            if (!PC.Properties.ContainsKey(MessageProperty.MessageClass))
                return;
            MessageClass = pst.Header.isUnicode
                ? Encoding.Unicode.GetString(PC.Properties[MessageProperty.MessageClass].Data)
                : Encoding.ASCII.GetString(PC.Properties[MessageProperty.MessageClass].Data);
        }

        protected IPMItem() { }
    }
}
