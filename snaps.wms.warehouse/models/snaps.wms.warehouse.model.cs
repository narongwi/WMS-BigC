using System;
namespace Snaps.WMS.warehouse { 
    public class warehouse_ls {
        public string orgcode { get; set; } 
        public string sitecode { get; set; } 
        public string sitename { get; set; } 
        public string sitetype { get; set; } 
        public string sitehash { get; set; }
        public DateTimeOffset? datestart { get; set; } 
        public DateTimeOffset? dateend { get; set; } 
        public string tflow { get; set; } 
    }
    public class warehouse_pm : warehouse_ls { 
        public String sitenamealt { get; set; } 
    }
    public class warehouse_ix : warehouse_ls { 

    }
    public class warehouse_md : warehouse_ls  {
        public String sitenamealt { get; set; } 
        public String sitekey { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
    }

}