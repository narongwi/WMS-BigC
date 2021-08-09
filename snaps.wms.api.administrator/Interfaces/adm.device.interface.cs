using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.Device;

namespace Snaps.WMS.Interfaces { 
     public interface IadmdeviceService { 
         Task<List<admdevice_ls>> listAsync(admdevice_pm o); //list
         Task<admdevice_md> getAsync(admdevice_ls o); // get
         Task upsertAsync(admdevice_md o); //upsert
     }
}