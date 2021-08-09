using System;
using System.Collections.Generic;
using Snaps.WMS;

namespace Snaps.WMS {
    public class accn_acs { 
        public String accscode { get; set;} 
        public DateTimeOffset accsexpire { get; set; }
        public accn_acs(String code, DateTimeOffset expire){ this.accscode = code; this.accsexpire = expire; }
    }
    public class accn_ls {
        public String orgcode { get; set; } 
        public String accntype { get; set; } 
        public String accncode { get; set; } 
        public String accnname { get; set; } 
        public String accnsurname { get; set; }
        public String email { get; set; } 
        public DateTimeOffset? dateexpire { get; set; } 
        public DateTimeOffset? sessionexpire { get; set;}
        public String lang { get; set;}
        public String tflow { get; set; } 
        public String accnavartar { get; set; } 
        
        public String formatdateshort { get; set; }
        public String formatdatelong { get; set; }
        public String formatdate { get; set; }
    }
    public class accn_pm : accn_ls { 
        public String accnapline { get; set; } 
        public string site { get; set; }
        public string depot { get; set; }
    }
    public class accn_ix : accn_ls { 
    }
    public class accn_md : accn_ls  {
        public String mobileno { get; set; } 
        public String accnapline { get; set; } 
        public String tkrqpriv { get; set; } 
        public Int32 cntfailure { get; set; } 
        public DateTimeOffset? datelogin { get; set; } 
        public DateTimeOffset? datelogout { get; set; } 
        public DateTimeOffset? datechnpriv { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public string accsrole { get; set; }
        public string accscode { get; set; }
        public string opriv { get; set; }
        public string npriv { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public List<accn_cfg> accncfg { get; set; }
    }
    public class accn_cfg {
        public accn_cfg() {
        }

        public accn_cfg(object orgcode,object site,object depot,object accncode,object rolecode,object rolename,object accncreate,object datecreate) {
            this.orgcode = orgcode.ToString();
            this.site = site.ToString();
            this.depot = depot.ToString();
            this.accncode = accncode.ToString();
            this.rolecode = rolecode.ToString();
            this.rolename = rolename.ToString();
            this.accncreate = accncreate.ToString();
            this.datecreate = Convert.ToDateTime(datecreate.ToString());
        }

        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string accncode { get; set; }
        public string rolecode { get; set; }
        public string rolename { get; set; }
        public string accncreate { get; set; }
        public DateTimeOffset datecreate { get; set; }
    }

    public class accn_signup { 
        public String accncode { get; set; } 
        public String accnname { get; set; }
        public String accnsurname { get; set;} 
        public String email { get; set; } 
        public String password { get; set; }
        public String lang { get; set; }
        public String accstoken { get; set; }
    }
    public class accn_prc { 
        public String accncode { get; set; }
        public String lang { get; set;}
        public String unitdimension { get; set; } 
        public String unitweight { get; set; }
        public String unitvolumd { get; set; } 
        public String orgcode { get; set;} 
        public String sitecode { get; set;} 
        public String depot { get; set;} 
        public Int32 pagestart { get; set; } 
        public Int32 pagelimit { get; set; }
    }

    public class accn_ppm : accn_prc { 
        public accn_pm objops { get; set;}
    }
    
    public class accn_pom : accn_prc { 
        public accn_md objops { get; set; } 
    }
    public class accn_authen {
        public String valcode { get; set;}
        public accn_ls account { get; set;}
    }
    public class accn_profile { 
        public String orgcode { get; set; } 
        public String accntype { get; set; } 
        public String accncode { get; set; } 
        public String accnname { get; set; } 
        public String email { get; set; } 
        public DateTimeOffset? dateexpire { get; set; } 
        public String accnavartar { get; set; } 
        public String accnapline { get; set; } 
        public String mobileno { get; set; } 
        public DateTime? datelogin { get; set; } 
        public DateTime? datelogout { get; set; } 
        public DateTime? datechnpriv { get; set; } 
        public List<accn_roleacs> roleaccess { get; set; }
        public String formatdateshort { get; set; }
        public String formatdatelong { get; set; }
        public String formatdate { get; set; }
    }

    public class accn_roleacs { 
        public string site { get; set; }  
        public string depot { get; set; }
        public string rolecode { get; set; }
        public string rolename { get; set; } 
        public List<module> modules { get; set; }
        public string roletype { get; set; }
    }
    public class module { 
        public String modulecode { get; set;} 
        public String modulename { get; set;} 
        public String moduleicon { get; set; }
        public List<permission> permission { get; set; }
    }
    public class permission { 

        public String orgcode { get; set; } 
        public String apcode { get; set; } 
        public String rolecode { get; set; } 
        public String objmodule { get; set; } 
        public String objtype { get; set; } 
        public String objcode { get; set; } 
        public String objname { get; set; } 
        public Int32 objseq { get; set; } 
        public String objroute { get; set; } 
        public String objicon { get; set; }
        public Int32 isenable { get; set;} 
        public Int32 isexecute { get; set;} 

    } 


    public class accn_priv { 
        public string orgcode   { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string accncode  { get; set; }
        public string oldpriv   { get; set; }
        public string newpriv   { get; set; }
        public Int32  lifetime  { get; set; }
        
    }

}