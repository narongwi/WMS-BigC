using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.WMS.preparation;
using Snaps.WMS;
using System.Globalization;
using Snaps.Helpers.Logger;

namespace Snaps.WMS {

    public partial class inbound_ops : IDisposable {
        private SqlConnection cn = null;
        private readonly ISnapsLogger logger;

        public inbound_ops() { }
        public inbound_ops(String cx) {
            cn = new SqlConnection(cx);
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<inbound_ops>();
        }

        public async Task<List<inbound_ls>> find(inbound_pm rs) {
            SqlCommand cm = null;
            List<inbound_ls> rn = new List<inbound_ls>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */

                if(string.IsNullOrEmpty(rs.orgcode) || string.IsNullOrEmpty(rs.site) || string.IsNullOrEmpty(rs.depot)) {
                    throw new Exception("Parameter is not setup");
                } else {
                    cm = (sqlinbound_order_find).snapsCommand(cn);
                    cm.snapsPar(rs.orgcode,"orgcode");
                    cm.snapsPar(rs.site,"site");
                    cm.snapsPar(rs.depot,"depot");

                    cm.snapsCdn(rs.thcode,"thcode",string.Format(" and ( t.thcode like '%{0}%' or t.thnameint like '%{0}%' ) ",rs.thcode.ClearReg()));
                    cm.snapsCdn(rs.inorder,"inorder",string.Format(" and i.inorder like '%{0}%' ",rs.inorder.ClearReg()));

                    if(!rs.dateplanfrom.Equals(null) && !rs.dateplanto.Equals(null)) {
                        cm.snapsPar(rs.dateplanto,"dateplanto");
                        cm.snapsCdn(rs.dateplanfrom,"dateplanfrom"," and cast(i.datereplan as date) between cast(@dateplanfrom as date) and cast(@dateplanto as date) ");
                    } else if(!rs.dateplanfrom.Equals(null) && rs.dateplanto.Equals(null)) {
                        cm.snapsCdn(rs.dateplanfrom,"dateplanfrom"," and cast(i.datereplan as date) = cast(@dateplanfrom as date) ");
                    } else if(!rs.dateplanto.Equals(null) && rs.dateplanfrom.Equals(null)) {
                        cm.snapsCdn(rs.dateplanto,"dateplanto"," and cast(i.datereplan as date) <= cast( @dateplanto as date) ");
                    }

                    if(!rs.dateorderfrom.Equals(null) && !rs.dateorderto.Equals(null)) {
                        cm.snapsPar(rs.dateorderto,"dateorderto");
                        cm.snapsCdn(rs.dateorderfrom,"dateorderfrom"," and cast(i.datereplan as date) between cast(@dateorderfrom as date) and cast(@dateorderto as date) ");
                    } else if(!rs.dateorderfrom.Equals(null) && rs.dateorderto.Equals(null)) {
                        cm.snapsCdn(rs.dateorderfrom,"dateorderfrom"," and cast(i.datereplan as date) = cast(@dateorderfrom as date) ");
                    } else if(!rs.dateorderto.Equals(null) && rs.dateorderfrom.Equals(null)) {
                        cm.snapsCdn(rs.dateorderto,"dateorderto"," and cast(i.datereplan as date) <= cast( @dateorderto as date) ");
                    }

                    if(!rs.daterecfrom.Equals(null) && !rs.daterecto.Equals(null)) {
                        cm.snapsPar(rs.daterecto,"daterecto");
                        cm.snapsCdn(rs.daterecfrom,"daterecfrom"," and cast(i.datereplan as date) between cast(@daterecfrom as date) and cast(@daterecto as date) ");
                    } else if(!rs.daterecfrom.Equals(null) && rs.daterecto.Equals(null)) {
                        cm.snapsCdn(rs.daterecfrom,"daterecfrom"," and cast(i.datereplan as date) = cast(@daterecfrom as date) ");
                    } else if(!rs.daterecto.Equals(null) && rs.daterecfrom.Equals(null)) {
                        cm.snapsCdn(rs.daterecto,"daterecto"," and cast(i.datereplan as date) <= cast( @daterecto as date) ");
                    }

                    cm.snapsCdn(rs.ordertype,"ordertype"," and i.subtype = @ordertype ");
                    cm.snapsCdn(rs.spcarea,"spcarea"," and i.spcarea = @spcarea ");
                    cm.snapsCdn(rs.inpriority,"inpriority",(rs.inpriority.notNull()) ? " and i.inpriority " + ((rs.inpriority == "1") ? " > 0 " : " = 0") : null);
                    cm.snapsCdn(rs.article,"article",string.Format(" and exists (select 1 from wm_inbouln l where exists (select 1 from wm_product p where l.orgcode = p.orgcode " +
                    " and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv and (p.article like '%{0}%'  " +
                    "  or p.description like '%{0}%' or p.descalt like '%{0}%')) and i.orgcode = l.orgcode and i.site = l.site and i.depot = l.depot  " +
                    "  and i.inorder = l.inorder) ",rs.article.ClearReg()));
                    cm.snapsCdn(rs.inpromo,"inpromo",string.Format(" and i.inpromo like '%{0}%' ",rs.inpromo.ClearReg()));
                    cm.snapsCdn(rs.inflag,"inflag",string.Format(" and ( i.inflag like '%{0}%' or i.remarkrec like '%{0}%' ) ",rs.inflag.ClearReg()));
                    cm.snapsCdn(rs.dockno,"dockno"," and i.dockrec = @dockno");
                    cm.snapsCdn(rs.tflow,"tflow"," and i.tflow = @tflow ");
                    cm.snapsCdn(rs.ismeasure,"ismeasure"," and isnull(ismeasurement,0) = @ismeasure ");

                    r = await cm.snapsReadAsync();
                    while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                    await r.CloseAsync(); await cn.CloseAsync();
                    return rn;
                }
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.orderno,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<List<inbound_hs>> history(inbound_pm rs) {
            SqlCommand cm = null;
            List<inbound_hs> rn = new List<inbound_hs>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm = sqlhistory.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");

                cm.snapsCdn(rs.thcode,"thcode",string.Format(" and ( t.thcode like '%{0}' or t.thnameint like '%{0}' ) ",rs.thcode.ClearReg()));
                cm.snapsCdn(rs.inorder,"inorder",string.Format(" and m.inorder like '%{0}%' ",rs.inorder.ClearReg()));

                if(!rs.daterecfrom.Equals(null)) {
                    cm.snapsCdn(rs.daterecfrom,"daterecfrom"," and cast(m.daterec as date) " + ((!rs.daterecto.Equals(null)) ? ">=" : "=") + " cast(@daterecfrom as date) ");
                }
                if(!rs.daterecto.Equals(null)) { cm.snapsCdn(rs.daterecto,"daterecto"," and cast(m.daterec as date) <= cast(@daterecto as date) "); }

                if(!rs.dateorderfrom.Equals(null)) { cm.snapsCdn(rs.dateorderfrom,"dateorderfrom"," and m.dateorder >= @dateorderfrom "); }
                if(!rs.dateorderto.Equals(null)) { cm.snapsCdn(rs.dateorderto,"dateorderto"," and i.dateorder <= @dateorderto "); }

                cm.snapsCdn(rs.ordertype,"ordertype"," and i.ordetype = @ordertype ");
                cm.snapsCdn(rs.spcarea,"spcarea"," and i.spcarea = @spcarea ");
                cm.snapsCdn(rs.inpriority,"inpriority"," and i.inpriority = @inpriority");
                cm.snapsCdn(rs.article,"article"," exists (select 1 from wm_inbouln ln where i.orgcode = ln.orgcode and i.site = ln.orgcode " +
                " and i.depot = ln.depot and i.inorder = ln.inorder and ( article like '%@article%' and barcode like '%@article%')) ");
                cm.snapsCdn(rs.inpromo,"inpromo"," and i.inpromo = @inpromo");
                cm.snapsCdn(rs.inflag,"inflag"," and i.inflag = @inflag");
                cm.snapsCdn(rs.dockno,"dockno"," and i.dockrec = @docno");
                cm.snapsCdn(rs.tflow,"tflow"," and i.tflow = @tflow ");

                cm.CommandText += " order by m.daterec";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillhs(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { logger.Error(rs.orgcode,rs.site,rs.orderno,ex,ex.Message); throw ex; } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<List<lov>> getstaging(String orgcode,String site,String depot,Int32 quantity) {
            SqlCommand cm = null;
            List<lov> rn = new List<lov>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm = sqlgetstaging.snapsCommand(cn);
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsCdn(quantity,"quantity","and (lsmxhuno - isnull(crhu,0)) > @quantity ");
                cm.CommandText += " order by spcseqpath ";

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodedsc"].ToString())); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { logger.Error(orgcode,site,"",ex,ex.Message); throw ex; } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<List<lov>> getproductratio(String orgcode,String site,String depot,String article,String pv,String lv) {
            SqlCommand cm = null;
            List<lov> rn = new List<lov>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm = sqlgetratio.snapsCommand(cn);
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(article,"article");
                cm.snapsPar(pv,"pv");
                cm.snapsPar(lv,"lv");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["rtosku"].ToString(),r["unit"].ToString(),r["bnvalue"].ToString(),"","","","")); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { logger.Error(orgcode,site,article,ex,ex.Message); throw ex; } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<inbound_md> get(inbound_ls rs) {
            SqlCommand cm = null; SqlDataReader r = null;
            inbound_md rn = new inbound_md();
            try {
                /* Vlidate parameter */
                cm = sqlgetheader.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsPar(rs.inorder,"inorder");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync();
                rn.lines = new List<inbouln_md>();

                cm.CommandText = sqlgetline;
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.lines.Add(filllnmdl(ref r)); }
                await r.CloseAsync();

                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { logger.Error(rs.orgcode,rs.site,rs.inorder,ex,ex.Message); throw ex; } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<inbound_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            Int32 ix = 0;
            try {
                foreach(inbound_md ln in rs) {
                    cm.Add(obcommand(ln,sqlvalidate));
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlinsert : sqlupdate;
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var first = rs.FirstOrDefault();
                logger.Error(first.orgcode,first.site,first.inorder,ex,ex.Message,"inorder",first.inorder);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task upsert(inbound_md rs) {
            List<inbound_md> ro = new List<inbound_md>();
            try {
                ro.Add(rs); await upsert(ro);
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message,"inorder",rs.inorder);
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task setpriority(String orgcode,String site,String depot,String inorder,Int32 inpriority,String accncode) {
            SqlCommand cm = new SqlCommand(sqlsetpriority,cn);
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(accncode,"accncode");
                cm.snapsPar(inpriority,"inpriority");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
                logger.Info(orgcode,site,accncode,"setpriority",inorder,"success");
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task setstaging(String orgcode,String site,String depot,String inorder,String staging,String accncode) {
            SqlCommand cm = new SqlCommand(sqlsetstaging,cn);
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(staging,"staging");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
                logger.Info(orgcode,site,accncode,"setstaging",inorder,"success");
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task setremarks(String orgcode,String site,String depot,String inorder,String remarks,String accncode) {
            SqlCommand cm = new SqlCommand(sqlsetremark,cn);
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(remarks,"remarks");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
                logger.Info(orgcode,site,accncode,"setremarks",inorder,"success");
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task setinvoice(String orgcode,String site,String depot,String inorder,String invoiceno,String accncode) {
            SqlCommand cm = new SqlCommand(sqlsetinvoice,cn);
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(invoiceno,"invno");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
                logger.Info(orgcode,site,accncode,"setinvoice",inorder,"success");
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task setreplan(String orgcode,String site,String depot,String inorder,DateTimeOffset? datereplan,String accncode) {
            SqlCommand cm = new SqlCommand(sqlsetreplan,cn);
            try {

                DateTimeOffset ndw = new DateTimeOffset(new DateTime((datereplan.Value.Year > 2500) ? datereplan.Value.Year - 543 : datereplan.Value.Year,datereplan.Value.Month,datereplan.Value.Day,datereplan.Value.Hour,datereplan.Value.Minute,0));
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(ndw,"datereplan");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
                logger.Info(orgcode,site,accncode,"set Replan",inorder,"success");
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task setunloadstart(String orgcode,String site,String depot,String inorder,String accncode) {
            SqlCommand cm = new SqlCommand(sqlunloadstart,cn);
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
                logger.Info(orgcode,site,accncode,"set unloadstart",inorder,"success");
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task setunloadend(String orgcode,String site,String depot,String inorder,String accncode) {
            List<SqlCommand> cm = new List<SqlCommand>();
            SqlCommand sq = new SqlCommand("select next value for seq_recno recno",cn);
            orbit_ops oo = new orbit_ops();
            //SqlCommand cm = new SqlCommand(sqlfinish,cn); 
            try {
                cm.Add(sqlunloadend.snapsCommand());
                cm.Add(oo.sqlInterface_landing_receipt_confirm.snapsCommand());

                // generate receive no
                string ingrno = await sq.snapsScalarStrAsync();

                // Confirm sent receiption to orbit
                foreach(SqlCommand ln in cm) {
                    ln.snapsPar(orgcode,"orgcode");
                    ln.snapsPar(site,"site");
                    ln.snapsPar(depot,"depot");
                    ln.snapsPar(inorder,"inorder");
                    ln.snapsPar(accncode,"accncode");
                    ln.snapsPar(ingrno,"ingrno");
                    ln.snapsParsysdateoffset();
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.ForEach(e => e.Dispose()); }


        }
        public async Task setfinish(String orgcode,String site,String depot,String inorder,String accncode) {
            SqlCommand cm = new SqlCommand(sqlfinish,cn);
            SqlDataReader r = null;
            inbound_md vld = new inbound_md();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();

                // validate pending finish
                cm.CommandText = sqlgetheader;
                r = await cm.snapsReadAsync();

                while(await r.ReadAsync()) {
                    vld = fillmdl(ref r);
                }

                if(!r.IsClosed)
                    await r.CloseAsync();
                
                if(vld.waitconfirm > 0) throw new Exception("Order item is waiting confirm receipt");
                if(vld.pendinginf > 0) throw new Exception("Order item is waiting finish load");

                if(vld.spcarea == "XD") {
                    // Close order in StoredProcedure
                    // Auto Generate Plan for Distribution 
                    using(var command = new SqlCommand("",cn)) {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "[dbo].[snaps_share_operate]";
                        command.Parameters.AddWithValue("@inorgcode",vld.orgcode);
                        command.Parameters.AddWithValue("@insite",vld.site);
                        command.Parameters.AddWithValue("@indepot",vld.depot);
                        command.Parameters.AddWithValue("@ininorder",vld.inorder);
                        command.Parameters.AddWithValue("@picker",accncode);
                        await command.snapsExecuteAsync();
                    }// end using
                }else {
                    // confirm close order
                    cm.CommandText = sqlfinish;
                    await cm.snapsExecuteAsync();
                }
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { 
                cm.Dispose();

                if(cn.State == System.Data.ConnectionState.Open)
                    await cn.CloseAsync();
            }
        }
        public async Task setcancel(String orgcode,String site,String depot,String inorder,String remarks,String accncode) {
            SqlCommand cm = new SqlCommand(sqlcancel,cn);
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(inorder,"inorder");
                cm.snapsPar(remarks,"remarks");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();
                logger.Info(orgcode,site,accncode,"set cancel",inorder,"success");
            } catch(Exception ex) {
                logger.Error(orgcode,site,accncode,ex,ex.Message,"inorder",inorder);
                throw ex;
            } finally { cm.Dispose(); }
        }
        public async Task remove(List<inbound_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            var f = rs.FirstOrDefault();
            try {
                foreach(inbound_md ln in rs) {
                    cm.Add(obcommand(ln,sqldelete_step1));
                    cm.Add(obcommand(ln,sqldelete_step2));
                }
                await cm.snapsExecuteTransAsync(cn);
                logger.Info(f.orgcode,f.site,f.accnmodify,"set cancel",f.inorder,"success");
            } catch(Exception ex) {
                logger.Error(f.orgcode,f.site,f.accnmodify,ex,ex.Message,"inorder",f.inorder);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task remove(inbound_md rs) {
            List<inbound_md> ro = new List<inbound_md>();
            try {
                ro.Add(rs); await remove(ro);
                logger.Info(rs.orgcode,rs.site,rs.accnmodify,"remove",rs.inorder,"success");
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message,"inorder",rs.inorder);
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task<List<inboulx>> getlx(inboulx ln) {
            SqlCommand cm = new SqlCommand(sqlgetlx,cn); SqlDataReader r = null;
            List<inboulx> rn = new List<inboulx>();
            try {
                /* Vlidate parameter */
                cm.snapsPar(ln.orgcode,"orgcode");
                cm.snapsPar(ln.site,"site");
                cm.snapsPar(ln.depot,"depot");
                cm.snapsCdn(ln.spcarea,"spcarea"," and x.spcarea = @spcarea ");
                cm.snapsCdn(ln.inorder,"inorder"," and x.inorder = @inorder ");
                cm.snapsCdn(ln.inln,"inln"," and x.inln = @inln");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(lxfill(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { logger.Error(ln.orgcode,ln.site,ln.accnmodify,ex,ex.Message,"inorder",ln.inorder); throw ex; } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<List<inboulx>> upsertlx(inboulx ln) {
            SqlCommand cm = new SqlCommand();
            try {
                ln.procmodify = "api.upsertlx";
                ln.qtyhurec = (ln.qtyhurec == 0) ? 1 : ln.qtyhurec; // Auto assign atlease 1 HU
                cm = lxcomamnd(ln);
                cm.CommandText = sqlinbounlx_valmea;
                if(cm.snapsScalarStrAsync().Result == "1") {
                    throw new Exception("Product still require to measurement");
                } else {
                    cm.CommandText = sqlinboulx_val;
                    if(cm.snapsScalarStrAsync().Result == "1") {
                        throw new Exception("Quantity receive is over order");
                    } else {

                        if(ln.serialno != "") {
                            cm.CommandText = sqlinboulc_valserail;
                            if(cm.snapsScalarStrAsync().Result == "1") {
                                throw new Exception("Serial no " + ln.serialno + " has exists on wms");
                            }
                        }
                        cm.CommandText = sqlinsertlx;
                        await cm.snapsExecuteAsync();
                        logger.Info(ln.orgcode,ln.site,ln.accnmodify,"Save Receipt",ln.inorder,ln.article,"success");
                        return await getlx(ln);
                    }
                }

            } catch(Exception ex) { logger.Error(ln.orgcode,ln.site,ln.accnmodify,ex,ex.Message,"inorder",ln.inorder); throw ex; } finally { if(cm != null) { cm.Dispose(); } }
        }
        public async Task<List<inboulx>> removelx(inboulx ln) {
            SqlCommand cm = new SqlCommand();
            try {
                cm = lxcomamnd(ln);
                cm.CommandText = sqlremovelx;
                await cm.snapsExecuteAsync();
                logger.Info(ln.orgcode,ln.site,ln.accnmodify,"Remove Receipt",ln.inorder,ln.article,"success");
                return await getlx(ln);

            } catch(Exception ex) { logger.Error(ln.orgcode,ln.site,ln.accnmodify,ex,ex.Message,"inorder",ln.inorder); throw ex; } finally { if(cm != null) { cm.Dispose(); } }
        }
        public async Task<List<inboulx>> commitlx(inboulx ln) {
            SqlCommand cm = new SqlCommand("",cn);                  //Selection Command
            SqlCommand om = new SqlCommand("",cn);                  //Operate Command 
            SqlTransaction tx = null;

            orbit_ops ix = new orbit_ops(cn);                         //interface to orbit
            statisic_ops so = new statisic_ops();                   //Statistic calculation
            sequence_ops sq = new sequence_ops(ref cn);

            task_ops top = new task_ops(cn.ConnectionString);       //Task operation
            stock_ops oiv = new stock_ops(cn.ConnectionString);    //Stock operation
            handerlingunit_ops ho = new handerlingunit_ops(cn.ConnectionString); //Handerling unit operate 
            handerlingunit_gen hg = new handerlingunit_gen();

            List<inboulx> lsops = new List<inboulx>(); // List of transaction to operate
            List<SqlCommand> lscmd = new List<SqlCommand>();

            SqlDataReader r;
            List<stock_mvin> miv = new List<stock_mvin>();
            List<task_md> tls = new List<task_md>();

            Int32 skuohd = 0;
            //Decimal skuweight = 0;
            String xloccode = "";

            List<string> putawaylabel = new List<string>();
            prep_ops po = new prep_ops(cn.ConnectionString);        //Auto create distribution process 
            prepset ps = new prepset();                            //Preparation set for distribution process

            try {
                cm.CommandText = sqlorbitconfirm;
                cm.snapsPar(ln.orgcode,"orgcode");
                cm.snapsPar(ln.site,"site");
                cm.snapsPar(ln.depot,"depot");
                cm.snapsPar(ln.spcarea,"spcarea");
                cm.snapsPar(ln.inorder,"inorder");
                cm.snapsPar(ln.inln,"inln");
                cm.snapsPar(ln.inagrn,"inagrn");
                cm.snapsPar(ln.inseq,"inseq");
                cm.snapsPar(ln.article,"article");
                cm.snapsPar(ln.pv,"pv");
                cm.snapsPar(ln.lv,"lv");
                cm.snapsPar(ln.accnmodify,"accnmodify");
                cm.snapsPar(ln.inlx,"inlx");
                cm.snapsParsysdateoffset();

                hg.orgcode = ln.orgcode;
                hg.site = ln.site;
                hg.depot = ln.depot;
                hg.loccode = ln.dockno;
                hg.thcode = ln.thcode;
                hg.hutype = "PL01";
                hg.mxsku = 9999;
                hg.mxvolume = 9999;
                hg.mxweight = 1000;
                hg.quantity = 1;
                hg.spcarea = "ST";
                //hg.spcarea = ln.spcarea;     
                // Do not change coz when we received need to generate HU belong of stocking first before 
                hg.accncreate = ln.accncreate;
                hg.accnmodify = ln.accnmodify;
                hg.promo = "";
                hg.priority = 0;

                //generate grno 
                ln.ingrno = sq.grtxNext(ref cn); //Generate sequence grno

                // Split pallet
                r = await cm.snapsReadAsync();
                if(r.HasRows) {
                    while(await r.ReadAsync()) { lsops.Add(lxfill(ref r)); }
                    await r.CloseAsync();
                    foreach(inboulx lx in lsops) {
                        lsops.Last().ingrno = ln.ingrno;
                        lx.ingrno = ln.ingrno;
                        skuohd = lx.qtyskurec;
                        xloccode = lx.dockno;

                        while(skuohd > 0) { 
                            //Split SKU receipted into HU
                            miv.Add(getStockIn(lx));
                            miv.Last().opscode = lsops.Last().ingrno;
                            miv.Last().opssku = (skuohd >= lx.rtohu) ? lx.rtohu : skuohd;
                            miv.Last().opspu = miv.Last().opssku / lx.rtopu;
                            miv.Last().opsweight = miv.Last().opssku * lx.skuweight;
                            miv.Last().opsvolume = miv.Last().opssku * lx.skuvolume;
                            //miv.Last().opspu = 
                            miv.Last().opsaccn = ln.accnmodify;

                            // Generate HU
                            await ho.generate(hg);
                            miv.Last().opshuno = hg.huno;
                            miv.Last().opsthcode = ln.thcode;

                            skuohd = skuohd - miv.Last().opssku;

                            miv.Last().ingrno = ln.ingrno;
                            miv.Last().opsunit = lx.unitops;
                            miv.Last().opsreftype = "GR";
                        }
                    }

                    //generate stock 
                    Decimal ostockid = 0;
                    foreach(SqlCommand me in oiv.stockIncommand(miv,ref cn,ref tx,ref ostockid)) {
                        lscmd.Add(me);
                    }

                    foreach(inboulx lx in lsops) {
                        lscmd.Add(sqlconfirmlx.snapsCommand(cn));
                        lscmd.Last().snapsPar(lx.orgcode,"orgcode");
                        lscmd.Last().snapsPar(lx.site,"site");
                        lscmd.Last().snapsPar(lx.depot,"depot");
                        lscmd.Last().snapsPar(lx.inlx,"inlx");
                        lscmd.Last().snapsPar(lx.ingrno,"ingrno");
                        lscmd.Last().snapsPar(lx.inorder,"inorder");
                        lscmd.Last().snapsPar(lx.inseq,"inseq");
                        lscmd.Last().snapsPar(lx.inagrn,"inagrn");
                        lscmd.Last().snapsPar(lx.accnmodify,"accnmodify");
                        lscmd.Last().snapsParsysdateoffset();

                        lscmd.Add(so.sqlinboundproductlas.snapsCommand(cn));
                        lscmd.Last().snapsPar(lx.orgcode,"orgcode");
                        lscmd.Last().snapsPar(lx.site,"site");
                        lscmd.Last().snapsPar(lx.depot,"depot");
                        lscmd.Last().snapsPar(lx.inlx,"inlx");
                        lscmd.Last().snapsPar(lx.ingrno,"ingrno");
                        lscmd.Last().snapsPar(lx.inorder,"inorder");
                        lscmd.Last().snapsPar(lx.inseq,"inseq");
                        lscmd.Last().snapsPar(lx.inagrn,"inagrn");
                        lscmd.Last().snapsPar(lx.accnmodify,"accnmodify");

                    }

                    //update detail line transaction
                    foreach(inboulx lx in lsops) {
                        //Generate Interface to orbit
                        lscmd.Add(ix.landing_receipt(orbit_receipt(lx)));
                    }

                    //Update statistic on Inbound order line
                    foreach(inboulx lx in lsops
                        .GroupBy(x => new { x.orgcode,x.site,x.depot,x.spcarea,x.inorder,x.inln })
                        .Select(s => s.FirstOrDefault()).ToList()
                    ) {
                        lscmd.Add(so.inboundLine(ln.orgcode,ln.site,ln.depot,ln.inorder,ln.inln,ln.accnmodify));
                    }

                    //Calculate stock on article
                    lscmd.Add(so.inbound(ln.orgcode,ln.site,ln.depot,ln.inorder,ln.accnmodify));

                    //Calcuate stock on hand
                    lscmd.Add(so.stockonhand(ln.orgcode,ln.site,ln.depot,ln.article,ln.pv,ln.lv));

                    //Calcaulate stock on location 
                    lscmd.Add(so.location(ln.orgcode,ln.site,ln.depot,xloccode));


                    //Execution 
                    string ersql = "";
                    try {
                        await lscmd.snapsExecuteTransAsync(cn,true,ersql);
                    } catch(Exception exl) {
                        logger.Error(ln.orgcode,ln.site,ln.accnmodify,exl,
                          "Execution Receiving Command",ln.inorder,ln.article,
                          ln.lv.ToString(),exl.Message);
                        throw exl;
                    }

                    // list task afetr generate putaway
                    List<string> putawys = new List<string>();

                    //Generate task movemnet for stoking 
                    if(ln.spcarea == "ST") {
                        foreach(stock_mvin tn in miv) {
                            tls.Add(filltask(tn,ln.insubtype));
                            tls.Last().lines.Add(filltaskln(tn));
                        }
                        // Generate Putaway Task
                        putawys =  await top.generateInboundAsync(tls);
                    }

                    // Generate distribution plan for XD
                    // move to close po 

                    // if(ln.spcarea == "XD") {
                    //    ps.orgcode = ln.orgcode;
                    //    ps.site = ln.site;
                    //    ps.depot = ln.depot;
                    //    ps.distb = new List<outbound_ls>();
                    //    miv.ForEach(mv => {
                    //        ps.distb.Add(new outbound_ls() {
                    //            disinbound = ln.inorder,
                    //            disproduct = ln.article,
                    //            dispv = ln.pv,
                    //            dislv = ln.lv,
                    //            dishuno = mv.opshuno,
                    //            disstockid = mv.stockid.ToString().CInt32()
                    //        });
                    //    });
                    //    await po.distsetup(ps);
                    //    await po.procdistb(ps.orgcode,ps.site,ps.depot,"XD",ps.setno,ln.accnmodify);
                    // }

                    //Recalculate stock 
                    foreach(inboulx lx in lsops
                        .GroupBy(x => new { x.orgcode,x.site,x.depot,x.spcarea,x.article,x.pv,x.lv })
                        .Select(s => s.FirstOrDefault()).ToList()
                    ) {
                        await so.inboundstock(lx.orgcode,lx.site,lx.depot,lx.article,lx.pv.ToString(),lx.lv.ToString()).snapsExecuteTransAsync(cn);
                    }

                    /* PRINT LABEL */
                    //if(ln.spcarea == "ST" && taskidls.Count > 0) {
                    if(putawys.Count > 0) {
                        //get DC parameter operate
                        var m = miv.FirstOrDefault();
                        foreach(var taskno in putawys.Distinct().ToList()) {
                            await printPutaway(m.orgcode,m.site,m.depot,taskno,m.opsaccn);                           
                        }
                    }

                    //return after commit 
                    return await getlx(ln);

                } else {
                    await r.CloseAsync();
                    await cn.CloseAsync();
                    throw new Exception("Not found receive line with status save");
                }

            } catch(Exception ex) {
                logger.Error(ln.orgcode,ln.site,ln.accnmodify,ex,ex.Message,"inorder",
                ln.inorder,ln.article,ln.lv.ToString());
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } }
        }
        private async Task<bool> printNormalHU(string orgcode,string site,string depot,string huno,string accode) {
            try {
                // get document api config
                string command =
                    @"select top 1 bnflex1 from wm_binary where orgcode=@orgcode and site=@site and depot=@depot 
                    and apps='WMS' and bntype = 'PRINTER' and bncode ='LABEL'";

                SqlCommand pm = new SqlCommand(command,cn);
                pm.snapsPar(orgcode,"orgcode");
                pm.snapsPar(site,"site");
                pm.snapsPar(depot,"depot");
                string baseAddress = pm.snapsScalarStrAsync().Result;
                using(var httpClient = new System.Net.Http.HttpClient()) {
                    httpClient.Timeout = TimeSpan.FromSeconds(60);
                    httpClient.BaseAddress = new Uri(baseAddress);
                    var formVariables = new List<KeyValuePair<string,string>>();
                    formVariables.Add(new KeyValuePair<string,string>("orgcode",orgcode));
                    formVariables.Add(new KeyValuePair<string,string>("site",site));
                    formVariables.Add(new KeyValuePair<string,string>("depot",depot));
                    formVariables.Add(new KeyValuePair<string,string>("huno",huno));
                    var formContent = new System.Net.Http.FormUrlEncodedContent(formVariables);
                    var httpResponse = await httpClient.PostAsync("print/printhu",formContent);
                    return httpResponse.IsSuccessStatusCode;
                }
            } catch(Exception ex) {
                logger.Error(orgcode,site,accode,ex,ex.Message,"HU:",ex.Message);
                return false;
            }
        }

        /* Print Label */
        private async Task<bool> printPutaway(string orgcode,string site,string depot,string taskno,string accode) {
            try {
                // get document api config
                string command =
                    @"select top 1 bnflex1 from wm_binary where orgcode=@orgcode and site=@site and depot=@depot 
                    and apps='WMS' and bntype = 'PRINTER' and bncode ='LABEL'";

                SqlCommand pm = new SqlCommand(command,cn);
                pm.snapsPar(orgcode,"orgcode");
                pm.snapsPar(site,"site");
                pm.snapsPar(depot,"depot");
                string baseAddress = pm.snapsScalarStrAsync().Result;
                using(var httpClient = new System.Net.Http.HttpClient()) {
                    httpClient.Timeout = TimeSpan.FromSeconds(60);
                    httpClient.BaseAddress = new Uri(baseAddress);
                    var formVariables = new List<KeyValuePair<string,string>>();
                    formVariables.Add(new KeyValuePair<string,string>("orgcode",orgcode));
                    formVariables.Add(new KeyValuePair<string,string>("site",site));
                    formVariables.Add(new KeyValuePair<string,string>("depot",depot));
                    formVariables.Add(new KeyValuePair<string,string>("taskno",taskno));
                    var formContent = new System.Net.Http.FormUrlEncodedContent(formVariables);
                    var httpResponse = await httpClient.PostAsync("print/printputaway",formContent);
                    return httpResponse.IsSuccessStatusCode;
                }
            } catch(Exception ex) {
                logger.Error(orgcode,site,accode,ex,ex.Message,"HU:",ex.Message);
                return false;
            }
        }
        
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing) {
            if(!disposedValue) { if(cn != null) { cn.Dispose(); } }
            this.sqlinsert = null;
            this.sqlupdate = null;
            this.sqldelete_step1 = null;
            this.sqldelete_step2 = null;
            this.sqlinbound_order_find = null;
            this.sqlhistory = null;
            this.sqlgetstaging = null;
            this.sqlgetratio = null;
            this.sqlgetheader = null;
            this.sqlgetline = null;
            this.sqlvalidate = null;
            this.sqlsetpriority = null;
            this.sqlsetstaging = null;
            this.sqlsetremark = null;
            this.sqlsetinvoice = null;
            this.sqlsetreplan = null;
            this.sqlunloadstart = null;
            this.sqlunloadend = null;
            this.sqlfinish = null;
            this.sqlcancel = null;
            this.sqlgetlx = null;
            this.sqlinsertlx = null;
            this.sqlremovelx = null;
            this.sqlconfirmlx = null;
            this.sqlorbitconfirm = null;
            disposedValue = true;
        }
        public void Dispose() { GC.SuppressFinalize(this); }

    }

}
