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
    public class admdepotController : ControllerBase {
        private IadmdepotService _sv;
        private readonly ILogger<admdepotController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.Administrator.admdepot";
        public admdepotController(ILogger<admdepotController> logger,IOptions<AppSettings> optn, IadmdepotService oaccnsv) { 
            _sv = oaccnsv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("List/{id}")]
        public async Task<IActionResult> ListAsync(String id, [FromBody] depot_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.sitecode = valsite;
                //o.depot = valdepot;
                return Ok(await _sv.listAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"ListAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("upsert/{id}")]
        public async Task<IActionResult> upsertAsync(String id, [FromBody] depot_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                //o.sitecode = valsite;
                //o.depot = valdepot;
                o.accncreate = valaccn;
                o.accnmodify = valaccn;
                o.procmodify = "api.admdepot.upsert";
                await _sv.upsertAsync(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"ListAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        

    }
}
