using System;
using System.Collections.Generic;
using System.Text;
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
        public TableContext AttachmentTable { get; private set; }
        public TableContext RecipientTable { get; private set; }

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
        public UInt32 InternetArticleNumber { get; private set; }
        public bool AttributeHidden { get; private set; }
        public bool ReadOnly { get; private set; }
        public DateTime CreationTime { get; private set; }
        public DateTime LastModificationTime { get; private set; }
        public UInt32 CodePage { get; private set; }
        public UInt32 NonUnicodeCodePage { get; private set; }

        public List<string> ContentEx { get; private set; }
        
        private UInt32 MessageFlags;
        private bool unicode;

        public List<Recipient> To = new List<Recipient>();
        public List<Recipient> From = new List<Recipient>();
        public List<Recipient> CC = new List<Recipient>();
        public List<Recipient> BCC = new List<Recipient>();

        public List<Attachment> Attachments = new List<Attachment>();

        public string Headers
        {
            get { return GetProperty(MessageProperty.Headers); }
        }

        public string BodyPlainText
        {
            get { return GetProperty(MessageProperty.BodyPlainText); }
        }

        public string HtmlBody
        {
            get { return GetProperty(MessageProperty.BodyHtml); }
        }

        public string BodyRTF
        {
            get
            {
                foreach (var prop in PC.Properties)
                {
                    if (prop.Value.ID == MessageProperty.BodyCompressedRTF)
                    {
                        return new Util.RtfDecompressor().Decompress(prop.Value.Data);
                    }
                }
                return null;
            }
        }


        public Message(uint NID, PSTFile pst)
            : base(pst, NID)
        {
            unicode = pst.Header.isUnicode;
            Data = BlockBO.GetNodeData(NID, pst);
            this.NID = NID;
            
            ContentEx = new List<string>();

            //MessagePC = new PropertyContext(Data);
            int attachmentPcIndex = 0;

            foreach(var subNode in Data.SubNodeData)
            {
                var temp = new NID(subNode.Key);
                switch(temp.Type)
                {
                    case NDB.NID.NodeType.ATTACHMENT_TABLE:
                        AttachmentTable = new TableContext(subNode.Value);
                        foreach (var row in AttachmentTable.RowMatrix.Rows)
                        {
                            Attachments.Add(new Attachment(pst.Header.isUnicode, row));
                        }
                        break;
                    case NDB.NID.NodeType.ATTACHMENT_PC:
                        var AttachmentPC = new PropertyContext(subNode.Value);
                        if (Attachments.Count > attachmentPcIndex)
                        {
                            Attachments[attachmentPcIndex].AddProperties(unicode, AttachmentPC);
                        }
                        attachmentPcIndex++;
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
                        // TODO: investigate what this is.
                        /*
                        foreach (var nodeData in subNode.Value.NodeData)
                        {
                            ContentEx.Add(pst.Header.isUnicode
                                ? Encoding.Unicode.GetString(subNode.Value.NodeData[0].Data)
                                : Encoding.ASCII.GetString(subNode.Value.NodeData[0].Data));
                        }
                        */
                        break;
                    default:
                        // TODO: investigate what this is.
                        /*
                        foreach (var nodeData in subNode.Value.NodeData)
                        {
                            string foo = pst.Header.isUnicode
                                ? Encoding.Unicode.GetString(subNode.Value.NodeData[0].Data)
                                : Encoding.ASCII.GetString(subNode.Value.NodeData[0].Data);
                            Console.WriteLine(foo);
                        }
                        */
                        break;
                }
            }
            foreach(var prop in PC.Properties)
            {
                if (prop.Value.Data == null || prop.Value.Data.Length == 0)
                    continue;
                
                switch (prop.Key)
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
                    case MessageProperty.NonUnicodeCodePage:
                        NonUnicodeCodePage = BitConverter.ToUInt32(prop.Value.Data, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        public string GetProperty(MessageProperty property)
        {
            foreach (var prop in PC.Properties)
            {
                if (prop.Value.ID == property)
                {
                    return MessagePropertyTypes.PropertyToString(unicode, prop.Value);
                }
            }
            return null;
        }
    }

    public enum MessageProperty
    {
        GuidList = 0x2,
        EntryList = 0x3,
        StringList = 0x4,
        Importance = 0x17,
        MessageClass = 0x1a,
        DeliveryReportRequested = 0x23,
        Priority = 0x26,
        ReadReceiptRequested = 0x29,
        RecipientReassignmentProhibited = 0x2B,
        SensitivityOriginal = 0x2E,
        ReportTime = 0x32,
        Sensitivity = 0x36,
        Subject = 0x37,
        ClientSubmitTime = 0x39,
        OriginalSenderWithScheme = 0x3b,
        ReceivedByEntryID = 0x3F,
        ReceivedByName = 0x40,
        SentRepresentingEntryID = 0x41,
        SentRepresentingName = 0x42,
        ReceivedRepresentingEntryID = 0x43,
        ReceivedRepresentingName = 0x44,
        ReplyRecipientEntries = 0x4F,
        ReplyRecipientNames = 0x50,
        ReceivedBySearchKey = 0x51,
        ReceivedRepresentingSearchKey = 0x52,
        MessageToMe = 0x57,
        MessageCCMe = 0x58,
        MessageRecipientMe = 0x59,
        ResponseRequested = 0x60,
        SentRepresentingAddressType = 0x64,
        SentRepresentingAddress = 0x65,
        ConversationTopic = 0x70,
        ConversationIndex = 0x71,
        OriginalDisplayBcc = 0x72,
        OriginalDisplayCc = 0x73,
        OriginalDisplayTo = 0x74,
        ReceivedByAddressType = 0x75,
        ReceivedByAddress = 0x76,
        ReceivedRepresentingAddressType = 0x77,
        ReceivedRepresentingAddress = 0x78,
        Headers = 0x7d,
        UserEntryID = 0x619,
        NdrReasonCode = 0xC04,
        NdrDiagCode = 0xC05,
        NonReceiptNotificationRequested = 0xC06,
        RecipientType = 0xc15,
        ReplyRequested = 0xc17,
        SenderEntryID = 0xc19,
        SenderName = 0xc1a,
        SupplementaryInfo = 0xc1b,
        SenderSearchKey = 0xc1d,
        SenderAddressType = 0xc1e,
        SenderAddress = 0xc1f,
        DeleteAfterSubmit = 0xe01,
        DisplayBccAddresses = 0xe02,
        DisplayCcAddresses = 0xe03,
        RecipientName = 0xe04,
        MessageDeliveryTime = 0xe06,
        MessageFlags = 0xe07,
        MessageSize = 0xe08,
        SentMailEntryID = 0xe0a,
        RecipientResponsibility = 0xe0f,
        NormalizedSubject = 0xe1d,
        RtfInSync = 0xe1f,
        AttachmentSize = 0xe20,
        InternetArticleNumber = 0xe23,
        NextSentAccount = 0xe29,
        TrustedSender = 0xe79,
        RecordKey = 0xff9,
        RecipientObjType = 0xffe,
        RecipientEntryID = 0xfff,
        BodyPlainText = 0x1000,
        ReportText = 0x1001,
        BodyRtfCrc = 0x1006,
        BodyRtfSyncCount = 0x1007,
        BodyRtfSyncTag = 0x1008,
        BodyCompressedRTF = 0x1009,
        BodyRtfSyncPrefixCount = 0x1010,
        BodyRtfSyncTrailingCount = 0x1011,
        BodyHtml = 0x1013,
        MessageID = 0x1035,
        ReferencesMessageID = 0x1039,
        ReplyToMessageID = 0x1042,
        UnsubscribeAddress = 0x1045,
        ReturnPath = 0x1046,
        UrlCompositeName = 0x10F3,
        AttributeHidden = 0x10F4,
        ReadOnly = 0x10F6,
        DisplayName = 0x3001,
        AddressType = 0x3002,
        AddressName = 0x3003,
        Comment = 0x3004,
        CreationTime = 0x3007,
        LastModificationTime = 0x3008,
        SearchKey = 0x300B,
        ValidFolderMask = 0x35df,
        RootFolder = 0x35e0,
        OutboxFolder = 0x35e2,
        DeletedItemsFolder = 0x35e3,
        SentFolder = 0x35e4,
        UserViewsFolder = 0x35e5,
        CommonViewsFolder = 0x35e6,
        SearchFolder = 0x35e7,
        FolderContentCount = 0x3602,
        FolderUnreadCount = 0x3603,
        FolderHasChildren = 0x360a,
        ContainerClass = 0x3613,
        AssocContentCount = 0x3617,
        AttachmentData = 0x3701,
        AttachmentFileName = 0x3704,
        AttachmentMethod = 0x3705,
        AttachmentLongFileName = 0x3707,
        AttachmentRenderPosition = 0x370b,
        AttachmentMimeType = 0x370e,
        AttachmentMimeSequence = 0x3710,
        AttachmentContentID = 0x3712,
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
        public static string PropertyToString(bool unicode, ExchangeProperty prop, bool enforceMaxLength = false)
        {
            int maxStringBytes = enforceMaxLength ? 2048 : Int32.MaxValue;
            try
            {
                if (prop.Type == ExchangeProperty.PropType.Binary && prop.Data.Length > 0)
                {
                    return Encoding.ASCII.GetString(prop.Data, 0, prop.Data.Length);
                }
                else if (prop.Type == ExchangeProperty.PropType.Boolean && prop.Data.Length > 0)
                {
                    // since it's little-endian, we can just take the value of the first byte,
                    // regardless of the total width of the value.
                    return (prop.Data[0] != 0).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.Currency)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.ErrorCode)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.Floating32 && prop.Data.Length >= 4)
                {
                    return BitConverter.ToSingle(prop.Data, 0).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.Floating64 && prop.Data.Length >= 8)
                {
                    return BitConverter.ToDouble(prop.Data, 0).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.FloatingTime && prop.Data.Length >= 8)
                {
                    return DateTime.FromBinary(BitConverter.ToInt64(prop.Data, 0)).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.Guid && prop.Data.Length >= 16)
                {
                    return (new Guid(prop.Data)).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.Integer16 && prop.Data.Length >= 2)
                {
                    return BitConverter.ToUInt16(prop.Data, 0).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.Integer32 && prop.Data.Length >= 4)
                {
                    return BitConverter.ToUInt32(prop.Data, 0).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.Integer64 && prop.Data.Length >= 8)
                {
                    return BitConverter.ToUInt64(prop.Data, 0).ToString();
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleBinary)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleCurrency)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleFloating32)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleFloating64)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleFloatingTime)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleGuid)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleInteger16)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleInteger32)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleInteger64)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleString && prop.Data.Length > 8)
                {
                    uint numStrings = BitConverter.ToUInt32(prop.Data, 0);
                    // screw it, just render the first string, up until the end of the data.
                    return unicode
                        ? Encoding.Unicode.GetString(prop.Data, 8, Math.Min(maxStringBytes, prop.Data.Length - 8))
                        : Encoding.ASCII.GetString(prop.Data, 8, Math.Min(maxStringBytes, prop.Data.Length - 8));
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleString8)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.MultipleTime)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.Restriction)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.RuleAction)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.ServerId)
                {
                }
                else if (prop.Type == ExchangeProperty.PropType.String)
                {
                    return unicode
                        ? Encoding.Unicode.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length))
                        : Encoding.ASCII.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length));
                }
                else if (prop.Type == ExchangeProperty.PropType.String8)
                {
                    return Encoding.ASCII.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length));
                }
                else if (prop.Type == ExchangeProperty.PropType.Time && prop.Data.Length >= 8)
                {
                    return DateTime.FromFileTimeUtc(BitConverter.ToInt64(prop.Data, 0)).ToString();
                }

                // If we fall through to here, then just try to render it as ascii...
                Encoding.ASCII.GetString(prop.Data, 0, Math.Min(maxStringBytes, prop.Data.Length));

            }
            catch { }
            return "";
        }
    }
}
