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
    public partial class policy_ops { 
        private void fillCommand(ref SqlCommand cm, policy_md o){ 
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.apcode,"apcode");
            cm.snapsPar(o.plccode,"plccode");
            cm.snapsPar(o.plcname,"plcname");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.reqnumeric,"reqnumeric");
            cm.snapsPar(o.requppercase,"requppercase");
            cm.snapsPar(o.reqlowercase,"reqlowercase");
            cm.snapsPar(o.reqspecialchar,"reqspecialchar");
            cm.snapsPar(o.spcchar,"spcchar");
            cm.snapsPar(o.minlength,"minlength");
            cm.snapsPar(o.maxauthfail,"maxauthfail");
            cm.snapsPar(o.exppdamobile,"exppdamobile");
            cm.snapsPar(o.expandriod,"expandriod");
            cm.snapsPar(o.expios,"expios");
            cm.snapsPar(o.seckey,"seckey");
            cm.snapsPar(o.dayexpire,"dayexpire");
            cm.snapsPar(o.hashplc,"hashplc");
            cm.snapsPar(o.datestart,"datestart");
            cm.snapsPar(o.dateend,"dateend");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
        }
        private SqlCommand getCommand(policy_md o){
            SqlCommand cm = new SqlCommand("",cn);
            fillCommand(ref cm, o);
            return cm;
        }
        private policy_md setPolicy(ref SqlDataReader r) {
            policy_md rn = new policy_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.apcode = r["apcode"].ToString();
            rn.plccode = r["plccode"].ToString();
            rn.plcname = r["plcname"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.reqnumeric = (r.IsDBNull(7)) ? 0 :  r.GetInt32(7);
            rn.requppercase = (r.IsDBNull(8)) ? 0 :  r.GetInt32(8);
            rn.reqlowercase = (r.IsDBNull(9)) ? 0 :  r.GetInt32(9);
            rn.reqspecialchar = (r.IsDBNull(10)) ? 0 :  r.GetInt32(10);
            rn.spcchar = r["spcchar"].ToString();
            rn.minlength = (r.IsDBNull(12)) ? 0 :  r.GetInt32(12);
            rn.maxauthfail = (r.IsDBNull(13)) ? 0 :  r.GetInt32(13);
            rn.exppdamobile = (r.IsDBNull(14)) ? 0 :  r.GetInt32(14);
            rn.expandriod = (r.IsDBNull(15)) ? 0 :  r.GetInt32(15);
            rn.expios = (r.IsDBNull(16)) ? 0 :  r.GetInt32(16);
            rn.seckey = r["seckey"].ToString();
            rn.dayexpire = (r.IsDBNull(18)) ? 0 :  r.GetInt32(18);
            rn.hashplc = r["hashplc"].ToString();
            rn.datestart = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20);
            rn.dateend = (r.IsDBNull(21)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(21);
            rn.datecreate = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            return rn;
        }
    }
}