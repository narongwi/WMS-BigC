using snaps.wms.api.report.Utils;
using System.Collections.Generic;
namespace snaps.wms.api.report.Models
{
    public class FormControlModel
    {
        public FormControlModel()
        {
        }

        public FormControlModel(object id, object no, object name, object label, object typ, List<OptionsModel> options)
        {
            Id = id.toInt();
            Param = no.toInt();
            Name = name.ToString();
            Label = label.ToString();
            Typ = typ.ToString();
            Options = options;
            Val = "";
        }

        public int Id { get; set; }
        public int Param { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Typ { get; set; }
        public string Val { get; set; }
        public List<OptionsModel> Options { get; set; }
    }
}
