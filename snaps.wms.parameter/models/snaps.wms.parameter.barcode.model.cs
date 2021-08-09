using System;
using System.Data.Common;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
namespace Snaps.WMS { 
    public class pam_barcode  {
        public bool allowchangeofexsource { get; set; }
   

        public pam_barcode(){ } 
        public pam_barcode(List<pam_set> o) { 
            try { 
                //this.allowchangeofexsource = o.Find(x=>x.pmcode == "allowchangeofexsource").pmvalue;
                this.allowchangeofexsource = o.Find(x=>x.pmcode == "allowchangeofexsource").pmvalue;
            }catch(Exception ex) { throw ex; }
        }
    }
}