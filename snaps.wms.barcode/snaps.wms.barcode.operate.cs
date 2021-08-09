using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.Hash;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS.Barcode {

    public partial class barcode_ops : IDisposable { 
        private SqlConnection cn = null;

        public barcode_ops() {  }
        public barcode_ops(String cx) { cn = new SqlConnection(cx); }

       

        public async Task<List<barcode_ls>> find(barcode_pm rs) { 
            SqlCommand cm = null;
            List<barcode_ls> rn = new List<barcode_ls>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm = (sqlbarcode_find).snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn(rs.article,"article"," and b.article = @article");
                cm.snapsCdn(rs.barcode,"thcode"," and t.thcode = @thcode");
                cm.snapsCdn(rs.searchall,"searchall",string.Format(" and barcode like '%{0}%' ", rs.searchall.ClearReg()));                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<barcode_md> get(barcode_ls rs){ 
            SqlCommand cm = null; SqlDataReader r = null;
            barcode_md rn = new barcode_md();
            try { 
                /* Vlidate parameter */
                cm = (sqlfnd).snapsCommand(cn);
                cm.snapsCdn(rs.site,"site");
                cm.snapsCdn(rs.depot,"depot");
                cm.snapsCdn(rs.barcode,"barcode");                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
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

        public async Task setPrimary(barcode_ls o,string accncode) { 
            List<SqlCommand> cm = new List<SqlCommand>();
            try { 
                cm.Add(sqlbarcode_clearprimary.snapsCommand());
                cm.Add(sqlbarcode_setprimary.snapsCommand());
                foreach(SqlCommand ln in cm){ 
                    ln.snapsPar(o.orgcode,"orgcode");
                    ln.snapsPar(o.site,"site");
                    ln.snapsPar(o.depot,"depot");
                    ln.snapsPar(o.article,"article");
                    ln.snapsPar(o.pv,"pv");
                    ln.snapsPar(o.lv,"lv");
                    ln.snapsPar(o.barcode,"barcode");
                    ln.snapsPar(accncode,"accnmodify");
                    ln.snapsPar("api.barcode.setprimary","procmodify");
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex){ 
                throw ex;
            }finally { cm.ForEach(x=>x.Dispose()); }
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
