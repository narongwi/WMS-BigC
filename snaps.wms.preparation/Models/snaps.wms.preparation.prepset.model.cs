using System;
using System.Collections.Generic;
namespace Snaps.WMS.preparation {
    public class prepset { 
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public string setno                 { get; set; }
        public DateTimeOffset? datestart    { get; set; }
        public DateTimeOffset? datefinish   { get; set; }
        public TimeSpan opsperform          { get; set; }
        public Int32 opsorder               { get; set; }
        public DateTimeOffset? datecreate   { get; set; }
        public string accncreate            { get; set; }
        public string procmodify            { get; set; }
        public List<prepsln> orders         { get; set; }
        public string slcouorder            { get; set; }
        public string hucode                { get; set; }
        public List<outbound_ls> distb      { get; set; }
    }
    public class prepsln { 
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public string routeno               { get; set; }
        public string thcode                { get; set; }
        public string ouorder               { get; set; }
        public string ouln                  { get; set; }
        public string przone                { get; set; }
        public string barcode               { get; set; }
        public string article               { get; set; }
        public Int32 pv                     { get; set; }
        public Int32 lv                     { get; set; }
        public Int32 unitprep               { get; set; }
        public Int32 qtyskuorder            { get; set; }
        public Int32 qtypuorder             { get; set; }
        public Decimal qtyweightorder       { get; set; }
        public Decimal qtyvolumeorder       { get; set; }
        public Int32 qtyskuops              { get; set; }
        public Int32 qtypuops               { get; set; }
        public Decimal qtyweightops         { get; set; }
        public Decimal qtyvolumeops         { get; set; }
        public string batchno               { get; set; }
        public string lotno                 { get; set; }
        public DateTimeOffset? datemfg      { get; set; }
        public DateTimeOffset? dateexp      { get; set; }
        public string serialno              { get; set; }
        public string tflow                 { get; set; }
        public string errmsg                { get; set; }

        public string description           { get; set; }
        public string loccode               { get; set; }
        public Int32 rtoskuofpu             { get; set; }
        public string ourefno               { get; set; }
        public string ourefln               { get; set; }

        public string dishuno               { get; set; }
        public Int32 disstockid             { get; set; }

        public decimal skuweight            { get; set; }
        public decimal skuvolume            { get; set; }

    }

    public class ouselect {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string ouorder { get; set; }
        public string outype { get; set; }
        public string ousubtype { get; set; }
        public string thcode { get; set; }
        public int selected { get; set; }
        public DateTime? selectdate { get; set; }
        public string selectaccn { get; set; }
        public string selectflow { get; set; }
    }
}