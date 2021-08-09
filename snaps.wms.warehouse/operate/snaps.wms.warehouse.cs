using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.warehouse {

    public partial class warehouse_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_warehouse";
        private static string sqlmcom = " and orgcode = @orgcode  and sitecode = @sitecode " ;
        private String sqlins = "insert into " + tbn + 
        " ( orgcode, sitecode, sitename, sitenamealt, datestart, dateend, sitekey, tflow, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
        " values "  +
        " ( @orgcode, @sitecode, @sitename, @sitenamealt, @datestart, @dateend, @sitekey, @tflow, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify )";
        private String sqlupd = " update " + tbn + 
        " set   sitename = @sitename, sitenamealt = @sitenamealt, datestart = @datestart, dateend = @dateend, sitekey = @sitekey, tflow = @tflow, " + 
        "   datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " +
        " where 1=1 " + sqlmcom;
        private String sqlinx = "insert into ix" + tbn + 
        " ( ) values "  +
        " ( )";

        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 and orgcode = @orgcode "; 
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public warehouse_ops() {  }
        public warehouse_ops(String cx) { cn = new SqlConnection(cx); }

        private warehouse_ls fillls(ref SqlDataReader r) { 
            return new warehouse_ls() { 
                orgcode = r["orgcode"].ToString(),
                sitecode = r["sitecode"].ToString(),
                sitename = r["sitename"].ToString(),
                datestart = (r.IsDBNull(4)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(4),
                dateend = (r.IsDBNull(5)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(5),
                tflow = r["tflow"].ToString(),
                sitetype = r["sitetype"].ToString()
            };
        }
        private warehouse_ix fillix(ref SqlDataReader r) { 
            return new warehouse_ix() { 

            };
        }
        private warehouse_md fillmdl(ref SqlDataReader r) { 
            return new warehouse_md() {
                orgcode = r["orgcode"].ToString(),
                sitecode = r["sitecode"].ToString(),
                sitename = r["sitename"].ToString(),
                sitenamealt = r["sitenamealt"].ToString(),
                datestart = (r.IsDBNull(4)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(4),
                dateend = (r.IsDBNull(5)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(5),
                sitekey = r["sitekey"].ToString(),
                tflow = r["tflow"].ToString(),
                datecreate = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
                sitetype = r["sitetype"].ToString()
            };
        }
        private SqlCommand ixcommand(warehouse_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            return cm;
        }
        private SqlCommand obcommand(warehouse_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
            cm.Parameters.Add(o.sitecode.snapsPar(nameof(o.sitecode)));
            cm.Parameters.Add(o.sitename.snapsPar(nameof(o.sitename)));
            cm.Parameters.Add(o.sitenamealt.snapsPar(nameof(o.sitenamealt)));
            cm.Parameters.Add(o.datestart.snapsPar(nameof(o.datestart)));
            if(o.dateend == null){ 
                cm.Parameters.Add("dateend",sqlDbType:System.Data.SqlDbType.DateTimeOffset).Value = DBNull.Value;
                cm.Parameters["dateend"].IsNullable = true;
            }else { 
                cm.Parameters.Add(o.dateend.snapsPar(nameof(o.dateend)));
            }            
            cm.Parameters.Add(o.sitekey.snapsPar(nameof(o.sitekey)));
            cm.Parameters.Add(o.tflow.snapsPar(nameof(o.tflow)));
            cm.Parameters.Add(o.datecreate.snapsPar(nameof(o.datecreate)));
            cm.Parameters.Add(o.accncreate.snapsPar(nameof(o.accncreate)));
            cm.Parameters.Add(o.datemodify.snapsPar(nameof(o.datemodify)));
            cm.Parameters.Add(o.accnmodify.snapsPar(nameof(o.accnmodify)));
            cm.Parameters.Add(o.procmodify.snapsPar(nameof(o.procmodify)));
            cm.snapsPar(o.sitetype,"sitetype");
            return cm;
        }

        public SqlCommand oucommand(warehouse_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
            cm.Parameters.Add(o.sitecode.snapsPar(nameof(o.sitecode)));
            return cm;
        }
        public SqlCommand oucommand(warehouse_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
            cm.Parameters.Add(o.sitecode.snapsPar(nameof(o.sitecode)));
            return cm;
        }

        public async Task<List<warehouse_md>> find(warehouse_pm rs) { 
            SqlCommand cm = new SqlCommand("",cn);
            List<warehouse_md> rn = new List<warehouse_md>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsCdn(rs.orgcode,nameof(rs.orgcode));
                cm.snapsCdn(rs.sitecode,nameof(rs.sitecode));
                cm.snapsCdn(rs.sitename,nameof(rs.sitename));
                cm.snapsCdn(rs.sitenamealt,nameof(rs.sitenamealt));
                cm.snapsCdn(rs.datestart,"datestart");               
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillmdl(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<warehouse_md> get(warehouse_ls rs){ 
            SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
            warehouse_md rn = new warehouse_md();
            try { 
                /* Vlidate parameter */
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsCdn(rs.orgcode,nameof(rs.orgcode));
                cm.snapsCdn(rs.sitecode,nameof(rs.sitecode));                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<warehouse_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (warehouse_md ln in rs) {
                    ln.tflow = (ln.tflow == "NW") ? "IO" : ln.tflow;
                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(warehouse_md rs){ 
            List<warehouse_md> ro = new List<warehouse_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<warehouse_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (warehouse_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(warehouse_md rs){
            List<warehouse_md> ro = new List<warehouse_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<warehouse_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (warehouse_ix ln in rs) {
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



        public String getMessage(String Lang,String Ercode){ 
            SqlCommand cm = new SqlCommand(string.Format("select ISNULL((select descmsg from wm_message where apps = 'WMS' and typemsg = 'ER'" + 
            " and langmsg = '{0}' and codemsg = '{1}'),'{1}')",Lang,Ercode),cn);
            try { 
                return cm.snapsScalarStrAsync().Result;
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose();}
        }

    }

}
