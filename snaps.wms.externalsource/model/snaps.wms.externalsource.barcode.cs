using System;
using Snaps.Helpers.StringExt;
using System.Data.SqlClient;

namespace Snaps.WMS {
    public class exsBarcode { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string article    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string barops    { get; set; }
        public string barcode    { get; set; }
        public string bartype    { get; set; }
        public string thcode    { get; set; }
        public string orbitsource    { get; set; }
        public string tflow    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public string ermsg    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public decimal rowid    { get; set; }

        public exsBarcode(){}

        public exsBarcode(string orgcode, string site, string depot, string csv){ 
            string[] rsl = csv.Split(',');
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.barcode = rsl[0].ToString().Trim().ClearReg();
            this.thcode = rsl[1].ToString().Trim().ClearReg();
            this.article = rsl[2].ToString().Trim().ClearReg();
            this.pv = rsl[3].ToString().CInt32();
            this.lv = rsl[4].ToString().CInt32();
            this.rowops = rsl[5].ToString().Trim().ClearReg();
            this.barops = rsl[6].ToString().Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.bartype = "EAN13";
            this.tflow = "WC";
            
        }
        public exsBarcode(string orgcode, string site,string depot,string barcode,string  thcode,string  article,string  pv,string  lv,string  rowops,string barops) {
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.barcode = barcode.Trim().ClearReg();
            this.thcode = thcode.Trim().ClearReg();
            this.article = article.Trim().ClearReg();
            this.pv = pv.Trim().ClearReg().CInt32();
            this.lv = lv.Trim().ClearReg().CInt32();
            this.rowops = rowops.Trim().ClearReg();
            this.barops = barops.Trim().ClearReg();
            this.orbitsource = "External.Excel";
            this.bartype = "EAN13";
            this.tflow = "WC";
        }
        public exsBarcode(ref SqlDataReader r){ 
            this.orgcode = r["orgcode"].ToString();
            this.site = r["site"].ToString();
            this.depot = r["depot"].ToString();
            this.spcarea = r["spcarea"].ToString();
            this.article = r["article"].ToString();
            this.pv = (r.IsDBNull(5)) ? 0 :  r.GetInt32(5);
            this.lv = (r.IsDBNull(6)) ? 0 :  r.GetInt32(6);
            this.barops = r["barops"].ToString();
            this.barcode = r["barcode"].ToString();
            this.bartype = r["bartype"].ToString();
            this.thcode = r["thcode"].ToString();
            this.orbitsource = r["orbitsource"].ToString();
            this.tflow = r["tflow"].ToString();
            this.fileid = r["fileid"].ToString();
            this.rowops = r["rowops"].ToString();
            this.ermsg = r["ermsg"].ToString();
            this.dateops = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(16);
            this.rowid = (r.IsDBNull(17)) ? 0 :  r.GetDecimal(17);
        }
    
    }
}