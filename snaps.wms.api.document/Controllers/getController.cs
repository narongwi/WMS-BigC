using Microsoft.Reporting.WebForms;
using snaps.wms.api.document.Extensions;
using snaps.wms.api.document.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Zen.Barcode;
namespace snaps.wms.api.document.Controllers {
    public class getController : Controller {
        private Warning[] warnings;
        private string[] streamIds;
        private string mimeType = string.Empty;
        private string encoding = string.Empty;
        private string extension = string.Empty;
        private readonly string cnx;

        public getController() {
            cnx = ConfigurationManager.ConnectionStrings["WMSConn"].ToString();
        }

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

        [HttpGet]
        public ActionResult CountSheet(string orgcode,string site,string depot,string countcode,string plancode) {
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
                }
                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpGet]
        public ActionResult picklist(string orgcode,string site,string depot,string prepno) {
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
                }
                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpGet]
        public ActionResult distlist(string orgcode,string site,string depot,string prepno) {
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
                }
                return Content("");
            } catch(Exception ex) { throw ex; } finally { }
        }


        [HttpGet]
        public ActionResult getLoadingDraft(string orgcode,string site,string depot,string routeno) {
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
                    Response.AddHeader("content-disposition","inline; filename=Pick_List.pdf");
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                }
                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpGet]
        public ActionResult getTransportnote(string orgcode,string site,string depot,string routeno,string outrno) {
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
                Response.AddHeader("content-disposition","inline; filename=Pick_List.pdf");
                Response.BinaryWrite(bytes); // create the file
                Response.Flush(); // send it to the client to download
                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
        }

        [HttpGet]
        public ActionResult getPackinglist(string orgcode,string site,string depot,string routeno,string outrno) {
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
                return Content("");
            } catch(Exception ex) { throw ex; } finally {
                rv.Dispose();
                tb.Dispose();
            }
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