using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class policyService : IpolicyService { 
        private string cnx;
        public policyService(IOptions<AppSettings> aps) {  cnx = aps.Value.Conxstr; }
        public async Task<List<policy_md>> findAsync(policy_md rs){ 
            policy_ops ao = new policy_ops(cnx);
            try     { return await ao.findAsync(rs); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task upsertAsync(policy_md o) { 
            policy_ops ao = new policy_ops(cnx);
            try     { await ao.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

        public async Task dropAsync(policy_md o) { 
            policy_ops ao = new policy_ops(cnx);
            try     { await ao.dropAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

    }
}