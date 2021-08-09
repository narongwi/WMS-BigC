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
    public class ouborderController : ControllerBase {
        private IouborderService _sv;
        private readonly ILogger<ouborderController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.inv.ouborder";
        public ouborderController(ILogger<ouborderController> logger,IOptions<AppSettings> optn, IouborderService oaccnsv) { 
            _sv = oaccnsv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("list/{id}")]
        public async Task<IActionResult> listProductAsync(String id, [FromBody] outbound_pm o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.listAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                
                return Ok(await _sv.listAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.listAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("listdist/{id}")]
        public async Task<IActionResult> listdistAsync(String id, [FromBody] outbound_pm o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.listdistAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                return Ok(await _sv.listdistAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.listdistAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("get/{id}")]
        public async Task<IActionResult> getAsync(String id, [FromBody] outbound_ls o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.getAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                return Ok(await _sv.getAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.getAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("getdist/{id}")]
        public async Task<IActionResult> getdistAsync(String id, [FromBody] outbound_ls o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.getdistAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                
                return Ok(await _sv.getdistAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.getdistAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("setpriority/{id}")]
        public async Task<IActionResult> setpriority(String id, [FromBody] outbound_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.setpriorityAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode   = valorg;
                o.site      = valsite;
                o.depot     = valdepot;
                o.accnmodify  = valaccn;
                await _sv.setPriority(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.setpriorityAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
 
        [Authorize] [HttpPost("setremarks/{id}")]
        public async Task<IActionResult> setremarks(String id, [FromBody] outbound_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.setremarks", id, Request.getIP(), app, valaccn); }
                o.orgcode   = valorg;
                o.site      = valsite;
                o.depot     = valdepot;
                o.accnmodify  = valaccn;
                await _sv.setremarks(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.setremarks",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        
        [Authorize] [HttpPost("changeRequest/{id}")]
        public async Task<IActionResult> changeRequest(String id, [FromBody] outbound_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
                Process ps ;  SnapsLogDbg p = null; 
                try { 
                    if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.changeRequest", id, Request.getIP(), app, valaccn); }
                    o.orgcode   = valorg;
                    o.site      = valsite;
                    o.depot     = valdepot;
                    o.accnmodify  = valaccn;
                    await _sv.changeRequest(o);
                    return Ok();                                
                }catch (Exception exr) {         
                    _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.changeRequest",rqid:id,ob:o));       
                    return BadRequest(exr.SnapsBadRequest());
                }finally { 
                    if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
                }
        }
    
        [Authorize] [HttpPost("setlineorder/{id}")]
        public async Task<IActionResult> setlineorder(String id, [FromBody] outbouln_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                ) {
                Process ps ;  SnapsLogDbg p = null; 
                try { 
                    if (_log.IsEnabled(LogLevel.Debug)){ ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.setlineorder", id, Request.getIP(), app, valaccn); }
                    o.orgcode       = valorg;
                    o.site          = valsite;
                    o.depot         = valdepot;
                    o.accnmodify    = valaccn;
                    await _sv.setlineorder(o);
                    return Ok();                                
                }catch (Exception exr) {         
                    _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.setlineorder",rqid:id,ob:o));       
                    return BadRequest(exr.SnapsBadRequest());
                }finally { 
                    if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
                }
        }
    }

    //Task (outbouln_md o)

}
