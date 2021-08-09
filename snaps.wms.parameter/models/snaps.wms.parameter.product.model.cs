using System;
using System.Data.Common;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 
    public class pam_product { 
        public bool allowchangeofexsource { get; set; }
        public bool allowchangehirachy { get; set; }
        public bool allowchangedimension { get; set; }
        public bool allowchangedlc { get; set; }
        public bool allowchangeunit { get; set; }

        public pam_product(){ } 
        public pam_product(List<pam_set> o) { 
            try { 
                this.allowchangeofexsource = o.Find(x=>x.pmcode == "allowchangeofexsource").pmvalue;
                this.allowchangehirachy = o.Find(x=>x.pmcode == "allowchangehirachy").pmvalue;
                this.allowchangedimension = o.Find(x=>x.pmcode == "allowchangedimension").pmvalue;
                this.allowchangedlc = o.Find(x=>x.pmcode == "allowchangedlc").pmvalue;
                this.allowchangeunit = o.Find(x=>x.pmcode == "allowchangeunit").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }
}