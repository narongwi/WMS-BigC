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
    public class parameterController : ControllerBase {
        private readonly ILogger<parameterController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.Inbound.parameter";
        private readonly IinboundparameterService _sv;
        public parameterController(ILogger<parameterController> logger,IOptions<AppSettings> optn, IinboundparameterService isv) { 
            _sv = isv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("get/{id}")]
        public async Task<IActionResult> get(String id, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getparameter", id, Request.getIP(), app, valaccn); }
                return Ok(await _sv.getInbound(valorg,valsite,valdepot));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"getparameter",rqid:id,ob:""));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

    }
}