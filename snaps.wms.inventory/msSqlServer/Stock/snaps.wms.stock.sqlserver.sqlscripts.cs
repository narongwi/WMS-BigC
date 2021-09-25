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

    public partial class stock_ops : IDisposable { 

            //Find product stock 
            private String sqlstock_fnd = ""+
            " select p.orgcode,p.site,p.depot,p.spcarea,p.article,p.pv,p.lv,articletype,                      " +
            " description, p.thcode, t.thnamealt, isnull(cronhand,0) cronhand, p.tflow, p.unitprep unitmanage,         " +
            " 		case when p.unitprep = 1 then 1                                                         " +
            " 			 when p.unitprep = 2 then rtoskuofipck                                              " +
            " 			 when p.unitprep = 3 then rtoskuofpck                                               " +
            " 			 when p.unitprep = 4 then rtoskuoflayer                                             " +
            " 			 when p.unitprep = 5 then rtoskuofhu                                              " +
            " 		end unitratio ,                                                                            " +
            "           Ceiling((p.dlcall * p.dlcfactory)/100) dlcfactory, " +
            "            Ceiling((dlcall* (p.dlcshop + p.dlconsumer))/100) as dlcwarehouse ,p.dlcall " +
            " from wm_product p                                                                               " +
            " left join wm_thparty t on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot       " +
            "       and t.thcode = p.thcode                                                                   " +
            " left join wm_productstate s on p.orgcode = s.orgcode and p.site = s.site and p.depot = s.depot  " + 
            "        and p.article = s.article and p.pv = s.pv and p.lv = s.lv                                " + 
            " where p.orgcode = @orgcode and p.site = @site and p.depot = @depot                              ";
 
            //Get stock info
            string sqlstock_info = " SELECT  " + 
            "   p.orgcode,p.site,p.depot,b.barcode,p.article,p.pv,p.lv,stampdate,isnull(s.cronhand,0) cronhand,      " + 
            "   isnull(cravailable,0) cravailable, isnull(crbulknrtn,0) crbulknrtn, isnull(croverflow,0) croverflow," +
            " 	isnull(crprep,0) crprep,isnull(crstaging,0) crstaging,isnull(crrtv,0) crrtv,                        " +
            " 	isnull(crsinbin,0) crsinbin,isnull(crdamage,0) crdamage,isnull(crblock,0) crblock,                  " +
            "   isnull(crincoming,0) crincoming,isnull(crplanship,0) crplanship,isnull(crexchange,0) crexchange,    " + 
            "   isnull(crtask,0) crtask, p.unitprep unitmanage, " + 
            " 		case when p.unitprep = 1 then 1                                                               " +
            " 			 when p.unitprep = 2 then rtoskuofipck                                                    " +
            " 			 when p.unitprep = 3 then rtoskuofpck                                                     " +
            " 			 when p.unitprep = 4 then rtoskuoflayer                                                   " +
            " 			 when p.unitprep = 5 then rtoskuofhu                                                      " +
            " 		end unitratio,p.thcode,t.thnameint ,p.descalt ,p.skuweight skuweight                            " +
            "       ,cast(FORMAT(p.rtopckoflayer,'N0') as varchar(10)) + ' x ' + cast(FORMAT(p.rtolayerofhu,'N0') as varchar(10)) tihi " +
            "       ,cast(FORMAT(p.rtoskuofipck ,'N0') as varchar(10)) + ' x ' +  cast(FORMAT(p.rtoipckofpck ,'N0') as varchar(10)) dimension" +
            "       ,p.isdlc,p.isunique,p.isbatchno," +
            "       Ceiling((p.dlcall * p.dlcfactory)/100) dlcfactory, " +
            "       Ceiling((dlcall* (p.dlcshop + p.dlconsumer))/100) as dlcwarehouse ,p.dlcall " +
            " FROM wm_product p "+ 
            " left join  ( select * from dbo.wm_productstate where orgcode = @orgcode and site = @site and depot = @depot " + 
            "  and article = @article and pv = @pv and lv = @lv ) s on s.orgcode = p.orgcode and s.site = p.site     " + 
            "  and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv  " +
            " left join wm_thparty t on s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and p.thcode = t.thcode " +
            " left join (select orgcode,site,depot, article,pv,lv,barcode from wm_barcode where tflow = 'IO' and isprimary = 1 " +
            "			   and orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv " + 
            " 		   ) b on p.orgcode = b.orgcode and p.site = b.site and p.depot = b.depot and p.article = b.article and p.pv = b.pv and p.lv = b.lv " +
            " where p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.article = @article and p.pv = @pv and p.lv = @lv ";

            //Validate state before blocking stock 
            string sqlstock_block_valid = " select isnull((select tflow rsl from wm_stock " + 
            " where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv and stockid = @stockid),'NF') rsl ";

            //Blocking stock on WMS
            string sqlstock_block_step1 = "update wm_stock set tflow = @tflow, stkremarks = @stkremarks ,datemodify = sysdatetimeoffset(), accnmodify = @accnmodify,procmodify = @procmodify " + 
            " where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv and stockid = @stockid ";

            //Send blocking stock to orbit
            public string sqlstock_block_step2 = @"insert into xm_xoblock (orgcode ,site,depot,spcarea,stockid,hutype,huno,hunosource,thcode,
            inrefno,inrefln,loccode,article,pv,lv,qtysku,qtypu,qtyweight,qtyvolume,daterec,batchno,
            lotno,datemfg,dateexp,serialno,stkremarks,tflow,opstype,xaction,xcreate, xmodify, xmsg,rowid,accnmodify)
            select orgcode ,site,depot,spcarea,stockid,hutype,huno,hunosource,thcode,inrefno,inrefln
            ,loccode,article,pv,lv,qtysku,qtypu,qtyweight,qtyvolume,daterec,batchno,lotno,datemfg
            ,dateexp,serialno,stkremarks,tflow,@opstype,'WC' xaction,sysdatetimeoffset() xcreate
            ,null xmodify,null xmsg,next value for seq_oxblock rowid,@accnmodify accnmodify from wm_stock where orgcode = @orgcode and site = @site 
            and depot = @depot and stockid = @stockid and article = @article and pv = @pv and lv = @lv ";
            //Remarks stock 
            string sqlstock_remarks = " update wm_stock set tflow = @tflow, stkremarks = @stkremarks ,datemodify = sysdatetimeoffset(), accnmodify = @accnmodify,procmodify = @procmodify " +
            " where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv and stockid = @stockid ";

            //Stock line detail 
            public string sqlstock_detail_all = "" + 
            " SELECT	 s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,s.huno,hunosource,thcode			    								" +
            " 	   ,inrefno,inrefln,loccode,article,pv,lv,isnull(qtysku,0) qtysku,qtypu - isnull(qtyprep,0) qtypu,isnull(qtyweight,0) qtyweight " + 
            " 	   ,isnull(qtyvolume,0) qtyvolume,daterec,batchno,lotno,datemfg,dateexp,serialno,stkremarks,s.tflow                             " +
            " 	   ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,isnull(qtyprep,0) qtyprep                                  " +
            "      ,case when l.spcarea in ('BL') and s.tflow = 'IO' then 'crbulknrtn'                                                          " +
            " 			 when l.spcarea in ('OV') and s.tflow = 'IO' then 'croverflow'                                                          " +
            " 			 when l.spcarea in ('RS','DS') and s.tflow = 'IO' then 'crstaging'                                                      " +
            " 			 when l.spcarea = 'RN' and s.tflow = 'IO' then 'crrtv'                                                                  " +
            " 			 when l.spcarea = 'SB' and s.tflow = 'IO' then 'crsinbin'                                                               " +
            " 			 when l.spcarea = 'DM' and s.tflow = 'IO' then 'crdamage'                                                               " +
            " 			 when l.spcarea = 'XC' and s.tflow = 'IO' then 'crexchange'                                                             " +
            " 			 when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 1 and ap.huno is null then 'crpicking'                     " +
            " 			 when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 0 and ap.huno is null then 'crreserve'                     " +
            " 			 when l.spcarea = 'ST' and lsloctype = 'LC' and s.huno = ap.huno then 'crtask'                                          " +
            " 			 when l.tflow in ('XX','IX') or s.tflow = 'XX' then 'crblock'                                                           " +
            " 			 else 'notfound' end tflowsign, inagrn,ingrno,unitops                                                                   " +
            " from wm_stock s left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode " +
            @" left join (select t.orgcode, t.site, t.depot, t.sourcehuno huno from wm_taln t, wm_task l where l.tflow in ('IO','SS','PT') 
                        and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.article = @article and t.pv = @pv and t.lv = @lv 
                        and t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.tasktype = 'A') ap
                on s.orgcode = ap.orgcode and s.site = ap.site and s.depot = ap.depot and s.huno = ap.huno "+
            " where s.qtysku > 0 " +
            " and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv			";

            public string sqlstock_detail_staging = "" + 
            " SELECT	 s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,huno,hunosource,thcode			    								" +
            " 	   ,inrefno,inrefln,loccode,article,pv,lv,isnull(qtysku,0) qtysku,qtypu - isnull(qtyprep,0) qtypu,isnull(qtyweight,0) qtyweight " + 
            " 	   ,isnull(qtyvolume,0) qtyvolume,daterec,batchno,lotno,datemfg,dateexp,serialno,stkremarks,s.tflow                             " +
            " 	   ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,isnull(qtyprep,0) qtyprep                                  " +
            "      ,case when l.spcarea in ('BL') and s.tflow = 'IO' then 'crbulknrtn'                                                          " +
            " 			 when l.spcarea in ('OV') and s.tflow = 'IO' then 'croverflow'                                                          " +
            " 			 when l.spcarea in ('RS','DS') and s.tflow = 'IO' then 'crstaging'                                                      " +
            " 			 when l.spcarea = 'RN' and s.tflow = 'IO' then 'crrtv'                                                                  " +
            " 			 when l.spcarea = 'SB' and s.tflow = 'IO' then 'crsinbin'                                                               " +
            " 			 when l.spcarea = 'DM' and s.tflow = 'IO' then 'crdamage'                                                               " +
            " 			 when l.spcarea = 'XC' and s.tflow = 'IO' then 'crexchange'                                                             " +
            " 			 when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 1 then 'crpicking'                                         " +
            " 			 when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 0 then 'crreserve'                                         " +
            " 			 when l.tflow in ('XX','IX') or s.tflow = 'XX' then 'crblock'                                                           " +
            " 			 else 'notfound' end tflowsign, inagrn,ingrno,unitops                                                                             " +
            " from wm_stock s left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode " +
            " where s.qtysku > 0 and l.spcarea in ('RS','DS') " +
            " and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv			";


        public string sqlstock_detail_sinbin = "" +
        " SELECT	 s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,huno,hunosource,thcode			    								" +
        " 	   ,inrefno,inrefln,loccode,article,pv,lv,isnull(qtysku,0) qtysku,qtypu - isnull(qtyprep,0) qtypu,isnull(qtyweight,0) qtyweight " +
        " 	   ,isnull(qtyvolume,0) qtyvolume,daterec,batchno,lotno,datemfg,dateexp,serialno,stkremarks,s.tflow                             " +
        " 	   ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,isnull(qtyprep,0) qtyprep                                  " +
        "      ,case when l.spcarea in ('BL') and s.tflow = 'IO' then 'crbulknrtn'                                                          " +
        " 			 when l.spcarea in ('OV') and s.tflow = 'IO' then 'croverflow'                                                          " +
        " 			 when l.spcarea in ('RS','DS') and s.tflow = 'IO' then 'crstaging'                                                      " +
        " 			 when l.spcarea = 'RN' and s.tflow = 'IO' then 'crrtv'                                                                  " +
        " 			 when l.spcarea = 'SB' and s.tflow = 'IO' then 'crsinbin'                                                               " +
        " 			 when l.spcarea = 'DM' and s.tflow = 'IO' then 'crdamage'                                                               " +
        " 			 when l.spcarea = 'XC' and s.tflow = 'IO' then 'crexchange'                                                             " +
        " 			 when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 1 then 'crpicking'                                         " +
        " 			 when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 0 then 'crreserve'                                         " +
        " 			 when l.tflow in ('XX','IX') or s.tflow = 'XX' then 'crblock'                                                           " +
        " 			 else 'notfound' end tflowsign, inagrn,ingrno,unitops                                                                             " +
        " from wm_stock s left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode " +
        " where s.qtysku > 0 and l.spcarea = 'SB' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv			";

        //Stock line detail of task movement 
        public string sqlstock_detail_task = ""+
            " select	t.orgcode, t.site, t.depot, '' spcarea,stockid, '' hutype,sourcehuno huno,'' hunosource, '' thcode,taskno inrefno,taskseq inrefln,    " +
            " 		sourceloc loccode, article, pv, lv, sourceqty qtysku, cast(isnull(sourceqty,0) as decimal(18,0)) qtypu, cast(0 as decimal(18,0)) qtyweight,        " +
            " 		cast(0 as decimal(18,0)) qtyvolume, null daterec, batchno, lotno, datemfg, dateexp,serialno, '' stkremarks, tflow,          " +
            " 		t.datecreate,t.accncreate,t.datemodify,t.accnmodify,t.procmodify,0 qtyprep,'crtask' tflowsign,null inagrn,null ingrno,null unitops   " +
            "   from wm_taln t                                                                                                                  " +
            " where tflow in ('IO','SS','PT')                                                                                                   " +
            " and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and article = @article and pv = @pv and lv = @lv                 ";
            //Stock line detail of preparation > prep lot + full pallet 
            public string sqlstock_detail_prep = @" select  s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,hunosource huno,huno hunosource,'' thcode 
             		,opsno inrefno,opsln inrefln,loccode,article,pv,lv,rsvsku qtysku,cast(rsvpu as decimal(18,3)) qtypu,cast(0 as decimal(18,0)) qtyweight 
             		,cast(0 as decimal(18,0)) qtyvolume ,null daterec,batchno,lotno,datemfg,dateexp,serialno,'' stkremarks,s.tflow 
             		,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,0 qtyprep,'crprep' tflowsign, inagrn,ingrno,null unitops 
             from wm_stobc s where s.tflow = 'IO' and rsvpu > 0 
             and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and article = @article and pv = @pv and lv = @lv 
             union all
            select	t.orgcode, t.site, t.depot, '' spcarea,stockid, '' hutype,sourcehuno huno,'' hunosource, '' thcode,t.taskno inrefno,taskseq inrefln, 
                    sourceloc loccode, article, pv, lv, sourceqty qtysku, isnull(sourceqty,0) qtypu, cast(0 as decimal(18,0)) qtyweight, 
                    cast(0 as decimal(18,0)) qtyvolume, null daterec, batchno, lotno, datemfg, dateexp,serialno, '' stkremarks, t.tflow,   
                    t.datecreate,t.accncreate,t.datemodify,t.accnmodify,t.procmodify,0 qtyprep,'crtask' tflowsign,null inagrn,null ingrno,null unitops 
            from wm_taln t, wm_task l
            where t.tflow in ('IO','SS','PT') and l.orgcode = t.orgcode and l.site = t.site and l.depot = t.depot and l.taskno = t.taskno and l.tasktype = 'A'
            and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and article = @article and pv = @pv and lv = @lv ";
            //Stock line detail of inbound incoming
            public string sqlstock_detail_incoming = "" +
            " select 	t.orgcode, t.site, t.depot, '' spcarea, cast(0 as decimal(18,0)) stockid, '' hutype,inorder huno,'' hunosource, '' thcode,inorder inrefno,inln inrefln, " +
            " 		'' loccode, article, pv, lv, qtysku qtysku, cast(isnull(qtypu,0) as decimal(18,3)) qtypu, cast(isnull(qtyweight,0) as decimal(18,0)) qtyweight, cast(0 as decimal(18,0)) qtyvolume,                                    " +
            " 		null daterec,'' batchno,'' lotno,null datemfg,null dateexp,'' serialno, '' stkremarks,'IO' tflow,                         " +
            " 		t.datecreate,t.accncreate,t.datemodify,t.accnmodify,t.procmodify,0 qtyprep,                                         " +
            " 		'crincoming' tflowsign,null inagrn,null ingrno,null unitops                                                                                " +
            " from wm_inbouln t where tflow = 'IO'                                                                                    " +
            "  and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and article = @article and pv = @pv and lv = @lv        ";
            //Stokc line detail of outbound plan delivery 
            public string sqlstock_detail_planship = "" +
            " select 	t.orgcode, t.site, t.depot, '' spcarea, cast(0 as decimal(18,0)) stockid, '' hutype,ouorder huno,'' hunosource, '' thcode,ouorder inrefno,ouln inrefln, " +
            " 		disthcode+ '   '+ p.thnameint loccode, article, pv, lv, qtysku qtysku, cast(isnull(qtypu,0) as decimal(18,3)) qtypu, cast(isnull(qtyweight,0) as decimal(18,0)) qtyweight, cast(0 as decimal(18,0)) qtyvolume,              " +
            " 		null daterec,'' batchno,'' lotno,null datemfg,null dateexp,'' serialno, '' stkremarks,'IO' tflow,                         " +
            " 		t.datecreate,t.accncreate,t.datemodify,t.accnmodify,t.procmodify,0 qtyprep,                                         " +
            " 		'crplanship' tflowsign,null inagrn,null ingrno,null unitops                                                                                " +
            " from wm_outbouln t left join wm_thparty p on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot              " +
            " and t.disthcode = p.thcode where t.tflow = 'IO'                                                                           " +
            " and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and article = @article and pv = @pv and lv = @lv         ";
            //Stock line detail of blocking 
            public string sqlstock_detail_block = "" + 
            " SELECT	 s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,huno,hunosource,thcode	" + 
            " 	   ,inrefno,inrefln,loccode,article,pv,lv,isnull(qtysku,0) qtysku,qtypu - isnull(qtyprep,0) qtypu,isnull(qtyweight,0) qtyweight " + 
            " 	   ,isnull(qtyvolume,0) qtyvolume,daterec,batchno,lotno,datemfg,dateexp,serialno,stkremarks,s.tflow  " + 
            " 	   ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,isnull(qtyprep,0) qtyprep  " + 
            "       ,'crblock' tflowsign,null inagrn,null ingrno,null unitops " + 
            " from wm_stock s left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode " + 
            " where qtysku > 0 and ( s.tflow = 'XX' or  isnull(lsloctype,'XX') = 'BK' or l.tflow in ('XX','IX') ) " + 
            " and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv  " ;
            
            //stock without movement 
            public string sqlstock_detail_womovement =  @"select s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,huno,hunosource,thcode 
                ,inrefno,inrefln,loccode,article,pv,lv,isnull(qtysku,0) qtysku,cast(qtypu - isnull(qtyprep,0) as decimal(10,2)) qtypu,isnull(qtyweight,0) qtyweight 
                ,isnull(qtyvolume,0) qtyvolume,daterec,batchno,lotno,datemfg,dateexp,serialno,stkremarks,s.tflow 
                ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,isnull(qtyprep,0) qtyprep 
                ,case when l.spcarea in ('BL') and s.tflow = 'IO' then 'crbulknrtn' 
                        when l.spcarea in ('OV') and s.tflow = 'IO' then 'croverflow' 
                        when l.spcarea in ('RS','DS') and s.tflow = 'IO' then 'crstaging' 
                        when l.spcarea = 'RN' and s.tflow = 'IO' then 'crrtv' 
                        when l.spcarea = 'SB' and s.tflow = 'IO' then 'crsinbin' 
                        when l.spcarea = 'DM' and s.tflow = 'IO' then 'crdamage' 
                        when l.spcarea = 'XC' and s.tflow = 'IO' then 'crexchange' 
                        when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 1 then 'crpicking' 
                        when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 0 then 'crreserve' 
                        when l.tflow in ('XX','IX') or s.tflow = 'XX' then 'crblock' 
                        else 'notfound' end tflowsign, inagrn,ingrno,unitops 
                from wm_stock s left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode 
                where s.qtysku > 0 
                and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv 
                and not exists (select 1 from wm_stobc b where s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and tflow not in ('PE','CL','ED')) 
                and not exists (select 1 from wm_taln  t where t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.sourcehuno = s.huno and t.tflow in ('IO','SS','PT') )";

        //Stock available
        //public string sqlstock_detail_available =  @"select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.hutype, s.huno, s.hunosource, s.thcode, s.inrefno, s.inrefln, s.loccode, 
        //        s.article, s.pv, s.lv, s.qtysku - isnull(b.qtysku,0) qtysku, s.qtypu - isnull(b.qtypu,0) qtypu, 
        //        s.qtyweight - isnull(b.qtyweight,0) qtyweight, s.qtyvolume - isnull(b.qtyvolume,0) qtyvolume, 
        //        s.daterec, s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.stkremarks, s.tflow, s.datecreate, s.accncreate,
        //        s.datemodify, s.accnmodify, s.procmodify, s.qtyprep, s.tflowsign, s.inagrn, s.ingrno, s.unitops 
        //from 
        //(select s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,huno,hunosource,thcode 
        //    ,inrefno,inrefln,loccode,article,pv,lv,isnull(qtysku,0) qtysku,qtypu - isnull(qtyprep,0) qtypu,isnull(qtyweight,0) qtyweight 
        //    ,isnull(qtyvolume,0) qtyvolume,daterec,batchno,lotno,datemfg,dateexp,serialno,stkremarks,s.tflow 
        //    ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,isnull(qtyprep,0) qtyprep 
        //    ,case when l.spcarea in ('BL') and s.tflow = 'IO' then 'crbulknrtn' 
        //            when l.spcarea in ('OV') and s.tflow = 'IO' then 'croverflow' 
        //            when l.spcarea in ('RS','DS') and s.tflow = 'IO' then 'crstaging' 
        //            when l.spcarea = 'RN' and s.tflow = 'IO' then 'crrtv' 
        //            when l.spcarea = 'SB' and s.tflow = 'IO' then 'crsinbin' 
        //            when l.spcarea = 'DM' and s.tflow = 'IO' then 'crdamage' 
        //            when l.spcarea = 'XC' and s.tflow = 'IO' then 'crexchange' 
        //            when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 1 then 'crpicking' 
        //            when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 0 then 'crreserve' 
        //            when l.tflow in ('XX','IX') or s.tflow = 'XX' then 'crblock' 
        //            else 'notfound' end tflowsign, inagrn,ingrno,unitops 
        //from wm_stock s left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode 
        //where s.qtysku > 0 and l.spcarea not in ('XC','SB','RN','RS','DS','BL','DM') and s.tflow = 'IO'
        //and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv 
        //) s left join 
        //( select orgcode, site, depot, spcarea, stockid,hutype,huno, hunosource, thcode, inrefno, inrefln, loccode, article, pv, lv, 
        //        sum(qtysku) qtysku, sum(qtypu) qtypu, sum(qtyweight) qtyweight, sum(qtyvolume) qtyvolume, daterec, batchno, lotno, 
        //        datemfg, dateexp, serialno, stkremarks, tflow, datecreate, accncreate,
        //        datemodify, accnmodify, procmodify, qtyprep, tflowsign, inagrn, ingrno, unitops
        //from (
        //select  s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,hunosource huno,huno hunosource,'' thcode 
        //        ,opsno inrefno,opsln inrefln,loccode,article,pv,lv,rsvsku qtysku,rsvpu qtypu,cast(0 as decimal(18,0)) qtyweight 
        //        ,cast(0 as decimal(18,0)) qtyvolume ,null daterec,batchno,lotno,datemfg,dateexp,serialno,'' stkremarks,s.tflow 
        //        ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,0 qtyprep,'crprep' tflowsign, inagrn,ingrno,null unitops 
        //from wm_stobc s where s.tflow = 'IO' and rsvpu > 0 
        //and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and article = @article and pv = @pv and lv = @lv 
        //union all 
        //select	t.orgcode, t.site, t.depot, '' spcarea,stockid, '' hutype,sourcehuno huno,'' hunosource, '' thcode,t.taskno inrefno,taskseq inrefln, 
        //        sourceloc loccode, article, pv, lv, sourceqty qtysku, isnull(sourceqty,0) qtypu, cast(0 as decimal(18,0)) qtyweight, 
        //        cast(0 as decimal(18,0)) qtyvolume, null daterec, batchno, lotno, datemfg, dateexp,serialno, '' stkremarks, t.tflow,   
        //        t.datecreate,t.accncreate,t.datemodify,t.accnmodify,t.procmodify,0 qtyprep,'crtask' tflowsign,null inagrn,null ingrno,null unitops 
        //from wm_taln t, wm_task l
        //where t.tflow in ('IO','SS','PT') and l.orgcode = t.orgcode and l.site = t.site and l.depot = t.depot and l.taskno = t.taskno and l.tasktype = 'A'
        //and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and article = @article and pv = @pv and lv = @lv 
        //) minus
        //group by orgcode, site, depot, spcarea, stockid,hutype,huno, hunosource, thcode, inrefno, inrefln, loccode, article, pv, lv, 
        //        daterec, batchno, lotno, datemfg, dateexp, serialno, stkremarks, tflow, datecreate, accncreate,
        //        datemodify, accnmodify, procmodify, qtyprep, tflowsign, inagrn, ingrno, unitops ) b
        //on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.huno = b.huno
        //where s.qtysku - isnull(b.qtysku,0) > 0 ";
        public string sqlstock_detail_available = @"select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.hutype, s.huno, s.hunosource, s.thcode, s.inrefno, s.inrefln, s.loccode, 
                    s.article, s.pv, s.lv, s.qtysku - isnull(b.qtysku,0) qtysku, s.qtypu - isnull(b.qtypu,0) qtypu, 
                    s.qtyweight - isnull(b.qtyweight,0) qtyweight, s.qtyvolume - isnull(b.qtyvolume,0) qtyvolume, 
                    s.daterec, s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.stkremarks, s.tflow, s.datecreate, s.accncreate,
                    s.datemodify, s.accnmodify, s.procmodify, s.qtyprep, s.tflowsign, s.inagrn, s.ingrno, s.unitops 
            from 
            (select s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,huno,hunosource,thcode 
                ,inrefno,inrefln,loccode,article,pv,lv,isnull(qtysku,0) qtysku,qtypu - isnull(qtyprep,0) qtypu,isnull(qtyweight,0) qtyweight 
                ,isnull(qtyvolume,0) qtyvolume,daterec,batchno,lotno,datemfg,dateexp,serialno,stkremarks,s.tflow 
                ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,isnull(qtyprep,0) qtyprep 
                ,case when l.spcarea in ('BL') and s.tflow = 'IO' then 'crbulknrtn' 
                        when l.spcarea in ('OV') and s.tflow = 'IO' then 'croverflow' 
                        when l.spcarea in ('RS','DS') and s.tflow = 'IO' then 'crstaging' 
                        when l.spcarea = 'RN' and s.tflow = 'IO' then 'crrtv' 
                        when l.spcarea = 'SB' and s.tflow = 'IO' then 'crsinbin' 
                        when l.spcarea = 'DM' and s.tflow = 'IO' then 'crdamage' 
                        when l.spcarea = 'XC' and s.tflow = 'IO' then 'crexchange' 
                        when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 1 then 'crpicking' 
                        when l.spcarea = 'ST' and lsloctype = 'LC' and spcpicking = 0 then 'crreserve' 
                        when l.tflow in ('XX','IX') or s.tflow = 'XX' then 'crblock' 
                        else 'notfound' end tflowsign, inagrn,ingrno,unitops 
            from wm_stock s left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode 
            where s.qtysku > 0 and l.spcarea not in ('XC','SB','RN','RS','DS','BL','DM') and s.tflow = 'IO'
            and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv 
            ) s left join 
            ( 		select orgcode, site, depot, spcarea, stockid,hutype,huno, loccode, article, pv, lv, 
	        sum(qtysku) qtysku, sum(qtypu) qtypu, sum(qtyweight) qtyweight, sum(qtyvolume) qtyvolume
		    from 
		    ( select  s.orgcode,s.site,s.depot,s.spcarea,stockid,hutype,hunosource huno,huno hunosource,'' thcode 
		            ,opsno inrefno,opsln inrefln,loccode,article,pv,lv,rsvsku qtysku,rsvpu qtypu,cast(0 as decimal(18,0)) qtyweight 
		            ,cast(0 as decimal(18,0)) qtyvolume ,null daterec,batchno,lotno,datemfg,dateexp,serialno,'' stkremarks,s.tflow 
		            ,s.datecreate,s.accncreate,s.datemodify,s.accnmodify,s.procmodify,0 qtyprep,'crprep' tflowsign, inagrn,ingrno,null unitops 
		        from wm_stobc s where s.tflow = 'IO' and rsvpu > 0 
		        and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and article = @article and pv = @pv and lv = @lv
		        union all 
		        select	t.orgcode, t.site, t.depot, '' spcarea,stockid, '' hutype,sourcehuno huno,'' hunosource, '' thcode,t.taskno inrefno,taskseq inrefln, 
		                sourceloc loccode, article, pv, lv, sourceqty qtysku, isnull(sourceqty,0) qtypu, cast(0 as decimal(18,0)) qtyweight, 
		                cast(0 as decimal(18,0)) qtyvolume, null daterec, batchno, lotno, datemfg, dateexp,serialno, '' stkremarks, t.tflow,   
		                t.datecreate,t.accncreate,t.datemodify,t.accnmodify,t.procmodify,0 qtyprep,'crtask' tflowsign,null inagrn,null ingrno,null unitops 
		        from wm_taln t, wm_task l
		        where t.tflow in ('IO','SS','PT') and l.orgcode = t.orgcode and l.site = t.site and l.depot = t.depot and l.taskno = t.taskno and l.tasktype = 'A'
			        and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and article = @article and pv = @pv and lv = @lv 
	           ) minus group by orgcode, site, depot, spcarea, stockid,hutype,huno, loccode, article, pv, lv
            ) b
            on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.huno = b.huno
            where s.qtysku - isnull(b.qtysku,0) > 0 ";

        //Valicate stock is exists 
        private string sqlstock_validate_exists = "select count(1) rsl from wm_stock where stockid = @stockid and orgcode = @orgcode        " + 
            " and site = @site and depot = @depot ";

            //Validate stock before decrease stock
            private string sqlstock_decrease_valid = "" + 
            " select isnull((select case                                                                      " +
            "                 when @opssku > s.qtysku                       then 'stock'                      " +
            "                 when @opsweight > (mxweight - h.crweight)     then 'HUweight'                   " +
            "                 when @opsvolume > (mxvolume - h.crvolume)     then 'HUvolume'                   " +
            "                 when @opssku > (mxsku - crsku)                then 'HUsku'                      " +
            "                 when h.tflow not in ('IO','LD')               then 'HUflow'                     " +
            "                 when p.tflow not in ('IO','IX')               then 'ATflow'                     " +
            "                 when p.ismeasurement = 1                      then 'ATrqm'                      " +
            "                 when t.tflow != 'IO'                          then 'THflow'                     " +
            "                 when l.tflow not in ('IX','IO')               then 'LCflow'                     " +
            "                 when @opsweight > (lsmxweight - l.crweight)   then 'LCweight'                   " +
            "                 when @opsvolume > (lsmxvolume - l.crvolume)   then 'LCvolume'                   " +
            "                 when l.tflowcnt != 'IO'                       then 'LCcount'                    " +
            "                 else 'pass'                                   end   rsl                         " +
            " from wm_stock s, wm_handerlingunit h, wm_product p, wm_thparty t, wm_locdw l                    " +
            " where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno       " +
            "   and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article " +
            "   and s.pv = p.pv and s.lv = p.lv                                                               " +
            "   and s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and s.thcode = t.thcode   " +
            "   and s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode  " +
            "   and s.stockid = @stockid  and s.orgcode = @orgcode and s.site = @site and s.depot = @depot    " +
            "   ),'notfound') rsl";

            private string sqlstock_decrease_skuinfo = " " +
            " select  @opssku / rtoskuofpu qtypu,  @opssku * skuweight qtyweight,  @opssku * skuvolume qtyvolume,hucode from wm_product where            " +
            " orgcode = orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv";


            private string sqlstock_decrease_step1 = "update wm_stock set qtysku = qtysku - @opssku, qtypu = qtypu - @opspu,                    " +
            "qtyweight = qtyweight - @opsweight, qtyvolume = qtyvolume - @opsvolume, datemodify = sysdatetimeoffset(), accnmodify = @opsaccn,   " + 
            "procmodify =@procmodify where orgcode = @orgcode and site = @site and depot = @depot and stockid = @stockid";
            private string sqlstock_decrease_step2 = " INSERT INTO wm_stockmvlx " + 
            "(orgcode,site,depot,spcarea,stockid,article,pv,lv,opsunit,opssku,opspu,opsweight,opsdate,opstype,opscode,opsroute,opsthcode,opsaccn)" +
            " values " +
            "(@orgcode,@site,@depot,@spcarea,@stockid,@article,@pv,@lv,@opsunit,@opssku,@opspu,@opsweight,sysdatetimeoffset(),@opstype,@opscode, " +
            " @opsroute,@opsthcode, @opsaccn)";

            //get stock status 
            private string sqlstock_gettflow = "select tflow from wm_stock where orgcode = @orgcode and site = @site and depot = @depot and huno = @opshuno ";

            //Validate stock before increase stock
            private string sqlstock_increase_valid = 
            @" select case when h.rsl != 'pass' then h.rsl when p.rsl != 'pass' then p.rsl when t.rsl != 'pass' then t.rsl 
                        when l.rsl != 'pass' then l.rsl else 'pass' end rsl  from 
                (select orgcode,site,depot, huno, 
                        case 
                        when ( isnull(crweight,0) + isnull(@opsweight,0) ) > mxweight then 'HUweight' 
                        when ( isnull(crvolume,0) + isnull(@opsvolume,0) ) > mxvolume then 'HUvolume' 
                        when ( isnull(crsku,0)    + isnull(@opssku,0)  )   > mxsku    then 'HUsku' 
                        when tflow != 'IO' then 'HUflow' else 'pass' end rsl 
                from wm_handerlingunit 
                where huno = @opshuno 
                and orgcode = @orgcode and site = @site and depot = @depot 
                ) h 
                left join 
                (select orgcode,site,depot, @opshuno huno, 
                        case when tflow not in ('IO','IX') then 'ATflow' 
                                when ismeasurement = 1 then 'ATrqm' else 'pass' end rsl 
                from wm_product where 1=1 
                and orgcode = @orgcode and site = @site and depot = @depot 
                and article = @article and pv = '0' and lv = '0' 
                ) p 
                on h.orgcode = p.orgcode and h.site = p.site and h.depot = p.depot and h.huno = p.huno 
                left join 
                ( select @opshuno huno,orgcode,site,depot, 
                        case when tflow != 'IO' then 'THflow' else 'pass' end rsl 
                from wm_thparty 
                where 1=1 
                and orgcode = @orgcode and site = @site and depot = @depot and thcode = @opsthcode
                ) t on h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.huno =t.huno 
                left join  
                (select @opshuno huno, orgcode, site, depot , 
                case  when tflow not in ('IX','IO') then 'LCflow' 
                        when (isnull(@opsweight,0) + isnull(crweight,0)) > lsmxweight then 'LCweight' 
                        when (isnull(@opsvolume,0) + isnull(crvolume,0)) > lsmxvolume then 'LCvolume' 
                        when tflowcnt != 'IO' then 'LCcount' 
                        when (isnull(crhu,0) + @newhu ) > lsmxhuno then 'LCOH'
                        when isnull(lsmixarticle,0) = 1 and (@article != isnull((select max(article) from wm_stock s 
                        where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = @opshuno),@article)) then 'LCMA'
                        when isnull(lsmixlotno,0)   = 1 and (@batchno != isnull((select max(batchno) from wm_stock s 
                        where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = @opshuno),@batchno)) then 'LCMB'
                        else 'pass' end rsl 
                from wm_locdw l 
                where orgcode = @orgcode and site = @site and depot = @depot and lscode = @opsloccode 
                ) l on h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.huno = l.huno ";

        //     " select case when h.rsl != 'pass' then h.rsl                                                 " +
        //     "             when p.rsl != 'pass' then p.rsl                                                 " +
        //     "             when t.rsl != 'pass' then t.rsl                                                 " +
        //     "             when l.rsl != 'pass' then l.rsl                                                 " +
        //     "             else 'pass' end rsl                                                             " +
        //     "   from                                                                                      " +
        //     "  (select orgcode,site,depot, huno,                                                          " +
        //     "          case                                                                               " +
        //     "              when @opsweight > (mxweight - crweight) then 'HUweight'                        " +
        //     "              when @opsvolume > (mxvolume - crvolume) then 'HUvolume'                        " +
        //     "              when @opssku > (mxsku - crsku)  then 'HUsku'                                   " +
        //     "              when tflow != 'IO' then 'HUflow' else 'pass' end rsl                           " +
        //     "   from wm_handerlingunit                                                                    " +
        //     "  where huno = @opshuno                                                               	  " +
        //     "    and orgcode = @orgcode and site = @site and depot = @depot                               " +
        //     "   ) h                                                                                       " +
        //     " left join                                                                                   " +
        //     "  (select orgcode,site,depot, @opshuno huno,                                                 " +
        //     "            case when tflow not in ('IO','IX') then 'ATflow'                                 " +
        //     "                 when ismeasurement = 1 then 'ATrqm' else 'pass' end rsl                     " +
        //     "    from wm_product where 1=1                                                                " +
        //     "     and orgcode = @orgcode and site = @site and depot = @depot                              " +
        //     "     and article = @article and pv = @pv and lv = @lv                                        " +
        //     "   ) p                                                                                       " +
        //     "  on h.orgcode = p.orgcode and h.site = p.site and h.depot = p.depot and h.huno = p.huno     " +
        //     " left join                                                                                   " +
        //     "  ( select @opshuno huno,orgcode,site,depot,                                          	  " +
        //     "            case when tflow != 'IO' then 'THflow' else 'pass' end rsl                        " +
        //     "      from wm_thparty                                                                        " +
        //     "     where 1=1                                                                               " +
        //     "      and orgcode = @orgcode and site = @site and depot = @depot and thcode = @opsthcode     " +
        //     "  ) t on h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.huno =t.huno  " +
        //     " left join                                                                                   " +
        //     " (select @opshuno huno, orgcode, site, depot ,                                        	  " +
        //     "       case when tflow not in ('IX','IO') then 'LCflow'                                      " +
        //     "            when @opsweight > (lsmxweight - crweight) then 'LCweight'                        " +
        //     "            when @opsvolume > (lsmxvolume - crvolume) then 'LCvolume'                        " +
        //     "            when tflowcnt != 'IO'                     then 'LCcount'                         " +
        //     "            else 'pass' end rsl                                                              " +
        //     "  from wm_locdw                                                                              " +
        //     " where 1=1                                                                                   " +
        //     "    and orgcode = @orgcode and site = @site and depot = @depot and lscode = @opsloccode      " +
        //     " ) l on h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.huno = l.huno  " ;

            private string sqlstock_increase_step1_new = "INSERT INTO wm_stock " + 
            " (orgcode,site,depot,spcarea,stockid,hutype,huno,hunosource,thcode,inrefno,inrefln,loccode     " +
            " ,article,pv,lv,qtysku,qtypu,qtyweight,qtyvolume,daterec,batchno,lotno,datemfg,dateexp         " +
            " ,serialno,stkremarks,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,qtyprep     " +
            " ,inagrn,ingrno,unitops )    " +
            " values " +
            " (@orgcode,@site,@depot,@spcarea,@stockid,@hutype,@opshuno,@opshusource,@opsthcode,@opsrefno,@inrefln  " +
            " ,@opsloccode,@article,@pv,@lv,@opssku,@opspu,@opsweight,@opsvolume,@daterec,@batchno,@lotno   " +
            " ,@datemfg,@dateexp,@serialno,@stkremarks,@tflow,sysdatetimeoffset(),@opsaccn,sysdatetimeoffset() " +
            " ,@opsaccn,@procmodify,0,@inagrn,@ingrno,@unitops ) " ;
            private string sqlstock_increase_step1_upd = "update wm_stock set qtysku = qtysku + @opssku, qtypu = qtypu + @opspu,                " +
            "qtyweight = qtyweight + @opsweight, qtyvolume = qtyvolume + @opsvolume, datemodify = sysdatetimeoffset(), accnmodify = @opsaccn," + 
            "procmodify =@procmodify where orgcode = @orgcode and site = @site and depot = @depot and stockid = @stockid";
            private string sqlstock_increase_step2 = " INSERT INTO wm_stockmvlx " + 
            "(orgcode,site,depot,spcarea,stockid,article,pv,lv,opsunit,opssku,opspu,opsweight,opsdate,opstype,opscode,opsroute,opsthcode,opsaccn,opsrefno,opsreftype)" +
            " values " +
            "(@orgcode,@site,@depot,@spcarea,@stockid,@article,@pv,@lv,@opsunit,@opssku,@opspu,@opsweight,sysdatetimeoffset(),@opstype,@opscode, " +
            " @opsroute,@opsthcode,@opsaccn,@opsrefno,@opsreftype)";


            //Gett product ratio 
            private String sqlgetratio =  @"select bndescalt unit, case when bnvalue = '1' then 1 when 
            bnvalue = '2' then rtoskuofipck when bnvalue = '3' then rtoskuofpck when 
            bnvalue = '4' then rtoskuoflayer when bnvalue = '5' then rtoskuofhu else 0 end rtosku,  
            bnvalue from 
            (select orgcode,site,depot,bndesc, bndescalt,bnvalue from wm_binary 
            where bntype = 'UNIT' and bncode = 'KEEP' and bnstate = 'IO'
            union all
            select orgcode, site, depot,'Prep' bndesc,'Prep', unitprep 
            from wm_product where orgcode = @orgcode and site = @site and depot = @depot and pv = @pv and lv = @lv and article = @article
            union all
            select orgcode, site, depot,'Recv' bndesc,'Recv', unitreceipt 
            from wm_product where orgcode = @orgcode and site = @site and depot = @depot and pv = @pv and lv = @lv and article = @article
            ) b, 
            (select orgcode,site,depot,rtoskuofpu, rtoskuofipck, rtoskuofpck, rtoskuoflayer, rtoskuofhu
            from wm_product where orgcode = @orgcode and site = @site and depot = @depot  and pv = @pv and lv = @lv and article = @article ) p 
            where b.orgcode = p.orgcode and b.site = p.site and b.depot = p.depot ";


    }
}