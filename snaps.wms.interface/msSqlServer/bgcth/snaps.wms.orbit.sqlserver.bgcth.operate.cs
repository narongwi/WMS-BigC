using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Oracle.ManagedDataAccess.Client;
using Snaps.Helpers.Logger;

namespace Snaps.WMS {
    public partial class orbit_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        private string cnx_snapswms = "";
        private string cnx_legacysource = "";
        public orbit_ops() { }
        public orbit_ops(SqlConnection cx) {
            cn = cx;
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<orbit_ops>();
        }
        public orbit_ops(string snapsWMS, string LegacySource){     
            this.cn = new SqlConnection(snapsWMS);
            this.cnx_snapswms = snapsWMS;
            this.cnx_legacysource = LegacySource;
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                if (cn != null) { cn.Dispose(); }
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}