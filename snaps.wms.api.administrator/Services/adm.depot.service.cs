using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS.warehouse;
namespace Snaps.WMS.Services { 
    public class admdepotService : IadmdepotService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private depot_ops op;
        public admdepotService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new depot_ops(_appSettings.Value.Conxstr); 
        }
        public async Task<List<depot_md>> listAsync(depot_md o){ 
            try     { return await op.findAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task upsertAsync(depot_md o){ 
            try     { await op.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}