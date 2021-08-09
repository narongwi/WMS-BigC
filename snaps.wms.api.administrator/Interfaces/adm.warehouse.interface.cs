using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.warehouse;
namespace Snaps.WMS.Interfaces { 
     public interface IadmwarehouseService { 
         Task<List<warehouse_md>> listAsync(warehouse_pm o); //list
         Task<warehouse_md> getAsync(warehouse_ls o); // get
         Task upsertAsync(warehouse_md o); //upsert
         Task removeAsync(warehouse_md o); //Remove
     }
}