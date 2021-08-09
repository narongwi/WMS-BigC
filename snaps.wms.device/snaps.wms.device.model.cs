using System;

namespace Snaps.WMS.Device {
    public class admdevice_ls {
        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String devtype { get; set; } 
        public Int32 devid { get; set; }
        public String devcode { get; set; } 
        public String devmodel { get; set; } 
        public DateTimeOffset? datelastactive { get; set; } 
        public Int32 isreceipt { get; set; } 
        public Int32 istaskptw { get; set; } 
        public Int32 istasktrf { get; set; } 
        public Int32 istaskload { get; set; } 
        public Int32 istaskrpn { get; set; } 
        public Int32 istaskgen { get; set; } 
        public Int32 ispick { get; set; } 
        public Int32 isdistribute { get; set; } 
        public Int32 isforward { get; set; } 
        public Int32 iscount { get; set; } 
        public String tflow { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
    }
    public class admdevice_pm : admdevice_ls { 

    }
    public class admdevice_ix : admdevice_ls { 
    }
    public class admdevice_md : admdevice_ls  {
        public String devserial { get; set; } 
        public Decimal opsmaxheight { get; set; } 
        public Decimal opsmaxweight { get; set; } 
        public Decimal opsmaxvolume { get; set; } 
        public String devhash { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public String devremarks { get; set; }
    }

}