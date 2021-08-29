using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Snaps.WMS {
    public partial class mergehu_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        private readonly string cs;
        private bool disposedValue;

        public mergehu_ops() { }
        public mergehu_ops(string cx) {
            cn = new SqlConnection(cx);
            cs = cx;
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<correction_ops>();
        }

        public async Task<merge_md> Generate(merge_set os) {
            merge_md md = new merge_md();
            try {
                // generate hu
                using(var con = new SqlConnection(cs)) {
                    con.Open();
                    // check location is active in system
                    using(var cm = new SqlCommand(sqlmergehu_chkloc,con)) {
                        cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                        cm.Parameters.AddWithValue("@site",os.site);
                        cm.Parameters.AddWithValue("@depot",os.depot);
                        cm.Parameters.AddWithValue("@loccode",os.loccode);
                        int isloccode = Convert.ToInt32(cm.ExecuteScalar());
                        if(isloccode == 0) throw new Exception("location is invalid !");
                    }

                    using(var cm = new SqlCommand("[dbo].[snaps_stocktake_genhuno]",con)) {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                        cm.Parameters.AddWithValue("@site",os.site);
                        cm.Parameters.AddWithValue("@depot",os.depot);
                        cm.Parameters.AddWithValue("@loccode",os.loccode);
                        cm.Parameters.Add(new SqlParameter("@huno",SqlDbType.VarChar,30));
                        cm.Parameters.Add(new SqlParameter("@hutype",SqlDbType.VarChar,30));
                        cm.Parameters.Add(new SqlParameter("@spcarea",SqlDbType.VarChar,30));
                        cm.Parameters["@huno"].Direction = ParameterDirection.Output;
                        cm.Parameters["@hutype"].Direction = ParameterDirection.Output;
                        cm.Parameters["@spcarea"].Direction = ParameterDirection.Output;
                        await cm.ExecuteNonQueryAsync();

                        // generate result
                        os.huno = (string)cm.Parameters["@huno"].Value;
                        os.hutype = (string)cm.Parameters["@hutype"].Value;
                        os.spcarea = (string)cm.Parameters["@spcarea"].Value;

                        // merge transaction
                        md.orgcode = os.orgcode;
                        md.site = os.site;
                        md.depot = os.depot;
                        md.spcarea = os.spcarea;
                        md.hutype = os.hutype;
                        md.hutarget = os.huno;
                        md.loccode = os.loccode;
                        md.tflow = "IO";
                        md.datecreate = DateTime.Now;
                        md.accncreate = os.accncode;
                        md.datemodify = DateTime.Now;
                        md.accnmodify = os.accncode;
                        md.procmodify = "wms.mergesetup";
                        md.remarks = "";
                        md.lines = new List<mergehu_ln>();

                        cm.Parameters.Clear();
                        cm.CommandText = sqlmergehu_insert;
                        cm.CommandType = CommandType.Text;
                        cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                        cm.Parameters.AddWithValue("@site",os.site);
                        cm.Parameters.AddWithValue("@depot",os.depot);
                        cm.Parameters.AddWithValue("@spcarea",os.spcarea);
                        cm.Parameters.AddWithValue("@mergeno",os.mergeno);
                        cm.Parameters.AddWithValue("@hutype",os.hutype);
                        cm.Parameters.AddWithValue("@hutarget",os.huno);
                        cm.Parameters.AddWithValue("@loccode",os.loccode);
                        cm.Parameters.AddWithValue("@tflow","IO");
                        cm.Parameters.AddWithValue("@datecreate",DateTime.Now);
                        cm.Parameters.AddWithValue("@accncreate",os.accncode);
                        cm.Parameters.AddWithValue("@datemodify",DateTime.Now);
                        cm.Parameters.AddWithValue("@accnmodify",os.accncode);
                        cm.Parameters.AddWithValue("@procmodify","wms.mergesetup");
                        cm.Parameters.AddWithValue("@remarks","");

                        // merge transaction id
                        md.mergeno = Convert.ToInt32(await cm.ExecuteScalarAsync());
                    }
                }

                return md;
            } catch(Exception ex) {
                logger.Error(os.orgcode,os.site,os.accncode,ex,ex.Message);
                throw ex;
            }

        }
        public async Task<bool> Cancel(merge_set os) {
            try {
                // generate hu
                using(var con = new SqlConnection(cs)) {
                    con.Open();
                    // check location is active in system
                    using(var cm = new SqlCommand(sqlmergehu_cancel,con)) {
                        cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                        cm.Parameters.AddWithValue("@site",os.site);
                        cm.Parameters.AddWithValue("@depot",os.depot);
                        cm.Parameters.AddWithValue("@spcarea",os.spcarea);
                        cm.Parameters.AddWithValue("@mergeno",os.mergeno);
                        cm.Parameters.AddWithValue("@huno",os.huno);
                        cm.Parameters.AddWithValue("@tflow","CL");
                        cm.Parameters.AddWithValue("@accnmodify",os.accncode);
                        int affectedrow = await cm.ExecuteNonQueryAsync();
                        return true;
                    }
                }

            } catch(Exception ex) {
                logger.Error(os.orgcode,os.site,os.accncode,ex,ex.Message);
                throw ex;
            }
        }
        public async Task Label(merge_set os) {
            try {
                // print label hu
            } catch(Exception ex) {
                logger.Error(os.orgcode,os.site,os.accncode,ex,ex.Message);
                throw ex;
            }
        }
        public async Task<List<mergehu_md>> MergeList(merge_find of) {
            try {
                List<mergehu_md> ln = new List<mergehu_md>();
                using(var con = new SqlConnection(cs)) {
                    con.Open();
                    using(var cm = new SqlCommand(sqlmergeln_mergelist,con)) {
                        cm.CommandType = CommandType.Text;
                        cm.Parameters.AddWithValue("@orgcode",of.orgcode);
                        cm.Parameters.AddWithValue("@site",of.site);
                        cm.Parameters.AddWithValue("@depot",of.depot);
                        //cm.Parameters.AddWithValue("@loccode",of.loccode ?? "");
                        //cm.Parameters.AddWithValue("@huno",of.huno ?? "");
                        cm.snapsCdn(of.loccode,"loccode");
                        cm.snapsCdn(of.huno,"hutarget");
                        cm.snapsCdn(of.datecreate,"datecreate"," and cast(datecreate as date) = cast(@datecreate as date)");
                        using(SqlDataReader r = await cm.ExecuteReaderAsync()) {
                            while(r.Read()) {
                                mergehu_md rl = new mergehu_md();
                                rl.orgcode = (string)r["orgcode"];
                                rl.site = (string)r["site"];
                                rl.depot = (string)r["depot"];
                                rl.spcarea = (string)r["spcarea"];
                                rl.mergeno = (int)r["mergeno"];
                                rl.hutype = (string)r["hutype"];
                                rl.hutarget = (string)r["hutarget"];
                                rl.loccode = (string)r["loccode"];
                                rl.tflow = (string)r["tflow"];
                                rl.tflowdes = (string)r["tflowdes"];
                                rl.datecreate = (DateTimeOffset?)r["datecreate"];
                                rl.accncreate = (string)r["accncreate"];
                                rl.datemodify = (DateTimeOffset?)r["datemodify"];
                                rl.accnmodify = (string)r["accnmodify"];
                                rl.procmodify = (string)r["procmodify"];
                                rl.remarks = (string)r["remarks"];
                                ln.Add(rl);
                            }
                        }
                    }
                }
                return ln;
            } catch(Exception ex) {
                logger.Error(of.orgcode,of.site,of.accncode,ex,ex.Message);
                throw ex;
            }
        }
        public async Task<List<mergehu_ln>> Mergeline(mergehu_md of) {
            try {
                // generate hu
                List<mergehu_ln> ln = new List<mergehu_ln>();
                using(var con = new SqlConnection(cs)) {
                    con.Open();
                    using(var cm = new SqlCommand(sqlmergeln_mergeline,con)) {
                        cm.CommandType = CommandType.Text;
                        cm.Parameters.AddWithValue("@orgcode",of.orgcode);
                        cm.Parameters.AddWithValue("@site",of.site);
                        cm.Parameters.AddWithValue("@depot",of.depot);
                        cm.Parameters.AddWithValue("@mergeno",of.mergeno);
                        using(SqlDataReader r = await cm.ExecuteReaderAsync()) {
                            while(r.Read()) {
                                mergehu_ln rl = new mergehu_ln();
                                rl.mergeln = (int)r["mergeln"].ValueOrNull();
                                rl.mergeno = (int)r["mergeno"].ValueOrNull();
                                rl.orgcode = (string)r["orgcode"].ValueOrNull();
                                rl.site = (string)r["site"].ValueOrNull();
                                rl.depot = (string)r["depot"].ValueOrNull();
                                rl.spcarea = (string)r["spcarea"].ValueOrNull();
                                rl.stockid = (decimal)r["stockid"].ValueOrNull();
                                rl.hutype = (string)r["hutype"].ValueOrNull();
                                rl.loccode = (string)r["loccode"].ValueOrNull();
                                rl.huno = (string)r["huno"].ValueOrNull();
                                rl.inrefno = (string)r["inrefno"].ValueOrNull();
                                rl.inrefln = (int?)r["inrefln"].ValueOrNull();
                                rl.inagrn = (string)r["inagrn"].ValueOrNull();
                                rl.ingrno = (string)r["ingrno"].ValueOrNull();
                                rl.article = (string)r["article"].ValueOrNull();
                                rl.pv = (int)r["pv"].ValueOrNull();
                                rl.lv = (int)r["lv"].ValueOrNull();
                                rl.descalt = (string)r["descalt"].ValueOrNull();
                                rl.qtysku = (int)r["qtysku"];
                                rl.qtypu = (decimal)r["qtypu"].ValueOrNull();
                                rl.qtyweight = (decimal)r["qtyweight"].ValueOrNull();
                                rl.qtyvolume = (decimal)r["qtyvolume"].ValueOrNull();
                                rl.qtyunit = (string)r["qtyunit"].ValueOrNull();
                                rl.qtyunitdes = (string)r["qtyunitdes"].ValueOrNull();
                                rl.daterec = (DateTimeOffset?)r["daterec"].ValueOrNull();
                                rl.batchno = (string)r["batchno"].ValueOrNull();
                                rl.lotno = (string)r["lotno"].ValueOrNull();
                                rl.datemfg = (DateTimeOffset?)r["datemfg"].ValueOrNull();
                                rl.serialno = (string)r["serialno"].ValueOrNull();
                                rl.dateexp = (DateTimeOffset?)r["dateexp"].ValueOrNull();
                                rl.tflowops = (string)r["tflowops"].ValueOrNull();
                                rl.tflowdes = (string)r["tflowdes"].ValueOrNull();
                                rl.tflowsign = (string)r["tflowsign"].ValueOrNull();
                                rl.skuops = (int)r["skuops"].ValueOrNull();
                                rl.puops = (decimal)r["puops"].ValueOrNull();
                                rl.weightops = (decimal)r["weightops"].ValueOrNull();
                                rl.volumeops = (decimal)r["volumeops"].ValueOrNull();
                                rl.unitops = (string)r["unitops"].ValueOrNull();
                                rl.unitopsdes = (string)r["unitopsdes"].ValueOrNull();
                                rl.refops = (string)r["refops"].ValueOrNull();
                                rl.reflnops = (int?)r["reflnops"].ValueOrNull();
                                rl.remarks = (string)r["remarks"].ValueOrNull();
                                rl.tflow = (string)r["tflow"].ValueOrNull();
                                rl.datecreate = (DateTimeOffset?)r["datecreate"].ValueOrNull();
                                rl.accncreate = (string)r["accncreate"].ValueOrNull();
                                rl.datemodify = (DateTimeOffset?)r["datemodify"].ValueOrNull();
                                rl.accnmodify = (string)r["accnmodify"].ValueOrNull();
                                rl.procmodify = (string)r["procmodify"].ValueOrNull();
                                rl.msgops = (string)r["msgops"].ValueOrNull();
                                ln.Add(rl);
                            }
                        }
                    }
                }
                return ln;
            } catch(Exception ex) {
                logger.Error(of.orgcode,of.site,of.accnmodify,ex,ex.Message);
                throw ex;
            }
        }
        public async Task<List<mergehu_ln>> Find(merge_find of) {
            try {
                // generate hu
                List<mergehu_ln> ln = new List<mergehu_ln>();
                using(var con = new SqlConnection(cs)) {
                    con.Open();
                    using(var cm = new SqlCommand(sqlmergeln_find,con)) {
                        cm.CommandType = CommandType.Text;
                        cm.Parameters.AddWithValue("@orgcode",of.orgcode);
                        cm.Parameters.AddWithValue("@site",of.site);
                        cm.Parameters.AddWithValue("@depot",of.depot);
                        //cm.Parameters.AddWithValue("@spcarea",of.spcarea);
                        cm.Parameters.AddWithValue("@loccode",of.loccode ?? "");
                        cm.Parameters.AddWithValue("@huno",of.huno ?? "");
                        cm.Parameters.AddWithValue("@article",of.article ?? "");

                        using(SqlDataReader r = await cm.ExecuteReaderAsync()) {
                            while(r.Read()) {
                                mergehu_ln rl = new mergehu_ln();
                                rl.mergeln = (int)r["mergeln"].ValueOrNull();
                                rl.mergeno = (int)r["mergeno"].ValueOrNull();
                                rl.orgcode = (string)r["orgcode"].ValueOrNull();
                                rl.site = (string)r["site"].ValueOrNull();
                                rl.depot = (string)r["depot"].ValueOrNull();
                                rl.spcarea = (string)r["spcarea"].ValueOrNull();
                                rl.stockid = (decimal)r["stockid"].ValueOrNull();
                                rl.hutype = (string)r["hutype"].ValueOrNull();
                                rl.loccode = (string)r["loccode"].ValueOrNull();
                                rl.huno = (string)r["huno"].ValueOrNull();
                                rl.inrefno = (string)r["inrefno"].ValueOrNull();
                                rl.inrefln = (int?)r["inrefln"].ValueOrNull();
                                rl.inagrn = (string)r["inagrn"].ValueOrNull();
                                rl.ingrno = (string)r["ingrno"].ValueOrNull();
                                rl.article = (string)r["article"].ValueOrNull();
                                rl.pv = (int)r["pv"].ValueOrNull();
                                rl.lv = (int)r["lv"].ValueOrNull();
                                rl.descalt = (string)r["descalt"].ValueOrNull();
                                rl.qtysku = (int)r["qtysku"];
                                rl.qtypu = (decimal)r["qtypu"].ValueOrNull();
                                rl.qtyweight = (decimal)r["qtyweight"].ValueOrNull();
                                rl.qtyvolume = (decimal)r["qtyvolume"].ValueOrNull();
                                rl.qtyunit = (string)r["qtyunit"].ValueOrNull();
                                rl.qtyunitdes = (string)r["qtyunitdes"].ValueOrNull();
                                rl.daterec = (DateTimeOffset?)r["daterec"].ValueOrNull();
                                rl.batchno = (string)r["batchno"].ValueOrNull();
                                rl.lotno = (string)r["lotno"].ValueOrNull();
                                rl.datemfg = (DateTimeOffset?)r["datemfg"].ValueOrNull();
                                rl.serialno = (string)r["serialno"].ValueOrNull();
                                rl.dateexp = (DateTimeOffset?)r["dateexp"].ValueOrNull();
                                rl.tflowops = (string)r["tflowops"].ValueOrNull();
                                rl.tflowdes = (string)r["tflowdes"].ValueOrNull();
                                rl.tflowsign = (string)r["tflowsign"].ValueOrNull();
                                rl.skuops = (int)r["skuops"].ValueOrNull();
                                rl.puops = (decimal)r["puops"].ValueOrNull();
                                rl.weightops = (decimal)r["weightops"].ValueOrNull();
                                rl.volumeops = (decimal)r["volumeops"].ValueOrNull();
                                rl.unitops = (string)r["unitops"].ValueOrNull();
                                rl.unitopsdes = (string)r["unitopsdes"].ValueOrNull();
                                rl.refops = (string)r["refops"].ValueOrNull();
                                rl.reflnops = (int?)r["reflnops"].ValueOrNull();
                                rl.remarks = (string)r["remarks"].ValueOrNull();
                                rl.tflow = (string)r["tflow"].ValueOrNull();
                                rl.datecreate = (DateTimeOffset?)r["datecreate"].ValueOrNull();
                                rl.accncreate = (string)r["accncreate"].ValueOrNull();
                                rl.datemodify = (DateTimeOffset?)r["datemodify"].ValueOrNull();
                                rl.accnmodify = (string)r["accnmodify"].ValueOrNull();
                                rl.procmodify = (string)r["procmodify"].ValueOrNull();
                                rl.msgops = (string)r["msgops"].ValueOrNull();
                                ln.Add(rl);
                            }
                        }
                    }
                }
                return ln;
            } catch(Exception ex) {
                logger.Error(of.orgcode,of.site,of.accncode,ex,ex.Message);
                throw ex;
            }
        }
        public async Task<bool> Mergehu(merge_md os) {
            try {
                // generate hu
                using(var con = new SqlConnection(cs)) {
                    con.Open();
                    // check location is active in system
                    SqlTransaction tr = con.BeginTransaction("mergehu_ln");
                    using(var cm = new SqlCommand()) {
                        cm.Transaction = tr;
                        cm.Connection = con;
                        cm.CommandText = sqlmergeln_insert;
                        try {
                            // Line
                            foreach(mergehu_ln o in os.lines) {
                                cm.Parameters.Clear();
                                //cm.Parameters.AddWithValue("@mergeln",o.mergeln);
                                cm.Parameters.AddWithValue("@mergeno",o.mergeno);
                                cm.Parameters.AddWithValue("@orgcode",o.orgcode);
                                cm.Parameters.AddWithValue("@site",o.site);
                                cm.Parameters.AddWithValue("@depot",o.depot);
                                cm.Parameters.AddWithValue("@spcarea",o.spcarea);
                                cm.Parameters.AddWithValue("@stockid",o.stockid);
                                cm.Parameters.AddWithValue("@hutype",o.hutype);
                                cm.Parameters.AddWithValue("@loccode",o.loccode);
                                cm.Parameters.AddWithValue("@huno",o.huno);
                                cm.Parameters.AddWithValue("@inrefno",o.inrefno);
                                cm.Parameters.AddWithValue("@inrefln",o.inrefln);
                                cm.Parameters.AddWithValue("@inagrn",o.inagrn);
                                cm.Parameters.AddWithValue("@ingrno",o.ingrno);
                                cm.Parameters.AddWithValue("@article",o.article);
                                cm.Parameters.AddWithValue("@pv",o.pv);
                                cm.Parameters.AddWithValue("@lv",o.lv);
                                cm.Parameters.AddWithValue("@descalt",o.descalt);
                                cm.Parameters.AddWithValue("@qtysku",o.qtysku);
                                cm.Parameters.AddWithValue("@qtypu",o.qtypu);
                                cm.Parameters.AddWithValue("@qtyweight",o.qtyweight);
                                cm.Parameters.AddWithValue("@qtyvolume",o.qtyvolume);
                                cm.Parameters.AddWithValue("@qtyunit",o.qtyunit);
                                cm.Parameters.AddWithValue("@qtyunitdes",o.qtyunitdes);
                                cm.Parameters.AddWithValue("@daterec",o.daterec);
                                cm.Parameters.AddWithValue("@batchno",o.batchno);
                                cm.Parameters.AddWithValue("@lotno",o.lotno);
                                cm.Parameters.AddWithValue("@datemfg",o.datemfg);
                                cm.Parameters.AddWithValue("@serialno",o.serialno);
                                cm.Parameters.AddWithValue("@dateexp",o.dateexp);
                                cm.Parameters.AddWithValue("@tflowops",o.tflowops);
                                cm.Parameters.AddWithValue("@tflowdes",o.tflowdes);
                                cm.Parameters.AddWithValue("@tflowsign",o.tflowsign);
                                cm.Parameters.AddWithValue("@skuops",o.skuops);
                                cm.Parameters.AddWithValue("@puops",o.puops);
                                cm.Parameters.AddWithValue("@weightops",o.weightops);
                                cm.Parameters.AddWithValue("@volumeops",o.volumeops);
                                cm.Parameters.AddWithValue("@unitops",o.unitops);
                                cm.Parameters.AddWithValue("@unitopsdes",o.unitopsdes);
                                cm.Parameters.AddWithValue("@refops",o.refops);
                                cm.Parameters.AddWithValue("@reflnops",o.reflnops);
                                cm.Parameters.AddWithValue("@remarks",o.remarks);
                                cm.Parameters.AddWithValue("@accncreate",o.accncreate);
                                cm.Parameters.AddWithValue("@accnmodify",o.accnmodify);
                                cm.Parameters.AddWithValue("@procmodify",o.procmodify);
                                cm.Parameters.AddWithValue("@msgops",o.msgops);
                                await cm.ExecuteNonQueryAsync();
                            }

                            // Header
                            cm.Parameters.Clear();
                            cm.CommandText = sqlmergehu_update;
                            cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                            cm.Parameters.AddWithValue("@site",os.site);
                            cm.Parameters.AddWithValue("@depot",os.depot);
                            cm.Parameters.AddWithValue("@spcarea",os.spcarea);
                            cm.Parameters.AddWithValue("@mergeno",os.mergeno);
                            cm.Parameters.AddWithValue("@accncode",os.accnmodify);
                            cm.Parameters.AddWithValue("@tflow","ED");
                            cm.Parameters.AddWithValue("@accnmodify",os.accnmodify);
                            cm.Parameters.AddWithValue("@remarks","Completed");
                            await cm.ExecuteNonQueryAsync();

                            cm.Parameters.Clear();
                            cm.CommandType = CommandType.StoredProcedure;
                            cm.CommandText = "[dbo].[snaps_mergehu_instock]";
                            cm.Parameters.AddWithValue("@orgcode",os.orgcode);
                            cm.Parameters.AddWithValue("@site",os.site);
                            cm.Parameters.AddWithValue("@depot",os.depot);
                            cm.Parameters.AddWithValue("@mergeno",os.mergeno);
                            cm.Parameters.AddWithValue("@accncode",os.accnmodify);
                            await cm.ExecuteNonQueryAsync();

                            tr.Commit();
                            return true;
                        } catch(Exception ex) {
                            tr.Rollback();
                            throw ex;
                        }
                    }
                }// end connection
            } catch(Exception ex) {
                logger.Error(os.orgcode,os.site,os.accnmodify,ex,ex.Message);
                throw ex;
            }
        }
        protected virtual void Dispose(bool disposing) {
            if(!disposedValue) {
                if(disposing) {
                    // TODO: dispose managed state (managed objects)
                    cn.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~mergehu_ops()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public static class DbHelper {
        public static object ValueOrNull(this object value) {
            return (value == DBNull.Value ? null : value);
        }
    }
}
