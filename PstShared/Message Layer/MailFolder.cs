using System;
using System.Collections.Generic;
using System.IO;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class MailFolder
    {
        public const string RecoveredFolderName = "Recovered items";

        // Any of these may be null if the corresponding part of the folder is damaged.
        public PropertyContext PC { get; private set; }
        public TableContext HierarchyTC { get; private set; }
        public TableContext ContentsTC { get; private set; }
        //public TableContext FaiTC { get; private set; }

        public string DisplayName { get; private set; }
        public List<string> Path { get; private set; }

        public List<MailFolder> SubFolders { get; private set; }
        public List<Message> Messages { get; private set; }

        private PSTFile _pst;

        public MailFolder(ulong NID, List<string> path, PSTFile pst)
        {
            _pst = pst;
            var nid = NID;
            pst.VisitedNIDs.Add(nid);

            SubFolders = new List<MailFolder>();
            Messages = new List<Message>();

            var pcNID = ((nid >> 5) << 5) | 0x02;
            try
            {
                PC = new PropertyContext(pcNID, pst);
                DisplayName = pst.GetString(PC.Properties[MessageProperty.DisplayName].Data);
            }
            catch (Exception)
            {
                DisplayName = "Folder " + nid.ToString("X");
            }

            Path = new List<string>(path) { DisplayName };

            var heirachyNID = ((nid >> 5) << 5) | 0x0D;
            try
            {
                HierarchyTC = new TableContext(heirachyNID, pst);
            }
            catch (Exception)
            {
                // the folder's children may still be found later, as orphans.
            }

            var contentsNID = ((nid >> 5) << 5) | 0x0E;
            try
            {
                ContentsTC = new TableContext(contentsNID, pst);
            }
            catch (Exception)
            {
                // the folder's messages may still be found later, as orphans.
            }

            if (PC == null && HierarchyTC == null && ContentsTC == null)
                throw new InvalidDataException("Folder " + nid.ToString("X") + " is unreadable.");

            if (HierarchyTC != null)
            {
                foreach (var row in HierarchyTC.ReverseRowIndex)
                {
                    // guard against cycles in a damaged hierarchy.
                    if (pst.VisitedNIDs.Contains(row.Value))
                        continue;
                    var child = TryCreate(row.Value, Path, pst);
                    if (child != null)
                        SubFolders.Add(child);
                }
            }

            //var faiNID = ((nid >> 5) << 5) | 0x0F;
            //FaiTC = new TableContext(faiNID, pst);

            if (ContentsTC != null)
            {
                foreach (var row in ContentsTC.ReverseRowIndex)
                {
                    if (pst.VisitedNIDs.Contains(row.Value))
                        continue;
                    pst.VisitedNIDs.Add(row.Value);
                    try
                    {
                        Messages.Add(new Message(row.Value, _pst));
                    }
                    catch (Exception)
                    {
                        // skip an unreadable message.
                    }
                }
            }
        }

        // Constructs a synthetic folder that doesn't exist in the PST, e.g. for holding recovered items.
        public MailFolder(string displayName, List<string> path, List<MailFolder> subFolders, List<Message> messages)
        {
            DisplayName = displayName;
            Path = new List<string>(path) { DisplayName };
            SubFolders = subFolders;
            Messages = messages;
        }

        // Returns null if the folder is unreadable.
        public static MailFolder TryCreate(ulong NID, List<string> path, PSTFile pst)
        {
            try
            {
                return new MailFolder(NID, path, pst);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
