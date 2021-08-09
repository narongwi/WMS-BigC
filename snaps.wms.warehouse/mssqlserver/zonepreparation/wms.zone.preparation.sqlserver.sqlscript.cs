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

namespace Snaps.WMS.warehouse {
    public partial class zoneprep_ops : IDisposable { 
        SqlConnection cn = new SqlConnection();
        public string sqlzoneprep_find = "select * from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea ";
        public string sqlzoneprep_validate = "select count(1) from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone ";

        public string sqlzoneprep_insert = " insert into wm_loczn (orgcode,site,depot,spcarea,przone,przonename,przonedesc,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,hutype, huvalweight, huvalvolume, hucapweight, hucapvolume) " + 
        " values (@orgcode, @site, @depot, @spcarea, @przone, @przonename,@przonedesc,@tflow, sysdatetimeoffset() ,@accncreate,sysdatetimeoffset(),@accnmodify,@procmodify,@hutype, @huvalweight, @huvalvolume, @hucapweight, @hucapvolume) ";
        public string sqlzoneprep_update = " update wm_loczn set przonename = @przonename, przonedesc = @przonedesc, tflow = @tflow, datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, " + 
        " procmodify = @procmodify, hutype = @hutype, huvalweight = @huvalweight, huvalvolume = @huvalvolume, hucapweight = @hucapweight, hucapvolume = @hucapvolume " + 
        " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone ";
        public string sqlzoneprep_remvld = "select count(1) rsl from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone";
        public string sqlzoneprep_remval = @"select count(1) rsl from wm_stock s where exists (select 1 from wm_loczp z where s.orgcode = z.orgcode and s.site = z.site and s.depot = z.depot 
        and s.loccode = z.lscode and przone = @przone and z.orgcode = @orgcode and z.site = @site and z.depot = @depot ) and orgcode = @orgcode and site = @site and depot = @depot";

        public string sqlzoneprep_remove_step1 = "delete from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone";
        public string sqlzoneprep_remove_step2 = "delete from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and przone = @przone";

        public string sqlzoneprln_find = " select * from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone ";
        //public string sqlzoneprln_isexists = @"select case when isnull((select count(1) from wm_locdw where lscode = @lscode and orgcode = @orgcode and site = @site and depot = @depot),0) = 0 then 'Location not found'
        //when isnull((select count(1) from wm_loczp where lscode = @lscode and orgcode = @orgcode and site = @site and depot = @depot),0) > 0 then 'Location has exists on prezone'
        //else 'pass' end";
        public string sqlzoneprln_isexists = @"select case
	        when isnull((select count(1) from wm_locdw where lscode = @lscode and orgcode = @orgcode and site = @site and depot = @depot),0) = 0 then 'Location not found'
	        when isnull((select count(1) from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and przone = @tempzone and lscode = @lscode and spcproduct = @spcproduct),0)>0 then 'article has exists on prepzone'
	        when isnull((select count(1) from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and przone <> @tempzone and lscode = @lscode),0)> 0 then 'Location has exists on prepzone'
            when isnull((select count(1) from wm_product wp where orgcode = @orgcode and site = @site and depot = @depot and wp.article = @spcproduct and wp.pv = @spcpv and wp.lv = @spclv),0) = 0 then 'article not found'
	        else 'pass' end";
        public string sqlzoneprln_ismodify = @"select case
         when isnull((select count(1) from wm_locdw where lscode = @lscode and orgcode = @orgcode and site = @site and depot = @depot),0) = 0 then 'Location not found'
            when @spcproduct is not null and isnull((select count(1) from wm_product wp where orgcode = @orgcode and site = @site and depot = @depot and wp.article = @spcproduct and wp.pv = @spcpv and wp.lv = @spclv),0) = 0 then 'article not found'
         else 'pass' end";
        public string sqlzoneprln_tempzone = @"select isnull((select top 1 bnvalue from wm_binary wb where wb.orgcode = @orgcode and wb.site = @site and depot = @depot and wb.bntype ='TEMPZONE' and bncode ='TEMP'),'XD')";
        public string sqlzoneprln_validate = "select count(1) from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone and lscode = @lscode";
        private string sqlzoneprep_getratio = "select case when @spcunit = 1 then 1 when @spcunit = 2 then rtoskuofipck when @spcunit = 3 then rtoskuofpck when @spcunit = 4 then rtoskuoflayer " +
        " when @spcunit = 5 then rtoskuofhu else 1 end from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @spcproduct and pv = @spcpv and lv = @spclv ";

        public string sqlzoneprln_insert = " insert into wm_loczp (orgcode,site,depot,spcarea,fltype,przone,lszone,lsaisle,lsbay,lslevel,lsloc,lsstack,lscode ,spcproduct " + 
        " ,spcpv,spclv,spcunit,spcthcode,lsdirection,lspath,tflow,lshash,datecreate,accncreate ,datemodify,accnmodify,procmodify, rtoskuofpu) " +
        " select orgcode,site,depot,spcarea,fltype,@przone przone,lszone,lsaisle,lsbay,lslevel,lsloc,lsstack,lscode " +
        "         ,@spcproduct spcproduct,@spcpv spcpv,@spclv spclv,@spcunit spcunit,@spcthcode spcthcode,@lsdirection lsdirection,@lspath lspath " + 
        "         ,@tflow tflow,@lshash lshash,sysdatetimeoffset(),accncreate ,sysdatetimeoffset(),accnmodify,'prep.zone.line' procmodify,@rtoskuofpu rto " +
        "  from wm_locdw where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and lscode = @lscode ";
        public string sqlzoneprln_update = " update wm_loczp set spcproduct = @spcproduct,spcpv = @spcpv,spclv = @spclv,spcunit = @spcunit,spcthcode = @spcthcode,lsdirection = @lsdirection," + 
        " lspath = @lspath,tflow = @tflow,datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify, rtoskuofpu = @rtoskuofpu " +
        " where orgcode = @orgcode and site = @site and depot = @depot " +
        " and spcarea = @spcarea and przone = @przone and lscode = @lscode";
        public string sqlzoneprln_remove = " delete from wm_loczp where  orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone and lscode = @lscode and spcproduct = @spcproduct and spcpv = @spcpv and spclv = @spclv";
        public string sqlzoneprln_remove2 = " delete from wm_loczp where  orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and przone = @przone and lscode = @lscode";

        public string sqlzoneprln_remval = @"select count(1) rsl from wm_stock s where exists (select 1 from wm_loczp z where s.orgcode = z.orgcode and s.site = z.site and s.depot = z.depot 
        and s.loccode = z.lscode and przone = @przone and z.orgcode = @orgcode and z.site = @site and z.depot = @depot and lscode = @lscode ) and orgcode = @orgcode and site = @site and depot = @depot";

        public zoneprep_ops(){ }
        public zoneprep_ops(string cx){ cn = new SqlConnection(cx); }
        public zoneprep_ops(SqlConnection ocn) { cn = ocn; }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }

            this.sqlzoneprep_find = null;
            this.sqlzoneprep_validate = null;
            this.sqlzoneprep_insert = null;
            this.sqlzoneprep_update = null;
            //this.sqlzoneprep_remove_step1 = null;
            this.sqlzoneprep_remove_step2 = null;
            
            this.sqlzoneprln_validate = null;
            this.sqlzoneprln_insert = null;
            this.sqlzoneprln_update = null;
            this.sqlzoneprln_remove = null;
            
            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}