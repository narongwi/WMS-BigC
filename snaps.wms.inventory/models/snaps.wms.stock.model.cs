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

    // Stock 
    public class stock_ls {
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String article { get; set; }
        public Int32 pv { get; set; }
        public Int32 lv { get; set ;}
        public String barcode { get; set ;}
        public String description { get; set; } 
        public Decimal cronhand { get; set; }
        public Decimal cronhandpu { get; set; }
        public String tflow { get; set; } 
        public string unitmanage { get; set; }
        public Int32 unitratio { get; set; }

        public string huno      { get; set; } 
        public string loccode   { get; set; }
        public string inrefno   { get; set; }
        public DateTime? dateexp { get; set; }
        public string serialno  { get; set; }
        public string isblock   { get; set; }


        public string thcode { get; set; }
        public Decimal crweight { get; set; }
        public Decimal crvolume { get; set; }

        public Int32 dlcall { get; set; }
        public Int32 dlcfactory { get; set; }
        public Int32 dlcwarehouse { get; set; }


    } 
    public class stock_pm : stock_ls { 
        public String stockid { get; set; } 
        public Int32 inrefln { get; set; } 
        public String batchno { get; set; }
        public DateTime? daterec { get; set; }
        public DateTime? datemfg { get; set; } 
        public String stkremarks { get; set; } 
        public string hdivision { get; set; }
        public string hdepartment { get; set; }
        public string hsubdepart { get; set; }
        public string hclass { get; set; }
        public Int32 isfastmove { get; set; }
        public Int32 ishighvalue { get; set; }
        public Int32 isslowmove { get; set; }
        public Int32 isdangerous { get; set;}
        public Int32 isdlc { get; set; }
        public Int32 isunique { get; set; }
        public Int32 isprescription { get; set; }
        public Int32 isalcohol { get; set; }

        public string searchall { get; set; }
    }
    public class stock_ix : stock_ls { 
    }
    public class stock_md : stock_ls  {
        public Decimal stockid { get; set;}
        public Decimal qtysku { get; set; }
        public Decimal qtypu { get; set; }

        public String hutype { get; set; } 
        public String huno { get; set; } 
        public String hunosource { get; set; } 
        public String thcode { get; set; } 
        public String inrefno { get; set; } 
        public String inrefln { get; set; } 
        public String loccode { get; set; } 
        public Decimal qtyweight { get; set; } 
        public Decimal qtyvolume { get; set; } 
        public DateTimeOffset? daterec { get; set; } 
        public String batchno { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? datemfg { get; set; } 
        public DateTimeOffset? dateexp { get; set; } 
        public String serialno { get; set; } 
        public String stkremarks { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; }
        public String procmodify { get; set;}
        
        public DateTimeOffset? datemodify { get; set; } 
        public String tflowsign { get; set; }

        public string inagrn { get; set; }
        public string ingrno { get; set; }
    }

    //Info
    public class stock_info : stock_ls{
        public string thname { get; set ;} 
        

        public String tihi{ get; set; }
        public Decimal skuweight { get; set; }
        public String dimension { get; set; }
        public List<stock_md> lines { get; set; }

        public Decimal crincoming { get; set; }
        public Decimal cravailable { get; set; }
        public Decimal crbulknrtn { get; set; }
        public Decimal croverflow { get; set; }
        public Decimal crplanship { get; set ;}
        public Decimal crprep { get; set; }
        public Decimal crstaging { get; set; }
        public Decimal crtask { get; set; } 
        public Decimal crreturn { get; set; }
        public Decimal crsinbin { get; set; }
        public Decimal crdamage { get; set; }
        public Decimal crblock { get; set; } 
        public Decimal crexchange { get; set; }
        public Decimal crrtv { get; set; }

        public Int32 isdlc { get; set; }
        public Int32 isunique { get; set; }
        public Int32 isbatchno { get; set; }
    }
    
    // Movement
    public class stock_mvin { 
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public Decimal stockid { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; }
        public String opsunit { get; set; }
        public Int32 opssku { get; set; } 
        public Decimal opspu { get; set; }
        public Decimal opsweight { get; set; } 
        public Decimal opsvolume { get; set ;}
        public Decimal opsnaturalloss { get; set; }
        public DateTimeOffset? opsdate { get; set; } 
        public String opstype { get; set; } 
        public String opscode { get; set; } 
        public String opsroute { get; set; } 
        public String opsthcode { get; set; } 
        public String stockhash { get; set; }
        public String opsaccn { get; set; }
        public String opshuno { get; set;}
        public String opsloccode { get; set; }
        public String opsrefno { get; set; }
        public String procmodify { get; set; }
        public String inagrn { get; set; }
        public String ingrno { get; set; }
        public DateTimeOffset? daterec { get; set; }
        public DateTimeOffset? dateexp { get; set; }
        public DateTimeOffset? datemfg { get; set; }
        public String batchno { get; set; }
        public String lotno { get; set; }
        public String serialno { get; set; }
        public String hutype { get; set; }
        public String opshusource { get; set; }
        public String opsreftype { get; set ;}
        public string thcode { get; set; }
        public string newhu { get; set; }
        public string tflow { get; set; }

        public string stocktflow { get; set; }
        public Decimal prepstockid { get; set; } // use for collection old stock id before split quantity on preparation end process.
    }

    public class stock_mvou {
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public Decimal stockid { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; }
        public String opsunit { get; set; }
        public Int32 opssku { get; set; } 
        public Decimal opspu { get; set; }
        public Decimal opsweight { get; set; } 
        public Decimal opsvolume { get; set ;}
        public Decimal opsnaturalloss { get; set; }
        public DateTimeOffset? opsdate { get; set; } 
        public String opstype { get; set; } 
        public String opscode { get; set; } 
        public String opsroute { get; set; } 
        public String opsthcode { get; set; } 
        public String stockhash { get; set; }
        public String opsaccn { get; set; }
        public String opshuno { get; set;}
        public String opsloccode { get; set; }
        public String opsrefno { get; set; }
        public String procmodify { get; set; }

        public DateTimeOffset? daterec { get; set; }
        public DateTimeOffset? dateexp { get; set; }
        public DateTimeOffset? datemfg { get; set; }
        public String batchno { get; set; }
        public String lotno { get; set; }
        public String serialno { get; set; }
        public String hutype { get; set; }        
        public String opshusource { get; set; }

        public String inagrn { get; set; }
        public String ingrno { get; set; }
        
        public String opsreftype { get; set ;}
    }

}