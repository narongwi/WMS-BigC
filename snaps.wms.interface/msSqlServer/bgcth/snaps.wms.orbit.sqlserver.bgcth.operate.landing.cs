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
using Oracle.ManagedDataAccess.Client;

namespace Snaps.WMS {
    public partial class orbit_ops : IDisposable { 
        //Landing to orbit for recetiptio
        public SqlCommand landing_receipt(orbit_receipt o) { 
            SqlCommand em = new SqlCommand(sqlInterface_landing_orbitsouce,cn);
             DataTable odt = new DataTable(); 
            try { 
                em.snapsPar(o.orgcode,"orgcode");
                em.snapsPar(o.site,"site");
                em.snapsPar(o.depot,"depot");
                em.snapsPar(o.inorder,"inorder");
                
                odt = em.snapsTableAsync().Result;
                if (odt.Rows.Count > 0){
                    o.orbitsite = odt.Rows[0]["orbitsite"].ToString();
                    o.orbitdepot = odt.Rows[0]["orbitdepot"].ToString();
                }else { 
                    o.orbitsite = "";
                    o.orbitdepot = "";
                }
               return receiptCommand(o);
            }catch (Exception ex) {
                logger.Error("91917", "01", "landing_receipt", ex, ex.Message);
                throw ex;
            }
            finally { 
                em.Dispose();
                odt.Dispose();
            }           

        }
        public List<SqlCommand> landing_receipt(List<orbit_receipt> o) {
            SqlCommand em = new SqlCommand(sqlInterface_landing_orbitsouce,cn);
             DataTable odt = new DataTable(); 
            List<SqlCommand> cm = new List<SqlCommand>();
            try { 
                em.snapsPar(o[0].orgcode,"orgcode");
                em.snapsPar(o[0].site,"site");
                em.snapsPar(o[0].depot,"depot");
                odt = em.snapsTableAsync().Result;
                o.ForEach(e=> { 
                    e.site = odt.Rows[0]["orbitsite"].ToString();
                    e.depot = odt.Rows[0]["orbitdepot"].ToString();
                });
               receiptCommand(ref cm, o);
               return cm;
            }catch (Exception ex) {
                logger.Error("91917", "01", "landing_receipt", ex, ex.Message);
                throw ex;
            }
            finally { 
                em.Dispose();
                odt.Dispose();
                cm.ForEach(e=> { e.Dispose(); });
            }
        }
        public void landing_receipt(List<orbit_receipt> o,ref SqlConnection cn) { 
            SqlCommand em = new SqlCommand(sqlInterface_landing_orbitsouce,cn);
            DataTable odt = new DataTable(); 
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                em.snapsPar(o[0].orgcode,"orgcode");
                em.snapsPar(o[0].site,"site");
                em.snapsPar(o[0].depot,"depot");
                odt = em.snapsTableAsync().Result;
                o.ForEach(e=> { 
                    e.site = odt.Rows[0]["orbitsite"].ToString();
                    e.depot = odt.Rows[0]["orbitdepot"].ToString();
                });
                cm = landing_receipt(o);
                cm.snapsExecuteTrans(cn);
            }catch(Exception ex){
                logger.Error("91917", "01", "landing_receipt", ex, ex.Message);
                throw ex;
            }finally { 
                em.Dispose();
                cm.ForEach(c=> c.Dispose());
            }
        }
        
        //Landing Correction
        public List<SqlCommand> landing_correction(List<orbit_correction> o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try { 
               correctionCommand(ref cm, o);
               return cm;
            }catch (Exception ex) {
                logger.Error("91917", "01", "landing_correction", ex, ex.Message);
                throw ex;
            }
        }
        public void landing_correction(List<orbit_correction> o,ref SqlConnection cn) { 
            SqlCommand em = new SqlCommand("",cn);
            List<SqlCommand> cm = new List<SqlCommand>();
            try {               
                cm = landing_correction(o);
                cm.snapsExecuteTrans(cn);
            }catch(Exception ex){ 
                throw ex;
            }finally { 
                em.Dispose();
                cm.ForEach(c=> c.Dispose());
            }
        }

        //Landing Delivery
        public List<SqlCommand> landing_delivery(List<orbit_delivery> o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try { 
               deliveryCommand(ref cm, o);
               return cm;
            }catch (Exception ex) {
                logger.Error("91917", "01", "landing_delivery", ex, ex.Message);
                throw ex;
            }
        }
        public void landing_correction(List<orbit_delivery> o,ref SqlConnection cn) { 
            SqlCommand em = new SqlCommand("",cn);
            List<SqlCommand> cm = new List<SqlCommand>();
            try {               
                cm = landing_delivery(o);
                cm.snapsExecuteTrans(cn);
            }catch(Exception ex){
                logger.Error("91917", "01", "landing_correction", ex, ex.Message);
                throw ex;
            }finally { 
                em.Dispose();
                cm.ForEach(c=> c.Dispose());
            }
        }

    }
}