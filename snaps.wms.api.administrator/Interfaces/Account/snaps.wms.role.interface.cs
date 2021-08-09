using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IroleService { 
         Task<List<role_md>> findAsync(role_md rs);
         Task<role_md> getAsync(role_md o);
         Task upsertAsync(role_md o);
         Task dropAsync(role_md o);
         Task<role_md> getMasterAsync(string orgcode, string site, string depot, string accncreate);
     }
}