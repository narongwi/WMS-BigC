using System;
using Snaps.Helpers.StringExt;
using System.Globalization;
using System.Data.SqlClient;
namespace Snaps.WMS {
    public class exsInbound { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string thcode    { get; set; }
        public string intype    { get; set; }
        public string subtype    { get; set; }
        public string inorder    { get; set; }
        public DateTimeOffset? dateorder    { get; set; }
        public DateTimeOffset? dateplan    { get; set; }
        public DateTimeOffset? dateexpire    { get; set; }
        public Int32 inpriority    { get; set; }
        public string inflag    { get; set; }
        public string inpromo    { get; set; }
        public string tflow    { get; set; }
        public string orbitsource    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public string ermsg    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public Decimal rowid    { get; set; }
        public exsInbound(){}
        public exsInbound(string orgcode, string site, string depot, string csv){ 
            string[] rsl = csv.Split(',');
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.thcode = rsl[0].ToString().Trim().ClearReg();
            this.intype = rsl[1].ToString().Trim().ClearReg();
            this.subtype = rsl[2].ToString().Trim().ClearReg();
            this.inorder = rsl[3].ToString().Trim().ClearReg();
            this.dateorder = (rsl[4].ToString().isNull()) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(rsl[4].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.dateplan = (rsl[5].ToString().isNull()) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(rsl[5].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.dateexpire = (rsl[6].ToString().isNull()) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(rsl[6].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.inpriority = rsl[7].ToString().Trim().CInt32();
            this.inflag = rsl[8].ToString().Trim().ClearReg();
            this.inpromo = rsl[9].ToString().Trim().ClearReg();
            this.tflow = "";
            this.rowops = rsl[10].ToString().Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsInbound(string orgcode,string site,string depot,string thcode,string intype,string subtype,string inorder,string dateorder,
            string dateplan,string dateexpire,string inpriority,string inflag,string inpromo,string rowops) {
            this.orgcode = orgcode.Trim().ClearReg();
            this.site = site.Trim().ClearReg();
            this.depot = depot.Trim().ClearReg();
            this.thcode = thcode.Trim().ClearReg();
            this.intype = intype.Trim().ClearReg();
            this.subtype = subtype.Trim().ClearReg();
            this.inorder = inorder.Trim().ClearReg();
            this.dateorder = (dateorder.isNull()) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(dateorder.Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.dateplan = (dateplan.isNull()) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(dateplan.Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.dateexpire = (dateexpire.isNull()) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(dateexpire.Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.inpriority = inpriority.Trim().ClearReg().CInt32();
            this.inflag = inflag.Trim().ClearReg();
            this.inpromo = inpromo.Trim().ClearReg();
            this.tflow = "";
            this.rowops = rowops.Trim().ClearReg();
            this.orbitsource = "External.Excel";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsInbound(ref SqlDataReader r){ 
            this.orgcode = r["orgcode"].ToString();
            this.site = r["site"].ToString();
            this.depot = r["depot"].ToString();
            this.spcarea = r["spcarea"].ToString();
            this.thcode = r["thcode"].ToString();
            this.intype = r["intype"].ToString();
            this.subtype = r["subtype"].ToString();
            this.inorder = r["inorder"].ToString();
            this.dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
            this.dateplan = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            this.dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10);
            this.inpriority = (r.IsDBNull(11)) ? 0 :  r.GetInt32(11);
            this.inflag = r["inflag"].ToString();
            this.inpromo = r["inpromo"].ToString();
            this.tflow = r["tflow"].ToString();
            this.orbitsource = r["orbitsource"].ToString();
            this.fileid = r["fileid"].ToString();
            this.rowops = r["rowops"].ToString();
            this.ermsg = r["ermsg"].ToString();
            this.dateops = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19);
            this.rowid = (r.IsDBNull(20)) ? 0 :  r.GetDecimal(20);
        }
    
    }


    public class exsInbouln { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string inorder    { get; set; }
        public string inln    { get; set; }
        public string inrefno    { get; set; }
        public string inrefln    { get; set; }
        public string article    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string unitops    { get; set; }
        public Int32 qtysku    { get; set; }
        public Int32 qtypu    { get; set; }
        public Decimal qtyweight    { get; set; }
        public string batchno    { get; set; }
        public string lotno    { get; set; }
        public DateTimeOffset? expdate    { get; set; }
        public string serialno    { get; set; }
        public string orbitsource    { get; set; }
        public string tflow    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public string ermsg    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public Decimal rowid    { get; set; }

        public exsInbouln(){}
        public exsInbouln(string orgcode, string site, string depot, string csv){ 
            string[] rsl = csv.Split(',');
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.inorder = rsl[0].ToString().Trim().ClearReg();
            this.inln = rsl[1].ToString().Trim().ClearReg();
            this.inrefno = rsl[2].ToString().Trim().ClearReg();
            this.inrefln = rsl[3].ToString().Trim().ClearReg();
            this.article = rsl[4].ToString().Trim().ClearReg();
            this.pv = rsl[5].ToString().Trim().ClearReg().CInt32();
            this.lv = rsl[6].ToString().Trim().ClearReg().CInt32();
            this.unitops = rsl[7].ToString().Trim().ClearReg();
            this.qtysku = rsl[8].ToString().Trim().ClearReg().CInt32();
            this.qtypu = rsl[9].ToString().Trim().ClearReg().CInt32();
            this.qtyweight = rsl[10].ToString().Trim().ClearReg().CDecimal();
            this.batchno = rsl[11].ToString().Trim().ClearReg();
            this.lotno = rsl[12].ToString().Trim().ClearReg();
            this.expdate = (rsl[13].isNull()||rsl[13].ToString().ToLower() == "null") ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(rsl[13].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture); 
            this.serialno = rsl[14].ToString().Trim().ClearReg();
            this.rowops = rsl[15].ToString().Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsInbouln(string orgcode,string site,string depot,string inorder,string inln,string inrefno,string inrefln,
                string article,string pv,string lv,string unitops,string qtysku,string qtypu,string qtyweight,string batchno,string lotno,
                string expdate,string serialno,string rowops) {
            this.orgcode = orgcode.Trim().ClearReg();
            this.site = site.Trim().ClearReg();
            this.depot = depot.Trim().ClearReg();
            this.spcarea = spcarea.Trim().ClearReg();
            this.inorder = inorder.Trim().ClearReg();
            this.inln = inln.Trim().ClearReg();
            this.inrefno = inrefno.Trim().ClearReg();
            this.inrefln = inrefln.Trim().ClearReg();
            this.article = article.Trim().ClearReg();
            this.pv = pv.Trim().ClearReg().CInt32();
            this.lv = lv.Trim().ClearReg().CInt32();
            this.unitops = unitops.Trim().ClearReg();
            this.qtysku = qtysku.Trim().ClearReg().CInt32();
            this.qtypu = qtypu.Trim().ClearReg().CInt32();
            this.qtyweight = qtyweight.Trim().ClearReg().CDecimal();
            this.batchno = batchno.Trim().ClearReg();
            this.lotno = lotno.Trim().ClearReg();
            this.expdate = (expdate.isNull()||expdate.ToString().ToLower() == "null") ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(expdate.ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.serialno = serialno.Trim().ClearReg();
            this.orbitsource = orbitsource.Trim().ClearReg();
            this.tflow = "";
            this.rowops = rowops.Trim().ClearReg();
            this.orbitsource = "External.Excel";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsInbouln(ref SqlDataReader r){ 
        this.orgcode = r["orgcode"].ToString();
        this.site = r["site"].ToString();
        this.depot = r["depot"].ToString();
        this.spcarea = r["spcarea"].ToString();
        this.inorder = r["inorder"].ToString();
        this.inln = r["inln"].ToString();
        this.inrefno = r["inrefno"].ToString();
        this.inrefln = r["inrefln"].ToString();
        this.article = r["article"].ToString();
        this.pv = (r.IsDBNull(9)) ? 0 :  r.GetInt32(9);
        this.lv = (r.IsDBNull(10)) ? 0 :  r.GetInt32(10);
        this.unitops = r["unitops"].ToString();
        this.qtysku = (r.IsDBNull(12)) ? 0 :  r.GetInt32(12);
        this.qtypu = (r.IsDBNull(13)) ? 0 :  r.GetInt32(13);
        this.qtyweight =  (r.IsDBNull(14)) ? 0 : r.GetDecimal(14);
        this.batchno = r["batchno"].ToString();
        this.lotno = r["lotno"].ToString();
        this.expdate = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17);
        this.serialno = r["serialno"].ToString();
        this.orbitsource = r["orbitsource"].ToString();
        this.tflow = r["tflow"].ToString();
        this.fileid = r["fileid"].ToString();
        this.rowops = r["rowops"].ToString();
        this.ermsg = r["ermsg"].ToString();
        this.dateops = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24);
        this.rowid = (r.IsDBNull(25)) ? 0 :  r.GetDecimal(25);
        }
    
    }
}