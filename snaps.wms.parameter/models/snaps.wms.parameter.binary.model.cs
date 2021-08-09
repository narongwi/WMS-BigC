using System;

namespace Snaps.WMS.parameter { 
    public class binary_ls {
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String bntype { get; set; } 
        public String bnvalue { get; set;} 
        public String bncode { get; set; } 
        public String bndesc { get; set; } 
        public String apps { get; set; }
        public String bnicon { get; set; }
        public String bndescalt { get; set; } 
        public String bnflex1 { get; set; } 
        public String bnflex2 { get; set; } 
        public String bnflex3 { get; set; } 
        public String bnflex4 { get; set; }

    }
    public class binary_pm : binary_ls { 
    }
    public class binary_ix : binary_ls { 
    }
    public class binary_md : binary_ls  {
       
        public String bnstate { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String accnmodify { get; set; } 
    }
    public class binary_prc { 
        public String opsaccn { get; set; }
        public binary_md opsobj { get; set; } 
    }
}