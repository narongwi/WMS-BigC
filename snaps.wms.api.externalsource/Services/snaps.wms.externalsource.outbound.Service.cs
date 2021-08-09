using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class exsOutboundService : IexsOutboundService { 
        private string cnx;
        public exsOutboundService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;        
        }

        public async Task<List<exsFile>> findAsync(exsFile o) {
            exSource_ops eo = new exSource_ops(cnx);
            try { return await eo.findAsync(o);
            }catch (Exception ex){ throw ex; 
            }finally { eo.Dispose(); }
        }
        public async Task<List<exsOutbound>> lineOutboundAsync(exsFile o) { 
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.lineOutboundAsync(o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<string> ImpexsOutboundAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsOutbound> o){ 
            DateTimeOffset impstart = DateTimeOffset.Now;
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.ImpexsOutboundAsync(orgcode,site,depot,accncode,filetype, filename,filelength,decstart,o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<List<exsOutbouln>> lineOutboulnAsync(exsFile o) { 
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.lineOutboulnAsync(o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<string> ImpexsOutboulnAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsOutbouln> o){ 
            DateTimeOffset impstart = DateTimeOffset.Now;
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.ImpexsOutboulnAsync(orgcode,site,depot,accncode,filetype, filename,filelength,decstart,o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
    }
}