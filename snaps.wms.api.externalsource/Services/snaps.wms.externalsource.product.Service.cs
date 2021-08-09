using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class exsProductService : IexsProductService { 
        private string cnx;
        public exsProductService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;        
        }

        public async Task<List<exsFile>> findAsync(exsFile o) {
            exSource_ops eo = new exSource_ops(cnx);
            try { return await eo.findAsync(o);
            }catch (Exception ex){ throw ex; 
            }finally { eo.Dispose(); }
        }
        public async Task<List<exsProduct>> lineProductAsync(exsFile o) { 
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.lineProductAsync(o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<string> ImpexsProductAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsProduct> o){ 
            DateTimeOffset impstart = DateTimeOffset.Now;
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.ImpexsProductAsync(orgcode,site,depot,accncode,filetype, filename,filelength,decstart,o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }


    }
}