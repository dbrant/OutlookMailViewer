using PSTParse.LTP;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse
{
    public class MailStore
    {
        public EntryID RootFolder { get; private set; }
        private PropertyContext _pc;

        public MailStore(PSTFile pst)
        {
            _pc = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE, pst);
            RootFolder = new EntryID(_pc.BTH.GetExchangeProperties()[MessageProperty.RootFolder].Data);
        }
    }
}
