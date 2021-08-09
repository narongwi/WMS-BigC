using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.pda.Models {
    public class AccountModel {
        public string orgcode { get; set; }
        public string accntype { get; set; }
        public string accncode { get; set; }
        public string accnname { get; set; }
        public string accnsurname { get; set; }
        public string accnavartar { get; set; }
        public string accsrole { get; set; }
        public string email { get; set; }
        public string site { get; set; }
        public string depot { get; set; }

    }
}
