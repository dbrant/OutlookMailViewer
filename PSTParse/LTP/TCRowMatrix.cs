﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class TCRowMatrix
    {
        public TableContext TableContext;
        public List<BlockDataDTO> TCRMData;

        public List<TCRowMatrixData> Rows;
        public Dictionary<uint, TCRowMatrixData> RowXREF; 

        public TCRowMatrix(TableContext tableContext, BTH heap)
        {
            this.Rows = new List<TCRowMatrixData>();
            this.RowXREF = new Dictionary<uint, TCRowMatrixData>();

            this.TableContext = tableContext;
            var rowMatrixHNID = this.TableContext.TCHeader.RowMatrixLocation;
            if (rowMatrixHNID == 0)
                return;
            
            if ((rowMatrixHNID & 0x1F) == 0)//HID
            {
                this.TCRMData = new List<BlockDataDTO>{
                    new BlockDataDTO
                        {
                            Data = this.TableContext.HeapNode.GetHIDBytes(new HID(BitConverter.GetBytes(rowMatrixHNID))).Data
                        }};
            } else
            {
                if (this.TableContext.HeapNode.HeapSubNode.ContainsKey(rowMatrixHNID))
                    this.TCRMData = this.TableContext.HeapNode.HeapSubNode[rowMatrixHNID].NodeData;
                else
                {
                    var tempSubNodes = new Dictionary<ulong, NodeDataDTO>();
                    foreach(var nod in this.TableContext.HeapNode.HeapSubNode)
                        tempSubNodes.Add(nod.Key & 0xffffffff, nod.Value);
                    this.TCRMData = tempSubNodes[rowMatrixHNID].NodeData;
                }
            }
            //this.TCRMSubNodeData = this.TableContext.HeapNode.HeapSubNode[];
            var rowSize = this.TableContext.TCHeader.EndOffsetCEB;
            //var rowPerBlock = (8192 - 16)/rowSize;
            
            foreach(var row in this.TableContext.RowIndexBTH.Properties)
            {
                var rowIndex = TableContext.RowIndexBTH.GetDataValue(row.Value.Data);

                var blockTrailerSize = 16;
                var maxBlockSize = 8192 - blockTrailerSize;
                var recordsPerBlock = maxBlockSize/rowSize;

                var blockIndex = (int)rowIndex / recordsPerBlock;
                var indexInBlock = rowIndex % recordsPerBlock;
                var curRow = new TCRowMatrixData(this.TCRMData[blockIndex].Data, this.TableContext, heap, (int) indexInBlock*rowSize);
                this.RowXREF.Add(TableContext.RowIndexBTH.GetKeyValue(row.Key), curRow);
                this.Rows.Add(curRow);
            }
            /*
            uint curIndex = 0;
            foreach (var dataBlock in this.TCRMData)
            {
                for(int i = 0;i + rowSize < dataBlock.Data.Length; i += rowSize)
                {
                    var curRow = new TCRowMatrixData(dataBlock.Data, this.TableContext, i);
                    this.RowXREF.Add(this.TableContext.ReverseRowIndex[curIndex], curRow);
                    this.Rows.Add(curRow);
                    curIndex++;
                }
            }*/
            
        }
    }
}
