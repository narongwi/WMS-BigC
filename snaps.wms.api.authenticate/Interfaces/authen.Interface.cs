using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IauthenService { 
          Task<accn_authen> Authenticate(accn_signup oc); //Authenticate
          string validateAccount(accn_signup oc); // validate account
          string validateEmail(accn_signup oc) ; // validate email
          Task<String> signupAsync(accn_signup oc); // signup account
          Task forgotAsync(accn_signup oc); // forgot account
          Task recoveryAsync(string oc); // receovery

    } 
} 
