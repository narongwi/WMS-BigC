using System;
using System.Data.Common;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 
    public class pam_correction { 
        public bool allowblankremarks { get; set; }
        public bool allowblankrefereceno { get; set; }
        public bool allowchangeunit { get; set; }
        public bool allowprintlabelonreserve { get; set; }
        public bool allowgentaskfornewhu { get; set; }
        public bool allowincludehubelongingtask { get; set; }

        public pam_correction(){ } 
        public pam_correction(List<pam_set> o) { 
            try { 
                this.allowblankremarks = o.Find(x=>x.pmcode == "allowblankremarks").pmvalue;
                this.allowblankrefereceno = o.Find(x=>x.pmcode == "allowblankrefereceno").pmvalue;
                this.allowchangeunit = o.Find(x=>x.pmcode == "allowchangeunit").pmvalue;
                this.allowprintlabelonreserve = o.Find(x=>x.pmcode == "allowprintlabelonreserve").pmvalue;
                this.allowgentaskfornewhu = o.Find(x=>x.pmcode == "allowgentaskfornewhu").pmvalue;
                this.allowincludehubelongingtask = o.Find(x=>x.pmcode == "allowincludehubelongingtask").pmvalue;
            }catch (Exception ex) { throw ex; }
        }
 
    }
}