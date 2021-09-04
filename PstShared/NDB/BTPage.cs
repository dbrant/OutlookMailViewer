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
                    using (var view = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        InternalChildren.Add(new BTPage(unicode, bytes, entry.BREF, pst));
                    }
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
            var isBTEntry = Entries[0] is BTENTRY;
            for (int i = 0; i < Entries.Count; i++)
            {
                if (i == Entries.Count - 1)
                {
                    if (isBTEntry)
                        return InternalChildren[i].GetNIDBID(NID);
                    var cur = Entries[i] as NBTENTRY;
                    return new Tuple<ulong, ulong>(cur.BID_Data,cur.BID_SUB);
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

        public void GetAllNIDBIDs(List<Tuple<ulong, ulong>> list)
        {
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
