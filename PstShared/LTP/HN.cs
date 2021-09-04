using System.Collections.Generic;
using PSTParse.NDB;

namespace PSTParse.LTP
{
    public class HN
    {
        public NBTENTRY HNNode { get; private set; }
        public List<HNBlock> HeapNodes { get; private set; }
        public Dictionary<ulong, NodeDataDTO> HeapSubNode { get; private set; }

        public HN(NodeDataDTO nodeData)
        {
            HeapNodes = new List<HNBlock>();
            var numBlocks = nodeData.NodeData.Count;
            for (int i = 0; i < numBlocks; i++)
            {
                var curBlock = new HNBlock(i, nodeData.NodeData[i]);
                HeapNodes.Add(curBlock);
            }

            HeapSubNode = nodeData.SubNodeData;
        }

        public HNDataDTO GetHIDBytes(HID hid)
        {
            return HeapNodes[(int)hid.hidBlockIndex].GetAllocation(hid);
        }
    }
}
