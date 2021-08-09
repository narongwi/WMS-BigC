using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.Barcode;

namespace Snaps.WMS.Interfaces { 
     public interface IadmbarcodeService { 
        Task<List<barcode_ls>> listAsync(barcode_pm o); //list
        Task<barcode_md> getAsync(barcode_ls o); // get
        Task upsertAsync(barcode_md o); //upsert
        Task setPrimary(barcode_ls o, string accncode); // set primary
     }
}