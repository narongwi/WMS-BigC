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

    public partial class parameter_ops : IDisposable { 
        private bool disposedValue = false;
        private SqlConnection cn = new SqlConnection();

        public parameter_ops(){ }
        public parameter_ops(String connection){ this.cn.ConnectionString = connection; }
        public parameter_ops(SqlConnection connection){ this.cn = connection; }
        public async Task<List<pam_parameter>> getParameterListAsync(String orgcode, String site, String depot, String pmmodule,String type ) { 
            SqlCommand cm = sqlfind.snapsCommand(cn);
            SqlDataReader r;
            List<pam_parameter> rn = new List<pam_parameter>();
            try { 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsCdn(pmmodule,"pmmodule"," and pmmodule = @pmmodule ");
                cm.snapsCdn(type,"pmtype"," and pmtype = @pmtype");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { 
                    rn.Add(new pam_parameter(ref r)); 
                } await r.CloseAsync();
                return rn;
            }catch (Exception ex) { throw ex; } 
            finally { cm.Dispose(); } 
        }
        public List<pam_set> getParameterAsync(List<pam_parameter> o) { 
            List<pam_set> rn = new List<pam_set>();
            try {                 
                o.ForEach(x=>{ rn.Add(new pam_set(x)); });
                return rn;
            }catch (Exception ex) { throw ex; }            
        }
        public async Task updateParameterAsync(pam_parameter o) {
            SqlCommand cm = new SqlCommand();
            try {                 
                cm = getCommand(o);
                await cm.snapsExecuteAsync();
            }catch (Exception ex) { throw ex; }
            finally { cm.Dispose(); }
        }

        //Inbound Process
        public async Task<pam_inbound> getInbound(String orgcode, String site, String depot){ 
            //List<pam_parameter> rn = new List<pam_parameter>();
            try{ 
                var rn = await this.getParameterListAsync(orgcode,site,depot,"inbound","receipt");
                return new pam_inbound(rn );
                //return getParameterAsync(rn); 
            }catch (Exception ex) { 
                throw ex;
            }
        }

        //Barcode
        public List<pam_set> getBarcode(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"barcode","").Result);            
        }

        //Correction 
        public List<pam_set> getCorrection(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"inventory","correction").Result);            
        }
        
        //Outbound
        public List<pam_set> getOutbound(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"outbound","").Result);            
        }
        public List<pam_set> getOutbound_allowcate(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"outbound","allocate").Result);            
        }
        public List<pam_set> getOutbound_shipment(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"outbound","shipment").Result);            
        }


        //Preparation
        public List<pam_set> getPreparation(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"preparation","").Result);            
        }
        public List<pam_set> getPreparation_stock(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"preparation","prepstock").Result);            
        }
        public List<pam_set> getPreparation_stockmobile(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"preparation","prepstock_mobile").Result);            
        }

        public List<pam_set> getPreparation_dist(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"preparation","prepdist").Result);            
        }
        public List<pam_set> getPreparation_distmobile(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"preparation","prepdist_mobile").Result);            
        }

        //Product
        public List<pam_set> getProduct(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"product","").Result);            
        }

        //Task movement 
        public List<pam_set> getTaskputaway(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"task","putaway").Result);            
        }
        public List<pam_set> getTaskapproach(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"task","approach").Result);            
        }
        public List<pam_set> getTaskreplenishment(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"task","replenishment").Result);            
        }

        //Third party
        public List<pam_set> getThirdparty(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"thirdparty","").Result);            
        }


        public List<pam_set> getTransfer(String orgcode, String site, String depot) { 
            return this.getParameterAsync(this.getParameterListAsync(orgcode,site,depot,"inventory","Transfer").Result);            
        }
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                if (cn != null) { cn.Dispose(); }
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}