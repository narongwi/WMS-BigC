using Microsoft.Reporting.NETCore;
using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace snaps.wms.printing
{
    public class LabelPrinting
    {
        public string PrinterName { get; set; }
        public string ReportPath { get; set; }
        /// <summary>
        /// Print Directo Report To Printer
        /// </summary>
        /// <param name="Name">Name of putaway,fullpallet, shipped, prephu, empty</param>
        /// <param name="dataSet"></param>
        public void FullPallet(string Name,DataSet dataSet)
        {
            try
            {
                string[] nameList = new string[] { "putaway", "fullpallet", "shipped", "prephu", "empty" };
                int fileIndex = Array.IndexOf(nameList, Name);
                if (fileIndex == -1) throw new Exception("Invalid Report Name " + Name);
                string filePath = Directory.GetCurrentDirectory();
                string fileName = string.Concat(nameList[fileIndex], ".rdlc");
                string fullName = Path.Combine(filePath, fileName);
                using var rs = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName);
                using (LocalReport report = new LocalReport())
                {
                    report.LoadReportDefinition(rs);
                    report.DataSources.Add(new ReportDataSource("source", dataSet));
                    report.SetParameters(new[] { new ReportParameter("Parameter1", "Parameter value") });
                    report.Print(PrinterName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
       
         
        }
    }
}
