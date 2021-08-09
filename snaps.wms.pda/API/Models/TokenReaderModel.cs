using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.pda.Models {
    public class TokenReaderModel {
        public TokenReaderModel() {
        }

        public string Uname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string HasCode { get; set; }
        public string Images { get; set; }
    }
}
