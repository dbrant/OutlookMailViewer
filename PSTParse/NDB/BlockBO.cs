﻿using System;
using System.Collections.Generic;

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
            var dataBlock = allData[0];
            if (entry.Internal)
            {
                var type = dataBlock.Data[0];
                var cLevel = dataBlock.Data[1];
                if (cLevel == 0) //SLBlock, no intermediate
                {
                    return BlockBO.GetSLBlockData(new SLBLOCK(pst.Header.isUnicode, dataBlock), pst);
                } else //SIBlock
                {
                    return BlockBO.GetSIBlockData(new SIBLOCK(pst.Header.isUnicode, dataBlock), pst);
                }
            } else
            {
                throw new Exception("Whoops");
            }
        }

        private static Dictionary<ulong, NodeDataDTO> GetSIBlockData(SIBLOCK siblock, PSTFile pst)
        {
            var ret = new Dictionary<ulong, NodeDataDTO>();

            foreach(var entry in siblock.Entries)
            {
                var curSLBlockBBT = pst.GetBlockBBTEntry(entry.SLBlockBID);
                var slblock = new SLBLOCK(pst.Header.isUnicode, BlockBO.GetBBTEntryData(curSLBlockBBT, pst)[0]);
                var data = BlockBO.GetSLBlockData(slblock, pst);
                foreach(var item in data)
                    ret.Add(item.Key, item.Value);
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
                ret.Add(entry.SubNodeNID, cur);

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
            int blockTrailerLen = pst.Header.isUnicode ? 16 : 12;

            var dataSize = entry.BlockByteCount;
            var blockSize = entry.BlockByteCount + blockTrailerLen;
            if (blockSize % 64 != 0)
                blockSize += 64 - (blockSize % 64);
            List<BlockDataDTO> dataBlocks;

            /*if (isSubNode)
            {
                using (var viewer = PSTFile.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, blockSize))
                {
                    var blockBytes = new byte[dataSize];
                    viewer.ReadArray(0, blockBytes, 0, dataSize);
                    dataBlocks = new List<BlockDataDTO>
                                     {new BlockDataDTO {Data = blockBytes, PstOffset = entry.BREF.IB, BBTEntry = entry}};
                    return dataBlocks;
                }
            } else */
            if (entry.Internal)
            {
                using(var viewer = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,blockSize))
                {
                    var blockBytes = new byte[dataSize];
                    viewer.ReadArray(0, blockBytes, 0, dataSize);

                    var trailerBytes = new byte[blockTrailerLen];
                    viewer.ReadArray(blockSize - blockTrailerLen, trailerBytes, 0, blockTrailerLen);
                    var trailer = new BlockTrailer(pst.Header.isUnicode, trailerBytes, 0);
                    
                    var dataBlockDTO = new BlockDataDTO
                                           {
                                               Data = blockBytes,
                                               PstOffset = entry.BREF.IB,
                                               CRCOffset = (uint)((long)entry.BREF.IB + (blockSize - (pst.Header.isUnicode ? 12 : 4))),
                                               BBTEntry = entry
                                           };
                    var type = blockBytes[0];
                    var level = blockBytes[1];

                    if (type == 2) //si or sl entry
                    {
                        return new List<BlockDataDTO> {dataBlockDTO};
                    } else if (type == 1)
                    {
                        if (blockBytes[1] == 0x01) //XBLOCK
                        {
                            var xblock = new XBLOCK(pst.Header.isUnicode, dataBlockDTO);
                            return BlockBO.GetXBlockData(xblock, pst);
                        
                        } else //XXBLOCK
                        {
                            var xxblock = new XXBLOCK(pst.Header.isUnicode, dataBlockDTO);
                            return BlockBO.GetXXBlockData(xxblock, pst);
                        }
                    } else
                    {
                        throw new NotImplementedException();
                    }
                }
            } else
            {
                using(var viewer = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,blockSize))
                {
                    var dataBytes = new byte[dataSize];
                    viewer.ReadArray(0, dataBytes, 0, dataSize);
                    
                    var trailerBytes = new byte[blockTrailerLen];
                    viewer.ReadArray(blockSize - blockTrailerLen, trailerBytes, 0, blockTrailerLen);
                    var trailer = new BlockTrailer(pst.Header.isUnicode, trailerBytes, 0);
                    dataBlocks = new List<BlockDataDTO>
                                     {
                                         new BlockDataDTO
                                             {
                                                 Data = dataBytes,
                                                 PstOffset = entry.BREF.IB,
                                                 CRC32 = trailer.CRC,
                                                 CRCOffset = (uint) (blockSize - (pst.Header.isUnicode ? 12 : 4)),
                                                 BBTEntry = entry
                                             }
                                     };
                }
            }

            for (int i = 0; i < dataBlocks.Count; i++)
            {
                var temp = dataBlocks[i].Data;   
                DataEncoder.CryptPermute(temp, temp.Length, false, pst);
            }
            return dataBlocks;
        }

        private static List<BlockDataDTO> GetXBlockData(XBLOCK xblock, PSTFile pst)
        {
            var ret = new List<BlockDataDTO>();
            foreach(var bid in xblock.BIDEntries)
            {
                var bbtEntry = pst.GetBlockBBTEntry(bid);
                ret.AddRange(BlockBO.GetBBTEntryData(bbtEntry,pst));
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
