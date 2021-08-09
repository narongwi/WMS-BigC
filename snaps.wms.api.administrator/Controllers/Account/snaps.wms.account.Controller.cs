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
    public class AccountController : ControllerBase {
        private IaccountService _sv;
        private IconfigService _cf;
        private readonly ILogger<AccountController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.admin.account";
        public AccountController(ILogger<AccountController> logger,IOptions<AppSettings> optn, 
                IaccountService oaccnsv, IconfigService ocf ) { 
            _sv = oaccnsv; _log = logger; _optn = optn.Value; _cf = ocf; }

        //[Authorize(Roles = "Admin,Snaps")] [HttpPost("List/{id}")]
        //public async Task<IActionResult> ListAsync(String id, [FromBody] accn_pm o, 
        //            [FromHeader(Name="site")] String site,
        //            [FromHeader(Name="depot")] String depot,
        //            [FromHeader(Name="accncode")] String accncode,
        //            [FromHeader(Name="accscode")] String accscode,
        //            [FromHeader(Name="orgcode")] String orgcode, 
        //            [FromHeader(Name="lang")] String lng
        //            ) {
        //    Process ps ;  SnapsLogDbg p = null; 
        //    try { 
        //        if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync", id, Request.getIP(), app, accncode); }
        //        o.orgcode = orgcode;
        //        o.site = site;
        //        o.depot = depot;
        //        return Ok(await _sv.findAsync(o));                                
        //    }catch (Exception exr) {         
        //        _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ListAsync",rqid:id,ob:o));       
        //        return BadRequest(exr.SnapsBadRequest());
        //    }finally { 
        //        if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
        //    }
        //}
        [Authorize(Roles = "Admin,Snaps")]
        [HttpPost("List/{id}")]
        public async Task<IActionResult> ListAsync(String id,[FromBody] accn_pm o,
            [FromHeader(Name = "site")] String site,
            [FromHeader(Name = "depot")] String depot,
            [FromHeader(Name = "accncode")] String accncode,
            [FromHeader(Name = "accscode")] String accscode,
            [FromHeader(Name = "orgcode")] String orgcode,
            [FromHeader(Name = "lang")] String lng
            ) {
            Process ps; SnapsLogDbg p = null;
            try {
                if(_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ListAsync",id,Request.getIP(),app,accncode); }
                o.orgcode = orgcode;
                return Ok(await _sv.findAsync(o));
            } catch(Exception exr) {
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ListAsync",rqid: id,ob: o));
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize(Roles = "Admin,Snaps")] [HttpPost("get/{id}")]
        public async Task<IActionResult> getAsync(String id, [FromBody] accn_pm o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getAsync", id, Request.getIP(), app, accncode); }
                o.orgcode = orgcode;
                o.site = site;
                o.depot = depot;
                return Ok(await _sv.getProfileAsync(o.orgcode,o.site,o.depot,o.accncode));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ListAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
         [Authorize(Roles = "Admin,Snaps")] [HttpPost("mod/{id}")]
        public async Task<IActionResult> modAsync(String id, [FromBody] accn_pm o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getAsync", id, Request.getIP(), app, accncode); }
                o.orgcode = orgcode;
                o.site = site;
                o.depot = depot;
                return Ok(await _sv.getModifyAsync(o.orgcode,o.accncode));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ListAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize(Roles = "Admin,Snaps")]
        [HttpPost("addcfg/{id}")]
        public async Task<IActionResult> addCfgAsync(String id,[FromBody] accn_cfg cfg,
                   [FromHeader(Name = "site")] String site,
                   [FromHeader(Name = "depot")] String depot,
                   [FromHeader(Name = "accncode")] String accncode,
                   [FromHeader(Name = "accscode")] String accscode,
                   [FromHeader(Name = "orgcode")] String orgcode,
                   [FromHeader(Name = "lang")] String lng
                   ) {
            Process ps; SnapsLogDbg p = null;
            try {
                if(_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsertAsync",id,Request.getIP(),app,accncode); }
                if(await _sv.vldSession(accscode) == false) {
                    return Unauthorized("Security passcode has expire, please login again");
                } else {
                    await _sv.addCfgAsync(cfg,accncode);
                    return Ok();
                }
            } catch(Exception exr) {
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"upsertAsync",rqid: id,ob: cfg));
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize(Roles = "Admin,Snaps")]
        [HttpPost("delcfg/{id}")]
        public async Task<IActionResult> delCfgAsync(String id,[FromBody] accn_cfg cfg,
                [FromHeader(Name = "site")] String site,
                [FromHeader(Name = "depot")] String depot,
                [FromHeader(Name = "accncode")] String accncode,
                [FromHeader(Name = "accscode")] String accscode,
                [FromHeader(Name = "orgcode")] String orgcode,
                [FromHeader(Name = "lang")] String lng
                ) {
            Process ps; SnapsLogDbg p = null;
            try {
                if(_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsertAsync",id,Request.getIP(),app,accncode); }
                if(await _sv.vldSession(accscode) == false) {
                    return Unauthorized("Security passcode has expire, please login again");
                } else {
                    await _sv.delCfgAsync(cfg,accscode);
                    return Ok();
                }
            } catch(Exception exr) {
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"upsertAsync",rqid: id,ob: cfg));
                return BadRequest(exr.SnapsBadRequest());
            } finally {
                if(_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize(Roles = "Admin,Snaps")] [HttpPost("upsert/{id}")]
        public async Task<IActionResult> upsertAsync(String id, [FromBody] accn_md o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"upsertAsync", id, Request.getIP(), app, accncode); }
                if (await _sv.vldSession(accscode) == false) { 
                    return Unauthorized("Security passcode has expire, please login again");
                }else { 
                    o.orgcode = orgcode;
                    o.site = site; o.depot = depot;
                    o.accnmodify = accncode; 
                    await _sv.upsertAsync(o);
                    return Ok();
                }                      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"upsertAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize] [HttpPost("changePriv/{id}")]
        public async Task<IActionResult> changePrivAsync(String id, [FromBody] accn_priv o, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps ;  SnapsLogDbg p = null; 
            try {
                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "changePrivAsync", id, Request.getIP(), app, accncode); }
                if (await _sv.vldSession(accscode) == false)
                {
                    return Unauthorized("Security passcode has expire, please login again");
                }
                else
                {
                    o.orgcode = orgcode;
                    o.site = site;
                    o.depot = depot;
                    o.accncode = accncode;
                    await _sv.changePrivAsync(o);
                    return Ok();
                }
            }
            catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"changePrivAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("resetPriv/{id}")]
        public async Task<IActionResult> resetPrivAsync(String id, [FromBody] accn_md o,
                    [FromHeader(Name = "site")] String site,
                    [FromHeader(Name = "depot")] String depot,
                    [FromHeader(Name = "accncode")] String accncode,
                    [FromHeader(Name = "accscode")] String accscode,
                    [FromHeader(Name = "orgcode")] String orgcode,
                    [FromHeader(Name = "lang")] String lng
                    )
        {
            Process ps; SnapsLogDbg p = null;
            try
            {

                if (_log.IsEnabled(LogLevel.Debug)) { ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "resetPrivAsync", id, Request.getIP(), app, accncode); }
                if (await _sv.vldSession(accscode) == false)
                {
                    return Unauthorized("Security passcode has expire, please login again");
                }
                else
                {
                    var ap = new accn_priv();
                    ap.orgcode = orgcode;
                    ap.site = site;
                    ap.depot = depot;
                    ap.accncode = o.accncode;
                    await _sv.resetPrivAsync(ap, accncode);
                    return Ok();
                }
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), accncode, app, "resetPrivAsync", rqid: id, ob: o));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug))
                {
                    p.snap();
                    _log.LogDebug(p.toJson()); p.Dispose();
                }
            }
        }
        [Authorize][HttpPost("getProfile/{selsite}/{id}")]
        public async Task<IActionResult> getActiveprofile(string selsite,string id, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng)  {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getProfile", id, Request.getIP(), app, accncode); 
                }
                if (await _sv.vldSession(accscode) == false) { 
                    return Unauthorized("Security passcode has expire, please login again");
                }else {                     
                  return Ok(await _cf.getWebActive(selsite,accscode));                               
                }                      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"getProfile",rqid:id,ob:""));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }
        [Authorize]
        [HttpPost("getPdaProfile/{id}")]
        public async Task<IActionResult> getPdaprofile(string id,
                   [FromHeader(Name = "site")] String site,
                   [FromHeader(Name = "depot")] String depot,
                   [FromHeader(Name = "accncode")] String accncode,
                   [FromHeader(Name = "accscode")] String accscode,
                   [FromHeader(Name = "orgcode")] String orgcode,
                   [FromHeader(Name = "lang")] String lng)
        {
            Process ps; SnapsLogDbg p = null;
            try
            {
                if (_log.IsEnabled(LogLevel.Debug))
                {
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps, "getProfile", id, Request.getIP(), app, accncode);
                }
                if (await _sv.vldSession(accscode) == false)
                {
                    return Unauthorized("Security passcode has expire, please login again");
                }
                else
                {
                    return Ok(await _cf.getPdaActive(accscode));
                }
            }
            catch (Exception exr)
            {
                _log.LogError(exr.SnapsLogExc(Request.getIP(), accncode, app, "getProfile", rqid: id, ob: ""));
                return BadRequest(exr.SnapsBadRequest());
            }
            finally
            {
                if (_log.IsEnabled(LogLevel.Debug)) { p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [Authorize][HttpPost("myAccount/{id}")]
        public async Task<IActionResult> getMyaccount(string id, 
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng)  {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  
                    ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"getProfile", id, Request.getIP(), app, accncode); 
                }                
                return Ok(await _sv.getProfileAsync(orgcode,site,depot,accncode));  
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"getProfile",rqid:id,ob:""));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        [AllowAnonymous] [HttpPost("forgot/{id}")]
        public async Task<IActionResult> forgotAsync(String id, accn_signup o) {
            Process ps ;  SnapsLogDbg p = null; 
            
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"forgotAsync", id, Request.getIP(), app, o.accncode); }
                await _sv.forgotAsync(o);
                return Ok();
      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),o.accncode,app,"forgotAsync",rqid:id,ob:o));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [AllowAnonymous] [HttpPost("Recovery/{id}")]
        public async Task<IActionResult> valdRecoveryAsync(String id, string tkreqid) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"valdRecoveryAsync", id, Request.getIP(), app, tkreqid); }
                await _sv.valdRecoveryAsync(tkreqid);
                return Ok();
      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),tkreqid,app,"valdRecoveryAsync",rqid:id,ob:tkreqid));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [AllowAnonymous] [HttpPost("validateAccount/{id}")]
        public ActionResult vldAccountAsync(String id, string accncode) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"vldAccountAsync", id, Request.getIP(), app, accncode); }
                _sv.vldAccountAsync(accncode);
                return Ok();      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"vldAccountAsync",rqid:id,ob:accncode));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [AllowAnonymous] [HttpPost("validateEmail/{id}")]
        public ActionResult vldEmailAsync(String id, string email) {
            Process ps ;  SnapsLogDbg p = null; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"vldEmailAsync", id, Request.getIP(), app, email); }
                _sv.vldEmailAsync(email);
                return Ok();      
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),email,app,"vldEmailAsync",rqid:id,ob:email));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }



    }
}