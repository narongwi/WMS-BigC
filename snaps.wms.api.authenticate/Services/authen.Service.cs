using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
     public class authenService : IauthenService { 
          private readonly string cnx;
          public authenService(IOptions<AppSettings> asg) { this.cnx = asg.Value.Conxstr; }

          public async Task<accn_authen> Authenticate(accn_signup oc)  { //authenticate permission
               accn_ops ao = new accn_ops(cnx);
               try     { return await ao.authen(oc);} 
               catch   (Exception ex) { throw ex; } 
               finally { ao.Dispose(); }
          }
          public string validateAccount(accn_signup oc)  { //validate account code
               accn_ops ao = new accn_ops(cnx);
               try     { return ao.vldAccount(oc.accncode);} 
               catch   (Exception ex) { throw ex; } 
               finally { ao.Dispose(); }
          }
          public string validateEmail(accn_signup oc)  { //validate email
               accn_ops ao = new accn_ops(cnx);
               try     { return ao.vldEmail(oc.email);} 
               catch   (Exception ex) { throw ex; } 
               finally { ao.Dispose(); }
          }
          public async Task<String> signupAsync(accn_signup oc)  { //confirm signup
               accn_ops ao = new accn_ops(cnx);
               try     { return await ao.signupAsync(oc);} 
               catch   (Exception ex) { throw ex; } 
               finally { ao.Dispose(); }
          }

          public async Task forgotAsync(accn_signup oc)  { //confirm signup
               accn_ops ao = new accn_ops(cnx);
               try     { await ao.forgotAsync(oc);} 
               catch   (Exception ex) { throw ex; } 
               finally { ao.Dispose(); }
          }

          public async Task recoveryAsync(string oc)  { //confirm signup
               accn_ops ao = new accn_ops(cnx);
               try     { await ao.valdRecoveryAsync(oc);} 
               catch   (Exception ex) { throw ex; } 
               finally { ao.Dispose(); }
          }

     }
}