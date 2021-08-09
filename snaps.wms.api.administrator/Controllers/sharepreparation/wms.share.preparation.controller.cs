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
    public class shareprepController : ControllerBase {
        private IshareprepService _sv;
        private readonly ILogger<shareprepController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.shareprep";
        public shareprepController(ILogger<shareprepController> logger,IOptions<AppSettings> optn, IshareprepService osv) { 
            _sv = osv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("list/{id}")]
        public async Task<IActionResult> listAsync(String id, [FromBody] shareprep_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"listAsync", id, Request.getIP(), app, oaccn); 
                } 
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;
                //Operate    
                return Ok(await _sv.listAsync(o));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"listAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("get/{id}")]
        public async Task<IActionResult> getAsync(String id, [FromBody] shareprep_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getAsync", id, Request.getIP(), app, oaccn); 
                } 
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;

                //Operate    
                return Ok(await _sv.getAsync(o));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"getAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("upsert/{id}")]
        public async Task<IActionResult> upsertAsnc(String id, [FromBody] shareprep_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsertAsnc", id, Request.getIP(), app, oaccn); 
                } 
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;

                //Operate
                await _sv.upsertAsnc(o);
                return Ok();                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"upsertAsnc",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("remove/{id}")]
        public async Task<IActionResult> removeAsync(String id, [FromBody] shareprep_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"removeAsync", id, Request.getIP(), app, oaccn); 
                } 
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;

                //Operate
                await _sv.removeAsync(o);
                return Ok();                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"removeAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }


        [Authorize] [HttpPost("upline/{id}")]
        public async Task<IActionResult> uplineAsync(String id, [FromBody] shareprln_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"uplineAsync", id, Request.getIP(), app, oaccn); 
                } 
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;

                //Operate
                await _sv.uplineAsync(o);
                return Ok();                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"uplineAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("rmline/{id}")]
        public async Task<IActionResult> rmlineAsync(String id, [FromBody] shareprln_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"rmlineAsync", id, Request.getIP(), app, oaccn); 
                } 
                o.orgcode = oorg;
                o.site = osite;
                o.depot = odepot;
                o.accncreate = oaccn;
                o.accnmodify = oaccn;

                //Operate
                await _sv.rmlineAsync(o);
                return Ok();                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"rmlineAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }


    }
}
    