using System.Collections.Generic;
using System.IO;
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

            HeapSubNode = nodeData.SubNodeData ?? new Dictionary<ulong, NodeDataDTO>();

            if (HeapNodes.Count == 0 || HeapNodes[0].Header == null)
                throw new InvalidDataException("Heap node data is missing.");
            if (HeapNodes[0].Header.bSig != 0xEC)
                throw new InvalidDataException("Heap node signature is invalid.");
        }

        public HNDataDTO GetHIDBytes(HID hid)
        {
            if ((int)hid.hidBlockIndex >= HeapNodes.Count)
                throw new InvalidDataException("Heap block " + hid.hidBlockIndex + " not found.");
            return HeapNodes[(int)hid.hidBlockIndex].GetAllocation(hid);
        }
    }
}
