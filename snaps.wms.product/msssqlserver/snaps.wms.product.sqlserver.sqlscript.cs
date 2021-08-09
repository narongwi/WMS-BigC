using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.product
{
    
    public partial class product_ops : IDisposable { 

        private string sqlFind_product = "select p.orgcode,p.site,p.depot,p.spcarea,article,pv,lv, articletype,descalt,p.thcode, " +
        " case when p.ismeasurement = 1 then 'RM' else p.tflow end tflow,thname from wm_product p left join wm_thparty t on      " + 
        " p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot and p.thcode = t.thcode where p.orgcode = @orgcode     "+ 
        " and p.site = @site and p.depot = @depot ";
        //private string sqlGet_product = @"select p.*,t.thnameint thname,
        //d.bndesc hdivisionname, e.bndesc hdepartmentname, s.bndesc hsubdepartname, c.bndesc hclassname, l.bndesc hsubclassname
        //from  wm_product p 
        //    left join (select orgcode,site,depot,bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'DIV' 
        //    and orgcode = @orgcode and site = @site and depot = @depot) d
        //    on p.orgcode = d.orgcode and p.site = d.site and p.depot = d.depot and p.hdivison = d.bnvalue
        //    left join (select orgcode,site,depot,bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'DEP' 
        //    and orgcode = @orgcode and site = @site and depot = @depot) e
        //    on p.orgcode = e.orgcode and p.site = e.site and p.depot = e.depot and p.hdepartment = e.bnvalue
        //    left join (select orgcode,site,depot,bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'SDP' 
        //    and orgcode = @orgcode and site = @site and depot = @depot) s
        //    on p.orgcode = s.orgcode and p.site = s.site and p.depot = s.depot and p.hsubdepart = s.bnvalue
        //    left join (select orgcode,site,depot,bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'CLS' 
        //    and orgcode = @orgcode and site = @site and depot = @depot) c
        //    on p.orgcode = c.orgcode and p.site = c.site and p.depot = c.depot and p.hclass = c.bnvalue
        //    left join (select orgcode,site,depot,bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'SCL' 
        //    and orgcode = @orgcode and site = @site and depot = @depot) l
        //    on p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.hsubclass = l.bnvalue
        //    left join wm_thparty t on p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot and p.thcode = t.thcode
        //where p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.article = @article and p.pv = @pv and p.lv = @lv";

        private string sqlGet_product = @"select p.*,t.thnameint thname, d.bndesc hdivisionname, e.bndesc hdepartmentname, s.bndesc hsubdepartname, c.bndesc hclassname, l.bndesc hsubclassname
        from  wm_product p left join (
	        select orgcode,site,depot,RIGHT(REPLICATE('0', 10) + bnvalue,10) as bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'DIV'
	        and orgcode = @orgcode and site = @site and depot = @depot
	        ) d on p.orgcode = d.orgcode and p.site = d.site and p.depot = d.depot and RIGHT(REPLICATE('0', 10)+ p.hdivison,10) =   d.bnvalue
            left join (select orgcode,site,depot,RIGHT(REPLICATE('0', 10) + bnvalue , 10) bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'DEP'
            and orgcode = @orgcode and site = @site and depot =  @depot) e
            on p.orgcode = e.orgcode and p.site = e.site and p.depot = e.depot and RIGHT(REPLICATE('0', 10)+p.hdepartment,10) = e.bnvalue
            left join (select orgcode,site,depot,RIGHT(REPLICATE('0', 10)+ bnvalue,10) bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'SDP'
            and orgcode = @orgcode and site = @site and depot =  @depot) s
            on p.orgcode = s.orgcode and p.site = s.site and p.depot = s.depot and RIGHT(REPLICATE('0', 10)+p.hsubdepart,10) = s.bnvalue
            left join (select orgcode,site,depot,RIGHT(REPLICATE('0', 10)+ bnvalue,10) bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'CLS'
            and orgcode = @orgcode and site = @site and depot =  @depot) c
            on p.orgcode = c.orgcode and p.site = c.site and p.depot = c.depot and RIGHT(REPLICATE('0', 10)+ p.hclass,10)  = c.bnvalue
            left join (select orgcode,site,depot,RIGHT(REPLICATE('0', 10)+ bnvalue,10) bnvalue,bndesc from wm_binary where bntype = 'PRODUCT' and bncode = 'SCL'
            and orgcode = @orgcode and site = @site and depot =  @depot) l
            on p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and RIGHT(REPLICATE('0', 10)+p.hsubclass,10)  = l.bnvalue
            left join wm_thparty t on p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot and p.thcode = t.thcode
        where p.orgcode = @orgcode and p.site = @site and p.depot =  @depot and p.article = @article and p.pv = @pv and p.lv = @lv";

        private String sqlproduct_insert = "insert into " + tbn + 
        " ( orgcode, site, depot, spcarea, article, articletype, pv, lv, description, descalt, thcode, dlcall, dlcfactory, dlcwarehouse, "+ 
        " dlcshop, dlconsumer, hdivison, hdepartment, hsubdepart, hclass, hsubclass, typemanage, unitmanage, unitreceipt, unitprep, "+ 
        " unitsale, unitstock, unitweight, unitdimension, unitvolume, hucode, skulength, skuwidth, skuheight, skugrossweight, "+ 
        " skuweight, skuvolume, rtoskuofpu, rtoskuipck, rtoipckpck, rtopckoflayer, rtolayerofhu, innaturalloss, ounaturalloss, cost, "+ 
        " pulength, puwidth, puheight, pugrossweight, puweight, puvolume, ipcklength, ipckwidth, ipckheight, ipckgrossweight, "+ 
        " ipckweight, ipckvolume, pcklength, pckwidth, pckheight, pckgrossweight, pckweight, pckvolume, layerlength, layerwidth, "+ 
        " layerheight, layergrossweight, layerweight, layervolume, hulength, huwidth, huheight, hugrossweight, huweight, huvolume, "+ 
        " tempmin, tempmax, isdangerous, ishighvalue, isfastmove, isslowmove, isprescription, isdlc, ismaterial, isunique, isalcohol, "+ 
        " istemperature, isimflammable, ismixingprep, isfinishgoods, temptype, alcmanage, alccategory, alccontent, alccolor, "+ 
        " dangercategory, dangerlevel, thresholdmin, spczone, spcbay, spclevel, spclocation, spchdivison, spchcategory, "+ 
        " spcpivot, cronhand, cronblock, crincmng, croudeliv, laslotno, lasdatemfg, lasdateexp, lasserialno, tflow, "+ 
        " datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
        " values "  +
        " ( @orgcode, @site, @depot, @spcarea, @article, @articletype, @pv, @lv, @description, @descalt, @thcode, @dlcall, @dlcfactory, "+ 
        " @dlcwarehouse, @dlcshop, @dlconsumer, @hdivison, @hdepartment, @hsubdepart, @hclass, @hsubclass, @typemanage, "+ 
        " @unitmanage, @unitreceipt, @unitprep, @unitsale, @unitstock, @unitweight, @unitdimension, @unitvolume, @hucode, "+ 
        " @skulength, @skuwidth, @skuheight, @skugrossweight, @skuweight, @skuvolume, @rtoskuofpu, @rtoskuipck, @rtoipckpck, "+ 
        " @rtopckoflayer, @rtolayerofhu, @innaturalloss, @ounaturalloss, @cost, @pulength, @puwidth, @puheight, @pugrossweight, "+ 
        " @puweight, @puvolume, @ipcklength, @ipckwidth, @ipckheight, @ipckgrossweight, @ipckweight, @ipckvolume, @pcklength, "+ 
        " @pckwidth, @pckheight, @pckgrossweight, @pckweight, @pckvolume, @layerlength, @layerwidth, @layerheight, @layergrossweight, "+ 
        " @layerweight, @layervolume, @hulength, @huwidth, @huheight, @hugrossweight, @huweight, @huvolume, @tempmin, @tempmax, "+ 
        " @isdangerous, @ishighvalue, @isfastmove, @isslowmove, @isprescription, @isdlc, @ismaterial, @isunique, @isalcohol, "+ 
        " @istemperature, @isimflammable, @ismixingprep, @isfinishgoods, @temptype, @alcmanage, @alccategory, @alccontent, "+ 
        " @alccolor, @dangercategory, @dangerlevel, @thresholdmin, @spczone, @spcbay, @spclevel, @spclocation, @spchdivison, "+ 
        " @spchcategory, @spcpivot, @cronhand, @cronblock, @crincmng, @croudeliv, @laslotno, @lasdatemfg, @lasdateexp, "+ 
        " @lasserialno, @tflow, @sysdate, @accncreate, @sysdate, @accnmodify, @procmodify) ";
        private string sqlproduct_update_hs = "" + 
        " insert into wm_producthx ( orgcode,site ,depot,spcarea,article,articletype,pv,lv,description,descalt,thcode,dlcall,dlcfactory,dlcwarehouse,dlcshop,dlconsumer " +
        " ,hdivison,hdepartment,hsubdepart,hclass,hsubclass,typemanage,unitmanage,unitdesc,unitreceipt,unitprep,unitsale,unitstock,unitweight       " + 
        " ,unitdimension,unitvolume,hucode,rtoskuofpu,rtoskuofipck,rtoskuofpck,rtoskuoflayer,rtoskuofhu,rtoipckofpck,rtopckoflayer,rtolayerofhu     " + 
        " ,innaturalloss,ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight,skugrossweight,skuweight,skuvolume             " + 
        " ,pulength,puwidth,puheight,pugrossweight,puweight,puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight,ipckvolume          " + 
        " ,pcklength,pckwidth,pckheight,pckgrossweight,pckweight,pckvolume,layerlength,layerwidth,layerheight,layergrossweight,layerweight          " + 
        " ,layervolume,hulength,huwidth,huheight,hugrossweight,huweight,huvolume,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription       " + 
        " ,isdlc,ismaterial,isunique,isalcohol,istemperature,isdynamicpick,ismixingprep,isfinishgoods,isnaturalloss,isbatchno,ismeasurement         " + 
        " ,roomtype,tempmin,tempmax,alcmanage,alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin,stockthresholdmax        " + 
        " ,spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation,spcprepzone,spcdistzone,spcdistshare,spczonedelv,orbitsource            " + 
        " ,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,lasrecv,lasdelivery,lasbatchno,laslotno    " + 
        " ,lasdatemfg,lasdateexp,lasserialno,spcrecvaisleto, spcrecvbayto, spcrecvlevelto )                                                                                                      " + 
        " SELECT orgcode,site ,depot,spcarea,article,articletype,pv,lv,description,descalt,thcode,dlcall,dlcfactory,dlcwarehouse,dlcshop,dlconsumer " + 
        " ,hdivison,hdepartment,hsubdepart,hclass,hsubclass,typemanage,unitmanage,unitdesc,unitreceipt,unitprep,unitsale,unitstock,unitweight       " +
        " ,unitdimension,unitvolume,hucode,rtoskuofpu,rtoskuofipck,rtoskuofpck,rtoskuoflayer,rtoskuofhu,rtoipckofpck,rtopckoflayer,rtolayerofhu     " +
        " ,innaturalloss,ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight,skugrossweight,skuweight,skuvolume             " +
        " ,pulength,puwidth,puheight,pugrossweight,puweight,puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight,ipckvolume          " +
        " ,pcklength,pckwidth,pckheight,pckgrossweight,pckweight,pckvolume,layerlength,layerwidth,layerheight,layergrossweight,layerweight          " +
        " ,layervolume,hulength,huwidth,huheight,hugrossweight,huweight,huvolume,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription       " +
        " ,isdlc,ismaterial,isunique,isalcohol,istemperature,isdynamicpick,ismixingprep,isfinishgoods,isnaturalloss,isbatchno,ismeasurement         " +
        " ,roomtype,tempmin,tempmax,alcmanage,alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin,stockthresholdmax        " +
        " ,spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation,spcprepzone,spcdistzone,spcdistshare,spczonedelv,orbitsource            " +
        " ,tflow,datecreate,accncreate,sysdatetimeoffset() datemodify, @accnmodify accnmodify,procmodify,lasrecv,lasdelivery,lasbatchno,laslotno    " +
        " ,lasdatemfg,lasdateexp,lasserialno, spcrecvaisleto, spcrecvbayto, spcrecvlevelto " + 
        " from dbo.wm_product where orgcode = @orgcode and site = @site and depot = @depot " +
        " and article = @article and pv = @pv and lv = @lv " ;
        private String sqlproduct_update = " update wm_product set " + 
        " spcarea = @spcarea, descalt = @descalt,thcode = @thcode,dlcall = @dlcall,dlcfactory = @dlcfactory,dlcwarehouse = @dlcwarehouse,           " + 
        " dlcshop = @dlcshop, dlconsumer = @dlconsumer, hdivison = @hdivison, hdepartment = @hdepartment, hsubdepart = @hsubdepart,                 " + 
        " hclass = @hclass, hsubclass = @hsubclass, typemanage = @typemanage, unitmanage = @unitmanage, unitdesc = @unitdesc,                       " + 
        " unitreceipt = @unitreceipt, unitprep = @unitprep, unitsale = @unitsale, unitstock = @unitstock, unitweight = @unitweight,                 " + 
        " unitdimension = @unitdimension, unitvolume = @unitvolume, hucode = @hucode, "+ 
        " rtoskuofpu = case when @unitprep = 1 then 1 when @unitprep = 2 then @rtoskuofipck when @unitprep = 3 then @rtoskuofpck when @unitprep = 4 " + 
        " then @rtoskuoflayer when @unitprep = 5 then @rtoskuofhu end , " +
        " rtoskuofipck = @rtoskuofipck,       " + 
        " rtoskuofpck = @rtoskuofpck, rtoskuoflayer = @rtoskuoflayer, rtoskuofhu = @rtoskuofhu, rtopckoflayer = @rtopckoflayer,                     " + 
        " rtolayerofhu = @rtolayerofhu, innaturalloss = @innaturalloss, ounaturalloss = @ounaturalloss, costinbound = @costinbound,                 " + 
        " costoutbound = @costoutbound, costavg = @costavg, skulength = @skulength, skuwidth = @skuwidth, skuheight = @skuheight,                   " + 
        " skugrossweight = @skugrossweight, skuweight = @skuweight, skuvolume = @skuvolume, pulength = @pulength, puwidth = @puwidth,               " + 
        " puheight = @puheight, pugrossweight = @pugrossweight, puweight = @puweight, puvolume = @puvolume, ipcklength = @ipcklength,               " + 
        " ipckwidth = @ipckwidth, ipckheight = @ipckheight, ipckgrossweight = @ipckgrossweight, ipckweight = @ipckweight, ipckvolume = @ipckvolume, " + 
        " pcklength = @pcklength, pckwidth = @pckwidth, pckheight = @pckheight, pckgrossweight = @pckgrossweight, pckweight = @pckweight,           " + 
        " pckvolume = @pckvolume, layerlength = @layerlength, layerwidth = @layerwidth, layerheight = @layerheight,                                 " + 
        " layergrossweight = @layergrossweight, layerweight = @layerweight, layervolume = @layervolume, hulength = @hulength, huwidth = @huwidth,   " + 
        " huheight = @huheight, hugrossweight = @hugrossweight, huweight = @huweight, huvolume = @huvolume, isdangerous = @isdangerous,             " + 
        " ishighvalue = @ishighvalue, isfastmove = @isfastmove, isslowmove = @isslowmove, isprescription = @isprescription, isdlc = @isdlc,         " + 
        " ismaterial = @ismaterial, isunique = @isunique, isalcohol = @isalcohol, istemperature = @istemperature, isdynamicpick = @isdynamicpick,   " + 
        " ismixingprep = @ismixingprep, isfinishgoods = @isfinishgoods, isnaturalloss = @isnaturalloss, isbatchno = @isbatchno,                     " + 
        " ismeasurement = @ismeasurement, roomtype = @roomtype, tempmin = @tempmin, tempmax = @tempmax, alcmanage = @alcmanage,                     " + 
        " alccategory = @alccategory, alccontent = @alccontent, alccolor = @alccolor, dangercategory = @dangercategory, dangerlevel = @dangerlevel, " + 
        " stockthresholdmin = @stockthresholdmin, stockthresholdmax = @stockthresholdmax, spcrecvzone = @spcrecvzone, spcrecvaisle = @spcrecvaisle, " + 
        " spcrecvbay = @spcrecvbay, spcrecvlevel = @spcrecvlevel, spcrecvlocation = @spcrecvlocation, spcprepzone = @spcprepzone,                   " +
        " spcdistzone = @spcdistzone, spcdistshare = @spcdistshare, spczonedelv = @spczonedelv, orbitsource = @orbitsource,                         " +
        " tflow = @tflow, datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = @procmodify,   rtoipckofpck = @rtoipckofpck,      " +
        " spcrecvaisleto = @spcrecvaisleto, spcrecvbayto = @spcrecvbayto, spcrecvlevelto = @spcrecvlevelto " +
        " where orgcode = @orgcode and site = @site and depot = @depot and article = @article and articletype = @articletype and pv = @pv and lv = @lv ";
        private String sqlinx = "insert into ix" + tbn + 
        " ( orgcode,site, depot, spcarea, article, articletype, pv, lv, description, descalt, thcode, dlcall, dlcfactory, dlcwarehouse, dlcshop, dlconsumer, hdivison, "+ 
        " hdepartment, hsubdepart, hclass, hsubclass, typemanage, unitmanage, unitreceipt, unitprep, unitsale, unitstock, unitweight, unitdimension, unitvolume,  "+ 
        " hucode, skulength, skuwidth, skuheight, skugrossweight, skuweight, skuvolume, rtoskuofpu, rtoskuipck, rtoipckpck, rtopckoflayer, rtolayerofhu, innaturalloss,  "+ 
        " ounaturalloss, cost, pulength, puwidth, puheight, pugrossweight, puweight, puvolume, ipcklength, ipckwidth, ipckheight, ipckgrossweight, ipckweight,  "+ 
        " ipckvolume, pcklength, pckwidth, pckheight, pckgrossweight, pckweight, pckvolume, layerlength, layerwidth, layerheight, layergrossweight, layerweight,  "+ 
        " layervolume, hulength, huwidth, huheight, hugrossweight, huweight, huvolume, tempmin, tempmax, tflow, fileid, rowops, ermsg, dateops) " + 
        " values " + 
        " ( @orgcode, @site, @depot, @spcarea, @article, @articletype, @pv, @lv, @description, @descalt, @thcode, @dlcall, @dlcfactory, @dlcwarehouse, @dlcshop, @dlconsumer,  "+ 
        " @hdivison, @hdepartment, @hsubdepart, @hclass, @hsubclass, @typemanage, @unitmanage, @unitreceipt, @unitprep, @unitsale, @unitstock, @unitweight,  "+ 
        " @unitdimension, @unitvolume, @hucode, @skulength, @skuwidth, @skuheight, @skugrossweight, @skuweight, @skuvolume, @rtoskuofpu, @rtoskuipck, @rtoipckpck,  "+ 
        " @rtopckoflayer, @rtolayerofhu, @innaturalloss, @ounaturalloss, @cost, @pulength, @puwidth, @puheight, @pugrossweight, @puweight, @puvolume, @ipcklength,  "+ 
        " @ipckwidth, @ipckheight, @ipckgrossweight, @ipckweight, @ipckvolume, @pcklength, @pckwidth, @pckheight, @pckgrossweight, @pckweight, @pckvolume, @layerlength, "+ 
        " @layerwidth, @layerheight, @layergrossweight, @layerweight, @layervolume, @hulength, @huwidth, @huheight, @hugrossweight, @huweight, @huvolume, @tempmin,  "+ 
        " @tempmax, @tflow, @fileid, @rowops, @ermsg, @dateops) ";    
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  

        //Validate location 
        //Aisle
        public string sqlvalloc_aisle = " select count(1) rsl from wm_locup where lsaisle = @loc and orgcode = @orgcode and site = @site and depot = @depot ";
        //Bay 
        public string sqlvalloc_bay = " select count(1) rsl from wm_locup where lsbay = @loc and orgcode = @orgcode and site = @site and depot = @depot ";
        //Level
        public string sqlvalloc_level = " select count(1) rsl from wm_locup where lslevel = @loc and orgcode = @orgcode and site = @site and depot = @depot ";
        //Location
        public string sqlvalloc_location = " select count(1) rsl from wm_locdw where lscode = @loc and orgcode = @orgcode and site = @site and depot = @depot ";


        private string sqlFind_productact = @"select top 1 p.orgcode,p.site,p.depot,p.spcarea, b.barcode,b.article,b.pv,b.lv,p.descalt,p.rtoskuofpu,p.rtopckoflayer,p.rtolayerofhu,
        (p.rtopckoflayer * p.rtolayerofhu) rtopckofpallet,p.rtoskuofipck,p.rtoskuofpck,p.rtoskuoflayer,p.rtoskuofhu,p.unitprep,p.unitreceipt,p.unitmanage, p.unitdesc,p.articletype,
        skulength,skuwidth,skuheight,skugrossweight,skuweight,skuvolume
        from wm_barcode b ,wm_product p where b.orgcode = p.orgcode and b.site = p.site and b.depot = p.depot and b.article = p.article and b.pv = p.pv and b.lv = p.lv and b.orgcode = @orgcode 
        and b.site = @site and b.depot = @depot and b.isprimary = 1 and p.tflow = 'IO' and b.article = (select top 1 t.article from wm_barcode t where t.orgcode = @orgcode and t.site = @site 
        and t.depot = @depot and (t.article = @productCode or  t.barcode =@productCode))";


    }
}