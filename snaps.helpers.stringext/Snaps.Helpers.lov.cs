using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace Snaps.Helpers.StringExt {
    public class lov { 
        public String desc { get; set; }
        public String value { get; set; } 
        public String valopnfirst { get; set; }
        public String valopnsecond { get; set;}
        public String valopnthird { get; set; }
        public String valopnfour { get; set; } 
        public String icon { get; set;}
        public lov(String val, String dsv) { this.desc = dsv; this.value = val; }
        public lov(String val, String dsv, String opnfn) { 
            this.desc = dsv; this.value = val; this.valopnfirst = opnfn; 
        }
        public lov(String val, String dsv, String opnfn, String opnsc) { 
            this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc;
        }
        public lov(String val, String dsv, String opnfn, String opnsc,String icn) { 
            this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc; this.icon = icn;
        }
        public lov(String val, String dsv, String opnfn, String opnsc,String opnth, String icn) { 
            this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc; this.icon = icn; this.valopnthird = opnth;
        }
        public lov(String val, String dsv, String opnfn, String opnsc,String opnth, String opnfr, String icn) { 
            this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc; this.icon = icn; this.valopnthird = opnth; this.valopnfour = opnfr;
        }

    }

    
}