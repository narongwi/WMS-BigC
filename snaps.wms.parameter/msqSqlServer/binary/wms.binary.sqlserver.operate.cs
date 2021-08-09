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
using Snaps.WMS.parameter;

namespace Snaps.WMS.warehouse {

    public partial class binary_ops : IDisposable { 

        //Description
        public async Task<List<binary_md>> descAsync(binary_md o) { 
            SqlCommand cm = new SqlCommand(sqlbinary_desc,cn);
            SqlDataReader r = null;
            List<binary_md> rn = new List<binary_md>();
            try {
                Fillpars(ref cm, o);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(Fill(ref r)); }
                await r.
                CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                 if (r!=null){ await r.DisposeAsync();}
            }
        }
        //list zone prep
        public async Task<List<binary_md>> listAsync(binary_md o) { 
            SqlCommand cm = new SqlCommand(sqlbinary_find,cn);
            SqlDataReader r = null;
            List<binary_md> rn = new List<binary_md>();
            try {
                Fillpars(ref cm, o);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(Fill(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                 if (r!=null){ await r.DisposeAsync();}
            }
        }
        
        //upsert zone prep
        public async Task upsertAsync(binary_md o) { 
            SqlCommand cm = new SqlCommand(sqlbinary_validate, cn);
            try { 
                Fillpars(ref cm, o);
                cm.CommandText = (cm.snapsScalarStrAsync().Result == "1") ? sqlbinary_update : sqlbinary_insert;
                await cm.snapsExecuteAsync();
            }catch (Exception ex){ 
                throw ex;
            }finally { 
                await cm.DisposeAsync();
            }
        }

        //remove zone prep 
        public async Task removeAsync(binary_md o){ 
            SqlCommand cm = new SqlCommand(sqlbinary_remove,cn);
            try {
                Fillpars(ref cm, o);
                cm.CommandText = sqlbinary_remove;
                await cm.snapsExecuteAsync();                              
            }catch (Exception ex){
                throw ex;   
            }finally {
                await cm.DisposeAsync();
            }
        }

        //get LOV
        public async Task<List<lov>> lovAsync(binary_md o,Boolean isdesc = true) { 
            SqlCommand cm = new SqlCommand(sqlbinary_find,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                Fillpars(ref cm, o);
                r = await cm.snapsReadAsync();
                if (isdesc == true){
                    while(await r.ReadAsync()) { rn.Add(FillLOV(ref r)); }
                }else { 
                    while(await r.ReadAsync()) { rn.Add(FillLOVmaster(ref r)); }                    
                }
                
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }


        public async Task<List<lov>> lovAsync(binary_md o) { 
            SqlCommand cm = new SqlCommand(sqlbinary_find,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                Fillpars(ref cm, o);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(FillLOV(ref r)); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }

        public async Task<List<lov>> lovwarehouseAsync() {
            SqlCommand cm = new SqlCommand(sqllov_warehouseall,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["sitecode"].ToString(),r["sitenamealt"].ToString(),r["depotcode"].ToString(),r["depotname"].ToString(),"")); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                if(r != null) { await r.CloseAsync(); }
                await cn.CloseAsync();
                throw ex;
            } finally {
                await cm.DisposeAsync();
                if(r != null) { await r.DisposeAsync(); }
            }
        }

        //Warehouse 
        public async Task<List<lov>> lovwarehouseAsync(string orgcode,string site, string depot){
            SqlCommand cm = new SqlCommand(sqllov_warehouse,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                //cm.snapsPar(site,"site");
                //cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["sitecode"].ToString(),r["sitenamealt"].ToString(),r["depotcode"].ToString(),r["depotname"].ToString(),"")); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //depot 
        public async Task<List<lov>> lovdepotAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_depot,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["depotcode"].ToString(),r["depotnamealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //role
        public async Task<List<lov>> lovroleAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_role,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["rolecode"].ToString(),r["rolename"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //binary 
        public async Task<List<lov>> lovAsync(string orgcode, string site, string depot, string bntype,string bncode,Boolean isdesc = true){
            SqlCommand cm = new SqlCommand(sqllov_find,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(bntype,"bntype");
                cm.snapsPar(bncode,"bncode");
                r = await cm.snapsReadAsync();
                if (isdesc == true) { 
                    while(await r.ReadAsync()) { 
                        rn.Add(new lov(r["bnvalue"].ToString(),r["bndescalt"].ToString(),r["bnflex1"].ToString(),r["bnflex2"].ToString(), r["bnflex3"].ToString(), r["bnflex4"].ToString(), r["bnicon"].ToString())); 
                    }
                }else { 
                     while(await r.ReadAsync()) { 
                        rn.Add(new lov(r["bnvalue"].ToString(),r["bndesc"].ToString(),r["bnflex1"].ToString(),r["bnflex2"].ToString(), r["bnflex3"].ToString(), r["bnflex4"].ToString(), r["bnicon"].ToString())); 
                    }
                }

                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //prep zone stock 
        public async Task<List<lov>> lovprepzonestockAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_prepzonestock,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["przone"].ToString(),r["przonename"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //prep zone distribute 
        public async Task<List<lov>> lovprepzonedistAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_prepzonedist,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["przone"].ToString(),r["przonename"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //storage zone 
        public async Task<List<lov>> lovstoragezoneAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_storagecount,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { 
                    rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString(),r["spcarea"].ToString(),r["fltype"].ToString(),"")); 
                }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        
        //share distribute 
        public async Task<List<lov>> lovsharedistAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_sharedist,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["shprep"].ToString(),r["shprepname"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }

        //Handerling unit type
        public async Task<List<lov>> lovhuAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_hutype,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["article"].ToString(),r["descalt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }


        //Staing Inbound
        public async Task<List<lov>> lovstaginginbAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_staginginb,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //Staging Outbound       
        public async Task<List<lov>> lovstagingoubAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_stagingoub,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }

        //Bulk
        public async Task<List<lov>> lovbulkAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_bulk,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }
        //Damage

        public async Task<List<lov>> lovdamageAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_damage,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }

        //Sinbin
        public async Task<List<lov>> lovsinbinAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_sinbin,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }


        //Exchange 
        public async Task<List<lov>> lovexchangeAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_exchange,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }

        //Overflow 
        public async Task<List<lov>> lovoverflowAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_overflow,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }

        //Return 
        public async Task<List<lov>> lovreturnAsync(string orgcode,string site, string depot) { 
            SqlCommand cm = new SqlCommand(sqllov_return,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["lscode"].ToString(),r["lscodealt"].ToString())); }
                await r.CloseAsync();
                await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                if (r!=null){ await r.CloseAsync();}
                 await cn.CloseAsync();
                throw ex;
            }finally { 
                await cm.DisposeAsync();
                if (r!=null){ await r.DisposeAsync();}
            }
        }


        //Validate location 

        //Aisle
        public Boolean valaisle(string orgcode, string site, string depot, string loc){ 
            SqlCommand cm = new SqlCommand(sqlvalloc_aisle, cn);
            try { 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(loc,"loc");
                return (cm.snapsScalarStrAsync().Result.ToString() == "0") ? false : true;
            }catch(Exception ex){ throw ex; }
            finally { cn.Dispose(); }
        }

        //Bay
        public Boolean valbay(string orgcode, string site, string depot, string loc){ 
            SqlCommand cm = new SqlCommand(sqlvalloc_bay, cn);
            try { 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(loc,"loc");
                return (cm.snapsScalarStrAsync().Result.ToString() == "0") ? false : true;
            }catch(Exception ex){ throw ex; }
            finally { cn.Dispose(); }
        }

        //Level
        public Boolean vallevel(string orgcode, string site, string depot, string loc){ 
            SqlCommand cm = new SqlCommand(sqlvalloc_level, cn);
            try { 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(loc,"loc");
                return (cm.snapsScalarStrAsync().Result.ToString() == "0") ? false : true;
            }catch(Exception ex){ throw ex; }
            finally { cn.Dispose(); }
        }

        //Location
        public Boolean vallocation(string orgcode, string site, string depot, string loc){ 
            SqlCommand cm = new SqlCommand(sqlvalloc_location, cn);
            try { 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(loc,"loc");
                return (cm.snapsScalarStrAsync().Result.ToString() == "0") ? false : true;
            }catch(Exception ex){ throw ex; }
            finally { cn.Dispose(); }
        }
    }
}