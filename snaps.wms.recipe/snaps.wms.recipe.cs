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
    public class barcode_ls {
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String recipetype { get; set; } 
        public String recipecode { get; set; } 
        public String recipename { get; set; } 
        public String version { get; set; } 
        public String tflow { get; set; } 
    }
    public class barcode_pm : barcode_ls { 

    }
    public class barcode_ix : barcode_ls { 
        public String fileid { get; set; } 
        public String rowops { get; set; } 
        public String ermsg { get; set; } 
        public DateTimeOffset? dateops { get; set; } 
    }
    public class barcode_md : barcode_ls  {
        public Decimal recipeqtyresult { get; set; } 
        public Decimal recipeqtyrawm { get; set; } 
        public Int32 reciperawmat { get; set; } 
        public String reciperemarks { get; set; } 

        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
    }
    public class barcode_prc { 
        public String opsaccn { get; set; }
        public barcode_md opsobj { get; set; } 
    }
    public class barcode_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_recipe";
        private static string sqlmcom = " site = @site and depot = @depot and spcarea = @spcarea and recipecode = @recipecode " ;
        private String sqlins = "insert into " + tbn + 
        " ( site, depot, spcarea, recipetype, recipecode, recipename, version, recipeqtyresult, recipeqtyrawm, reciperawmat, "+ 
         "  reciperemarks, tflow, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
        " values "  +
        " ( @site, @depot, @spcarea, @recipetype, @recipecode, @recipename, @version, @recipeqtyresult, @recipeqtyrawm, "+ 
         "  @reciperawmat, @reciperemarks, @tflow, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " recipetype = @recipetype,  recipename = @recipename,  recipeqtyresult = @recipeqtyresult, recipeqtyrawm = @recipeqtyrawm, " + 
        " reciperawmat = @reciperawmat, reciperemarks = @reciperemarks, tflow = @tflow,   " + 
        " datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " + 
        " where 1=1 " + sqlmcom; 
        private String sqlinx = "insert into ix" + tbn + 
        " ( site, depot, spcarea, recipetype, recipecode, recipename, version, tflow, fileid, rowops, ermsg, dateops ) " + 
        " values " + 
        " ( @site, @depot, @spcarea, @recipetype, @recipecode, @recipename, @version, @tflow, @fileid, @rowops, @ermsg, @dateops ) ";
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 + ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public barcode_ops() {  }
        public barcode_ops(String cx) { cn = new SqlConnection(cx); }

        private barcode_ls fillls(ref DbDataReader r) { 
            return new barcode_ls() { 
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                recipetype = r["recipetype"].ToString(),
                recipecode = r["recipecode"].ToString(),
                recipename = r["recipename"].ToString(),
                version = r["version"].ToString(),
                tflow = r["tflow"].ToString(),
            };
        }
        private barcode_ix fillix(ref DbDataReader r) { 
            return new barcode_ix() { 
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                recipetype = r["recipetype"].ToString(),
                recipecode = r["recipecode"].ToString(),
                recipename = r["recipename"].ToString(),
                version = r["version"].ToString(),
                tflow = r["tflow"].ToString(),
                fileid =  r["tflow"].ToString(),
                rowops =  r["tflow"].ToString(),
                ermsg =  r["tflow"].ToString(),
                dateops = (r.IsDBNull(11)) ? (DateTime?) null : r.GetDateTime(39),
            };
        }
        private barcode_md fillmdl(ref DbDataReader r) { 
            return new barcode_md() {
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                recipetype = r["recipetype"].ToString(),
                recipecode = r["recipecode"].ToString(),
                recipename = r["recipename"].ToString(),
                version = r["version"].ToString(),
                recipeqtyresult = r.GetDecimal(7),
                recipeqtyrawm = r.GetDecimal(8),
                reciperawmat = r.GetInt32(9),
                reciperemarks = r["reciperemarks"].ToString(),
                tflow = r["tflow"].ToString(),
                datecreate = (r.IsDBNull(12)) ? (DateTime?) null : r.GetDateTime(12),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(14)) ? (DateTime?) null : r.GetDateTime(14),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
            };
        }
        private SqlCommand ixcommand(barcode_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.spcarea.snapsPar());
            cm.Parameters.Add(o.recipetype.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.recipename.snapsPar());
            cm.Parameters.Add(o.version.snapsPar());
            cm.Parameters.Add(o.tflow.snapsPar());
            cm.Parameters.Add(o.fileid.snapsPar());
            cm.Parameters.Add(o.rowops.snapsPar());
            cm.Parameters.Add(o.ermsg.snapsPar());
            cm.Parameters.Add(o.dateops.snapsPar());
            return cm;
        }
        private SqlCommand obcommand(barcode_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.spcarea.snapsPar());
            cm.Parameters.Add(o.recipetype.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.recipename.snapsPar());
            cm.Parameters.Add(o.version.snapsPar());
            cm.Parameters.Add(o.recipeqtyresult.snapsPar());
            cm.Parameters.Add(o.recipeqtyrawm.snapsPar());
            cm.Parameters.Add(o.reciperawmat.snapsPar());
            cm.Parameters.Add(o.reciperemarks.snapsPar());
            cm.Parameters.Add(o.tflow.snapsPar());
            cm.Parameters.Add(o.datecreate.snapsPar());
            cm.Parameters.Add(o.accncreate.snapsPar());
            cm.Parameters.Add(o.datemodify.snapsPar());
            cm.Parameters.Add(o.accnmodify.snapsPar());
            cm.Parameters.Add(o.procmodify.snapsPar());
            return cm;
        }

        public SqlCommand oucommand(barcode_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.spcarea.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.version.snapsPar());
            return cm;
        }
        public SqlCommand oucommand(barcode_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.spcarea.snapsPar());
            cm.Parameters.Add(o.recipecode.snapsPar());
            cm.Parameters.Add(o.version.snapsPar());
            return cm;
        }

        public async Task<List<barcode_ls>> find(barcode_pm rs) { 
            SqlCommand cm = null;
            String sqlpam = "";
            List<barcode_ls> rn = new List<barcode_ls>();
            DbDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm.snapsCdn(rs.site);
                cm.snapsCdn(rs.depot);
                cm.snapsCdn(rs.spcarea);
                cm.snapsCdn(rs.recipetype);
                cm.snapsCdn(rs.recipecode);
                cm.snapsCdn(rs.recipename);
                cm.snapsCdn(rs.version);
                cm.snapsCdn(rs.tflow);

                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<barcode_md> get(barcode_ls rs){ 
            SqlCommand cm = null; DbDataReader r = null;
            barcode_md rn = new barcode_md();
            String sqlpam = "";
            try { 
                /* Vlidate parameter */
                cm.snapsCdn(rs.site);
                cm.snapsCdn(rs.depot);
                cm.snapsCdn(rs.spcarea);
                cm.snapsCdn(rs.recipecode);
                cm.snapsCdn(rs.version);

                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<barcode_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (barcode_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(barcode_md rs){ 
            List<barcode_md> ro = new List<barcode_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<barcode_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (barcode_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(barcode_md rs){
            List<barcode_md> ro = new List<barcode_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<barcode_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (barcode_ix ln in rs) {
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
