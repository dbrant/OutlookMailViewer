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
        EMBEDDEED_MESSAGE = 0X05,
        STORAGE = 0X06

    }

    public class Attachment
    {
        public AttachmentMethod Method { get; private set; }
        public uint Size { get; private set; }
        public uint RenderingPosition { get; private set; }
        public string Filename { get; private set; }
        public uint LTPRowID { get; private set; }
        public uint LTPRowVer { get; private set; }
        public bool InvisibleInHTML { get; private set; }
        public bool InvisibleInRTF { get; private set; }
        public bool RenderedInBody { get; private set; }

        public Attachment(bool unicode, TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                switch (exProp.ID)
                {
                    case MessageProperty.AttachmentSize:
                        Size = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentFileName:
                        if (exProp.Data != null)
                            Filename = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case MessageProperty.AttachmentMethod:
                        Method = (AttachmentMethod) BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentRenderPosition:
                        RenderingPosition = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentFlags:
                        var flags = BitConverter.ToUInt32(exProp.Data, 0);
                        InvisibleInHTML = (flags & 0x1) != 0;
                        InvisibleInRTF = (flags & 0x02) != 0;
                        RenderedInBody = (flags & 0x04) != 0;
                        break;
                    case MessageProperty.AttachmentLTPRowID:
                        LTPRowID = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case MessageProperty.AttachmentLTPRowVer:
                        LTPRowVer = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
