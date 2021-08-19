using System;
using System.Collections.Generic;
using System.Text;

namespace Snaps.WMS.Inventory.models {
    public class merge_gn {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public int mergeno { get; set; }
        public string hutype { get; set; }
        public string huno { get; set; }
    }
    public class merge_md {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public int mergeno { get; set; }
        public string hutype { get; set; }
        public string husource { get; set; }
        public string inrefno { get; set; }
        public int inrefln { get; set; }
        public string loccode { get; set; }
        public string article { get; set; }
        public int pv { get; set; }
        public int lv { get; set; }
        public int qtysku { get; set; }
        public decimal qtypu { get; set; }
        public decimal qtyweight { get; set; }
        public decimal qtyvolume { get; set; }
        public string qtyunit { get; set; }
        public DateTimeOffset daterec { get; set; }
        public string batchno { get; set; }
        public string lotno { get; set; }
        public DateTimeOffset datemfg { get; set; }
        public DateTimeOffset dateexp { get; set; }
        public string serialno { get; set; }
        public string remarks { get; set; }
        public string tflow { get; set; }
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
        public DateTimeOffset datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodify { get; set; }
        public string remarks { get; set; }
    }

    public class mergehu_ln {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public int mergeln { get; set; }
        public int mergeno { get; set; }
        public string hutype { get; set; }
        public string husource { get; set; }
        public string inrefno { get; set; }
        public int inrefln { get; set; }
        public string loccode { get; set; }
        public string article { get; set; }
        public int pv { get; set; }
        public int lv { get; set; }
        public int qtysku { get; set; }
        public decimal qtypu { get; set; }
        public decimal qtyweight { get; set; }
        public decimal qtyvolume { get; set; }
        public string qtyunit { get; set; }
        public DateTimeOffset daterec { get; set; }
        public string batchno { get; set; }
        public string lotno { get; set; }
        public DateTimeOffset datemfg { get; set; }
        public DateTimeOffset dateexp { get; set; }
        public string serialno { get; set; }
        public string remarks { get; set; }
        public string tflow { get; set; }
        public DateTimeOffset datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodify { get; set; }
    }
}
