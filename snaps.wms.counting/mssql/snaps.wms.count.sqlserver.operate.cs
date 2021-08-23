using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Snaps.WMS {
    public partial class counting_ops : IDisposable {
        private readonly string cnx;
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        public counting_ops() { }
        public counting_ops(String cx) {
            // data
            cnx = cx;
            cn = new SqlConnection(cx);
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<counting_ops>();
        }
        public counting_ops(SqlConnection cx) { cn = cx; }
        //Task 
        public async Task<List<counttask_md>> listTaskAsync(counttask_md o) {
            SqlCommand cm = new SqlCommand(sqltask_fnd,cn);
            SqlDataReader r = null;
            List<counttask_md> rn = new List<counttask_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsCdn(o.countname,"countcode");
                cm.snapsCdnOrderby("cast(countcode as int) asc");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setCounttask(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();

                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }

        public async Task<counttask_md> getTaskAsync(counttask_md o) {
            SqlCommand cm = new SqlCommand(sqltask_fnd,cn);
            SqlDataReader r = null;
            counttask_md rn = new counttask_md();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsCdn(o.countcode,"countcode");
                cm.snapsCdnOrderby("cast(countcode as int) asc");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = setCounttask(ref r); }
                await r.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,
                ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }

        public async Task upsertTaskAsnc(counttask_md o) {
            SqlCommand cm = new SqlCommand(sqltask_vald,cn);
            snapsCmdTag cmd = new snapsCmdTag("Counttask");
            try {
                if(string.IsNullOrEmpty(o.counttype)) throw new Exception("Count Name is Required !");
                if(string.IsNullOrEmpty(o.countname)) throw new Exception("Count Name is Required !");
                if(string.IsNullOrEmpty(o.tflow)) throw new Exception("Please Select New Task");
                if(o.datestart == null) throw new Exception("Please Enter Start Date");
                if(o.dateend == null) throw new Exception("Please Enter End Date ");

                fillCommand(ref cm,o);

                if(o.tflow == "NW") {
                    cm.CommandText = sqltask_insert;
                    cmd.Addtags("Create New Count Task",cm.Clone());
                } else {
                    if(o.tflow == "ED") {
                        // clear count line state is IO
                        cm.CommandText = sqltask_closed_line;
                        cmd.Addtags($"{o.countcode} Update Count Line state is IO to XX",cm.Clone());

                        // clear count plan state is IO
                        cm.CommandText = sqltask_closed_plan;
                        cmd.Addtags($"{o.countcode} Update Count Plan state is IO to XX",cm.Clone());
                    }

                    cm.CommandText = sqltask_update;
                    cmd.Addtags($"{o.countcode} Update Count Task for type {o.counttype} => {o.tflow}",cm.Clone());
                }


                var result = await cmd.snapsExecuteTagsAsync(cn);
                try {
                    foreach(string affectedRes in result.SerializeTag()) {
                        logger.Debug(o.orgcode,o.site,o.countcode,affectedRes);
                    }
                } catch(Exception) { } finally { result.cmdTags.Clear(); }
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,
                ex,ex.Message);
                throw ex;
            } finally { await cm.DisposeAsync(); }
        }

        public async Task removeTaskAsync(counttask_md o) {
            SqlCommand cm = new SqlCommand(sqltask_remove_step1,cn);
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");
                await cm.snapsExecuteAsync();

                cm.CommandText = sqltask_remove_step2;
                await cm.snapsExecuteAsync();

                cm.CommandText = sqltask_remove_step3;
                await cm.snapsExecuteAsync();
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,
                ex,ex.Message);
                throw ex;
            } finally {
                await cm.DisposeAsync();
            }
        }

        //Plan 
        public async Task<List<countplan_md>> listPlanAsync(counttask_md o) {
            SqlCommand cm = new SqlCommand(sqlplan_fnd,cn);
            SqlDataReader r = null;
            List<countplan_md> rn = new List<countplan_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setCountplan(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,
                ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }

        public async Task<countplan_md> getPlanAsync(countplan_md o) {
            SqlCommand cm = new SqlCommand(sqlplan_fnd,cn);
            SqlDataReader r = null;
            countplan_md rn = new countplan_md();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");
                cm.snapsPar(o.plancode,"plancode");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = setCountplan(ref r); }
                await r.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,
                ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }


        public async Task upsertPlanAysnc(countplan_md o) {
            SqlCommand cm = new SqlCommand(sqlplan_vald,cn);
            snapsCmdTag cmt = new snapsCmdTag("Countpan");
            try {
                if(o.tflow == "NW") {
                    fillCommand(ref cm,o);

                    cm.CommandText = sqlbulk_zone;
                    if(cm.snapsScalarStrAsync().Result == "0") {
                        cm.CommandText = sqlplan_select_step1_1;
                    } else {
                        cm.CommandText = sqlplan_select_step1_2;
                    }
                    var dt = cm.snapsTableAsync().Result;

                    if(dt.Rows.Count == 0) {
                        // no location 
                        throw new Exception("Data not found.");
                    } else {
                        DataTable tablePln = new DataTable();
                        if(o.isoddeven == 1) {
                            DataTable tableOne = new DataTable();
                            DataTable tableTow = new DataTable();
                            tableOne = dt.Select("oddeven = 1").CopyToDataTable();
                            tableTow = dt.Select("oddeven = 2").CopyToDataTable();

                            if(tableOne.Rows.Count > 0) {
                                tableOne.DefaultView.Sort = "loccode asc";
                                tablePln.Merge(tableOne.DefaultView.ToTable());
                            }

                            if(tableTow.Rows.Count > 0) {
                                tableTow.DefaultView.Sort = "loccode desc";
                                tablePln.Merge(tableTow.DefaultView.ToTable());
                            }
                        } else {
                            dt.DefaultView.Sort = "loccode asc";
                            tablePln = dt.DefaultView.ToTable();
                        }

                        cm.CommandText = "select next value for seq_ccp";
                        o.plancode = cm.snapsScalarStrAsync().Result;
                        cm.CommandText = sqlplan_insert_step1;
                        cm.Parameters["tflow"].Value = "IO";
                        cm.Parameters["plancode"].Value = o.plancode;
                        // await cm.snapsExecuteAsync();
                        cmt.Addtags($"{o.countcode}-{o.plancode} insert wm coupn for new plan ",cm);


                        int locseq = 0;
                        //var pm = new List<SqlCommand>();
                        foreach(DataRow rw in tablePln.Rows) {
                            locseq++;
                            var cx = new SqlCommand(sqlplan_insert_step2_1,cn);
                            cx.Parameters.AddWithValue("orgcode",rw["orgcode"]);
                            cx.Parameters.AddWithValue("site",rw["site"]);
                            cx.Parameters.AddWithValue("depot",rw["depot"]);
                            cx.Parameters.AddWithValue("spcarea",rw["spcarea"]);
                            cx.Parameters.AddWithValue("countcode",o.countcode);
                            cx.Parameters.AddWithValue("plancode",o.plancode);
                            cx.Parameters.AddWithValue("loccode",rw["loccode"]);
                            cx.Parameters.AddWithValue("unitcount",rw["unitcount"]);
                            cx.Parameters.AddWithValue("tflow","IO");
                            cx.Parameters.AddWithValue("accncreate",o.accnmodify);
                            cx.Parameters.AddWithValue("accnmodify",o.accnmodify);
                            cx.Parameters.AddWithValue("procmodify",rw["procmodify"]);
                            cx.Parameters.AddWithValue("stbarcode",rw["barcode"]);
                            cx.Parameters.AddWithValue("starticle",rw["article"]);
                            cx.Parameters.AddWithValue("stpv",rw["pv"]);
                            cx.Parameters.AddWithValue("stlv",rw["lv"]);
                            cx.Parameters.AddWithValue("stqtysku",rw["qtypu"]);
                            cx.Parameters.AddWithValue("stqtypu",rw["qtysku"]);
                            cx.Parameters.AddWithValue("stlotmfg",rw["batchno"]);
                            cx.Parameters.AddWithValue("stdatemfg",rw["datemfg"]);
                            cx.Parameters.AddWithValue("stdateexp",rw["dateexp"]);
                            cx.Parameters.AddWithValue("stserialno",rw["serialno"]);
                            cx.Parameters.AddWithValue("sthuno",rw["huno"]);
                            cx.Parameters.AddWithValue("locseq",locseq);
                            cx.Parameters.AddWithValue("locctype",rw["locctype"]);

                            cmt.Addtags($"{o.countcode}-{o.plancode} insert wm couln for new {rw["loccode"]} {rw["huno"]} {rw["article"]} {rw["lv"]}",cx);
                            //pm.Add(cx);
                        }
                        // insert data
                        //await pm.snapsExecuteAsync();

                        var result = await cmt.snapsExecuteTagsAsync(cn);
                        try {
                            foreach(string affectedRes in result.SerializeTag()) {
                                logger.Debug(o.orgcode,o.site,o.countcode,affectedRes);
                            }
                        } catch(Exception) { } finally { result.cmdTags.Clear(); }
                    }

                } else {
                    cm.CommandText = sqlplan_update;
                    fillCommand(ref cm,o);
                    await cm.snapsExecuteAsync();
                }
            } catch(Exception ex) { logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message); throw ex; } finally { await cm.DisposeAsync(); }
        }

        public async Task removePlanAsync(countplan_md o) {
            SqlCommand cm = new SqlCommand(sqlplan_cancel_step1,cn);
            snapsCmdTag cmt = new snapsCmdTag("Countplan");
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");
                cm.snapsPar(o.plancode,"plancode");
                cm.snapsPar(o.accnmodify,"accnmodify");
                cmt.Addtags($"{o.countcode}-{o.plancode} Cancle Plan",cm);

                //await cm.snapsExecuteAsync();
                //logger.Debug(o.orgcode,o.site,o.accnmodify,"Cancle Plan Successful");

                var ncm = cm.Clone();
                ncm.CommandText = sqlplan_cancel_step2;

                cmt.Addtags($"{o.countcode}-{o.plancode} Cancle Plan",ncm);
                var result = await cmt.snapsExecuteTagsAsync(cn);
                try {
                    foreach(string affectedRes in result.SerializeTag()) {
                        logger.Debug(o.orgcode,o.site,o.countcode,affectedRes);
                    }
                } catch(Exception) { } finally { result.cmdTags.Clear(); }

                //await cm.snapsExecuteAsync();
                //logger.Info(o.orgcode,o.site,o.accnmodify,"Cancle Plan Line Successful");
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally {
                await cm.DisposeAsync();
            }
        }
        void fillcommand(ref SqlCommand c,countplan_md o) {
            c.snapsPar(o.orgcode,"orgcode");
            c.snapsPar(o.site,"site");
            c.snapsPar(o.depot,"depot");
            c.snapsPar(o.countcode,"countcode");
            c.snapsPar(o.plancode,"plancode");
            c.snapsPar(o.accnmodify,"accnmodify");
            c.snapsPar(o.pctvld,"pctvld");
            c.snapsPar(o.remarksvld,"remarksvld");
        }
        public async Task validatePlanAsync(countplan_md o) {
            SqlCommand cm = new SqlCommand("",cn);
            SqlCommand cmn = new SqlCommand("",cn);
            SqlCommand cml = new SqlCommand("",cn);

            List<SqlCommand> lm = new List<SqlCommand>();
            try {
                // gen parameter
                fillcommand(ref cm,o);
                fillcommand(ref cmn,o);
                fillcommand(ref cml,o);
                logger.Debug(o.orgcode,o.site,o.accnmodify,"fillcommand Successful");
                // is input count qty
                cm.CommandText = sqlplan_valcount_step0;
                if(cm.snapsScalarStrAsync().Result != "0") { throw new Exception("Count qty is Required !"); }

                // check count type
                cm.CommandText = sqlplan_iscyclecount;
                if(cm.snapsScalarStrAsync().Result == "1") {
                    // cycle count plan
                    cmn.CommandText = sqlplan_valcount_step1;
                    lm.Add(cmn);
                    // cycle count line
                    cml.CommandText = sqlplan_valcount_step2;
                    lm.Add(cml);

                    await lm.snapsExecuteTransAsync(cn);
                    logger.Debug(o.orgcode,o.site,o.accnmodify,"Cycle Count Validated Successful");
                } else {
                    // Replan Process
                    lm.Clear();
                    cmn.CommandText = sqlplan_valcount_line;
                    lm.Add(cmn);

                    cml.CommandText = sqlplan_valcount_head;
                    lm.Add(cml);

                    await lm.snapsExecuteTransAsync(cn);
                    logger.Debug(o.orgcode,o.site,o.accnmodify,"Stock Take Validated");

                    lm.Clear();
                    cm.CommandText = sqlplan_valcount_isre;
                    if(cm.snapsScalarStrAsync().Result != "0") {
                        cm.CommandText = "select next value for seq_ccp";
                        var newplan = cm.snapsScalarStrAsync().Result;

                        cmn.snapsPar(newplan,"newplan");
                        cmn.CommandText = sqlplan_recount_line;
                        lm.Add(cmn);

                        string sufix = "-Recount " + o.pctvld + "%";
                        var newname = o.planname.IndexOf("-Recount") == -1
                            ? o.planname
                            : o.planname.Substring(0,o.planname.IndexOf("-Recount"));

                        cml.snapsPar(newplan,"newplan");
                        cml.snapsPar(newname + sufix,"@planname");
                        cml.CommandText = sqlplan_recount_head;
                        lm.Add(cml);

                        await lm.snapsExecuteTransAsync(cn);
                        logger.Debug(o.orgcode,o.site,o.accnmodify,"Replan Successful");
                    } else {
                        logger.Debug(o.orgcode,o.site,o.accnmodify,"Completed without replan");
                    }
                }
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally {
                await cmn.DisposeAsync();
            }
        }
        //Line 
        public async Task<List<countline_md>> listLineAsync(countplan_md o) {
            SqlCommand cm = new SqlCommand(sqlline_fnd,cn);
            SqlDataReader r = null;
            List<countline_md> rn = new List<countline_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");
                cm.snapsPar(o.plancode,"plancode");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setCountline(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }
        public async Task<List<countline_md>> countLineAsync(countplan_md o) {
            SqlCommand cm = new SqlCommand(sqlcount_fnd,cn);
            SqlDataReader r = null;
            List<countline_md> rn = new List<countline_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");
                cm.snapsPar(o.plancode,"plancode");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setCountline(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }
        //Line 
        public async Task<List<countline_md>> getLineAsync(findcountline_md o) {
            SqlCommand cm = new SqlCommand(sqlline_fnd,cn);
            SqlDataReader r = null;
            List<countline_md> rn = new List<countline_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");
                cm.snapsPar(o.plancode,"plancode");
                // for pda scan
                cm.snapsCdn(o.loccode,"loccode"," and loccode = @loccode");
                cm.snapsCdnOrderby("locseq asc");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setCountline(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,"Plan code " + o.plancode,ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }
        public async Task upsertLineAsync(List<countline_md> o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            SqlCommand vl = new SqlCommand(sqlplan_valcount_close,cn);
            try {
                if(o.Count == 0) return;
                vl.snapsPar(o[0].orgcode,"orgcode");
                vl.snapsPar(o[0].site,"site");
                vl.snapsPar(o[0].depot,"depot");
                vl.snapsPar(o[0].countcode,"countcode");
                vl.snapsPar(o[0].plancode,"plancode");
                if(vl.snapsScalarStrAsync().Result != "0") {
                    throw new Exception("Plan is Closed or Canceled");
                } else {
                    foreach(countline_md ln in o) {
                        /* Change Article line */
                        if(ln.cnflow == "NW") {
                            ln.cnflow = "IO";
                            ln.cnmsg = "saved";

                            /*set NEW line */
                            var nw = getCommand(ln,sqlcount_newcheck);

                            // check exists insert
                            if(nw.snapsScalarStrAsync().Result == "0") {
                                nw.CommandText = sqlcount_newline;
                                cm.Add(nw);
                            } else {
                                throw new Exception("New Count line is duplicate");
                            }
                        } else {
                            // update row
                            ln.cnflow = "IO";
                            ln.cnmsg = "saved";
                            cm.Add(getCommand(ln,sqlline_update));
                        }
                    }
                    await cm.snapsExecuteTransAsync(cn);
                }
            } catch(Exception ex) {
                logger.Error(o[0].orgcode,o[0].site,o[0].accnmodify,ex,ex.Message);
                throw ex;
            } finally { cm.ForEach(e => e.Dispose()); }
        }

        public async Task upsertLineAsync(countline_md o) {
            SqlCommand cm = new SqlCommand(sqlline_vald,cn);
            try {
                string isInsert = cm.snapsScalarStrAsync().Result.ToString();
                fillCommand(ref cm,o);
                cm.CommandText = isInsert == "0" ? sqlline_insert : sqlline_update;
                await cm.snapsExecuteAsync();
                logger.Debug(o.orgcode,o.site,o.accnmodify,isInsert == "0" ? "Insert Line Successful" : "Update Line Successful");
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            } finally { await cm.DisposeAsync(); }
        }
        public async Task<List<countcorrection_md>> getConfrimLineAsync(counttask_md o) {
            SqlCommand cm = new SqlCommand(sqlcount_correctionline,cn);
            SqlDataReader r = null;
            List<countcorrection_md> rn = new List<countcorrection_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.countcode,"countcode");

                // check pending close plan
                cm.CommandText = sqlcount_isvalidateplan;
                if(cm.snapsScalarStrAsync().Result == "0") {
                    // get currection items
                    cm.CommandText = sqlcount_correctionline;
                    r = await cm.snapsReadAsync();
                    while(await r.ReadAsync()) {
                        rn.Add(setCountcorrection(ref r));
                    }
                    await r.CloseAsync();
                }
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,
                ex,ex.Message);
                throw ex;
            } finally {
                if(r != null) { await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }

        public async Task<bool> countConfirmAsync(counttask_md o) {
            SqlCommand vd = new SqlCommand(sqlcount_isvalidateplan,cn);
            try {
                vd.snapsPar(o.orgcode,"orgcode");
                vd.snapsPar(o.site,"site");
                vd.snapsPar(o.depot,"depot");
                vd.snapsPar(o.countcode,"countcode");
                vd.CommandText = sqlcount_isvalidateplan;
                if(vd.snapsScalarStrAsync().Result == "0") {
                    using(SqlConnection cnx = cn) {
                        cn.Open();
                        using(SqlCommand cm = new SqlCommand()) {
                            cm.Connection = cn;
                            cm.CommandType = CommandType.StoredProcedure;
                            cm.CommandText = "snaps_stocktake_confirm";
                            cm.snapsPar(o.orgcode,"orgcode");
                            cm.snapsPar(o.site,"site");
                            cm.snapsPar(o.depot,"depot");
                            cm.snapsPar(o.countcode,"countcode");
                            cm.snapsPar(o.accnmodify,"@coraccn");
                            await cm.ExecuteNonQueryAsync();
                        }
                        cn.Close();
                    }
                } else {
                    throw new Exception("Count data not found");
                }
                return true;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.accnmodify,ex,ex.Message);
                throw ex;
            }
        }


        //Dispose
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing) {
            if(!disposedValue) {
                if(cn != null) { cn.Dispose(); }
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }
}