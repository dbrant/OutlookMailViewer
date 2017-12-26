using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.LTP
{
    public class BTH
    {
        public HN HeapNode { get; private set; }
        public BTHHEADER Header { get; private set; }
        public BTHIndexNode Root { get; private set; }
        public int CurrentLevel { get; private set; }
        public Dictionary<byte[], BTHDataEntry> Properties { get; private set; }

        public BTH(HN heapNode, HID userRoot = null)
        {
            HeapNode = heapNode;

            var bthHeaderHID = userRoot ?? heapNode.HeapNodes[0].Header.UserRoot;
            Header = new BTHHEADER(HeapNodeBO.GetHNHIDBytes(heapNode, bthHeaderHID));
            Root = new BTHIndexNode(Header.BTreeRoot, this, (int)Header.NumLevels);

            Properties = new Dictionary<byte[], BTHDataEntry>(new ArrayUtilities.ByteArrayComparer());

            var stack = new Stack<BTHIndexNode>();
            stack.Push(Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Data != null)
                    foreach (var entry in cur.Data.DataEntries)
                        Properties.Add(entry.Key, entry);

                if (cur.Children != null)
                    foreach (var child in cur.Children)
                        stack.Push(child);

            }
        }

        public HNDataDTO GetHIDBytes(HID hid)
        {
            return HeapNode.GetHIDBytes(hid);
        }

        public UInt32 GetKeyValue(byte[] key)
        {
            if (key.Length == 4)
            {
                return BitConverter.ToUInt32(key, 0);
            }
            else if (key.Length == 2)
            {
                return BitConverter.ToUInt16(key, 0);
            }
            return 0;
        }

        public UInt32 GetDataValue(byte[] data)
        {
            if (data.Length == 4)
            {
                return BitConverter.ToUInt32(data, 0);
            }
            else if (data.Length == 2)
            {
                return BitConverter.ToUInt16(data, 0);
            }
            return 0;
        }

        public Dictionary<ushort, ExchangeProperty> GetExchangeProperties()
        {
            var ret = new Dictionary<ushort, ExchangeProperty>();

            var stack = new Stack<BTHIndexNode>();
            stack.Push(Root);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();

                if (cur.Data != null)
                    foreach (var entry in cur.Data.DataEntries)
                    {
                        var curKey = BitConverter.ToUInt16(entry.Key, 0);
                        int i = 0;
                        if (curKey == 0x02)
                            i++;
                        ret.Add(curKey, new ExchangeProperty(entry, this));
                    }

                if (cur.Children != null)
                    foreach (var child in cur.Children)
                        stack.Push(child);

            }

            return ret;
        }
    }
}
