using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public class config_md   {
        public string orgcode    { get; set; }
        public string apcode    { get; set; }
        public string accncode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string cfgtype    { get; set; }
        public string cfgcode    { get; set; }
        public string cfgname    { get; set; }
        public string cfgvalue    { get; set; }
        public string cfghash    { get; set; }
        public string tflow    { get; set; }
        public string formatdate    { get; set; }
        public string formatdateshort    { get; set; }
        public string formatdatelong    { get; set; }
        public string unitdimension    { get; set; }
        public string unitweight    { get; set; }
        public string unitvolume    { get; set; }
        public string unitcubic    { get; set; }
        public Int32 pagelimit    { get; set; }
        public string lang    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
        
        public accn_roleacs roleaccs { get; set; }
        public Int32 isdefault { get; set; }
    }
}