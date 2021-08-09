using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS.product;
namespace Snaps.WMS.Services { 
    public class admproductService : IadmproductService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private product_ops op;
        public admproductService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new product_ops(_appSettings.Value.Conxstr); 
        }


        public async Task<List<product_ls>> listAsync(product_pm o){ 
            try     { return await op.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<product_active> findActive(product_pm o)
        {
            try { return await op.findActive(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task<product_md> getAsync(product_ls o) { 
            try     { return await op.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task upsertAsync(product_md o){ 
            try     { await op.upsert(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}