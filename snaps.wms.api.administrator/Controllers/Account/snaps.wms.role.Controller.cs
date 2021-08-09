using System;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

using Snaps.Helpers;
using Snaps.Helpers.Json;
using Snaps.WMS.Interfaces;
using Snaps.Helpers.Logging;

namespace Snaps.WMS.Controllers {

    [ApiController] [Route("[controller]")]
    public class roleController : ControllerBase {
        private IroleService svpl;
        private IaccountService svac;
        private readonly ILogger<policyController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.admin.role";
        public roleController(ILogger<policyController> logger,IOptions<AppSettings> optn, IroleService _svpl, IaccountService _svac) { 
            svpl = _svpl; _log = logger; _optn = optn.Value; svac = _svac; }

        [Authorize] [HttpPost("List/{id}")]
        public async Task<IActionResult> findAsync(String id, [FromBody] role_md o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] string accscode,
                    [FromHeader(Name="orgcode")] String orgcode , 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"findAsync", id, Request.getIP(), app, accncode); }                
                if (await svac.vldSession(accscode) == false) { 
                    return Unauthorized("Security passcode has expire, please login again");
                }else { 
                    o.orgcode = orgcode;
                    //o.site = site;
                    //o.depot = depot;
                    o.accnmodify = accncode;
                    o.accncreate = accncode;
                    return Ok(await svpl.findAsync(o));        
                }                      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"findAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("get/{id}")]
        public async Task<IActionResult> getAsync(String id, [FromBody] role_md o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] string accscode,
                    [FromHeader(Name="orgcode")] String orgcode , 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"findAsync", id, Request.getIP(), app, accncode); }                
                if (await svac.vldSession(accscode) == false) { 
                    return Unauthorized("Security passcode has expire, please login again");
                }else { 
                    o.orgcode = orgcode;
                    //o.site = site;
                    //o.depot = depot;
                    o.accnmodify = accncode;
                    o.accncreate = accncode;
                    return Ok(await svpl.getAsync(o));        
                }                      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"findAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("upsert/{id}")]
        public async Task<IActionResult> upsertAsync(String id, [FromBody] role_md o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] string accscode,
                    [FromHeader(Name="orgcode")] String orgcode , 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsertAsync", id, Request.getIP(), app, accncode); }                
                if (await svac.vldSession(accscode) == false) { 
                    return Unauthorized("Security passcode has expire, please login again");
                }else { 
                    o.orgcode = orgcode;
                    //o.site = site;
                    //o.depot = depot;
                    o.accnmodify = accncode;
                    o.accncreate = accncode;
                    await svpl.upsertAsync(o);
                    return Ok();        
                }                      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"upsertAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("drop/{id}")]
        public async Task<IActionResult> dropAsync(String id, [FromBody] role_md o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] string accscode,
                    [FromHeader(Name="orgcode")] String orgcode , 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"dropAsync", id, Request.getIP(), app, accncode); }                
                if (await svac.vldSession(accscode) == false) { 
                    return Unauthorized("Security passcode has expire, please login again");
                }else { 
                    o.orgcode = orgcode;
                    //o.site = site;
                    //o.depot = depot;
                    o.accnmodify = accncode;
                    o.accncreate = accncode;
                    await svpl.dropAsync(o);
                    return Ok();        
                }                      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"dropAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("getMaster/{rolesite}/{roledepot}/{id}")]
        public async Task<IActionResult> getMasterAsync(string rolesite,string roledepot,String id, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] string accscode,
                    [FromHeader(Name="orgcode")] String orgcode , 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getMasterAsync", id, Request.getIP(), app, accncode); }                
                if (await svac.vldSession(accscode) == false) { 
                    return Unauthorized("Security passcode has expire, please login again");
                }else { 
                    return Ok(await svpl.getMasterAsync(orgcode,rolesite,roledepot, accncode));        
                }                      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"getMasterAsync",rqid:id,ob:""));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

    }
}