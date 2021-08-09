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


namespace Snaps.WMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ouhanderlingunitController : ControllerBase
    {
        private IouhanderlingunitService _sv;
        private readonly ILogger<ouhanderlingunitController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.inv.prep";
        public ouhanderlingunitController(ILogger<ouhanderlingunitController> logger, IOptions<AppSettings> optn, IouhanderlingunitService oaccnsv)
        {
            _sv = oaccnsv; _log = logger; _optn = optn.Value;
        }

        [Authorize]
        [HttpPost("list/{id}")]
        public async Task<IActionResult> listAsync(String id, [FromBody] handerlingunit o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.listAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                return Ok(await _sv.listAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.listAsync", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        [Authorize]
        [HttpPost("get/{id}")]
        public async Task<IActionResult> getAsync(String id, [FromBody] handerlingunit o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.getAsync", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                return Ok(await _sv.getAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.getAsync", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("upsert/{id}")]
        public async Task<IActionResult> upsert(String id, [FromBody] handerlingunit o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.upsert", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                await _sv.upsert(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.upsert", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Task close(handerlingunit o)
        [Authorize]
        [HttpPost("close/{id}")]
        public async Task<IActionResult> close(String id, [FromBody] handerlingunit o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.close", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                await _sv.close(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.close", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("genereate/{id}")]
        public async Task<IActionResult> generate(String id, [FromBody] handerlingunit_gen o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.generate", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.accncreate = valaccn;
                o.accnmodify = valaccn;
                o.procmodfiy = "handerlingunit.generate";
                await _sv.generate(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.generate", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        // Task<List<lov>> getmaster(String orgcode, String site, String depot);
        [Authorize]
        [HttpPost("getmaster/{id}")]
        public async Task<IActionResult> getmaster(String id, [FromBody] handerlingunit_gen o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.getmaster", id, Request.getIP(), app, valaccn); }
                return Ok(await _sv.getmaster(valorg, valsite, valdepot));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.getmaster", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        // Task<List<handerlingunit_item>> lines(handerlingunit o)
        [Authorize]
        [HttpPost("lines/{id}")]
        public async Task<IActionResult> lines(String id, [FromBody] handerlingunit o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.lines", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.accnmodify = valaccn;
                return Ok(await _sv.lines(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.lines", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        //Task<List<handerlingunit_item>> linesnonsum(handerlingunit o);
        [Authorize]
        [HttpPost("linesnonsum/{id}")]
        public async Task<IActionResult> linesnonsum(String id, [FromBody] handerlingunit o,
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
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "api.oub.handerlingunit.linesnonsum", id, Request.getIP(), app, valaccn); }
                o.orgcode = valorg;
                o.site = valsite;
                o.depot = valdepot;
                o.accnmodify = valaccn;
                return Ok(await _sv.linesnonsum(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "api.oub.handerlingunit.lines", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
    }

}
