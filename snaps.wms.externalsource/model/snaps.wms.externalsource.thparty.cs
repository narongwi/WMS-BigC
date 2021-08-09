using System;
using Snaps.Helpers.StringExt;
using System.Data.SqlClient;
namespace Snaps.WMS {
    public class exsThirdparty { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string thtype    { get; set; }
        public string thbutype    { get; set; }
        public string thcode    { get; set; }
        public string thcodealt    { get; set; }
        public string vatcode    { get; set; }
        public string thname    { get; set; }
        public string thnamealt    { get; set; }
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
        public string thgroup    { get; set; }
        public string thcomment    { get; set; }
        public string throuteformat    { get; set; }
        public decimal plandelivery    { get; set; }
        public decimal naturalloss    { get; set; }
        public string mapaddress    { get; set; }
        public string orbitsource    { get; set; }
        public string tflow    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public string ermsg    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public decimal rowid    { get; set; }

        public exsThirdparty(){}

        public exsThirdparty(string orgcode, string site, string depot, string csv){ 
            string[] rsl = csv.Split(',');
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.thtype = rsl[0].ToString().Trim().ClearReg();
            this.thbutype = rsl[1].ToString().Trim().ClearReg();
            this.thcode = rsl[2].ToString().Trim().ClearReg();
            this.thcodealt = rsl[3].ToString().Trim().ClearReg();
            this.vatcode = rsl[4].ToString().Trim().ClearReg();
            this.thname = rsl[5].ToString().Trim().ClearReg();
            this.thnamealt = rsl[6].ToString().Trim().ClearReg();
            this.addressln1 = rsl[7].ToString().Trim().ClearReg();
            this.addressln2 = rsl[8].ToString().Trim().ClearReg();
            this.addressln3 = rsl[9].ToString().Trim().ClearReg();
            this.subdistrict = rsl[10].ToString().Trim().ClearReg();
            this.district = rsl[11].ToString().Trim().ClearReg();
            this.city = rsl[12].ToString().Trim().ClearReg();
            this.country = rsl[13].ToString().Trim().ClearReg();
            this.postcode = rsl[14].ToString().Trim().ClearReg();
            this.region = rsl[15].ToString().Trim().ClearReg();
            this.telephone = rsl[16].ToString().Trim().ClearReg();
            this.email = rsl[17].ToString().Trim().ClearReg();
            this.thgroup = rsl[18].ToString().Trim().ClearReg();
            this.thcomment = rsl[19].ToString().Trim().ClearReg();
            this.throuteformat = rsl[20].ToString().Trim().ClearReg();
            this.plandelivery = rsl[21].ToString().Trim().CInt32();
            this.naturalloss = rsl[22].ToString().Trim().CInt32();
            this.mapaddress = rsl[23].ToString().Trim().ClearReg();
            this.rowops = rsl[24].ToString().Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsThirdparty(string orgcode,string site,string depot,string thtype,string thbutype,string thcode,
            string thcodealt,string vatcode,string thname,string thnamealt,string addressln1,string addressln2,string addressln3,
            string subdistrict,string district,string city,string country,string postcode,string region,string telephone,
            string email,string thgroup,string thcomment,string throuteformat,string plandelivery,string naturalloss,
            string mapaddress,string rowops) {
            this.orgcode = orgcode.Trim().ClearReg();
            this.site = site.Trim().ClearReg();
            this.depot = depot.Trim().ClearReg();
            this.thtype = thtype.Trim().ClearReg();
            this.thbutype = thbutype.Trim().ClearReg();
            this.thcode = thcode.Trim().ClearReg();
            this.thcodealt = thcodealt.Trim().ClearReg();
            this.vatcode = vatcode.Trim().ClearReg();
            this.thname = thname.Trim().ClearReg();
            this.thnamealt = thnamealt.Trim().ClearReg();
            this.addressln1 = addressln1.Trim().ClearReg();
            this.addressln2 = addressln2.Trim().ClearReg();
            this.addressln3 = addressln3.Trim().ClearReg();
            this.subdistrict = subdistrict.Trim().ClearReg();
            this.district = district.Trim().ClearReg();
            this.city = city.Trim().ClearReg();
            this.country = country.Trim().ClearReg();
            this.postcode = postcode.Trim().ClearReg();
            this.region = region.Trim().ClearReg();
            this.telephone = telephone.Trim().ClearReg();
            this.email = email.Trim().ClearReg();
            this.thgroup = thgroup.Trim().ClearReg();
            this.thcomment = thcomment.Trim().ClearReg();
            this.throuteformat = throuteformat.Trim().ClearReg();
            this.plandelivery = plandelivery.Trim().CInt32();
            this.naturalloss = naturalloss.Trim().CInt32();
            this.mapaddress = mapaddress.Trim().ClearReg();
            this.rowops = rowops.Trim().ClearReg();
            this.orbitsource = "External.Excel";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsThirdparty(ref SqlDataReader r){ 
            this.orgcode = r["orgcode"].ToString();
            this.site = r["site"].ToString();
            this.depot = r["depot"].ToString();
            this.spcarea = r["spcarea"].ToString();
            this.thtype = r["thtype"].ToString();
            this.thbutype = r["thbutype"].ToString();
            this.thcode = r["thcode"].ToString();
            this.thcodealt = r["thcodealt"].ToString();
            this.vatcode = r["vatcode"].ToString();
            this.thname = r["thname"].ToString();
            this.thnamealt = r["thnamealt"].ToString();
            this.addressln1 = r["addressln1"].ToString();
            this.addressln2 = r["addressln2"].ToString();
            this.addressln3 = r["addressln3"].ToString();
            this.subdistrict = r["subdistrict"].ToString();
            this.district = r["district"].ToString();
            this.city = r["city"].ToString();
            this.country = r["country"].ToString();
            this.postcode = r["postcode"].ToString();
            this.region = r["region"].ToString();
            this.telephone = r["telephone"].ToString();
            this.email = r["email"].ToString();
            this.thgroup = r["thgroup"].ToString();
            this.thcomment = r["thcomment"].ToString();
            this.throuteformat = r["throuteformat"].ToString();
            this.plandelivery = (r.IsDBNull(25)) ? 0 :  r.GetDecimal(25);
            this.naturalloss = (r.IsDBNull(26)) ? 0 :  r.GetDecimal(26);
            this.mapaddress = r["mapaddress"].ToString();
            this.orbitsource = r["orbitsource"].ToString();
            this.tflow = r["tflow"].ToString();
            this.fileid = r["fileid"].ToString();
            this.rowops = r["rowops"].ToString();
            this.ermsg = r["ermsg"].ToString();
            this.dateops = (r.IsDBNull(33)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(33);
            this.rowid = (r.IsDBNull(34)) ? 0 : r.GetDecimal(34);
        }
    

    }
}