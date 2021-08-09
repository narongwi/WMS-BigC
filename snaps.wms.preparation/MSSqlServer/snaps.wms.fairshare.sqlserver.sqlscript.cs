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
//     public partial class fairshare_ops : IDisposable {

//         public static string share_tbn = "wm_distshare";
//         public static string share_force = " and orgcode = @orgcode and site = @site  and depot = @depot  and sharecode = @sharecode " ;
//         public static string share_forceln = " and orgcode = @orgcode and site = @site  and depot = @depot  and sharecode = @sharecode and thcode = @thcode "

//         public static string share_seq = "next value for seq_distshare";
//         public static string share_fnd = String.format("select * from {0}", share_tbn);
//         public static string share_ins = String.format("INSERT INTO wm_distshare (orgcode,site,depot,sharecode,sharename,hashcode,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify) " + 
//         " VALUES (@orgcode,@site,@depot,{0},@sharename,@hashcode,@tflow,@sysdate,@accncreate, @sysdate ,@accnmodify,@procmodify) ", share_seq);
//         public static string share_insln = "INSERT INTO dbo.wm_distshaln (orgcode,site,depot,sharecode,thcode,priority,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify) " + 
//         " VALUES (@orgcode,@site,@depot,@sharecode,@thcode,@priority,@tflow,@sysdate,@accncreate,@sysdate,@accnmodify,@procmodify)";

//         public static string share_upd = "UPDATE dbo.wm_distshare SET sharename = @sharename, ,hashcode = @hashcode,tflow = @tflow,datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify" + 
//         " WHERE 1=1 ";
//         public static string share_updln = "UPDATE dbo.wm_distshaln SET priority = @priority ,tflow = @tflow ,datemodify = @sysdate ,accnmodify = @accnmodify ,procmodify = @procmodify " + 
//         " WHERE 1=1 ";

//         public static string share_rem = String.format(" delete from wm_distshare where 1=1 {0}", share_force);
//         public static string share_remln = String.format(" delete from wm_distshaln where 1=1 {0}", share_forceln);

//         public static string share_val = string.format("select count(1) rsl from wm_distshare where 1=1 {0} ", share_force);
//         public static string share_valln = string.format("select count(1) rsl from wm_distshaln where  1=1 {0}", share_forceln);
        

//         private SqlConnection cn = null;
//         public fairshare_ops() {  }
//         public fairshare_ops(String cx) { cn = new SqlConnection(cx); }

//         private bool disposedValue = false;
//         protected virtual void Dispose(bool Disposing){ 
//             if(!disposedValue) { 
//                 //if (cn != null) { cn.Dispose(); } sqlval = null; sqlins = null; sqlupd = null; sqlrem = null; sqlfnd = null; tbn = null; sqlmcom = null;

//                 prep_opspick_stp1 = null;
//                 prep_opspick_stp2 = null;
//                 prep_opscancel_stp1 = null;

//             }
//             disposedValue = true;
//         }
//         public void Dispose() {
//             GC.SuppressFinalize(this);
//         }
//     }
// }