using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.Hash;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public partial class depot_ops : IDisposable { 

        private string sqldepot_validate = "";
        private string sqldepot_fnd = " select * from wm_depot where orgcode = @orgcode and sitecode = @site ";

        private string sqldepot_update = @"update wm_depot set depottype = @depottype,depotname = @depotname,depotnamealt = @depotnamealt,datestart = @datestart,
        dateend = @dateend,depotops = @depotops,tflow = @tflow,depothash = @depothash,unitweight = @unitweight,unitvolume = @unitvolume,
        unitdimension = @unitdimension,unitcubic = @unitcubic,formatdate = @formatdate,formatdateshort = @formatdateshort,
        formatdatelong = @formatdatelong,datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify
        where orgcode = @orgcode and site = @site and depot = @depot";
        private string sqldepot_remove_step1 = " delete from wm_barcode where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step2 = " delete from wm_thparty where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step3 = " delete from wm_policy where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step4 = " delete from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step5 = " delete from wm_product where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step6 = " delete from wm_handerlingunit where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step7 = " delete from wm_role where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step8 = " delete from wm_parameters where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step9 = " delete from wm_binary where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqldepot_remove_step10 = " delete from wm_depot where orgcode = @orgcode and sitecode = @site and depotcode = @depot ";

        private string sqldepot_add_step1 = " insert into wm_barcode (orgcode,site,depot,article,pv,lv,barops,barcode,bartype,thcode,tflow, " + 
        " barremarks,datecreate,accncreate,datemodify,accnmodify,procmodify,orbitsource,isprimary) " + 
        " values (@orgcode,@site,@depot,'Article',0,0,'A','Barcode','EAN13',null,'IO',null,sysdatetimeoffset(), " + 
        " @accncode,sysdatetimeoffset(),@accncode,'Initial','Initial',1) ";

        private string sqldepot_add_step2 = @"INSERT INTO wm_thparty([orgcode],[site],[depot],[spcarea],[thbutype],[thtype],[thcode],[thcodealt],[vatcode],[thname],
        [thnamealt],[thnameint],[addressln1],[addressln2],[addressln3],[subdistrict],[district],[city],[country],[postcode],[region],[telephone],[email],[thgroup],
        [thcomment],[throuteformat],[plandelivery],[naturalloss],[mapaddress],[carriercode],[orbitsource],[timeslotday],[timeslothourmin],[timeslothourmax],[tflow],
        [datecreate],[accncreate],[datemodify],[accnmodify],[procmodify],[indock],[oudock])VALUES(@orgcode,@site,@depot,NULL,'1','1','000','000','000',  
        'Default Thirdparty','Default Thirdparty','Default Thirdparty','Addressln1','Addressln2','','','','Bangkok','Thailand','10000','','','','','','','0','0.000',
        '',NULL,'Initial', NULL,NULL,NULL,'IO',NULL,NULL,sysdatetimeoffset(),'Initial',NULL,NULL,NULL)";

        private string sqldepot_add_step3 = @" INSERT INTO wm_policy([orgcode],[site],[depot],[apcode],[plccode],[plcname],[tflow],[reqnumeric],[requppercase]," + 
        " [reqlowercase],[reqspecialchar],[spcchar],[minlength],[maxauthfail],[exppdamobile],[expandriod],[expios],[seckey],[dayexpire],[hashplc],[datestart]," +
        " [dateend],[datecreate],[accncreate],[datemodify],[accnmodify],[procmodify],[suffixpriv])VALUES(@orgcode,@site,@depot,'WMS','DEF','Default policy','IO', "+ 
        "'1','1','1','1','','6','18','1','1','1','b132e9d302eaa6abf27433bde9896271041af06eb7922c31e64bba37f14e50d8','90','000',NULL,NULL,sysdatetimeoffset(), " +
        " @accncode,sysdatetimeoffset(),@accncode,'Initial','') ";

        private string sqldepot_add_step4 = @"INSERT INTO wm_loczn([orgcode],[site],[depot],[spcarea],[przone],[przonename],[przonedesc],[hutype],[tflow],[datecreate],[accncreate],
        [datemodify],[accnmodify],[procmodify],[huvalweight],[huvalvolume],[hucapweight],[hucapvolume],[isprimary]) VALUES (@orgcode,@site,@depot,'ST','DF-ST','Default zone',
        'Default Preparation zone','PL01','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,'Initial',1000,10000,100,100,'1'),(@orgcode,@site,@depot,'XD','DF-XD',
        'Default zone','Default Distribution zone','PL01','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,'Initial',1000,10000,100,100,'1'),(@orgcode,@site,
        @depot,'XD','DF-FW','Default zone','Default Forward zone','PL01','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,'Initial',1000,10000,100,100,'1')";

        private string sqldepot_add_step5 = @"INSERT INTO wm_product([orgcode],[site],[depot],[spcarea],[article],[articletype],[pv],[lv],[description],[descalt],
        [thcode],[dlcall],[dlcfactory],[dlcwarehouse],[dlcshop],[dlconsumer],[hdivison],[hdepartment],[hsubdepart],[hclass],[hsubclass],[typemanage],[unitmanage],
        [unitdesc],[unitreceipt],[unitprep],[unitsale],[unitstock],[unitweight],[unitdimension],[unitvolume],[hucode],[rtoskuofpu],[rtoskuofipck],[rtoskuofpck],
        [rtoskuoflayer],[rtoskuofhu],[rtoipckofpck],[rtopckoflayer],[rtolayerofhu],[innaturalloss],[ounaturalloss],[costinbound],[costoutbound],[costavg],
        [skulength],[skuwidth],[skuheight],[skugrossweight],[skuweight],[skuvolume],[pulength],[puwidth],[puheight],[pugrossweight],[puweight],[puvolume],
        [ipcklength],[ipckwidth],[ipckheight],[ipckgrossweight],[ipckweight],[ipckvolume],[pcklength],[pckwidth],[pckheight],[pckgrossweight],[pckweight],
        [pckvolume],[layerlength],[layerwidth],[layerheight],[layergrossweight],[layerweight],[layervolume],[hulength],[huwidth],[huheight],[hugrossweight],
        [huweight],[huvolume],[isdangerous],[ishighvalue],[isfastmove],[isslowmove],[isprescription],[isdlc],[ismaterial],[isunique],[isalcohol],[istemperature],
        [isdynamicpick],[ismixingprep],[isfinishgoods],[isnaturalloss],[isbatchno],[ismeasurement],[roomtype],[tempmin],[tempmax],[alcmanage],[alccategory],
        [alccontent],[alccolor],[dangercategory],[dangerlevel],[stockthresholdmin],[stockthresholdmax],[spcrecvzone],[spcrecvaisle],[spcrecvbay],[spcrecvlevel],
        [spcrecvlocation],[spcprepzone],[spcdistzone],[spcdistshare],[spczonedelv],[orbitsource],[tflow],[datecreate],[accncreate],[datemodify],[accnmodify],
        [procmodify],[lasrecv],[lasdelivery],[lasbatchno],[laslotno],[lasdatemfg],[lasdateexp],[lasserialno])VALUES(@orgcode,@site,@depot,'','PL01','P','0',
        '0','Pallet','Pallet','01','0','15','60','0','0','01','01','01','01','01','PP','1','01','1','1','','','','','','','1','1','1','1','1','1','1','12',
        '0.000','0.000','0.00000',NULL,NULL,'1.00000','1.00000','1.00000','0.00000','1.00000','0.00100','0.00000','0.00000','0.00000','0.00000','0.00000',
        '0.00000','2.00000','2.00000','2.00000','0.00000','2.00000','0.00000','24.00000','24.00000','24.00000','0.00000','24.00000','0.00000','72.00000',
        '72.00000','72.00000','0.00000','72.00000','0.00000','430.00000','430.00000','430.00000','0.00000','430.00000','0.00000','0','0','0','0','0','0',
        '0','0','0','0','0','0','0','0','0','1','','0.000','0.000','','','','','','','0',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'IO',
        sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,'Initial',NULL,NULL,NULL,'',sysdatetimeoffset(),sysdatetimeoffset(),'')";

        private string sqldepot_add_step6 = " insert into wm_handerlingunit " + 
        " (orgcode,site,depot,thcode,hutype,huno,loccode,mxsku,mxweight,mxvolume,crsku,crweight,crvolume, " +
        " crcapacity,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,priority) values  " +
        " ( @orgcode,@site,@depot,NULL,'DEFHU','DEFHU001','DEFLOC',99999,9999999,9999999,0,0,0,100,'IO', " +
        " sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,'Initial',0) " ;

        private string sqldepot_add_step7 = @"insert into wm_role([orgcode],[apcode],[site],[depot],[rolecode],[rolename],[roledesc],[tflow],[hashrol],[datecreate]," + 
        " [accncreate],[datemodify],[accnmodify],[procmodify],[roljson],[roletype]) values ( @orgcode,'WMS',@site,@depot,'ROLEDEF','Default role product', " + 
        " 'Default row profile','IO','000',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,'Initial',NULL,'Admin' )";
        private string sqldepot_add_step8 = " insert into wm_parameters ([orgcode],[site],[depot],[apps],[pmmodule],[pmtype],[pmcode],[pmvalue],[pmdesc],[pmdescalt],[pmstate],[datecreate],[accncreate],[datemodify],[accnmodify],[pmseq],[pmoption]) values " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowcalculatemfg','1','Allow to calculdate product MFG or Expire','Auto calculate expire dat or manufactoring date when input each other','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowpartail','1','Allow to partial receipt product','Allow to partial confirm receiption on Inbound order ( Order will be never close )','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowoverplan','1','Allow to receipt inbound order that over plan date','Allow to receipt inbound order that over plan date','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowexpired','1','Allow to receive product has expired ( Warehouse DLC )','Allow to receipt product that over dlc warehouse','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowchangeunit','1','Allow to change unit of receiption','Allow to change unit type when receive return product','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowshowqtyorder','1','Allow to visibility of inbound order line quantity','Appear quantity of order for receiption process','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowautostaging','1','Allow to auto assign inbound staging','Allow system auto assign staging for receive ( must specific staging )','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowcontrolcapacity','1','Allow to control staging capacity','Allow system calculate estimate Inbound coming HU vs current staging capacity','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowchangepriority','1','Allow to modify priority of order','Allow to worker change priority of order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,9,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowcancel','0','Allow to cancel an inbound order','Allow to worker cancel an order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowgendistplan','1','Allow to auto generate distribution prep for crossdock','Allow to create distribute preparation when confirm receive order of crossdock','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,11,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowreplandelivery','1','Enforce to auto generate task putaway for crossdock','Enforce system generate task putaway movement to bulk for crossdock order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,12,''), " + 
        "(@orgcode, @site, @depot,'WMS','inbound','receipt','allowgenstckputaway','1','Enforce to auto generate task putaway for stocking','Enforce system generate task putaway movement to bulk for stocking order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,13,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','product','allowchangehirachy','1','Enforce to block modification product hiracy that came from other legacy.','Block any modification product that came from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','product','allowchangedimension','1','Enforce to block modification product dimension that came from other lagecy.','Worker can not modification product dimension that came from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','product','allowchangedlc','1','Enforce to block modification product downloadable content ( DLC ) from other legacy.','Worker can not modification product dlc from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','product','allowchangeunit','1','Enforce to block modification product unit from other legacy.','Worker can not modification product unit that came from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','thirdparty','allowchangeofexsource','1','Enforce to block Plan delivery that came from other legacy.','Block any modification plan delivery date that came from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','thirdparty','allowchangeplandate','1','Enforce to block Address info that came from other legacy.','Worker can not modification third party address that came from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','thirdparty','allowchangestate','1','Enforce to block modification third party state that came from other legacy.','Worker can not modification third party state from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','master','barcode','allowchangeofexsource','1','Enforce to block modification that came from other legacy.','Block any modification that came from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowscanhuongrap','1','Allow to scan HU.no for grap the task.','Allow to worker can select task to do by himself','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowautoassign','1','Enforce to auto assign task to worker.','Enforce to auto assign task to worker.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowscansourcelocation','1','Enforce to scan source location','Enforce to worker must scan source target location ( staging or dock ).','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowscanbarcode','1','Enforce to scan barcode on HU','Enforce to worker must scan barcode on HU to verify product','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowinputqtyongrap','1','Enforce to input quantity on HU','Enforce to worker must must input quantity in HU No.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowpickndrop','1','Enforce to use pick and drop process','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowcheckdigit','1','Enforce to input check digit of target location','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowfullygrap','1','Enforce to fully grap on source location','System will not allow to revise quantity when grap source location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowfullycollect','1','Enforce to put full HU on target location','System will not allow to revise quantity when confirm target location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,9,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','putaway','allowchangetarget','1','Allow to change target location','Allow worker change target location when operate','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowchangeworker','1','Allow to change worker when task has start','Allow to change worker when task has stated but still wait for completed','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowscanhuno','1','Allow to scan HU.no for grap the task','Allow to worker can select task to do by himself','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowautoassign','1','Enforce to auto assign task to worker','Enforce to auto assign task to worker.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowscansourcelocation','1','Enforce to scan source location','Enforce to worker must scan source target location ( staging or dock ).','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowscanbarcode','1','Enforce to scan barcode on HU','Enforce to worker must scan barcode on HU to verify product','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowchangequantity','1','Enforce to input quantity on HU','Enforce to worker must must input quantity in HU No.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowpickndrop','1','Enforce to use pick and drop process','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowcheckdigit','1','Enforce to input check digit of target location','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowfullycollect','1','Enforce to put full HU on target location','System will not allow to revise quantity when confirm target location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,9,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowmanual','1','Allow to manual generate replenishment','Allow to manaual generate replenishe task by manaual from PDA Device','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowchangeworker','1','Allow to change worker when task has start','Allow to change worker when task has stated but still wait for completed','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowscanhuno','1','Allow to scan HU.no for grap the task','Allow to worker can select task to do by himself','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowscanbarcode','1','Enforce to scan barcode on HU','Enforce to worker must scan barcode on HU to verify product','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowautoassign','1','Enforce to auto assign task to worker','Enforce to auto assign task to worker.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowscansourcelocation','1','Enforce to scan source location','Enforce to worker must scan source target location ( staging or dock ).','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowchangequantity','1','Enforce to input quantity on HU','Enforce to worker must must input quantity in HU No.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowpickndrop','1','Enforce to use pick and drop process','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowcheckdigit','1','Enforce to input check digit of target location','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,9,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','replenishment','allowfullycollect','1','Enforce to put full HU on target location','System will not allow to revise quantity when confirm target location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowmanual','1','Allow to manual generate transfer','Allow to manaual generate transfer task','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowchangeworker','1','Allow to change worker when task has start','Allow to change worker when task has stated but still wait for completed','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowscanhuno','1','Allow to scan HU.no for grap the task','Allow to worker can select task to do by himself','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowautoassign','1','Enforce to auto assign task to worker','Enforce to auto assign task to worker.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowscansourcelocation','1','Enforce to scan source location','Enforce to worker must scan source target location ( staging or dock ).','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowscanbarcode','1','Enforce to scan barcode on HU','Enforce to worker must scan barcode on HU to verify product','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowchangequantity','1','Enforce to input quantity on HU','Enforce to worker must must input quantity in HU No.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowpickndrop','1','Enforce to use pick and drop process','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowcheckdigit','1','Enforce to input check digit of target location','Enforce to system must generate task include pick and drop location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,9,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowfullycollect','1','Enforce to put full HU on target location','System will not allow to revise quantity when confirm target location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','approach','allowchangetarget','1','Allow to change target location','Allow worker change target location when operate','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,''), " + 
        "(@orgcode, @site, @depot,'WMS','task','transfer','allowchangetarget','1','Allow to change target location','Allow worker change target location when operate','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','correction','allowblankremarks','1','Enforce to require remarks for correction','Worker must input remarks every transaction of correction','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','correction','allowchangeunit','1','Allow to change unit','Allow to change unit on correction process','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','correction','allowblankrefereceno','1','Enforce to require reference no ( Inbound order or Outbound Order )','Worker must input reference no. ( may be from other legacy ) every transaction.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','correction','allowprintlabelonreserve','1','Auto print label when correction to reserve location.','Auto printout label when correction stock from reserve','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','correction','allowgentaskfornewhu','1','Auto generate task transfer when generate a new HU.','System will generate movement task when correction to the new HU.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','correction','allowincludehubelongingtask','1','Enfore to correction stock that belong in task','System not allow to correction for HU that belong in the task movement','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowchangeunit','1','Allow to change unit for transfer','Worker can change transfer unit of product for transfer','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenreservetoreserve','1','Generate task movement for reserver to reserve','System will generate task movement for source and target is reserve','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,'PCK'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenreservetopicking','1','Generate task movement for reserve to picking','System will generate task movement for source and target is reserve','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,'IPCK'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenreservetobulk','1','Generate Task movement for Reserve to Bulk','System will generate task movement for Reserve to Bulk location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,'SKU'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenbulktoreserve','1','Generate Task movement for Bulk to Reserv','System will generate task movement for bulk to reserve location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,'SKU'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenbulktopicking','1','Generate Task movement for Bulk to Picking','System will generate task movement for bulk to picking location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,'SKU'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenbulktobulk','1','Generate Task movement for Bulk to Bulk','System will generate task movement for bulk to bulk location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,'IPCK'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenpickingtoreserve','1','Generate Task movement for Picking to Reserv','System will generate task movement for picking to reserve location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,'PCK'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenpickingtopicking','1','Generate Task movement for Picking to Picking','System will generate task movement for picking to picking location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,9,'LAYER'), " + 
        "(@orgcode, @site, @depot,'WMS','inventory','transfer','allowgenpickingtobulk','1','Generate Task movement for Picking to Bulk','System will generate task movement for picking to bulk location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,'LAYER'), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock','allowincludestaging','1','Allow to use inbound staging for process stock','System will include stock on staging to be process for preparation','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock','allowpartialprocess','1','Allow to partial order line for process stock ( Repoecess order )','Worker can partial select order line to process order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock','allowprocessbyselectline','0','Enforce to stock must equal or morethan orderline can be process only','Stock on hand must be equal or more than order quantity that can be process','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock','allowchangeunit','0','Allow to select order line by line to process','Worker can select order some line to process','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock','allowconsolidateorder','1','Allow to consolidate multiple order on 1 preparation','Systme will consolidate an order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock_mobile','mobiledigitIOloc','1','Mobile : Enforce to input check digit instead of scan location.','Worker must input check digit on location instead of scan location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock_mobile','mobilecheckdigit','1','Mobile : Enforce to input check digit of picking location.','Worker must input check digit on location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock_mobile','mobilescanbarcode','1','Mobile : Enforce to scan product barcode on location.','Worker must scan the barcode of product to confirm product on location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock_mobile','mobilefullypick','1','Mobile : Enforce to pick 100% ( can not change quantity ).','Worker can not change quantity of preparation','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,9,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock_mobile','mobilerepickforshortage','1','Mobile : Enforce to repick when some preparation is shortage.','Worker must repick for some preparation line is shortage.','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,10,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute','allowincludestaging','1','Allow to use inbound staging for process distribute','System will include stock on staging to be process for preparation','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute','allowpartialprocess','1','Allow to partial order line for process stock ( Repoecess order )','Worker can partial select order line to process order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute','allowstocklessthanorder','1','Enforce to stock must equal or morethan orderline can be process only','Stock on hand must be equal or more than order quantity that can be process','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute','allowprocessbyselectline','1','Allow to select order line by line to process','Worker can select order some line to process','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute_mobile','mobiledigitIOlc','1','Mobile : Enforce to input check digit instead of scan location.','Worker must input check digit on location instead of scan location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute_mobile','mobilecheckdigit','1','Mobile : Enforce to input check digit of distribution grid.','Worker must input check digit on location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute_mobile','mobilescanbarcode','1','Mobile : Enfroce to scan product barcode.','Worker must scan the barcode of product to confirm product on location','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','distribute_mobile','mobilefullypick','1','Mobile : Enforce to put 100% ( can not change quantity ).','Worker can not change quantity of distribution','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,8,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','opereate','allowcancel','1','Allow to cancel preparation on watiing start only','Worker can cancel preparation when state is waiting start','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','opereate','allowautoassign','1','Enforce to auto assign preparation task to worker','System will auto assign task when logon to Mobile','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','preparation','stock','allowstocklessthenorder','1','Enforce to stock must equal or morethan orderline can be process only','Stock on hand must be equal or more than order quantity that can be process','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','outbound','order','allowchangeofexsource','1','Enforce to block modification outbound order that came from other legacy.','Block any modification order that came from other legacy','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,1,''), " + 
        "(@orgcode, @site, @depot,'WMS','outbound','order','allowchangespcdlc','1','Allow to modify specific range of age in order line','Worker can modify specific range of age in order line','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,2,''), " + 
        "(@orgcode, @site, @depot,'WMS','outbound','order','allowchangereqdate','1','Allow to revise request delivery date','Worker can modify request delivery date of outbound order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,3,''), " + 
        "(@orgcode, @site, @depot,'WMS','outbound','order','allowchangespcbatch','1','Allow to modify batch specific on order line','Worker can modify batch no in outbound order line','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,4,''), " + 
        "(@orgcode, @site, @depot,'WMS','outbound','order','allowcancel','1','Allow to cancle outbound order','Worker can cancel an outbound order','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,5,''), " + 
        "(@orgcode, @site, @depot,'WMS','outbound','allocate','allocatehuwhenprepdone','1','Allow to allocate HU to route when preparation completed','Worker can allocate HU to route when preparation completed only','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,6,''), " + 
        "(@orgcode, @site, @depot,'WMS','outbound','delivery','allowrevisequantity','1','Allow to revise quantity before close shipment ( must generate task movement and specific location )','Worker can revise quantity before delivery to customer','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode,7,'')" ;


        private string sqldepot_add_step9 = "insert into wm_binary (orgcode,site,depot,apps,bntype,bncode,bnvalue,bndesc,bndescalt,bnflex1,bnflex2,bnflex3,bnflex4,bnicon,bnstate,datecreate,accncreate,datemodify,accnmodify) values " + 
        "(@orgcode, @site, @depot,'WMS','ACCOUNT','FLOW','BL','Blocked','Blocked','','','','','fas fa-stop-circle text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','FLOW','EX','Expired','Expired','','','','','fas fa-heart-broken text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','FLOW','XX','Deleted','Deleted','','','','','fas fa-trash-alt text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','FLOW','FG','Forgoting','Forgoting','','','','','fas fa-key text-muted','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','FLOW','PC','Waiting for profile','Waiting for profile','','','','','fas fa-spinner text-warning','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','FLOW','IO','Active','Active','','','','','fas fa-check-circle  text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','TYPE','Admin','Administrator','Administrator','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','TYPE','Support','Support','Support','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','TYPE','Manager','Manager','Manager','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','TYPE','Supervisor','Supervisor','Supervisor','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ACCOUNT','TYPE','Worker','Worker','Worker','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','FLOW','IO','Active','Active','','','','','fas fa-check-circle  text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','FLOW','IX','Active In operate only','Active In operate only','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','FLOW','XO','Active Out operate only','Active Out operate only','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','FLOW','WC','Waiting','Wainting ','','','','','fas fa-hourglass-half text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','FLOW','XX','Block','Block','','','','','fas fa-stop-circle text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','PRIORITY','50','Medium','Medium','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','PRIORITY','100','Critical','Critical','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','PRIORITY','75','High','High','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','PRIORITY','25','Low','Low','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ALL','PRIORITY','0','Lowest','Lowest','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','MANAGE','PP','Manage by piece','Manage by piece','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','MANAGE','WG','Manage by weight','Manage by weight','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','MANAGE','PT','Manage by liter','Manage by liter','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','MANAGE','PW','Manage by Piece / weight','Manage by Piece / weight','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','ROOMTYPE','Normal','Normal temp','Normal temp','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','TEMPERATURE','C','Celcius','Celcius','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','TEMPERATURE','F','Fahrenheit','Fahrenheit','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','TYPE','P','Handling Unit','Handling Unit','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','TYPE','G','Goods','Goods','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ARTICLE','TYPE','M','Material','Material','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ASSEMBLY','TYPE','UT','Utilizing space','Utilizing space','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ASSEMBLY','TYPE','QC','Quality check','Quality check','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','OPR','IX','Inbound process','Inbound process','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','OPR','OX','Outbound process','Outbound process','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','OPR','RT','Return process','Return process','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','OPR','AL','All purpose','All purpose','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','EAN8','European Article Number 8 digit','EAN 8 digit','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','UPCA','Universal Product Code','UPC','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','JAN','Japanese Article Number','JAN','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','ISBN','International Standard Book Number','ISBN','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','ISSN','International Standard Serial Number','ISSN','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','CODE39','Code 39','Code 39','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','EAN13','European Article Number 13 digit','EAN 13 Digit','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BARCODE','TYPE','IN','Internal warehouse','Internal warehouse','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BULK','TYPE','BL','Bulk Storage','Bulk Storage','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','BULK','TYPE','CP','Cluster pick storage','Cluster pick storage','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','CORRECTION','CODE','I01','Correction In ','I01 Correction In  ( + )','A01','1','+','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','CORRECTION','CODE','O01','Correction Out','O01 Correction Out ( - )','B01','1','-','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','0','00-SYSTEM / BLOCKING-DEBLOCKING','00-SYSTEM / BLOCKING-DEBLOCKING','-','1','0','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','10','10-INCORRECT RECEIVING (-)','10-INCORRECT RECEIVING (-)','-','1','10','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','11','11-INCORRECT RECEIVING (+)','11-INCORRECT RECEIVING (+)','+','1','11','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','14','14-TRANSFER PRODUCT BETWEEN DC (-)','14-TRANSFER PRODUCT BETWEEN DC (-)','-','1','14','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','15','15-TRANSFER PRODUCT BETWEEN DC (+)','15-TRANSFER PRODUCT BETWEEN DC (+)','+','1','15','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','22','22-PRODUCT EXPIRY DATE -','22-PRODUCT EXPIRY DATE -','-','1','22','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','26','26-QTY PRODUCT DAMAGED AT DC -','26-QTY PRODUCT DAMAGED AT DC -','-','1','26','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','27','27-GOODS REPACK (+)','27-GOODS REPACK (+)','+','1','27','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','28','28-DEFECTMENT FROM FACTORY (-)','28-DEFECTMENT FROM FACTORY (-)','-','1','28','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','30','30-STOCK TAKE ADJUSTMENT MINUS -','30-STOCK TAKE ADJUSTMENT MINUS -','-','1','30','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','31','31-STOCK TAKE ADJUSTMENT PLUS +','31-STOCK TAKE ADJUSTMENT PLUS +','+','1','31','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','32','32-ADJ AFTER STOCK TAKE (-)','32-ADJ AFTER STOCK TAKE (-)','-','1','32','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','33','33-ADJ AFTER STOCK TAKE (+)','33-ADJ AFTER STOCK TAKE (+)','+','1','33','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','34','34-ADJUST POSITIVE SOH(-)','34-ADJUST POSITIVE SOH(-)','-','1','34','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','35','35-ADJUST NEGATIVE SOH (+)','35-ADJUST NEGATIVE SOH (+)','+','1','35','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','36','36-ADJUST STOCK DISCREPANCY (-)','36-ADJUST STOCK DISCREPANCY (-)','-','1','36','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','37','37-ADJUST STOCK DISCREPANCY (+)','37-ADJUST STOCK DISCREPANCY (+)','+','1','37','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','40','40-TRANSFER  PRODUCT CODE (-)','40-TRANSFER  PRODUCT CODE (-)','-','1','40','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','41','41-TRANSFER PRODUCT CODE (+)','41-TRANSFER PRODUCT CODE (+)','+','1','41','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','50','50-ADJUSTMENT OUT (-)','50-ADJUSTMENT OUT (-)','-','1','50','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','51','51-ADJUSTMENT IN (+)','51-ADJUSTMENT IN (+)','+','1','51','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','52','52-DAMEGE FROM OTHER TRANSPORTATION (-)','52-DAMEGE FROM OTHER TRANSPORTATION (-)','-','1','52','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','53','53-DAMEGE FROM OTHER TRANSPORTATION (+)','53-DAMEGE FROM OTHER TRANSPORTATION (+)','+','1','53','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','72','72-ADJUSTMENT OUT (-)','72-ADJUSTMENT OUT (-)','-','1','72','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','73','73-ADJUSTMENT IN (+)','73-ADJUSTMENT IN (+)','+','1','73','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','78','78-PICK/BULK (RECEPTION)  RECEIVING INVOICE (-)','78-PICK/BULK (RECEPTION)  RECEIVING INVOICE (-)','-','1','78','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','79','79-PICK/BULK (RECEPTION)  RECEIVING INVOICE (+)','79-PICK/BULK (RECEPTION)  RECEIVING INVOICE (+)','+','1','79','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','82','82-DAMAGE SAGAWA TRANSPORTATION (-)','82-DAMAGE SAGAWA TRANSPORTATION (-)','-','1','82','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','83','83-DAMAGE SAGAWA TRANSPORTATION (+)','83-DAMAGE SAGAWA TRANSPORTATION (+)','+','1','83','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','84','84-DAMAGE FROM TRANSPORTATION (-)','84-DAMAGE FROM TRANSPORTATION (-)','-','1','84','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','85','85-DAMAGE FROM TRANSPORTATION (+)','85-DAMAGE FROM TRANSPORTATION (+)','+','1','85','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','94','94-CREDIT SALE (-)','94-CREDIT SALE (-)','-','1','94','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','95','95-CREDIT SALE (+)','95-CREDIT SALE (+)','+','1','95','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','I01','I01 Correction In  ( + )','I01 Correction In  ( + )','+','1','A01','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','CORRECTION','CODE','O01','Correction Out','Correction Out','-','1','B01','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','RESULT','IO','Waiting','Waiting','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','RESULT','ED','Correct','Correct','','','','','fa-check-circle text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','RESULT','WQ','Wrong quantity','Wrong quantity','','','','','fa-bug text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','RESULT','WP','Wrong product','Wrong product','','','','','fa-bug text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','RESULT','WC','Ignore count','Ignore count','','','','','fa-bug text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','TYPE','SK','Stock Take','Stock Take','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','TYPE','CL','Cycle count','Cyclr count','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','COUNT','VALIDATE','100','100 %','100 %','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DAMAGE','TYPE','AD','Analysis and Decision','Analysis and Decision','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DAMAGE','TYPE','RP','Repair goods','Repair goods','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DAMAGE','TYPE','WO','Write-off','Write-off','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DATAGRID','ROWLIMIT','10','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DATAGRID','ROWLIMIT','30','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DATAGRID','ROWLIMIT','50','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DATAGRID','ROWLIMIT','100','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DATAGRID','ROWLIMIT','200','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DATAGRID','ROWLIMIT','500','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DATAGRID','ROWLIMIT','1000','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','DEPOT','DEPOT','1','Depot','Depot','','','01','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ALL','PRIORITY','Priority of operate',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ALL','FLOW','State of transaction',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ARTICLE','ROOMTYPE','Room type of product',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ARTICLE','TEMPERATURE','Tempterature unit',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ARTICLE','TYPE','Type of article ',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ARTICLE','MANAGE','Article manage type',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ASSEMBLY','TYPE','Type of Assembly location',NULL,'LOCATION','ASSEMBLY','AS','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','BARCODE','OPR','Barcode operate on system',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','BARCODE','TYPE','Barcode type that support on operate',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','BULK','TYPE','Type of bulk',NULL,'LOCATION','AREA','BL','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','CORRECTION','CODE','Code for Correction',NULL,'Mapping Code to drop interface','Send to drop interface','Method','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','COUNT','RESULT','Result of validate count','Result of validate count','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','COUNT','TYPE','Type of counting process ','Type of counting process','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','COUNT','VALIDATE','Percentage validate','Percentage validate','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','DAMAGE','TYPE','Type of Damage location',NULL,'LOCATION','AREA','DM','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','DATAGRID','ROWLIMIT','Limit rows per page','Limit row per page','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','DEVICE','TYPE','Type of Device',NULL,'DEVICE','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','DSPSTAGING','TYPE','Type of Dispatch staging',NULL,'LOCATION','AREA','DS','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','FORWARD','TYPE','Type of Forwarding location',NULL,'LOCATION','ASSEMBLY','FW','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','HANDERLING','TYPE','Type of handerling unit',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','HANDERLING','FLOW','State of handerling unit','State of handerling unit','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','INBORDER','FLOW','Inbound order status',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','INBSTAGING','TYPE','Type of Inbound staging',NULL,'LOCATION','AREA','RS','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','IXSTATE','FLOW','State of transaction',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','IXSTATE','OPS','Operate status transaction',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','LOCATION','AREA','Category of location',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','LOCATION','TYPE','Type of location',NULL,'ORDERTYPE','1','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','LOCATION','FORMAT','Format of location',NULL,'Level','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','LOCATION','SPCAREA','Category of location',NULL,'','0','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ORDER','TYPE','Type of Inbound order operate',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ORDER','SUBTYPE','Sub type of Inbound order process',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','OUORDER','FLOW','Outbound order status',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','OVERFLOW','TYPE','Type of overflow area',NULL,'LOCATION','AREA','OV','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','PICKNDROP','TYPE','Type of Pick n drop location',NULL,'LOCATION','AREA','PD','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','PREP','TYPE','Preparation type',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','PRODUCT','DIV','Division','Division','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','PRODUCT','DEP','Department','Department','DIV','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','PRODUCT','SDP','Sub department','Sub department','DIV','DEP','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','PRODUCT','CLS','Class','Class','DIV','DEP','SDP','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','PRODUCT','SCL','Sub class','Sub class','DIV','DEP','SDP','CLS','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','RECIPE','OPS','Type of Operate',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','RECIPE','TYPE','Type of Recipe',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','RETURN','TYPE','Type of return',NULL,'LOCATION','AREA','RN','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','ROUTE','TYPE','Type of Route delivery',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','SINBIN','TYPE','Type of sin bin location',NULL,'LOCATION','AREA','SB','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','TASK','TYPE','Type of Task movement',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','TASK','FLOW','State of transaction',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','THPARTY','TYPE','Business unit',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','THPARTY','BUTYPE','Business unit',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','TRANSPORT','PAYMENT','Payment type of transportation',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','TRANSPORT','LOADTYPE','Loading type of transportation',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','TRANSPORT','TRUCKTYPE','Truck type','Truck type','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','TRANSPORT','TRTMODE','Transportation mode','Transportation mode','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','UNIT','STOCKDESC','Stock description unit',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','UNIT','WEIGHT','Weight unit measurement',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','UNIT','LENGTH','Length unit measurement',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','UNIT','KEEP','Keeping stock unit measurement',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','UNIT','VOLUME','Volume unit measurement',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DESC','WAREHOUSE','TYPE','Type of warehouse',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DEVICE','TYPE','MB','Mobile Device','Mobile Device','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DEVICE','TYPE','RT','Reach Truck','Reach Truck','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DEVICE','TYPE','FL','Folk Lift','Folk Lift','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DEVICE','TYPE','PM','Pallet Mover','Pallet Mover','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DSPSTAGING','TYPE','DK','Dock Dispatch','Dock Dispatch','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DSPSTAGING','TYPE','SG','Staging Dispatch','Staging Dispatch','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DSPSTAGING','TYPE','CB','Combine Dispatch','Combine Dispatch','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','DSPSTAGING','TYPE','FC','Full Container','Full Container','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','FORWARD','TYPE','FI','Inbound','Inbound','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','FORWARD','TYPE','FO','Outbound','Outbound','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLIND','FLOW','LD','Loaded','Loaded','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','FLOW','DL','Delivery','Deliveried','','','','','fas fa-truck-moving text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','FLOW','IO','Active','Active','','','','','fas fa-pallet text-warning','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','FLOW','ED','Completed','Completed','','','','','fas fa-pallet text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','FLOW','LD','Loading','Loading','','','','','fas fa-truck-loading text-primary','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','FLOW','PE','Closed','Closed','','','','','fas fa-pallet text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','TYPE','ST','Handerling unit for stocking','Storage','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','TYPE','XD','Handerling unit for crossdock ','XD Storage','XD','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','TYPE','XE','Handerling unit for crossdock empty','XD Empty','XD','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','HANDERLING','TYPE','SD','Handerling unit for preparation','Preparation','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBORDER','FLOW','CL','Cancelled','Close Order receipt on <span class=''text-danger''>%s</span> ago','','','','','fas fa-times text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBORDER','FLOW','ED','Finished','Closed order receipt on <span class=''text-danger''>%s</span> ago','','','','','fas fa-check-circle text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBORDER','FLOW','IO','Waiting','Waiting','','','','','fas fa-clock text-dark','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBORDER','FLOW','SS','Unloading','Unload start in  <span class=''text-primary''>%s</span> on <span class=''text-danger''>%s</span> ago','','','','','fas fa-truck-loading text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBORDER','FLOW','SA','Assign staging','Assign to stating <span class=''text-primary''>%s</span> on <span class=''text-danger''>%s</span> ago','','','','','fas fa-door-closed text-primary','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBORDER','FLOW','SE','Unload completed','Unload finish in  <span class=''text-primary''>%s</span> on <span class=''text-danger''>%s</span> ago','','','','','fas fa-truck-loading text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBSTAGING','TYPE','DK','Dock Receive','Dock Receive','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','INBSTAGING','TYPE','SG','Staging Receive','Staging Receive','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','IXSTATE','FLOW','IO','Enable for active transaction','Enable for active transaction','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','IXSTATE','FLOW','ER','Transaction Error','Transaction Error','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','IXSTATE','OPS','R','Remove transction','Remove transction','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','IXSTATE','OPS','U','Update transaction','Update transaction','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','IXSTATE','OPS','I','Insert transaction','Insert transaction','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATIOIN','DIRECTION','L','Left side','Left side','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATIOIN','DIRECTION','R','Right side','Right side','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','PD','Pick and Drop area','Pick and Drop area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','ST','Stocking area','Stocking area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','BL','Bulk area','Bulk area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','BN','Sin Bin area','Sin Bin area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','RS','Inbound Staging','Inbound Staging','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','DS','Dispatch Staging','Dispatch Staging','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','AS','Assembly area','Assembly area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','DM','Damage area','Damage area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','RN','Return area','Return area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','OV','Overflow area','Overflow area','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','XD','Crossdock category','Crossdock category','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','AREA','FW','Fowarding cateogry','Fowarding cateogry','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','FORMAT','SPCAREA','Category of location','Category of location','1','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','FORMAT','AREA','Area of location','Area of location','2','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','FORMAT','TYPE','Type of location','Type of location','3','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','SPCAREA','ST','Stocking','Stocking','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','SPCAREA','XD','Distribution','Distribution','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','SPCAREA','FW','Forwarding','Forwarding','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','LC','Location Address','Location Address','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','ZN','Zone address','Zone address','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','AL','Aisle address','Aisle address','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','BA','Bay address','Bay address','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','LV','Level address','Level address','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','SA','Stack address','Stack address','ST','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','DG','Distribution grid address','Distribution grid address','XD','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','LOCATION','TYPE','FS','Forwarding slot address','Forwarding slot address','FW','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','RC','Inbound receive','Inbound receive','ST','RC','1','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','RM','Inbound return damage','Inbound return damage','ST','RC','2','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','RW','Inbound return warehouse','Inbound return damage','ST','RC','3','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','RV','Inbound return vendor','Inbound return vendor','ST','RC','4','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','DL','Outbound delivery customer','Outbound delivery customer','ST','DL','1','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','DV','Outbound delivery vendor','Outbound delivery vendor','ST','DL','2','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','DH','Outbound delivery hub','Outbound delivery hub','ST','DL','3','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','DM','Outbound delivery damage','Outbound delivery damage','ST','DL','4','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','FW','Fowarding','Fowarding','FW','FW','1','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','XD','Crossdock','Crossdock','XD','XD','1','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','SUBTYPE','XB','Crossdock Before receipt','Crossdock Before receipt','FW','FW','2','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','RC','Receive','Receive','ST','RC','','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','RM','Return damage','Damage','ST','RC','','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','RW','Return warehouse','Warehouse','ST','RC','','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','RV','Return vendor','Vendor','ST','RC','','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','DC','Delivery customer','Customer','ST','DL','','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','DV','Delivery vendor','Vendor','ST','DL','','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','DH','Delivery Hub','Hub','ST','DL','','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','DM','Delivery damage','Damage','ST','DL','','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','FW','Forwarding','Forwarding','FW','FW','','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','XD','Distribute','Distribute','XD','XD','','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','XB','Distribute before','Distribute before','XD','XD','','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','SUBTYPE','XA','Distribute Package','Distribute Package','XD','XD','','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','TYPE','RC','Receive','Receive','ST','','','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','TYPE','DL','Delivery','Delivery','ST','','','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','TYPE','FW','Forwarding','Forwarding','FW','','','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ORDER','TYPE','XD','Distribute','Distribute','XD','','','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','TYPE','RC','Receive','Receive','ST','','1','IN','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','TYPE','XD','Delivery','Delivery','ST','','2','OU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','ORDER','TYPE','FW','Forwarding','Forwarding','FW','','4','INOU','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','IO','Active','Waiting','','','','','fas fa-pause-circle','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','NE','Not enough','Not enough','','','','','fas fa-pause-circle text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','PC','Preparing','Preparing since <span class=''text-danger''>%s</span> ago','','','','','fas fa-shopping-basket text-warning','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','ED','Post process','Post process <span class=''text-danger''>%s</span> ago','','','','','fas fa-truck text-success ','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','RP','Ready to process','Ready to processs','','','','','fas fa-check-circle text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','PA','Pick started','Pick stated since <span class=''text-danger''>%s</span> ago','','','','','fas fa-shopping-basket text-warning','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','PE','Pick finised','Pick finished since <span class=''text-danger''>%s</span> ago','','','','','fas fa-pallet text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','CF','Confirmed','Confirmed since <span class=''text-danger''>%s</span> ago','','','','','fas fa-truck-loading text-primary','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OUBORDER','FLOW','CS','Confirmed with short','Confirmed with shortage since <span class=''text-danger''>%s</span> ago','','','','','fas fa-truck-loading text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OVERFLOW','TYPE','OV','Overflow','Overflow','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','OVERFLOW','TYPE','WD','Waiting delivery','Waiting delivery','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PICKNDROP','TYPE','RS','Put away Task','Put away Task','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PICKNDROP','TYPE','DS','Replen or Approch Task','Replen or Approch Task','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PREP','TYPE','P','Piece',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PREP','TYPE','A','Pallet',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PREP','TYPE','D','Distribute',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PREP','TYPE','F','Distribute',NULL,'','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PRODUCT','DIV','00001','HARD LINE','HARD LINE','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PRODUCT','DIV','00002','HOME LINE','HOME LINE','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PRODUCT','DIV','00004','DRY FOOD','DRY FOOD','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PRODUCT','DIV','00003','SOFT LINE','SOFT LINE','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','PRODUCT','DIV','00005','FRESH FOOD','FRESH FOOD','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','RECIPE','OPS','MAT','Material for recipe','Material for recipe','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','RECIPE','OPS','RES','Result of recipe','Result of recipe','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','RECIPE','TYPE','MT','Mutation process','Mutation process','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','RECIPE','TYPE','Repacking','Repacking process','Repacking process','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','RETURN','TYPE','VD','Vendor','Vendor','','','','','fas fa-industry text-muted','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','RETURN','TYPE','WH','Warehouse','Warehouse','','','','','fas fa-warehouse text-muted','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','RETURN','TYPE','LC','Location','Location','','','','','fas fa-pallet text-muted','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ROUTE','TYPE','P','Route for preparation','Preparation','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ROUTE','TYPE','D','Route for Delivery','Delivery','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','ROUTE','TYPE','R','Route for HUB or Dropship','HUB or Dropship','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','SINBIN','TYPE','VP','Re-allocate','Re-allocate','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','SINBIN','TYPE','WO','Write-off','Write-off','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','SINBIN','TYPE','IV','Investigating','Investigating','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'BGCTH.GC','SITE','SITE','91917','Site','Site','','','980','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','FLOW','IO','Waiting','Waiting on <span class=''text-danger''>%s</span> ago','','','','','fas fa-clock text-muted','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','FLOW','ED','Confirmed','finish on <span class=''text-danger''>%s</span> ago','','','','','fas fa-check-circle text-success','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','FLOW','CL','Cancelled','cancelled on <span class=''text-danger''>%s</span> ago','','','','','fas fa-minus-circle text-danger','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','FLOW','PT','Take HU','take HU on <span class=''text-danger''>%s</span> ago','','','','','fas fa-arrow-circle-right text-primary','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','TYPE','R','Replenishment','Replen.','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','TYPE','A','Approch','Approch','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','TYPE','P','Putaway','Putaway','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','TYPE','T','Transfer','Transfer','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TASK','TYPE','S','Partial Replenishment','Partial','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','BUTYPE','WH','Warehouse','Warehouse','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','BUTYPE','CS','Customer','Customer','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','BUTYPE','DS','Drop ship','Drop ship','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','BUTYPE','TP','Transportor','Transportor','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','BUTYPE','SP','Supplier','Supplier','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','SP','Supplier','Supplier','SP','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','VD','Vendor','Vendor','SP','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','HP','Hyper Store','Hyper Store','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','MK','Market Store','Market Store','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','MN','Mini Store','Mini Store','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','JB','Jumbo Store','Jumbo Store','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','PR','Pure Store','Pure Store','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','CB','Crossborder','Crossborder','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','OS','Online Store','Online Store','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','OC','Online Customer','Online Customer','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','B2','B2B Customer','B2B Customer','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','DS','Drop ship','Drop ship','DS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','LC','Local Transport','Local Transport','TP','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','SB','Subby Transport','Subby Transport','TP','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','OV','Oversea Transport','Oversea Transport','TP','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','AF','Air Freight','Air Frieght','TP','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','MB','Motorbike','Motobike','TP','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','WH','Warehouse','Warehouse','WH','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','HB','Hub Transport','Hub Transfer','WH','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','HV','Virtual Hub','Virtual Hub','WH','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','CG','Cargo','Cargo','WH','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','THPARTY','TYPE','BR','Branch','Branch','CS','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','LOADTYPE','LH','Loading with Handling unit','Loading with Handling unit','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','LOADTYPE','LL','LoseLoad','LoseLoad','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','PAYMENT','COD','Cash on delivery','Cash on delivery','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','PAYMENT','BTF','Bank transfer','Bank transfer','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','PAYMENT','CRE','Credit','Credit','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','PAYMENT','PRE','Pre payment','Pre payment','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','TRTMODE','ROAD','Road','Road','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','TRTMODE','AIR','Air Freight','Air Freight','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','TRUCKTYPE','4WD','Truck 4WD','Truck 4WD','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','TRANSPORT','TRUCKTYPE','MB','Motor Bike','Motor Bike','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','KEEP','1','SKU','SKU','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','KEEP','2','IPCK','IPCK','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','KEEP','3','PCK','PCK','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','KEEP','4','LAYER','Layer','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','KEEP','5','HU','HU','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','LENGTH','1','mm','Millimeters','0.001 Meters','1000','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','LENGTH','2','cm','Centimeters','0.10 Meters','100','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','LENGTH','3','dm','Decimeters','0.1 Meters','10','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','LENGTH','4','m','Meter','1 Meter','1','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','LENGTH','5','dam','Decameters','10 Meters','10','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','LENGTH','6','hm','Hectometers','100 Meters','100','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','LENGTH','7','km','Kilometers','1000 Meters','1000','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','01','ชิ้น','ชิ้น','1','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','02','อัน','อัน','1','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','11','แพ็ค','แพ็ค','2','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','12','เซ็ท','เซ็ท','2','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','13','ชุด','ชุด','2','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','20','PCK','PCK','3','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','21','ลัง','ลัง','3','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','22','Carton','Carton','3','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','23','หีบ','หีบ','3','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','30','Layer','Layer','4','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','31','แถว','แถว','4','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','32','ชั้น','ชั้น','4','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','40','HU','HU','5','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','41','พาเลท','พาเลท','5','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','STOCKDESC','51','แถว','','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','VOLUME','1','mm3','Cubic millimeter','0.001 m3','1000','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','VOLUME','2','cm3','Cubic centimeter','0.10 Meters','100','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','VOLUME','3','dm3','Cubic decimeter','0.1 Meters','10','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','VOLUME','4','m3','Cubic meter','1 Meter','1','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','VOLUME','5','dam3','Cubic decameters','10 Meters','10','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','VOLUME','6','hm3','Cubic hectometer','100 Meters','100','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','VOLUME','7','km3','Cubic Kilometers','1000 Meters','1000','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','WEIGHT','1','mg','Milligram','0.001 gram','1000','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','WEIGHT','2','cg','Centigram','0.10 gram','100','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','WEIGHT','3','dg','Decigram','0.1 gram','10','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','WEIGHT','4','g','Gram','1 gram','1','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','WEIGHT','5','dag','Decagram','10 gram','10','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','WEIGHT','6','hg','Hectogram','100 gram','100','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','UNIT','WEIGHT','7','kg','Kilogram','1000 gram','1000','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','WAREHOUSE','TYPE','RT','Retail','Retail','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','WAREHOUSE','TYPE','CS','Cold Storage','Cold Storage','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','WAREHOUSE','TYPE','OV','Overseas','Overseas','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode), " + 
"(@orgcode, @site, @depot,'WMS','WAREHOUSE','TYPE','FL','Fullfillment','Fullfillment','','','','','','IO',sysdatetimeoffset(),@accncode,sysdatetimeoffset(),@accncode) ";


        private string sqldepot_add_step10 = @"insert into wm_depot ( orgcode,sitecode,depottype,depotcode,depotname,depotnamealt,
        datestart,dateend,depotkey,depotops,tflow,depothash,unitweight,unitvolume,unitdimension,unitcubic,formatdate,
        formatdateshort,formatdatelong,datecreate,accncreate,datemodify,accnmodify,procmodify) values 
        (@orgcode,@sitecode,@depottype,@depotcode,@depotname,@depotnamealt,@datestart,@dateend,@depotkey,@depotops,@tflow,
        @depothash,@unitweight,@unitvolume,@unitdimension,@unitcubic,@formatdate,@formatdateshort,@formatdatelong,sysdatetimeoffset(),
        @accncreate,sysdatetimeoffset(),@accnmodify,@procmodify)";

        private SqlConnection cn = new SqlConnection();
        private string cnx = "";
        public depot_ops(){ }
        public depot_ops(string cx){ cn = new SqlConnection(cx); this.cnx = cx; }
        public depot_ops(String svID, SqlConnection ocn) { cn = ocn; }

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }
            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}