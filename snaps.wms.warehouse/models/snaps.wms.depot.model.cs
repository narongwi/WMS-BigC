using System;
namespace Snaps.WMS.warehouse { 
    public class depot_ls {
        public String orgcode { get; set; } 
        public String sitecode { get; set; } 
        public String depottype { get; set; } 
        public String depotcode { get; set; } 
        public String depotname { get; set; } 
        public String tflow { get; set; } 
    }
    public class depot_pm : depot_ls { 
    }
    public class depot_ix : depot_ls { 
    }
    public class depot_md   {
        public string orgcode    { get; set; }
        public string sitecode    { get; set; }
        public string depottype    { get; set; }
        public string depotcode    { get; set; }
        public string depotname    { get; set; }
        public string depotnamealt    { get; set; }
        public DateTimeOffset? datestart    { get; set; }
        public DateTimeOffset? dateend    { get; set; }
        public string depotkey    { get; set; }
        public string depotops    { get; set; }
        public string tflow    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
        public string depothash    { get; set; }
        public string unitweight    { get; set; }
        public string unitvolume    { get; set; }
        public string unitdimension    { get; set; }
        public string unitcubic    { get; set; }
        public string formatdate    { get; set; }
        public string formatdateshort    { get; set; }
        public string formatdatelong    { get; set; }
        public string authcode    { get; set; }
        public string authdigit    { get; set; }
        public DateTimeOffset? authdate    { get; set; }
        
    }

}