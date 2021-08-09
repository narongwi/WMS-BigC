using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers.Hash;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.WMS.warehouse;

namespace Snaps.WMS {
    public partial class depot_ops : IDisposable { 

        public async Task<List<depot_md>> findAsync(depot_md o) {
            SqlCommand cm = new SqlCommand(sqldepot_fnd,cn);
            SqlDataReader r = null;
            List<depot_md> rn = new List<depot_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.sitecode,"site");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setDepot(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                throw ex;
            }finally {
                if (r!=null){ await r.DisposeAsync(); }
                await cm.DisposeAsync();
            }
        }

        public async Task upsertAsync(depot_md o) {
            SqlCommand cm = new SqlCommand(sqldepot_validate,cn);
            List<SqlCommand> lm = new List<SqlCommand>();
            try { 
                if ("" == "0"){ 
                    throw new Exception("Autorization code invalid");
                }else { 
                    if (o.tflow == "NW"){
                        if (o.authcode.TovldAuth(o.authdigit,(o.authdate??DateTimeOffset.Now).ToString("yyyyMMdd")) == false) { 
                            throw new Exception("Autorization code invalid");
                        }else { 
                            o.tflow = "IO";
                            lm.Add(sqldepot_add_step1.snapsCommand());
                            lm.Add(sqldepot_add_step2.snapsCommand());
                            lm.Add(sqldepot_add_step3.snapsCommand());
                            lm.Add(sqldepot_add_step4.snapsCommand());
                            lm.Add(sqldepot_add_step5.snapsCommand());
                            lm.Add(sqldepot_add_step6.snapsCommand());
                            lm.Add(sqldepot_add_step7.snapsCommand());
                            lm.Add(sqldepot_add_step8.snapsCommand());
                            lm.Add(sqldepot_add_step9.snapsCommand());
                            lm.ForEach(e=> { 
                                e.snapsPar(o.orgcode,"orgcode");
                                e.snapsPar(o.sitecode,"site");
                                e.snapsPar(o.depotcode,"depot");
                                e.snapsPar(o.accnmodify,"accncode");
                            });
                            
                            lm.Add(getCommand(o,sqldepot_add_step10));
                            await lm.snapsExecuteTransAsync(cn,true);
                        }

                        
                    }else { 
                        fillCommand(ref cm, o);
                        cm.CommandText = sqldepot_update;
                        await cm.snapsExecuteAsync();
                    }
                }
            }
            catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync(); }
        }

        public async Task removeAsync(depot_md o) {
            SqlCommand cm = new SqlCommand(sqldepot_validate,cn);
            List<SqlCommand> lm = new List<SqlCommand>();
            try { 
                if (o.authcode.TovldAuth(o.authdigit,(o.authdate??DateTimeOffset.Now).ToString("yyyyMMdd")) == false) { 
                    throw new Exception("Autorization code invalid");
                }else { 
                    lm.Add(sqldepot_remove_step1.snapsCommand());
                    lm.Add(sqldepot_remove_step2.snapsCommand());
                    lm.Add(sqldepot_remove_step3.snapsCommand());
                    lm.Add(sqldepot_remove_step4.snapsCommand());
                    lm.Add(sqldepot_remove_step5.snapsCommand());
                    lm.Add(sqldepot_remove_step6.snapsCommand());
                    lm.Add(sqldepot_remove_step7.snapsCommand());
                    lm.Add(sqldepot_remove_step8.snapsCommand());
                    lm.Add(sqldepot_remove_step9.snapsCommand());
                    lm.Add(sqldepot_remove_step10.snapsCommand());
                    lm.ForEach(e=> { 
                        e.snapsPar(o.orgcode,"orgcode");
                        e.snapsPar(o.sitecode,",site");
                        e.snapsPar(o.depotcode,"depot");
                        e.snapsPar(o.accnmodify,"accncode");
                    });
                    
                    await lm.snapsExecuteTransAsync(cn);
                }
                await cm.snapsExecuteAsync();
            }
            catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync(); }
        }

    }
}