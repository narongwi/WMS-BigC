using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
using Snaps.WMS.warehouse;
namespace Snaps.WMS.Services { 
    public class zoneprepService : IzoneprepService { 
        private readonly string cnx ;
        public zoneprepService(IOptions<AppSettings> appSettings) { cnx = appSettings.Value.Conxstr;  }

        public async Task<List<zoneprep_md>> list(zoneprep_md o){ 
            zoneprep_ops ops = new zoneprep_ops(cnx);
            try     { return await ops.listAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task upsert(zoneprep_md o) { 
            zoneprep_ops ops = new zoneprep_ops(cnx);
            try     { await ops.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task remove(zoneprep_md o) { 
            zoneprep_ops ops = new zoneprep_ops(cnx);
            try     { await ops.removeAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task<List<zoneprln_md>> line(zoneprep_md o) {
            zoneprep_ops ops = new zoneprep_ops(cnx);
            try     { return await ops.lineAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task upsert(zoneprln_md o) {
            zoneprep_ops ops = new zoneprep_ops(cnx);
            try     { await ops.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task remove(zoneprln_md o){ 
            zoneprep_ops ops = new zoneprep_ops(cnx);
            try     { await ops.removeAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
    }
}


