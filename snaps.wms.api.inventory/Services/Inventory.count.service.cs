using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Services { 
    public class countService : IcountService { 
        private readonly string cnx = "";
        public countService(IOptions<AppSettings> appSettings) { cnx = appSettings.Value.Conxstr;  }
        //Task
        public async Task<List<counttask_md>> listTaskAsync(counttask_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { return await ops.listTaskAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task<counttask_md> getTaskAsync(counttask_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { return await ops.getTaskAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task upsertTaskAsnc(counttask_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { await ops.upsertTaskAsnc(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task removeTaskAsync(counttask_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { await ops.removeTaskAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        //Plan 
        public async Task<List<countplan_md>> listPlanAsync(counttask_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { return await ops.listPlanAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task<countplan_md> getPlanAsync(countplan_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { return await ops.getPlanAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task upsertPlanAysnc(countplan_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { await ops.upsertPlanAysnc(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task removePlanAsync(countplan_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { await ops.removePlanAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        public async Task validatePlanAsync(countplan_md o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { await ops.validatePlanAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); } 
        }

        //Line
        public async Task<List<countline_md>> listLineAsync(countplan_md o)  { 
            counting_ops ops = new counting_ops(cnx);
            try     { return await ops.listLineAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        //Line
        public async Task<List<countline_md>> listCountAsync(countplan_md o)
        {
            counting_ops ops = new counting_ops(cnx);
            try { return await ops.countLineAsync(o); }
            catch (Exception ex) { throw ex; }
            finally { ops.Dispose(); }
        }
        public async Task<List<countline_md>> countLineAsync(countplan_md o)
        {
            counting_ops ops = new counting_ops(cnx);
            try { return await ops.countLineAsync(o); }
            catch (Exception ex) { throw ex; }
            finally { ops.Dispose(); }
        }
        public async Task<List<countline_md>> getLineAsync(findcountline_md o)
        {
            counting_ops ops = new counting_ops(cnx);
            try { return await ops.getLineAsync(o); }
            catch (Exception ex) { throw ex; }
            finally { ops.Dispose(); }
        }
        public async Task upsertLineAsync(List<countline_md> o) { 
            counting_ops ops = new counting_ops(cnx);
            try     { await ops.upsertLineAsync(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }

        public async Task<List<countcorrection_md>> getConfrimLineAsync(counttask_md o)
        {
            counting_ops ops = new counting_ops(cnx);
            try { return await ops.getConfrimLineAsync(o); }
            catch (Exception ex) { throw ex; }
            finally { ops.Dispose(); }
        }

        public async Task countConfirmAsync(counttask_md o)
        {
            counting_ops ops = new counting_ops(cnx);
            try { await ops.countConfirmAsync(o); }
            catch (Exception ex) { throw ex; }
            finally { ops.Dispose(); }
        }
        public async Task<createhu_md> CreateHUAsync(createhu_md o) {
            counting_ops ops = new counting_ops(cnx);
            try { return await ops.CreateHUAsync(o); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
    }
}