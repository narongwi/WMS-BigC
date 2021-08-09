using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using System.Runtime.CompilerServices;

namespace Snaps.WMS {
    public partial class route_ops : IDisposable { 
        private SqlConnection cn = null;

        private String sqlins = "insert into wm_route " +
        " ( orgcode, site, depot, spcarea, routetype, routeno, routename, oupromo, thcode, trttype, loadtype, trucktype, trtmode, loccode, paytype, " + 
        " oupriority, plandate, loaddate, dateshipment, datereqdelivery, relocationto, relocationtask, shipper, mxhu, mxweight, mxvolume, " + 
        " crhu, crweight, crvolume, driver, plateNo, tflow, contactno, utlzcode, datecreate, accncreate, datemodify, accnmodify, procmodify,sealno,transportor ) " + 
        " values "  +
        " ( @orgcode, @site, @depot, @spcarea, @routetype, @routeno, @routename, @oupromo, " + 
        " case when @routetype = 'C' then 'CMB' else @thcode end ," + 
        " @trttype, @loadtype, @trucktype, @trtmode, @loccode , " + 
        " @paytype, @oupriority, @plandate, @loaddate, @dateshipment, @datereqdelivery, @relocationto, @relocationtask, @shipper, @mxhu, @mxweight, " + 
        " @mxvolume, @crhu, @crweight, @crvolume, @driver, @plateNo, @tflow, @contactno, @utlzcode, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify,@sealno,@transportor ) ";
        private String sqlupd = " update wm_route set " + 
        " routetype = @routetype,  routename = @routename, oupromo = @oupromo, thcode = @thcode, trttype = @trttype, loadtype = @loadtype, trucktype = @trucktype, " + 
        " trtmode = @trtmode, loccode = @loccode, paytype = @paytype, oupriority = @oupriority, plandate = @plandate, loaddate = @loaddate, dateshipment = @dateshipment, " + 
        " datereqdelivery = @datereqdelivery, relocationto = @relocationto, relocationtask = @relocationtask, shipper = @shipper, mxhu = @mxhu, mxweight = @mxweight, "+ 
        " mxvolume = @mxvolume, crhu = @crhu, crweight = @crweight, crvolume = @crvolume, driver = @driver, plateNo = @plateNo, tflow = @tflow, contactno = @contactno, "+ 
        " utlzcode = @utlzcode,   datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify,sealno = @sealno, transportor = @transportor " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno "; 

        private string sqlrem_vld = "select count(1) rsl from wm_handerlingunit where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno";
        private String sqlrem = "delete from wm_route where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno "; 
        private String sqlfnd = " select r.*, case when routetype = 'C' then 'Combine route' else t.thnameint end thname " +
        " from wm_route r left join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot " + 
        " and r.thcode = t.thcode where r.orgcode = @orgcode and r.site = @site and r.depot = @depot ";
        private string sqlsumbythcode = " select r.orgcode,r.site, r.depot,case when r.thcode = '' then 'CMB' else r.thcode end thcode ,isnull(t.thnameint,'Combine route') thname,  " + 
        "        count(distinct routeno) crroute, sum(isnull(crhu,0)) crhu, sum(isnull(crweight,0)) crweight, " + 
        "        sum(isnull(crvolume,0)) crvolume,avg(isnull(crcapacity,0))  crcapacity, sum(isnull(crophu,0)) crophu " + 
        "   from wm_route r left join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and r.thcode = t.thcode " + 
        "    where r.orgcode = @orgcode and r.site = @site and r.depot = @depot " + 
        "  group by  r.orgcode,r.site, r.depot,r.thcode,t.thnameint " ;

        private String sqlval = "select count(1) rsl from wm_route where  orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno " ;
        private string sqlfnd_customer = " select thcode,thcode + ' : ' + thname thname from wm_thparty t where exists (select 1 from wm_handerlingunit p "+ 
        " where  t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.thcode = p.thcode and opscode is not null and tflow in ('IO','LD') ) " + 
        " and orgcode = @orgcode and site = @site and depot = @depot order by thcode ";
        private String sqlfnd_transporter = "select thcode,thnameint thname from wm_thparty where thbutype = 'TP' and orgcode = @orgcode and site = @site and depot = @depot order by thnameint ";
        
        private String route_allocate_step1 = "update wm_handerlingunit set routeno = @routeno ,datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno";
        private String route_allocate_step2 = "update wm_prep set routeno = @routeno, datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno";
        private string route_allocate_step3 = "update h set routeno = @routeno, datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify from wm_task h join wm_taln l on h.orgcode = l.orgcode and h.site = l.site" +
            " and h.depot = l.depot and h.taskno = l.taskno where h.orgcode = @orgcode and h.site = @site and h.depot = @depot and l.sourcehuno=@huno and h.tasktype = 'A'";

        private String route_huload_step1 = "update wm_handerlingunit set tflow = case when tflow = 'LD' then 'PE' else 'LD' end,datemodify = @sysdate, accnmodify = @accnmodify, " + 
        " procmodify = @procmodify where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno and tflow in ('PE','LD','IO')";
        private String route_huload_step2 = "update wm_prep set tflow = case when tflow = 'LD' then 'PE' else 'LD' end, datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno and tflow in ('PE','LD')";
        private String route_huload_step3 = "update wm_prln set tflow = case when tflow = 'LD' then 'PE' else 'LD' end, datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno and tflow in ('PE','LD')";
        //shipment 

        // private string route_opship_prepfnd = " select l.*, p.hutype,p.routeno,p.thcode shthcode from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot " +
        // " and p.prepno = l.prepno and p.orgcode = @orgcode and p.site = @site ande p.depot = @depot and p.tflow = 'PE' and p.prepno = @prepno ";
        //private string route_opsship_prepfnd = @" select p.orgcode,p.site,p.depot,p.spcarea, ouorder,ouln,'' ourefno,0 ourefln,0 ouseq,spcorder inorder, barcode, article, pv, 
        //        lv,unitprep unitops,qtyskuops opssku,qtypuops opspu,qtyweightops opsweight,qtyvolumeops opsvolume, batchno, lotno, 
        //        datemfg, dateexp, serialno, p.routeno,prepstockid stockid, l.huno opshuno,hunosource opshusource ,'P' preprtype,l.loccode 
        //from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.tflow = 'LD' 
        //and p.prepno = l.prepno and l.tflow = 'LD' and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.routeno = @routeno 
        //union  
        //select  t.orgcode,t.site, t.depot, t.spcarea, l.ouorder, l.ouln, l.ourefno, l.ourefln,0 ouseq, null inorder, null barcode, article, pv, 
        //        lv, 5 unitprep, targetqty opssku, 1 opspu, 0 opsweight , sourcevolume  opsvolume, l.batchno,l.lotno, 
        //        l.datemfg, l.dateexp, l.serialno, t.routeno, l.stockid, l.sourcehuno opshuno, l.sourcehuno opshusource, 'A' preptype,l.targetloc loccode 
        //from wm_task t, wm_taln l 
        //where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype = 'A' and t.tflow = 'ED' 
        //    and l.tflow = 'ED' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and routeno = @routeno 
        //    and exists (select 1 from wm_handerlingunit h where h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.huno = l.sourcehuno and h.tflow ='LD')
        //union 
        //select p.orgcode,p.site,p.depot,p.spcarea, ouorder,ouln,'' ourefno,0 ourefln,0 ouseq,spcorder inorder, barcode, article, pv, 
        //        lv,unitprep unitops,qtyskuops opssku,qtypuops opspu,qtyweightops opsweight,qtyvolumeops opsvolume, batchno, lotno, 
        //        datemfg, dateexp, serialno, p.routeno,prepstockid stockid, l.huno opshuno,hunosource opshusource ,'X' preprtype,l.loccode 
        //    from wm_handerlingunit p , wm_prln l, wm_prep h 
        //where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.huno = l.huno 
        //    and h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.prepno = l.prepno
        //    and p.spcarea = 'XD' and p.hutype = 'XE' and p.tflow = 'LD' and l.tflow = 'LD'
        //    and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.routeno = @routeno  ";
        private string route_opsship_prepfnd = @"select p.orgcode,p.site,p.depot,p.spcarea, ouorder,ouln,'' ourefno,0 ourefln,0 ouseq,spcorder inorder, barcode, article, pv, 
            lv,unitprep unitops,qtyskuops opssku,qtypuops opspu,qtyweightops opsweight,qtyvolumeops opsvolume, batchno, lotno, 
            datemfg, dateexp, serialno, p.routeno,prepstockid stockid, l.huno opshuno,hunosource opshusource ,'P' preprtype,l.loccode 
    from wm_prep p, wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.tflow = 'LD' 
    and p.prepno = l.prepno and l.tflow = 'LD' and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.routeno = @routeno 
    union  
    select  t.orgcode,t.site, t.depot, t.spcarea, l.ouorder, l.ouln, l.ourefno, l.ourefln,0 ouseq, null inorder, null barcode, l.article, l.pv, 
            l.lv, 5 unitprep, l.targetqty opssku, (l.sourceqty/wp.rtoskuofpck) opspu, (l.sourceqty*wp.skugrossweight) opsweight , l.sourcevolume opsvolume, l.batchno,l.lotno, 
            l.datemfg, l.dateexp, l.serialno, t.routeno, l.stockid, l.sourcehuno opshuno, l.sourcehuno opshusource, 'A' preptype, l.targetloc loccode 
    from wm_task t, wm_taln l ,wm_product wp
    where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and tasktype = 'A' and t.tflow = 'ED' 
        and l.tflow = 'ED' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and routeno = @routeno 
        and l.orgcode = wp.orgcode and l.site  = wp.site  and l.depot = wp.depot and l.article = wp.article and l.pv = wp.pv and l.lv = wp.lv
        and exists (select 1 from wm_handerlingunit h where h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.huno = l.sourcehuno and h.tflow ='LD')
    union 
    select p.orgcode,p.site,p.depot,p.spcarea, ouorder,ouln,'' ourefno,0 ourefln,0 ouseq,spcorder inorder, barcode, article, pv, 
            lv,unitprep unitops,qtyskuops opssku,qtypuops opspu,qtyweightops opsweight,qtyvolumeops opsvolume, batchno, lotno, 
            datemfg, dateexp, serialno, p.routeno,prepstockid stockid, l.huno opshuno,hunosource opshusource ,'X' preprtype,l.loccode 
        from wm_handerlingunit p , wm_prln l, wm_prep h 
    where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.huno = l.huno 
        and h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.prepno = l.prepno
        and p.spcarea = 'XD' and p.hutype = 'XE' and p.tflow = 'LD' and l.tflow = 'LD'
        and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.routeno = @routeno ";


        private string route_opsship_step1_route = " update wm_route  " + 
        "    set dateshipment = sysdatetimeoffset(), shipper = @accncode, datemodify = sysdatetimeoffset(), accnmodify = @accncode, procmodify = 'route.shipment', tflow = 'ED', outrno = @outrno " +
        " where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno and tflow = 'IO' ";
        private string route_opsship_step2_prep = "" +
        " update wm_prep set datemodify = sysdatetimeoffset(), accnmodify = @accncode, procmodify = 'route.shipment', tflow = 'ED' " +
        "  where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno and tflow = 'LD' " ;
        private string route_opsship_step3_history = ""+ 
        " insert into wm_outboulx (orgcode,site,depot,spcarea,ouorder,ouln,ourefno,ourefln,            " + 
        " ouseq,inorder,barcode,article,pv,lv,unitops,opssku,opspu,opsweight,opsvolume,oudono,         " + 
        " oudnno,outrno,batchno,lotno,datemfg,dateexp,serialno,routenodel,stockid,opshuno,opshusource, " + 
        " datedel,datecreate,accncreate,datemodify,accnmodify,loccode) values                          " + 
        " (@orgcode,@site,@depot,@spcarea,@ouorder,@ouln,@ourefno,@ourefln,@ouseq,@inorder,@barcode,   " + 
        " @article,@pv,@lv,@unitops,@opssku,@opspu,@opsweight,@opsvolume,@oudono," + 
        " @oudnno,@outrno,@batchno,@lotno,@datemfg,@dateexp,@serialno,@routenodel,@stockid,@opshuno,   " +
        " @opshusource,@datedel,@datecreate,@accncreate,@datemodify,@accnmodify,@loccode)              " ;

        private string route_opsship_step3_stockmove = @"insert into wm_stockmvhx(orgcode, site, depot, spcarea, stockid, hutype, huno, hunosource, thcode, inrefno, inrefln, loccode, article,
	        pv, lv, qtysku, qtypu, qtyweight, qtyvolume, daterec, batchno, lotno, datemfg,dateexp, serialno, stkremarks, tflow, datecreate, accncreate, 
	        datemodify, accnmodify, procmodify, qtyprep, inagrn, ingrno, unitops)
         select s.orgcode, s.site, s.depot, s.spcarea, s.stockid, s.hutype, s.huno, s.hunosource, s.thcode, s.inrefno, s.inrefln, s.loccode, s.article, 
 	        s.pv, s.lv, s.qtysku, s.qtypu, s.qtyweight, s.qtyvolume, s.daterec, s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.stkremarks, s.tflow, s.datecreate, s.accncreate, 
 	        s.datemodify, s.accnmodify, s.procmodify, s.qtyprep, s.inagrn, s.ingrno, s.unitops 
         from wm_stock s, wm_handerlingunit h where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno 
            and h.routeno = @routeno and h.orgcode = @orgcode and h.site = @site and h.depot = @depot and h.tflow = 'LD' and s.tflow = 'IO'";

        private string route_opsship_step3_stockclear = @"delete s from wm_stock s, wm_handerlingunit h
        where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot and s.huno = h.huno
        and h.routeno = @routeno and h.orgcode = @orgcode and h.site = @site and h.depot = @depot and h.tflow = 'LD' and s.tflow = 'IO'";

        private string route_opsship_step3_cleabloc = "" +
        " delete s from wm_stobc s where orgcode = @orgcode and site = @site and depot = @depot and opstype = 'P' and tflow = 'PE' " + 
        " and exists (select 1 from wm_prep p where p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.prepno = s.opsno and p.routeno = @routeno and tflow in ('LD','ED')) " ;

        private string route_opsship_step4_hu = " update wm_handerlingunit  "+
        "    set loccode = @thcode , datemodify = sysdatetimeoffset(), accnmodify = @accncode, outrno = @outrno, tflow = 'ED' " +
        "  where  orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno and tflow = 'LD' " ;

        private string route_opsship_step5_order = ""+
        " update t set  datedelivery = sysdatetimeoffset(), " +
        "        qtyskudel = qtyskudel+ x.opssku , qtypudel = qtyskudel+ x.opspu , " +
        "        qtyweightdel = qtyweightdel + opsweight, oudnno = x.oudnno, qtypndsku = qtysku - (qtyskudel+ x.opssku), " +
        "        qtypndpu = qtypu - (qtypudel+ x.opspu), qtyreqsku = qtysku - (qtyskudel+ x.opssku), qtyreqpu = qtypu - (qtypudel+ x.opspu), " +
        "        tflow = case when t.qtyskudel + x.opssku = t.qtysku then 'ED' else 'PC' end  " +
        " from wm_outbouln t left join  " +
        "     (select orgcode,site,depot,ouorder,ouln,article,pv,lv, sum(opssku) opssku, sum(opspu) opspu, sum(opsweight) opsweight,  " +
        "             sum(opsvolume) opsvolume, routenodel, oudono,oudnno,outrno  " +
        "  from wm_outboulx where routenodel = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site and depot = @depot " +
        " group by orgcode,site,depot,ouorder,ouln,article,pv,lv, routenodel, oudono,oudnno,outrno ) x  " +
        "    on t.site = x.site and t.depot = x.depot and t.ouorder = x.ouorder and t.ouln = x.ouln and t.article = x.article  " +
        "   and t.pv = x.pv and t.lv = x.lv " +
        " where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.routenodel = @routeno and x.outrno = @outrno  " ;
        private string route_opsship_step5_ordln = @"
         update t set  tflow = 'ED', datedelivery = sysdatetimeoffset()
        from wm_outbound t left join (select distinct orgcode,site,depot,ouorder,routenodel,outrno
        from wm_outboulx where routenodel = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site and depot = @depot ) x 
            on t.site = x.site and t.depot = x.depot and t.ouorder = x.ouorder 
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.routenodel = @routeno and x.outrno = @outrno";

        private string route_opsship_step6_product = "update wm_product set lasdelivery = sysdatetimeoffset(), datemodify = sysdatetimeoffset(), procmodify = 'route.shipment' " + 
        " where article in ( select distinct article from wm_outboulx where routenodel = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site and depot = @depot ) " + 
        " and orgcode = @orgcode and depot = @depot and site = @site ";

        private string route_opsship_step7_orbit = "" +
        " insert into xm_xodelivery ( orgcode,site,depot,dateops,routeops,transportno,thcode,thtype,dropship,ouorder,ouln,ourefno,ourefln " + 
        "        ,oudnno,inorder,ingrno,article,pv,lv,unitops,qtysku,qtypu,qtyweight,qtyvolume,dateexp,datemfg,lotmfg,serialno " + 
        " 	   ,huno,accnops,oupromo,xaction,xcreate,rowid,orbitsite,orbitdepot,orbitsource)  " + 
        " SELECT 	x.orgcode,x.site,x.depot,x.datecreate dateops,routenodel routeops,outrno transportno,o.thcode thcode,'' thtype,null dropship, " + 
        " 		x.ouorder,ouln,ourefno,ourefln ,oudnno,null inorder,null ingrno,article,pv,lv,unitops,opssku qtysku,opspu qtypu, " + 
        " 		opsweight qtyweight,opsvolume qtyvolume,dateexp,datemfg,batchno lotmfg,serialno,opshuno huno,x.accncreate accnops, " + 
        " 		oupromo,'WC' xaction,sysdatetimeoffset() xcreate,next value for seq_xodelivery OVER (ORDER BY stockid) rowid, " +
        "        o.site orbitsite,o.depot orbitdepot,o.orbitsource " + 
        " FROM wm_outboulx x, wm_outbound o " + 
        " where x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder " + 
        "  and not exists (select 1 from xm_xodelivery e where x.orgcode = e.orgcode and x.site = e.site and x.depot = e.depot and x.routenodel = e.routeops  " + 
        "  and x.ouorder = e.ouorder and x.article = e.article and x.pv = e.pv and x.lv = e.lv and x.oudnno = e.oudnno) " + 
        "  and x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.routenodel = @routeno and outrno = @outrno" ;

        private string route_opsship_step7_1_orbitblock =
        @"insert into xm_xoblock (orgcode ,site,depot,spcarea,stockid,hutype,huno,hunosource,thcode,
              inrefno,inrefln,loccode,article,pv,lv,qtysku,qtypu,qtyweight,qtyvolume,daterec,batchno,
              lotno,datemfg,dateexp,serialno,stkremarks,tflow,opstype,xaction,xcreate, xmodify, xmsg,rowid,accnmodify)
        select x.orgcode ,x.site,x.depot ,o.spcarea ,null stockid ,null hutype ,null huno, null hunosource,x.thcode thcode, x.routeops inrefno,
	        x.transportno inrefln,null loccode,x.article,x.pv,x.lv,sum(x.qtysku) qtysku ,sum(x.qtypu) qtypu,sum(x.qtyweight) qtyweight,sum(x.qtyvolume) qtyvolume,
	        null daterec,null batchno,null  lotno,null  datemfg,null dateexp,null serialno,'RTV' stkremarks,'IO' tflow,'U' opstype,'WC' xaction,
	        sysdatetimeoffset() xcreate,null xmodify,null xmsg,next value for seq_oxblock rowid,@accncode accnmodify
        from xm_xodelivery x join wm_outbound o on x.orgcode =o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.routeops = @routeno and x.transportno = @outrno
        and o.outype='DL' and o.ousubtype='DV' group by  x.orgcode ,x.site,x.depot ,o.spcarea ,x.thcode, x.routeops,x.transportno,x.article,x.pv,x.lv";

        private string route_opsship_step8_backustock = @"insert into wm_stockmvhx select * from wm_stock s where
        exists (select 1 from wm_handerlingunit h where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot 
        and s.huno = h.huno and routeno = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site 
        and depot = @depot and tflow = 'ED' )  and qtysku = 0";

        private string route_opsship_step9_clearstock = @"delete s from wm_stock s where
        exists (select 1 from wm_handerlingunit h where s.orgcode = h.orgcode and s.site = h.site and s.depot = h.depot 
        and s.huno = h.huno and routeno = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site 
        and depot = @depot and tflow = 'ED' ) and qtysku = 0";

        private string route_opsship_step10_backuphu = @"insert into wm_handerlingunitlx select * from wm_handerlingunit 
        where routeno = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site 
        and depot = @depot and tflow = 'ED'";

        private string route_opsship_step11_clearhu = @"delete s from wm_handerlingunit s
        where routeno = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site 
        and depot = @depot and tflow = 'ED'";

        private string route_opsship_step12_backupstockzero = @"insert into wm_stockmvhx select * from wm_stock where qtysku = 0 and orgcode = @orgcode and site = @site and depot = @depot";

        private string route_opsship_step12_clearstockzeno = @"delete from wm_stock where qtysku = 0 and orgcode = @orgcode and site = @site and depot = @depot";
    }
}