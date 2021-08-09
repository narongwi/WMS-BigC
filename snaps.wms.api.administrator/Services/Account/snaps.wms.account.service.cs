using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class accountService : IaccountService { 
        private string cnx;
        public accountService(IOptions<AppSettings> appSettings) {
            cnx = appSettings.Value.Conxstr;        
        }

        public async Task<List<accn_ls>> findAsync(accn_pm rs){ 
            accn_ops ao = new accn_ops(cnx);
            try     { return await ao.findAsync(rs); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task<Boolean> vldSession(string accscode) { 
            accn_ops ao = new accn_ops(cnx);
            try     { return await ao.vldSession(accscode); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task upsertAsync(accn_md o) { 
            accn_ops ao = new accn_ops(cnx);
            try     { await ao.upsertAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task forgotAsync(accn_signup o) { 
            accn_ops ao = new accn_ops(cnx);
            try     { await ao.forgotAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

        public async Task<accn_md> getProfileAsync(string orgcode, string site, string depot, string accncode) { 
            accn_ops ao = new accn_ops(cnx);
            try     { return await ao.getProfile(orgcode,site,depot,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task<accn_md> getModifyAsync(string orgcode,string accncode) {
            accn_ops ao = new accn_ops(cnx);
            try { return await ao.getModify(orgcode,accncode); } catch(Exception ex) { throw ex; } finally { ao.Dispose(); }
        }
        public async Task addCfgAsync(accn_cfg cfg,string accnmodify) {
            accn_ops ao = new accn_ops(cnx);
            try { await ao.addCfgAsync(cfg,accnmodify); } catch(Exception ex) { throw ex; } finally { ao.Dispose(); }
        }
        public async Task delCfgAsync(accn_cfg cfg,string accnmodify) {
            accn_ops ao = new accn_ops(cnx);
            try { await ao.delCfgAsync(cfg,accnmodify); } catch(Exception ex) { throw ex; } finally { ao.Dispose(); }
        }
        public async Task valdRecoveryAsync(string tkreqid){ 
            accn_ops ao = new accn_ops(cnx);
            try     { await ao.valdRecoveryAsync(tkreqid); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task changePrivAsync(accn_priv o){ 
            accn_ops ao = new accn_ops(cnx);
            try     { await ao.changePrivAsync(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public async Task resetPrivAsync(accn_priv o,string usermodify)
        {
            accn_ops ao = new accn_ops(cnx);
            try { await ao.resetPrivAsync(o, usermodify); }
            catch (Exception ex) { throw ex; }
            finally { ao.Dispose(); }
        }
        public string vldAccountAsync(string accncode){ 
            accn_ops ao = new accn_ops(cnx);
            try     { return ao.vldAccount(accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }
        public void vldEmailAsync(string email){ 
            accn_ops ao = new accn_ops(cnx);
            try     { ao.vldEmail(email); } 
            catch   (Exception ex) { throw ex; } 
            finally { ao.Dispose(); }
        }

    }
}