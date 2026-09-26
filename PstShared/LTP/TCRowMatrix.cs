using System;
using System.Collections.Generic;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class TCRowMatrix
    {
        public TableContext TableContext { get; private set; }
        public List<BlockDataDTO> TCRMData { get; private set; }

        public List<TCRowMatrixData> Rows { get; private set; }
        public Dictionary<uint, TCRowMatrixData> RowXREF { get; private set; }

        public TCRowMatrix(TableContext tableContext, BTH heap)
        {
            Rows = new List<TCRowMatrixData>();
            RowXREF = new Dictionary<uint, TCRowMatrixData>();

            TableContext = tableContext;
            var rowMatrixHNID = TableContext.TCHeader.RowMatrixLocation;
            if (rowMatrixHNID == 0)
                return;
            
            if ((rowMatrixHNID & 0x1F) == 0)//HID
            {
                TCRMData = new List<BlockDataDTO>{
                    new BlockDataDTO
                        {
                            Data = TableContext.HeapNode.GetHIDBytes(new HID(BitConverter.GetBytes(rowMatrixHNID))).Data
                        }};
            } else
            {
                if (TableContext.HeapNode.HeapSubNode.ContainsKey(rowMatrixHNID))
                    TCRMData = TableContext.HeapNode.HeapSubNode[rowMatrixHNID].NodeData;
                else
                {
                    var tempSubNodes = new Dictionary<ulong, NodeDataDTO>();
                    foreach(var nod in TableContext.HeapNode.HeapSubNode)
                        tempSubNodes[nod.Key & 0xffffffff] = nod.Value;
                    TCRMData = tempSubNodes.TryGetValue(rowMatrixHNID, out var node) ? node.NodeData : new List<BlockDataDTO>();
                }
            }
            //TCRMSubNodeData = TableContext.HeapNode.HeapSubNode[];
            var rowSize = TableContext.TCHeader.EndOffsetCEB;
            if (rowSize == 0 || TCRMData.Count == 0)
                return;

            // Each block of the row matrix holds as many whole rows as will fit. Every block except the last
            // is full, so the first block tells us how many rows each block holds. (This avoids assuming the
            // size of the block trailer, which differs between ANSI and Unicode files.)
            var recordsPerBlock = TCRMData.Count > 1 && TCRMData[0].Data.Length >= rowSize
                ? TCRMData[0].Data.Length / rowSize
                : (8192 - 12) / rowSize;

            foreach(var row in TableContext.RowIndexBTH.Properties)
            {
                try
                {
                    var rowIndex = TableContext.RowIndexBTH.GetDataValue(row.Value.Data);
                    var blockIndex = (int)rowIndex / recordsPerBlock;
                    var indexInBlock = rowIndex % recordsPerBlock;
                    if (blockIndex >= TCRMData.Count)
                        continue;
                    var curRow = new TCRowMatrixData(TCRMData[blockIndex].Data, TableContext, heap, (int) indexInBlock*rowSize);
                    RowXREF[TableContext.RowIndexBTH.GetKeyValue(row.Key)] = curRow;
                    Rows.Add(curRow);
                }
                catch (Exception)
                {
                    // skip a damaged row, and keep the rest.
                }
            }
            /*
            uint curIndex = 0;
            foreach (var dataBlock in TCRMData)
            {
                for(int i = 0;i + rowSize < dataBlock.Data.Length; i += rowSize)
                {
                    var curRow = new TCRowMatrixData(dataBlock.Data, TableContext, i);
                    RowXREF.Add(TableContext.ReverseRowIndex[curIndex], curRow);
                    Rows.Add(curRow);
                    curIndex++;
                }
            }*/
            
        }
    }
}
