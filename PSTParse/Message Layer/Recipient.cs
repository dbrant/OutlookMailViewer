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
                    case MessageProperty.RecipientType:
                        Type = exProp.Data.Length > 0 ? (RecipientType)BitConverter.ToUInt32(exProp.Data, 0) : RecipientType.FROM;
                        break;
                    case MessageProperty.RecipientResponsibility:
                        Responsibility = exProp.Data.Length > 0 ? exProp.Data[0] == 0x01 : false;
                        break;
                    case MessageProperty.RecipientObjType:
                        ObjType = exProp.Data.Length >= 4 ? (PSTEnums.ObjectType)BitConverter.ToUInt32(exProp.Data, 0) : PSTEnums.ObjectType.MAIL_USER;
                        break;
                    case MessageProperty.RecipientEntryID:
                        EntryID = exProp.Data.Length >= EntryID.Size ? new EntryID(exProp.Data) : null;
                        break;
                    case MessageProperty.DisplayName:
                        DisplayName = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case MessageProperty.AddressType:
                        EmailAddressType = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    case MessageProperty.AddressName:
                        EmailAddress = unicode ? Encoding.Unicode.GetString(exProp.Data) : Encoding.ASCII.GetString(exProp.Data);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
