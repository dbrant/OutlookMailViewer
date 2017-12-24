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

        public List<BTPAGEENTRY> Entries;

        public List<BTPage> InternalChildren = new List<BTPage>();

        public bool IsNode { get { return this._trailer.PageType == PageType.NBT; } }
        public bool IsBlock { get { return this._trailer.PageType == PageType.BBT; } }

        public ulong BID { get { return this._trailer.BID; } }

        public BTPage(bool unicode, byte[] pageData, BREF _ref, PSTFile pst)
        {
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

            this.Entries = new List<BTPAGEENTRY>();
            for (var i = 0; i < this._numEntries; i++)
            {
                var curEntryBytes = pageData.RangeSubset(i*this._cbEnt, this._cbEnt);
                if (this._cLevel == 0)
                {
                    if (this._trailer.PageType == PageType.NBT)
                        this.Entries.Add(new NBTENTRY(unicode, curEntryBytes));
                    else
                        this.Entries.Add(new BBTENTRY(unicode, curEntryBytes));
                }
                else
                {
                    //btentries
                    var entry = new BTENTRY(unicode, curEntryBytes);
                    this.Entries.Add(entry);
                    using (var view = pst.PSTMMF.CreateViewAccessor((long)entry.BREF.IB,512))
                    {
                        var bytes = new byte[512];
                        view.ReadArray(0, bytes, 0, 512);
                        this.InternalChildren.Add(new BTPage(unicode, bytes, entry.BREF, pst));
                    }
                }
            }
        }

        public BBTENTRY GetBIDBBTEntry(ulong BID)
        {
            int ii = 0;
            if (BID % 2 == 1)
                ii++;
            BID = BID & 0xfffffffffffffffe;
            for (int i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];
                if (i == this.Entries.Count - 1)
                {

                    if (entry is BTENTRY)
                        return this.InternalChildren[i].GetBIDBBTEntry(BID);
                    else
                    {
                        var temp = entry as BBTENTRY;
                        if (BID == temp.Key)
                            return temp;
                    }

                }
                else
                {
                    var entry2 = this.Entries[i + 1];
                    if (entry is BTENTRY)
                    {
                        var cur = entry as BTENTRY;
                        var next = entry2 as BTENTRY;
                        if (BID >= cur.Key && BID < next.Key)
                            return this.InternalChildren[i].GetBIDBBTEntry(BID);
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
            var isBTEntry = this.Entries[0] is BTENTRY;
            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (i == this.Entries.Count - 1)
                {
                    if (isBTEntry)
                        return this.InternalChildren[i].GetNIDBID(NID);
                    var cur = this.Entries[i] as NBTENTRY;
                    return new Tuple<ulong, ulong>(cur.BID_Data,cur.BID_SUB);
                }

                var curEntry = this.Entries[i];
                var nextEntry = this.Entries[i + 1];
                if (isBTEntry)
                {
                    var cur = curEntry as BTENTRY;
                    var next = nextEntry as BTENTRY;
                    if (NID >= cur.Key && NID < next.Key)
                        return this.InternalChildren[i].GetNIDBID(NID);
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
    }
}
