using System.Collections;
using System.Collections.Generic;
using System.Text;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class MailFolder : IEnumerable<IPMItem>
    {
        public PropertyContext PC { get; private set; }
        public TableContext HeirachyTC { get; private set; }
        public TableContext ContentsTC { get; private set; }
        public TableContext FaiTC { get; private set; }

        public string DisplayName { get; private set; }
        public List<string> Path { get; private set; }

        public List<MailFolder> SubFolders { get; private set; }

        private PSTFile _pst;

        public MailFolder(ulong NID, List<string> path, PSTFile pst)
        {
            _pst = pst;

            Path = path;
            var nid = NID;
            var pcNID = ((nid >> 5) << 5) | 0x02;
            PC = new PropertyContext(pcNID, pst);
            DisplayName = pst.Header.isUnicode
                ? Encoding.Unicode.GetString(PC.Properties[0x3001].Data)
                : Encoding.ASCII.GetString(PC.Properties[0x3001].Data);

            Path = new List<string>(path);
            Path.Add(DisplayName);

            var heirachyNID = ((nid >> 5) << 5) | 0x0D;
            var contentsNID = ((nid >> 5) << 5) | 0x0E;
            var faiNID = ((nid >> 5) << 5) | 0x0F;

            HeirachyTC = new TableContext(heirachyNID, pst);

            SubFolders = new List<MailFolder>();
            foreach(var row in HeirachyTC.ReverseRowIndex)
            {
                SubFolders.Add(new MailFolder(row.Value, Path, pst));
                //var temp = row.Key;
                //var temp2 = row.Value;
                //SubFolderEntryIDs.Add(row.);
            }
            
            ContentsTC = new TableContext(contentsNID, pst);

            
            FaiTC = new TableContext(faiNID, pst);
        }

        public IEnumerator<IPMItem> GetEnumerator()
        {
            foreach(var row in ContentsTC.ReverseRowIndex)
            {
                var curItem = new IPMItem(_pst, row.Value);
                //if (curItem.MessageClass.StartsWith("IPM.Note"))
                    yield return new Message(row.Value, curItem, _pst);
                /*else
                    yield return curItem;*/
                //yield return new Message(row.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
