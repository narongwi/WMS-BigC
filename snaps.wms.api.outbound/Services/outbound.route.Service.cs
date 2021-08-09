using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using System.Collections.Generic;
using Snaps.WMS;
using Snaps.Helpers.Logger;

namespace Snaps.WMS.Services { 
    public class ourouteService : IourouteService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private route_ops op;
        public ourouteService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new route_ops(_appSettings.Value.Conxstr); 
        }
        
        public async Task<List<route_thsum>> thsum(route_pm o) { 
            try     { return await op.thsum(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task<List<route_ls>> listAsync(route_pm o){ 
            try     { return await op.find(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task<route_md> getAsync(route_ls o) { 
            try     { return await op.get(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }
        public async Task upsert(route_md o){ 
            try     {  
                if (o.tflow == "NW") { 
                    await op.bgcth_getroute(o,o.tflow);
                } else { 
                    await op.upsert(o);  
                }
            }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        public async Task allocate(route_md o){ 
            try     { await op.allocate(o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        public async Task huload(string orgcode, string site, string depot, route_hu o) {
            try     { await op.huload(orgcode, site,depot,o); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        //shipment
        public async Task shipment(route_md o,string accncode) {
            try     { await op.shipment(o,accncode); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        // Task<List<lov>> getstaging(String orgcode, String site,String depot)
        public async Task<List<Snaps.WMS.lov>> getstaging(String orgcode, String site,String depot){ 
            try     {  return await op.getstaging(orgcode, site,depot);  }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        //Task<List<lov>> getThirdparty(String orgcode,String site, String depot)
        public async Task<List<Snaps.WMS.lov>> getthirdparty(String orgcode, String site, String depot)
        {
            try { return await op.getThirdparty(orgcode, site, depot); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        //public async Task<List<lov>> getTransporter(String orgcode, String site, String depot )
        public async Task<List<Snaps.WMS.lov>> getTransporter(String orgcode, String site, String depot)
        {
            try { return await op.getTransporter(orgcode, site, depot); }
            catch (Exception ex) { throw ex; }
            finally { }
        }

        public async Task remove(route_md o){ 
            try     { await op.remove(o); }
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        

    }
}