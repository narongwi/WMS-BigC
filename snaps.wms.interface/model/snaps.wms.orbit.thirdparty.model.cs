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

    public class orbit_thirdparty  {
        public string orgcode           { get; set; }
        public string site              { get; set; }
        public string depot             { get; set; }
        public string spcarea           { get; set; }
        public string thtype            { get; set; }
        public string thbutype          { get; set; }
        public string thcode            { get; set; }
        public string thcodealt         { get; set; }
        public string vatcode           { get; set; }
        public string thname            { get; set; }
        public string thnamealt         { get; set; }
        public string thnameInt32       { get; set; }
        public string addressln1        { get; set; }
        public string addressln2        { get; set; }
        public string addressln3        { get; set; }
        public string subdistrict       { get; set; }
        public string district          { get; set; }
        public string city              { get; set; }
        public string country           { get; set; }
        public string postcode          { get; set; }
        public string region            { get; set; }
        public string telephone         { get; set; }
        public string email             { get; set; }
        public string thgroup           { get; set; }
        public string thcomment         { get; set; }
        public string throuteformat     { get; set; }
        public Int32 plandelivery       { get; set; }
        public Int32 naturalloss        { get; set; }
        public string mapaddress        { get; set; }
        public string orbitsource       { get; set; }
        public string tflow             { get; set; }
        public string fileid            { get; set; }
        public string rowops            { get; set; }
        public string ermsg             { get; set; }
        public DateTimeOffset? dateops  { get; set; }
    }
}