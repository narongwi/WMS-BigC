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
using Snaps.WMS.preparation;

namespace Snaps.WMS.Controllers {

    [ApiController] [Route("[controller]")]
    public class ouprepController : ControllerBase {
        private IouprepService _sv;
        private readonly ILogger<ouprepController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.inv.prep";
        public ouprepController(ILogger<ouprepController> logger,IOptions<AppSettings> optn, IouprepService oaccnsv) { 
            _sv = oaccnsv; _log = logger; _optn = optn.Value; }

        [Authorize] [HttpPost("list/{id}")]
        public async Task<IActionResult> listPrepAsync(String id, [FromBody] prep_pm o, 
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
        public async Task<IActionResult> getAsync(String id, [FromBody] prep_ls o, 
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

        [Authorize] [HttpPost("setpriority/{id}")]
        public async Task<IActionResult> setpriority(String id, [FromBody] prep_md o, 
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
                o.accncreate = valaccn;
                o.procmodify = "oub.preparation.setpriority";
                await _sv.setPriority(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.setpriorityAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        


        [Authorize] [HttpPost("setStart/{id}")]
        public async Task<IActionResult> setStart(String id, [FromBody] prep_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.setStart", id, Request.getIP(), app, valaccn); }
                o.orgcode   = valorg;
                o.site      = valsite;
                o.depot     = valdepot;
                o.accnmodify  = valaccn;
                await _sv.setStart(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.setStart",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("setEnd/{id}")]
        public async Task<IActionResult> setEnd(String id, [FromBody] prep_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.setEnd", id, Request.getIP(), app, valaccn); }
                o.orgcode   = valorg;
                o.site      = valsite;
                o.depot     = valdepot;
                o.accnmodify  = valaccn;
                await _sv.setEnd(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.setEnd",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("opsPick/{id}")]
        public async Task<IActionResult> opsPick(String id, [FromBody] prln_md[] o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.opsPick", id, Request.getIP(), app, valaccn); }

                foreach (prln_md pl in o)
                {
                    pl.orgcode = valorg;
                    pl.site = valsite;
                    pl.depot = valdepot;
                    pl.accnmodify = valaccn;
                    await _sv.opsPick(pl);
                }
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.opsPick",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("pdaPick/{id}")]
        public async Task<IActionResult> opsPick(String id, [FromBody] prln_md o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.order.opsPick", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.accnmodify = valaccn;
                await _sv.opsPick(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.order.opsPick", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("opsPut/{id}")]
        public async Task<IActionResult> opsPut(String id, [FromBody] prln_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.opsPut", id, Request.getIP(), app, valaccn); }
                o.orgcode   = valorg;
                o.site      = valsite;
                o.depot     = valdepot;
                o.accnmodify  = valaccn;
                await _sv.opsPut(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.opsPut",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("opsCancel/{id}")]
        public async Task<IActionResult> opsCancel(String id, [FromBody] prep_md o, 
                    [FromHeader(Name="site")] String valsite,
                    [FromHeader(Name="depot")] String valdepot,
                    [FromHeader(Name="accncode")] String valaccn,
                    [FromHeader(Name="orgcode")] String valorg= "bgc", 
                    [FromHeader(Name="lang")] String lng = "EN"
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.opsCancel", id, Request.getIP(), app, valaccn); }
                o.orgcode   = valorg;
                o.site      = valsite;
                o.depot     = valdepot;
                o.accnmodify  = valaccn;
                await _sv.opsCancel(o);
                return Ok();                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.opsCancel",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("opsSelect/{id}")]
        public async Task<IActionResult> opsSelect(String id,[FromBody] ouselect o,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg = "bgc",
                    [FromHeader(Name = "lang")] String lng = "EN"
                    ) {
            Process ps; SnapsLogDbg p = null;
            try {
                if(_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.opsCancel",id,Request.getIP(),app,valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.selectaccn = valaccn;
                await _sv.opsSelect(o);
                return Ok();
            } catch(Exception exr) {
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.opsSelect",rqid: id,ob: o));
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("opsUnselect/{id}")]
        public async Task<IActionResult> opsUnselect(String id,[FromBody] ouselect o,
                   [FromHeader(Name = "site")] String valsite,
                   [FromHeader(Name = "depot")] String valdepot,
                   [FromHeader(Name = "accncode")] String valaccn,
                   [FromHeader(Name = "orgcode")] String valorg = "bgc",
                   [FromHeader(Name = "lang")] String lng = "EN"
                   ) {
            Process ps; SnapsLogDbg p = null;
            try {
                if(_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"api.oub.order.opsUnSelect",id,Request.getIP(),app,valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.selectaccn = valaccn;
                await _sv.opsUnselect(o);
                return Ok();
            } catch(Exception exr) {
                _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"api.oub.order.opsUnSelect",rqid: id,ob: o));
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        //Task<prepset> procsetup(string orgcode,string site, string depot, string accn, List<outbound_ls> o);
        [Authorize]
        [HttpPost("procsetup/{id}")]
        public async Task<IActionResult> procsetup(String id, [FromBody] prepset o,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg,
                    [FromHeader(Name = "lang")] String lng 
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.prep.procsetup", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg; o.site = valsite; o.depot = valdepot; o.accncreate = valaccn;
                return Ok(await _sv.procsetup(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.prep.procsetup", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("procstock/{setno}/{id}")]
        public async Task<IActionResult> procstock(String id, String setno,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg = "bgc",
                    [FromHeader(Name = "lang")] String lng = "EN"
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.order.procstock", id, Request.getIP(), app, valaccn); }
                return Ok(await _sv.procstock(valorg, valsite,valdepot,"ST",setno,valaccn));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.order.procstock", rqid: id, ob: setno));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        // Task<prepset> (prepset o); 
        [Authorize]
        [HttpPost("distsetup/{id}")]
        public async Task<IActionResult> distsetup(String id, [FromBody] prepset o,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg,
                    [FromHeader(Name = "lang")] String lng 
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.prep.distsetup", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg; o.site = valsite; o.depot = valdepot; o.accncreate = valaccn;
                return Ok(await _sv.distsetup(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.prep.distsetup", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        [Authorize]
        [HttpPost("procdistb/{setno}/{id}")]
        public async Task<IActionResult> procdistb(String id, String setno,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg = "bgc",
                    [FromHeader(Name = "lang")] String lng = "EN"
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.order.procdistb", id, Request.getIP(), app, valaccn); }
                return Ok(await _sv.procdistb(valorg, valsite,valdepot,"XD",setno,valaccn));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.order.procdistb", rqid: id, ob: setno));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        // [Authorize]
        // [HttpPost("procdistb/{id}")]
        // public async Task<IActionResult> procdistb(String id, [FromBody] List<outbound_ls> o,
        //             [FromHeader(Name = "site")] String valsite,
        //             [FromHeader(Name = "depot")] String valdepot,
        //             [FromHeader(Name = "accncode")] String valaccn,
        //             [FromHeader(Name = "orgcode")] String valorg = "bgc",
        //             [FromHeader(Name = "lang")] String lng = "EN"
        //             )
        // {
        //     Process ps; SnapsLogDbg p = null;
        //     try
        //     {
        //         if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.order.procdistb", id, Request.getIP(), app, valaccn); }
        //         o.ForEach(c => {
        //             c.orgcode = valorg; c.depot = valdepot; c.site = valsite;
        //         });
        //         await _sv.procdistb(o);
        //         return Ok();
        //     }
        //     catch (Exception exr)
        //     {
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.order.procdistb", rqid: id, ob: o));
        //         return BadRequest(exr.SnapsBadRequest());
        //     }
        //     finally
        //     {
        //         if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
        //     }
        // }






    }
}