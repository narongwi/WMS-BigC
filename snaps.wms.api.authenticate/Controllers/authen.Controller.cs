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
    [ApiController] [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IauthenService _sv;
        private readonly ILogger<AuthController> _log;
        private readonly AppSettings _optn;

        private readonly String app = "api.authen";
        public AuthController(ILogger<AuthController> logger,IOptions<AppSettings> optn, IauthenService oauthsv) {  _sv = oauthsv; _log = logger; _optn = optn.Value; }

        [AllowAnonymous] [HttpPost("verify/{id}")] 
        public async Task<IActionResult> verify(string id,[FromBody] accn_signup model)
        {
            Process ps ;  SnapsLogDbg p = null; //String err = "";
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor; accn_authen rn = new accn_authen(); string rne ="";
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"verify", id, Request.getIP(), app, model.accncode );  } 
                rn = await _sv.Authenticate(model);
                if (rn.valcode.IndexOf("Auth") == -1) { 
                    return BadRequest(rn.valcode);
                }else {                    
                    
                    var key = Encoding.ASCII.GetBytes(_optn.Secret);
                        tokenDescriptor = new SecurityTokenDescriptor {
                        Subject = new ClaimsIdentity(
                            new Claim[] { 
                                new Claim(ClaimTypes.Name, rn.account.accnname),
                                new Claim(ClaimTypes.Email, rn.account.email),
                                new Claim(ClaimTypes.Sid, rn.account.accncode), 
                                new Claim(ClaimTypes.Hash, rn.valcode),
                                new Claim(ClaimTypes.UserData, rn.account.accnavartar),
                                new Claim(ClaimTypes.Role, rn.account.accntype)
                            }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                        Audience = "snapsListener",
                        //Audience = "bigcListener",
                        Issuer = "snapsAuthen",                        
                    };
                    rne = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
                    return Ok(new resultRequest(resultState.Ok,rne,rn.valcode));
                }                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),model.accncode,app,"verify",rqid:id,ob:model));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
                tokenDescriptor = null;
                tokenDescriptor = null;
            }
        }
        [AllowAnonymous][HttpPost("validateAccount/{id}")]
        public IActionResult validateAccount(string id,[FromBody] accn_signup model)
        {
            Process ps ;  SnapsLogDbg p = null; String rsl = "";
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"validateAccount", id, Request.getIP(), app, model.accncode);  }      
                rsl = _sv.validateAccount(model);
                if (rsl != "") { 
                    return BadRequest(rsl);
                }else { 
                    return Ok("".getResultOk());
                }                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),model.accncode,app,"validateAccount",rqid:id,ob:model));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [AllowAnonymous][HttpPost("validateEmail/{id}")]
        public IActionResult validateEmail(String id,[FromBody] accn_signup model) {
            Process ps ;  SnapsLogDbg p = null; String rsl = "";
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"validateEmail", id, Request.getIP(), app, model.accncode);  }      
                rsl = _sv.validateEmail(model);
                if (rsl != "") { 
                    return BadRequest(rsl);
                }else { 
                    return Ok("".getResultOk());
                }                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),model.accncode,app,"validateEmail",rqid:id,ob:model));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }

        [AllowAnonymous][HttpPost("signup/{id}")]
        public async Task<IActionResult> signup(String id,[FromBody] accn_signup model){
            Process ps ;  SnapsLogDbg p = null; String rsl = "";
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"signup", id, Request.getIP(), app, model.accncode);  }      
                rsl = await _sv.signupAsync(model);
                if (rsl != "") { 
                    return BadRequest(rsl);
                }else { 
                    return Ok("".getResultOk());
                }                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),model.accncode,app,"signup",rqid:id,ob:model));       
                return BadRequest(exr);
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
            }
        }


        // [AllowAnonymous][HttpPost("forgot/{id}")]
        // public async Task<IActionResult> forgot(String id, [FromBody] accn_signup model) { 
        //     Process ps ;  SnapsLogDbg p = null; String rsl = "";
        //     try { 
        //         if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"forgot", id, Request.getIP(), app, model.accncode);  }      
        //         await _sv.forgot(model);
        //         if (rsl != "Ok") { 
        //             return BadRequest(_sv.getMessage(model.lang, rsl, resultState.Error,""));
        //         }else { 
        //             return Ok("".getResultOk());
        //         }                
        //     }catch (Exception exr) {         
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),model.accncode,app,"forgot",rqid:id,ob:model));       
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally { 
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
        //     }
        // }

        // [AllowAnonymous][HttpPost("recovery/{id}")]
        // public async Task<IActionResult> recovery(String id, [FromBody] accn_signup model) { 
        //     Process ps ;  SnapsLogDbg p = null; String rsl = "";
        //     try { 
        //         if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"recovery", id, Request.getIP(), app, model.accncode);  }      
        //         rsl = await _sv.recovery(model);
        //         if (rsl != "Ok") { 
        //             return BadRequest(_sv.getMessage(model.lang, rsl, resultState.Error,""));
        //         }else { 
        //             return Ok("".getResultOk());
        //         }                
        //     }catch (Exception exr) {         
        //         _log.LogError(exr.SnapsLogExc(Request.getIP(),model.accncode,app,"recovery",rqid:id,ob:model));       
        //         return BadRequest(exr.SnapsBadRequest());
        //     }finally { 
        //         if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); }
        //     }
        // }

    }
}