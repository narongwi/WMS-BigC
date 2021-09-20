using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public partial class counting_ops : IDisposable { 
        private string sqltask_vald = "select count(1) rsl from wm_count where where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode ";
        private string sqltask_fnd = "select * from wm_count where orgcode = @orgcode and site = @site and depot = @depot";

        private string sqltask_insert = "" +
        " insert into wm_count ( orgcode, site,depot,spcarea,counttype,countcode,countname,datestart,  " +
        " dateend,isblock,remarks,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify )       " +
        " values ( @orgcode,@site,@depot,@spcarea,@counttype,next value for seq_cct,@countname,@datestart,@dateend,@isblock, " +
        " @remarks,@tflow,sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify)      " ;
        //private string sqltask_update = "" + 
        //" update wm_count set datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify, " +
        //" countname = @countname,datestart = @datestart,dateend = @dateend,isblock = @isblock,remarks = @remarks, " +
        //" tflow = @tflow where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode  " ;
        private string sqltask_update = "" +
       " update wm_count set datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify, " +
       " dateend = @dateend, tflow = @tflow where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode  ";
        private string sqltask_closed_plan = "update wm_coupn set tflow ='XX' ,datemodify = SYSDATETIMEOFFSET(),accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and countcode =@countcode and tflow = 'IO'";
        private string sqltask_closed_line = "update wm_couln set tflow ='XX' ,datemodify = SYSDATETIMEOFFSET(),accnmodify = @accnmodify  where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and tflow = 'IO'";

        private string sqltask_remove_step1 = "" +
        " delete from wm_count where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode  " ;
        private string sqltask_remove_step2 = "" +
        " delete from wm_coupn where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode  " ;
        private string sqltask_remove_step3 = "" +
        " delete from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode  " ;


        private string sqlplan_vald = "select count(1) rsl from wm_coupn where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode ";
        private string sqlbulk_zone = "SELECT count(1) isbl from wm_locup WHERE lower(orgcode) = @orgcode and site = @site and depot = @depot AND spcarea =@szone AND fltype = 'BL'";
        //private string sqlbulk_zone = "SELECT count(1) isbl from wm_locup WHERE lower(orgcode) = @orgcode and site = @site and depot = @depot AND lscode =@szone AND fltype = 'BL'";
        private string sqlplan_fnd = "select * from wm_coupn where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode order by cast(plancode as int)";
        private string sqlplan_insert_step1 = "" + 
        " insert into wm_coupn (orgcode,site,depot,spcarea,countcode,plancode,planname,accnassign,accnwork,szone,ezone,    " + 
        " saisle,eaisle,sbay,ebay,slevel,elevel,isroaming,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify," +
        " isblock, isdatemfg, isdatexp, isbatchno, allowscanhu, isserailno,planorigin) values (     " + 
        " @orgcode,@site,@depot,@spcarea,@countcode,@plancode,@planname,@accnassign,@accnwork,@szone,@ezone,@saisle,@eaisle," +
        " @sbay,@ebay,@slevel,@elevel,@isroaming,@tflow,sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify," +
        " @isblock,@isdatemfg,@isdateexp,@isbatchno,@allowscanhu,@isserialno,@plancode)    ";

        //private string sqlplan_select_step1_1 = @"select distinct l.orgcode,l.site,l.depot,l.spcarea,@countcode countcode,@plancode plancode,l.lscode loccode, 
        //    isnull(z.spcunit,s.unitops) unitcount,'IO' tflow,sysdatetimeoffset() datecreate,@accnmodify accncreate,sysdatetimeoffset() datemodify,@accnmodify accnmodify,'generate' procmodify,
        //    case when @isbatchno = 1 then batchno else null end stlotmfg, case when @isdatemfg = 1 then cast(datemfg as date) else null end stdatemfg,
        //    case when @isdateexp = 1  then cast(dateexp as date) else null end stdateexp, case when @isserialno = 1 then serialno else null end stserialno, 
        //    case when @allowscanhu = 1 then huno else null end sthuno, l.lsaisle,l.lsbay,l.lslevel ,
        //    (select top 1 b.barcode from wm_barcode b where b.orgcode = s.orgcode  and b.site = s.site and b.depot = s.depot  and b.article=s.article and b.pv = s.pv and b.lv = s.lv and b.tflow='IO' order by isprimary desc ) barcode,
        //    s.article ,s.lv ,s.pv,s.qtysku,s.qtypu,s.batchno,s.datemfg,s.dateexp,s.serialno,s.huno, (case when(l.lsbay % 2) = 0  then 2  else 1 end) oddeven,(case when isnull(l.spcpicking,0) = 0 then 'R' else 'P' end) locctype         
        //   from wm_locdw l left 
        //    join wm_loczp z on l.orgcode = z.orgcode and l.site = z.site and l.depot = z.depot and l.lscode = z.lscode and l.tflow = 'IO' 
        //    left join wm_stock s on l.orgcode = s.orgcode and l.site = s.site and l.depot = s.depot and l.lscode = s.loccode and case when @isblock = 1 then 'IO' else s.tflow end = 'IO'
        //  where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.lszone = @szone and l.lsaisle between @saisle and @eaisle and l.lsbay between @sbay and @ebay and l.lslevel between @slevel and @elevel      
        //    and not exists (select 1 from wm_couln c where c.orgcode = l.orgcode and c.site = l.site and c.depot = l.depot and c.loccode =  l.lscode and tflow = 'IO')  ";
        //private string sqlplan_select_step1_1 = 
        //   @"select distinct l.orgcode, l.site, l.depot, l.spcarea, @countcode countcode, @plancode plancode,
        //     l.lscode loccode,  isnull(z.spcunit,s.unitops) unitcount, 'IO' tflow, sysdatetimeoffset() datecreate,
        //        @accnmodify accncreate, sysdatetimeoffset() datemodify, @accnmodify accnmodify, 'generate' procmodify,
        //        max(s.batchno) stlotmfg,max(cast(datemfg as date)) stdatemfg,max(cast(dateexp as date)) stdateexp,
        //        max(serialno) stserialno,max(huno) sthuno, l.lsaisle,l.lsbay, l.lslevel,dbo.get_barcode(l.orgcode,
        //        l.site,l.depot,s.article,s.pv,s.lv) as barcode,
        //      s.article ,s.lv ,s.pv,s.qtysku,s.qtypu,s.batchno,s.datemfg,s.dateexp,s.serialno,s.huno, 
        //        (case when(l.lsbay % 2) = 0  then 2  else 1 end) oddeven,
        //        (case when isnull(l.spcpicking,0) = 0 then 'R' else 'P' end) locctype         
        //    from wm_locdw l 
        //       left join wm_loczp z on l.orgcode = z.orgcode and l.site = z.site and l.depot = z.depot and l.lscode = z.lscode and l.tflow = 'IO' 
        //       left join wm_stock s on l.orgcode = s.orgcode and l.site = s.site and l.depot = s.depot and l.lscode = s.loccode and case when @isblock = 1 then 'IO' else s.tflow end = 'IO'
        //     where l.orgcode = @orgcode 
        //      and l.site = @site 
        //      and l.depot = @depot 
        //      and l.lszone = @szone 
        //      and l.lsaisle between @saisle and @eaisle 
        //      and l.lsbay between @sbay and @ebay 
        //      and l.lslevel between @slevel and @elevel      
        //      and not exists (select 1 from wm_couln c where c.orgcode = l.orgcode and c.site = l.site and c.depot = l.depot and c.loccode =  l.lscode and tflow = 'IO')
        //    group by l.orgcode,l.site,	l.depot,l.spcarea,l.lscode ,isnull(z.spcunit,s.unitops),l.lsaisle,l.lsbay,l.lslevel,s.article ,s.lv,s.pv,s.qtysku,s.qtypu,
        //        s.batchno,s.datemfg,s.dateexp,s.serialno,s.huno,l.spcpicking";

        // comment 18/07/2021 for change count unit
        //private string sqlplan_select_step1_1 =
        //  @"select distinct l.orgcode, l.site, l.depot, l.spcarea, @countcode countcode, @plancode plancode,
        //    l.lscode loccode,  isnull(z.spcunit,s.unitops) unitcount, 'IO' tflow, sysdatetimeoffset() datecreate,
        //    @accnmodify accncreate, sysdatetimeoffset() datemodify, @accnmodify accnmodify, 'generate' procmodify,
        //    max(s.batchno) batchno,max(cast(datemfg as date)) datemfg,max(cast(dateexp as date)) dateexp,
        //    max(serialno) serialno,max(huno) huno, l.lsaisle,l.lsbay, l.lslevel,dbo.get_barcode(l.orgcode,l.site,l.depot,s.article,s.pv,s.lv) as barcode,
        //    s.article ,s.lv ,s.pv,sum(s.qtysku) qtysku,sum(s.qtypu) qtypu,(case when(l.lsbay % 2) = 0  then 2  else 1 end) oddeven,(case when isnull(l.spcpicking,0) = 0 then 'R' else 'P' end) locctype         
        //from wm_locdw l  left join wm_loczp z on l.orgcode = z.orgcode and l.site = z.site and l.depot = z.depot and l.lscode = z.lscode and l.tflow = 'IO' 
        //   left join wm_stock s on l.orgcode = s.orgcode and l.site = s.site and l.depot = s.depot and l.lscode = s.loccode and case when @isblock = 1 then 'IO' else s.tflow end = 'IO'
        // where l.orgcode = @orgcode and l.site = @site  and l.depot = @depot  and l.lszone = @szone and l.lsaisle between @saisle and @eaisle and l.lsbay between @sbay and @ebay 
        //  and l.lslevel between @slevel and @elevel and not exists (select 1 from wm_couln c where c.orgcode = l.orgcode and c.site = l.site and c.depot = l.depot and c.loccode =  l.lscode and tflow = 'IO')
        //group by l.orgcode,l.site,	l.depot,l.spcarea,l.lscode ,isnull(z.spcunit,s.unitops),l.lsaisle,l.lsbay,l.lslevel,s.article ,s.lv,s.pv,l.spcpicking";


        //iif(isnull(l.spcpicking,0) = 0, dbo.get_unitops(l.orgcode,l.site,l.depot,s.article,s.lv),isnull(z.spcunit,s.unitops)) unitcount,

        private string sqlplan_select_step1_1 =
        @"select distinct l.orgcode, l.site, l.depot, l.spcarea, @countcode countcode, @plancode plancode,l.lscode loccode, 
            [dbo].[get_stocktake_unit](l.spcarea,l.lsloctype,l.spcpicking,l.spcpickunit,p.unitmanage,p.unitprep) unitcount,
            'IO' tflow, sysdatetimeoffset() datecreate,@accnmodify accncreate, sysdatetimeoffset() datemodify, @accnmodify accnmodify, 'wms.stockcount' procmodify,
            max(s.batchno) batchno,max(cast(datemfg as date)) datemfg,max(cast(dateexp as date)) dateexp,
            max(serialno) serialno,max(huno) huno, l.lsaisle,l.lsbay, l.lslevel,dbo.get_barcode(l.orgcode,l.site,l.depot,s.article,s.pv,s.lv) as barcode,
            s.article ,s.lv ,s.pv,sum(s.qtysku) qtysku,sum(s.qtypu) qtypu,(case when(l.lsbay % 2) = 0  then 2  else 1 end) oddeven,(case when isnull(l.spcpicking,0) = 0 then 'R' else 'P' end) locctype         
        from wm_locdw l  
           left join wm_loczp z on l.orgcode = z.orgcode and l.site = z.site and l.depot = z.depot and l.lscode = z.lscode and l.tflow = 'IO' 
           left join wm_stock s on l.orgcode = s.orgcode and l.site = s.site and l.depot = s.depot and l.lscode = s.loccode and case when @isblock = 1 then 'IO' else s.tflow end = 'IO'
           left join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and p.tflow = 'IO'
         where l.orgcode = @orgcode and l.site = @site  and l.depot = @depot  and l.lszone = @szone and l.lsaisle between @saisle and @eaisle and l.lsbay between @sbay and @ebay 
          and l.lslevel between @slevel and @elevel and not exists (select 1 from wm_couln c where c.orgcode = l.orgcode and c.site = l.site and c.depot = l.depot and c.loccode =  l.lscode and tflow = 'IO')
        group by l.orgcode,l.site,	l.depot,l.spcarea,l.lscode ,isnull(z.spcunit,s.unitops),l.lsaisle,l.lsbay,l.lslevel,s.article ,s.lv,s.pv,l.spcpicking,l.lsloctype,l.spcpickunit,p.unitmanage,p.unitprep";

        // comment 18/07/2021 for change count unit

        private string sqlplan_select_step1_2 =
            @"select l.orgcode, l.site, l.depot, l.spcarea, @countcode countcode, @plancode plancode,l.lscode loccode,
                [dbo].[get_stocktake_unit](l.spcarea,l.lsloctype,l.spcpicking,l.spcpickunit,p.unitmanage,p.unitprep) unitcount,
                'IO' tflow, sysdatetimeoffset() datecreate,@accnmodify accncreate, sysdatetimeoffset() datemodify, @accnmodify accnmodify, 'wms.stockcount' procmodify,
                s.batchno,cast(datemfg as date) datemfg,cast(dateexp as date) dateexp,s.serialno,huno huno, l.lsaisle,l.lsbay, l.lslevel,
                dbo.get_barcode(l.orgcode,l.site,l.depot,s.article,s.pv,s.lv) as barcode,s.article ,s.lv ,s.pv,s.qtysku,s.qtypu,1 oddeven,l.spcarea locctype        
            from wm_locdw l 
                left join wm_stock s on l.orgcode = s.orgcode and l.site = s.site and l.depot = s.depot and l.lscode = s.loccode and case when @isblock = 1 then 'IO' else s.tflow end = 'IO'
                left join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv=p.lv and p.tflow= 'IO'
            WHERE not exists (select 1 from wm_couln c where c.orgcode = l.orgcode and c.site = l.site and c.depot = l.depot and c.loccode =  l.lscode AND c.sthuno = s.huno and c.tflow = 'IO')
            AND l.orgcode = @orgcode and l.site = @site and l.depot = @depot  AND l.fltype = 'BL' and l.spcarea = @szone  AND l.tflow='IO'
            order BY ( case WHEN isnull(p.isdlc,0) = 1 THEN s.dateexp ELSE s.daterec end) ASC,s.huno asc";

        private string sqlplan_insert_step2_1 = @"insert into wm_couln (orgcode,site,depot,spcarea,countcode,plancode,loccode,unitcount,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify, 
        stbarcode, starticle, stpv, stlv, stqtysku, stqtypu,stlotmfg, stdatemfg, stdateexp, stserialno,sthuno ,locseq,locctype,seqno) values(@orgcode,@site,@depot,@spcarea,@countcode,@plancode,@loccode,@unitcount,@tflow,
        getdate(),@accncreate,getdate(),@accnmodify,@procmodify,@stbarcode, @starticle,@stpv,@stlv,@stqtysku,@stqtypu,@stlotmfg,@stdatemfg,@stdateexp,@stserialno,@sthuno,@locseq,@locctype,cast(@locseq as numeric(8,2)) + 0.01)";

        //private string sqlplan_validate_step1 = ""+ 
        //@"update t set tflow = case when isnull(isskip,0) = 1 and stbarcode is null then 'ED'              
        //    when starticle = cnarticle and stpv = cnpv and stlv = cnlv and stqtypu = cnqtypu then 'ED'               
        //    when starticle = cnarticle and stpv = cnpv and stlv = cnlv and stqtysku != cnqtysku then 'WQ'                                    
        //    when isnull(cnbarcode,'') != '' and isnull(cnbarcode,'aa') != isnull(stbarcode,'xx') then 'WP'                                   
        //    when isnull(isskip,0) = 0 and stbarcode is null then 'WC' end  from wm_couln t                                     
        //where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.countcode = @countcode and t.plancode = @plancode and tflow = 'IO'";

        //private string sqlplan_validate_step2 = ""+
        //" update t set cnttime = CAST(CONVERT(varchar(12), DATEADD(minute, DATEDIFF(minute, datestart, sysdatetimeoffset()), 0), 114) AS time(7))," +
        //"     datevld = sysdatetimeoffset(), pctvld  = '10',accnvld = @accnmodify,cntpercentage = l.cntpercentage,cnterror = l.cnterr,            " +
        //"     cntlines = l.cntlines,remarksvld = @remarksvld,datemodify = sysdatetimeoffset(),procmodify = 'count.plan.validate',                 " +
        //"     accnmodify = @accnmodify,tflow = 'ED' from wm_coupn t,                                                                              " +
        //" (select orgcode, site, depot ,countcode, plancode, count(1) cntlines,                                                                   " +
        //"         sum(case when tflow != 'ED' then 1 else 0 end) cnterr,                                                                          " +
        //"         (sum(case when countdate is not null then 1 else 0 end) / count(1)) * 100 cntpercentage                                        " +
        //" from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode          " +
        //"     group by orgcode, site, depot ,countcode, plancode) l                                                                               " +
        //"  where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.countcode = l.countcode and t.plancode = l.plancode        " +
        //"    and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.countcode = @countcode and t.plancode = @plancode             " ;

        private string sqlplan_update = "" + 
        " update wm_coupn set planname = @planname,accnassign = @accnassign, isroaming = @isroaming,tflow = @tflow,       " + 
        " remarksvld = @remarksvld,datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify             " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode" ;
        //private string sqlplan_remove_step1 = "" +
        //" delete from wm_coupn where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode" ;
        //private string sqlplan_remove_step2 = "" +
        //" delete from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode" ;

        private string sqlplan_cancel_step1 = " update wm_coupn set tflow = 'XX', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode ";
        private string sqlplan_cancel_step2 = "  update wm_couln set tflow = 'XX', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode ";

        private string sqlplan_valcount_step0 = "  select count(1) issave from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode  and cnqtypu is null";
        private string sqlplan_valcount_step1 = " update wm_coupn set tflow = 'ED', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode ";
        private string sqlplan_valcount_step2 = "  update wm_couln set tflow = 'ED',datemodify = sysdatetimeoffset(), accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode ";


        private string sqlplan_iscyclecount = "select count(1) from wm_count where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and counttype ='CC'";
        //private string sqlplan_valcount_line =
        //@"update wm_couln set 
	       // tflow = case when @pctvld = 0 then 'WQ'  
	       // 	         when @pctvld = 100 then 'ED' when cnqtypu = isnull(stqtypu,0) then 'ED' 
	       // 	         when abs(((isnull(cnqtypu,0) * 100) / iif(isnull(stqtypu,0) = 0 , 1 , stqtypu))-100) > @pctvld then 'WQ' 
	       // 	         else 'ED' end,
	       // cnmsg = case when @pctvld = 0 then 'Recount'  when @pctvld = 100 then 'Validated'
	       // 	         when cnqtypu = isnull(stqtypu,0) then 'Validated' 
	       // 	         when abs(((isnull(cnqtypu,0) * 100) / iif(isnull(stqtypu,0) = 0 , 1 , stqtypu))-100) > @pctvld then 'Recount' 
	       // 	         else 'Validated' end,
	       //isrgen = case when @pctvld = 0 then 1 when @pctvld = 100 then 0 
	       // 	         when cnqtypu = isnull(stqtypu,0) then 0 
	       // 	         when abs(((isnull(cnqtypu,0) * 100) / iif(isnull(stqtypu,0) = 0 , 1 , stqtypu))-100) > @pctvld then 1 
	       // 	         else 0 end,
	       //iswrgln = case when isnull(stqtypu,0) <> isnull(cnqtypu,0) then  1 	else  0  end,
	       //corcode = case when isnull(stqtypu,0) >= isnull(cnqtypu,0) then '-' else '+' end,
	       //corqty  = case when isnull(stqtypu,0) >= isnull(cnqtypu,0) then isnull(cnqtypu,0)-isnull(stqtypu,0) 
	       // 	         else isnull(cnqtypu,0)  - isnull(stqtypu,0) end,
	       //coraccn =  @accnmodify,cordate = sysdatetimeoffset()
        //where orgcode = @orgcode 
        //and site = @site 
        //and depot = @depot 
        //and countcode = @countcode
        //and plancode = @plancode";

        //private string sqlplan_valcount_head =
        //   @"update wm_coupn 
	       //     set tflow = 'ED',
	       //     pctvld = @pctvld,
	       //     accnvld = @accnmodify,
	       //     remarksvld = @remarksvld,
	       //     datevld  = sysdatetimeoffset()
        //    where orgcode = @orgcode 
        //    and site = @site 
        //    and depot = @depot 
        //    and countcode = @countcode 
        //    and plancode = @plancode";

        private string sqlplan_valcount_isre = "select count(1) from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode   and tflow in('OL','OH','WQ')";
        private string sqlplan_valcount_close = "select count(1) from wm_coupn where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode  and tflow in('ED','CL','XX')";
        private string sqlplan_recount_head =
         @"insert into wm_coupn (orgcode, site, depot, spcarea, countcode, plancode, planname, szone, ezone, saisle,eaisle, sbay, ebay, slevel, elevel, 
        isroaming, tflow, datecreate, accncreate, datemodify, accnmodify, procmodify, isblock, isdatemfg, isdatexp,isbatchno, allowscanhu, isserailno,planorigin)
        select orgcode, site, depot, spcarea, countcode,@newplan plancode,@planname planname, szone, ezone, saisle,eaisle, sbay, ebay, slevel, 
	        elevel, isroaming, 'IO' tflow, sysdatetimeoffset() datecreate,@accnmodify accncreate, sysdatetimeoffset() datemodify, @accnmodify accnmodify, 
	        procmodify, isblock, isdatemfg, isdatexp,isbatchno, allowscanhu, isserailno,iif(isnull(planorigin,0)=0,plancode,planorigin) planorigin
        from wm_coupn where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode ";

       // private string sqlplan_recount_line =
       //@"insert into wm_couln(orgcode, site, depot, spcarea, countcode, plancode, loccode, locseq, unitcount, stbarcode, starticle, stpv, stlv, stqtysku, 
       // stqtypu, stlotmfg, stdatemfg, stdateexp, stserialno, sthuno,tflow, datecreate, accncreate, datemodify, accnmodify, procmodify,locctype)
       // select orgcode, site, depot, spcarea, countcode, @newplan plancode, loccode, locseq, unitcount, stbarcode, starticle, stpv, stlv, stqtysku, stqtypu, stlotmfg, stdatemfg, stdateexp, 
       // stserialno, sthuno,'IO' tflow, sysdatetimeoffset() datecreate, @accnmodify accncreate,sysdatetimeoffset() datemodify,@accnmodify accnmodify,'' procmodify,locctype        
       // from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode and tflow in('OL','OH','WQ')";
     
        private string sqlplan_recount_line =
        @"insert into wm_couln(orgcode, site, depot, spcarea, countcode, plancode, loccode, locseq, unitcount, stbarcode, starticle, stpv, stlv, stqtysku, 
        stqtypu, stlotmfg, stdatemfg, stdateexp, stserialno, sthuno,tflow, datecreate, accncreate, datemodify, accnmodify, procmodify,locctype, seqno ,addnew)
        select orgcode, site, depot, spcarea, countcode, @newplan plancode, loccode, locseq, unitcount, cnbarcode stbarcode, cnarticle as starticle,cnpv as stpv,cnlv as stlv,cnqtysku as stqtysku, 
	    cnqtypu as stqtypu, cnlotmfg as stlotmfg, cndatemfg as stdatemfg, cndateexp as stdateexp,cnserialno as stserialno,cnhuno as sthuno,'IO' tflow, sysdatetimeoffset() datecreate,
	    @accnmodify accncreate,sysdatetimeoffset() datemodify, @accnmodify accnmodify,'' procmodify, locctype,locseq + 0.01 as seqno , 0 as addnew
        from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode and tflow in('OL','OH','WQ')";


        // private string sqlplan_recount_reline =
        //@"insert into wm_couln(orgcode, site, depot, spcarea, countcode, plancode, loccode, locseq, unitcount, stbarcode, starticle, stpv, stlv, stqtysku, 
        // stqtypu, stlotmfg, stdatemfg, stdateexp, stserialno, sthuno,tflow, datecreate, accncreate, datemodify, accnmodify, procmodify,locctype)
        // select orgcode, site, depot, spcarea, countcode, @newplan plancode, loccode, locseq, unitcount, stbarcode, starticle, stpv, stlv, stqtysku, stqtypu, stlotmfg, stdatemfg, stdateexp, 
        // stserialno, sthuno,'IO' tflow, sysdatetimeoffset() datecreate, @accnmodify accncreate,sysdatetimeoffset() datemodify,@accnmodify accnmodify,'' procmodify,locctype
        // from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode";

        //private string sqlline_fnd = "select * from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode  ";
        //private string sqlline_fnd = "	select *,p.description from wm_couln l left join wm_product p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot " +
        //    " and l.starticle = p.article  and l.stpv= p.pv and l.stlv = p.lv where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.countcode = @countcode " +
        //    "and l.plancode = @plancode ";
        private string sqlline_fnd = @"SELECT l.*, p.description FROM ( SELECT
            h.orgcode,h.site,h.depot,h.spcarea,l.countcode,l.plancode,l.loccode,l.locseq,l.unitcount,l.stbarcode,l.starticle,l.stpv,l.stlv,
            l.stqtysku,l.stqtypu,l.stlotmfg,l.stdatemfg,l.stdateexp,l.stserialno,l.sthuno,
            iif(l.cnflow is null,l.stbarcode,l.cnbarcode) cnbarcode,
            iif(l.cnflow is null,l.starticle,l.cnarticle) cnarticle, 
            iif(l.cnflow is null,l.stpv,l.cnpv) cnpv, 
            iif(l.cnflow is null,l.stlv,l.cnlv) cnlv,l.cnqtysku,l.cnqtypu, 
            iif(l.cnflow is null,l.stlotmfg,l.cnlotmfg) cnlotmfg, 
            iif(l.cnflow is null,l.stdatemfg,l.cndatemfg) cndatemfg, 
            iif(l.cnflow is null,l.stdateexp,l.cndateexp) cndateexp, 
            iif(l.cnflow is null,l.stserialno,l.cnserialno) cnserialno, 
            iif(l.cnflow is null,l.sthuno,l.cnhuno) cnhuno, 
            l.cnflow,l.cnmsg,l.isskip,l.isrgen,l.iswrgln,l.countdevice,
            l.countdate,l.corcode,l.corqty,l.coraccn,l.cordevice,l.cordate,
            l.tflow,l.datecreate,l.accncreate,l.datemodify,l.accnmodify,l.procmodify,
            l.locctype,l.seqno,l.addnew from wm_coupn h 
              join wm_couln l on h.orgcode = l.orgcode and h.site = l.site 
              and h.depot = l.depot and h.countcode = l.countcode and h.plancode = l.plancode 
            where l.orgcode = @orgcode
             and l.site = @site
             and l.depot = @depot
             and l.countcode = @countcode
             and l.plancode = @plancode
            ) l left join wm_product p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot  and l.cnarticle = p.article  and l.cnpv= p.pv and l.cnlv = p.lv
            where 1=1 ";

        private string sqlcount_fnd = @"SELECT l.*, p.description FROM ( SELECT
             h.orgcode,h.site,h.depot,h.spcarea,l.countcode,l.plancode,l.loccode,l.locseq,l.unitcount,l.stbarcode,l.starticle,l.stpv,l.stlv,
             l.stqtysku,l.stqtypu,l.stlotmfg,l.stdatemfg,l.stdateexp,l.stserialno,l.sthuno,
            iif(l.cnflow is null,l.stbarcode,l.cnbarcode) cnbarcode,
            iif(l.cnflow is null,l.starticle,l.cnarticle) cnarticle, 
            iif(l.cnflow is null,l.stpv,l.cnpv) cnpv, 
            iif(l.cnflow is null,l.stlv,l.cnlv) cnlv,l.cnqtysku,l.cnqtypu, 
            iif(l.cnflow is null,l.stlotmfg,l.cnlotmfg) cnlotmfg, 
            iif(l.cnflow is null,l.stdatemfg,l.cndatemfg) cndatemfg, 
            iif(l.cnflow is null,l.stdateexp,l.cndateexp) cndateexp, 
            iif(l.cnflow is null,l.stserialno,l.cnserialno) cnserialno, 
            iif(l.cnflow is null,l.sthuno,l.cnhuno) cnhuno, 
            l.cnflow,l.cnmsg,l.isskip,l.isrgen,l.iswrgln,l.countdevice,
            l.countdate,l.corcode,l.corqty,l.coraccn,l.cordevice,l.cordate,
            l.tflow,l.datecreate,l.accncreate,l.datemodify,l.accnmodify,l.procmodify,
            l.locctype,l.seqno,l.addnew from wm_coupn h 
              join wm_couln l on h.orgcode = l.orgcode and h.site = l.site 
              and h.depot = l.depot and h.countcode = l.countcode and h.plancode = l.plancode 
            where l.orgcode = @orgcode
             and l.site = @site
             and l.depot = @depot
             and l.countcode = @countcode
             and l.plancode = @plancode
            ) l left join wm_product p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot 
              and l.cnarticle = p.article  and l.cnpv= p.pv and l.cnlv = p.lv ";

        private string sqlline_vald = "select count(1) rsl from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode and loccode = @loccode";
        private string sqlline_insert = "" + 
        " insert into wm_couln ( orgcode,site,depot,spcarea,countcode,plancode,loccode,locseq,unitcount,stbarcode,starticle,stpv,         " +
        " stlv,stqtysku,stqtypu,stlotmfg,stdatemfg,stdateexp,stserialno,sthuno,cnbarcode,cnarticle,cnpv,cnlv,cnqtysku,cnqtypu,cnlotmfg,   " +
        " cndatemfg,cndateexp,cnserialno,cnhuno,cnflow,cnmsg,isskip,isrgen,iswrgln,countdevice,countdate,corcode,corqty,coraccn,          " +
        " cordevice,cordate,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify ) values  ( @orgcode,@site,@depot,@spcarea,      " +
        " @countcode,@plancode,@loccode,@locseq,@unitcount,@stbarcode,@starticle,@stpv,@stlv,@stqtysku,@stqtypu,@stlotmfg,@stdatemfg,     " +
        " @stdateexp,@stserialno,@sthuno,@cnbarcode,@cnarticle,@cnpv,@cnlv,@cnqtysku,@cnqtypu,@cnlotmfg,@cndatemfg,@cndateexp,@cnserialno," +
        " @cnhuno,@cnflow,@cnmsg,@isskip,@isrgen,@iswrgln,@countdevice,@countdate,@corcode,@corqty,@coraccn,@cordevice,@cordate,@tflow,   " +
        " sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify )                                                   " ;

        // private string sqlline_update = " update wm_couln set cnbarcode = @cnbarcode, cnqtypu = @cnqtypu, cnlotmfg = @cnlotmfg, cndatemfg = @cndatemfg, cndateexp = @cndateexp, cnserialno = @cnserialno, countdate = sysdatetimeoffset(), " + 
        // " datemodify = sysdatetimeoffset(), accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and loccode = @loccode and locseq = @locseq and countcode = @countcode and plancode = @plancode ";

        private string sqlline_update = @" update wm_couln set cnhuno=@cnhuno, cnbarcode = @cnbarcode,cnarticle=@cnarticle,cnpv=@cnpv,cnlv=@cnlv,cnqtypu = @cnqtypu, cnlotmfg = @cnlotmfg, cndatemfg = @cndatemfg, cndateexp = @cndateexp, cnserialno = @cnserialno,countdate = sysdatetimeoffset(),datemodify = sysdatetimeoffset(), accnmodify = @accnmodify,cnflow=@cnflow,cnmsg=@cnmsg from wm_couln where orgcode = @orgcode and site = @site and depot = @depot and loccode = @loccode 
           and locseq = @locseq and countcode = @countcode and plancode = @plancode and tflow in ('IO')";

        //private string sqlline_update2 = @" update t set stqtysku = isnull(s.qtysku,0), stqtypu = isnull(s.qtypu,0), starticle = s.article , stpv = s.pv , stlv = s.lv, 
        //            cnbarcode = @cnbarcode, cnqtypu = @cnqtypu, cnlotmfg = @cnlotmfg, cndatemfg = @cndatemfg, cndateexp = @cndateexp, cnserialno = @cnserialno, 
        //            countdate = sysdatetimeoffset(),datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, stbarcode = s.barcode,
        //            cnarticle = s.article, cnpv = s.pv , cnlv = s.lv
        //from wm_couln t left join wm_coupn p on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.countcode = p.countcode and t.plancode = p.plancode
        //    left join 
        //    (select s.orgcode, s.site, s.depot, s.loccode,dbo.get_barcode_active(s.orgcode,s.site, s.depot,s.article, s.pv,s.lv) barcode, s.article,s.pv,s.lv,sum(qtysku) qtysku, sum(qtypu) qtypu,
        //                case when p.isdatemfg = 1 then s.datemfg else null end datemfg, case when p.isdatexp = 1 then s.dateexp else null end dateexp,
        //                case when p.isbatchno = 1 then isnull(s.lotno,'')   else '' end lotno,   case when p.allowscanhu = 1 then isnull(s.huno,'')  else '' end huno
        //        from  wm_stock s , 
        //            (select orgcode,site, depot, isdatemfg, isdatexp, isbatchno, allowscanhu from wm_coupn 
        //                where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and plancode = @plancode ) p
        //        where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and loccode = @loccode
        //        and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot
        //        group by s.orgcode, s.site, s.depot, s.loccode, s.article, s.pv, s.lv,
        //                case when p.isdatemfg = 1 then s.datemfg else null end , case when p.isdatexp = 1  then s.dateexp else null end ,
        //                case when p.isbatchno = 1 then isnull(s.lotno,'')   else '' end , case when p.allowscanhu = 1 then isnull(s.huno,'') else '' end 
        //        ) s 
        //     on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.loccode = s.loccode and @cnbarcode = s.barcode
        //    and case when p.isbatchno = 1 then isnull(s.lotno,'') else '' end = case when p.isbatchno = 1 then isnull(t.cnlotmfg,'') else '' end 
        //    and case when p.allowscanhu = 1 then isnull(s.huno,'') else '' end = case when p.allowscanhu = 1 then isnull(t.cnhuno,'') else '' end
        //    and case when p.isdatemfg = 1 then s.datemfg else isnull(null,'') end = case when p.isdatemfg = 1 then t.cndatemfg else isnull(null,'') end
        //    and case when p.isdatexp = 1 then s.dateexp else isnull(null,'') end = case when p.isdatexp = 1 then t.cndateexp else isnull(null,'') end 
        // where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.loccode = @loccode and t.locseq = @locseq and t.countcode = @countcode and t.plancode = @plancode and t.tflow in ('IO') ";

        private string sqlcount_isvalidateplan = "select count(1) from wm_coupn where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode and tflow not in('ED','XX','WQ','OL','OH')";
        private string sqlcount_correctionline = @"select  t.orgcode ,t.site,t.depot,t.counttype,t.countcode,t.countname,max(l.cnbarcode) cnbarcode,l.cnarticle,l.cnpv,cnlv,a.descalt,
		        case when sum(l.corqty) > 0 then '+' when sum(l.corqty) < 0 then '-' else '' end as corcode,sum(l.corqty) * a.rtoipckofpck as corsku,
		        sum(l.corqty) corqty,isnull(l.unitcount,a.unitmanage) unitcount,a.rtoipckofpck,a.skugrossweight,a.skuvolume, a.unitmanage , a.unitprep,t.tflow
        from wm_count t join wm_coupn p on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.countcode = p.countcode 
            join wm_couln l on p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.countcode = l.countcode and p.plancode = l.plancode 
            join wm_product a on l.orgcode = a.orgcode and l.site = a.site and l.depot = a.depot  and l.cnarticle = a.article and l.cnpv = a.pv and l.cnlv = a.lv
        where t.orgcode = @orgcode and t.site = @site and t.depot =@depot and t.countcode = @countcode 
        and t.tflow = 'IO' and t.counttype = 'CT' and p.tflow ='ED' and l.tflow ='ED' and l.corqty <> 0
        group by t.orgcode,t.site,t.depot,t.counttype,t.countcode,t.countname,l.cnarticle,l.cnpv,cnlv,isnull(l.unitcount,a.unitmanage),
        a.descalt,a.rtoipckofpck,a.skugrossweight,a.skuvolume, a.unitmanage , a.unitprep,t.tflow HAVING sum(l.corqty) <> 0";

        //private string sqlcount_confirmline = @" select  t.orgcode ,t.site,t.depot,t.spcarea,t.datestart,t.dateend,t.counttype,t.countcode,t.countname,p.plancode,p.planname,p.pctvld,l.locseq,l.loccode,l.sthuno,
        //l.stbarcode,l.starticle,l.stpv,l.stlv,a.descalt,l.unitcount,l.stqtysku,l.stqtypu,l.cnqtypu,l.corcode,l.corqty,a.rtoipckofpck,a.skugrossweight,a.skuvolume, a.unitmanage , a.unitprep,t.tflow,
        //    l.cndateexp,l.cndatemfg ,l.cnlotmfg,l.cnserialno,l.cnflow,l.cnmsg,p.isdatemfg , p.isdatexp , p.isbatchno, p.isserailno,l.locctype
        //from wm_count t join wm_coupn p on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.countcode = p.countcode 
	       // join wm_couln l on p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.countcode = l.countcode and p.plancode = l.plancode 
	       // join wm_product a on l.orgcode = a.orgcode and l.site = a.site and l.depot = a.depot and l.starticle = a.article and l.stpv = a.pv and l.stlv = a.lv
        //where t.orgcode = @orgcode and t.site = @site and t.depot =@depot and t.countcode = @countcode and t.tflow = 'IO' and t.counttype = 'CT' and p.tflow ='ED' and l.tflow ='ED'
        //and l.corqty <> 0 order by l.locseq asc";

        //private string sqlcount_confirmtask = "" +
        //       " update wm_count set datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify,tflow = 'ED'" +
        //       " where orgcode = @orgcode and site = @site and depot = @depot and countcode = @countcode";

        //private string sqlcorrection_updateresult = "" +
        //      @"UPDATE wm_couln set cnflow = @cnflow , cnmsg = @cnmsg ,datemodify = SYSDATETIMEOFFSET(),accnmodify = @accnmodify
        //       where orgcode = @orgcode and countcode = @countcode and plancode = @plancode and locseq = @locseq";

        //private string sqlcount_getproduct = @"select top 1 b.barcode,b.article,b.pv,b.lv,p.descalt,p.rtoskuofpu,p.unitprep
        //    from wm_product p left join wm_barcode b on p.orgcode = b.orgcode and p.site = b.site and p.depot = b.depot and p.article =b.article and p.pv=b.pv and p.lv = b.lv  and b.isprimary = 1 
        //    where p.orgcode = @orgcode  and p.site = @site and p.depot = @depot  and (p.article = @article or b.barcode = @barcode)";


        //private string sqlcount_correctchk = "select top 1 stockid,inrefno,thcode,qtypu ,qtysku,unitops from wm_stock ws where orgcode = @orgcode and site = @site and depot = @depot " +
        //    "and loccode = @loccode and huno = @huno and  article = @article and pv = @pv and lv = @lv";

        //private string sqlcount_newcheck = "select count(1) existrow from wm_couln l where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.countcode = @countcode " +
        //    "   and l.plancode = @plancode and l.loccode=@loccode and l.cnhuno=@cnhuno and l.cnarticle =@cnarticle and l.cnpv = @cnpv and l.cnlv=@cnlv";

        private string sqlcount_newline = @"insert into wm_couln (orgcode, site, depot, spcarea, countcode, plancode, loccode, locseq, unitcount, stbarcode, starticle, stpv, stlv, 
            stqtysku, stqtypu, stlotmfg, stdatemfg, stdateexp, stserialno, sthuno, cnbarcode, cnarticle, cnpv, cnlv, cnqtysku, cnqtypu, cnlotmfg, 
            cndatemfg, cndateexp, cnserialno, cnhuno, cnflow, cnmsg, isskip, isrgen, iswrgln, countdevice, countdate, corcode, corqty, coraccn, 
            cordevice, cordate, tflow, datecreate, accncreate, datemodify, accnmodify, procmodify, locctype, seqno, addnew)
            select top 1 orgcode, site, depot, spcarea, countcode, plancode, loccode,
            (select max(locseq) + 1 from wm_couln s where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.countcode =  @countcode and s.plancode = @plancode ) locseq,
            @unitcount unitcount, stbarcode, starticle, stpv, stlv, stqtysku, stqtypu, stlotmfg, stdatemfg, stdateexp, stserialno, sthuno, @cnbarcode cnbarcode,@cnarticle cnarticle,@cnpv cnpv,
            @cnlv cnlv,@cnqtysku cnqtysku,@cnqtypu cnqtypu,@cnlotmfg cnlotmfg, @cndatemfg cndatemfg,@cndateexp cndateexp,@cnserialno,@cnhuno cnhuno,@cnflow cnflow,
            @cnmsg cnmsg , isskip, isrgen, iswrgln, countdevice,SYSDATETIMEOFFSET() countdate, corcode, corqty, coraccn, cordevice, cordate,tflow,SYSDATETIMEOFFSET() datecreate,
            @accncreate accncreate, SYSDATETIMEOFFSET() datemodify, @accnmodify accnmodify, procmodify, locctype,@seqno + 0.01 as seqno, @addnew as addnew  
            from wm_couln wc where wc.orgcode=@orgcode  and wc.site = @site and wc.depot = @depot and wc.countcode = @countcode and wc.plancode = @plancode and wc.locseq = @locseq and wc.tflow='IO';
            select max(locseq) locseq from wm_couln s where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.countcode =  @countcode and s.plancode = @plancode and s.loccode = @loccode and s.cnhuno = @cnhuno";

        private string sqlcount_findproduct =
            @"select top 1 p.orgcode,p.[site],p.depot,p.article,p.pv,p.lv,p.descalt,p.skuweight,p.skuvolume,p.unitmanage,
                   [dbo].[get_barcode](p.orgcode,p.[site],p.depot,p.article,p.pv,p.lv) barcode,l.loccode,l.spcarea locarea,l.lsloctype loctype,spcpickunit locunit,
                   [dbo].[get_stocktake_unit](l.spcarea,l.lsloctype,l.spcpicking,l.spcpickunit,p.unitmanage,p.unitprep) unitcount,
                   [dbo].[get_unitdes](p.orgcode,p.[site],p.depot,[dbo].[get_stocktake_unit](l.spcarea,l.lsloctype,l.spcpickunit,l.spcpickunit,p.unitmanage,p.unitprep)) unitdestr,
                   [dbo].[get_ratio](p.orgcode,p.[site],p.depot,p.article,p.pv,p.lv,[dbo].[get_stocktake_unit](l.spcarea,l.lsloctype,l.spcpickunit,l.spcpickunit,p.unitmanage,p.unitprep)) skuofunit,
                   p.rtoskuofpu,p.rtopckoflayer,p.rtolayerofhu,(p.rtopckoflayer * p.rtolayerofhu) rtopckofpallet,p.rtoskuofipck,p.rtoskuofpck,p.rtoskuoflayer,p.rtoskuofhu
            from [dbo].[wm_product] p  
                cross join  (
                    select top 1 l.lscode loccode,l.spcarea,l.lsloctype,l.spcpickunit,l.spcpicking
                    from [dbo].[wm_locdw] l 
                    where l.orgcode = @orgcode 
                        and l.[site] = @site 
                        and l.depot = @depot 
                        and l.lscode = @loccode 
                        and l.tflow='IO'
                ) l
            where p.orgcode = @orgcode 
                and p.site = @site 
                and p.depot = @depot 
                and p.tflow = 'IO' 
                and exists (
                    select top 1 article 
                    from [dbo].[wm_barcode] b 
                    where p.orgcode = b.orgcode and p.site = b.site and p.depot = b.depot
                    and p.article=b.article and p.pv = b.pv and p.lv = b.lv and b.tflow = 'IO'
                    and (b.barcode = @product or b.article = @product)
                ) ";

        private string sqlcount_huactive =
          @"select count(1) ishuno
                from wm_handerlingunit s 
                where s.orgcode = @orgcode
                    and s.site = @site
                    and s.depot = @depot
                    and s.huno = @huno
                    and s.tflow = 'IO'";

        private string sqlcount_huother_product=
            @"select count(1) ishuno
                from wm_stock s 
                where s.orgcode = @orgcode
                    and s.site = @site 
                    and s.depot = @depot
                    and s.huno = @huno
                     and s.loccode <> @loccode";
    }
}