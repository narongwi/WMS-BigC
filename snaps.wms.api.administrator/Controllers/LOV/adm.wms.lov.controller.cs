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
    public class LOVController : ControllerBase {
        private ILOVService _sv;
        private readonly ILogger<LOVController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.adm.LOV";
        public LOVController(ILogger<LOVController> logger,IOptions<AppSettings> optn, ILOVService osv) { 
            _sv = osv; _log = logger; _optn = optn.Value; }

        [AllowAnonymous]
        [HttpPost("allwarehouse/{id}")]
        public async Task<IActionResult> lovwarehouseAsync(String id) {
            Process ps; SnapsLogDbg p = null;
            try {
                //Log performance
                if(_log.IsEnabled(LogLevel.Debug)) {
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovwarehouseAsync",id,Request.getIP(),app,"");
                }
                //Operate    
                return Ok(await _sv.lovwarehouseAsync());
            } catch(Exception exr) {
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),"",app,"lovwarehouseAsync",rqid: id,ob: ""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                //Dispose object
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("Warehouse/{id}")]
        public async Task<IActionResult> lovwarehouseAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovwarehouseAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovwarehouseAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovwarehouseAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("Depot/{id}")]
        public async Task<IActionResult> lovdepotAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovdepotAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovdepotAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovdepotAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize]
        [HttpPost("Role/{id}")]
        public async Task<IActionResult> lovroleAsync(String id,
                    [FromHeader(Name = "site")] String osite,
                    [FromHeader(Name = "depot")] String odepot,
                    [FromHeader(Name = "accncode")] String oaccn,
                    [FromHeader(Name = "orgcode")] String oorg,
                    [FromHeader(Name = "lang")] String olng) {
            Process ps; SnapsLogDbg p = null;
            try {
                //Log performance
                if(_log.IsEnabled(LogLevel.Debug)) {
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovroleAsync",id,Request.getIP(),app,oaccn);
                }
                //Operate    
                return Ok(await _sv.lovroleAsync(oorg,osite,odepot));
            } catch(Exception exr) {
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovroleAsync",rqid: id,ob: ""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                //Dispose object
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("SelRole/{rolesite}/{roledepot}/{id}")]
        public async Task<IActionResult> lovroleAsync(string rolesite,string roledepot, String id,
            [FromHeader(Name = "site")] String osite,
            [FromHeader(Name = "depot")] String odepot,
            [FromHeader(Name = "accncode")] String oaccn,
            [FromHeader(Name = "orgcode")] String oorg,
            [FromHeader(Name = "lang")] String olng) {
            Process ps; SnapsLogDbg p = null;
            try {
                //Log performance
                if(_log.IsEnabled(LogLevel.Debug)) {
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovroleAsync",id,Request.getIP(),app,oaccn);
                }
                //Operate    
                return Ok(await _sv.lovroleAsync(oorg,rolesite,roledepot));
            } catch(Exception exr) {
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovroleAsync",rqid: id,ob: ""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                //Dispose object
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("State/{bntype}/{bncode}/{id}")]
        public async Task<IActionResult> lovAsync(String id, string bntype,string bncode,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovAsync(oorg,osite,odepot,bntype,bncode));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }


        [Authorize] [HttpPost("Master/{bntype}/{bncode}/{id}")]
        public async Task<IActionResult> lovMasterAsync(String id, string bntype,string bncode,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovMasterAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovAsync(oorg,osite,odepot,bntype,bncode,false));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovMasterAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }


        [Authorize] [HttpPost("State/{bntype}/{bncode}")]
        public async Task<IActionResult> lovAsyncold(String id, string bntype,string bncode,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovAsyncold", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovAsync(oorg,osite,odepot,bntype,bncode));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovAsyncold",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("Prepzonestock/{id}")]
        public async Task<IActionResult> lovprepzonestockAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovprepzonestockAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovprepzonestockAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovprepzonestockAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }


        [Authorize] [HttpPost("Prepzonedist/{id}")]
        public async Task<IActionResult> lovprepzonedistAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovprepzonedistAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovprepzonedistAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovprepzonedistAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("Sharedist/{id}")]
        public async Task<IActionResult> lovsharedistAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovsharedistAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovsharedistAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovsharedistAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("Storagezone/{id}")]
        public async Task<IActionResult> lovstoragezoneAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovstoragezoneAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovstoragezoneAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovstoragezoneAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("HU/{id}")]
        public async Task<IActionResult> lovhuAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovhuAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovhuAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovhuAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }


        //Zone 
 
        [Authorize] [HttpPost("WMP/{id}")]
        public async Task<IActionResult> lovzoneAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovzoneAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovzoneAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovzoneAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        //Asile 
        [Authorize] [HttpPost("WMP/{ozone}/{id}")]
        public async Task<IActionResult> lovaisleAsync(String id, string ozone,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovaisleAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovaisleAsync(oorg,osite,odepot,ozone));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovaisleAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        //Bay 
        [Authorize] [HttpPost("WMP/{ozone}/{oaisle}/{id}")]
        public async Task<IActionResult> lovbayAsync(String id, string ozone, string oaisle,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovbayAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovbayAsync(oorg,osite,odepot,ozone,oaisle));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovbayAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        //Level 
        [Authorize] [HttpPost("WMP/{ozone}/{oaisle}/{obay}/{id}")]
        public async Task<IActionResult> lovlevelAsync(String id, string ozone, string oaisle, string obay,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovlevelAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovlevelAsync(oorg,osite,odepot, ozone, oaisle, obay));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovlevelAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        //Location 
        [Authorize] [HttpPost("WMP/{zone}/{aisle}/{bay}/{level}/{id}")]
        public async Task<IActionResult> lovlocationAsync(String id,  string ozone, string oaisle, string obay, string olevel,
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovlocationAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovlocationAsync(oorg,osite,odepot, ozone, oaisle, obay, olevel));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovlocationAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        //Staing 
        [Authorize] [HttpPost("staginginb/{id}")]
        public async Task<IActionResult> lovstaginginbAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovstaginginbAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovstaginginbAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovstaginginbAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
        //Staing 
        [Authorize] [HttpPost("stagingoub/{id}")]
        public async Task<IActionResult> lovstagingoubAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovstagingoubAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovstagingoubAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovstagingoubAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
        //Bulk
        [Authorize] [HttpPost("bulk/{id}")]
        public async Task<IActionResult> lovbulkAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovbulkAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovbulkAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovbulkAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
        //Damage
        [Authorize] [HttpPost("damage/{id}")]
        public async Task<IActionResult> lovdamageAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovdamageAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovdamageAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovdamageAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
        //Sinbin
        [Authorize] [HttpPost("sinbin/{id}")]
        public async Task<IActionResult> lovsinbinAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovsinbinAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovsinbinAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovsinbinAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
        //Exchange 
        [Authorize] [HttpPost("exchange/{id}")]
        public async Task<IActionResult> lovexchangeAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovexchangeAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovexchangeAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovexchangeAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
        //Overflow 
        [Authorize] [HttpPost("overflow/{id}")]
        public async Task<IActionResult> lovoverflowAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovoverflowAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovoverflowAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovoverflowAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }
        //Return 
        [Authorize] [HttpPost("return/{id}")]
        public async Task<IActionResult> lovreturnAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lovreturnAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(await _sv.lovreturnAsync(oorg,osite,odepot));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"lovreturnAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        //Validate location 
        [Authorize] [HttpPost("valaisle/{id}")]
        public ActionResult valaisleAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); 
                    p = new SnapsLogDbg(ps,"valaisleAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(_sv.valaisle(oorg,osite,odepot,id));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"valaisleAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("valbay/{id}")]
        public ActionResult valbayAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); 
                    p = new SnapsLogDbg(ps,"valbayAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(_sv.valbay(oorg,osite,odepot,id));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"valbayAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("vallevel/{id}")]
        public ActionResult vallevelAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); 
                    p = new SnapsLogDbg(ps,"vallevelAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(_sv.vallevel(oorg,osite,odepot,id));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"vallevelAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

        [Authorize] [HttpPost("vallocation/{id}")]
        public ActionResult vallocationAsync(String id, 
                    [FromHeader(Name="site")] String osite,
                    [FromHeader(Name="depot")] String odepot,
                    [FromHeader(Name="accncode")] String oaccn,
                    [FromHeader(Name="orgcode")] String oorg, 
                    [FromHeader(Name="lang")] String olng) {
            Process ps ;  SnapsLogDbg p = null;
            try { 
                //Log performance
                if (_log.IsEnabled(LogLevel.Debug)){ 
                    ps = Process.GetCurrentProcess(); 
                    p = new SnapsLogDbg(ps,"vallocationAsync", id, Request.getIP(), app, oaccn); 
                }  
                //Operate    
                return Ok(_sv.vallevel(oorg,osite,odepot,id));                                
            }catch (Exception exr) {         
                //Error log
                _log.LogError(exr.SnapsLogExc(Request.getIP(),oaccn,app,"vallocationAsync",rqid:id,ob:""));
                //Bad Request  
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                //Dispose object
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); } 
            }
        }

    }
}