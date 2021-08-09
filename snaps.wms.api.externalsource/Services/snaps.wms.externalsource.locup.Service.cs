using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class exsLocupService : IexsLocupService { 
        private string cnx;
        public exsLocupService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;        
        }

        public async Task<List<exsFile>> findAsync(exsFile o) {
            exSource_ops eo = new exSource_ops(cnx);
            try { return await eo.findAsync(o);
            }catch (Exception ex){ throw ex; 
            }finally { eo.Dispose(); }
        }
        public async Task<List<exsLocup>> lineLocupAsync(exsFile o) { 
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.lineLocupAsync(o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<string> ImpexsLocupAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsLocup> o){ 
            DateTimeOffset impstart = DateTimeOffset.Now;
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.ImpexsLocupAsync(orgcode,site,depot,accncode,filetype, filename,filelength,decstart,o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }


    }
}