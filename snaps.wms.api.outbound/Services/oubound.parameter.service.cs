using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
namespace Snaps.WMS.Services { 
    public class ouboundparameterService : IoutboundparameterService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private parameter_ops op;
        public ouboundparameterService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new parameter_ops(_appSettings.Value.Conxstr); 
        }
        public List<pam_set> getOutbound(string orgcode, string site, string depot)
        {
            try { return op.getOutbound(orgcode, site, depot); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
    }
}