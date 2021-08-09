using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.WMS.warehouse;

namespace Snaps.WMS {
    public partial class depot_ops : IDisposable { 

        public void fillCommand(ref SqlCommand cm, depot_md o) {
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.sitecode,"sitecode");
            cm.snapsPar(o.depottype,"depottype");
            cm.snapsPar(o.depotcode,"depotcode");
            cm.snapsPar(o.depotname,"depotname");
            cm.snapsPar(o.depotnamealt,"depotnamealt");
            cm.snapsPar(o.datestart,"datestart");
            cm.snapsPar(o.dateend,"dateend");
            cm.snapsPar(o.depotkey,"depotkey");
            cm.snapsPar(o.depotops,"depotops");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.depothash,"depothash");
            cm.snapsPar(o.unitweight,"unitweight");
            cm.snapsPar(o.unitvolume,"unitvolume");
            cm.snapsPar(o.unitdimension,"unitdimension");
            cm.snapsPar(o.unitcubic,"unitcubic");
            cm.snapsPar(o.formatdate,"formatdate");
            cm.snapsPar(o.formatdateshort,"formatdateshort");
            cm.snapsPar(o.formatdatelong,"formatdatelong");
            cm.snapsPar(o.tflow,"tflow");
        }
        public SqlCommand getCommand(depot_md o,string sqlcmd = "") {
            SqlCommand cm = new SqlCommand(sqlcmd,cn);
            fillCommand(ref cm, o);
            return cm;
        }
        public depot_md setDepot(ref SqlDataReader r) {
            depot_md rn = new depot_md();
            rn.sitecode = r["sitecode"].ToString();
            rn.depottype = r["depottype"].ToString();
            rn.depotcode = r["depotcode"].ToString();
            rn.depotname = r["depotname"].ToString();
            rn.depotnamealt = r["depotnamealt"].ToString();
            rn.datestart = (r.IsDBNull(6)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(6);
            rn.dateend = (r.IsDBNull(7)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(7);
            rn.depotkey = "";
            rn.depotops = r["depotops"].ToString();
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(13)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(13);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.depothash = r["depothash"].ToString();
            rn.unitweight = r["unitweight"].ToString();
            rn.unitvolume = r["unitvolume"].ToString();
            rn.unitdimension = r["unitdimension"].ToString();
            rn.unitcubic = r["unitcubic"].ToString();
            rn.formatdate = r["formatdate"].ToString();
            rn.formatdateshort = r["formatdateshort"].ToString();
            rn.formatdatelong = r["formatdatelong"].ToString();
            return rn;
        }

    }
}