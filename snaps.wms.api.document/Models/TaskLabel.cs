using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace snaps.wms.api.document.Models
{
    public class TaskLabel
    {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string taskno { get; set; }
    }
}