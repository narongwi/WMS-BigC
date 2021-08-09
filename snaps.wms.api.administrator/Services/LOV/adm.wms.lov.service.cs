using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
using Snaps.WMS.warehouse;
using Snaps.WMS.parameter;

namespace Snaps.WMS.Services {
    public class LOVService : ILOVService {
        private readonly string cnx;
        public LOVService(IOptions<AppSettings> appSettings) { cnx = appSettings.Value.Conxstr; }


        public async Task<List<lov>> lovwarehouseAsync() {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovwarehouseAsync(); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        public async Task<List<lov>> lovwarehouseAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovwarehouseAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovdepotAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovdepotAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovroleAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovroleAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovAsync(string orgcode,string site,string depot,string bntype,string bncode,Boolean isdesc = true) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovAsync(orgcode,site,depot,bntype,bncode,isdesc); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovprepzonestockAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovprepzonestockAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovprepzonedistAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovprepzonedistAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovstoragezoneAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovstoragezoneAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovsharedistAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovsharedistAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovhuAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovhuAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovzoneAsync(string orgcode,string site,string depot) {
            locup_ops ops = new locup_ops(cnx);
            try { return await ops.LovLocdw("zone",orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        public async Task<List<lov>> lovaisleAsync(string orgcode,string site,string depot,string zone) {
            locup_ops ops = new locup_ops(cnx);
            try { return await ops.LovLocdw("aisle",orgcode,site,depot,zone); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        public async Task<List<lov>> lovbayAsync(string orgcode,string site,string depot,string zone,string aisle) {
            locup_ops ops = new locup_ops(cnx);
            try { return await ops.LovLocdw("bay",orgcode,site,depot,zone,aisle); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        public async Task<List<lov>> lovlevelAsync(string orgcode,string site,string depot,string zone,string aisle,string bay) {
            locup_ops ops = new locup_ops(cnx);
            try { return await ops.LovLocdw("level",orgcode,site,depot,zone,aisle,bay); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        public async Task<List<lov>> lovlocationAsync(string orgcode,string site,string depot,string zone,string aisle,string bay,string level) {
            locup_ops ops = new locup_ops(cnx);
            try { return await ops.LovLocdw("location",orgcode,site,depot,zone,aisle,bay,level); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        //Staing Inbound 
        public async Task<List<lov>> lovstaginginbAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovstaginginbAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Staging Outbound
        public async Task<List<lov>> lovstagingoubAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovstagingoubAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Bulk

        public async Task<List<lov>> lovbulkAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovbulkAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Damage
        public async Task<List<lov>> lovdamageAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovdamageAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Sinbin
        public async Task<List<lov>> lovsinbinAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovsinbinAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Exchange 
        public async Task<List<lov>> lovexchangeAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovexchangeAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Overflow 
        public async Task<List<lov>> lovoverflowAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovoverflowAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Return 
        public async Task<List<lov>> lovreturnAsync(string orgcode,string site,string depot) {
            binary_ops ops = new binary_ops(cnx);
            try { return await ops.lovreturnAsync(orgcode,site,depot); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }

        //Location validate 
        //Asile 
        public Boolean valaisle(string orgcode,string site,string depot,string loc) {
            binary_ops ops = new binary_ops(cnx);
            try { return ops.valaisle(orgcode,site,depot,loc); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Bay
        public Boolean valbay(string orgcode,string site,string depot,string loc) {
            binary_ops ops = new binary_ops(cnx);
            try { return ops.valbay(orgcode,site,depot,loc); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Level
        public Boolean vallevel(string orgcode,string site,string depot,string loc) {
            binary_ops ops = new binary_ops(cnx);
            try { return ops.vallevel(orgcode,site,depot,loc); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
        //Location
        public Boolean vallocation(string orgcode,string site,string depot,string loc) {
            binary_ops ops = new binary_ops(cnx);
            try { return ops.vallocation(orgcode,site,depot,loc); } catch(Exception ex) { throw ex; } finally { ops.Dispose(); }
        }
    }
}


