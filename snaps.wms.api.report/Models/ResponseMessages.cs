using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.report.Models
{
    /// <summary>
    /// response object message
    /// </summary>
    public class ResponseMessages
    {
        public ResponseMessages()
        {
        }

        public ResponseMessages(bool success, string message, object data)
        {
            this.success = success;
            this.message = message;
            this.data = data;
        }

        /// <summary>
        /// true = success, false = error
        /// </summary>
        public bool success { get; set; }

        // error message
        public string message { get; set; }

        // data object return
        public object data { get; set; }
    }
}
