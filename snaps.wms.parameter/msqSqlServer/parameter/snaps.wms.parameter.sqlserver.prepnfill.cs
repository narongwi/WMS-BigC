using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS { 
    public partial class parameter_ops : IDisposable {
       
       public SqlCommand getCommand(pam_parameter o){ 
            SqlCommand rn = new SqlCommand(sqlupdate,cn);
            try {
                rn.snapsPar(o.accncreate,"accncreate");
                rn.snapsPar(o.accnmodify,"accnmodify");
                rn.snapsPar(o.apps,"apps");
                rn.snapsPar(o.depot,"depot");
                rn.snapsPar(o.orgcode,"orgcode");
                rn.snapsPar(o.pmcode,"pmcode");
                rn.snapsPar(o.pmdesc,"pmdesc");
                rn.snapsPar(o.pmdescalt,"pmdescalt");
                rn.snapsPar(o.pmmodule,"pmmodule");
                rn.snapsPar(o.pmstate,"pmstate");
                rn.snapsPar(o.pmtype,"pmtype");
                rn.snapsPar(o.pmvalue,"pmvalue");
                rn.snapsPar(o.pmoption,"pmoption");
                rn.snapsPar(o.site,"site");
                return rn;
            }catch (Exception ex) { throw ex; }
       }
    }
}