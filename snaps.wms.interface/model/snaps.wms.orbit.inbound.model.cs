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

    public class orbit_inbound  {
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public string thcode                { get; set; }
        public string intype                { get; set; }
        public string subtype               { get; set; }
        public string inorder               { get; set; }
        public DateTimeOffset? dateorder    { get; set; }
        public DateTimeOffset? dateplan     { get; set; }
        public DateTimeOffset? dateexpire   { get; set; }
        public Int32 inpriority             { get; set; }
        public string inflag                { get; set; }
        public string inpromo               { get; set; }
        public string tflow                 { get; set; }
        public string orbitsource           { get; set; }
        public string fileid                { get; set; }
        public string rowops                { get; set; }
        public string ermsg                 { get; set; }
        public DateTimeOffset? dateops      { get; set; }
    }

    public class orbit_inbouln { 
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public string inorder               { get; set; }
        public string inln                   { get; set; }
        public string inrefno               { get; set; }
        public Int32 inrefln                { get; set; }
        public string article               { get; set; }
        public Int32 pv                     { get; set; }
        public Int32 lv                     { get; set; }
        public string unitops               { get; set; }
        public Int32 qtysku                 { get; set; }
        public Int32 qtypu                  { get; set; }
        public Decimal qtyweight            { get; set; }
        public string batchno               { get; set; }
        public string lotno                 { get; set; }
        public DateTimeOffset? expdate      { get; set; }
        public string serialno              { get; set; }
        public string tflow                 { get; set; }
        public Int32 qtyhurec               { get; set; }
        public string fileid                { get; set; }
        public string rowops                { get; set; }
        public string ermsg                 { get; set; }
        public DateTimeOffset? dateops      { get; set; }
        public string orbitsource           { get; set; }
    }


    public class orbit_receipt { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string thcode    { get; set; }
        public string intype    { get; set; }
        public string insubtype    { get; set; }
        public string inorder    { get; set; }
        public string inln    { get; set; }
        public string ingrno    { get; set; }
        public string inrefno    { get; set; }
        public string inrefln    { get; set; }
        public string barcode    { get; set; }
        public string article    { get; set; }
        public Int32 pv    { get; set; }
        public Int32 lv    { get; set; }
        public string unitops    { get; set; }
        public Int32 qtysku    { get; set; }
        public Int32 qtypu    { get; set; }
        public Decimal qtyweight    { get; set; }
        public Decimal qtyvolume    { get; set; }
        public Decimal qtynaturalloss    { get; set; }
        public DateTimeOffset? dateops    { get; set; }
        public string accnops    { get; set; }
        public DateTimeOffset? dateexp    { get; set; }
        public DateTimeOffset? datemfg    { get; set; }
        public string batchno    { get; set; }
        public string lotmfg    { get; set; }
        public string serialno    { get; set; }
        public string huno    { get; set; }
        public string inpromo    { get; set; }
        public string orbitsource    { get; set; }
        public string xaction    { get; set; }
        public DateTimeOffset? xcreate    { get; set; }
        public DateTimeOffset? xmodify    { get; set; }
        public string xmsg    { get; set; }
        public Int32 rowid    { get; set; }

        public string orbitsite  { get; set; }
        public string orbitdepot { get; set; }
        public string inagrn    { get; set; }
        }

}