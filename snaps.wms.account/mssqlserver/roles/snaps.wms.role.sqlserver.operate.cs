using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS
{
    public partial class role_ops : IDisposable
    {

        public async Task<List<role_md>> findAsync(role_md rs)
        {
            SqlCommand cm = new SqlCommand(sqlrole_find, cn);
            List<role_md> rn = new List<role_md>();
            SqlDataReader r = null;
            try
            {
                cm.snapsPar(rs.orgcode, "orgcode");
                cm.snapsPar(rs.site, "site");
                //cm.snapsPar(rs.depot, "depot");
                cm.snapsCdnlike(rs.rolecode, "rolecode");
                cm.snapsCdnlike(rs.rolename, "rolename");
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn.Add(setRole(ref r, false)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if (cm != null) { await cm.DisposeAsync(); } if (r != null) { await r.DisposeAsync(); } }
        }
        public async Task<role_md> getAsync(role_md rs)
        {
            SqlCommand cm = new SqlCommand(sqlrole_find, cn);
            role_md rn = new role_md();
            SqlDataReader r = null;
            try
            {

                cm.snapsPar(rs.orgcode, "orgcode");
                cm.snapsPar(rs.site, "site");
                //cm.snapsPar(rs.depot, "depot");

                cm.snapsCdnequal(rs.rolecode, "rolecode");
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn = setRole(ref r, true); }
                await r.CloseAsync();

                //cm.Parameters[""]
                //cm.CommandText = sqlroln_fnd;
                //r = await cm.snapsReadAsync();
                //while (await r.ReadAsync())
                //{
                //    rn.lines.Add(setRoln(ref r));
                //}
                //await r.CloseAsync();

                await cn.CloseAsync();
                return rn;

            }
            catch (Exception ex) { throw ex; }
            finally { if (cm != null) { await cm.DisposeAsync(); } if (r != null) { await r.DisposeAsync(); } }
        }

        public async Task<role_md> getMasterAsync(string orgcode, string site, string depot, string accncreate)
        {
            SqlCommand cm = new SqlCommand(sqlrole_master, cn);
            role_md rn = new role_md();
            SqlDataReader r = null;
            module crmodule;
            try
            {
                // for New role master
                rn.orgcode = orgcode;
                rn.apcode = "WMS";
                rn.site = site;
                rn.depot = depot;
                rn.tflow = "IO";
                rn.datecreate = DateTime.Now;
                rn.accncreate = accncreate;
                rn.datemodify = DateTime.Now;
                rn.accnmodify = accncreate;
                rn.procmodify = "wms.role.setup";
                rn.roleaccs = new List<accn_roleacs>();
                rn.roleaccs.Add(new accn_roleacs());
                rn.roleaccs[0].modules = new List<module>();

                // read role category master 
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync())
                {
                    rn.roleaccs[0].site = site;
                    rn.roleaccs[0].depot = depot;
                    rn.roleaccs[0].modules.Add(new module()
                    {
                        modulecode = r["objcode"].ToString(),
                        modulename = r["objname"].ToString(),
                        moduleicon = r["objicon"].ToString(),
                        permission = new List<permission>()
                    });
                }
                await r.CloseAsync();

                //rn.roleaccs.Add(new accn_roleacs());
                //rn.roleaccs[0].modules = new List<module>();
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "inb", moduleicon = "fas fa-truck-loading", modulename = "Inbound", permission = new List<permission>() });
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "mmv", moduleicon = "fas fa-people-carry", modulename = "Task movement", permission = new List<permission>() });
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "inv", moduleicon = "fas fa-boxes", modulename = "Inventory", permission = new List<permission>() });                
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "oub", moduleicon = "fas fa-truck-loading fa-flip-horizontal", modulename = "Outbound", permission = new List<permission>() });                
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "reps", moduleicon = "fas fa-cloud-download-alt", modulename = "Report", permission = new List<permission>() });                
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "mast", moduleicon = "fas fa-heartbeat", modulename = "Master", permission = new List<permission>() });               
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "conf", moduleicon = "fas fa-wrench", modulename = "Configuration", permission = new List<permission>() });               
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "wahm", moduleicon = "fas fa-warehouse", modulename = "Warehouse Map", permission = new List<permission>() });               
                ////rn.roleaccs[0].modules.Add(new module() { modulecode = "imps", moduleicon = "fas fa-file-upload", modulename = "Import data", permission = new List<permission>() });               
                //rn.roleaccs[0].modules.Add(new module() { modulecode = "adms", moduleicon = "fas fa-user-circle", modulename = "Account", permission = new List<permission>() });

                cm.CommandText = sqlroln_master;
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync())
                {
                    crmodule = rn.roleaccs[0].modules.FirstOrDefault(e => e.modulecode == r["objmodule"].ToString());
                    if(crmodule != null)
                    {
                        crmodule.permission.Add(
                         new permission()
                         {
                             objmodule = r["objmodule"].ToString(),
                             objtype = r["objtype"].ToString(),
                             objcode = r["objcode"].ToString(),
                             objname = r["objname"].ToString(),
                             apcode = "",
                             objicon = "",
                             objroute = r["objroute"].ToString(),
                             objseq = r["objseq"].ToString().CInt32(),
                             orgcode = "",
                             rolecode = "",
                             isenable = 1,
                             isexecute = 1
                             //isenable = (r.IsDBNull(6)) ? 0 :  r.GetInt32(6),
                             //isexecute = (r.IsDBNull(7)) ? 0 :  r.GetInt32(7),
                         }
                    );
                    }
                    
                }
                await r.CloseAsync();

                await cn.CloseAsync();
                return rn;

            }
            catch (Exception ex) { throw ex; }
            finally { if (cm != null) { await cm.DisposeAsync(); } if (r != null) { await r.DisposeAsync(); } }
        }
        public async Task upsertAsync(role_md o)
        {
            SqlCommand cm = new SqlCommand(sqlrole_exists, cn);
            List<SqlCommand> lm = new List<SqlCommand>();
            try
            {
                fillCommand(ref cm, o);


                if(o.tflow == "NW" )
                {
                    if(cm.snapsScalarStrAsync().Result != "0") throw new Exception("Role code already exists in system");
                    // insert 
                    cm.CommandText = sqlrole_insert;
                    await cm.snapsExecuteAsync();
                }
                else
                {
                    // update
                    cm.CommandText = sqlrole_update;
                    await cm.snapsExecuteAsync();
                }
            }
            catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync(); }
        }

        public async Task dropAsync(role_md o)
        {
            SqlCommand cm = new SqlCommand(sqlrole_remove_step0, cn);
            try
            {
                fillCommand(ref cm, o);

                // check alredy use role
                if(cm.snapsScalarStrAsync().Result == "0")
                {
                    cm.CommandText = sqlrole_remove_step1;
                    await cm.snapsExecuteAsync();

                    //cm.CommandText = sqlrole_remove_step2;
                    //await cm.snapsExecuteAsync();
                }else
                {
                    throw new Exception("this role is active on user");
                }

            }
            catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync(); }
        }
    }
}