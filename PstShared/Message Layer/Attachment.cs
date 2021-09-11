using System;
using System.Text;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public enum AttachmentMethod
    {
        NONE = 0x00,
        BY_VALUE= 0x01,
        BY_REFERENCE = 0X02,
        BY_REFERENCE_ONLY = 0X04,
        EMBEDDED_MESSAGE = 0X05,
        STORAGE = 0X06
    }

    public class Attachment
    {
        public AttachmentMethod Method { get; private set; }
        public uint Size { get; private set; }
        public uint RenderingPosition { get; private set; }
        public string FileName { get; private set; }
        public string ShortFileName { get; private set; }
        public string MimeType { get; private set; }
        public uint LTPRowID { get; private set; }
        public uint LTPRowVer { get; private set; }
        public bool InvisibleInHTML { get; private set; }
        public bool InvisibleInRTF { get; private set; }
        public bool RenderedInBody { get; private set; }
        public byte[] Data { get; private set; }
        
        public Attachment(bool unicode, TCRowMatrixData row)
        {
            foreach (var prop in row)
            {
                SetProperty(unicode, prop);
            }
        }

        public void AddProperties(bool unicode, PropertyContext pc)
        {
            foreach (var prop in pc.Properties)
            {
                SetProperty(unicode, prop.Value);
            }
        }

        private void SetProperty(bool unicode, ExchangeProperty prop)
        {
            if (prop.Data == null)
            {
                return;
            }
            switch (prop.ID)
            {
                case MessageProperty.AttachmentSize:
                    Size = BitConverter.ToUInt32(prop.Data, 0);
                    break;
                case MessageProperty.AttachmentData:
                    Data = prop.Data;
                    break;
                case MessageProperty.AttachmentFileName:
                    ShortFileName = unicode ? Encoding.Unicode.GetString(prop.Data) : Encoding.ASCII.GetString(prop.Data);
                    ShortFileName = ShortFileName.Replace("\0", "");
                    if (FileName == null)
                    {
                        FileName = ShortFileName;
                    }
                    break;
                case MessageProperty.AttachmentLongFileName:
                case MessageProperty.DisplayName: // in the case of EMBEDDED_MESSAGE
                    FileName = unicode ? Encoding.Unicode.GetString(prop.Data) : Encoding.ASCII.GetString(prop.Data);
                    FileName = FileName.Replace("\0", "");
                    if (ShortFileName == null)
                    {
                        ShortFileName = FileName;
                    }
                    break;
                case MessageProperty.AttachmentMimeType:
                    MimeType = unicode ? Encoding.Unicode.GetString(prop.Data) : Encoding.ASCII.GetString(prop.Data);
                    break;
                case MessageProperty.AttachmentMethod:
                    Method = (AttachmentMethod)BitConverter.ToUInt32(prop.Data, 0);
                    break;
                case MessageProperty.AttachmentRenderPosition:
                    RenderingPosition = BitConverter.ToUInt32(prop.Data, 0);
                    break;
                case MessageProperty.AttachmentFlags:
                    var flags = BitConverter.ToUInt32(prop.Data, 0);
                    InvisibleInHTML = (flags & 0x1) != 0;
                    InvisibleInRTF = (flags & 0x02) != 0;
                    RenderedInBody = (flags & 0x04) != 0;
                    break;
                case MessageProperty.AttachmentLTPRowID:
                    LTPRowID = BitConverter.ToUInt32(prop.Data, 0);
                    break;
                case MessageProperty.AttachmentLTPRowVer:
                    LTPRowVer = BitConverter.ToUInt32(prop.Data, 0);
                    break;
                default:
                    break;
            }
        }
    }
}
