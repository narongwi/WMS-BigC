using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class shareprepService : IshareprepService { 
        private string cnx;
        public shareprepService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;
        }

        public Task<List<shareprep_md>> listAsync(shareprep_md o){ 
            shareprep_ops ops = new shareprep_ops(cnx);
            try     { 
                return ops.listAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose();}
        }
        public async Task<shareprep_md> getAsync(shareprep_md o){ 
            shareprep_ops ops = new shareprep_ops(cnx);
            try     {  return await ops.getAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose();}
        }
        public async Task upsertAsnc(shareprep_md o) { 
            shareprep_ops ops = new shareprep_ops(cnx);
            try     { await ops.upsertAsnc(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task removeAsync(shareprep_md o) { 
            shareprep_ops ops = new shareprep_ops(cnx);
            try     { await ops.removeAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task uplineAsync(shareprln_md o) { 
            shareprep_ops ops = new shareprep_ops(cnx);
            try     { await ops.uplineAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task rmlineAsync(shareprln_md o) { 
            shareprep_ops ops = new shareprep_ops(cnx);
            try     { await ops.rmlineAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
    }
}