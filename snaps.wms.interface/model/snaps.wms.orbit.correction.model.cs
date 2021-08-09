using System;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS { 

    public class orbit_correction  {
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public string accnops    { get; set; }
        public string seqops    { get; set; }
        public string codeops    { get; set; }
        public string typeops    { get; set; }
        public string thcode    { get; set; }
        public string article    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string unitops    { get; set; }
        public Int32 qtysku    { get; set; }
        public Int32 qtyweight    { get; set; }
        public string inreftype    { get; set; }
        public string inrefno    { get; set; }
        public string ingrno    { get; set; }
        public string inpromo    { get; set; }
        public string reason    { get; set; }
        public string xaction    { get; set; }
        public DateTimeOffset? xcreate    { get; set; }
        public DateTimeOffset? xmodify    { get; set; }
        public string xmsg    { get; set; }
        public Int32 rowid    { get; set; }
        public string orbitsite  { get; set; }
        public string orbitdepot { get; set; }
    }
}