using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    public static class BlockBO
    {
        public static NodeDataDTO GetNodeData(ulong nid, PSTFile pst)
        {
            return GetNodeData(pst.GetNodeBIDs(nid), pst);
        }

        public static NodeDataDTO GetNodeData(Tuple<ulong, ulong> nodeBIDs, PSTFile pst)
        {
            var mainData = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(nodeBIDs.Item1), pst);
            var subNodeData = new Dictionary<ulong, NodeDataDTO>();

            if (nodeBIDs.Item2 != 0)
                subNodeData = BlockBO.GetSubNodeData(pst.GetBlockBBTEntry(nodeBIDs.Item2), pst);

            return new NodeDataDTO { NodeData = mainData, SubNodeData = subNodeData };
        }

        private static Dictionary<ulong, NodeDataDTO> GetSubNodeData(BBTENTRY entry, PSTFile pst)
        {
            var allData = BlockBO.GetBBTEntryData(entry, pst);
            if (allData == null || allData.Count == 0)
                return new Dictionary<ulong, NodeDataDTO>();

            var dataBlock = allData[0];
            if (entry.Internal)
            {
                var type = dataBlock.Data[0];
                var cLevel = dataBlock.Data[1];
                if (type != 2)
                    return new Dictionary<ulong, NodeDataDTO>();
                if (cLevel == 0) //SLBlock, no intermediate
                {
                    return BlockBO.GetSLBlockData(new SLBLOCK(pst.Header.isUnicode, dataBlock), pst);
                } else //SIBlock
                {
                    return BlockBO.GetSIBlockData(new SIBLOCK(pst.Header.isUnicode, dataBlock), pst);
                }
            }
            return new Dictionary<ulong, NodeDataDTO>();
        }

        private static Dictionary<ulong, NodeDataDTO> GetSIBlockData(SIBLOCK siblock, PSTFile pst)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();

            foreach(var entry in siblock.Entries)
            {
                var curSLBlockBBT = pst.GetBlockBBTEntry(entry.SLBlockBID);
                var slBlockData = BlockBO.GetBBTEntryData(curSLBlockBBT, pst);
                if (slBlockData.Count == 0)
                    continue;
                var slblock = new SLBLOCK(pst.Header.isUnicode, slBlockData[0]);
                var data = BlockBO.GetSLBlockData(slblock, pst);
                foreach(var item in data)
                    ret[item.Key] = item.Value;
            }
            
            return ret;
        }
        //gets all the data for an SL block.  an SL block points directly to all the immediate subnodes
        private static Dictionary<ulong, NodeDataDTO> GetSLBlockData(SLBLOCK slblock, PSTFile pst)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();
            foreach(var entry in slblock.Entries)
            {
                //this data should represent the main data part of the subnode
                var data = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(entry.SubNodeBID), pst);
                var cur = new NodeDataDTO {NodeData = data};
                ret[entry.SubNodeNID] = cur;

                //see if there are sub nodes of this current sub node
                if (entry.SubSubNodeBID != 0)
                    //if there are subnodes, treat them like any other subnode
                    cur.SubNodeData = GetSubNodeData(pst.GetBlockBBTEntry(entry.SubSubNodeBID), pst);

                
            }
            return ret;
        }
        public static NodeDataDTO GetNodeData(NBTENTRY entry, PSTFile pst)
        {
            var mainData = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(entry.BID_Data), pst);
            if (entry.BID_SUB != 0)
            {
                var subnodeData = BlockBO.GetSubNodeData(pst.GetBlockBBTEntry(entry.BID_SUB), pst);
                return new NodeDataDTO {NodeData = mainData, SubNodeData = subnodeData};
            } 

            return new NodeDataDTO {NodeData = mainData, SubNodeData = null};
        }

        public static NodeDataDTO GetNodeData(SLENTRY entry, PSTFile pst)
        {
            var mainData = BlockBO.GetBBTEntryData(pst.GetBlockBBTEntry(entry.SubNodeBID), pst);
            if (entry.SubSubNodeBID != 0)
            {
                var subNodeData = BlockBO.GetSubNodeData(pst.GetBlockBBTEntry(entry.SubSubNodeBID),pst);
                return new NodeDataDTO {NodeData = mainData, SubNodeData = subNodeData};
            }

            return new NodeDataDTO {NodeData = mainData, SubNodeData = null};
        }

        //for a given bbt entry, retrieve the raw bytes associated with the BID
        //this includes retrieving data trees via xblocks
        public static List<BlockDataDTO> GetBBTEntryData(BBTENTRY entry, PSTFile pst)
        {
            if (entry == null)
                return new List<BlockDataDTO>();

            var block = ReadBlock(entry, pst, out bool valid);
            if (!valid)
            {
                // The block at the location given by the BBT is damaged; see if an intact copy
                // of it can be found elsewhere in the file.
                var scanned = pst.GetScannedBlockEntry(entry.Key);
                if (scanned != null && scanned.BREF.IB != entry.BREF.IB)
                {
                    entry = scanned;
                    block = ReadBlock(entry, pst, out valid);
                    pst.ReportBlockRecovered(entry.Key);
                }
                else
                {
                    pst.ReportBlockDamaged(entry.Key);
                }
            }
            if (block == null)
                return new List<BlockDataDTO>();

            if (entry.Internal)
            {
                if (block.Data.Length < 8)
                    return new List<BlockDataDTO>();
                var type = block.Data[0];
                var level = block.Data[1];

                if (type == 2) //si or sl entry
                {
                    return new List<BlockDataDTO> {block};
                } else if (type == 1)
                {
                    if (level == 0x01) //XBLOCK
                    {
                        var xblock = new XBLOCK(pst.Header.isUnicode, block);
                        return BlockBO.GetXBlockData(xblock, pst);
                    } else //XXBLOCK
                    {
                        var xxblock = new XXBLOCK(pst.Header.isUnicode, block);
                        return BlockBO.GetXXBlockData(xxblock, pst);
                    }
                } else
                {
                    return new List<BlockDataDTO>();
                }
            }

            DataEncoder.CryptPermute(block.Data, block.Data.Length, false, pst);
            return new List<BlockDataDTO> {block};
        }

        // Reads the raw (still encoded) data of a single block, and checks it against the block trailer.
        private static BlockDataDTO ReadBlock(BBTENTRY entry, PSTFile pst, out bool valid)
        {
            valid = false;
            bool unicode = pst.Header.isUnicode;
            int blockTrailerLen = unicode ? 16 : 12;
            int dataSize = entry.BlockByteCount;
            int blockSize = dataSize + blockTrailerLen;
            if (blockSize % 64 != 0)
                blockSize += 64 - (blockSize % 64);
            if ((long)entry.BREF.IB + blockSize > pst.FileSize)
                return null;

            using (var viewer = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, blockSize))
            {
                var dataBytes = new byte[dataSize];
                viewer.ReadArray(0, dataBytes, 0, dataSize);

                var trailerBytes = new byte[blockTrailerLen];
                viewer.ReadArray(blockSize - blockTrailerLen, trailerBytes, 0, blockTrailerLen);
                var trailer = new BlockTrailer(unicode, trailerBytes, 0);

                valid = trailer.DataSize == dataSize
                        && (trailer.BID_raw & 0xfffffffffffffffe) == entry.Key
                        && trailer.CRC == new CRC32().ComputeCRC(0, dataBytes, (uint)dataSize);

                return new BlockDataDTO
                           {
                               Data = dataBytes,
                               PstOffset = entry.BREF.IB,
                               CRC32 = trailer.CRC,
                               CRCOffset = (uint) (blockSize - (unicode ? 12 : 4)),
                               BBTEntry = entry
                           };
            }
        }

        private static List<BlockDataDTO> GetXBlockData(XBLOCK xblock, PSTFile pst)
        {
            var ret = new List<BlockDataDTO>();
            foreach(var bid in xblock.BIDEntries)
            {
                var bbtEntry = pst.GetBlockBBTEntry(bid);
                var data = BlockBO.GetBBTEntryData(bbtEntry, pst);
                if (data.Count > 0)
                    ret.AddRange(data);
                else
                    // keep a placeholder for a missing block, so that the indices of subsequent blocks are preserved.
                    ret.Add(new BlockDataDTO { Data = new byte[0] });
            }
            return ret;
        }

        private static List<BlockDataDTO> GetXXBlockData(XXBLOCK xxblock, PSTFile pst)
        {
            var ret = new List<BlockDataDTO>();
            foreach(var bid in xxblock.XBlockBIDs)
            {
                var bbtEntry = pst.GetBlockBBTEntry(bid);
                var curXblockData = BlockBO.GetBBTEntryData(bbtEntry, pst);
                //var curXblockData = BlockBO.GetXBlockData(curXblock);
                foreach(var block in curXblockData)
                    ret.Add(block);
            }
            return ret;
        }
    }
}
