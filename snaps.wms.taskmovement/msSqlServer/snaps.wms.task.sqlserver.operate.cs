using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.WMS;
using Snaps.Helpers.Logger;
using System.Net.Http;
using System.Linq;
namespace Snaps.WMS {

    public partial class task_ops : IDisposable {
        private SqlConnection cn = null;
        private readonly ISnapsLogger logger;

        public task_ops() { }
        public task_ops(SqlConnection ocn) {
            cn = ocn;

        }
        public task_ops(String cx) {
            cn = new SqlConnection(cx);
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<task_ops>();
        }


        public enum tasktype { putaway, replenishment, movement }
        private int getRowlimit(string orgcode,string site,string depot) {
            SqlCommand rm = new SqlCommand(pagerowlimit,cn);
            try {
                rm.snapsPar(orgcode,"orgcode");
                rm.snapsPar(site,"site");
                rm.snapsPar(depot,"depot");
                return rm.snapsScalarStrAsync().Result.CInt32();
            } catch(Exception ex) {
                logger.Error(orgcode,site,depot,ex,ex.Message);
                return 200;
            } finally { rm.Dispose(); }
        }
        public async Task<List<task_ls>> find(task_pm rs) {
            SqlCommand cm = null;
            List<task_ls> rn = new List<task_ls>();
            SqlDataReader r = null;
            try {
                //fix slow with limit row perpage default 200
                int rowlimt = getRowlimit(rs.orgcode,rs.site,rs.depot);
                /* Vlidate parameter */
                cm = (sqltask_find).snapsCommand(cn);
                cm.snapsPar(rowlimt,"rowlimit");
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");


                if(!rs.taskdatefrom.Equals(null)) {
                    cm.snapsCdn(rs.taskdatefrom,"taskdatefrom"," and cast(t.taskdate as date) " + ((!rs.taskdateto.Equals(null)) ? ">=" : "=") + " cast(@taskdatefrom as date) ");
                }
                if(!rs.taskdateto.Equals(null)) { cm.snapsCdn(rs.taskdateto,"taskdateto"," and cast(t.taskdate as date) <= cast( @taskdateto as date) "); }

                cm.snapsCdn(rs.tasktype,"tasktype"," and t.tasktype = @tasktype ");
                cm.snapsCdn(rs.taskno,"taskno",string.Format(" and t.taskno like '%{0}%' ",rs.taskno.ClearReg()));
                cm.snapsCdn(rs.iopromo,"iopromo",string.Format(" and t.iopromo like '%{0}%' ",rs.iopromo.ClearReg()));
                cm.snapsCdn(rs.iorefno,"iorefno",string.Format(" and t.iorefno like '%{0}%' ",rs.iorefno.ClearReg()));
                cm.snapsCdn(rs.priority,"priority",(rs.priority.notNull()) ? " and t.priority " + ((rs.priority == "1") ? " > 0 " : " = 0") : null);
                cm.snapsCdn(rs.tflow,"tflow"," and t.tflow = @tflow ");
                cm.snapsCdn(rs.article,"article",string.Format(" and (p.article like '%{0}%' or p.description like '%{0}%' or p.descalt like '%{0}%') ",rs.article.ClearReg()));
                cm.snapsCdn(rs.sourcehuno,"sourcehu",string.Format(" and sourcehuno like '%{0}%' ",rs.sourcehuno.ClearReg()));
                cm.snapsCdn(rs.targetadv,"targetadv",string.Format(" and targetadv like '%{0}%'",rs.targetadv.ClearReg()));
                cm.snapsCdn(rs.sourceloc,"sourceloc",string.Format(" and sourceloc like '%{0}%'",rs.sourceloc.ClearReg()));

                cm.CommandText += " order by taskdate desc,sourceloc asc ";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }

        public async Task<task_md> get(task_ls rs) {
            SqlCommand cm = null; SqlDataReader r = null;
            task_md rn = new task_md();
            try {
                /* Vlidate parameter */
                cm = sqlselect_task.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsPar(rs.spcarea,"spcarea");
                cm.snapsPar(rs.tasktype,"tasktype");
                cm.snapsPar(rs.taskno,"taskno");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync();

                rn.lines = new List<taln_md>();
                cm.CommandText = sqlselect_line;
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.lines.Add(fillln(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();

                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<task_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            Int32 ix = 0;
            try {
                foreach(task_md ln in rs) {
                    cm.Add(obcommand(ln,sqlvalidate_task));
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlinsert_task : sqlupdate_task;
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        public async Task upsert(task_md rs) {
            List<task_md> ro = new List<task_md>();
            try {
                ro.Add(rs); await upsert(ro);
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<task_md> rs) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                foreach(task_md ln in rs) {
                    cm.Add(obcommand(ln,sqlremove_task_step1));
                    cm.Add(obcommand(ln,sqlremove_task_step2));
                }
                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                var es = rs.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task remove(task_md rs) {
            List<task_md> ro = new List<task_md>();
            try {
                ro.Add(rs); await remove(ro);
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.accnmodify,ex,ex.Message);
                throw ex;
            } finally { ro.Clear(); }
        }

        //Assign Task
        public async Task assignTask(task_md o) {
            SqlCommand cm = new SqlCommand(sqlassign_task,cn);
            List<SqlCommand> lcm = new List<SqlCommand>();
            try {

                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.taskno,"taskno");
                cm.snapsPar(o.lines[0].accnassign,"accnassign");
                cm.snapsPar(o.accnmodify,"accnmodify");
                cm.snapsParsysdateoffset();
                lcm.Add(cm);
                await lcm.snapsExecuteAsync();
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.Dispose(); lcm.Clear(); }
        }

        //Start Task
        public async Task startTask(task_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {

                cm.Add(sqlstart_task_step1.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnwork,"accnwork");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();

                cm.Add(sqlstart_task_step2.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnassign,"accnwork");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();

                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        //Fill Task
        public async Task fillTask(task_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {

                cm.Add(sqlfill_task_step1.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnfill,"accnfill");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();

                cm.Add(sqlfill_task_step2.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnfill,"accnfill");
                cm.Last().snapsPar(o.lines[0].targetloc,"targetloc");
                cm.Last().snapsPar(o.lines[0].sourcehuno,"targethuno");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();

                cm.Add(sqlfill_task_step3.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnfill,"accnfill");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsPar(o.lines[0].sourcehuno,"sourcehuno");
                cm.Last().snapsPar(o.lines[0].sourceloc,"sourceloc");
                cm.Last().snapsPar(o.lines[0].targetloc,"targetloc");
                cm.Last().snapsPar(o.lines[0].article,"article");
                cm.Last().snapsPar(o.lines[0].pv,"pv");
                cm.Last().snapsPar(o.lines[0].lv,"lv");
                cm.Last().snapsParsysdateoffset();

                await cm.snapsExecuteTransAsync(cn);
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        //Collect Task 
        public async Task collectTask(task_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            statisic_ops so = new statisic_ops(ref cn);
            try {

                cm.Add(sqlfill_task_step1.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnfill,"accnfill");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();

                cm.Add(sqlfill_task_step2.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnfill,"accnfill");
                cm.Last().snapsPar(o.lines[0].targetloc,"targetloc");
                cm.Last().snapsPar(o.lines[0].sourcehuno,"targethuno");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();

                cm.Add(sqlfill_task_step3.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnfill,"accnfill");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsPar(o.lines[0].sourcehuno,"sourcehuno");
                cm.Last().snapsPar(o.lines[0].sourceloc,"sourceloc");
                cm.Last().snapsPar(o.lines[0].targetloc,"targetloc");
                cm.Last().snapsPar(o.lines[0].article,"article");
                cm.Last().snapsPar(o.lines[0].pv,"pv");
                cm.Last().snapsPar(o.lines[0].lv,"lv");
                cm.Last().snapsParsysdateoffset();



                await cm.snapsExecuteTransAsync(cn);

                //Recal stock capacity 
                cm.Clear();
                cm = so.correctionstock(o.orgcode,o.site,o.depot,o.lines[0].article,o.lines[0].pv,o.lines[0].lv);
                //Recal location capacity 
                cm.Add(so.location(o.orgcode,o.site,o.depot,o.lines[0].sourceloc));
                //Recal location capacity 
                cm.Add(so.location(o.orgcode,o.site,o.depot,o.lines[0].targetloc));
                // Execute
                await cm.snapsExecuteTransAsync(cn);

            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        //Cancel Task
        public async Task cancelTask(task_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            statisic_ops so = new statisic_ops(ref cn);

            try {
                // task header
                cm.Add(sqlcancel_task_step1.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnassign,"accnassign");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();
                // task line
                cm.Add(sqlcancel_task_step2.snapsCommand(cn));
                cm.Last().snapsPar(o.orgcode,"orgcode");
                cm.Last().snapsPar(o.site,"site");
                cm.Last().snapsPar(o.depot,"depot");
                cm.Last().snapsPar(o.spcarea,"spcarea");
                cm.Last().snapsPar(o.taskno,"taskno");
                cm.Last().snapsPar(o.lines[0].accnassign,"accnassign");
                cm.Last().snapsPar(o.accnmodify,"accnmodify");
                cm.Last().snapsParsysdateoffset();
                if(o.tasktype == "T") {
                    cm.Add(sqlcancel_task_step3.snapsCommand(cn));
                    cm.Last().snapsPar(o.orgcode,"orgcode");
                    cm.Last().snapsPar(o.site,"site");
                    cm.Last().snapsPar(o.depot,"depot");
                    cm.Last().snapsPar(o.spcarea,"spcarea");
                    cm.Last().snapsPar(o.taskno,"taskno");
                    cm.Last().snapsPar(o.lines[0].accnassign,"accnassign");
                    cm.Last().snapsPar(o.accnmodify,"accnmodify");
                    cm.Last().snapsParsysdateoffset();
                } else if(o.tasktype == "A") {
                    // Un reserved process Stock
                    cm.Add(sqlcancel_reserved_step1.snapsCommand(cn));
                    cm.Last().snapsPar(o.orgcode,"orgcode");
                    cm.Last().snapsPar(o.site,"site");
                    cm.Last().snapsPar(o.depot,"depot");
                    cm.Last().snapsPar(o.taskno,"taskno");
                    cm.Last().snapsPar(o.setno,"setno");

                    cm.Add(sqlcancel_reserved_step2.snapsCommand(cn));
                    cm.Last().snapsPar(o.orgcode,"orgcode");
                    cm.Last().snapsPar(o.site,"site");
                    cm.Last().snapsPar(o.depot,"depot");
                    cm.Last().snapsPar(o.setno,"setno");
                }

                await cm.snapsExecuteTransAsync(cn);

                //Recal stock capacity 
                cm.Clear();
                cm = so.correctionstock(o.orgcode,o.site,o.depot,o.lines[0].article,o.lines[0].pv,o.lines[0].lv);
                //Recal sourceloc location capacity 
                cm.Add(so.location(o.orgcode,o.site,o.depot,o.lines[0].sourceloc));
                //Recal targetloc location capacity 
                cm.Add(so.location(o.orgcode,o.site,o.depot,o.lines[0].targetloc));
                // Execute
                await cm.snapsExecuteTransAsync(cn);

            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        //Confirm Task 
        public async Task confirmTask(task_md o) {
            SqlCommand vl = new SqlCommand(sqlval_digit,cn);
            statisic_ops so = new statisic_ops();
            parameter_ops op = new parameter_ops(cn);

            string loccodeadv = "";
            string verifydigit = "0";
            string huno = "";
            List<SqlCommand> cm = new List<SqlCommand>();
            List<SqlCommand> lc = new List<SqlCommand>();
            try {
                vl.snapsPar(o.orgcode,"orgcode");
                vl.snapsPar(o.site,"site");
                vl.snapsPar(o.depot,"depot");
                vl.snapsPar(o.taskno,"taskno");
                vl.snapsPar(o.confirmdigit,"cfdigit");

                // support pda confirm without digit
                if(o.lines[0].skipdigit == "skip") {
                    verifydigit = "skip";
                } else {
                    verifydigit = vl.snapsScalarStrAsync().Result;
                }

                if(verifydigit == "0") {
                    throw new Exception("location digit is invalid ");
                } else {
                    // get location on task
                    vl.CommandText = sqlconfirm_task_loccode;
                    loccodeadv = vl.snapsScalarStrAsync().Result;

                    // check user is change putaway location adv 
                    if(o.tasktype == "P") {
                       if(o.lines[0].targetloc != loccodeadv) {
                            var pm = await op.getParameterListAsync(o.orgcode,o.site,o.depot,"task","putaway");
                            var allowchangetarget = pm.Where(s => s.pmcode == "allowchangetarget").FirstOrDefault();
                            if(allowchangetarget.pmvalue == false && o.lines[0].targetloc != loccodeadv) {
                                throw new Exception("Can not Change target Location !");
                            }else {
                                vl.CommandText = sqlval_locl;
                                // change target location
                                vl.snapsPar(o.lines[0].targetloc,"lscode");
                                // validate location is valid
                                if(string.IsNullOrEmpty(vl.snapsScalarStrAsync().Result)) {
                                    throw new Exception($"location {o.lines[0].targetloc} is invalid");
                                } else {
                                    // set new location 
                                    loccodeadv = o.lines[0].targetloc;
                                }
                            }
                        }
                    } else if(o.tasktype == "T" && o.lines[0].targetloc != loccodeadv) {
                        throw new Exception("the target location does not match.");
                    } 

                    // get huno on task 
                    vl.CommandText = sqlconfirm_task_huno;
                    huno = vl.snapsScalarStrAsync().Result;
                    //
                    cm.Add(sqlconfirm_task_step1.snapsCommand(cn));
                    cm.Add(sqlconfirm_task_step2.snapsCommand(cn));

                    if(o.tasktype == "A") {
                        var iorefno = o.iorefno;
                        cm.Add(sqlconfirm_task_step4.snapsCommand(cn));
                        cm.Add(sqlconfirm_task_step5.snapsCommand(cn));
                        cm.Add(sqlconfirm_task_step6.snapsCommand(cn));

                        // completed reserve stock
                        cm.Add(sqlstock_reserved_step1.snapsCommand(cn));
                        cm.Add(sqlstock_reserved_step2.snapsCommand(cn));

                    } else {
                        // update stock info replen , putaway , transfer 
                        cm.Add(sqlconfirm_task_step3.snapsCommand(cn));

                        // set finish transfer task
                        if(o.tasktype == "T") { 
                            cm.Add(sqlconfirm_task_step7.snapsCommand(cn));
                        }
                    }

                    cm.ForEach(x => {
                        x.snapsPar(o.orgcode,"orgcode");
                        x.snapsPar(o.site,"site");
                        x.snapsPar(o.depot,"depot");
                        x.snapsPar(o.spcarea,"spcarea");
                        x.snapsPar(o.taskno,"taskno");
                        x.snapsPar(o.accnmodify,"accnmodify");
                        x.snapsPar(o.devid,"device");
                        x.snapsPar(loccodeadv,"loccode");
                        x.snapsPar(huno,"huno");
                        x.snapsPar(o.setno,"setno");
                        x.snapsParsysdateoffset();
                        // full pallet step 4
                        x.snapsPar(o.iorefno,"ouorder");
                    });

                    // block or unboock transfer
                    if(o.tasktype == "T") {
                        SqlCommand atcm = new SqlCommand();
                        SqlCommand vllc = new SqlCommand(sqltransfer_reqblock,cn);
                        vllc.snapsPar(o.orgcode,"orgcode");
                        vllc.snapsPar(o.site,"site");
                        vllc.snapsPar(o.depot,"depot");
                        vllc.snapsPar(o.lines[0].targetloc,"lscode");
                        vllc.snapsPar(o.lines[0].article,"article");
                        vllc.snapsPar(o.lines[0].pv,"pv");
                        vllc.snapsPar(o.lines[0].lv,"lv");
                        vllc.snapsPar(o.lines[0].accnmodify,"accnmodify");
                        vllc.snapsPar(huno,"huno");
                        // check is block or unblock 

                        // 1 block , 0 none block
                        vllc.CommandText = sqltransfer_reqblock;

                        // get source location 
                        vllc.Parameters["lscode"].Value = o.lines[0].sourceloc;
                        string sourceblock = vllc.snapsScalarStrAsync().Result.ToString();

                        // get target location 
                        vllc.Parameters["lscode"].Value = o.lines[0].targetloc;
                        string targetblock = vllc.snapsScalarStrAsync().Result.ToString();

                        // get target huno status
                        vllc.CommandText = sqlstock_status;
                        var huflow = vllc.snapsTableAsync().Result;

                        if(huflow.Rows.Count > 0) {
                            string stocktflow = huflow.Rows[0]["tflow"].ToString();
                            string stockhuid = huflow.Rows[0]["stockid"].ToString();
                            vllc.snapsPar(stockhuid,"stockid");
                            if(sourceblock == "0" && stocktflow == "XX" && targetblock == "1") {
                                // selective block to location block
                                // change XX  to IO skip interface 
                                atcm.CommandText = sqlstock_tfowactive;
                                atcm.snapsPar(o.orgcode,"orgcode");
                                atcm.snapsPar(o.site,"site");
                                atcm.snapsPar(o.depot,"depot");
                                atcm.snapsPar(o.lines[0].accnmodify,"accnmodify");
                                atcm.snapsPar(huno,"huno");
                                cm.Add(atcm);
                            } else if(sourceblock == "0" && targetblock == "0") {
                                // selective to selective skip interface
                            } else if(sourceblock == "0" && targetblock == "1") {
                                // selective to locatoion block send block to GC
                                vllc.snapsPar("B","opstype");
                                vllc.CommandText = sqlstock_block_step2;
                                cm.Add(vllc);
                            } else if(sourceblock == "1" && targetblock == "0") {
                                // location block to selective send Unblock
                                vllc.snapsPar("U","opstype");
                                vllc.CommandText = sqlstock_block_step2;
                                cm.Add(vllc);
                            } else if(sourceblock == "1" && targetblock == "1") {
                                // locatoion block to locatoion block
                            }
                        } else {

                            throw new Exception("invalid stock id huno:" + huno);
                        }
                    }

                    // commit transaction 
                    await cm.snapsExecuteTransAsync(cn);

                    // Calculate statistic for source location task 
                    // Calculate statistic for target location task
                    // Calculate statistic for product state task
                    cm = so.taskmovement(o.orgcode,o.site,o.depot,
                            o.lines[0].sourceloc,o.lines[0].targetloc,
                            o.lines[0].article,o.lines[0].pv.ToString(),
                            o.lines[0].lv.ToString());

                    await cm.snapsExecuteTransAsync(cn);
                }
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(x => x.Dispose()); }
        }

        private String getAdivceLocation(ref SqlCommand cm,taln_md o,string intype = "ST",string thcode = "",string article = "") {
            string spcrecvzone = "";
            string spcrecvaisle = "";
            string spcrecvaisleto = "";
            string spcrecvbay = "";
            string spcrecvbayto = "";
            string spcrecvlevel = "";
            string spcrecvlevelto = "";
            string spcrecvlocation = "";
            string isslowmove = "";
            // product master huheight
            double huheight = 0.0;
            try {
                o.sourceweight = (o.sourceweight.notNull()) ? o.sourceweight : 0;
                o.sourcevolume = (o.sourcevolume.notNull()) ? o.sourcevolume : 0;
                o.article = (o.article.notNull()) ? o.article : "";
                o.thcode = (o.thcode.notNull()) ? o.thcode : "";
                if(cm.Parameters.Count == 0) {

                    cm.snapsPar(o.orgcode,"orgcode");
                    cm.snapsPar(o.site,"site");
                    cm.snapsPar(o.depot,"depot");
                    cm.snapsPar(o.article,"article");
                    cm.snapsPar(o.pv,"pv");
                    cm.snapsPar(o.lv,"lv");
                    cm.snapsPar(o.thcode,"thcode");
                    cm.snapsPar(o.sourcevolume,"crvolume");
                    cm.snapsPar(o.sourceweight,"crweight");
                } else {

                    cm.Parameters["orgcode"].Value = o.orgcode;
                    cm.Parameters["site"].Value = o.site;
                    cm.Parameters["depot"].Value = o.depot;
                    cm.Parameters["article"].Value = o.article;
                    cm.Parameters["pv"].Value = o.pv;
                    cm.Parameters["lv"].Value = o.lv;
                    cm.Parameters["thcode"].Value = o.thcode;
                    cm.Parameters["crvolume"].Value = o.sourcevolume;
                    cm.Parameters["crweight"].Value = o.sourceweight;

                }
                try {

                    if(intype == "RM") {
                        cm.CommandText = sqlputaway_strategic_step2_dm_damage;
                    } else if(intype == "RW") {
                        cm.CommandText = sqlputaway_strategic_step2_wh_warehouse;
                    } else if(intype == "RV") {
                        cm.CommandText = sqlputaway_strategic_step2_vd_vendor;
                    } else if(intype == "FW") {
                        cm.CommandText = sqlputaway_strategic_step2_fw_forward;
                    } else {


                        cm.CommandText = sqlputaway_stategic_step1_product;
                        foreach(System.Data.DataRow ln in cm.snapsTableAsync().Result.Rows) {
                            spcrecvzone = ln["spcrecvzone"].ToString();
                            spcrecvaisle = ln["spcrecvaisle"].ToString();
                            spcrecvaisleto = ln["spcrecvaisleto"].ToString();
                            spcrecvbay = ln["spcrecvbay"].ToString();
                            spcrecvbayto = ln["spcrecvbayto"].ToString();
                            spcrecvlevel = ln["spcrecvlevel"].ToString();
                            spcrecvlevelto = ln["spcrecvlevelto"].ToString();
                            spcrecvlocation = ln["spcrecvlocation"].ToString();
                            isslowmove = ln["isslowmove"].ToString();
                            huheight = ln["huheight"].ToString().CDouble();
                        }

                        // set huheight parameter
                        if(cm.Parameters.IndexOf("huheight") == -1) {
                            cm.snapsPar(huheight,"huheight");
                        } else {
                            cm.Parameters["huheight"].Value = huheight;
                        }

                        /* Prameter Location */
                        parameter_ops parameterops = new parameter_ops(cn);
                        var parameters = parameterops.getParameterListAsync(o.orgcode,o.site,o.depot,"task","putaway");
                        var allowputawaypicking = parameters.Result.Where(x => x.pmcode == "allowputawaypicking").SingleOrDefault();
                        if(allowputawaypicking == null) {
                            allowputawaypicking = new pam_parameter();
                            allowputawaypicking.pmvalue = false;
                        }

                        cm.CommandText = string.Format(
                            sqlputaway_statetic_step2_selection,
                            // index 0
                            string.Format("and lszone ='{0}' ",spcrecvzone), // specific zone // index 1
                            (spcrecvaisle == "") ? "" : " and exists ( select 1 from wm_locup al where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'AL' and lsseq >= " + spcrecvaisle +
                            ((spcrecvaisleto == "") ? "" : " and lsseq <=  " + spcrecvaisleto) + " and l.orgcode = al.orgcode and l.site = al.site and l.depot = al.depot and l.lsaisle = al.lscode)", // specific aisle

                            // index 2
                            (spcrecvbay == "") ? "" : " and exists ( select 1 from wm_locup ab where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'BA' and lsseq >= " + spcrecvbay +
                            ((spcrecvbayto == "") ? "" : " and lsseq <=  " + spcrecvbayto) + " and l.orgcode = ab.orgcode and l.site = ab.site and l.depot = ab.depot and l.lsbay = ab.lscode )", // specific bay 

                            // index 3
                            (spcrecvlevel == "") ? "" : " and exists ( select 1 from wm_locup ab where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'LV' and lsseq >= " + spcrecvlevel +
                            ((spcrecvlevelto == "") ? "" : " and lsseq <=  " + spcrecvlevelto) + " and l.orgcode = ab.orgcode and l.site = ab.site and l.depot = ab.depot and l.lslevel = ab.lscode ) ", // spceic level

                            // index 4
                            (spcrecvlocation == "") ? "" : " and lscode = '" + spcrecvlocation + "' ", // specific location 

                            // index 5
                            (isslowmove == "1") ? "asc" : "desc",// specific slow move selection

                            // index 6
                            huheight,

                            // index 7
                            allowputawaypicking.pmvalue ? "" : " and spcpicking = 0 " // allow putaway picking location         

                        );
                    }

                    return cm.snapsScalarStrAsync().Result;
                } catch(Exception ex) {
                    logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                    // cm.CommandText = sqlputaway_default_bulk; // default bulk when location is not free
                    //return cm.snapsScalarStrAsync().Result;
                    throw ex;
                }

            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw new Exception("Not found location to mathcing with your product receive, so use manaua transfer instead");
            }
        }

        // public async Task generate(tasktype mtype, String huno, String locSource, String locTarget){ 
        //Check HUNO is belonging on some task ( stock is blocking )
        //sqlstategic_step1_stockvalidate

        //Check Location source is available ( Localtion active, weight is over, volume is over, height is over )
        //Check Location source is available ( Location active, weight is over, volume is over heoght is over, sometask is booking ( Reserve only ), temperature is mathcing with article )

        //}

        public List<SqlCommand> generateTaskCommand(task_md o,ref SqlConnection ocn,ref string taskno) {
            sequence_ops so = new sequence_ops();
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                o.taskno = so.taskNext(ref ocn);

                // send out task no
                taskno = o.taskno;
                return getcommand(o);
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { so.Dispose(); }
        }

        public async Task generate(List<task_md> o) {
            List<SqlCommand> lm = new List<SqlCommand>();
            SqlCommand cm = new SqlCommand("sqlins",cn);
            String taskid = "";
            try {
                foreach(task_md sn in o) {
                    lm.Clear();
                    cm.CommandText = "select  NEXT VALUE FOR seq_task rsl";
                    taskid = cm.snapsScalarStrAsync().Result; //generate task no 
                    lm.Add(prepInb(sn,taskid));
                    foreach(taln_md en in sn.lines) {
                        en.orgcode = sn.orgcode;
                        en.site = sn.site;
                        en.depot = sn.depot;
                        en.targetqty = 0;
                        en.collectqty = 0;
                        lm.Add(prepInl(en,taskid,""));
                        lm.Add(prepInl(en,taskid,sn.routeno));
                        lm.Last().CommandText = sqlupdate_hu_fullpallet;
                    }
                    await lm.snapsExecuteTransAsync(cn);
                }
            } catch(Exception ex) {
                var es = o.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.Dispose(); }
        }


        public async Task<List<string>> generateInboundAsync(List<task_md> o) {
            List<SqlCommand> lm = new List<SqlCommand>();
            SqlCommand cm = new SqlCommand("sqlins",cn);
            String taskid = "";
            String loccode = "";
            List<string> taskls = new List<string>();
            try {

                foreach(task_md sn in o) {
                    lm.Clear();
                    cm.CommandText = "select  NEXT VALUE FOR seq_task rsl";
                    taskid = cm.snapsScalarStrAsync().Result; //generate task no 
                    sn.lines[0].targetadv = getAdivceLocation(ref cm,sn.lines[0],sn.intype);
                    sn.lines[0].targetqty = 0; //hardcode
                    sn.lines[0].collectqty = 0; //hardcode
                    lm.Add(prepInb(sn,taskid));
                    lm.Add(prepInl(sn.lines[0],taskid,""));
                    await lm.snapsExecuteTransAsync(cn);
                    taskls.Add(taskid);
                }

                return taskls;
            } catch(Exception ex) {
                var es = o.FirstOrDefault();
                logger.Error(es.orgcode,es.site,es.accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.Dispose(); }
        }

        public async Task UrgenReplenishment(replen_md o) {
            try {
                using(var cm = new SqlCommand("[dbo].[snaps_urgent_replen]",cn)) {
                    cm.CommandType = System.Data.CommandType.StoredProcedure;
                    cm.snapsPar(o.orgcode,"orgcode");
                    cm.snapsPar(o.site,"site");
                    cm.snapsPar(o.depot,"depot");
                    cm.snapsPar(o.zone,"zone");
                    cm.snapsPar(o.aisle,"aisle");
                    cm.snapsPar(o.level,"level");
                    cm.snapsPar(o.location,"loccode");
                    cm.snapsPar(o.article,"article");
                    cm.snapsPar(o.lv,"lv");
                    cm.snapsPar(o.accncode,"accncode");
                    await cm.snapsExecuteAsync();
                }
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accncode,ex,ex.Message);
                throw ex;
            }
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing) {
            if(!disposedValue) {

            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}
