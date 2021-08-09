using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class exsLocdwService : IexsLocdwService { 
        private string cnx;
        public exsLocdwService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;        
        }

        public async Task<List<exsFile>> findAsync(exsFile o) {
            exSource_ops eo = new exSource_ops(cnx);
            try { return await eo.findAsync(o);
            }catch (Exception ex){ throw ex; 
            }finally { eo.Dispose(); }
        }
        public async Task<List<exsLocdw>> lineLocdwAsync(exsFile o) { 
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.lineLocdwAsync(o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<string> ImpexsLocdwAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsLocdw> o){ 
            DateTimeOffset impstart = DateTimeOffset.Now;
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.ImpexsLocdwAsync(orgcode,site,depot,accncode,filetype, filename,filelength,decstart,o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }


    }
}