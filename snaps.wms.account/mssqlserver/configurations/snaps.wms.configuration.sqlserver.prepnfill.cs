using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Newtonsoft.Json;
namespace Snaps.WMS {
    public partial class config_ops { 
        private void fillCommand(ref SqlCommand cm, config_md o){ 
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.apcode,"apcode");
            cm.snapsPar(o.accncode,"accncode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.cfgtype,"cfgtype");
            cm.snapsPar(o.cfgcode,"cfgcode");
            cm.snapsPar(o.cfgname,"cfgname");
            cm.snapsPar(o.cfgvalue,"cfgvalue");
            cm.snapsPar(o.cfghash,"cfghash");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.formatdate,"formatdate");
            cm.snapsPar(o.formatdateshort,"formatdateshort");
            cm.snapsPar(o.formatdatelong,"formatdatelong");
            cm.snapsPar(o.unitdimension,"unitdimension");
            cm.snapsPar(o.unitweight,"unitweight");
            cm.snapsPar(o.unitvolume,"unitvolume");
            cm.snapsPar(o.unitcubic,"unitcubic");
            cm.snapsPar(o.pagelimit,"pagelimit");
            cm.snapsPar(o.lang,"lang");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.isdefault,"isdefault");
        }
        public SqlCommand getCommand(config_md o){
            SqlCommand cm = new SqlCommand(sqlconfig_insert,cn);
            fillCommand(ref cm, o);
            return cm;
        }
        private config_md setConfiguration(ref SqlDataReader r,string objmodule = "web") {
            config_md rn = new config_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.apcode = r["apcode"].ToString();
            rn.accncode = r["accncode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.cfgtype = r["cfgtype"].ToString();
            rn.cfgcode = r["cfgcode"].ToString();
            rn.cfgname = r["cfgname"].ToString();
            rn.cfgvalue = r["cfgvalue"].ToString();
            rn.cfghash = r["cfghash"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.formatdate = r["formatdate"].ToString();
            rn.formatdateshort = r["formatdateshort"].ToString();
            rn.formatdatelong = r["formatdatelong"].ToString();
            rn.unitdimension = r["unitdimension"].ToString();
            rn.unitweight = r["unitweight"].ToString();
            rn.unitvolume = r["unitvolume"].ToString();
            rn.unitcubic = r["unitcubic"].ToString();
            rn.pagelimit = (r.IsDBNull(18)) ? 0 :  r.GetInt32(18);
            rn.lang = r["lang"].ToString();
            rn.datecreate = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            if (r["roljson"].ToString() != ""){
                rn.roleaccs = JsonConvert.DeserializeObject<accn_roleacs>(r["roljson"].ToString());
            }
            return rn;
        }


    }
}