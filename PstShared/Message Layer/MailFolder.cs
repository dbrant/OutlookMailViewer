﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class MailFolder
    {
        public PropertyContext PC { get; private set; }
        public TableContext HierarchyTC { get; private set; }
        public TableContext ContentsTC { get; private set; }
        public TableContext FaiTC { get; private set; }

        public string DisplayName { get; private set; }
        public List<string> Path { get; private set; }

        public List<MailFolder> SubFolders { get; private set; }
        public List<Message> Messages { get; private set; }
        public List<IPMItem> OtherItems { get; private set; }

        private PSTFile _pst;

        public MailFolder(ulong NID, List<string> path, PSTFile pst)
        {
            _pst = pst;

            Path = path;
            var nid = NID;
            var pcNID = ((nid >> 5) << 5) | 0x02;
            PC = new PropertyContext(pcNID, pst);
            DisplayName = pst.GetString(PC.Properties[MessageProperty.DisplayName].Data);

            Path = new List<string>(path);
            Path.Add(DisplayName);

            var heirachyNID = ((nid >> 5) << 5) | 0x0D;
            var contentsNID = ((nid >> 5) << 5) | 0x0E;
            var faiNID = ((nid >> 5) << 5) | 0x0F;

            HierarchyTC = new TableContext(heirachyNID, pst);

            SubFolders = new List<MailFolder>();
            foreach(var row in HierarchyTC.ReverseRowIndex)
            {
                SubFolders.Add(new MailFolder(row.Value, Path, pst));
                //var temp = row.Key;
                //var temp2 = row.Value;
                //SubFolderEntryIDs.Add(row.);
            }
            
            ContentsTC = new TableContext(contentsNID, pst);
            
            FaiTC = new TableContext(faiNID, pst);

            Messages = new List<Message>();
            OtherItems = new List<IPMItem>();
            foreach (var row in ContentsTC.ReverseRowIndex)
            {
                //var item = new IPMItem(_pst, row.Value);
                //if (item.MessageClass.StartsWith("IPM.Note"))
                //{
                    Messages.Add(new Message(row.Value, _pst));
                //}
                //else
                //{
                //    OtherItems.Add(item);
                //}
            }
            
        }
    }
}
