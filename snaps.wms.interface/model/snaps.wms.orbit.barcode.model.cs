using System;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS { 

    public class orbit_barcode  {
        public string orgcode               { get; set; }
        public string site      			{ get; set; }
        public string depot     			{ get; set; }
        public string spcarea   			{ get; set; }
        public string article   			{ get; set; }
        public Int32 pv         			{ get; set; }
        public Int32 lv         			{ get; set; }
        public string barops    			{ get; set; }
        public string barcode   			{ get; set; }
        public string bartype   			{ get; set; }
        public string thcode    			{ get; set; }
        public string tflow     			{ get; set; }
        public string fileid    			{ get; set; }
        public string rowops    			{ get; set; }
        public string ermsg     			{ get; set; }
        public DateTimeOffset? dateops    	{ get; set; }
        public string orbitsource           { get; set; }
    }
}