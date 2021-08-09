using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using System.Runtime.CompilerServices;

namespace Snaps.WMS {
    
    public class route_thsum { 
        public String orgcode { get; set ;} 
        public String site { get; set; } 
        public String depot { get; set; } 
        public string thcode { get; set; }
        public string thname { get; set; }
        public Int32 crroute { get; set; }
        public Int32 crhu { get; set; }
        public decimal crweight { get; set; }
        public decimal crvolume { get; set; }
        public decimal crcapacity { get; set; }
        public decimal crophu { get; set; }

    }
    public class route_ls {
        public String orgcode { get; set ;} 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String routetype { get; set; } 
        public String routeno { get; set; } 
        public String oupromo { get; set; } 
        public String thcode { get; set; } 
        public DateTimeOffset? plandate { get; set; } 
        public String utlzcode { get; set; } 
        public String tflow { get; set; } 
        public String driver { get; set; } 
        public String thname { get; set; } 
        public String routetypename { get; set; } 
        public Int32 oupriority { get; set; } 
        public DateTimeOffset? datereqdelivery { get; set; } 
        public Int32 crhu { get; set; } 
        public Decimal crweight { get; set; } 
        public Decimal crvolume { get; set; } 
        public decimal crophu { get; set; }
        public string remarks { get; set; }
        public decimal crcapacity { get; set; }
    }
    public class route_pm  { 
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string routetype { get; set; }
        public string routeno { get; set; }
        public string oupromo { get; set; }
        public string thcode { get; set; }
        public DateTimeOffset? plandate { get; set; }
        public string utlzcode { get; set; } 
        public string tflow { get; set; }
        public string driver { get; set; }
        public string thname { get; set; }
        public string routetypename { get; set; }
        public string oupriority { get; set; }
        public DateTimeOffset? datereqfrom { get; set; }
        public DateTimeOffset? datereqto { get; set; }
        public string remarks { get; set; }
        public DateTimeOffset? datedeliveryfrom { get; set; }
        public DateTimeOffset? datedeliveryto { get; set; }
        public string staging { get; set; }
        public string trttype { get; set; }
        public string transportor { get; set; }
        public string loadtype { get; set; }
        public string paymenttype { get; set; }
        public string trucktype { get; set; }

        public string iscombine { get; set; }
    }
    public class route_ix : route_ls { 
    }
    public class route_md : route_ls  {
        public String routename { get; set; } 
        public String trttype { get; set; } 
        public String loadtype { get; set; } 
        public String trucktype { get; set; } 
        public String trtmode { get; set; } 
        public String loccode { get; set; } 
        public String paytype { get; set; } 
        public DateTimeOffset? loaddate { get; set; } 
        public DateTimeOffset? dateshipment { get; set; }
        public String relocationto { get; set; } 
        public String relocationtask { get; set; } 
        public String shipper { get; set; } 
        public Int32 mxhu { get; set; } 
        public Decimal mxweight { get; set; } 
        public Decimal mxvolume { get; set; } 

        public String plateNo { get; set; } 
        public String contactno { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; }
        public List<route_hu> hus { get; set; }
        public List<route_hu> allocate { get; set; }
        public string transportor { get; set ;}

        public string outrno { get; set; }

        public string sealno { get; set; }
        public string routesource { get; set; }

    }
    public class route_prc { 
        public String opsaccn { get; set; }
        public route_md opsobj { get; set; } 
    }
    public class route_hu { 
        public String huno { get; set ;} 
        public String loccode { get; set ;} 
        public String worker { get; set; } 
        public String tflow { get; set; }
        public String routeno { get; set; }
        public Int32 crsku { get; set; } 
        public Int32 priority { get; set; }
        public Decimal crweight { get; set; } 
        public Decimal crvolume { get; set; } 
        public Decimal crcapacity { get; set; } 
        public String accnmodify {get; set;}
        public string opstype { get; set; }
        public string opscode { get; set; }
        public string spcarea { get; set ;}
    }
}