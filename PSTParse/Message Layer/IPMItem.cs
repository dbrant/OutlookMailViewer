using System;
using System.Text;
using PSTParse.LTP;

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
            MessageClass = pst.Header.isUnicode
                ? Encoding.Unicode.GetString(PC.Properties[MessageProperty.MessageClass].Data)
                : Encoding.ASCII.GetString(PC.Properties[MessageProperty.MessageClass].Data);
        }

        protected IPMItem()
        {
        }
    }
}
