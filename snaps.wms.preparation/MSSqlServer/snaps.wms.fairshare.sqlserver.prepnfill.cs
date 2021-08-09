// using System;
// using System.Linq;
// using System.Data.Common;
// using System.Data.SqlClient;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Snaps.Helpers;
// using Snaps.Helpers.StringExt;
// using Snaps.Helpers.DbContext.SQLServer;
// namespace Snaps.WMS.preparation {
//     public partial class faitshare_ops : IDisposable {
//         private fairshare fairshare_get(ref SqlDataReader r) { 
//             fairshare rn = new fairshare();
//             rn.accncreate = r["accncreate"].ToString();
//             rn.accnmodify = r["accnmodify"].ToString();
//             rn.orgcode = r["orgcode"].ToString(); 
//             rn.site = r["site"].ToString();
//             rn.depot = r["depot"].ToString();
//             rn.sharecode = r["sharecode"].ToString();
//             rn.sharename = r["sharename"].ToString();
//             rn.hashcode  = r["hashcode"].ToString(); 
//             rn.tflow = r["tflow"].ToString(); 
//             rn.datecreate = r.GetDateTimeOffset(7);
//             rn.accncreate = r["accncreate"].ToString();
//             rn.datemodify = r.GetDateTimeOffset(9);
//             rn.accnmodify = r["accnmodify"].ToString();
//             return rn;
//         }

//         private SqlCommand fairshare_set(fairshare o,String sql){ 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.snapsPar(o.orgcode,"orgcode");
//             cm.snapsPar(o.site,"site");
//             cm.snapsPar(o.depot,"depot");
//             cm.snapsPar(o.sharecode,"sharecode");
//             cm.snapsPar(o.sharename,"sharename");
//             cm.snapsPar(o.hashcode,"hashcode");
//             cm.snapsPar(o.tflow,"tflow");
//             cm.snapsPar(o.accnmodify,"accnmodify");
//             cm.snapsPar(o.procmodify,"procmodify");
//             cm.snapsParsysdateoffset();
//             return cm;
//         }


//         //Line 
//         private fairshaln fairshare_get(ref SqlDataReader r) { 
//             fairshaln rn = new fairshaln();
//             rn.thcode = r["thcode"].ToString();
//             rn.priority = r.GetInt32(2);
//             rn.tflow = r["tflow"].ToString();
//             rn.datecreate = r.GetDateTimeOffset(4);
//             rn.accncreate = r["accncreate"].ToString();
//             rn.datemodify = r.GetDateTimeOffset(6);
//             rn.accnmodify = r["accnmodify"].ToString();
//             rn.procmodify = r["procmodify"].ToString();
//             return rn;
//         }
//         private SqlCommand prln_setmd(fairshaln o,String sql){ 
//             SqlCommand cm = new SqlCommand(sql,cn);
//             cm.snapsPar(o.orgcode,"orgcode");
//             cm.snapsPar(o.site,"site");
//             cm.snapsPar(o.depot,"depot");
//             cm.snapsPar(o.thcode,"thcode");
//             cm.snapsPar(o.priority,"priority");
//             cm.snapsPar(o.tflow,"tflow");
//             cm.snapsPar(o.accncreate,"accncreate");
//             cm.snapsPar(o.accnmodify,"accnmodify");
//             cm.snapsParsysdateoffset();
//             return cm;
//         }

//     }
// }