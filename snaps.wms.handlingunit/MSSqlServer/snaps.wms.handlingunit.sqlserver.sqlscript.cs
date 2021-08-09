using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text;

namespace Snaps.WMS
{
    public partial class handerlingunit_ops : IDisposable
    {
        public static string hu_tbn = "wm_handerlingunit";
        public static string hu_force = " and orgcode = @orgcode and site = @site  and depot = @depot  and spcarea = @spcarea ";
        public string hu_sqlins_stp1 = String.Format("INSERT INTO {0} (orgcode,site,depot,spcarea,thcode,hutype,huno,loccode,routeno,mxsku,mxweight, mxvolume, crsku, " + 
          " crweight,crvolume,crcapacity,tflow,datecreate,accncreate,datemodify,accnmodify, procmodify, priority) values "+
          " (@orgcode, @site, @depot,@spcarea,@thcode,@hutype,@huno,@loccode,@routeno,@mxsku,@mxweight,@mxvolume, " +
          "  @crsku,@crweight,@crvolume,@crcapacity,@tflow,@sysdate,@accncreate,@sysdate,@accnmodify,@procmodify, @priority ) ", hu_tbn);
        public string hu_sqlupd_stp1 = "update wm_handerlingunit set tflow = @tflow, datemodify = @sysdate, accnmodify = @accnmodify, prodmodify = @prodmodify " + 
          " where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno ";
        public string hu_sqlrem_stp1 = string.Format("delete from {0} where 1=1 {1}", hu_tbn, hu_force, " and huno = @huno " );
        public string hu_sqlfnd = " select *,t.thnameint from wm_handerlingunit h left join wm_thparty t on h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.thcode = t.thcode where h.orgcode = @orgcode and h.site = @site and h.depot = @depot ";

        public string hu_sqlget =@" select *,t.thnameint from wm_handerlingunit h left join wm_thparty t on h.orgcode = t.orgcode and h.site = t.site 
        and h.depot = t.depot and h.thcode = t.thcode where h.orgcode = @orgcode and h.site = @site and h.depot = @depot and h.huno = @huno ";
        public string hu_getmaster = "select huno,p.descalt from wm_handerlingunit h, wm_product p where h.hutype = 'MS' and h.orgcode = p.orgcode and h.site = p.site and h.depot = p.depot and h.huno = p.article " +
            "and h.orgcode = @orgcode and h.site = @site and h.depot = @depot";

        // public string hu_sqlline = "select t.taskno prepno,'' inorder, iorefno ouorder, s.article,s.pv,s.lv, qtysku, qtypu,qtyweight,qtyvolume, p.descalt," +
        // " s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, unitops unitprep,loccode " +
        // " from wm_stock s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv " +
        // " join (select l.* from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A' "+ 
        // " and t.orgcode = @orgcode and t.depot = @depot and t.site = @site and l.sourcehuno = @huno) t " + 
        // " on s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and s.article = t.article and s.pv = t.pv and s.lv = t.lv and s.huno = t.sourcehuno  " +
        // " where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno " +
        // " union all  " +
        // " select s.prepno,h.spcorder inorder,s.ouorder, s.article,s.pv,s.lv, qtyskuops, qtypuops, qtyweightops, qtyvolumeops,p.descalt, " + 
        // " 		s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.unitprep,loccode " +
        // "   from wm_prep h, wm_prln s,wm_product p " +
        // "  where h.orgcode = s.orgcode and h.site = s.site and h.depot = s.depot and h.spcarea = s.spcarea and h.prepno = s.prepno " +
        // "  and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv  " +
        // "  and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno ";
        public string hu_sqlline_nonsum = "select t.taskno prepno,'' inorder, iorefno ouorder, s.article,s.pv,s.lv, qtysku, qtypu,qtyweight,qtyvolume, p.descalt," +
        " s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, unitops unitprep,loccode " +
        " from wm_stock s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv " +
        " join (select l.* from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A' "+ 
        " and t.orgcode = @orgcode and t.depot = @depot and t.site = @site and l.sourcehuno = @huno) t " + 
        " on s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and s.article = t.article and s.pv = t.pv and s.lv = t.lv and s.huno = t.sourcehuno  " +
        " where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno " +
        " union all  " +
        " select s.prepno,h.spcorder inorder,s.ouorder, s.article,s.pv,s.lv, qtyskuops, qtypuops, qtyweightops, qtyvolumeops,p.descalt, " + 
        " 		s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.unitprep,loccode " +
        "   from wm_prep h, wm_prln s,wm_product p " +
        "  where h.orgcode = s.orgcode and h.site = s.site and h.depot = s.depot and h.spcarea = s.spcarea and h.prepno = s.prepno " +
        "  and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv  " +
        "  and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno ";
        public string hu_sqlline =  @" select '' prepno,''inorder, iorefno ouorder, s.article,s.pv,s.lv, sum(qtysku) qtysku, sum(qtypu) qtypu, sum(qtyweight) qtyweight, sum(qtyvolume) qtyvolume, p.descalt,
            s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, unitops unitprep,loccode 
        from wm_stock s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv 
        join (select l.* from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A' 
        and t.orgcode = @orgcode and t.depot = @depot and t.site = @site and l.sourcehuno = @huno) t 
        on s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and s.article = t.article and s.pv = t.pv and s.lv = t.lv and s.huno = t.sourcehuno 
        where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno 
        group by iorefno , s.article,s.pv,s.lv, p.descalt, s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, unitops,loccode 
        union all 
        select '' prepno,''inorder, s.ouorder, s.article,s.pv,s.lv, sum(qtyskuops) qtyskuops, sum(qtypuops) qtypuops, sum(qtyweightops) qtyweightops, sum(qtyvolumeops) qtyvolumeops,p.descalt, 
                s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.unitprep,loccode 
        from wm_prep h, wm_prln s,wm_product p 
        where h.orgcode = s.orgcode and h.site = s.site and h.depot = s.depot and h.spcarea = s.spcarea and h.prepno = s.prepno 
        and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv 
        and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno
        group by s.ouorder, s.article,s.pv,s.lv,p.descalt, s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.unitprep,loccode  ";

        // public string hu_sqllinehx = "select t.taskno prepno,'' inorder, iorefno ouorder, s.article,s.pv,s.lv, qtysku, qtypu,qtyweight,qtyvolume, p.descalt," +
        // " s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, unitops unitprep,loccode " +
        // " from wm_stockmvhx s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv " +
        // " join (select l.* from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A' "+ 
        // " and t.orgcode = @orgcode and t.depot = @depot and t.site = @site and l.sourcehuno = @huno) t " + 
        // " on s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and s.article = t.article and s.pv = t.pv and s.lv = t.lv and s.huno = t.sourcehuno  " +
        // " where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno " +
        // " union all  " +
        // " select s.prepno,h.spcorder inorder,s.ouorder, s.article,s.pv,s.lv, qtyskuops, qtypuops, qtyweightops, qtyvolumeops,p.descalt, " + 
        // " 		s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.unitprep,loccode " +
        // "   from wm_prep h, wm_prln s,wm_product p " +
        // "  where h.orgcode = s.orgcode and h.site = s.site and h.depot = s.depot and h.spcarea = s.spcarea and h.prepno = s.prepno " +
        // "  and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv  " +
        // "  and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno ";
        public string hu_sqllinehx =  @" select '' prepno,''inorder, iorefno ouorder, s.article,s.pv,s.lv, sum(qtysku) qtysku, sum(qtypu) qtypu, sum(qtyweight) qtyweight, sum(qtyvolume) qtyvolume, p.descalt,
            s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, unitops unitprep,loccode 
        from wm_stockmvhx s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv 
        join (select l.* from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A' 
        and t.orgcode = @orgcode and t.depot = @depot and t.site = @site and l.sourcehuno = @huno) t 
        on s.orgcode = t.orgcode and s.site = t.site and s.depot = t.depot and s.article = t.article and s.pv = t.pv and s.lv = t.lv and s.huno = t.sourcehuno 
        where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno 
        group by iorefno , s.article,s.pv,s.lv, p.descalt, s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, unitops,loccode 
        union all 
        select '' prepno,''inorder,  s.ouorder, s.article,s.pv,s.lv, sum(qtyskuops) qtyskuops, sum(qtypuops) qtypuops, sum(qtyweightops) qtyweightops, sum(qtyvolumeops) qtyvolumeops,p.descalt, 
                s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.unitprep,loccode 
        from wm_prep h, wm_prln s,wm_product p 
        where h.orgcode = s.orgcode and h.site = s.site and h.depot = s.depot and h.spcarea = s.spcarea and h.prepno = s.prepno 
        and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv 
        and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno
        group by s.ouorder, s.article,s.pv,s.lv,p.descalt, s.batchno, s.lotno, s.datemfg, s.dateexp, s.serialno, s.unitprep,loccode  ";
        public string hu_seq = "  select '{0}'+RIGHT('000' + convert(varchar(18),NEXT VALUE FOR seq_huno), 8) rsl";

        public string hu_sql_close = " update wm_handerlingunit set tflow = 'PE', accnmodify = @accnmodify, datemodify = @sysdate where orgcode = @orgcode " + 
        " and site = @site and depot = @depot and spcarea = @spcarea and huno = @huno and tflow = 'IO'";

        public string hu_sql_calhu = @"
        update t set crsku = p.qtyskuops, crpu = p.qtypuops, crweight = p.qtyweightops, crvolume = p.qtyvolumeops , tflow = 'PE', datemodify = sysdatetimeoffset(), 
        accnmodify = @accnmodify, procmodify = 'prepdist.end', crcapacity = 100 - ((p.qtyvolumeops / mxvolume) * 100) 
        from wm_handerlingunit t, 
            (select orgcode,site,depot, huno, sum(p.qtysku) qtyskuops, sum(p.qtypu) qtypuops, sum(p.qtyweight) qtyweightops,sum(p.qtyvolume) qtyvolumeops  
                from wm_stock p where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno
            group by orgcode,site,depot, huno ) p 
        where t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.huno = p.huno and t.huno = p.huno and t.tflow in ('IO','PE','LD') 
        and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.huno = @huno";

        public string hu_sql_calroute = @" update t set crhu = h.crhu, crweight = h.crweight, crvolume = h.crvolume, procmodify = 'prepdist.end'
        from wm_route t ,
            ( select orgcode, site, depot , routeno , count(distinct huno) crhu, sum(crpu) crpu, sum(crweight) crweight ,sum(crvolume) crvolume
                from wm_handerlingunit where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno and tflow in ('LD','IO','PE') 
                group by orgcode, site, depot , routeno 
            ) h 
        where t.orgcode = h.orgcode and t.site = h.site and t.depot = h.depot and t.routeno = h.routeno and t.tflow = 'IO' 
        and t.orgcode = @orgcode and t.site = @site and t.depot = @depot ";

        private string sqlhu_lov = "select article,descalt from wm_product where articletype = 'P' and tflow = 'IO' and orgcode = @orgcode and depot = @depot and site = @site order by descalt ";
        private SqlConnection cn = null;
        public handerlingunit_ops() { }
        public handerlingunit_ops(String cx) { cn = new SqlConnection(cx);  }
        public handerlingunit_ops(SqlConnection ocn) { cn = ocn; }

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing)
        {
            if (!disposedValue)
            {
                if (cn != null) { cn.Dispose(); }
                hu_tbn = null;
                hu_force = null;
                hu_sqlins_stp1 = null;
                hu_sqlupd_stp1 = null;
                hu_sqlrem_stp1 = null;
                hu_sqlfnd = null;
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}
