using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS.warehouse;
namespace Snaps.WMS.Services { 
    public class admwarehouseService : IadmwarehouseService { 
        private string cnx;
        public admwarehouseService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr; 
        }
        public async Task<List<warehouse_md>> listAsync(warehouse_pm o){ 
            warehouse_ops op = new warehouse_ops(cnx); 
            try { 
                return await op.find(o);
            }catch (Exception ex) { 
                throw ex;
            }finally { op.Dispose(); }
        }
        public async Task<warehouse_md> getAsync(warehouse_ls o) { 
            warehouse_ops op = new warehouse_ops(cnx); 
            try { 
                return await op.get(o); 
            }catch (Exception ex) { 
                throw ex;
            }finally { op.Dispose(); }
        }
        public async Task upsertAsync(warehouse_md o){ 
            warehouse_ops op = new warehouse_ops(cnx); 
            try { 
                await op.upsert(o); 
            }catch (Exception ex) { 
                throw ex;
            }finally { op.Dispose(); }
        }

        public async Task removeAsync(warehouse_md o) { 
            warehouse_ops op = new warehouse_ops(cnx); 
            try { 
                await op.remove(o);
            }catch (Exception ex) { 
                throw ex;
            }finally { op.Dispose(); }
        }
    }
}