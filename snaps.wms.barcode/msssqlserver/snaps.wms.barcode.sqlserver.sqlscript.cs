using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.Hash;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS.Barcode {

    public partial class barcode_ops : IDisposable { 
        private static string tbn = "wm_barcode";
        private static string sqlmcom = " and orgcode = @orgcode and site = @site and depot = @depot and barcode = @barcode " ;
        private String sqlins = "insert into " + tbn + 
        " ( site, depot, spcarea, article, pv, lv, barops, barcode, bartype, thcode, tflow, datecreate, accncreate, datemodify, accnmodify, procmodify, barremarks ) " + 
        " values "  +
        " ( @site, @depot, @spcarea, @article, @pv, @lv, @barops, @barcode, @bartype, @thcode, @tflow, @sysdate, @accncreate, @sysdate, @accnmodify, @procmodify, @barremarks ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " article = @article, pv = @pv, lv = @lv, barops = @barops,  bartype = @bartype, thcode = @thcode, tflow = @tflow,   "+ 
        " datemodify = @sysdate, accnmodify = @accnmodify, procmodify = @procmodify , barremarks = @barremarks " + 
        " where 1=1 " + sqlmcom;
        private String sqlinx = "insert into ix" +tbn +  
        " ( site, depot, spcarea, article, pv, lv, barops, barcode, bartype, thcode, tflow, fileid, rowops, ermsg, dateops, ) " + 
        " values " + 
        " ( @site, @depot, @spcarea, @article, @pv, @lv, @barops, @barcode, @bartype, @thcode, @tflow, @fileid, @rowops, @ermsg, @dateops ) ";     
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  

        public string sqlbarcode_find = " select b.*, p.descalt, t.thnamealt from wm_barcode b left join wm_product p " + 
        " on b.orgcode = p.orgcode and b.site = p.site and b.depot = b.depot and p.article = b.article and p.lv = b.lv " + 
        " left join wm_thparty t on b.orgcode = t.orgcode and b.site = t.site and b.depot = t.depot and b.thcode = t.thcode " + 
        " where b.orgcode = @orgcode and b.site = @site and b.depot = @depot ";
        public string sqlbarcode_clearprimary = @"update wm_barcode set datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = @procmodify, isprimary = 0
        where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv";

        public string sqlbarcode_setprimary = @"update wm_barcode set datemodify = sysdatetimeoffset(), accnmodify = @accnmodify, procmodify = @procmodify, isprimary = 1
        where orgcode = @orgcode and site = @site and depot = @depot and barcode = @barcode";
    }
}