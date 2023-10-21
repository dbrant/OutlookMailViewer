using System;
using System.Collections.Generic;
using MiscParseUtilities;
using PSTParse.Message_Layer;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class ExchangeProperty
    {
        public enum PropType
        {
            Unspecified = 0x0000,
            NullType = 0x0001,
            Integer16 = 0x0002,
            Integer32 = 0x0003,
            Floating32 = 0x0004,
            Floating64 = 0x0005,
            Currency = 0x0006,
            FloatingTime = 0x0007,
            ErrorCode = 0x000A,
            Boolean = 0x000B,
            ObjectType = 0x000D,
            Integer64 = 0x0014,
            String = 0x001F,
            String8 = 0x001E,
            Time = 0x0040,
            Guid = 0x0048,
            ServerId = 0x00FB,
            Restriction = 0x00FD,
            RuleAction = 0x00FE,
            Binary = 0x0102,
            MultipleInteger16 = 0x1002,
            MultipleInteger32 = 0x1003,
            MultipleFloating32 = 0x1004,
            MultipleFloating64 = 0x1005,
            MultipleCurrency = 0x1006,
            MultipleFloatingTime = 0x1007,
            MultipleInteger64 = 0x1014,
            MultipleString = 0x101F,
            MultipleString8 = 0x101E,
            MultipleTime = 0x1040,
            MultipleGuid = 0x1048,
            MultipleBinary = 0x1102,
        }

        public static List<ExchangeProperty> PropertyLookupByType = new List<ExchangeProperty> {
            new ExchangeProperty{Type = PropType.Integer16, ByteCount = 2, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.Integer32, ByteCount = 4, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.Floating32, ByteCount = 4, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.Floating64, ByteCount = 8, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.Currency, ByteCount = 8, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.FloatingTime, ByteCount = 8, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.ErrorCode, ByteCount = 4, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.Boolean, ByteCount = 1, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.Integer64, ByteCount = 8, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.String, ByteCount = 0, MultiValue = true, Variable = true},
            new ExchangeProperty{Type = PropType.String8, ByteCount = 0, MultiValue = true, Variable = true},
            new ExchangeProperty{Type = PropType.Time, ByteCount = 8, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.Guid, ByteCount = 16, MultiValue = false, Variable = false},
            new ExchangeProperty{Type = PropType.ServerId, ByteCount = 0, MultiValue = false, Variable = true},
            new ExchangeProperty{Type = PropType.Restriction, ByteCount = 0, MultiValue = false, Variable = true},
            new ExchangeProperty{Type = PropType.RuleAction, ByteCount = 0, MultiValue = true, Variable = true},
            new ExchangeProperty{Type = PropType.Binary, ByteCount = 1, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleInteger16, ByteCount = 2, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleInteger32, ByteCount = 4, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleFloating32, ByteCount = 4, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleFloating64, ByteCount = 8, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleCurrency, ByteCount = 8, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleFloatingTime, ByteCount = 8, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleInteger64, ByteCount = 8, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleString, ByteCount = 0, MultiValue = true, Variable = true},
            new ExchangeProperty{Type = PropType.MultipleString8, ByteCount = 0, MultiValue = true, Variable = true},
            new ExchangeProperty{Type = PropType.MultipleTime, ByteCount = 8, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleGuid, ByteCount = 8, MultiValue = true, Variable = false},
            new ExchangeProperty{Type = PropType.MultipleBinary, ByteCount = 0, MultiValue = true, Variable = true},
            new ExchangeProperty{Type = PropType.ObjectType, ByteCount = 8, MultiValue = false, Variable = false},
        };

        public MessageProperty ID { get; private set; }
        public PropType Type { get; private set; }
        public bool MultiValue { get; private set; }
        public bool Variable { get; private set; }
        public uint ByteCount { get; private set; }
        public byte[] Data { get; set; }
        //private BTHDataEntry entry;
        private byte[] Key;

        public ExchangeProperty() { }

        public ExchangeProperty(UInt16 ID, UInt16 type, BTH heap, byte[] key)
        {
            this.ID = (MessageProperty)ID;
            Type = (PropType)type;
            /*var tempKey = new byte[key.Length + 2];
            tempKey[0] = 0x00;
            tempKey[1] = 0x00;
            for (int i = 0; i < key.Length; i++)
                tempKey[i + 2] = key[i];*/
            Key = key;

            GetData(heap, true);
        }

        public ExchangeProperty(BTHDataEntry entry, BTH heap)
        {
            //this.entry = entry;

            Key = entry.Data.RangeSubset(2, entry.Data.Length - 2);
            ID = (MessageProperty)BitConverter.ToUInt16(entry.Key, 0);
            Type = (PropType)BitConverter.ToUInt16(entry.Data, 0);

            GetData(heap);
        }

        private void GetData(BTH heap, bool isTable = false)
        {
            var lookupProp = PropertyLookupByType.Find(p => p.Type == Type);
            if (lookupProp != null)
            {
                MultiValue = lookupProp.MultiValue;
                Variable = lookupProp.Variable;
                ByteCount = lookupProp.ByteCount;
            }
            //get data here

            if (!MultiValue && !Variable)
            {
                if (ByteCount <= 4 || (isTable && ByteCount <= 8))
                {
                    Data = Key;
                }
                else
                {
                    Data = heap.GetHIDBytes(new HID(Key)).Data;
                }
            }
            else
            {
                //oh no, it's an HNID
                var curID = BitConverter.ToUInt32(Key, 0);

                if (curID == 0)
                {

                }
                else if ((curID & 0x1F) == 0) //must be HID
                {
                    Data = heap.GetHIDBytes(new HID(Key)).Data;
                }
                else //let's assume NID
                {
                    var totalSize = 0;
                    var dataBlocks = new List<BlockDataDTO>();
                    if (heap.HeapNode.HeapSubNode.ContainsKey(curID))
                        dataBlocks = heap.HeapNode.HeapSubNode[curID].NodeData;
                    else
                    {
                        var tempSubNodeXREF = new Dictionary<ulong, NodeDataDTO>();
                        foreach (var heapSubNode in heap.HeapNode.HeapSubNode)
                            tempSubNodeXREF.Add(heapSubNode.Key & 0xFFFFFFFF, heapSubNode.Value);
                        dataBlocks = tempSubNodeXREF[curID].NodeData;
                        //dataBlocks = entry.ParentTree.HeapNode.HeapSubNode[curID].NodeData;
                    }
                    foreach (var dataBlock in dataBlocks)
                        totalSize += dataBlock.Data.Length;
                    var allData = new byte[totalSize];
                    var curPos = 0;
                    foreach (var datablock in dataBlocks)
                    {
                        for (int i = 0; i < datablock.Data.Length; i++)
                        {
                            allData[i + curPos] = datablock.Data[i];
                        }
                        curPos += datablock.Data.Length;
                    }
                    Data = allData;
                }
            }
        }
    }
}
