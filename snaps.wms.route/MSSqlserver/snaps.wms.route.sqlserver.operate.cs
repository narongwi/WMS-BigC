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
using Snaps.WMS;
using Snaps.Helpers.Logger;

namespace Snaps.WMS {

    public partial class route_ops : IDisposable {
        private readonly ISnapsLogger logger;
        public route_ops() { }
        public route_ops(String cx) {
            cn = new SqlConnection(cx);

            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<route_ops>();
        }
        public async Task<List<route_thsum>> thsum(route_pm rs) {
            SqlCommand cm = null;
            List<route_thsum> rn = new List<route_thsum>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm = (sqlsumbythcode).snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                if(rs.thcode.notNull()) { cm.snapsCdn(rs.thcode,"thcode",string.Format(" and ( r.thcode like '%{0}%' or t.thnameint like '%{0}%' or t.thname like '%{0}%' ) ",rs.thcode.ClearReg())); }
                cm.CommandText += " order by r.thcode";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillthsum(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<List<route_ls>> find(route_pm rs) {
            SqlCommand cm = null;
            List<route_ls> rn = new List<route_ls>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm = (sqlfnd).snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");

                if(rs.routeno.notNull()) { cm.snapsCdn(rs.routeno,"routeno",string.Format(" and r.routeno like '%{0}%'",rs.routeno.ClearReg())); }
                if(!rs.plandate.Equals(null)) {
                    cm.snapsCdn(rs.plandate,"plandate"," and cast(r.plandate as date) = cast(@plandate as date)");
                }
                if(!rs.datereqfrom.Equals(null) && !rs.datereqto.Equals(null)) {
                    cm.snapsPar(rs.datereqto,"datereqto");
                    cm.snapsCdn(rs.datereqfrom,"datereqfrom"," and cast(r.datereqdelivery as date) between cast(@datereqfrom as date) and cast(@datereqto as date) ");
                } else if(!rs.datereqfrom.Equals(null) && rs.datereqto.Equals(null)) {
                    cm.snapsCdn(rs.datereqfrom,"datereqfrom"," and cast(r.datereqdelivery as date) = cast(@datereqfrom as date) ");
                } else if(!rs.datereqto.Equals(null) && rs.datereqfrom.Equals(null)) {
                    cm.snapsCdn(rs.datereqto,"datereqto"," and cast(r.datereqdelivery as date) <= cast( @datereqto as date) ");
                }

                if(rs.thcode.notNull()) {
                    cm.snapsCdn(rs.thcode,"thcode",string.Format(" and r.thcode like '%{0}%' ",rs.thcode.ClearEmail()));
                }
                if(rs.oupromo.notNull()) { cm.snapsCdn(rs.oupromo,"oupromo",string.Format(" and r.oupromo like '{0}'",rs.oupromo.ClearReg())); }
                if(rs.remarks.notNull()) { cm.snapsCdn(rs.remarks,"remarks",string.Format(" and r.remarks like '%{0}%'",rs.remarks.ClearReg())); }
                cm.snapsCdn(rs.routetype,"routetype"," and r.routetype = @routetype ");
                cm.snapsCdn(rs.trttype,"trttype"," and trtmcode = @trttype");
                cm.snapsCdn(rs.transportor,"transportor"," and transportor = @transportor");
                cm.snapsCdn(rs.loadtype,"loadtype"," and r.loadtype = @loadtype ");
                cm.snapsCdn(rs.paymenttype,"paymenttype"," and r.payment = @paymenttype");
                cm.snapsCdn(rs.trucktype,"trucktype"," and r.trucktype = @trucktype");
                cm.snapsCdn(rs.staging,"stating"," and r.loccode = @staging");
                cm.snapsCdn(rs.tflow,"tflow"," and r.tflow = @tflow");

                if(rs.iscombine.isNull()) {
                    cm.CommandText = @" select distinct * from ( " + cm.CommandText + @"
                    union all 
                    select r.*, case when routetype = 'C' then 'Combine route' else t.thnameint end thname 
                    from wm_route r left join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot  
                    and r.thcode = t.thcode where r.orgcode = @orgcode and r.site = @site and r.depot = @depot and routetype = 'C'  ) r ";
                }
                cm.CommandText += " order by r.oupriority, r.routeno";

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<route_md> get(route_ls rs) {
            SqlCommand cm = null; SqlDataReader r = null;
            route_md rn = new route_md();
            String sqlpam = "";
            try {
                /* Vlidate parameter */
                cm = (sqlfnd).snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn(rs.routeno,"routeno"," and r.routeno = @routeno ");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync();

                rn.hus = new List<route_hu>();
                //cm.CommandText = " select huno,prepno, tflow from wm_prep where preptype != 'XE' and orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno";
                if(rn.tflow == "ED") {
                    cm.snapsPar(rn.outrno,"outrno");
                    cm.CommandText = @"select orgcode,site,depot,spcarea,huno,loccode,routeno, crsku, crweight,crvolume, crcapacity,tflow,priority,opscode, case when spcarea = 'XD' then 'X' else opstype end opstype 
                    from wm_handerlingunitlx h where 1=1 and orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno and outrno = @outrno order by huno";
                } else {
                    //(+) add exists and t.taskno = h.opscode
                    cm.CommandText = @"select orgcode,site,depot,spcarea,huno,loccode,routeno, crsku, crweight,crvolume, crcapacity,tflow,priority,opscode, opstype 
                    from wm_handerlingunit h where 1=1
                    and (
                        exists (select 1 from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A' and t.tflow <> 'CL'
                            and l.orgcode = h.orgcode and l.site = h.site and l.depot = h.depot and l.sourcehuno = h.huno and t.taskno = h.opscode) 
                     or exists (select 1 from wm_prep p where p.orgcode = h.orgcode and p.site = h.site and p.depot = h.depot and p.huno = h.huno) )
                    and orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno 
                    union select orgcode,site,depot,spcarea,huno,loccode,routeno, crsku, crweight,crvolume, crcapacity,tflow,priority,opscode,'X' opstype 
                    from wm_handerlingunit h where 1=1 and spcarea = 'XD' and orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno 
                    order by huno";
                }

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) {
                    rn.hus.Add(new route_hu() {
                        huno = r["huno"].ToString(),
                        loccode = r["loccode"].ToString(),
                        tflow = r["tflow"].ToString(),
                        crsku = r.GetInt32(7),
                        crweight = r.GetDecimal(8),
                        crvolume = r.GetDecimal(9),
                        crcapacity = r.GetDecimal(10),
                        priority = r.GetInt32(12),
                        worker = "",
                        opstype = r["opstype"].ToString(),
                        opscode = r["opscode"].ToString(),
                        spcarea = r["spcarea"].ToString()
                    });
                }
                await r.CloseAsync();

                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } sqlpam = null; if(r != null) { await r.DisposeAsync(); } }
        }

        public async Task<List<Snaps.WMS.lov>> getThirdparty(String orgcode,String site,String depot) {
            SqlCommand cm = new SqlCommand(sqlfnd_customer,cn);
            List<lov> rn = new List<lov>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) {
                    rn.Add(new lov(r["thcode"].ToString(),r["thname"].ToString()));
                }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(orgcode,site,depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }

        public async Task<List<Snaps.WMS.lov>> getTransporter(String orgcode,String site,String depot) {
            SqlCommand cm = new SqlCommand(sqlfnd_transporter,cn);
            List<lov> rn = new List<lov>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) {
                    rn.Add(new lov(r["thcode"].ToString(),r["thname"].ToString()));
                }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(orgcode,site,depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }


        public async Task upsert(List<route_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            statisic_ops so = new statisic_ops();
            Int32 ix = 0;
            try {
                foreach(route_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval));
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd;
                    cm.Add(so.routeforcast(ln.orgcode,ln.site,ln.depot,ln.routeno,ln.accnmodify));
                }

                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        public async Task upsert(route_md rs) {
            List<route_md> ro = new List<route_md>();
            try {
                ro.Add(rs); await upsert(ro);
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<route_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {

                foreach(route_md ln in rs) {
                    cm.Add(obcommand(ln,sqlrem));
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task remove(route_md rs) {
            SqlCommand vcm = new SqlCommand(sqlrem_vld,cn);
            List<route_md> ro = new List<route_md>();
            try {
                vcm.snapsPar(rs.orgcode,"orgcode");
                vcm.snapsPar(rs.site,"site");
                vcm.snapsPar(rs.depot,"depot");
                vcm.snapsPar(rs.routeno,"routeno");

                if(vcm.snapsScalarStrAsync().Result != "0") {
                    throw new Exception("Route still using");
                } else {
                    ro.Add(rs); await remove(ro);
                }
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<route_ix> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            Int32 ix = 0;
            try {
                foreach(route_ix ln in rs) {
                    cm.Add(ixcommand(ln,sqlval));
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd;
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.depot,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        public async Task<List<Snaps.WMS.lov>> getstaging(String orgcode,String site,String depot) {
            SqlCommand cm = null;
            List<lov> rn = new List<lov>();
            SqlDataReader r = null;
            String sqlfnd_slc = "select lscodealt,lscodealt + ' free : ' + convert(varchar(10) ,(lsmxhuno - isnull(crhu,0))) lscodedsc " +
            " from wm_locdw where tflow = 'IO' and spcarea = 'DS'";

            try {
                /* Vlidate parameter */
                cm = (sqlfnd_slc).snapsCommand(cn);
                cm.snapsCdn(orgcode,"orgcode");
                cm.snapsCdn(site,"site");
                cm.snapsCdn(depot,"depot");
                cm.CommandText += " order by spcseqpath ";

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscodealt"].ToString(),r["lscodedsc"].ToString())); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(orgcode,site,depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task allocate(route_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            statisic_ops so = new statisic_ops();
            try {
                foreach(route_hu hu in o.allocate) {
                    // handering unit 
                    cm.Add(route_allocate_step1.snapsCommand(cn));
                    cm.Last().snapsPar(o.routeno,"routeno");
                    cm.Last().snapsPar(o.accnmodify,"accnmodify");
                    cm.Last().snapsPar(o.procmodify,"procmodify");
                    cm.Last().snapsPar(o.orgcode,"orgcode");
                    cm.Last().snapsPar(o.site,"site");
                    cm.Last().snapsPar(o.depot,"depot");
                    cm.Last().snapsPar(hu.huno,"huno");
                    cm.Last().snapsParsysdateoffset();
                    // preperation
                    cm.Add(route_allocate_step2.snapsCommand(cn));
                    cm.Last().snapsPar(o.routeno,"routeno");
                    cm.Last().snapsPar(o.accnmodify,"accnmodify");
                    cm.Last().snapsPar(o.procmodify,"procmodify");
                    cm.Last().snapsPar(o.orgcode,"orgcode");
                    cm.Last().snapsPar(o.site,"site");
                    cm.Last().snapsPar(o.depot,"depot");
                    cm.Last().snapsPar(hu.huno,"huno");
                    cm.Last().snapsParsysdateoffset();
                    // full pallet task
                    cm.Add(route_allocate_step3.snapsCommand(cn));
                    cm.Last().snapsPar(o.routeno,"routeno");
                    cm.Last().snapsPar(o.accnmodify,"accnmodify");
                    cm.Last().snapsPar(o.procmodify,"procmodify");
                    cm.Last().snapsPar(o.orgcode,"orgcode");
                    cm.Last().snapsPar(o.site,"site");
                    cm.Last().snapsPar(o.depot,"depot");
                    cm.Last().snapsPar(hu.huno,"huno");
                    cm.Last().snapsParsysdateoffset();

                }
                cm.Add(so.routeforcast(o.orgcode,o.site,o.depot,o.routeno,o.accnmodify));
                cm.Add(so.routeforcast(o.orgcode,o.site,o.depot,o.routesource,o.accnmodify));
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        private outboulx_md filloulx(ref SqlDataReader r,string opsaccn,string opsdnno,string opstrno,string routeno) {
            outboulx_md rn = new outboulx_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.ouorder = r["ouorder"].ToString();
            rn.ouln = r["ouln"].ToString();
            rn.ourefno = r["ourefno"].ToString();
            rn.ourefln = r["ourefln"].ToString();
            rn.ouseq = (r.IsDBNull(8)) ? 0 : r.GetInt32(8);
            rn.inorder = r["inorder"].ToString();
            rn.barcode = r["barcode"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(12)) ? 0 : r.GetInt32(12);
            rn.lv = (r.IsDBNull(13)) ? 0 : r.GetInt32(13);
            rn.unitops = (r.IsDBNull(14)) ? "1" : r.GetInt32(14).ToString();
            rn.opssku = (r.IsDBNull(15)) ? 0 : r.GetInt32(15);
            rn.opspu = (r.IsDBNull(16)) ? 0 : r.GetInt32(16);
            rn.opsweight = (r.IsDBNull(17)) ? 0 : r.GetDecimal(17);
            rn.opsvolume = (r.IsDBNull(18)) ? 0 : r.GetDecimal(18);
            //rn.oudono = r["oudono"].ToString();
            rn.oudnno = opsdnno;
            rn.outrno = opstrno;
            rn.batchno = r["batchno"].ToString();
            rn.lotno = r["lotno"].ToString();
            rn.datemfg = (r.IsDBNull(21)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(21);
            rn.dateexp = (r.IsDBNull(22)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(22);
            rn.serialno = r["serialno"].ToString();
            rn.routenodel = routeno;
            rn.stockid = (r.IsDBNull(25)) ? 0 : r.GetDecimal(25);
            rn.opshuno = r["opshuno"].ToString();
            rn.opshusource = r["opshusource"].ToString();
            rn.datedel = DateTimeOffset.Now;
            rn.datecreate = DateTimeOffset.Now;
            rn.accncreate = opsaccn;
            rn.datemodify = DateTimeOffset.Now;
            rn.accnmodify = opsaccn;
            rn.loccode = r["loccode"].ToString();
            return rn;
        }
        private stock_mvou fillmvou(ref SqlDataReader r,string opsaccn,string routeno,string thcode) {
            stock_mvou rn = new stock_mvou();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.opsloccode = r["loccode"].ToString();
            rn.opsrefno = r["ouorder"].ToString();
            rn.opshuno = r["opshuno"].ToString();
            rn.opsaccn = opsaccn;
            rn.opsthcode = thcode;
            rn.opsroute = routeno;
            rn.opscode = "CS";
            rn.opstype = "DL";
            rn.opsdate = DateTimeOffset.Now;

            rn.stockid = r["stockid"].ToString().CDecimal();
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(12)) ? 0 : r.GetInt32(12);
            rn.lv = (r.IsDBNull(13)) ? 0 : r.GetInt32(13);
            rn.hutype = "";
            rn.opsunit = (r.IsDBNull(14)) ? "1" : r.GetInt32(14).ToString();
            rn.opssku = (r.IsDBNull(15)) ? 0 : r.GetInt32(15);
            rn.opspu = (r.IsDBNull(16)) ? 0 : r.GetInt32(16);
            rn.opsweight = (r.IsDBNull(17)) ? 0 : r.GetDecimal(17);
            rn.opsvolume = (r.IsDBNull(18)) ? 0 : r.GetDecimal(18);
            rn.opsnaturalloss = 0;
            rn.opshusource = r["opshuno"].ToString(); //r["opshusource"].ToString();
            rn.procmodify = "route.shipment";
            return rn;
        }
        public async Task shipment(route_md o,string accncode) {
            SqlCommand cm = new SqlCommand(route_opsship_prepfnd,cn);
            SqlTransaction tx = null;
            sequence_ops so = new sequence_ops();
            statisic_ops ss = new statisic_ops();
            stock_ops oiv = new stock_ops(cn.ConnectionString);
            orbit_ops oo = new orbit_ops(cn);
            SqlDataReader r = null;
            //List<SqlCommand> lcm = new List<SqlCommand>();
            List<stock_mvou> mov = new List<stock_mvou>(); // List to move ou
            List<outboulx_md> lox = new List<outboulx_md>(); // List to shipment
            //var trace = new List<KeyValuePair<SqlCommand,string>>();
            var tag = new snapsCmdTag();
            string oudnno = "";
            string outrno = "";
            Decimal ostockid = 0;
            try {
                // update route info
                try {
                    if(!string.IsNullOrEmpty(o.driver) || !string.IsNullOrEmpty(o.plateNo) || !string.IsNullOrEmpty(o.sealno)) {
                        await upsert(o);
                    }
                } catch(Exception rx) {
                    logger.Error(o.orgcode,o.site,o.routeno,rx,"update route info error");
                }

                // generate transport note
                outrno = so.trnoNext(ref cn);

                // 
                cm.CommandText = route_opsship_prepfnd;
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.accnmodify,"accnmodify");
                cm.snapsPar(o.accnmodify,"accncode");
                cm.snapsPar(o.routeno,"routeno");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) {
                    mov.Add(fillmvou(ref r,accncode,o.routeno,o.thcode));
                    lox.Add(filloulx(ref r,accncode,"",outrno,o.routeno));
                }
                await r.CloseAsync();
                await cn.CloseAsync();
                //Generate dnno

                foreach(string selorder in lox.Select(e => e.ouorder).Distinct()) {
                    //solution 2
                    var loxs = lox.Where(n => n.ouorder == selorder);
                    if(loxs.Count() > 0) {
                        var dnno = so.dnnoNext(ref cn);
                        foreach(var lx in loxs) {
                            lx.oudnno = dnno;
                        }
                    }

                    ////solution 1 bug dn not show all line
                    //lox.Find(n=> n.ouorder == selorder).oudnno = so.dnnoNext(ref cn);  
                }

                //Close Route 
                //lcm.Add(this.route_opsship_step1_route.snapsCommand(cn));
                tag.Addtags("Shipment",$"{o.routeno} update wm route for Close Route",route_opsship_step1_route.snapsCommand(cn));

                //Close Preparatioin 
                //lcm.Add(this.route_opsship_step2_prep.snapsCommand(cn));
                tag.Addtags("Shipment",$"{o.routeno} update wm prep for Close Prep",route_opsship_step2_prep.snapsCommand(cn));

                //Decrease stock 
                //lcm.Add(this.route_opsship_step3_stockmove.snapsCommand(cn));
                //lcm.Add(this.route_opsship_step3_stockclear.snapsCommand(cn));
                tag.Addtags("Shipment",$"{o.routeno} insert wm stockmvhx for Decrease stock",route_opsship_step3_stockmove.snapsCommand(cn));
                tag.Addtags("Shipment",$"{o.routeno} delete wm stock for Decrease stock",route_opsship_step3_stockclear.snapsCommand(cn));
                //Move stock to history wm_outboulx
                foreach(outboulx_md lx in lox) {
                    //lcm.Add(lxcommand(lx,route_opsship_step3_history));
                    //trace.Add(lxcommand(lx,route_opsship_step3_history).snapsKeyValue($"Shipment 05 of 21 {o.routeno} =>insert wm outboulx for move history"));
                    tag.Addtags("Shipment",$"{o.routeno} insert wm outboulx for move history",lxcommand(lx,route_opsship_step3_history));
                }

                //Clear stock block 
                //lcm.Add(this.route_opsship_step3_cleabloc.snapsCommand(cn));
                //trace.Add(route_opsship_step3_cleabloc.snapsCommand(cn).snapsKeyValue($"Shipment 06 of 21 {o.routeno} =>insert wm outboulx for history"));
                tag.Addtags("Shipment",$"{o.routeno} insert wm outboulx for history",route_opsship_step3_cleabloc.snapsCommand(cn));

                //Close HU
                //lcm.Add(this.route_opsship_step4_hu.snapsCommand(cn));
                //trace.Add(route_opsship_step4_hu.snapsCommand(cn).snapsKeyValue($"Shipment 07 of 21 {o.routeno} =>update wm handerlingunit for Close HU"));
                tag.Addtags("Shipment",$"{o.routeno} update wm handerlingunit for Close HU",route_opsship_step4_hu.snapsCommand(cn));


                //UPdate order status 
                //lcm.Add(this.route_opsship_step5_order.snapsCommand(cn));
                //lcm.Add(this.route_opsship_step5_ordln.snapsCommand(cn));
                //trace.Add(route_opsship_step5_order.snapsCommand(cn).snapsKeyValue($"Shipment 08 of 21 {o.routeno} =>update wm outbouln for order status"));
                //trace.Add(route_opsship_step5_ordln.snapsCommand(cn).snapsKeyValue($"Shipment 09 of 21 {o.routeno} =>update wm outbound for order status"));
                tag.Addtags("Shipment",$"{o.routeno} update wm outbouln for order status",route_opsship_step5_order.snapsCommand(cn));
                tag.Addtags("Shipment",$"{o.routeno} update wm outbound for order status",route_opsship_step5_ordln.snapsCommand(cn));

                //Update product status 
                //lcm.Add(this.route_opsship_step6_product.snapsCommand(cn));
                //trace.Add(route_opsship_step6_product.snapsCommand(cn).snapsKeyValue($"Shipment 10 of 21 {o.routeno} =>update wm_product"));
                tag.Addtags("Shipment",$"{o.routeno} update wm_product state",route_opsship_step6_product.snapsCommand(cn));

                //Update location status
                //lcm.Add(ss.sqlopsship_location.snapsCommand(cn));
                //trace.Add(ss.sqlopsship_location.snapsCommand(cn).snapsKeyValue($"Shipment 11 of 21 {o.routeno} =>update wm locdw for location status"));
                tag.Addtags("Shipment",$"{o.routeno} update wm locdw for location status",ss.sqlopsship_location.snapsCommand(cn));

                //Update stock on hanad 
                //lcm.Add(ss.sqlopsship_stock_step1.snapsCommand(cn));
                //lcm.Add(ss.sqlopsship_stock_step2.snapsCommand(cn));
                //trace.Add(ss.sqlopsship_stock_step1.snapsCommand(cn).snapsKeyValue($"Shipment 12 of 21 {o.routeno} =>delete wm productstate for stock on hanad "));
                //trace.Add(ss.sqlopsship_stock_step2.snapsCommand(cn).snapsKeyValue($"Shipment 13 of 21 {o.routeno} =>insert wm productstate for stock on hanad "));
                tag.Addtags("Shipment",$"{o.routeno} delete wm productstate for stock on hanad",ss.sqlopsship_stock_step1.snapsCommand(cn));
                tag.Addtags("Shipment",$"{o.routeno} insert wm productstate for stock on hanad",ss.sqlopsship_stock_step2.snapsCommand(cn));

                //Send to orbit shipment
                //lcm.Add(this.route_opsship_step7_orbit.snapsCommand(cn));
                //trace.Add(route_opsship_step7_orbit.snapsCommand(cn).snapsKeyValue($"Shipment 14 of 21 {o.routeno} =>insert xm xodelivery for orbit shipment"));
                tag.Addtags("Shipment",$"{o.routeno} insert xm xodelivery for orbit shipment",route_opsship_step7_orbit.snapsCommand(cn));

                //Send to orbit rtv block and unblock
                //lcm.Add(this.route_opsship_step7_1_orbitblock.snapsCommand(cn));
                //trace.Add(route_opsship_step7_1_orbitblock.snapsCommand(cn).snapsKeyValue($"Shipment 15 of 21 {o.routeno} =>insert xm xoblock for rtv unblock"));
                tag.Addtags("Shipment",$"{o.routeno} insert xm xoblock for rtv unblock",route_opsship_step7_1_orbitblock.snapsCommand(cn));

                //Backup old Stock
                //lcm.Add(this.route_opsship_step8_backustock.snapsCommand(cn));
                //trace.Add(route_opsship_step8_backustock.snapsCommand(cn).snapsKeyValue($"Shipment 16 of 21 {o.routeno} =>insert wm stockmvhx for backup stock"));
                tag.Addtags("Shipment",$"{o.routeno} insert wm stockmvhx for backup stock",route_opsship_step8_backustock.snapsCommand(cn));

                //Clear old stock with empty 
                //lcm.Add(this.route_opsship_step9_clearstock.snapsCommand(cn));
                //trace.Add(route_opsship_step9_clearstock.snapsCommand(cn).snapsKeyValue($"Shipment 17 of 21 {o.routeno} =>delete wm stock for clear old stock"));
                tag.Addtags("Shipment",$"{o.routeno} delete wm stock for clear old stock",route_opsship_step9_clearstock.snapsCommand(cn));

                //Backup old HU
                //lcm.Add(this.route_opsship_step10_backuphu.snapsCommand(cn));
                //trace.Add(route_opsship_step10_backuphu.snapsCommand(cn).snapsKeyValue($"Shipment 18 of 21 {o.routeno} =>insert wm handerlingunitlx for backup old HU"));
                tag.Addtags("Shipment",$"{o.routeno} insert wm handerlingunitlx for backup old HU",route_opsship_step10_backuphu.snapsCommand(cn));
                //Clear old HU with empty stock 
                //lcm.Add(this.route_opsship_step11_clearhu.snapsCommand(cn));
                //trace.Add(route_opsship_step11_clearhu.snapsCommand(cn).snapsKeyValue($"Shipment 19 of 21 {o.routeno} =>delete wm handerlingunit for clear old HU"));
                tag.Addtags("Shipment",$"{o.routeno} delete wm handerlingunit for clear old HU",route_opsship_step11_clearhu.snapsCommand(cn));

                //Clear stock zero 
                //lcm.Add(this.route_opsship_step12_backupstockzero.snapsCommand(cn));
                //lcm.Add(this.route_opsship_step12_clearstockzeno.snapsCommand(cn));
                //trace.Add(route_opsship_step12_backupstockzero.snapsCommand(cn).snapsKeyValue($"Shipment 20 of 21 {o.routeno} =>insert wm stockmvhx for backup stock zero "));
                //trace.Add(route_opsship_step12_clearstockzeno.snapsCommand(cn).snapsKeyValue($"Shipment 21 of 21 {o.routeno} =>delete wm stock for clear stock zero"));
                tag.Addtags("Shipment",$"{o.routeno} insert wm stockmvhx for backup stock zero",route_opsship_step12_backupstockzero.snapsCommand(cn));
                tag.Addtags("Shipment",$"{o.routeno} delete wm stock for clear stock zero",route_opsship_step12_clearstockzeno.snapsCommand(cn));

                //foreach(SqlCommand ln in lcm) { 
                //    if(!ln.Parameters.Contains("orgcode")) { ln.snapsPar(o.orgcode,"orgcode"); }
                //    if(!ln.Parameters.Contains("site")) { ln.snapsPar(o.site,"site"); }
                //    if(!ln.Parameters.Contains("depot")) { ln.snapsPar(o.depot,"depot"); }
                //    if(!ln.Parameters.Contains("routeno")) { ln.snapsPar(o.routeno,"routeno"); }
                //    if(!ln.Parameters.Contains("accncode")) { ln.snapsPar(o.accnmodify,"accncode"); }
                //    if(!ln.Parameters.Contains("thcode")) { ln.snapsPar(o.thcode,"thcode"); }
                //    if(!ln.Parameters.Contains("outrno")) { ln.snapsPar(outrno,"outrno"); }
                //}

                for(int i = 0 ; i < tag.cmdTags.Count ; i++) {
                    if(!tag.cmdTags[i].sqlCommand.Parameters.Contains("orgcode")) { tag.cmdTags[i].sqlCommand.snapsPar(o.orgcode,"orgcode"); }
                    if(!tag.cmdTags[i].sqlCommand.Parameters.Contains("site")) { tag.cmdTags[i].sqlCommand.snapsPar(o.site,"site"); }
                    if(!tag.cmdTags[i].sqlCommand.Parameters.Contains("depot")) { tag.cmdTags[i].sqlCommand.snapsPar(o.depot,"depot"); }
                    if(!tag.cmdTags[i].sqlCommand.Parameters.Contains("routeno")) { tag.cmdTags[i].sqlCommand.snapsPar(o.routeno,"routeno"); }
                    if(!tag.cmdTags[i].sqlCommand.Parameters.Contains("accncode")) { tag.cmdTags[i].sqlCommand.snapsPar(o.accnmodify,"accncode"); }
                    if(!tag.cmdTags[i].sqlCommand.Parameters.Contains("thcode")) { tag.cmdTags[i].sqlCommand.snapsPar(o.thcode,"thcode"); }
                    if(!tag.cmdTags[i].sqlCommand.Parameters.Contains("outrno")) { tag.cmdTags[i].sqlCommand.snapsPar(outrno,"outrno"); }
                }

                //Execute
                //await lcm.snapsExecuteTransAsync(cn, true);

                // Execute and Tags Result
                var result = await tag.snapsExecuteTagsAsync(cn);
                try {
                    foreach(string tagresult in result.SerializeTag()) {
                        logger.Debug(o.orgcode,o.site,o.routeno,tagresult);
                    }
                } catch(Exception) {} finally { result.cmdTags.Clear(); }
                //return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally {
            }
        }
        public async Task huload(string orgcode,string site,string depot,route_hu o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                cm.Add(route_huload_step1.snapsCommand(cn));
                cm.Add(route_huload_step2.snapsCommand(cn));
                cm.Add(route_huload_step3.snapsCommand(cn));
                cm.ForEach(e => {
                    e.snapsParsysdateoffset();
                    e.snapsPar(o.routeno,"routeno");
                    e.snapsPar(o.accnmodify,"accnmodify");
                    e.snapsPar("ouroute.load","procmodify");
                    e.snapsPar(orgcode,"orgcode");
                    e.snapsPar(site,"site");
                    e.snapsPar(depot,"depot");
                    e.snapsPar(o.huno,"huno");
                });
                await cm.snapsExecuteTransAsync(cn);

            } catch(Exception ex) {
                logger.Error(orgcode,site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }



        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing) {
            if(!disposedValue) {
                if(cn != null) { cn.Dispose(); }
                sqlval = null; sqlins = null; sqlupd = null; sqlrem = null; sqlfnd = null;
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}
