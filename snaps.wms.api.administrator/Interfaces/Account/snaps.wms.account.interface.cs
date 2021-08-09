using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces
{
    public interface IaccountService
    {
        Task<List<accn_ls>> findAsync(accn_pm rs);
        Task<Boolean> vldSession(string accscode);
        Task upsertAsync(accn_md o);
        Task forgotAsync(accn_signup o);
        Task valdRecoveryAsync(string tkreqid);
        Task changePrivAsync(accn_priv o);
        Task resetPrivAsync(accn_priv o, string usermodify);
        string vldAccountAsync(string accncode);
        void vldEmailAsync(string email);
        Task<accn_md> getProfileAsync(string orgcode, string site, string depot, string accncode);
        Task<accn_md> getModifyAsync(string orgcode,string accncode);
        public Task addCfgAsync(accn_cfg cfg,string accnmodify);
        public Task delCfgAsync(accn_cfg cfg,string accnmodify);
    }
}