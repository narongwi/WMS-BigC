using System;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 
    public class pam_thirdparty {
        public bool allowchangeofexsource { get; set; }
        public bool allowchangeplandate { get; set; }

        public pam_thirdparty(){ } 
        public pam_thirdparty(List<pam_set> o) { 
            try { 
                this.allowchangeofexsource = o.Find(x=>x.pmcode == "allowchangeofexsource").pmvalue;
                this.allowchangeplandate = o.Find(x=>x.pmcode == "allowchangeplandate").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }
}