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
    public partial class role_ops : IDisposable { 
        public string sqlrole_find = "select * from wm_role where orgcode = @orgcode and site = @site";// and depot = @depot
        public string sqlrole_master = "select * from wm_roms where orgcode = 'bgc' and rolecode = 'master' and objmodule = 'category' and tflow='IO' ";
        public string sqlrole_exists = "select count(1) from wm_role where orgcode = @orgcode and site = @site and depot = @depot and rolecode = @rolecode";
        public string sqlrole_insert = " insert into wm_role ( orgcode,apcode,site,depot,rolecode,rolename,roledesc,tflow,hashrol, " + 
        " datecreate,accncreate,datemodify,accnmodify,procmodify,roljson ) values  " + 
        " ( @orgcode,@apcode,@site,@depot,@rolecode,@rolename,@roledesc,'IO',@hashrol, " + 
        " sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify,@roljson ) " ; 

        public string sqlrole_update = " update wm_role set rolename = @rolename,roledesc = @roledesc,tflow = @tflow,hashrol = @hashrol, " + 
        " datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify,roljson = @roljson " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and rolecode = @rolecode " ;
        public string sqlrole_remove_step0 = " select count(1) from wm_acfg where orgcode = @orgcode and site = @site and depot = @depot and cfgcode = @rolecode";
        public string sqlrole_remove_step1 = " delete from wm_role where orgcode = @orgcode and site = @site and depot = @depot and rolecode = @rolecode " ;
        public string sqlrole_remove_step2 = " delete from wm_roln where orgcode = @orgcode and site = @site and depot = @depot and rolecode = @rolecode " ;

        //public string sqlroln_fnd = "select * from wm_roln where orgcode = @orgcode and site = @site and depot = @depot and rolecode = @rolecode";
        public string sqlroln_fnd = "select * from wm_roln where orgcode = @orgcode and rolecode = @rolecode";
        public string sqlroln_master = "select * from wm_roms where orgcode = 'bgc' and rolecode = 'master' and objmodule != 'category' and tflow='IO'";
        public string sqlroln_insert = " insert into wm_roln (orgcode,apcode,rolecode,objmodule,objtype,objcode,isenable,isexecute, " + 
        " hashrln,datecreate,accncreate,datemodify,accnmodify,procmodify ) values   " + 
        " ( @orgcode,@apcode,@rolecode,@objmodule,@objtype,@objcode,@isenable,@isexecute,@hashrln, " + 
        " sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify) " ;
        public string sqlroln_update = " update wm_roln set isenable = @isenable,isexecute = @isexecute,hashrln = @hashrln, " + 
        " datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify " + 
        " where orgcode = @orgcode and apcode = @apcode and rolecode = @rolecode  " + 
        " and objmodule = @objmodule and objtype = @objtype and objcode = @objcode " ;

        private SqlConnection cn ;
        public role_ops() {  }
        public role_ops(String cx) { cn = new SqlConnection(cx); }
        public role_ops(SqlConnection cx) { this.cn = cx; }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }
            this.sqlrole_find = null;
            this.sqlrole_insert = null;
            this.sqlrole_update = null;
            this.sqlrole_remove_step1 = null;
            this.sqlrole_remove_step2 = null;

            this.sqlroln_fnd = null;
            this.sqlroln_insert = null;
            this.sqlroln_update = null; 
            
            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}