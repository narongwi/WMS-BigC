using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Snaps.Helpers;
using Snaps.Helpers.Logging;
using Snaps.WMS.Controllers;
using Snaps.WMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Snaps.Helpers.Json;
namespace Snaps.WMS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ouparameterController : ControllerBase
    {
        private IoutboundparameterService _sv;
        private readonly ILogger<ouborderController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.inv.ouborder";
        public ouparameterController(ILogger<ouborderController> logger, IOptions<AppSettings> optn, IoutboundparameterService oaccnsv)
        {
            _sv = oaccnsv; _log = logger; _optn = optn.Value;
        }

        [Authorize]
        [HttpPost("get/{id}")]
        public IActionResult get(String id,
                    [FromHeader(Name = "site")] String valsite,
                    [FromHeader(Name = "depot")] String valdepot,
                    [FromHeader(Name = "accncode")] String valaccn,
                    [FromHeader(Name = "orgcode")] String valorg,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "getparameter", id, Request.getIP(), app, valaccn); }
                return Ok( _sv.getOutbound(valorg, valsite, valdepot));
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), valaccn, app, "getparameter", rqid: id, ob: ""));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

    }
}
