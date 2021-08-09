using System;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 

    public class pam_taskputaway {
        public bool allowscanhuongrap {get; set;}
        public bool allowscansourcelocation { get; set; }
        public bool allowinputqtyongrap { get; set; }
        public bool allowfullygrap { get; set; }
        public bool allowpickndrop { get; set; }
        public bool allowcheckdigit { get; set; }
        public bool allowautoassign { get; set; }

        public pam_taskputaway(){ } 
        public pam_taskputaway(List<pam_set> o) { 
            try { 
                this.allowscanhuongrap = o.Find(x=>x.pmcode == "allowscanhuongrap").pmvalue;
                this.allowscansourcelocation = o.Find(x=>x.pmcode == "allowscansourcelocation").pmvalue;
                this.allowinputqtyongrap = o.Find(x=>x.pmcode == "allowinputqtyongrap").pmvalue;
                this.allowfullygrap = o.Find(x=>x.pmcode == "allowfullygrap").pmvalue;
                this.allowpickndrop = o.Find(x=>x.pmcode == "allowpickndrop").pmvalue;             
                this.allowpickndrop = o.Find(x=>x.pmcode == "allowpickndrop").pmvalue;
                this.allowcheckdigit = o.Find(x=>x.pmcode == "allowcheckdigit").pmvalue;
                this.allowautoassign = o.Find(x=>x.pmcode == "allowautoassign").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }

    public class pam_taskapproach { 
        public bool allowchangeworker { get; set; }
        public bool allowscanhuno { get; set; }
        public bool allowscansourcelocation { get; set; }
        public bool allowscanbarcode { get; set; }
        public bool allowchangequantity { get; set; }
        public bool allowpickndrop { get; set; }
        public bool allowcheckdigit { get; set; }
        public bool allowautoassign { get; set; }

        public pam_taskapproach(){ } 
        public pam_taskapproach(List<pam_set> o) { 
            try { 
                this.allowchangeworker = o.Find(x=>x.pmcode == "allowchangeworker").pmvalue;
                this.allowscanhuno = o.Find(x=>x.pmcode == "allowscanhuno").pmvalue;
                this.allowscansourcelocation = o.Find(x=>x.pmcode == "allowscansourcelocation").pmvalue;
                this.allowscanbarcode = o.Find(x=>x.pmcode == "allowscanbarcode").pmvalue;
                this.allowchangequantity = o.Find(x=>x.pmcode == "allowchangequantity").pmvalue;        
                this.allowpickndrop = o.Find(x=>x.pmcode == "allowpickndrop").pmvalue;
                this.allowcheckdigit = o.Find(x=>x.pmcode == "allowcheckdigit").pmvalue;
                this.allowautoassign = o.Find(x=>x.pmcode == "allowautoassign").pmvalue;
            }catch (Exception ex) { throw ex; }
        }
    }

    public class pam_taskreplenishment { 
        public bool allowmanual { get; set; }
        public bool allowchangeworker { get; set; }
        public bool allowscanhuno { get; set; }
        public bool allowscanbarcode { get; set; }
        public bool allowchangequantity { get; set; }
        public bool allowpickndrop { get; set; }
        public bool allowscansourcelocation { get; set; }
        public bool allowcheckdigit { get; set; }
        public bool allowautoassign { get; set; }

        public pam_taskreplenishment(){ } 
        public pam_taskreplenishment(List<pam_set> o) { 
            try { 
                this.allowmanual = o.Find(x=>x.pmcode == "allowmanual").pmvalue;
                this.allowchangeworker = o.Find(x=>x.pmcode == "allowchangeworker").pmvalue;
                this.allowscanhuno = o.Find(x=>x.pmcode == "allowscanhuno").pmvalue;
                this.allowscanbarcode = o.Find(x=>x.pmcode == "allowscanbarcode").pmvalue;
                this.allowchangequantity = o.Find(x=>x.pmcode == "allowchangequantity").pmvalue;            
                this.allowpickndrop = o.Find(x=>x.pmcode == "allowpickndrop").pmvalue;
                this.allowcheckdigit = o.Find(x=>x.pmcode == "allowcheckdigit").pmvalue;
                this.allowautoassign = o.Find(x=>x.pmcode == "allowautoassign").pmvalue;
                this.allowscansourcelocation = o.Find(x=>x.pmcode == "allowscansourcelocation").pmvalue;
            }catch (Exception ex) { throw ex; }
        }
    }
}