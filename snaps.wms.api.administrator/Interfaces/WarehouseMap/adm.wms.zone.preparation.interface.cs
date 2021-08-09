using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.warehouse;
using Snaps.Helpers.StringExt;
namespace Snaps.WMS.Interfaces { 
     public interface IzoneprepService { 

        Task<List<zoneprep_md>> list(zoneprep_md o);
        Task upsert(zoneprep_md o);
        Task remove(zoneprep_md o);
        
        Task<List<zoneprln_md>> line(zoneprep_md o);
        Task upsert(zoneprln_md o);
        Task remove(zoneprln_md o);

     }
}