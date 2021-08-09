using System;
using Snaps.Helpers.StringExt;
using System.Data.SqlClient;
namespace Snaps.WMS {
    public class exsProduct { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string spcarea    { get; set; }
        public string article    { get; set; }
        public string articletype    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string description    { get; set; }
        public string descalt    { get; set; }
        public string thcode    { get; set; }
        public Int32 dlcall    { get; set; }
        public Int32 dlcfactory    { get; set; }
        public Int32 dlcwarehouse    { get; set; }
        public Int32 dlcshop    { get; set; }
        public Int32 dlconsumer    { get; set; }
        public string hdivison    { get; set; }
        public string hdepartment    { get; set; }
        public string hsubdepart    { get; set; }
        public string hclass    { get; set; }
        public string hsubclass    { get; set; }
        public string typemanage    { get; set; }
        public string unitmanage    { get; set; }
        public string unitdesc    { get; set; }
        public string unitreceipt    { get; set; }
        public string unitprep    { get; set; }
        public string unitsale    { get; set; }
        public string unitstock    { get; set; }
        public string unitweight    { get; set; }
        public string unitdimension    { get; set; }
        public string unitvolume    { get; set; }
        public string hucode    { get; set; }
        public Int32 rtoskuofpu    { get; set; }
        public Int32 rtoskuofipck    { get; set; }
        public Int32 rtoskuofpck    { get; set; }
        public Int32 rtoskuoflayer    { get; set; }
        public Int32 rtoskuofhu    { get; set; }
        public Int32 rtopckoflayer    { get; set; }
        public Int32 rtolayerofhu    { get; set; }
        public Int32 innaturalloss    { get; set; }
        public Int32 ounaturalloss    { get; set; }
        public Decimal costinbound    { get; set; }
        public Decimal costoutbound    { get; set; }
        public Decimal costavg    { get; set; }
        public Decimal skulength    { get; set; }
        public Decimal skuwidth    { get; set; }
        public Decimal skuheight    { get; set; }
        public Decimal skugrossweight    { get; set; }
        public Decimal skuweight    { get; set; }
        public Decimal skuvolume    { get; set; }
        public Decimal pulength    { get; set; }
        public Decimal puwidth    { get; set; }
        public Decimal puheight    { get; set; }
        public Decimal pugrossweight    { get; set; }
        public Decimal puweight    { get; set; }
        public Decimal puvolume    { get; set; }
        public Decimal ipcklength    { get; set; }
        public Decimal ipckwidth    { get; set; }
        public Decimal ipckheight    { get; set; }
        public Decimal ipckgrossweight    { get; set; }
        public Decimal ipckweight    { get; set; }
        public Decimal ipckvolume    { get; set; }
        public Decimal pcklength    { get; set; }
        public Decimal pckwidth    { get; set; }
        public Decimal pckheight    { get; set; }
        public Decimal pckgrossweight    { get; set; }
        public Decimal pckweight    { get; set; }
        public Decimal pckvolume    { get; set; }
        public Decimal layerlength    { get; set; }
        public Decimal layerwidth    { get; set; }
        public Decimal layerheight    { get; set; }
        public Decimal layergrossweight    { get; set; }
        public Decimal layerweight    { get; set; }
        public Decimal layervolume    { get; set; }
        public Decimal hulength    { get; set; }
        public Decimal huwidth    { get; set; }
        public Decimal huheight    { get; set; }
        public Decimal hugrossweight    { get; set; }
        public Decimal huweight    { get; set; }
        public Decimal huvolume    { get; set; }
        public Int32 isdangerous    { get; set; }
        public Int32 ishighvalue    { get; set; }
        public Int32 isfastmove    { get; set; }
        public Int32 isslowmove    { get; set; }
        public Int32 isprescription    { get; set; }
        public Int32 isdlc    { get; set; }
        public Int32 ismaterial    { get; set; }
        public Int32 isunique    { get; set; }
        public Int32 isalcohol    { get; set; }
        public Int32 istemperature    { get; set; }
        public Int32 isdynamicpick    { get; set; }
        public Int32 ismixingprep    { get; set; }
        public Int32 isfinishgoods    { get; set; }
        public Int32 isnaturalloss    { get; set; }
        public Int32 isbatchno    { get; set; }
        public Int32 ismeasurement    { get; set; }
        public string roomtype    { get; set; }
        public Int32 tempmin    { get; set; }
        public Int32 tempmax    { get; set; }
        public string alcmanage    { get; set; }
        public string alccategory    { get; set; }
        public string alccontent    { get; set; }
        public string alccolor    { get; set; }
        public string dangercategory    { get; set; }
        public string dangerlevel    { get; set; }
        public Int32 stockthresholdmin    { get; set; }
        public Int32 stockthresholdmax    { get; set; }
        public string spcrecvzone    { get; set; }
        public string spcrecvaisle    { get; set; }
        public string spcrecvbay    { get; set; }
        public string spcrecvlevel    { get; set; }
        public string spcrecvlocation    { get; set; }
        public string spcprepzone    { get; set; }
        public string spcdistzone    { get; set; }
        public string spcdistshare    { get; set; }
        public string spczonedelv    { get; set; }
        public string orbitsource    { get; set; }
        public string tflow    { get; set; }
        public string fileid    { get; set; }
        public string rowops    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public string ermsg    { get; set; }
        public Decimal rowid    { get; set; }
        public Int32 rtoipckofpck { get; set; }
        public exsProduct() {}
        public exsProduct(string orgcode, string site, string depot, string csv){ 
            string[] rsl = csv.Split(',');
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.article = rsl[0].ToString().Trim().ClearReg();
            this.articletype = rsl[1].ToString().Trim().ClearReg();
            this.pv = rsl[2].ToString().Trim().ClearReg().CInt32();
            this.lv = rsl[3].ToString().Trim().ClearReg().CInt32();
            this.description = rsl[4].ToString().Trim().ClearReg();
            this.descalt = rsl[5].ToString().Trim().ClearReg();
            this.thcode = rsl[6].ToString().Trim().ClearReg();
            this.dlcall = rsl[7].ToString().Trim().ClearReg().CInt32();
            this.dlcfactory = rsl[8].ToString().Trim().ClearReg().CInt32();
            this.dlcwarehouse = rsl[9].ToString().Trim().ClearReg().CInt32();
            this.dlcshop = rsl[10].ToString().Trim().ClearReg().CInt32();
            this.dlconsumer = rsl[11].ToString().Trim().ClearReg().CInt32();
            this.hdivison = rsl[12].ToString().Trim().ClearReg();
            this.hdepartment = rsl[13].ToString().Trim().ClearReg();
            this.hsubdepart = rsl[14].ToString().Trim().ClearReg();
            this.hclass = rsl[15].ToString().Trim().ClearReg();
            this.hsubclass = rsl[16].ToString().Trim().ClearReg();
            this.typemanage = rsl[17].ToString().Trim().ClearReg();
            this.unitmanage = rsl[18].ToString().Trim().ClearReg();
            this.unitdesc = rsl[19].ToString().Trim().ClearReg();
            this.unitreceipt = rsl[20].ToString().Trim().ClearReg();
            this.unitprep = rsl[21].ToString().Trim().ClearReg();
            this.unitsale = rsl[22].ToString().Trim().ClearReg();
            this.unitstock = rsl[23].ToString().Trim().ClearReg();
            this.unitweight = rsl[24].ToString().Trim().ClearReg();
            this.unitdimension = rsl[25].ToString().Trim().ClearReg();
            this.unitvolume = rsl[26].ToString().Trim().ClearReg();
            this.hucode = rsl[27].ToString().Trim().ClearReg();
            this.rtoskuofpu = rsl[28].ToString().Trim().ClearReg().CInt32();
            this.rtoskuofipck = rsl[29].ToString().Trim().ClearReg().CInt32();
            this.rtoskuofpck = rsl[30].ToString().Trim().ClearReg().CInt32();
            this.rtoskuoflayer = rsl[31].ToString().Trim().ClearReg().CInt32();
            this.rtoskuofhu = rsl[32].ToString().Trim().ClearReg().CInt32();
            this.rtoipckofpck = rsl[33].ToString().Trim().ClearReg().CInt32();
            this.rtopckoflayer = rsl[34].ToString().Trim().ClearReg().CInt32();
            this.rtolayerofhu = rsl[35].ToString().Trim().ClearReg().CInt32();
            this.innaturalloss = rsl[36].ToString().Trim().ClearReg().CInt32();
            this.ounaturalloss = rsl[37].ToString().Trim().ClearReg().CInt32();
            this.costinbound = rsl[38].ToString().Trim().ClearReg().CInt32();
            this.costoutbound = rsl[39].ToString().Trim().ClearReg().CInt32();
            this.costavg = rsl[40].ToString().Trim().ClearReg().CInt32();
            this.skulength = rsl[41].ToString().Trim().ClearReg().CDecimal();
            this.skuwidth = rsl[42].ToString().Trim().ClearReg().CDecimal();
            this.skuheight = rsl[43].ToString().Trim().ClearReg().CDecimal();
            this.skugrossweight = rsl[44].ToString().Trim().ClearReg().CDecimal();
            this.skuweight = rsl[45].ToString().Trim().ClearReg().CDecimal();
            this.skuvolume = rsl[46].ToString().Trim().ClearReg().CDecimal();
            this.pulength = rsl[47].ToString().Trim().ClearReg().CDecimal();
            this.puwidth = rsl[48].ToString().Trim().ClearReg().CDecimal();
            this.puheight = rsl[49].ToString().Trim().ClearReg().CDecimal();
            this.pugrossweight = rsl[50].ToString().Trim().ClearReg().CDecimal();
            this.puweight = rsl[51].ToString().Trim().ClearReg().CDecimal();
            this.puvolume = rsl[52].ToString().Trim().ClearReg().CDecimal();
            this.ipcklength = rsl[53].ToString().Trim().ClearReg().CDecimal();
            this.ipckwidth = rsl[54].ToString().Trim().ClearReg().CDecimal();
            this.ipckheight = rsl[55].ToString().Trim().ClearReg().CDecimal();
            this.ipckgrossweight = rsl[56].ToString().Trim().ClearReg().CDecimal();
            this.ipckweight = rsl[57].ToString().Trim().ClearReg().CDecimal();
            this.ipckvolume = rsl[58].ToString().Trim().ClearReg().CDecimal();
            this.pcklength = rsl[59].ToString().Trim().ClearReg().CDecimal();
            this.pckwidth = rsl[60].ToString().Trim().ClearReg().CDecimal();
            this.pckheight = rsl[61].ToString().Trim().ClearReg().CDecimal();
            this.pckgrossweight = rsl[62].ToString().Trim().ClearReg().CDecimal();
            this.pckweight = rsl[63].ToString().Trim().ClearReg().CDecimal();
            this.pckvolume = rsl[64].ToString().Trim().ClearReg().CDecimal();
            this.layerlength = rsl[65].ToString().Trim().ClearReg().CDecimal();
            this.layerwidth = rsl[66].ToString().Trim().ClearReg().CDecimal();
            this.layerheight = rsl[67].ToString().Trim().ClearReg().CDecimal();
            this.layergrossweight = rsl[68].ToString().Trim().ClearReg().CDecimal();
            this.layerweight = rsl[69].ToString().Trim().ClearReg().CDecimal();
            this.layervolume = rsl[70].ToString().Trim().ClearReg().CDecimal();
            this.hulength = rsl[71].ToString().Trim().ClearReg().CDecimal();
            this.huwidth = rsl[72].ToString().Trim().ClearReg().CDecimal();
            this.huheight = rsl[73].ToString().Trim().ClearReg().CDecimal();
            this.hugrossweight = rsl[74].ToString().Trim().ClearReg().CDecimal();
            this.huweight = rsl[75].ToString().Trim().ClearReg().CDecimal();
            this.huvolume = rsl[76].ToString().Trim().ClearReg().CDecimal();
            this.isdangerous = rsl[77].ToString().Trim().ClearReg().CInt32();
            this.ishighvalue = rsl[78].ToString().Trim().ClearReg().CInt32();
            this.isfastmove = rsl[79].ToString().Trim().ClearReg().CInt32();
            this.isslowmove = rsl[80].ToString().Trim().ClearReg().CInt32();
            this.isprescription = rsl[81].ToString().Trim().ClearReg().CInt32();
            this.isdlc = rsl[82].ToString().Trim().ClearReg().CInt32();
            this.ismaterial = rsl[83].ToString().Trim().ClearReg().CInt32();
            this.isunique = rsl[84].ToString().Trim().ClearReg().CInt32();
            this.isalcohol = rsl[85].ToString().Trim().ClearReg().CInt32();
            this.istemperature = rsl[86].ToString().Trim().ClearReg().CInt32();
            this.isdynamicpick = rsl[87].ToString().Trim().ClearReg().CInt32();
            this.ismixingprep = rsl[88].ToString().Trim().ClearReg().CInt32();
            this.isfinishgoods = rsl[89].ToString().Trim().ClearReg().CInt32();
            this.isnaturalloss = rsl[90].ToString().Trim().ClearReg().CInt32();
            this.isbatchno = rsl[91].ToString().Trim().ClearReg().CInt32();
            this.ismeasurement = rsl[92].ToString().Trim().ClearReg().CInt32();
            this.roomtype = rsl[93].ToString().Trim().ClearReg();
            this.tempmin = rsl[94].ToString().Trim().ClearReg().CInt32();
            this.tempmax = rsl[95].ToString().Trim().ClearReg().CInt32();
            this.alcmanage = rsl[96].ToString().Trim().ClearReg();
            this.alccategory = rsl[97].ToString().Trim().ClearReg();
            this.alccontent = rsl[98].ToString().Trim().ClearReg();
            this.alccolor = rsl[99].ToString().Trim().ClearReg();
            this.dangercategory = rsl[100].ToString().Trim().ClearReg();
            this.dangerlevel = rsl[101].ToString().Trim().ClearReg();
            this.stockthresholdmin = rsl[102].ToString().Trim().ClearReg().CInt32();
            this.stockthresholdmax = rsl[103].ToString().Trim().ClearReg().CInt32();
            this.spcrecvzone = rsl[104].ToString().Trim().ClearReg();
            this.spcrecvaisle = rsl[105].ToString().Trim().ClearReg();
            this.spcrecvbay = rsl[106].ToString().Trim().ClearReg();
            this.spcrecvlevel = rsl[107].ToString().Trim().ClearReg();
            this.spcrecvlocation = rsl[108].ToString().Trim().ClearReg();
            this.spcprepzone = rsl[109].ToString().Trim().ClearReg();
            this.spcdistzone = rsl[110].ToString().Trim().ClearReg();
            this.spcdistshare = rsl[111].ToString().Trim().ClearReg();
            this.spczonedelv = rsl[112].ToString().Trim().ClearReg();
            this.rowops = rsl[113].ToString().Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
            
        }
        public exsProduct(string orgcode, string site, string depot, string article,string articletype,string pv,string lv,string description,string descalt,string thcode,string dlcall,string dlcfactory,string dlcwarehouse,string dlcshop,
        string dlconsumer,string hdivison,string hdepartment,string hsubdepart,string hclass,string hsubclass,string typemanage,string unitmanage,string unitdesc,
        string unitreceipt,string unitprep,string unitsale,string unitstock,string unitweight,string unitdimension,string unitvolume,string hucode,string rtoskuofpu,
        string rtoskuofipck,string rtoskuofpck,string rtoskuoflayer,string rtoskuofhu,string rtoipckofpck,string rtopckoflayer,string rtolayerofhu,string innaturalloss,
        string ounaturalloss,string costinbound,string costoutbound,string costavg,string skulength,string skuwidth,string skuheight,string skugrossweight,
        string skuweight,string skuvolume,string pulength,string puwidth,string puheight,string pugrossweight,string puweight,string puvolume,string ipcklength,
        string ipckwidth,string ipckheight,string ipckgrossweight,string ipckweight,string ipckvolume,string pcklength,string pckwidth,string pckheight,
        string pckgrossweight,string pckweight,string pckvolume,string layerlength,string layerwidth,string layerheight,string layergrossweight,
        string layerweight,string layervolume,string hulength,string huwidth,string huheight,string hugrossweight,string huweight,string huvolume,
        string isdangerous,string ishighvalue,string isfastmove,string isslowmove,string isprescription,string isdlc,string ismaterial,string isunique,string isalcohol,
        string istemperature,string isdynamicpick,string ismixingprep,string isfinishgoods,string isnaturalloss,string isbatchno,string ismeasurement,string roomtype,
        string tempmin,string tempmax,string alcmanage,string alccategory,string alccontent,string alccolor,string dangercategory,string dangerlevel,
        string stockthresholdmin,string stockthresholdmax,string spcrecvzone,string spcrecvaisle,string spcrecvbay,string spcrecvlevel,string spcrecvlocation,
        string spcprepzone,string spcdistzone,string spcdistshare,string spczonedelv,string rowops
        ) {
            this.orgcode = orgcode;
            this.site = site;
            this.depot = depot;
            this.article = article.Trim().ClearReg();
            this.articletype = articletype.Trim().ClearReg();
            this.pv = pv.Trim().ClearReg().CInt32();
            this.lv = lv.Trim().ClearReg().CInt32();
            this.description = description.Trim().ClearReg();
            this.descalt = descalt.Trim().ClearReg();
            this.thcode = thcode.Trim().ClearReg();
            this.dlcall = dlcall.Trim().ClearReg().CInt32();
            this.dlcfactory = dlcfactory.Trim().ClearReg().CInt32();
            this.dlcwarehouse = dlcwarehouse.Trim().ClearReg().CInt32();
            this.dlcshop = dlcshop.Trim().ClearReg().CInt32();
            this.dlconsumer = dlconsumer.Trim().ClearReg().CInt32();
            this.hdivison = hdivison.Trim().ClearReg();
            this.hdepartment = hdepartment.Trim().ClearReg();
            this.hsubdepart = hsubdepart.Trim().ClearReg();
            this.hclass = hclass.Trim().ClearReg();
            this.hsubclass = hsubclass.Trim().ClearReg();
            this.typemanage = typemanage.Trim().ClearReg();
            this.unitmanage = unitmanage.Trim().ClearReg();
            this.unitdesc = unitdesc.Trim().ClearReg();
            this.unitreceipt = unitreceipt.Trim().ClearReg();
            this.unitprep = unitprep.Trim().ClearReg();
            this.unitsale = unitsale.Trim().ClearReg();
            this.unitstock = unitstock.Trim().ClearReg();
            this.unitweight = unitweight.Trim().ClearReg();
            this.unitdimension = unitdimension.Trim().ClearReg();
            this.unitvolume = unitvolume.Trim().ClearReg();
            this.hucode = hucode.Trim().ClearReg();
            this.rtoskuofpu = rtoskuofpu.Trim().ClearReg().CInt32();
            this.rtoskuofipck = rtoskuofipck.Trim().ClearReg().CInt32();
            this.rtoskuofpck = rtoskuofpck.Trim().ClearReg().CInt32();
            this.rtoskuoflayer = rtoskuoflayer.Trim().ClearReg().CInt32();
            this.rtoskuofhu = rtoskuofhu.Trim().ClearReg().CInt32();
            this.rtopckoflayer = rtopckoflayer.Trim().ClearReg().CInt32();
            this.rtolayerofhu = rtolayerofhu.Trim().ClearReg().CInt32();
            this.innaturalloss = innaturalloss.Trim().ClearReg().CInt32();
            this.ounaturalloss = ounaturalloss.Trim().ClearReg().CInt32();
            this.costinbound = costinbound.Trim().ClearReg().CDecimal();
            this.costoutbound = costoutbound.Trim().ClearReg().CDecimal();
            this.costavg = costavg.Trim().ClearReg().CDecimal();
            this.skulength = skulength.Trim().ClearReg().CDecimal();
            this.skuwidth = skuwidth.Trim().ClearReg().CDecimal();
            this.skuheight = skuheight.Trim().ClearReg().CDecimal();
            this.skugrossweight = skugrossweight.Trim().ClearReg().CDecimal();
            this.skuweight = skuweight.Trim().ClearReg().CDecimal();
            this.skuvolume = skuvolume.Trim().ClearReg().CDecimal();
            this.pulength = pulength.Trim().ClearReg().CDecimal();
            this.puwidth = puwidth.Trim().ClearReg().CDecimal();
            this.puheight = puheight.Trim().ClearReg().CDecimal();
            this.pugrossweight = pugrossweight.Trim().ClearReg().CDecimal();
            this.puweight = puweight.Trim().ClearReg().CDecimal();
            this.puvolume = puvolume.Trim().ClearReg().CDecimal();
            this.ipcklength = ipcklength.Trim().ClearReg().CDecimal();
            this.ipckwidth = ipckwidth.Trim().ClearReg().CDecimal();
            this.ipckheight = ipckheight.Trim().ClearReg().CDecimal();
            this.ipckgrossweight = ipckgrossweight.Trim().ClearReg().CDecimal();
            this.ipckweight = ipckweight.Trim().ClearReg().CDecimal();
            this.ipckvolume = ipckvolume.Trim().ClearReg().CDecimal();
            this.pcklength = pcklength.Trim().ClearReg().CDecimal();
            this.pckwidth = pckwidth.Trim().ClearReg().CDecimal();
            this.pckheight = pckheight.Trim().ClearReg().CDecimal();
            this.pckgrossweight = pckgrossweight.Trim().ClearReg().CDecimal();
            this.pckweight = pckweight.Trim().ClearReg().CDecimal();
            this.pckvolume = pckvolume.Trim().ClearReg().CDecimal();
            this.layerlength = layerlength.Trim().ClearReg().CDecimal();
            this.layerwidth = layerwidth.Trim().ClearReg().CDecimal();
            this.layerheight = layerheight.Trim().ClearReg().CDecimal();
            this.layergrossweight = layergrossweight.Trim().ClearReg().CDecimal();
            this.layerweight = layerweight.Trim().ClearReg().CDecimal();
            this.layervolume = layervolume.Trim().ClearReg().CDecimal();
            this.hulength = hulength.Trim().ClearReg().CDecimal();
            this.huwidth = huwidth.Trim().ClearReg().CDecimal();
            this.huheight = huheight.Trim().ClearReg().CDecimal();
            this.hugrossweight = hugrossweight.Trim().ClearReg().CDecimal();
            this.huweight = huweight.Trim().ClearReg().CDecimal();
            this.huvolume = huvolume.Trim().ClearReg().CDecimal();
            this.isdangerous = isdangerous.Trim().ClearReg().CInt32();
            this.ishighvalue = ishighvalue.Trim().ClearReg().CInt32();
            this.isfastmove = isfastmove.Trim().ClearReg().CInt32();
            this.isslowmove = isslowmove.Trim().ClearReg().CInt32();
            this.isprescription = isprescription.Trim().ClearReg().CInt32();
            this.isdlc = isdlc.Trim().ClearReg().CInt32();
            this.ismaterial = ismaterial.Trim().ClearReg().CInt32();
            this.isunique = isunique.Trim().ClearReg().CInt32();
            this.isalcohol = isalcohol.Trim().ClearReg().CInt32();
            this.istemperature = istemperature.Trim().ClearReg().CInt32();
            this.isdynamicpick = isdynamicpick.Trim().ClearReg().CInt32();
            this.ismixingprep = ismixingprep.Trim().ClearReg().CInt32();
            this.isfinishgoods = isfinishgoods.Trim().ClearReg().CInt32();
            this.isnaturalloss = isnaturalloss.Trim().ClearReg().CInt32();
            this.isbatchno = isbatchno.Trim().ClearReg().CInt32();
            this.ismeasurement = ismeasurement.Trim().ClearReg().CInt32();
            this.roomtype = roomtype.Trim().ClearReg();
            this.tempmin = tempmin.Trim().ClearReg().CInt32();
            this.tempmax = tempmax.Trim().ClearReg().CInt32();
            this.alcmanage = alcmanage.Trim().ClearReg();
            this.alccategory = alccategory.Trim().ClearReg();
            this.alccontent = alccontent.Trim().ClearReg();
            this.alccolor = alccolor.Trim().ClearReg();
            this.dangercategory = dangercategory.Trim().ClearReg();
            this.dangerlevel = dangerlevel.Trim().ClearReg();
            this.stockthresholdmin = stockthresholdmin.Trim().ClearReg().CInt32();
            this.stockthresholdmax = stockthresholdmax.Trim().ClearReg().CInt32();
            this.spcrecvzone = spcrecvzone.Trim().ClearReg();
            this.spcrecvaisle = spcrecvaisle.Trim().ClearReg();
            this.spcrecvbay = spcrecvbay.Trim().ClearReg();
            this.spcrecvlevel = spcrecvlevel.Trim().ClearReg();
            this.spcrecvlocation = spcrecvlocation.Trim().ClearReg();
            this.spcprepzone = spcprepzone.Trim().ClearReg();
            this.spcdistzone = spcdistzone.Trim().ClearReg();
            this.spcdistshare = spcdistshare.Trim().ClearReg();
            this.spczonedelv = spczonedelv.Trim().ClearReg();
            this.orbitsource = orbitsource.Trim().ClearReg();
            this.tflow = tflow.Trim().ClearReg();
            this.rowops = rowops.Trim().ClearReg();
            this.orbitsource = "External.CSV";
            this.tflow = "WC";
            this.fileid = "0";            
            this.ermsg = "";
            this.dateops = DateTimeOffset.Now;
            this.rowid = 0;
        }
        public exsProduct(ref SqlDataReader r){ 
            this.orgcode = r["orgcode"].ToString();
            this.site = r["site"].ToString();
            this.depot = r["depot"].ToString();
            this.spcarea = r["spcarea"].ToString();
            this.article = r["article"].ToString();
            this.articletype = r["articletype"].ToString();
            this.pv = (r.IsDBNull(6)) ? 0 :  r.GetInt32(6);
            this.lv = (r.IsDBNull(7)) ? 0 :  r.GetInt32(7);
            this.description = r["description"].ToString();
            this.descalt = r["descalt"].ToString();
            this.thcode = r["thcode"].ToString();
            this.dlcall = (r.IsDBNull(11)) ? 0 : r["dlcall"].ToString().CInt32();
            this.dlcfactory = (r.IsDBNull(12)) ? 0 :  r["dlcfactory"].ToString().CInt32();
            this.dlcwarehouse = (r.IsDBNull(13)) ? 0 :  r["dlcwarehouse"].ToString().CInt32();
            this.dlcshop = (r.IsDBNull(14)) ? 0 :  r["dlcshop"].ToString().CInt32();
            this.dlconsumer = (r.IsDBNull(15)) ? 0 : r["dlconsumer"].ToString().CInt32();
            this.hdivison = r["hdivison"].ToString();
            this.hdepartment = r["hdepartment"].ToString();
            this.hsubdepart = r["hsubdepart"].ToString();
            this.hclass = r["hclass"].ToString();
            this.hsubclass = r["hsubclass"].ToString();
            this.typemanage = r["typemanage"].ToString();
            this.unitmanage = r["unitmanage"].ToString();
            this.unitdesc = r["unitdesc"].ToString();
            this.unitreceipt = r["unitreceipt"].ToString();
            this.unitprep = r["unitprep"].ToString();
            this.unitsale = r["unitsale"].ToString();
            this.unitstock = r["unitstock"].ToString();
            this.unitweight = r["unitweight"].ToString();
            this.unitdimension = r["unitdimension"].ToString();
            this.unitvolume = r["unitvolume"].ToString();
            this.hucode = r["hucode"].ToString();
            this.rtoskuofpu = (r.IsDBNull(32)) ? 0 :  r.GetInt32(32);
            this.rtoskuofipck = (r.IsDBNull(33)) ? 0 :  r.GetInt32(33);
            this.rtoskuofpck = (r.IsDBNull(34)) ? 0 :  r.GetInt32(34);
            this.rtoskuoflayer = (r.IsDBNull(35)) ? 0 :  r.GetInt32(35);
            this.rtoskuofhu = (r.IsDBNull(36)) ? 0 :  r.GetInt32(36);
            this.rtoipckofpck = (r.IsDBNull(37)) ? 0 :  r.GetInt32(37);
            this.rtopckoflayer = (r.IsDBNull(38)) ? 0 :  r.GetInt32(38);
            this.rtolayerofhu = (r.IsDBNull(39)) ? 0 :  r.GetInt32(39);
            this.innaturalloss = (r.IsDBNull(40)) ? 0 :  r["innaturalloss"].ToString().CInt32();
            this.ounaturalloss = (r.IsDBNull(41)) ? 0 : r["ounaturalloss"].ToString().CInt32();
            this.costinbound =  (r.IsDBNull(42)) ? 0 : r.GetDecimal(42);
            this.costoutbound =  (r.IsDBNull(43)) ? 0 : r.GetDecimal(43);
            this.costavg =  (r.IsDBNull(44)) ? 0 : r.GetDecimal(44);
            this.skulength =  (r.IsDBNull(45)) ? 0 : r.GetDecimal(45);
            this.skuwidth =  (r.IsDBNull(46)) ? 0 : r.GetDecimal(46);
            this.skuheight =  (r.IsDBNull(47)) ? 0 : r.GetDecimal(47);
            this.skugrossweight =  (r.IsDBNull(48)) ? 0 : r.GetDecimal(48);
            this.skuweight =  (r.IsDBNull(49)) ? 0 : r.GetDecimal(49);
            this.skuvolume =  (r.IsDBNull(50)) ? 0 : r.GetDecimal(50);
            this.pulength =  (r.IsDBNull(51)) ? 0 : r.GetDecimal(51);
            this.puwidth =  (r.IsDBNull(52)) ? 0 : r.GetDecimal(52);
            this.puheight =  (r.IsDBNull(53)) ? 0 : r.GetDecimal(53);
            this.pugrossweight =  (r.IsDBNull(54)) ? 0 : r.GetDecimal(54);
            this.puweight =  (r.IsDBNull(55)) ? 0 : r.GetDecimal(55);
            this.puvolume =  (r.IsDBNull(56)) ? 0 : r.GetDecimal(56);
            this.ipcklength =  (r.IsDBNull(57)) ? 0 : r.GetDecimal(57);
            this.ipckwidth =  (r.IsDBNull(58)) ? 0 : r.GetDecimal(58);
            this.ipckheight =  (r.IsDBNull(59)) ? 0 : r.GetDecimal(59);
            this.ipckgrossweight =  (r.IsDBNull(60)) ? 0 : r.GetDecimal(60);
            this.ipckweight =  (r.IsDBNull(61)) ? 0 : r.GetDecimal(61);
            this.ipckvolume =  (r.IsDBNull(62)) ? 0 : r.GetDecimal(62);
            this.pcklength =  (r.IsDBNull(63)) ? 0 : r.GetDecimal(63);
            this.pckwidth =  (r.IsDBNull(64)) ? 0 : r.GetDecimal(64);
            this.pckheight =  (r.IsDBNull(65)) ? 0 : r.GetDecimal(65);
            this.pckgrossweight =  (r.IsDBNull(66)) ? 0 : r.GetDecimal(66);
            this.pckweight =  (r.IsDBNull(67)) ? 0 : r.GetDecimal(67);
            this.pckvolume =  (r.IsDBNull(68)) ? 0 : r.GetDecimal(68);
            this.layerlength =  (r.IsDBNull(69)) ? 0 : r.GetDecimal(69);
            this.layerwidth =  (r.IsDBNull(70)) ? 0 : r.GetDecimal(70);
            this.layerheight =  (r.IsDBNull(71)) ? 0 : r.GetDecimal(71);
            this.layergrossweight =  (r.IsDBNull(72)) ? 0 : r.GetDecimal(72);
            this.layerweight =  (r.IsDBNull(73)) ? 0 : r.GetDecimal(73);
            this.layervolume =  (r.IsDBNull(74)) ? 0 : r.GetDecimal(74);
            this.hulength =  (r.IsDBNull(75)) ? 0 : r.GetDecimal(75);
            this.huwidth =  (r.IsDBNull(76)) ? 0 : r.GetDecimal(76);
            this.huheight =  (r.IsDBNull(77)) ? 0 : r.GetDecimal(77);
            this.hugrossweight =  (r.IsDBNull(78)) ? 0 : r.GetDecimal(78);
            this.huweight =  (r.IsDBNull(79)) ? 0 : r.GetDecimal(79);
            this.huvolume =  (r.IsDBNull(80)) ? 0 : r.GetDecimal(80);
            this.isdangerous = (r.IsDBNull(81)) ? 0 :  r.GetInt32(81);
            this.ishighvalue = (r.IsDBNull(82)) ? 0 :  r.GetInt32(82);
            this.isfastmove = (r.IsDBNull(83)) ? 0 :  r.GetInt32(83);
            this.isslowmove = (r.IsDBNull(84)) ? 0 :  r.GetInt32(84);
            this.isprescription = (r.IsDBNull(85)) ? 0 :  r.GetInt32(85);
            this.isdlc = (r.IsDBNull(86)) ? 0 :  r.GetInt32(86);
            this.ismaterial = (r.IsDBNull(87)) ? 0 :  r.GetInt32(87);
            this.isunique = (r.IsDBNull(88)) ? 0 :  r.GetInt32(88);
            this.isalcohol = (r.IsDBNull(89)) ? 0 :  r.GetInt32(89);
            this.istemperature = (r.IsDBNull(90)) ? 0 :  r.GetInt32(90);
            this.isdynamicpick = (r.IsDBNull(91)) ? 0 :  r.GetInt32(91);
            this.ismixingprep = (r.IsDBNull(92)) ? 0 :  r.GetInt32(92);
            this.isfinishgoods = (r.IsDBNull(93)) ? 0 :  r.GetInt32(93);
            this.isnaturalloss = (r.IsDBNull(94)) ? 0 :  r.GetInt32(94);
            this.isbatchno = (r.IsDBNull(95)) ? 0 :  r.GetInt32(95);
            this.ismeasurement = (r.IsDBNull(96)) ? 0 :  r.GetInt32(96);
            this.roomtype = r["roomtype"].ToString();
            this.tempmin = (r.IsDBNull(98)) ? 0 :  r["tempmin"].ToString().CInt32();
            this.tempmax = (r.IsDBNull(99)) ? 0 :  r["tempmax"].ToString().CInt32();
            this.alcmanage = r["alcmanage"].ToString();
            this.alccategory = r["alccategory"].ToString();
            this.alccontent = r["alccontent"].ToString();
            this.alccolor = r["alccolor"].ToString();
            this.dangercategory = r["dangercategory"].ToString();
            this.dangerlevel = r["dangerlevel"].ToString();
            this.stockthresholdmin = (r.IsDBNull(106)) ? 0 :  r.GetInt32(106);
            this.stockthresholdmax = (r.IsDBNull(107)) ? 0 :  r.GetInt32(107);
            this.spcrecvzone = r["spcrecvzone"].ToString();
            this.spcrecvaisle = r["spcrecvaisle"].ToString();
            this.spcrecvbay = r["spcrecvbay"].ToString();
            this.spcrecvlevel = r["spcrecvlevel"].ToString();
            this.spcrecvlocation = r["spcrecvlocation"].ToString();
            this.spcprepzone = r["spcprepzone"].ToString();
            this.spcdistzone = r["spcdistzone"].ToString();
            this.spcdistshare = r["spcdistshare"].ToString();
            this.spczonedelv = r["spczonedelv"].ToString();
            this.rowops = r["rowops"].ToString();
        }
    
    }
}