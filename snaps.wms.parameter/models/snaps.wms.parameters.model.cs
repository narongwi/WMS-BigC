using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
namespace Snaps.WMS { 

    public class pam_set { 
        public String pmmodule { get; set;}
        public String pmtype { get; set; }
        public String pmcode { get; set; }
        public bool pmvalue { get; set; }
        public String pmstate { get; set; }
         public String pmoption { get; set; }
        public pam_set(){} 
        public pam_set(ref SqlDataReader r) { 
            this.pmmodule = r["pmmodule"].ToString();
            this.pmtype = r["pmtype"].ToString();
            this.pmcode = r["pmcode"].ToString();
            this.pmvalue = r["pmvalue"].ToString().CBoolean();
            this.pmstate = r["pmstate"].ToString();
            this.pmoption = r["pmoption"].ToString();
        }

        public pam_set(pam_parameter o){ 
            this.pmmodule = o.pmmodule;
            this.pmtype = o.pmtype;
            this.pmcode = o.pmcode;
            this.pmvalue = o.pmvalue;
            this.pmstate = o.pmstate;
        }
    }
    
    public class pam_list : pam_set { 
        public String pmdesc { get; set; }
        public String pmdescalt { get; set; } 
        public DateTimeOffset? datemodify { get; set; }
        public String accnmodify { get; set; }
       
        public pam_list(){} 
        public pam_list(ref SqlDataReader r) { 
            this.pmmodule = r["pmmodule"].ToString();
            this.pmtype = r["pmtype"].ToString();
            this.pmcode = r["pmcode"].ToString();
            this.pmvalue = r["pmvalue"].ToString().CBoolean();
            this.pmstate = r["pmstate"].ToString();
            this.pmdesc = r["pmdesc"].ToString();
            this.pmdescalt = r["pmdescalt"].ToString();            
            this.accnmodify = r["accnmodify"].ToString();            
            this.datemodify = r.GetDateTimeOffset(13);
            this.pmoption = r["pmoption"].ToString();
        }
    }

    public class pam_parameter : pam_list { 
        public String orgcode { get; set; }
        public String site { get; set ;}
        public String depot { get; set ;}
        public String apps { get; set; }
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public pam_parameter(){}
        public pam_parameter(ref SqlDataReader r) { 
            this.pmmodule = r["pmmodule"].ToString();
            this.pmtype = r["pmtype"].ToString();
            this.pmcode = r["pmcode"].ToString();
            this.pmvalue = r["pmvalue"].ToString().CBoolean();
            this.pmstate = r["pmstate"].ToString();
            this.pmdesc = r["pmdesc"].ToString();
            this.pmdescalt = r["pmdescalt"].ToString();            
            this.accnmodify = r["accnmodify"].ToString();
            this.datemodify = r.GetDateTimeOffset(13);
            this.orgcode = r["orgcode"].ToString();
            this.site = r["site"].ToString();
            this.depot = r["depot"].ToString();
            this.apps = r["apps"].ToString();
             this.pmoption = r["pmoption"].ToString();
        }
        
        public pam_set getParameterSet(){  return new pam_set(this); }
    }
    
}