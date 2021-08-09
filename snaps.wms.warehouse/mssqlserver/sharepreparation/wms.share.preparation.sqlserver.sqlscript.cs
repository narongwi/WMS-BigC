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
        private SqlConnection cn;
        private string cnx;

        private string sqlshareprep_fnd = "" + 
        " select * from wm_shareprep where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqlshareprep_vald = "select count(1) rsl from wm_shareprep where orgcode = @orgcode and site = @site and depot = @depot and shprep = @shprep";
        private string sqlshareprep_insert = "" + 
        " insert into wm_shareprep ( orgcode,site,depot,spcarea,shprep,shprepname,shprepdesc,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,isfullfill ) " + 
        " values ( @orgcode,@site,@depot,@spcarea,next value for seq_shareprep,@shprepname,@shprepdesc,@tflow,sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify, " +
        " @isfullfill )";
        private string sqlshareprep_update = "update wm_shareprep set shprepname = @shprepname,shprepdesc = @shprepdesc,priority = @priority,tflow = @tflow,datemodify = @datemodify, " + 
        " accnmodify = sysdatetimeoffset(),procmodify = @procmodify,isfullfill = @isfullfill where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea " + 
        " and shprep = @shprep";
        private string sqlshareprep_remove = " delete from wm_shareprep where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and shprep = @shprep ";

        private string sqlshareprln_fnd = "select l.*,t.thcodealt from wm_shareprln l left join wm_thparty t on l.orgcode = t.orgcode and l.site = t.site and l.depot = t.depot " + 
        " and l.thcode = t.thcode where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.shprep = @shprep";
        private string sqlshareprln_vald = "select count(1) rsl from wm_shareprln where orgcode = @orgcode and site = @site and depot = @depot and shprep = @shprep and thcode= @thcode ";
        private string sqlshareprln_insert = "insert into wm_shareprln ( orgcode,site,depot,shprep,thcode,priority,tflow,datecreate,accncreate,datemodify,accnmodify ) " + 
        " values ( @orgcode,@site,@depot,@shprep,@thcode,@priority,@tflow,sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify ) ";
        private string sqlshareprln_update = "update wm_shareprln  set priority = @priority, datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,tflow = @tflow " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and shprep = @shprep and thcode = @thcode ";
        private string sqlshareprln_remove = " delete from wm_shareprln where orgcode = @orgcode and site = @site and depot = @depot and shprep = @shprep and thcode = @thcode ";


        public shareprep_ops(){ }
        public shareprep_ops(string cx){ cn = new SqlConnection(cx); this.cnx = cx; }
        public shareprep_ops(SqlConnection ocn) { cn = ocn; }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }

            this.sqlshareprep_fnd = null;
            this.sqlshareprep_vald = null;
            this.sqlshareprep_insert = null;
            this.sqlshareprep_update = null;
            this.sqlshareprep_remove = null;
            
            this.sqlshareprln_fnd = null;
            this.sqlshareprln_vald = null;
            this.sqlshareprln_insert = null;
            this.sqlshareprln_update = null;
            this.sqlshareprln_remove = null; 
            
            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }
}