using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS { 

    public partial class orbit_ops  {

        //Insert to Orbit Thirdaparty 
        private string sqlorbit_thirdparty_insert = ""+ 
        " insert into xm_xithirdparty (						                      " +
        " orgcode,site,depot,spcarea,thtype,thbutype,thcode,thcodealt,vatcode,    " +
        " thname,thnamealt,addressln1,addressln2,addressln3,subdistrict,district, " +
        " city,country,postcode,region,telephone,email,thgroup,thcomment,		  " +
        " throuteformat,plandelivery,naturalloss,mapaddress,orbitsource,tflow,    " +
        " fileid,rowops,ermsg,dateops,rowid ) values                              " +
        " (@orgcode,@site,@depot,@spcarea,@thtype,@thbutype,@thcode,@thcodealt,   " +
        " @vatcode,@thname,@thnamealt,@addressln1,@addressln2,@addressln3,        " +
        " @subdistrict,@district,@city,@country,@postcode,@region,@telephone,     " +
        " @email,@thgroup,@thcomment,@throuteformat,@plandelivery,@naturalloss,   " +
        " @mapaddress,@orbitsource,@tflow,@fileid,@rowops,@ermsg,sysdatetimeoffset()," +
        " next value for seq_ixthirdparty )       " ;

        //Insert to orbit Product
        private string sqlorbit_product_insert = "" + 
        " insert into XM_XIPRODUCT	(                                                                                     " +
        " orgcode,site,depot,spcarea,article,articletype,pv,lv,description,descalt,thcode,dlcall,dlcfactory,              " +
        " dlcwarehouse,dlcshop,dlconsumer,hdivison,hdepartment,hsubdepart,hclass,hsubclass,typemanage,                    " +
        " unitmanage,unitdesc,unitreceipt,unitprep,unitsale,unitstock,unitweight,unitdimension,unitvolume,                " +
        " hucode,rtoskuofpu,rtoskuofipck,rtoskuofpck,rtoskuoflayer,rtoskuofhu,rtopckoflayer,rtolayerofhu,                 " +
        " innaturalloss,ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight,                      " +
        " skugrossweight,skuweight,skuvolume,pulength,puwidth,puheight,pugrossweight,puweight,puvolume,                   " +
        " ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight,ipckvolume,pcklength,pckwidth,pckheight,             " +
        " pckgrossweight,pckweight,pckvolume,layerlength,layerwidth,layerheight,layergrossweight,layerweight,             " +
        " layervolume,hulength,huwidth,huheight,hugrossweight,huweight,huvolume,isdangerous,ishighvalue,                  " +
        " isfastmove,isslowmove,isprescription,isdlc,ismaterial,isunique,isalcohol,istemperature,isdynamicpick,           " +
        " ismixingprep,isfinishgoods,isnaturalloss,isbatchno,ismeasurement,roomtype,tempmin,tempmax,alcmanage,            " +
        " alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin,stockthresholdmax,                 " +
        " spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation,spcprepzone,spcdistzone,spcdistshare,          " +
        " spczonedelv,orbitsource,tflow,fileid,rowops,datecreate,dateops,ermsg,rowid,rtoipckofpck) values                 " +
        " (@orgcode,@site,@depot,@spcarea,@article,@articletype,@pv,@lv,@description,@descalt,@thcode,@dlcall,@dlcfactory," +
        " @dlcwarehouse,@dlcshop,@dlconsumer,@hdivison,@hdepartment,@hsubdepart,@hclass,@hsubclass,@typemanage,			  " +
        " @unitmanage,@unitdesc,@unitreceipt,@unitprep,@unitsale,@unitstock,@unitweight,@unitdimension,@unitvolume,       " +
        " @hucode,@rtoskuofpu,@rtoskuofipck,@rtoskuofpck,@rtoskuoflayer,@rtoskuofhu,@rtopckoflayer,@rtolayerofhu,         " +
        " @innaturalloss,@ounaturalloss,@costinbound,@costoutbound,@costavg,@skulength,@skuwidth,@skuheight,              " +
        " @skugrossweight,@skuweight,@skuvolume,@pulength,@puwidth,@puheight,@pugrossweight,@puweight,@puvolume,          " +
        " @ipcklength,@ipckwidth,@ipckheight,@ipckgrossweight,@ipckweight,@ipckvolume,@pcklength,@pckwidth,@pckheight,    " +
        " @pckgrossweight,@pckweight,@pckvolume,@layerlength,@layerwidth,@layerheight,@layergrossweight,@layerweight,     " +
        " @layervolume,@hulength,@huwidth,@huheight,@hugrossweight,@huweight,@huvolume,@isdangerous,@ishighvalue,         " +
        " @isfastmove,@isslowmove,@isprescription,@isdlc,@ismaterial,@isunique,@isalcohol,@istemperature,@isdynamicpick,  " +
        " @ismixingprep,@isfinishgoods,@isnaturalloss,@isbatchno,@ismeasurement,@roomtype,@tempmin,@tempmax,@alcmanage,   " +
        " @alccategory,@alccontent,@alccolor,@dangercategory,@dangerlevel,@stockthresholdmin,@stockthresholdmax,          " +
        " @spcrecvzone,@spcrecvaisle,@spcrecvbay,@spcrecvlevel,@spcrecvlocation,@spcprepzone,@spcdistzone,@spcdistshare,  " +
        " @spczonedelv,@orbitsource,@tflow,@fileid,@rowops,sysdatetimeoffset(),sysdatetimeoffset(),@ermsg,                " +
        " next value for seq_ixproduct ,@rtoipckofpck)  ";

        //Insert to order Barcode 
        private string sqlorbit_barcode_insert = "" +
        " insert into xm_xibarcode (orgcode,site,depot,spcarea,article,pv,lv,barops,barcode,bartype,thcode,tflow,       " +
        " fileid,rowops,ermsg,dateops,rowid,orbitsource) values (@orgcode,@site,@depot,@spcarea,@article,@pv,@lv,@barops,@barcode,  " +
        " @bartype,@thcode,@tflow,@fileid,@rowops, @ermsg,sysdatetimeoffset(), next value for seq_ixbarcode, @orbitsource ) ";

        //Insert to orbit Inbound header 
        private string sqlorbit_inbound_header_insert = "" + 
        " insert into xm_xiinbound	                                                                                   " +
        " ( orgcode,site,depot,spcarea,thcode,intype,subtype,inorder,dateorder,dateplan,dateexpire,inpriority,inflag,  " + 
        "   inpromo,tflow,orbitsource,fileid,rowops,ermsg,dateops,rowid ) values                                       " + 
        " ( @orgcode,@site,@depot,@spcarea,@thcode,@intype,@subtype,@inorder,@dateorder,@dateplan,@dateexpire,         " + 
        "   @inpriority,@inflag,@inpromo,@tflow,@orbitsource,@fileid,@rowops,@ermsg,sysdatetimeoffset(), next value for seq_ixinbound ) " ;

        //Insert to orbit Inbound line
        private string sqlorbit_inbound_line_insert = "" + 
        " insert into xm_xiinbouln                                                                                          " + 
        " ( orgcode,site,depot,spcarea,inorder,inln,inrefno,inrefln,article,pv,lv,unitops,qtysku,qtypu,qtyweight,batchno,   " + 
        "   lotno,expdate,serialno,orbitsource,tflow,fileid,rowops,ermsg,dateops,rowid ) values                             " + 
        " ( @orgcode,@site,@depot,@spcarea,@inorder,@inln,@inrefno,@inrefln,@article,@pv,@lv,@unitops,@qtysku,@qtypu,       " + 
        "   @qtyweight,@batchno,@lotno,@expdate,@serialno,@orbitsource,@tflow,@fileid,@rowops,@ermsg,sysdatetimeoffset(), " + 
        "   next value for seq_ixinbouln ) " ;

        //Insert to orbit Outbound header 
        private string sqlorbit_outbound_header_insert = "" + 
        " insert into xm_xioutbound " +
        " (orgcode,site,depot,spcarea,ouorder,outype,ousubtype,thcode,dateorder,dateprep,dateexpire,oupriority,  " + 
        "  ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,stoaddressln2,stoaddressln3,        " + 
        "  stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,tflow,inorder,fileid,     " + 
        "  rowops,ermsg,dateops,rowid ) values (                                                                 " + 
        "  @orgcode,@site,@depot,@spcarea,@ouorder,@outype,@ousubtype,@thcode,@dateorder,@dateprep,@dateexpire,  " + 
        "  @oupriority,@ouflag,@oupromo,@dropship,@orbitsource,@stocode,@stoname,@stoaddressln1,@stoaddressln2,  " + 
        "  @stoaddressln3,@stosubdistict,@stodistrict,@stocity,@stocountry,@stopostcode,@stomobile,@stoemail,    " + 
        "  @tflow,@inorder,@fileid,@rowops,@ermsg,sysdatetimeoffset(), next value for seq_ixoutbound )           " ;

        //Insert to orbit Outbound line
        //private string sqlorbit_outbound_line_insert = "" + 
        //" insert into xm_xioutbouln                                                                    " +
        //" ( orgcode,site,depot,spcarea,ouorder,ouln,ourefno,ourefln,inorder,article,pv,lv,unitops,     " +
        //"   qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,serialno,orbitsource,       " +
        //"   tflow,disthcode,fileid,rowops,ermsg,dateops,ouseq,rowid ) values                                 " +
        //" ( @orgcode,@site,@depot,@spcarea,@ouorder,@ouln,@ourefno,@ourefln,@inorder,@article,@pv,@lv, " +
        //"   @unitops,@qtysku,@qtypu,@qtyweight,@spcselect,@batchno,@lotno,@datemfg,@dateexp,@serialno, " +
        //"   @orbitsource,@tflow,@disthcode,@fileid,@rowops,@ermsg,sysdatetimeoffset(), @ouseq," +
        //"   next value for seq_ixoutbouln ) " ;
        private string sqlorbit_outbound_line_insert = "" +
        " insert into xm_xioutbouln                                                                    " +
        " ( orgcode,site,depot,spcarea,ouorder,ouln,ourefno,ourefln,inorder,article,pv,lv,unitops,     " +
        "   qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,serialno,orbitsource,       " +
        "   tflow,disthcode,fileid,rowops,ermsg,dateops,ouseq,rowid ) values                           " +
        " ( @orgcode,@site,@depot,@spcarea,@ouorder,@ouln,@ourefno,@ourefln,@inorder,@article,@pv,@lv, " +
        "   dbo.get_unitops(@orgcode,@site,@depot,@article,@lv),@qtysku,@qtypu,dbo.cal_weight(@orgcode,@site,@depot,@article,@lv,@qtysku),@spcselect,@batchno," +
        "   @lotno,@datemfg,@dateexp,@serialno,@orbitsource,@tflow,@disthcode,@fileid,@rowops,@ermsg,sysdatetimeoffset(), @ouseq," +
        "   next value for seq_ixoutbouln ) ";

        //Send Inbound receipt info to orbit
        string sqlorbit_receipt_insert = "INSERT INTO xm_xoreceipt " + 
        " (orgcode,site,depot,thcode,inorder,inln,ingrno,intype,inrefno,inrefln,barcode,article,pv,lv   " +
        " ,unitops,qtysku,qtypu,qtyweight,qtyvolume,qtynaturalloss, dateops,accnops,dateexp,datemfg     " + 
        " ,batchno,lotmfg,serialno,huno,inpromo,insource,xaction,xcreate) VALUES " + 
        " (@orgcode,@site,@depot,@thcode,@inorder,@inln,@ingrno,@intype,@inrefno,@inrefln,@barcode      " +
        " ,@article,@pv,@lv,@unitops,@qtysku,@qtypu,@qtyweigh,@qtyvolume,@qtynaturalloss, @dateops      " + 
        " ,@accnops,@dateexp,@datemfg,@batchno,@lotmfg,@serialno,@huno,@inpromo,@insource,'WC',CURRENT_TIMESTAMP) ";

        //Send Correction stock info to order 
        string sqlorbit_correction_insert = "INSERT INTO xm_xocorrection                    " + 
        " (orgcode,site,depot,dateops,accnops,seqops,codeops,typeops,thcode,article         " +
        " ,pv,lv,unitops,qtysku,qtyweight,inreftype,inrefno,ingrno,inpromo,reason           " + 
        " ,xaction,xcreate) values                                                          " + 
        " (@orgcode,@site,@depot,@dateops,@accnops,@seqops,@codeops,@typeops,@thcode        " +
        " ,@article,@pv,@lv,@unitops,@qtysku,@qtyweight,@inreftype,@inrefno,@ingrno         " + 
        " ,@inpromo,@reason,'WA',sysdatetimeoffset()) ";

        //Send Outbound delivery to orbit
        string sqlorbit_delivery_insert = "" + 
        " insert into xm_xodelivery                                                                             " +
        " ( orgcode,site,depot,dateops,routeops,transportno,thcode,thtype,dropship,ouorder,ouline,              " +
        "   ourefno,ourefln,oudnno,inorder,ingrno,article,pv,lv,unitops,qtysku,qtypu,qtyweight,                 " +
        "   qtyvolume,dateexp,datemfg,lotmfg,serialno,huno,accnops,oupromo,xaction,xcreate,xmodify )            " +
        " values                                                                                                " +
        " ( @orgcode,@site,@depot,@dateops,@routeops,@transportno,@thcode,@thtype,@dropship,@ouorder,@ouline,   " +
        "   @ourefno,@ourefln,@oudnno,@inorder,@ingrno,@article,@pv,@lv,@unitops,@qtysku,@qtypu,@qtyweight,     " +
        "   @qtyvolume,@dateexp,@datemfg,@lotmfg,@serialno,@huno,@accnops,@oupromo,@xaction,@xcreate,@xmodify ) " ;

        //Send Stock block to orbit
        string sqlorbit_stockblock_insert = "";
    }
}