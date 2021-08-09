using snaps.wms.api.pda.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.pda.Data {
    public class AppInfo {
        public string name { get; set; }
        public string package { get; set; }
        public string version { get; set; }
        public string information { get; set; }
        public string downloaduri { get; set; }
        public string apkname { get; set; }

        private readonly IConnector connector;

        public AppInfo() { }
        public AppInfo(string _connectionString) {
            connector = new Connector(_connectionString);
        }
        public AppInfo getInfo(string orgcode,string site,string depot) {
            string commandText =
               @"select bndesc as name,bndescalt as package,bnvalue as version,bnflex1 as information , bnflex2 downloaduri,bnflex3 apkname
                from wm_binary where orgcode = @orgcode and site =@site and depot = @depot and bntype ='PDA' 
                and bncode ='VERSION' and bnstate ='IO'";
            var param = new Params();
            param.add("orgcode",orgcode);
            param.add("site",site);
            param.add("depot",depot);
            var datatable = connector.GetDtb(commandText,param.Values);
            return datatable.toObj<AppInfo>();
        }
    }
}
