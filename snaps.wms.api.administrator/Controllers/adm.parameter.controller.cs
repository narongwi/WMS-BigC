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
    public class admparameterController : ControllerBase {
        private IadmparameterService _sv;
        private readonly ILogger<admparameterController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.Administrator.admparameter";
        public admparameterController(ILogger<admparameterController> logger,IOptions<AppSettings> optn, IadmparameterService oaccnsv) { 
            _sv = oaccnsv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("getParameterList/{id}")]
        public async Task<IActionResult> getParameterList(String id,
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getParameterList", id, Request.getIP(), app, valaccn); }               
                return Ok(await _sv.getParameterListAsync(valorg, valsite,valdepot,"",""));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"getParameterList",rqid:id,ob:""));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        [Authorize] [HttpPost("getParameter/{pmmodule}/{pmtype}/{id}")]
        public async Task<IActionResult> getParameter(String id,string pmmodule,string pmtype,
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getParameterList", id, Request.getIP(), app, valaccn); }               
                return Ok(await _sv.getParameterListAsync(valorg, valsite,valdepot,pmmodule,pmtype));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"getParameterList",rqid:id,ob:""));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("updateParameter/{id}")]
        public async Task<IActionResult> updateParameterAsync(String id, [FromBody] pam_parameter o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"updateParameter", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg; o.site = valsite; o.depot = valdepot; o.accncreate = valaccn; o.accnmodify = valaccn;
                await _sv.updateParameterAsync(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"updateParameter",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


    }
}
