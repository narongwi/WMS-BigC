using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 
    public class pam_outbound { 

        public bool allowchangeofexsource { get; set; }
        public bool allowchangespcdlc { get; set; }
        public bool allowchangereqdate { get; set; }
        public bool allowchangespcbatch { get; set; }
        public bool allowcancel { get; set; }

        public pam_outbound(){ } 
        public pam_outbound(List<pam_set> o) { 
            try { 
                this.allowchangeofexsource = o.Find(x=>x.pmcode == "allowchangeofexsource").pmvalue;
                this.allowchangespcdlc = o.Find(x=>x.pmcode == "allowchangespcdlc").pmvalue;
                this.allowchangereqdate = o.Find(x=>x.pmcode == "allowchangereqdate").pmvalue;
                this.allowchangespcbatch = o.Find(x=>x.pmcode == "allowchangespcbatch").pmvalue;
                this.allowcancel = o.Find(x=>x.pmcode == "allowcancel").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }

    public class pam_allocate {

        public bool allowautoallocate { get; set; }

        public pam_allocate(){ } 
        public pam_allocate(List<pam_set> o) { 
            try { 
                this.allowautoallocate = o.Find(x=>x.pmcode == "allowautoallocate").pmvalue;
            }catch (Exception ex) { throw ex; }
        }
    }

    public class pam_shipment { 

        public bool allowrevisequantity { get; set; } 
        public pam_shipment(){ } 
        public pam_shipment(List<pam_set> o) { 
            try { 
                this.allowrevisequantity = o.Find(x=>x.pmcode == "allowrevisequantity").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }
}