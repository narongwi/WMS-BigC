using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.Logger;

namespace Snaps.WMS.preparation { 
    public partial class prep_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        private string cnx = "";
        public prep_ops() { }
        public prep_ops(String cx) {
            cn = new SqlConnection(cx);
            cnx = cx;
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<prep_ops>();
        }

        public static string prep_tbn = "wm_prep";
        public static string prep_force = " and orgcode = @orgcode and site = @site  and depot = @depot  and spcarea = @spcarea  and prepno = @prepno " ;
        public static string partial_shipsql =
           @"select (case when sum(isnull(qtyskuorder,0)) > sum(isnull(p.cravailable,0)) then 0 else 1 end) from wm_prepsln s , wm_productstate p where s.setno =@setno
            and s.orgcode = @orgcode and s.site  = @site and s.depot = @depot and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article 
            and s.pv = p.pv and s.lv = p.lv ";
        //public static string sqlcompare_stock = 
        //    @"select (case when sum(isnull(qtyskuorder,0)) > sum(isnull(p.cravailable,0)) then 0 else 1 end) from wm_prepsln s , wm_productstate p where s.setno =@setno
        //    and s.orgcode = @orgcode and s.site  = @site and s.depot = @depot and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article 
        //    and s.pv = p.pv and s.lv = p.lv ";
       
        public static string sqlcompare_stock = @"select top 1 case when s.tflow ='XX' then 3 when s.przone ='RTV' and sum(isnull(qtyskuorder, 0)) > sum(isnull(p.crrtv, 0)) then 2 
        when  sum(isnull(qtyskuorder, 0)) > sum(isnull(p.cravailable, 0))  then 1 else 0 end currect
        from wm_prepsln s ,wm_productstate p where s.setno = @setno  and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.orgcode = p.orgcode
        and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
        group by s.przone,s.tflow order by currect desc";

        private String prep_sqlins = " insert into wm_prep (orgcode,site,depot,spcarea,routeno,huno,preptype,prepno,prepdate,priority, " + 
        " preppct,thcode,spcorder,spcarticle,dateassign,datestart,dateend,tflow,deviceID, " + 
        " picker,datecreate,accncreate,datemodify,accnmodify,procmodify,hutype,przone,setno ) values " + 
        " ( @orgcode, @site,@depot,@spcarea,@routeno,@huno,@preptype,@prepno,sysdatetimeoffset(),@priority, " +
        " @preppct,@thcode,@spcorder,@spcarticle,null,null,null,@tflow, " +
        " @deviceID,@picker,@sysdate,@accncreate,@sysdate,@accnmodify,@procmodify,@hutype,@przone,@setno )"; 
        
        private String prep_sqlupd = " update wm_prep set routeno = @routeno,huno = @huno,preptype = @preptype,prepdate = @prepdate,priority = @priority, " + 
            " preppct = @preppct,thcode = @thcode,spcorder = @spcorder,spcarticle = @spcarticle,dateassign = @dateassign, " + 
            " datemodify = @sysdate,accnmodify = @accnmodify,procmodify = @procmodify "+ 
            " where 1=1 and orgcode = @orgcode and site = @site  and depot = @depot  and spcarea = @spcarea  and prepno = @prepno ";
        
        private String prep_sqlinx = string.Concat("insert into ix" , prep_force , 
            " ( orgcode, site, depot, spcarea, ouorder, outype, ousubtype, thcode, dateorder, dateprep, dateexpire, oupriority, ouflag, oupromo, ousource, dropship, " + 
            " stocode, stoname, stoaddressln1, stoaddressln2, stoaddressln3, stosubdistict, stodistrict, stocity, stocountry, stopostcode, stomobile, " + 
            " stoemail, fileid, rowops, tflow, ermsg, opsdate ) " + 
            " values " + 
            " ( @orgcode, @site, @depot, @spcarea, @ouorder, @outype, @ousubtype, @thcode, @dateorder, @dateprep, @dateexpire, @oupriority, @ouflag, @oupromo, @ousource, " + 
            " @dropship, @stocode, @stoname, @stoaddressln1, @stoaddressln2, @stoaddressln3, @stosubdistict, @stodistrict, @stocity, @stocountry, @stopostcode, " + 
            " @stomobile, @stoemail, @fileid, @rowops, @tflow, @ermsg, @opsdate )");        

        private String prep_sqlrem = string.Concat("delete from ",prep_tbn," where 1=1 ",prep_force);
        private String prep_sqlfnd = string.Concat("select o.*,p.thnameint thname from wm_outbound o,wm_thparty p where o.orgcode = p.orgcode and o.site = p.site " + 
        " and o.depot = p.depot and o.thcode = p.thcode and o.orgcode = @orgcode and o.site = @site  and o.depot = @depot ");
        private String prep_sqlvld = string.Concat("select count(1) rsl from ",prep_tbn," where 1=1 ",prep_force);

        private String prep_setpriority = "update wm_prep set pririty = 30, datemodify =@sysdate, accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and ouorder = @ouorder";
        
        //Generate plan 
        private String prep_genplan = "select  next value for seq_prep rsl";

        //Find prepparation
        private string sqlprep_find = "" + 
        " select a.orgcode,a.site, a.depot, a.spcarea, a.routeno, a.huno, a.preptype, a.prepno, a.prepdate,a.preppct, a.picker, a.priority,   " +
        " a.tflow, a.thcode,case when a.spcarea = 'XD' then a.thname else t.thnameint end thname, '' spcorder, '' spcarticle, 0 capacity,b.bndesc preptypename ,przone, setno " +
        " from ( select p.orgcode,p.site,p.depot,p.spcarea, routeno, huno, preptype, prepno ,prepdate,                                        " +
        "         preppct,picker, priority,p.tflow , p.thcode, t.thnameint thname ,p.przone,'' spcorder,'' spcarticle,p.setno                 " +
        "         from wm_prep p, wm_thparty t                                                                                                " +
        "         where p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot and p.thcode = t.thcode and p.spcarea = 'ST'          " +
        "           and p.orgcode = @orgcode and p.site = @site and p.depot = @depot {0} {1}                                                  " +
        "         union all                                                                                                                   " +
        "         select t.orgcode,t.site,t.depot,t.spcarea, t.routeno, l.sourcehuno,tasktype preptype,t.taskno,t.taskdate,0 preppct, null picker, " + 
        "		        priority,l.tflow,t.routethcode, '' thname,null przone,'' spcorder,'' spcarticle, t.setno                              " + 
        "           from wm_task t,wm_taln l                                                                                                  " + 
        "          where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.tflow = 'IO'           " + 
        "            and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and tasktype = 'A' {2} {3}                              " + 
        "         union all                                                                                                                   " + 
        "         select p.orgcode,p.site,p.depot,p.spcarea,spcorder routeno, huno, preptype, prepno ,prepdate,                               " + 
        "         preppct, picker ,p.priority,p.tflow, p.spcarticle thcode, t.descalt thname,p.przone, spcorder , spcarticle, p.setno                  " + 
        "          from wm_prep p, wm_product t                                                                                               " + 
        "         where p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot and p.spcarea = 'XD' and p.spcarticle = t.article     " + 
        "           and p.orgcode = @orgcode and p.site = @site and p.depot = @depot                                                          " + 
        "         union all                                                                                                                   " + 
        "         select p.orgcode,p.site,p.depot,p.spcarea,spcorder routeno, huno, preptype, prepno ,prepdate,                               " + 
        "         preppct, picker ,p.priority,p.tflow, p.thcode thcode,'' thname ,p.przone,'' spcorder,'' spcarticle  , p.setno               " + 
        "         from wm_prep p where p.preptype = 'XE'                                                                                      " + 
        " ) a join (select orgcode,site,depot,bnvalue,bndesc from wm_binary where bntype = 'PREP' and bncode = 'TYPE' and orgcode = @orgcode  " + 
        " and site = @site and depot = @depot and bnstate = 'IO') b                                                                           " + 
        " on a.orgcode = b.orgcode and a.site = b.site and a.depot = b.depot and a.preptype = b.bnvalue                                       " + 
        " left join wm_thparty t on a.orgcode = t.orgcode and a.site = t.site and a.depot = t.depot and a.thcode = t.thcode                   " + 
        " where a.orgcode = @orgcode and a.site = @site and a.depot = @depot                                                                  " ;

        //line 
        private String prln_sqlfnd =
        @" select o.orgcode,o.site,o.depot,o.spcarea,o.routeno,o.thcode,o.huno,o.hunosource,o.prepno,o.prepln,o.loczone,o.loccode,o.locseq,o.locdigit
                ,o.ouorder,o.ouln,o.barcode,o.article,o.pv,o.lv,o.stockid,o.unitprep,o.qtyskuorder,o.qtypuorder,o.qtyweightorder,o.qtyvolumeorder
                ,o.qtyskuops,o.qtypuops,o.qtyweightops,o.qtyvolumeops,o.batchno,o.lotno,o.datemfg,o.dateexp,o.serialno,o.picker,o.datepick
                ,o.devicecode,o.tflow,o.datecreate,o.accncreate,o.datemodify,o.accnmodify,o.procmodify
                ,p.descalt,p.unitprep,p.rtoskuofpu, max(t.taskno) taskno,null daterec,null inagrn, null ingrno,o.preptypeops,o.preplineops 
        from wm_prln o join wm_product p on o.orgcode = p.orgcode and o.site = p.site 
        and o.depot = p.depot and o.article = p.article and o.pv = p.pv and o.lv = p.lv 
        left join wm_taln t on o.orgcode = t.orgcode and o.site = t.site and o.depot = t.depot and o.stockid = t.stockid and o.loccode = t.targetadv and o.article = t.article and o.pv = t.pv and o.lv = t.lv and t.tflow not in ('ED','CL') 
        where  o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.spcarea = @spcarea and o.prepno = @prepno and o.spcarea = 'ST'
        group by o.orgcode,o.site,o.depot,o.spcarea,o.routeno,o.thcode,o.huno,o.hunosource,o.prepno,o.prepln,o.loczone,o.loccode,o.locseq,o.locdigit
                ,o.ouorder,o.ouln,o.barcode,o.article,o.pv,o.lv,o.stockid,o.unitprep,o.qtyskuorder,o.qtypuorder,o.qtyweightorder,o.qtyvolumeorder
                ,o.qtyskuops,o.qtypuops,o.qtyweightops,o.qtyvolumeops,o.batchno,o.lotno,o.datemfg,o.dateexp,o.serialno,o.picker,o.datepick
                ,o.devicecode,o.tflow,o.datecreate,o.accncreate,o.datemodify,o.accnmodify,o.procmodify,p.descalt,p.unitprep,p.rtoskuofpu,o.preptypeops,o.preplineops";
        private string prln_sqlfnd_dist =
        @" select   o.orgcode,o.site,o.depot,o.spcarea,o.routeno,o.thcode,o.huno,o.hunosource,o.prepno,o.prepln,o.loczone,o.loccode,o.locseq,o.locdigit
            ,o.ouorder,o.ouln,o.barcode,o.article,o.pv,o.lv,o.stockid,o.unitprep,o.qtyskuorder,o.qtypuorder,o.qtyweightorder,o.qtyvolumeorder
            ,o.qtyskuops,o.qtypuops,o.qtyweightops,o.qtyvolumeops,o.batchno,o.lotno,o.datemfg,o.dateexp,o.serialno,o.picker,o.datepick
            ,o.devicecode,o.tflow,o.datecreate,o.accncreate,o.datemodify,o.accnmodify,o.procmodify
            ,p.descalt,p.unitprep,p.rtoskuofpu, null taskno,s.daterec daterec,s.inagrn inagrn, s.inagrn ingrno,o.preptypeops,o.preplineops 
        from wm_prln o 
        join wm_product p on o.orgcode = p.orgcode and o.site = p.site and o.depot = p.depot and o.article = p.article and o.pv = p.pv and o.lv = p.lv 
        left join wm_stock s on o.orgcode = s.orgcode and o.site = s.site and o.depot = s.depot and o.article = s.article and p.pv = s.pv and o.lv = s.lv 
            and o.stockid = s.stockid and o.hunosource = s.huno
        where  o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.spcarea = 'XD' and o.prepno = @prepno and o.spcarea = 'XD'";

        // Bug decrease Stock double 09092021
        //private string prlin_sqlfnd_forclose = @"select o.*, s.daterec, s.inagrn, s.ingrno from ( 
        //select o.*, p.descalt,p.rtoskuofpu, t.taskno from wm_prln o join wm_product p on o.orgcode = p.orgcode and o.site = p.site 
        //and o.depot = p.depot and o.article = p.article and o.pv = p.pv and o.lv = p.lv left join wm_taln t on o.orgcode = t.orgcode
        //and o.site = t.site and o.depot = t.depot and o.loccode = t.targetadv and o.article = t.article and o.pv = t.pv and o.lv = t.lv and t.tflow not in ('ED','CL') 
        //where o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.spcarea = @spcarea and o.prepno = @prepno
        //) o, wm_stock s 
        //where  o.orgcode = s.orgcode and o.site = s.site and o.depot = s.depot and o.article = s.article and o.pv = s.pv and o.lv = s.lv and o.stockid = s.stockid";

        // Fix Bug decrease Stock double 09092021
        private string prlin_sqlfnd_forclose = 
           @"select
	       o.orgcode, o.site, o.depot, o.spcarea, o.routeno, o.thcode, o.huno, o.hunosource, o.prepno, o.prepln, o.loczone, o.loccode, o.locseq, o.locdigit, o.ouorder, o.ouln, o.barcode, 
	       o.article, o.pv, o.lv, o.stockid, o.unitprep, o.qtyskuorder, o.qtypuorder, o.qtyweightorder, o.qtyvolumeorder, o.qtyskuops, o.qtypuops, o.qtyweightops, o.qtyvolumeops, 
	       o.batchno, o.lotno, o.datemfg, o.dateexp, o.serialno, o.picker, o.datepick, o.devicecode, o.tflow, o.datecreate, o.accncreate, o.datemodify, o.accnmodify, o.procmodify, 
	       o.prepstockid,o.descalt,o.rtoskuofpu,o.taskno, s.daterec, s.inagrn, s.ingrno ,o.preptypeops,o.preplineops
            from ( 
	            select o.orgcode, o.site, o.depot, o.spcarea, o.routeno, o.thcode, o.huno, o.hunosource, o.prepno, o.prepln, o.loczone, o.loccode, o.locseq, o.locdigit, o.ouorder, o.ouln, o.barcode, 
	               o.article, o.pv, o.lv, o.stockid, o.unitprep, o.qtyskuorder, o.qtypuorder, o.qtyweightorder, o.qtyvolumeorder, o.qtyskuops, o.qtypuops, o.qtyweightops, o.qtyvolumeops, 
	               o.batchno, o.lotno, o.datemfg, o.dateexp, o.serialno, o.picker, o.datepick, o.devicecode, o.tflow, o.datecreate, o.accncreate, o.datemodify, o.accnmodify, o.procmodify, 
	               o.prepstockid, p.descalt,p.rtoskuofpu, max(t.taskno) taskno,o.preptypeops,o.preplineops
	            from wm_prln o join wm_product p on o.orgcode = p.orgcode and o.site = p.site and o.depot = p.depot and o.article = p.article and o.pv = p.pv and o.lv = p.lv 
	            left join wm_taln t on o.orgcode = t.orgcode and o.site = t.site and o.depot = t.depot and o.stockid = t.stockid and o.loccode = t.targetadv and o.article = t.article 	and o.pv = t.pv and o.lv = t.lv  and t.tflow not in ('ED','CL') 
	            where o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.spcarea = @spcarea and o.prepno = @prepno
	            group by  o.orgcode, o.site, o.depot, o.spcarea, o.routeno, o.thcode, o.huno, o.hunosource, o.prepno, o.prepln, o.loczone, o.loccode, o.locseq, o.locdigit, o.ouorder, o.ouln, o.barcode, 
	               o.article, o.pv, o.lv, o.stockid, o.unitprep, o.qtyskuorder, o.qtypuorder, o.qtyweightorder, o.qtyvolumeorder, o.qtyskuops, o.qtypuops, o.qtyweightops, o.qtyvolumeops, 
	               o.batchno, o.lotno, o.datemfg, o.dateexp, o.serialno, o.picker, o.datepick, o.devicecode, o.tflow, o.datecreate, o.accncreate, o.datemodify, o.accnmodify, o.procmodify, 
	               o.prepstockid, p.descalt,p.rtoskuofpu,o.preptypeops,o.preplineops
            ) o, wm_stock s 
            where  o.orgcode = s.orgcode and o.site = s.site and o.depot = s.depot and o.article = s.article and o.pv = s.pv and o.lv = s.lv and o.stockid = s.stockid";

        //line 
        private String prln_sqlfndA = @"select th.orgcode,th.site,th.depot,th.spcarea,tl.sourcehuno huno,tl.sourcehuno hunosource,
            th.taskno prepno, tl.taskseq prepln,null loczone,tl.sourceloc loccode,0 locseq,'' locdigit, ord.ouorder ouorder, 
            ord.ouln ouln, ord.barcode,tl.article,tl.pv,tl.lv,tl.stockid,p.descalt,p.unitprep, ord.qtysku qtyskuorder, ord.qtypu qtypuorder,
            ord.qtyweight qtyweightorder,0.0 qtyvolumeorder,ord.qtyreqsku qtyskuops, tl.targetqty qtypuops,
            ord.qtyweight qtyweightops,0.0 qtyvolumeops,ord.batchno batchno, ord.lotno lotno, ord.datemfg datemfg,
            ord.dateexp dateexp, ord.serialno serialno, tl.accnassign picker, tl.dateassign datepick,null devicecode,tl.tflow,
	        tl.datecreate,tl.accncreate,tl.datemodify,tl.accnmodify,tl.procmodify,p.description,p.rtoskuofpu,th.routethcode thcode, th.taskno, null daterec, null inagrn, null ingrno,
            null preptypeops,0 preplineops 
            from wm_taln tl, wm_task th, wm_outbouln ord, wm_outbound orh , wm_product p
            where tl.orgcode = @orgcode and tl.site =@site and tl.depot = @depot and tl.spcarea = @spcarea and tl.taskno = @prepno
            and tl.orgcode = th.orgcode and tl.site = th.site and tl.depot = th.depot and tl.taskno = th.taskno
            and tl.orgcode = ord.orgcode and tl.site = ord.site and tl.depot = ord.depot and tl.iorefno = ord.ouorder
            and tl.article = ord.article and tl.pv = ord.pv and tl.lv = ord.lv
            and ord.orgcode = orh.orgcode and ord.site = orh.site and ord.depot = orh.depot and ord.ouorder = orh.ouorder
            and tl.orgcode = p.orgcode and tl.site = p.site and tl.depot = tl.depot and tl.article = p.article and tl.pv = p.pv and tl.lv = p.lv ";

        private String prln_sqlins = " insert into wm_prln " +
        " ( orgcode,site,depot,spcarea,huno,hunosource,prepno,prepln,loczone,loccode,locseq,locdigit, " + 
        " ouorder,ouln,barcode,article,pv,lv,stockid,unitprep,qtyskuorder,qtypuorder,qtyweightorder, " + 
        " qtyvolumeorder,qtyskuops,qtypuops,qtyweightops,qtyvolumeops,batchno,lotno,datemfg,dateexp, " + 
        " serialno,picker,datepick,devicecode,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify, thcode ) " + 
        " values ( " + 
        " @orgcode,@site,@depot,@spcarea,@huno,@hunosource,@prepno,@prepln,@loczone,@loccode,@locseq,@locdigit, " + 
        " @ouorder,@ouln,@barcode,@article,@pv,@lv,@stockid,@unitprep,@qtyskuorder,@qtypuorder,@qtyweightorder, " + 
        " @qtyvolumeorder,@qtyskuops,@qtypuops,@qtyweightops,@qtyvolumeops,@batchno,@lotno,@datemfg,@dateexp, " + 
        " @serialno,@picker,@datepick,@devicecode,'IO',@sysdate,@accncreate,@sysdate,@accnmodify,@procmodify, @thcode ) ";
 

        private string prbc_sqlins = "INSERT INTO wm_stobc (orgcode,site,depot,spcarea,stockid,hutype,huno,inorder,inln,ouorder,ouln,opstype,opsno,opsln,article " + 
        " ,pv,lv,rsvsku,rsvpu,qtysku,qtypu,batchno,lotno,datemfg,dateexp,serialno,tflow,datecreate,accncreate,procmodify,loccode, hunosource) " + 
        " values " + 
        " (@orgcode,@site,@depot,@spcarea,@stockid,@hutype,@huno,@inorder,@inln,@ouorder,@ouln,@opstype,@opsno,@opsln " +
        " ,@article,@pv,@lv,@rsvsku,@rsvpu,0,0,@batchno,@lotno,@datemfg,@dateexp,@serialno,@tflow,@datecreate,@accncreate,@procmodify, @loccode, @hunosource)";
        private string prsrp_sqlhead = "UPDATE dbo.wm_prepsrp SET tflow ='ED',tflowdate = SYSDATETIMEOFFSET() WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND setno = @setno and tflow='IO'";
        private string prsrp_sqlline = "UPDATE dbo.wm_prepsrl SET tflow ='ED',tflowdate = SYSDATETIMEOFFSET() WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND setno = @setno and tflow='IO'";
        public String prep_sqlupdprc = " update wm_outbound set tflow = 'PC' where orgcode = @orgcode and site = @site and depot = @depot and ouorder in ({0}) ";
        public String prep_getpickstock = "select top 1 s.orgcode, s.site, s.depot, stockid, huno, article, pv, lv, qtysku, qtypu,s.loccode, dateexp,datemfg, batchno, lotno " + 
        "  from wm_stock s, wm_locdw l where s.orgcode = l.orgcode " +
        " and s.site = l.site and s.depot = l.depot and s.loccode = l.lscodealt and s.tflow IN ('IO','XO') and l.tflow in ('IO','XO') and s.spcarea = l.spcarea " + 
        " and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv " + 
        " and spcpicking = 1 order by case when spcpickunit = '1' then '1' else 0 end " ;
        public String prep_getreservestock = "select stockid,huno, article, pv, lv, qtysku, qtypu,lscode  from wm_stock s, wm_locdw l where s.orgcode = l.orgcode " +
        " and s.site = l.site and s.depot = l.depot and s.loccode = l.lscodealt and s.tflow IN ('IO','XO') and l.tflow in ('IO','XO') and s.spcarea = l.spcarea " + 
        " and o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.article = @article and o.pv = @pv and o.lv = @lv " +
        " and spcpicking = 1 order by case when spcpickunit = '1' then '1' else 0 end " ;

        public String prep_opspriority = "update wm_prep set  priority = case when priority = 30 then 0 else 30 end, accnmodify = @accnmodify, datemodify = @sysdate, procmodify = @procmodify " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno ";

        //Start prep
        public String prep_opsstart = "update wm_prep set dateassign = @sysdate, datestart = @sysdate, picker = @picker, " + 
        " accnmodify = @accnmodify, datemodify = @sysdate, procmodify = @procmodify, tflow = 'PA' " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno ";

        //confirm prep
        public string prep_opsend_step0 = @"update top(1) wm_prln set prepstockid = @prepstockid where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno and hunosource = @hunosource
        and prepno = @prepno and article = @article and pv = @pv and lv = @lv and stockid = @stockid ";
        public String prep_opsend_step1 = "update wm_prep set dateend = @sysdate, accnmodify = @accnmodify, datemodify = @sysdate, procmodify = @procmodify,tflow = 'PE' " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno ";
        public string prep_opsend_step2 = "update wm_prln set tflow = 'PE', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = @procmodify " +
        " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno and tflow = 'IO'";
        public String prep_opsend_step3 = " update t set crsku = p.qtyskuops, crpu = p.qtypuops, crweight = p.qtyweightops, crvolume = p.qtyvolumeops , tflow = 'PE', datemodify = sysdatetimeoffset(), " + 
        " accnmodify = @accnmodify, procmodify = 'prepstock.end', crcapacity = 100 - ((p.qtyvolumeops / mxvolume) * 100) " + 
        " from wm_handerlingunit t, (select orgcode,site,depot, huno, sum(p.qtyskuops) qtyskuops, sum(p.qtypuops) qtypuops, sum(p.qtyweightops) qtyweightops,sum(p.qtyvolumeops) qtyvolumeops  " + 
        " from wm_prln p where orgcode = @orgcode and depot = @depot and site = @site and prepno = @prepno group by orgcode,site,depot, huno ) p " + 
        " where t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.huno = p.huno and t.huno = p.huno and t.tflow = 'IO' " + 
        " and t.orgcode = @orgcode and t.site = @site and t.depot = @depot " ;

        public string prep_opsend_step4 = " update wm_stobc set tflow = 'PE', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = @procmodify "+ 
        " where orgcode = @orgcode and site = @site and depot = @depot and opstype = 'P' and @prepno = @prepno";
        public string prep_opsend_step5 = " update t set t.qtyprep = x.qtyprep, datemodify = sysdatetimeoffset(), accnmodify = @accnmodify " +
        "  from wm_stock t, " +
        "     ( select orgcode, site, depot, stockid, article, pv, lv from wm_stobc b where orgcode = @orgcode " +
        "       and site = @site and depot = @depot and opsno = @prepno and opstype = 'P' ) b, " +
        "     ( select orgcode, site, depot, stockid, article, pv, lv, sum(rsvsku - isnull(qtysku,0)) qtyprep " +
        "      from wm_stobc b where orgcode = @orgcode and site = @site and depot = @depot group by orgcode, site, depot, stockid, article, pv, lv ) x " +
        "  where t.orgcode = b.orgcode and t.site = b.site and t.depot = b.depot and t.stockid = b.stockid and t.article  = b.article and t.pv = b.pv and t.lv = b.lv " +
        "  and b.orgcode = x.orgcode and b.site = x.site and b.depot = x.depot and b.stockid = x.stockid and b.article = x.article and b.pv = x.pv and b.lv = x.lv ";
        public string prep_opsend_step6 = 
        @"update t set t.crweight = isnull(s.crweight,0), t.crvolume = isnull(s.crvolume,0), t.crophu = isnull(s.crophu,0)
        from wm_route t left join (
            select orgcode, site, depot, routeno, sum(crweight) crweight,  sum(crvolume) crvolume, count(distinct huno) crophu
            from wm_handerlingunit where orgcode = @orgcode and site = @site and depot= @depot and routeno = @routeno and tflow != 'ED'
            group by  orgcode, site, depot, routeno) s 
        on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.routeno = s.routeno 
        where t.orgcode = @orgcode and t.site =  @site and t.depot= @depot and t.routeno = @routeno";


        
        //cancel prep 
        public String prep_opscancel_stp1 = "update wm_prep set accnmodify = @accnmodify, datemodify = @sysdate, procmodify = @procmodify,tflow = 'CL' " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno ";
        public String prep_opscancel_stp2 = "update wm_stobc set qtysku = 0, qtypu = 0, tflow = 'CL',accnmodify = @accnmodify, datemodify = @sysdate, procmodify = @procmodify " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and opsno = @prepno ";

        //Picking process
        public string prep_opspick_vld =" select case when p.loccode != s.loccode and t.taskno is not null then 'Waiting Task RPN no '+t.taskno "+
        "             when p.loccode = s.loccode and p.qtyskuorder > s.qtysku then 'Stock not enought'  "+
        "             when l.tflow = 'IX' then 'Location has block'  when @skipdigit != 'skip' and l.lsdigit != @lsdigit then 'Verify digit incorrect' " +
        "             else 'PASS' end   "+
        "   from wm_prln p join wm_stock s on p.orgcode = s.orgcode and p.site = s.site and p.depot = s.depot and p.stockid = s.stockid and p.article = s.article and p.pv = s.pv and p.lv = s.lv  "+
        "  left join wm_taln t on p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot and p.loccode = t.targetadv and p.article = t.article and p.pv = t.pv and p.lv = t.lv and t.tflow not in ('ED','CL')  "+
        "  left join wm_locdw l on p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.loccode = l.lscode "+
        "  where p.prepno = @prepno and p.prepln = @prepln and  p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.article = @article and p.pv = @pv and p.lv = @lv  ";
        public String prep_opspick_stp1 =" update t  set " + 
        " qtyskuops = case when t.unitprep = 1 then 1 when t.unitprep = 2 then rtoskuofipck when t.unitprep = 3 then rtoskuofpck when t.unitprep = 4 then rtoskuoflayer when t.unitprep = 5 then rtoskuofhu else 1 end *   @qtyskuops, " + 
        " qtypuops = @qtypuops,  datemodify = sysdatetimeoffset(), procmodify = @procmodify, serialno = @serialno,  qtyweightops = round(p.skuweight * @qtyskuops,3,1), qtyvolumeops = round(p.skuvolume * @qtyskuops,3,1), " +
        " datepick = sysdatetimeoffset(), picker = @accnmodify, accnmodify = @accnmodify , preplineops = isnull(t.preplineops) + 1" + 
        " from wm_prln t , wm_product p " + 
        " where t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.article = p.article and t.pv = p.pv and t.lv = p.lv " + 
        " and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.prepno = @prepno and t.prepln = @prepln and t.article = @article and t.pv = @pv and t.lv = @lv " ;
        public String prep_opspick_stp2 =" update wm_stobc set qtysku = @qtyskuops, qtypu = @qtypuops, accnmodify = @accnmodify, datemodify = @sysdate, procmodify = @procmodify " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and opsno = @prepno and opsln = @prepln and article = @article and pv = @pv and lv = @lv ";
        public String prep_opspick_stp3 = "update wm_prep set preppct = isnull((select (sum(qtypuops) * 100)/ sum(case when qtypuorder = 0 then 1 else qtypuorder end) from wm_prln l where " +
        " wm_prep.orgcode = l.orgcode and wm_prep.site = l.site and wm_prep.depot = l.depot and wm_prep.prepno = l.prepno),0) " +
        " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno";
        //Puting process 
        // public string prep_opsput_stp1 = " update wm_prln set qtyskuops = @qtyskuops, qtypuops = @qtypuops, datemodify = @sysdate, procmodify = @procmodify, serialno = @serialno, " +
        // " qtyweightops = round(qtyweightorder * @qtyskuops,3,1), qtyvolumeops = round(qtyvolumeorder * @qtyskuops,3,1), huno = @huno " +
        // " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno and prepln = @prepln and article = @article and pv = @pv and lv = @lv";//wm_prln
        public string prep_opsput_stp1 = @"update l set qtyskuops = @qtypuops * dbo.get_ratiopu_prep(l.orgcode,l.site, l.depot, l.article, l.pv, l.lv), qtypuops  = @qtypuops, 
                    qtyweightops = round(p.skuweight * ( @qtypuops * dbo.get_ratiopu_prep(l.orgcode,l.site, l.depot, l.article, l.pv, l.lv) ),3,1),
                    qtyvolumeops = round(p.skuvolume * ( @qtypuops * dbo.get_ratiopu_prep(l.orgcode,l.site, l.depot, l.article, l.pv, l.lv) ),3,1),             
                    procmodify = @procmodify, serialno = @serialno, huno = @huno, datemodify = sysdatetimeoffset(), preplineops = isnull(l.preplineops,0) + 1
        from wm_prln l, wm_product p
        where l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
        and l.orgcode = @orgcode  and l.site = @site  and l.depot = @depot  and l.prepno = @prepno and l.prepln = @prepln and l.article = @article and l.pv = @pv and l.lv = @lv";
        public String prep_opsput_stp2 = " update wm_stobc set qtysku = @qtypuops * dbo.get_ratiopu_prep(orgcode,site, depot,article, pv, lv), qtypu = @qtypuops, accnmodify = @accnmodify, datemodify = @sysdate, procmodify = @procmodify " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and opsno = @prepno and opsln = @prepln and article = @article and pv = @pv and lv = @lv ";//wm_stobc
        public String prep_opsput_stp3 = "update wm_prep set preppct = isnull((select (sum(qtypuops) * 100)/ sum(qtypuorder) from wm_prln l where " +
        " wm_prep.orgcode = l.orgcode and wm_prep.site = l.site and wm_prep.depot = l.depot and wm_prep.prepno = l.prepno),0) " +
        " where orgcode = @orgcode and site = @site and depot = @depot and prepno = @prepno"; //wm_prep
        public string prep_opsput_stp4 =  "" +
        @"update h set crsku = qtysku, crvolume = qtyvolume, crweight = qtyweight, opstype = 'P', opscode = prepno, crcapacity = round( ( qtyweight / mxweight ) * 100,2,1)
        from wm_handerlingunit h , 
        ( select p.orgcode, p.site, p.depot, p.huno, p.prepno, sum(qtyskuorder) qtysku, sum(qtyweightorder) qtyweight, sum(qtyvolumeorder) qtyvolume 
            from wm_prep p ,wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno 
            group by p.orgcode, p.site, p.depot, p.huno, p.prepno ) p 
        where h.orgcode = p.orgcode and h.site = p.site and h.depot = p.depot and h.huno = p.huno
            and h.orgcode = @orgcode and h.site = @site and h.depot = @depot ";
        // public String prep_opsput_stp4 = " update wm_handerlingunit set  " + 
        // " 		crsku = isnull(( select sum(qtyskuops) from wm_prln where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno ),0), " + 
        // "		crweight = isnull(( select sum(qtyweightops) from wm_prln where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno ),0),  " +
        // "		crvolume = isnull(( select sum(qtyvolumeops) from wm_prln where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno ),0), " +
        // "		crcapacity = case when isnull(( select sum(qtyvolumeops) from wm_prln where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno ),0) = 0 then 0 else  " +
        // "                    round(isnull( ( cast( ( select sum(qtyvolumeops) from wm_prln where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno ) as decimal) * 100) / cast(mxvolume as decimal) ,0),2,1) end,  " +
        // "		datemodify = sysdatetimeoffset() ,accnmodify = @accnmodify, procmodify = @procmodify " +
        // " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno ";//wm_huno


        
        //Preparation Set 
        private string sqlprepset_line = " select *,'' descalt, p.skuweight, p.skuvolume from wm_prepsln l, wm_product p where l.tflow = 'WC' and l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot " +
        " and l.article = p.article and l.pv = p.pv and l.lv = p.lv and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.setno = @setno and p.tflow ='IO'";
        private string sqlprepset_lineed = "select *,p.descalt descalt from wm_prepsln o,wm_product p where tflow = 'ED' and o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.setno = @setno " + 
        " and o.orgcode = p.orgcode and o.site = p.site and o.depot = p.depot and o.article = o.article and o.pv = l.pv and o.lv = l.lv ";
        private string sqlprepset_hucode = " select hutype from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and przone = @przone ";
        private string sqlprepset_start = " update wm_prepset set datestart = sysdatetimeoffset(),tflow = 'PC' where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and setno = @setno and tflow = 'IO' ";
        // private string sqlprepset_finish_step1 = " update wm_prepset set datefinish = sysdatetimeoffset(),tflow = 'ED', " +
        //  " opsperform =  CAST(CONVERT(varchar(12), DATEADD(MILLISECOND, DATEDIFF(MILLISECOND, datestart, sysdatetimeoffset()), 0), 114) AS time(7) ) " + 
        //  " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and setno = @setno and tflow = 'PC' ";
        private string sqlprepset_finish_step1 = @"update top(1) t set t.prepsku = isnull(qtyskuprep,0), t.preppu = isnull(qtypuprep,0), t.tasksku = isnull(qtyskutask,0), t.taskqty = 0,
            datefinish = sysdatetimeoffset(),tflow = 'ED', opsperform =  CAST(CONVERT(varchar(12), DATEADD(MILLISECOND, DATEDIFF(MILLISECOND, datestart, sysdatetimeoffset()), 0), 114) AS time(7) )
        from wm_prepset t left join 
        (  select p.orgcode, p.site, p.depot,p.setno, sum(l.qtyskuorder) qtyskuprep, sum(l.qtypuorder) qtypuprep
        from wm_prep p, wm_prln l where p.prepno = l.prepno and p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot
        and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.setno = @setno
        group by p.orgcode, p.site, p.depot,p.setno ) p
        on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.setno = p.setno
        left join 
        ( select  t.orgcode, t.site, t.depot,t.setno, sum(l.sourceqty) qtyskutask
        from wm_task t, wm_taln l 
        where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype = 'A' and t.tflow = 'IO' and t.tflow = 'IO' 
            and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.setno = @setno
        group by t.orgcode, t.site, t.depot,t.setno ) m
        on t.orgcode = m.orgcode and t.site = m.site and t.depot = m.depot and t.setno = m.setno
        where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.spcarea = @spcarea and t.setno = @setno and t.tflow = 'PC' ";
        private string sqlprepset_finish_step2 = " update wm_prepsln set tflow = 'ED' where orgcode = @orgcode and site = @site and depot = @depot and setno = @setno and tflow = 'WC' ";
        private string sqlprepset_finish_step2_1 = @" update p set tflow ='ED', errmsg = 'Full pallet has generated'
        from wm_prepsln p ,wm_taln t where t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot 
        and t.ouorder = p.ouorder and t.ouln = p.ouln and t.article = p. article and t.tflow = 'IO' 
        and p.setno = @setno and p.orgcode = @orgcode and p.site = @site and p.depot = @depot  and p.tflow = 'XX' ";

        private string sqlprepset_finish_step3 = @" update h set crsku = qtysku, crvolume = qtyvolume, crweight = qtyweight, opstype = 'P', opscode = prepno
        from wm_handerlingunit h , 
        ( select p.orgcode, p.site, p.depot, p.huno, p.prepno, sum(qtyskuorder) qtysku, sum(qtyweightorder) qtyweight, sum(qtyvolumeorder) qtyvolume 
            from wm_prep p ,wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno 
            and setno = @setno group by p.orgcode, p.site, p.depot, p.huno, p.prepno ) p 
        where h.orgcode = p.orgcode and h.site = p.site and h.depot = p.depot and h.huno = p.huno";

        private string sqlprepset_finis_step4_cleahu = "delete top(1) from wm_handerlingunit where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno";

        // " update w set opscode = prepno , opstype = 'P' " + 
        // " from wm_handerlingunit w , " + 
        // "      (select orgcode,site,depot, huno,prepno from wm_prep p where orgcode = @orgcode and site = @site " + 
        // "      and depot = @depot and p.tflow = 'IO' and setno = @setno ) p " + 
        // " where w.orgcode = p.orgcode and w.site = p.site and w.depot = p.depot and w.huno = p.huno " ;
        
        // private string sqlprepset_insert = "insert into wm_prepset ( orgcode,site,depot,spcarea,setno,datestart,datefinish,opsorder,datecreate,accncreate,procmodify,tflow ) " + 
        // "values ( @orgcode,@site,@depot,@spcarea,@setno,@datestart,@datefinish,@opsorder,sysdatetimeoffset(),@accncreate,@procmodify,'IO')";
        private string sqlprepset_insert = @" 
        insert into wm_prepset ( orgcode,site,depot,spcarea,setno,datestart,datefinish,opsorder,datecreate,accncreate,procmodify,tflow,ordersku,orderpu )
        select @orgcode,@site,@depot,@spcarea,@setno,@datestart,@datefinish,@opsorder,sysdatetimeoffset() datecreate,@accncreate,@procmodify,'IO',
               sum(qtyreqsku) qtyreqsku, cast(sum(qtyreqpu) as decimal(13,3)) qtyreqku
          from wm_outbouln where orgcode = @orgcode and site = @site and depot = @depot and ouorder in ({0})
           and qtysku >= isnull(qtyskudel,0) + isnull(qtypndsku,0)";

        private string sqlprepset_insert_dist = @" 
        insert into wm_prepset ( orgcode,site,depot,spcarea,setno,datestart,datefinish,opsorder,datecreate,accncreate,procmodify,tflow,ordersku,orderpu )
        select @orgcode,@site,@depot,@spcarea,@setno,@datestart,@datefinish,@opsorder,sysdatetimeoffset() datecreate,@accncreate,@procmodify,'IO',
               sum(qtyreqsku) qtyreqsku, cast(sum(qtyreqpu) as decimal(13,3)) qtyreqku
          from wm_outbouln where orgcode = @orgcode and site = @site and depot = @depot and inorder in ('{0}')
           and qtysku >= isnull(qtyskudel,0) + isnull(qtypndsku,0)";
        private string sqlprlnset_insert = "" +
        " insert into wm_prepsln                                                                                                                        " +
        " ( orgcode,site,depot,setno,spcarea,routeno,thcode,ouorder,ouln,przone,barcode,article,pv,lv,unitprep,qtyskuorder,                             " +
        "   qtypuorder,qtyweightorder,qtyvolumeorder,qtyskuops,qtypuops,qtyweightops,qtyvolumeops,batchno,lotno,datemfg,                                " +
        "   dateexp,serialno,tflow,errmsg,loccode )                                                                                                     " +
        " select  o.orgcode, o.site, o.depot,@setno, o.spcarea, '' routeno, b.thcode, o.ouorder, ouln,                                                  " + 
        "         case when b.ousubtype = 'DV' then 'RTV' else przone end przone , o.barcode barcode,                                                   " +
        "         o.article, o.pv, o.lv,o.unitops unitprep, o.qtyreqsku qtyskuorder,                                                                    " +
        "         o.qtyreqsku / case when o.unitops = 1 then 1 when o.unitops = 2 then p.rtoskuofipck when o.unitops = 3 then p.rtoskuofpck             " +
        "                            when o.unitops = 4 then p.rtoskuoflayer when o.unitops = 5 then p.rtoskuofhu else 1 end qtypuorder,                " +
        "         o.qtyreqsku * p.skuweight qtyweightorder,o.qtyreqsku * p.skuvolume qtyvolumeorder,                                                    " +
        "         0 qtyskuops,0 qtypuops, 0 qtyweightops, 0 qtyvolumnops, o.batchno, o.lotno, o.datemfg, o.dateexp, o.serialno,                         " +
        "         case when z.przone is null and b.ousubtype != 'DV' then 'XX' when p.ismeasurement = 1 then 'XX' when p.tflow !='IO' then 'XX' else 'WC' end tflow,     " +
        "         case when z.przone is null and b.ousubtype != 'DV' then 'Preparation zone does not setup'                                             " +
        "              when p.ismeasurement = 1 then 'Product still require to measurement'                                                             " +
        "              when p.tflow !='IO' then 'Product has block for outbound operate'                                                        " +
        "              else '' end errmsg, z.lscode                                                                                                     " +
        "  from wm_outbound b join wm_outbouln o                                                                                                        " +
        "    on b.orgcode = o.orgcode and b.site = o.site and b.depot = o.depot and b.ouorder = o.ouorder and b.spcarea = 'ST' and b.spcarea = o.spcarea" +
        "  left join wm_product p                                                                                                                       " +
        "    on o.orgcode = p.orgcode and o.site = p.site and o.depot = p.depot and o.article = p.article and o.pv = p.pv and o.lv = o.lv               " +
        //"  left join wm_loczp z on spcprepzone = przone and p.article = z.spcproduct and p.pv = z.spcpv and p.lv = z.spclv                            " +
        " CROSS apply get_pzone(b.ousubtype,o.orgcode,o.site ,o.depot ,o.article ,o.pv ,o.lv ,o.unitops) z " +
        " where b.orgcode = @orgcode and b.site = @site and b.depot = @depot and b.spcarea = 'ST' and qtypndsku > 0 and qtyreqsku > 0 and b.tflow not in('CL','ED') and o.tflow not in('CL','ED')  ";

        private string sqlprlnset_insert_dist =
        @"  insert into wm_prepsln (orgcode, site, depot, setno,spcarea,thcode,przone,article,pv,lv,unitprep, qtyskuops, qtypuops, 
        qtyweightops, qtyvolumeops, batchno,lotno,datemfg, dateexp,serialno, tflow, dishuno,disstockid, loccode,  disinorder ) 
        select  s.orgcode, s.site, s.depot, @setno,'XD' spcarea,s.thcode,p.spcdistzone przone,s.article,s.pv,s.lv,p.unitprep,
                s.qtysku qtyskuops, s.qtypu qtypuops, s.qtyweight qtyweightops, s.qtyvolume qtyvolumeops, batchno,lotno,datemfg, dateexp,serialno, 'WC' tflow,huno dishuno,s.stockid,
                loccode, s.inrefno disinorder
        from wm_stock s, wm_product p
        where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
            and s.tflow = 'IO' and p.tflow = 'IO' 
            and not exists (select 1 from wm_prep o where s.orgcode = o.orgcode and s.site = o.site and s.depot = o.depot and s.huno = o.huno and o.tflow != 'CL')
            and s.huno = @huno and s.orgcode = @orgcode and s.site = @site and s.depot = @depot ";

        //  private string sqlprlnset_insert_dist = "" +
        // @"  insert into wm_prepsln 
        // ( orgcode,site,depot,setno,spcarea,routeno,thcode,ouorder,ouln,przone,barcode,article,pv,lv,unitprep,qtyskuorder,  
        //     qtypuorder,qtyweightorder,qtyvolumeorder,qtyskuops,qtypuops,qtyweightops,qtyvolumeops,batchno,lotno,datemfg,  
        //     dateexp,serialno,tflow,errmsg,dishuno,disstockid,loccode,disinorder ) 
        // select  orgcode,site,depot,setno,spcarea,routeno,disthcode thcode,ouorder,ouln,przone,barcode,article,pv,lv,unitprep, 
        //         case when tflow = 'WC' then qtysku else 0 end qtyskuorder,  
        //         case when tflow = 'WC' then ceiling(qtysku / rtoskuofpu) else 0 end qtypuorder, 
        //         qtyweightorder,qtyvolumeorder,0 qtyskuops,0 qtypuops,0 qtyweightops,0 qtyvolumeops,batchno,lotno,datemfg,  
        //         dateexp,serialno,tflow,errmsg,dishuno,disstockid, lscode, @inorder 
        //     from ( 
        // select * ,case when lvsum > 0 then qtyskuorder - lvsum  else qtyskuorder end qtysku from ( 
        // select * , sum(qtyskuorder) over (partition by dishuno order by  lspath,disthcode asc ) - qtystock lvsum 
        //     from ( 
        // select o.orgcode, o.site, o.depot,@setno setno, o.spcarea, '' routeno, p.thcode thcode, o.inorder,o.ouln ouln,przone,o.barcode barcode, 
        //         o.article, o.pv, o.lv,o.unitops unitprep, 
        //         sum(o.qtyreqsku) qtyskuorder, 
        //         case when o.unitops = 1 then 1 when o.unitops = 2 then p.rtoskuofipck when o.unitops = 3 then p.rtoskuofpck 
        //                             when o.unitops = 4 then p.rtoskuoflayer when o.unitops = 5 then p.rtoskuofhu else 1 end rtoskuofpu, 
        //         sum(o.qtyreqsku * p.skuweight) qtyweightorder, 
        //         sum(o.qtyreqsku * p.skuvolume) qtyvolumeorder, 
        //         0 qtyskuops,0 qtypuops, 0 qtyweightops, 0 qtyvolumnops, s.batchno, s.lotno, cast(s.datemfg as date) datemfg,cast(s.dateexp as date) dateexp, s.serialno, 
        //         case when z.przone is null then 'XX' when p.ismeasurement = 1 then 'XX' when p.tflow in ('XX','IX') then 'XX' else 'WC' end tflow, 
        //         case when z.przone is null then 'Distribution zone does not setup' 
        //             when p.ismeasurement = 1 then 'Product still require to measurement' 
        //             when p.tflow in ('XX','IX') then 'Product has block for outbound operate' 
        //             else '' end errmsg, 
        //         s.huno dishuno, s.stockid disstockid, s.qtysku qtystock,isnull(lspath,99999) lspath, o.ouorder,o.disthcode, z.lscode 
        // from wm_outbound b join wm_outbouln o 
        //     on b.orgcode = o.orgcode and b.site = o.site and b.depot = o.depot and b.inorder = o.inorder and b.spcarea = 'XD' 
        //     and b.spcarea = o.spcarea and b.ouorder = o.ouorder  
        // join wm_stock s on o.orgcode = s.orgcode and o.site = s.site and o.depot = s.depot and o.inorder = s.inrefno and o.article = s.article and o.pv = s.pv and o.lv = s.lv and s.huno = @huno 
        // left join wm_product p  
        //     on o.orgcode = p.orgcode and o.site = p.site and o.depot = p.depot and o.article = p.article and o.pv = p.pv and o.lv = o.lv 
        // left join (select * from wm_loczp where spcarea = 'XD' and orgcode = @orgcode and site = @site and depot = @depot and tflow = 'IO' ) z 
        //     on spcdistzone = przone and b.thcode = z.spcthcode 
        // where b.orgcode = @orgcode and b.site = @site and b.depot = @depot and b.spcarea = 'XD' 
        //     and b.inorder = @inorder and o.article = @article and o.pv = @pv and o.lv = @lv 
        //     and s.huno = @huno  and o.qtyreqsku > 0 
        //     and not exists (select 1 from wm_prep p where p.orgcode = s.orgcode and p.site = s.site and p.depot = s.depot and p.huno = s.huno) 
        // group by o.orgcode, o.site, o.depot, o.spcarea, p.thcode , o.inorder,przone,o.barcode, s.serialno, 
        //         o.article, o.pv, o.lv,o.unitops,s.batchno, s.lotno,z.przone, p.ismeasurement,p.tflow,s.huno, s.stockid, s.qtysku,isnull(lspath,99999), o.ouorder,o.disthcode, 
        //             cast(s.datemfg as date),cast(s.dateexp as date),case when o.unitops = 1 then 1 when o.unitops = 2 then p.rtoskuofipck when o.unitops = 3 then p.rtoskuofpck  
        //                 when o.unitops = 4 then p.rtoskuoflayer when o.unitops = 5 then p.rtoskuofhu else 1 end, lscode,o.ouln 
        // ) a ) a where ( lvsum < qtyskuorder and tflow = 'WC') or (tflow = 'XX') ) a ";

        private string sqlprln_find = " select l.*,p.descalt,p.skuweight, p.skuvolume from wm_prepsln l, wm_product p where l.orgcode = p.orgcode and l.site = p.site and l.depot =  " + 
        " p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.spcarea = 'ST' and  setno = @setno";

        //Process order for stocking 
        private string sqlprepset_process_rpnonprocess = @"insert into wm_prepsrp (orgcode, site, depot, spcarea, stockid, article, pv, lv, loccode, sourceqty , datemfg, dateexp,batchno,lotno, serialno,daterec, rsvqty, setno, huno, tasktype, targetloc,tflow,tflowdate) 
        values (@orgcode, @site, @depot, @spcarea, @stockid, @article, @pv, @lv,  @loccode, @sourceqty , @datemfg, @dateexp, @batchno, @lotno, @serialno, @daterec,  @rsvqty,  @setno, @huno, @tasktype, @targetloc,'IO',SYSDATETIMEOFFSET())";
       
        //private string sqlprepset_process_rpnonblock = @" update wm_prepsrp set rsvqty = rsvqty + @rsvqty where tasktype = 'R' and setno = @setno and stockid = @stockid and huno = @huno and targetloc = @targetloc
        //and orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ";
        private string sqlprepset_process_rpnonblock = @"update wm_prepsrp set rsvqty = rsvqty + @rsvqty where tasktype = 'R' and stockid = @stockid and huno = @huno and targetloc = @targetloc
        and orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ";
        private string sqlprepset_process_rpnunblock = @"exec dbo.snaps_Unpnprocess @orgcode,@site,@depot,@stockid,@article,@pv,@lv,@setno,@ouorder,@huno,@targetloc ";
        //1-Checking full pallet xx
        //private string sqlprepset_process_stock_step1 = "" +
        //" select * from (select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,s.loccode, s.qtysku qtysku , s.datemfg, s.dateexp, " +
        //"       s.batchno,s.lotno, s.serialno, daterec,huno,  sum( s.qtysku ) over ( partition by s.article order by (case when p.isdlc = 1 then s.dateexp else s.daterec end) asc, loccode , huno asc) - @qtyreqsku lvsum,p.rtoskuofpck,p.skuvolume,p.isdlc " +
        //"  from wm_stock s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot  and s.article = p.article  and s.pv = p.pv and s.lv = p.lv" +
        //" where qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO' " +
        //"   and not exists (select 1 from wm_loczp z where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.tflow = 'IO') " +
        //"   and not exists (select 1 from wm_stobc b where b.orgcode = s.orgcode and b.site = s.site and b.depot = s.depot and b.huno = s.huno) " +
        //"   and not exists (select 1 from wm_taln t  where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.sourcehuno = s.huno and t.tflow not in ('CL','ED')) " +
        //"   and not exists (select 1 from wm_prepsrp t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.huno = s.huno and setno = @setno ) " +
        //"   and exists (select 1 from wm_locdw l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and spcarea = 'ST' and fltype = 'LC' " +
        //"   and l.spcpicking = 0 and tflow in ('IO','XO') ) ) a where lvsum <= 0 ";

        // private string sqlprepset_process_stock_step1 = "" +
        //" select * from (select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,s.loccode, s.qtysku qtysku , s.datemfg, s.dateexp, " +
        //"       s.batchno,s.lotno, s.serialno, daterec,huno,  sum( s.qtysku ) over ( partition by s.article order by (case when p.isdlc = 1 then s.dateexp else s.daterec end) asc, s.loccode, huno asc) - @qtyreqsku lvsum,p.rtoskuofpck,p.skuvolume,p.isdlc " +
        //"  from wm_stock s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot  and s.article = p.article  and s.pv = p.pv and s.lv = p.lv" +
        //" where qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO' " +
        //"   and not exists (select 1 from wm_loczp z where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.tflow = 'IO') " +
        //"   and not exists (select 1 from wm_stobc b where b.orgcode = s.orgcode and b.site = s.site and b.depot = s.depot and b.huno = s.huno) " +
        //"   and not exists (select 1 from wm_taln t  where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.sourcehuno = s.huno and t.tflow not in ('CL','ED')) " +
        //"   and not exists (select 1 from wm_prepsrp t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.huno = s.huno and t.setno = @setno ) " +
        //"   and exists (select 1 from wm_locdw l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and l.spcarea in('ST','OV') and l.fltype in('LC','BL') and l.lsloctype in ('LC','OV') " +
        //"   and l.spcpicking = 0 and tflow in ('IO','XO') ) ) a where lvsum <= 0 ";

        private string sqlprepset_process_stock_step1 = @" select *  from (
         select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,s.loccode, s.qtysku qtysku , s.datemfg, s.dateexp, 
               s.batchno,s.lotno, s.serialno, daterec,huno, 
               sum( s.qtysku ) over ( partition by s.article order by (case when p.isdlc = 1 then s.dateexp else s.daterec end) asc, 
       			         (case when l.spcarea ='OV' and l.fltype = 'BL' and l.lsloctype = 'OV'  then 1  else 2 end) asc, 
       			         s.loccode asc, huno asc) - @qtyreqsku lvsum,p.rtoskuofpck,p.skuvolume,p.isdlc 
          from wm_stock s
  	        join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot  and s.article = p.article  and s.pv = p.pv and s.lv = p.lv
  	        join wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode 
  	        and l.spcarea in ('ST','OV') and l.fltype in('LC','BL') and l.lsloctype in ('LC','OV') and l.spcpicking = 0 and l.tflow in ('IO','XO')
         where qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO' 
           and not exists (select 1 from wm_loczp z where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.tflow = 'IO') 
           and not exists (select 1 from wm_stobc b where b.orgcode = s.orgcode and b.site = s.site and b.depot = s.depot and b.huno = s.huno) 
           and not exists (select 1 from wm_taln t  where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.sourcehuno = s.huno and t.tflow not in ('CL','ED')) 
           and not exists (select 1 from wm_prepsrp t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.huno = s.huno and t.setno = @setno ) 
        ) a where lvsum <= 0 ";

        //2-Check stock on picking 
        private string sqlprepset_process_stock_step2 = @" select isnull(sum(qtysku),0) rsl from ( 
            select s.orgcode, s.site, s.depot, s.spcarea, stockid, article,pv,lv,loccode,qtysku, datemfg,dateexp,batchno,lotno, serialno, daterec  
            from wm_loczp z , wm_stock s where s.qtysku > 0 and z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode 
            and z.spcproduct = s.article and z.spcpv = s.pv and z.spclv = s.lv and z.spcarea = s.spcarea 
            and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcarea = 'ST' and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @lv 
            and z.tflow = 'IO' and s.tflow = 'IO'             
            union all 
            select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,t.targetadv loccode,t.sourceqty - isnull(sum(rsvsku), 0) qtysku, s.datemfg,s.dateexp,s.batchno,s.lotno, s.serialno, s.daterec 
            from wm_loczp z,wm_task th, wm_taln t , wm_stock s, wm_stobc b
            where s.qtysku > 0 and z.orgcode = t.orgcode and z.site = t.site and z.depot = t.depot and z.lscode = t.targetadv and z.spcproduct = t.article and z.spcpv = t.pv and z.spclv = t.lv 
                and t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.sourceloc = s.loccode and t.stockid = s.stockid and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
                and s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid
                and th.orgcode = t.orgcode and th.site = t.site and th.depot = t.depot and th.tasktype = 'R' and th.tflow not in ('CL','ED')
                and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcarea = 'ST' and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @lv
                and z.tflow = 'IO' and t.tflow not in ('CL','ED') and s.tflow = 'IO'
            group by s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,t.targetadv ,t.sourceqty, s.datemfg,s.dateexp,s.batchno,s.lotno, s.serialno, s.daterec 
            having (t.sourceqty- isnull(sum(rsvsku), 0)) > 0 
            
            union all 
            select orgcode, site, depot, spcarea, stockid, article, pv, lv, loccode, sourceqty - isnull(rsvqty,0) sourceqty, datemfg, dateexp, batchno, lotno, serialno, daterec 
            from wm_prepsrp where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and setno = @setno and tasktype = 'R'
                and article = @article and pv = @pv and lv = @lv and (sourceqty - isnull(rsvqty,0)) > 0
            ) s ";
        //2.1 - Generate replenishment task when stock on picking is not enough 
        // (+) = >Add Overflow Replen to picking
        private string sqlprepset_process_stock_step201 = @"
           select top 1 s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,s.loccode, s.qtysku qtysku , s.datemfg,s.dateexp, s.batchno,s.lotno,
            s.serialno, s.daterec, s.huno,p.rtoskuofpck,p.skuvolume,p.isdlc  from wm_stock s, wm_handerlingunit h , wm_product p , wm_locdw l
          where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO' and s.tflow in ('IO','XO')
            and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot  and s.article = p.article  and s.pv = p.pv and s.lv = p.lv
            and s.qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow in ('IO','XO')
            and s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode
            and l.spcarea in('ST','OV') and  l.lsloctype in ('LC','OV') and l.fltype in('LC','BL') and l.spcpicking = 0
            and not exists (select 1 from wm_loczp z where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.tflow = 'IO')
            and not exists (select 1 from wm_taln t  where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.sourcehuno = s.huno and t.tflow not in ('CL','ED'))
            and not exists (select 1 from wm_prepsrp p where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.huno = p.huno and s.stockid = p.stockid and setno = @setno )
            order by (case when p.isdlc = 1 then s.dateexp else s.daterec end) asc,(case when l.spcarea ='OV' and l.fltype = 'BL' and l.lsloctype = 'OV' then 1  else 2 end) asc,s.loccode asc, s.huno asc";
        private string sqlprepset_process_replens = @"
         select top 1 s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,s.loccode, s.qtysku qtysku , s.datemfg,s.dateexp, 
 	        s.batchno,s.lotno, s.serialno, s.daterec, s.huno,p.rtoskuofpck,p.skuvolume,p.isdlc ,p.unitprep ,p.unitmanage, p.skuweight,p.skuvolume,
            dbo.get_ratio(s.orgcode,s.site,s.depot,s.article,s.pv,s.lv,s.unitops) prepratio
            from wm_stock s, wm_handerlingunit h , wm_product p , wm_locdw l  
            where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO' and s.tflow in ('IO','XO')
	            and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot  and s.article = p.article  and s.pv = p.pv and s.lv = p.lv
	            and s.qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow in ('IO','XO')
	            and s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and l.spcarea in('ST','OV') and  l.lsloctype in ('LC','OV') and l.fltype in('LC','BL') and l.spcpicking = 0
	            and not exists (select 1 from wm_loczp z where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.tflow = 'IO')
	            and not exists (select 1 from wm_taln t  where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.sourcehuno = s.huno and t.tflow not in ('CL','ED'))
	            and not exists (select 1 from wm_prepsrp p where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.huno = p.huno /*and s.stockid = p.stockid and setno = @setno*/)
            order by  (case when p.isdlc = 1 then s.dateexp else s.daterec end) asc,(case when l.spcarea ='OV' and l.fltype = 'BL' and l.lsloctype = 'OV' then 1  else 2 end) asc,s.loccode asc, s.huno asc";

        // (-) = > backup below 12-07-2021 
        //private string sqlprepset_process_stock_step201 = @"
        //select top 1 s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,s.loccode, s.qtysku qtysku , s.datemfg,s.dateexp, s.batchno,s.lotno, 
        //    s.serialno, s.daterec, s.huno,p.rtoskuofpck,p.skuvolume,p.isdlc from wm_stock s, wm_handerlingunit h , wm_product p 
        //  where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO' and s.tflow in ('IO','XO')  
        //    and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot  and s.article = p.article  and s.pv = p.pv and s.lv = p.lv 
        //    and s.qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow in ('IO','XO') 
        //    and not exists (select 1 from wm_loczp z where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.tflow = 'IO') 
        //    and not exists (select 1 from wm_taln t  where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.sourcehuno = s.huno and t.tflow not in ('CL','ED')) 
        //    and not exists (select 1 from wm_prepsrp p where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.huno = p.huno and s.stockid = p.stockid and setno = @setno ) 
        //    and exists (select 1 from wm_locdw l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and spcarea = 'ST' and fltype = 'LC' and l.spcpicking = 0 ) 
        //    order by (case when p.isdlc = 1 then s.dateexp else s.daterec end) asc";



        //private string sqlprepset_process_stock_step201 = "" +
        //" select top 1 s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,s.loccode, s.qtysku qtysku , s.datemfg,s.dateexp, s.batchno,s.lotno, s.serialno, daterec, s.huno  " +
        //"   from wm_stock s, wm_handerlingunit h " +
        //"  where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO' and s.tflow in ('IO','XO')  " + 
        //"    and s.qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and article = @article and pv = @pv and lv = @lv and s.tflow in ('IO','XO') " +
        //"    and not exists (select 1 from wm_loczp z where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.tflow = 'IO') " +
        //"    and not exists (select 1 from wm_taln t  where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.sourcehuno = s.huno and t.tflow not in ('CL','ED')) " +
        //"    and not exists (select 1 from wm_prepsrp p where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.huno = p.huno and s.stockid = p.stockid and setno = @setno ) " +
        //"    and exists (select 1 from wm_locdw l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and spcarea = 'ST' and fltype = 'LC' and l.spcpicking = 0 ) " +
        //"  order by daterec asc";

        // FIFO = daterec asc,loccode asc
        // FEFO = dateexp asc,loccode asc


        //3 - Generate pick line 
        // private string sqlprepset_process_stock_step3 = " select orgcode,site,depot,stockid,huno,article,pv,lv, "+ 
        // " case when lvsum <= 0 then qtysku else qtysku-lvsum end qtysku, " +
        // " cast(case when lvsum <= 0 then qtysku else qtysku-lvsum end / rtoskuofpu as int) qtypu,  "+
        // " loccode,dateexp,datemfg,batchno,lotno,serialno,daterec,spcarea " +
        // " from ( select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv, " +
        // "               s.qtysku - isnull(rsvsku,0) qtysku , s.qtypu - isnull(rsvpu,0) qtypu, " +
        // "               s.loccode, s.dateexp,s.datemfg, s.batchno,s.lotno, s.serialno, daterec, s.huno, " +
        // "               sum( s.qtysku - isnull(rsvsku,0) ) over ( partition by s.article order by case when @opsunit = spcunit then 9 else 0 end desc, daterec asc ) - @qtyreqsku lvsum, rtoskuofpu " +  
        // "   from ( select s.orgcode, s.site, s.depot, s.spcarea, stockid, article,pv,lv,loccode,qtysku,qtypu, datemfg,dateexp,batchno,lotno, serialno, daterec, huno, rtoskuofpu,spcunit " +
        // "            from wm_loczp z , wm_stock s where  z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode " +
        // "             and z.spcproduct = s.article and z.spcpv = s.pv and z.spclv = s.lv and z.spcarea = s.spcarea and z.tflow = 'IO' and s.tflow = 'IO' " +
        // "             and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcarea = 'ST' and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @lv and qtysku > 0 " +
        // "  union all " + 
        // " select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.article, s.pv, s.lv,t.targetadv loccode,t.sourceqty qtysku,qtypu, s.datemfg,s.dateexp,s.batchno,s.lotno, " +
        // "        s.serialno, s.daterec, hunosource , rtoskuofpu, spcunit " +
        // "   from wm_loczp z, wm_taln t , wm_stock s " + 
        // "  where z.orgcode = t.orgcode and z.site = t.site and z.depot = t.depot and z.lscode = t.targetadv and z.spcproduct = t.article and z.spcpv = t.pv and z.spclv = t.lv " +
        // "    and t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.sourceloc = s.loccode and t.stockid = s.stockid and t.article = s.article and t.pv = s.pv " +
        // "    and t.lv = s.lv and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcarea = 'ST' and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @lv " +
        // "    and z.tflow = 'IO' and t.tflow not in ('CL','ED') and s.tflow = 'IO') s " +
        // "   left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv " +
        // "    and s.loccode = b.loccode and b.tflow = 'IO' " +
        // "  where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv " +
        // "    ) a where qtysku > lvsum ";
        // private string sqlprepset_process_stock_step3 = @" select s.orgcode,s.site,s.depot,s.stockid,s.huno,s.article,s.pv,s.lv, 
        //     case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end qtysku, 
        //     cast(case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end / s.rtoskuofpu as int) qtypu,  
        //     s.loccode,s.dateexp,s.datemfg,s.batchno,s.lotno,s.serialno,s.daterec,s.spcarea, s.taskno, p.skuweight  * s.rtoskuofpu skuweight , p.skuvolume * s.rtoskuofpu skuvolume,s.rtoskuofpu
        // from (
        //     select s.orgcode, s.site, s.depot, s.spcarea, stockid, article,pv,lv,loccode,qtysku,qtypu, datemfg,dateexp,batchno,lotno, serialno, daterec, huno, rtoskuofpu,spcunit,taskno,
        //             sum( s.qtysku ) over ( partition by s.article order by case when 1 = spcunit then 9 else 0 end desc, daterec asc ) - 1 lvsum
        //     from (select orgcode,site,depot, spcarea,lscode,spcproduct,spcpv,spclv, spcunit,rtoskuofpu
        //             from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcproduct = @article and spcpv = @pv and spclv = @lv and tflow = 'IO'
        //             union all 
        //             select orgcode,site,depot, spcarea,lscode,@article spcproduct, 0 spcpv, 0 spclv, 1 spcunit, 1 rtoskuofpu
        //             from wm_locdw where spcarea = 'RS' and orgcode = @orgcode and site = @site and depot = @depot 
        //                 and 0 = isnull((select pmvalue from wm_parameters where orgcode = @orgcode and site = @site and depot = @depot  and apps = 'WMS' 
        //                 and pmmodule = 'preparation' and pmtype = 'stock' and pmcode = 'allowincludestaging'),0) ) z , 
        //         ( 
        //         select  s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
        //                     s.qtysku - sum(isnull(b.rsvsku,0)) qtysku, s.qtypu - sum(isnull(b.rsvpu,0)) qtypu, s.qtyweight , s.qtyvolume, max(isnull(t.taskno,'')) taskno
        //             from wm_stock s join wm_handerlingunit h on s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno
        //         left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv
        //         left join 
        //         ( select l.* from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno
        //         and t.tflow = 'IO' and l.tflow = 'IO' and t.tasktype = 'R' and article = @article 
        //         and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and l.article = @article and l.pv = @pv and l.lv = @lv ) t 
        //         on s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and s.stockid = t.stockid and s.article = t.article and s.pv = t.pv and s.lv = t.lv and t.tflow not in ('CL','ED')
        //             where s.tflow = 'IO' and  h.tflow = 'IO' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv and s.qtysku > 0
        //             and not exists (select 1 from wm_task t, wm_taln l, wm_locdw c 
        //                             where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype != 'R' 
        //                                 and s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.sourcehuno )
        //             and not exists (select 1 from wm_prepsrp l where tasktype != 'A' and s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.huno and setno = @setno )
        //         group by s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv,
        //                 s.qtysku,s.qtypu, s.qtyweight , s.qtyvolume,s.daterec 
        //         having  s.qtysku - sum(isnull(b.rsvsku,0)) > 0 
        //         ) s 
        //     where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode 
        //         and z.spcproduct = s.article and z.spcpv = s.pv and z.spclv = s.lv and z.spcarea = s.spcarea
        //         and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @lv and qtysku > 0 
        //     ) s , (select orgcode, site, depot, article, pv, lv, skuweight, skuvolume from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ) p 
        // where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv
        // and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv";
        /* private string sqlprepset_process_stock_step3 = @"
         select s.orgcode,s.site,s.depot,s.stockid,s.huno,s.article,s.pv,s.lv, 
                 case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end qtysku, 
                 cast(case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end / s.rtoskuofpu as int) qtypu,  
                 s.loccode,s.dateexp,s.datemfg,s.batchno,s.lotno,s.serialno,s.daterec,s.spcarea, s.taskno, p.skuweight  * s.rtoskuofpu skuweight , p.skuvolume * s.rtoskuofpu skuvolume,s.rtoskuofpu
             from (
                 select s.orgcode, s.site, s.depot, s.spcarea, stockid, article,pv,lv,loccode,qtysku,qtypu, datemfg,dateexp,batchno,lotno, serialno, daterec, huno, rtoskuofpu,spcunit,taskno,
                         sum( s.qtysku ) over ( partition by s.article order by case when 1 = spcunit then 9 else 0 end desc, daterec asc ) - @qtyreqsku lvsum
                 from (select orgcode,site,depot, spcarea,lscode,spcproduct,spcpv,spclv, spcunit,rtoskuofpu
                         from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcproduct = @article and spcpv = @pv and spclv = @lv and tflow = 'IO'
                         union all 
                         select orgcode,site,depot, spcarea,lscode,@article spcproduct, @pv spcpv, @lv spclv, 1 spcunit, 1 rtoskuofpu
                         from wm_locdw where spcarea = 'RS' and orgcode = @orgcode and site = @site and depot = @depot 
                             and 0 = isnull((select pmvalue from wm_parameters where orgcode = @orgcode and site = @site and depot = @depot  and apps = 'WMS' 
                             and pmmodule = 'preparation' and pmtype = 'stock' and pmcode = 'allowincludestaging'),0) ) z , 
                     ( 

                     select  s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                             s.qtysku - sum(isnull(b.rsvsku,0)) qtysku, s.qtypu - sum(isnull(b.rsvpu,0)) qtypu, s.qtyweight , s.qtyvolume, max(isnull(taskno,'')) taskno
                         from 
                         ( select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                                 s.qtysku qtysku, s.qtypu qtypu, s.qtyweight , s.qtyvolume, null taskno
                             from wm_stock s, wm_handerlingunit h where qtysku > 0
                             and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO'
                             and s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO'
                             and not exists (select 1 from wm_taln t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.tflow not in ('CL','ED') and s.stockid = t.stockid )
                             and not exists (
                                     select 1 from wm_prepsrp l 
                                     where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.huno and s.stockid = l.stockid
                                     and exists (select 1 from wm_taln t where l.orgcode = t.orgcode and l.site = t.site and l.depot = l.depot and t.tflow = 'IO' and s.stockid = t.stockid)
                                     )
                             union 
                             select l.orgcode,l.site,l.depot,l.spcarea, l.stockid, l.sourcehuno,l.sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
                                 l.sourceqty qtysku, 0 qtypu, 0 qtyweight , l.sourcevolume, t.taskno
                             from wm_task t, wm_taln l
                             where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype = 'A' and l.tflow not in ('CL','ED')
                             and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.article = @article and l.pv = @pv and l.lv = @lv
                             union  
                             select l.orgcode,l.site,l.depot,l.spcarea, l.stockid,huno sourcehuno,targetloc sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
                                 l.sourceqty qtysku, 0 qtypu, 0 qtyweight , 0 sourcevolume, null taskno
                             from wm_prepsrp l where tasktype = 'R' and orgcode = @orgcode and site = @site and depot =  @depot and article = @article and pv = @pv and lv = @lv and setno = @setno ) s
                         left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv
                     group by s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv,
                             s.qtysku,s.qtypu, s.qtyweight , s.qtyvolume,s.daterec 
                     having  s.qtysku - sum(isnull(b.rsvsku,0)) > 0 

                     ) s 
                 where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode 
                     and z.spcproduct = s.article and z.spcpv = s.pv and z.spclv = s.lv and z.spcarea = s.spcarea
                     and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @pv and qtysku > 0 
                 ) s , (select orgcode, site, depot, article, pv, lv, skuweight, skuvolume from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @pv ) p 
             where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @pv
             and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv";*/

        /* comment sql */
        /* (-) 12/07/2021 => and wm_prepsrp.setno = @setno */
        private string sqlprepset_process_stock_step3 = @"
            select s.orgcode,s.site,s.depot,s.stockid,s.huno,s.article,s.pv,s.lv, 
                case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end qtysku, 
                cast(case when s.lvsum <= 0 then s.qtysku else  s.qtysku - s.lvsum end / s.rtoskuofpu as int) qtypu,  
                s.loccode,s.dateexp,s.datemfg,s.batchno,s.lotno,s.serialno,s.daterec,s.spcarea, s.taskno, p.skuweight  * s.rtoskuofpu skuweight , p.skuvolume * s.rtoskuofpu skuvolume,s.rtoskuofpu
            from (
               select s.orgcode, s.site, s.depot, s.spcarea, s.stockid,s.article,s.pv,s.lv,s.loccode,qtysku,qtypu, datemfg,dateexp,batchno,lotno, serialno, daterec, huno, rtoskuofpu,spcunit,taskno,
	                sum( s.qtysku ) over ( partition by s.stockid,s.article order by (case when 1 = spcunit then 9 else 0 end ) desc,daterec asc ) - @qtyreqsku lvsum
                from (select orgcode,site,depot, spcarea,lscode,spcproduct,spcpv,spclv, spcunit,rtoskuofpu
                        from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcproduct = @article and spcpv = @pv and spclv = @lv and tflow = 'IO'
                        union all 
                        select orgcode,site,depot, spcarea,lscode,@article spcproduct, @pv spcpv, @lv spclv, 1 spcunit, 1 rtoskuofpu
                        from wm_locdw where spcarea = 'RS' and orgcode = @orgcode and site = @site and depot = @depot 
                            and 0 = isnull((select pmvalue from wm_parameters where orgcode = @orgcode and site = @site and depot = @depot  and apps = 'WMS' 
                            and pmmodule = 'preparation' and pmtype = 'stock' and pmcode = 'allowincludestaging'),0) 
                       union all 
                       select orgcode,site,depot, spcarea,lscode,@article spcproduct, @pv spcpv, @lv spclv, 1 spcunit, 1 rtoskuofpu
	                   from wm_locdw where spcarea = 'OV' and lsloctype ='OV' and orgcode = @orgcode and site = @site and depot = @depot 
                    ) z , 
                    ( select  s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                            s.qtysku - sum(isnull(b.rsvsku,0)) qtysku, s.qtypu - sum(isnull(b.rsvpu,0)) qtypu, s.qtyweight , s.qtyvolume, max(isnull(taskno,'')) taskno
                        from 
                        ( select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                                s.qtysku qtysku, s.qtypu qtypu, s.qtyweight , s.qtyvolume, null taskno
                            from wm_stock s, wm_handerlingunit h where qtysku > 0
                            and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO'
                            and s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO'
                            and not exists (select 1 from wm_taln t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.tflow not in ('CL','ED') and s.stockid = t.stockid )
                            and not exists (
                                    select 1 from wm_prepsrp l 
                                    where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.huno and s.stockid = l.stockid
                                    and exists (select 1 from wm_taln t where l.orgcode = t.orgcode and l.site = t.site and l.depot = l.depot and t.tflow = 'IO' and s.stockid = t.stockid)
                                    )
                            union 
                            select l.orgcode,l.site,l.depot,l.spcarea, l.stockid, l.sourcehuno,l.sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
                                l.sourceqty qtysku, 0 qtypu, 0 qtyweight , l.sourcevolume, t.taskno
                            from wm_task t, wm_taln l
                            where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype = 'A' and l.tflow not in ('CL','ED')
                            and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.article = @article and l.pv = @pv and l.lv = @lv
                            union  
                            select l.orgcode,l.site,l.depot,l.spcarea, l.stockid,huno sourcehuno,targetloc sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
                                l.sourceqty qtysku, 0 qtypu, 0 qtyweight , 0 sourcevolume, null taskno
                            from wm_prepsrp l where tasktype = 'R' and orgcode = @orgcode and site = @site and depot =  @depot and article = @article and pv = @pv and lv = @lv and setno = @setno) s
                        left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv
                    group by s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv,
                            s.qtysku,s.qtypu, s.qtyweight , s.qtyvolume,s.daterec 
                    having  s.qtysku - sum(isnull(b.rsvsku,0)) > 0 
                 ) s 

                where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode 
                    and z.spcproduct = s.article and z.spcpv = s.pv and z.spclv = s.lv 
                    and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @pv and qtysku > 0 
                ) s , 
            (select orgcode, site, depot, article, pv, lv, skuweight, skuvolume,isdlc from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @pv ) p 
            where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @pv
            and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
            order by (case when p.isdlc = 1 then s.dateexp else s.daterec end) asc";

        /// <summary>
        /// get top picking location
        /// </summary>
        private string sqlprepset_process_topone_stock_step3_1 = @"
            select top 1 orgcode,site,depot,stockid,huno,article,pv,lv,qtysku,cast(qtysku /prepratio as int) qtypu,loccode,dateexp,datemfg,batchno,lotno,serialno,
		        daterec,spcarea, taskno,skuweight , skuvolume,prepratio,stocktype,isdlc
            from ( 
            select s.orgcode,s.site,s.depot,s.stockid,s.huno,s.article,s.pv,s.lv, s.qtysku,cast(s.qtysku /z.rtoskuofpu as int) qtypu,
               s.loccode,s.dateexp,s.datemfg,s.batchno,s.lotno,s.serialno,s.daterec,s.spcarea, null taskno,(p.skuweight  * z.rtoskuofpu) as skuweight , 
               (p.skuvolume * z.rtoskuofpu) as skuvolume,z.rtoskuofpu,dbo.get_ratio(z.orgcode,z.site,z.depot,z.spcproduct,z.spcpv,z.spclv,z.spcunit) prepratio,
               p.isdlc,s.stocktype
            from  wm_loczp z ,wm_product p , (
	            select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, 
		             s.daterec,s.qtysku - sum(isnull(b.rsvsku,0)) qtysku, s.qtypu - sum(isnull(b.rsvpu,0)) qtypu, s.qtyweight , s.qtyvolume,s.stocktype
                from (select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                           s.qtysku qtysku, s.qtypu qtypu, s.qtyweight , s.qtyvolume ,'S' stocktype  from wm_stock s, wm_handerlingunit h where qtysku > 0
                    and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO'
                    and s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO'
                    and not exists (select 1 from wm_taln t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.tflow not in ('CL','ED') and s.stockid = t.stockid )
                    and not exists (select 1 from wm_prepsrp l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.huno and s.stockid = l.stockid
                    and exists (select 1 from wm_taln t where l.orgcode = t.orgcode and l.site = t.site and l.depot = l.depot and t.tflow = 'IO' and s.stockid = t.stockid)) 
                    union select l.orgcode,l.site,l.depot,l.spcarea, l.stockid,huno sourcehuno,targetloc sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
                          l.sourceqty qtysku, 0 qtypu, 0 qtyweight , 0 qtyvolume,'R' stocktype
                    from wm_prepsrp l where tasktype = 'R' and orgcode = @orgcode and site = @site and depot =  @depot and article = @article and pv = @pv and lv = @lv
                ) s left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv
                group by s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv,
                         s.qtysku,s.qtypu, s.qtyweight , s.qtyvolume,s.daterec,s.stocktype  having s.qtysku-sum(isnull(b.rsvsku,0)) > 0 
            ) s where z.orgcode = s.orgcode and z.site = s.site and z.depot = s.depot and z.lscode = s.loccode and z.spcproduct = s.article and z.spcpv = s.pv and z.spclv = s.lv 
	            and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
	            and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and z.spcproduct = @article and z.spcpv = @pv and z.spclv = @pv and z.tflow = 'IO' 
	            and qtysku > 0 	and exists (select 1 from wm_locdw d where d.orgcode = z.orgcode and d.site = z.site and d.depot = z.depot and d.lscode = z.lscode and d.tflow = 'IO')
            ) picking order by (case when isdlc = 1 then dateexp else daterec end) asc,loccode asc, huno asc";

        //4 - Update order status from prep and full pallet task
        //private string sqlprepset_process_stock_step4 = @" 
        //update t set t.orgcode = s.orgcode,t.site = s.site,t.depot = s.depot,t.spcarea = s.spcarea,t.article = s.article,  
        //t.qtypndsku = qtysku - ( qtyskuorder + qtyskudel ),t.qtypndpu =  qtypu - ( qtypuorder + qtypudel ), 
        //t.qtyreqpu = qtysku - ( qtyskuorder + qtyskudel ),t.qtyreqsku = qtypu - ( qtypuorder + qtypudel ), t.tflow = 'PC' 
        //from wm_outbouln t left join  (
        //    select p.orgcode,p.site,p.depot,p.spcarea,ouorder,ouln, article,pv,lv, sum(qtyskuorder) qtyskuorder, sum(qtypuorder) qtypuorder, sum(qtyweightorder) qtyweightorder, sum(qtyvolumeorder) qtyvolumeorder 
        //        from wm_prep p, wm_prln l where p.tflow = 'IO' and l.tflow = 'IO'
        //        and p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno
        //        and p.orgcode = @orgcode and p.site = @site and p.depot = @depot  and p.setno = @setno
        //    group by p.orgcode,p.site,p.depot,p.spcarea,l.ouorder,l.ouln, l.article,l.pv,l.lv
        //    union 
        //    select p.orgcode,p.site,p.depot,p.spcarea,l.ouorder,l.ouln, l.article,l.pv,l.lv, sum(sourceqty) qtyskuorder, sum(sourceqty) qtypuorder, 0 qtyweightorder, sum(sourcevolume) qtyvolumeorder 
        //    from wm_task p, wm_taln l where tasktype = 'A' and p.tflow = 'IO' and l.tflow = 'IO'
        //    and p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.taskno = l.taskno 
        //    and p.orgcode = @orgcode and p.site = @site and p.depot = @depot  and p.setno = @setno
        //    group by p.orgcode,p.site,p.depot,p.spcarea,l.ouorder,l.ouln, l.article,l.pv,l.lv

        //) s on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder and t.ouln = s.ouln  and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
        //where exists (select 1 from (select distinct l.orgcode,l.site, l.depot, l.ouorder,l.ouln,l.article, l.pv, l.lv from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site 
        //and p.depot = l.depot and p.prepno = l.prepno and setno = @setno) a where t.orgcode = a.orgcode and t.site = a.site and t.depot = a.depot and t.ouorder = a.ouorder and t.ouln = a.ouln 
        //and t.article = a.article and t.pv = a.pv and t.lv = a.lv) and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.tflow not in ('CL','ED')
        //";
        private string sqlprepset_process_stock_step4 = @" 
        update t set t.qtypndsku = qtysku - ( qtyskuorder + qtyskudel ),t.qtypndpu =  qtypu - ( qtypuorder + qtypudel ), 
        t.qtyreqpu = qtysku - (qtyskuorder + qtyskudel ),t.qtyreqsku = qtypu - ( qtypuorder + qtypudel ), t.tflow = 'PC' 
        from wm_outbouln t left join  (
            select p.orgcode,p.site,p.depot,p.spcarea,ouorder,ouln, article,pv,lv, sum(qtyskuorder) qtyskuorder, sum(qtypuorder) qtypuorder, sum(qtyweightorder) qtyweightorder, sum(qtyvolumeorder) qtyvolumeorder 
                from wm_prep p, wm_prln l where p.tflow = 'IO' and l.tflow = 'IO'
                and p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno
                and p.orgcode = @orgcode and p.site = @site and p.depot = @depot  and p.setno = @setno
            group by p.orgcode,p.site,p.depot,p.spcarea,l.ouorder,l.ouln, l.article,l.pv,l.lv
            union 
            select p.orgcode,p.site,p.depot,p.spcarea,l.ouorder,l.ouln, l.article,l.pv,l.lv, sum(sourceqty) qtyskuorder, sum(sourceqty) qtypuorder, 0 qtyweightorder, sum(sourcevolume) qtyvolumeorder 
            from wm_task p, wm_taln l where tasktype = 'A' and p.tflow = 'IO' and l.tflow = 'IO'
            and p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.taskno = l.taskno 
            and p.orgcode = @orgcode and p.site = @site and p.depot = @depot  and p.setno = @setno
            group by p.orgcode,p.site,p.depot,p.spcarea,l.ouorder,l.ouln, l.article,l.pv,l.lv
            
        ) s on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder and t.ouln = s.ouln  and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
        where exists (select 1 from (select distinct l.orgcode,l.site, l.depot, l.ouorder,l.ouln,l.article, l.pv, l.lv from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site 
        and p.depot = l.depot and p.prepno = l.prepno and setno = @setno) a where t.orgcode = a.orgcode and t.site = a.site and t.depot = a.depot and t.ouorder = a.ouorder and t.ouln = a.ouln 
        and t.article = a.article and t.pv = a.pv and t.lv = a.lv) and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.tflow not in ('CL','ED')
        ";
        private string sqlprepset_process_stock_step5 = " update wm_outbound set tflow = 'PC',datemodify = sysdatetimeoffset(), accnmodify = @accncode, procmodify = @procmodify  "+ 
        " where orgcode = @orgcode and site = @site and depot = @depot and exists (select 1 from wm_prepsln p where wm_outbound.orgcode = p.orgcode and wm_outbound.site = p.site " +
        " and wm_outbound.depot = p.depot and wm_outbound.ouorder = p.ouorder and p.setno=@setno and tflow != 'ED')";
        //6 - update preparation set status
        private string sqlprepset_process_stock_step6 = 
        " update t set qtyskuops = s.qtyskuorder,qtypuops = s.qtypuorder,qtyweightops = s.qtyweightorder,qtyvolumeops = s.qtyvolumeorder, " + 
        " tflow = case when t.qtyskuorder = s.qtyskuorder then 'CM' else 'XX' end, errmsg = case when t.qtyskuorder = s.qtyskuorder then '' else 'Stock not enought for preparation' end, " +
        " routeno = s.routeno " + 
        "  from wm_prepsln t left join ( select p.orgcode, p.site, p.depot, p.spcarea,p.setno,p.routeno, l.ouorder,l.ouln, l.article, l.pv, l.lv, " +
        "       sum(qtyskuorder) qtyskuorder, sum(qtypuorder) qtypuorder, sum(qtyweightorder) qtyweightorder, sum(qtyvolumeorder) qtyvolumeorder " +
        "      from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno and p.spcarea= 'ST'" +
        "       and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.setno = @setno " +
        "     group by p.orgcode, p.site, p.depot, p.spcarea,p.setno,p.routeno, l.ouorder,l.ouln, l.article, l.pv, l.lv ) s" +
        "     on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.setno = s.setno and t.ouorder = s.ouorder and t.ouln = s.ouln " +
        "       and t.article = s.article and t.pv = s.pv and t.lv = s.lv where t.orgcode = @orgcode and t.site=  @site and t.depot = @depot and t.setno = @setno";

        // process stock for RTV 
        private string sqlprepset_process_stock_step3_forrtv = @"
        select s.orgcode,s.site,s.depot,s.stockid,s.huno,s.article,s.pv,s.lv, 
            case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end qtysku, 
            cast(case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end / 1 as int) qtypu,  
            s.loccode,s.dateexp,s.datemfg,s.batchno,s.lotno,s.serialno,s.daterec,s.spcarea, s.taskno, p.skuweight  * 1 skuweight , p.skuvolume * 1 skuvolume,1 rtoskuofpu
        from (
            select s.orgcode, s.site, s.depot, s.spcarea, stockid, article,pv,lv,loccode,qtysku,qtypu, datemfg,dateexp,batchno,lotno, serialno, daterec, huno,1 rtoskuofpu,1 spcunit,taskno,
                    sum( s.qtysku ) over ( partition by s.article order by daterec asc ) - @qtyreqsku  lvsum
            from ( 
                select  s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                        s.qtysku - sum(isnull(b.rsvsku,0)) qtysku, s.qtypu - sum(isnull(b.rsvpu,0)) qtypu, s.qtyweight , s.qtyvolume, max(isnull(taskno,'')) taskno
                    from 
                    ( select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                            s.qtysku qtysku, s.qtypu qtypu, s.qtyweight , s.qtyvolume, null taskno
                        from wm_stock s, wm_handerlingunit h where qtysku > 0
                        and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO'
                        and s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO'
                        and not exists (select 1 from wm_taln t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.tflow not in ('CL','ED') and s.stockid = t.stockid )
                        and not exists (select 1 from wm_prepsrp l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.huno and s.stockid = l.stockid )
                        and exists     (select 1 from wm_locdw l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and l.tflow in ('IO','XO') and l.spcarea = 'RN' and l.lsloctype = 'VD' )
                        union 
                        select l.orgcode,l.site,l.depot,l.spcarea, l.stockid, l.sourcehuno,l.sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
                            l.sourceqty qtysku, 0 qtypu, 0 qtyweight , l.sourcevolume, t.taskno
                        from wm_task t, wm_taln l
                        where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype = 'A' and l.tflow not in ('CL','ED')
                        and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.article = @article and l.pv = @pv and l.lv = @lv
                        union  
                        select l.orgcode,l.site,l.depot,l.spcarea, l.stockid,huno sourcehuno,targetloc sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
                            l.sourceqty qtysku, 0 qtypu, 0 qtyweight , 0 sourcevolume, null taskno
                        from wm_prepsrp l where tasktype = 'R' and orgcode = @orgcode and site = @site and depot =  @depot and article = @article and pv = @pv and lv = @lv and setno = @setno ) s
                    left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv
                group by s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv,
                        s.qtysku,s.qtypu, s.qtyweight , s.qtyvolume,s.daterec 
                having  s.qtysku - sum(isnull(b.rsvsku,0)) > 0 
                ) s 
            where qtysku > 0 
            ) s , (select orgcode, site, depot, article, pv, lv, skuweight, skuvolume from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ) p 
        where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv
        and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
        ";

        /// <summary>
        /// get top rtv location
        /// </summary>
        private string sqlprepset_process_topone_stock_step3_1rtv = @"
            select top 1 orgcode,site,depot,stockid,huno,article,pv,lv,qtysku,cast(qtysku /prepratio as int) qtypu,loccode,dateexp,datemfg,batchno,lotno,serialno,
                daterec,spcarea, taskno,skuweight , skuvolume,prepratio,stocktype,isdlc
            from ( 
            select s.orgcode,s.site,s.depot,s.stockid,s.huno,s.article,s.pv,s.lv,s.qtysku,s.loccode,s.dateexp,s.datemfg,s.batchno,
	            s.lotno,s.serialno,s.daterec,s.spcarea, null taskno,p.skuweight as skuweight , p.skuvolume,p.rtoskuofpu,
	            dbo.get_ratio(p.orgcode,p.site,p.depot,p.article ,p.pv,p.lv,p.rtoskuofpu) prepratio, p.isdlc,s.stocktype
            from (
                select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, 
                     s.daterec,s.qtysku - sum(isnull(b.rsvsku,0)) qtysku, s.qtypu - sum(isnull(b.rsvpu,0)) qtypu, s.qtyweight , s.qtyvolume,s.stocktype
                from (
                    select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
                           s.qtysku qtysku, s.qtypu qtypu, s.qtyweight , s.qtyvolume ,'S' stocktype  from wm_stock s, wm_handerlingunit h where qtysku > 0
                    and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO'
                    and s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO'
                    and     exists (select 1 from wm_locdw l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and l.tflow in ('IO','XO') and l.spcarea = 'RN' and l.lsloctype = 'VD' )
                    and not exists (select 1 from wm_taln t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.tflow not in ('CL','ED') and s.stockid = t.stockid )
                    and not exists (select 1 from wm_prepsrp l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.huno and s.stockid = l.stockid
                    and exists (select 1 from wm_taln t where l.orgcode = t.orgcode and l.site = t.site and l.depot = l.depot and t.tflow = 'IO' and s.stockid = t.stockid))
                    union
                    select l.orgcode,l.site,l.depot,l.spcarea, l.stockid,huno sourcehuno,targetloc sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp,
                           l.serialno, l.lv, null daterec,l.sourceqty qtysku, 0 qtypu, 0 qtyweight , 0 qtyvolume,'R' stocktype
                    from wm_prepsrp l where tasktype = 'R' and orgcode = @orgcode and site = @site and depot =  @depot and article = @article and pv = @pv and lv = @lv 
                ) s left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv
                    group by s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv,
                             s.qtysku,s.qtypu, s.qtyweight , s.qtyvolume,s.daterec,s.stocktype  having s.qtysku-sum(isnull(b.rsvsku,0)) > 0
            ) s , wm_product p
            where  s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
                and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @pv and qtysku > 0
            ) rtv order by (case when isdlc = 1 then dateexp else daterec end) asc,loccode asc, huno asc";


        // backup script 01/07/2021 nuchit
        //private string sqlprepset_process_stock_step3_forrtv = @"
        //select s.orgcode,s.site,s.depot,s.stockid,s.huno,s.article,s.pv,s.lv, 
        //    case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end qtysku, 
        //    cast(case when s.lvsum <= 0 then s.qtysku else s.qtysku-s.lvsum end / 1 as int) qtypu,  
        //    s.loccode,s.dateexp,s.datemfg,s.batchno,s.lotno,s.serialno,s.daterec,s.spcarea, s.taskno, p.skuweight  * 1 skuweight , p.skuvolume * 1 skuvolume,1 rtoskuofpu
        //from (
        //    select s.orgcode, s.site, s.depot, s.spcarea, stockid, article,pv,lv,loccode,qtysku,qtypu, datemfg,dateexp,batchno,lotno, serialno, daterec, huno,1 rtoskuofpu,1 spcunit,taskno,
        //            sum( s.qtysku ) over ( partition by s.article order by daterec asc ) - @qtyreqsku  lvsum
        //    from ( 
        //        select  s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
        //                s.qtysku - sum(isnull(b.rsvsku,0)) qtysku, s.qtypu - sum(isnull(b.rsvpu,0)) qtypu, s.qtyweight , s.qtyvolume, max(isnull(taskno,'')) taskno
        //            from 
        //            ( select s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv, s.daterec,
        //                    s.qtysku qtysku, s.qtypu qtypu, s.qtyweight , s.qtyvolume, null taskno
        //                from wm_stock s, wm_handerlingunit h where qtysku > 0
        //                and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv and s.tflow = 'IO'
        //                and s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno and h.tflow = 'IO'
        //                and not exists (select 1 from wm_taln t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and t.tflow not in ('CL','ED') and s.stockid = t.stockid )
        //                and not exists (select 1 from wm_prepsrp l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.huno = l.huno and s.stockid = l.stockid )
        //                and exists     (select 1 from wm_locdw l where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and l.tflow in ('IO','XO') and l.spcarea = 'RN' and l.lsloctype = 'VD' )
        //                union 
        //                select l.orgcode,l.site,l.depot,l.spcarea, l.stockid, l.sourcehuno,l.sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
        //                    l.sourceqty qtysku, 0 qtypu, 0 qtyweight , l.sourcevolume, t.taskno
        //                from wm_task t, wm_taln l
        //                where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype = 'A' and l.tflow not in ('CL','ED')
        //                and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.article = @article and l.pv = @pv and l.lv = @lv
        //                union  
        //                select l.orgcode,l.site,l.depot,l.spcarea, l.stockid,huno sourcehuno,targetloc sourceloc,l.article, l.pv, l.batchno, l.lotno, l.datemfg, l.dateexp, l.serialno, l.lv, null daterec,
        //                    l.sourceqty qtysku, 0 qtypu, 0 qtyweight , 0 sourcevolume, null taskno
        //                from wm_prepsrp l where tasktype = 'R' and orgcode = @orgcode and site = @site and depot =  @depot and article = @article and pv = @pv and lv = @lv and setno = @setno ) s
        //            left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv
        //        group by s.orgcode,s.site,s.depot,s.spcarea, s.stockid, s.huno,s.loccode,s.article, s.pv,s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.lv,
        //                s.qtysku,s.qtypu, s.qtyweight , s.qtyvolume,s.daterec 
        //        having  s.qtysku - sum(isnull(b.rsvsku,0)) > 0 
        //        ) s 
        //    where qtysku > 0 
        //    ) s , (select orgcode, site, depot, article, pv, lv, skuweight, skuvolume from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ) p 
        //where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv
        //and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
        //";
        //Process order for distribution
        //1 - Generate distribution plan : pam > @prepno, @accncreate, @procmodify, @orgcode, @site, @depot, @inorder, @huno
        private string sqlprepset_process_dist_huinfo = ""+ 
        " select s.huno, s.hutype, n.przone, s.inrefno inorder, s.article, s.pv, s.lv						   " + 
        "   from wm_stock s, wm_loczn n  where s.orgcode = n.orgcode and s.site = n.site and s.depot = n.depot " +  
        "    and s.spcarea = n.spcarea and s.tflow = 'IO' and n.tflow = 'IO' and s.spcarea = 'XD'              " + 
        "    and s.inrefno = @inorder and s.article = @article and s.pv = @pv and s.lv = @lv                   " + 
        "    and not exists ( select 1  from wm_prep p where s.orgcode = p.orgcode and s.site = p.site         " + 
        "    and s.depot = p.depot and p.huno = s.huno )  and s.stockid = @stockid and s.huno = @huno          " ;
        
        private string sqlprepset_process_dist_step1 = "" + 
        //  @"insert into wm_prln (orgcode,site,depot,spcarea,routeno,thcode,huno,hunosource,prepno,prepln,loczone,loccode,locseq,locdigit, 
        //     ouorder,ouln,barcode,article,pv,lv,stockid,unitprep,qtyskuorder,qtypuorder,qtyweightorder,qtyvolumeorder,qtyskuops,qtypuops, 
        //     qtyweightops,qtyvolumeops,batchno,lotno,datemfg,dateexp,serialno,picker,datepick,devicecode,tflow,datecreate,accncreate, 
        //     datemodify,accnmodify,procmodify)
        @"select  p.orgcode, p.site, p.depot, p.spcarea,null routeno,p.thcode thcode, null huno, dishuno hunosource, @prepno prepno, 
            ROW_NUMBER() OVER(ORDER BY p.dishuno, lspath ASC) prepln, lszone loczone, z.lscode loccode,ROW_NUMBER() OVER(ORDER BY dishuno, lspath ASC) locseq, null locdigit, 
            ouorder,ouln,barcode, p.article, p.pv, p.lv, disstockid, p.unitprep, qtyskuorder,qtypuorder,qtyweightorder, qtyvolumeorder, 
            0 qtyskuops, 0 qtypuops,0.0 qtyweightops, 
            0.0 qtyvolumeops, p.batchno, p.lotno, p.datemfg, p.dateexp, p.serialno, null picker, null datepick, null devicecode, 'IO' tflow, sysdatetimeoffset() datecreate,
            @accncreate accncreate, sysdatetimeoffset() datemodify, @accncreate accnmodify,@procmodify procmodify, a.descalt,a.skuweight,a.rtoskuofpu,null taskno,
            disstockid stockid, s.daterec daterec, s.inagrn, s.ingrno
        from   wm_prepsln p, (select * from wm_loczp where spcarea = 'XD' and orgcode = @orgcode and site = @site and depot = @depot  and tflow = 'IO'  ) z, wm_product a,  wm_stock s 
        where   p.tflow = 'WC' and p.orgcode = z.orgcode and p.site = z.site and p.depot = z.depot and p.przone = z.przone and p.thcode = z.spcthcode
        and  p.orgcode = a.orgcode and p.site = a.site and p.depot = a.depot and p.article = a.article and p.pv = a.pv and p.lv = a.lv 
        and  p.orgcode = s.orgcode and p.site = s.site and p.depot = s.depot and p.article = s.article and p.pv = s.pv and p.lv = s.lv and p.disstockid = s.stockid and p.dishuno = s.huno
        and   setno = @setno and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.tflow = 'WC' order by prepln";
        // " insert into wm_prln (orgcode,site,depot,spcarea,routeno,thcode,huno,hunosource,prepno,prepln,loczone,loccode,locseq,locdigit, " + 
        // " ouorder,ouln,barcode,article,pv,lv,stockid,unitprep,qtyskuorder,qtypuorder,qtyweightorder,qtyvolumeorder,qtyskuops,qtypuops, " + 
        // " qtyweightops,qtyvolumeops,batchno,lotno,datemfg,dateexp,serialno,picker,datepick,devicecode,tflow,datecreate,accncreate, " +
        // " datemodify,accnmodify,procmodify)  " + 
        // " select  a.orgcode, a.site, a.depot, a.spcarea,null routeno,disthcode thcode, null huno, huno hunosource, @prepno prepno, " +
        // "         ROW_NUMBER() OVER(ORDER BY lspath ASC) prepln, lszone loczone, lscode loccode,0 locseq, null locdigit, " +
        // "         ouorder,ouln,barcode, a.article, a.pv, a.lv, stockid, unitops unitprep, qtyreqsku,qtyreqpu,qtyreqsku * skuweight qtyweightorder, qtyreqsku * skuvolume qtyvolumeorder, "+ 
        // "         0 qtyskuops, 0 qtypuops,0.0 qtyweightops, " +
        // "         0.0 qtyvolumeops,batchno, lotno, datemfg, dateexp, serialno, null picker, null datepick, null devicecode, 'IO' tflow, sysdatetimeoffset(),@accncreate accncreate," +
        // "         sysdatetimeoffset(),@accncreate accnmodify,@procmodify procmodify, p.descalt,p.skuweight,p.rtoskuofpu " +
        // " from ( select s.orgcode, s.site, s.depot, s.spcarea, ouorder, ouln , inorder, barcode, s.article, s.pv, s.lv, o.unitops,o.qtyreqsku, " +
        // "       qtyreqpu,disthcode, s.qtysku, sum( o.qtyreqsku ) over ( partition by o.article order by disthcode asc) - s.qtysku lvsum, " +
        // "       s.datemfg, s.dateexp, s.batchno, s.lotno,s.serialno,huno,lscode,lszone,s.stockid,lspath " +
        // "          from ( select o.orgcode, o.site, o.depot, o.spcarea,ouorder,ouln,inorder,o.barcode,o.article,o.pv,o.lv,unitops,qtyreqsku, qtyreqpu,disthcode,lscode,lszone,lspath" +
        // "                   from wm_outbouln o , wm_loczp z where z.tflow = 'IO' and o.tflow in ('IO')" +
        // "                    and o.orgcode = z.orgcode and o.site = z.site and o.depot = z.depot and o.disthcode = z.spcthcode" +
        // "                    and o.orgcode = @orgcode  and o.site = @site and  o.depot = @depot and inorder = @inorder ) o, wm_stock s " +
        // "         where o.orgcode = s.orgcode and o.site = s.site and o.depot = s.depot and o.spcarea = s.spcarea and o.article = s.article and o.pv = s.pv and o.lv = s.lv" +
        // "           and not exists (select 1 from wm_stobc b where s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid "+ 
        // " and s.article = b.article and s.pv = b.pv and s.lv = b.lv  and opsno != @prepno )" +
        // "           and s.huno = @huno and o.inorder = s.inrefno and o.article = s.article and o.pv = s.pv and o.lv = s.lv ) a  ,wm_product p " + 
        // "  where a.orgcode = p.orgcode and a.site = p.site and a.depot = p.depot and a.article = p.article and a.pv = p.lv and a.lv = p.lv and lvsum <= 0 ";
        private string sqlprepset_process_dist_step2 = @" update t set 
                t.qtypndsku = qtysku - ( isnull(qtyskuorder,0) + isnull(qtyskudel,0) ),
                t.qtyreqsku = qtysku - ( isnull(qtyskuorder,0) + isnull(qtyskudel,0) ), 
                t.qtypndpu = ( qtysku - ( isnull(qtyskuorder,0) + isnull(qtyskudel,0) ) ) /  dbo.get_ratiopu_prep(t.orgcode, t.site, t.depot,t.article,t.pv,t.lv) , 
                t.qtyreqpu = ( qtysku - ( isnull(qtyskuorder,0) + isnull(qtyskudel,0) ) ) /  dbo.get_ratiopu_prep(t.orgcode, t.site, t.depot,t.article,t.pv,t.lv) ,        
                t.tflow = 'PC' 
        from wm_outbouln t  left join 
        (select orgcode,site,depot,spcarea,ouorder,ouln, article,pv,lv, sum(qtyskuorder) qtyskuorder, sum(qtypuorder) qtypuorder, 
                sum(qtyweightorder) qtyweightorder, sum(qtyvolumeorder) qtyvolumeorder 
            from wm_prln where ouorder in (select distinct ouorder from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site 
            and p.depot = l.depot and p.prepno = l.prepno and setno = @setno and p.orgcode = @orgcode and p.site = @site and p.depot = @depot ) 
            and orgcode = @orgcode and site = @site and depot = @depot and tflow = 'IO' 
            group by orgcode,site,depot,spcarea,ouorder,ouln, article,pv,lv) s 
                on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder and t.ouln = s.ouln 
            and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
            where exists (select 1 from (select distinct l.orgcode,l.site, l.depot, l.ouorder,l.ouln,l.article, l.pv, l.lv from wm_prep p, wm_prln l 
                            where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno 
                            and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and setno = @setno ) a 
                            where t.orgcode = a.orgcode and t.site = a.site and t.depot = a.depot and t.ouorder = a.ouorder and t.ouln = a.ouln 
                            and t.article = a.article  and t.pv = a.pv and t.lv = a.lv) 
            and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.tflow not in ('CL','ED') ";
        private string sqlprepset_process_dist_step3 = "" +
        " update t set qtyskuops = s.qtyskuorder,qtypuops = s.qtypuorder,qtyweightops = s.qtyweightorder,qtyvolumeops = s.qtyvolumeorder, " +
        "              tflow = case when t.qtyskuorder = s.qtyskuorder then 'CM' else 'XX' end, " +
        "              errmsg = case when t.qtyskuorder = s.qtyskuorder then '' else 'Stock not enought for preparation' end " + 
        " from wm_prepsln t, " +
        "     ( select p.orgcode, p.site, p.depot, p.spcarea,p.setno,l.ouorder,l.ouln, l.article, l.pv, l.lv, " +
        "              sum(qtyskuorder) qtyskuorder, sum(qtypuorder) qtypuorder, sum(qtyweightorder) qtyweightorder, sum(qtyvolumeorder) qtyvolumeorder " +
        "         from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno and p.spcarea= 'XD' " +
        "          and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.setno = @setno" +
        "     group by p.orgcode, p.site, p.depot, p.spcarea,p.setno,l.ouorder,l.ouln, l.article, l.pv, l.lv ) s " +
        "     where t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.setno = s.setno and t.ouorder = s.ouorder and t.ouln = s.ouln " +
        "       and t.article = s.article and t.pv = s.pv and t.lv = s.lv " +
        "       and t.orgcode = @orgcode and t.site=  @site and t.depot = @depot and t.setno = @setno ";
        
        private string sqlprep_stock_getroute = "select top 1 routeno,thcode,loccode from wm_route where orgcode = @orgcode and site = @site " + 
        " and depot = @depot and thcode = @thcode and tflow = 'IO' and len(routeno) = 8 order by routeno ";


        private string sqlprep_cancel_step1 = @"update t set tflow = 'CL', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = 'ouprep.cancel'
        from wm_stobc t, wm_prep p where t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.opsno = p.prepno and p.spcarea = 'XD' 
        and p.huno = t.hunosource and p.tflow = 'IO' and t.tflow = 'IO' and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.prepno = @prepno";
        private string sqlprep_cancel_step2 = @"update wm_prep set tflow = 'CL', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = 'api.prep.cancel'
        where tflow = 'IO' and spcarea = 'XD' and prepno = @prepno and orgcode = @orgcode and site = @site and depot = @depot";
        private string sqlprep_cancel_step3 = @"update wm_prln set tflow = 'CL', datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = 'api.prep.cancel'
        where tflow = 'IO' and spcarea = 'XD' and prepno = @prepno and orgcode = @orgcode and site = @site and depot = @depot";
        private string sqlprep_cancel_step4 = @" update t 
        set qtypndsku = qtysku - ( isnull(qtyskudel,0) + qtyskuorder ) ,
            qtypndpu  = qtysku - ( isnull(qtyskudel,0) + qtyskuorder ) /  dbo.get_ratiopu_prep(t.orgcode,t.site,t.depot,t.article,t.pv,t.lv) ,
            qtyreqsku = qtysku - ( isnull(qtyskudel,0) + qtyskuorder ) ,
            qtyreqpu  = qtysku - ( isnull(qtyskudel,0) + qtyskuorder ) /  dbo.get_ratiopu_prep(t.orgcode,t.site,t.depot,t.article,t.pv,t.lv),
            datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = 'api.prep.cancel'
        from wm_outbouln t,
        ( select orgcode,site,depot, ouorder, ouln, sum(qtyskuorder) qtyskuorder, sum(qtypuorder) qtypuorder 
            from wm_prln l where exists (select 1 from wm_prln e where l.orgcode = e.orgcode and l.site = e.site and l.depot = e.depot and l.ouorder = e.ouorder and l.ouln = e.ouln 
            and e.prepno = @prepno and e.orgcode = @orgcode and e.site = @site and e.depot = @depot ) and l.tflow not in ('CL')
            group by orgcode, site, depot, ouorder, ouln ) o
        where t.orgcode = o.orgcode and t.site = o.site and t.depot = o.depot and t.ouorder = o.ouorder and t.ouln = o.ouln and t.tflow in ('IO','PC')
        and t.orgcode = @orgcode  and t.site = @site and t.depot = @depot ";

        private string sqlprep_cancel_step5 = @"UPDATE o set tflow ='IO', datemodify = sysdatetimeoffset(),accnmodify = @accnmodify, procmodify = 'api.prep.cancel'
        FROM wm_outbound o WHERE o.orgcode = @orgcode and o.site = @site and o.depot = @depot
        AND EXISTS (SELECT l.ouorder FROM wm_outbouln l WHERE l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder 
	        AND exists (select 1 from wm_prln p WHERE p.orgcode = l.orgcode AND p.site = l.site and p.depot = l.depot and p.ouorder = l.ouorder and p.prepno = @prepno )
	        GROUP BY l.ouorder HAVING sum(qtypndsku) = 0)";

        /* Select for Pereperation Order */
        private string sqlSelected = 
            @"INSERT INTO dbo.wm_ouselect(orgcode, site, depot, spcarea, ouorder, outype, ousubtype, thcode, selected, selectdate, selectaccn, selectflow)
                VALUES(@orgcode,@site,@depot,@spcarea,@ouorder,@outype,@ousubtype,@thcode, 1, getdate(),@selectaccn,@selectflow)";

        /* Unselect for Pereperation Order */
        private string sqlUnselected = @"DELETE FROM wm_ouselect WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND ouorder = @ouorder AND selected = 1";

        /* Check for Pereperation Order */
        private string sqlIsselected = @"SELECT top 1 selectaccn,selectdate FROM wm_ouselect WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND spcarea = @spcarea AND ouorder = @ouorder AND selected = 1";

        /* process full pallet */
        private string sqlprocess_order_fullpallet_top1 =
         @"select top 1 * from (select  orgcode,site,depot,spcarea,stockid,article, pv, lv,loccode, qtysku ,datemfg,dateexp,batchno,lotno,serialno, daterec,huno,skuweight,rtoskuofpu,
                skuvolume,isdlc, prepratio,lspcarea,fltype,lsloctype,sum(qtysku ) over ( partition by article order by (case when isdlc = 1 then dateexp else daterec end) asc, 
   			    lsloctype desc, loccode asc, huno asc) - @qtyreqsku lvsum
        from snaps_Stockfullpallet s  where orgcode = @orgcode and site = @site and depot = @depot
        and spcarea = 'ST' and article = @article and pv = @pv and lv = @lv ) a where lvsum <= 0";

         /* process loospick */
        private string sqlprocess_order_loosepick_top1 =
        @"select top 1 s.orgcode, site, depot, spcarea, stockid, article, pv, lv, loccode, qtysku, datemfg, dateexp, batchno, lotno, serialno,
               daterec, huno, rtoskuofpu, skuweight,skuvolume, isdlc,prepratio from snaps_Stockpicking s
        where s.qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.article = @article and s.pv = @pv and s.lv = @lv
         order by (case when isdlc = 1 then dateexp else daterec end) asc,loccode asc, huno asc";
        /* process replen pick */
        private string sqlprocess_order_replen_top1 =
        @"select top 1 s.orgcode, site, depot, spcarea, stockid, article, pv, lv, loccode, qtysku, datemfg, dateexp, batchno, lotno, serialno,
               daterec, huno, rtoskuofpu, skuweight,skuvolume, isdlc,prepratio, lspcarea, fltype, lsloctype from snaps_Stockreplens s
        where s.qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv
        order by (case when s.isdlc = 1 then s.dateexp else s.daterec end) asc,lsloctype desc, s.loccode asc, huno asc";

        private string sqlprocess_order_rtvpick_top1 =
        @"select top 1 orgcode, site, depot, spcarea, stockid, article, pv, lv, loccode, qtysku, datemfg, dateexp, batchno, lotno, serialno, daterec, huno, rtoskuofpu,skuweight, skuvolume, isdlc, prepratio
        from snaps_Stockvendor s where s.qtysku > 0 and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = 'ST' and s.article = @article and s.pv = @pv and s.lv = @lv
        order by (case when s.isdlc = 1 then s.dateexp else s.daterec end) asc,s.loccode asc, huno asc";

        private string sqlprepset_process_reserve =
           @"exec dbo.snaps_Reservestock 
                               @orgcode
                              ,@site
                              ,@depot
                              ,@spcarea
                              ,@stockid
                              ,@article
                              ,@pv
                              ,@lv
                              ,@loccode
                              ,@huno
                              ,@setno
                              ,@tasktype
                              ,@targetloc
                              ,@rsvqty
                              ,@ouorder
                              ,@prepno
                              ,@accncode";

        private string sqlprepset_process_unreserve =
            @"exec dbo.snaps_Unreservestock 
                             @orgcode
                            ,@site
                            ,@depot
                            ,@setno
                            ,@ouorder
                            ,@huno
                            ,@accncode";

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                //if (cn != null) { cn.Dispose(); } sqlval = null; sqlins = null; sqlupd = null; sqlrem = null; sqlfnd = null; tbn = null; sqlmcom = null;

                prep_opspick_stp1 = null;
                prep_opspick_stp2 = null;
                prep_opscancel_stp1 = null;

            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }
}