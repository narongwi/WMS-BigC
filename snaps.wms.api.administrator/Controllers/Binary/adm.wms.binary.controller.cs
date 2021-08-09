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
using Snaps.WMS.parameter;

namespace Snaps.WMS.Controllers {

    [ApiController] [Route("[controller]")]
    public class binaryController : ControllerBase {
        private IbinaryService _sv;
        private readonly ILogger<binaryController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.adm.Binary";
        public binaryController(ILogger<binaryController> logger,IOptions<AppSettings> optn, IbinaryService osv) { 
            _sv = osv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("desc/{id}")]
        public async Task<IActionResult> descAsync(String id, binary_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"descAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncreate = oaccn; o.accnmodify = oaccn;
                return Ok(await _sv.desc(o));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"descAsync",rqid:id,ob: o));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("list/{id}")]
        public async Task<IActionResult> listAsync(String id,  binary_md o,
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
                //Operate
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncreate = oaccn; o.accnmodify = oaccn;
                return Ok(await _sv.list(o));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"listAsync",rqid:id,ob: o));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("upsert/{id}")]
        public async Task<IActionResult> upsertAsync(String id, binary_md o,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsertAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate
                 o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncreate = oaccn; o.accnmodify = oaccn;
                 await _sv.upsert(o);
                return Ok();                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovroleAsync",rqid:id,ob:o));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }


        [Authorize] [HttpPost("remove/{id}")]
        public async Task<IActionResult> removeAsync(String id, binary_md o,
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
                //Operate
                 o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncreate = oaccn; o.accnmodify = oaccn;
                 await _sv.remove(o);
                return Ok();                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"removeAsync",rqid:id,ob:o));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
    }
}