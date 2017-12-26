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
                        tempSubNodes.Add(nod.Key & 0xffffffff, nod.Value);
                    TCRMData = tempSubNodes[rowMatrixHNID].NodeData;
                }
            }
            //TCRMSubNodeData = TableContext.HeapNode.HeapSubNode[];
            var rowSize = TableContext.TCHeader.EndOffsetCEB;
            //var rowPerBlock = (8192 - 16)/rowSize;
            
            foreach(var row in TableContext.RowIndexBTH.Properties)
            {
                var rowIndex = TableContext.RowIndexBTH.GetDataValue(row.Value.Data);

                var blockTrailerSize = 16;
                var maxBlockSize = 8192 - blockTrailerSize;
                var recordsPerBlock = maxBlockSize/rowSize;

                var blockIndex = (int)rowIndex / recordsPerBlock;
                var indexInBlock = rowIndex % recordsPerBlock;
                var curRow = new TCRowMatrixData(TCRMData[blockIndex].Data, TableContext, heap, (int) indexInBlock*rowSize);
                RowXREF.Add(TableContext.RowIndexBTH.GetKeyValue(row.Key), curRow);
                Rows.Add(curRow);
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
