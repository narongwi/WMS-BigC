using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snaps.WMS
{
    public partial class sequence_ops : IDisposable {

        private String _cnx = "";
        public sequence_ops(){}

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                _cnx = null;
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
