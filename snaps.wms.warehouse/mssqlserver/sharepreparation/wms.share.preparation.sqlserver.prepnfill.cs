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

namespace Snaps.WMS {
    public partial class shareprep_ops : IDisposable { 

        public void fillCommand(ref SqlCommand cm, shareprep_md o) {
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.shprep,"shprep");
            cm.snapsPar(o.shprepname,"shprepname");
            cm.snapsPar(o.shprepdesc,"shprepdesc");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.isfullfill,"isfullfill");
        }
        public SqlCommand getCommand(shareprep_md o) {
            SqlCommand cm = new SqlCommand("",cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.shprep,"shprep");
            cm.snapsPar(o.shprepname,"shprepname");
            cm.snapsPar(o.shprepdesc,"shprepdesc");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.isfullfill,"isfullfill");
             return cm;
        }
        public shareprep_md setShareprep(ref SqlDataReader r) {
            shareprep_md rn = new shareprep_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.shprep = r["shprep"].ToString();
            rn.shprepname = r["shprepname"].ToString();
            rn.shprepdesc = r["shprepdesc"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.isfullfill = (r.IsDBNull(14)) ? 0 :  r.GetInt32(14);
             return rn;
        }

        public void fillCommand(ref SqlCommand cm, shareprln_md o){ 
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.shprep,"shprep");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.priority,"priority");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
        }
        public SqlCommand GetCommand(shareprln_md o){ 
            SqlCommand cm = new SqlCommand();
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.shprep,"shprep");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.priority,"priority");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            return cm;
        }
        public shareprln_md setShareprln(ref SqlDataReader r){ 
            shareprln_md rn = new shareprln_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.shprep = r["shprep"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.priority = (r.IsDBNull(5)) ? 0 :  r.GetInt32(5);
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(7)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(7);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.thname = r["thcodealt"].ToString();
            return rn;
        }

    }
}