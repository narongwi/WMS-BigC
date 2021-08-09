using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
namespace Snaps.WMS.Services { 
    public class taskService : ItaskService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private task_ops op;
        public taskService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new task_ops(_appSettings.Value.Conxstr); 
        }

        public async Task<List<task_ls>> listAsync(task_pm o){ 
            try     { return await op.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<task_md> getAsync(task_ls o) { 
            try     { return await op.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task upsertAsync(task_md o){ 
            try     { await op.upsert(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task assignAsync(task_md o) { 
            try     { await op.assignTask(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task startAsync(task_md o) { 
            try     { await op.startTask(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task fillAsync(task_md o){ 
            try     { await op.fillTask(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task cancelAsync(task_md o){ 
            try     { await op.cancelTask(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task collectAsync(task_md o){
            try     { await op.collectTask(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task confirmAsync(task_md o) { 
            try     { await op.confirmTask(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

    }
}