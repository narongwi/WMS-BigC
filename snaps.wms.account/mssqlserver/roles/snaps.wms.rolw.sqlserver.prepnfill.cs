using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Newtonsoft.Json;
using Snaps.Helpers.Json;

namespace Snaps.WMS {
    public partial class role_ops : IDisposable { 
        private void fillCommand(ref SqlCommand cm, role_md o){ 
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.apcode,"apcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.rolecode,"rolecode");
            cm.snapsPar(o.rolename,"rolename");
            cm.snapsPar(o.roledesc,"roledesc");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.hashrol,"hashrol");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.roleaccs[0].toJson(),"roljson");
        }
        private void fillCommand(ref SqlCommand cm, roln_md o, role_md e) { 
            cm.snapsPar(e.orgcode,"orgcode");
            cm.snapsPar(e.apcode,"apcode");
            cm.snapsPar(e.rolecode,"rolecode");
            cm.snapsPar(o.objmodule,"objmodule");
            cm.snapsPar(o.objtype,"objtype");
            cm.snapsPar(o.objcode,"objcode");
            cm.snapsPar(o.isenable,"isenable");
            cm.snapsPar(o.isexecute,"isexecute");
            cm.snapsPar(o.hashrln,"hashrln");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
        }
        private SqlCommand getCommand(role_md o){
            SqlCommand cm = new SqlCommand("",cn);
            fillCommand(ref cm, o);
            return cm;
        }
         private SqlCommand getCommand(role_md e, roln_md o,string sql = ""){
            SqlCommand cm = new SqlCommand("",cn);
            fillCommand(ref cm, o,e);
            return cm;
        }
    
        private role_md setRole(ref SqlDataReader r, Boolean reqdec = false) {
            role_md rn = new role_md();
            accn_roleacs ac = new accn_roleacs();

            rn.orgcode = r["orgcode"].ToString();
            rn.apcode = r["apcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.rolecode = r["rolecode"].ToString();
            rn.rolename = r["rolename"].ToString();
            rn.roledesc = r["roledesc"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.hashrol = r["hashrol"].ToString();
            rn.datecreate = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.roljson = r["roljson"].ToString();
            if (reqdec == true) { 
                
               // ac = JsonConvert.DeserializeObject<accn_roleacs>(r["roljson"].ToString());
                rn.roleaccs = new List<accn_roleacs> ();
                rn.roleaccs.Add(JsonConvert.DeserializeObject<accn_roleacs>(r["roljson"].ToString()));
            }    
            return rn;
        }
        private role_md setRomd(ref SqlDataReader r, Boolean reqdec = false)
        {
            role_md rn = new role_md();
            accn_roleacs ac = new accn_roleacs();

            rn.orgcode = r["orgcode"].ToString();
            rn.apcode = r["apcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.rolecode = r["rolecode"].ToString();
            rn.rolename = r["rolename"].ToString();
            rn.roledesc = r["roledesc"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.hashrol = r["hashrol"].ToString();
            rn.datecreate = (r.IsDBNull(9)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(9);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(11)) ? (DateTimeOffset?)null : r.GetDateTimeOffset(11);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.roljson = r["roljson"].ToString();
            if (reqdec == true)
            {

                // ac = JsonConvert.DeserializeObject<accn_roleacs>(r["roljson"].ToString());
                rn.roleaccs = new List<accn_roleacs>();
                rn.roleaccs.Add(JsonConvert.DeserializeObject<accn_roleacs>(r["roljson"].ToString()));
            }
            return rn;
        }
        private roln_md setRoln(ref SqlDataReader r) {
            roln_md rn = new roln_md();
            // rn.orgcode = r["orgcode"].ToString();
            // rn.apcode = r["apcode"].ToString();
            // rn.rolecode = r["rolecode"].ToString();
            rn.objmodule = r["objmodule"].ToString();
            rn.objtype = r["objtype"].ToString();
            rn.objcode = r["objcode"].ToString();
            rn.isenable = (r.IsDBNull(6)) ? 0 :  r.GetInt32(6);
            rn.isexecute = (r.IsDBNull(7)) ? 0 :  r.GetInt32(7);
            // rn.hashrln = r["hashrln"].ToString();
            // rn.datecreate = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            // rn.accncreate = r["accncreate"].ToString();
            // rn.datemodify = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11);
            // rn.accnmodify = r["accnmodify"].ToString();
            // rn.procmodify = r["procmodify"].ToString();
            return rn;
        }



    }
}