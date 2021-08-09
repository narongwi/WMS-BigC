// using System;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Options;
// using Snaps.WMS.Interfaces;
// using Snaps.Helpers;
// using System.Collections.Generic;
// using Snaps.WMS;
// namespace Snaps.WMS.Services { 
//     public class RoleService : IRoleService { 
//         private readonly IOptions<AppSettings>  _appSettings;
//         private role_ops op;
//         public RoleService(IOptions<AppSettings> appSettings) {
//             _appSettings = appSettings;
//             op = new role_ops(_appSettings.Value.Conxstr); 
//         }

//         public resultRequest getMessage(String lang, String ercode,resultState result,String reqid, String opt1 = "",String opt2 = "" ){ 
//             try     { return new resultRequest(result,string.Format(op.getMessage(lang, ercode).ToString(), opt1, opt2),reqid); } 
//             catch   (Exception ex) { throw ex; } 
//             finally { }
//         }
//         public async Task<List<role_ls>> listAsync(role_pm o){ 
//             try     { return await op.find(o); } 
//             catch   (Exception ex) { throw ex; } 
//             finally { }
//         }
//         public async Task<role_md> getAsync(role_ls o) { 
//             try     { return await op.get(o); } 
//             catch   (Exception ex) { throw ex; } 
//             finally { }
//         }
//         public async Task upsertAsync(role_md o){ 
//             try     { await op.upsert(o); } 
//             catch   (Exception ex) { throw ex; } 
//             finally { }
//         }
//     }
// }