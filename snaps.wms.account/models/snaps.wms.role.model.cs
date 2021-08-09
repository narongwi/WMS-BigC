using System;
using System.Collections.Generic;

namespace Snaps.WMS {    

    public class role_md { 
        public string orgcode    { get; set; }
        public string apcode    { get; set; }
        public string site    { get; set; }
        public string depot    { get; set; }
        public string rolecode    { get; set; }
        public string rolename    { get; set; }
        public string roledesc    { get; set; }
        public string tflow    { get; set; }
        public string hashrol    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
        public string roljson    { get; set; }
        public List<accn_roleacs> roleaccs { get; set; }
        public List<roln_md> lines { get; set; }

    }
    public class roln_md {
        public string objmodule    { get; set; }
        public string objtype    { get; set; }
        public string objcode    { get; set; }
        public Int32 isenable    { get; set; }
        public Int32 isexecute    { get; set; }
        public string hashrln    { get; set; }
        public DateTimeOffset? datecreate    { get; set; }
        public string accncreate    { get; set; }
        public DateTimeOffset? datemodify    { get; set; }
        public string accnmodify    { get; set; }
        public string procmodify    { get; set; }
    }

}