using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public partial class shareprep_ops : IDisposable { 

        public async Task<List<shareprep_md>> listAsync(shareprep_md o) { 
            SqlCommand cm = new SqlCommand(sqlshareprep_fnd,cn);
            SqlDataReader r = null;
            List<shareprep_md> rn = new List<shareprep_md>();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.shprep,"shprep");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setShareprep(ref r)); }
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

        public async Task<shareprep_md> getAsync(shareprep_md o){ 
            SqlCommand cm = new SqlCommand(sqlshareprep_fnd,cn);
            SqlDataReader r = null;
            shareprep_md rn = new shareprep_md();
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.shprep,"shprep");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = setShareprep(ref r); }
                await r.CloseAsync();

                rn.lines = new List<shareprln_md>();
                cm.CommandText = sqlshareprln_fnd;
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()){ rn.lines.Add(setShareprln(ref r)); }
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
        
        public async Task upsertAsnc(shareprep_md o){ 
            SqlCommand cm = new SqlCommand(sqlshareprep_vald,cn);
            try { 
                
                fillCommand(ref cm, o);
                cm.CommandText = (cm.snapsScalarStrAsync().Result.ToString() == "0") ? sqlshareprep_insert : sqlshareprep_update;
                await cm.snapsExecuteAsync();
            }
            catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync(); }
        }

        public async Task removeAsync(shareprep_md o){
            SqlCommand cm = new SqlCommand(sqlshareprep_remove,cn);
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.shprep,"shprep");

                await cm.snapsExecuteAsync();
                cm.CommandText = sqlshareprln_remove;
                await cm.snapsExecuteAsync();
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }

        public async Task uplineAsync(shareprln_md o){ 
            SqlCommand cm = new SqlCommand(sqlshareprln_vald,cn);
            try {
                fillCommand(ref cm,o);
                cm.CommandText = (cm.snapsScalarStrAsync().Result.ToString() == "0") ? sqlshareprln_insert : sqlshareprln_update;
                await cm.snapsExecuteAsync();
                
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }

        public async Task rmlineAsync(shareprln_md o) {
            SqlCommand cm = new SqlCommand(sqlshareprln_remove,cn);
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.shprep,"shprep");
                cm.snapsPar(o.thcode,"thcode");
                await cm.snapsExecuteAsync();
                cm.CommandText = sqlshareprln_remove;
                await cm.snapsExecuteAsync();
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }

    }
}