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

        public RecipientType Type;
        public PSTEnums.ObjectType ObjType;
        public bool Responsibility;
        public byte[] Tag;
        public EntryID EntryID;
        public string DisplayName;
        public string EmailAddress;
        public string EmailAddressType;

        public Recipient(bool unicode, TCRowMatrixData row)
        {
            foreach (var exProp in row)
            {
                switch (exProp.ID)
                {
                    case 0x0c15:
                        this.Type = (RecipientType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0e0f:
                        this.Responsibility = exProp.Data[0] == 0x01;
                        break;
                    case 0x0ff9:
                        this.Tag = exProp.Data;
                        break;
                    case 0x0ffe:
                        this.ObjType = (PSTEnums.ObjectType)BitConverter.ToUInt32(exProp.Data, 0);
                        break;
                    case 0x0fff:
                        this.EntryID = new EntryID(exProp.Data);
                        break;
                    case 0x3001:
                        this.DisplayName = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case 0x3002:
                        this.EmailAddressType = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case 0x3003:
                        this.EmailAddress = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
