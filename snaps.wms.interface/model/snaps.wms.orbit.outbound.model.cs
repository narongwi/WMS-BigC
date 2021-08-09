using System;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS {

    public class orbit_outbound {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string ouorder { get; set; }
        public string outype { get; set; }
        public string ousubtype { get; set; }
        public string thcode { get; set; }
        public DateTimeOffset? dateorder { get; set; }
        public DateTimeOffset? dateprep { get; set; }
        public DateTimeOffset? dateexpire { get; set; }
        public Int32 oupriority { get; set; }
        public string ouflag { get; set; }
        public string oupromo { get; set; }
        public string dropship { get; set; }
        public string orbitsource { get; set; }
        public string stocode { get; set; }
        public string stoname { get; set; }
        public string stoaddressln1 { get; set; }
        public string stoaddressln2 { get; set; }
        public string stoaddressln3 { get; set; }
        public string stosubdistict { get; set; }
        public string stodistrict { get; set; }
        public string stocity { get; set; }
        public string stocountry { get; set; }
        public string stopostcode { get; set; }
        public string stomobile { get; set; }
        public string stoemail { get; set; }
        public string tflow { get; set; }
        public string inorder { get; set; }
        public string fileid { get; set; }
        public string rowops { get; set; }
        public string ermsg { get; set; }
        public DateTimeOffset? dateops { get; set; }
    }

    public class orbit_outbouln {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string ouorder { get; set; }
        public string ouln { get; set; }
        public string ourefno { get; set; }
        public string ourefln { get; set; }
        public string inorder { get; set; }
        public string article { get; set; }
        public Int32 pv { get; set; }
        public Int32 lv { get; set; }
        public string unitops { get; set; }
        public Int32 qtysku { get; set; }
        public Int32 qtypu { get; set; }
        public Decimal qtyweight { get; set; }
        public string spcselect { get; set; }
        public string batchno { get; set; }
        public string lotno { get; set; }
        public DateTimeOffset? datemfg { get; set; }
        public DateTimeOffset? dateexp { get; set; }
        public string serialno { get; set; }
        public string tflow { get; set; }
        public string disthcode { get; set; }
        public string fileid { get; set; }
        public string rowops { get; set; }
        public string ermsg { get; set; }
        public DateTimeOffset? dateops { get; set; }
        public string orbitsource { get; set; }
        public int ouseq { get; set; }
    }

    public class orbit_outbound_seq {
        public orbit_outbound_seq() {
        }
        public orbit_outbound_seq(object site, object depot, object ouorder, int oudono) {
            this.site = site.ToString();
            this.depot = depot.ToString();
            this.ouorder = ouorder.ToString();
            this.oudono = oudono;
        }

        public string site { get; set; }
        public string depot { get; set; }
        public string ouorder { get; set; }
        public int oudono { get; set; }
    }

    public class orbit_delivery {
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public string routeops    { get; set; }
        public string transportno    { get; set; }
        public string thcode    { get; set; }
        public string thtype    { get; set; }
        public string dropship    { get; set; }
        public string ouorder    { get; set; }
        public Int32 ouline    { get; set; }
        public string ourefno    { get; set; }
        public Int32 ourefln    { get; set; }
        public string oudnno    { get; set; }
        public string inorder    { get; set; }
        public string ingrno    { get; set; }
        public string article    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string unitops    { get; set; }
        public Int32 qtysku    { get; set; }
        public Int32 qtypu    { get; set; }
        public Int32 qtyweight    { get; set; }
        public Int32 qtyvolume    { get; set; }
        public DateTimeOffset? dateexp    { get; set; }
        public DateTimeOffset? datemfg    { get; set; }
        public string lotmfg    { get; set; }
        public string serialno    { get; set; }
        public string huno    { get; set; }
        public string accnops    { get; set; }
        public string oupromo    { get; set; }
        public string xaction    { get; set; }
        public DateTimeOffset? xcreate    { get; set; }
        public DateTimeOffset? xmodify    { get; set; }
        public string xmsg    { get; set; }
        public Int32 rowid    { get; set; }
        public string orbitsite  { get; set; }
        public string orbitdepot { get; set; }
    }
}