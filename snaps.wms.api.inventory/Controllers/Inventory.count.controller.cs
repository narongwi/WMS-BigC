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
using Snaps.Helpers.Logger;
using System.Reflection;

namespace Snaps.WMS.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class countController : ControllerBase
    {
        private IcountService _sv;
        private readonly ILogger<countController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.inv.count";
        private readonly ISnapsLogger _snaplog;
        public countController(ILogger<countController> logger, IOptions<AppSettings> optn, IcountService osv)
        {
            _sv = osv; _log = logger;
            _optn = optn.Value;

            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            _snaplog = snapsLogFactory.Create<countController>();
        }

        [Authorize]
        [HttpPost("listTask/{id}")]
        public async Task<IActionResult> listTaskAsync(String id, [FromBody] counttask_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {

                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "listTask", id, Request.getIP(), app, oaccn); }

                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.listTaskAsync(o));
            }
            catch (Exception exr)
            {
                _snaplog.Error(osite, oaccn, id, exr, exr.Message);
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "listTask", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        [Authorize]
        [HttpPost("getTask/{id}")]
        public async Task<IActionResult> getTaskAsync(String id, [FromBody] counttask_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "getTask", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.listTaskAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "getTask", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("upsertTask/{id}")]
        public async Task<IActionResult> upsertTaskAsnc(String id, [FromBody] counttask_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "upsertTask", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                await _sv.upsertTaskAsnc(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "upsertTask", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("removeTask/{id}")]
        public async Task<IActionResult> removeTaskAsync(String id, [FromBody] counttask_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "removeTask", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                await _sv.removeTaskAsync(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "removeTask", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }




        [Authorize]
        [HttpPost("listPlan/{id}")]
        public async Task<IActionResult> listPlanAsync(String id, [FromBody] counttask_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "listPlan", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.listPlanAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "listPlan", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("getPlan/{id}")]
        public async Task<IActionResult> getPlanAsync(String id, [FromBody] countplan_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "getPlan", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.getPlanAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "getPlan", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        [Authorize]
        [HttpPost("upsertPlan/{id}")]
        public async Task<IActionResult> upsertPlanAysnc(String id, [FromBody] countplan_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "upsertPlan", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                await _sv.upsertPlanAysnc(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "upsertPlan", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("removePlan/{id}")]
        public async Task<IActionResult> removePlanAsync(String id, [FromBody] countplan_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "removePlan", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                await _sv.removePlanAsync(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "removePlan", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("validatePlan/{id}")]
        public async Task<IActionResult> validatePlanAsync(String id, [FromBody] countplan_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "validatePlan", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                await _sv.validatePlanAsync(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "validatePlan", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        [Authorize]
        [HttpPost("listLine/{id}")]
        public async Task<IActionResult> listLineAsync(String id, [FromBody] countplan_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "listLine", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.listLineAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "listLine", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("countline/{id}")]
        public async Task<IActionResult> countLineAsync(String id, [FromBody] countplan_md o,
                  [FromHeader(Name = "site")] String osite,
                  [FromHeader(Name = "depot")] String odepot,
                  [FromHeader(Name = "accncode")] String oaccn,
                  [FromHeader(Name = "orgcode")] String oorg,
                  [FromHeader(Name = "lang")] String lng
                  )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "countLine", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.countLineAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "listLine", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("getLine/{id}")]
        public async Task<IActionResult> getLineAsync(String id, [FromBody] findcountline_md o,
                   [FromHeader(Name = "site")] String osite,
                   [FromHeader(Name = "depot")] String odepot,
                   [FromHeader(Name = "accncode")] String oaccn,
                   [FromHeader(Name = "orgcode")] String oorg,
                   [FromHeader(Name = "lang")] String lng
                   )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "listLine", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot;
                return Ok(await _sv.getLineAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "getLine", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("upsertLine/{id}")]
        public async Task<IActionResult> upsertLineAsnc(String id, [FromBody] List<countline_md> o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "upsertLine", id, Request.getIP(), app, oaccn); }
                foreach (countline_md ln in o)
                {
                    ln.orgcode = oorg; ln.site = osite; ln.depot = odepot; ln.accnmodify = oaccn; ln.accncreate = oaccn;
                }

                await _sv.upsertLineAsync(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "upsertLine", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("getConfirmLine/{id}")]
        public async Task<IActionResult> getConfirmLineAsnc(String id, [FromBody] counttask_md o,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "upsertLine", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.getConfrimLineAsync(o));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "upsertLine", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize]
        [HttpPost("countConfirm/{id}")]
        public async Task<IActionResult> countConfirmAsync(String id, [FromBody] counttask_md o,
                  [FromHeader(Name = "site")] String osite,
                  [FromHeader(Name = "depot")] String odepot,
                  [FromHeader(Name = "accncode")] String oaccn,
                  [FromHeader(Name = "orgcode")] String oorg,
                  [FromHeader(Name = "lang")] String lng
                  )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "upsertLine", id, Request.getIP(), app, oaccn); }
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                await _sv.countConfirmAsync(o);
                return Ok();
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), oaccn, app, "upsertLine", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
    }
}