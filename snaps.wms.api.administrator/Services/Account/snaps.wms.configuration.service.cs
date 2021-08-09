using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class configService : IconfigService { 
        private string cnx;
        public configService(IOptions<AppSettings> aps) {  cnx = aps.Value.Conxstr; }
        public async Task<List<config_md>> findAsync(config_md rs){ 
            config_ops ao = new config_ops(cnx);
            try     { return await ao.findAsync(rs); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task upsertAsync(config_md o) { 
            config_ops ao = new config_ops(cnx);
            try     { await ao.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

        public async Task dropAsync(config_md o) { 
            config_ops ao = new config_ops(cnx);
            try     { await ao.dropAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

        public async Task<config_md> getWebActive(string site,string accscode){ 
            config_ops ao = new config_ops(cnx);
            try     { return await ao.getWebActive(site,accscode); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task<config_md> getPdaActive(string accscode) {
            config_ops ao = new config_ops(cnx);
            try { return await ao.getPdaActive(accscode); } catch(Exception ex) { throw ex; } finally { ao.Dispose(); }
        }
        public async Task<config_md> getPdaProfile(string accscode)
        {
            config_ops ao = new config_ops(cnx);
            try { return await ao.getPdaActive(accscode); }
            catch (Exception ex) { throw ex; }
            finally { ao.Dispose(); }
        }

    }
}