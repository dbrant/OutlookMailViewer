﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSTParse.NDB
{
    public class PSTBTree
    {
        public BTPage Root;
        public PSTBTree(bool unicode, BREF bref, PSTFile pst)
        {
            using (var viewer = pst.PSTMMF.CreateViewAccessor((long)bref.IB, 512))
            {
                var data = new byte[512];
                viewer.ReadArray(0, data, 0, 512);
                this.Root = new BTPage(unicode, data, bref, pst);
            }
        }
    }
}
