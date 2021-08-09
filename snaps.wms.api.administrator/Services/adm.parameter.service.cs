using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;

namespace Snaps.WMS.Services { 
    public class admparameterService : IadmparameterService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private parameter_ops op;
        public admparameterService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new parameter_ops(_appSettings.Value.Conxstr); 
        }
        public async Task<List<pam_parameter>> getParameterListAsync(String orgcode, String site, String depot, String pmmodule,String type ) { 
            try     { return await op.getParameterListAsync(orgcode,site, depot, pmmodule,type); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task updateParameterAsync(pam_parameter o) { 
            try     { await op.updateParameterAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}