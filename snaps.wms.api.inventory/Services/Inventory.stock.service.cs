using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
namespace Snaps.WMS.Services { 
    public class stockService : IstockService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private stock_ops op;
        public stockService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new stock_ops(_appSettings.Value.Conxstr); 
        }

        public async Task<List<stock_ls>> listProductAsync(stock_pm o){ 
            try     { return await op.findProduct(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<stock_info> getstockInfo(stock_ls o){ 
            try     { return await op.getstockInfo(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<List<stock_md>> getstockLine(string typesel, stock_ls o) { 
            try     { return await op.getstockLine(typesel,o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setstatus(stock_md o){ 
            try     { await op.setstatus(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        
        
        public async Task<List<lov>> getproductratio(string orgcode, string site, string depot, string article, string pv, string lv){ 
            try     { return await op.getproductratio(orgcode, site, depot, article, pv, lv); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        

    }
}