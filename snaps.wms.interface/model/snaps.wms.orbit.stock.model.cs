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

    public class orbit_stockblock  {
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public Int32 stockid                { get; set; }
        public string hutype                { get; set; }
        public string huno                  { get; set; }
        public string hunosource            { get; set; }
        public string thcode                { get; set; }
        public string inrefno               { get; set; }
        public Int32 inrefln                { get; set; }
        public string loccode               { get; set; }
        public string article               { get; set; }
        public Int32 pv                     { get; set; }
        public Int32 lv                     { get; set; }
        public Int32 qtysku                 { get; set; }
        public Int32 qtypu                  { get; set; }
        public Int32 qtyweight              { get; set; }
        public Int32 qtyvolume              { get; set; }
        public DateTimeOffset? daterec      { get; set; }
        public string batchno               { get; set; }
        public string lotno                 { get; set; }
        public DateTimeOffset? datemfg      { get; set; }
        public DateTimeOffset? dateexp      { get; set; }
        public string serialno              { get; set; }
        public string stkremarks            { get; set; }
        public string tflow                 { get; set; }
        public Int32 qtyprep                { get; set; }
        public string xaction               { get; set; }
        public DateTimeOffset? xcreate      { get; set; }
        public DateTimeOffset? xmodify      { get; set; }
        public string xmsg                  { get; set; }
    }
}