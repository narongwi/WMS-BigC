using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.warehouse {

    public partial class locup_ops : IDisposable { 
        public string sqllov_zone = " select lscode,lscodealt + ' : ' + bndescalt bndescalt,spcarea from wm_locup l,(select bnvalue,bndescalt " + 
        " from wm_binary where bntype = 'LOCATION' and bncode = 'SPCAREA' and bnstate = 'IO' and orgcode = @orgcode and site = @site and depot = @depot ) b " +
        " where fltype = 'ZN' and tflow IN ('IO','IX','XO') and orgcode = @orgcode and site = @site and depot = @depot and l.spcarea = b.bnvalue order by lscode " ;


        public string sqllov_aisle = " select distinct lscode,lscodealt + ' : ' + bndescalt bndescalt,spcarea from wm_locup l,(select bnvalue,bndescalt " + 
        " from wm_binary where bntype = 'LOCATION' and bncode = 'SPCAREA' and bnstate = 'IO' and orgcode = @orgcode and site = @site and depot = @depot ) b where fltype = 'AL' and tflow IN ('IO','IX','XO') " + 
        " and orgcode = @orgcode and site = @site and depot = @depot and l.lszone = @lszone " + 
        " and l.spcarea = b.bnvalue order by lscode " ;
        public string sqllov_bay = " select distinct lscode,lscodealt + ' : ' + bndescalt bndescalt,spcarea from wm_locup l,(select bnvalue,bndescalt " + 
        " from wm_binary where bntype = 'LOCATION' and bncode = 'SPCAREA' and bnstate = 'IO' and orgcode = @orgcode and site = @site and depot = @depot ) b where fltype = 'BA' and tflow IN ('IO','IX','XO') " + 
        " and orgcode = @orgcode and site = @site and depot = @depot and l.lszone = @lszone and l.lszone = @lszone " + 
        " and l.spcarea = b.bnvalue order by lscode " ;
        public string sqllov_level = " select distinct lscode,lscodealt + ' : ' + bndescalt bndescalt,spcarea from wm_locup l,(select bnvalue,bndescalt " + 
        " from wm_binary where bntype = 'LOCATION' and bncode = 'SPCAREA' and bnstate = 'IO' and orgcode = @orgcode and site = @site and depot = @depot) b where fltype = 'LV' and tflow IN ('IO','IX','XO') " + 
        " and orgcode = @orgcode and site = @site and depot = @depot and l.lszone = @lszone and l.lsaisle = @lsaisle and l.lsbay = @lsbay " + 
        " and l.spcarea = b.bnvalue order by lscode " ;
        public string sqllov_location =" select distinct lscode,lscodealt + ' : ' + bndescalt bndescalt,spcarea from wm_locdw l,(select bnvalue,bndescalt " + 
        " from wm_binary where bntype = 'LOCATION' and bncode = 'SPCAREA' and bnstate = 'IO'  and orgcode = @orgcode and site = @site and depot = @depot) b where fltype = 'LC' and tflow IN ('IO','IX','XO') " + 
        " and orgcode = @orgcode and site = @site and depot = @depot and l.lszone = @lszone and l.lsaisle = @lsaisle  and l.lsbay = @lsbay and l.lslevel = @lslevel  " + 
        " and l.spcarea = b.bnvalue order by lscode " ;


        private string sqlvalzonefree = @"select count(1) rsl from wm_locdw l where exists (select 1 from wm_stock s where l.orgcode = s.orgcode and l.site = s.site 
        and l.depot = s.depot and l.lscode = s.loccode) and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and lszone = @lscode ";
        private string sqlvalaislefree = @"select count(1) rsl from wm_locdw l where exists (select 1 from wm_stock s where l.orgcode = s.orgcode and l.site = s.site 
        and l.depot = s.depot and l.lscode = s.loccode) and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and lszone = @lszone and lsaisle = @lsaisle ";
        private string sqlvalbayfree =  @"select count(1) rsl from wm_locdw l where exists (select 1 from wm_stock s where l.orgcode = s.orgcode and l.site = s.site 
        and l.depot = s.depot and l.lscode = s.loccode) and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and lszone = @lszone and lsaisle = @lsaisle and lsbay = @lsbay ";
        private string sqlvallevelfree =@"select count(1) rsl from wm_locdw l where exists (select 1 from wm_stock s where l.orgcode = s.orgcode and l.site = s.site 
        and l.depot = s.depot and l.lscode = s.loccode) and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and lszone = @lszone and lsaisle = @lsaisle and lsbay = @lsbay and lslevel = @lslevel ";
        private string sqlvallocationfree = @"select count(1) rsl from wm_locdw l where exists (select 1 from wm_stock s where l.orgcode = s.orgcode and l.site = s.site 
        and l.depot = s.depot and l.lscode = s.loccode) and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and lszone = @lszone and lsaisle = @lsaisle and lsbay = @lsbay and lslevel = @lslevel and lscode = @lscode ";



    }
}