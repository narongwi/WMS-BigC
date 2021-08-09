namespace snaps.wms.api.report.Models
{
    public class OptionsModel
    {
        public OptionsModel(object id, object text)
        {
            Id = id.ToString();
            Text = text.ToString();
        }
        public string Id { get; set; }
        public string Text { get; set; }


    }
}