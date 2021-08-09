using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Snaps.WMS.Interfaces;
using Snaps.Helpers;
using System.Collections.Generic;
using Snaps.WMS;
using Snaps.WMS.warehouse;
using Snaps.Helpers.StringExt;
using Snaps.WMS.parameter;
namespace Snaps.WMS.Services { 
    public class SystemBinaryService : ISystemBinaryService { 
        private readonly IOptions<AppSettings>  aps;
        public SystemBinaryService(IOptions<AppSettings> appSettings) {
            aps = appSettings;
        }

        public resultRequest getMessage(String lang, String ercode,resultState result,String reqid, String opt1 = "",String opt2 = "" ){ 
            warehouse_ops ops = new warehouse_ops(aps.Value.Conxstr);
            try     { return new resultRequest(result,string.Format(ops.getMessage(lang, ercode).ToString(), opt1, opt2),reqid); } 
            catch   (Exception ex) { throw ex; } 
            finally { ops.Dispose(); }
        }
        // public async Task<List<lov>> LOVWarehouse(String valorg, String valsite, String valdepot){ 
        //     warehouse_ops ops = new warehouse_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try     { 
        //         foreach(warehouse_ls ls in await ops.find(new warehouse_pm() { orgcode = valorg, sitecode = valsite })) { 
        //             rn.Add(new lov(ls.sitecode, ls.sitename));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }

        // public async Task<List<lov>> LOVDepot(String valorg, String valsite, String valdepot) { 
        //     depot_ops ops = new depot_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try     { 
        //         foreach(depot_ls ls in await ops.find(new depot_pm() { orgcode = valorg, sitecode = valsite, depotcode = valdepot })) { 
        //             rn.Add(new lov(ls.depotcode, ls.depotname, ls.sitecode,""));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }

        // public async Task<List<lov>> LOVRole(String valorg, String valsite, String valdepot) { 
        //     role_ops ops = new role_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try     { 
        //         foreach(role_ls ls in await ops.find(new role_pm(){ orgcode = valorg } )) { 
        //             rn.Add(new lov(ls.rolecode, ls.rolename));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }

        // public async Task<List<lov>> LOVState(String valorg, String valsite, String valdepot, String valtype,String valcode) { 
        //     binary_ops ops = new binary_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try     { 
        //         foreach(binary_ls ls in await ops.find(new binary_pm() { 
        //             apps = "WMS", bntype = valtype, site = valsite, depot = valdepot, orgcode = valorg, bncode = valcode
        //         })) { 
        //             rn.Add(new lov(ls.bnvalue, ls.bndescalt,ls.bnflex1,ls.bnflex2,ls.bnflex3,ls.bnflex4,ls.bnicon));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }

        // public async Task<List<lov>> LOVConfig(String valorg, String valsite, String valdepot, String valtype,String valcode) { 
        //     binary_ops ops = new binary_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try     { 
        //         foreach(binary_ls ls in await ops.find(new binary_pm() { 
        //             apps = "WMS", bntype = valtype, site = valsite, depot = valdepot, orgcode = valorg, bncode = valcode
        //         })) { 
        //             rn.Add(new lov(ls.bncode, ls.bndescalt,ls.bnflex1,ls.bnflex2,ls.bnflex3,ls.bnflex4,ls.bnicon));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }


        // //List of location upper 
        // public async Task<List<lov>> LOVLocupper(locup_pm o) { 
        //     locup_ops ops = new locup_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try { 
        //         foreach(locup_ls ls in await ops.find(o)) { 
        //             rn.Add(new lov(ls.lscode, ls.lscodealt,ls.spcarea,ls.lszone,ls.lsaisle,ls.lsbay,""));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }
        // public async Task<List<lov>> LOVLoclower(locdw_pm o) { 
        //     locdw_ops ops = new locdw_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try { 
        //         foreach(locdw_ls ls in await ops.find(o)) { 
        //             rn.Add(new lov(ls.lscode, ls.lscode,"","",""));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }

        // public async Task<List<lov>> LOVLocdist(locup_pm o) { 
        //     locup_ops ops = new locup_ops(aps.Value.Conxstr);
        //     List<lov> rn = new List<lov>();
        //     try { 
        //         foreach(locup_ls ls in await ops.finddist(o)) { 
        //             rn.Add(new lov(ls.lscode, ls.lscodealt,"","",""));
        //         } 
        //         return rn;
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ops.Dispose(); }
        // }

        // //LOV of handerling unit 
        // public async Task<List<lov>> LOVHU(string orgcode, string site, string depot) { 
        //     handerlingunit_ops ho = new handerlingunit_ops(aps.Value.Conxstr);
        //     try {                 
        //         return await ho.Lov(orgcode,site,depot);
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { ho.Dispose(); }
        // }

        // //LOV of storage 
        // public async Task<List<lov>> LOVZone(string orgcode, string site, string depot){ 
        //      locup_ops opup = new locup_ops(aps.Value.Conxstr); 
        //      try {                 
        //         return await opup.Lovzone(orgcode,site,depot);
        //     } 
        //     catch   (Exception ex) { throw ex; } 
        //     finally { opup.Dispose(); }
        // }
    
    
    }
}