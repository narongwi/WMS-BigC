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
    public partial class config_ops { 
        private string sqlconfig_find = "select * from wm_acfg where orgcode = @orgcode and site = @site and depot = @depot ";
        private string sqlconfig_insert = "" + 
        " insert into wm_acfg ( orgcode,apcode,accncode,site,depot,cfgtype,cfgcode,cfgname,cfgvalue,cfghash,tflow, " + 
        " formatdate,formatdateshort,formatdatelong,unitdimension,unitweight,unitvolume,unitcubic,pagelimit,lang, " +
        " datecreate,accncreate,datemodify,accnmodify,procmodify,isdefault ) " +
        " values ( @orgcode,@apcode,@accncode,@site,@depot,@cfgtype,@cfgcode,@cfgname,@cfgvalue,@cfghash,@tflow, " +
        " @formatdate,@formatdateshort,@formatdatelong,@unitdimension,@unitweight,@unitvolume,@unitcubic,@pagelimit, " +
        " @lang,sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify,@isdefault ) " ;
        private string sqlconfig_update = "" + 
        " update wm_acfg  set cfgcode = @cfgcode,cfgname = @cfgname,cfgvalue = @cfgvalue,cfghash = @cfghash, " +
        " tflow = @tflow,formatdate = @formatdate,formatdateshort = @formatdateshort,formatdatelong = @formatdatelong, " +
        " unitdimension = @unitdimension,unitweight = @unitweight,unitvolume = @unitvolume,unitcubic = @unitcubic, " +
        " pagelimit = @pagelimit,lang = @lang,datecreate = @datecreate,accncreate = @accncreate,datemodify = @datemodify, " +
        " accnmodify = @accnmodify,procmodify = @procmodify " +
        " where orgcode = @orgcode and apcode = @apcode and accncode = @accncode and site = @site and depot = @depot cfgtype = @cfgtype ";
        private string sqlconfig_remove = "delete from wm_acfg where orgcode = @orgcode and site = @site and depot = @depot ";

        private string sqlconfig_default = " select c.*,r.roljson from wm_accn a, wm_acfg c, wm_role r where c.orgcode = r.orgcode " +
        " and c.cfgcode = r.rolecode and cfgtype = 'role' and c.tflow = 'IO' and r.tflow = 'IO' " +
        " and a.orgcode = c.orgcode and a.accncode = c.accncode and a.accscode = @accscode and c.site = @site";

        private string sqlconfig_pda = " select c.*,r.roljson from wm_accn a, wm_acfg c, wm_role r where c.orgcode = r.orgcode " +
   " and c.cfgcode = r.rolecode and cfgtype = 'role' and c.tflow = 'IO' and r.tflow = 'IO' " +
   " and a.orgcode = c.orgcode and a.accncode = c.accncode and a.accscode = @accscode ";
        private SqlConnection cn ;
        public config_ops() {  }
        public config_ops(String cx) { cn = new SqlConnection(cx); }
        public config_ops(SqlConnection cx) { this.cn = cx; }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }

            this.sqlconfig_insert = null;
            this.sqlconfig_update = null;
            this.sqlconfig_remove = null;
            
            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    } 
}
