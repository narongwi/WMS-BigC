using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.THParty;

namespace Snaps.WMS.Interfaces { 
     public interface IadmthpartyService { 
         Task<List<thparty_ls>> listAsync(thparty_pm o); //list
         Task<thparty_md> getAsync(thparty_ls o); // get
         Task upsertAsync(thparty_md o); //upsert
     }
}