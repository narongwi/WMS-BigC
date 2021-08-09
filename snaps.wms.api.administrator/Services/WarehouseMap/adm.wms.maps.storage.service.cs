using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
using Snaps.WMS.warehouse;
namespace Snaps.WMS.Services { 
    public class mapstorageService : ImapstorageService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private locdw_ops opdw;
        private locup_ops opup;
        public mapstorageService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            opdw = new locdw_ops(_appSettings.Value.Conxstr);
            opup = new locup_ops(_appSettings.Value.Conxstr); 
        }

        public async Task<List<locup_md>> lstloczone(locup_pm o){ 
            try     { return await opup.findzone(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<List<locup_md>> lstlocup(locup_pm o){ 
            try     { return await opup.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task upsertlocup(locup_md o) { 
            try     { await opup.upsert(o); } 
            catch   (Exception ex) { throw ex; }  
            finally { }
        }
        public async Task removelocup(locup_md o) { 
            try     { await opup.remove(o); } 
            catch   (Exception ex) { throw ex; }  
            finally { }
        }

        public async Task<List<locdw_ls>> lstlocdw(locdw_pm o){ 
            try     { return await opdw.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<locdw_md> getlocdw(locdw_ls o){ 
            try     { return await opdw.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task upsertlocdw(locdw_md o) { 
            try     { await opdw.upsert(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task removelocdw(locdw_md o){ 
            try     { await opdw.remove(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task generatelocdw(locdw_gn o) { 
            try     { await opdw.generate(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        
        public async Task generatelocgd(locdw_gngrid o) { 
            try     { await opdw.generate(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<List<locdw_pivot>> getpivot(locdw_pm o) { 
            try     { return await opdw.findpivot(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task setpivot(locdw_pivot o,String accncode){ 
            try     { await opdw.setpivot(o,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<List<locdw_picking>> getpicking(locdw_pm o){ 
            try     { return await opdw.findpicking(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task setpicking(locdw_picking o,String accncode){
            try     { await opdw.setpicking(o,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
    }
}