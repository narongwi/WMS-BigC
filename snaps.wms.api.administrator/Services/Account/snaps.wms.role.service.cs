using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class roleService : IroleService { 
        private string cnx;
        public roleService(IOptions<AppSettings> aps) {  cnx = aps.Value.Conxstr; }
        public async Task<List<role_md>> findAsync(role_md rs){ 
            role_ops ao = new role_ops(cnx);
            try     { return await ao.findAsync(rs); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

        public async Task<role_md> getAsync(role_md rs) { 
            role_ops ao = new role_ops(cnx);
            try     { return await ao.getAsync(rs); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task upsertAsync(role_md o) { 
            role_ops ao = new role_ops(cnx);
            try     { await ao.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

        public async Task dropAsync(role_md o) { 
            role_ops ao = new role_ops(cnx);
            try     { await ao.dropAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

        public async Task<role_md> getMasterAsync(string orgcode, string site, string depot, string accncreate) { 
            role_ops ao = new role_ops(cnx);
            try     { return await ao.getMasterAsync(orgcode, site, depot, accncreate); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }            
        }

    }
}