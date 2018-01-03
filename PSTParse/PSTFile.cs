using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse
{
    public class PSTFile : IDisposable
    {
        //public static PSTFile CurPST { get; set; }
        public string Path { get; private set; }
        public MemoryMappedFile PSTMMF { get; private set; }
        public PSTHeader Header { get; private set; }
        public MailStore MailStore { get; private set; }
        public MailFolder TopOfPST { get; private set; }
        public NamedToPropertyLookup NamedPropertyLookup { get; private set; }

        public PSTFile(string path)
        {
            Path = path;
            PSTMMF = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
            try
            {
                Header = new PSTHeader(this);

                /*var messageStoreData = BlockBO.GetNodeData(SpecialNIDs.NID_MESSAGE_STORE);
                var temp = BlockBO.GetNodeData(SpecialNIDs.NID_ROOT_FOLDER);*/
                MailStore = new MailStore(this);

                TopOfPST = new MailFolder(MailStore.RootFolder.NID, new List<string>(), this);
                NamedPropertyLookup = new NamedToPropertyLookup(this);
                //var temp = new TableContext(rootEntryID.NID);
                //PasswordReset.ResetPassword();
            }
            catch (Exception ex)
            {
                // don't hold the MMF open if something failed here.
                PSTMMF.Dispose();
                throw ex;
            }
        }

        public void CloseMMF()
        {
            PSTMMF.Dispose();
        }

        public void OpenMMF()
        {
            PSTMMF = MemoryMappedFile.CreateFromFile(Path, FileMode.Open);
        }

        public Tuple<ulong,ulong> GetNodeBIDs(ulong NID)
        {
            return Header.NodeBT.Root.GetNIDBID(NID);
        }

        public List<Tuple<ulong, ulong>> GetAllNodeBIDs()
        {
            var list = new List<Tuple<ulong, ulong>>();
            Header.NodeBT.Root.GetAllNIDBIDs(list);
            return list;
        }

        public void Dispose()
        {
            CloseMMF();
        }

        public BBTENTRY GetBlockBBTEntry(ulong item1)
        {
            return Header.BlockBT.Root.GetBIDBBTEntry(item1);
        }
    }
}
