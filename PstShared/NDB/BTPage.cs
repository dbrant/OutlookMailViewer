using System;
using System.Collections.Generic;
using MiscParseUtilities;

namespace PSTParse.NDB
{
    public class BTPage
    {
        private PageTrailer _trailer;
        private int _numEntries;
        private int _maxEntries;
        private int _cbEnt;
        private int _cLevel;
        private BREF _ref;

        public List<BTPAGEENTRY> Entries { get; private set; }
        public List<BTPage> InternalChildren { get; private set; }

        public bool IsNode { get { return _trailer.PageType == PageType.NBT; } }
        public bool IsBlock { get { return _trailer.PageType == PageType.BBT; } }

        public ulong BID { get { return _trailer.BID; } }

        public BTPage(bool unicode, byte[] pageData, BREF _ref, PSTFile pst)
        {
            InternalChildren = new List<BTPage>();
            this._ref = _ref;
            if (unicode)
            {
                _trailer = new PageTrailer(unicode, pageData.RangeSubset(496, 16));
                _numEntries = pageData[488];
                _maxEntries = pageData[489];
                _cbEnt = pageData[490];
                _cLevel = pageData[491];
            }
            else
            {
                _trailer = new PageTrailer(unicode, pageData.RangeSubset(500, 12));
                _numEntries = pageData[496];
                _maxEntries = pageData[497];
                _cbEnt = pageData[498];
                _cLevel = pageData[499];
            }

            Entries = new List<BTPAGEENTRY>();

            // A damaged (e.g. zeroed) page is treated as having no entries.
            if (_trailer.PageType != PageType.NBT && _trailer.PageType != PageType.BBT)
                return;
            int entryAreaSize = unicode ? 488 : 496;
            if (_cbEnt == 0 || _numEntries * _cbEnt > entryAreaSize)
                return;

            for (var i = 0; i < _numEntries; i++)
            {
                var curEntryBytes = pageData.RangeSubset(i*_cbEnt, _cbEnt);
                if (_cLevel == 0)
                {
                    if (_trailer.PageType == PageType.NBT)
                        Entries.Add(new NBTENTRY(unicode, curEntryBytes));
                    else
                        Entries.Add(new BBTENTRY(unicode, curEntryBytes));
                }
                else
                {
                    //btentries
                    var entry = new BTENTRY(unicode, curEntryBytes);
                    Entries.Add(entry);
                    var bytes = new byte[512];
                    if ((long)entry.BREF.IB + 512 <= pst.FileSize)
                    {
                        using (var view = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB, 512))
                            view.ReadArray(0, bytes, 0, 512);
                    }
                    var child = new BTPage(unicode, bytes, entry.BREF, pst);
                    // Discard a child page that doesn't belong to this tree, which also guards against cycles.
                    if (child._trailer.PageType != _trailer.PageType || child._cLevel != _cLevel - 1)
                        child = new BTPage(unicode, new byte[512], entry.BREF, pst);
                    InternalChildren.Add(child);
                }
            }
        }

        public BBTENTRY GetBIDBBTEntry(ulong BID)
        {
            int ii = 0;
            if (BID % 2 == 1)
                ii++;
            BID &= 0xfffffffffffffffe;
            for (int i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];
                if (i == Entries.Count - 1)
                {

                    if (entry is BTENTRY)
                        return InternalChildren[i].GetBIDBBTEntry(BID);
                    else
                    {
                        var temp = entry as BBTENTRY;
                        if (BID == temp.Key)
                            return temp;
                    }

                }
                else
                {
                    var entry2 = Entries[i + 1];
                    if (entry is BTENTRY)
                    {
                        var cur = entry as BTENTRY;
                        var next = entry2 as BTENTRY;
                        if (BID >= cur.Key && BID < next.Key)
                            return InternalChildren[i].GetBIDBBTEntry(BID);
                    }
                    else if (entry is BBTENTRY)
                    {
                        var cur = entry as BBTENTRY;
                        if (BID == cur.Key)
                            return cur;
                    }
                }
            }
            return null;
        }

        public Tuple<ulong,ulong> GetNIDBID(ulong NID)
        {
            if (Entries.Count == 0)
                return new Tuple<ulong, ulong>(0, 0);
            var isBTEntry = Entries[0] is BTENTRY;
            for (int i = 0; i < Entries.Count; i++)
            {
                if (i == Entries.Count - 1)
                {
                    if (isBTEntry)
                        return InternalChildren[i].GetNIDBID(NID);
                    var cur = Entries[i] as NBTENTRY;
                    if (NID == cur.NID)
                        return new Tuple<ulong, ulong>(cur.BID_Data,cur.BID_SUB);
                    break;
                }

                var curEntry = Entries[i];
                var nextEntry = Entries[i + 1];
                if (isBTEntry)
                {
                    var cur = curEntry as BTENTRY;
                    var next = nextEntry as BTENTRY;
                    if (NID >= cur.Key && NID < next.Key)
                        return InternalChildren[i].GetNIDBID(NID);
                }
                else
                {
                    var cur = curEntry as NBTENTRY;
                    if (NID == cur.NID)
                        return new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB);
                }
            }
            return new Tuple<ulong, ulong>(0, 0);
        }

        public void GetAllNBTEntries(List<NBTENTRY> list)
        {
            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i] is BTENTRY)
                    InternalChildren[i].GetAllNBTEntries(list);
                else if (Entries[i] is NBTENTRY entry)
                    list.Add(entry);
            }
        }

        public void GetAllNIDBIDs(List<Tuple<ulong, ulong>> list)
        {
            if (Entries.Count == 0)
                return;
            var isBTEntry = Entries[0] is BTENTRY;
            for (int i = 0; i < Entries.Count; i++)
            {
                if (isBTEntry)
                {
                    InternalChildren[i].GetAllNIDBIDs(list);
                }
                else
                {
                    var cur = Entries[i] as NBTENTRY;
                    list.Add(new Tuple<ulong, ulong>(cur.BID_Data, cur.BID_SUB));
                }
            }
        }
    }
}
