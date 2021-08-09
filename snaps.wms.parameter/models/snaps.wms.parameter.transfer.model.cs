using System;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 
    public class pam_transfer { 

        public bool allowchangeunit { get; set; }
        public bool allowgenreservetoreserve { get; set; }
        public bool allowgenreservetopicking { get; set; }
        public bool allowgenreservetobulk { get; set; }

        public bool allowgenbulktoreserve { get; set; }
        public bool allowgenbulktopicking { get; set; }
        public bool allowgenbulktobulk { get; set; }

        public bool allowgenpickingtoreserve { get; set; }
        public bool allowgenpickingtopicking { get; set; }
        public bool allowgenpickingtobulk { get; set; }

        public pam_transfer(){ } 
        public pam_transfer(List<pam_set> o) { 
            try { 
                this.allowchangeunit = o.Find(x=>x.pmcode == "allowchangeunit").pmvalue;
                this.allowgenreservetoreserve = o.Find(x=>x.pmcode == "allowgenreservetoreserve").pmvalue;
                this.allowgenreservetopicking = o.Find(x=>x.pmcode == "allowgenreservetopicking").pmvalue;
                this.allowgenreservetobulk = o.Find(x=>x.pmcode == "allowgenreservetobulk").pmvalue;
                this.allowgenbulktoreserve = o.Find(x=>x.pmcode == "allowgenbulktoreserve").pmvalue;
                this.allowgenbulktopicking = o.Find(x=>x.pmcode == "allowgenbulktopicking").pmvalue;
                this.allowgenbulktobulk = o.Find(x=>x.pmcode == "allowgenbulktobulk").pmvalue;
                this.allowgenpickingtoreserve = o.Find(x=>x.pmcode == "allowgenpickingtoreserve").pmvalue;
                this.allowgenpickingtopicking = o.Find(x=>x.pmcode == "allowgenpickingtopicking").pmvalue;
                this.allowgenpickingtobulk = o.Find(x=>x.pmcode == "allowgenpickingtobulk").pmvalue;
            }catch (Exception ex) { throw ex; }
        }

    }
}