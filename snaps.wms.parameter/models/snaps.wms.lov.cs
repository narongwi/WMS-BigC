using System;
using System.Data.Common;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
namespace Snaps.WMS { 
        public class lov { 
            public String desc { get; set; }
            public String value { get; set; } 
            public String valopnfirst { get; set; }
            public String valopnsecond { get; set;}
            public String valopnthird { get; set; }
            public String valopnfour { get; set; } 
            public String icon { get; set;}
            public lov(){}
            public lov(String val, String dsv) { this.desc = dsv; this.value = val; }
            public lov(String val, String dsv, String opnfn, String icn) { 
                this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.icon = icn;
            }
            public lov(String val, String dsv, String opnfn, String opnsc, String icn) { 
                this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc; this.icon = icn;
            }
            public lov(String val, String dsv, String opnfn, String opnsc,String opnth, String icn) { 
                this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc; this.valopnthird = opnth; this.icon = icn;
            }

            public lov(String val, String dsv, String opnfn, String opnsc,String opnth, String opnfou, String icn) { 
                this.desc = dsv; this.value = val; this.valopnfirst = opnfn; this.valopnsecond = opnsc; this.valopnthird = opnth; this.valopnfour = opnfou; this.icon = icn;
            }
        }
}
