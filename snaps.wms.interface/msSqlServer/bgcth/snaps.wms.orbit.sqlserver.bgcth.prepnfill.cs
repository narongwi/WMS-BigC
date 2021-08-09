 using System;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Oracle.ManagedDataAccess.Client;
namespace Snaps.WMS { 

    public partial class orbit_ops : IDisposable {

        //Barcode
        private orbit_barcode barcodefill(ref OracleDataReader r) { 
            orbit_barcode rn = new orbit_barcode();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(6)) ? 0 : r["pv"].ToString().CInt32();
            rn.lv = (r.IsDBNull(7)) ? 0 : r["lv"].ToString().CInt32();
            rn.barops = r["barops"].ToString();
            rn.barcode = r["barcode"].ToString();
            rn.bartype = r["bartype"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.ermsg = r["ermsg"].ToString();
            rn.dateops = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTime(16).CDateTimeOffset();
            return rn;
        }
        private List<SqlCommand> barcodecommand(ref List<SqlCommand> cm,orbit_barcode o,String sqlcmd = ""){ 
            foreach(SqlCommand x in cm){
                x.Parameters["orgcode"].Value = o.orgcode;
                x.Parameters["site"].Value = o.site;
                x.Parameters["depot"].Value = o.depot;
                x.Parameters["spcarea"].Value = o.spcarea;
                x.Parameters["article"].Value = o.article;
                x.Parameters["pv"].Value = o.pv;
                x.Parameters["lv"].Value = o.lv;
                x.Parameters["barops"].Value = o.barops;
                x.Parameters["barcode"].Value = o.barcode;
                x.Parameters["bartype"].Value = o.bartype;
                x.Parameters["thcode"].Value = o.thcode;
                x.Parameters["orbitsource"].Value = o.orbitsource;
                x.Parameters["tflow"].Value = o.tflow;
                x.Parameters["fileid"].Value = o.fileid;
                x.Parameters["rowops"].Value = o.rowops;
                x.Parameters["ermsg"].Value = o.ermsg;
            };
            return cm;
        }
    
        //Third party
        private orbit_thirdparty thirdpartyfill(ref OracleDataReader r) {
            orbit_thirdparty rn = new orbit_thirdparty();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.thtype = r["thtype"].ToString();
            rn.thbutype = r["thbutype"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.thcodealt = r["thcodealt"].ToString();
            rn.vatcode = r["vatcode"].ToString();
            rn.thname = r["thname"].ToString();
            rn.thnamealt = r["thnamealt"].ToString();
            rn.addressln1 = r["addressln1"].ToString();
            rn.addressln2 = r["addressln2"].ToString();
            rn.addressln3 = r["addressln3"].ToString();
            rn.subdistrict = r["subdistrict"].ToString();
            rn.district = r["district"].ToString();
            rn.city = r["city"].ToString();
            rn.country = r["country"].ToString();
            rn.postcode = r["postcode"].ToString();
            rn.region = r["region"].ToString();
            rn.telephone = r["telephone"].ToString();
            rn.email = r["email"].ToString();
            rn.thgroup = r["thgroup"].ToString();
            rn.thcomment = r["thcomment"].ToString();
            rn.throuteformat = r["throuteformat"].ToString();
            rn.plandelivery = (r.IsDBNull(26)) ? 0 : r.GetInt32(26);
            rn.naturalloss = (r.IsDBNull(27)) ?  0 : r.GetInt32(27);
            rn.mapaddress = r["mapaddress"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.ermsg = r["ermsg"].ToString();
            rn.dateops = (r.IsDBNull(33)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(33);
            return rn;
        }
        private List<SqlCommand> thirdpartycommand(orbit_thirdparty o, String sqlcmd = ""){
            List<SqlCommand> cm = new List<SqlCommand>();
            cm.Add(new SqlCommand(sqlorbit_thirdparty_insert,cn));
            cm.ForEach(x=>{ 
                x.snapsPar(o.orgcode,"orgcode");
                x.snapsPar(o.site,"site");
                x.snapsPar(o.depot,"depot");
                x.snapsPar(o.spcarea,"spcarea");
                x.snapsPar(o.thtype,"thtype");
                x.snapsPar(o.thbutype,"thbutype");
                x.snapsPar(o.thcode,"thcode");
                x.snapsPar(o.thcodealt,"thcodealt");
                x.snapsPar(o.vatcode,"vatcode");
                x.snapsPar(o.thname,"thname");
                x.snapsPar(o.thnamealt,"thnamealt");
                x.snapsPar(o.addressln1,"addressln1");
                x.snapsPar(o.addressln2,"addressln2");
                x.snapsPar(o.addressln3,"addressln3");
                x.snapsPar(o.subdistrict,"subdistrict");
                x.snapsPar(o.district,"district");
                x.snapsPar(o.city,"city");
                x.snapsPar(o.country,"country");
                x.snapsPar(o.postcode,"postcode");
                x.snapsPar(o.region,"region");
                x.snapsPar(o.telephone,"telephone");
                x.snapsPar(o.email,"email");
                x.snapsPar(o.thgroup,"thgroup");
                x.snapsPar(o.thcomment,"thcomment");
                x.snapsPar(o.throuteformat,"throuteformat");
                x.snapsPar(o.plandelivery,"plandelivery");
                x.snapsPar(o.naturalloss,"naturalloss");
                x.snapsPar(o.mapaddress,"mapaddress");
                x.snapsPar(o.tflow,"tflow");
                x.snapsPar(o.fileid,"fileid");
                x.snapsPar(o.rowops,"rowops");
                x.snapsPar(o.ermsg,"ermsg");
                x.snapsPar(o.dateops,"dateops");
                x.snapsPar(o.orbitsource,"orbitsource");
            });
            return cm;
        }
        
        //Product
        private orbit_product productfill(ref OracleDataReader r) {
            orbit_product rn = new orbit_product();
            try { 
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.article = r["article"].ToString();
            rn.articletype = r["articletype"].ToString();
            rn.pv = (r.IsDBNull(r.GetOrdinal("pv"))) ? 0 : r["pv"].ToString().CInt32();
            rn.lv = (r.IsDBNull(r.GetOrdinal("lv"))) ? 0 : r["lv"].ToString().CInt32();
            rn.description = r["description"].ToString();
            rn.descalt = r["descalt"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.dlcall = (r.IsDBNull(r.GetOrdinal("dlcall"))) ? 0 : r["dlcall"].ToString().CInt32();
            rn.dlcfactory = (r.IsDBNull(r.GetOrdinal("dlcfactory"))) ? 0 : r["dlcfactory"].ToString().CInt32();
            rn.dlcwarehouse = (r.IsDBNull(r.GetOrdinal("dlcwarehouse"))) ? 0 : r["dlcwarehouse"].ToString().CInt32();
            rn.dlcshop = (r.IsDBNull(r.GetOrdinal("dlcshop"))) ? 0 : r["dlcshop"].ToString().CInt32();
            rn.dlconsumer = (r.IsDBNull(r.GetOrdinal("dlconsumer"))) ? 0 : r["dlconsumer"].ToString().CInt32();
            rn.hdivison = r["hdivison"].ToString();
            rn.hdepartment = r["hdepartment"].ToString();
            rn.hsubdepart = r["hsubdepart"].ToString();
            rn.hclass = r["hclass"].ToString();
            rn.hsubclass = r["hsubclass"].ToString();
            rn.typemanage = r["typemanage"].ToString();
            rn.unitmanage = r["unitmanage"].ToString();
            rn.unitdesc = r["unitdesc"].ToString();
            rn.unitreceipt = r["unitreceipt"].ToString();
            rn.unitprep = r["unitprep"].ToString();
            rn.unitsale = r["unitsale"].ToString();
            rn.unitstock = r["unitstock"].ToString();
            rn.unitweight = r["unitweight"].ToString();
            rn.unitdimension = r["unitdimension"].ToString();
            rn.unitvolume = r["unitvolume"].ToString();
            rn.hucode = r["hucode"].ToString();
            rn.rtoskuofpu = (r.IsDBNull(r.GetOrdinal("rtoskuofpu"))) ? 0 : r["rtoskuofpu"].ToString().CInt32();
            rn.rtoskuofipck = (r.IsDBNull(r.GetOrdinal("rtoskuofipck"))) ? 0 : r["rtoskuofipck"].ToString().CInt32();
            rn.rtoskuofpck = (r.IsDBNull(r.GetOrdinal("rtoskuofpck"))) ? 0 : r["rtoskuofpck"].ToString().CInt32();
            rn.rtoskuoflayer = (r.IsDBNull(r.GetOrdinal("rtoskuoflayer"))) ? 0 : r["rtoskuoflayer"].ToString().CInt32();
            rn.rtoskuofhu = (r.IsDBNull(r.GetOrdinal("rtoskuofhu"))) ? 0 : r["rtoskuofhu"].ToString().CInt32();
            rn.rtopckoflayer = (r.IsDBNull(r.GetOrdinal("rtopckoflayer"))) ? 0 : r["rtopckoflayer"].ToString().CInt32();
            rn.rtolayerofhu = (r.IsDBNull(r.GetOrdinal("rtolayerofhu"))) ? 0 : r["rtolayerofhu"].ToString().CInt32();
            rn.innaturalloss = (r.IsDBNull(r.GetOrdinal("innaturalloss"))) ? 0 : r["innaturalloss"].ToString().CInt32();
            rn.ounaturalloss = (r.IsDBNull(r.GetOrdinal("ounaturalloss"))) ? 0 : r["ounaturalloss"].ToString().CInt32();
            rn.rtoipckofpck = (r.IsDBNull(r.GetOrdinal("rtoipckofpck"))) ? 0 : r["rtoipckofpck"].ToString().CInt32();
            rn.costinbound =  (r.IsDBNull(r.GetOrdinal("costinbound"))) ? 0 : r.GetDecimal(r.GetOrdinal("costinbound"));
            rn.costoutbound =  (r.IsDBNull(r.GetOrdinal("costoutbound"))) ? 0 : r.GetDecimal(r.GetOrdinal("costoutbound"));
            rn.costavg =  (r.IsDBNull(r.GetOrdinal("costavg"))) ? 0 : r.GetDecimal(r.GetOrdinal("costavg"));
            rn.skulength =  (r.IsDBNull(r.GetOrdinal("skulength"))) ? 0 : r.GetDecimal(r.GetOrdinal("skulength"));
            rn.skuwidth =  (r.IsDBNull(r.GetOrdinal("skuwidth"))) ? 0 : r.GetDecimal(r.GetOrdinal("skuwidth"));
            rn.skuheight =  (r.IsDBNull(r.GetOrdinal("skuheight"))) ? 0 : r.GetDecimal(r.GetOrdinal("skuheight"));
            rn.skugrossweight =  (r.IsDBNull(r.GetOrdinal("skugrossweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("skugrossweight"));
            rn.skuweight =  (r.IsDBNull(r.GetOrdinal("skuweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("skuweight"));
            rn.skuvolume =  (r.IsDBNull(r.GetOrdinal("skuvolume"))) ? 0 : r.GetDecimal(r.GetOrdinal("skuvolume"));
            rn.pulength =  (r.IsDBNull(r.GetOrdinal("pulength"))) ? 0 : r.GetDecimal(r.GetOrdinal("pulength"));
            rn.puwidth =  (r.IsDBNull(r.GetOrdinal("puwidth"))) ? 0 : r.GetDecimal(r.GetOrdinal("puwidth"));
            rn.puheight =  (r.IsDBNull(r.GetOrdinal("puheight"))) ? 0 : r.GetDecimal(r.GetOrdinal("puheight"));
            rn.pugrossweight =  (r.IsDBNull(r.GetOrdinal("pugrossweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("pugrossweight"));
            rn.puweight =  (r.IsDBNull(r.GetOrdinal("puweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("puweight"));
            rn.puvolume =  (r.IsDBNull(r.GetOrdinal("puvolume"))) ? 0 : r.GetDecimal(r.GetOrdinal("puvolume"));
            rn.ipcklength =  (r.IsDBNull(r.GetOrdinal("ipcklength"))) ? 0 : r.GetDecimal(r.GetOrdinal("ipcklength"));
            rn.ipckwidth =  (r.IsDBNull(r.GetOrdinal("ipckwidth"))) ? 0 : r.GetDecimal(r.GetOrdinal("ipckwidth"));
            rn.ipckheight =  (r.IsDBNull(r.GetOrdinal("ipckheight"))) ? 0 : r.GetDecimal(r.GetOrdinal("ipckheight"));
            rn.ipckgrossweight =  (r.IsDBNull(r.GetOrdinal("ipckgrossweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("ipckgrossweight"));
            rn.ipckweight =  (r.IsDBNull(r.GetOrdinal("ipckweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("ipckweight"));
            rn.ipckvolume =  (r.IsDBNull(r.GetOrdinal("ipckvolume"))) ? 0 : r.GetDecimal(r.GetOrdinal("ipckvolume"));
            rn.pcklength =  (r.IsDBNull(r.GetOrdinal("pcklength"))) ? 0 : r.GetDecimal(r.GetOrdinal("pcklength"));
            rn.pckwidth =  (r.IsDBNull(r.GetOrdinal("pckwidth"))) ? 0 : r.GetDecimal(r.GetOrdinal("pckwidth"));
            rn.pckheight =  (r.IsDBNull(r.GetOrdinal("pckheight"))) ? 0 : r.GetDecimal(r.GetOrdinal("pckheight"));
            rn.pckgrossweight =  (r.IsDBNull(r.GetOrdinal("pckgrossweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("pckgrossweight"));
            rn.pckweight =  (r.IsDBNull(r.GetOrdinal("pckweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("pckweight"));
            rn.pckvolume =  (r.IsDBNull(r.GetOrdinal("pckvolume"))) ? 0 : r.GetDecimal(r.GetOrdinal("pckvolume"));
            rn.layerlength =  (r.IsDBNull(r.GetOrdinal("layerlength"))) ? 0 : r.GetDecimal(r.GetOrdinal("layerlength"));
            rn.layerwidth =  (r.IsDBNull(r.GetOrdinal("layerwidth"))) ? 0 : r.GetDecimal(r.GetOrdinal("layerwidth"));
            rn.layerheight =  (r.IsDBNull(r.GetOrdinal("layerheight"))) ? 0 : r.GetDecimal(r.GetOrdinal("layerheight"));
            rn.layergrossweight =  (r.IsDBNull(r.GetOrdinal("layergrossweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("layergrossweight"));
            rn.layerweight =  (r.IsDBNull(r.GetOrdinal("layerweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("layerweight"));
            rn.layervolume =  (r.IsDBNull(r.GetOrdinal("layervolume"))) ? 0 : r.GetDecimal(r.GetOrdinal("layervolume"));
            rn.hulength =  (r.IsDBNull(r.GetOrdinal("hulength"))) ? 0 : r.GetDecimal(r.GetOrdinal("hulength"));
            rn.huwidth =  (r.IsDBNull(r.GetOrdinal("huwidth"))) ? 0 : r.GetDecimal(r.GetOrdinal("huwidth"));
            rn.huheight =  (r.IsDBNull(r.GetOrdinal("huheight"))) ? 0 : r.GetDecimal(r.GetOrdinal("huheight"));
            rn.hugrossweight =  (r.IsDBNull(r.GetOrdinal("hugrossweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("hugrossweight"));
            rn.huweight =  (r.IsDBNull(r.GetOrdinal("huweight"))) ? 0 : r.GetDecimal(r.GetOrdinal("huweight"));
            rn.huvolume =  (r.IsDBNull(r.GetOrdinal("huvolume"))) ? 0 : r.GetDecimal(r.GetOrdinal("huvolume"));
            rn.isdangerous = (r.IsDBNull(r.GetOrdinal("isdangerous"))) ? 0 : r["isdangerous"].ToString().CInt32();
            rn.ishighvalue = (r.IsDBNull(r.GetOrdinal("ishighvalue"))) ? 0 : r["ishighvalue"].ToString().CInt32();
            rn.isfastmove = (r.IsDBNull(r.GetOrdinal("isfastmove"))) ? 0 : r["isfastmove"].ToString().CInt32();
            rn.isslowmove = (r.IsDBNull(r.GetOrdinal("isslowmove"))) ? 0 : r["isslowmove"].ToString().CInt32();
            rn.isprescription = (r.IsDBNull(r.GetOrdinal("isprescription"))) ? 0 : r["isprescription"].ToString().CInt32();
            rn.isdlc = (r.IsDBNull(r.GetOrdinal("isdlc"))) ? 0 : r["isdlc"].ToString().CInt32();
            rn.ismaterial = (r.IsDBNull(r.GetOrdinal("ismaterial"))) ? 0 : r["ismaterial"].ToString().CInt32();
            rn.isunique = (r.IsDBNull(r.GetOrdinal("isunique"))) ? 0 : r["isunique"].ToString().CInt32();
            rn.isalcohol = (r.IsDBNull(r.GetOrdinal("isalcohol"))) ? 0 : r["isalcohol"].ToString().CInt32();
            rn.istemperature = (r.IsDBNull(r.GetOrdinal("istemperature"))) ? 0 : r["istemperature"].ToString().CInt32();
            rn.isdynamicpick = (r.IsDBNull(r.GetOrdinal("isdynamicpick"))) ? 0 : r["isdynamicpick"].ToString().CInt32();
            rn.ismixingprep = (r.IsDBNull(r.GetOrdinal("ismixingprep"))) ? 0 : r["ismixingprep"].ToString().CInt32();
            rn.isfinishgoods = (r.IsDBNull(r.GetOrdinal("isfinishgoods"))) ? 0 : r["isfinishgoods"].ToString().CInt32();
            rn.isnaturalloss = (r.IsDBNull(r.GetOrdinal("isnaturalloss"))) ? 0 : r["isnaturalloss"].ToString().CInt32();
            rn.isbatchno = (r.IsDBNull(r.GetOrdinal("isbatchno"))) ? 0 : r["isbatchno"].ToString().CInt32();
            rn.ismeasurement = (r.IsDBNull(r.GetOrdinal("ismeasurement"))) ? 0 : r["ismeasurement"].ToString().CInt32();
            rn.roomtype = r["roomtype"].ToString();
            rn.tempmin = (r.IsDBNull(r.GetOrdinal("tempmin"))) ? 0 : r["tempmin"].ToString().CInt32();
            rn.tempmax = (r.IsDBNull(r.GetOrdinal("tempmax"))) ? 0 : r["tempmax"].ToString().CInt32();
            rn.alcmanage = r["alcmanage"].ToString();
            rn.alccategory = r["alccategory"].ToString();
            rn.alccontent = r["alccontent"].ToString();
            rn.alccolor = r["alccolor"].ToString();
            rn.dangercategory = r["dangercategory"].ToString();
            rn.dangerlevel = r["dangerlevel"].ToString();
            rn.stockthresholdmin = (r.IsDBNull(r.GetOrdinal("stockthresholdmin"))) ? 0 : r["stockthresholdmin"].ToString().CInt32();
            rn.stockthresholdmax = (r.IsDBNull(r.GetOrdinal("stockthresholdmax"))) ? 0 : r["stockthresholdmax"].ToString().CInt32();
            rn.spcrecvzone = r["spcrecvzone"].ToString();
            rn.spcrecvaisle = r["spcrecvaisle"].ToString();
            rn.spcrecvbay = r["spcrecvbay"].ToString();
            rn.spcrecvlevel = r["spcrecvlevel"].ToString();
            rn.spcrecvlocation = r["spcrecvlocation"].ToString();
            rn.spcprepzone = r["spcprepzone"].ToString();
            rn.spcdistzone = r["spcdistzone"].ToString();
            rn.spcdistshare = r["spcdistshare"].ToString();
            rn.spczonedelv = r["spczonedelv"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.datecreate = (r.IsDBNull(r.GetOrdinal("datecreate"))) ? (DateTimeOffset?) null : r.GetDateTimeOffset(r.GetOrdinal("datecreate"));
            rn.dateops = (r.IsDBNull(r.GetOrdinal("dateops"))) ? (DateTimeOffset?) null : r.GetDateTimeOffset(r.GetOrdinal("dateops"));
            rn.ermsg = r["ermsg"].ToString();
             return rn;
            }catch(Exception ex) { 
                throw ex;
            }
           
        }
        private List<SqlCommand> productcommand(orbit_product o, String sqlcmd = ""){
            List<SqlCommand> cm = new List<SqlCommand>();
            cm.Add(new SqlCommand(sqlorbit_product_insert,cn));
            cm.ForEach(x=>{ 
                x.snapsPar(o.orgcode,"orgcode");
                x.snapsPar(o.site,"site");
                x.snapsPar(o.depot,"depot");
                x.snapsPar(o.spcarea,"spcarea");
                x.snapsPar(o.article,"article");
                x.snapsPar(o.articletype,"articletype");
                x.snapsPar(o.pv,"pv");
                x.snapsPar(o.lv,"lv");
                x.snapsPar(o.description,"description");
                x.snapsPar(o.descalt,"descalt");
                x.snapsPar(o.thcode,"thcode");
                x.snapsPar(o.dlcall,"dlcall");
                x.snapsPar(o.dlcfactory,"dlcfactory");
                x.snapsPar(o.dlcwarehouse,"dlcwarehouse");
                x.snapsPar(o.dlcshop,"dlcshop");
                x.snapsPar(o.dlconsumer,"dlconsumer");
                x.snapsPar(o.hdivison,"hdivison");
                x.snapsPar(o.hdepartment,"hdepartment");
                x.snapsPar(o.hsubdepart,"hsubdepart");
                x.snapsPar(o.hclass,"hclass");
                x.snapsPar(o.hsubclass,"hsubclass");
                x.snapsPar(o.typemanage,"typemanage");
                x.snapsPar(o.unitmanage,"unitmanage");
                x.snapsPar(o.unitdesc,"unitdesc");
                x.snapsPar(o.unitreceipt,"unitreceipt");
                x.snapsPar(o.unitprep,"unitprep");
                x.snapsPar(o.unitsale,"unitsale");
                x.snapsPar(o.unitstock,"unitstock");
                x.snapsPar(o.unitweight,"unitweight");
                x.snapsPar(o.unitdimension,"unitdimension");
                x.snapsPar(o.unitvolume,"unitvolume");
                x.snapsPar(o.hucode,"hucode");
                x.snapsPar(o.rtoskuofpu,"rtoskuofpu");
                x.snapsPar(o.rtoskuofipck,"rtoskuofipck");
                x.snapsPar(o.rtoskuofpck,"rtoskuofpck");
                x.snapsPar(o.rtoskuoflayer,"rtoskuoflayer");
                x.snapsPar(o.rtoskuofhu,"rtoskuofhu");
                x.snapsPar(o.rtopckoflayer,"rtopckoflayer");
                x.snapsPar(o.rtolayerofhu,"rtolayerofhu");
                x.snapsPar(o.innaturalloss,"innaturalloss");
                x.snapsPar(o.rtoipckofpck, "rtoipckofpck");
                x.snapsPar(o.ounaturalloss,"ounaturalloss");
                x.snapsPar(o.costinbound,"costinbound");
                x.snapsPar(o.costoutbound,"costoutbound");
                x.snapsPar(o.costavg,"costavg");
                x.snapsPar(o.skulength,"skulength");
                x.snapsPar(o.skuwidth,"skuwidth");
                x.snapsPar(o.skuheight,"skuheight");
                x.snapsPar(o.skugrossweight,"skugrossweight");
                x.snapsPar(o.skuweight,"skuweight");
                x.snapsPar(o.skuvolume,"skuvolume");
                x.snapsPar(o.pulength,"pulength");
                x.snapsPar(o.puwidth,"puwidth");
                x.snapsPar(o.puheight,"puheight");
                x.snapsPar(o.pugrossweight,"pugrossweight");
                x.snapsPar(o.puweight,"puweight");
                x.snapsPar(o.puvolume,"puvolume");
                x.snapsPar(o.ipcklength,"ipcklength");
                x.snapsPar(o.ipckwidth,"ipckwidth");
                x.snapsPar(o.ipckheight,"ipckheight");
                x.snapsPar(o.ipckgrossweight,"ipckgrossweight");
                x.snapsPar(o.ipckweight,"ipckweight");
                x.snapsPar(o.ipckvolume,"ipckvolume");
                x.snapsPar(o.pcklength,"pcklength");
                x.snapsPar(o.pckwidth,"pckwidth");
                x.snapsPar(o.pckheight,"pckheight");
                x.snapsPar(o.pckgrossweight,"pckgrossweight");
                x.snapsPar(o.pckweight,"pckweight");
                x.snapsPar(o.pckvolume,"pckvolume");
                x.snapsPar(o.layerlength,"layerlength");
                x.snapsPar(o.layerwidth,"layerwidth");
                x.snapsPar(o.layerheight,"layerheight");
                x.snapsPar(o.layergrossweight,"layergrossweight");
                x.snapsPar(o.layerweight,"layerweight");
                x.snapsPar(o.layervolume,"layervolume");
                x.snapsPar(o.hulength,"hulength");
                x.snapsPar(o.huwidth,"huwidth");
                x.snapsPar(o.huheight,"huheight");
                x.snapsPar(o.hugrossweight,"hugrossweight");
                x.snapsPar(o.huweight,"huweight");
                x.snapsPar(o.huvolume,"huvolume");
                x.snapsPar(o.isdangerous,"isdangerous");
                x.snapsPar(o.ishighvalue,"ishighvalue");
                x.snapsPar(o.isfastmove,"isfastmove");
                x.snapsPar(o.isslowmove,"isslowmove");
                x.snapsPar(o.isprescription,"isprescription");
                x.snapsPar(o.isdlc,"isdlc");
                x.snapsPar(o.ismaterial,"ismaterial");
                x.snapsPar(o.isunique,"isunique");
                x.snapsPar(o.isalcohol,"isalcohol");
                x.snapsPar(o.istemperature,"istemperature");
                x.snapsPar(o.isdynamicpick,"isdynamicpick");
                x.snapsPar(o.ismixingprep,"ismixingprep");
                x.snapsPar(o.isfinishgoods,"isfinishgoods");
                x.snapsPar(o.isnaturalloss,"isnaturalloss");
                x.snapsPar(o.isbatchno,"isbatchno");
                x.snapsPar(o.ismeasurement,"ismeasurement");
                x.snapsPar(o.roomtype,"roomtype");
                x.snapsPar(o.tempmin,"tempmin");
                x.snapsPar(o.tempmax,"tempmax");
                x.snapsPar(o.alcmanage,"alcmanage");
                x.snapsPar(o.alccategory,"alccategory");
                x.snapsPar(o.alccontent,"alccontent");
                x.snapsPar(o.alccolor,"alccolor");
                x.snapsPar(o.dangercategory,"dangercategory");
                x.snapsPar(o.dangerlevel,"dangerlevel");
                x.snapsPar(o.stockthresholdmin,"stockthresholdmin");
                x.snapsPar(o.stockthresholdmax,"stockthresholdmax");
                x.snapsPar(o.spcrecvzone,"spcrecvzone");
                x.snapsPar(o.spcrecvaisle,"spcrecvaisle");
                x.snapsPar(o.spcrecvbay,"spcrecvbay");
                x.snapsPar(o.spcrecvlevel,"spcrecvlevel");
                x.snapsPar(o.spcrecvlocation,"spcrecvlocation");
                x.snapsPar(o.spcprepzone,"spcprepzone");
                x.snapsPar(o.spcdistzone,"spcdistzone");
                x.snapsPar(o.spcdistshare,"spcdistshare");
                x.snapsPar(o.spczonedelv,"spczonedelv");
                x.snapsPar(o.orbitsource,"orbitsource");
                x.snapsPar(o.tflow,"tflow");
                x.snapsPar(o.fileid,"fileid");
                x.snapsPar(o.rowops,"rowops");
                x.snapsPar(o.datecreate,"datecreate");
                x.snapsPar(o.dateops,"dateops");
                x.snapsPar(o.ermsg,"ermsg");
            });            
            return cm;
        }

        //Inbound
        private orbit_inbound inboundfill(ref OracleDataReader r) {
            orbit_inbound rn = new orbit_inbound();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.intype = r["intype"].ToString();
            rn.subtype = r["subtype"].ToString();
            rn.inorder = r["inorder"].ToString();
            rn.dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTime(8).CDateTimeOffset();
            rn.dateplan = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTime(9).CDateTimeOffset();
            rn.dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTime(10).CDateTimeOffset();
            rn.inpriority = (r.IsDBNull(12)) ? 0 : r["inpriority"].ToString().CInt32();
            rn.inflag = r["inflag"].ToString();
            rn.inpromo = r["inpromo"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.ermsg = r["ermsg"].ToString();
            rn.dateops = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTime(19).CDateTimeOffset();
            return rn;
        }
        private orbit_inbouln inboulnfill(ref OracleDataReader r) {
            orbit_inbouln rn = new orbit_inbouln(); 
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.inorder = r["inorder"].ToString();
            rn.inln = r["inln"].ToString();
            rn.inrefno = r["inrefno"].ToString();
            rn.inrefln = (r.IsDBNull(7)) ? 0 :  r.GetInt32(7);
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(9)) ? 0 :  r.GetInt32(9);
            rn.lv = (r.IsDBNull(10)) ? 0 :  r.GetInt32(10);
            rn.unitops = r["unitops"].ToString();
            rn.qtysku = (r.IsDBNull(12)) ? 0 :  r.GetInt32(12);
            rn.qtypu = (r.IsDBNull(13)) ? 0 :  r.GetInt32(13);
            rn.qtyweight =  (r.IsDBNull(14)) ? 0 : r.GetDecimal(14);
            rn.batchno = r["batchno"].ToString();
            rn.lotno = r["lotno"].ToString();
            rn.expdate = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTime(17).CDateTimeOffset();
            rn.serialno = r["serialno"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.ermsg = r["ermsg"].ToString();
            rn.dateops = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTime(24).CDateTimeOffset();
            return rn;
        }

        private List<SqlCommand> inboundcommand(ref List<SqlCommand> cm,orbit_inbound o, string sqlcmd = ""){
             foreach(SqlCommand x in cm){
                x.Parameters["orgcode"].Value = o.orgcode;
                x.Parameters["site"].Value = o.site;
                x.Parameters["depot"].Value = o.depot;
                x.Parameters["spcarea"].Value = o.spcarea;
                x.Parameters["thcode"].Value = o.thcode;
                x.Parameters["intype"].Value = o.intype;
                x.Parameters["subtype"].Value = o.subtype;
                x.Parameters["inorder"].Value = o.inorder;

                if (o.dateorder == null){ 
                    x.Parameters["dateorder"].Value = DBNull.Value;
                }else { 
                    x.Parameters["dateorder"].Value = o.dateorder;
                }

                if (o.dateplan == null){ 
                    x.Parameters["dateplan"].Value = DBNull.Value;
                }else { 
                    x.Parameters["dateplan"].Value = o.dateplan;
                }

                if (o.dateexpire == null){ 
                    x.Parameters["dateexpire"].Value = DBNull.Value;
                }else { 
                    x.Parameters["dateexpire"].Value = o.dateexpire;
                }

                x.Parameters["inpriority"].Value = o.inpriority;
                x.Parameters["inflag"].Value = o.inflag;
                x.Parameters["inpromo"].Value = o.inpromo;
                x.Parameters["tflow"].Value = o.tflow;
                x.Parameters["orbitsource"].Value = o.orbitsource;
                x.Parameters["fileid"].Value = o.fileid;
                x.Parameters["rowops"].Value = o.rowops;
                x.Parameters["ermsg"].Value = o.ermsg;
            };
            return cm;
        }
        private List<SqlCommand> inboulncommand(ref List<SqlCommand> cm, orbit_inbouln o, string sqlcmd = ""){
             foreach(SqlCommand x in cm){
                x.Parameters["orgcode"].Value = o.orgcode;
                x.Parameters["site"].Value = o.site;
                x.Parameters["depot"].Value = o.depot;
                x.Parameters["spcarea"].Value = o.spcarea;
                x.Parameters["inorder"].Value = o.inorder;
                x.Parameters["inln"].Value = o.inln;
                x.Parameters["inrefno"].Value = o.inrefno;
                x.Parameters["inrefln"].Value = o.inrefln;
                x.Parameters["article"].Value = o.article;
                x.Parameters["pv"].Value = o.pv;
                x.Parameters["lv"].Value = o.lv;
                x.Parameters["unitops"].Value = o.unitops;
                x.Parameters["qtysku"].Value = o.qtysku;
                x.Parameters["qtypu"].Value = o.qtypu;
                x.Parameters["qtyweight"].Value = o.qtyweight;
                x.Parameters["batchno"].Value = o.batchno;
                x.Parameters["lotno"].Value = o.lotno;
                if (o.expdate == null){ 
                    x.Parameters["expdate"].Value = DBNull.Value;
                }else { 
                    x.Parameters["expdate"].Value = o.expdate;
                }
                x.Parameters["serialno"].Value = o.serialno;
                x.Parameters["orbitsource"].Value = o.orbitsource;
                x.Parameters["tflow"].Value = o.tflow;
                x.Parameters["fileid"].Value = o.fileid;
                x.Parameters["rowops"].Value = o.rowops;
                x.Parameters["ermsg"].Value = o.ermsg;
            };
            return cm;
        }

        //Outbound
        private orbit_outbound outboundfill(ref OracleDataReader r){ 

            orbit_outbound rn = new orbit_outbound();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.ouorder = r["ouorder"].ToString();
            rn.outype = r["outype"].ToString();
            rn.ousubtype = r["ousubtype"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTime(8).CDateTimeOffset();
            rn.dateprep = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTime(9).CDateTimeOffset();
            rn.dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTime(10).CDateTimeOffset();
            rn.oupriority = (r.IsDBNull(12)) ? 0 : r["oupriority"].ToString().CInt32();
            rn.ouflag = r["ouflag"].ToString();
            rn.oupromo = r["oupromo"].ToString();
            rn.dropship = r["dropship"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.stocode = r["stocode"].ToString();
            rn.stoname = r["stoname"].ToString();
            rn.stoaddressln1 = r["stoaddressln1"].ToString();
            rn.stoaddressln2 = r["stoaddressln2"].ToString();
            rn.stoaddressln3 = r["stoaddressln3"].ToString();
            rn.stosubdistict = r["stosubdistict"].ToString();
            rn.stodistrict = r["stodistrict"].ToString();
            rn.stocity = r["stocity"].ToString();
            rn.stocountry = r["stocountry"].ToString();
            rn.stopostcode = r["stopostcode"].ToString();
            rn.stomobile = r["stomobile"].ToString();
            rn.stoemail = r["stoemail"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.inorder = r["inorder"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.ermsg = r["ermsg"].ToString();
            rn.dateops = (r.IsDBNull(33)) ? (DateTimeOffset?) null : r.GetDateTime(33).CDateTimeOffset();
            return rn;
        }
        private orbit_outbouln outboulnfill(ref OracleDataReader r) {
            orbit_outbouln rn = new orbit_outbouln();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.ouorder = r["ouorder"].ToString();
            rn.ouln =  r["ouln"].ToString();
            rn.ourefno = r["ourefno"].ToString();
            rn.ourefln =  r["ourefln"].ToString();
            rn.inorder = r["inorder"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(11)) ? 0 : r["pv"].ToString().CInt32();
            rn.lv = (r.IsDBNull(12)) ? 0 : r["lv"].ToString().CInt32();
            rn.unitops = r["unitops"].ToString();
            rn.qtysku = (r.IsDBNull(14)) ? 0 : r["qtysku"].ToString().CInt32();
            rn.qtypu = (r.IsDBNull(r.GetOrdinal("qtypu"))) ? 0 : r["qtypu"].ToString().CInt32();
            rn.qtyweight =  (r.IsDBNull(16)) ? 0 : r.GetDecimal(16);
            rn.spcselect = r["spcselect"].ToString();
            rn.batchno = r["batchno"].ToString();
            rn.lotno = r["lotno"].ToString();
            rn.datemfg = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTime(19).CDateTimeOffset();
            rn.dateexp = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTime(20).CDateTimeOffset();
            rn.serialno = r["serialno"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.disthcode = r["disthcode"].ToString();
            rn.fileid = r["fileid"].ToString();
            rn.rowops = r["rowops"].ToString();
            rn.ermsg = r["ermsg"].ToString();
            rn.dateops = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTime(28).CDateTimeOffset();
            rn.ouseq = (r.IsDBNull(29)) ? 0 : r["ouseq"].ToString().CInt32();
            return rn;
        }

        private List<SqlCommand> outboundcommand(ref List<SqlCommand> cm, orbit_outbound o, string sqlcmd = ""){
            cm.ForEach(x=>{ 
                x.Parameters["orgcode"].Value = o.orgcode;
                x.Parameters["site"].Value = o.site;
                x.Parameters["depot"].Value = o.depot;
                x.Parameters["spcarea"].Value = o.spcarea;
                x.Parameters["ouorder"].Value = o.ouorder;
                x.Parameters["outype"].Value = o.outype;
                x.Parameters["ousubtype"].Value = o.ousubtype;
                x.Parameters["thcode"].Value = o.thcode;
                x.Parameters["dateorder"].Value = o.dateorder;
                x.Parameters["dateprep"].Value = o.dateprep;
                x.Parameters["dateexpire"].Value = o.dateexpire;
                x.Parameters["oupriority"].Value = o.oupriority;
                x.Parameters["ouflag"].Value = o.ouflag;
                x.Parameters["oupromo"].Value = o.oupromo;
                x.Parameters["dropship"].Value = o.dropship;
                x.Parameters["orbitsource"].Value = o.orbitsource;
                x.Parameters["stocode"].Value = o.stocode;
                x.Parameters["stoname"].Value = o.stoname;
                x.Parameters["stoaddressln1"].Value = o.stoaddressln1;
                x.Parameters["stoaddressln2"].Value = o.stoaddressln2;
                x.Parameters["stoaddressln3"].Value = o.stoaddressln3;
                x.Parameters["stosubdistict"].Value = o.stosubdistict;
                x.Parameters["stodistrict"].Value = o.stodistrict;
                x.Parameters["stocity"].Value = o.stocity;
                x.Parameters["stocountry"].Value = o.stocountry;
                x.Parameters["stopostcode"].Value = o.stopostcode;
                x.Parameters["stomobile"].Value = o.stomobile;
                x.Parameters["stoemail"].Value = o.stoemail;
                x.Parameters["tflow"].Value = o.tflow;
                x.Parameters["inorder"].Value = o.inorder;
                x.Parameters["fileid"].Value = o.fileid;
                x.Parameters["rowops"].Value = o.rowops;
                x.Parameters["ermsg"].Value = o.ermsg;
            });

            return cm;
        }
        private List<SqlCommand> outboulncommand(ref List<SqlCommand> cm, orbit_outbouln o, string sqlcmd = ""){
            foreach(SqlCommand x in cm){
                x.Parameters["orgcode"].Value = o.orgcode;
                x.Parameters["site"].Value = o.site;
                x.Parameters["depot"].Value = o.depot;
                x.Parameters["spcarea"].Value = o.spcarea;
                x.Parameters["ouorder"].Value = o.ouorder;
                x.Parameters["ouln"].Value = o.ouln;
                x.Parameters["ourefno"].Value = o.ourefno;
                x.Parameters["ourefln"].Value = o.ourefln;
                x.Parameters["inorder"].Value = o.inorder;
                x.Parameters["article"].Value = o.article;
                x.Parameters["pv"].Value = o.pv;
                x.Parameters["lv"].Value = o.lv;
                x.Parameters["unitops"].Value = o.unitops;
                x.Parameters["qtysku"].Value = o.qtysku;
                x.Parameters["qtypu"].Value = o.qtypu;
                x.Parameters["qtyweight"].Value = o.qtyweight;
                x.Parameters["spcselect"].Value = o.spcselect;
                x.Parameters["batchno"].Value = o.batchno;
                x.Parameters["lotno"].Value = o.lotno;
                if (o.datemfg == null){ 
                    x.Parameters["datemfg"].Value = DBNull.Value;
                }else { 
                    x.Parameters["datemfg"].Value = o.datemfg;
                }
                if (o.dateexp == null){ 
                    x.Parameters["dateexp"].Value = DBNull.Value;
                }else { 
                    x.Parameters["dateexp"].Value = o.datemfg;
                }
                x.Parameters["serialno"].Value = o.serialno;
                x.Parameters["orbitsource"].Value = o.orbitsource;
                x.Parameters["tflow"].Value = o.tflow;
                x.Parameters["disthcode"].Value = o.disthcode;
                x.Parameters["fileid"].Value = o.fileid;
                x.Parameters["rowops"].Value = o.rowops;
                x.Parameters["ermsg"].Value = o.ermsg;
                x.Parameters["ouseq"].Value = o.ouseq;
            };
            return cm;
        }

        //Receipt
        private void receiptCommand(ref List<SqlCommand> cm, List<orbit_receipt> o) { 
            foreach(orbit_receipt ln in o){ 
                cm.Add(receiptCommand(ln));
            }
        }
        private SqlCommand receiptCommand(orbit_receipt o){ 
            SqlCommand cm = new SqlCommand(sqlInterface_landing_receipt); 
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.intype,"intype");
            cm.snapsPar(o.insubtype,"insubtype");
            cm.snapsPar(o.inorder,"inorder");
            cm.snapsPar(o.inln,"inln");
            cm.snapsPar(o.ingrno,"ingrno");
            cm.snapsPar(o.inrefno,"inrefno");
            cm.snapsPar(o.inrefln,"inrefln");
            cm.snapsPar(o.barcode,"barcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.unitops,"unitops");
            cm.snapsPar(o.qtysku,"qtysku");
            cm.snapsPar(o.qtypu,"qtypu");
            cm.snapsPar(o.qtyweight,"qtyweight");
            cm.snapsPar(o.qtyvolume,"qtyvolume");
            cm.snapsPar(o.qtynaturalloss,"qtynaturalloss");
            cm.snapsPar(o.dateops,"dateops");
            cm.snapsPar(o.accnops,"accnops");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotmfg,"lotmfg");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.inpromo,"inpromo");
            cm.snapsPar(o.orbitsource,"orbitsource");
            cm.snapsPar(o.orbitsite,"orbitsite");
            cm.snapsPar(o.orbitdepot,"orbitdepot");
            cm.snapsPar(o.inagrn,"inagrn");
            return cm;
        }

        //Correction
        private void correctionCommand(ref List<SqlCommand> cm, List<orbit_correction> o) { 
            foreach(orbit_correction ln in o){ 
                cm.Add(new SqlCommand(sqlInterface_landing_correction,cn));
                cm.Last().snapsPar(ln.orgcode,"orgcode");
                cm.Last().snapsPar(ln.site,"site");
                cm.Last().snapsPar(ln.depot,"depot");
                cm.Last().snapsPar(ln.dateops,"dateops");
                cm.Last().snapsPar(ln.accnops,"accnops");
                cm.Last().snapsPar(ln.seqops,"seqops");
                cm.Last().snapsPar(ln.codeops,"codeops");
                cm.Last().snapsPar(ln.typeops,"typeops");
                cm.Last().snapsPar(ln.thcode,"thcode");
                cm.Last().snapsPar(ln.article,"article");
                cm.Last().snapsPar(ln.pv,"pv");
                cm.Last().snapsPar(ln.lv,"lv");
                cm.Last().snapsPar(ln.unitops,"unitops");
                cm.Last().snapsPar(ln.qtysku,"qtysku");
                cm.Last().snapsPar(ln.qtyweight,"qtyweight");
                cm.Last().snapsPar(ln.inreftype,"inreftype");
                cm.Last().snapsPar(ln.inrefno,"inrefno");
                cm.Last().snapsPar(ln.ingrno,"ingrno");
                cm.Last().snapsPar(ln.inpromo,"inpromo");
                cm.Last().snapsPar(ln.reason,"reason");
                cm.Last().snapsPar(ln.rowid,"rowid");
            }
        }

        //Delivery 
        private void deliveryCommand(ref List<SqlCommand> cm, List<orbit_delivery> o) { 
            foreach(orbit_delivery ln in o){ 
                cm.Add(new SqlCommand(sqlInterface_landing_delivery,cn));
                cm.Last().snapsPar(ln.orgcode,"orgcode");
                cm.Last().snapsPar(ln.site,"site");
                cm.Last().snapsPar(ln.depot,"depot");
                cm.Last().snapsPar(ln.dateops,"dateops");
                cm.Last().snapsPar(ln.routeops,"routeops");
                cm.Last().snapsPar(ln.transportno,"transportno");
                cm.Last().snapsPar(ln.thcode,"thcode");
                cm.Last().snapsPar(ln.thtype,"thtype");
                cm.Last().snapsPar(ln.dropship,"dropship");
                cm.Last().snapsPar(ln.ouorder,"ouorder");
                cm.Last().snapsPar(ln.ouline,"ouline");
                cm.Last().snapsPar(ln.ourefno,"ourefno");
                cm.Last().snapsPar(ln.ourefln,"ourefln");
                cm.Last().snapsPar(ln.oudnno,"oudnno");
                cm.Last().snapsPar(ln.inorder,"inorder");
                cm.Last().snapsPar(ln.ingrno,"ingrno");
                cm.Last().snapsPar(ln.article,"article");
                cm.Last().snapsPar(ln.pv,"pv");
                cm.Last().snapsPar(ln.lv,"lv");
                cm.Last().snapsPar(ln.unitops,"unitops");
                cm.Last().snapsPar(ln.qtysku,"qtysku");
                cm.Last().snapsPar(ln.qtypu,"qtypu");
                cm.Last().snapsPar(ln.qtyweight,"qtyweight");
                cm.Last().snapsPar(ln.qtyvolume,"qtyvolume");
                cm.Last().snapsPar(ln.dateexp,"dateexp");
                cm.Last().snapsPar(ln.datemfg,"datemfg");
                cm.Last().snapsPar(ln.lotmfg,"lotmfg");
                cm.Last().snapsPar(ln.serialno,"serialno");
                cm.Last().snapsPar(ln.huno,"huno");
                cm.Last().snapsPar(ln.accnops,"accnops");
                cm.Last().snapsPar(ln.oupromo,"oupromo");
                cm.Last().snapsPar(ln.rowid,"rowid");
            }
        }

    }
}