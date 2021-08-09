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
        //Crunch interface
        public void crunch_barcode() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            SqlCommand cm = new SqlCommand("",on);
            SqlCommand em = new SqlCommand("",on);
            DataTable dt = new DataTable();
            List<SqlCommand> lcm = new List<SqlCommand>();
            string errsql = "";
            try {
                //Read Interface object
                cm.CommandText = sqlInterface_crunching_barcode;
                em.CommandText = sqlInterface_crunching_barcode_error;
                dt = cm.snapsTableAsync().Result;
                lcm.Add(sqlInterface_crunching_barcode_insert.snapsCommand(cn));
                lcm.Add(sqlInterface_crunching_barcode_clear.snapsCommand(cn));
                lcm.ForEach(x=> { x.snapsPar(0,"rowid"); x.snapsPar(0, "site"); });
                em.snapsPar("","ermsg"); em.snapsPar(0,"rowid");
                foreach(DataRow rw in dt.Rows){
                    if (rw["rowops"].ToString()=="1") {
                        lcm[0].CommandText = sqlInterface_crunching_barcode_insert;
                    }else if (rw["rowops"].ToString()=="2") {
                        lcm[0].CommandText = sqlInterface_crunching_barcode_update;
                    }else if (rw["rowops"].ToString()=="3") {
                        lcm[0].CommandText = sqlInterface_crunching_barcode_cancel;
                    }

                    if (rw["rowops"].ToString() =="9"){
                        em.Parameters["rowid"].Value = rw["rowid"].ToString();
                        em.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        em.snapsExecute();
                    } else if(rw["rowops"].ToString() == "4") {
                        // Delete Article Only
                        using(var oon = new OracleConnection(cnx_legacysource)) {
                            oon.Open();
                            using(OracleCommand ocm = new OracleCommand(sqlInterface_crunching_article_cleargc, oon)) {
                                ocm.Parameters.Add(new OracleParameter("site", Convert.ToInt32(rw["site"])));
                                ocm.Parameters.Add(new OracleParameter("depot", Convert.ToInt32(rw["depot"])));
                                ocm.Parameters.Add(new OracleParameter("article", rw["article"].ToString()));
                                ocm.Parameters.Add(new OracleParameter("lv", Convert.ToInt32(rw["lv"].ToString())));
                                ocm.ExecuteNonQuery();
                            }
                        }
                    } else {
                        try {
                            lcm.ForEach(x=> { x.Parameters["rowid"].Value = rw["rowid"].ToString(); });
                            lcm.snapsExecuteTrans(cn, ref errsql);

                            // Interface Completed
                            using(var oon = new OracleConnection(cnx_legacysource)) {
                                oon.Open();
                                using(OracleCommand ocm = new OracleCommand(sqlInterface_crunching_barcode_cleargc, oon)) {
                                    ocm.Parameters.Add(new OracleParameter("site", Convert.ToInt32(rw["site"])));
                                    ocm.Parameters.Add(new OracleParameter("depot", Convert.ToInt32(rw["depot"])));
                                    ocm.Parameters.Add(new OracleParameter("article", rw["article"].ToString()));
                                    ocm.Parameters.Add(new OracleParameter("lv", Convert.ToInt32(rw["lv"].ToString())));
                                    ocm.Parameters.Add(new OracleParameter("barcode", rw["barcode"].ToString()));
                                    ocm.ExecuteNonQuery();
                                }
                            }

                        } catch (Exception exl){
                            em.Parameters["rowid"].Value = rw["rowid"].ToString();
                            em.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                            em.snapsExecute();
                        }
                    }
                }

            }catch(Exception ex){
                logger.Error("91917", "01", "crunch_barcode", ex, ex.Message);
                throw ex;
            }finally {
                cn.Dispose(); lcm.ForEach(x=> x.Dispose());
                on.Dispose();
            }
        }
        public void crunch_inbound(){
            // WMS Database
            SqlConnection on = new SqlConnection(cnx_snapswms);
            SqlCommand cm = new SqlCommand("",on);
            SqlCommand em = new SqlCommand("",on);
            //SqlCommand el = new SqlCommand("",on);
            DataTable dt = new DataTable();
            List<SqlCommand> lcm = new List<SqlCommand>();
            string errsql = "";
            try {
                // Header Read Interface object
                cm.CommandText = sqlInterface_crunching_inbound;
                em.CommandText = sqlInterface_crunching_inbound_error;
                //el.CommandText = sqlInterface_crunching_inbound_lnerror;
                dt = cm.snapsTableAsync().Result;
                lcm.Add(sqlInterface_crunching_inbound_insert.snapsCommand(cn));
                lcm.Add(sqlInterface_crunching_inbound_clear.snapsCommand(cn));
                lcm.ForEach(x=> { x.snapsPar(0,"rowid"); });
                em.snapsPar("","ermsg");
                em.snapsPar(0,"rowid");

                //el.snapsPar("", "ermsg");
                //el.snapsPar(0, "rowid");
                foreach(DataRow rw in dt.Rows){
                    if(rw["rowops"].ToString() == "1") {
                        lcm[0].CommandText = sqlInterface_crunching_inbound_insert;
                    } else if(rw["rowops"].ToString() == "2") {
                        lcm[0].CommandText = sqlInterface_crunching_inbound_update;
                    } else if(rw["rowops"].ToString() == "3") {
                        lcm[0].CommandText = sqlInterface_crunching_inbound_cancel;
                    }

                    if (rw["rowops"].ToString() =="9"){
                        em.Parameters["rowid"].Value = rw["rowid"].ToString();
                        em.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        em.snapsExecute();

                        // Cancel Pending Interface Line
                        //el.Parameters["rowid"].Value = rw["rowid"].ToString();
                        //el.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        //el.snapsExecute();
                    } else {
                        try {
                            lcm.ForEach(x=> { x.Parameters["rowid"].Value = rw["rowid"].ToString(); });
                            lcm.snapsExecuteTrans(cn, ref errsql);
                            // Interface Completed
                            using(var oon = new OracleConnection(cnx_legacysource)) {
                                oon.Open();
                                using(OracleCommand ocm = new OracleCommand(sqlInterface_crunching_inbound_cleargc, oon)) {
                                    ocm.Parameters.Add(new OracleParameter("site", Convert.ToInt32(rw["site"])));
                                    ocm.Parameters.Add(new OracleParameter("depot", Convert.ToInt32(rw["depot"])));
                                    ocm.Parameters.Add(new OracleParameter("inorder", rw["inorder"].ToString()));
                                    ocm.ExecuteNonQuery();
                                }
                            }
                        } catch (Exception exl){
                            em.Parameters["rowid"].Value = rw["rowid"].ToString();
                            em.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                            em.snapsExecute();

                            // Cancel Pending Interface Line
                            // el.Parameters["rowid"].Value = rw["rowid"].ToString();
                            // el.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0, 100) : exl.Message.ToString();
                            // el.snapsExecute();
                        }
                   }
                }// ./ forloop

                List<OracleCommand> lom = new List<OracleCommand>();
                cm.CommandText = sqlInterface_crunching_inbouln;
                em.CommandText = sqlInterface_crunching_inbouln_error;
                dt = cm.snapsTableAsync().Result;
                lcm[0].CommandText = sqlInterface_crunching_inbouln_insert;
                lcm[1].CommandText = sqlInterface_crunching_inbouln_clear;
                foreach(DataRow rw in dt.Rows){
                    if (rw["rowops"].ToString()=="1") {
                        lcm[0].CommandText = sqlInterface_crunching_inbouln_insert;
                    }else if (rw["rowops"].ToString()=="2") {
                        lcm[0].CommandText = sqlInterface_crunching_inbouln_update;
                    }else if (rw["rowops"].ToString()=="3") {
                        lcm[0].CommandText = sqlInterface_crunching_inbouln_cancel;
                    }

                    if (rw["rowops"].ToString() =="9"){
                        em.Parameters["rowid"].Value = rw["rowid"].ToString();
                        em.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        em.snapsExecute();
                    }else {
                        try {
                            lcm.ForEach(x=> { x.Parameters["rowid"].Value = rw["rowid"].ToString(); });
                            lcm.snapsExecuteTrans(cn, ref errsql);
                            // Interface Completed
                            using(var oon = new OracleConnection(cnx_legacysource)) {
                                oon.Open();
                                using(OracleCommand ocm = new OracleCommand(sqlInterface_crunching_inbouln_cleargc, oon)) {
                                    ocm.Parameters.Add(new OracleParameter("site", Convert.ToInt32(rw["site"])));
                                    ocm.Parameters.Add(new OracleParameter("depot", Convert.ToInt32(rw["depot"])));
                                    ocm.Parameters.Add(new OracleParameter("inorder", rw["inorder"].ToString()));
                                    ocm.Parameters.Add(new OracleParameter("article", rw["article"].ToString()));
                                    ocm.ExecuteNonQuery();
                                }
                            }

                        } catch (Exception exl){
                            em.Parameters["rowid"].Value = rw["rowid"].ToString();
                            em.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                            em.snapsExecute();
                        }
                   }
                }
            } catch(Exception ex){
                logger.Error("91917", "01", "crunch_inbound", ex, ex.Message);
                throw ex;
            }finally {
                cn.Dispose(); lcm.ForEach(x=> x.Dispose());
                on.Dispose();
            }
        }
        public void InterfaceCompleted(List<OracleCommand> oracleCommands) {
            if(oracleCommands.Count > 0) {
                using(var oon = new OracleConnection(cnx_legacysource)) {
                    oon.Open();
                    //var otx = oon.BeginTransaction(IsolationLevel.ReadCommitted);
                    try {
                        foreach(var cmd in oracleCommands) {
                            //cmd.Transaction = otx;
                            cmd.Connection = oon;
                            cmd.ExecuteNonQuery();
                        }
                        //otx.Commit();
                    } catch(Exception) {
                        //otx.Rollback();
                    }
                }
            }
        }
        public void crunch_outbound(){
            SqlConnection on = new SqlConnection(cnx_snapswms);
            SqlCommand cm = new SqlCommand("",on);
            SqlCommand em = new SqlCommand("",on);
            SqlCommand dn = new SqlCommand("", on);
            //SqlCommand el = new SqlCommand("", on);

            DataTable dt = new DataTable();

            List<orbit_outbouln> rn = new List<orbit_outbouln>();
            List<SqlCommand> lcm = new List<SqlCommand>();

            string errsql = "";
            try {
                //Read Interface object
                cm.CommandText = sqlInterface_crunching_outbound;
                em.CommandText = sqlInterface_crunching_outbound_error;
                //el.CommandText = sqlInterface_crunching_outbound_lnerror;
                dt = cm.snapsTableAsync().Result;
                lcm.Add(sqlInterface_crunching_outbound_insert.snapsCommand(cn));
                lcm.Add(sqlInterface_crunching_outbound_clear.snapsCommand(cn));
                lcm.ForEach(x=> { x.snapsPar(0,"rowid"); });
                em.snapsPar("","ermsg");
                em.snapsPar(0,"rowid");
                //el.snapsPar("", "ermsg");
                //el.snapsPar(0, "rowid");
                foreach(DataRow rw in dt.Rows){
                    if (rw["rowops"].ToString() =="9"){
                        em.Parameters["rowid"].Value = rw["rowid"].ToString();
                        em.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        em.snapsExecute();

                        // Cancel Interface Line
                        //el.Parameters["rowid"].Value = rw["rowid"].ToString();
                        //el.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        //el.snapsExecute();
                    } else {
                        try {
                            if(rw["rowops"].ToString() == "1") {
                                lcm[0].CommandText = sqlInterface_crunching_outbound_insert;
                            } else if(rw["rowops"].ToString() == "2") {
                                lcm[0].CommandText = sqlInterface_crunching_outbound_update;
                            } else if(rw["rowops"].ToString() == "3") {
                                //lcm[0].CommandText = sqlInterface_crunching_outbouln_delete;
                                lcm[0].CommandText = sqlInterface_crunching_outbound_cancel;
                            } else {
                                throw new Exception($"rowops {rw["rowops"]}  is not support");
                            }
                            lcm.ForEach(x=> { x.Parameters["rowid"].Value = rw["rowid"].ToString(); });
                            lcm.snapsExecuteTrans(cn, ref errsql);
                        } catch (Exception exl){
                            em.Parameters["rowid"].Value = rw["rowid"].ToString();
                            em.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                            em.snapsExecute();

                            // Cancel Interface Line
                            //el.Parameters["rowid"].Value = rw["rowid"].ToString();
                            //el.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                            //el.snapsExecute();
                        }
                   }
                }

                cm.CommandText = sqlInterface_crunching_outbouln;
                em.CommandText = sqlInterface_crunching_outbouln_error;
                dt = cm.snapsTableAsync().Result;

                // Distinct Order In line
                var orders = dt.AsEnumerable()
                    .Where(s=>s["rowops"].ToString()=="1")
                    .Select(s => new orbit_outbound_seq(s["site"], s["depot"], s["ouorder"], 0))
                    .Distinct().ToList();

                // Generate Dono
                var lsoudono = new List<orbit_outbound_seq>();
                foreach(var order in orders) {
                    dn.CommandText = sqlInterface_crunching_outbound_oudono;
                    order.oudono = Convert.ToInt32(dn.snapsScalarStrAsync().Result);
                    lsoudono.Add(order);
                }
                // initial command text
                lcm[0].CommandText = sqlInterface_crunching_outbouln_insert;
                lcm[1].CommandText = sqlInterface_crunching_outbouln_clear;
                lcm[0].snapsPar("","oudono");
                foreach(DataRow rw in dt.Rows){

                    if(rw["rowops"].ToString() == "9") {
                        em.Parameters["rowid"].Value = rw["rowid"].ToString();
                        em.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        em.snapsExecute();
                    } else {
                        try {
                            if(rw["rowops"].ToString() == "1") {
                                lcm[0].CommandText = sqlInterface_crunching_outbouln_insert;
                                var oudo = lsoudono.Where(x => x.site == $"{rw["site"]}" && x.depot == $"{rw["depot"]}" && x.ouorder == $"{rw["ouorder"]}").FirstOrDefault();
                                lcm[0].Parameters["oudono"].Value = oudo == null ? 0 : oudo.oudono;
                            } else if(rw["rowops"].ToString() == "2") {
                                lcm[0].CommandText = sqlInterface_crunching_outbouln_update;
                            } else if(rw["rowops"].ToString() == "3") {
                                //lcm[0].CommandText = sqlInterface_crunching_outbouln_delete;
                                lcm[0].CommandText = sqlInterface_crunching_outbouln_cancel;
                            } else {
                                throw new Exception($"rowops {rw["rowops"]} is not support");
                            }

                            lcm.ForEach(x=> { x.Parameters["rowid"].Value = rw["rowid"].ToString(); });
                            lcm.snapsExecuteTrans(cn, ref errsql);

                            // Interface Completed
                            using(var oon = new OracleConnection(cnx_legacysource)) {
                                oon.Open();
                                using(OracleCommand ocm = new OracleCommand(sqlInterface_crunching_outbouln_cleargc, oon)) {
                                    ocm.Parameters.Add(new OracleParameter("site", Convert.ToInt32(rw["site"])));
                                    ocm.Parameters.Add(new OracleParameter("depot", Convert.ToInt32(rw["depot"])));
                                    ocm.Parameters.Add(new OracleParameter("ouorder", rw["ouorder"].ToString()));
                                    ocm.Parameters.Add(new OracleParameter("article", rw["article"].ToString()));
                                    ocm.ExecuteNonQuery();
                                }
                            }

                        } catch (Exception exl){
                           em.Parameters["rowid"].Value = rw["rowid"].ToString();
                           em.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                           em.snapsExecute();
                        }
                    }
                }
            }catch(Exception ex){
                logger.Error("91917", "01", "crunch_outbound", ex, ex.Message);
                throw ex;
            }finally {
                cn.Dispose(); lcm.ForEach(x=> x.Dispose());
                on.Dispose();
                //rn.Clear();
            }

        }

        public void crunch_product() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            SqlCommand cm = new SqlCommand(sqlInterface_crunching_product,on);
            SqlCommand em = new SqlCommand(sqlInterface_crunching_product_error,on);
            DataTable dt = new DataTable();

            List<orbit_product> rn = new List<orbit_product>();
            List<SqlCommand> lcm = new List<SqlCommand>();

            string errsql = "";
            try {
                //Read Interface object
                dt = cm.snapsTableAsync().Result;
                lcm.Add(sqlInterface_crunching_product_insert.snapsCommand(cn));
                lcm.Add(sqlInterface_crunching_product_clear.snapsCommand(cn));
                lcm.ForEach(x=> { x.snapsPar(0,"rowid"); });
                em.snapsPar("","ermsg"); em.snapsPar(0,"rowid");
                foreach(DataRow rw in dt.Rows){
                    if (rw["rowops"].ToString()=="1") {
                        lcm[0].CommandText = sqlInterface_crunching_product_insert;
                    }else if (rw["rowops"].ToString()=="2") {
                        lcm[0].CommandText = sqlInterface_crunching_product_update;
                    }else if (rw["rowops"].ToString()=="3") {
                        lcm[0].CommandText = sqlInterface_crunching_product_cancel;
                    }

                    if (rw["rowops"].ToString() =="9"){
                        em.Parameters["rowid"].Value = rw["rowid"].ToString();
                        em.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        em.snapsExecute();
                    }else {
                        try {
                            lcm.ForEach(x=> { x.Parameters["rowid"].Value = rw["rowid"].ToString(); });
                            lcm.snapsExecuteTrans(cn, ref errsql);

                        }catch (Exception exl){
                            em.Parameters["rowid"].Value = rw["rowid"].ToString();
                            em.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                            em.snapsExecute();
                        }
                   }
                }
            }catch(Exception ex){
                logger.Error("91917", "01", "crunch_product", ex, ex.Message);
                throw ex;
            }finally {
                cn.Dispose(); lcm.ForEach(x=> x.Dispose());
                on.Dispose();
                rn.Clear();
            }
        }
        public void crunch_thirdparty() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            SqlCommand cm = new SqlCommand(sqlInterface_crunching_thirdparty,on);
            SqlCommand em = new SqlCommand(sqlInterface_crunching_thirdparty_error,on);
            DataTable dt = new DataTable();

            List<orbit_thirdparty> rn = new List<orbit_thirdparty>();
            List<SqlCommand> lcm = new List<SqlCommand>();

            string errsql = "";
            try {
                //Read Interface object
                dt = cm.snapsTableAsync().Result;
                lcm.Add(sqlInterface_crunching_thirdparty_insert.snapsCommand(cn));
                lcm.Add(sqlInterface_crunching_thirdparty_clear.snapsCommand(cn));
                lcm.ForEach(x=> { x.snapsPar(0,"rowid"); });
                em.snapsPar("","ermsg"); em.snapsPar(0,"rowid");
                foreach(DataRow rw in dt.Rows){
                    if (rw["rowops"].ToString()=="1") {
                        lcm[0].CommandText = sqlInterface_crunching_thirdparty_insert;
                    }else if (rw["rowops"].ToString()=="2") {
                        lcm[0].CommandText = sqlInterface_crunching_thirdparty_update;
                    }else if(rw["rowops"].ToString() == "3") {
                        lcm[0].CommandText = sqlInterface_crunching_thirdparty_cancel;
                    }

                    if(rw["rowops"].ToString() == "9") {
                        em.Parameters["rowid"].Value = rw["rowid"].ToString();
                        em.Parameters["ermsg"].Value = rw["ermsg"].ToString();
                        em.snapsExecute();
                    } else {
                        try {
                            lcm.ForEach(x => { x.Parameters["rowid"].Value = rw["rowid"].ToString(); });
                            lcm.snapsExecuteTrans(cn, ref errsql);

                            // delete Interface Completed
                            using(var oon = new OracleConnection(cnx_legacysource)) {
                                oon.Open();
                                using(OracleCommand ocm = new OracleCommand(sqlInterface_crunching_thirdparty_cleargc, oon)) {
                                    ocm.Parameters.Add(new OracleParameter("tsite", Convert.ToInt32(rw["site"])));
                                    ocm.Parameters.Add(new OracleParameter("tdepot", Convert.ToInt32(rw["depot"])));
                                    ocm.Parameters.Add(new OracleParameter("tcode", rw["thcode"].ToString()));
                                    ocm.ExecuteNonQuery();
                                }
                            }
                        } catch(Exception exl) {
                            em.Parameters["rowid"].Value = rw["rowid"].ToString();
                            em.Parameters["ermsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0, 100) : exl.Message.ToString();
                            em.snapsExecute();
                        }
                    }
                }
            }catch(Exception ex){
                logger.Error("91917", "01", "crunch_thirdparty", ex, ex.Message);
                throw ex;
            }finally {
                cn.Dispose(); lcm.ForEach(x=> x.Dispose());
                on.Dispose();
                rn.Clear();
            }
        }
    }
}