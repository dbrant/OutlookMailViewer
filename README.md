# PST-Parser

A library for reading the [PST](https://learn.microsoft.com/en-us/openspecs/office_file_formats/ms-pst/141923d5-15ab-4ef1-a524-6dce75aae546) mailstore file format, used by legacy versions of Outlook, Exchange, etc. The focus is on maximal compatibility with various versions of the PST format, dating as far back as possible. In addition, the library attempts to be tolerant of corruption in PST files, allowing messages to be recovered from mailstores that are otherwise unreadable.

Dmitry Brant, 2017+
Daniel Cash, 2013-2017

License: MIT


## PST Structure Overview 

  The structure of the PST file format is divided into 3 layers: NDB layer, LTP layer, and the Messaging Layer.  Each layer is implemented on top of the preceeding layer.  For example, the LTP layer may implement a heap which is stored on a node in the NDB layer.  Each layer is divided into it's own namespace.  The main entry point of parsing a PST is through the header.  In the header, information about the format and encoding is stored.  The first offsets for the NDB layer are contained Root structure in the header.
  
  The Node Database (NDB) layer layer consists of two [B-trees](https://en.wikipedia.org/wiki/B-tree): one for nodes and another for data blocks.  Each B-tree implementation consists of intermediate blocks and leaf blocks.  The node B-tree consists of nodes that reference block IDs (BIDs) and sub nodes.  BIDs are used to traverse the data block B-tree to resolve to absolute offsets to data streams in the PST.  Data stream themselves can be in one data block or stored in another BTree if the data stream is too large to fit in one page.  XBLOCK and XXBLOCKs structures are used to store the B-trees that are used to store large data streams.
  
  The LTP layer provides the interface for the messaging layer to access properties and variable arrays of content.  The base of the LTP layer is a heap which can be stored on a node (heap-on-node or HN).  On the HN, yet another B-tree (B-tree-on-heap or BTH) is implmeneted and is used to store values on the HN using keys.  The BTH (can be thought of just as a heap) is used to store Property Contexts (PCs) and Table Contexts (TCs).  
  
  The messaging layer uses the LTP layer to represent folder heirarchies and the messages that exist in a given folder.

