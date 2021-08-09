using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.warehouse;
namespace Snaps.WMS.Interfaces { 
     public interface IadmdepotService { 
         Task<List<depot_md>> listAsync(depot_md o); //list
         Task upsertAsync(depot_md o); //upsert
     }
}