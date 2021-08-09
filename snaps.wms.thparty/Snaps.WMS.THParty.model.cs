using System;

namespace Snaps.WMS.THParty {
    public class thparty_ls {
        public string orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String thtype { get; set; } 
        public String thbutype { get; set; } 
        public String thcode { get; set; } 
        public String thname { get; set; } 
        public String thgroup { get; set; } 
        public String tflow { get; set; } 
        public String thtypename { get; set; } 
        public String thbutypename { get; set; } 
        public String thcodealt { get; set; } 
        public String thnamealt { get; set; } 
        public string indock { get; set; }
        public string oudock { get; set; }
    }
    public class thparty_pm : thparty_ls { 
        public String telephone { get; set; } 
        public String email { get; set; } 
        public String mapaddress { get; set; } 
        public string searchall { get; set; }
    }
    public class thparty_ix : thparty_ls { 

        public string vatcode    { get; set; }
        public string thnameint    { get; set; }
        public string addressln1    { get; set; }
        public string addressln2    { get; set; }
        public string addressln3    { get; set; }
        public string subdistrict    { get; set; }
        public string district    { get; set; }
        public string city    { get; set; }
        public string country    { get; set; }
        public string postcode    { get; set; }
        public string region    { get; set; }
        public string telephone    { get; set; }
        public string email    { get; set; }
        public string thcomment    { get; set; }
        public string throuteformat    { get; set; }
        public Int32 plandelivery    { get; set; }
        public Int32 naturalloss    { get; set; }
        public string mapaddress    { get; set; }
        public string carriercode    { get; set; }
        public string orbitsource    { get; set; }
        public string timeslotday    { get; set; }
        public string timeslothourmin    { get; set; }
        public string timeslothourmax    { get; set; }
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
    }
    public class thparty_md : thparty_ls  {
        public string vatcode    { get; set; }
        public string thnameint    { get; set; }
        public string addressln1    { get; set; }
        public string addressln2    { get; set; }
        public string addressln3    { get; set; }
        public string subdistrict    { get; set; }
        public string district    { get; set; }
        public string city    { get; set; }
        public string country    { get; set; }
        public string postcode    { get; set; }
        public string region    { get; set; }
        public string telephone    { get; set; }
        public string email    { get; set; }
        public string thcomment    { get; set; }
        public string throuteformat    { get; set; }
        public Int32 plandelivery    { get; set; }
        public Int32 naturalloss    { get; set; }
        public string mapaddress    { get; set; }
        public string carriercode    { get; set; }
        public string orbitsource    { get; set; }
        public string timeslotday    { get; set; }
        public string timeslothourmin    { get; set; }
        public string timeslothourmax    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
        public Int32 traveltime { get; set; }
    }

}