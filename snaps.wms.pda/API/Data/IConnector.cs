using snaps.wms.api.pda.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.pda.Data {
    interface IConnector {
        int GetInt(string commandText, params Params[] parameters);
        double GetDbl(string commandText, params Params[] parameters);
        string GetStr(string commandText, params Params[] parameters);
        DataTable GetDtb(string commandText, params Params[] parameters);
        bool ExcSql(string commandText, params Params[] parameters);
    }
}
