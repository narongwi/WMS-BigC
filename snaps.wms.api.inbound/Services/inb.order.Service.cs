using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
namespace Snaps.WMS.Services { 
    public class inboundService : IinboundService { 
        private readonly IOptions<AppSettings>  _appSettings;
        private inbound_ops op;
        public inboundService(IOptions<AppSettings> appSettings) {
            _appSettings = appSettings;
            op = new inbound_ops(_appSettings.Value.Conxstr); 
        }
        public async Task<List<inbound_ls>> listAsync(inbound_pm o){ 
            try     { return await op.find(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<inbound_md> getAsync(inbound_ls o) { 
            try     { return await op.get(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task upsertAsync(inbound_md o){ 
            try     { await op.upsert(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<List<lov>> getstaging(String orgcode, String site,String depot, Int32 quantity) { 
            try     { return await op.getstaging(orgcode,site,depot,quantity); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
  
        public async Task<List<lov>> getproductratio(String orgcode, String site, String depot, String article, String pv, String lv) {
            try     { return await op.getproductratio(orgcode,site,depot,article,pv,lv); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setinvoice(String orgcode, String site, String depot, String inorder, String invoiceno, String accncode) { 
            try     { await op.setinvoice(orgcode,site,depot,inorder,invoiceno,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task setreplan(String orgcode, String site, String depot, String inorder, DateTimeOffset? datereplan, String accncode) {
            try     { await op.setreplan(orgcode,site,depot,inorder,datereplan,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setpriority(String orgcode, String site,String depot, String inorder,Int32 inpriority, String accncode) {
            try     { await op.setpriority(orgcode,site,depot,inorder,inpriority,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setstaging(String orgcode, String site,String depot, String inorder,String staging,String accncode) {
            try     { await op.setstaging(orgcode,site,depot,inorder,staging,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setremarks(String orgcode, String site,String depot, String inorder, String remarks, String accncode){ 
            try     { await op.setremarks(orgcode,site,depot,inorder,remarks,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setunloadstart(String orgcode, String site,String depot, String inorder, String accncode) {
            try     { await op.setunloadstart(orgcode,site,depot,inorder,accncode ); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setunloadend(String orgcode, String site,String depot, String inorder, String accncode) { 
            try     { await op.setunloadend(orgcode,site,depot,inorder,accncode ); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setfinish(String orgcode, String site,String depot, String inorder, String accncode) {
            try     {  await op.setfinish(orgcode,site,depot,inorder,accncode ); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setcancel(String orgcode, String site, String depot, String inorder, String remarks, String accncode) {
            try     { await op.setcancel(orgcode,site,depot,inorder,remarks,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task setreplan(String orgcode, String site, String depot, String inorder, DateTime datereplan, String accncode) { 
            try     { await op.setreplan(orgcode,site,depot,inorder,datereplan,accncode); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        //Line receive 
        public async Task<List<inboulx>> getlx(inboulx o){ 
            try     { return await op.getlx(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<List<inboulx>> upsertlx(inboulx o) { 
            try     { return await op.upsertlx(o); } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<List<inboulx>> removelx(inboulx o) { 
            try     { return  await op.removelx(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }
        public async Task<List<inboulx>> commitlx(inboulx o) { 
            try     { return  await op.commitlx(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

        public async Task<List<inbound_hs>> history(inbound_pm o) { 
            try     { return  await op.history(o);  } 
            catch   (Exception ex) { throw ex; } 
            finally { }
        }

    }
}