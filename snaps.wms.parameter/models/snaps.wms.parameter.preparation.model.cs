using System;
using System.Data.Common;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 
    public class pam_prepstock { 
        
        public bool allowincludestaging { get; set; }
        public bool allowpartialprocess { get; set; }
        public bool allowstocklessthenorder { get; set; }
        public bool allowconsolidateorder { get; set; }

        public pam_prepstock(){ } 
        public pam_prepstock(List<pam_set> o) { 
            try { 
                this.allowincludestaging = o.Find(x=>x.pmcode == "allowincludestaging").pmvalue;
                this.allowpartialprocess = o.Find(x=>x.pmcode == "allowpartialprocess").pmvalue;
                this.allowstocklessthenorder = o.Find(x=>x.pmcode == "allowstocklessthenorder").pmvalue;
                this.allowconsolidateorder = o.Find(x=>x.pmcode == "allowconsolidateorder").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }

    public class pam_prepstock_mobile { 
        
        public bool mobiledigitIOloc { get; set; }
        public bool mobilecheckdigit { get; set; }
        public bool mobilescanbarcode { get; set; }
        public bool mobilefullypick { get; set; }
        public bool mobilerepickforshortage { get; set; }

        public pam_prepstock_mobile(){ } 
        public pam_prepstock_mobile(List<pam_set> o) { 
            try { 
                this.mobiledigitIOloc = o.Find(x=>x.pmcode == "mobiledigitIOloc").pmvalue;
                this.mobilecheckdigit = o.Find(x=>x.pmcode == "mobilecheckdigit").pmvalue;
                this.mobilescanbarcode = o.Find(x=>x.pmcode == "mobilescanbarcode").pmvalue;
                this.mobilefullypick = o.Find(x=>x.pmcode == "mobilefullypick").pmvalue;
                this.mobilerepickforshortage = o.Find(x=>x.pmcode == "mobilerepickforshortage").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }

    public class pam_prepdist {

        public bool allowincludestaging { get; set; }
        public bool allowpartialprocess { get; set ;}
        public bool allowstocklessthanorder { get; set; }
        public bool allowprocessbyselectline { get; set; }

        public pam_prepdist(){ } 
        public pam_prepdist(List<pam_set> o) { 
            try { 
                this.allowincludestaging = o.Find(x=>x.pmcode == "allowincludestaging").pmvalue;
                this.allowpartialprocess = o.Find(x=>x.pmcode == "allowpartialprocess").pmvalue;
                this.allowstocklessthanorder = o.Find(x=>x.pmcode == "allowstocklessthanorder").pmvalue;
                this.allowprocessbyselectline = o.Find(x=>x.pmcode == "allowprocessbyselectline").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }

    public class pam_prepdist_mobile { 
        
        public bool mobiledigitIOlc { get; set; }
        public bool mobilecheckdigit { get; set; }
        public bool mobilescanbarcode { get; set; }
        public bool mobilefullypick { get; set; }

        public pam_prepdist_mobile(){ } 
        public pam_prepdist_mobile(List<pam_set> o) { 
            try {                 
                this.mobiledigitIOlc = o.Find(x=>x.pmcode == "mobiledigitIOlc").pmvalue;
                this.mobilecheckdigit = o.Find(x=>x.pmcode == "mobilecheckdigit").pmvalue;
                this.mobilescanbarcode = o.Find(x=>x.pmcode == "mobilescanbarcode").pmvalue;
                this.mobilefullypick = o.Find(x=>x.pmcode == "mobilefullypick").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }

    public class pam_preparation { 

        public Boolean allowcancel { get; set; }
        public Boolean allowautoassign { get; set; }

        public pam_preparation(){ } 
        public pam_preparation(List<pam_set> o) { 
            try {                 
                this.allowcancel = o.Find(x=>x.pmcode == "allowcancel").pmvalue;
                this.allowautoassign = o.Find(x=>x.pmcode == "allowautoassign").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }
}