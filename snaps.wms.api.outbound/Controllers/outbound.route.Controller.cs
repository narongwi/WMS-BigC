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
    public class ourouteController : ControllerBase {
        private IourouteService _sv;
        private readonly ILogger<ourouteController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.oub.route";
        public ourouteController(ILogger<ourouteController> logger,IOptions<AppSettings> optn, IourouteService oaccnsv) { 
            _sv = oaccnsv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("thsum/{id}")]
        public async Task<IActionResult> thsumAsync(String id, [FromBody] route_pm o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"thsumAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                return Ok(await _sv.thsum(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"thsumAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("list/{id}")]
        public async Task<IActionResult> listAsync(String id, [FromBody] route_pm o, 
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


        [Authorize] [HttpPost("get/{id}")]
        public async Task<IActionResult> getAsync(String id, [FromBody] route_ls o, 
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
                o.site = valsite;
                o.depot = valdepot;
                return Ok(await _sv.getAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.getAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("upsert/{id}")]
        public async Task<IActionResult> upsert(String id, [FromBody] route_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.upsert", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                await _sv.upsert(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.upsert",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Task allocate(route_md o)
        [Authorize] [HttpPost("allocate/{id}")]
        public async Task<IActionResult> allocate(String id, [FromBody] route_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.allocate", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.accnmodify = valaccn;
                await _sv.allocate(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.allocate",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Task shipment(route_md o) ; 
        [Authorize] [HttpPost("shipment/{id}")]
        public async Task<IActionResult> shipment(String id, [FromBody] route_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.allocate", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.accnmodify = valaccn;
                await _sv.shipment(o,valaccn);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.allocate",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Task huload(string orgcode, string site, string depot, route_hu o)
        [Authorize] [HttpPost("huload/{id}")]
        public async Task<IActionResult> huload(String id, [FromBody] route_hu o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.huload", id, Request.getIP(), app, valaccn); }
                o.accnmodify = valaccn;
                await _sv.huload(valorg, valsite, valdepot, o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.huload",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //public async Task<List<lov>> getstaging(String orgcode, String site,String depot){ 
        [Authorize] [HttpPost("getstaging/{id}")]
        public async Task<IActionResult> getstaging(String id, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.route.getstaging", id, Request.getIP(), app, valaccn); }
                return Ok(await _sv.getstaging(valorg,valsite,valdepot));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.route.getstaging",rqid:id,ob:""));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Task<List<lov>> getthirdparty(String orgcode, String site, String depot)
        [Authorize]
        [HttpPost("getthirdparty/{id}")]
        public async Task<IActionResult> getthirdparty(String id,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg = "bgc",
                    [FromHeader(Name = "lang")] String lng = "EN"
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.route.getthirdparty", id, Request.getIP(), app, valaccn); }
                return Ok(await _sv.getthirdparty(valorg, valsite, valdepot));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.route.getthirdparty", rqid: id, ob: ""));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Task<List<lov>> getTransporter(String orgcode, String site, String depot)
        [Authorize]
        [HttpPost("gettransporter/{id}")]
        public async Task<IActionResult> gettransporter(String id,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg = "bgc",
                    [FromHeader(Name = "lang")] String lng = "EN"
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.route.gettransporter", id, Request.getIP(), app, valaccn); }
            
                return Ok(await _sv.getTransporter(valorg, valsite, valdepot));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.route.gettransporter", rqid: id, ob: ""));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("remove/{id}")]
        public async Task<IActionResult> remove(String id, [FromBody] route_md o, 
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg = "bgc",
                    [FromHeader(Name = "lang")] String lng = "EN"
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.route.remove", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.accnmodify = valaccn;
                await _sv.remove(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.route.remove", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

    }

}