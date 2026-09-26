using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse
{
    public class PSTFile : IDisposable
    {
        //public static PSTFile CurPST { get; set; }
        public string Path { get; private set; }
        public MemoryMappedFile PSTMMF { get; private set; }
        public long FileSize { get; private set; }
        public PSTHeader Header { get; private set; }
        public MailStore MailStore { get; private set; }
        public MailFolder TopOfPST { get; private set; }
        // May be null if the named property map could not be read.
        public NamedToPropertyLookup NamedPropertyLookup { get; private set; }

        // Number of distinct blocks that were read from a location other than the one given by the Block B-tree,
        // because the B-tree entry was missing or pointed to data that failed validation.
        public int RecoveredBlockCount => _recoveredBlocks.Count;

        // Number of distinct blocks that failed validation and for which no intact copy could be found.
        // Their data is used as-is, so any content derived from them may be partially corrupt.
        public int DamagedBlockCount => _damagedBlocks.Count;

        private readonly HashSet<ulong> _recoveredBlocks = new HashSet<ulong>();
        private readonly HashSet<ulong> _damagedBlocks = new HashSet<ulong>();

        // Number of folders and messages that could not be reached through the folder hierarchy,
        // and were placed into a synthetic "Recovered items" folder instead.
        public int OrphanedItemCount { get; private set; }

        private Dictionary<ulong, BBTENTRY> _scannedBlocks;
        internal HashSet<ulong> VisitedNIDs { get; } = new HashSet<ulong>();

        public PSTFile(string path)
        {
            Path = path;
            FileSize = new FileInfo(path).Length;
            PSTMMF = MemoryMappedFile.CreateFromFile(path, FileMode.Open);
            try
            {
                Header = new PSTHeader(this);

                /*var messageStoreData = BlockBO.GetNodeData(SpecialNIDs.NID_MESSAGE_STORE);
                var temp = BlockBO.GetNodeData(SpecialNIDs.NID_ROOT_FOLDER);*/
                MailStore = new MailStore(this);

                TopOfPST = new MailFolder(MailStore.RootFolderNID, new List<string>(), this);
                try
                {
                    NamedPropertyLookup = new NamedToPropertyLookup(this);
                }
                catch (Exception)
                {
                    // The named property map is not needed for reading ordinary messages.
                }
                RecoverOrphanedItems();
                //var temp = new TableContext(rootEntryID.NID);
                //PasswordReset.ResetPassword();
            }
            catch (Exception)
            {
                // don't hold the MMF open if something failed here.
                PSTMMF.Dispose();
                throw;
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
            var entry = Header.BlockBT.Root.GetBIDBBTEntry(item1);
            if (entry != null)
                return entry;
            // The BBT doesn't know about this block, possibly because one of its pages is damaged.
            entry = GetScannedBlockEntry(item1);
            if (entry != null)
                _recoveredBlocks.Add(entry.Key);
            return entry;
        }

        // Finds a block by scanning the file for a valid trailer with the given BID, bypassing the Block B-tree.
        // The scan is performed once, and only if it's needed.
        internal void ReportBlockRecovered(ulong bid) => _recoveredBlocks.Add(bid);
        internal void ReportBlockDamaged(ulong bid) => _damagedBlocks.Add(bid);

        public BBTENTRY GetScannedBlockEntry(ulong bid)
        {
            if (_scannedBlocks == null)
            {
                var maxBid = Header.NextBID > 0 ? Header.NextBID * 2 + 0x100000 : ulong.MaxValue;
                _scannedBlocks = BlockScanner.Scan(Path, Header.isUnicode, maxBid);
            }
            return _scannedBlocks.TryGetValue(bid & 0xfffffffffffffffe, out var entry) ? entry : null;
        }

        // Determines whether a node belongs under the top-level folder of the store (as opposed to
        // internal folders such as search roots and views), by following its chain of parents.
        // A node whose chain of parents is broken is also considered to be within the top folder,
        // since it's likely to be a user item that has become detached.
        private bool IsWithinTopOfPST(ulong nid, Dictionary<ulong, ulong> parents)
        {
            var topNid = MailStore.RootFolderNID;
            var cur = nid;
            for (int i = 0; i < 64; i++)
            {
                if (cur == topNid)
                    return true;
                if (cur == SpecialNIDs.NID_ROOT_FOLDER)
                    return false;
                if (!parents.TryGetValue(cur, out var parent) || parent == cur || parent == 0)
                    return true;
                cur = parent;
            }
            return true;
        }

        // Finds folders and messages that exist in the Node B-tree but were not reachable from the root folder
        // (e.g. because a parent folder's hierarchy or contents table is damaged), and attaches them to a
        // synthetic folder under the root.
        private void RecoverOrphanedItems()
        {
            var allNodes = new List<NBTENTRY>();
            Header.NodeBT.Root.GetAllNBTEntries(allNodes);

            var parents = new Dictionary<ulong, ulong>();
            foreach (var node in allNodes)
                parents[node.NID] = node.NID_Parent;

            var orphanFolders = new Dictionary<ulong, NBTENTRY>();
            var orphanMessages = new List<NBTENTRY>();
            foreach (var node in allNodes)
            {
                if (VisitedNIDs.Contains(node.NID) || !IsWithinTopOfPST(node.NID, parents))
                    continue;
                if (node.NID_TYPE == (ulong)NID.NodeType.NORMAL_FOLDER)
                    orphanFolders[node.NID] = node;
                else if (node.NID_TYPE == (ulong)NID.NodeType.NORMAL_MESSAGE)
                    orphanMessages.Add(node);
            }
            if (orphanFolders.Count == 0 && orphanMessages.Count == 0)
                return;

            var recoveredFolders = new List<MailFolder>();
            // Start with folders whose parent is not itself an orphan, so that nested orphans end up
            // under their own parent rather than at the top level.
            foreach (var folder in orphanFolders.Values.OrderBy(f => orphanFolders.ContainsKey(f.NID_Parent) && f.NID_Parent != f.NID ? 1 : 0))
            {
                if (VisitedNIDs.Contains(folder.NID))
                    continue;
                var recovered = MailFolder.TryCreate(folder.NID, new List<string> { TopOfPST.DisplayName, MailFolder.RecoveredFolderName }, this);
                if (recovered != null)
                    recoveredFolders.Add(recovered);
            }

            var recoveredMessages = new List<Message>();
            foreach (var node in orphanMessages)
            {
                if (VisitedNIDs.Contains(node.NID))
                    continue;
                VisitedNIDs.Add(node.NID);
                try
                {
                    recoveredMessages.Add(new Message((uint)node.NID, this));
                }
                catch (Exception)
                {
                    // unreadable message; skip it.
                }
            }

            OrphanedItemCount = recoveredFolders.Count + recoveredMessages.Count;
            if (OrphanedItemCount > 0)
                TopOfPST.SubFolders.Add(new MailFolder(MailFolder.RecoveredFolderName, TopOfPST.Path, recoveredFolders, recoveredMessages));
        }

        public string GetString(byte[] data)
        {
            return GetString(Header.isUnicode, data);
        }

        public static string GetString(bool unicode, byte[] data)
        {
            return unicode ? Encoding.Unicode.GetString(data) : Encoding.Latin1.GetString(data);
        }
    }
}
