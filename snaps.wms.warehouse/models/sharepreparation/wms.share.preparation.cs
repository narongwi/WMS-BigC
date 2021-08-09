using System;
using System.Collections.Generic;

namespace Snaps.WMS { 

    public class shareprep_md { 
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string spcarea               { get; set; }
        public string shprep                { get; set; }
        public string shprepname            { get; set; }
        public string shprepdesc            { get; set; }
        public string tflow                 { get; set; }
        public DateTimeOffset? datecreate   { get; set; }
        public string accncreate            { get; set; }
        public DateTimeOffset? datemodify   { get; set; }
        public string accnmodify            { get; set; }
        public string procmodify            { get; set; }
        public Int32 isfullfill             { get; set; }
        public List<shareprln_md> lines     { get; set; }
    }

    public class shareprln_md { 
        public string orgcode               { get; set; }
        public string site                  { get; set; }
        public string depot                 { get; set; }
        public string shprep                { get; set; }
        public string thcode                { get; set; }
        public Int32  priority              { get; set; }
        public string tflow                 { get; set; }
        public DateTimeOffset? datecreate   { get; set; }
        public string accncreate            { get; set; }
        public DateTimeOffset? datemodify   { get; set; }        
        public string accnmodify            { get; set; }
        public string thname                { get; set; }
    }

}