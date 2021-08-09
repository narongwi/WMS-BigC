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
    public partial class binary_ops : IDisposable { 
        SqlConnection cn = new SqlConnection();
        
        public string sqlbinary_desc = " select * from wm_binary where bntype = 'DESC' and orgcode = @orgcode and site = @site and depot = @depot and apps = 'WMS' order by bncode";
        public string sqlbinary_find = " select * from wm_binary where orgcode = @orgcode and site = @site and depot = @depot and bntype = @bncode and bncode = @bnvalue and apps = 'WMS' ";
        public string sqlbinary_validate = "select count(1) from wm_binary where orgcode = @orgcode and site = @site and depot = @depot and bntype = @bntype and bncode = @bncode and bnvalue = @bnvalue and apps = 'WMS' ";
        public string sqlbinary_insert = " insert into wm_binary ( orgcode,site,depot,apps,bntype,bncode,bnvalue,bndesc,bndescalt,bnflex1,bnflex2,bnflex3,bnflex4,bnicon,bnstate,datecreate,accncreate,datemodify,accnmodify ) " + 
        " values ( @orgcode,@site,@depot,@apps,@bntype,@bncode,@bnvalue,@bndesc,@bndescalt,@bnflex1,@bnflex2,@bnflex3,@bnflex4,@bnicon,@bnstate,sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accnmodify ) ";
        public string sqlbinary_update = " update wm_binary set bndesc = @bndesc,bndescalt = @bndescalt,bnflex1 = @bnflex1,bnflex2 = @bnflex2,bnflex3 = @bnflex3,bnflex4 = @bnflex4, " + 
        " bnicon = @bnicon,bnstate = @bnstate,datemodify = sysdatetimeoffset(),accnmodify = @accnmodify where orgcode = @orgcode and site = @site and depot = @depot and bntype = @bntype and bncode = @bncode and bnvalue = @bnvalue ";
        public string sqlbinary_remove = "delete from wm_binary where orgcode = @orgcode and site = @site and depot = @depot and bntype = @bntype and bncode = @bncode and bnvalue = @bnvalue and apps = 'WMS' ";


        //public string sqllov_warehouse = "select sitecode,sitenamealt from wm_warehouse where orgcode = @orgcode and tflow = 'IO' order by sitecode";
        public string sqllov_warehouseall = @"SELECT w.orgcode ,w.sitecode ,w.sitenamealt , d.depotcode,d.depotname FROM wm_warehouse w JOIN wm_depot d on w.orgcode = d.orgcode and w.sitecode = d.sitecode and w.tflow ='IO' and d.tflow ='IO' order by w.sitecode";
        public string sqllov_warehouse = @"SELECT w.orgcode ,w.sitecode ,w.sitenamealt , d.depotcode,d.depotname FROM wm_warehouse w JOIN wm_depot d on w.orgcode = d.orgcode and w.sitecode = d.sitecode and w.tflow ='IO' and d.tflow ='IO' and  w.orgcode = @orgcode order by w.sitecode";
        public string sqllov_depot = "select depotcode,depotnamealt from wm_depot where orgcode =  @orgcode and sitecode = @site and tflow = 'IO' order by depotcode";
        public string sqllov_role = "select rolecode, rolename from wm_role where orgcode = @orgcode and site = @site and depot = @depot and tflow = 'IO' order by rolecode";
        public string sqllov_find = "select * from wm_binary where orgcode = @orgcode and site = @site and depot = @depot and bnstate = 'IO' and bntype = @bntype and bncode =  @bncode and apps = 'WMS' order by bnvalue";
        public string sqllov_prepzonestock = "select przone, przonename from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and tflow = 'IO' order by przone";
        public string sqllov_prepzonedist = "select przone, przonename from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'XD' and tflow = 'IO' order by przone";
        public string sqllov_prepzonefowd = "select przone, przonename from wm_loczn where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'FW' and tflow = 'IO' order by przone";
        public string sqllov_storagezone = "select lscode ,lscodealt from wm_locup where fltype = 'ZN' and spcarea = 'ST' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";
        public string sqllov_sharedist = "select shprep,shprepname from wm_shareprep where orgcode = @orgcode and site = @site and depot = @depot order by shprepname";
        public string sqllov_hutype = "select article,descalt from wm_product where articletype = 'P' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by article";
        public string sqllov_storagecount = @"SELECT 1 AS seq, lscode ,lscodealt,spcarea,fltype 
                from wm_locup where fltype = 'ZN' and spcarea = 'ST' and tflow = 'IO' and lower(orgcode) = @orgcode and site = @site and depot = @depot
                UNION ALL 
                SELECT 1 AS seq, spcarea as lscode ,
		                (case when spcarea = 'RS' then 'RCV Staging' 
			                  when spcarea = 'DS' then 'DSP Staging'
			                  when spcarea = 'BL' then 'Bulk'
			                  when spcarea = 'DM' then 'Damage'
			                  when spcarea = 'SB' then 'Sinbin'
			                  when spcarea = 'EX' then 'Exchange'
			                  when spcarea = 'OV' then 'Overflow'
			                  when spcarea = 'RN' then 'Return'
			                  when spcarea = 'AS' then 'Assembly'
			                  when spcarea = 'FW' then 'Forwarding'
			                  when spcarea = 'PD' then 'Pick&Drop'
		                  else spcarea end) lscodealt,
		                spcarea,fltype 
                from wm_locup wl where fltype = 'BL' and tflow = 'IO' and lower(orgcode) = @orgcode and site = @site and depot = @depot
                AND exists (select 1 FROM wm_binary wb WHERE wb.orgcode=wl.orgcode and wb.site = wl.site and wb.depot = wl.depot AND wb.bntype='LOCATION' AND wb.bncode='AREA' and wb.bnvalue = wl.spcarea)
                group by fltype, spcarea,fltype
                order BY seq,spcarea desc, lscode asc";
        
        //public string sqllov_storagecount = @"SELECT 1 AS seq, lscode ,lscodealt,spcarea,fltype from wm_locup where fltype = 'ZN' and spcarea = 'ST' and tflow = 'IO' and lower(orgcode) = @orgcode and site = @site and depot = @depot
        //AND lscodealt NOT in('RTV','TM') UNION ALL SELECT 1 AS seq, lscode ,lscodealt,spcarea,fltype from wm_locup wl where fltype = 'BL' and tflow = 'IO' and lower(orgcode) = @orgcode and site = @site and depot = @depot
        //AND exists (select 1 FROM wm_binary wb WHERE wb.orgcode=wl.orgcode and wb.site = wl.site and wb.depot = wl.depot AND wb.bntype='LOCATION' AND wb.bncode='AREA' and wb.bnvalue = wl.spcarea)
        //order BY seq,spcarea desc, lscode asc";

        //Staging Inbound 
        public string sqllov_staginginb = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'RS' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";
        //Staging Outbound 
        public string sqllov_stagingoub = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'DS' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";
        //Bulk
        public string sqllov_bulk = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'BL' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";

        //Damage
        public string sqllov_damage = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'DM' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";

        //Sinbin
        public string sqllov_sinbin = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'SB' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";

        //Exchange 
        public string sqllov_exchange = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'EX' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";

        //Overflow 
        public string sqllov_overflow = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'OV' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";

        //Return 
        public string sqllov_return = "select lscode ,lscodealt from wm_locup where fltype = 'BL' and spcarea = 'RN' and tflow = 'IO' and orgcode = @orgcode and site = @site and depot = @depot order by lscode";

        //Validate location 
        //Aisle
        public string sqlvalloc_aisle = " select count(1) rsl from wm_locup where lsaisle = @loc and orgcode = @orgcode and site = @site and depot = @depot ";
        //Bay 
        public string sqlvalloc_bay = " select count(1) rsl from wm_locup where lsbay = @loc and orgcode = @orgcode and site = @site and depot = @depot ";
        //Level
        public string sqlvalloc_level = " select count(1) rsl from wm_locup where lslevel = @loc and orgcode = @orgcode and site = @site and depot = @depot ";
        //Location
        public string sqlvalloc_location = " select count(1) rsl from wm_locdw where lscode = @loc and orgcode = @orgcode and site = @site and depot = @depot ";

        public binary_ops(){ }
        public binary_ops(string cx){ cn = new SqlConnection(cx); }
        public binary_ops(SqlConnection ocn) { cn = ocn; }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }

            this.sqlbinary_desc = null;
            this.sqlbinary_find = null;
            this.sqlbinary_validate = null;
            this.sqlbinary_insert = null;
            this.sqlbinary_update = null;            
            this.sqlbinary_remove = null;

            this.sqllov_warehouse = null;
            this.sqllov_depot = null;
            this.sqllov_role = null;
            this.sqllov_find = null;
            this.sqllov_prepzonedist = null;
            this.sqllov_prepzonestock = null;
            this.sqllov_storagezone = null;
            this.sqllov_hutype = null;

            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}