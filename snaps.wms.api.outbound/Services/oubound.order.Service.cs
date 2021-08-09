using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
namespace Snaps.WMS.Services { 
    public class ouborderService : IouborderService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private outbound_ops op;
        public ouborderService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new outbound_ops(_appSettings.Value.Conxstr); 
        }

        public async Task<List<outbound_ls>> listAsync(outbound_pm o){ 
            try     { return await op.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<List<outbound_ls>> listdistAsync(outbound_pm o){ 
            try     { return await op.finddist(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<outbound_md> getAsync(outbound_ls o) { 
            try     { return await op.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<outbound_md> getdistAsync(outbound_ls o) { 
            try     { return await op.getdist(o); }
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task setPriority(outbound_md o){ 
            try     {  await op.setPriority(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task setremarks(outbound_md o){ 
            try     {  await op.setremarks(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        } 
        public async Task changeRequest(outbound_md o){ 
            try     {  await op.changeRequest(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task setlineorder(outbouln_md o) {
            try     {  await op.setlineorder(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}