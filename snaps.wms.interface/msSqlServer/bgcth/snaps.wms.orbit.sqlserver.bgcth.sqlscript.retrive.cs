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

    public partial class orbit_ops : IDisposable {
        //Retrive site to operate 
        string sqlInterface_retrive_site = "" + 
        "select sitecode,depotcode from wm_depot where tflow = 'IO' and orbitsource = 'BGCTH.GC' and orbitoperate = '1' and sitecode = '91917' ";
        //Retrive Inbound order to orbit
        //rowops 1:insert , 2:update , 3 cancel else error
        string sqlInterface_retrive_inbound_header = "" +
        @"select distinct 'bgc' orgcode,iersite site,lpad(ierdepo,2,'0') depot,  
                case when (iertype in ('1','2') and iertyco in ('0','1')) then 'ST'
                       when (iertype in ('1','2') and iertyco = '3') then 'FW'
                       when (iertype in ('1','2') and iertyco = '4') then 'XD'
                else 'ER' end spcarea,
                case when iertype = '2' then IERNCLI else IERCNUF end thcode,
                case when (iertype in ('1','2') and iertyco in ('0','1')) then 'RC'
                       when (iertype in ('1','2') and iertyco = '3') then 'FW'
                       when (iertype in ('1','2') and iertyco = '4') then 'XD'
                else 'ER' end intype,      
                case when (iertype ='1' and iertyco in ('0','1')) then 'RC'
                     when (iertype ='2' and iertyco in ('0','1')) then 'RW'
                     when (iertype in ('1','2') and iertyco = '3') then 'FW'
                     when (iertype in ('1','2') and iertyco = '4') then 'XD'
                else 'ER' end subtype,    
               IERCEXCDE inorder,null dateorder,ierdliv dateplan,ierdlim dateexpire,  
               null inpriority,null inflag,null inpromo,'WC' tflow,'BGCTH.GC' orbitsource,null fileid,  
               IERACTI rowops,null ermsg,null dateops
        from ICSENTOR where iersite = :site and ierdepo = :depot and ierdmaj is null";

        //" select 'bgc' orgcode,iersite site,ierdepo depot,                                                                                 " + 
        //"         case when iertype in ('1','2') then 'ST' when iertype = '3' then 'FW' when iertype = '4' then 'XD' else 'ER' end spcarea," + 
        //"         case when iertype = '2' then IERNCLI else IERCNUF end thcode,                                                            " +
        //"         case when iertype in ('1','2') then 'RC' when iertype = '3' then 'FW' when iertype = '4' then 'XD' else 'ER' end intype, " +
        //"         case when iertype = '1' then 'RC'                                                                                        " +
        //"                when iertype = '2' then 'RW'                                                                                      " +
        //"                when iertype = '3' then 'FW'                                                                                      " +
        //"                when iertype = '4' then 'XD'else 'ER' end subtype,                                                                " +
        //"        IERCEXCDE inorder,null dateorder,ierdliv dateplan,ierdlim dateexpire,                                                     " +
        //" null inpriority,null inflag,null inpromo,'WC' tflow,'BGCTH.GC' orbitsource,null fileid,                                          " +
        //" IERACTI rowops,null ermsg,null dateops from ICSENTOR where iersite = :site and ierdepo = :depot                                  " ;

        // + idrnolign 
        string sqlInterface_retrive_inbound_line = "" +
        " select 'bgc' orgcode,idrsite site,lpad(idrdepo,2,'0') depot,null spcarea,idrcexcde inorder, " +
        " idrnolign inln,idrcincde inrefno,idrnlig inrefln,idrcode article,null pv,         " +
        " idrcexvl lv,null unitops,idrqtec qtysku,null qtypu,null qtyweight,null batchno, " +
        " null lotno,null expdate,null serialno,'BGCTH.GC' orbitsource,'WC' tflow,        " +
        " null fileid,IDRACTI rowops,null ermsg,null dateops from ICSDETOR                " +
        " where idrsite = :site and idrdepo = :depot and idrdmaj is null  ";

        //Retrive Outbound order to orbit
        //string sqlInterface_retrive_outbound_header = ""+
        //" select distinct 'bgc' orgcode,iolsite site,ioldepo depot,(case when ioltype = '1' then 'RTV' when ioltype = '2' then 'ST' when ioltype = '4' then 'XD' else 'ER' end) spcarea,iolcexcde ouorder,      " +
        //" ioltype outype,ioltype ousubtype,iolncli thcode,ioldcom dateorder,ioldplim dateprep,          " +
        //" ioldliv dateexpire,ioliurg oupriority,null ouflag,null oupromo,null dropship,                 " +
        //" 'BGCTH.GC' orbitsource,null stocode,iolrais stoname,ioladr1 stoaddressln1,                    " +
        //" ioladr2 stoaddressln2,null stoaddressln3,null stosubdistict,null stodistrict,                 " +
        //" iolvill stocity,iolcpay stocountry,iolcpos stopostcode,null stomobile,null stoemail,          " +
        //" 'WC' tflow,iolcexglo inorder,null fileid,iolacti rowops,null ermsg,null dateops 	            " +
        //" from ICSDEOL where iolsite = :site and ioldepo = :depot ";

        // ioltype = 1 then 'RTV'
        string sqlInterface_retrive_outbound_header = "" +
         " select distinct 'bgc' orgcode,iolsite site,lpad(ioldepo,2,'0') depot,(case when ioltype = 1 then 'ST' when ioltype = 2 then 'ST' when ioltype = 4 then 'XD' else 'ER' end) spcarea,iolcexcde ouorder,      " +
         " (case when ioltype = 1 then 'DL' when ioltype = 2 then 'DL' when ioltype = 4 then 'XD' else to_char(ioltype) end) outype," +
         " (case when ioltype = 1 then 'DV' when ioltype = 2 then 'DC' when ioltype = 4 then 'XD' else to_char(ioltype) end) ousubtype," +
         " (case when ioltype = 1 then iolcnuf else iolncli end) thcode,ioldcom dateorder,ioldplim dateprep,          " +
         " ioldliv dateexpire,ioliurg oupriority,null ouflag,null oupromo,null dropship,                 " +
         " 'BGCTH.GC' orbitsource,null stocode,iolrais stoname,ioladr1 stoaddressln1,                    " +
         " ioladr2 stoaddressln2,null stoaddressln3,null stosubdistict,null stodistrict,                 " +
         " iolvill stocity,iolcpay stocountry,iolcpos stopostcode,null stomobile,null stoemail,          " +
         " 'WC' tflow,iolcexglo inorder,null fileid,iolacti rowops,null ermsg,null dateops 	            " +
         " from ICSDEOL where iolsite = :site and ioldepo = :depot and ioldmaj is null";
        //string sqlInterface_retrive_outbound_line = ""+
        //" select 'bgc' orgcode,iolsite site,ioldepo depot,null spcarea,iolcexcde ouorder,  " +
        //" iolcincde ouln,iolnolign ourefno,null ourefln,iolcexglo inorder,iolcodc article, " +
        //" null pv,iolcexvlc lv,iolcuex unitops,iolqtec qtysku,iolqtei qtypu,null qtyweight," +
        //" null spcselect,null batchno,null lotno,null datemfg,null dateexp,null serialno,  " +
        //" 'BGCTH.GC' orbitsource,'WC' tflow,null disthcode,null fileid,null rowops,        " +
        //" null ermsg,null dateops from icsdeol where iolsite = :site and ioldepo = :depot   " ;

        string sqlInterface_retrive_outbound_line = "" +
            " select 'bgc' orgcode,iolsite site,lpad(ioldepo,2,'0') depot,(case when ioltype = '1' then 'ST' when ioltype = '2' then 'ST' when ioltype = '4' then 'XD' else 'ER' end) spcarea,iolcexcde ouorder,  " +
            " iolcincde ouln,iolnctl ourefno,iolnolign ourefln,iolcexglo inorder,iolcodc article,null pv,iolcexvlc lv,iolcuex unitops, " +
            " IOLQTEC qtysku,IOLQTEC qtypu,null qtyweight,null spcselect,null batchno,null lotno,null datemfg,null dateexp,null serialno,  " +
            " 'BGCTH.GC' orbitsource,'WC' tflow,null disthcode,null fileid,iolacti rowops, " +
            " null ermsg,null dateops,iolnlig as ouseq from icsdeol where iolsite = :site and ioldepo = :depot and ioldmaj is null";


        //Retrive Product to orbit 
        //string sqlInterface_retrive_product = ""+
        //" select 'bgc' orgcode,iarsite site,iardepo depot,null spcarea,iarcexr article,null articletype,   " +
        //" null pv,iarcexvl lv,iarliba description,iarliba descalt,iarcnuf thcode,iardlc dlcall,            " +
        //" iarpof dlcfactory,iarpoe dlcwarehouse,iarpov dlcshop,iarpoc dlconsumer,iarniv1 hdivison,         " +
        //" iarniv2 hdepartment,iarniv3 hsubdepart,iarniv4 hclass,iarniv5 hsubclass,iargpds typemanage,      " +
        //" iargpds unitmanage,null unitdesc,1 unitreceipt,iarupre unitprep,iaruven unitsale,          " +
        //" null unitstock,iarupds unitweight,iaruvol unitdimension,null unitvolume,null hucode,             " +
        //" 1 rtoskuofpu,1 rtoskuofipck,1 rtoskuofpck,1 rtoskuoflayer,1 rtoskuofhu,           " +
        //" iarcarc rtopckoflayer,iarcpal rtolayerofhu,null innaturalloss,null ounaturalloss,                " +
        //" null costinbound,null costoutbound,null costavg,iaruvlo skulength,iaruvla skuwidth,              " +
        //" iaruvha skuheight,iarpbru skugrossweight,iarpnet skuweight,null skuvolume,null pulength,         " +
        //" null puwidth,null puheight,null pugrossweight,null puweight,null puvolume,iarsplo ipcklength,    " +
        //" iarspla ipckwidth,iarspha ipckheight,iarpbspc ipckgrossweight,iarupspc ipckweight,               " +
        //" null ipckvolume,iarlong pcklength,iarlarg pckwidth,iarhaut pckheight,iarpbpcb pckgrossweight,    " +
        //" iaruppcb pckweight,null pckvolume,iarcolo layerlength,iarcola layerwidth,iarcoha layerheight,    " +
        //" iarpbco layergrossweight,iarupcou layerweight,null layervolume,iarlonpl hulength,                " +
        //" iarlarpl huwidth,iarhaupl huheight,iarpbpl hugrossweight,iaruppal huweight,null huvolume,        " +
        //" null isdangerous,null ishighvalue,null isfastmove,null isslowmove,null isprescription,           " +
        //" null isdlc,null ismaterial,null isunique,null isalcohol,null istemperature,null isdynamicpick,   " +
        //" null ismixingprep,null isfinishgoods,null isnaturalloss,null isbatchno,1 ismeasurement,          " +
        //" null roomtype,iartmax tempmin,iartmin tempmax,null alcmanage,null alccategory,null alccontent,   " +
        //" null alccolor,null dangercategory,null dangerlevel,null stockthresholdmin,null stockthresholdmax," +
        //" null spcrecvzone,null spcrecvaisle,null spcrecvbay,null spcrecvlevel,null spcrecvlocation,	   " +
        //" null spcprepzone,null spcdistzone,null spcdistshare,null spczonedelv,'BGCTH.GC' orbitsource,     " +
        //" 'WC' tflow,null fileid,iaracti rowops,null datecreate,null dateops,null ermsg                    " +
        //" from ICSARTICLE where iarsite = :site and  iardepo = :depot                                      " ;
        string sqlInterface_retrive_product =
         @"select distinct 'bgc' orgcode,iarsite site,lpad(iardepo,2,'0') depot,null spcarea,iarcexr article,null articletype,   
            null pv,iarcexvl lv,iarliba description,iarliba descalt,iarcnuf thcode,iardlc dlcall,            
            iarpof dlcfactory,iarpoe dlcwarehouse,iarpov dlcshop,iarpoc dlconsumer,iarniv1 hdivison,         
            iarniv2 hdepartment,iarniv3 hsubdepart,iarniv4 hclass,iarniv5 hsubclass,iartcon typemanage,      
            iarupre unitmanage,
            (case when iarupre = '1' then '01' when iarupre = '2' then '11' when iarupre = '3' then '21' when iarupre = '4' then '32' when iarupre = '5' then '41'  else null end) unitdesc,
            iarupre unitreceipt,iarupre unitprep,iarupre unitsale,          
            null unitstock,iarupds unitweight,iarulon unitdimension,iaruvol unitvolume,null hucode,             
            iarspcb rtoskuofpu,iarspcb rtoskuofipck,(iarspcb*iarpcb) rtoskuofpck,(iarspcb*iarpcb*iarcarc) rtoskuoflayer,(iarspcb*iarpcb*iarcarc*iarcpal) rtoskuofhu,           
            iarcarc rtopckoflayer,iarcpal rtolayerofhu,null innaturalloss,null ounaturalloss,                
            null costinbound,null costoutbound,iarpttc costavg,iaruvlo skulength,iaruvla skuwidth,              
            iaruvha skuheight,iarpbru skugrossweight,iarpbru skuweight,trunc((iaruvlo*iaruvla*iaruvha)/1000,5) skuvolume,
            (case when iarupre = '1' then iaruvlo when iarupre = '2' then iarsplo when iarupre = '3' then iarlong when iarupre = '4' then iarcolo when iarupre = '5' then iarlonpl  else 1 end) pulength,         
            (case when iarupre = '1' then iaruvla when iarupre = '2' then iarspla when iarupre = '3' then iarlarg when iarupre = '4' then iarcola when iarupre = '5' then iarlarpl  else 1 end) puwidth,
            (case when iarupre = '1' then iaruvha when iarupre = '2' then iarspha when iarupre = '3' then iarhaut when iarupre = '4' then iarcoha when iarupre = '5' then iarhaupl  else 1 end) puheight,
            (case when iarupre = '1' then iarpbru when iarupre = '2' then iarpbspc when iarupre = '3' then iarpbpcb when iarupre = '4' then iarpbco when iarupre = '5' then iarpbpl  else 1 end) pugrossweight,
            (case when iarupre = '1' then iarpbru when iarupre = '2' then iarpbspc when iarupre = '3' then iarpbpcb when iarupre = '4' then iarpbco when iarupre = '5' then iarpbpl  else 1 end) puweight,
            (case when iarupre = '1' then  (iaruvlo*iaruvla*iaruvha) when iarupre = '2' then iarsplo when iarupre = '3' then iarlong when iarupre = '4' then iarcolo when iarupre = '5' then iarlonpl  else 1 end) puvolume,
            iarsplo ipcklength,iarspla ipckwidth,iarspha ipckheight,iarpbspc ipckgrossweight,iarpbspc ipckweight,               
            trunc((iarsplo*iarspla*iarspha)/1000,5) ipckvolume,iarlong pcklength,iarlarg pckwidth,iarhaut pckheight,iarpbpcb pckgrossweight,    
            iarpbpcb pckweight,trunc((iarlong*iarlarg*iarhaut)/1000,5) pckvolume,iarcolo layerlength,iarcola layerwidth,iarcoha layerheight,    
            iarpbco layergrossweight,iarpbco layerweight, trunc((iarcolo*iarcola*iarcoha)/1000,5) layervolume,iarlonpl hulength,                
            iarlarpl huwidth,iarhaupl huheight,iarpbpl hugrossweight,iarpbpl huweight,trunc((iarlonpl*iarlarpl*iarhaupl)/1000,5) huvolume,        
            null isdangerous,null ishighvalue,null isfastmove,null isslowmove,null isprescription,           
            null isdlc,null ismaterial,null isunique,null isalcohol,null istemperature,null isdynamicpick,   
            null ismixingprep,null isfinishgoods,null isnaturalloss,null isbatchno,1 ismeasurement,          
            null roomtype,iartmax tempmin,iartmin tempmax,null alcmanage,null alccategory,null alccontent,   
            null alccolor,null dangercategory,null dangerlevel,null stockthresholdmin,null stockthresholdmax,
            null spcrecvzone,null spcrecvaisle,null spcrecvbay,null spcrecvlevel,null spcrecvlocation,	   
            null spcprepzone,null spcdistzone,null spcdistshare,null spczonedelv,'BGCTH.GC' orbitsource,     
            'WC' tflow,null fileid,iaracti rowops,null datecreate,null dateops,null ermsg,iarpcb as rtoipckofpck 
            from ICSARTICLE where iarsite = :site and  iardepo = :depot";
        //Retrive Barcode to orbit 
        //string sqlInterface_retrive_barcode = ""+ 
        //" select  distinct 'bgc' orgcode, iarsite site, iardepo depot,'' spcarea, iarcexr article,0 pv, iarcexvl lv,  " +
        //"         'AL' barops,IAREAN barcode, 'EAN13' bartype,IARCNUF thcode,'BGCTH.GC' orbitsource,'WC' tflow,       " +
        //"         null fileid, IARTEAN rowops, null datecreate,null dateops,null ermsg " +
        //" from icsarticle where IAREAN is not null and iarsite = :site and iardepo = :depot " ;
        string sqlInterface_retrive_barcode = @"
           select distinct 'bgc' orgcode, iarsite site, lpad(iardepo,2,'0') depot,null spcarea, iarcexr article,0 pv, iarcexvl lv,
           iareancde barops,IAREAN barcode, (case when iarcean = '1' then 'EAN13' else 'UNKNOW' end) bartype,IARCNUF thcode,'BGCTH.GC' orbitsource,'WC' tflow,    
	        null fileid, iaracti rowops, null datecreate,null dateops,null ermsg
        from icsarticle where iarsite = :site and iardepo = :depot";

        //Retrive Thirdparty to orbit 
        string sqlInterface_retrive_thirdparty = "" +
        " select 'bgc' orgcode,itisite site,lpad(itidepo,2,'0') depot,null spcarea,case when itityp = '2' then 'CS' when itityp = '1' then 'SP' when itityp = '4' then 'TP' else to_char(itityp) end  thtype,          " +
        " null thbutype,iticod thcode, iticoda thcodealt,itiiden vatcode,itilib1 thname,case when itityp = '2' then replace(replace(itidist,'(',''),')','') else itilib1 end thnamealt,itiadr1 addressln1, " + 
        " itiadr2 addressln2, null addressln3,null subdistrict,itidist district,itiville city,itipaylc country,iticpos postcode, itiregn region,itintel telephone,itimail email,null thgroup,              " + 
        " iticomm thcomment,null throuteformat, null plandelivery,null naturalloss,null mapaddress,'BGCTH.GC' orbitsource,'WC' tflow,null fileid, itiact rowops,null ermsg,null dateops                    " + 
        " from ICSTIERS where itisite = :site and itidepo = :depot ";
        
    }
}