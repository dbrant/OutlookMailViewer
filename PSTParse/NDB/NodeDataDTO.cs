using System.Collections.Generic;

namespace PSTParse.NDB
{
    public class NodeDataDTO
    {
        public List<BlockDataDTO> NodeData; 
        public Dictionary<ulong, NodeDataDTO> SubNodeData;
    }
}
