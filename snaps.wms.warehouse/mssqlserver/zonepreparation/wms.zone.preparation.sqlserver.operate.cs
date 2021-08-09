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

namespace Snaps.WMS.warehouse {

    public partial class zoneprep_ops : IDisposable { 

        //list zone prep
        public async Task<List<zoneprep_md>> listAsync(zoneprep_md o) { 
            SqlCommand cm = new SqlCommand(sqlzoneprep_find,cn);
            SqlDataReader r = null;
            List<zoneprep_md> rn = new List<zoneprep_md>();
            try {
                fillPRpar(ref cm, o);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillPR(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }
        
        //upsert zone prep
        public async Task upsertAsync(zoneprep_md o) { 
            SqlCommand cm = new SqlCommand(sqlzoneprep_validate, cn);
            try { 
                fillPRpar(ref cm, o);
                cm.CommandText = (cm.snapsScalarStrAsync().Result == "1") ? sqlzoneprep_update : sqlzoneprep_insert;
                await cm.snapsExecuteAsync();
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }

        //remove zone prep 
        public async Task removeAsync(zoneprep_md o){ 
            
            SqlCommand cm = new SqlCommand(sqlzoneprep_remove_step1,cn);
            SqlCommand cm2 = new SqlCommand(sqlzoneprep_remove_step2,cn);
            try {
                fillPRpar(ref cm, o);
                cm.CommandText = sqlzoneprep_remval;
                if (cm.snapsScalarStrAsync().Result != "0") {
                    throw new Exception("Preparation zone still using");
                }else{
                    cm.CommandText = sqlzoneprep_remove_step1;
                    fillPRpar(ref cm2, o);
                    await cm.snapsExecuteAsync();
                    await cm2.snapsExecuteAsync();
                }                
            }catch (Exception ex){
                throw ex;   
            }finally {
                await cm.DisposeAsync();
            }
        }



        //list line prep
        public async Task<List<zoneprln_md>> lineAsync(zoneprep_md o) { 
            SqlCommand cm = new SqlCommand(sqlzoneprln_find,cn);
            SqlDataReader r = null;
            List<zoneprln_md> rn = new List<zoneprln_md>();
            try {
                fillPRpar(ref cm, o);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillPL(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }
        
        //upsert line prep
        public async Task upsertAsync(zoneprln_md o) { 
            SqlCommand cm = new SqlCommand(sqlzoneprln_validate, cn);
            SqlCommand tm = new SqlCommand(sqlzoneprln_tempzone, cn);

            Int32 rtoskuofpu = 0;
            string tempzone = "";
            string rsl = "";
            try { 
                if (o.tflow == "NW") {
                    tm.snapsPar(o.orgcode, "orgcode");
                    tm.snapsPar(o.site, "site");
                    tm.snapsPar(o.depot, "depot");
                    tempzone = tm.snapsScalarStrAsync().Result;
                    cm.CommandText = sqlzoneprln_isexists;
                    cm.snapsPar(tempzone, "tempzone");
                    fillPLpar(ref cm, o);
                    rsl = cm.snapsScalarStrAsync().Result;
                    if (cm.snapsScalarStrAsync().Result != "pass"){ 
                        throw new Exception(rsl);
                    }else { 
                        if (!string.IsNullOrEmpty(o.spcproduct)){ 
                            try { 
                                cm.CommandText = sqlzoneprep_getratio;
                                rtoskuofpu = cm.snapsScalarStrAsync().Result.ToString().CInt32();
                            }catch (Exception) { 
                                throw new Exception("Article incorrect ");
                            }                           
                        }else {
                            rtoskuofpu = 0;
                        }
                        cm.Parameters["tflow"].Value = "IO";
                        cm.Parameters["rtoskuofpu"].Value = rtoskuofpu;
                        cm.CommandText = sqlzoneprln_insert;
                        await cm.snapsExecuteAsync();
                    }
                }else {
                    tm.snapsPar(o.orgcode, "orgcode");
                    tm.snapsPar(o.site, "site");
                    tm.snapsPar(o.depot, "depot");
                    tempzone = tm.snapsScalarStrAsync().Result;
                    cm.CommandText = sqlzoneprln_ismodify;
                    cm.snapsPar(tempzone, "tempzone");
                    fillPLpar(ref cm, o);
                    rsl = cm.snapsScalarStrAsync().Result;
                    if (cm.snapsScalarStrAsync().Result != "pass")
                    {
                        throw new Exception(rsl);
                    }else
                    {
                        if (!string.IsNullOrEmpty(o.spcproduct))
                        {
                            try
                            {
                                cm.CommandText = sqlzoneprep_getratio;
                                rtoskuofpu = cm.snapsScalarStrAsync().Result.ToString().CInt32();
                            }
                            catch (Exception)
                            {
                                throw new Exception("Article incorrect ");
                            }
                        }
                        else
                        {
                            rtoskuofpu = 0;
                        }
                        cm.CommandText = sqlzoneprln_update;
                        await cm.snapsExecuteAsync();
                    }

                }                
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }

        //remove line prep 
        public async Task removeAsync(zoneprln_md o){ 
            SqlCommand cm = new SqlCommand(sqlzoneprln_remove,cn);
            try {
                fillPLpar(ref cm, o);
                cm.CommandText = sqlzoneprln_remval;
                if (cm.snapsScalarStrAsync().Result != "0"){
                    throw new Exception("Locatin still using");
                }else { 
                    if (string.IsNullOrEmpty(o.spcproduct) && o.spcarea == "XD")
                    {
                        cm.CommandText = sqlzoneprln_remove2;                         
                    }
                    else
                    {
                        cm.CommandText = sqlzoneprln_remove;
                    }
                    
                    await cm.snapsExecuteAsync();
                }                
            }catch (Exception ex){
                throw ex;   
            }finally {
                await cm.DisposeAsync();
            }
        }

    }
}