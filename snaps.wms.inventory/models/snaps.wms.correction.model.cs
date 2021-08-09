using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public class correction_ls { 
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
        public String codeops { get; set; } 
        public String typeops { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public String unitops { get; set; } 
        public decimal qtypu { get; set; } 
        public String tflow { get; set; } 
        public String description { get; set; } 
        public String accnops { get; set; } 
        public string accnname { get; set ;}
    }

    public class correction_pm : correction_ls { 

    }
    public class correction_ix : correction_ls { 

    }

    public class correction_md : correction_ls { 
        public String seqops { get; set; } 

        public String thcode { get; set; } 
        public Int32 qtysku { get; set; } 
        public Decimal qtyweight { get; set; } 
        public Decimal qtyvolume { get; set; } 
        public String inreftype { get; set; } 
        public String inrefno { get; set; } 
        public String ingrno { get; set; } 
        public String inpromo { get; set; } 
        public String reason { get; set; } 
        public Int32 stockid { get; set; } 
        public String huno { get; set; } 
        public String loccode { get; set; } 
        public DateTimeOffset? daterec { get; set; } 
        public String batchno { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? datemfg { get; set; } 
        public DateTimeOffset? dateexp { get; set; } 
        public String serialno { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String procmodify { get; set; } 

        public Int32 variancesku { get; set; }
        public Decimal variancepu { get; set ;}
        public Decimal stockbeforepu { get; set ;}
        public Int32 stockbeforesku { get; set; }
        public Int32 aftersku { get; set ;}
        public Decimal afterpu { get; set; }

        public string inagrn { get; set; }
        public string orbitsource { get; set; } 
        public string orbitsite { get; set; } 
        public string orbitdepot { get; set; }

        public string stocktflow { get; set; }

    }

    public class transfer_md : correction_md { 
        public string rsltaskno { get; set; }
        public string rslhuno { get; set; }
        public Decimal rslstockid { get; set ;}

        public string sourcelocation { get; set ;}
        public string targetlocation { get; set; }
        public string targethuno { get; set; }

    }

}