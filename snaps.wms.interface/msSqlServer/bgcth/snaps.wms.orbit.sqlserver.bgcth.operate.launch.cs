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
        //luanch interface 
        public void luanch_receipt() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection(cnx_legacysource);
            SqlCommand cm = new SqlCommand("",on);
            SqlCommand em = new SqlCommand("",on);
            SqlCommand sq = new SqlCommand("",on);
            OracleCommand om = new OracleCommand("",oc);
            DataTable dt = new DataTable();
            List<SqlCommand> lcm = new List<SqlCommand>();
            try {
                //Read Interface object 
                cm.CommandText = sqlInterface_launch_receipt;
                dt = cm.snapsTableAsync().Result;

                om.BindByName = true; //Fix for error ORA-01008: not all variables bound
                om.CommandText = sqlInterface_launch_receipt_insert;
                cm.CommandText = sqlInterface_launch_receipt_clear;
                em.CommandText = sqlInterface_launch_receipt_error;
                sq.CommandText = sqlInterface_launch_receipt_grno;

                cm.snapsPar(0,"rowid");
                em.snapsPar("","xmsg"); em.snapsPar(0,"rowid");

                om.Parameters.Add("MR3CODSIT","");
                om.Parameters.Add("MR3DATMVT",OracleDbType.Date);
                om.Parameters.Add("MR3CODREC","");
                om.Parameters.Add("MR3LIBREC","");
                om.Parameters.Add("MR3DATREC",OracleDbType.Date);
                om.Parameters.Add("MR3NCDEFO","");
                om.Parameters.Add("MR3BDLFOU","");
                om.Parameters.Add("MR3NUMGPR","");
                om.Parameters.Add("MR3NUMROC","");
                om.Parameters.Add("MR3CPROIN","");
                om.Parameters.Add("MR3UVCREC","");
                om.Parameters.Add("MR3PDSREC","");
                om.Parameters.Add("MR3TYPOR","");
                om.Parameters.Add("MR3CODCLI","");
                om.Parameters.Add("MR3ARPROM","");
                om.Parameters.Add("MR3ILOGIS","");
                om.Parameters.Add("MR3FOURN","");
                om.Parameters.Add("MR3STASTK","");
                om.Parameters.Add("MR3DONORD","");
                om.Parameters.Add("MR3NDOS","");
                om.Parameters.Add("MR3DABL","");
                om.Parameters.Add("MR3TYPMVT","");
                om.Parameters.Add("MR3DEPOT","");
                om.Parameters.Add("MR3CINCDE","");
                om.Parameters.Add("MR3NOLIGN","");
                om.Parameters.Add("MR3PRVE","");
                om.Parameters.Add("MR3NLIGP","");
                om.Parameters.Add("MR3NSEQOR","");
                om.Parameters.Add("MR3DATACT",OracleDbType.Date);
                om.Parameters.Add("MR3UMCREC","");

                int row = 0;
                foreach(DataRow rw in dt.Rows) {
                    //string receiveNo = Convert.ToString(dt.Compute("MAX(MR3NSEQOR)", $"MR3NUMROC = '{rw["MR3NUMROC"]}'"));
                    //if(string.IsNullOrEmpty(receiveNo) || receiveNo == "0") {
                    //    receiveNo = Convert.ToString(sq.snapsScalarAsync().Result);
                    //}
                    // select next value for seq_recno recno
                    // dt.Rows[row]["MR3NSEQOR"] = receiveNo;

                    row++;

                    om.Parameters["MR3CODSIT"].Value = rw["MR3CODSIT"].ToString();
                    om.Parameters["MR3DATMVT"].Value = rw["MR3DATMVT"].ToString().CDateTime();
                    om.Parameters["MR3CODREC"].Value = rw["MR3CODREC"].ToString();
                    om.Parameters["MR3LIBREC"].Value = rw["MR3LIBREC"].ToString();
                    om.Parameters["MR3DATREC"].Value = rw["MR3DATREC"].ToString().CDateTime();
                    om.Parameters["MR3NCDEFO"].Value = rw["MR3NCDEFO"].ToString();
                    om.Parameters["MR3BDLFOU"].Value = rw["MR3BDLFOU"].ToString();
                    om.Parameters["MR3NUMGPR"].Value = rw["MR3NUMGPR"].ToString();
                    om.Parameters["MR3NUMROC"].Value = rw["MR3NUMROC"].ToString();
                    om.Parameters["MR3CPROIN"].Value = rw["MR3CPROIN"].ToString();
                    om.Parameters["MR3UVCREC"].Value = rw["MR3UVCREC"].ToString();
                    om.Parameters["MR3PDSREC"].Value = rw["MR3PDSREC"].ToString();
                    om.Parameters["MR3TYPOR"].Value = rw["MR3TYPOR"].ToString();
                    om.Parameters["MR3CODCLI"].Value = rw["MR3CODCLI"].ToString();
                    om.Parameters["MR3ARPROM"].Value = rw["MR3ARPROM"].ToString();
                    om.Parameters["MR3ILOGIS"].Value = rw["MR3ILOGIS"].ToString();
                    om.Parameters["MR3FOURN"].Value = rw["MR3FOURN"].ToString();
                    om.Parameters["MR3STASTK"].Value = rw["MR3STASTK"].ToString();
                    om.Parameters["MR3DONORD"].Value = rw["MR3DONORD"].ToString();
                    om.Parameters["MR3NDOS"].Value = rw["MR3NDOS"].ToString();
                    om.Parameters["MR3DABL"].Value = rw["MR3DABL"].ToString();
                    om.Parameters["MR3TYPMVT"].Value = rw["MR3TYPMVT"].ToString();
                    om.Parameters["MR3DEPOT"].Value = rw["MR3DEPOT"].ToString();
                    om.Parameters["MR3CINCDE"].Value = rw["MR3CINCDE"].ToString();
                    om.Parameters["MR3NOLIGN"].Value = rw["MR3NOLIGN"].ToString();
                    om.Parameters["MR3PRVE"].Value = rw["MR3PRVE"].ToString();
                    om.Parameters["MR3NLIGP"].Value = rw["MR3NLIGP"].ToString();
                    om.Parameters["MR3NSEQOR"].Value = rw["MR3NSEQOR"].ToString();
                    om.Parameters["MR3DATACT"].Value = rw["MR3DATACT"].ToString().CDateTime();
                    om.Parameters["MR3UMCREC"].Value = rw["MR3UMCREC"].ToString();

                    try {
                        cm.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        oc.Open();
                        om.ExecuteNonQuery();
                        oc.Close();
                        cm.snapsExecute();
                    } catch(Exception exl) {
                        oc.Close();
                        em.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        em.Parameters["xmsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                        em.snapsExecute();
                    }
                }

            } catch(Exception ex) {
                logger.Error("91917","01","luanch_receipt",ex,ex.Message);
                throw ex;
            } finally {
                cn.Dispose(); lcm.ForEach(x => x.Dispose());
                on.Dispose();
                OracleConnection.ClearPool(oc);
                oc.Dispose();
                om.Dispose();
            }
        }
        public void luanch_correction() {

            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection(cnx_legacysource);
            SqlCommand cm = new SqlCommand("",on);
            SqlCommand em = new SqlCommand("",on);
            OracleCommand om = new OracleCommand("",oc);
            DataTable dt = new DataTable();
            List<SqlCommand> lcm = new List<SqlCommand>();
            try {
                //Read Interface object 
                cm.CommandText = sqlInterface_launch_correction;
                dt = cm.snapsTableAsync().Result;

                om.BindByName = true; //Fix for error ORA-01008: not all variables bound
                om.CommandText = sqlInterface_launch_correction_insert;
                cm.CommandText = sqlInterface_launch_correction_clear;
                em.CommandText = sqlInterface_launch_correction_error;

                cm.snapsPar(0,"rowid");
                em.snapsPar("","xmsg"); em.snapsPar(0,"rowid");

                om.Parameters.Add("MI3CODSIT","");
                om.Parameters.Add("MI3DATMVT",OracleDbType.Date);
                om.Parameters.Add("MI3CPROIN","");
                om.Parameters.Add("MI3CECART","");
                om.Parameters.Add("MI3QTEUVC","");
                om.Parameters.Add("MI3CODINV","");
                om.Parameters.Add("MI3NUMORC","");
                om.Parameters.Add("MI3CODCLI","");
                om.Parameters.Add("MI3POIDS","");
                om.Parameters.Add("MI3SECTIO","");
                om.Parameters.Add("MI3STASTK","");
                om.Parameters.Add("MI3DONORD","");
                om.Parameters.Add("MI3NDOS","");
                om.Parameters.Add("MI3DABL","");
                om.Parameters.Add("MI3DEPOT","");
                om.Parameters.Add("MI3ILOGIS","");
                om.Parameters.Add("MI3NOLIGN","");
                om.Parameters.Add("MI3NLIGP","");
                om.Parameters.Add("MI3DATACT",OracleDbType.Date);
                om.Parameters.Add("MI3QTEUMC","");


                foreach(DataRow rw in dt.Rows) {

                    om.Parameters["MI3CODSIT"].Value = rw["MI3CODSIT"].ToString();
                    om.Parameters["MI3DATMVT"].Value = rw["MI3DATMVT"].ToString().CDateTime();
                    om.Parameters["MI3CPROIN"].Value = rw["MI3CPROIN"].ToString();
                    om.Parameters["MI3CECART"].Value = rw["MI3CECART"].ToString();
                    om.Parameters["MI3QTEUVC"].Value = rw["MI3QTEUVC"].ToString();
                    om.Parameters["MI3CODINV"].Value = rw["MI3CODINV"].ToString();
                    om.Parameters["MI3NUMORC"].Value = rw["MI3NUMORC"].ToString();
                    om.Parameters["MI3CODCLI"].Value = rw["MI3CODCLI"].ToString();
                    om.Parameters["MI3POIDS"].Value = rw["MI3POIDS"].ToString();
                    om.Parameters["MI3SECTIO"].Value = rw["MI3SECTIO"].ToString();
                    om.Parameters["MI3STASTK"].Value = rw["MI3STASTK"].ToString();
                    om.Parameters["MI3DONORD"].Value = rw["MI3DONORD"].ToString();
                    om.Parameters["MI3NDOS"].Value = rw["MI3NDOS"].ToString();
                    om.Parameters["MI3DABL"].Value = rw["MI3DABL"].ToString();
                    om.Parameters["MI3DEPOT"].Value = rw["MI3DEPOT"].ToString();
                    om.Parameters["MI3ILOGIS"].Value = rw["MI3ILOGIS"].ToString();
                    om.Parameters["MI3NOLIGN"].Value = rw["MI3NOLIGN"].ToString();
                    om.Parameters["MI3NLIGP"].Value = rw["MI3NLIGP"].ToString();
                    om.Parameters["MI3DATACT"].Value = rw["MI3DATACT"].ToString().CDateTime();
                    om.Parameters["MI3QTEUMC"].Value = rw["MI3QTEUMC"].ToString();

                    try {
                        cm.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        oc.Open();
                        om.ExecuteNonQuery();
                        oc.Close();
                        cm.snapsExecute();
                    } catch(Exception exl) {
                        oc.Close();
                        em.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        em.Parameters["xmsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                        em.snapsExecute();
                    }
                }

            } catch(Exception ex) {
                logger.Error("91917","01","luanch_correction",ex,ex.Message);
                throw ex;
            } finally {
                cn.Dispose(); lcm.ForEach(x => x.Dispose());
                on.Dispose();
                OracleConnection.ClearPool(oc);
                oc.Dispose();
                om.Dispose();
            }

        }
        public void luanch_delivery() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection(cnx_legacysource);
            SqlCommand cm = new SqlCommand("",on);
            SqlCommand em = new SqlCommand("",on);
            OracleCommand om = new OracleCommand("",oc);
            DataTable dt = new DataTable();
            List<SqlCommand> lcm = new List<SqlCommand>();
            try {
                //Read Interface object 
                cm.CommandText = sqlInterface_launch_delivery;
                dt = cm.snapsTableAsync().Result;
                om.BindByName = true; //Fix for error ORA-01008: not all variables bound
                om.CommandText = sqlInterface_launch_delivery_insert;
                cm.CommandText = sqlInterface_launch_delivery_clear;
                em.CommandText = sqlInterface_launch_delivery_error;

                cm.snapsPar(0,"rowid");
                em.snapsPar("","xmsg"); em.snapsPar(0,"rowid");
                om.Parameters.Add("ME3CODSIT","");
                om.Parameters.Add("ME3NUMCDE","");
                om.Parameters.Add("ME3NUMCTL","");
                om.Parameters.Add("ME3DATLIV",OracleDbType.Date);
                om.Parameters.Add("ME3CPROIN","");
                om.Parameters.Add("ME3QTEEXP","");
                om.Parameters.Add("ME3QTEALIV","");
                om.Parameters.Add("ME3QTECDE","");
                om.Parameters.Add("ME3TYPOL","");
                om.Parameters.Add("ME3CODCLI","");
                om.Parameters.Add("ME3NUMORL","");
                om.Parameters.Add("ME3CPROCD","");
                om.Parameters.Add("ME3CODEXP","");
                om.Parameters.Add("ME3NSEQBL","");
                om.Parameters.Add("ME3PDSEXP","");
                om.Parameters.Add("ME3STASTK","");
                om.Parameters.Add("ME3NDOS","");
                om.Parameters.Add("ME3DONORD","");
                om.Parameters.Add("ME3TYPMVT","");
                om.Parameters.Add("ME3FINCDE","");
                om.Parameters.Add("ME3NSETRA","");
                om.Parameters.Add("ME3DEPOT","");
                om.Parameters.Add("ME3ILOGIS","");
                om.Parameters.Add("ME3CTOURN","");
                om.Parameters.Add("ME3NLIGOL","");
                om.Parameters.Add("ME3NCOLIS","");
                om.Parameters.Add("ME3CINCDE","");
                om.Parameters.Add("ME3NOLIGN","");
                om.Parameters.Add("ME3SSCC","");
                om.Parameters.Add("ME3ILOGCD","");
                om.Parameters.Add("ME3NLIGP","");
                om.Parameters.Add("ME3DATEXP",OracleDbType.Date);
                om.Parameters.Add("ME3DATACT",OracleDbType.Date);
                om.Parameters.Add("ME3CODTRA","");
                om.Parameters.Add("ME3NLIGMAG","");
                om.Parameters.Add("ME3UMCEXP","");
                //om.Parameters.Add("ME3TIEEMB", "");

                foreach(DataRow rw in dt.Rows) {
                    om.Parameters["ME3CODSIT"].Value = rw["ME3CODSIT"].ToString();
                    om.Parameters["ME3NUMCDE"].Value = rw["ME3NUMCDE"].ToString();
                    om.Parameters["ME3NUMCTL"].Value = rw["ME3NUMCTL"].ToString();
                    om.Parameters["ME3DATLIV"].Value = rw["ME3DATLIV"].ToString().CDateTime();
                    om.Parameters["ME3CPROIN"].Value = rw["ME3CPROIN"].ToString();
                    om.Parameters["ME3QTEEXP"].Value = rw["ME3QTEEXP"].ToString();
                    om.Parameters["ME3QTEALIV"].Value = rw["ME3QTEALIV"].ToString();
                    om.Parameters["ME3QTECDE"].Value = rw["ME3QTECDE"].ToString();
                    om.Parameters["ME3TYPOL"].Value = rw["ME3TYPOL"].ToString();
                    om.Parameters["ME3CODCLI"].Value = rw["ME3CODCLI"].ToString();
                    om.Parameters["ME3NUMORL"].Value = rw["ME3NUMORL"].ToString();
                    om.Parameters["ME3CPROCD"].Value = rw["ME3CPROCD"].ToString();
                    om.Parameters["ME3CODEXP"].Value = rw["ME3CODEXP"].ToString();
                    om.Parameters["ME3NSEQBL"].Value = rw["ME3NSEQBL"].ToString();
                    om.Parameters["ME3PDSEXP"].Value = rw["ME3PDSEXP"].ToString();
                    om.Parameters["ME3STASTK"].Value = rw["ME3STASTK"].ToString();
                    om.Parameters["ME3NDOS"].Value = rw["ME3NDOS"].ToString();
                    om.Parameters["ME3DONORD"].Value = rw["ME3DONORD"].ToString();
                    om.Parameters["ME3TYPMVT"].Value = rw["ME3TYPMVT"].ToString();
                    om.Parameters["ME3FINCDE"].Value = rw["ME3FINCDE"].ToString();
                    om.Parameters["ME3NSETRA"].Value = rw["ME3NSETRA"].ToString();
                    om.Parameters["ME3DEPOT"].Value = rw["ME3DEPOT"].ToString();
                    om.Parameters["ME3ILOGIS"].Value = rw["ME3ILOGIS"].ToString();
                    om.Parameters["ME3CTOURN"].Value = rw["ME3CTOURN"].ToString();
                    om.Parameters["ME3NLIGOL"].Value = rw["ME3NLIGOL"].ToString();
                    om.Parameters["ME3NCOLIS"].Value = rw["ME3NCOLIS"].ToString();
                    om.Parameters["ME3CINCDE"].Value = rw["ME3CINCDE"].ToString();
                    om.Parameters["ME3NOLIGN"].Value = rw["ME3NOLIGN"].ToString();
                    om.Parameters["ME3SSCC"].Value = rw["ME3SSCC"].ToString();
                    om.Parameters["ME3ILOGCD"].Value = rw["ME3ILOGCD"].ToString();
                    om.Parameters["ME3NLIGP"].Value = rw["ME3NLIGP"].ToString();
                    om.Parameters["ME3DATEXP"].Value = rw["ME3DATEXP"].ToString().CDateTime();
                    om.Parameters["ME3DATACT"].Value = rw["ME3DATACT"].ToString().CDateTime();
                    om.Parameters["ME3CODTRA"].Value = rw["ME3CODTRA"].ToString();
                    om.Parameters["ME3NLIGMAG"].Value = rw["ME3NLIGMAG"].ToString();
                    om.Parameters["ME3UMCEXP"].Value = rw["ME3UMCEXP"].ToString();
                    // om.Parameters["ME3TIEEMB"].Value = rw["ME3TIEEMB"].ToString();

                    try {
                        cm.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        oc.Open();
                        om.ExecuteNonQuery();
                        oc.Close();
                        cm.snapsExecute();
                    } catch(Exception exl) {
                        oc.Close();
                        em.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        em.Parameters["xmsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                        em.snapsExecute();
                    }
                }

            } catch(Exception ex) {
                logger.Error("91917","01","luanch_delivery",ex,ex.Message);
                throw ex;
            } finally {
                cn.Dispose(); lcm.ForEach(x => x.Dispose());
                on.Dispose();
                OracleConnection.ClearPool(oc);
                oc.Dispose();
                om.Dispose();
            }
        }

        public void launch_block() {

            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection(cnx_legacysource);
            SqlCommand cm = new SqlCommand("",on);
            SqlCommand em = new SqlCommand("",on);
            OracleCommand om = new OracleCommand("",oc);
            OracleCommand ol = new OracleCommand("",oc);
            DataTable dt = new DataTable();
            List<SqlCommand> lcm = new List<SqlCommand>();
            try {
                //Read Interface object 
                cm.CommandText = sqlinterface_launch_block;

                dt = cm.snapsTableAsync().Result;

                om.BindByName = true; //Fix for error ORA-01008: not all variables bound
                ol.BindByName = true;
                om.CommandText = sqlinterface_launch_block_insert;
                ol.CommandText = sqlinterface_launch_block_insert;
                cm.CommandText = sqlinterface_launch_block_clear;
                em.CommandText = sqlinterface_launch_block_error;

                cm.snapsPar(0,"rowid");
                em.snapsPar("","xmsg"); em.snapsPar(0,"rowid");

                om.Parameters.Add("MI3CODSIT","");
                om.Parameters.Add("MI3DATMVT",OracleDbType.Date);
                om.Parameters.Add("MI3CPROIN","");
                om.Parameters.Add("MI3CECART","");
                om.Parameters.Add("MI3QTEUVC","");
                om.Parameters.Add("MI3CODINV","");
                om.Parameters.Add("MI3NUMORC","");
                om.Parameters.Add("MI3CODCLI","");
                om.Parameters.Add("MI3POIDS","");
                om.Parameters.Add("MI3SECTIO","");
                om.Parameters.Add("MI3STASTK","");
                om.Parameters.Add("MI3DONORD","");
                om.Parameters.Add("MI3NDOS","");
                om.Parameters.Add("MI3DABL","");
                om.Parameters.Add("MI3DEPOT","");
                om.Parameters.Add("MI3ILOGIS","");
                om.Parameters.Add("MI3NOLIGN","");
                om.Parameters.Add("MI3NLIGP","");
                om.Parameters.Add("MI3DATACT",OracleDbType.Date);
                om.Parameters.Add("MI3QTEUMC","");

                ol.Parameters.Add("MI3CODSIT","");
                ol.Parameters.Add("MI3DATMVT",OracleDbType.Date);
                ol.Parameters.Add("MI3CPROIN","");
                ol.Parameters.Add("MI3CECART","");
                ol.Parameters.Add("MI3QTEUVC","");
                ol.Parameters.Add("MI3CODINV","");
                ol.Parameters.Add("MI3NUMORC","");
                ol.Parameters.Add("MI3CODCLI","");
                ol.Parameters.Add("MI3POIDS","");
                ol.Parameters.Add("MI3SECTIO","");
                ol.Parameters.Add("MI3STASTK","");
                ol.Parameters.Add("MI3DONORD","");
                ol.Parameters.Add("MI3NDOS","");
                ol.Parameters.Add("MI3DABL","");
                ol.Parameters.Add("MI3DEPOT","");
                ol.Parameters.Add("MI3ILOGIS","");
                ol.Parameters.Add("MI3NOLIGN","");
                ol.Parameters.Add("MI3NLIGP","");
                ol.Parameters.Add("MI3DATACT",OracleDbType.Date);
                ol.Parameters.Add("MI3QTEUMC","");

                foreach(DataRow rw in dt.Rows) {
                    // Line 1 
                    om.Parameters["MI3CODSIT"].Value = rw["MI3CODSIT"].ToString();
                    om.Parameters["MI3DATMVT"].Value = rw["MI3DATMVT"].ToString().CDateTime();
                    om.Parameters["MI3CPROIN"].Value = rw["MI3CPROIN"].ToString();
                    om.Parameters["MI3CECART"].Value = rw["MI3CECART"].ToString();
                    om.Parameters["MI3CODINV"].Value = rw["MI3CODINV"].ToString();
                    om.Parameters["MI3NUMORC"].Value = rw["MI3NUMORC"].ToString();
                    om.Parameters["MI3CODCLI"].Value = rw["MI3CODCLI"].ToString();
                    om.Parameters["MI3SECTIO"].Value = rw["MI3SECTIO"].ToString();
                    om.Parameters["MI3DONORD"].Value = rw["MI3DONORD"].ToString();
                    om.Parameters["MI3NDOS"].Value = rw["MI3NDOS"].ToString();
                    om.Parameters["MI3DABL"].Value = rw["MI3DABL"].ToString();
                    om.Parameters["MI3DEPOT"].Value = rw["MI3DEPOT"].ToString();
                    om.Parameters["MI3ILOGIS"].Value = rw["MI3ILOGIS"].ToString();
                    om.Parameters["MI3NOLIGN"].Value = rw["MI3NOLIGN"].ToString();
                    om.Parameters["MI3NLIGP"].Value = rw["MI3NLIGP"].ToString();
                    om.Parameters["MI3DATACT"].Value = rw["MI3DATACT"].ToString().CDateTime();
                    // Line 2 
                    ol.Parameters["MI3CODSIT"].Value = rw["MI3CODSIT"].ToString();
                    ol.Parameters["MI3DATMVT"].Value = rw["MI3DATMVT"].ToString().CDateTime();
                    ol.Parameters["MI3CPROIN"].Value = rw["MI3CPROIN"].ToString();
                    ol.Parameters["MI3CECART"].Value = rw["MI3CECART"].ToString();
                    ol.Parameters["MI3CODINV"].Value = rw["MI3CODINV"].ToString();
                    ol.Parameters["MI3NUMORC"].Value = rw["MI3NUMORC"].ToString();
                    ol.Parameters["MI3CODCLI"].Value = rw["MI3CODCLI"].ToString();
                    ol.Parameters["MI3SECTIO"].Value = rw["MI3SECTIO"].ToString();
                    ol.Parameters["MI3DONORD"].Value = rw["MI3DONORD"].ToString();
                    ol.Parameters["MI3NDOS"].Value = rw["MI3NDOS"].ToString();
                    ol.Parameters["MI3DABL"].Value = rw["MI3DABL"].ToString();
                    ol.Parameters["MI3DEPOT"].Value = rw["MI3DEPOT"].ToString();
                    ol.Parameters["MI3ILOGIS"].Value = rw["MI3ILOGIS"].ToString();
                    ol.Parameters["MI3NOLIGN"].Value = rw["MI3NOLIGN"].ToString();
                    ol.Parameters["MI3NLIGP"].Value = rw["MI3NLIGP"].ToString();
                    ol.Parameters["MI3DATACT"].Value = rw["MI3DATACT"].ToString().CDateTime();

                    if(rw["opstype"].ToString() == "B") {
                        // Block Logic
                        // ==>line 1 MI3QTEUVC(+) , MI3STASTK = 1
                        // ==>line 2 MI3QTEUVC(-) , MI3STASTK = 0

                        // Block Line 1 (+)
                        om.Parameters["MI3QTEUVC"].Value = rw["MI3QTEUVC"].ToString().Replace("-","").CInt32();
                        om.Parameters["MI3POIDS"].Value = rw["MI3POIDS"].ToString().Replace("-","").CDecimal();
                        om.Parameters["MI3STASTK"].Value = "1";
                        om.Parameters["MI3QTEUMC"].Value = rw["MI3QTEUMC"].ToString().Replace("-","").CInt32();

                        // Block Line 2 (-)
                        ol.Parameters["MI3QTEUVC"].Value = "-" + rw["MI3QTEUVC"].ToString().Replace("-","").CInt32();
                        ol.Parameters["MI3POIDS"].Value = "-" + rw["MI3POIDS"].ToString().Replace("-","").CDecimal();
                        ol.Parameters["MI3STASTK"].Value = "0";
                        ol.Parameters["MI3QTEUMC"].Value = "-" + rw["MI3QTEUMC"].ToString().Replace("-","").CInt32();

                    } else {
                        // UnBlock Logic
                        // ==>line 1 MI3QTEUVC(+) , MI3STASTK = 0
                        // ==>line 2 MI3QTEUVC(-) , MI3STASTK = 1

                        // UnBlock Line 1 (+)
                        om.Parameters["MI3QTEUVC"].Value = rw["MI3QTEUVC"].ToString().Replace("-","").CInt32();
                        om.Parameters["MI3POIDS"].Value = rw["MI3POIDS"].ToString().Replace("-","").CDecimal();
                        om.Parameters["MI3STASTK"].Value = "0";
                        om.Parameters["MI3QTEUMC"].Value = rw["MI3QTEUMC"].ToString().Replace("-","").CInt32();


                        // UnBlock Line 2 (-)
                        ol.Parameters["MI3QTEUVC"].Value = "-" + rw["MI3QTEUVC"].ToString().Replace("-","").CInt32();
                        ol.Parameters["MI3POIDS"].Value = "-" + rw["MI3POIDS"].ToString().Replace("-","").CDecimal();
                        ol.Parameters["MI3STASTK"].Value = "1";
                        ol.Parameters["MI3QTEUMC"].Value = "-" + rw["MI3QTEUMC"].ToString().Replace("-","").CInt32();

                    }

                    try {
                        cm.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        oc.Open();
                        om.ExecuteNonQuery();
                        ol.ExecuteNonQuery();
                        oc.Close();
                        cm.snapsExecute();
                    } catch(Exception exl) {
                        oc.Close();
                        em.Parameters["rowid"].Value = rw["rowid"].ToString().CInt32();
                        em.Parameters["xmsg"].Value = (exl.Message.Length > 100) ? exl.Message.ToString().Substring(0,100) : exl.Message.ToString();
                        em.snapsExecute();
                    }
                }

            } catch(Exception ex) {
                logger.Error("91917","01","launch_block",ex,ex.Message);
                //throw ex;
            } finally {
                cn.Dispose(); lcm.ForEach(x => x.Dispose());
                on.Dispose();
                OracleConnection.ClearPool(oc);
                oc.Dispose();
                om.Dispose(); ol.Dispose();
            }

        }

        public void launch_imstock() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            SqlCommand cm = new SqlCommand("",on);
            DataTable dt = new DataTable();
            List<SqlCommand> lcm = new List<SqlCommand>();
            try {
                //Read Interface object 
                cm.CommandText = sqlinterface_launch_imstock;
                cm.CommandTimeout = 120;
                dt = cm.snapsTableAsync().Result;
                int tt = dt.Rows.Count;
                if(tt > 0) {

                    int[] IS3CODSIT = new int[tt];
                    string[] IS3CPROIN = new string[tt];
                    int[] IS3STKUVC = new int[tt];
                    int[] IS3STKPOI = new int[tt];
                    string[] IS3DONORD = new string[tt];
                    int[] IS3STPALE = new int[tt];
                    string[] IS3DEPOT = new string[tt];
                    string[] IS3ILOGIS = new string[tt];

                    for(int j = 0 ; j < dt.Rows.Count; j++) {
                        try { IS3CODSIT[j] = Convert.ToInt32(dt.Rows[j]["IS3CODSIT"]); } catch(Exception e) { throw new Exception("Column :IS3CODSIT[" + j + "] Is " + e.Message); }
                        try { IS3CPROIN[j] = Convert.ToString(dt.Rows[j]["IS3CPROIN"]); } catch(Exception e) { throw new Exception("Column :IS3CPROIN[" + j + "] Is " + e.Message); }
                        try { IS3STKUVC[j] = Convert.ToInt32(dt.Rows[j]["IS3STKUVC"]); } catch(Exception e) { throw new Exception("Column :IS3STKUVC[" + j + "] Is " + e.Message); }
                        try { IS3STKPOI[j] = Convert.ToInt32(dt.Rows[j]["IS3STKPOI"]); } catch(Exception e) { throw new Exception("Column :IS3STKPOI[" + j + "] Is " + e.Message); }
                        try { IS3DONORD[j] = Convert.ToString(dt.Rows[j]["IS3DONORD"]); } catch(Exception e) { throw new Exception("Column :IS3DONORD[" + j + "] Is " + e.Message); }
                        try { IS3STPALE[j] = Convert.ToInt32(dt.Rows[j]["IS3STPALE"]); } catch(Exception e) { throw new Exception("Column :IS3STPALE[" + j + "] Is " + e.Message); }
                        try { IS3DEPOT[j] = Convert.ToString(dt.Rows[j]["IS3DEPOT"]); } catch(Exception e) { throw new Exception("Column :IS3DEPOT[" + j + "] Is " + e.Message); }
                        try { IS3ILOGIS[j] = Convert.ToString(dt.Rows[j]["IS3ILOGIS"]); } catch(Exception e) { throw new Exception("Column :IS3ILOGIS[" + j + "] Is " + e.Message); }
                    }

                    using(var oraconnection = new OracleConnection(cnx_legacysource)) {
                        oraconnection.Open();
                        Console.WriteLine("Gold Connection Opened!");

                        OracleParameter P_S3CODSIT = new OracleParameter() { OracleDbType = OracleDbType.Int32,Value = IS3CODSIT };
                        OracleParameter P_S3CPROIN = new OracleParameter() { OracleDbType = OracleDbType.Varchar2,Value = IS3CPROIN };
                        OracleParameter P_S3STKUVC = new OracleParameter() { OracleDbType = OracleDbType.Int32,Value = IS3STKUVC };
                        OracleParameter P_S3STKPOI = new OracleParameter() { OracleDbType = OracleDbType.Int32,Value = IS3STKPOI };
                        OracleParameter P_S3DONORD = new OracleParameter() { OracleDbType = OracleDbType.Varchar2,Value = IS3DONORD };
                        OracleParameter P_S3STPALE = new OracleParameter() { OracleDbType = OracleDbType.Int32,Value = IS3STPALE };
                        OracleParameter P_S3DEPOT = new OracleParameter() { OracleDbType = OracleDbType.Varchar2,Value = IS3DEPOT };
                        OracleParameter P_S3ILOGIS = new OracleParameter() { OracleDbType = OracleDbType.Varchar2,Value = IS3ILOGIS };

                        Console.WriteLine("launch_imstock truncate");
                        using(OracleCommand copyCommand = new OracleCommand("DELETE FROM n3_imstk",oraconnection)) {
                            copyCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine("launch_imstock insert");
                        using(OracleCommand copyCommand = new OracleCommand(sqlinterface_launch_imstock_insert,oraconnection)) {
                            copyCommand.CommandTimeout = 600;// TimeOut in 10 minute
                            copyCommand.ArrayBindCount = tt;
                            copyCommand.Parameters.Add(P_S3CODSIT);
                            copyCommand.Parameters.Add(P_S3CPROIN);
                            copyCommand.Parameters.Add(P_S3STKUVC);
                            copyCommand.Parameters.Add(P_S3STKPOI);
                            copyCommand.Parameters.Add(P_S3DONORD);
                            copyCommand.Parameters.Add(P_S3STPALE);
                            copyCommand.Parameters.Add(P_S3DEPOT);
                            copyCommand.Parameters.Add(P_S3ILOGIS);
                            copyCommand.ExecuteNonQuery();
                            Console.WriteLine("launch_imstock Success!");
                        }
                        oraconnection.Close();
                    }
                }else { Console.WriteLine("launch_imstock 0 row Success!"); }
            } catch(Exception ex) {
                logger.Error("91917","01","launch_imstock",ex,ex.Message);
                //throw ex;
            } finally {
                cn.Dispose(); lcm.ForEach(x => x.Dispose());
                on.Dispose();
            }

        }
    }
}