using System;
using System.Text;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public class IPMItem
    {
        protected uint _nid;
        protected bool unicode;

        public NodeDataDTO Data { get; private set; }
        public string MessageClass { get; protected set; }
        public PropertyContext PC { get; protected set; }

        public IPMItem(PSTFile pst, uint nid, NodeDataDTO parentData = null)
        {
            _nid = nid;
            Data = parentData == null ? BlockBO.GetNodeData(nid, pst) : FindSubnodeWithKey(parentData, nid);

            PC = parentData == null ? new PropertyContext(nid, pst) : new PropertyContext(Data);
            GetMessageClass(pst);
        }

        public IPMItem(PSTFile pst, Tuple<ulong, ulong> nodeBIDs)
        {
            PC = new PropertyContext(BlockBO.GetNodeData(nodeBIDs, pst));
            GetMessageClass(pst);
        }

        private void GetMessageClass(PSTFile pst)
        {
            unicode = pst.Header.isUnicode;
            if (!PC.Properties.ContainsKey(MessageProperty.MessageClass))
                return;
            MessageClass = unicode
                ? Encoding.Unicode.GetString(PC.Properties[MessageProperty.MessageClass].Data)
                : Encoding.ASCII.GetString(PC.Properties[MessageProperty.MessageClass].Data);
        }

        public static NodeDataDTO FindSubnodeWithKey(NodeDataDTO parent, uint NID)
        {
            if (parent == null || parent.SubNodeData == null)
                return null;
            foreach (var subNode in parent.SubNodeData)
            {
                if (subNode.Key == NID)
                    return subNode.Value;

                if (subNode.Value.SubNodeData != null && subNode.Value.SubNodeData.Count > 0)
                {
                    var ret = FindSubnodeWithKey(subNode.Value, NID);
                    if (ret != null)
                        return ret;
                }
            }
            return null;
        }

        protected IPMItem() { }
    }
}
