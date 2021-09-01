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


    public class counttask_md { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string counttype    { get; set; }
        public string countcode    { get; set; }
        public string countname    { get; set; }
        public DateTimeOffset? datestart    { get; set; }
        public DateTimeOffset? dateend    { get; set; }
        public Int32 isblock    { get; set; }
        public string remarks    { get; set; }
        public string tflow    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
    }

    public class countplan_md { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string countcode    { get; set; }
        public string plancode    { get; set; }
        public string planname    { get; set; }
        public string accnassign    { get; set; }
        public string accnwork    { get; set; }
        public string szone    { get; set; }
        public string ezone    { get; set; }
        public string saisle    { get; set; }
        public string eaisle    { get; set; }
        public string sbay    { get; set; }
        public string ebay    { get; set; }
        public string slevel    { get; set; }
        public string elevel    { get; set; }
        public Int32 isroaming    { get; set; }
        public string tflow    { get; set; }
        public Int32 cntpercentage    { get; set; }
        public Int32 cnterror    { get; set; }
        public Int32 cntlines    { get; set; }
        public TimeSpan cnttime    { get; set; }
        public DateTimeOffset? datestart    { get; set; }
        public Int32 pctstart    { get; set; }
        public DateTimeOffset? datevld    { get; set; }
        public Int32 pctvld    { get; set; }
        public string accnvld    { get; set; }
        public string devicevld    { get; set; }
        public string remarksvld    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }

        public Int32 isblock { get; set; }
        public Int32 isdatemfg { get; set; }
        public Int32 isdateexp { get; set; }
        public Int32 isbatchno { get; set; }
        public Int32 isserialno { get; set; }
        public Int32 allowscanhu { get; set; }
        public Int32 isoddeven { get; set; }
        public string planorigin { get; set; }
    }

    public class countline_md {
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string countcode    { get; set; }
        public string plancode    { get; set; }
        public string loccode    { get; set; }
        public Int32 locseq    { get; set; }
        public string unitcount    { get; set; }
        public string stbarcode    { get; set; }
        public string starticle    { get; set; }
        public Int32 stpv    { get; set; }
        public Int32 stlv    { get; set; }
        public Int32 stqtysku    { get; set; }
        public Int32 stqtypu    { get; set; }
        public string stlotmfg    { get; set; }
        public DateTimeOffset? stdatemfg    { get; set; }
        public DateTimeOffset? stdateexp    { get; set; }
        public string stserialno    { get; set; }
        public string sthuno    { get; set; }
        public string cnbarcode    { get; set; }
        public string cnarticle    { get; set; }
        public Int32 cnpv    { get; set; }
        public Int32 cnlv    { get; set; }
        public Int32 cnqtysku    { get; set; }
        public Int32 cnqtypu    { get; set; }
        public string cnlotmfg    { get; set; }
        public DateTimeOffset? cndatemfg    { get; set; }
        public DateTimeOffset? cndateexp    { get; set; }
        public string cnserialno    { get; set; }
        public string cnhuno    { get; set; }
        public string cnflow    { get; set; }
        public string cnmsg    { get; set; }
        public Int32 isskip    { get; set; }
        public Int32 isrgen    { get; set; }
        public Int32 iswrgln    { get; set; }
        public string countdevice    { get; set; }
        public DateTimeOffset? countdate    { get; set; }
        public string corcode    { get; set; }
        public Int32 corqty    { get; set; }
        public string coraccn    { get; set; }
        public string cordevice    { get; set; }
        public DateTimeOffset? cordate    { get; set; }
        public string tflow    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
        public string productdesc { get; set; }
        public string locctype { get; set; }
    }

    public class findcountline_md
    {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string countcode { get; set; }
        public string plancode { get; set; }
        public string loccode { get; set; }
        public string tflow    { get; set; }
    }

    public class countcorrection_md {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string counttype { get; set; }
        public string countcode { get; set; }
        public string countname { get; set; }
        public string barcode { get; set; }
        public string article { get; set; }
        public Int32 pv { get; set; }
        public Int32 lv { get; set; }
        public string description { get; set; }
        public string corcode { get; set; }
        public Int32 corqty { get; set; }
        public Int32 corsku { get; set; }
        public string unitcount { get; set; }
        public Int32 skuofpck { get; set; }
        public double skuweight { get; set; }
        public double skuvolume { get; set; }
        public int unitmanage { get; set; }
        public int unitprep { get; set; }
        public string taskstate { get; set; }
    }
    public class confirmline_md
    {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string counttype { get; set; }
        public string countcode { get; set; }
        public string countname { get; set; }
        public DateTimeOffset? datestart { get; set; }
        public DateTimeOffset? dateend  {   get; set;   }
        public string plancode { get; set; }
        public int isdatemfg { get; set; }
        public int isdatexp { get; set; }
        public int isbatchno { get; set; }
        public int isserailno { get; set; }
        public string loccode { get; set; }
        public string huno { get; set; }
        public Int32 pctvld { get; set; }
        public Int32 locseq { get; set; }
        public string unitcount { get; set; }
        public string barcode { get; set; }
        public string article { get; set; }
        public Int32 pv { get; set; }
        public Int32 lv { get; set; }
        public string description { get; set; }
        public Int32 stqtysku { get; set; }
        public Int32 stqtypu { get; set; }
        public Int32 cnqtypu { get; set; }
        public string corcode { get; set; }
        public Int32 corqty { get; set; }
        public Int32 corsku { get; set; }
        public Int32 skuofpck { get; set; }
        public double skuweight { get; set; }
        public double skuvolume { get; set; }
        public int unitmanage { get; set; }
        public int unitprep { get; set; }
        public string taskstate { get; set; }
        public string mfglot { get; set; }
        public string serial { get; set; }
        public DateTimeOffset? datemfg { get; set; }
        public DateTimeOffset? dateexp { get; set; }
        public string cnflow { get; set; }
        public string cnmsg { get; set; }
        public string locctype { get; set; }
    }

    public class createhu_md {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string loccode { get; set; }
        public string article { get; set; }
        public string pv { get; set; }
        public string lv { get; set; }
        public decimal qtypu { get; set; }
        public string qtyunit { get; set; }
        public string countcode { get; set; }
        public string plancode { get; set; }
        public string accncode { get; set; }
        public string remarks { get; set; }
        public string mergeno { get; set; }
        public string huno { get; set; }
    }
    public class product_vld {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string barcode { get; set; }
        public string article { get; set; }
        public int pv { get; set; }
        public int lv { get; set; }
        public string descalt { get; set; }
        public decimal? skuweight { get; set; }
        public decimal? skuvolume { get; set; }
        public string unitmanage { get; set; }
        public string loccode { get; set; }
        public string locarea { get; set; }
        public string loctype { get; set; }
        public string locunit { get; set; }
        public string huno { get; set; }
        public string unitcount { get; set; }
        public string unitdestr { get; set; }
        public int? skuofunit { get; set; }
        public string countcode { get; set; }
        public string plancode { get; set; }
        public string linecode { get; set; }
        public int qtycount { get; set; }
        public string accncode { get; set; }
        public bool isnewhu { get; set; }
        public string lotmfg { get; set; }
        public DateTimeOffset? datemfg { get; set; }
        public DateTimeOffset? dateexp { get; set; }
        public string serialno { get; set; }
        public int? rtoskuofpu { get; set; }
        public int? rtopckoflayer { get; set; }
        public int? rtolayerofhu { get; set; }
        public int? rtopckofpallet { get; set; }
        public int? rtoskuofipck { get; set; }
        public int? rtoskuofpck { get; set; }
        public int? rtoskuoflayer { get; set; }
        public int? rtoskuofhu { get; set; }
    }
}