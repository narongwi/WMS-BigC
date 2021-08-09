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

        private string sqlpolicy_insert = " insert into wm_policy (orgcode,site,depot,apcode,plccode,plcname,tflow,reqnumeric,requppercase,reqlowercase, " + 
        " reqspecialchar,spcchar,minlength,maxauthfail,exppdamobile,expandriod,expios,seckey,dayexpire,hashplc,datestart, " + 
        " dateend,datecreate,accncreate,datemodify,accnmodify,procmodify) values  " + 
        " ( @orgcode,@site,@depot,@apcode,@plccode,@plcname,@tflow,@reqnumeric,@requppercase,@reqlowercase,@reqspecialchar, " + 
        " @spcchar,@minlength,@maxauthfail,@exppdamobile,@expandriod,@expios,@seckey,@dayexpire,@hashplc,@datestart,@dateend, " + 
        " sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify ) " ;
        private string sqlpolicy_update = " update wm_policy set plcname = @plcname,tflow = @tflow,reqnumeric = @reqnumeric,requppercase = @requppercase, " + 
        " reqlowercase = @reqlowercase,reqspecialchar = @reqspecialchar,spcchar = @spcchar,minlength = @minlength, " + 
        " maxauthfail = @maxauthfail,exppdamobile = @exppdamobile,expandriod = @expandriod,expios = @expios, " + 
        " seckey = @seckey,dayexpire = @dayexpire,hashplc = @hashplc,datestart = @datestart,dateend = @dateend, " + 
        " datemodify = @datemodify,accnmodify = @accnmodify,procmodify = @procmodify  " + 
        " where  orgcode = @orgcode and site = @site and depot = @depot and plccode = @plccode  " ;
        private string sqlpolicy_remove = " delete from wm_policy where orgcode = @orgcode and site = @site and depot = @depot and plccode = @plccode ";
        private string sqlpolicy_get = "select top 1 * from wm_policy where orgcode = @orgcode and site = @site and depot = @depot ";
        public string sqlpolicy_vld = "select count(1) from wm_policy where orgcode = @orgcode and site = @site and depot = @depot and plccode = @plccode";
        private SqlConnection cn = new SqlConnection();
        public policy_ops() {  }
        public policy_ops(String cx) { cn = new SqlConnection(cx); }

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }

            this.sqlpolicy_insert = null;
            this.sqlpolicy_update = null;
            this.sqlpolicy_remove = null;
            this.sqlpolicy_get = null;
            
            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}