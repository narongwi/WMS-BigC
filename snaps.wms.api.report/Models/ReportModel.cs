using snaps.wms.api.report.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.report.Models
{
    public class ReportModel
    {
        public ReportModel()
        {
        }
        public ReportModel(object id, object code, object name, List<FormControlModel> forms)
        {
            Id = id.toInt();
            Code = code.ToString();
            Name = name.ToString();
            Forms = forms;
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<FormControlModel> Forms { get; set; }
    }
}
