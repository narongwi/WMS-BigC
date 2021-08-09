
using System;
using System.Collections.Generic;

namespace Snaps.WMS {    
    public class policy_md { 
        public string orgcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string apcode    { get; set; }
        public string plccode    { get; set; }
        public string plcname    { get; set; }
        public string tflow    { get; set; }
        public Int32 reqnumeric    { get; set; }
        public Int32 requppercase    { get; set; }
        public Int32 reqlowercase    { get; set; }
        public Int32 reqspecialchar    { get; set; }
        public string spcchar    { get; set; }
        public Int32 minlength    { get; set; }
        public Int32 maxauthfail    { get; set; }
        public Int32 exppdamobile    { get; set; }
        public Int32 expandriod    { get; set; }
        public Int32 expios    { get; set; }
        public string seckey    { get; set; }
        public Int32 dayexpire    { get; set; }
        public string hashplc    { get; set; }
        public DateTimeOffset? datestart    { get; set; }
        public DateTimeOffset? dateend      { get; set; }
        public DateTimeOffset? datecreate   { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
    }
}