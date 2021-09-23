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
        //Retrive interface 
        public void retrive_barcode() {

            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection();
            SqlCommand om = new SqlCommand(sqlInterface_retrive_site,cn);
            OracleCommand rm = new OracleCommand(sqlInterface_retrive_barcode,oc);
            OracleDataReader r = null;

            List<orbit_barcode> hn = new List<orbit_barcode>();
            List<SqlCommand> lcm = new List<SqlCommand>();

            DataTable odepot = new DataTable();
            string errsql = "";
            try {

                odepot = om.snapsTableAsync().Result;
                rm.Parameters.Add("site","");
                rm.Parameters.Add("depot","");

                foreach(DataRow rn in odepot.Rows) {
                    rm.Parameters["site"].Value = rn["sitecode"].ToString();
                    rm.Parameters["depot"].Value = rn["depotcode"].ToString();

                    oc.ConnectionString = cnx_legacysource;
                    oc.Open();
                    //Read Interface object 
                    r = rm.ExecuteReader();
                    while(r.Read()) {
                        hn.Add(barcodefill(ref r));
                    }
                    r.Close();
                    oc.Close();

                    //Collaboration object
                    if(hn.Count > 0) {
                        lcm.Add(sqlorbit_barcode_insert.snapsCommand(on));
                        lcm[0].snapsPar(hn[0].orgcode,"orgcode");
                        lcm[0].snapsPar(hn[0].site,"site");
                        lcm[0].snapsPar(hn[0].depot,"depot");
                        lcm[0].snapsPar(hn[0].spcarea,"spcarea");
                        lcm[0].snapsPar(hn[0].article,"article");
                        lcm[0].snapsPar(hn[0].pv,"pv");
                        lcm[0].snapsPar(hn[0].lv,"lv");
                        lcm[0].snapsPar(hn[0].barops,"barops");
                        lcm[0].snapsPar(hn[0].barcode,"barcode");
                        lcm[0].snapsPar(hn[0].bartype,"bartype");
                        lcm[0].snapsPar(hn[0].thcode,"thcode");
                        lcm[0].snapsPar(hn[0].orbitsource,"orbitsource");
                        lcm[0].snapsPar(hn[0].tflow,"tflow");
                        lcm[0].snapsPar(hn[0].fileid,"fileid");
                        lcm[0].snapsPar(hn[0].rowops,"rowops");
                        lcm[0].snapsPar(hn[0].ermsg,"ermsg");
                    }

                    foreach(orbit_barcode oo in hn) {
                        try {
                            barcodecommand(ref lcm,oo);
                            lcm.snapsExecuteTrans(cn,ref errsql);
                        } catch(Exception exl) {
                            throw exl;
                        }
                    }

                }
            } catch(Exception ex) {
                if(oc.State == ConnectionState.Open) { oc.Close(); }
                logger.Error("91917","01","retrive_barcode",ex,ex.Message);
                throw ex;
            } finally {
                cn.Dispose(); if(r != null) { r.Dispose(); }
                lcm.ForEach(x => x.Dispose());
                rm.Dispose(); on.Dispose();
                hn.Clear();
                OracleConnection.ClearPool(oc);
            }
        }
        public void retrive_inbound() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            SqlCommand om = new SqlCommand(sqlInterface_retrive_site,cn);
            OracleConnection oc = new OracleConnection(cnx_legacysource);

            OracleCommand rm = new OracleCommand(sqlInterface_retrive_inbound_header,oc);
            OracleDataReader r = null;

            List<orbit_inbound> hn = new List<orbit_inbound>();
            List<orbit_inbouln> ln = new List<orbit_inbouln>();
            List<SqlCommand> lcm = new List<SqlCommand>();

            DataTable odepot = new DataTable();

            string errsql = "";
            try {

                odepot = om.snapsTableAsync().Result;
                rm.Parameters.Add("site","");
                rm.Parameters.Add("depot","");

                foreach(DataRow rn in odepot.Rows) {
                    rm.Parameters["site"].Value = rn["sitecode"].ToString();
                    rm.Parameters["depot"].Value = rn["depotcode"].ToString();

                    oc.Open();
                    //Read Interface object 
                    r = rm.ExecuteReader();
                    while(r.Read()) {
                        hn.Add(inboundfill(ref r));
                    }
                    r.Close();

                    //Read Interface object
                    rm.CommandText = sqlInterface_retrive_inbound_line;
                    r = rm.ExecuteReader();
                    while(r.Read()) {
                        ln.Add(inboulnfill(ref r));
                    }
                    r.Close();
                    oc.Close();

                    //Collaboration object
                    if(hn.Count > 0) {
                        lcm.Add(sqlorbit_inbound_header_insert.snapsCommand(on));
                        lcm[0].snapsPar(hn[0].orgcode,"orgcode");
                        lcm[0].snapsPar(hn[0].site,"site");
                        lcm[0].snapsPar(hn[0].depot,"depot");
                        lcm[0].snapsPar(hn[0].spcarea,"spcarea");
                        lcm[0].snapsPar(hn[0].thcode,"thcode");
                        lcm[0].snapsPar(hn[0].intype,"intype");
                        lcm[0].snapsPar(hn[0].subtype,"subtype");
                        lcm[0].snapsPar(hn[0].inorder,"inorder");
                        lcm[0].Parameters.Add("dateorder",SqlDbType.DateTimeOffset);
                        lcm[0].Parameters.Add("dateplan",SqlDbType.DateTimeOffset);
                        lcm[0].Parameters.Add("dateexpire",SqlDbType.DateTimeOffset);
                        lcm[0].snapsPar(hn[0].inpriority,"inpriority");
                        lcm[0].snapsPar(hn[0].inflag,"inflag");
                        lcm[0].snapsPar(hn[0].inpromo,"inpromo");
                        lcm[0].snapsPar(hn[0].tflow,"tflow");
                        lcm[0].snapsPar(hn[0].orbitsource,"orbitsource");
                        lcm[0].snapsPar(hn[0].fileid,"fileid");
                        lcm[0].snapsPar(hn[0].rowops,"rowops");
                        lcm[0].snapsPar(hn[0].ermsg,"ermsg");
                    }

                    foreach(orbit_inbound oo in hn) {
                        try {
                            inboundcommand(ref lcm,oo);
                            lcm.snapsExecuteTrans(cn,ref errsql);
                        } catch(Exception exl) {
                            throw exl;
                        }
                    }

                    lcm.Clear();
                    if(ln.Count > 0) {
                        lcm.Add(sqlorbit_inbound_line_insert.snapsCommand(on));
                        lcm[0].snapsPar(ln[0].orgcode,"orgcode");
                        lcm[0].snapsPar(ln[0].site,"site");
                        lcm[0].snapsPar(ln[0].depot,"depot");
                        lcm[0].snapsPar(ln[0].spcarea,"spcarea");
                        lcm[0].snapsPar(ln[0].inorder,"inorder");
                        lcm[0].snapsPar(ln[0].inln,"inln");
                        lcm[0].snapsPar(ln[0].inrefno,"inrefno");
                        lcm[0].snapsPar(ln[0].inrefln,"inrefln");
                        lcm[0].snapsPar(ln[0].article,"article");
                        lcm[0].snapsPar(ln[0].pv,"pv");
                        lcm[0].snapsPar(ln[0].lv,"lv");
                        lcm[0].snapsPar(ln[0].unitops,"unitops");
                        lcm[0].snapsPar(ln[0].qtysku,"qtysku");
                        lcm[0].snapsPar(ln[0].qtypu,"qtypu");
                        lcm[0].snapsPar(ln[0].qtyweight,"qtyweight");
                        lcm[0].snapsPar(ln[0].batchno,"batchno");
                        lcm[0].snapsPar(ln[0].lotno,"lotno");
                        lcm[0].Parameters.Add("expdate",SqlDbType.DateTimeOffset);
                        lcm[0].snapsPar(ln[0].serialno,"serialno");
                        lcm[0].snapsPar(ln[0].orbitsource,"orbitsource");
                        lcm[0].snapsPar(ln[0].tflow,"tflow");
                        lcm[0].snapsPar(ln[0].fileid,"fileid");
                        lcm[0].snapsPar(ln[0].rowops,"rowops");
                        lcm[0].snapsPar(ln[0].ermsg,"ermsg");
                        lcm[0].snapsPar(ln[0].dateops,"dateops");
                    }

                    foreach(orbit_inbouln oo in ln) {
                        try {
                            inboulncommand(ref lcm,oo);
                            lcm.snapsExecuteTrans(cn,ref errsql);
                        } catch(Exception exl) {
                            throw exl;
                        }
                    }

                }
            } catch(Exception ex) {
                if(oc.State == ConnectionState.Open) { oc.Close(); }
                logger.Error("91917","01","retrive_inbound",ex,ex.Message);
                throw ex;
            } finally {
                cn.Dispose(); if(r != null) { r.Dispose(); }
                lcm.ForEach(x => x.Dispose());
                rm.Dispose(); on.Dispose();
                hn.Clear(); ln.Clear();
                OracleConnection.ClearPool(oc);
            }
        }
        public void retrive_outbound() {

            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection();
            SqlCommand om = new SqlCommand(sqlInterface_retrive_site,cn);
            OracleCommand rm = new OracleCommand(sqlInterface_retrive_outbound_header,oc);
            OracleDataReader r = null;

            List<orbit_outbound> hn = new List<orbit_outbound>();
            List<orbit_outbouln> ln = new List<orbit_outbouln>();
            List<SqlCommand> lcm = new List<SqlCommand>();
            DataTable odepot = new DataTable();
            string errsql = "";
            int goldisready = 0;
            int wmsImported = 0;
            int wmsfailure = 0;

            try {

                odepot = om.snapsTableAsync().Result;
                rm.Parameters.Add("site","");
                rm.Parameters.Add("depot","");

                foreach(DataRow rn in odepot.Rows) {
                    rm.Parameters["site"].Value = rn["sitecode"].ToString();
                    rm.Parameters["depot"].Value = rn["depotcode"].ToString();

                    oc.ConnectionString = cnx_legacysource;
                    oc.Open();
                    //Read Interface object 
                    r = rm.ExecuteReader();
                    while(r.Read()) {
                        hn.Add(outboundfill(ref r));
                    }
                    r.Close();

                    //Read Interface object
                    rm.CommandText = sqlInterface_retrive_outbound_line;
                    r = rm.ExecuteReader();
                    while(r.Read()) {
                        ln.Add(outboulnfill(ref r));
                    }
                    r.Close();
                    oc.Close();

                    //Collaboration object
                    if(hn.Count > 0) {
                        lcm.Add(sqlorbit_outbound_header_insert.snapsCommand(on));
                        lcm[0].snapsPar(hn[0].orgcode,"orgcode");
                        lcm[0].snapsPar(hn[0].site,"site");
                        lcm[0].snapsPar(hn[0].depot,"depot");
                        lcm[0].snapsPar(hn[0].spcarea,"spcarea");
                        lcm[0].snapsPar(hn[0].ouorder,"ouorder");
                        lcm[0].snapsPar(hn[0].outype,"outype");
                        lcm[0].snapsPar(hn[0].ousubtype,"ousubtype");
                        lcm[0].snapsPar(hn[0].thcode,"thcode");
                        lcm[0].snapsPar(hn[0].dateorder,"dateorder");
                        lcm[0].snapsPar(hn[0].dateprep,"dateprep");
                        lcm[0].snapsPar(hn[0].dateexpire,"dateexpire");
                        lcm[0].snapsPar(hn[0].oupriority,"oupriority");
                        lcm[0].snapsPar(hn[0].ouflag,"ouflag");
                        lcm[0].snapsPar(hn[0].oupromo,"oupromo");
                        lcm[0].snapsPar(hn[0].dropship,"dropship");
                        lcm[0].snapsPar(hn[0].orbitsource,"orbitsource");
                        lcm[0].snapsPar(hn[0].stocode,"stocode");
                        lcm[0].snapsPar(hn[0].stoname,"stoname");
                        lcm[0].snapsPar(hn[0].stoaddressln1,"stoaddressln1");
                        lcm[0].snapsPar(hn[0].stoaddressln2,"stoaddressln2");
                        lcm[0].snapsPar(hn[0].stoaddressln3,"stoaddressln3");
                        lcm[0].snapsPar(hn[0].stosubdistict,"stosubdistict");
                        lcm[0].snapsPar(hn[0].stodistrict,"stodistrict");
                        lcm[0].snapsPar(hn[0].stocity,"stocity");
                        lcm[0].snapsPar(hn[0].stocountry,"stocountry");
                        lcm[0].snapsPar(hn[0].stopostcode,"stopostcode");
                        lcm[0].snapsPar(hn[0].stomobile,"stomobile");
                        lcm[0].snapsPar(hn[0].stoemail,"stoemail");
                        lcm[0].snapsPar(hn[0].tflow,"tflow");
                        lcm[0].snapsPar(hn[0].inorder,"inorder");
                        lcm[0].snapsPar(hn[0].fileid,"fileid");
                        lcm[0].snapsPar(hn[0].rowops,"rowops");
                        lcm[0].snapsPar(hn[0].ermsg,"ermsg");
                    }

                    foreach(orbit_outbound oo in hn) {
                        try {
                            outboundcommand(ref lcm,oo);
                            lcm.snapsExecuteTrans(cn,ref errsql);
                        } catch(Exception exl) {
                            throw exl;
                        }
                    }

                    lcm.Clear();
                    if(ln.Count > 0) {
                        lcm.Add(sqlorbit_outbound_line_insert.snapsCommand(on));
                        lcm[0].snapsPar(ln[0].orgcode,"orgcode");
                        lcm[0].snapsPar(ln[0].site,"site");
                        lcm[0].snapsPar(ln[0].depot,"depot");
                        lcm[0].snapsPar(ln[0].spcarea,"spcarea");
                        lcm[0].snapsPar(ln[0].ouorder,"ouorder");
                        lcm[0].snapsPar(ln[0].ouln,"ouln");
                        lcm[0].snapsPar(ln[0].ourefno,"ourefno");
                        lcm[0].snapsPar(ln[0].ourefln,"ourefln");
                        lcm[0].snapsPar(ln[0].inorder,"inorder");
                        lcm[0].snapsPar(ln[0].article,"article");
                        lcm[0].snapsPar(ln[0].pv,"pv");
                        lcm[0].snapsPar(ln[0].lv,"lv");
                        lcm[0].snapsPar(ln[0].unitops,"unitops");
                        lcm[0].snapsPar(ln[0].qtysku,"qtysku");
                        lcm[0].snapsPar(ln[0].qtypu,"qtypu");
                        lcm[0].snapsPar(ln[0].qtyweight,"qtyweight");
                        lcm[0].snapsPar(ln[0].spcselect,"spcselect");
                        lcm[0].snapsPar(ln[0].batchno,"batchno");
                        lcm[0].snapsPar(ln[0].lotno,"lotno");
                        lcm[0].Parameters.Add("datemfg",SqlDbType.DateTime);
                        lcm[0].Parameters.Add("dateexp",SqlDbType.DateTime);
                        lcm[0].snapsPar(ln[0].serialno,"serialno");
                        lcm[0].snapsPar(ln[0].orbitsource,"orbitsource");
                        lcm[0].snapsPar(ln[0].tflow,"tflow");
                        lcm[0].snapsPar(ln[0].disthcode,"disthcode");
                        lcm[0].snapsPar(ln[0].fileid,"fileid");
                        lcm[0].snapsPar(ln[0].rowops,"rowops");
                        lcm[0].snapsPar(ln[0].ermsg,"ermsg");
                        lcm[0].snapsPar(ln[0].ouseq,"ouseq");
                    }

                    goldisready = ln.Count;
                    foreach(orbit_outbouln oo in ln) {
                        try {
                            outboulncommand(ref lcm,oo);
                            lcm.snapsExecuteTrans(cn,ref errsql);
                            wmsImported++;
                        } catch(Exception exl) {
                            wmsfailure++;
                            throw exl;
                        }
                    }
                }
            } catch(Exception ex) {
                logger.Error("91917","01","retrive_outbound",ex,ex.Message);
                if(oc.State == ConnectionState.Open) { oc.Close(); }
                throw ex;
            } finally {
                cn.Dispose(); if(r != null) { r.Dispose(); }
                lcm.ForEach(x => x.Dispose());
                rm.Dispose(); on.Dispose();
                hn.Clear(); ln.Clear();
                OracleConnection.ClearPool(oc);


            }

            // WMS line Notify
            //if(goldisready > 0 && !string.IsNullOrEmpty(notifyToken)) {
            //    try {
            //        if(errorOnly) {
            //            // check is error
            //            if(wmsfailure > 0) {
            //                Console.WriteLine("Outbound Line Notify Sending");
            //                var notify = new snaps.wms.notify.LineNotify(notifyToken,notifyProxy);
            //                string message = "\nWMS Outbound Interface\nGOLD is ready " + goldisready 
            //                    + " row\nWMS Imported " + wmsImported + " row\nWMS Failure " + wmsfailure + " row";
            //                notify.Send(message,true,wmsfailure == 0 ? true : false).Wait();
            //                Console.WriteLine("Outbound Line Notify Successfully!");
            //            }
            //        }else{
            //            // All Notify
            //            Console.WriteLine("Outbound Line Notify Sending");
            //            var notify = new snaps.wms.notify.LineNotify(notifyToken,notifyProxy);
            //            string message = "\nWMS Outbound Interface\nGOLD is ready " + goldisready+ " row\nWMS Imported " + wmsImported + " row\nWMS Failure " + wmsfailure + " row";
            //            notify.Send(message,true,wmsfailure == 0 ? true : false).Wait();
            //            Console.WriteLine("Outbound Line Notify Successfully!");
            //        }
            //    } catch(Exception lx) {
            //        logger.Error("91917","01","nWMS Outbound Line Notify",lx,lx.Message);
            //    }

            //} else {
            //    Console.WriteLine("Outbound Line Notify Successfully!");
            //}

        }
        public void retrive_product() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection();
            SqlCommand om = new SqlCommand(sqlInterface_retrive_site,cn);
            OracleCommand rm = new OracleCommand(sqlInterface_retrive_product,oc);
            OracleDataReader r = null;

            List<orbit_product> rn = new List<orbit_product>();
            List<SqlCommand> lcm = new List<SqlCommand>();

            DataTable odepot = new DataTable();
            string errsql = "";
            try {

                odepot = om.snapsTableAsync().Result;
                rm.Parameters.Add("site","");
                rm.Parameters.Add("depot","");

                foreach(DataRow dn in odepot.Rows) {
                    rm.Parameters["site"].Value = dn["sitecode"].ToString();
                    rm.Parameters["depot"].Value = dn["depotcode"].ToString();

                    oc.ConnectionString = cnx_legacysource;
                    oc.Open();
                    //Read Interface object 
                    r = rm.ExecuteReader();
                    while(r.Read()) {
                        rn.Add(productfill(ref r));
                    }
                    r.Close();
                    oc.Close();
                    //Collaboration object
                    foreach(orbit_product ln in rn) {
                        try {
                            lcm = productcommand(ln);
                            lcm.snapsExecuteTrans(cn,ref errsql);
                        } catch(Exception exl) {
                            throw exl;
                        }
                    }
                }


            } catch(Exception ex) {
                if(oc.State == ConnectionState.Open) { oc.Close(); }
                logger.Error("91917","01","retrive_product",ex,ex.Message);
                throw ex;
            } finally {
                cn.Dispose(); if(r != null) { r.Dispose(); }
                lcm.ForEach(x => x.Dispose());
                rm.Dispose(); on.Dispose();
                rn.Clear();
                OracleConnection.ClearPool(oc);
            }
        }
        public void retrive_thirdparty() {
            SqlConnection on = new SqlConnection(cnx_snapswms);
            OracleConnection oc = new OracleConnection();
            SqlCommand om = new SqlCommand(sqlInterface_retrive_site,cn);
            OracleCommand rm = new OracleCommand(sqlInterface_retrive_thirdparty,oc);
            OracleDataReader r = null;

            List<orbit_thirdparty> rn = new List<orbit_thirdparty>();
            List<SqlCommand> lcm = new List<SqlCommand>();
            DataTable odepot = new DataTable();
            string errsql = "";
            try {

                odepot = om.snapsTableAsync().Result;
                rm.Parameters.Add("site","");
                rm.Parameters.Add("depot","");

                foreach(DataRow dn in odepot.Rows) {
                    rm.Parameters["site"].Value = dn["sitecode"].ToString();
                    rm.Parameters["depot"].Value = dn["depotcode"].ToString();

                    oc.ConnectionString = cnx_legacysource;
                    oc.Open();
                    //Read Interface object 
                    r = rm.ExecuteReader();
                    while(r.Read()) {
                        rn.Add(thirdpartyfill(ref r));
                    }
                    r.Close();
                    oc.Close();

                    //Collaboration object
                    foreach(orbit_thirdparty ln in rn) {
                        try {
                            lcm = thirdpartycommand(ln);
                            lcm.snapsExecuteTrans(cn,ref errsql);
                        } catch(Exception exl) {
                            throw exl;
                        }
                    }
                }

            } catch(Exception ex) {
                if(oc.State == ConnectionState.Open) { oc.Close(); }
                logger.Error("91917","01","retrive_thirdparty",ex,ex.Message);
                throw ex;
            } finally {
                cn.Dispose(); if(r != null) { r.Dispose(); }
                lcm.ForEach(x => x.Dispose());
                rm.Dispose(); on.Dispose();
                rn.Clear();
                OracleConnection.ClearPool(oc);
            }
        }
    }
}