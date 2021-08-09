using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Snaps.WMS { 
    public partial class parameter_ops : IDisposable {
        public string sqlfind = "select * from wm_parameters where orgcode = @orgcode and site = @site and depot = @depot   ";
        public string sqlupdate = "update wm_parameters set datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify,       " +
        " pmvalue = @pmvalue, pmoption = @pmoption where orgcode = @orgcode and site = @site and depot = @depot and pmmodule = @pmmodule          " + 
        " and pmtype = @pmtype and pmcode = @pmcode  ";
    }
}