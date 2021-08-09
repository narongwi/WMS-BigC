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

namespace Snaps.WMS
{

    public partial class outbound_ops : IDisposable
    {
        public static string outbound_tbn = "wm_outbound";
        public static string outbound_force = " and orgcode = @orgcode and site = @site  and depot = @depot  and spcarea = @spcarea  and ouorder = @ouorder ";
        public string pagerowlimit = "select isnull((select top 1 bnvalue from wm_binary wb where orgcode = @orgcode and site = @site and depot = @depot and bntype ='DATAGRID' and bncode ='PAGEROWLIMIT'),200)";
        private String outbound_sqlins = string.Concat("insert into ", outbound_tbn, " ( orgcode, site, depot, spcarea, ouorder, outype, ousubtype, thcode, dateorder, dateprep, " +
            " dateexpire, oupriority, ouflag, oupromo, orbitsource, dropship, stocode, stoname, stoaddressln1, stoaddressln2, stoaddressln3, stosubdistict, stodistrict, " +
            " stocity, stocountry, stopostcode, stomobile, stoemail, datereqdel, dateprocess, datedelivery, qtyorder, qtypnd, ouremarks, tflow, datecreate,  " +
            " accncreate, datemodify, accnmodify, procmodify ) " +
            " values " +
            " ( @orgcode, @site, @depot, @spcarea, @ouorder, @outype, @ousubtype, @thcode, @dateorder, @dateprep, @dateexpire, @oupriority, @ouflag, " +
            " @oupromo, @orbitsource, @dropship, @stocode, @stoname, @stoaddressln1, @stoaddressln2, @stoaddressln3, @stosubdistict, @stodistrict, " +
            " @stocity, @stocountry, @stopostcode, @stomobile, @stoemail, @datereqdel, @dateprocess, @datedelivery, @qtyorder, @qtypnd, @ouremarks, " +
            " @tflow, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify) ");

        private String outbound_sqlupd = string.Concat(" update ", outbound_tbn, " set " +
            " outype = @outype, ousubtype = @ousubtype, thcode = @thcode, dateorder = @dateorder, dateprep = @dateprep, dateexpire = @dateexpire, " +
            " oupriority = @oupriority, ouflag = @ouflag, oupromo = @oupromo, orbitsource = @orbitsource, dropship = @dropship, stocode = @stocode, " +
            " stoname = @stoname, stoaddressln1 = @stoaddressln1, stoaddressln2 = @stoaddressln2, stoaddressln3 = @stoaddressln3, " +
            " stosubdistict = @stosubdistict, stodistrict = @stodistrict, stocity = @stocity, stocountry = @stocountry, stopostcode = @stopostcode, " +
            " stomobile = @stomobile, stoemail = @stoemail, datereqdel = @datereqdel, dateprocess = @dateprocess, datedelivery = @datedelivery, " +
            " qtyorder = @qtyorder, qtypnd = @qtypnd, ouremarks = @ouremarks, tflow = @tflow,   " +
            " datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify" +
            " where 1=1 ", outbound_force);

        private String outbound_sqlinx = string.Concat("insert into ix", outbound_force,
            " ( orgcode, site, depot, spcarea, ouorder, outype, ousubtype, thcode, dateorder, dateprep, dateexpire, oupriority, ouflag, oupromo, orbitsource, dropship, " +
            " stocode, stoname, stoaddressln1, stoaddressln2, stoaddressln3, stosubdistict, stodistrict, stocity, stocountry, stopostcode, stomobile, " +
            " stoemail, fileid, rowops, tflow, ermsg, opsdate ) " +
            " values " +
            " ( @orgcode, @site, @depot, @spcarea, @ouorder, @outype, @ousubtype, @thcode, @dateorder, @dateprep, @dateexpire, @oupriority, @ouflag, @oupromo, @orbitsource, " +
            " @dropship, @stocode, @stoname, @stoaddressln1, @stoaddressln2, @stoaddressln3, @stosubdistict, @stodistrict, @stocity, @stocountry, @stopostcode, " +
            " @stomobile, @stoemail, @fileid, @rowops, @tflow, @ermsg, @opsdate )");


        private String outbound_sqlrem = string.Concat("delete from ", outbound_tbn, " where 1=1 ", outbound_force);

        private String outbound_sqlfnd = "" +
        " select top(isnull(@rowlimit,200)) o.*,p.thnameint thname, '' disinbound, '' disproduct, '' disproductdsc,                                                                              " +
        "        case when o.tflow not in ('IO') and o.datereqdel > GETDATE()-1 then FORMATMESSAGE(bndescalt,dbo.dsc_dateshort(o.datemodify,SYSDATETIMEOFFSET()),'')  " +
        "             else bndesc end dateremarks,'' disunitops ,'' dishuno ,0 disstockid,null disloccode,0 dislv, 0 dispv, inorder                                                                                         " +
        "   from wm_outbound o,wm_thparty p,                                                                                                                          " +
        "        ( select orgcode, site, depot, bnvalue, bndesc, bndescalt from wm_binary where bntype = 'OUBORDER' and bncode = 'FLOW' and bnstate = 'IO' ) b        " +
        " where o.orgcode = p.orgcode and o.site = p.site and o.depot = p.depot and o.thcode = p.thcode                                                               " +
        "   and o.orgcode = b.orgcode and o.site = b.site and o.depot = b.depot and o.tflow = b.bnvalue                                                               " +
        "   and o.orgcode = @orgcode and o.site = @site  and o.depot = @depot                                                                                         ";

        private String outbound_sqlvld = string.Concat("select count(1) rsl from", outbound_tbn, " where 1=1 ", outbound_force);


        // private String outbound_sqlfnddist = string.Concat("  select o.*,l.inorder disinbound,l.article disproduct,  t.thnameint thname,p.descalt disproductdsc from wm_outbound o, " + 
        // " wm_thparty t, wm_outbouln l, wm_product p  where o.orgcode = t.orgcode and o.site = t.site and o.depot = t.depot and o.thcode = t.thcode and " + 
        // " o.orgcode = t.orgcode and o.site = t.site and o.depot = t.depot and o.thcode = t.thcode and " + 
        // " o.orgcode = l.orgcode and o.site = l.site and o.depot = l.depot and o.ouorder = l.ouorder and o.spcarea = 'XD' and " + 
        // " l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv ");

        private String outbound_sqlfnddist = "  select *, FORMATMESSAGE('Received in %s ago ',dbo.dsc_dateshort(daterec,SYSDATETIMEOFFSET()),'')  dateremarks " +
        " from ( " +
        " select i.orgcode,i.site,i.depot,i.spcarea,i.inorder ouorder,i.intype outype,i.subtype ousubtype,i.thcode,i.dateorder,i.dateplan dateprep,i.dateexpire " +
        "        ,i.inpriority oupriority,i.inflag ouflag,i.inpromo oupromo,null dropship,i.orbitsource,null stocode,null stoname,null stoaddressln1,null stoaddressln2 , null city " +
        "        ,dateplan datereqdel,null dateprocess,null datedelivery,sum(s.qtysku) qtyorder " +
        "        ,case when p.unitprep = 1 then sum(s.qtysku) when p.unitprep = 2 then sum(s.qtysku) / p.rtoskuofipck when p.unitprep = 3 then sum(s.qtysku) / p.rtoskuofpck " +
        " when p.unitprep = 4 then  sum(s.qtysku) / p.rtoskuoflayer when p.unitprep = 5 then  sum(s.qtysku) / p.rtoskuofhu end qtypnd" +
        "        ,remarkrec ouremarks,i.tflow,i.datecreate " +
        "        ,i.accncreate,i.datemodify,i.accnmodify,i.procmodify,i.inorder disinbound,s.pv dispv,s.lv dislv,s.loccode disloccode " +
        "        ,s.article disproduct,  t.thnameint thname,p.descalt disproductdsc, p.thcode disthcode,p.unitprep disunitops,s.datecreate daterec,s.huno dishuno,stockid disstockid,i.inorder " +
        "    from wm_inbound i, wm_stock s, wm_thparty t, wm_product p " +
        "   where i.spcarea = 'XD' and i.intype = 'XD' and i.subtype = 'XD' and s.spcarea = 'XD' and i.orgcode = s.orgcode and i.site = s.site  " +
        "     and i.depot = s.depot  and i.inorder = s.inrefno and i.orgcode = t.orgcode and i.site = t.site and i.depot = t.depot and i.thcode = t.thcode " +
        "     and s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv " +
        "     and not exists (select 1 from wm_stobc b where s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot  " +
        "     and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv  and b.tflow not in ('CL','ED') )  " +
        " group by i.orgcode,i.site,i.depot,i.spcarea,i.inorder,i.intype,i.subtype,i.thcode,i.dateorder,i.dateplan,i.dateexpire " +
        "         ,i.inpriority,i.inflag,i.inpromo,i.orbitsource,dateplan,remarkrec ,i.tflow,i.datecreate,i.accncreate,i.datemodify " +
        "         ,i.accnmodify,i.procmodify,i.inorder,s.article, t.thnameint,p.descalt, p.thcode,p.unitprep,s.datecreate  " +
        "         ,p.rtoskuofipck, p.rtoskuofpck, p.rtoskuoflayer, p.rtoskuofhu,s.huno,s.stockid,s.pv,s.lv,s.loccode ) o where 1=1  ";
        private string outbound_sqlgetdist = "" +
        " select o.*, 0 qtystock,p.descalt, t.thnameint distthname  " +
        "   from wm_outbouln o join wm_product p on o.orgcode = p.orgcode and o.depot = p.depot and o.site = p.site and o.article = p.article and o.pv = p.pv and o.lv = p.lv  " +
        "   left join wm_thparty t on o.orgcode = t.orgcode and o.depot = t.depot and o.site = t.site and o.disthcode = t.thcode " +
        " where o.orgcode = @orgcode and o.site = @site and o.depot = @depot and qtypndpu > 0  and o.inorder = @inorder and o.article = @article ";

        private String outbound_sqlprcdist = " select o.*,l.lscodealt lscode,s.huno,s.loccode,s.stockid " +
        " from wm_outbouln o join wm_stock s on o.orgcode = s.orgcode and o.site = s.site and o.depot = s.depot and o.article = s.article and o.pv = s.pv and o.lv = s.lv and  o.ouorder = s.inrefno " +
        " left join ( select orgcode,site,depot,spcarea, spcthcode, min(lscodealt) lscodealt  " +
        " from wm_locdw where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'XD' " +
        " and spcthcode is not null and isnull(lsmxhuno,3) > crhu group by orgcode,site,depot, spcthcode,spcarea ) l " +
        " on o.orgcode = l.orgcode and o.spcarea = l.spcarea and o.site = l.site and o.depot = l.depot and o.disthcode = l.spcthcode  " +
        " where o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.spcarea = 'XD' and o.tflow = 'IO' and o.ouorder in ('{0}') ";

        //distinct product for create dist prep
        private String outbound_sqlprcdistproc = " select o.*,s.huno,s.loccode,s.stockid from wm_outbouln o join wm_stock s on o.orgcode = s.orgcode and o.site = s.site and o.depot = s.depot " +
        " and o.article = s.article and o.pv = s.pv and o.lv = s.lv and  o.ouorder = s.inrefno where 1=1 " +
        " and o.orgcode = @orgcode and o.site = @site and o.depot = @depot  and o.ouorder in ('{0}')  ";


        //Line 
        public static string outbouln_tbn = "wm_outbouln";
        public static string outbouln_force = " and orgcode = @orgcode and site = @site  and depot = @depot  and spcarea = @spcarea  and ouorder = @ouorder and ouln = @ouln ";
        private String outbouln_sqlins = string.Concat("insert into ", outbouln_tbn,
        " ( site, depot, spcarea, ouorder, ouln, ourefno, ourefln, inorder, barcode, article, pv, lv, unitops, qtysku, qtypu, " +
        " qtyweight, spcselect, lotno, datemfg, dateexp, serialno, tflow, qtypnd, datedelivery, qtyskudel, qtypudel, qtyweightdel, " +
        " oudnno, datecreate, accncreate, datemodify, accnmodify, procmodify ) " +
        " values " +
        " ( @site, @depot, @spcarea, @ouorder, @ouln, @ourefno, @ourefln, @inorder, @barcode, @article, @pv, @lv, @unitops, @qtysku, " +
        " @qtypu, @qtyweight, @spcselect, @lotno, @datemfg, @dateexp, @serialno, @tflow, @qtypnd, @datedelivery, @qtyskudel, @qtypudel, " +
        " @qtyweightdel, @oudnno, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ");
        private String outbouln_sqlupd = string.Concat(" update ", outbouln_tbn, " set " +
        " ourefno = @ourefno, ourefln = @ourefln, inorder = @inorder, barcode = @barcode, article = @article, pv = @pv, lv = @lv, " +
        " unitops = @unitops, qtysku = @qtysku, qtypu = @qtypu, qtyweight = @qtyweight, spcselect = @spcselect, lotno = @lotno, " +
        " datemfg = @datemfg, dateexp = @dateexp, serialno = @serialno, tflow = @tflow, qtypnd = @qtypnd, datedelivery = @datedelivery, " +
        " qtyskudel = @qtyskudel, qtypudel = @qtypudel, qtyweightdel = @qtyweightdel, oudnno = @oudnno,  " +
        "  datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " +
        " where 1=1 ", outbouln_force);
        private String outbouln_sqlinx = string.Concat("insert into ix" + outbouln_tbn +
        " ( site, depot, spcarea, ouorder, ouln, ourefno, ourefln, inorder, barcode, article, pv, lv, unitops, qtysku, qtypu, qtyweight, " +
        " lotno, expdate, serialno, tflow, fileid, rowops, ermsg, dateops, ) " +
        " values " +
        " ( @site, @depot, @spcarea, @ouorder, @ouln, @ourefno, @ourefln, @inorder, @barcode, @article, @pv, @lv, @unitops, @qtysku, " +
        " @qtypu, @qtyweight, @lotno, @expdate, @serialno, @tflow, @fileid, @rowops, @ermsg, @dateops )");
        private String outbouln_sqlrem = string.Concat("delete from ", outbouln_tbn, " where 1=1 ", outbouln_force);
        private String outbouln_sqlfnd = " select o.*, 0 qtystock,p.descalt, t.thnameint distthname " +
        "   from wm_outbouln o join wm_product p on o.orgcode = p.orgcode and o.depot = p.depot and o.site = p.site and o.article = p.article and o.pv = p.pv and o.lv = p.lv  " +
        "   left join wm_thparty t on o.orgcode = t.orgcode and o.depot = t.depot and o.site = t.site and o.disthcode = t.thcode " +
        " where  o.orgcode = @orgcode and o.site = @site and o.depot = @depot ";
        private String outbouln_sqlval = string.Concat("select count(1) rsl from", outbouln_tbn, " where 1=1 ", outbouln_force);




        //Change request delivery date 
        private string sqloutbound_changedelivery = "" +
        " update wm_outbound set datereqdel = @datereqdel, accnmodify = @accnmodify, datemodify = SYSDATETIMEOFFSET(), procmodify = @procmodify " +
        " where tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot and ouorder = @ouorder                                  ";
        //Set order remarks 
        private string sqloutbound_setremarks = "" +
        " update wm_outbound set ouremarks = @ouremarks, accnmodify = @accnmodify, datemodify = SYSDATETIMEOFFSET(), procmodify = @procmodify " +
        " where tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot and ouorder = @ouorder                                  ";
        //Set order priority
        private String sqloutbound_setpriority = "update wm_outbound set oupriority = case when oupriority = 0 then 30 else 0 end,   " +
        " datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, procmodify = @procmodify where tflow = 'IO' and orgcode = @orgcode " +
        " and site = @site and depot = @depot and ouorder = @ouorder ";
        //revise order line 
        private String sqloutbound_setlinebatch = "update wm_outbouln set qtyreqsku = @qtyreqpu * dbo.get_ratiopu_prep(orgcode,site,depot,article,pv,lv), " +
        " qtyreqpu = @qtyreqpu, dateexp = @dateexp, batchno = @batchno, serialno = @serialno, " +
        " datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, procmodify = @procmodify where tflow = 'IO' and orgcode = @orgcode and site = @site    " +
        " and depot = @depot and ouorder = @ouorder and ouln = @ouln ";

        //get order detail line 
        private string sqloutbound_orderline = "" +
        " select o.*,t.thnamealt thname from wm_outbound o,wm_thparty t where o.orgcode = t.orgcode and o.site = t.site and o.depot = t.depot    " +
        " and o.thcode = t.thcode and o.orgcode = @orgcode and o.site = @site and o.depot = @depot ";

        //Cancel and order 
        private string sqloutbound_ordercancel = "update wm_outbound set tflow = 'XX', ouremarks = @ouremarks, " +
        " datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, procmodify = @procmodify where tflow = 'IO' and orgcode = @orgcode and site = @site    " +
        " and depot = @depot and ouorder = @ouorder ";

        private readonly ISnapsLogger logger;

        private SqlConnection cn = null;
        public outbound_ops() { }
        public outbound_ops(String cx)
        {
            cn = new SqlConnection(cx);
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<outbound_ops>();
        }
        public void setConnection(String cx)
        {
            cn = new SqlConnection(cx);
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing)
        {
            if (!disposedValue)
            {
                if (cn != null) { cn.Dispose(); }
                outbound_tbn = null;
                outbound_force = null;
                outbound_sqlins = null;
                outbound_sqlupd = null;
                outbound_sqlinx = null;
                outbound_sqlrem = null;
                outbound_sqlfnd = null;
                outbound_sqlvld = null;
                outbouln_tbn = null;
                outbouln_force = null;
                outbouln_sqlins = null;
                outbouln_sqlupd = null;
                outbouln_sqlinx = null;
                outbouln_sqlrem = null;
                outbouln_sqlfnd = null;
                outbouln_sqlval = null;
            }
            disposedValue = true;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }



}