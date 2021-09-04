using System;
using System.Collections.Generic;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class TableContext
    {
        public TCINFOHEADER TCHeader { get; private set; }
        public HN HeapNode { get; private set; }
        public NodeDataDTO NodeData { get; private set; }
        public BTH RowIndexBTH { get; private set; }
        public Dictionary<uint, uint> ReverseRowIndex { get; private set; }
        public TCRowMatrix RowMatrix { get; private set; }

        public TableContext(ulong nid, PSTFile pst)
            : this(BlockBO.GetNodeData(nid, pst))
        { }

        public TableContext(NodeDataDTO nodeData)
        {
            NodeData = nodeData;
            HeapNode = new HN(NodeData);

            var tcinfoHID = HeapNode.HeapNodes[0].Header.UserRoot;
            var tcinfoHIDbytes = HeapNode.GetHIDBytes(tcinfoHID);
            TCHeader = new TCINFOHEADER(tcinfoHIDbytes.Data);

            RowIndexBTH = new BTH(HeapNode, TCHeader.RowIndexLocation);
            ReverseRowIndex = new Dictionary<uint, uint>();
            foreach (var prop in RowIndexBTH.Properties)
            {
                uint temp = RowIndexBTH.GetDataValue(prop.Value.Data);
                ReverseRowIndex.Add(temp, BitConverter.ToUInt32(prop.Key, 0));
            }
            RowMatrix = new TCRowMatrix(this, RowIndexBTH);
        }
    }
}
