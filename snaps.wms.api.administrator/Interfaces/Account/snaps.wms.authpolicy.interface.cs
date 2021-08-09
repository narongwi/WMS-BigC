using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IpolicyService { 
         Task<List<policy_md>> findAsync(policy_md rs);
         Task upsertAsync(policy_md o);
         Task dropAsync(policy_md o);
     }
}