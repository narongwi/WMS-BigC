using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using System.Data;
using Snaps.WMS;
using System.Net.Http;

namespace Snaps.WMS.preparation {
    //Header 
    public partial class prep_ops : IDisposable {
        public async Task<List<prep_ls>> find(prep_pm rs) {
            SqlCommand cm = new SqlCommand(sqlprep_find,cn);
            List<prep_ls> rn = new List<prep_ls>();
            SqlDataReader r = null;
            try {
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");

                cm.snapsCdn(rs.spcarea,"spcarea"," and a.spcarea = @spcarea");
                cm.snapsCdn(rs.routeno,"routeno"," and a.routeno = @routeno ");
                cm.snapsCdn(rs.huno,"huno"," and a.huno = @huno ");
                cm.snapsCdn(rs.preptype,"preptype"," and a.preptype = @preptype ");
                cm.snapsCdn(rs.prepno,"prepno"," and a.prepno = @prepno ");
                cm.snapsCdn(rs.prepdate,"prepdate"," and cast(a.prepdate as date) = cast(@prepdate as date) ");
                cm.snapsCdn(rs.thcode,"thcode"," and a.thcode = @thcode ");
                cm.snapsCdn(rs.spcorder,"spcorder"," and a.spcorder = @spcorder ");
                cm.snapsCdn(rs.spcarticle,"spcarticle"," and a.spcarticle = @spcarticle ");
                cm.snapsCdn(rs.dateassign,"dateassign"," and a.dateassign = @dateassign ");
                cm.snapsCdn(rs.tflow,"tflow"," and a.tflow = @tflow ");

                cm.snapsCdn(rs.setno,"setno"," and a.setno = @setno");

                //Date prep
                if(!rs.dateprepfrom.Equals(null) && !rs.dateprepto.Equals(null)) {
                    cm.snapsPar(rs.dateprepto,"dateprepto");
                    cm.snapsCdn(rs.dateprepfrom,"datereqfrom"," and cast(o.datereqdel as date) between cast(@dateprepfrom as date) and cast(@dateprepto as date) ");
                } else if(!rs.dateprepfrom.Equals(null) && rs.dateprepto.Equals(null)) {
                    cm.snapsCdn(rs.dateprepfrom,"dateprepfrom"," and cast(o.datereqdel as date) = cast(@dateprepfrom as date) ");
                } else if(!rs.dateprepto.Equals(null) && rs.dateprepfrom.Equals(null)) {
                    cm.snapsCdn(rs.dateprepto,"datereqto"," and cast(o.datereqdel as date) <= cast( @dateprepto as date) ");
                }

                //Date order 
                if(!rs.dateorderfrom.Equals(null) && !rs.dateorderto.Equals(null)) {
                    cm.snapsPar(rs.dateorderto,"dateorderto");
                    cm.snapsCdn(rs.dateorderfrom,"datereqfrom"," and cast(o.datereqdel as date) between cast(@dateorderfrom as date) and cast(@dateorderto as date) ");
                } else if(!rs.dateorderfrom.Equals(null) && rs.dateorderto.Equals(null)) {
                    cm.snapsCdn(rs.dateorderfrom,"dateorderfrom"," and cast(o.datereqdel as date) = cast(@dateorderfrom as date) ");
                } else if(!rs.dateorderto.Equals(null) && rs.dateprepfrom.Equals(null)) {
                    cm.snapsCdn(rs.dateorderto,"datereqto"," and cast(o.datereqdel as date) <= cast( @dateorderto as date) ");
                }


                cm.CommandText = string.Format(cm.CommandText,
                (!rs.ouorder.notNull()) ? "" : string.Format(" and exists (select 1 from wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno and l.ouorder like '%{0}%' ) ",rs.ouorder.ClearReg()),
                (!rs.article.notNull()) ? "" : string.Format(" and exists (select 1 from wm_prln l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno and l.article like '%{0}%' ) ",rs.article.ClearReg()),
                (!rs.ouorder.notNull()) ? "" : string.Format(" and l.ouorder like '%{0}%' ",rs.ouorder.ClearReg()),
                (!rs.article.notNull()) ? "" : string.Format(" and l.article like '%{0}%' ",rs.article.ClearReg())) + " order by priority desc,preptype asc, cast(prepno as int) desc";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(prep_getls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<prep_md> get(prep_ls rs,Boolean isforclose = false) {
            return await get(rs.orgcode,rs.site,rs.depot,rs.spcarea,rs.prepno,rs.preptype);
        }
        public async Task<prep_md> get(string orgcode,string site,string depot,string spcarea,string prepno,string preptype,Boolean isforclose = false) {
            SqlCommand cm = new SqlCommand();
            SqlDataReader r = null;
            prep_md rn = new prep_md();
            try {
                /* Vlidate parameter */
                // cm = ("select o.*,t.thnamealt thname from wm_outbound o,wm_thparty t where o.orgcode = t.orgcode " + 
                // " and o.site = t.site and o.depot = t.depot and o.thcode = t.thcode and o.orgcode = @orgcode " +
                // " and o.site = @site and o.depot = @depot and o.ouorder = @ouorder ").snapsCommand(cn);
                if(preptype == "A") {
                    // for Approach -- Pallet Pick
                    cm = @" select distinct th.orgcode, th.site, th.depot, th.spcarea, th.routeno, tl.sourcehuno as huno, th.tasktype as preptype,
                            th.taskno as prepno, orh.dateprep as prepdate, orh.oupriority as priority, 0.0 as preppct,
                            th.routethcode as thcode, orh.ouorder as spcorder, tl.article as spcarticle, tl.dateassign,
                            th.datestart, th.dateend, th.tflow, th.devid as deviceID, tl.accnassign as picker,
                            th.datecreate, th.accncreate, th.datemodify, th.accnmodify, th.procmodify, null as przone, r.loccode
                            from wm_taln tl, wm_task th, wm_outbound orh, wm_route r
                            where th.orgcode = @orgcode and th.site = @site and th.depot = @depot and th.taskno = @prepno
                            and th.orgcode = tl.orgcode and th.site = tl.site and th.depot = tl.depot and th.taskno = tl.taskno
                            and th.orgcode = orh.orgcode and th.site = orh.site and th.depot = orh.depot and th.iorefno = orh.ouorder 
                            and th.orgcode = r.orgcode and th.site = r.site and th.depot = r.depot and th.routeno = r.routeno ".snapsCommand(cn);
                } else {
                    //cm = "select * from wm_prep where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and prepno = @prepno ".snapsCommand(cn);   
                    cm = @"select p.*, r.loccode from wm_prep p, wm_route r where p.orgcode = r.orgcode and p.site = r.site and p.depot = r.depot and p.routeno = r.routeno 
                     and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.spcarea = @spcarea and p.prepno = @prepno  ".snapsCommand(cn);
                }
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(spcarea,"spcarea");
                cm.snapsPar(prepno,"prepno");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = prep_getmd(ref r); }
                await r.CloseAsync();


                rn.lines = new List<prln_md>();
                if(preptype == "A") {
                    cm.CommandText = prln_sqlfndA;
                } else if(preptype == "P" && spcarea == "XD") {
                    cm.CommandText = prln_sqlfnd_dist;
                } else {
                    if(isforclose == true) {
                        cm.CommandText = prlin_sqlfnd_forclose;
                    } else {
                        cm.CommandText = prln_sqlfnd;
                    }
                }

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.lines.Add(prln_getmd(ref r)); }
                await r.CloseAsync();

                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rn.orgcode,rn.site,rn.depot,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<prep_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            Int32 ix = 0;
            try {
                foreach(prep_md ln in rs) {
                    cm.Add(prep_setmd(ln,prep_sqlins));
                    //cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? prep_sqlins : prep_sqlupd; 
                    foreach(prln_md rn in ln.lines) {
                        rn.devicecode = "";
                        rn.huno = (ln.spcarea == "XD") ? null : ln.huno;
                        rn.locdigit = "";
                        rn.loczone = ln.przone;
                        rn.picker = "";
                        rn.prepno = ln.prepno;
                        rn.procmodify = ln.procmodify;
                        rn.spcarea = ln.spcarea;
                        rn.unitname = "";
                        rn.accncreate = ln.accncreate;
                        cm.Add(prln_setmd(rn,prln_sqlins));
                        cm.Add(prbc_setmd(rn,"P",ln.hutype,prbc_sqlins)); // assign block stock for prepline 
                    }
                    ix++;
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task upsert(prep_md rs) {
            List<prep_md> ro = new List<prep_md>();
            try {
                ro.Add(rs); await upsert(ro);
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<prep_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                foreach(prep_md ln in rs) {
                    cm.Add(prep_setmd(ln,prep_sqlrem));
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task remove(prep_md rs) {
            List<prep_md> ro = new List<prep_md>();
            try {
                ro.Add(rs); await remove(ro);
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<prep_ix> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            Int32 ix = 0;
            try {
                foreach(prep_ix ln in rs) {
                    cm.Add(prep_setix(ln,prep_sqlvld));
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? prep_sqlinx : prep_sqlupd;
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,"",ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task setPriority(prep_md o) {
            SqlCommand cm = new SqlCommand(prep_opspriority,cn);
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.prepno,"prepno");
                cm.snapsPar(o.procmodify,"procmodify");
                cm.snapsPar(o.accnmodify,"accnmodify");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task setStart(prep_md o) {
            SqlCommand cm = new SqlCommand(prep_opsstart,cn);
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.prepno,"prepno");
                cm.snapsPar(o.procmodify,"procmodify");
                cm.snapsPar(o.accnmodify,"accnmodify");
                cm.snapsPar(o.accnmodify,"picker");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.Dispose(); }
        }

        private stock_mvin tomoveIn(prln_md o,string routeno,string prepno,string location,string thcode) {
            stock_mvin rn = new stock_mvin();
            rn.orgcode = o.orgcode;
            rn.site = o.site;
            rn.depot = o.depot;
            rn.spcarea = o.spcarea;
            rn.stockid = 0; // Force to 0 coz need to create a new line of stock to shipment 
            rn.article = o.article;
            rn.batchno = o.batchno;
            rn.dateexp = o.dateexp;
            rn.datemfg = o.datemfg;
            rn.daterec = o.daterec;
            rn.depot = o.depot;
            rn.lotno = o.lotno;
            rn.lv = o.lv;
            rn.opsaccn = o.picker;
            rn.opsunit = o.unitprep;
            rn.opssku = o.qtyskuops;
            rn.opspu = o.qtypuops;
            rn.opsweight = o.qtyweightops;
            rn.opsvolume = o.qtyvolumeops;
            rn.opsnaturalloss = 0;
            rn.opsdate = DateTimeOffset.Now;
            rn.opstype = "";
            rn.opscode = prepno;
            rn.opsroute = routeno;
            rn.opsthcode = thcode;
            rn.stockhash = "";
            rn.opshuno = o.huno;
            rn.opsloccode = location;
            rn.opsrefno = prepno;
            rn.procmodify = o.procmodify;
            rn.batchno = o.batchno;
            rn.lotno = o.lotno;
            rn.serialno = o.serialno;
            rn.opshusource = o.hunosource;
            rn.inagrn = o.inagrn;
            rn.ingrno = o.ingrno;
            rn.stocktflow = "IO";
            rn.prepstockid = o.stockid;     // for keep old stock id coz we to spilt old stock id 
            return rn;
        }

        private SqlCommand toSplitIn(stock_mvin o) {
            SqlCommand cm = new SqlCommand(prep_opsend_step0);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.opshuno,"huno");
            cm.snapsPar(o.opshusource,"hunosource");
            cm.snapsPar(o.opscode,"prepno");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.prepstockid,"stockid");       // use for update old stock id with new stock id column name must be switch coz prep cmd we swtich them 
            cm.snapsPar(o.stockid,"prepstockid");       // use for update old stock id with new stock id column name must be switch coz prep cmd we swtich them 
            return cm;
        }
        private SqlCommand toUpdate(string sql,prep_md o) {
            SqlCommand lm = new SqlCommand(sql);
            lm.snapsPar(o.orgcode,"orgcode");
            lm.snapsPar(o.site,"site");
            lm.snapsPar(o.depot,"depot");
            lm.snapsPar(o.spcarea,"spcarea");
            lm.snapsPar(o.prepno,"prepno");
            lm.snapsPar(o.procmodify,"procmodify");
            lm.snapsPar(o.accnmodify,"accnmodify");
            lm.snapsPar(o.accnmodify,"picker");
            lm.snapsPar(o.huno,"huno");
            lm.snapsPar(o.routeno,"routeno");
            lm.snapsParsysdateoffset();
            return lm;
        }

        private stock_mvou tomoveOut(prln_md o,string routeno,string prepno,string location) {
            stock_mvou rn = new stock_mvou();
            rn.orgcode = o.orgcode;
            rn.site = o.site;
            rn.depot = o.depot;
            rn.spcarea = o.spcarea;
            rn.stockid = o.stockid;
            rn.article = o.article;
            rn.batchno = o.batchno;
            rn.dateexp = o.dateexp;
            rn.datemfg = o.datemfg;

            rn.daterec = o.daterec;
            rn.depot = o.depot;
            rn.lotno = o.lotno;
            rn.lv = o.lv;
            rn.opsaccn = o.picker;
            rn.opsunit = o.unitprep;
            rn.opssku = o.qtyskuops;
            rn.opspu = o.qtypuops;
            rn.opsweight = o.qtyweightops;
            rn.opsvolume = o.qtyvolumeops;
            rn.opsnaturalloss = 0;
            rn.opsdate = DateTimeOffset.Now;
            rn.opstype = "";
            rn.opscode = o.prepno;
            rn.opsroute = routeno;
            rn.opsthcode = o.thcode;
            rn.stockhash = "";
            rn.opshuno = o.huno;
            rn.opsloccode = location;
            rn.opsrefno = o.ouorder;
            rn.procmodify = o.procmodify;
            rn.batchno = o.batchno;
            rn.lotno = o.lotno;
            rn.serialno = o.serialno;
            rn.opshusource = o.hunosource;
            rn.hutype = "";
            rn.inagrn = o.inagrn;
            rn.ingrno = o.ingrno;
            return rn;
        }

        public async Task setEnd(prep_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            statisic_ops so = new statisic_ops(ref cn);
            List<stock_mvou> mou = new List<stock_mvou>();         //Stock movement Out object
            List<stock_mvin> min = new List<stock_mvin>();         //Stock movement Out object
            stock_ops oiv = new stock_ops(cn.ConnectionString);
            SqlTransaction tx = null;
            Decimal outstockid = 0;
            string errsql = "";
            Int32 ix = 0;
            try {
                o = await get(o.orgcode,o.site,o.depot,o.spcarea,o.prepno,o.preptype,true);
                //Move stock from storage to dispath coz it safty than use the same location
                o.lines.Where(e => e.qtyskuops > 0).ToList().ForEach(ln => {
                    min.Add(tomoveIn(ln,o.routeno,o.prepno,o.loccode,o.thcode));
                });
                oiv.stockIncommand(ref min,ref cn,ref tx,ref outstockid).ForEach(x => {
                    cm.Add(x);
                });

                //Set new stock Id after spilt hu
                min.ForEach(ln => { cm.Add(toSplitIn(ln)); });
                //End 

                //Move stock from storage to dispath coz it safty than use the same location
                o.lines.Where(e => e.qtyskuops > 0).ToList().ForEach(ln => {
                    mou.Add(tomoveOut(ln,o.routeno,o.prepno,o.loccode));
                });
                oiv.stockOutcommand(mou,ref cn,ref tx,ref outstockid).ForEach(x => { cm.Add(x); });
                //End 

                cm.Add(toUpdate(prep_opsend_step1,o));
                cm.Add(toUpdate(prep_opsend_step2,o));
                if(o.spcarea == "ST") {
                    //Close prep HU for stocking only
                    cm.Add(toUpdate(prep_opsend_step3,o));
                }
                
                cm.Add(toUpdate(prep_opsend_step4,o));
                cm.Add(toUpdate(prep_opsend_step5,o));
                cm.Add(toUpdate(prep_opsend_step6,o));

                // cm.Add(new SqlCommand(prep_opsend_step2));
                // cm.Add(new SqlCommand(prep_opsend_step3));
                // cm.Add(new SqlCommand(prep_opsend_step4));
                // cm.Add(new SqlCommand(prep_opsend_step5));
                // cm.Add(prep_opsend_step6.snapsCommand());

                // cm.ForEach(lm => {                    
                //     ix++;
                //     if ( ix > cm.Count - 6 ) { 
                //         lm.snapsPar(o.orgcode, "orgcode");
                //         lm.snapsPar(o.site, "site");
                //         lm.snapsPar(o.depot, "depot");
                //         lm.snapsPar(o.spcarea, "spcarea");
                //         lm.snapsPar(o.prepno, "prepno");
                //         lm.snapsPar(o.procmodify, "procmodify");
                //         lm.snapsPar(o.accnmodify, "accnmodify");
                //         lm.snapsPar(o.accnmodify, "picker");
                //         lm.snapsPar(o.huno, "huno");
                //         lm.snapsPar(o.routeno, "routeno");
                //         lm.snapsParsysdateoffset();
                //     }

                // });


                foreach(prln_md ln in o.lines) {
                    cm.Add(so.location(o.orgcode,o.site,o.depot,ln.loccode));
                }

                cm.snapsExecuteTransRef(ref cn,ref tx,ref errsql,true); //debuging

                tx.Commit();
                cn.Close();

                //Recalculate stock 
                foreach(prln_md lx in o.lines
                    .GroupBy(x => new { x.orgcode,x.site,x.depot,x.spcarea,x.article,x.pv,x.lv })
                    .Select(s => s.FirstOrDefault()).ToList()
                ) {
                    await so.inboundstock(lx.orgcode,lx.site,lx.depot,lx.article,lx.pv.ToString(),lx.lv.ToString()).snapsExecuteTransAsync(cn);
                }
                //Cal route statistic
                await so.routeforcast(o.orgcode,o.site,o.depot,o.routeno,o.accnmodify,cn).snapsExecuteAsync();

                if(o.spcarea == "ST") {
                    // print label
                    await printShipLabel(o.orgcode,o.site,o.depot,o.huno,o.accnmodify);
                }
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => { x.Dispose(); }); }
        }
        public async Task opsPick(prln_md o) {
            SqlCommand om = new SqlCommand(prep_opspick_vld,cn);
            List<SqlCommand> cm = new List<SqlCommand>();
            string rsl = "";
            try {
                om.snapsPar(o.orgcode,"orgcode");
                om.snapsPar(o.site,"site");
                om.snapsPar(o.depot,"depot");
                om.snapsPar(o.spcarea,"spcarea");
                om.snapsPar(o.prepno,"prepno");
                om.snapsPar(o.prepln,"prepln");
                om.snapsPar(o.article,"article");
                om.snapsPar(o.pv,"pv");
                om.snapsPar(o.lv,"lv");
                om.snapsPar(o.accnmodify,"accnmodify");
                om.snapsPar(o.locdigit,"lsdigit");
                om.snapsPar(o.skipdigit,"skipdigit");

                rsl = om.snapsScalarStrAsync().Result;
                if(rsl != "PASS") {
                    throw new Exception(rsl);
                } else {
                    cm.Add(new SqlCommand(prep_opspick_stp1));
                    cm.Add(new SqlCommand(prep_opspick_stp2));
                    cm.Add(new SqlCommand(prep_opspick_stp3));
                    cm.ForEach(lm => {
                        lm.snapsPar(o.orgcode,"orgcode");
                        lm.snapsPar(o.site,"site");
                        lm.snapsPar(o.depot,"depot");
                        lm.snapsPar(o.spcarea,"spcarea");
                        lm.snapsPar(o.prepno,"prepno");
                        lm.snapsPar(o.procmodify,"procmodify");
                        lm.snapsPar(o.accnmodify,"accnmodify");
                        lm.snapsPar(o.accnmodify,"picker");
                        lm.snapsPar(o.prepln,"prepln");
                        lm.snapsPar(o.article,"article");
                        lm.snapsPar(o.pv,"pv");
                        lm.snapsPar(o.lv,"lv");
                        lm.snapsPar(o.qtypuops,"qtypuops");
                        lm.snapsPar(o.qtyskuops,"qtyskuops");
                        lm.snapsPar(o.serialno.ToString(),"serialno");
                        lm.snapsParsysdateoffset();
                    });
                    await cm.snapsExecuteTransAsync(cn);
                }
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        public async Task opsPut(prln_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                cm.Add(new SqlCommand(prep_opsput_stp1));
                cm.Add(new SqlCommand(prep_opsput_stp2));
                cm.Add(new SqlCommand(prep_opsput_stp3));
                cm.Add(new SqlCommand(prep_opsput_stp4));
                cm.ForEach(lm => {
                    lm.snapsPar(o.orgcode,"orgcode");
                    lm.snapsPar(o.site,"site");
                    lm.snapsPar(o.depot,"depot");
                    lm.snapsPar(o.spcarea,"spcarea");
                    lm.snapsPar(o.prepno,"prepno");
                    lm.snapsPar(o.procmodify,"procmodify");
                    lm.snapsPar(o.accnmodify,"accnmodify");
                    lm.snapsPar(o.accnmodify,"picker");
                    lm.snapsPar(o.prepln,"prepln");
                    lm.snapsPar(o.article,"article");
                    lm.snapsPar(o.pv,"pv");
                    lm.snapsPar(o.lv,"lv");
                    lm.snapsPar(o.qtypuops,"qtypuops");
                    lm.snapsPar(o.qtyskuops,"qtyskuops");
                    lm.snapsPar(o.serialno.ToString(),"serialno");
                    lm.snapsPar(o.huno,"huno");
                    lm.snapsParsysdateoffset();
                });
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        public async Task opsCancel(prep_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                cm.Add(new SqlCommand(sqlprep_cancel_step1));
                cm.Add(new SqlCommand(sqlprep_cancel_step2));
                cm.Add(new SqlCommand(sqlprep_cancel_step3));
                cm.Add(new SqlCommand(sqlprep_cancel_step4));

                // Re active order if cancel all prep task
                cm.Add(new SqlCommand(sqlprep_cancel_step5));

                cm.ForEach(lm => {
                    lm.snapsPar(o.orgcode,"orgcode");
                    lm.snapsPar(o.site,"site");
                    lm.snapsPar(o.depot,"depot");
                    lm.snapsPar(o.spcarea,"spcarea");
                    lm.snapsPar(o.prepno,"prepno");
                    lm.snapsPar(o.accnmodify,"accnmodify");
                    lm.snapsParsysdateoffset();
                });
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        public void rqnprep(ref SqlCommand cm,DataRow r,ref List<prep_md> pls) {

            //assign route no
            pls.Add(new prep_md());
            //generate prep no 
            cm.CommandText = "select  next value for seq_huno rsl";
            pls.Last().prepno = cm.snapsScalarStrAsync().Result;

            //generate huno 
            cm.CommandText = "select next value for seq_prep rsl";
            pls.Last().huno = cm.snapsScalarStrAsync().Result;

            pls.Last().capacity = 10000;
            pls.Last().dateassign = new DateTimeOffset();
            pls.Last().datecreate = new DateTimeOffset();
            pls.Last().depot = r["depot"].ToString();
            pls.Last().lines = new List<prln_md>();
            pls.Last().orgcode = r["orgcode"].ToString();
            pls.Last().prepdate = new DateTimeOffset();
            pls.Last().preppct = 0;
            pls.Last().preptype = "ST";
            pls.Last().priority = 0;
            pls.Last().site = r["site"].ToString();
            pls.Last().spcarea = r["spcarea"].ToString();
            pls.Last().spcarticle = "";
            pls.Last().spcorder = "";
            pls.Last().tflow = "IO";
            pls.Last().thcode = r["thcode"].ToString();
            pls.Last().routeno = r["thcode"].ToString();
            pls.Last().deviceID = "";
            pls.Last().picker = "";
            pls.Last().accncreate = "";
            pls.Last().accnmodify = "";
            pls.Last().procmodify = "api.outbound.procstock";

        }

        public prep_stock getpickingstock(ref SqlDataReader r) {
            prep_stock rn = new prep_stock();
            try {
                while(r.Read()) {
                    rn.orgcode = r["orgcode"].ToString();
                    rn.site = r["site"].ToString();
                    rn.depot = r["depot"].ToString();
                    rn.stockid = r.GetDecimal(3);
                    rn.huno = r["huno"].ToString();
                    rn.article = r["article"].ToString();
                    rn.pv = r.GetInt32(6);
                    rn.lv = r.GetInt32(7);
                    rn.qtysku = r.GetInt32(8);
                    rn.qtypu = r.GetInt32(9);
                    rn.loccode = r["loccode"].ToString();
                    rn.dateexp = (r.IsDBNull(11)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(11);
                    rn.datemfg = (r.IsDBNull(12)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(12);
                    rn.batchno = r["batchno"].ToString();
                    rn.lotno = r["lotno"].ToString();
                }
                r.Close();
                return rn;
            } catch(Exception ex) {
                throw ex;
            }

        }

        public async Task opsSelect(ouselect o) {
            /* Select for Pereperation Order */
                if(o != null) {
                SqlCommand cm = new SqlCommand(sqlIsselected,cn);
                try {
                    cm.snapsPar(o.orgcode,"orgcode");
                    cm.snapsPar(o.site,"site");
                    cm.snapsPar(o.depot,"depot");
                    cm.snapsPar(o.spcarea,"spcarea");
                    cm.snapsPar(o.ouorder,"ouorder");
                    cm.snapsPar(o.outype,"outype");
                    cm.snapsPar(o.ousubtype,"ousubtype");
                    cm.snapsPar(o.thcode,"thcode");
                    cm.snapsPar(o.selectaccn,"selectaccn");
                    cm.snapsPar(o.selectflow,"selectflow");

                    var dtselect = await cm.snapsTableAsync();
                    if(dtselect.Rows.Count == 0) {
                        cm.CommandText = sqlSelected;
                        await cm.snapsExecuteAsync();
                    }else {
                        throw new Exception("Order already selected with " + dtselect.Rows[0]["selectaccn"].ToString());
                    }

                } catch(Exception ex) {
                    throw ex;
                } finally { cm.Dispose(); }
            }
        }
        public async Task opsUnselect(ouselect o) {
            /* Unselect for Pereperation Order */
            if(o != null) {
                SqlCommand cm = new SqlCommand(sqlIsselected,cn);
                try {
                    cm.snapsPar(o.orgcode,"orgcode");
                    cm.snapsPar(o.site,"site");
                    cm.snapsPar(o.depot,"depot");
                    cm.snapsPar(o.spcarea,"spcarea");
                    cm.snapsPar(o.ouorder,"ouorder");
                    cm.snapsPar(o.outype,"outype");
                    cm.snapsPar(o.ousubtype,"ousubtype");
                    cm.snapsPar(o.thcode,"thcode");
                    cm.snapsPar(o.selectaccn,"selectaccn");
                    cm.snapsPar(o.selectflow,"selectflow");
                    // check selection
                    var dtselect = await cm.snapsTableAsync();
                    if(dtselect.Rows.Count > 0) {
                        cm.CommandText = sqlUnselected;
                        await cm.snapsExecuteAsync();
                    } 
                } catch(Exception ex) {
                    logger.Error(o.orgcode,o.site,o.selectaccn,ex,ex.Message);
                    throw ex;
                } finally { cm.Dispose(); }
            }
        }
        public async Task<prepset> procsetup(prepset po) {
            sequence_ops so = new sequence_ops();
            List<SqlCommand> cm = new List<SqlCommand>();
            SqlCommand rm = new SqlCommand(sqlprln_find,cn);
            SqlDataReader r = null;
            string selorder = "";
            try {
                // concat order for appen query with '
                po.orders.ForEach(c => { selorder += "'" + c.ouorder + "',"; });
                selorder = selorder.Substring(0,selorder.Length - 1);

                // generate prepset seq
                po.setno = so.prepsetNext(ref cn);

                // prepset header
                cm.Add(prepset_setcmd(po));
                cm.Last().CommandText = string.Format(cm.Last().CommandText,selorder);


                //prepset line
                cm.Add(sqlprlnset_insert.snapsCommand(cn));

                cm[1].snapsPar(po.orgcode,"orgcode");
                cm[1].snapsPar(po.site,"site");
                cm[1].snapsPar(po.depot,"depot");
                cm[1].snapsPar(po.setno,"setno");
                cm[1].snapsPar(po.hucode,"hucode");
                cm[1].CommandText += string.Format(" and b.ouorder in ({0}) ",selorder);
                await cm.snapsExecuteTransAsync(cn);

                rm.snapsPar(po.orgcode,"orgcode");
                rm.snapsPar(po.site,"site");
                rm.snapsPar(po.depot,"depot");
                rm.snapsPar(po.setno,"setno");
                po.orders = new List<prepsln>();
                r = await rm.snapsReadAsync();
                while(await r.ReadAsync()) {
                    po.orders.Add(prepsln_fill(ref r));

                }
                await r.CloseAsync();

                await cn.CloseAsync();
                return po;
            } catch(Exception ex) {
                logger.Error(po.orgcode,po.site,po.accncreate,ex,ex.Message);
                if(r != null) { await r.CloseAsync(); }
                await cn.CloseAsync();
                throw ex;
            } finally {
                so.Dispose();
                cm.ForEach(x => x.Dispose());
                rm.Dispose();
                selorder = null;
                if(r != null) { await r.DisposeAsync(); }
            }
        }

        public async Task<prepset> distsetup(prepset po) {
            sequence_ops so = new sequence_ops();
            List<SqlCommand> cm = new List<SqlCommand>();
            SqlCommand rm = new SqlCommand(sqlprln_find.Replace("ST","XD"),cn);
            SqlDataReader r = null;
            string selorder = "";
            try {
                //po.orders.ForEach(c => { selorder += "'" + c.ouorder + "',"; });
                //selorder = selorder.Substring(0,selorder.Length - 1);
                po.setno = so.prepsetNext(ref cn);
                cm.Add(prepset_setcmd(po));
                //change command to suppoer distribution
                cm.Last().CommandText = string.Format(sqlprepset_insert_dist,po.distb[0].disinbound);

                foreach(outbound_ls ln in po.distb) {

                    cm.Add(sqlprlnset_insert_dist.snapsCommand(cn));
                    cm.Last().snapsPar(ln.disinbound,"inorder");
                    cm.Last().snapsPar(ln.disproduct,"article");
                    cm.Last().snapsPar(ln.dispv,"pv");
                    cm.Last().snapsPar(ln.dislv,"lv");
                    cm.Last().snapsPar(po.orgcode,"orgcode");
                    cm.Last().snapsPar(po.site,"site");
                    cm.Last().snapsPar(po.depot,"depot");
                    cm.Last().snapsPar(po.setno,"setno");
                    cm.Last().snapsPar(ln.dishuno,"huno");
                    cm.Last().snapsPar(ln.disstockid,"disstockid");


                }
                await cm.snapsExecuteTransAsync(cn);

                rm.snapsPar(po.orgcode,"orgcode");
                rm.snapsPar(po.site,"site");
                rm.snapsPar(po.depot,"depot");
                rm.snapsPar(po.setno,"setno");
                po.orders = new List<prepsln>();
                r = await rm.snapsReadAsync();
                while(await r.ReadAsync()) { po.orders.Add(prepsln_fill(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();

                return po;
            } catch(Exception ex) {
                logger.Error(po.orgcode,po.site,po.accncreate,ex,ex.Message);
                if(r != null) { await r.CloseAsync(); }
                await cn.CloseAsync();
                throw ex;
            } finally {
                so.Dispose();
                cm.ForEach(x => x.Dispose());
                rm.Dispose();
                selorder = null;
                if(r != null) { await r.DisposeAsync(); }
            }
        }

        private void prepgenhu(ref handerlingunit_gen hg,route_md o,string hucode) {
            hg.orgcode = o.orgcode;
            hg.site = o.site;
            hg.depot = o.depot;
            hg.loccode = o.loccode;
            hg.thcode = o.thcode;
            hg.hutype = hucode;
            hg.mxsku = 9999;
            hg.mxvolume = 2000;
            hg.mxweight = 1000;
            hg.quantity = 1;
            hg.spcarea = "ST";
            hg.accncreate = "me";
            hg.accnmodify = "me";
            hg.promo = "";
            hg.priority = 0;
            hg.routeno = o.routeno;
        }
        private void distgenhu(ref handerlingunit_gen hg,route_md o,string hucode) {
            hg.orgcode = o.orgcode;
            hg.site = o.site;
            hg.depot = o.depot;
            hg.loccode = o.loccode;
            hg.thcode = o.thcode;
            hg.hutype = hucode;
            hg.mxsku = 9999;
            hg.mxvolume = 2000;
            hg.mxweight = 1000;
            hg.quantity = 1;
            hg.spcarea = "XD";
            hg.accncreate = "me";
            hg.accnmodify = "me";
            hg.promo = "";
            hg.priority = 0;
            hg.routeno = o.routeno;
        }

        private prep_stock preptask(ref List<task_md> tl,DataRow r,
                string orgcode,string site,string depot,string spcarea,string accncode,
                string ouorder,string targetlocation,string tasktype,string routeno,string routethcode,
                string ouln,string ourefno,string ourefln,string setno = "") {
            prep_stock rn = new prep_stock();
            tl.Add(new task_md());
            tl.Last().accncreate = accncode;
            tl.Last().accnmodify = accncode;
            tl.Last().datecreate = DateTimeOffset.Now;
            tl.Last().depot = depot;
            tl.Last().iorefno = ouorder;
            tl.Last().orgcode = orgcode;
            tl.Last().priority = "0";
            tl.Last().procmodify = "prep.procstock";
            tl.Last().site = site;
            tl.Last().spcarea = spcarea;
            tl.Last().taskdate = DateTimeOffset.Now;
            tl.Last().taskname = (tasktype == "A") ? "Full pallet" : (tasktype == "R") ? "Replenishment" : tasktype;
            tl.Last().tasktype = tasktype;
            tl.Last().routeno = routeno;
            tl.Last().routethcode = routethcode;
            tl.Last().tflow = "IO";
            tl.Last().setno = setno;

            tl.Last().lines = new List<taln_md>();
            tl.Last().lines.Add(new taln_md());
            tl.Last().lines.Last().accncreate = accncode;
            tl.Last().lines.Last().article = r["article"].ToString();
            tl.Last().lines.Last().datecreate = DateTimeOffset.Now;
            //tl.Last().lines.Last().dateexp;
            tl.Last().lines.Last().iorefno = ouorder;
            tl.Last().lines.Last().ioreftype = "outbound";
            tl.Last().lines.Last().lv = r["lv"].ToString().CInt32();
            tl.Last().lines.Last().pv = r["pv"].ToString().CInt32();
            tl.Last().lines.Last().sourcehuno = r["huno"].ToString();
            tl.Last().lines.Last().sourceloc = r["loccode"].ToString();
            tl.Last().lines.Last().sourceqty = r["qtysku"].ToString().CInt32();
            tl.Last().lines.Last().sourcevolume = (r["qtysku"].ToString().CInt32() * r["skuvolume"].ToString().CDecimal());
            tl.Last().lines.Last().spcarea = r["spcarea"].ToString();
            tl.Last().lines.Last().stockid = r["stockid"].ToString().CInt32();
            tl.Last().lines.Last().targetadv = targetlocation;
            tl.Last().lines.Last().targethuno = r["huno"].ToString();
            tl.Last().lines.Last().taskseq = 1;
            tl.Last().lines.Last().tflow = "IO";
            tl.Last().lines.Last().iopromo = "";
            tl.Last().lines.Last().ouorder = ouorder;
            tl.Last().lines.Last().ouln = ouln;
            tl.Last().lines.Last().ourefno = ourefno;
            tl.Last().lines.Last().ourefln = ourefln;

            rn.orgcode = orgcode;
            rn.site = site;
            rn.depot = depot;
            rn.stockid = r["stockid"].ToString().CInt32();
            rn.huno = r["huno"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = r["pv"].ToString().CInt32();
            rn.lv = r["lv"].ToString().CInt32();
            rn.loccode = r["loccode"].ToString();
            rn.spcarea = r["spcarea"].ToString();

            return rn;
        }
        private prep_md prephead(prepsln ln,string prepno,string setno,string hutype,string huno,string routeno,string accncode,decimal mxweight,decimal mxvolume,Int32 mxsku) {
            prep_md rn = new prep_md();
            rn.prepno = prepno;
            rn.huno = huno;
            rn.capacity = mxsku;
            rn.dateassign = null;
            rn.datecreate = DateTimeOffset.Now;
            rn.depot = ln.depot;
            rn.lines = new List<prln_md>();
            rn.orgcode = ln.orgcode;
            rn.prepdate = DateTimeOffset.Now;
            rn.preppct = 0;
            rn.preptype = "P";
            rn.priority = 0;
            rn.site = ln.site;
            rn.spcarea = ln.spcarea;
            rn.spcarticle = "";
            rn.spcorder = "";
            rn.tflow = "IO";
            rn.thcode = ln.thcode;
            rn.routeno = routeno;
            rn.deviceID = "";
            rn.picker = "";
            rn.accncreate = accncode;
            rn.accnmodify = accncode;
            rn.procmodify = "api.prep.procstock";
            rn.przone = ln.przone;
            rn.hutype = hutype;
            rn.setno = setno;
            rn.crsku = 0;
            rn.crweight = 0;
            rn.crvolume = 0;
            rn.mxsku = mxsku;
            rn.mxweight = mxweight;
            rn.mxvolume = mxvolume;
            return rn;
        }

        private void prepline(ref prep_md crprep,string ouorder,string ouln,string unitops,List<prep_stock> crstock,string setno,Boolean block = false,Int32 qtysku = 0,Int32 qtypu = 0,Decimal qtyweight = 0,Decimal qtyvolume = 0,Boolean isAutoblockonprocess = true) {
            foreach(prep_stock ls in crstock) {
                crprep.lines.Add(new prln_md());
                crprep.lines.Last().accncreate = "";
                crprep.lines.Last().accnmodify = "";
                crprep.lines.Last().article = ls.article;
                crprep.lines.Last().pv = ls.pv;
                crprep.lines.Last().lv = ls.lv;
                crprep.lines.Last().barcode = "";
                crprep.lines.Last().batchno = ls.batchno;
                crprep.lines.Last().datecreate = DateTimeOffset.Now;
                crprep.lines.Last().dateexp = ls.dateexp;
                crprep.lines.Last().datemfg = ls.datemfg;
                crprep.lines.Last().datemodify = DateTimeOffset.Now;
                crprep.lines.Last().datepick = null;
                crprep.lines.Last().depot = ls.depot;
                crprep.lines.Last().hunosource = ls.huno;
                crprep.lines.Last().loccode = ls.loccode;
                crprep.lines.Last().lotno = ls.lotno;
                crprep.lines.Last().orgcode = ls.orgcode;
                crprep.lines.Last().site = ls.site;
                crprep.lines.Last().depot = ls.depot;

                crprep.lines.Last().qtyskuorder = (block == true) ? qtysku : ls.qtysku;
                crprep.lines.Last().qtypuorder = (block == true) ? qtypu : ls.qtypu;
                crprep.lines.Last().qtyweightorder = (block == true) ? qtyweight : ls.qtyweight;
                crprep.lines.Last().qtyvolumeorder = (block == true) ? qtyvolume : ls.qtyvolume;

                crprep.lines.Last().serialno = ls.serialno;
                crprep.lines.Last().spcarea = ls.spcarea;
                crprep.lines.Last().stockid = ls.stockid;
                crprep.lines.Last().ouln = ouln;
                crprep.lines.Last().ouorder = ouorder;
                crprep.lines.Last().prepln = crprep.lines.Count + 1;
                crprep.lines.Last().tflow = "IO";
                crprep.lines.Last().unitprep = unitops;

                crprep.crsku = crprep.lines.Sum(e => e.qtyskuorder);
                crprep.crvolume = crprep.lines.Sum(e => e.qtyvolumeorder);
                crprep.crweight = crprep.lines.Sum(e => e.qtyweightorder);

                //generate block stock on process 
                if(isAutoblockonprocess) {
                    setrpnblock(
                            ls.orgcode,
                            ls.site,
                            ls.depot,
                            ls.spcarea,
                            ls.stockid.ToString().CInt32(),
                            ls.article,
                            ls.pv,
                            ls.lv,
                            ls.loccode,
                            ls.qtysku,
                            setno,
                            ls.huno,
                            ls.loccode);
                }


            }
        }
        private prep_md disthead(string orgcode,string site,string depot,string accncode,string przone,string prepno,
                                 string hutype,string huno,string routeno,decimal mxweight,decimal mxvolume,Int32 mxsku,
                                 string inorder,string thcode,string article,string pv,string lv,string setno) {
            prep_md rn = new prep_md();
            rn.prepno = prepno;
            rn.huno = huno;
            rn.capacity = mxsku;
            rn.dateassign = null;
            rn.datecreate = DateTimeOffset.Now;
            rn.depot = depot;
            rn.lines = new List<prln_md>();
            rn.orgcode = orgcode;
            rn.prepdate = DateTimeOffset.Now;
            rn.preppct = 0;
            rn.preptype = "P";
            rn.priority = 0;
            rn.site = site;
            rn.spcarea = "XD";
            rn.spcarticle = article;
            rn.spcorder = inorder;
            rn.tflow = "IO";
            rn.thcode = thcode;
            rn.routeno = routeno;
            rn.deviceID = "";
            rn.picker = "";
            rn.accncreate = accncode;
            rn.accnmodify = accncode;
            rn.procmodify = "api.prep.diststock";
            rn.przone = przone;
            rn.hutype = hutype;
            rn.setno = setno;
            rn.crsku = 0;
            rn.crweight = 0;
            rn.crvolume = 0;
            rn.mxsku = mxsku;
            rn.mxweight = mxweight;
            rn.mxvolume = mxvolume;

            return rn;
        }

        private List<prep_stock> prepstock(ref SqlDataReader r,string setno) {
            List<prep_stock> rn = new List<prep_stock>();
            try {
                // while (r.Read()) {
                rn.Add(new prep_stock() {
                    orgcode = r["orgcode"].ToString(),
                    site = r["site"].ToString(),
                    depot = r["depot"].ToString(),
                    stockid = r.GetDecimal(3),
                    huno = r["huno"].ToString(),
                    article = r["article"].ToString(),
                    pv = r["pv"].ToString().CInt32(),
                    lv = r["lv"].ToString().CInt32(),
                    qtysku = r["qtysku"].ToString().CInt32(),
                    qtypu = r["qtypu"].ToString().CInt32(),
                    qtyvolume = r["skuvolume"].ToString().CDecimal() * r["qtypu"].ToString().CInt32(),
                    qtyweight = r["skuweight"].ToString().CDecimal() * r["qtypu"].ToString().CInt32(),
                    loccode = r["loccode"].ToString(),
                    dateexp = (r.IsDBNull(11)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(11),
                    datemfg = (r.IsDBNull(12)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(12),
                    batchno = r["batchno"].ToString(),
                    lotno = r["lotno"].ToString(),
                    serialno = r["serialno"].ToString(),
                    spcarea = r["spcarea"].ToString(),

                });

                //}
                //r.Close();
                return rn;
            } catch(Exception ex) { throw ex; }
        }

        public async Task setrpnonprocess(string orgcode,string site,string depot,string spcarea,Int32 stockid,
        string article,Int32 pv,Int32 lv,string loccode,Int32 sourceqty,DateTimeOffset? datemfg,DateTimeOffset? dateexp,
        string batchno,string lotno,string serialno,DateTimeOffset? daterec,Int32 rsvqty,string setno,string huno,string tasktype,string targetloc) {
            SqlConnection oox = new SqlConnection(cnx);
            SqlCommand om = new SqlCommand();
            try {
                om.CommandText = sqlprepset_process_rpnonprocess;
                om.Connection = oox;
                om.snapsPar(orgcode,"orgcode");
                om.snapsPar(site,"site");
                om.snapsPar(depot,"depot");
                om.snapsPar(spcarea,"spcarea");
                om.snapsPar(stockid,"stockid");
                om.snapsPar(article,"article");
                om.snapsPar(pv,"pv");
                om.snapsPar(lv,"lv");
                om.snapsPar(loccode,"loccode");
                om.snapsPar(sourceqty,"sourceqty");
                om.snapsPar(datemfg,"datemfg");
                om.snapsPar(dateexp,"dateexp");
                om.snapsPar(batchno,"batchno");
                om.snapsPar(lotno,"lotno");
                om.snapsPar(serialno,"serialno");
                om.snapsPar(daterec,"daterec");
                om.snapsPar(rsvqty,"rsvqty");
                om.snapsPar(setno,"setno");
                om.snapsPar(huno,"huno");
                om.snapsPar(tasktype,"tasktype");
                om.snapsPar(targetloc,"targetloc");
                await om.snapsExecuteAsync();
            } catch(Exception ex) {
                logger.Error(orgcode,site,"",ex,ex.Message);
                throw ex;
            } finally { om.Dispose(); oox.Dispose(); }
        }

        public void setrpnblock(string orgcode,string site,string depot,string spcarea,Int32 stockid,
        string article,Int32 pv,Int32 lv,string loccode,Int32 rsvqty,
        string setno,string huno,string targetloc) {
            SqlConnection oox = new SqlConnection(cnx);
            SqlCommand om = new SqlCommand(sqlprepset_process_rpnonblock);
            try {
                om.Connection = oox;
                om.snapsPar(orgcode,"orgcode");
                om.snapsPar(site,"site");
                om.snapsPar(depot,"depot");
                om.snapsPar(spcarea,"spcarea");
                om.snapsPar(stockid,"stockid");
                om.snapsPar(article,"article");
                om.snapsPar(pv,"pv");
                om.snapsPar(lv,"lv");
                om.snapsPar(loccode,"loccode");
                om.snapsPar(rsvqty,"rsvqty");
                om.snapsPar(setno,"setno");
                om.snapsPar(huno,"huno");
                om.snapsPar(targetloc,"targetloc");
                om.snapsExecute();
            } catch(Exception ex) { logger.Error(orgcode,site,"",ex,ex.Message); throw ex; } finally { om.Dispose(); oox.Dispose(); }
        }

        public async Task<prepset> procstock(string orgcode,string site,string depot,string spcarea,string setno,string accncode) {
            SqlCommand cm = new SqlCommand(sqlprepset_line,cn);

            SqlConnection hucn = new SqlConnection(cnx); // Use for generate HU only

            //SqlCommand om = new SqlCommand("",cn);
            SqlDataReader r = null;
            List<SqlCommand> lm = new List<SqlCommand>();
            List<SqlCommand> om = new List<SqlCommand>();
            handerlingunit_ops ho = new handerlingunit_ops(hucn);
            handerlingunit_gen hg = new handerlingunit_gen();
            route_ops ro = new route_ops(cnx);
            route_md rm = new route_md();
            sequence_ops so = new sequence_ops();
            task_ops to = new task_ops(cn);
            List<task_md> tl = new List<task_md>();
            prep_ops po = new prep_ops(cnx);
            prepset ps = new prepset();
            List<prep_md> pls = new List<prep_md>();
            prep_md crprep = new prep_md();
            Int32 avaistock = 0;
            Int32 crreq = 0;
            DataTable odt = new DataTable();
            statisic_ops sso = new statisic_ops();
            List<prln_product> lp = new List<prln_product>();

            //Keep sku weight, volume for cal cap on HU
            Decimal pick_skuweight = 0;
            Decimal pick_skuvolume = 0;
            Int32 pick_ratio = 0;
            List<Decimal> hucap = new List<Decimal>();
            try {
                ps.orders = new List<prepsln>();
                //fetch order prep 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(spcarea,"spcarea");
                cm.snapsPar(setno,"setno");
                cm.snapsPar(accncode,"accncode");
                cm.snapsPar("prep.procstock","procmodify");
                cm.snapsPar("","article");
                cm.snapsPar("","pv");
                cm.snapsPar("","lv");
                cm.snapsPar(0,"qtyreqsku");
                cm.snapsPar(0,"rtoskuofpu");
                cm.snapsPar(0,"opsunit");
                cm.snapsPar("routeon","routeno");
                cm.snapsPar("","przone");

                // get parameter 
                parameter_ops op = new parameter_ops(cn);
                var parameters = op.getOutbound(orgcode,site,depot);
                var partialshipment = parameters.FirstOrDefault(x => x.pmtype == "order" && x.pmcode == "allowpartialship");

                // check allow partial process
                foreach(var ck in ps.orders) {
                    if(partialshipment.pmvalue == false) {
                        cm.CommandText = sqlcompare_stock;
                        if(cm.snapsScalarStrAsync().Result == "0") {
                            throw new Exception("Stock not enourgh with allow partial process order parameter");
                        }
                    }
                }

                // reset parameter to process
                cm.Parameters["przone"].Value = "";

                // update wm_prepset start
                cm.CommandText = sqlprepset_start;
                await cm.snapsExecuteAsync();

                // get wm_prepsln is WC
                cm.CommandText = sqlprepset_line;
                r = await cm.snapsReadAsync();
                if(!r.HasRows) {

                    await r.CloseAsync();
                    await cn.CloseAsync();
                    throw new Exception("Preparation set has completed can not reprocess again");

                } else {
                    // add line to process list item
                    while(await r.ReadAsync()) {
                        ps.orders.Add(prepsln_fill(ref r));
                    }
                    await r.CloseAsync();
                    await cn.CloseAsync();
                    
                    // read prep line
                    foreach(prepsln ln in ps.orders) {
                        Console.WriteLine("process order " + ln.ouorder + " qty " + ln.qtyskuorder.ToString() + " start ");

                        // normal process order
                        if(ln.przone != "RTV") {
                            //+ Check customer has create on prep list 
                            if(pls.Count(e => e.thcode == ln.thcode && e.przone == ln.przone && e.tflow == "IO") == 0) {
                                // get or generate route
                                rm = await ro.bgcth_getroute(orgcode,site,depot,ln.thcode,accncode,DateTimeOffset.Now,"P","IO");

                                //+ generate hu no
                                cm.CommandText = sqlprepset_hucode;

                                //cm.snapsPar(ln.przone,"przone");
                                cm.Parameters["przone"].Value = ln.przone;
                                ps.hucode = cm.snapsScalarStrAsync().Result;
                                hg.loccode = rm.loccode;
                                prepgenhu(ref hg,rm,ps.hucode);
                                await ho.generate(hg);

                                //+ generate prep setno 
                                pls.Add(prephead(ln,so.prepNext(ref cn),setno,ps.hucode,hg.huno,rm.routeno,accncode,hg.mxweight,hg.mxvolume,hg.mxsku.ToString().CInt32()));
                            }

                            /* 
                             * check stock on full pallet 
                             * find product on reserve
                             */
                            cm.CommandText = sqlprepset_process_stock_step1;
                            cm.Parameters["article"].Value = ln.article;
                            cm.Parameters["pv"].Value = ln.pv;
                            cm.Parameters["lv"].Value = ln.lv;
                            cm.Parameters["qtyreqsku"].Value = ln.qtyskuorder;
                            cm.Parameters["opsunit"].Value = ln.unitprep;
                            odt = await cm.snapsTableAsync();
                            crreq = ln.qtyskuorder;

                            //+ yes generate task full pallet pick 
                            //- no find proudct on picking

                            if(odt.Rows.Count > 0) {
                                foreach(DataRow lo in odt.Rows) {
                                    Console.WriteLine("process order " + ln.ouorder + " generate Full HU " + lo["huno"].ToString());

                                    preptask(ref tl,lo,orgcode,site,depot,spcarea,accncode,ln.ouorder,rm.loccode,"A",
                                    rm.routeno,rm.thcode,ln.ouln,ln.ourefno,ln.ourefln,setno); //Generate task 
                                    // stamp hu to route delivery
                                    await setrpnonprocess(
                                        orgcode,site,depot,spcarea,
                                        lo["stockid"].ToString().CInt32(),
                                        lo["article"].ToString(),
                                        lo["pv"].ToString().CInt32(),
                                        lo["lv"].ToString().CInt32(),
                                        lo["loccode"].ToString(),
                                        lo["qtysku"].ToString().CInt32(),
                                        lo["datemfg"].ToString().CDateTime(),
                                        lo["dateexp"].ToString().CDateTime(),
                                        lo["batchno"].ToString(),
                                        lo["lotno"].ToString(),
                                        lo["serialno"].ToString(),
                                        lo["daterec"].ToString().CDateTime(),
                                        (crreq > lo["qtysku"].ToString().CInt32()) ? lo["qtysku"].ToString().CInt32() : crreq,
                                        setno,
                                        lo["huno"].ToString(),"A",
                                        rm.loccode);

                                    // deduct stock qty
                                    crreq -= lo["qtysku"].ToString().CInt32();
                                }
                            }

                            /* 
                             * check stock on full pallet 
                             * find in picking if order qty > 0
                             */
                            if(crreq > 0) {
                                //+ check stock on picking
                                cm.CommandText = sqlprepset_process_stock_step2;
                                avaistock = cm.snapsScalarStrAsync().Result.CInt32();

                                Console.WriteLine("process order " + ln.ouorder + " article " + ln.article + " available " + avaistock.ToString());

                                // yse picking not have stock or not enourgh stock
                                if(avaistock < crreq) {
                                   
                                    // + Generate replenishment task 
                                    cm.CommandText = sqlprepset_process_stock_step201;
                                    odt = cm.snapsTableAsync().Result;
                                    List<prep_stock> ol = new List<prep_stock>();
                                    if(odt.Rows.Count > 0) {
                                        foreach(DataRow lo in odt.Rows) {
                                            ol.Add(preptask(ref tl,lo,orgcode,site,depot,spcarea,accncode,ln.ouorder,
                                            ln.loccode,"R","","",ln.ouln,ln.ourefno,ln.ourefln));
                                            //generate pick line 
                                            ol.Last().dateexp = ln.dateexp;
                                            ol.Last().datemfg = ln.datemfg;
                                            ol.Last().batchno = ln.batchno;
                                            ol.Last().lotno = ln.lotno;
                                            ol.Last().serialno = ln.serialno;
                                            ol.Last().qtysku = crreq;
                                            ol.Last().qtypu = crreq;
                                            ol.Last().qtyweight = crreq * ln.skuweight;
                                            ol.Last().qtyvolume = crreq * ln.skuvolume;

                                            crprep = pls.Find(e => e.thcode == ln.thcode && e.tflow == "IO");
                                            ol[0].loccode = ln.loccode; //Reassign picking instead of reserve for preparation                                        

                                            await setrpnonprocess(
                                                orgcode,site,depot,spcarea,
                                                lo["stockid"].ToString().CInt32(),
                                                lo["article"].ToString(),
                                                lo["pv"].ToString().CInt32(),
                                                lo["lv"].ToString().CInt32(),
                                                lo["loccode"].ToString(),
                                                lo["qtysku"].ToString().CInt32(),
                                                lo["datemfg"].ToString().CDateTime(),
                                                lo["dateexp"].ToString().CDateTime(),
                                                lo["batchno"].ToString(),
                                                lo["lotno"].ToString(),
                                                lo["serialno"].ToString(),
                                                lo["daterec"].ToString().CDateTime(),
                                                crreq,
                                                setno,
                                                lo["huno"].ToString(),
                                                "R",
                                                ln.loccode
                                            );

                                            prepline(ref crprep,ln.ouorder,ln.ouln,ln.unitprep.ToString(),ol,setno,false,isAutoblockonprocess: false);
                                            Console.WriteLine("order  " + ln.ouorder + " prep  " + crprep.prepno + " article " + ln.article + " qty : " + crreq);
                                            Console.WriteLine("generate rpn HU " + lo["huno"].ToString());
                                        }
                                    }
                                } else {
                                    Console.WriteLine("process order " + ln.ouorder + " Finding pick ");

                                    //+ yes generate pick line 
                                    cm.CommandText = sqlprepset_process_stock_step3;

                                    //Find the last HU of store 
                                    crprep = pls.Find(e => e.thcode == ln.thcode && e.tflow == "IO");

                                    r = await cm.snapsReadAsync();
                                    if(!r.HasRows) {
                                        Console.WriteLine("process order " + ln.ouorder + " Finding pick not found stock on picking ");
                                    }

                                    // read stock soh
                                    while(await r.ReadAsync()) {
                                        pick_skuweight = r["skuweight"].ToString().CDecimal();
                                        pick_skuvolume = r["skuvolume"].ToString().CDecimal();
                                        pick_ratio = r["rtoskuofpu"].ToString().CInt32();

                                        var ressku = 0;
                                        var respu = 0;
                                        var pndpu = 0;
                                        decimal resweight = 0;
                                        decimal resvolume = 0;
                                        hucap.Clear();
                                        // cap volume 
                                        hucap.Add(Math.Ceiling((crprep.mxvolume - crprep.crvolume) / pick_skuvolume));
                                        // cap weight 
                                        hucap.Add(Math.Ceiling((crprep.mxweight - crprep.crweight) / pick_skuweight));

                                        // Calculate currec weight, volum of hu compare with order 
                                        // Get minimum sku for current hu      
                                        //+ If HU not enourgh then generate new HU  
                                        Console.WriteLine("Allow " + hucap.Min().ToString() + " order " + r["qtypu"].ToString());

                                        if(hucap.Min() < r["qtypu"].ToString().CInt32()) {

                                            pndpu = hucap.Min().ToString().CInt32();
                                            respu = r["qtypu"].ToString().CInt32() - hucap.Min().ToString().CInt32();
                                            ressku = respu * pick_ratio;
                                            resweight = respu * pick_skuweight;
                                            resvolume = respu * pick_skuvolume;
                                            if(pndpu > 0) {
                                                prepline(ref crprep,
                                                    ln.ouorder,
                                                    ln.ouln,
                                                    ln.unitprep.ToString(),
                                                    prepstock(ref r,setno),
                                                    setno,
                                                    true,
                                                    pndpu * pick_ratio, // SKU
                                                    pndpu, // PU
                                                    pndpu * pick_skuweight,
                                                    pndpu * pick_skuvolume);

                                                Console.WriteLine("generate pick line " + r["huno"].ToString());

                                            }

                                            //Close old HU
                                            crprep.tflow = "ED";

                                            // generate a new hu
                                            prepgenhu(ref hg,rm,ps.hucode);
                                            await ho.generate(hg);

                                            //+ generate prep setno 
                                            pls.Add(prephead(ln,so.prepNext(ref hucn),setno,ps.hucode,hg.huno,rm.routeno,accncode,
                                            hg.mxweight,hg.mxvolume,hg.mxsku.ToString().CInt32()));

                                            crprep = pls.Find(e => e.thcode == ln.thcode && e.tflow == "IO");
                                            prepline(ref crprep,ln.ouorder,ln.ouln,ln.unitprep.ToString(),prepstock(ref r,setno),
                                                        setno,
                                                        true,
                                                        respu * pick_ratio, // SKU
                                                        respu, // PU
                                                        respu * pick_skuweight,
                                                        respu * pick_skuvolume);

                                        } else {
                                            //- Use old HU
                                            prepline(ref crprep,ln.ouorder,ln.ouln,ln.unitprep.ToString(),prepstock(ref r,setno),setno);
                                            Console.WriteLine("order  " + ln.ouorder + " prep  " + crprep.prepno + " article " + ln.article + " qty : " + crreq);

                                        }
                                    }// while

                                    await r.CloseAsync();
                                    await cn.CloseAsync();
                                }

                            }
                        } else if(ln.przone == "RTV") {

                            if(pls.Count(e => e.thcode == ln.thcode && e.przone == ln.przone && e.tflow == "IO") == 0) {
                                //+ check route for customer ( with auto generate )
                                rm = await ro.bgcth_getroute(orgcode,site,depot,ln.thcode,accncode,DateTimeOffset.Now);

                                //+ generate hu no
                                cm.CommandText = sqlprepset_hucode;

                                //cm.snapsPar(ln.przone,"przone");
                                cm.Parameters["przone"].Value = ln.przone;
                                ps.hucode = cm.snapsScalarStrAsync().Result;
                                hg.loccode = rm.loccode;
                                prepgenhu(ref hg,rm,ps.hucode);
                                await ho.generate(hg);

                                //+ generate prep setno 
                                pls.Add(prephead(ln,so.prepNext(ref cn),setno,ps.hucode,hg.huno,rm.routeno,accncode,hg.mxweight,hg.mxvolume,hg.mxsku.ToString().CInt32()));

                            }

                            Console.WriteLine("process order " + ln.ouorder + " Finding pick ");
                            cm.Parameters["article"].Value = ln.article;
                            cm.Parameters["pv"].Value = ln.pv;
                            cm.Parameters["lv"].Value = ln.lv;
                            cm.Parameters["qtyreqsku"].Value = ln.qtyskuorder;
                            cm.Parameters["opsunit"].Value = ln.unitprep;
                            crreq = ln.qtyskuorder;

                            //+ yes generate pick line 
                            cm.CommandText = sqlprepset_process_stock_step3_forrtv;

                            //Find the last HU of store 
                            crprep = pls.Find(e => e.thcode == ln.thcode && e.tflow == "IO");

                            r = await cm.snapsReadAsync();
                            if(!r.HasRows) {
                                Console.WriteLine("process order " + ln.ouorder + " Finding pick not found stock on picking ");
                            }
                            while(await r.ReadAsync()) {
                                pick_skuweight = r["skuweight"].ToString().CDecimal();
                                pick_skuvolume = r["skuvolume"].ToString().CDecimal();
                                pick_ratio = r["rtoskuofpu"].ToString().CInt32();

                                prepline(ref crprep,ln.ouorder,ln.ouln,ln.unitprep.ToString(),prepstock(ref r,setno),setno);
                                Console.WriteLine("order  " + ln.ouorder + " prep  " + crprep.prepno + " article " + ln.article + " qty : " + crreq);

                            }
                            await r.CloseAsync();
                            await cn.CloseAsync();
                        } else {
                            throw new Exception("preparation zone not found");
                        }
                    } // end loop order

                    //execute to db
                    //pls = pls.Where(e => e.lines.Count > 0).ToList(); //Filter only prep has line
                    var prep = pls.Where(e => e.lines.Count > 0).ToList();
                    await po.upsert(prep);

                    //generate task 
                    if(tl.Count > 0) {
                        await to.generate(tl);
                    }

                    //Update order status 
                    cm.CommandText = sqlprepset_process_stock_step4;
                    await cm.snapsExecuteAsync();

                    //update order status 
                    cm.CommandText = sqlprepset_process_stock_step5;
                    await cm.snapsExecuteAsync();
                    cm.CommandText = sqlprepset_process_stock_step6;
                    await cm.snapsExecuteAsync();

                    //update process has completed
                    cm.CommandText = sqlprepset_finish_step1;
                    await cm.snapsExecuteAsync();
                    cm.CommandText = sqlprepset_finish_step2;
                    await cm.snapsExecuteAsync();
                    cm.CommandText = sqlprepset_finish_step2_1;
                    await cm.snapsExecuteAsync();
                    cm.CommandText = sqlprepset_finish_step3;
                    await cm.snapsExecuteAsync();

                    //update product statistic by prep
                    lp.AddRange(
                            from prep_md px in pls
                            from prln_md pn in px.lines
                            where lp.Count(e => e.article == pn.article && e.pv == pn.pv && e.lv == pn.lv) == 0
                            select new prln_product { article = pn.article,pv = pn.pv,lv = pn.lv });

                    //update product statistic by task
                    lp.AddRange(from task_md th in tl
                                from taln_md td in th.lines
                                where lp.Count(e => e.article == td.article && e.pv == td.pv && e.lv == td.lv) == 0
                                select new prln_product { article = td.article,pv = td.pv,lv = td.lv });

                    // create product statistic list
                    foreach(prln_product lx in lp) {
                        foreach(SqlCommand nm in sso.inboundstock(orgcode,site,depot,lx.article,lx.pv.ToString(),lx.lv.ToString())) { om.Add(nm); };
                    }

                    //update hu statistic 
                    foreach(string calhu in pls.Select(e => e.huno).Distinct()) {
                        om.Add(sso.HUforcast(orgcode,site,depot,calhu,accncode));

                        //If hu is empty must be clear 
                        if(pls.Find(e => e.huno == calhu).lines.Count == 0) {
                            om.Add(sqlprepset_finis_step4_cleahu.snapsCommand());
                            om.Last().snapsPar(orgcode,"orgcode");
                            om.Last().snapsPar(site,"site");
                            om.Last().snapsPar(depot,"depot");
                            om.Last().snapsPar(calhu,"huno");
                        }
                        if(pls.Find(e => e.huno == calhu).lines == null) {
                            om.Add(sqlprepset_finis_step4_cleahu.snapsCommand());
                            om.Last().snapsPar(orgcode,"orgcode");
                            om.Last().snapsPar(site,"site");
                            om.Last().snapsPar(depot,"depot");
                            om.Last().snapsPar(calhu,"huno");
                        }
                    }

                    //update route statistic                            
                    foreach(string calroute in pls.Select(e => e.routeno).Distinct()) {
                        om.Add(sso.route(orgcode,site,depot,calroute,accncode));
                    }

                    //update full pallet route statistic                            
                    //foreach (string calroute in tl.Select(e => e.routeno).Distinct())
                    //{
                    //    if(pls.Count(x=>x.routeno == calroute) == 0)
                    //    {
                    //        om.Add(sso.route(orgcode, site, depot, calroute, accncode));
                    //    }
                    //}


                    // UPDATE 
                    await om.snapsExecuteTransAsync(cn);

                    /*
                     * PRINT LABEL FULLPALLET
                     */

                    var tkfull = tl.Where(x => x.tasktype == "A").ToList();
                    string baseAddress = await printApiUri(orgcode,site,depot);
                    foreach(var tk in tkfull) {
                        foreach(var huno in tk.lines.Select(x => x.sourcehuno).Distinct()) {
                            await printFullpallet(baseAddress,orgcode,site,depot,huno,accncode);
                        }
                       
                    }

                    // DELETE ROW SELECT
                    List<SqlCommand> sm = new List<SqlCommand>();
                    foreach(var pep in pls) {
                        //Clear Selection Checkbox
                        var oulist = pep.lines
                            .Select(o => o.ouorder)
                            .Distinct()
                            .ToList();

                        foreach(string ouorder in oulist) {
                            var csl = new SqlCommand();
                            csl.snapsPar(orgcode,"orgcode");
                            csl.snapsPar(site,"site");
                            csl.snapsPar(depot,"depot");
                            csl.snapsPar(ouorder,"ouorder");
                            csl.CommandText = sqlUnselected;
                            sm.Add(csl);
                        }
                    }


                    // DELETE ROW SELECT
                    if(sm.Count > 0)
                        await sm.snapsExecuteTransAsync(cn);


                    //Fill for return
                    cm.CommandText = sqlprepset_line.Replace("l.tflow = 'WC' and ","");
                    ps.orders = new List<prepsln>();
                    r = await cm.snapsReadAsync();
                    while(await r.ReadAsync()) { 
                        ps.orders.Add(prepsln_fill(ref r)); 
                    }
                    await r.CloseAsync();
                    await cn.CloseAsync();

                    return ps;

                }
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message);
                throw ex;
            } finally {
                await cm.DisposeAsync();
                if(r != null) { await r.DisposeAsync(); }
                lm.ForEach(e => e.Dispose());
                ho.Dispose();
                hg = null;
                ro.Dispose();
                rm = null;
                so.Dispose();
                to.Dispose();
                tl.Clear();
                po.Dispose();
                pls.Clear();
                crprep = null;
                avaistock = 0;
                crreq = 0;
                odt.Dispose();
                lp.Clear(); lp = null;
            }
        }


        public async Task<prepset> procdistb(string orgcode,string site,string depot,string spcarea,string setno,string accncode) {
            SqlCommand cm = new SqlCommand("",cn);
            SqlCommand vm = new SqlCommand("",cn);
            SqlDataReader r = null;
            List<SqlCommand> lm = new List<SqlCommand>();
            route_ops ro = new route_ops(cnx);
            route_md rm = new route_md();
            sequence_ops so = new sequence_ops();
            prep_ops po = new prep_ops(cnx);
            prep_md crprep = new prep_md();
            List<prep_md> pls = new List<prep_md>();
            DataTable odt = new DataTable();
            List<route_ls> rl = new List<route_ls>();
            prepset ps = new prepset();
            Int32 crstock = 0;
            string nprep = "";
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(accncode,"accncode");
                cm.snapsPar("XD","spcarea");
                cm.snapsPar("","inorder");
                cm.snapsPar("","article");
                cm.snapsPar(0,"pv");
                cm.snapsPar(0,"lv");
                cm.snapsPar("","huno");
                cm.snapsPar(0,"stockid");
                cm.snapsPar(setno,"setno");
                cm.snapsPar(nprep,"prepno");
                cm.snapsPar(accncode,"accncreate");
                cm.snapsPar("prep.procdist","procmodify");

                vm.snapsPar(orgcode,"orgcode");
                vm.snapsPar(site,"site");
                vm.snapsPar(depot,"depot");
                vm.snapsPar(setno,"setno");
                vm.snapsPar("","article");
                vm.snapsPar(0,"pv");
                vm.snapsPar(0,"lv");
                vm.snapsPar("","inorder");

                cm.CommandText = " select distinct p.przone,hutype,dishuno huno,thcode,ouorder,article,pv,lv,setno,disinorder from wm_prepsln p, wm_loczn z " +
                " where p.orgcode = z.orgcode and p.site = z.site and p.depot = z.depot and p.przone = z.przone " +
                " and setno = @setno and p.tflow = 'WC' and p.orgcode = @orgcode and p.site = @site and p.depot = @depot ";

                vm.CommandText = @" 
                select o.*, a.unitprep, dbo.get_ratiopu_prep(o.orgcode, o.site, o.depot, o.article, o.pv, o.lv) ratiopu, s.priority,z.lscode loccode, 
                a.spcdistzone, a.skuweight, a.skuvolume from 
                (select o.orgcode, o.site, o.depot, o.spcarea, o.ouorder, o.thcode, l.ouln, l.ourefno, l.ourefln, 
                        l.article, l.pv, l.lv, qtyreqsku, qtyreqpu
                from wm_outbound o, wm_outbouln l where o.orgcode = l.orgcode and o.site = l.site and o.depot = l.depot 
                    and o.ouorder = l.ouorder and o.inorder = l.inorder  and qtyreqsku > 0 
                    and o.orgcode = @orgcode and o.site = @site and o.depot = @depot and l.article = @article and l.pv = @pv and l.lv = @lv 
                    and o.inorder = @inorder and o.tflow in ('IO','PC') and o.tflow in ('IO','PC')
                    ) o
                join wm_product a 
                    on o.orgcode = o.orgcode and o.site = a.site and o.depot = a.depot and o.article = a.article and o.pv = a.pv and o.lv = a.lv
                join (select * from wm_loczp where spcarea = 'XD' and orgcode = @orgcode and site = @site and depot = @depot  and tflow = 'IO'  ) z
                    on o.orgcode = z.orgcode and o.site = z.site and o.depot = z.depot and a.spcdistzone = z.przone and o.thcode = z.spcthcode  
                left join wm_shareprln s
                    on o.orgcode = o.orgcode and o.site = s.site and o.depot = s.depot and a.spcdistshare = s.shprep and o.thcode = s.thcode ";

                odt = cm.snapsTableAsync().Result;

                //Check customer route 
                foreach(DataRow rw in odt.Rows) {
                    rm = await ro.bgcth_getroute(orgcode,site,depot,rw["thcode"].ToString(),accncode,DateTimeOffset.Now,"P","IO");
                    rl.Add(new route_ls() {
                        orgcode = rm.orgcode,
                        site = rm.site,
                        depot = rm.depot,
                        routeno = rm.routeno,
                        thcode = rm.thcode
                    });

                    //set prep hrader 
                    //generate distribution plan 
                    nprep = so.prepNext(ref cn);
                    pls.Add(
                        disthead(
                            orgcode,site,depot,accncode,
                            rw["przone"].ToString(),
                            nprep,
                            rw["hutype"].ToString(),
                            rw["huno"].ToString(),
                            rl.Find(e => e.thcode == rw["thcode"].ToString()).routeno,
                            1000,1000,999999,
                            rw["disinorder"].ToString(),
                            rw["thcode"].ToString(),
                            rw["article"].ToString(),
                            rw["pv"].ToString(),
                            rw["lv"].ToString(),
                            setno
                        )
                    );
                }


                cm.CommandText = sqlprepset_start;
                await cm.snapsExecuteAsync();

                //cm.CommandText = sqlprepset_process_dist_step1;
                cm.CommandText = "select * from wm_prepsln where setno = @setno and orgcode = @orgcode and site = @site and depot = @depot and tflow = 'WC'";

                //Loop HU stock for distribute 
                foreach(DataRow rw in cm.snapsTableAsync().Result.Rows) {
                    //orgcode, site, depot, article, pv, lv, inorder, huno, current stock 
                    crstock = rw["qtyskuops"].ToString().CInt32();

                    vm.Parameters["article"].Value = rw["article"].ToString();
                    vm.Parameters["pv"].Value = rw["pv"].ToString().CInt32();
                    vm.Parameters["lv"].Value = rw["lv"].ToString().CInt32();
                    vm.Parameters["inorder"].Value = rw["disinorder"].ToString();

                    foreach(DataRow ln in vm.snapsTableAsync().Result.Rows) {
                        Console.WriteLine(ln["qtyreqsku"].ToString() + " from " + rw["dishuno"].ToString());

                        if(crstock >= ln["qtyreqsku"].ToString().CInt32()) {

                            //generate pick line
                            //use qtyreqsku 
                            pls.Find(e => e.huno == rw["dishuno"].ToString()).lines.Add(
                                prln_getmd(orgcode,site,depot,spcarea,
                                rw["dishuno"].ToString(),nprep,
                                rw["disstockid"].ToString().CInt32(),
                                ln,
                                ln["qtyreqsku"].ToString().CInt32(),
                                ln["qtyreqsku"].ToString().CInt32(),//bug 
                                rw["batchno"].ToString(),
                                rw["lotno"].ToString(),
                                rw["datemfg"].ToString().CDateTime(),
                                rw["dateexp"].ToString().CDateTime(),
                                rw["serialno"].ToString(),
                                null,
                                "",//inagrn
                                "", //inagrno
                                pls.Find(e => e.huno == rw["dishuno"].ToString()).lines.Count + 1
                                )
                             );
                            crstock -= ln["qtyreqsku"].ToString().CInt32();
                            Console.WriteLine(crstock);
                        } else if(crstock > 0 && ln["qtyreqsku"].ToString().CInt32() > crstock) {

                            //generate rest stock to pick line 
                            //use crstock
                            pls.Find(e => e.huno == rw["dishuno"].ToString()).lines.Add(
                               prln_getmd(orgcode,site,depot,spcarea,
                               rw["dishuno"].ToString(),nprep,
                               rw["disstockid"].ToString().CInt32(),
                               ln,
                               ln["qtyreqsku"].ToString().CInt32() >= crstock ? crstock : ln["qtyreqsku"].ToString().CInt32(),
                               ln["qtyreqsku"].ToString().CInt32() >= crstock ? crstock : ln["qtyreqsku"].ToString().CInt32(),//bug 
                               rw["batchno"].ToString(),
                               rw["lotno"].ToString(),
                               rw["datemfg"].ToString().CDateTime(),
                               rw["dateexp"].ToString().CDateTime(),
                               rw["serialno"].ToString(),
                               null,
                               "",//inagrn
                               "", //inagrno,
                               pls.Find(e => e.huno == rw["dishuno"].ToString()).lines.Count + 1
                           ));
                            crstock = 0;
                            Console.WriteLine(crstock);
                        }
                    }

                }


                // ps.orders = new List<prepsln>();
                // while(await r.ReadAsync()) { ps.orders.Add( prepsln_fill( ref r) ) ; }
                // await r.CloseAsync(); 
                // await cn.CloseAsync();

                //foreach(prepsln ln in ps.orders) { 
                //Find stock has receipted to create distribution plan 
                // cm.Parameters["inorder"].Value = ln.ouorder;
                // cm.Parameters["article"].Value = ln.article;
                // cm.Parameters["pv"].Value = ln.pv;
                // cm.Parameters["lv"].Value = ln.lv;
                // cm.Parameters["huno"].Value = ln.dishuno;
                // cm.Parameters["stockid"].Value = ln.disstockid;                            
                // cm.Parameters["prepno"].Value = nprep;
                // cm.Parameters["huno"].Value = ln.dishuno;
                // cm.Parameters["inorder"].Value = ln.ouorder;

                // cm.CommandText = sqlprepset_process_dist_step1;
                // r = await cm.snapsReadAsync();


                // while (await r.ReadAsync())
                // {
                //     pls.Find(e => e.huno == r["hunosource"].ToString()).lines.Add(prln_getmd(ref r));
                // }
                // await r.CloseAsync();
                // await cn.CloseAsync();

                //upsert prepset, 
                await po.upsert(pls);

                //update order status 
                cm.CommandText = sqlprepset_process_dist_step2;
                await cm.snapsExecuteAsync();

                //update order 
                cm.CommandText = sqlprepset_process_dist_step3;
                await cm.snapsExecuteAsync();

                // }

                return ps;

                //}

            } catch(Exception ex) {
                throw ex;
            } finally {
                await cm.DisposeAsync();
                if(r != null) { await r.DisposeAsync(); }
                lm.ForEach(e => e.Dispose());
                ro.Dispose();
                rm = null;
                so.Dispose();
                po.Dispose();
                crprep = null;
            }
        }


        public async Task<string> printApiUri(string orgcode,string site,string depot) {
            try {
                // get document api config
                string command =
                    @"select top 1 bnflex1 from wm_binary where orgcode=@orgcode and site=@site and depot=@depot 
                    and apps='WMS' and bntype = 'PRINTER' and bncode ='LABEL'";

                SqlCommand pm = new SqlCommand(command,cn);
                pm.snapsPar(orgcode,"orgcode");
                pm.snapsPar(site,"site");
                pm.snapsPar(depot,"depot");
                string baseAddress = await pm.snapsScalarStrAsync();
                return baseAddress;
            } catch(Exception) {
                return "";
            }
        }
        private async Task<bool> printFullpallet(string baseAddress,string orgcode,string site,string depot,string huno,string acncode) {
            try {

                using(var httpClient = new System.Net.Http.HttpClient()) {
                    httpClient.Timeout = TimeSpan.FromSeconds(60);
                    httpClient.BaseAddress = new Uri(baseAddress);
                    using(var form = new MultipartFormDataContent()) {
                        form.Add(new StringContent(orgcode),"orgcode");
                        form.Add(new StringContent(site),"site");
                        form.Add(new StringContent(depot),"depot");
                        form.Add(new StringContent(huno),"huno");
                        var httpResponse = await httpClient.PostAsync("print/printFullpallet",form);
                        return httpResponse.IsSuccessStatusCode;
                    }
                }
            } catch(Exception ex) {
                logger.Error(orgcode,site,acncode,ex,ex.Message);
                return false;
            }
        }

        private async Task<bool> printShipLabel(string orgcode,string site,string depot,string huno,string acncode) {
            try {
                string baseAddress = await printApiUri(orgcode,site,depot);
                using(var httpClient = new System.Net.Http.HttpClient()) {
                    httpClient.Timeout = TimeSpan.FromSeconds(60);
                    httpClient.BaseAddress = new Uri(baseAddress);
                    using(var form = new MultipartFormDataContent()) {
                        form.Add(new StringContent(orgcode),"orgcode");
                        form.Add(new StringContent(site),"site");
                        form.Add(new StringContent(depot),"depot");
                        form.Add(new StringContent(huno),"huno");
                        var httpResponse = await httpClient.PostAsync("print/printlabelshipped",form);
                        return httpResponse.IsSuccessStatusCode;
                    }
                }
            } catch(Exception ex) {
                logger.Error(orgcode,site,acncode,ex,ex.Message,"HU:",huno);
                return false;
            }
        }


        // public async Task procstock(List<outbound_ls> o){ 

        // SqlCommand cm = new SqlCommand("",cn);
        // SqlCommand cmr = new SqlCommand("",cn);
        // DataTable om = new DataTable(); 
        // DataTable on = new DataTable();
        // SqlDataReader r ;
        // List<prep_md> pls = new List<prep_md>();
        // prep_ops ops = new prep_ops(cn.ConnectionString);

        // handerlingunit_ops ho = new handerlingunit_ops(cn.ConnectionString);
        // handerlingunit_gen hg = new handerlingunit_gen(); 
        // sequence_ops so = new sequence_ops();

        // prep_md crprep ;
        // Int32   crorderrq = 0;
        // String  crouorder = "";
        // Int32   crouln = 0;

        // string crthcode = "";
        // string crshipdock = "";
        // string crroute = "";


        // prep_stock crstock = new prep_stock();
        // try { 

        //     cmr.CommandText = string.Format("  select l.*,o.thcode from wm_outbound o, wm_outbouln l where o.orgcode = l.orgcode and o.site = l.site and o.depot = l.depot and o.ouorder = l.ouorder "+ 
        //     "  and o.tflow = 'IO' and l.tflow = 'IO' and o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.ouorder in ('{0}') ",o[0].ouorder);
        //     cmr.snapsPar(o[0].orgcode.ToString(),"orgcode"); 
        //     cmr.snapsPar(o[0].site.ToString(),"site");
        //     cmr.snapsPar(o[0].depot.ToString(),"depot");
        //     cmr.snapsPar(o[0].thcode,"thcode");

        //     //fetch order list 
        //     om = await cmr.snapsTableAsync();
        //      foreach(DataRow rn in om.Rows) {

        //         //primary parameter 
        //         if (cm.Parameters.Count == 0){ 
        //             cm.snapsPar(rn["orgcode"].ToString(),"orgcode"); 
        //             cm.snapsPar(rn["site"].ToString(),"site");
        //             cm.snapsPar(rn["depot"].ToString(),"depot");
        //             cm.snapsPar(rn["thcode"].ToString(),"spcarea");
        //             cm.snapsPar(rn["ouorder"].ToString(),"ouorder");
        //             cm.snapsPar(rn["accnmodify"].ToString(),"accnmodify");

        //             cm.snapsPar(rn["article"].ToString(),"article");
        //             cm.snapsPar(rn["pv"].ToString(),"pv");
        //             cm.snapsPar(rn["lv"].ToString(),"lv");
        //             cm.snapsParsysdateoffset();
        //         }else { 
        //             cm.Parameters["spcarea"].Value = rn["thcode"].ToString();
        //             cm.Parameters["ouorder"].Value = rn["ouorder"].ToString();
        //             cm.Parameters["article"].Value = rn["article"].ToString();
        //             cm.Parameters["pv"].Value = rn["pv"].ToString();
        //             cm.Parameters["lv"].Value = rn["lv"].ToString();
        //         }
        //         crorderrq = rn["qtyreqpu"].ToString().CInt32(); 
        //         crouorder = rn["ouorder"].ToString();
        //         crouln = rn["ouln"].ToString().CInt32();

        //         //Check customer on prep is exists 
        //         if(pls.Count(c => c.thcode == rn["thcode"].ToString()) == 0) { 

        //             cmr.CommandText = sqlprep_stock_getroute;
        //             on = await cmr.snapsTableAsync();

        //             crthcode = on.Rows[0]["thcode"].ToString();
        //             crroute = on.Rows[0]["routeno"].ToString();
        //             crshipdock = on.Rows[0]["loccode"].ToString();

        //             hg.orgcode = rn["orgcode"].ToString();
        //             hg.site = rn["site"].ToString();
        //             hg.depot = rn["depot"].ToString();
        //             hg.loccode = crshipdock;
        //             hg.thcode = crthcode;
        //             hg.hutype = "PL01";
        //             hg.mxsku = 9999;
        //             hg.mxvolume = 2000;
        //             hg.mxweight = 1000;
        //             hg.quantity = 1;
        //             hg.spcarea = "ST";
        //             hg.accncreate = "me";
        //             hg.accnmodify = "me"; 
        //             hg.promo = "";
        //             hg.priority = 0;
        //             hg.routeno = crroute;

        //             await ho.generate(hg);

        //             //assign route no
        //             pls.Add(new prep_md());
        //             //generate prep no 
        //             //cm.CommandText= "select  next value for seq_huno rsl";
        //             //pls.Last().prepno = cm.snapsScalarStrAsync().Result;
        //             pls.Last().prepno = so.prepNext(ref cn);

        //             //generate huno 
        //            // cm.CommandText= "select next value for seq_prep rsl";
        //             //pls.Last().huno = cm.snapsScalarStrAsync().Result;
        //             pls.Last().huno = hg.huno;

        //             pls.Last().capacity = 10000;
        //             pls.Last().dateassign = new DateTimeOffset();
        //             pls.Last().datecreate = new DateTimeOffset();
        //             pls.Last().depot = rn["depot"].ToString();
        //             pls.Last().lines = new List<prln_md>();
        //             pls.Last().orgcode = rn["orgcode"].ToString();
        //             pls.Last().prepdate = new DateTimeOffset();
        //             pls.Last().preppct = 0;
        //             pls.Last().preptype = "ST";
        //             pls.Last().priority = 0;
        //             pls.Last().site = rn["site"].ToString();
        //             pls.Last().spcarea = rn["spcarea"].ToString();
        //             pls.Last().spcarticle = "";
        //             pls.Last().spcorder = "";
        //             pls.Last().tflow = "IO";
        //             pls.Last().thcode = rn["thcode"].ToString();
        //             pls.Last().routeno = crroute;
        //             pls.Last().deviceID = "";
        //             pls.Last().picker = "";
        //             pls.Last().accncreate = "";
        //             pls.Last().accnmodify = "";
        //             pls.Last().procmodify = "api.outbound.procstock";


        //             //this.rqnprep(ref cm,rn,ref pls); 

        //         }

        //         // Check stock on picking
        //         cm.CommandText = ops.prep_getpickstock;
        //         r = await cm.snapsReadAsync();
        //         crstock = getpickingstock(ref r);

        //         // If picking is not enoug
        //         if (crstock.qtysku < crorderrq) { 
        //                 //Check stock on Reserver 
        //                 // cm.CommandText = oiv.prep_getreservestock;
        //                 // crstock = cm.ExecuteScalarAsync().Result.ToString().CInt32();
        //                 // //If yes > create rpn task
        //                 // if (crstock > 0){
        //                 //     //get picking to locate stock 
        //                 // }
        //                 // //If no exit 
        //         }else{    
        //             crprep = pls.Where(c => c.thcode == rn["thcode"].ToString()).First();
        //             crprep.lines.Add(new prln_md());
        //             crprep.lines.Last().accncreate = "";
        //             crprep.lines.Last().accnmodify = "";
        //             crprep.lines.Last().article = crstock.article;
        //             crprep.lines.Last().pv = crstock.pv;
        //             crprep.lines.Last().lv = crstock.lv;
        //             crprep.lines.Last().barcode = "";
        //             crprep.lines.Last().batchno = crstock.batchno;
        //             crprep.lines.Last().datecreate = DateTimeOffset.Now;
        //             crprep.lines.Last().dateexp = crstock.dateexp;
        //             crprep.lines.Last().datemfg = crstock.datemfg;
        //             crprep.lines.Last().datemodify = DateTimeOffset.Now;
        //             crprep.lines.Last().datepick = null;
        //             crprep.lines.Last().depot = crstock.depot;
        //             crprep.lines.Last().hunosource = crstock.huno;
        //             crprep.lines.Last().loccode = crstock.loccode;
        //             crprep.lines.Last().lotno = crstock.lotno;
        //             crprep.lines.Last().lv = crstock.lv;
        //             crprep.lines.Last().orgcode = crstock.orgcode;
        //             crprep.lines.Last().site = crstock.site;
        //             crprep.lines.Last().depot = crstock.depot;
        //             crprep.lines.Last().qtyskuorder = crorderrq;
        //             crprep.lines.Last().serialno = crstock.serialno;
        //             crprep.lines.Last().site = crstock.site;
        //             crprep.lines.Last().spcarea = crstock.spcarea;
        //             crprep.lines.Last().stockid = crstock.stockid;
        //             crprep.lines.Last().ouln = crouln;
        //             crprep.lines.Last().ouorder = crouorder;
        //             crprep.lines.Last().prepln = crprep.lines.Count + 1;
        //             crprep.lines.Last().pv = crstock.pv;
        //             crprep.lines.Last().tflow = "IO";
        //             crprep.lines.Last().unitprep = 1;
        //         }                    
        //     }

        //     //upsert prep 
        //     await ops.upsert(pls);

        //     //update order processed
        //     cm.CommandText = String.Format(ops.prep_sqlupdprc, o[0].ouorder);
        //     await cm.snapsExecuteAsync();

        //     //update stock block on article

        //     //update stock qtyprep on stsock line 


        // }catch (Exception ex) { throw ex; }
        // finally { if(cm!=null) { cm.Dispose(); cmr.Dispose(); }  } 

        // }

        // public async Task procdistb(List<outbound_ls> o){  
        //     SqlCommand cm = new SqlCommand("",cn);
        //     DataTable tb = new DataTable();
        //     SqlDataReader r;

        //     prep_ops ops = new prep_ops(cn.ConnectionString );

        //     String slchuno = "";
        //     String slcarticle = "";
        //     String slcbatchno = "";
        //     String slclotno = "";
        //     String slcinorder = "";
        //     Int32 slcstockid = 0;
        //     DateTimeOffset? slcmfgdate = null;
        //     DateTimeOffset? slcexpdate = null;

        //     List<procdist_stock> lsstock = new List<procdist_stock>();
        //     List<procdist_order> lsorder = new List<procdist_order>();
        //     List<prep_md> lsplan = new List<prep_md>();

        //     procdist_order crorder;
        //     Int32 nxprepln = 1;

        //     try { 

        //         // get stock on hand of inbound 
        //         cm.CommandText = "select	s.orgcode, s.site, s.depot, s.spcarea , s.stockid, s.huno, s.article,s.pv, s.lv, s.qtysku - isnull(b.qtysku,0) qtysku, " + 
        //         "		s.qtypu - isnull(b.qtypu,0) qtypu,s.batchno, s.lotno, s.serialno,s.datemfg, s.dateexp,s.loccode, c.barcode" + 
        //         " from wm_stock s left join wm_stobc b on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.stockid = b.stockid and s.article = b.article and s.pv = b.pv and s.lv = b.lv " +
        //         " left join wm_barcode c on s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.article = c.article and s.pv = c.pv and s.lv = c.lv and c.tflow = 'IO' " +
        //         " where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.spcarea = @spcarea and s.inrefno = @inrefno and s.article = @product order by s.huno" ; //sql_prcdist_1_getstock
        //         cm.snapsPar(o[0].orgcode,"orgcode"); 
        //         cm.snapsPar(o[0].site,"site");
        //         cm.snapsPar(o[0].depot,"depot");
        //         cm.snapsPar(o[0].spcarea,"spcarea");
        //         cm.snapsPar(o[0].disinbound,"inrefno");
        //         cm.snapsPar(o[0].disproduct,"product");
        //         r = await cm.snapsReadAsync();
        //         while(await r.ReadAsync()) {                        
        //            lsstock.Add(new procdist_stock() { 
        //                orgcode = r["orgcode"].ToString(), 
        //                article = r["article"].ToString(), 
        //                batchno = r["batchno"].ToString(), 
        //                dateexp = r.GetDateTimeOffset(15),
        //                datemfg = r.GetDateTimeOffset(14),
        //                depot = r["depot"].ToString(),
        //                huno = r["huno"].ToString(),
        //                loccode = r["loccode"].ToString(), 
        //                lotno = r["lotno"].ToString(),
        //                lv = r.GetInt32(8),
        //                pv = r.GetInt32(7),
        //                prepno = "",
        //                qtypu = r.GetInt32(10),
        //                qtypuoh = r.GetInt32(10),
        //                qtysku = r.GetInt32(9),
        //                qtyskuoh = r.GetInt32(9),
        //                serialno = r["serialno"].ToString(),
        //                site = r["site"].ToString(),
        //                spcarea = r["spcarea"].ToString(),
        //                stockid = r.GetDecimal(4),
        //                barcode = r["barcode"].ToString()                         
        //            });
        //         }
        //         await r.CloseAsync();

        //         //get pending order of outbond 
        //         cm.CommandText = " select l.orgcode, l.site,l.depot,l.spcarea, l.disthcode, l.ouorder,l.ouln, l.article,l.pv, l.lv, l.unitops, l.qtypnd qtysku,l.qtypndpu qtypu, lscodealt loccode, b.inorder " + 
        //         " from wm_outbound b, wm_outbouln l , (select lscodealt,spcthcode from wm_locdw where spcthcode != '' and orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea ) c " + 
        //         " where b.orgcode = l.orgcode and b.site = l.site and b.depot = l.depot and b.spcarea = l.spcarea and l.disthcode = c.spcthcode and b.inorder = l.inorder " + 
        //         " and b.orgcode = @orgcode and b.site = @site and b.depot = @depot and b.spcarea = @spcarea and b.inorder = @inrefno and l.article = @product ";
        //         r = await cm.snapsReadAsync();
        //         while(await r.ReadAsync()) {
        //             lsorder.Add(new procdist_order() {
        //                 article = r["article"].ToString(), 
        //                 depot = r["depot"].ToString(),
        //                 inorder = r["inorder"].ToString(), 
        //                 lv = r.GetInt32(9),
        //                 orgcode = r["orgcode"].ToString(), 
        //                 ouln = r.GetInt32(6),
        //                 ouorder = r["ouorder"].ToString(), 
        //                 pv = r.GetInt32(8),
        //                 qtypu = r.GetInt32(12),
        //                 qtypuprep = 0,
        //                 qtysku = r.GetInt32(11),
        //                 qtyskuprep = 0,
        //                 site = r["site"].ToString(),
        //                 spcarea = r["spcarea"].ToString(),
        //                 thcode = r["disthcode"].ToString(),
        //                 unitops = r["unitops"].ToString().CInt32(), // warning : string convertion must change later 
        //                 loccode = r["loccode"].ToString()
        //             });
        //         }
        //         await r.CloseAsync();

        //         // generate distribution plan per pallet 
        //         cm.CommandText = prep_genplan;
        //         lsstock.ForEach(x=> { 
        //             x.prepno = cm.snapsScalarStrAsync().Result.ToString(); 
        //             // generate plan header
        //             lsplan.Add(new prep_md(){ 
        //                 orgcode = x.orgcode, site = x.site, depot = x.depot , tflow = "IO", spcarea = x.spcarea, prepno = x.prepno , lines = new List<prln_md>(), huno = x.huno
        //             });
        //         }); 



        //         // Option put 1 by 1
        //         foreach(procdist_stock x in lsstock){                    
        //        // lsstock.ForEach(x=> { 
        //             //Int32 crstock = x.qtyskuoh;                        
        //             // lsorder.Where(o=>o.orgcode == x.orgcode && o.site == x.site && o.depot == x.depot && o.article == x.article && o.qtypu != o.qtypuprep)
        //             //     .OrderBy(s=>s.thcode);
        //             do {
        //                 try { 
        //                     crorder = lsorder.Where(o=>o.orgcode == x.orgcode && o.site == x.site && o.depot == x.depot && o.article == x.article && o.qtysku != o.qtyskuprep).OrderBy(s=>s.thcode).First();
        //                 }catch (Exception exl) { 
        //                     crorder = new procdist_order();
        //                 }

        //                 if (crorder.ouorder != null) { 
        //                     if (crorder.qtysku == x.qtyskuoh ) { 
        //                         //equal case                                
        //                         crorder.qtyskuprep = x.qtyskuoh;

        //                         // Insert prep line 
        //                         nxprepln = lsplan.Find(p=>p.prepno == x.prepno).lines.Count + 1;
        //                         lsplan.Find(p=>p.prepno == x.prepno).lines.Add(new prln_md() { 
        //                             accncreate = "",
        //                             accnmodify = "",
        //                             article = x.article,
        //                             pv = x.pv, 
        //                             lv = x.lv, 
        //                             barcode = x.barcode,
        //                             batchno = x.batchno,
        //                             datecreate = DateTimeOffset.Now,
        //                             dateexp = x.dateexp,
        //                             datemfg = x.datemfg,
        //                             datemodify = DateTimeOffset.Now,
        //                             datepick = null,
        //                             depot = x.depot,
        //                             hunosource = x.huno,
        //                             loccode = crorder.loccode,
        //                             lotno = x.lotno,
        //                             orgcode = x.orgcode,
        //                             site = x.site,
        //                             qtyskuorder = x.qtyskuoh,
        //                             qtypuorder = x.qtyskuoh,//bug
        //                             serialno = x.serialno,
        //                             spcarea = x.spcarea,
        //                             stockid = x.stockid,
        //                             ouorder = crorder.ouorder,
        //                             ouln = crorder.ouln.ToString(),
        //                             prepln = nxprepln, //hardcode
        //                             tflow = "IO",
        //                             unitprep = crorder.unitops.ToString(),
        //                             inln = 1, //hardcode
        //                             inorder = o[0].disinbound
        //                         });

        //                         //assign 0 of stock 
        //                         x.qtyskuoh = 0;
        //                         break;
        //                     }else if ( crorder.qtypu > x.qtypu) {
        //                         // order is more than stsock 

        //                     }else {
        //                         // stock is less than order 
        //                     }
        //                 }else { break; }

        //             }while (x.qtyskuoh == 0);
        //         }
        //         //);


        //         // Option Put full stock 


        //         // Resetup preparation
        //         lsplan.ForEach(x=> { 
        //             x.capacity = 10000; //hardcode
        //             x.dateassign = new DateTimeOffset();
        //             x.datecreate = new DateTimeOffset();
        //             x.prepdate = new DateTimeOffset();
        //             x.preppct = 0;
        //             x.preptype = "XD";
        //             x.priority = 0;
        //             x.spcarticle = "";
        //             x.spcorder = "";
        //             x.thcode = o[0].thcode;
        //             x.routeno = "";
        //             x.deviceID = "";
        //             x.picker = "";
        //             x.accncreate = "";
        //             x.accnmodify = "";
        //             x.procmodify = "api.outbound.procdist";
        //             x.spcorder = o[0].disinbound;
        //             x.spcarticle = o[0].disproduct;
        //         });
        //         lsplan = lsplan.Where(x=>x.lines.Count > 0).ToList();

        //         //Confirm create plan 
        //         await ops.upsert(lsplan);                    

        //         //update stock block article 

        //         //update stock qtyprep on stock line 

        //         //Close Connection 
        //         await cn.CloseAsync(); 


        //     }catch (Exception ex) { throw ex; 
        //     }finally { if(cm!=null) { cm.Dispose(); }  } 

        // }
    }
}