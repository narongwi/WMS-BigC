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
    public class reciln_ls {
        public String site { get; set; } 
        public String depot { get; set; } 
        public String recipecode { get; set; } 
        public String typeops { get; set; } 
        public String article { get; set; } 
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public String unitops { get; set; } 
        public Int32 qtypuops { get; set; } 
        public Int32 qtyskuops { get; set; } 
        public String articledsc { get; set; } 
    }
    public class reciln_pm : reciln_ls { 

    }
    public class reciln_ix : reciln_ls { 
        public Decimal qtyweightops { get; set; } 
        public Int32 expops { get; set; } 
        public Decimal lossprep { get; set; } 
        public Decimal lossops { get; set; } 
        public String tflow { get; set; } 
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
    }
    public class reciln_md : reciln_ls  {
        public Decimal qtyweightops { get; set; } 
        public Int32 expops { get; set; } 
        public Decimal lossprep { get; set; } 
        public Decimal lossops { get; set; } 
        public String tflow { get; set; } 
        public String mfglot { get; set; } 
        public String serailno { get; set; } 
        public DateTime? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTime? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
    }
    public class reciln_prc { 
        public String opsaccn { get; set; }
        public reciln_md opsobj { get; set; } 
    }
    public class reciln_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_reciln";
        private static string sqlmcom = "  and site = @site  and depot = @depot  and recipecode = @recipecode  and typeops = @typeops  and article = @article  and pv = @pv  and lv = @lv " ;
        private String sqlins = "insert into " + tbn + 
        " ( site, depot, recipecode, typeops, article, pv, lv, unitops, qtypuops, qtyskuops, qtyweightops, expops, lossprep, " +
        " lossops, tflow, mfglot, serailno, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
        " values "  +
        " ( @site, @depot, @recipecode, @typeops, @article, @pv, @lv, @unitops, @qtypuops, @qtyskuops, @qtyweightops, @expops, " +
        " @lossprep, @lossops, @tflow, @mfglot, @serailno, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " unitops = @unitops, qtypuops = @qtypuops, qtyskuops = @qtyskuops, qtyweightops = @qtyweightops, expops = @expops, " +
        " lossprep = @lossprep, lossops = @lossops, tflow = @tflow, mfglot = @mfglot, serailno = @serailno,  "+ 
        "  datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify, " + 
        " where 1=1 " + sqlmcom;
        private String sqlinx = "insert into ix" + tbn + 
        " ( site, depot, spcarea, recipetype, recipecode, recipename, version, tflow, fileid, rowops, ermsg, dateops ) " + 
        " values " + 
        " ( @site, @depot, @spcarea, @recipetype, @recipecode, @recipename, @version, @tflow, @fileid, @rowops, @ermsg, @dateops ) ";    
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 + ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public reciln_ops() {  }
        public reciln_ops(String cx) { cn = new SqlConnection(cx); }

        private reciln_ls fillls(ref DbDataReader r) { 
            return new reciln_ls() { 
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                recipecode = r["recipecode"].ToString(),
                typeops = r["typeops"].ToString(),
                article = r["article"].ToString(),
                pv = r.GetInt32(5),
                lv = r.GetInt32(6),
                unitops = r["unitops"].ToString(),
                qtypuops = r.GetInt32(8),
                qtyskuops = r.GetInt32(9),
                articledsc =  r["articledsc"].ToString(),
            };
        }
        private reciln_ix fillix(ref DbDataReader r) { 
            return new reciln_ix() { 
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                recipecode = r["recipecode"].ToString(),
                typeops = r["typeops"].ToString(),
                article = r["article"].ToString(),
                pv = r.GetInt32(5),
                lv = r.GetInt32(6),
                unitops = r["unitops"].ToString(),
                qtypuops = r.GetInt32(8),
                qtyskuops = r.GetInt32(9),
                qtyweightops = r.GetDecimal(10),
                expops = r.GetInt32(11),
                lossprep = r.GetDecimal(12),
                lossops = r.GetDecimal(13),
                tflow = r["tflow"].ToString(),
                fileid =  r["fileid"].ToString(),
                rowops =  r["rowops"].ToString(),
                ermsg =  r["ermsg"].ToString(),
                dateops =  (r.IsDBNull(18)) ? (DateTime?) null : r.GetDateTime(18),
            };
        }
        private reciln_md fillmdl(ref DbDataReader r) { 
            return new reciln_md() {
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                recipecode = r["recipecode"].ToString(),
                typeops = r["typeops"].ToString(),
                article = r["article"].ToString(),
                pv = r.GetInt32(5),
                lv = r.GetInt32(6),
                unitops = r["unitops"].ToString(),
                qtypuops = r.GetInt32(8),
                qtyskuops = r.GetInt32(9),
                qtyweightops = r.GetDecimal(10),
                expops = r.GetInt32(11),
                lossprep = r.GetDecimal(12),
                lossops = r.GetDecimal(13),
                tflow = r["tflow"].ToString(),
                mfglot = r["mfglot"].ToString(),
                serailno = r["serailno"].ToString(),
                datecreate = (r.IsDBNull(17)) ? (DateTime?) null : r.GetDateTime(17),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(19)) ? (DateTime?) null : r.GetDateTime(19),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
            };
        }
        private SqlCommand ixcommand(reciln_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.typeops.snapsPar());
            cm.Parameters.Add(o.article.snapsPar());
            cm.Parameters.Add(o.pv.snapsPar());
            cm.Parameters.Add(o.lv.snapsPar());
            cm.Parameters.Add(o.unitops.snapsPar());
            cm.Parameters.Add(o.qtypuops.snapsPar());
            cm.Parameters.Add(o.qtyskuops.snapsPar());
            cm.Parameters.Add(o.qtyweightops.snapsPar());
            cm.Parameters.Add(o.expops.snapsPar());
            cm.Parameters.Add(o.lossprep.snapsPar());
            cm.Parameters.Add(o.lossops.snapsPar());
            cm.Parameters.Add(o.tflow.snapsPar());
            cm.Parameters.Add(o.fileid.snapsPar());
            cm.Parameters.Add(o.rowops.snapsPar());
            cm.Parameters.Add(o.ermsg.snapsPar());
            cm.Parameters.Add(o.dateops.snapsPar());
            return cm;
        }
        private SqlCommand obcommand(reciln_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.typeops.snapsPar());
            cm.Parameters.Add(o.article.snapsPar());
            cm.Parameters.Add(o.pv.snapsPar());
            cm.Parameters.Add(o.lv.snapsPar());
            cm.Parameters.Add(o.unitops.snapsPar());
            cm.Parameters.Add(o.qtypuops.snapsPar());
            cm.Parameters.Add(o.qtyskuops.snapsPar());
            cm.Parameters.Add(o.qtyweightops.snapsPar());
            cm.Parameters.Add(o.expops.snapsPar());
            cm.Parameters.Add(o.lossprep.snapsPar());
            cm.Parameters.Add(o.lossops.snapsPar());
            cm.Parameters.Add(o.tflow.snapsPar());
            cm.Parameters.Add(o.mfglot.snapsPar());
            cm.Parameters.Add(o.serailno.snapsPar());
            cm.Parameters.Add(o.datecreate.snapsPar());
            cm.Parameters.Add(o.accncreate.snapsPar());
            cm.Parameters.Add(o.datemodify.snapsPar());
            cm.Parameters.Add(o.accnmodify.snapsPar());
            cm.Parameters.Add(o.procmodify.snapsPar());
            return cm;
        }

        public SqlCommand oucommand(reciln_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.typeops.snapsPar());
            cm.Parameters.Add(o.article.snapsPar());
            cm.Parameters.Add(o.pv.snapsPar());
            cm.Parameters.Add(o.lv.snapsPar());
            return cm;
        }
        public SqlCommand oucommand(reciln_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.typeops.snapsPar());
            cm.Parameters.Add(o.article.snapsPar());
            cm.Parameters.Add(o.pv.snapsPar());
            cm.Parameters.Add(o.lv.snapsPar());
            return cm;
        }

        public async Task<List<reciln_ls>> find(reciln_pm rs) { 
            SqlCommand cm = null;
            String sqlpam = "";
            List<reciln_ls> rn = new List<reciln_ls>();
            DbDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm.snapsCdn(rs.site);
                cm.snapsCdn(rs.depot);
                cm.snapsCdn(rs.recipecode);
                cm.snapsCdn(rs.typeops);
                cm.snapsCdn(rs.article);
                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<reciln_md> get(reciln_ls rs){ 
            SqlCommand cm = null; DbDataReader r = null;
            reciln_md rn = new reciln_md();
            String sqlpam = "";
            try { 
                /* Vlidate parameter */
                cm.snapsCdn(rs.site);
                cm.snapsCdn(rs.depot);
                cm.snapsCdn(rs.recipecode);
                cm.snapsCdn(rs.typeops);
                cm.snapsCdn(rs.article);
                cm.snapsCdn(rs.pv);
                cm.snapsCdn(rs.lv);
                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<reciln_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (reciln_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(reciln_md rs){ 
            List<reciln_md> ro = new List<reciln_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<reciln_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (reciln_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(reciln_md rs){
            List<reciln_md> ro = new List<reciln_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<reciln_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (reciln_ix ln in rs) {
                    cm.Add(ixcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                if (cn != null) { cn.Dispose(); } sqlval = null; sqlins = null; sqlupd = null; sqlrem = null; sqlfnd = null; tbn = null; sqlmcom = null;
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}
