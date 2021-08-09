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
using Snaps.WMS.warehouse;

namespace Snaps.WMS.Controllers {

    [ApiController] [Route("[controller]")]
    public class zoneprepController : ControllerBase {
        private IzoneprepService _sv;
        private readonly ILogger<zoneprepController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.adm.zone.preparation";
        public zoneprepController(ILogger<zoneprepController> logger,IOptions<AppSettings> optn, IzoneprepService omapsv) { 
            _sv = omapsv; _log = logger; _optn = optn.Value; }

        //Zone prep header
        [Authorize] [HttpPost("list/{id}")]
        public async Task<IActionResult> listAsync(String id, [FromBody] zoneprep_md o, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg,
                    [FromHeader(Name="lang")] String olng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){ ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"list", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;
                return Ok(await _sv.list(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"list",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("ops/upsert/{id}")]
        public async Task<IActionResult> upsertAsync(String id, [FromBody] zoneprep_md o, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg,
                    [FromHeader(Name="lang")] String olng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){ ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsert", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;
                await _sv.upsert(o);
                return Ok();                              
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"upsert",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("ops/remove/{id}")]
        public async Task<IActionResult> removeAsync(String id, [FromBody] zoneprep_md o, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){ ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"remove", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;
                await _sv.remove(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"remove",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Zone prep line 
        [Authorize] [HttpPost("line/{id}")]
        public async Task<IActionResult> lineAsync(String id, [FromBody] zoneprep_md o, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){ ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"list", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;
                return Ok(await _sv.line(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"list",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("ops/upsertline/{id}")]
        public async Task<IActionResult> upsertlnAsync(String id, [FromBody] zoneprln_md o, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){ ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsert", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;
                await _sv.upsert(o);
                return Ok();                              
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"upsert",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("ops/removeline/{id}")]
        public async Task<IActionResult> removelnAsync(String id, [FromBody] zoneprln_md o, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){ ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"remove", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;
                await _sv.remove(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"remove",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


    }
}
