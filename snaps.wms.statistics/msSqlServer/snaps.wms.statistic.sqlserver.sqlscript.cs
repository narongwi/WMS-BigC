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

    public partial class statisic_ops : IDisposable {

        //Statistic of Inbound order 
        String sqlinbound = " update	wm_inbound set  " +
        "		opsprogress = (select case when sum(qtyskurec) = 0 then 0 else (sum(qtyskurec) * 100) / sum(qtysku) end from wm_inbouln l where wm_inbound.orgcode = l.orgcode and wm_inbound.site = l.site and wm_inbound.depot = l.depot and wm_inbound.inorder = l.inorder),   " + 
        "		opsperform = (select CONVERT(VARCHAR(8), DATEADD(SECOND, DATEDIFF(SECOND,min(daterec), max(daterec)),0), 108) from wm_inboulx l where wm_inbound.orgcode = l.orgcode and wm_inbound.site = l.site and wm_inbound.depot = l.depot and wm_inbound.inorder = l.inorder),   " + 
        "		opsrate = (select case when sum(qtyskurec) = 0 then 0 else DATEDIFF(MINUTE,min(daterec), max(daterec)) / sum(qtyskurec) end from wm_inboulx l where wm_inbound.orgcode = l.orgcode and wm_inbound.site = l.site and wm_inbound.depot = l.depot and wm_inbound.inorder = l.inorder)   " + 
        "       where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder   ";

        //Statistic of Inbound order line 
        String sqlinboundline = " update wm_inbouln set	"+ 
        " qtypnd =  qtysku - (select sum(qtyskurec) from wm_inboulx x where wm_inbouln.orgcode = x.orgcode and wm_inbouln.site = x.site and wm_inbouln.depot = x.depot " + 
        "           and wm_inbouln.spcarea = x.spcarea and wm_inbouln.inorder = x.inorder and wm_inbouln.inln = x.inln and wm_inbouln.article = x.article and wm_inbouln.lv = x.lv and wm_inbouln.pv = x.pv), " + 
        " qtyskurec = (select sum(qtyskurec) from wm_inboulx x where wm_inbouln.orgcode = x.orgcode and wm_inbouln.site = x.site and wm_inbouln.depot = x.depot " + 
        "           and wm_inbouln.spcarea = x.spcarea and wm_inbouln.inorder = x.inorder and wm_inbouln.inln = x.inln and wm_inbouln.article = x.article and wm_inbouln.lv = x.lv and wm_inbouln.pv = x.pv), " +
        " qtypurec = (select sum(qtypurec) from wm_inboulx x where wm_inbouln.orgcode = x.orgcode and wm_inbouln.site = x.site and wm_inbouln.depot = x.depot and wm_inbouln.spcarea = x.spcarea " + 
        "           and wm_inbouln.inorder = x.inorder and wm_inbouln.inln = x.inln and wm_inbouln.article = x.article and wm_inbouln.lv = x.lv and wm_inbouln.pv = x.pv), " +
        " qtyweightrec = (select sum(qtyweightrec) from wm_inboulx x where wm_inbouln.orgcode = x.orgcode and wm_inbouln.site = x.site and wm_inbouln.depot = x.depot "+ 
        "           and wm_inbouln.spcarea = x.spcarea and wm_inbouln.inorder = x.inorder and wm_inbouln.inln = x.inln and wm_inbouln.article = x.article and wm_inbouln.lv = x.lv and wm_inbouln.pv = x.pv), " +
        " qtynaturalloss = (select sum(qtynaturalloss) from wm_inboulx x where wm_inbouln.orgcode = x.orgcode and wm_inbouln.site = x.site and wm_inbouln.depot = x.depot " +
        "           and wm_inbouln.spcarea = x.spcarea and wm_inbouln.inorder = x.inorder and wm_inbouln.inln = x.inln and wm_inbouln.article = x.article and wm_inbouln.lv = x.lv and wm_inbouln.pv = x.pv), " +
        " accnmodify = @accnmodify, datemodify = @sysdate " +
        " where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder and inln = @inln ";

        //Update Stock - Stock on hand on product table 
        //String sqlinboundsoh  = "update	wm_product set cronhand = (select sum(qtysku) rsl "+ 
        //" from wm_stock s where  wm_product.orgcode = s.orgcode and wm_product.site = s.site " + 
        //" and wm_product.depot = s.depot and wm_product.article = s.article and wm_product.pv = s.pv " +
        //" and wm_product.lv = s.lv and tflow = 'IO') " +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ";

        //Update last receipt of product 
        public string sqlinboundproductlas = @"update p set lasrecv = sysdatetimeoffset(), 
                    lasbatchno =  case when isnull(x.batchno,'') ! = '' then x.batchno else lasbatchno end, 
                    lasdatemfg =  case when x.datemfg is not null then x.datemfg else p.lasdatemfg end, 
                    lasdateexp = case when x.dateexp is not null  then x.dateexp else p.lasdateexp end, 
                    lasserialno = case when isnull(x.serialno,'') != '' then x.serialno else p.lasserialno end
        from wm_product p ,wm_inboulx x
        where p.orgcode = x.orgcode and p.site = x.site and p.depot = x.depot and p.article = x.article and p.pv = x.pv and p.lv = x.lv
        and x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.inorder = @inorder and inlx = @inlx";
            
        //Update location statistic 
        // String sqllocation = "  update wm_locdw set " + 
        // " crweight = isnull(( select sum(isnull(qtyweight,0)) from wm_stock s where s.orgcode = wm_locdw.orgcode "+ 
        // " and s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ),0), " + 
        // " crvolume = isnull(( select sum(isnull(qtyvolume,0)) from wm_stock s where s.orgcode = wm_locdw.orgcode " + 
        // " and s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ),0), " + 
        // " crfreepct = 100 - (( select sum(isnull(qtyvolume,0)) from wm_stock s where s.orgcode = wm_locdw.orgcode and " +
        // " s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ) / isnull(lsmxvolume,999999)) * 100, " + 
        // " crhu = isnull(( select count(distinct huno) from wm_stock s where s.orgcode = wm_locdw.orgcode " + 
        // " and s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ),0) " + 
        // " where orgcode = @orgcode and site = @site and depot = @depot and lscode = @loccode " ;

        string sqllocation = @"
        update l set crweight = isnull(qtyweight,0), crvolume = isnull(qtyvolume,0), 
        crfreepct = 100 - isnull((qtyvolume * 100)/isnull(lsmxvolume,999999),0), 
            crhu = isnull(qtyhu,0) 
        from wm_locdw l left join 
            (select orgcode, site ,depot, loccode, isnull(sum(qtyweight),0) qtyweight, isnull(sum(qtyvolume),0) qtyvolume, isnull(count(distinct huno),0) qtyhu
                from wm_stock  where loccode = @loccode and orgcode = @orgcode and site = @site and depot = @depot and qtysku > 0
                group by orgcode, site,depot,loccode ) s
        on l.orgcode = s.orgcode and l.site = s.site and l.depot = s.depot and l.lscode = s.loccode
        where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.lscode = @loccode";

        // 11110801

        //route
        string sqlroute = " update r set crhu = isnull(qtyhu,0),crweight = isnull(qtyweightops,0), crvolume = isnull(qtyvolumeops,0), crcapacity = isnull(( qtyvolumeops / mxvolume  ) * 100,0) ,crophu = isnull(ophu,0)," +
        " datemodify = sysdatetimeoffset(), accnmodify = @accnmodify " +
        "  from wm_route r left join (select o.orgcode, o.site,o.depot, o.routeno,count(distinct o.huno) qtyhu, sum(qtyskuops) qtyskuops, sum(qtyweightops) qtyweightops, sum(qtyvolumeops) qtyvolumeops, " +
        "  count(distinct case when o.tflow  in ('PE','LD','ED','CL') then o.huno else null end) ophu   " +
        "  from wm_prep o, wm_prln l where o.orgcode = l.orgcode and o.site = l.site and o.depot = l.depot and o.prepno = l.prepno and o.routeno = @routeno " +
        " and o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.routeno = @routeno " +
        " group by o.orgcode, o.site,o.depot, o.routeno) p " +
        "   on r.orgcode = p.orgcode and r.site = p.site and r.depot = p.depot and r.routeno = p.routeno " +
        " where r.routeno = @routeno and r.tflow = 'IO' and r.orgcode = @orgcode and r.site = @site and r.depot = @depot ";
        //string sqlroute = @" update r set crhu=p.qtyhu,crweight=p.qtyweightops,crvolume=p.qtyvolumeops,crcapacity=(qtyvolumeops/mxvolume)*100,crophu=ophu,
        //  datemodify = sysdatetimeoffset(),accnmodify = @accnmodify
        //  from wm_route r left join (
        //    select orgcode,site,depot,routeno,count(distinct qtyhu) qtyhu,sum(isnull(qtyweightops,0)) qtyweightops,sum(isnull(qtyvolumeops,0)) qtyvolumeops, 
        //  sum(isnull(qtyvolumeops,0)) as crcapacity, count(distinct ophu) as ophu 
        //    from (select t.orgcode,t.site,t.depot,t.routeno,l.sourcehuno qtyhu, l.sourceqty qtyskuops,(l.sourceqty * p.skugrossweight) qtyweightops,
        //   (l.sourceqty * p.skuvolume) qtyvolumeops,l.sourcehuno ophu
        //  from wm_task t join wm_taln l on t.orgcode = l.orgcode and t.site = l.site and t.depot=l.depot  and t.taskno = l.taskno 
        //  join wm_product p  on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
        //  where t.tasktype = 'A' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.routeno = @routeno
        //  union all
        //  select o.orgcode,o.site,o.depot, o.routeno,o.huno qtyhu,(l.qtyskuorder) qtyskuops,(l.qtyweightorder) qtyweightops,
        //  (l.qtyvolumeorder) qtyvolumeops,(case when o.tflow  in ('PE','LD','ED','CL') then o.huno else null end) ophu   
        //  from wm_prep o join wm_prln l on o.orgcode = l.orgcode and o.site = l.site and o.depot = l.depot and o.prepno = l.prepno
        //  where o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.routeno = @routeno
        // ) a	group by orgcode,site,depot,routeno
        // ) p on r.orgcode = p.orgcode and r.site = p.site and r.depot = p.depot and r.routeno = p.routeno 
        // where r.orgcode = @orgcode and r.site = @site and r.depot = @depot and r.routeno = @routeno and r.tflow = 'IO'";
        //Route forcast 
        string sqlcal_routeforcast = @"update t set crweight = s.crweight, crvolume = s.crvolume,
        crcapacity = (s.crvolume / mxvolume) * 100,datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, crhu = s.crhu 
        from wm_route t, ( select orgcode, site, depot,routeno, sum(crweight) crweight, sum(crvolume) crvolume,count(huno) crhu
        from wm_handerlingunit h where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno
        group by orgcode, site, depot,routeno ) s
        where t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and s.routeno = s.routeno
        and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.routeno =  @routeno";

        //update hu forcast 
        string sqlcal_huforcast =  @"update t set crsku = qtysku, crweight = qtyweight, crvolume = qtyvolume, crcapacity = (crvolume / mxvolume) * 100 
        from wm_handerlingunit t, ( select s.orgcode, s.site, s.depot, s.huno, sum(s.qtysku) qtysku, sum(s.qtysku) * p.skuweight qtyweight, sum(s.qtysku) * p.skuvolume qtyvolume 
        from ( select orgcode, site, depot, huno, qtysku,article,pv,lv from wm_stock where huno = @huno and orgcode = @orgcode and site = @site and depot = @depot
        union all select orgcode, site, depot, huno, rsvsku,article,pv,lv from wm_stobc where huno = @huno and orgcode = @orgcode and site = @site and depot = @depot ) s, 
        wm_product p where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
        group by s.orgcode, s.site, s.depot, s.huno,p.skuweight,p.skuvolume ) s
        where t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.huno = @huno and t.orgcode = @orgcode and t.site = @site and t.depot = @depot";

        public string sqlopsship_location =  "update wm_locdw set " + 
        " crweight = ( select sum(isnull(qtyweight,0)) from wm_stock s where s.orgcode = wm_locdw.orgcode "+ 
        " and s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ), " + 
        " crvolume = ( select sum(isnull(qtyvolume,0)) from wm_stock s where s.orgcode = wm_locdw.orgcode " + 
        " and s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ), " + 
        " crfreepct = 100 - (( select sum(isnull(qtyvolume,0)) from wm_stock s where s.orgcode = wm_locdw.orgcode " + 
        " and s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ) * 100) / lsmxvolume, " + 
        " crhu = ( select count(distinct huno) from wm_stock s where s.orgcode = wm_locdw.orgcode " + 
        " and s.site =  wm_locdw.site and s.depot = wm_locdw.depot  and s.loccode = wm_locdw.lscode ) " + 
        " where orgcode = @orgcode and site = @site and depot = @depot " + 
        " and lscode in (  select distinct loccode from wm_outboulx where routenodel = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site and depot = @depot ) " +
        " and orgcode = @orgcode and depot = @depot and site = @site" ;

        public string sqlopsship_stock_step1 = "delete from wm_productstate where orgcode = @orgcode and site = @site and depot = @depot " + 
        " and article in ( select distinct article from wm_outboulx where routenodel = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site and depot = @depot  ) " +
        " and orgcode = @orgcode and depot = @depot and site = @site";

        // remve  + croverflow 
        public string sqlopsship_stock_step2 = "" +
        " insert into wm_productstate select orgcode,site,depot, barcode,  article, pv, lv, sysdatetimeoffset(), cronhand,  " +
        " 		case when (isnull(cronhand,0) - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) +  " +
        " isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) )) < 0 then 0 else  " +
        "                   isnull(cronhand,0) - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) + " +
        " isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) ) end cravailable,    " +
        " 		crbulknrtn,croverflow, crprep + isnull(crtaskap,0) crprep, crstaging,crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask                     " +
        " from ( select s.orgcode,s.site,s.depot,s.article,s.pv,s.lv,'' barcode,                                                                            " +
        "               isnull(sum(qtysku),0) cronhand, '' cravailable,                                                                                     " +
        "               isnull(sum(case when l.spcarea in ('BL') then qtysku else 0 end),0) crbulknrtn,                                                     " +
        "               isnull(sum(case when l.spcarea in ('OV') then qtysku else 0 end),0) croverflow,                                                     " +
        "               isnull((select sum(rsvsku - qtysku) from wm_stobc i  where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site               " +
        "               and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crprep,                                           " +
        "               isnull(sum(case when l.spcarea in ('RS','DS') then qtysku else 0 end),0) crstaging, " +
        "               isnull(sum(case when l.spcarea = 'RN' then qtysku else 0 end),0) crrtv, " +
        "               isnull(sum(case when l.spcarea = 'SB' then qtysku else 0 end),0) crsinbin, " +
        "               isnull(sum(case when l.spcarea = 'DM' then qtysku else 0 end),0) crdamage, " +
        "               isnull(sum(case when l.tflow in ('XX','IX') or s.tflow = 'XX' then qtysku else 0 end),0) crblock, " +
        "               isnull((select	sum(qtypnd) rsl from wm_inbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site                  " +
        "               				and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crincoming, " +
        "               isnull((select	sum(qtypndsku) rsl from wm_outbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site  " +
        "               				and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crplanship " +
        "               ,isnull(sum(case when l.spcarea = 'XC' then qtysku else 0 end),0) crexchange " +
        "               ,isnull((select sum(sourceqty) rsl from wm_taln t where s.orgcode = t.orgcode and s.site = t.site  " +
        "                           and s.depot = t.depot	and tflow in ('IO','SS','PT') " +
        "                           and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtask " +
        "              ,isnull((select sum(sourceqty) rsl from wm_taln t, wm_task l where s.orgcode = t.orgcode and s.site = t.site  " +
        "                           and s.depot = t.depot and l.tflow in ('IO','SS','PT')  " +
        "                           and t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.tasktype = 'A' " +
        "                           and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtaskap " +
        " from (select * from wm_stock where orgcode = @orgcode and site = @site and depot = @depot " +
        " and article in ( select distinct article from wm_outboulx where routenodel = @routeno and outrno = @outrno and orgcode = @orgcode and site = @site and depot = @depot  )) s  " +
        " left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode " +
        " group by s.orgcode,s.site,s.depot,s.article,s.pv,s.lv ) A ";


        // remove and article = @article and pv = @pv and lv = @lv


        //String sqlstockblock =  " update wm_product set " + 
        //" cronblock = (select isnull(sum(qtysku),0) rsl from wm_stock s, wm_locdw l where s.orgcode = l.orgcode  " + 
        //" and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and  ( l.fltype = 'BL' or s.tflow = 'XX' ) and l.spcarea not in ('DS','RS')     " +
        //" and wm_product.orgcode = s.orgcode and wm_product.site = s.site and wm_product.depot = s.depot and wm_product.article = s.article                     " + 
        //" and wm_product.pv = s.pv and wm_product.lv = s.lv)," + 
        //" cronavai = ()  " +
        //" datemodify = @sysdate, accnmodify = @accnmodify,procmodify = @procmodify where orgcode = @orgcode  " + 
        //" and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ";


        //Calculate on available
        private static string sqlcalonhand = " update wm_productstate set " + 
        " cronhand = isnull((select sum(qtysku) rsl from wm_stock where orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0) " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcalonavai = " update wm_productstate set cravailable  = cronhand - ( crbulknrtn + crprep + crstaging + crrtv + crsinbin + crdamage + crblock + crexchange + crtask) " +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv  ";

        //private static string sqlcalbulknrtn = " update wm_productstate set " + 
        //" crbulknrtn  = isnull((select isnull(sum(case when l.spcarea in ('BL') then qtysku else 0 end),0) rsl from wm_stock where tflow = 'IO' and spcarea = 'BL'      " + 
        //" and orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0) " + 
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcaloverflow = " update wm_productstate set " + 
        //" croverflow  = isnull((select sum(case when l.spcarea in ('OV') then qtysku else 0 end),0) rsl from wm_stock where tflow = 'IO' and spcarea = 'OV'             " + 
        //" and orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0) " + 
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcalprep = " update wm_productstate set " +
        //" crprep = isnull((select sum(rsvsku - qtysku) from wm_stobc i  where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site and s.depot = i.depot          " + 
        //" and s.article = i.article and s.pv = i.pv and s.lv = lv),0) " +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv " ;

        //private static string sqlcalstaging = " update wm_productstate set " + 
        //" crstaging = isnull((select sum(case when l.spcarea in ('RS','DS') then qtysku else 0 end),0) rsl from wm_stock where tflow = 'IO' and spcarea in ('RS','DS')  " + 
        //" and orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0) " + 
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcalrtv = " update wm_productstate set " + 
        //" crrtv = isnull((select sum(case when l.spcarea in ('RN') then qtysku else 0 end),0) rsl from wm_stock where tflow = 'IO' and spcarea in ('RN')                " + 
        //" and orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0) " +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcalsinbin = " update wm_productstate set " + 
        //" crsinbin = isnull((select sum(case when l.spcarea in ('SB') then qtysku else 0 end),0) rsl from wm_stock where tflow = 'IO' and spcarea in ('SB')             " + 
        //" and orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0)" +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcaldamage = " update wm_productstate set " + 
        //" crdamage = isnull((select sum(case when l.spcarea in ('DM') then qtysku else 0 end),0) rsl from wm_stock where tflow = 'IO' and spcarea in ('DM')             " + 
        //" and orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0)" +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcalblock = "" + 
        //" update wm_productstate set  crblock = isnull((select sum(case when l.tflow in ('XX','IX') or s.tflow = 'XX' then qtysku else 0 end) rsl " + 
        //" from wm_stock s left join wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode where    " + 
        //" s.orgcode = @orgcode and s.site = @site and s.depot = @depot  and s.article = @article and s.pv = @pv and s.lv = @lv),0)                " + 
        //" where orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv                          " ;

        //private static string sqlcalincoming = " update wm_productstate set " + 
        //" crincoming = isnull((select	sum(qtypnd) rsl from wm_inbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site and s.depot = i.depot        " + 
        //" and s.article = i.article and s.pv = i.pv and s.lv = lv),0) " + 
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcalplanship = " update wm_productstate set " + 
        //" crplanship = isnull((select	sum(qtypndsku) rsl from wm_outbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site and s.depot = i.depot    " + 
        //" and s.article = i.article and s.pv = i.pv and s.lv = lv),0) " +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcalexchange = " update wm_productstate set " + 
        //" crexchange  = isnull((select isnull(sum(case when l.spcarea in ('XC') then qtysku else 0 end),0) rsl from wm_stock where tflow = 'IO' and spcarea = 'XC'      " + 
        //" and orgcode = @orgcode and site = @site  and depot = @depot and article = @article and pv = @pv and lv = @lv),0) " + 
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //private static string sqlcaltask = " update wm_productstate set " + 
        //" crtask = isnull((select sum(sourceqty) rsl from wm_taln t where s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot                               " + 
        //" and tflow in ('IO','SS','PT') and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) " +
        //" where orgcode = @orgcode and site = @site and depot = @depot and article = @article  and pv = @pv and lv = @lv ";

        //Correction snapshot

        private string sqlcalculate_stock_clear = " delete from wm_productstate where orgcode = @orgcode and site = @site and depot = @depot " + 
        " and article = @article and pv = @pv and lv = @lv ";
        //private string sqlcalculate_step2 = " insert into wm_productstate select orgcode,site,depot, barcode,  article, pv, lv, sysdatetimeoffset(), cronhand,  " + 
        //" 		case when (( isnull(cronhand,0) + isnull(croverflow,0))  - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) +  " + 
        //" isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) )) < 0 then 0 else  " + 
        //"                   ( isnull(cronhand,0) + isnull(croverflow,0))  - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) + " + 
        //" isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) ) end cravailable,    " + 
        //" 		crbulknrtn,croverflow, crprep + isnull(crtaskap,0) crprep, crstaging,crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask                     " + 
        //" from ( select s.orgcode,s.site,s.depot,s.article,s.pv,s.lv,'' barcode,                                                                            " + 
        //"               isnull(sum(qtysku),0) cronhand, '' cravailable,                                                                                     " + 
        //"               isnull(sum(case when l.spcarea in ('BL') then qtysku else 0 end),0) crbulknrtn,                                                     " + 
        //"               isnull(sum(case when l.spcarea in ('OV') then qtysku else 0 end),0) croverflow,                                                     " + 
        //"               isnull((select sum(rsvsku - qtysku) from wm_stobc i  where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site               " + 
        //"               and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crprep,                                           " + 
        //"               isnull(sum(case when l.spcarea in ('RS','DS') then qtysku else 0 end),0) crstaging, " + 
        //"               isnull(sum(case when l.spcarea = 'RN' then qtysku else 0 end),0) crrtv, " + 
        //"               isnull(sum(case when l.spcarea = 'SB' then qtysku else 0 end),0) crsinbin, " + 
        //"               isnull(sum(case when l.spcarea = 'DM' then qtysku else 0 end),0) crdamage, " + 
        //"               isnull(sum(case when l.tflow in ('XX','IX') or s.tflow = 'XX' then qtysku else 0 end),0) crblock, " + 
        //"               isnull((select	sum(qtypnd) rsl from wm_inbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site                  " + 
        //"               				and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crincoming, " + 
        //"               isnull((select	sum(qtypndsku) rsl from wm_outbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site  " + 
        //"               				and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crplanship " + 
        //"               ,isnull(sum(case when l.spcarea = 'XC' then qtysku else 0 end),0) crexchange " + 
        //"               ,isnull((select sum(sourceqty) rsl from wm_taln t where s.orgcode = t.orgcode and s.site = t.site  " + 
        //"                           and s.depot = t.depot	and tflow in ('IO','SS','PT') " + 
        //"                           and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtask " + 
        //@"              ,isnull((select sum(sourceqty) rsl from wm_taln t, wm_task l where s.orgcode = t.orgcode and s.site = t.site 
        //                   and s.depot = t.depot and l.tflow in ('IO','SS','PT') 
        //                   and t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.tasktype = 'A'
        //                   and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtaskap " +
        //" from (select * from wm_stock where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ) s  " + 
        //" left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode " + 
        //" group by s.orgcode,s.site,s.depot,s.article,s.pv,s.lv ) A ";

        // FIX BUG
        public string sqlcalculate_stock_info = " insert into wm_productstate select orgcode,site,depot, barcode,  article, pv, lv, sysdatetimeoffset(), cronhand,  " +
        " 		case when (isnull(cronhand,0) - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) +  " +
        " isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) )) < 0 then 0 else  " +
        "                   isnull(cronhand,0) - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) + " +
        " isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) ) end cravailable,    " +
        " 		crbulknrtn,croverflow, crprep + isnull(crtaskap,0) crprep, crstaging,crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask                     " +
        " from ( select s.orgcode,s.site,s.depot,s.article,s.pv,s.lv,'' barcode,                                                                            " +
        "               isnull(sum(qtysku),0) cronhand, '' cravailable,                                                                                     " +
        "               isnull(sum(case when l.spcarea in ('BL') then qtysku else 0 end),0) crbulknrtn,                                                     " +
        "               isnull(sum(case when l.spcarea in ('OV') then qtysku else 0 end),0) croverflow,                                                     " +
        "               isnull((select sum(rsvsku - qtysku) from wm_stobc i  where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site               " +
        "               and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crprep,                                           " +
        "               isnull(sum(case when l.spcarea in ('RS','DS') then qtysku else 0 end),0) crstaging, " +
        "               isnull(sum(case when l.spcarea = 'RN' then qtysku else 0 end),0) crrtv, " +
        "               isnull(sum(case when l.spcarea = 'SB' then qtysku else 0 end),0) crsinbin, " +
        "               isnull(sum(case when l.spcarea = 'DM' then qtysku else 0 end),0) crdamage, " +
        "               isnull(sum(case when l.tflow in ('XX','IX') or s.tflow = 'XX' then qtysku else 0 end),0) crblock, " +
        "               isnull((select	sum(qtypnd) rsl from wm_inbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site                  " +
        "               				and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crincoming, " +
        "               isnull((select	sum(qtypndsku) rsl from wm_outbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site  " +
        "               				and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crplanship " +
        "               ,isnull(sum(case when l.spcarea = 'XC' then qtysku else 0 end),0) crexchange " +
        "               ,isnull((select sum(sourceqty) rsl from wm_taln t where s.orgcode = t.orgcode and s.site = t.site  " +
        "                           and s.depot = t.depot	and tflow in ('IO','SS','PT') " +
        "                           and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtask " +
        @"              ,isnull((select sum(sourceqty) rsl from wm_taln t, wm_task l where s.orgcode = t.orgcode and s.site = t.site 
                                   and s.depot = t.depot and l.tflow in ('IO','SS','PT') 
                                   and t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.tasktype = 'A'
                                   and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtaskap " +
        " from (select * from wm_stock where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv ) s  " +
        " left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode " +
        " group by s.orgcode,s.site,s.depot,s.article,s.pv,s.lv ) A ";
        //./ FIX BUG

        //Hourly snapshot 
        private string sqlstocksnapshot_step1 = @"insert into wm_productstahx (orgcode, site, depot, barcode, article, pv, lv, stampdate, cronhand, cravailable, crbulknrtn, croverflow, crprep, crstaging, crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask)
                    select orgcode, site, depot, barcode, article, pv, lv, stampdate, cronhand, cravailable, crbulknrtn, croverflow, crprep, crstaging, crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask 
                    from wm_productstate";
        private string sqlstocksnapshot_step2 = " truncate table wm_productstate ";

        // fixed data 
        private string sqlstocksnapshot_step3 = @" insert into wm_productstate(orgcode, site, depot, barcode, article, pv, lv, stampdate, cronhand, cravailable, crbulknrtn, 
         croverflow, crprep, crstaging, crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask)
         select orgcode,site,depot, barcode,  article, pv, lv, sysdatetimeoffset(), cronhand,  
 		        case when (isnull(cronhand,0) - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) +  
         isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) )) < 0 then 0 else  
                           isnull(cronhand,0) - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) + 
         isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) ) end cravailable,    
 		        crbulknrtn,croverflow, crprep + isnull(crtaskap,0) crprep, crstaging,crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask                     
         from ( select s.orgcode,s.site,s.depot,s.article,s.pv,s.lv,'' barcode,                                                                            
                       isnull(sum(qtysku),0) cronhand, '' cravailable,                                                                                     
                       isnull(sum(case when l.spcarea in ('BL') then qtysku else 0 end),0) crbulknrtn,                                                     
                       isnull(sum(case when l.spcarea in ('OV') then qtysku else 0 end),0) croverflow,                                                     
                       isnull((select sum(rsvsku - qtysku) from wm_stobc i  where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site               
                       and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crprep,                                           
                       isnull(sum(case when l.spcarea in ('RS','DS') then qtysku else 0 end),0) crstaging, 
                       isnull(sum(case when l.spcarea = 'RN' then qtysku else 0 end),0) crrtv, 
                       isnull(sum(case when l.spcarea = 'SB' then qtysku else 0 end),0) crsinbin, 
                       isnull(sum(case when l.spcarea = 'DM' then qtysku else 0 end),0) crdamage, 
                       isnull(sum(case when l.tflow in ('XX','IX') or s.tflow = 'XX' then qtysku else 0 end),0) crblock, 
                       isnull((select	sum(qtypnd) rsl from wm_inbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site                  
               				        and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crincoming, 
                       isnull((select	sum(qtypndsku) rsl from wm_outbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site  
               				        and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crplanship 
                       ,isnull(sum(case when l.spcarea = 'XC' then qtysku else 0 end),0) crexchange 
                       ,isnull((select sum(sourceqty) rsl from wm_taln t where s.orgcode = t.orgcode and s.site = t.site  
                                   and s.depot = t.depot	and tflow in ('IO','SS','PT') 
                                   and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtask 
                      ,isnull((select sum(sourceqty) rsl from wm_taln t, wm_task l where s.orgcode = t.orgcode and s.site = t.site 
                                  and s.depot = t.depot and l.tflow in ('IO','SS','PT') 
                                  and t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.tasktype = 'A'
                                  and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtaskap 
         from (select * from wm_stock where orgcode = 'bgc' and site = '91917' and depot = '01' ) s  
         left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode 
         group by s.orgcode,s.site,s.depot,s.article,s.pv,s.lv ) A ";

        //private string sqlstocksnapshot_step3 = @" insert into wm_productstate select orgcode,site,depot, barcode,  article, pv, lv, sysdatetimeoffset(), cronhand,   
        //        case when (( isnull(cronhand,0) /*+ isnull(croverflow,0)*/)  - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) +   
        //isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) )) < 0 then 0 else   
        //                (isnull(cronhand,0))  - ( isnull(crbulknrtn,0) + isnull(crprep,0) + isnull(crstaging,0) + isnull(crrtv,0) +  
        //isnull(crsinbin,0) + isnull(crdamage,0) + isnull(crblock,0) + isnull(crexchange,0)  + isnull(crtaskap,0) ) end cravailable,     
        //        crbulknrtn,croverflow, crprep + isnull(crtaskap,0) crprep, crstaging,crrtv, crsinbin, crdamage, crblock, crincoming, crplanship, crexchange, crtask                      
        //from ( select s.orgcode,s.site,s.depot,s.article,s.pv,s.lv,'' barcode,                                                                             
        //            isnull(sum(qtysku),0) cronhand, '' cravailable,                                                                                      
        //            isnull(sum(case when l.spcarea in ('BL') then qtysku else 0 end),0) crbulknrtn,                                                      
        //            isnull(sum(case when l.spcarea in ('OV') then qtysku else 0 end),0) croverflow,                                                      
        //            isnull((select sum(rsvsku - qtysku) from wm_stobc i  where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site                
        //            and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crprep,                                            
        //            isnull(sum(case when l.spcarea in ('RS','DS') then qtysku else 0 end),0) crstaging,  
        //            isnull(sum(case when l.spcarea = 'RN' then qtysku else 0 end),0) crrtv,  
        //            isnull(sum(case when l.spcarea = 'SB' then qtysku else 0 end),0) crsinbin,  
        //            isnull(sum(case when l.spcarea = 'DM' then qtysku else 0 end),0) crdamage,  
        //            isnull(sum(case when l.tflow in ('XX','IX') or s.tflow = 'XX' then qtysku else 0 end),0) crblock,  
        //            isnull((select	sum(qtypnd) rsl from wm_inbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site                   
        //                            and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crincoming,  
        //            isnull((select	sum(qtypndsku) rsl from wm_outbouln i where tflow = 'IO' and s.orgcode = i.orgcode and s.site = i.site   
        //                            and s.depot = i.depot and s.article = i.article and s.pv = i.pv and s.lv = lv),0) crplanship  
        //            ,isnull(sum(case when l.spcarea = 'XC' then qtysku else 0 end),0) crexchange  
        //            ,isnull((select sum(sourceqty) rsl from wm_taln t where s.orgcode = t.orgcode and s.site = t.site   
        //                        and s.depot = t.depot	and tflow in ('IO','SS','PT')  
        //                        and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtask  
        //            ,isnull((select sum(sourceqty) rsl from wm_taln t, wm_task l where s.orgcode = t.orgcode and s.site = t.site 
        //            and s.depot = t.depot and l.tflow in ('IO','SS','PT') 
        //            and t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and l.tasktype = 'A'
        //            and s.article = t.article and s.pv = t.pv and s.lv = t.lv),0) crtaskap 
        //from ( select * from wm_stock ) s   
        //left join  wm_locdw l on s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode  
        //group by s.orgcode,s.site,s.depot,s.article,s.pv,s.lv ) A "; 

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                sqlinbound = null;
                sqlinboundline = null;
                //sqlinboundsoh = null;
                _cnx = null;
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}