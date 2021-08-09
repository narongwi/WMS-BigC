using OfficeOpenXml;
using OfficeOpenXml.Style;
using snaps.wms.api.report.Data;
using snaps.wms.api.report.Models;
using snaps.wms.api.report.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.report.Manager
{
    /// <summary>
    /// Manage Dynamic Report Data
    /// </summary>
    public class ReportManager
    {
        private readonly Connector connector;
        public ReportManager(string _connectionString)
        {
            connector = new Connector(_connectionString);
        }
        /// <summary>
        /// List All Report Status is Active
        /// </summary>
        /// <returns>Report Item List</returns>
        public List<ReportModel> ReportList()
        {
            try
            {
                List<ReportModel> reports = new List<ReportModel>();
                string commandText = "SELECT [id],[codeno],[rptname] FROM[dbo].[wm_rpth] WHERE status = @status order by codeno asc";
                DataTable dataTable = connector.GetDataTable(commandText, new KeyValuePair<string, object>("@status", "1"));
                if (dataTable.Rows.Count > 0)
                    foreach (DataRow rw in dataTable.Rows)
                    {
                        var control = ReportControl(rw["codeno"].ToString());
                        reports.Add(new ReportModel(rw[0], rw[1], rw[2], control));
                    }
                return reports;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Mapping Report Control 
        /// </summary>
        /// <param name="rpcode">Report Code</param>
        /// <returns>Form Control List</returns>
        public List<FormControlModel> ReportControl(string rpcode)
        {
            try
            {
                List<FormControlModel> Controls = new List<FormControlModel>();
                string commandText = "SELECT [id],[ctrlno],[ctrlname],[ctrllabel],[ctrltype],[ctrlcommand] FROM [dbo].[wm_rptd] where [codeno]=@rpcode order by [ctrlno]";
                DataTable dataTable = connector.GetDataTable(commandText, new KeyValuePair<string, object>("@rpcode", rpcode));
                if (dataTable.Rows.Count > 0)
                    foreach (DataRow rw in dataTable.Rows)
                    {
                        List<OptionsModel> options = new List<OptionsModel>();
                        try
                        {
                            // DropDowList Data , T=Text,C=Combo,D=DatePick
                            if (rw["ctrltype"].ToString() == "C")
                            {
                                string commandOps = rw["ctrlcommand"].ToString();
                                if (!string.IsNullOrEmpty(commandOps))
                                {
                                    // Execute DropdowList Support Id & Text
                                    options = connector.GetDataTable(commandOps)
                                        .AsEnumerable()
                                        .Select(x => new OptionsModel(x[0], x[1]))
                                        .ToList();
                                }
                            }
                        }
                        catch { }
                        // Add Control 
                        Controls.Add(new FormControlModel(rw[0], rw[1], rw[2], rw[3], rw[4], options));
                    }

                return Controls;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Report Command 
        /// </summary>
        /// <param name="id">Report Id</param>
        /// <returns>Sql Command Text</returns>
        public string ReportCommand(int id)
        {
            try
            {
                List<ReportModel> reports = new List<ReportModel>();
                string commandText = "SELECT [command] FROM[dbo].[wm_rpth] WHERE id=@id";
                return connector.GetString(commandText, new KeyValuePair<string, object>("@id", id));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        ///  Generate Data to Excel Byte[]
        /// </summary>
        /// <param name="commandText">Sal Comment Text</param>
        /// <returns>Excel Byte </returns>
        public byte[] ReportExport(string commandText)
        {
            try
            {
                // Data
                DataTable dataTable = new DataTable();
                dataTable = connector.GetDataTable(commandText);
                // Excel
                byte[] response;
                using (var excelFile = new ExcelPackage())
                {
                    var worksheet = excelFile.Workbook.Worksheets.Add("Sheet 1");
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, PrintHeaders: true);
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells.AutoFitColumns();
                    using (ExcelRange rng = worksheet.Cells)
                    {
                        rng.Style.WrapText = false;
                        rng.Style.Font.SetFromFont(new Font("Tahoma", 10));
                    }
                    response = excelFile.GetAsByteArray();
                }
                dataTable.Dispose();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
