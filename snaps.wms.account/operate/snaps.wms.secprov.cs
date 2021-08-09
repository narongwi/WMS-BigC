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
    public class acpv_ls {
        public String orgcode { get; set; } 
        public String apcode { get; set; } 
        public String accncode { get; set; } 
        public String tflow { get; set; } 
        public DateTime? datexpire { get; set; } 
        public DateTime? datemodify { get; set; } 
        public String accnpriv { get; set; } 
    }
    public class acpv_pm : acpv_ls { 

    }
    public class acpv_ix : acpv_ls { 
    }
    public class acpv_md : acpv_ls  {
        public String devicecode { get; set; } 
        public String hashpriv { get; set; } 
        public DateTime? datecreate { get; set; } 
        public String accncreate { get; set; } 

        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
    }
    public class acpv_prc { 
        public String opsaccn { get; set; }
        public acpv_md opsobj { get; set; } 
    }
    public class acpv_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_accnprv";
        private static string sqlmcom = "  and orgcode = @orgcode  and apcode = @apcode  and accncode = @accncode  and accnpriv = @accnpriv  " ;
        private String sqlins = "insert into " + tbn + 
        " ( orgcode, apcode, accncode, accnpriv, devicecode, tflow, hashpriv, datexpire, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
        " values "  +
        " ( @orgcode, @apcode, @accncode, @accnpriv, @devicecode, @tflow, @hashpriv, @datexpire, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " devicecode = @devicecode, tflow = @tflow, hashpriv = @hashpriv, datexpire = @datexpire,   "+ 
        " datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " + 
        " where 1=1 " + sqlmcom;        
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 + ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public acpv_ops() {  }
        public acpv_ops(String cx) { cn = new SqlConnection(cx); }

        private acpv_ls fillls(ref SqlDataReader r) { 
            return new acpv_ls() { 
                orgcode = r["orgcode"].ToString(),
                apcode = r["apcode"].ToString(),
                accncode = r["accncode"].ToString(),
                tflow = r["tflow"].ToString(),
                datexpire = (r.IsDBNull(4)) ? (DateTime?) null : r.GetDateTime(4),
                datemodify = (r.IsDBNull(5)) ? (DateTime?) null : r.GetDateTime(5),
            };
        }
        private acpv_ix fillix(ref SqlDataReader r) { 
            return new acpv_ix() { 

            };
        }
        private acpv_md fillmdl(ref SqlDataReader r) { 
            return new acpv_md() {
                orgcode = r["orgcode"].ToString(),
                apcode = r["apcode"].ToString(),
                accncode = r["accncode"].ToString(),
                accnpriv = r["accnpriv"].ToString(),
                devicecode = r["devicecode"].ToString(),
                tflow = r["tflow"].ToString(),
                hashpriv = r["hashpriv"].ToString(),
                datexpire = (r.IsDBNull(7)) ? (DateTime?) null : r.GetDateTime(7),
                datecreate = (r.IsDBNull(8)) ? (DateTime?) null : r.GetDateTime(8),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(10)) ? (DateTime?) null : r.GetDateTime(10),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
            };
        }
        private SqlCommand ixcommand(acpv_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand obcommand(acpv_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
            cm.Parameters.Add(o.apcode.snapsPar("apcode"));
            cm.Parameters.Add(o.accncode.snapsPar("accncode"));
            cm.Parameters.Add(o.accnpriv.snapsPar("accnpriv"));
            cm.Parameters.Add(o.devicecode.snapsPar("devicecode"));
            cm.Parameters.Add(o.tflow.snapsPar("tflow"));
            cm.Parameters.Add(o.hashpriv.snapsPar("hashpriv"));
            cm.Parameters.Add(o.datexpire.snapsPar("datexpire"));
            cm.Parameters.Add(o.datecreate.snapsPar("datecreate"));
            cm.Parameters.Add(o.accncreate.snapsPar("accncreate"));
            cm.Parameters.Add(o.datemodify.snapsPar("datemodify"));
            cm.Parameters.Add(o.accnmodify.snapsPar("accnmodify"));
            cm.Parameters.Add(o.procmodify.snapsPar("procmodify"));
            return cm;
        }

        public SqlCommand oucommand(acpv_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
            cm.Parameters.Add(o.apcode.snapsPar("apcode"));
            cm.Parameters.Add(o.accncode.snapsPar("accncode"));
            cm.Parameters.Add(o.accnpriv.snapsPar("accnpriv"));
            return cm;
        }
        public SqlCommand oucommand(acpv_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
            cm.Parameters.Add(o.apcode.snapsPar("apcode"));
            cm.Parameters.Add(o.accncode.snapsPar("accncode"));
            cm.Parameters.Add(o.accnpriv.snapsPar("accnpriv"));
            return cm;
        }

        public async Task<List<acpv_ls>> find(acpv_pm rs) { 
            SqlCommand cm = new SqlCommand("",cn);
            List<acpv_ls> rn = new List<acpv_ls>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm.snapsCdn(rs.orgcode,"orgcode");
                cm.snapsCdn(rs.apcode,"apcode");
                cm.snapsCdn(rs.accncode,"accncode");
                cm.snapsCdn(rs.accnpriv,"accnpriv");
                cm.snapsCdn(rs.tflow,"tflow");
                cm.snapsCdn(rs.datexpire,"datexpire");
                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<acpv_md> get(acpv_ls rs){ 
            SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
            acpv_md rn = new acpv_md();
            try { 
                /* Vlidate parameter */
               cm.snapsCdn(rs.orgcode,"orgcode");
                cm.snapsCdn(rs.apcode,"apcode");
                cm.snapsCdn(rs.accncode,"accncode");
                cm.snapsCdn(rs.accnpriv,"accnpriv");
                cm.snapsCdn(rs.tflow,"tflow");
                cm.snapsCdn(rs.datexpire,"datexpire");
                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();}  if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<acpv_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (acpv_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(acpv_md rs){ 
            List<acpv_md> ro = new List<acpv_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<acpv_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (acpv_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(acpv_md rs){
            List<acpv_md> ro = new List<acpv_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<acpv_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (acpv_ix ln in rs) {
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
