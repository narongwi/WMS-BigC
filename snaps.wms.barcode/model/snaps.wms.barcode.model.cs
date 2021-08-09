using System;

namespace Snaps.WMS.Barcode {
    public class barcode_ls {
        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public String bartype { get; set;}
        public String barops { get; set; } 
        public String barcode { get; set; } 
        public String thname { get; set; } 
        public String thcode { get; set; } 
        public String tflow { get; set; } 
        public String articledsc { get; set; }
        public Int32 isprimary { get; set; } 
    }
    public class barcode_pm : barcode_ls { 
        public string searchall { get; set; }
    }
    public class barcode_ix : barcode_ls { 
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
    }
    public class barcode_md : barcode_ls  {
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public String barremarks { get; set;}
        
    }
}