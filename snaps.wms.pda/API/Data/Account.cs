using snaps.wms.api.pda.Data;
using snaps.wms.api.pda.Models;
using snaps.wms.api.pda.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.pda.Manager {
    public class Account {
        private readonly IConnector connector;
        public Account(string _connectionString) {
            connector = new Connector(_connectionString);
        }
        public AccountModel profile(string accncode) {
            try {
                List<AccountModel> reports = new List<AccountModel>();
                var param = new Params();
                string commandText = "SELECT orgcode,accntype,accncode,accnname,accnsurname,accnavartar,accsrole,email,site,depot " +
                    " from wm_accn where accncode = @accncode";
                param.add("accncode", accncode);
                var datatable = connector.GetDtb(commandText, param.Values);
                if(datatable.Rows.Count > 0) {
                    return datatable.toObj<AccountModel>();
                } else {
                    return null;
                }
            } catch(Exception e) {
                throw e;
            } 
        }

    }
}
