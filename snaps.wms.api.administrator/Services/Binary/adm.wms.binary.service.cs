using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
using Snaps.WMS.warehouse;
using Snaps.WMS.parameter;

namespace Snaps.WMS.Services { 
    public class binaryService : IbinaryService { 
        private readonly string cnx ;
        public binaryService(IOptions<AppSettings> appSettings) { cnx = appSettings.Value.Conxstr;  }

        public async Task<List<binary_md>> desc(binary_md o){ 
            binary_ops ops = new binary_ops(cnx);
            try     { return await ops.descAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task<List<binary_md>> list(binary_md o){ 
            binary_ops ops = new binary_ops(cnx);
            try     { return await ops.listAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task upsert(binary_md o) { 
            binary_ops ops = new binary_ops(cnx);
            try     { await ops.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task remove(binary_md o) { 
            binary_ops ops = new binary_ops(cnx);
            try     { await ops.removeAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
    }
}


