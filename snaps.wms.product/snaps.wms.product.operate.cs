using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.product
{
    
    public partial class product_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_product";
        private static string sqlmcom = " and orgcode = @orgcode and site = @site  and depot = @depot and isnull(spcarea,'') = isnull(@spcarea,'') and article = @article and pv = @pv  and lv = @lv " ;

        public product_ops() {  }
        public product_ops(String cx) { cn = new SqlConnection(cx); }

        

        public async Task<List<product_ls>> find(product_pm o) { 
            SqlCommand cm = null;
            List<product_ls> rn = new List<product_ls>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                 cm = (sqlFind_product).snapsCommand(cn);
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsCdn(o.article,"article", " and p.article = @article ");
                cm.snapsCdn(o.thcode,"thcode", " and p.thcode = @thcode ");
                cm.snapsCdn(o.descalt,"descalt", " and p.descalt like '%@descalt%' ");

                if (o.ismeasurement == 1) { cm.snapsCdn(o.ismeasurement,"ismeasurement"," and p.ismeasurement = @ismeasurement "); }

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<product_active> findActive(product_pm o)
        {
            SqlCommand cm = null;
            product_active rn = new product_active();
            SqlDataReader r = null;
            try
            {
                cm = sqlFind_productact.snapsCommand(cn);
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");
                cm.snapsPar(o.article, "productCode");
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync())
                {
                    rn = fillat(ref r);
                }

                await r.CloseAsync(); 
                await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }
        public async Task<product_md> get(product_ls o){ 
            SqlCommand cm = null; SqlDataReader r = null;
            product_md rn = new product_md();
            try { 
                /* Vlidate parameter */
                cm = (sqlGet_product).snapsCommand(cn);
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");
                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }

        public async Task upsert(List<product_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            SqlCommand vl = new SqlCommand("",cn);

            Int32 ix=0;
            try { 
                foreach (product_md ln in rs) {
                    vl.snapsPar(ln.orgcode,"orgcode");
                    vl.snapsPar(ln.site,"site");
                    vl.snapsPar(ln.depot,"depot");
                    vl.snapsPar("","loc");
                    //Check specific location 
                    if (ln.spcrecvaisle.notNull()) { 
                        vl.CommandText = sqlvalloc_aisle;
                        vl.Parameters["loc"].Value = ln.spcrecvaisle;
                        if (vl.snapsScalarStrAsync().Result.ToString() == "0"){ 
                            throw new Exception("Specific Aisle "+ln.spcrecvaisle+" not found on Warehouse location");
                        } 
                    }
                    if (ln.spcrecvaisleto.notNull()){ 
                        vl.CommandText = sqlvalloc_aisle;
                        vl.Parameters["loc"].Value = ln.spcrecvaisleto;
                        if (vl.snapsScalarStrAsync().Result.ToString() == "0"){ 
                            throw new Exception("Specific Aisle "+ln.spcrecvaisleto+" not found on Warehouse location");
                        } 
                    }
                    if (ln.spcrecvbay.notNull()){
                        vl.CommandText = sqlvalloc_bay;
                        vl.Parameters["loc"].Value = ln.spcrecvbay;
                        if (vl.snapsScalarStrAsync().Result.ToString() == "0"){ 
                            throw new Exception("Specific Bay "+ln.spcrecvbay+" not found on Warehouse location");
                        } 
                    }
                    if(ln.spcrecvbayto.notNull()){
                        vl.CommandText = sqlvalloc_bay;
                        vl.Parameters["loc"].Value = ln.spcrecvbayto;
                        if (vl.snapsScalarStrAsync().Result.ToString() == "0"){ 
                            throw new Exception("Specific Bay "+ln.spcrecvbayto+" not found on Warehouse location");
                        } 
                    }
                    if(ln.spcrecvlevel.notNull()){
                        vl.CommandText = sqlvalloc_level;
                        vl.Parameters["loc"].Value = ln.spcrecvlevel;
                        if (vl.snapsScalarStrAsync().Result.ToString() == "0"){ 
                            throw new Exception("Specific Level "+ln.spcrecvlevel+" not found on Warehouse location");
                        } 
                    }
                    if(ln.spcrecvlevelto.notNull()){
                        vl.CommandText = sqlvalloc_level;
                        vl.Parameters["loc"].Value = ln.spcrecvlevelto;
                        if (vl.snapsScalarStrAsync().Result.ToString() == "0"){ 
                            throw new Exception("Specific Level "+ln.spcrecvlevel+" not found on Warehouse location");
                        } 
                    }
                    if(ln.spcrecvlocation.notNull()){
                        vl.CommandText = sqlvalloc_location;
                        vl.Parameters["loc"].Value = ln.spcrecvlocation;
                        if (vl.snapsScalarStrAsync().Result.ToString() == "0"){ 
                            throw new Exception("Specific Location "+ln.spcrecvlocation+" not found on Warehouse location");
                        } 
                    }
                    
                    cm.Add(obcommand(ln,sqlval)); 
                    if (cm[ix].snapsScalarStrAsync().Result == "1"){

                        cm[ix].CommandText = sqlproduct_update_hs;
                        
                        //Specific for BGC
                        ln.unitreceipt = ln.unitmanage;
                        cm.Add(obcommand(ln,sqlproduct_update)); 


                    }else { 
                        cm[ix].CommandText = sqlproduct_insert;
                    }                     
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(product_md rs){ 
            List<product_md> ro = new List<product_md>(); 
            try { 
                ro.Add(rs); 
                await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<product_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (product_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(product_md rs){
            List<product_md> ro = new List<product_md>(); 
            try { 
                ro.Add(rs); 
                await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<product_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (product_ix ln in rs) {
                    cm.Add(ixcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlproduct_insert : sqlproduct_update; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                if (cn != null) { cn.Dispose(); } sqlval = null; sqlproduct_insert = null; sqlproduct_update = null; sqlproduct_update_hs = null; sqlrem = null; sqlfnd = null; tbn = null; sqlmcom = null;
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}

