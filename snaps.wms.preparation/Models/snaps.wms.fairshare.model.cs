 using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.preparation {
    public class fairshare {
        public string orgcode { get; set; }
        public string site { get; set ;} 
        public string depot { get; set; }
        public string sharecode { get; set; }
        public string sharename { get; set; } 
        public string hashcode { get; set; }
        public string tflow { get; set; } 
        public DateTimeOffset datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodify { get; set ;} 
        public List<fairshaln> lines { get; set; }
    }

    public class fairshaln  { 
        public string thcode { get; set; }
        public Int32 priority { get; set; } 
        public string tflow {get; set; }
        public DateTimeOffset datecreate { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset datemodify { get; set; }
        public string accnmodify { get; set; }
        public string procmodify { get; set ;} 
    }
}