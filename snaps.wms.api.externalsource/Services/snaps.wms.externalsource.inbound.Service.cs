using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class exsInboundService : IexsInboundService { 
        private string cnx;
        public exsInboundService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;        
        }

        public async Task<List<exsFile>> findAsync(exsFile o) {
            exSource_ops eo = new exSource_ops(cnx);
            try { return await eo.findAsync(o);
            }catch (Exception ex){ throw ex; 
            }finally { eo.Dispose(); }
        }
        public async Task<List<exsInbound>> lineInboundAsync(exsFile o) { 
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.lineInboundAsync(o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<string> ImpexsInboundAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsInbound> o){ 
            DateTimeOffset impstart = DateTimeOffset.Now;
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.ImpexsInboundAsync(orgcode,site,depot,accncode,filetype, filename,filelength,decstart,o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }

        public async Task<List<exsInbouln>> lineInboulnAsync(exsFile o) { 
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.lineInboulnAsync(o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }
        public async Task<string> ImpexsInboulnAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart, List<exsInbouln> o){ 
            DateTimeOffset impstart = DateTimeOffset.Now;
            exSource_ops eo = new exSource_ops(cnx);
            try { 
                return await eo.ImpexsInboulnAsync(orgcode,site,depot,accncode,filetype, filename,filelength,decstart,o);
            }catch (Exception ex){ throw ex; }
            finally { eo.Dispose(); }
        }


    }
}