using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IconfigService { 
         Task<List<config_md>> findAsync(config_md rs);
         Task upsertAsync(config_md o);
         Task dropAsync(config_md o);
         Task<config_md> getWebActive(string site,string accscode);
        Task<config_md> getPdaActive(string accscode);
    }
}