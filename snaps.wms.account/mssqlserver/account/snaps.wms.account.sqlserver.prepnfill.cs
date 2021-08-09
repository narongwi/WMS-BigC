using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.Hash;
using Snaps.Helpers.Json;
using Snaps.Helpers.DbContext.SQLServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Snaps.WMS {

    public partial class accn_ops : IDisposable { 
        private void fillCommand(ref SqlCommand cm, accn_md o){ 
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.accntype,"accntype");
            cm.snapsPar(o.accncode,"accncode");
            cm.snapsPar(o.accnname,"accnname");
            cm.snapsPar(o.accnsurname,"accnsurname");
            cm.snapsPar(o.email,"email");
            cm.snapsPar(o.mobileno,"mobileno");
            cm.snapsPar(o.accnapline,"accnapline");
            cm.snapsPar(o.dateexpire,"dateexpire");
            cm.snapsPar(o.sessionexpire,"sessionexpire");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accnavartar,"accnavartar");
            cm.snapsPar(o.tkrqpriv,"tkrqpriv");
            cm.snapsPar(o.cntfailure,"cntfailure");
            cm.snapsPar(o.accscode,"accscode");
            cm.snapsPar(o.datelogin,"datelogin");
            cm.snapsPar(o.datelogout,"datelogout");
            cm.snapsPar(o.datechnpriv,"datechnpriv");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.accsrole,"accsrole");
            cm.snapsPar(o.formatdateshort,"formatdateshort");
            cm.snapsPar(o.formatdatelong,"formatdatelong");
            cm.snapsPar(o.formatdate,"formatdate");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
        }
        private SqlCommand getCommand(accn_md o){
            SqlCommand cm = new SqlCommand();
            fillCommand(ref cm, o);
            return cm;
        }

        private SqlCommand setPriv(accn_priv o,string accncreate) { 
            SqlCommand cm = new SqlCommand(sqlaccount_insert_step2,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar("WMS","apcode");
            cm.snapsPar(o.accncode,"accncode");            
            cm.snapsPar((o.accncode.ClearReg()+o.newpriv.ClearReg()).ToHash(),"accnpriv");
            cm.snapsPar("IO","tflow");
            cm.snapsPar(DateTimeOffset.Now.ToString().ToHash(),"hashpriv");
            cm.snapsPar(DateTimeOffset.Now.AddDays(o.lifetime),"dateexpire");
            cm.snapsPar(accncreate,"accncreate");
            cm.snapsPar("accn.upsert","procmodify");
           
            return cm;
        }

        private SqlCommand updPriv(accn_priv o, string accnmodify,string procmodify)
        {
            SqlCommand cm = new SqlCommand(sqlaccount_update_passw, cn);
            cm.snapsPar(o.orgcode, "orgcode");
            cm.snapsPar("WMS", "apcode");
            cm.snapsPar(o.accncode, "accncode");
            cm.snapsPar(o.newpriv.ClearReg().ToHash(), "accnpriv");
            cm.snapsPar(DateTimeOffset.Now.ToString().ToHash(), "hashpriv");
            cm.snapsPar(DateTimeOffset.Now.AddDays(o.lifetime), "dateexpire");
            cm.snapsPar(accnmodify, "accnmodify");
            cm.snapsPar(procmodify, "procmodify");

            return cm;
        }
        private accn_md setAccn(ref SqlDataReader r) { 
            accn_md rn = new accn_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.accntype = r["accntype"].ToString();
            rn.accncode = r["accncode"].ToString();
            rn.accnname = r["accnname"].ToString();
            rn.accnsurname = r["accnsurname"].ToString();
            rn.email = r["email"].ToString();
            rn.mobileno = r["mobileno"].ToString();
            rn.accnapline = r["accnapline"].ToString();
            rn.dateexpire = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
            rn.sessionexpire = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.tflow = r["tflow"].ToString();
            rn.accnavartar = r["accnavartar"].ToString();
            rn.tkrqpriv = r["tkrqpriv"].ToString();
            rn.cntfailure = (r.IsDBNull(13)) ? 0 :  r.GetInt32(13);
            rn.accscode = r["accscode"].ToString();
            rn.datelogin = (r.IsDBNull(15)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(15);
            rn.datelogout = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(16);
            rn.datechnpriv = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17);
            rn.datecreate = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(18);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.accsrole = r["accsrole"].ToString();
            rn.formatdateshort = r["formatdateshort"].ToString();
            rn.formatdatelong = r["formatdatelong"].ToString();
            rn.formatdate = r["formatdate"].ToString();
            return rn;
        }

        private accn_ls setls(ref SqlDataReader r) { 
            accn_ls rn = new accn_ls();
            rn.orgcode = r["orgcode"].ToString();
            rn.accntype = r["accntype"].ToString();
            rn.accncode = r["accncode"].ToString();
            rn.accnname = r["accnname"].ToString() + "  " +  r["accnsurname"].ToString();
            rn.email = r["email"].ToString();
            rn.dateexpire = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
            rn.sessionexpire = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.tflow = r["tflow"].ToString();
            rn.accnavartar = r["accnavartar"].ToString();
            rn.formatdateshort = r["formatdateshort"].ToString();
            rn.formatdatelong = r["formatdatelong"].ToString();
            rn.formatdate = r["formatdate"].ToString();
            return rn;
        }


    }
}