using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace snaps.wms.api.document.Models
{
    public class LabelPutaway {
        public string dc { get; set; }
        public string site { get; set; }
        public string zone { get; set; }
        public string article { get; set; }
        public string artdesc { get; set; }
        public DateTimeOffset? recdate { get; set; }
        public string suppo { get; set; }
        public Int32 puqty { get; set; }
        public decimal weight { get; set; }
        public DateTimeOffset? mfgdate { get; set; }
        public Int32 skuqty { get; set; }
        public string mfglot { get; set; }
        public string skuofipck { get; set; }
        public Int32 ipckofpck { get; set; }
        public Int32 pckoflayer { get; set; }
        public Int32 layerofhu { get; set; }
        public Int32 pckofpallet { get; set; }
        public string hubo { get; set; }
        public byte[] hubar { get; set; }

        public LabelPutaway() { }
        public LabelPutaway(ref SqlDataReader r)
        {
            //
        }

    }
}