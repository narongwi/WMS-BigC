 using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.preparation {
    public class prep_ls {
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String routeno { get; set; } 
        public String huno { get; set; } 
        public String preptype { get; set; } 
        public String prepno { get; set; } 
        public DateTimeOffset? prepdate { get; set; } 
        public Int32? priority { get; set; } 
        public String thcode { get; set; } 
        public String spcorder { get; set; } 
        public String spcarticle { get; set; } 
        public String tflow { get; set; } 
        public Decimal capacity { get; set;}
        public String thname { get; set; }
        public String picker { get; set; }
        public Decimal preppct { get; set; }
        public string preptypename { get; set; }
        public string przone { get; set; }

    }
    public class prep_pm : prep_ls { 
        public DateTime? dateassign { get; set; } 
        public String deviceID { get; set; }

        public DateTimeOffset? dateprepfrom { get; set; }
        public DateTimeOffset? dateprepto { get; set; }
        public DateTimeOffset? dateorderfrom { get; set; }
        public DateTimeOffset? dateorderto { get; set; }
        public string ouflag { get; set; }
        public string ouorder { get; set; }
        public string article { get; set; }
        public string setno { get; set ;}
    }
    public class prep_ix : prep_ls { 
    }
    public class prep_md : prep_ls  {
        public DateTimeOffset? dateassign { get; set; } 
        public DateTimeOffset? datestart { get; set; } 
        public DateTimeOffset? dateend { get; set; } 
        public String deviceID { get; set; } 
        //public String picker { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        //public String przone { get; set;}
        public List<prln_md> lines { get; set; } 
        public string setno { get; set; }
        public string hutype { get; set; }

        public decimal mxweight { get; set; }
        public decimal crweight { get; set; }
        public decimal mxvolume { get; set; }
        public decimal crvolume { get; set; }
        public Int32 mxsku { get; set; }
        public Int32 crsku { get; set; }
        public string loccode { get; set; }
        public bool prepstate { get; set; }
    }

    public class prep_stock { 
        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public String loccode { get; set; } 
        public DateTimeOffset? datemfg { get; set; } 
        public DateTimeOffset? dateexp { get; set; } 
        public String batchno { get; set; } 
        public String lotno { get; set; } 
        public Decimal stockid { get; set; }
        public String huno { get; set; }
        public Int32 qtysku { get; set; }
        public Int32 qtypu { get; set; }
        public String serialno { get; set; }
        public String spcarea { get; set; }
        public decimal qtyweight { get; set; }
        public decimal qtyvolume { get; set; }
    }
   
    public class prep_stockops {
        public prep_stockops() {
        }

        public prep_stockops(string orgcode,string site,string depot,string stockid,string article,string pv,string lv,
            string setno,string ouorder,string huno,string loccode,int skuqty,int ordqty) {
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.stockid = stockid;
            this.article = article;
            this.pv = pv;
            this.lv = lv;
            this.skuqty = skuqty;
            this.setno = setno;
            this.ouorder = ouorder;
            this.huno = huno;
            this.loccode = loccode;
            this.ordqty = ordqty;
        }

        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string stockid { get; set; }
        public string huno { get; set; }
        public string loccode { get; set; }
        public string setno { get; set; }
        public string ouorder { get; set; }
        public string article { get; set; }
        public string pv { get; set; }
        public string lv { get; set; }
        public int skuqty { get; set; }
        public int ordqty { get; set; }

    }

    public class prln_ls {
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String article { get; set; } 
        public String description { get; set; } 
        public Int32 qtyskuorder { get; set; } 
        public Int32 qtypuorder { get; set; } 
        public Int32 qtypuops { get; set; } 
        public String tflow { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public string unitprep { get; set; } 
        public String unitname { get; set; } 
        public Int32 qtyskuops { get; set; } 
        public String prepno { get; set; } 
        public Int32 prepln { get; set; }
        public string preptypeops { get; set; }
        public int? preplineops { get; set; }
    }
    public class prln_pm : prln_ls { }
    public class prln_ix : prln_ls { 
    }
    public class prln_product { 
        public string article { get; set; }
        public Int32 pv { get; set; }
        public Int32 lv { get; set; }
    }
    public class prln_md : prln_ls  {
        public String huno { get; set; } 
        public String hunosource { get; set; } 
        public String loczone { get; set; } 
        public String loccode { get; set; } 
        public Int32 locseq { get; set; } 
        public String locdigit { get; set; } 
        public String ouorder { get; set; } 
        public string ouln { get; set; } 
        public String inorder { get; set; } 
        public Int32 inln { get; set ;} 
        public String barcode { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public Decimal stockid { get; set; } 
        public Decimal qtyweightorder { get; set; } 
        public Decimal qtyvolumeorder { get; set; } 
        public Decimal qtyweightops { get; set; } 
        public Decimal qtyvolumeops { get; set; } 
        public String batchno { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? datemfg { get; set; } 
        public DateTimeOffset? dateexp { get; set; } 
        public String serialno { get; set; } 
        public String picker { get; set; } 
        public DateTimeOffset? datepick { get; set; } 
        public String devicecode { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public Int32 rtoskuofpu { get; set; }
        public string thcode { get; set; }
        public string taskno { get; set; }

        public DateTimeOffset? daterec { get; set; } 
        public string inagrn { get; set; }
        public string ingrno { get; set; }
        public string skipdigit { get; set; }

    }

    public class prsplithu_md {
        public prsplithu_md() {
        }
        public prsplithu_md(int pickskuqty,decimal pickweight,decimal pickvolume,bool isGenerate) {
            this.pickskuqty = pickskuqty;
            this.pickweight = pickweight;
            this.pickvolume = pickvolume;
            IsGenerate = isGenerate;
        }

        public int pickskuqty { get; set; }
        public decimal pickweight { get; set; }
        public decimal pickvolume { get; set; }
        public bool IsGenerate { get; set; }
    }


    public class  procdist_order { 
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String article { get; set; }
        public Int32 pv { get; set ;}
        public Int32 lv { get ;set ;}
        public String inorder { get; set;}
        public String thcode { get; set; } 
        public String ouorder { get; set; }
        public Int32 ouln { get; set; } 
        public Int32 unitops { get; set; }
        public Int32 qtysku { get; set; } 
        public Int32 qtypu { get; set; }
        public Int32 qtyskuprep { get; set ;} 
        public Int32 qtypuprep { get; set; }
        public String loccode { get; set ;}
    }
    public class procdist_stock { 
        public String orgcode               { get; set; } 
        public String site                  { get; set; } 
        public String depot                 { get; set; } 
        public String spcarea               { get; set; } 
        public Decimal stockid              { get; set; } 
        public String huno                  { get; set; }
        public String article               { get; set; }
        public Int32 pv                     { get; set; } 
        public Int32 lv                     { get; set; }
        public String loccode               { get; set; } 
        public Int32 qtysku                 { get; set; } 
        public Int32 qtypu                  { get; set; }
        public String batchno               { get; set; }
        public String lotno                 { get; set; } 
        public String serialno              { get; set; } 
        public DateTimeOffset? datemfg      { get; set; }
        public DateTimeOffset? dateexp      { get; set; }
        public Int32 qtyskuoh               { get; set; } 
        public Int32 qtypuoh                { get; set; }
        public String barcode               { get; set; }
        public String prepno                { get; set; }
    }

}