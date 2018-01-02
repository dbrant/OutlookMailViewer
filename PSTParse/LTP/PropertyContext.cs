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
        {
            var bytes = BlockBO.GetNodeData(nid, pst);
            var HN = new HN(bytes);
            BTH = new BTH(HN);
            Properties = BTH.GetExchangeProperties();
        }

        public PropertyContext(NodeDataDTO data)
        {
            var HN = new HN(data);
            BTH = new BTH(HN);
            Properties = BTH.GetExchangeProperties();
        }
    }
}
