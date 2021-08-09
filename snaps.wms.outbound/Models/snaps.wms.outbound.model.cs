using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using System.ComponentModel.DataAnnotations;
namespace Snaps.WMS {
    //Header 
    public class outbound_ls {
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String ouorder { get; set; } 
        public String outype { get; set; } 
        public String ousubtype { get; set; } 
        public String thcode { get; set; } 
        public DateTimeOffset? dateorder { get; set; } 
        public DateTimeOffset? dateprep { get; set; } 
        public DateTimeOffset? dateexpire { get; set; } 
        public Int32 oupriority { get; set; } 
        public String oupromo { get; set; } 
        public String orbitsource { get; set; } 
        public DateTimeOffset? datereqdel { get; set; } 
        public String ouremarks { get; set; } 
        public String tflow { get; set; } 
        public DateTimeOffset? datedelivery { get; set; } 
        public String thname { get; set; } 
        public String orbitsourcedsc { get; set; } 

        public String disinbound { get; set; }
        public String disproduct { get; set; }
        public String disproductdesc { get; set; } 

        public String dateremarks { get; set; }

        public Int32 dispv { get; set; }
        public Int32 dislv { get; set; }
        public Int32 disqtypnd { get; set ;}
        public string disunitops { get; set; }
        public string dishuno { get; set; }
        public Int32 disstockid { get; set; }

        public string disloccode { get; set; }


    }

    public class outbound_pm : outbound_ls { 
        //public String outype { get; set; } 
        public String stomobile { get; set; } 
        public String stoemail { get; set; } 
        //public DateTimeOffset? datereqdel { get; set; } 
        public DateTimeOffset? datereqfrom { get; set; }
        public DateTimeOffset? datereqto { get; set; }

        public DateTimeOffset? dateprepfrom { get; set; }
        public DateTimeOffset? dateprepto { get; set; }

        public DateTimeOffset? dateorderfrom { get; set; }
        public DateTimeOffset? dateorderto { get; set; }
        public String ouflag { get; set; }
        public String dropship { get; set; }
        public String stopostcode { get; set; }
        public String article { get; set; }


        //public String ouremarks { get; set; } 

        public string ispending { get; set; }
        public string inorder { get; set; }
        public string huno { get; set; }
    }
    public class outbound_ix : outbound_ls { 
        public String ouflag { get; set; } 
        public String dropship { get; set; } 
        public String stocode { get; set; } 
        public String stoname { get; set; } 
        public String stoaddressln1 { get; set; } 
        public String stoaddressln2 { get; set; } 
        public String stoaddressln3 { get; set; } 
        public String stosubdistict { get; set; } 
        public String stodistrict { get; set; } 
        public String stocity { get; set; } 
        public String stocountry { get; set; } 
        public String stopostcode { get; set; } 
        public String stomobile { get; set; } 
        public String stoemail { get; set; } 
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? opsdate { get; set; } 
    }
    public class outbound_md : outbound_ls  {
        public String ouflag { get; set; } 
        public String dropship { get; set; } 
        public String stocode { get; set; } 
        public String stoname { get; set; } 
        public String stoaddressln1 { get; set; } 
        public String stoaddressln2 { get; set; } 
        public String stoaddressln3 { get; set; } 
        public String stosubdistict { get; set; } 
        public String stodistrict { get; set; } 
        public String stocity { get; set; } 
        public String stocountry { get; set; } 
        public String stopostcode { get; set; } 
        public String stomobile { get; set; } 
        public String stoemail { get; set; }
        public DateTimeOffset? dateprocess { get; set; } 
        public Decimal qtyorder { get; set; } 
        public Decimal qtypnd { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 

        public List<outbouln_md> lines { get; set; }
        public Decimal qtystock { get; set; }
    }



    //Line
    public class outbouln_ls {
        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String ouorder { get; set; } 
        public String ouln { get; set; } 
        public String ourefno { get; set; } 
        public String barcode { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public String unitops { get; set; } 
        public Int32 qtysku { get; set; } 
        public Int32 qtypu { get; set; } 
        public String tflow { get; set; } 
        public String articledsc { get; set; } 
        public Int32 qtystock { get; set; }
    }
    public class outbouln_pm : outbouln_ls { 
        public string ourefln { get; set; } 
        public String inorder { get; set; } 
        public String serialno { get; set; } 
    }
    public class outbouln_ix : outbouln_ls {
        public string ourefln { get; set; } 
        public String inorder { get; set; } 
        public Decimal qtyweight { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? expdate { get; set; } 
        public String serialno { get; set; } 
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
    }
    public class outbouln_md : outbouln_ls  {
        public string ourefln { get; set; } 
        public String inorder { get; set; } 
        public Decimal qtyweight { get; set; } 
        public String spcselect { get; set; } 
        public String batchno { get; set; }
        public String lotno { get; set; }
        public DateTimeOffset? datemfg { get; set; } 
        public DateTimeOffset? dateexp { get; set; } 
        public String serialno { get; set; } 
        public Decimal qtypnd { get; set; } 
        public DateTimeOffset? datedelivery { get; set; } 
        public Int32 qtyskudel { get; set; } 
        public Int32 qtypudel { get; set; } 
        public Decimal qtyweightdel { get; set; } 
        public String oudnno { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; }     
        public String disthcode { get; set; }
        public String disthname { get; set; }
        public Int32 qtypndpu { get; set;}
        public Int32 qtyreqpu { get; set; }
    }

    public class outboulx_md  { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string ouorder    { get; set; }
        public string ouln    { get; set; }
        public string ourefno    { get; set; }
        public string ourefln    { get; set; }
        public Int32 ouseq    { get; set; }
        public string inorder    { get; set; }
        public string barcode    { get; set; }
        public string article    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string unitops    { get; set; }
        public Int32 opssku    { get; set; }
        public Int32 opspu    { get; set; }
        public Decimal opsweight    { get; set; }
        public Decimal opsvolume    { get; set; }
        public string oudono    { get; set; }
        public string oudnno    { get; set; }
        public string outrno    { get; set; }
        public string batchno    { get; set; }
        public string lotno    { get; set; }
        public DateTimeOffset? datemfg    { get; set; }
        public DateTimeOffset? dateexp    { get; set; }
        public string serialno    { get; set; }
        public string routenodel    { get; set; }
        public Decimal stockid    { get; set; }
        public string opshuno    { get; set; }
        public string opshusource    { get; set; }
        public DateTimeOffset? datedel    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string loccode { get; set; }
    }

}