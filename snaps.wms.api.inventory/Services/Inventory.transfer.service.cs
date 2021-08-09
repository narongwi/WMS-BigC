using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
namespace Snaps.WMS.Services { 
    public class transferService : ItransferService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private transfer_ops op;
        public transferService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new transfer_ops(_appSettings.Value.Conxstr); 
        }

        public lov checklocation(string orgcode, string site, string depot, string loccode) { 
            try     { return op.checklocation(orgcode, site, depot, loccode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public void validatelocation(transfer_md o) { 
            try     { op.validatelocation(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<transfer_md> transferstock(transfer_md o)  { 
            try     { return await op.transferstock(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}