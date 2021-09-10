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

    public partial class task_ops : IDisposable {
        public string pagerowlimit = "select isnull((select top 1 bnvalue from wm_binary wb where orgcode = @orgcode and site = @site and depot = @depot and bntype ='DATAGRID' and bncode ='PAGEROWLIMIT'),200)";

        //Valudate constrain task 
        String sqlvalidate_task = "select count(1) rsl from wm_task where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno ";

        //Remove Task 
        String sqlremove_task_step1 = "delete from wm_task where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno ";
        String sqlremove_task_step2 = "delete form wm_taln where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno ";

        //Insert new task
        String sqlinsert_task = " insert into wm_task " +
        " ( orgcode,site, depot, spcarea, tasktype, taskno, iopromo, iorefno, priority, taskdate,   " + 
        "   tflow, datecreate, accncreate, datemodify, accnmodify, procmodify, taskname, routeno,routethcode,setno )  values                " +
        " ( @orgcode, @site, @depot, @spcarea, @tasktype, @taskno, @iopromo, @iorefno, @priority, SYSDATETIMEOFFSET(),                " + 
        "   @tflow, SYSDATETIMEOFFSET(), @accncreate, SYSDATETIMEOFFSET() , @accncreate, @procmodify, @taskname, @routeno, @routethcode, @setno ) ";
        String sqlinsert_line = "insert into wm_taln                                                                                  " +
        " ( orgcode,site, depot, spcarea, taskno, taskseq, article, pv, lv, sourceloc, sourcehuno, targetadv, targetloc, targethuno,  " +
        " targetqty, collectloc, collecthuno, collectqty, accnassign, accnwork, accnfill, accncollect, dateassign, datework,          " + 
        " datefill, datecollect, iopromo, ioreftype, iorefno, lotno, datemfg, dateexp, serialno, tflow, datecreate, accncreate,       " + 
        " datemodify, accnmodify, procmodify, sourceqty, sourcevolume,stockid, ouorder, ouln, ourefno,ourefln ) values         " + 
        " ( @orgcode, @site, @depot, @spcarea, @taskno, @taskseq, @article, @pv, @lv, @sourceloc, @sourcehuno, @targetadv, @targetloc, @targethuno, " +
        " @targetqty, null, null, 0, null, null, null, null,null, null,                                                               " +
        " null, null, @iopromo, @ioreftype, @iorefno, @lotno, @datemfg, @dateexp, @serialno, @tflow, @sysdate, @accncreate,           " + 
        " @sysdate, @accnmodify, @procmodify, @sourceqty, @sourcevolume, @stockid, @ouorder, @ouln, @ourefno, @ourefln )     ";

        string sqlupdate_hu_fullpallet = "update wm_handerlingunit set loccode = @targetadv, routeno = @routeno,opscode = @opscode, opstype = 'A' " +
        " where orgcode = @orgcode and site = @site and depot = @depot and huno = @sourcehuno and tflow = 'IO'";

        //Update Task 
        String sqlupdate_task = "update wm_task set                                                                                     " + 
        " tasktype = @tasktype,  iopromo = @iopromo, iorefno = @iorefno, priority = @priority, taskdate = @taskdate,                    " +
        " datestart = @datestart, dateend = @dateend, tflow = @tflow, datemodify = @sysdate, accnmodify = @accnmodify,                  " + 
        " procmodify = @procmodify, taskname = @taskname                                                                                " +
        " where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno" ;  

        //Select Task 
        String sqlselect_task = "select * from wm_task where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno ";

        //Select Line
        String sqlselect_line = "select t.*, p.descalt from wm_taln t, wm_product p where t.orgcode = p.orgcode and t.site = p.site     " + 
        " and t.depot = p.depot and t.article = p.article and t.pv = p.pv and t.lv = p.lv and t.orgcode = @orgcode and t.site = @site   " +
        " and t.depot = @depot and t.spcarea = @spcarea and t.taskno = @taskno ";

        //Find Task
        String sqltask_find = "select top(isnull(@rowlimit,200)) t.*, l.article,l.pv,l.lv, sourceloc, sourcehuno, targetadv,p.descalt,accnwork,                   " + 
        " case when t.datemodify > GETDATE()-1 then formatmessage(bndescalt,dbo.dsc_dateshort(t.datemodify,SYSDATETIMEOFFSET()),'')     " + 
        " else bndesc end dateremarks" +
        " from wm_task t, wm_taln l, wm_product p, ( select bnvalue,bndesc,bndescalt from wm_binary where bntype = 'TASK'               " + 
        " and bncode = 'FLOW' and bnstate = 'IO' and orgcode = @orgcode and site = @site and depot = @depot ) b                         " + 
        " where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.orgcode = p.orgcode       " + 
        " and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv and t.orgcode = @orgcode  " + 
        " and t.site = @site and t.depot = @depot and t.tflow = b.bnvalue ";

        //Assign Task
        String sqlassign_task = " update wm_taln set accnassign = @accnassign, dateassign = @sysdate, datemodify = @sysdate,              " +
        " accnmodify = @accnmodify, procmodify = 'task.assign'                                                                          " +
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea  and taskno = @taskno                     " ;

        //Start Task 
        String sqlstart_task_step1 = "update wm_task set tflow = 'PT', datestart = @sysdate,datemodify = @sysdate,                      " + 
        " accnmodify = @accnmodify, procmodify = 'task.start'                                                                           " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea  and taskno = @taskno                     ";
        String sqlstart_task_step2 = "update wm_taln set tflow = 'PT', accnwork = @accnwork, datework = @sysdate, datemodify = @sysdate," + 
        " accnmodify = @accnmodify, procmodify = 'task.start'                                                                           " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea  and taskno = @taskno                     ";

        //Fill Task 
        String sqlfill_task_step1 = "update wm_task set tflow = 'FL', dateend = @sysdate,datemodify = @sysdate,accnmodify = @accnmodify," + 
        " procmodify = 'task.fill' " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea  and taskno = @taskno                     ";
        String sqlfill_task_step2 = "update wm_taln set tflow = 'FL', accnfill = @accnfill,targetloc = @targetloc,                      " + 
        " targethuno = @targethuno, datefill = @sysdate, datecollect = @sysdate, datemodify = @sysdate,accnmodify = @accnmodify,        " + 
        " procmodify = 'task.fill'                                                                                                      " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea  and taskno = @taskno                     ";
        String sqlfill_task_step3 = " update wm_stock set loccode = @targetloc, datemodify = @sysdate, accnmodify = @accnmodify,        " +
        " procmodify = 'task.fill'                                                                                                      " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea  and huno = @sourcehuno                   " + 
        " and loccode = @sourceloc and article = @article and pv = @pv and lv = @lv                                                     ";

        //Collec Task 

        //Close Task 
        String sqlclose_task = "update wm_task set tflow = 'ED', dateend = @sysdate,datemodify = @sysdate,accnmodify = @accnmodify,     " + 
        "       opsprogress = isnull((select (count(1) * 100 ) / sum(case when tflow = 'ED' then 1 else 0 end) rsl from wm_taln l       " +  
        "                     where wm_task.orgcode = l.orgcode and wm_task.site = l.site and wm_task.depot = l.depot                   " + 
        "                     and wm_task.taskno = l.taskno),0),                                                                        " + 
        "       opsperform = CAST(CONVERT(varchar(12), DATEADD(minute, DATEDIFF(minute, taskdate, dateend), 0), 114) AS time(7) ),      " +
        "       opsrate = isnull((select sum(sourcevolume) from wm_taln l where wm_task.orgcode = l.orgcode and wm_task.site = l.site   " +
        "                 and wm_task.depot = l.depot and wm_task.taskno = l.taskno),100) / DATEDIFF(minute, taskdate, dateend)         " +
        "   where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno    " ;

        //Cancel Task
        String sqlcancel_task_step1 = "update wm_task set tflow = 'CL', datemodify = @sysdate, accnmodify = @accnmodify,                " + 
        " procmodify = 'task.cancel'                                                                                                    " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea  and taskno = @taskno                     ";
        String sqlcancel_task_step2 = "update wm_taln set tflow = 'CL', datemodify = @sysdate, accnmodify = @accnmodify,                " + 
        " procmodify = 'task.cancel' where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea                " + 
        " and taskno = @taskno ";
        String sqlcancel_task_step3 = "update wm_transfer set tflow ='CL', dateops = SYSDATETIMEOFFSET(),accnops = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and rsltaskno = @taskno";

        //validate digit 
        String sqlval_digit = "select case when @cfdigit = lsdigit then 1 else 0 end rsl from wm_taln t, wm_locdw d where t.orgcode = d.orgcode and t.site = d.site                " + 
        " and t.depot = d.depot and t.targetadv = d.lscode and t.orgcode = @orgcode and t.depot = @depot and t.site = @site  and t.taskno = @taskno ";

        //validate change location 
        String sqlval_locl = @"select wl.lscode from wm_locdw wl where wl.orgcode = @orgcode and wl.site = @site and wl.depot = @depot and wl.lscode = @lscode and wl.fltype in('LC','OV') and wl.tflow = 'IO'
            and not exists (select 1 from wm_stock st where st.orgcode = wl.orgcode and st.depot = wl.depot and st.loccode = wl.lscode)";

        //Confirm Task 
        string sqlconfirm_task_loccode = "select targetadv from wm_taln where orgcode = @orgcode and site = @site and depot = @depot " + 
        " and taskno = @taskno and taskseq = 1";
        string sqlconfirm_task_huno = "select sourcehuno from wm_taln where orgcode = @orgcode and site = @site and depot = @depot " + 
        " and taskno = @taskno and taskseq = 1";
        String sqlconfirm_task_step1 = " update	wm_taln  " +
        "   set	targetloc = @loccode,targethuno = sourcehuno,targetqty = sourceqty,accnassign = @accnmodify,   " + 
        "		accnwork = @accnmodify,accnfill = @accnmodify,dateassign = @sysdate,datework = @sysdate,    " +
        "		datefill = @sysdate,tflow = 'ED',datemodify = @sysdate,accnmodify =  @accnmodify, procmodify = 'task.confirm'   " +
        " where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno ";

        String sqlconfirm_task_step2 = " update wm_task " + 
        "   set	  datestart = @sysdate, dateend = @sysdate,  datemodify = @sysdate,         " +
        "         taskonweb = 1, devid = @device, accnmodify = @accnmodify, tflow = 'ED',                                                   " +
        "       opsprogress = isnull((select (count(1) * 100 ) / sum(case when tflow = 'ED' then 1 else 0 end) rsl from wm_taln l       " +  
        "                     where wm_task.orgcode = l.orgcode and wm_task.site = l.site and wm_task.depot = l.depot                   " + 
        "                     and wm_task.taskno = l.taskno),0),                                                                        " + 
        "        opsperform = '00:00:00',  opsrate = 0 " +
        "     where orgcode = @orgcode and site = @site and depot = @depot and taskno = @taskno    " ;
        string sqlconfirm_task_step3 = "update wm_stock set loccode = @loccode, datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, " + 
        " procmodify = 'task.collection' where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno ";

        string sqlconfirm_task_step4 = "update wm_stock set loccode = @loccode, datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, " +
        " procmodify = 'task.collection', inrefno=@ouorder where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno ";

        string sqlconfirm_task_step5 = @"update t set t.tflow = 'PE',t.crpu = cast((l.sourceqty / p.rtoskuofpu) as decimal(12,2)), 
                t.crsku = l.sourceqty,  t.crweight= cast((l.sourceqty * p.skuweight) as decimal(16,3)),
	            t.crvolume = cast((l.sourceqty * p.skuvolume) as decimal(16,3)), t.crcapacity = cast((((l.sourceqty * p.skuvolume) * 100)/t.mxvolume) as decimal(4,1))
            from wm_handerlingunit t, wm_taln l, wm_task h, wm_product p
            where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.huno = l.sourcehuno
            and h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.taskno = l.taskno and h.tasktype = 'A'
            and l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
            and h.taskno = @taskno and h.orgcode = @orgcode and h.site = @site and h.depot = @depot and t.huno = @huno";

        string sqlconfirm_task_step6 = @" update t 
	            set t.crhu = u.crhu,
	            t.crophu = u.crophu,
	            t.crweight = u.crweight,
	            t.crvolume = u.crvolume,
	            t.crcapacity = u.crcapacity
            from wm_route t,(
	            select h.orgcode,h.site, h.depot,h.routeno,  count(h.huno) crhu,count(h.huno) crophu , sum(h.crweight) crweight,sum(h.crvolume) crvolume, sum(h.crcapacity) crcapacity
	            from  wm_handerlingunit h where h.orgcode = @orgcode and h.site =@site and h.depot = @depot and h.huno = @huno
	            group by h.orgcode,h.site, h.depot,h.routeno
            ) u
            where t.orgcode = u.orgcode 
	            and t.site = u.site 
	            and t.depot = u.depot 
	            and t.routeno = u.routeno 
	            and t.tflow = 'IO'
	            and t.orgcode = @orgcode 
	            and t.site = @site 
	            and t.depot = @depot ";

        // unblock reserve stock line
        //      string sqlstock_reserved_step1 = @"update s set tflow ='ED',tflowdate = SYSDATETIMEOFFSET() from wm_prepsrl s 
        //   where exists( select 1 from wm_task t join wm_taln d on t.orgcode = d.orgcode and t.site = d.site and t.depot = d.depot and t.taskno = d.taskno
        //and s.orgcode = d.orgcode and s.site = d.site and s.depot = d.depot and s.stockid = d.stockid and s.ouorder = d.iorefno and s.article = d.article and s.pv = d.pv and s.lv = d.lv 
        //and t.orgcode  = @orgcode and t.site = @site and t.depot = @depot and tasktype ='A' and t.taskno = @taskno and t.setno = @setno)";

        string sqlstock_reserved_step1 = @"update s set tflow ='ED',tflowdate = SYSDATETIMEOFFSET() from wm_prepsrl s 
	    where exists( select 1 from wm_task t join wm_taln d on t.orgcode = d.orgcode and t.site = d.site and t.depot = d.depot and t.taskno = d.taskno
		and s.orgcode = d.orgcode and s.site = d.site and s.depot = d.depot and s.stockid = d.stockid and s.ouorder = d.iorefno 
		and t.orgcode  = @orgcode and t.site = @site and t.depot = @depot and tasktype ='A' and t.taskno = @taskno and t.setno = @setno)";

        // unblock reserve stock header
        string sqlstock_reserved_step2 = @"update r set tflow = iif((select sum(iif(l.tflow = 'IO',1,0)) from wm_prepsrl l where r.orgcode = l.orgcode and r.site = l.site and r.depot = l.depot and r.setno = l.setno and r.stockid = l.stockid) > 0,'IO','ED'),
		tflowdate = SYSDATETIMEOFFSET() from wm_prepsrp r where r.orgcode  = @orgcode and r.site = @site and r.depot = @depot and r.setno  = @setno";

        // unblock reserve stock line
        string sqlcancel_reserved_step1 = @"update s set tflow ='CL',tflowdate = SYSDATETIMEOFFSET() from wm_prepsrl s 
	    where exists( select 1 from wm_task t join wm_taln d on t.orgcode = d.orgcode and t.site = d.site and t.depot = d.depot and t.taskno = d.taskno
		and s.orgcode = d.orgcode and s.site = d.site and s.depot = d.depot and s.stockid = d.stockid and s.ouorder = d.iorefno
		and t.orgcode  = @orgcode and t.site = @site and t.depot = @depot and tasktype ='A' and t.taskno = @taskno and t.setno = @setno)";

        // unblock reserve stock header
        string sqlcancel_reserved_step2 = @"update r set tflow = iif((select sum(iif(l.tflow = 'IO',1,0)) from wm_prepsrl l where r.orgcode = l.orgcode and r.site = l.site and r.depot = l.depot and r.setno = l.setno and r.stockid = l.stockid) > 0,'IO','CL'),
		tflowdate = SYSDATETIMEOFFSET() from wm_prepsrp r where r.orgcode  = @orgcode and r.site = @site and r.depot = @depot and r.setno  = @setno";

        string sqlconfirm_task_step7 = "update wm_transfer set tflow ='ED', dateops = SYSDATETIMEOFFSET(),accnops = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and rsltaskno = @taskno";

        String sqlstategic_step1_stockvalidate = @"update t set t.tflow = 'PE', t.crvolume = l.sourcevolume
        from wm_handerlingunit t, wm_taln l, wm_task h
        where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.huno = l.sourcehuno
        and h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.taskno = l.taskno and h.tasktype = 'A'
        and h.taskno = @taskno and h.orgcode = @orgcode and h.site = @site and h.depot = @depot and t.huno = @huno";

        //Putaway stategic
        string sqlputaway_stategic_step1_product = @"select spcrecvzone,isnull(huheight,0) huheight, 
        case when isnull(spcrecvaisle,'')   != '' then (select cast(min(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'AL' and lscode = spcrecvaisle     ) else '' end spcrecvaisle, 
        case when isnull(spcrecvaisleto,'') != '' then (select cast(max(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'AL' and lscode = spcrecvaisleto   ) else '' end spcrecvaisleto, 
        case when isnull(spcrecvbay,'')     != '' then (select cast(min(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'BA' and lscode = spcrecvbay       ) else '' end spcrecvbay, 
        case when isnull(spcrecvbayto,'')   != '' then (select cast(max(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'BA' and lscode = spcrecvbayto     ) else '' end spcrecvbayto, 
        case when isnull(spcrecvlevel,'')   != '' then (select cast(min(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'LV' and lscode = spcrecvlevel     ) else '' end spcrecvlevel, 
        case when isnull(spcrecvlevelto,'') != '' then (select cast(max(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'LV' and lscode = spcrecvlevelto   ) else '' end spcrecvlevelto, 
        isnull(spcrecvlocation,'') spcrecvlocation, isnull(isslowmove,'0')  isslowmove
        from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv";

        // skipping location concept
        string sqlputaway_statetic_step2_selection = @"select isnull((select a.lscode from ( 
        select a.orgcode, a.site, a.depot, a.lscode,selpriv, row_number() over (order by cast(selpriv as numeric(16)) {5} ) rn  
        from (select  l.*, 
                case when l.spcarea = 'ST' then '99' else '90' end + 
                case when isdynamicpick = 1 then '9' else '0' end + 
                case when isdynamicpick = 1 and l.spclasttouch = p.article then '9' else '0' end + 
                case when z.lscode = l.lscode then '9' else '0' end + 
                case when spcarticle = p.article then '9' else '0' end + 
                case when spcrecvzone = l.lszone then '9' else '0' end + 
                case when spcrecvlocation = l.lscode then '9' else '0' end + 
                case when spcrecvaisle = l.lsaisle then '9' else '0' end + 
                case when spcrecvbay = l.lsbay then '9' else '0' end + 
                case when spcrecvlevel = l.lslevel then '9' else '0' end + 
                case when l.spcthcode = p.thcode then '9' else '0' end + 
                case when spcpicking = 0 then '9' else '0' end selpriv 
        from (
        select top 10 * from wm_locdw l where orgcode = @orgcode and site = @site and depot = @depot and tflow in ('IO','IX')
            and (l.spcarea = 'ST' and l.fltype = 'LC' and l.lsloctype = 'LC')  and (isnull(l.lsgaptop,0) + {6}) <= l.lsdmheight {7}
            and lsmxhuno >= isnull(crhu,0) +1 and lsmxweight >= isnull(crweight,0) + 0 and lsmxvolume >= isnull(crvolume,0) + 0
            and not exists (select 1 from wm_stock where wm_stock.orgcode = l.orgcode and wm_stock.site = l.site and wm_stock.depot = l.depot and wm_stock.loccode = l.lscode) 
            and not exists (select 1 from wm_taln where wm_taln.orgcode = l.orgcode and wm_taln.site = l.site and wm_taln.depot = l.depot and ( wm_taln.sourceloc = l.lscode or wm_taln.targetadv = l.lscode ) and wm_taln.tflow = 'IO') 
            {0} {1} {2} {3} {4} 
        ) l 
        left join (select top 1 * from wm_product where orgcode = @orgcode and site = @site and depot = @depot and tflow = 'IO' and article = @article and pv = @pv and lv = @lv) p 
            on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot 
        left join (select top 1 * from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcproduct = @article and spcpv = @pv and spclv = @lv ) z 
            on p.orgcode = z.orgcode and p.site = z.site and p.depot = z.depot and p.spcprepzone = z.przone   
        where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and article = @article and pv = @pv and lv = @lv) a 
        ) a where rn = 1), 
        ( select top 1 lscode from wm_locdw where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'OV' and fltype = 'BL' and lsloctype = 'OV' and tflow = 'IO') ) lscode";
        // " " +
        // " select a.lscode from ( select  a.orgcode, a.site, a.depot, a.lscode,selpriv,  " + 
        // " row_number() over (order by cast(selpriv as numeric(16)) desc,  " + 
        // " cast(case when isnull(spcpivot,'') = '' then '9999999' else spcpivot end as numeric(9)) asc) rn " + 
        // "  from ( select  l.*,  " + 
        // "        case when l.spcarea = 'ST' then '99' else '90' end +  " + 
        // "        case when isdynamicpick = 1 then '9' else '0' end +  " + 
        // "        case when isdynamicpick = 1 and l.spclasttouch = p.article then '9' else '0' end +  " + 
        // "        case when z.lsloc = l.lscode then '9' else '0' end +  " + 
        // "        case when spcarticle = p.article then '9' else '0' end +  " + 
        // "        case when spcrecvzone = l.lszone then '9' else '0' end +  " + 
        // "        case when spcrecvlocation = l.lscode then '9' else '0' end +  " + 
        // "        case when spcrecvaisle = l.lsaisle then '9' else '0' end +  " + 
        // "        case when spcrecvbay = l.lsbay then '9' else '0' end +  " + 
        // "        case when spcrecvlevel = l.lslevel then '9' else '0' end +  " + 
        // "        case when l.spcthcode = p.thcode then '9' else '0' end +  " + 
        // "        case when spcpicking = 0 then '9' else '0' end selpriv  " + 
        // "   from (select * from wm_locdw where orgcode = @orgcode and site = @site and depot = @depot and tflow in ('IO','IX')) l  " + 
        // "    left join (select * from wm_product where orgcode = @orgcode and site = @site and depot =@depot and tflow = 'IO') p  " + 
        // "     on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot  " + 
        // "    left join (select * from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcproduct = @article) z  " + 
        // "     on p.orgcode = z.orgcode and p.site = z.site and p.depot = z.depot and p.spcprepzone = z.przone  " + 
        // "  where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and article = @article and pv = 0 and lv = 0 and l.tflow in ('IO','IX')  " + 
        // "  and ( (l.spcarea = 'ST' and l.fltype = 'LC' and l.lsloctype = 'LC') or (l.spcarea = 'BL' and l.fltype = 'BL' and l.lsloctype = 'BL') )  " + 
        // " ) a where 1=1  " + 
        // " and not exists (select 1 from wm_stock where wm_stock.orgcode = a.orgcode and wm_stock.site = a.site and wm_stock.depot = a.depot and wm_stock.loccode = a.lsloc)  " + 
        // " and not exists (select 1 from wm_taln where wm_taln.orgcode = a.orgcode and wm_taln.site = a.site and wm_taln.depot = a.depot and wm_taln.sourceloc = a.lscode and wm_taln.tflow = 'IO') " + 
        // " and not exists (select 1 from wm_taln where wm_taln.orgcode = a.orgcode and wm_taln.site = a.site and wm_taln.depot = a.depot and wm_taln.targetadv = a.lscode and wm_taln.tflow = 'IO') " + 
        // " and (lsmxweight - crweight) > @crweight and (lsmxvolume - crvolume) > @crvolume ) a where rn = 1  ";

        string sqlputaway_strategic_step2_dm_damage = @"select top 1 u.lscode from wm_locup u, wm_locdw d 
        where u.orgcode = d.orgcode and u.site = d.site and u.depot = d.depot and u.spcarea = d.spcarea and u.lscode = d.lscode 
        and d.spcarea = 'DM' and d.fltype = 'BL' and lsloctype = 'AD' and d.tflow in ('IO','IX') and lsmxhuno > isnull(crhu,0) + 1  
        order by (case when d.spcthcode = @thcode  then 1 else 0 end + case when d.spcarticle = @article then 1 else 0 end ) desc, lsseq asc";
        string sqlputaway_strategic_step2_wh_warehouse = @"select top 1 u.lscode from wm_locup u, wm_locdw d 
        where u.orgcode = d.orgcode and u.site = d.site and u.depot = d.depot and u.spcarea = d.spcarea and u.lscode = d.lscode 
        and d.spcarea = 'RN' and d.fltype = 'BL' and lsloctype = 'WH' and d.tflow in ('IO','IX') and lsmxhuno > isnull(crhu,0) + 1  
        order by (case when d.spcthcode = @thcode  then 1 else 0 end + case when d.spcarticle = @article then 1 else 0 end ) desc, lsseq asc";
        string sqlputaway_strategic_step2_vd_vendor = @"select top 1 u.lscode from wm_locup u, wm_locdw d 
        where u.orgcode = d.orgcode and u.site = d.site and u.depot = d.depot and u.spcarea = d.spcarea and u.lscode = d.lscode 
        and d.spcarea = 'RN' and d.fltype = 'BL' and lsloctype = 'WH' and d.tflow in ('IO','IX') and lsmxhuno > isnull(crhu,0) + 1  
        order by (case when d.spcthcode = @thcode  then 1 else 0 end + case when d.spcarticle = @article then 1 else 0 end ) desc, lsseq asc";
        string sqlputaway_strategic_step2_fw_forward =  @"select top 1 u.lscode from wm_locup u, wm_locdw d 
        where u.orgcode = d.orgcode and u.site = d.site and u.depot = d.depot and u.spcarea = d.spcarea and u.lscode = d.lscode 
        and d.spcarea = 'DM' and d.fltype = 'BL' and lsloctype = 'fi' and d.tflow in ('IO','IX') and lsmxhuno > isnull(crhu,0) + 1   
        order by (case when d.spcthcode = @thcode  then 1 else 0 end + case when d.spcarticle = @article then 1 else 0 end ) desc, lsseq asc";


        string sqlputaway_default_bulk = " select top 1 lscode from wm_locdw where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'BL' and spcarea = 'BL' and lsloctype = 'BL' ";

        private string sqltransfer_reqblock = @"select b.bnflex from wm_locdw l, ( select orgcode, site, depot, bnvalue,isnull(bnflex1,0) bnflex from wm_binary 
        where bntype = 'LOCATION' and bncode = 'AREA' ) b where l.orgcode = b.orgcode and l.site = b.site and l.depot = b.depot and l.spcarea = b.bnvalue 
        and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.lscode = @lscode";

        private string sqlstock_status =
            @" select top 1 tflow , stockid from wm_stock where orgcode=@orgcode and site = @site and depot = @depot and huno= @huno";

        string sqlstock_tfowactive = "update wm_stock set tflow='IO', datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, " +
        " procmodify = 'task.transfer' where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno ";

        //Send blocking stock to orbit
        public string sqlstock_block_step2 = @"insert into xm_xoblock (orgcode ,site,depot,spcarea,stockid,hutype,huno,hunosource,thcode,
            inrefno,inrefln,loccode,article,pv,lv,qtysku,qtypu,qtyweight,qtyvolume,daterec,batchno,
            lotno,datemfg,dateexp,serialno,stkremarks,tflow,opstype,xaction,xcreate, xmodify, xmsg,rowid,accnmodify)
            select orgcode ,site,depot,spcarea,stockid,hutype,huno,hunosource,thcode,inrefno,inrefln
            ,loccode,article,pv,lv,qtysku,qtypu,qtyweight,qtyvolume,daterec,batchno,lotno,datemfg
            ,dateexp,serialno,stkremarks,tflow,@opstype,'WC' xaction,sysdatetimeoffset() xcreate
            ,null xmodify,null xmsg,next value for seq_oxblock rowid,@accnmodify accnmodify from wm_stock where orgcode = @orgcode and site = @site 
            and depot = @depot and stockid = @stockid and article = @article and pv = @pv and lv = @lv ";
    }
}