using System;
using System.Collections.Generic;
using System.Text;
using MiscParseUtilities;
using PSTParse.LTP;
using PSTParse.NDB;

namespace PSTParse.Message_Layer
{
    public enum Importance
    {
        LOW = 0x00,
        NORMAL = 0x01,
        HIGH = 0x02
    }

    public enum Sensitivity
    {
        Normal = 0x00,
        Personal = 0x01,
        Private = 0x02,
        Confidential = 0x03
    }

    public class Message: IPMItem
    {
        public uint NID { get; private set; }
        public NodeDataDTO Data { get; private set; }
        public PropertyContext MessagePC { get; private set; }
        public TableContext AttachmentTable { get; private set; }
        public TableContext RecipientTable { get; private set; }
        public PropertyContext AttachmentPC { get; private set; }

        public String Subject { get; private set; }
        public String SubjectPrefix { get; private set; }
        public Importance Imporance { get; private set; }
        public Sensitivity Sensitivity { get; private set; }
        public DateTime LastSaved { get; private set; }
        
        public DateTime ClientSubmitTime { get; private set; }
        public string SentRepresentingName { get; private set; }
        public string ConversationTopic { get; private set; }
        public string SenderName { get; private set; }
        public DateTime MessageDeliveryTime { get; private set; }
        public Boolean Read { get; private set; }
        public Boolean Unsent { get; private set; }
        public Boolean Unmodified { get; private set; }
        public Boolean HasAttachments { get; private set; }
        public Boolean FromMe { get; private set; }
        public Boolean IsFAI { get; private set; }
        public Boolean NotifyReadRequested { get; private set; }
        public Boolean NotifyUnreadRequested { get; private set; }
        public Boolean EverRead { get; private set; }
        public UInt32 MessageSize { get; private set; }
        public string BodyPlainText { get; private set; }
        public UInt32 InternetArticleNumber { get; private set; }
        public byte[] BodyCompressedRTF { get; private set; }
        public string InternetMessageID { get; private set; }
        public string UrlCompositeName { get; private set; }
        public bool AttributeHidden { get; private set; }
        public bool ReadOnly { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastModificationTime { get; private set; }
        public UInt32 CodePage { get; private set; }
        public String CreatorName { get; private set; }
        public UInt32 NonUnicodeCodePage { get; private set; }
        public string UnsubscribeAddress { get; private set; }

        public string Headers { get; private set; }
        public string FromHeaderField { get; private set; }
        public string HtmlBody { get; private set; }

        public List<string> ContentEx { get; private set; }

        public Dictionary<int, string> AllProperties { get; private set; }

        private UInt32 MessageFlags;
        private IPMItem _IPMItem;

        public List<Recipient> To = new List<Recipient>();
        public List<Recipient> From = new List<Recipient>();
        public List<Recipient> CC = new List<Recipient>();
        public List<Recipient> BCC = new List<Recipient>();

        public List<Attachment> Attachments = new List<Attachment>(); 

        public Message(uint NID, IPMItem item, PSTFile pst)
        {
            _IPMItem = item;
            Data = BlockBO.GetNodeData(NID, pst);
            this.NID = NID;

            AllProperties = new Dictionary<int, string>();
            ContentEx = new List<string>();

            //MessagePC = new PropertyContext(Data);
            foreach(var subNode in Data.SubNodeData)
            {
                var temp = new NID(subNode.Key);
                switch(temp.Type)
                {
                    case NDB.NID.NodeType.ATTACHMENT_TABLE:
                        AttachmentTable = new TableContext(subNode.Value);
                        break;
                    case NDB.NID.NodeType.ATTACHMENT_PC:
                        AttachmentPC = new PropertyContext(subNode.Value);
                        Attachments = new List<Attachment>();
                        foreach(var row in AttachmentTable.RowMatrix.Rows)
                        {
                            Attachments.Add(new Attachment(pst.Header.isUnicode, row));
                        }
                        break;
                    case NDB.NID.NodeType.RECIPIENT_TABLE:
                        RecipientTable = new TableContext(subNode.Value);
                        
                        foreach(var row in RecipientTable.RowMatrix.Rows)
                        {
                            var recipient = new Recipient(pst.Header.isUnicode, row);
                            switch(recipient.Type)
                            {
                                case Recipient.RecipientType.TO:
                                    To.Add(recipient);
                                    break;
                                case Recipient.RecipientType.FROM:
                                    From.Add(recipient);
                                    break;
                                case Recipient.RecipientType.CC:
                                    CC.Add(recipient);
                                    break;
                                case Recipient.RecipientType.BCC:
                                    BCC.Add(recipient);
                                    break;
                            }
                        }
                        break;
                    case NDB.NID.NodeType.CONTENT_EX:
                        foreach (var nodeData in subNode.Value.NodeData)
                        {
                            ContentEx.Add(pst.Header.isUnicode
                                ? Encoding.Unicode.GetString(subNode.Value.NodeData[0].Data)
                                : Encoding.ASCII.GetString(subNode.Value.NodeData[0].Data));
                        }
                        break;
                    default:
                        foreach (var nodeData in subNode.Value.NodeData)
                        {
                            string foo = pst.Header.isUnicode
                                ? Encoding.Unicode.GetString(subNode.Value.NodeData[0].Data)
                                : Encoding.ASCII.GetString(subNode.Value.NodeData[0].Data);
                            Console.WriteLine(foo);
                        }
                        break;
                }
            }
            foreach(var prop in _IPMItem.PC.Properties)
            {
                if (prop.Value.Data == null || prop.Value.Data.Length == 0)
                    continue;

                AllProperties.Add(prop.Key, pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data));

                switch (prop.Key)
                {
                    case 0x17:
                        Imporance = (Importance) BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case 0x36:
                        Sensitivity = (Sensitivity) BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case 0x37:
                        Subject = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        if (Subject.Length > 0)
                        {
                            var chars = Subject.ToCharArray();
                            if (chars[0] == 0x1)
                            {
                                /*
                                // for skipping past "Re:", "Fwd:", etc.
                                var length = (int)chars[1];
                                int i = 0;
                                if (length > 1)
                                    i++;
                                SubjectPrefix = Subject.Substring(2, length-1);
                                Subject = Subject.Substring(2 + length-1);
                                */
                                Subject = Subject.Substring(2);
                            }
                        }
                        break;
                    case 0x39:
                        ClientSubmitTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x42:
                        SentRepresentingName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x70:
                        ConversationTopic = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x1a:
                        MessageClass = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0xc1a:
                        SenderName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0xe06:
                        MessageDeliveryTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0xe07:
                        MessageFlags = BitConverter.ToUInt32(prop.Value.Data, 0);

                        Read = (MessageFlags & 0x1) != 0;
                        Unsent = (MessageFlags & 0x8) != 0;
                        Unmodified = (MessageFlags & 0x2) != 0;
                        HasAttachments = (MessageFlags & 0x10) != 0;
                        FromMe = (MessageFlags & 0x20) != 0;
                        IsFAI = (MessageFlags & 0x40) != 0;
                        NotifyReadRequested = (MessageFlags & 0x100) != 0;
                        NotifyUnreadRequested = (MessageFlags & 0x200) != 0;
                        EverRead = (MessageFlags & 0x400) != 0;
                        break;
                    case 0xe08:
                        MessageSize = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0xe23:
                        InternetArticleNumber = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0x1000:
                        BodyPlainText = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x1009:
                        BodyCompressedRTF = prop.Value.Data.RangeSubset(4, prop.Value.Data.Length - 4);
                        break;
                    case 0x1035:
                        InternetMessageID = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x10F3:
                        UrlCompositeName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x10F4:
                        AttributeHidden = prop.Value.Data[0] == 0x01;
                        break;
                    case 0x10F6:
                        ReadOnly = prop.Value.Data[0] == 0x01;
                        break;
                    case 0x3007:
                        CreationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x3008:
                        LastModificationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case 0x3fDE:
                        CodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0x3ff8:
                        CreatorName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x3ffd:
                        NonUnicodeCodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case 0x7D:
                        Headers = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x1013:
                        HtmlBody = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case 0x300B:
                        //seach key
                    case 0x3ff1:
                        //localeID
                    case 0xe27:
                        //unknown
                    case 0xe29:
                        //nextSentAccount, ignore this, string
                    case 0xe62:
                        //unknown
                    case 0xe79:
                        //trusted sender
                    case 0x3ff9:
                        //creator entryid
                    case 0x3ffa:
                        //last modifier name
                    case 0x3ffb:
                        //last modifier entryid
                    case 0x4019:
                        //unknown
                    case 0x401a:
                        //sentrepresentingflags
                    case 0x619:
                        //userentryid
                    case 0x10F5:
                        //unknown
                    default:
                        break;
                }
            }

            // Parse the headers and pull the "From" address from there.
            FromHeaderField = "";
            if (Headers != null)
            {
                string[] headerArray = Headers.Split('\r', '\n');
                foreach (var header in headerArray)
                {
                    if (header == null || header.Length == 0)
                    {
                        continue;
                    }
                    if (header.StartsWith("From:") && header.Length > 7)
                    {
                        FromHeaderField = header.Substring(6).Trim();
                    }
                }
            }

        }
    }
}
