using System;
using PSTParse.LTP;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse
{
    public class MailStore
    {
        // May be null if the message store is damaged.
        public EntryID RootFolder { get; private set; }
        public ulong RootFolderNID { get; private set; }
        private PropertyContext _pc;

        public MailStore(PSTFile pst)
        {
            RootFolderNID = SpecialNIDs.NID_ROOT_FOLDER;
            try
            {
                _pc = new PropertyContext(SpecialNIDs.NID_MESSAGE_STORE, pst);
                RootFolder = new EntryID(_pc.Properties[MessageProperty.RootFolder].Data);
                RootFolderNID = RootFolder.NID;
            }
            catch (Exception)
            {
                // fall back to the well-known NID of the root folder.
            }
        }
    }
}
