using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.product;

namespace Snaps.WMS.Interfaces { 
     public interface IadmproductService { 
         Task<List<product_ls>> listAsync(product_pm o); //list
        Task<product_active> findActive(product_pm o); //product barcode active
        Task<product_md> getAsync(product_ls o); // get
         Task upsertAsync(product_md o); //upsert
     }
}