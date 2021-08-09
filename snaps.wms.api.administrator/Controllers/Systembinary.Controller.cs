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
    public class SystemBinaryController : ControllerBase {
        private ISystemBinaryService _sv;
        private readonly ILogger<SystemBinaryController> _log;
        private readonly AppSettings _optn;
        public SystemBinaryController(ILogger<SystemBinaryController> logger,IOptions<AppSettings> optn, ISystemBinaryService oaccnsv) { 
            _sv = oaccnsv; _log = logger; _optn = optn.Value; }

        // [Authorize] [HttpPost("LOV/Warehouse/{id}")]
        // public async Task<IActionResult> LOVWarehouse(String id, 
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "en") {
        //     Process ps ;  SnapsLogDbg p = null;
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVWarehouse(valorg,valsite,valdepot));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"ListAsync",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
        //     }
        // }

        // [Authorize] [HttpPost("LOV/Depot/{id}")]
        // public async Task<IActionResult> LOVDepot(String id, 
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") {
        //     Process ps ;  SnapsLogDbg p = null; 
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVDepot(valorg,valsite,valdepot));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"ListAsync",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
        //     }
        // }

        // [Authorize] [HttpPost("LOV/Role/{id}")]
        // public async Task<IActionResult> LOVRole(String id,         
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") {
        //     Process ps ;  SnapsLogDbg p = null; 
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVRole(valorg,valsite,valdepot));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"ListAsync",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
        //     }
        // }

        // [Authorize] [HttpPost("LOV/State/{bintype}/{bincode}")]
        // public async Task<IActionResult> LOVState(String bintype,String bincode,
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") { 
        //     Process ps ;  SnapsLogDbg p = null; 
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", bintype, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVState(valorg,valsite,valdepot,bintype,bincode));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"ListAsync",rqid:bintype,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
        //     }
        // }
        
        // [Authorize][HttpPost("LOV/Config/{cnftype}/{cnfcode}")]
        // public async Task<IActionResult> LOVConfig(String cnftype,String cnfcode,
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") { 
        //     Process ps ;  SnapsLogDbg p = null;
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)) { 
        //             ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", cnftype, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVConfig(valorg,valsite,valdepot,cnftype,cnfcode));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"ListAsync",rqid:cnftype,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
        //     }
        // }


        // [Authorize][HttpPost("LOV/Locupper/{id}")]
        // public async Task<IActionResult> Locupper(String id,
        //             [FromBody] locup_pm o,
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") { 
        //     Process ps ;  SnapsLogDbg p = null;
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)) { 
        //             ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"Locupper", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVLocupper(o));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"Locupper",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
        //     }
        // }

        // [Authorize][HttpPost("LOV/Loclower/{id}")]
        // public async Task<IActionResult> Loclower(String id, 
        //             [FromBody] locdw_pm o,
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") { 
        //     Process ps ;  SnapsLogDbg p = null; 
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)) { 
        //             ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"Loclower", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVLoclower(o));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"Loclower",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
        //     }
        // }

        // [Authorize][HttpPost("LOV/Locdist/{id}")]
        // public async Task<IActionResult> LOVLocdist(String id, 
        //             [FromBody] locup_pm o,
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") { 
        //     Process ps ;  SnapsLogDbg p = null; 
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)) { 
        //             ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"Loclower", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVLocdist(o));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"Loclower",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
        //     }
        // }




        // //List of handerling unit 
        // [Authorize][HttpPost("LOV/HU/{id}")]
        // public async Task<IActionResult> LovHU(String id, 
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") { 
        //     Process ps ;  SnapsLogDbg p = null; 
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)) { 
        //             ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"LovHU", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVHU(valorg,valsite,valdepot));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"LovHU",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
        //     }
        // }

        // //List of storage zone 
        // [Authorize][HttpPost("LOV/Zone/{id}")]
        // public async Task<IActionResult> LovZone(String id, 
        //             [FromHeader(Name="site")] String valsite,
        //             [FromHeader(Name="depot")] String valdepot,
        //             [FromHeader(Name="accncode")] String valaccn,
        //             [FromHeader(Name="orgcode")] String valorg= "bgc", 
        //             [FromHeader(Name="lang")] String lng = "EN") { 
        //     Process ps ;  SnapsLogDbg p = null; 
        //     try { 
        //         //Log performance
        //         if (_log.IsEnabled(LogLevel.Debug)) { 
        //             ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"LovZone", id, Request.getIP(), app, valaccn);  }  
        //         //Operate    
        //         return Ok(await _sv.LOVZone(valorg,valsite,valdepot));                                
        //     }catch (Exception exr) {         
        //         //Error log
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),valaccn,app,"LovZone",rqid:id,ob:""));
        //         //Bad Request  
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally {
        //         //Dispose object
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
        //     }
        // }
    }
}