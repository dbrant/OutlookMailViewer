using System.Collections.Generic;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class PropertyContext
    {
        public BTH BTH { get; private set; }
        public Dictionary<MessageProperty, ExchangeProperty> Properties { get; private set; }

        public PropertyContext(ulong nid, PSTFile pst)
            : this(BlockBO.GetNodeData(nid, pst))
        { }

        public PropertyContext(NodeDataDTO data)
        {
            var HN = new HN(data);
            BTH = new BTH(HN);
            Properties = BTH.GetExchangeProperties();
        }
    }
}
