using System;
using Snaps.Helpers.StringExt;
using System.Globalization;
using System.Data.SqlClient;
namespace Snaps.WMS {
    public class exsOutbound { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string ouorder    { get; set; }
        public string outype    { get; set; }
        public string ousubtype    { get; set; }
        public string thcode    { get; set; }
        public DateTimeOffset? dateorder    { get; set; }
        public DateTimeOffset? dateprep    { get; set; }
        public DateTimeOffset? dateexpire    { get; set; }
        public Int32 oupriority    { get; set; }
        public string ouflag    { get; set; }
        public string oupromo    { get; set; }
        public string dropship    { get; set; }
        public string orbitsource    { get; set; }
        public string stocode    { get; set; }
        public string stoname    { get; set; }
        public string stoaddressln1    { get; set; }
        public string stoaddressln2    { get; set; }
        public string stoaddressln3    { get; set; }
        public string stosubdistict    { get; set; }
        public string stodistrict    { get; set; }
        public string stocity    { get; set; }
        public string stocountry    { get; set; }
        public string stopostcode    { get; set; }
        public string stomobile    { get; set; }
        public string stoemail    { get; set; }
        public string tflow    { get; set; }
        public string inorder    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public string ermsg    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public Decimal rowid    { get; set; }

        public exsOutbound(){}
        public exsOutbound(string orgcode, string site, string depot, string csv){ 
            string[] rsl = csv.Split(',');
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.ouorder = rsl[0].ToString().Trim().ClearReg();
            this.outype = rsl[1].ToString().Trim().ClearReg();
            this.ousubtype = rsl[2].ToString().Trim().ClearReg();
            this.thcode = rsl[3].ToString().Trim().ClearReg();
            this.dateorder = (rsl[4].isNull()||rsl[4].ToString().ToLower() == "null") ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(rsl[4].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture); 
            this.dateprep = (rsl[5].isNull()||rsl[5].ToString().ToLower() == "null")? (DateTimeOffset?) null : DateTimeOffset.ParseExact(rsl[5].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.dateexpire = (rsl[6].isNull()||rsl[6].ToString().ToLower() == "null") ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(rsl[6].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.oupriority = rsl[7].ToString().Trim().ClearReg().CInt32();
            this.ouflag = rsl[8].ToString().Trim().ClearReg();
            this.oupromo = rsl[9].ToString().Trim().ClearReg();
            this.dropship = rsl[10].ToString().Trim().ClearReg();
            this.stocode = rsl[11].ToString().Trim().ClearReg();
            this.stoname = rsl[12].ToString().Trim().ClearReg();
            this.stoaddressln1 = rsl[13].ToString().Trim().ClearReg();
            this.stoaddressln2 = rsl[14].ToString().Trim().ClearReg();
            this.stoaddressln3 = rsl[15].ToString().Trim().ClearReg();
            this.stosubdistict = rsl[16].ToString().Trim().ClearReg();
            this.stodistrict = rsl[17].ToString().Trim().ClearReg();
            this.stocity = rsl[18].ToString().Trim().ClearReg();
            this.stocountry = rsl[19].ToString().Trim().ClearReg();
            this.stopostcode = rsl[20].ToString().Trim().ClearReg();
            this.stomobile = rsl[21].ToString().Trim().ClearReg();
            this.stoemail = rsl[22].ToString().Trim().ClearReg();
            this.inorder = rsl[23].ToString().Trim().ClearReg();
            this.rowops = rsl[24].ToString().Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsOutbound(string orgcode,string site,string depot,string ouorder,string outype,string ousubtype,string thcode,string dateorder,
                string dateprep,string dateexpire,string oupriority,string ouflag,string oupromo,string dropship,string stocode,string stoname,
                string stoaddressln1,string stoaddressln2,string stoaddressln3,string stosubdistict,string stodistrict,string stocity,string stocountry,
                string stopostcode,string stomobile,string stoemail,string inorder,string rowops ) {
            this.orgcode = orgcode.Trim().ClearReg();
            this.site = site.Trim().ClearReg();
            this.depot = depot.Trim().ClearReg();
            this.ouorder = ouorder.Trim().ClearReg();
            this.outype = outype.Trim().ClearReg();
            this.ousubtype = ousubtype.Trim().ClearReg();
            this.thcode = thcode.Trim().ClearReg();
            this.dateorder = (dateorder.isNull()||dateorder.ToString().ToLower() == "null") ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(dateorder.Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.dateprep = (dateprep.isNull()||dateprep.ToString().ToLower() == "null") ? (DateTimeOffset?) null :  DateTimeOffset.ParseExact(dateprep.Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.dateexpire = (dateexpire.isNull()||dateexpire.ToString().ToLower() == "null") ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(dateexpire.Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture);
            this.oupriority = oupriority.Trim().ClearReg().CInt32();
            this.ouflag = ouflag.Trim().ClearReg();
            this.oupromo = oupromo.Trim().ClearReg();
            this.dropship = dropship.Trim().ClearReg();
            this.orbitsource = orbitsource.Trim().ClearReg();
            this.stocode = stocode.Trim().ClearReg();
            this.stoname = stoname.Trim().ClearReg();
            this.stoaddressln1 = stoaddressln1.Trim().ClearReg();
            this.stoaddressln2 = stoaddressln2.Trim().ClearReg();
            this.stoaddressln3 = stoaddressln3.Trim().ClearReg();
            this.stosubdistict = stosubdistict.Trim().ClearReg();
            this.stodistrict = stodistrict.Trim().ClearReg();
            this.stocity = stocity.Trim().ClearReg();
            this.stocountry = stocountry.Trim().ClearReg();
            this.stopostcode = stopostcode.Trim().ClearReg();
            this.stomobile = stomobile.Trim().ClearReg();
            this.stoemail = stoemail.Trim().ClearReg();
            this.tflow = tflow.Trim().ClearReg();
            this.inorder = inorder.Trim().ClearReg();
            this.rowops = rowops.Trim().ClearReg();
            this.orbitsource = "External.Excel";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsOutbound(ref SqlDataReader r){ 
            this.orgcode = r["orgcode"].ToString();
            this.site = r["site"].ToString();
            this.depot = r["depot"].ToString();
            this.spcarea = r["spcarea"].ToString();
            this.ouorder = r["ouorder"].ToString();
            this.outype = r["outype"].ToString();
            this.ousubtype = r["ousubtype"].ToString();
            this.thcode = r["thcode"].ToString();
            this.dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
            this.dateprep = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            this.dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10);
            this.oupriority = (r.IsDBNull(11)) ? 0 :  r.GetInt32(11);
            this.ouflag = r["ouflag"].ToString();
            this.oupromo = r["oupromo"].ToString();
            this.dropship = r["dropship"].ToString();
            this.orbitsource = r["orbitsource"].ToString();
            this.stocode = r["stocode"].ToString();
            this.stoname = r["stoname"].ToString();
            this.stoaddressln1 = r["stoaddressln1"].ToString();
            this.stoaddressln2 = r["stoaddressln2"].ToString();
            this.stoaddressln3 = r["stoaddressln3"].ToString();
            this.stosubdistict = r["stosubdistict"].ToString();
            this.stodistrict = r["stodistrict"].ToString();
            this.stocity = r["stocity"].ToString();
            this.stocountry = r["stocountry"].ToString();
            this.stopostcode = r["stopostcode"].ToString();
            this.stomobile = r["stomobile"].ToString();
            this.stoemail = r["stoemail"].ToString();
            this.tflow = r["tflow"].ToString();
            this.inorder = r["inorder"].ToString();
            this.fileid = r["fileid"].ToString();
            this.rowops = r["rowops"].ToString();
            this.ermsg = r["ermsg"].ToString();
            this.dateops = (r.IsDBNull(33)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(33);
            this.rowid = (r.IsDBNull(34)) ? 0 :  r.GetDecimal(34);
        }
    
    }

    public class exsOutbouln { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string ouorder    { get; set; }
        public string ouln    { get; set; }
        public string ourefno    { get; set; }
        public string ourefln    { get; set; }
        public string inorder    { get; set; }
        public string article    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string unitops    { get; set; }
        public Int32 qtysku    { get; set; }
        public Int32 qtypu    { get; set; }
        public Decimal qtyweight    { get; set; }
        public string spcselect    { get; set; }
        public string batchno    { get; set; }
        public string lotno    { get; set; }
        public DateTimeOffset? datemfg    { get; set; }
        public DateTimeOffset? dateexp    { get; set; }
        public string serialno    { get; set; }
        public string orbitsource    { get; set; }
        public string tflow    { get; set; }
        public string disthcode    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public string ermsg    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public Decimal rowid    { get; set; }
        public Int32 ouseq    { get; set; }

        public exsOutbouln(){}
        public exsOutbouln(string orgcode, string site, string depot, string csv){ 
            string[] rsl = csv.Split(',');
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.ouorder = rsl[0].ToString().Trim().ClearReg();
            this.ouln = rsl[1].ToString().Trim().ClearReg();
            this.ourefno = rsl[2].ToString().Trim().ClearReg();
            this.ourefln = rsl[3].ToString().Trim().ClearReg();
            this.inorder = rsl[4].ToString().Trim().ClearReg();
            this.article = rsl[5].ToString().Trim().ClearReg();
            this.pv = rsl[6].ToString().Trim().ClearReg().CInt32();
            this.lv = rsl[7].ToString().Trim().ClearReg().CInt32();
            this.unitops = rsl[8].ToString().Trim().ClearReg();
            this.qtysku = rsl[9].ToString().Trim().ClearReg().CInt32();
            this.qtypu = rsl[10].ToString().Trim().ClearReg().CInt32();
            this.qtyweight = rsl[11].ToString().Trim().ClearReg().CDecimal();
            this.spcselect = rsl[12].ToString().Trim().ClearReg();
            this.batchno = rsl[13].ToString().Trim().ClearReg();
            this.lotno = rsl[14].ToString().Trim().ClearReg();
            this.datemfg =  (rsl[15].isNull()||rsl[15].ToString().ToLower() == "null") ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(rsl[15].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture); 
            this.dateexp = (rsl[16].isNull()||rsl[16].ToString().ToLower() == "null") ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(rsl[16].ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture); 
            this.serialno = rsl[17].ToString().Trim().ClearReg();
            this.disthcode = rsl[18].ToString().Trim().ClearReg();
            this.rowops = rsl[19].ToString().Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsOutbouln(string orgcode,string site,string depot,string ouorder,string ouln,string ourefno,string ourefln,string inorder,
        string article,string pv,string lv,string unitops,string qtysku,string qtypu,string qtyweight,string spcselect,string batchno,
        string lotno,string datemfg,string dateexp,string serialno,string disthcode,string rowops) {
            this.orgcode = orgcode.Trim().ClearReg();
            this.site = site.Trim().ClearReg();
            this.depot = depot.Trim().ClearReg();
            this.ouorder = ouorder.Trim().ClearReg();
            this.ouorder = ouorder.Trim().ClearReg();
            this.ouln = ouln.Trim().ClearReg();
            this.ourefno = ourefno.Trim().ClearReg();
            this.ourefln = ourefln.Trim().ClearReg();
            this.inorder = inorder.Trim().ClearReg();
            this.article = article.Trim().ClearReg();
            this.pv = pv.Trim().ClearReg().CInt32();
            this.lv = lv.Trim().ClearReg().CInt32();
            this.unitops = unitops.Trim().ClearReg();
            this.qtysku = qtysku.Trim().ClearReg().CInt32();
            this.qtypu = qtypu.Trim().ClearReg().CInt32();
            this.qtyweight = qtyweight.Trim().ClearReg().CDecimal();
            this.spcselect = spcselect.Trim().ClearReg();
            this.batchno = batchno.Trim().ClearReg();
            this.lotno = lotno.Trim().ClearReg();
            this.datemfg = (datemfg.isNull()||datemfg.ToString().ToLower() == "null") ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(datemfg.ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture); 
            this.dateexp = (dateexp.isNull()||dateexp.ToString().ToLower() == "null") ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(dateexp.ToString().Trim().ClearReg(), "yyyyMMdd", CultureInfo.InvariantCulture); 
            this.serialno = serialno.Trim().ClearReg();
            this.orbitsource = orbitsource.Trim().ClearReg();
            this.tflow = tflow.Trim().ClearReg();
            this.disthcode = disthcode.Trim().ClearReg();
            this.rowops = rowops.Trim().ClearReg();
            this.orbitsource = "External.Excel";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsOutbouln(ref SqlDataReader r){ 
            this.orgcode = r["orgcode"].ToString();
            this.site = r["site"].ToString();
            this.depot = r["depot"].ToString();
            this.spcarea = r["spcarea"].ToString();
            this.ouorder = r["ouorder"].ToString();
            this.ouln = r["ouln"].ToString();
            this.ourefno = r["ourefno"].ToString();
            this.ourefln = r["ourefln"].ToString();
            this.inorder = r["inorder"].ToString();
            this.article = r["article"].ToString();
            this.pv = (r.IsDBNull(10)) ? 0 :  r.GetInt32(10);
            this.lv = (r.IsDBNull(11)) ? 0 :  r.GetInt32(11);
            this.unitops = r["unitops"].ToString();
            this.qtysku = (r.IsDBNull(13)) ? 0 :  r.GetInt32(13);
            this.qtypu = (r.IsDBNull(14)) ? 0 :  r.GetInt32(14);
            this.qtyweight =  (r.IsDBNull(15)) ? 0 : r.GetDecimal(15);
            this.spcselect = r["spcselect"].ToString();
            this.batchno = r["batchno"].ToString();
            this.lotno = r["lotno"].ToString();
            this.datemfg = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19);
            this.dateexp = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20);
            this.serialno = r["serialno"].ToString();
            this.orbitsource = r["orbitsource"].ToString();
            this.tflow = r["tflow"].ToString();
            this.disthcode = r["disthcode"].ToString();
            this.fileid = r["fileid"].ToString();
            this.rowops = r["rowops"].ToString();
            this.ermsg = r["ermsg"].ToString();
            this.dateops = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(28);
            this.rowid = (r.IsDBNull(29)) ? 0 :  r.GetDecimal(29);
            this.ouseq = (r.IsDBNull(30)) ? 0 :  r.GetInt32(30);
        }
    
    }
}