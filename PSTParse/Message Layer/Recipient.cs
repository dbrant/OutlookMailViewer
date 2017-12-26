using System;
using System.Text;
using PSTParse.LTP;

namespace PSTParse.Message_Layer
{
    public class Recipient
    {
        public enum RecipientType
        {
            FROM=0x00,
            TO=0x01,
            CC=0x02,
            BCC=0x03
        }

        public RecipientType Type { get; private set; }
        public PSTEnums.ObjectType ObjType { get; private set; }
        public bool Responsibility { get; private set; }
        public byte[] Tag { get; private set; }
        public EntryID EntryID { get; private set; }
        public string DisplayName { get; private set; }
        public string EmailAddress { get; private set; }
        public string EmailAddressType { get; private set; }

        public Recipient(bool unicode, TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                switch (exProp.ID)
                {
                    case 0x0c15:
                        Type = (RecipientType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0e0f:
                        Responsibility = exProp.Data[0] == 0x01;
                        break;
                    case 0x0ff9:
                        Tag = exProp.Data;
                        break;
                    case 0x0ffe:
                        ObjType = (PSTEnums.ObjectType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0fff:
                        EntryID = new EntryID(exProp.Data);
                        break;
                    case 0x3001:
                        DisplayName = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case 0x3002:
                        EmailAddressType = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case 0x3003:
                        EmailAddress = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
