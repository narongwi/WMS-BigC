using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS { 

    public class pam_inbound {        
        public bool allowchangeofexsource { get; set; }
        public bool allowcalculatemfg { get; set; }
        public bool allowpartail { get; set; }
        public bool allowoverplan { get; set; }
        public bool allowexpired { get; set; } 
        public bool allowchangeunit { get; set; }
        public bool allowshowqtyorder { get; set; }
        public bool allowautostaging { get; set; }
        public bool allowcontrolcapacity { get; set ;}
        public bool allowchangepriority { get; set; }
        public bool allowcancel { get; set; }
        public bool allowgendistplan { get; set; }
        public bool allowgenstckputaway { get; set; }
        public bool allowreplandelivery { get; set; }

        public pam_inbound(){ } 
        public pam_inbound(List<pam_parameter> o) { 
            try { 
                //this.allowchangeofexsource = o.Find(x=>x.pmcode == "allowchangeofexsource").pmvalue;
                this.allowcalculatemfg = o.Find(x=>x.pmcode == "allowcalculatemfg").pmvalue;
                this.allowpartail = o.Find(x=>x.pmcode == "allowpartail").pmvalue;
                this.allowoverplan = o.Find(x=>x.pmcode == "allowoverplan").pmvalue;
                this.allowexpired = o.Find(x=>x.pmcode == "allowexpired").pmvalue;
                this.allowchangeunit = o.Find(x=>x.pmcode == "allowchangeunit").pmvalue;
                this.allowshowqtyorder = o.Find(x=>x.pmcode == "allowshowqtyorder").pmvalue;      
                this.allowautostaging = o.Find(x=>x.pmcode == "allowautostaging").pmvalue;
                this.allowcontrolcapacity = o.Find(x=>x.pmcode == "allowcontrolcapacity").pmvalue;
                this.allowchangepriority = o.Find(x=>x.pmcode == "allowchangepriority").pmvalue;
                this.allowcancel = o.Find(x=>x.pmcode == "allowcancel").pmvalue;
                this.allowgendistplan = o.Find(x=>x.pmcode == "allowgendistplan").pmvalue;
                this.allowgenstckputaway = o.Find(x=>x.pmcode == "allowgenstckputaway").pmvalue;
                this.allowreplandelivery = o.Find(x=>x.pmcode == "allowreplandelivery").pmvalue;       
            }catch (Exception ex) { throw ex; }
        }
    }
}