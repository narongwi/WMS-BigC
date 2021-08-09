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

//     public class roln_prc { 

//     }
//     public class roln_ops : IDisposable { 
//         private SqlConnection cn = null;
//         private static string tbn = "wm_roln";
//         private static string sqlmcom = " and orgcode = @orgcode  and apcode = @apcode  and rolcode = @rolcode  and objmodule = @objmodule  and objtype = @objtype  and objcode = @objcode " ;
//         private String sqlins = "insert into " + tbn + 
//         " ( orgcode, apcode, rolcode, objmodule, objtype, objcode, isenable, isexecute, hashrln, datecreate, accncreate, datemodify, accnmodify, procmodify  ) " + 
//         " values "  +
//         " ( @orgcode, @apcode, @rolcode, @objmodule, @objtype, @objcode, @isenable, @isexecute, @hashrln, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
//         private String sqlupd = " update " + tbn + " set " + 
//         " isenable = @isenable, isexecute = @isexecute, hashrln = @hashrln,   datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " + 
//         " where 1=1 " + sqlmcom;        
//         private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
//         private String sqlfnd = "select * from " + tbn + " where 1=1 + ";
//         private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
//         public roln_ops() {  }
//         public roln_ops(String cx) { cn = new SqlConnection(cx); }

//         private roln_ls fillls(ref SqlDataReader r) { 
//             return new roln_ls() { 
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 rolcode = r["rolcode"].ToString(),
//                 objmodule = r["objmodule"].ToString(),
//                 objtype = r["objtype"].ToString(),
//                 objcode = r["objcode"].ToString(),
//             };
//         }
//         private roln_ix fillix(ref SqlDataReader r) { 
//             return new roln_ix() { 

//             };
//         }
//         private roln_md fillmdl(ref SqlDataReader r) { 
//             return new roln_md() {
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 rolcode = r["rolcode"].ToString(),
//                 objmodule = r["objmodule"].ToString(),
//                 objtype = r["objtype"].ToString(),
//                 objcode = r["objcode"].ToString(),
//                 isenable = r.GetInt32(6),
//                 isexecute = r.GetInt32(7),
//                 hashrln = r["hashrln"].ToString(),
//                 datecreate = (r.IsDBNull(9)) ? (DateTime?) null : r.GetDateTime(9),
//                 accncreate = r["accncreate"].ToString(),
//                 datemodify = (r.IsDBNull(11)) ? (DateTime?) null : r.GetDateTime(11),
//                 accnmodify = r["accnmodify"].ToString(),
//                 procmodify = r["procmodify"].ToString(),
//             };
//         }
//         private SqlCommand ixcommand(roln_ix o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);

//             return cm;
//         }
//         private SqlCommand obcommand(roln_md o,String sql){ 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
//             cm.Parameters.Add(o.apcode.snapsPar(nameof(o.apcode)));
//             cm.Parameters.Add(o.rolcode.snapsPar(nameof(o.rolcode)));
//             cm.Parameters.Add(o.objmodule.snapsPar(nameof(o.objmodule)));
//             cm.Parameters.Add(o.objtype.snapsPar(nameof(o.objtype)));
//             cm.Parameters.Add(o.objcode.snapsPar(nameof(o.objcode)));
//             cm.Parameters.Add(o.isenable.snapsPar(nameof(o.isenable)));
//             cm.Parameters.Add(o.isexecute.snapsPar(nameof(o.isexecute)));
//             cm.Parameters.Add(o.hashrln.snapsPar(nameof(o.hashrln)));
//             cm.Parameters.Add(o.datecreate.snapsPar(nameof(o.datecreate)));
//             cm.Parameters.Add(o.accncreate.snapsPar(nameof(o.accncreate)));
//             cm.Parameters.Add(o.datemodify.snapsPar(nameof(o.datemodify)));
//             cm.Parameters.Add(o.accnmodify.snapsPar(nameof(o.accnmodify)));
//             cm.Parameters.Add(o.procmodify.snapsPar(nameof(o.procmodify)));
//             return cm;
//         }

//         public SqlCommand oucommand(roln_ls o,String sql,String accnmodify) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
//             cm.Parameters.Add(o.apcode.snapsPar(nameof(o.apcode)));
//             cm.Parameters.Add(o.rolcode.snapsPar(nameof(o.rolcode)));
//             cm.Parameters.Add(o.objmodule.snapsPar(nameof(o.objmodule)));
//             cm.Parameters.Add(o.objtype.snapsPar(nameof(o.objtype)));
//             cm.Parameters.Add(o.objcode.snapsPar(nameof(o.objcode)));
//             return cm;
//         }
//         public SqlCommand oucommand(roln_md o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
//             cm.Parameters.Add(o.apcode.snapsPar(nameof(o.apcode)));
//             cm.Parameters.Add(o.rolcode.snapsPar(nameof(o.rolcode)));
//             cm.Parameters.Add(o.objmodule.snapsPar(nameof(o.objmodule)));
//             cm.Parameters.Add(o.objtype.snapsPar(nameof(o.objtype)));
//             cm.Parameters.Add(o.objcode.snapsPar(nameof(o.objcode)));
//             return cm;
//         }

//         public async Task<List<roln_ls>> find(roln_pm rs) { 
//             SqlCommand cm = new SqlCommand("",cn);
//             List<roln_ls> rn = new List<roln_ls>();
//             SqlDataReader r = null;
//             try { 
//                  /* Vlidate parameter */
//                 cm.snapsCdn(rs.orgcode,nameof(rs.orgcode));
//                 cm.snapsCdn(rs.apcode,nameof(rs.apcode));
//                 cm.snapsCdn(rs.rolcode,nameof(rs.rolcode));
//                 cm.snapsCdn(rs.objmodule,nameof(rs.objmodule));
//                 cm.snapsCdn(rs.objtype,nameof(rs.objtype));
//                 cm.snapsCdn(rs.objcode,nameof(rs.objcode));

//                 cm = (sqlfnd).snapsCommand(cn);
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn;
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();}  if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task<roln_md> get(roln_ls rs){ 
//             SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
//             roln_md rn = new roln_md();
//             try { 
//                 /* Vlidate parameter */
//                 cm.snapsCdn(rs.orgcode,nameof(rs.orgcode));
//                 cm.snapsCdn(rs.apcode,nameof(rs.apcode));
//                 cm.snapsCdn(rs.rolcode,nameof(rs.rolcode));
//                 cm.snapsCdn(rs.objmodule,nameof(rs.objmodule));
//                 cm.snapsCdn(rs.objtype,nameof(rs.objtype));
//                 cm.snapsCdn(rs.objcode,nameof(rs.objcode));

//                 cm = (sqlfnd).snapsCommand(cn);
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn = fillmdl(ref r); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn; 
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task upsert(List<roln_md> rs){ 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (roln_md ln in rs) {
//                     cm.Add(obcommand(ln,sqlval)); 
//                     cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//                 }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task upsert(roln_md rs){ 
//             List<roln_md> ro = new List<roln_md>(); 
//             try { 
//                 ro.Add(rs); await upsert(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task remove(List<roln_md> rs){
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             try { 
//                 foreach (roln_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task remove(roln_md rs){
//             List<roln_md> ro = new List<roln_md>(); 
//             try { 
//                 ro.Add(rs); await remove(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task import(List<roln_ix> rs) { 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (roln_ix ln in rs) {
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
