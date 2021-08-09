// using System;
// using System.Text;
// using System.Linq;
// using System.Diagnostics;
// using System.Threading.Tasks;
// using System.Security.Claims;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;

// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.AspNetCore.Authorization;

// using Snaps.Helpers;
// using Snaps.Helpers.Json;
// using Snaps.WMS.Interfaces;
// using Snaps.Helpers.Logging;

// namespace Snaps.WMS.Controllers {

//     [ApiController] [Route("[controller]")]
//     public class RoleController : ControllerBase {
//         private IRoleService _sv;
//         private readonly ILogger<RoleController> _log;
//         private readonly AppSettings _optn;
//         private readonly String app = "api.Administrator.Role";
//         public RoleController(ILogger<RoleController> logger,IOptions<AppSettings> optn, IRoleService orolesv) { 
//             _sv = orolesv; _log = logger; _optn = optn.Value; }

//         [Authorize] [HttpPost("list/{id}")]
//         public async Task<IActionResult> ListAsync(String id, [FromBody] role_md o, 
//                     [FromHeader(Name="site")] String valsite,
//                     [FromHeader(Name="depot")] String valdepot,
//                     [FromHeader(Name="rolecode")] String valrole,
//                     [FromHeader(Name="orgcode")] String valorg= "bgc", 
//                     [FromHeader(Name="lang")] String lng = "EN"
//                     ) {
//             Process ps ;  SnapsLogDbg p = null; 
//             try { 
//                 if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", id, Request.getIP(), app, valrole); }
//                 o.orgcode = valorg;
//                 return Ok(await _sv.listAsync(o));                                
//             }catch (Exception exr) {         
//                 _log.LogError(exr.SnapsLogExc(Request.getIP(),valrole,app,"ListAsync",rqid:id,ob:o));       
//                 return BadRequest(exr.SnapsBadRequest());
//             }finally { 
//                 if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
//             }
//         }

//         [Authorize] [HttpPost("get/{id}")]
//         public async Task<IActionResult> getAsync(String id, [FromBody] role_md o, 
//                     [FromHeader(Name="site")] String valsite,
//                     [FromHeader(Name="depot")] String valdepot,
//                     [FromHeader(Name="rolecode")] String valrole,
//                     [FromHeader(Name="orgcode")] String valorg= "bgc", 
//                     [FromHeader(Name="lang")] String lng = "EN"
//                     ) {
//             Process ps ;  SnapsLogDbg p = null; 
//             try { 
//                 if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getAsync", id, Request.getIP(), app, valrole); }
//                 o.orgcode = valorg;
//                 return Ok(await _sv.getAsync(o));                                
//             }catch (Exception exr) {         
//                 _log.LogError(exr.SnapsLogExc(Request.getIP(),valrole,app,"getAsync",rqid:id,ob:o));       
//                 return BadRequest(exr.SnapsBadRequest());
//             }finally { 
//                 if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
//             }
//         }

//         [Authorize] [HttpPost("upsert/{id}")]
//         public async Task<IActionResult> upsertAsync(String id, [FromBody] role_md o, 
//                     [FromHeader(Name="site")] String valsite,
//                     [FromHeader(Name="depot")] String valdepot,
//                     [FromHeader(Name="rolecode")] String valrole,
//                     [FromHeader(Name="orgcode")] String valorg= "bgc", 
//                     [FromHeader(Name="lang")] String lng = "EN"
//                     ) {
//             Process ps ;  SnapsLogDbg p = null; 
//             try { 
//                 if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsertAsync", id, Request.getIP(), app, valrole); }
//                 o.orgcode = valorg;
//                 await _sv.upsertAsync(o);
//                 return Ok();                                
//             }catch (Exception exr) {         
//                 _log.LogError(exr.SnapsLogExc(Request.getIP(),valrole,app,"upsertAsync",rqid:id,ob:o));       
//                 return BadRequest(exr.SnapsBadRequest());
//             }finally { 
//                 if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
//             }
//         }

//     }
// }