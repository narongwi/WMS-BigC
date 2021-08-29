using Microsoft.Reporting.WebForms;
using snaps.wms.api.document.Extensions;
using snaps.wms.api.document.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Zen.Barcode;
namespace snaps.wms.api.document.Controllers {
    public class printController : Controller {
        private Warning[] warnings;
        private string[] streamIds;
        private string mimeType = string.Empty;
        private string encoding = string.Empty;
        private string extension = string.Empty;
        private readonly string cnx;

        public printController() {
            cnx = ConfigurationManager.ConnectionStrings["WMSConn"].ToString();
        }

        [HttpPost]
        public ActionResult putaway(string orgcode,string site,string depot,string taskno) {
            //
                ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            string sqlcm, printerName;

            try {
                // form params
                sqlcm = string.Format(SqlReportModel.PutawaySql,orgcode,site,depot,taskno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    // create barcode
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    // data source
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L011_Inbound_Putaway.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));


                    // printer setting parameter
                    if(tb.Rows[0]["tasktype"].ToString().ToUpper() == "P") {
                        // putaway printer
                        printerName = getPrinterName(orgcode,site,depot,"PUTAWAY");
                        rv.LocalReport.Print(printerName);
                    } else {
                        // defult printer
                        printerName = getPrinterName(orgcode,site,depot);
                        rv.LocalReport.Print(printerName);
                    }

                    // export pdf
                    byte[] bytes = rv.LocalReport.Render("PDF",null,out mimeType,out encoding,out extension,out streamIds,out warnings);
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Putaway_Label.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                }

                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public ActionResult labelhu(string orgcode,string site,string depot,string huno,string hutype) {
            //
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            string sqlcm, printerName;

            try {
                // form params
                sqlcm = hutype == "N" ? SqlReportModel.StockHuLabel : SqlReportModel.MergeHuLabel;
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn)) {
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@orgcode",orgcode);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@site",site);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@depot",depot);
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@huno",huno);
                    dataAdapter.Fill(tb);
                }

                if(tb.Rows.Count > 0) {
                    // create barcode
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    // data source
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L011_Inbound_Putaway.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));


                    // printer setting parameter
                    if(tb.Rows[0]["tasktype"].ToString().ToUpper() == "P") {
                        // putaway printer
                        printerName = getPrinterName(orgcode,site,depot,"PUTAWAY");
                        rv.LocalReport.Print(printerName);
                    } else {
                        // defult printer
                        printerName = getPrinterName(orgcode,site,depot);
                        rv.LocalReport.Print(printerName);
                    }

                    // export pdf
                    byte[] bytes = rv.LocalReport.Render("PDF",null,out mimeType,out encoding,out extension,out streamIds,out warnings);
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Putaway_Label.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                } else {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Data Not Found");
                }

                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public ActionResult printputaway(string orgcode,string site,string depot,string taskno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            string sqlcm, printerName;
            try {

                // report data source
                sqlcm = string.Format(SqlReportModel.PutawaySql,orgcode,site,depot,taskno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    // generate barcode image
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    // report processing
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L011_Inbound_Putaway.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    // printer setting parameter
                    if(tb.Rows[0]["tasktype"].ToString().ToUpper() == "P") {
                        // putaway printer
                        printerName = getPrinterName(orgcode,site,depot,"PUTAWAY");
                        rv.LocalReport.Print(printerName);
                    } else {
                        // defult printer
                        printerName = getPrinterName(orgcode,site,depot);
                        rv.LocalReport.Print(printerName);
                    }

                }

                return Content("");
            } catch(Exception ex) {
                return Content(ex.Message);
            } finally {
                rv.Dispose();
            }
        }

        [HttpPost]
        public ActionResult hu(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                // report data source
                string sqlcm = string.Format(SqlReportModel.HUSql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    // generate barcode image
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    // report processing
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L011_Inbound_Putaway.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    // printer print
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);

                    byte[] bytes = rv.LocalReport.Render("PDF",
                        null,out mimeType,
                        out encoding,
                        out extension,
                        out streamIds,
                        out warnings);

                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Putaway_Label.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                }
                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }
        [HttpPost]
        public ActionResult printhu(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                // report data source
                string sqlcm = string.Format(SqlReportModel.HUSql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);
                if(tb.Rows.Count > 0) {
                    // generate barcode image
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    if(tb.Rows.Count > 0) { tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString()); }

                    // report processing
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L011_Inbound_Putaway.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    //print using  extensions
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);
                }
                return Content("");
            } catch(Exception ex) {
                rv.Dispose();
                return Content(ex.Message);
            } finally {
                rv.Dispose();
            }
        }

        [HttpPost]
        public ActionResult huempty(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                string sqlcm = string.Format(SqlReportModel.HUEmptySql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    // generate barcode image
                    tb.Columns.Add("hubar",typeof(byte[]));
                    tb.Rows[0]["hubar"] = _GBar(tb.Rows[0]["huno"].ToString());

                    // report processing
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L003EmptyHU.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    // printer print
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);

                    byte[] bytes = rv.LocalReport.Render("PDF",null,
                        out mimeType,
                        out encoding,
                        out extension,
                        out streamIds,
                        out warnings);
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Empty_Label.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush();
                }// send it to the client to download
                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }
        [HttpPost]
        public ActionResult printhuempty(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                // report data source
                string sqlcm = string.Format(SqlReportModel.HUEmptySql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    // generate barcode image
                    tb.Columns.Add("hubar",typeof(byte[]));
                    if(tb.Rows.Count > 0) { tb.Rows[0]["hubar"] = _GBar(tb.Rows[0]["huno"].ToString()); }

                    // report processing
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L003EmptyHU.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    //print using  extensions
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);
                }
                return Content("");
            } catch(Exception ex) {
                rv.Dispose();
                return Content(ex.Message);
            } finally {
                rv.Dispose();
            }
        }

        [HttpPost]
        public ActionResult labelshipped(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            ReportParameter[] parameters = new ReportParameter[1];

            try {
                string sqlcm = string.Format(SqlReportModel.LabelShippedSql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L020_Outbound_Ship_Loose_BGCTH.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    // printer print
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);


                    byte[] bytes = rv.LocalReport.Render("PDF",null,
                        out mimeType,out encoding,out extension,out streamIds,out warnings);


                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Outbound_Ship_Loose_BGCTH_" + huno + ".pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                    return Content("");
                }

                throw new Exception("No Data");

            } catch(Exception ex) { throw ex; } finally { }
        }

        [HttpPost]
        public ActionResult printlabelshipped(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {

                string sqlcm = string.Format(SqlReportModel.LabelShippedSql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                tb.Columns.Add("imgbarhuno",typeof(byte[]));
                if(tb.Rows.Count > 0) {
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());


                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L020_Outbound_Ship_Loose_BGCTH.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));


                    //print using  extensions
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);
                }

                return Content("printed");
            } catch(Exception ex) {
                rv.Dispose();
                return Content(ex.Message);
            } finally {
                rv.Dispose();
            }
        }

        [HttpPost]
        public ActionResult Fullpallet(string orgcode,string site,string depot,string taskno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                string sqlcm = string.Format(SqlReportModel.FullpalletSql,orgcode,site,depot,taskno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L021_Outbound_Full_Pallet_BGCTH.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);

                    byte[] bytes = rv.LocalReport.Render("PDF",null,
                        out mimeType,out encoding,out extension,
                        out streamIds,out warnings);

                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Fullpallet.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                    return Content("");
                }

                throw new Exception("No Data");
            } catch(Exception ex) { throw ex; } finally { rv.Dispose(); tb.Dispose(); }
        }
        [HttpPost]
        public ActionResult printFullpallet(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                string sqlcm = string.Format(SqlReportModel.FullpalletByHUSql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L021_Outbound_Full_Pallet_BGCTH.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);

                    byte[] bytes = rv.LocalReport.Render("PDF",null,
                        out mimeType,out encoding,out extension,
                        out streamIds,out warnings);

                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Fullpallet.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                    return Content("");
                }

                throw new Exception("No Data");
            } catch(Exception ex) { throw ex; } finally { rv.Dispose(); tb.Dispose(); }
        }
        [HttpPost]
        public ActionResult labelshipped_pallet(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();

            try {
                string sqlcm = string.Format(SqlReportModel.LabelShippedPalletSql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L021_Outbound_Ship_Pallet_BGCTH.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    // printer print
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);

                    byte[] bytes = rv.LocalReport.Render("PDF",null,
                        out mimeType,out encoding,out extension,
                        out streamIds,out warnings);


                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=LabelShipped.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                }

                return Content("");

            } catch(Exception ex) { throw ex; } finally { }
        }

        [HttpPost]
        public ActionResult printshipped_pallet(string orgcode,string site,string depot,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {

                string sqlcm = string.Format(SqlReportModel.LabelShippedPalletSql,orgcode,site,depot,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L021_Outbound_Ship_Pallet_BGCTH.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    //print using  extensions
                    string printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);
                }

                return Content("printed");
            } catch(Exception ex) {
                rv.Dispose();
                return Content(ex.Message);
            } finally {
                rv.Dispose();
            }
        }
    
        [HttpPost]
        public ActionResult Correction(string orgcode,string site,string depot,string seqops,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            string sqlcm, printerName;
            try {

                // report data source
                sqlcm = string.Format(SqlReportModel.CorrectionSql,orgcode,site,depot,seqops,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    // generate barcode image
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    // report processing
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L011_Inbound_Putaway.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    //print using  extensions
                    printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);

                    // export pdf
                    byte[] bytes = rv.LocalReport.Render("PDF",null,out mimeType,out encoding,out extension,out streamIds,out warnings);
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Putaway_Label.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                }

                return Content("");
            } catch(Exception ex) {
                return Content(ex.Message);
            } finally {
                rv.Dispose();
            }
        }

        [HttpPost]
        public ActionResult printCorrection(string orgcode,string site,string depot,string seqops,string huno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            string sqlcm, printerName;
            try {
                // report data source
                sqlcm = string.Format(SqlReportModel.CorrectionSql,orgcode,site,depot,seqops,huno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {
                    // generate barcode image
                    tb.Columns.Add("imgbarhuno",typeof(byte[]));
                    tb.Rows[0]["imgbarhuno"] = _GBar(tb.Rows[0]["huno"].ToString());

                    // report processing
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/L011_Inbound_Putaway.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    //print using  extensions
                    printerName = getPrinterName(orgcode,site,depot);
                    rv.LocalReport.Print(printerName);
                }

                return Content("");
            } catch(Exception ex) {
                return Content(ex.Message);
            } finally {
                rv.Dispose();
            }
        }

        /* EXPORT FILE */
        [HttpPost]
        public void receiptionConfirm(string order) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                string sqlcm = string.Format(SqlReportModel.ReceiptionConfirmSql,order);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R002ReceiptConfirm.rdlc";
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                byte[] bytes = rv.LocalReport.Render("PDF",
                    null,out mimeType,out encoding,
                    out extension,out streamIds,out
                    warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition","inline; filename=reception_confirm.pdf");

                Response.BinaryWrite(bytes); // create the file
                Response.Flush(); // send it to the client to download
                Response.End();
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public void statement(string orgcode,string site,string depot,string inorder) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                string sqlcm = string.Format(SqlReportModel.ReceiptStateSql,orgcode,site,depot,inorder);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R001ReceiptState.rdlc";
                rv.LocalReport.DataSources.Add(new ReportDataSource("Lines",tb));

                byte[] bytes = rv.LocalReport.Render("PDF",null,
                    out mimeType,out encoding,out extension,
                    out streamIds,out warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition","inline; filename=reception_statement.pdf");
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public void CountSheet(string orgcode,string site,string depot,string countcode,string plancode) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                string sqlcm = string.Format(SqlReportModel.CountSheetSql2,orgcode,site,depot,countcode,plancode);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {

                    tb.Columns.Add("planbar",typeof(byte[]));
                    if(tb.Rows.Count > 0) { tb.Rows[0]["planbar"] = _GBar(tb.Rows[0]["plancode"].ToString()); }

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R00CountSheet.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    byte[] bytes = rv.LocalReport.Render("PDF",null,out mimeType,
                        out encoding,out extension,out streamIds,out warnings);

                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Count_sheet.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                    Response.End();
                }
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public void picklist(string orgcode,string site,string depot,string prepno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {
                string sqlcm = string.Format(SqlReportModel.PickListSql,orgcode,site,depot,prepno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {

                    tb.Columns.Add("imgbarprep",typeof(byte[]));
                    if(tb.Rows.Count > 0) { tb.Rows[0]["imgbarprep"] = _GBar(tb.Rows[0]["prepno"].ToString()); }

                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R003PrepPick.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                    byte[] bytes = rv.LocalReport.Render("PDF",null,out mimeType,
                        out encoding,out extension,out streamIds,out warnings);

                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=Pick_List.pdf");
                    Response.BinaryWrite(bytes); // create the file
                    Response.Flush(); // send it to the client to download
                    Response.End();
                }
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public void distlist(string orgcode,string site,string depot,string prepno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            DataTable tbh = new DataTable();
            DataRow drh;
            ReportParameter[] parameters = new ReportParameter[1];

            try {
                string sqlcm = string.Format(SqlReportModel.DistrListSql,orgcode,site,depot,prepno);
                using(var cn = new SqlConnection(cnx))
                using(var dataAdapter = new SqlDataAdapter(sqlcm,cn))
                    dataAdapter.Fill(tb);

                if(tb.Rows.Count > 0) {

                    tbh.Columns.Add("prepbar",typeof(byte[]));
                    tbh.Columns.Add("product",typeof(string));
                    tbh.Columns.Add("productname",typeof(string));
                    tbh.Columns.Add("huno",typeof(string));
                    tbh.Columns.Add("inborder",typeof(string));
                    tbh.Columns.Add("barcode",typeof(string));
                    tbh.Columns.Add("daterec",typeof(string));
                    tbh.Columns.Add("prepno",typeof(string));
                    drh = tbh.NewRow();

                    drh["prepbar"] = _GBar(tb.Rows[0]["lotno"].ToString());
                    drh["product"] = tb.Rows[0]["product"].ToString();
                    drh["productname"] = tb.Rows[0]["productname"].ToString();
                    drh["huno"] = tb.Rows[0]["huno"].ToString();
                    drh["inborder"] = tb.Rows[0]["inborder"].ToString();
                    drh["barcode"] = tb.Rows[0]["barcode"].ToString();
                    drh["daterec"] = tb.Rows[0]["prepdate"];
                    drh["prepno"] = tb.Rows[0]["lotno"].ToString();

                    tbh.Rows.Add(drh);
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R13Distribution.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));
                    rv.LocalReport.DataSources.Add(new ReportDataSource("repheader",tbh));

                    byte[] bytes = rv.LocalReport.Render("PDF",null,
                        out mimeType,
                        out encoding,
                        out extension,
                        out streamIds,
                        out warnings);

                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=snaps_distribution_" + prepno + ".pdf");
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            } catch(Exception ex) { throw ex; } finally { }
        }


        [HttpPost]
        public void getLoadingDraft(string orgcode,string site,string depot,string routeno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            try {

                string sqlcm = string.Format(SqlReportModel.LoadingDraftSql,orgcode,site,depot,routeno);
                using(SqlConnection cn = new SqlConnection(cnx))
                using(SqlDataAdapter ds = new SqlDataAdapter(sqlcm,cn))
                    ds.Fill(tb);
                if(tb.Rows.Count > 0) {
                    rv.ProcessingMode = ProcessingMode.Local;
                    rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R030_Outbound_LoadingDraft_BGCTH.rdlc";
                    rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));


                    byte[] bytes = rv.LocalReport.Render("PDF",null,
                        out mimeType,out encoding,out extension,out streamIds,
                        out warnings);

                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","inline; filename=LoadingDraft.pdf");
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public void getTransportnote(string orgcode,string site,string depot,string routeno,string outrno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            string sqlcm = string.Format(SqlReportModel.TransportnoteSql2,orgcode,site,depot,routeno,outrno);
            try {

                using(SqlConnection cn = new SqlConnection(cnx))
                using(SqlDataAdapter ds = new SqlDataAdapter(sqlcm,cn))
                    ds.Fill(tb);

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R040_Outbound_Transpornote_BGCTH.rdlc";
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                byte[] bytes = rv.LocalReport.Render("PDF",null,out mimeType,
                    out encoding,out extension,out streamIds,out warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition","inline; filename=TransportNotes.pdf");
                Response.BinaryWrite(bytes); // create the file
                Response.Flush(); // send it to the client to download
                Response.End();
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpPost]
        public void getPackinglist(string orgcode,string site,string depot,string routeno,string outrno) {
            ReportViewer rv = new ReportViewer();
            DataTable tb = new DataTable();
            string sqlcm = string.Format(SqlReportModel.PackinglistSql,orgcode,site,depot,routeno,outrno);
            try {
                using(SqlConnection cn = new SqlConnection(cnx))
                using(SqlDataAdapter ds = new SqlDataAdapter(sqlcm,cn))
                    ds.Fill(tb);

                rv.ProcessingMode = ProcessingMode.Local;
                rv.LocalReport.ReportPath = Server.MapPath("~") + "/reprdlc/R041_Outbound_PackingList_BGCTH.rdlc";
                rv.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",tb));

                byte[] bytes = rv.LocalReport.Render("PDF",null,out mimeType,
                    out encoding,out extension,out streamIds,out warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition","inline; filename=PackingList.pdf");
                Response.BinaryWrite(bytes); // create the file
                Response.Flush(); // send it to the client to download
                Response.End();
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }


        private string getPrinterName(string orgcode,string site,string depot,string labelType = "") {
            DataTable tb = new DataTable();
            
            string sqlprint = string.Format(SqlReportModel.PrintSettingSql,
                orgcode,site,depot,string.IsNullOrEmpty(labelType) ? "LABEL" : labelType);

            using(SqlConnection cn = new SqlConnection(cnx))
            using(SqlDataAdapter ds = new SqlDataAdapter(sqlprint,cn))
                ds.Fill(tb);
            if(tb.Rows.Count == 0)
                return "";

            return tb.Rows[0]["bnvalue"].ToString();
        }
        private byte[] _GBar(string vTx) {
            BarcodeDraw ZB = BarcodeDrawFactory.Code128WithChecksum;
            MemoryStream M1 = new System.IO.MemoryStream();
            try {
                ZB.Draw(vTx,20,1).Save(M1,System.Drawing.Imaging.ImageFormat.Png);
                return M1.GetBuffer();
            } catch(Exception ex) { throw ex; } finally { ZB = null; M1.Close(); M1.Dispose(); }
        }
    }
}