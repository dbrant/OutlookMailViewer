
namespace PSTParse.NDB
{
    public class NID
    {
        public enum NodeType
        {
            HID = 0x00,
            INTERNAL = 0x01,
            NORMAL_FOLDER = 0x02,
            SEARCH_FOLDER = 0x03,
            NORMAL_MESSAGE_PC = 0x03,
            ATTACHMENT_PC = 0x05,
            SEARCH_UPDATE_QUEUE = 0x06,
            SEARCH_CRITERIA_OBJECT = 0x07,
            ASSOC_MESSAGE = 0x08,
            CONTENTS_TABLE_INDEX = 0x0A,
            RECEIVE_FOLDER_TABLE = 0x0B,
            OUTGOING_QUEUE_TABLE = 0x0C,
            HIERARCHY_TABLE = 0x0D,
            CONTENTS_TABLE = 0x0E,
            ASSOC_CONTENTS_TABLE = 0x0F,
            SEARCH_CONTENTS_TABLE = 0x10,
            ATTACHMENT_TABLE = 0x11,
            RECIPIENT_TABLE = 0x12,
            SEARCH_TABLE_INDEX = 0x13,
            LTP = 0x1F
        }

        public NodeType Type { get; private set; }

        public NID(ulong nid)
        {
            Type = (NodeType) (nid & 0x1f);
        }
    }
}
