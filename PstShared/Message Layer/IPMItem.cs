using System;
using System.Text;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public class IPMItem
    {
        protected bool unicode;

        public uint NID { get; private set; }
        public NodeDataDTO Data { get; private set; }
        public string MessageClass { get; protected set; }
        public PropertyContext PC { get; protected set; }

        public IPMItem(PSTFile pst, uint nid, NodeDataDTO parentData = null)
        {
            NID = nid;
            Data = parentData == null ? BlockBO.GetNodeData(nid, pst) : FindSubnodeWithKey(parentData, nid, 0);

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
            MessageClass = pst.GetString(PC.Properties[MessageProperty.MessageClass].Data);
        }

        public static NodeDataDTO FindSubnodeWithKey(NodeDataDTO parent, uint NID, int level)
        {
            if (parent == null || parent.SubNodeData == null || level > 32)
                return null;
            foreach (var subNode in parent.SubNodeData)
            {
                if (subNode.Key == NID)
                    return subNode.Value;

                if (subNode.Value.SubNodeData != null && subNode.Value.SubNodeData.Count > 0)
                {
                    var ret = FindSubnodeWithKey(subNode.Value, NID, level + 1);
                    if (ret != null)
                        return ret;
                }
            }
            return null;
        }

        protected IPMItem() { }
    }
}
