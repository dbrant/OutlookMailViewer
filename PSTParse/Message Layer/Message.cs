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

        public List<ExchangeProperty> AllProperties { get; private set; }

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

            AllProperties = new List<ExchangeProperty>();
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
                
                MessageProperty property = (MessageProperty)prop.Key;
                AllProperties.Add(prop.Value);

                switch (property)
                {
                    case MessageProperty.Importance:
                        Imporance = (Importance) BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case MessageProperty.Sensitivity:
                        Sensitivity = (Sensitivity) BitConverter.ToInt16(prop.Value.Data, 0);
                        break;
                    case MessageProperty.Subject:
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
                    case MessageProperty.ClientSubmitTime:
                        ClientSubmitTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.SentRepresentingName:
                        SentRepresentingName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.ConversationTopic:
                        ConversationTopic = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.MessageClass:
                        MessageClass = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.SenderName:
                        SenderName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.MessageDeliveryTime:
                        MessageDeliveryTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.MessageFlags:
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
                    case MessageProperty.MessageSize:
                        MessageSize = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case MessageProperty.InternetArticleNumber:
                        InternetArticleNumber = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case MessageProperty.BodyPlainText:
                        BodyPlainText = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.BodyCompressedRTF:
                        BodyCompressedRTF = prop.Value.Data.RangeSubset(4, prop.Value.Data.Length - 4);
                        break;
                    case MessageProperty.MessageID:
                        InternetMessageID = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.UrlCompositeName:
                        UrlCompositeName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.AttributeHidden:
                        AttributeHidden = prop.Value.Data[0] == 0x01;
                        break;
                    case MessageProperty.ReadOnly:
                        ReadOnly = prop.Value.Data[0] == 0x01;
                        break;
                    case MessageProperty.CreationTime:
                        CreationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.LastModificationTime:
                        LastModificationTime = DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Value.Data, 0));
                        break;
                    case MessageProperty.CodePage:
                        CodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case MessageProperty.CreatorName:
                        CreatorName = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.NonUnicodeCodePage:
                        NonUnicodeCodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    case MessageProperty.Headers:
                        Headers = pst.Header.isUnicode
                            ? Encoding.Unicode.GetString(prop.Value.Data)
                            : Encoding.ASCII.GetString(prop.Value.Data);
                        break;
                    case MessageProperty.BodyHtml:
                        // HACK?: the HTML body property always seems to be ASCII, even if the file is unicode.
                        HtmlBody = Encoding.ASCII.GetString(prop.Value.Data);
                        break;
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

    public enum MessageProperty
    {
        Importance = 0x17,
        MessageClass = 0x1a,
        Sensitivity = 0x36,
        Subject = 0x37,
        ClientSubmitTime = 0x39,
        OriginalSenderWithScheme = 0x3b,
        RecipientName2 = 0x40,
        SentRepresentingName = 0x42,
        RecipientName3 = 0x44,
        RecipientWithScheme = 0x51,
        RecipientWithScheme2 = 0x52,
        Scheme1 = 0x64,
        ReturnPath = 0x65,
        ConversationTopic = 0x70,
        Unknown1 = 0x71,
        Scheme2 = 0x75,
        RecipientName4 = 0x76,
        Scheme3 = 0x77,
        RecipientName5 = 0x78,
        Headers = 0x7d,
        UserEntryID = 0x619,

        RecipientType = 0xc15,

        SenderName = 0xc1a,
        SenderNameWithScheme = 0xc1d,
        Scheme4 = 0xc1e,
        SenderName2 = 0xc1f,
        RecipientName = 0xe04,
        MessageDeliveryTime = 0xe06,
        MessageFlags = 0xe07,
        MessageSize = 0xe08,
        RecipientResponsibility = 0xe0f,
        AttachmentSize = 0xe20,
        InternetArticleNumber = 0xe23,
        NextSentAccount = 0xe29,
        TrustedSender = 0xe79,
        RecipientTag = 0xff9,
        RecipientObjType = 0xffe,
        RecipientEntryID = 0xfff,
        BodyPlainText = 0x1000,
        BodyCompressedRTF = 0x1009,
        BodyHtml = 0x1013,
        MessageID = 0x1035,
        ReferencesMessageID = 0x1039,
        ReplyToMessageID = 0x1042,
        UnsubscribeAddress = 0x1045,
        SenderName3 = 0x1046,
        UrlCompositeName = 0x10F3,
        AttributeHidden = 0x10F4,
        ReadOnly = 0x10F6,
        RecipientDisplayName = 0x3001,
        RecipientAddressType = 0x3002,
        RecipientAddress = 0x3003,
        CreationTime = 0x3007,
        LastModificationTime = 0x3008,
        SearchKey = 0x300B,
        AttachmentFileName = 0x3704,
        AttachmentMethod = 0x3705,
        AttachmentRenderPosition = 0x370b,
        MessageID2 = 0x3712,
        AttachmentFlags = 0x3714,
        CodePage = 0x3fDE,
        CreatorName = 0x3ff8,
        NonUnicodeCodePage = 0x3ffd,
        LocaleID = 0x3ff1,
        CreatorEntryID = 0x3ff9,
        LastModifierName = 0x3ffa,
        LastModifierEntryID = 0x3ffb,
        SentRepresentingFlags = 0x401a,
        BodyPlainText2 = 0x6619,
        AttachmentLTPRowID = 0x67F2,
        AttachmentLTPRowVer = 0x67F3,
        BodyPlainText3 = 0x8008,
        ContentClass = 0x8009,
        PopAccountName = 0x800d,
        PopUri = 0x8011,
        ContentType2 = 0x8013,
        TransferEncoding2 = 0x8014,
        BodyPlainText4 = 0x8015,
        PopUri2 = 0x804c,
        PopServerName = 0x8070,
        ContentType = 0x8076,
        TransferEncoding = 0x807b,
        BodyPlainText5 = 0x807e,
        MailSoftwareName = 0x8088,
        PopAccountName2 = 0x808a,
        MailSoftwareEngine = 0x808b,
    }

    public class MessagePropertyTypes
    {
        public static List<MessageProperty> DateTimeProps = new List<MessageProperty> {
            MessageProperty.ClientSubmitTime,
            MessageProperty.MessageDeliveryTime,
            MessageProperty.CreationTime,
            MessageProperty.LastModificationTime,
        };

        public static List<MessageProperty> AsciiOnlyProps = new List<MessageProperty> {
            MessageProperty.BodyHtml,
            MessageProperty.SearchKey,
            MessageProperty.OriginalSenderWithScheme,
            MessageProperty.RecipientWithScheme,
            MessageProperty.SenderNameWithScheme,
            MessageProperty.BodyPlainText4,
            MessageProperty.BodyPlainText5,
            MessageProperty.Unknown1,
        };

        public static List<MessageProperty> NumericProps = new List<MessageProperty> {
            MessageProperty.Importance,
            MessageProperty.Sensitivity,
            MessageProperty.MessageFlags,
            MessageProperty.MessageSize,
            MessageProperty.CodePage,
            MessageProperty.NonUnicodeCodePage,
            MessageProperty.UserEntryID,
            MessageProperty.AttributeHidden,
            MessageProperty.ReadOnly,
            MessageProperty.LocaleID,
            MessageProperty.CreatorEntryID,
            MessageProperty.LastModifierEntryID,
            MessageProperty.SentRepresentingFlags,
        };

        public static string PropertyToString(bool unicode, MessageProperty prop, byte[] data)
        {
            int maxStringBytes = 2048;
            try
            {
                if (DateTimeProps.Contains(prop))
                {
                    return DateTime.FromFileTimeUtc(BitConverter.ToInt64(data, 0)).ToString();
                }
                else if (NumericProps.Contains(prop))
                {
                    if (data.Length == 1)
                    {
                        return ((int)data[0]).ToString();
                    }
                    else if (data.Length == 2)
                    {
                        return BitConverter.ToUInt16(data, 0).ToString();
                    }
                    else if (data.Length == 4)
                    {
                        return BitConverter.ToUInt32(data, 0).ToString();
                    }
                    else if (data.Length == 8)
                    {
                        return BitConverter.ToUInt64(data, 0).ToString();
                    }
                    else
                    {
                        return Encoding.ASCII.GetString(data, 0, Math.Min(maxStringBytes, data.Length));
                    }
                }
                else if (AsciiOnlyProps.Contains(prop))
                {
                    return Encoding.ASCII.GetString(data, 0, Math.Min(maxStringBytes, data.Length));
                }
                //return Encoding.ASCII.GetString(data);
                return unicode
                    ? Encoding.Unicode.GetString(data, 0, Math.Min(maxStringBytes, data.Length))
                    : Encoding.ASCII.GetString(data, 0, Math.Min(maxStringBytes, data.Length));
            }
            catch { }
            return "";
        }
    }
}
