using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS.Device;
namespace Snaps.WMS.Services { 
    public class admdeviceService : IadmdeviceService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private device_ops op;
        public admdeviceService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new device_ops(_appSettings.Value.Conxstr); 
        }
        public async Task<List<admdevice_ls>> listAsync(admdevice_pm o){ 
            try     { return await op.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<admdevice_md> getAsync(admdevice_ls o) { 
            try     { return await op.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task upsertAsync(admdevice_md o){ 
            try     { await op.upsert(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}