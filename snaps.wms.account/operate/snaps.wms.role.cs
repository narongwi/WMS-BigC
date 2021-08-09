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

//     public class role_ops : IDisposable { 
//         private SqlConnection cn = null;
//         private static string tbn = "wm_role";
//         private static string sqlmcom = " site = @site and depot = @depot and spcarea = @spcarea and thcode = @thcode " ;
//         private String sqlins = "insert into " + tbn + 
//         " ( orgcode, apcode, site, depot, rolecode, rolename, roledesc, tflow, hashrol, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
//         " values "  +
//         " ( @orgcode, @apcode, @site, @depot, @rolecode, @rolename, @roledesc, @tflow, @hashrol, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
//         private String sqlupd = " update " + tbn + " set " + 
//         " rolename = @rolename, roledesc = @roledesc, tflow = @tflow, hashrol = @hashrol,   "+ 
//         " datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " + 
//         " where 1=1 " + sqlmcom;        
//         private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
//         private String sqlfnd = "select * from " + tbn + " where 1=1 ";
//         private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
//         public role_ops() {  }
//         public role_ops(String cx) { cn = new SqlConnection(cx); }

//         private role_ls fillls(ref SqlDataReader r) { 
//             return new role_ls() { 
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 site = r["site"].ToString(),
//                 depot = r["depot"].ToString(),
//                 rolecode = r["rolecode"].ToString(),
//                 tflow = r["tflow"].ToString(),
//                 rolename = r["rolename"].ToString(),
//             };
//         }
//         private role_ix fillix(ref SqlDataReader r) { 
//             return new role_ix() {  

//             };
//         }
//         private role_md fillmdl(ref SqlDataReader r) { 
//             return new role_md() {
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 site = r["site"].ToString(),
//                 depot = r["depot"].ToString(),
//                 rolecode = r["rolecode"].ToString(),
//                 rolename = r["rolename"].ToString(),
//                 roledesc = r["roledesc"].ToString(),
//                 tflow = r["tflow"].ToString(),
//                 hashrol = r["hashrol"].ToString(),
//                 datecreate = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9),
//                 accncreate = r["accncreate"].ToString(),
//                 datemodify = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11),
//                 accnmodify = r["accnmodify"].ToString(),
//                 procmodify = r["procmodify"].ToString(),
//             };
//         }
//         private SqlCommand ixcommand(role_ix o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);

//             return cm;
//         }
//         private SqlCommand obcommand(role_md o,String sql){ 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//             cm.Parameters.Add(o.apcode.snapsPar("apcode"));
//             cm.Parameters.Add(o.site.snapsPar("site"));
//             cm.Parameters.Add(o.depot.snapsPar("depot"));
//             cm.Parameters.Add(o.rolecode.snapsPar("rolecode"));
//             cm.Parameters.Add(o.rolename.snapsPar("rolename"));
//             cm.Parameters.Add(o.roledesc.snapsPar("roledesc"));
//             cm.Parameters.Add(o.tflow.snapsPar("tflow"));
//             cm.Parameters.Add(o.hashrol.snapsPar("hashrol"));
//             cm.Parameters.Add(o.datecreate.snapsPar("datecreate"));
//             cm.Parameters.Add(o.accncreate.snapsPar("accncreate"));
//             cm.Parameters.Add(o.datemodify.snapsPar("datemodify"));
//             cm.Parameters.Add(o.accnmodify.snapsPar("accnmodify"));
//             cm.Parameters.Add(o.procmodify.snapsPar("procmodify"));
//             return cm;
//         }

//         public SqlCommand oucommand(role_ls o,String sql,String accnmodify) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//             cm.Parameters.Add(o.apcode.snapsPar("apcode"));
//             cm.Parameters.Add(o.rolecode.snapsPar("rolecode"));
//             cm.Parameters.Add(o.site.snapsPar("site"));
//             cm.Parameters.Add(o.depot.snapsPar("depot"));
//             return cm;
//         }
//         public SqlCommand oucommand(role_md o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//             cm.Parameters.Add(o.apcode.snapsPar("apcode"));
//             cm.Parameters.Add(o.rolecode.snapsPar("rolecode"));
//             cm.Parameters.Add(o.site.snapsPar("site"));
//             cm.Parameters.Add(o.depot.snapsPar("depot"));
//             return cm;
//         }

//         public async Task<List<role_ls>> find(role_pm rs) { 
//             SqlCommand cm = new SqlCommand("",cn);
//             List<role_ls> rn = new List<role_ls>();
//             SqlDataReader r = null;
//             try { 
//                  /* Vlidate parameter */
//                 cm.snapsCdn(rs.orgcode,"orgcode");
//                 cm.snapsCdn(rs.apcode,"apcode");
//                 cm.snapsCdn(rs.site,"site");
//                 cm.snapsCdn(rs.depot,"depot");
//                 cm.snapsCdn(rs.rolecode,"rolecode");
//                 cm.snapsCdn(rs.rolename,"rolename");
//                 cm.snapsCdn(rs.tflow,"tflow");
//                 cm.snapsCdn(rs.hashrol,"hashrol");
//                 cm = (sqlfnd).snapsCommand(cn);
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn;
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();}  if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task<role_md> get(role_ls rs){ 
//             String sqlcat = "select m.* from wm_roms m where m.objmodule = 'category' and m.orgcode = @orgcode order by objseq";
//             String sqlpms = "select m.*,isnull(n.isenable,0) isenable, isnull(n.isexecute,0) isexecute from wm_roms m left join (select * from wm_roln where orgcode = @orgcode and rolecode = @rolecode) n " + 
//             " on m.orgcode = n.orgcode and m.objmodule = n.objmodule and m.objcode = n.objcode where m.objmodule != 'category' order by objseq " ;
//             SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
//             role_md rn = new role_md();
//             try { 
//                 /* Vlidate parameter */
//                 cm = (sqlfnd).snapsCommand(cn);
//                 cm.snapsCdn(rs.orgcode,"orgcode");
//                 cm.snapsCdn(rs.apcode,"apcode");
//                 cm.snapsCdn(rs.rolecode,"rolecode");                
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn = fillmdl(ref r); }
//                 await r.CloseAsync(); 
//                 //fill category
//                 rn.roleaccess = new accn_roleacs();
//                 rn.roleaccess.site = rn.site;
//                 rn.roleaccess.depot = rn.depot;
//                 rn.roleaccess.rolecode = rn.rolecode;
//                 rn.roleaccess.rolename = rn.rolename;
//                 rn.roleaccess.modules = new List<accn_category>();
//                 cm.CommandText = sqlcat;
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { 
//                     rn.roleaccess.modules.Add(new accn_category() { 
//                         moduleicon = r["objicon"].ToString(),
//                         modulename = r["objname"].ToString(),
//                         modulecode = r["objcode"].ToString(),                    
//                         permission = new List<accn_permision>()
//                     });
//                 }
//                 await r.CloseAsync(); 
//                 //fill permission
//                 cm.CommandText = sqlpms;
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { 
//                     rn.roleaccess.modules.Where(x=>x.modulecode == r["objmodule"].ToString()).First().permission.Add(new accn_permision() {
//                         objcode = r["objcode"].ToString(),
//                         apcode = r["apcode"].ToString(), 
//                         objicon = r["objicon"].ToString(), 
//                         objmodule = r["objmodule"].ToString(),
//                         objname = r["objname"].ToString(),
//                         objroute = r["objroute"].ToString(),
//                         objseq = r.GetInt32(7), 
//                         objtype = r["objtype"].ToString(),
//                         orgcode = r["orgcode"].ToString(),
//                         rolecode = r["rolecode"].ToString(), 
//                         isenable = r.GetInt32(17),
//                         isexecute = r.GetInt32(18)
//                     });
//                 }
//                 return rn; 
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task upsert(List<role_md> rs){ 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (role_md ln in rs) {
//                     cm.Add(obcommand(ln,sqlval)); 
//                     cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//                 }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task upsert(role_md rs){ 
//             List<role_md> ro = new List<role_md>(); 
//             try { 
//                 ro.Add(rs); await upsert(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task remove(List<role_md> rs){
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             try { 
//                 foreach (role_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task remove(role_md rs){
//             List<role_md> ro = new List<role_md>(); 
//             try { 
//                 ro.Add(rs); await remove(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task import(List<role_ix> rs) { 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (role_ix ln in rs) {
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

//         public String getMessage(String Lang,String Ercode){ 
//             SqlCommand cm = new SqlCommand(string.Format("select ISNULL((select descmsg from wm_message where apps = 'WMS' and typemsg = 'ER'" + 
//             " and langmsg = '{0}' and codemsg = '{1}'),'{1}')",Lang,Ercode),cn);
//             try { 
//                 return cm.snapsScalarStrAsync().Result;
//             } catch (Exception ex) { throw ex; 
//             } finally { cm.Dispose();}
//         }

//     }

// }
