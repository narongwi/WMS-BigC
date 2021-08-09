// using System;
// using System.Linq;
// using System.Data.Common;
// using System.Data.SqlClient;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Snaps.Helpers;
// using Snaps.Helpers.StringExt;
// using Snaps.Helpers.DbContext.SQLServer;
// using System.Data;

// namespace Snaps.WMS.preparation {
//     //Header 
//     public partial class  fairshare_ops : IDisposable {
        
//         public async Task<List<fairshare>> find(fairshare o){
//             SqlCommand cm = new SqlCommand("",cn);
//             List<fairshare> rn = new List<fairshare>();
//             SqlDataReader r = null;
//             try { 
//                 cm.CommandText = share_fnd;
//                 cm.snapsPar(o.orgcode,"orgcode");
//                 cm.snapsPar(o.site,"site");
//                 cm.snapsPar(o.depot,"depot");
//                 cm.snapsCdn(o.sharecode,"sharecode");
//                 cm.snapsCdn(o.sharename,"sharename");
//                 cm.snapsCdn(o.tflow,"tflow");                
//                 r = await cm.snapsReadAsync();
//                 //while(await r.ReadAsync()) { rn.Add(fairshare_get(ref r)); }
//                 await r.CloseAsync(); 
//                 await cn.CloseAsync(); 
//                 return rn;
//             }catch (Exception ex) { 
//                 throw ex;
//             }finally { 
//                 cm.Dispose(); if (r!=null){ await r.DisposeAsync(); }
//             }
//         }

//         public async Task<fairshare> get(fairshare o) { 
//             SqlCommand cm = new SqlCommand("",cn);
//             fairshare rn = new fairshare(); 
//             SqlDataReader r = null;
//             try { 
//                 cm.CommandText = share_fnd;
//                 cm.snapsPar(o.orgcode,"orgcode");
//                 cm.snapsPar(o.site,"site");
//                 cm.snapsPar(o.depot,"depot");
//                 cm.snapsPar(o.sharecode,"sharecode");
//                 r = await cm.snapsReadAsync();
//                 //while(await r.ReadAsync()) { rn.Add(fairshare_get(ref r)); }
//                 await r.CloseAsync(); 

//                 //cm.CommandText = share_fndln;
//                 r = await cm.snapsReadAsync();
//                 //while(await r.ReadAsync()) { rn.Add(fairshaln_get(ref r)); }
//                 await r.CloseAsync(); 

//                 return rn;
//             }catch (Exception ex) { throw ex; } 
//             finally { cm.Dispose(); if (r!=null) { await r.DisposeAsync(); } }
//         }

//         public async Task upsert(fairshare o) { 
//             SqlCommand cm = new SqlCommand("", cn);
            
//         }
//     }
// }