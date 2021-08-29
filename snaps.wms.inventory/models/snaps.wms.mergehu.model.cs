using System;
using System.Collections.Generic;
using System.Text;

namespace Snaps.WMS {
    public class merge_set {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public int mergeno { get; set; }
        public string hutype { get; set; }
        public string huno { get; set; }
        public string loccode { get; set; }
        public string accncode { get; set; }
    }

    public class merge_find {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string loccode { get; set; }
        public string huno { get; set; }
        public string article { get; set; }
        public string accncode { get; set; }
        public DateTimeOffset? datecreate { get; set; }
    }

    public class merge_md : mergehu_md {
        public List<mergehu_ln> lines { get; set; }
    }

    public class mergehu_md {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public int mergeno { get; set; }
        public string hutype { get; set; }
        public string hutarget { get; set; }
        public string loccode { get; set; }
        public string tflow { get; set; }
        public string tflowdes { get; set; }
        public DateTimeOffset? datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset? datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodify { get; set; }
        public string remarks { get; set; }

    }

    public class mergehu_ln {
        public int mergeln { get; set; }
        public int mergeno { get; set; }
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public decimal stockid { get; set; }
        public string hutype { get; set; }
        public string loccode { get; set; }
        public string huno { get; set; }
        public string inrefno { get; set; }
        public int? inrefln { get; set; }
        public string inagrn { get; set; }
        public string ingrno { get; set; }
        public string article { get; set; }
        public int pv { get; set; }
        public int lv { get; set; }
        public string descalt { get; set; }
        public int qtysku { get; set; }
        public decimal qtypu { get; set; }
        public decimal qtyweight { get; set; }
        public decimal qtyvolume { get; set; }
        public string qtyunit { get; set; }
        public string qtyunitdes { get; set; }
        public DateTimeOffset? daterec { get; set; }
        public string batchno { get; set; }
        public string lotno { get; set; }
        public DateTimeOffset? datemfg { get; set; }
        public string serialno { get; set; }
        public DateTimeOffset? dateexp { get; set; }
        public string tflowops { get; set; }
        public string tflowdes { get; set; }
        public string tflowsign { get; set; }
        public int skuops { get; set; }
        public decimal puops { get; set; }
        public decimal weightops { get; set; }
        public decimal volumeops { get; set; }
        public string unitops { get; set; }
        public string unitopsdes { get; set; }
        public string refops { get; set; }
        public int? reflnops { get; set; }
        public string remarks { get; set; }
        public string tflow { get; set; }
        public DateTimeOffset? datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset? datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodify { get; set; }
        public string msgops { get; set; }
    }
}
