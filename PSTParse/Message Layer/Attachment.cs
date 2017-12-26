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
                    case 0x0e20:
                        Size = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x3704:
                        if (exProp.Data != null)
                            Filename = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case 0x3705:
                        Method = (AttachmentMethod) BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x370b:
                        RenderingPosition = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x3714:
                        var flags = BitConverter.ToUInt32(exProp.Data, 0);
                        InvisibleInHTML = (flags & 0x1) != 0;
                        InvisibleInRTF = (flags & 0x02) != 0;
                        RenderedInBody = (flags & 0x04) != 0;
                        break;
                    case 0x67F2:
                        LTPRowID = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x67F3:
                        LTPRowVer = BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
