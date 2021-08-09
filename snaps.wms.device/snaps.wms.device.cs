using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.WMS.Device;

namespace Snaps.WMS.Device {
    public class device_prc { 
        public String opsaccn { get; set; }
        public admdevice_md opsobj { get; set; } 
    }
    public class device_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_device";
        private static string sqlmcom = " and orgcode = @orgcode and site = @site  and depot = @depot and devtype = @devtype  and devid = @devid " ;
        private String sqlins = "insert into " + tbn + 
        " ( orgcode, site, depot, spcarea, devtype, devcode, devmodel, devserial, datelastactive, opsmaxheight, opsmaxweight, opsmaxvolume, isreceipt, istaskptw, " + 
        " istasktrf, istaskload, istaskrpn, istaskgen, ispick, isdistribute, isforward, iscount, tflow, devhash, datecreate, accncreate, datemodify, accnmodify, procmodify, devid ) " + 
        " values "  +
        " ( @orgcode, @site, @depot, @spcarea, @devtype, @devcode, @devmodel, @devserial, @datelastactive, @opsmaxheight, @opsmaxweight, @opsmaxvolume, @isreceipt, @istaskptw, " + 
        " @istasktrf, @istaskload, @istaskrpn, @istaskgen, @ispick, @isdistribute, @isforward, @iscount, @tflow, @devhash, @sysdate, @accncreate, @sysdate, @accnmodify, @procmodify, NEXT VALUE FOR seq_device ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " devmodel = @devmodel, devserial = @devserial, datelastactive = @datelastactive, opsmaxheight = @opsmaxheight, opsmaxweight = @opsmaxweight, opsmaxvolume = @opsmaxvolume, "+ 
        " isreceipt = @isreceipt, istaskptw = @istaskptw, istasktrf = @istasktrf, istaskload = @istaskload, istaskrpn = @istaskrpn, istaskgen = @istaskgen, ispick = @ispick, "+ 
        " isdistribute = @isdistribute, isforward = @isforward, iscount = @iscount, tflow = @tflow, devhash = @devhash,   datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify " + 
        " where 1=1 " + sqlmcom;        
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public device_ops() {  }
        public device_ops(String cx) { cn = new SqlConnection(cx); } 

        private admdevice_ls fillls(ref SqlDataReader r) { 
            return new admdevice_ls() { 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                devtype = r["devtype"].ToString(),
                devid = r.GetInt32(5),
                devcode = r["devcode"].ToString(),
                devmodel = r["devmodel"].ToString(),                
                datelastactive = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9),
                isreceipt = r.GetInt32(13),
                istaskptw = r.GetInt32(14),
                istasktrf = r.GetInt32(15),
                istaskload = r.GetInt32(16),
                istaskrpn = r.GetInt32(17),
                istaskgen = r.GetInt32(18),
                ispick = r.GetInt32(19),
                isdistribute = r.GetInt32(20),
                isforward = r.GetInt32(21),
                iscount = r.GetInt32(22),
                tflow = r["tflow"].ToString(),
                datemodify = (r.IsDBNull(27)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(27)                
            };
        }
        private admdevice_ix fillix(ref SqlDataReader r) { 
            return new admdevice_ix() { 

            };
        }
        private admdevice_md fillmdl(ref SqlDataReader r) { 
            admdevice_md rn = new admdevice_md();
            
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.devtype = r["devtype"].ToString();
            rn.devid = r.GetInt32(5);
            rn.devcode = r["devcode"].ToString();
            rn.devmodel = r["devmodel"].ToString();                
            rn.datelastactive = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.isreceipt = r.GetInt32(13);
            rn.istaskptw = r.GetInt32(14);
            rn.istasktrf = r.GetInt32(15);
            rn.istaskload = r.GetInt32(16);
            rn.istaskrpn = r.GetInt32(17);
            rn.istaskgen = r.GetInt32(18);
            rn.ispick = r.GetInt32(19);
            rn.isdistribute = r.GetInt32(20);
            rn.isforward = r.GetInt32(21);
            rn.iscount = r.GetInt32(22);
            rn.tflow = r["tflow"].ToString();
            rn.datemodify = (r.IsDBNull(27)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(27);
            rn.opsmaxheight = r.GetDecimal(10);
            rn.opsmaxweight = r.GetDecimal(11);
            rn.opsmaxvolume = r.GetDecimal(12);
            rn.devhash = r["devhash"].ToString();
            rn.datecreate = (r.IsDBNull(25)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(25);
            rn.accncreate = r["accncreate"].ToString();
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.devremarks = r["devremarks"].ToString();
            rn.devserial = r["devserial"].ToString();

            return rn;
        }
        private SqlCommand ixcommand(admdevice_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand obcommand(admdevice_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.devtype,"devtype");
            cm.snapsPar(o.devcode,"devcode");
            cm.snapsPar(o.devmodel,"devmodel");
            cm.snapsPar(o.devserial,"devserial");
            cm.snapsPar(o.datelastactive,"datelastactive");
            cm.snapsPar(o.opsmaxheight,"opsmaxheight");
            cm.snapsPar(o.opsmaxweight,"opsmaxweight");
            cm.snapsPar(o.opsmaxvolume,"opsmaxvolume");
            cm.snapsPar(o.isreceipt,"isreceipt");
            cm.snapsPar(o.istaskptw,"istaskptw");
            cm.snapsPar(o.istasktrf,"istasktrf");
            cm.snapsPar(o.istaskload,"istaskload");
            cm.snapsPar(o.istaskrpn,"istaskrpn");
            cm.snapsPar(o.istaskgen,"istaskgen");
            cm.snapsPar(o.ispick,"ispick");
            cm.snapsPar(o.isdistribute,"isdistribute");
            cm.snapsPar(o.isforward,"isforward");
            cm.snapsPar(o.iscount,"iscount");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.devhash,"devhash");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsParsysdateoffset();
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.devid,"devid");
            cm.snapsPar(o.devremarks,"devremarks");
            cm.snapsPar(o.procmodify,"procmodify");
            return cm;
        }

        public SqlCommand oucommand(admdevice_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.devtype,"devtype");
            cm.snapsPar(o.devcode,"devcode");
            cm.snapsPar(o.devid,"devid");
            return cm;
        }
        public SqlCommand oucommand(admdevice_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.devtype,"devtype");
            cm.snapsPar(o.devcode,"devcode");
            cm.snapsPar(o.devid,"devid");
            return cm;
        }

        public async Task<List<admdevice_ls>> find(admdevice_pm rs) { 
            SqlCommand cm = null;
            List<admdevice_ls> rn = new List<admdevice_ls>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm = (sqlfnd).snapsCommand(cn);
                cm.snapsCdn(rs.site,"site");
                cm.snapsCdn(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.devtype,"devtype");
                cm.snapsCdn(rs.devcode,"devcode");
                if (rs.devid != 0) { cm.snapsPar(rs.devid,"devid"); }
                cm.snapsCdn(rs.tflow,"tflow");
                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<admdevice_md> get(admdevice_ls rs){ 
            SqlCommand cm = null; SqlDataReader r = null;
            admdevice_md rn = new admdevice_md();
            String sqlpam = "";
            try { 
                /* Vlidate parameter */
                cm = (sqlfnd).snapsCommand(cn);
                cm.snapsCdn(rs.site,"site");
                cm.snapsCdn(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.devtype,"devtype");
                cm.snapsCdn(rs.devcode,"devcode");
                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<admdevice_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (admdevice_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval)); 
                    if (ln.devid == 0){ 
                        cm[ix].CommandText = sqlins; 
                    }else { 
                        cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                    }
                    
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) {  
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(admdevice_md rs){ 
            List<admdevice_md> ro = new List<admdevice_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<admdevice_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (admdevice_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(admdevice_md rs){
            List<admdevice_md> ro = new List<admdevice_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<admdevice_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (admdevice_ix ln in rs) {
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
