using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
using Snaps.WMS.preparation;

namespace Snaps.WMS.Services { 
    public class ouprepService : IouprepService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private prep_ops op;
        private string cnx;
        public ouprepService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new prep_ops(_appSettings.Value.Conxstr); 
            cnx = _appSettings.Value.Conxstr;
        }

        public async Task<List<prep_ls>> listAsync(prep_pm o){ 
            try     { return await op.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<prep_md> getAsync(prep_ls o) { 
            try     { return await op.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task setPriority(prep_md o){ 
            try     {  await op.setPriority(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setStart(prep_md o){ 
            try     {  await op.setStart(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setEnd(prep_md o){ 
            try     {  await op.setEnd(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task opsPick(prln_md o){ 
            try     {  await op.opsPick(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task opsPut(prln_md o) { 
            try     {  await op.opsPut(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task opsCancel(prep_md o) { 
            try     {  await op.opsCancel(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task opsSelect(ouselect o) {
            prep_ops op = new prep_ops(cnx);
            try {
                await op.opsSelect(o);
            } catch(Exception ex) { throw ex; } finally { op.Dispose(); }
        }

        public async Task opsUnselect(ouselect o) {
            prep_ops op = new prep_ops(cnx);
            try {
                await op.opsUnselect(o);
            } catch(Exception ex) { throw ex; } finally { op.Dispose(); }
        }
        public async Task<prepset> procsetup(prepset o){ 
            prep_ops op = new prep_ops(cnx);
            try {
                return await op.procsetup(o);
            }
            catch (Exception ex){ throw ex; }
            finally { op.Dispose(); }
        }


        public async Task<prepset> procstock(string orgcode, string site, string depot,string spcarea, string setno,string accncode) {
            try { return await op.processStock(orgcode,site,depot,"ST",setno,accncode); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        public async Task<prepset> distsetup(prepset o){ 
            prep_ops op = new prep_ops(cnx);
            try { 
                return await op.distsetup(o);
            }catch (Exception ex) { throw ex; }
            finally { op.Dispose(); }
        }
        public async Task<prepset> procdistb(string orgcode,string site,string depot,string spcarea, string setno,string accncode) { 
            try { return await op.procdistb(orgcode,site,depot,"XD",setno,accncode); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

    }
}