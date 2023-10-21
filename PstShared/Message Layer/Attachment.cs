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

        // The "rendering" is usually a Windows Metafile graphic that represents an icon of
        // the attachment as it appears in the message, e.g. a literal envelope icon.
        public byte[] Rendering { get; private set; }

        // If the attachment is by-reference, then this will be the NID of the subnode that
        // contains the actual data of the attachment.
        public uint RefNID { get; private set; }

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
                    if (prop.Type == ExchangeProperty.PropType.ObjectType)
                    {
                        RefNID = BitConverter.ToUInt32(prop.Data, 0);
                    }
                    else
                    {
                        Data = prop.Data;
                    }
                    break;
                case MessageProperty.AttachmentRendering:
                    Rendering = prop.Data;
                    break;
                case MessageProperty.AttachmentFileName:
                    ShortFileName = PSTFile.GetString(unicode, prop.Data);
                    ShortFileName = ShortFileName.Replace("\0", "");
                    FileName ??= ShortFileName;
                    break;
                case MessageProperty.AttachmentLongFileName:
                case MessageProperty.DisplayName: // in the case of EMBEDDED_MESSAGE
                    FileName = PSTFile.GetString(unicode, prop.Data);
                    FileName = FileName.Replace("\0", "");
                    ShortFileName ??= FileName;
                    break;
                case MessageProperty.AttachmentMimeType:
                    MimeType = PSTFile.GetString(unicode, prop.Data);
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
