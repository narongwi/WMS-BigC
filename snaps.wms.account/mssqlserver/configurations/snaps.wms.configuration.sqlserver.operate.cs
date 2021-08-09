using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Newtonsoft.Json;

namespace Snaps.WMS {
    public partial class config_ops {
        public async Task<List<config_md>> findAsync(config_md rs) {
            SqlCommand cm = new SqlCommand(sqlconfig_find,cn);
            List<config_md> rn = new List<config_md>();
            SqlDataReader r = null;
            try {
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn(rs.cfgcode,"plccode");
                cm.snapsCdn(rs.cfgname,"plcname");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setConfiguration(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { throw ex; } finally { if(cm != null) { await cm.DisposeAsync(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task upsertAsync(config_md o) {
            SqlCommand cm = new SqlCommand(sqlconfig_insert,cn);
            try {
                fillCommand(ref cm,o);
                cm.CommandText = (o.tflow == "NW") ? sqlconfig_insert : sqlconfig_update;
                await cm.snapsExecuteAsync();
            } catch(Exception ex) { throw ex; } finally { await cm.DisposeAsync(); }
        }
        public async Task dropAsync(config_md o) {
            SqlCommand cm = new SqlCommand(sqlconfig_remove,cn);
            try {
                fillCommand(ref cm,o);
                cm.CommandText = sqlconfig_remove;
                await cm.snapsExecuteAsync();
            } catch(Exception ex) { throw ex; } finally { await cm.DisposeAsync(); }
        }

        public async Task<config_md> getWebActive(string site,string accscode) {
            SqlCommand cm = new SqlCommand(sqlconfig_default,cn);
            config_md rn = new config_md();
            SqlDataReader r = null;
            try {
                cm.snapsPar(accscode,"accscode");
                cm.snapsPar(site,"site");

                r = await cm.snapsReadAsync();
                if(r.HasRows) {
                    while(await r.ReadAsync()) { rn = setConfiguration(ref r); }
                    // web module filter
                    rn.roleaccs.modules = rn.roleaccs.modules.Where(m => m.permission.Any(p => p.objtype == "web")).ToList();
                }

                await r.CloseAsync();
                await cn.CloseAsync();

                if(rn.roleaccs == null) {
                    throw new Exception("Access Denied !!!");//  You don't have permission 
                }

                return rn;
            } catch(Exception ex) { throw ex; } finally { if(cm != null) { await cm.DisposeAsync(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<config_md> getPdaActive(string accscode) {
            SqlCommand cm = new SqlCommand(sqlconfig_pda,cn);
            config_md rn = new config_md();
            SqlDataReader r = null;
            try {
                cm.snapsPar(accscode,"accscode");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = setConfiguration(ref r); }

                // web module filter
                rn.roleaccs.modules = rn.roleaccs.modules.Where(m => m.permission.Any(p => p.objtype == "pda")).ToList();

                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { throw ex; } finally { if(cm != null) { await cm.DisposeAsync(); } if(r != null) { await r.DisposeAsync(); } }
        }
    }
}