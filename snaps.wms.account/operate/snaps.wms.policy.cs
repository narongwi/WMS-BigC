// using System;
// using System.Linq;
// using System.Data.Common;
// using System.Data.SqlClient;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Snaps.Helpers;
// using Snaps.Helpers.StringExt;
// using Snaps.Helpers.DbContext.SQLServer;

// namespace Snaps.WMS {
//     public class policy_ls {
//         public String orgcode { get; set; } 
//         public String apcode { get; set; } 
//         public String plccode { get; set; } 
//         public String plcname { get; set; } 
//         public String tflow { get; set; } 
//     }
//     public class policy_pm : policy_ls { 
//         public Int32 reqnumeric { get; set; } 
//         public Int32 requppercase { get; set; } 
//         public Int32 reqlowercase { get; set; } 
//         public Int32 reqspecialchar { get; set; } 
//         public String spcchar { get; set; } 
//         public Int32 minlength { get; set; } 
//         public Int32 maxauthfail { get; set; } 
//         public Int32 exppdamobile { get; set; } 
//         public Int32 expandriod { get; set; } 
//         public Int32 expios { get; set; } 
//         public String seckey { get; set; } 
//         public Int32 dayexpire { get; set; } 
//     }
//     public class policy_ix : policy_ls { 
//     }
//     public class policy_md : policy_ls  {
//         public Int32 reqnumeric { get; set; } 
//         public Int32 requppercase { get; set; } 
//         public Int32 reqlowercase { get; set; } 
//         public Int32 reqspecialchar { get; set; } 
//         public String spcchar { get; set; } 
//         public Int32 minlength { get; set; } 
//         public Int32 maxauthfail { get; set; } 
//         public Int32 exppdamobile { get; set; } 
//         public Int32 expandriod { get; set; } 
//         public Int32 expios { get; set; } 
//         public String seckey { get; set; } 
//         public Int32 dayexpire { get; set; } 
//         public String hashplc { get; set; } 
//         public DateTimeOffset? datecreate { get; set; } 
//         public String accncreate { get; set; } 
//         public DateTimeOffset? datemodify { get; set; }
//         public String accnmodify { get; set; } 
//         public String procmodify { get; set; } 
//     }
//     public class policy_prc { 
//         public String opsaccn { get; set; }
//         public policy_md opsobj { get; set; } 
//     }
//     public class policy_ops : IDisposable { 
//         private SqlConnection cn = null;
//         private static string tbn = "wm_policy";
//         private static string sqlmcom = "  and orgcode = @orgcode  and apcode = @apcode  and plccode = @plccode " ;
//         private String sqlins = "insert into " + tbn + 
//         " ( orgcode, apcode, plccode, plcname, tflow, reqnumeric, requppercase, reqlowercase, reqspecialchar, spcchar, minlength, maxauthfail, " + 
//         " exppdamobile, expandriod, expios, seckey, dayexpire, hashplc, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
//         " values "  +
//         " ( @orgcode, @apcode, @plccode, @plcname, @tflow, @reqnumeric, @requppercase, @reqlowercase, @reqspecialchar, @spcchar, @minlength, "+ 
//         " @maxauthfail, @exppdamobile, @expandriod, @expios, @seckey, @dayexpire, @hashplc, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
//         private String sqlupd = " update " + tbn + " set " + 
//         " plcname = @plcname, tflow = @tflow, reqnumeric = @reqnumeric, requppercase = @requppercase, reqlowercase = @reqlowercase, reqspecialchar = @reqspecialchar, " +
//         " spcchar = @spcchar, minlength = @minlength, maxauthfail = @maxauthfail, exppdamobile = @exppdamobile, expandriod = @expandriod, expios = @expios, seckey = @seckey, " +
//         " dayexpire = @dayexpire, hashplc = @hashplc,   datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " + 
//         " where 1=1 " + sqlmcom;        
//         private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
//         private String sqlfnd = "select * from " + tbn + " where 1=1 + ";
//         private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
//         public policy_ops() {  }
//         public policy_ops(String cx) { cn = new SqlConnection(cx); }

//         private policy_ls fillls(ref SqlDataReader r) { 
//             return new policy_ls() { 
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 plccode = r["plccode"].ToString(),
//                 plcname = r["plcname"].ToString(),
//                 tflow = r["tflow"].ToString(),
//             };
//         }
//         private policy_ix fillix(ref SqlDataReader r) { 
//             return new policy_ix() { 

//             };
//         }
//         private policy_md fillmdl(ref SqlDataReader r) { 
//             return new policy_md() {
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 plccode = r["plccode"].ToString(),
//                 plcname = r["plcname"].ToString(),
//                 tflow = r["tflow"].ToString(),
//                 reqnumeric = r.GetInt32(5),
//                 requppercase = r.GetInt32(6),
//                 reqlowercase = r.GetInt32(7),
//                 reqspecialchar = r.GetInt32(8),
//                 spcchar = r["spcchar"].ToString(),
//                 minlength = r.GetInt32(10),
//                 maxauthfail = r.GetInt32(11),
//                 exppdamobile = r.GetInt32(12),
//                 expandriod = r.GetInt32(13),
//                 expios = r.GetInt32(14),
//                 seckey = r["seckey"].ToString(),
//                 dayexpire = r.GetInt32(16),
//                 hashplc = r["hashplc"].ToString(),
//                 datecreate = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTime(18),
//                 accncreate = r["accncreate"].ToString(),
//                 datemodify = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTime(20),
//                 accnmodify = r["accnmodify"].ToString(),
//                 procmodify = r["procmodify"].ToString(),
//             };
//         }
//         private SqlCommand ixcommand(policy_ix o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);

//             return cm;
//         }
//         private SqlCommand obcommand(policy_md o,String sql){ 
//             SqlCommand cm = new SqlCommand(sql,cn);
//                 cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//                 cm.Parameters.Add(o.apcode.snapsPar("apcode"));
//                 cm.Parameters.Add(o.plccode.snapsPar("plccode"));
//                 cm.Parameters.Add(o.plcname.snapsPar("plcname"));
//                 cm.Parameters.Add(o.tflow.snapsPar("tflow"));
//                 cm.Parameters.Add(o.reqnumeric.snapsPar("reqnumeric"));
//                 cm.Parameters.Add(o.requppercase.snapsPar("requppercase"));
//                 cm.Parameters.Add(o.reqlowercase.snapsPar("reqlowercase"));
//                 cm.Parameters.Add(o.reqspecialchar.snapsPar("reqspecialchar"));
//                 cm.Parameters.Add(o.spcchar.snapsPar("spcchar"));
//                 cm.Parameters.Add(o.minlength.snapsPar("minlength"));
//                 cm.Parameters.Add(o.maxauthfail.snapsPar("maxauthfail"));
//                 cm.Parameters.Add(o.exppdamobile.snapsPar("exppdamobile"));
//                 cm.Parameters.Add(o.expandriod.snapsPar("expandriod"));
//                 cm.Parameters.Add(o.expios.snapsPar("expios"));
//                 cm.Parameters.Add(o.seckey.snapsPar("seckey"));
//             return cm;
//         }

//         public SqlCommand oucommand(policy_ls o,String sql,String accnmodify) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//             cm.Parameters.Add(o.apcode.snapsPar("apcode"));
//             cm.Parameters.Add(o.plccode.snapsPar("plccode"));
//             return cm;
//         }
//         public SqlCommand oucommand(policy_md o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//             cm.Parameters.Add(o.apcode.snapsPar("apcode"));
//             cm.Parameters.Add(o.plccode.snapsPar("plccode"));
//             return cm;
//         }

//         public async Task<List<policy_ls>> find(policy_pm rs) { 
//             SqlCommand cm = new SqlCommand("",cn);
//             List<policy_ls> rn = new List<policy_ls>();
//             SqlDataReader r = null;
//             try { 
//                  /* Vlidate parameter */
//                 cm.snapsCdn(rs.orgcode,"orgcode");
//                 cm.snapsCdn(rs.apcode,"apcode");
//                 cm.snapsCdn(rs.plccode,"plccode");
//                 cm.snapsCdn(rs.plcname,"plcname");
//                 cm.snapsCdn(rs.tflow,"tflow");
//                 cm.snapsCdn(rs.reqnumeric,"reqnumeric");
//                 cm.snapsCdn(rs.requppercase,"requppercase");
//                 cm.snapsCdn(rs.reqlowercase,"reqlowercase");
//                 cm.snapsCdn(rs.reqspecialchar,"reqspecialchar");
//                 cm.snapsCdn(rs.spcchar,"spcchar");
//                 cm.snapsCdn(rs.minlength,"minlength");
//                 cm.snapsCdn(rs.maxauthfail,"maxauthfail");
//                 cm.snapsCdn(rs.exppdamobile,"exppdamobile");
//                 cm.snapsCdn(rs.expandriod,"expandriod");
//                 cm.snapsCdn(rs.expios,"expios");
//                 cm.snapsCdn(rs.seckey,"seckey");
//                 cm.snapsCdn(rs.dayexpire,"dayexpire");
//                 cm = (sqlfnd).snapsCommand(cn);
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn;
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task<policy_md> get(policy_ls rs){ 
//             SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
//             policy_md rn = new policy_md();
//             try { 
//                 /* Vlidate parameter */
//                 cm.snapsCdn(rs.orgcode,"orgcode");
//                 cm.snapsCdn(rs.apcode,"apcode");
//                 cm.snapsCdn(rs.plccode,"plccode");
//                 cm = (sqlfnd).snapsCommand(cn);
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn = fillmdl(ref r); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn; 
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task upsert(List<policy_md> rs){ 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (policy_md ln in rs) {
//                     cm.Add(obcommand(ln,sqlval)); 
//                     cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//                 }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task upsert(policy_md rs){ 
//             List<policy_md> ro = new List<policy_md>(); 
//             try { 
//                 ro.Add(rs); await upsert(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task remove(List<policy_md> rs){
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             try { 
//                 foreach (policy_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task remove(policy_md rs){
//             List<policy_md> ro = new List<policy_md>(); 
//             try { 
//                 ro.Add(rs); await remove(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task import(List<policy_ix> rs) { 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (policy_ix ln in rs) {
//                     cm.Add(ixcommand(ln,sqlval)); 
//                     cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//                 }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         private bool disposedValue = false;
//         protected virtual void Dispose(bool Disposing){ 
//             if(!disposedValue) { 
//                 if (cn != null) { cn.Dispose(); } sqlval = null; sqlins = null; sqlupd = null; sqlrem = null; sqlfnd = null; tbn = null; sqlmcom = null;
//             }
//             disposedValue = true;
//         }
//         public void Dispose() {
//             GC.SuppressFinalize(this);
//         }

//     }

// }
