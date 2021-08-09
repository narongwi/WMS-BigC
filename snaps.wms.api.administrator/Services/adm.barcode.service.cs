using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS.Barcode;
namespace Snaps.WMS.Services { 
    public class admbarcodeService : IadmbarcodeService { 
        private string cnx = "";
        public admbarcodeService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;
        }
        public async Task<List<barcode_ls>> listAsync(barcode_pm o){ 
            barcode_ops bo = new barcode_ops(cnx);
            try     { return await bo.find(o); }
            catch   (Exception ex){  throw ex; }
            finally { bo.Dispose(); }
        }
        public async Task<barcode_md> getAsync(barcode_ls o) { 
            barcode_ops bo = new barcode_ops(cnx);
            try     { return await bo.get(o); }
            catch   (Exception ex){ throw ex; }
            finally { bo.Dispose(); }
        }
        public async Task upsertAsync(barcode_md o){ 
            barcode_ops bo = new barcode_ops(cnx);
            try     { await bo.upsert(o); }
            catch   (Exception ex){ throw ex; }
            finally { bo.Dispose(); }
        }

        public async Task setPrimary(barcode_ls o, string accncode){ 
            barcode_ops bo = new barcode_ops(cnx);
            try     { await bo.setPrimary(o,accncode); }
            catch   (Exception ex){ throw ex; }
            finally { bo.Dispose(); }
        }
    }
}