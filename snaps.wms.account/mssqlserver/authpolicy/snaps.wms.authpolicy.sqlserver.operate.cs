using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public partial class policy_ops { 
        public async Task<List<policy_md>> findAsync(policy_md rs) { 
            SqlCommand cm =  new SqlCommand(sqlpolicy_get,cn);
            List<policy_md> rn = new List<policy_md>();
            SqlDataReader r = null;
            try { 
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn(rs.plccode,"plccode");
                cm.snapsCdn(rs.plcname,"plcname");               
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setPolicy(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { await cm.DisposeAsync();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsertAsync(policy_md o) {
            SqlCommand cm = new SqlCommand(sqlpolicy_vld,cn);
            try { 
                fillCommand(ref cm,o);
                cm.CommandText = (o.tflow == "NW") ? sqlpolicy_insert : sqlpolicy_update;
                await cm.snapsExecuteAsync();              
            }catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync();   }
        }

        public async Task dropAsync(policy_md o) {
            SqlCommand cm = new SqlCommand(sqlpolicy_remove,cn);
            try { 
                fillCommand(ref cm,o);
                cm.CommandText = sqlpolicy_remove;
                await cm.snapsExecuteAsync();              
            }catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync();   }
        }
    }
}