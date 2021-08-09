using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS.THParty;
namespace Snaps.WMS.Services { 
    public class admthpartyService : IadmthpartyService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private thparty_ops op;
        public admthpartyService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new thparty_ops(_appSettings.Value.Conxstr); 
        }
        public async Task<List<thparty_ls>> listAsync(thparty_pm o){ 
            try     { return await op.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<thparty_md> getAsync(thparty_ls o) { 
            try     { return await op.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task upsertAsync(thparty_md o){ 
            try     { await op.upsert(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}