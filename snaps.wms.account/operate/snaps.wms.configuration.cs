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
//     public class config_ls {
//         public String orgcode { get; set; } 
//         public String apcode { get; set; } 
//         public String accncode { get; set; } 
//         public String cfgtype { get; set; } 
//         public String cfgcode { get; set; } 
//         public String cfgname { get; set; } 
//         public String tflow { get; set; } 
//     }
//     public class config_pm : config_ls { 
//         public String cfgvalue { get; set; } 
//         public String cfghash { get; set; } 
//     }
//     public class config_ix : config_ls { 
//     }
//     public class config_md : config_ls  {
//         public String cfgvalue { get; set; } 
//         public String cfghash { get; set; } 
//         public DateTime? datecreate { get; set; } 
//         public String accncreate { get; set; } 
//         public DateTime? datemodify { get; set; } 
//         public String accnmodify { get; set; } 
//         public String procmodify { get; set; } 
//     }
//     public class config_prc { 
//         public String opsaccn { get; set; }
//         public config_md opsobj { get; set; } 
//     }
//     public class config_ops : IDisposable { 
//         private SqlConnection cn = null;
//         private static string tbn = "wm_configs";
//         private static string sqlmcom = " and orgcode = @orgcode  and apcode = @apcode  and accncode = @accncode  and cfgtype = @cfgtype  and cfgcode = @cfgcode " ;
//         private String sqlins = "insert into " + tbn + 
//         " ( orgcode, apcode, accncode, cfgtype, cfgcode, cfgname, cfgvalue, cfghash, tflow, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
//         " values "  +
//         " ( @orgcode, @apcode, @accncode, @cfgtype, @cfgcode, @cfgname, @cfgvalue, @cfghash, @tflow, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
//         private String sqlupd = " update " + tbn + " set " + 
//         " cfgname = @cfgname, cfgvalue = @cfgvalue, cfghash = @cfghash, tflow = @tflow,   datemodify = @datemodify, "+ 
//         " accnmodify = @accnmodify, procmodify = @procmodify " + 
//         " where 1=1 " + sqlmcom;        
//         private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
//         private String sqlfnd = "select * from " + tbn + " where 1=1 + ";
//         private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
//         public config_ops() {  }
//         public config_ops(String cx) { cn = new SqlConnection(cx); }

//         private config_ls fillls(ref SqlDataReader r) { 
//             return new config_ls() { 
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 accncode = r["accncode"].ToString(),
//                 cfgtype = r["cfgtype"].ToString(),
//                 cfgcode = r["cfgcode"].ToString(),
//                 cfgname = r["cfgname"].ToString(),
//                 tflow = r["tflow"].ToString(),
//             };
//         }
//         private config_ix fillix(ref SqlDataReader r) { 
//             return new config_ix() { 

//             };
//         }
//         private config_md fillmdl(ref SqlDataReader r) { 
//             return new config_md() {
//                 orgcode = r["orgcode"].ToString(),
//                 apcode = r["apcode"].ToString(),
//                 accncode = r["accncode"].ToString(),
//                 cfgtype = r["cfgtype"].ToString(),
//                 cfgcode = r["cfgcode"].ToString(),
//                 cfgname = r["cfgname"].ToString(),
//                 cfgvalue = r["cfgvalue"].ToString(),
//                 cfghash = r["cfghash"].ToString(),
//                 tflow = r["tflow"].ToString(),
//                 datecreate = (r.IsDBNull(9)) ? (DateTime?) null : r.GetDateTime(9),
//                 accncreate = r["accncreate"].ToString(),
//                 datemodify = (r.IsDBNull(11)) ? (DateTime?) null : r.GetDateTime(11),
//                 accnmodify = r["accnmodify"].ToString(),
//                 procmodify = r["procmodify"].ToString(),
//             };
//         }
//         private SqlCommand ixcommand(config_ix o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);

//             return cm;
//         }
//         private SqlCommand obcommand(config_md o,String sql){ 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
//             cm.Parameters.Add(o.apcode.snapsPar(nameof(o.apcode)));
//             cm.Parameters.Add(o.accncode.snapsPar(nameof(o.accncode)));
//             cm.Parameters.Add(o.cfgtype.snapsPar(nameof(o.cfgtype)));
//             cm.Parameters.Add(o.cfgcode.snapsPar(nameof(o.cfgcode)));
//             cm.Parameters.Add(o.cfgname.snapsPar(nameof(o.cfgname)));
//             cm.Parameters.Add(o.cfgvalue.snapsPar(nameof(o.cfgvalue)));
//             cm.Parameters.Add(o.cfghash.snapsPar(nameof(o.cfghash)));
//             cm.Parameters.Add(o.tflow.snapsPar(nameof(o.tflow)));
//             cm.Parameters.Add(o.datecreate.snapsPar(nameof(o.datecreate)));
//             cm.Parameters.Add(o.accncreate.snapsPar(nameof(o.accncreate)));
//             cm.Parameters.Add(o.datemodify.snapsPar(nameof(o.datemodify)));
//             cm.Parameters.Add(o.accnmodify.snapsPar(nameof(o.accnmodify)));
//             cm.Parameters.Add(o.procmodify.snapsPar(nameof(o.procmodify)));
//             return cm;
//         }

//         public SqlCommand oucommand(config_ls o,String sql,String accnmodify) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
//             cm.Parameters.Add(o.apcode.snapsPar(nameof(o.apcode)));
//             cm.Parameters.Add(o.accncode.snapsPar(nameof(o.accncode)));
//             cm.Parameters.Add(o.cfgtype.snapsPar(nameof(o.cfgtype)));
//             cm.Parameters.Add(o.cfgcode.snapsPar(nameof(o.cfgcode)));
//             return cm;
//         }
//         public SqlCommand oucommand(config_md o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.Parameters.Add(o.orgcode.snapsPar(nameof(o.orgcode)));
//             cm.Parameters.Add(o.apcode.snapsPar(nameof(o.apcode)));
//             cm.Parameters.Add(o.accncode.snapsPar(nameof(o.accncode)));
//             cm.Parameters.Add(o.cfgtype.snapsPar(nameof(o.cfgtype)));
//             cm.Parameters.Add(o.cfgcode.snapsPar(nameof(o.cfgcode)));
//             return cm;
//         }

//         public async Task<List<config_ls>> find(config_pm rs) { 
//             SqlCommand cm = new SqlCommand("",cn);
//             List<config_ls> rn = new List<config_ls>();
//             SqlDataReader r = null;
//             try { 
//                  /* Vlidate parameter */
//                 cm.snapsCdn(rs.orgcode,nameof(rs.orgcode));
//                 cm.snapsCdn(rs.apcode,nameof(rs.apcode));
//                 cm.snapsCdn(rs.accncode,nameof(rs.accncode));
//                 cm.snapsCdn(rs.cfgtype,nameof(rs.cfgtype));
//                 cm.snapsCdn(rs.cfgcode,nameof(rs.cfgcode));
//                 cm.snapsCdn(rs.cfgvalue,nameof(rs.cfgvalue));
//                 cm.snapsCdn(rs.cfghash,nameof(rs.cfghash));
//                 cm.snapsCdn(rs.tflow,nameof(rs.tflow));

//                 cm = (sqlfnd).snapsCommand(cn);
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn;
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();}  if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task<config_md> get(config_ls rs){ 
//             SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
//             config_md rn = new config_md();
//             try { 
//                 /* Vlidate parameter */
//                 cm.snapsCdn(rs.orgcode,nameof(rs.orgcode));
//                 cm.snapsCdn(rs.apcode,nameof(rs.apcode));
//                 cm.snapsCdn(rs.accncode,nameof(rs.accncode));
//                 cm.snapsCdn(rs.cfgtype,nameof(rs.cfgtype));
//                 cm.snapsCdn(rs.cfgcode,nameof(rs.cfgcode));
//                 cm = (sqlfnd).snapsCommand(cn);
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn = fillmdl(ref r); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn; 
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task upsert(List<config_md> rs){ 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (config_md ln in rs) {
//                     cm.Add(obcommand(ln,sqlval)); 
//                     cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//                 }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task upsert(config_md rs){ 
//             List<config_md> ro = new List<config_md>(); 
//             try { 
//                 ro.Add(rs); await upsert(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task remove(List<config_md> rs){
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             try { 
//                 foreach (config_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task remove(config_md rs){
//             List<config_md> ro = new List<config_md>(); 
//             try { 
//                 ro.Add(rs); await remove(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task import(List<config_ix> rs) { 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (config_ix ln in rs) {
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
