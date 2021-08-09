using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IRoleService { 
         Task<List<role_md>> listAsync(role_md o); //list
         Task<role_md> getAsync(role_md o); // getrole_pm
         Task upsertAsync(role_md o); //upsert
 
     }
}