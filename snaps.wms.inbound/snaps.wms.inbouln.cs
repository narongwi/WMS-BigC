// using System;
// using System.Linq;
// using System.Data.Common;
// using System.Data.SqlClient;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using System.ComponentModel.DataAnnotations;
// using Snaps.Helpers;
// using Snaps.Helpers.StringExt;
// using Snaps.Helpers.DbContext.SQLServer;

// namespace Snaps.WMS {
//     public class inbouln_ops : IDisposable { 
//         private SqlConnection cn = null;

//         public inbouln_ops() {  }
//         public inbouln_ops(String cx) { cn = new SqlConnection(cx); }

//         private inbouln_ls fillls(ref SqlDataReader r) { 
//             return new inbouln_ls() { 
//                 orgcode = r["orgcode"].ToString(),
//                 site = r["site"].ToString(),
//                 depot = r["depot"].ToString(),
//                 spcarea = r["spcarea"].ToString(),
//                 inorder = r["inorder"].ToString(),
//                 inln = r.GetInt32(5),
//                 barcode = r["barcode"].ToString(),
//                 article = r["article"].ToString(),
//                 pv = r.GetInt32(8),
//                 lv = r.GetInt32(9),
//                 qtysku = r.GetInt32(10),
//                 qtypu = r.GetInt32(11),
//                 qtyweight = r.GetDecimal(12),
//                 tflow = r["tflow"].ToString(),
//                 description = r["description"].ToString(),
//                 unitopsdesc = r["unitopsdesc"].ToString(),
//             };
//         }
//         private inbouln_ix fillix(ref SqlDataReader r) { 
//             return new inbouln_ix() { 
//                 orgcode = r["orgcode"].ToString(),
//                 site = r["site"].ToString(),
//                 depot = r["depot"].ToString(),
//                 spcarea = r["spcarea"].ToString(),
//                 inorder = r["inorder"].ToString(),
//                 inln = r.GetInt32(5),
//                 inrefno = r["inrefno"].ToString(),
//                 inrefln = r.GetInt32(7),
//                 inagrn = r["inagrn"].ToString(),
//                 barcode = r["barcode"].ToString(),
//                 article = r["article"].ToString(),
//                 pv = r.GetInt32(11),
//                 lv = r.GetInt32(12),
//                 unitops = r["unitops"].ToString(),
//                 qtysku = r.GetInt32(14),
//                 qtypu = r.GetInt32(15),
//                 qtyweight = r.GetDecimal(16),
//                 lotno = r["lotno"].ToString(),
//                 expdate = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(18),
//                 serialno = r["serialno"].ToString(),
//                 tflow = r["tflow"].ToString(),
//                 fileid = r["fileid"].ToString(),
//                 rowops = r["rowops"].ToString(),
//                 ermsg = r["ermsg"].ToString(),
//                 dateops = (r.IsDBNull(23)) ? (DateTimeOffset?) null : r.GetDateTime(23),
//             };
//         }
//         private inbouln_md fillmdl(ref SqlDataReader r) { 
//             return new inbouln_md() {
//                 orgcode = r["orgcode"].ToString(),
//                 site = r["site"].ToString(),
//                 depot = r["depot"].ToString(),
//                 spcarea = r["spcarea"].ToString(),
//                 inorder = r["inorder"].ToString(),
//                 inln = r.GetInt32(5),
//                 inrefno = r["inrefno"].ToString(),
//                 inrefln = r.GetInt32(7),
//                 inagrn = r["inagrn"].ToString(),
//                 barcode = r["barcode"].ToString(),
//                 article = r["article"].ToString(),
//                 pv = r.GetInt32(11),
//                 lv = r.GetInt32(12),
//                 unitops = r["unitops"].ToString(),
//                 qtysku = r.GetInt32(14),
//                 qtypu = r.GetInt32(15),
//                 qtyweight = r.GetDecimal(16),
//                 lotno = r["lotno"].ToString(),
//                 expdate = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(18),
//                 serialno = r["serialno"].ToString(),
//                 qtypnd = r.GetDecimal(20),
//                 qtyskurec = r.GetInt32(21),
//                 qtypurec = r.GetInt32(22),
//                 qtyweightrec = r.GetDecimal(23),
//                 qtynaturalloss = r.GetDecimal(24),
//                 tflow = r["tflow"].ToString(),
//                 datecreate = (r.IsDBNull(26)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(26),
//                 accncreate = r["accncreate"].ToString(),
//                 datemodify = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(28),
//                 accnmodify = r["accnmodify"].ToString(),
//                 procmodify = r["procmodify"].ToString(),
//             };
//         }
//         private SqlCommand ixcommand(inbouln_ix o,String sql) { 
//             SqlCommand cm = new SqlCommand(sql,cn);
//                 cm.snapsPar(o.orgcode,"orgcode");
//                 cm.snapsPar(o.site,"site");
//                 cm.snapsPar(o.depot,"depot");
//                 cm.snapsPar(o.spcarea,"spcarea");
//                 cm.snapsPar(o.inorder,"inorder");
//                 cm.snapsPar(o.inln,"inln");
//                 cm.snapsPar(o.inrefno,"inrefno");
//                 cm.snapsPar(o.inrefln,"inrefln");
//                 cm.snapsPar(o.inagrn,"inagrn");
//                 cm.snapsPar(o.barcode,"barcode");
//                 cm.snapsPar(o.article,"article");
//                 cm.snapsPar(o.pv,"pv");
//                 cm.snapsPar(o.lv,"lv");
//                 cm.snapsPar(o.unitops,"unitops");
//                 cm.snapsPar(o.qtysku,"qtysku");
//                 cm.snapsPar(o.qtypu,"qtypu");
//                 cm.snapsPar(o.qtyweight,"qtyweight");
//                 cm.snapsPar(o.lotno,"lotno");
//                 cm.snapsPar(o.expdate,"expdate");
//                 cm.snapsPar(o.serialno,"serialno");
//                 cm.snapsPar(o.tflow,"tflow");
//                 cm.snapsPar(o.fileid,"fileid");
//                 cm.snapsPar(o.rowops,"rowops");
//                 cm.snapsPar(o.ermsg,"ermsg");
//                 cm.snapsPar(o.dateops,"dateops");
//             return cm;
//         }
//         private SqlCommand obcommand(inbouln_md o,String sql){ 
//             SqlCommand cm = new SqlCommand(sql,cn);
//                 cm.snapsPar(o.orgcode,"orgcode");
//                 cm.snapsPar(o.site,"site");
//                 cm.snapsPar(o.depot,"depot");
//                 cm.snapsPar(o.spcarea,"spcarea");
//                 cm.snapsPar(o.inorder,"inorder");
//                 cm.snapsPar(o.inln,"inln");
//                 cm.snapsPar(o.inrefno,"inrefno");
//                 cm.snapsPar(o.inrefln,"inrefln");
//                 cm.snapsPar(o.inagrn,"inagrn");
//                 cm.snapsPar(o.barcode,"barcode");
//                 cm.snapsPar(o.article,"article");
//                 cm.snapsPar(o.pv,"pv");
//                 cm.snapsPar(o.lv,"lv");
//                 cm.snapsPar(o.unitops,"unitops");
//                 cm.snapsPar(o.qtysku,"qtysku");
//                 cm.snapsPar(o.qtypu,"qtypu");
//                 cm.snapsPar(o.qtyweight,"qtyweight");
//                 cm.snapsPar(o.lotno,"lotno");
//                 cm.snapsPar(o.expdate,"expdate");
//                 cm.snapsPar(o.serialno,"serialno");
//                 cm.snapsPar(o.qtypnd,"qtypnd");
//                 cm.snapsPar(o.qtyskurec,"qtyskurec");
//                 cm.snapsPar(o.qtypurec,"qtypurec");
//                 cm.snapsPar(o.qtyweightrec,"qtyweightrec");
//                 cm.snapsPar(o.qtynaturalloss,"qtynaturalloss");
//                 cm.snapsPar(o.tflow,"tflow");
//                 cm.snapsPar(o.accncreate,"accncreate");
//                 cm.snapsPar(o.accnmodify,"accnmodify");
//                 cm.snapsPar(o.procmodify,"procmodify");
//             cm.snapsParsysdateoffset();
//             return cm;
//         }
//         public async Task<List<inbouln_ls>> find(inbouln_pm rs) { 
//             SqlCommand cm = null;
//             //String sqlpam = "";
//             List<inbouln_ls> rn = new List<inbouln_ls>();
//             SqlDataReader r = null;
//             try { 
//                  /* Vlidate parameter */
//                 cm = (sqlfnd).snapsCommand(cn);
//                 cm.snapsCdn(rs.orgcode,"orgcode");
//                 cm.snapsCdn(rs.site,"site");
//                 cm.snapsCdn(rs.depot,"depot");
//                 cm.snapsCdn(rs.spcarea,"spcarea");
//                 cm.snapsCdn(rs.inorder,"inorder");
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn;
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task<inbouln_md> get(inbouln_ls rs){ 
//             SqlCommand cm = null; SqlDataReader r = null;
//             inbouln_md rn = new inbouln_md();
//             //String sqlpam = "";
//             try { 
//                 /* Vlidate parameter */
//                 cm = (sqlfnd).snapsCommand(cn);
//                 cm.snapsCdn(rs.orgcode,"orgcode");
//                 cm.snapsCdn(rs.site,"site");
//                 cm.snapsCdn(rs.depot,"depot");
//                 cm.snapsCdn(rs.spcarea,"spcarea");
//                 cm.snapsCdn(rs.inorder,"inorder");
                
//                 r = await cm.snapsReadAsync();
//                 while(await r.ReadAsync()) { rn = fillmdl(ref r); }
//                 await r.CloseAsync(); await cn.CloseAsync(); 
//                 return rn; 
//             }
//             catch (Exception ex) { throw ex; }
//             finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         }
//         public async Task upsert(List<inbouln_md> rs){ 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (inbouln_md ln in rs) {
//                     cm.Add(obcommand(ln,sqlval)); 
//                     cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//                 }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task upsert(inbouln_md rs){ 
//             List<inbouln_md> ro = new List<inbouln_md>(); 
//             try { 
//                 ro.Add(rs); await upsert(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task remove(List<inbouln_md> rs){
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             try { 
//                 foreach (inbouln_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task remove(inbouln_md rs){
//             List<inbouln_md> ro = new List<inbouln_md>(); 
//             try { 
//                 ro.Add(rs); await remove(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task import(List<inbouln_ix> rs) { 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (inbouln_ix ln in rs) {
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
//                 if (cn != null) { cn.Dispose(); } 
//             }
//             disposedValue = true;
//         }
//         public void Dispose() {
//             GC.SuppressFinalize(this);
//         }

//     }

// }

