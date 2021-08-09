using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.Hash;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.WMS.parameter;
namespace Snaps.WMS.warehouse {

    public partial class binary_ops : IDisposable { 
        //Header
        public void Fillpars(ref SqlCommand cm, binary_md o) {
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.apps,"apps");
            cm.snapsPar(o.bntype,"bntype");
            cm.snapsPar(o.bncode,"bncode");
            cm.snapsPar(o.bnvalue,"bnvalue");
            cm.snapsPar(o.bndesc,"bndesc");
            cm.snapsPar(o.bndescalt,"bndescalt");
            cm.snapsPar(o.bnflex1,"bnflex1");
            cm.snapsPar(o.bnflex2,"bnflex2");
            cm.snapsPar(o.bnflex3,"bnflex3");
            cm.snapsPar(o.bnflex4,"bnflex4");
            cm.snapsPar(o.bnicon,"bnicon");
            cm.snapsPar(o.bnstate,"bnstate");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
        }
        public void Fillparscmd(ref SqlCommand x, binary_md o){
            x.Parameters["orgcode"].Value = o.orgcode;
            x.Parameters["site"].Value = o.site;
            x.Parameters["depot"].Value = o.depot;
            x.Parameters["apps"].Value = o.apps;
            x.Parameters["bntype"].Value = o.bntype;
            x.Parameters["bncode"].Value = o.bncode;
            x.Parameters["bnvalue"].Value = o.bnvalue;
            x.Parameters["bndesc"].Value = o.bndesc;
            x.Parameters["bndescalt"].Value = o.bndescalt;
            x.Parameters["bnflex1"].Value = o.bnflex1;
            x.Parameters["bnflex2"].Value = o.bnflex2;
            x.Parameters["bnflex3"].Value = o.bnflex3;
            x.Parameters["bnflex4"].Value = o.bnflex4;
            x.Parameters["bnicon"].Value = o.bnicon;
            x.Parameters["bnstate"].Value = o.bnstate;
            x.Parameters["datecreate"].Value = o.datecreate;
            x.Parameters["accncreate"].Value = o.accncreate;
            x.Parameters["datemodify"].Value = o.datemodify;
            x.Parameters["accnmodify"].Value = o.accnmodify;
        }
        public binary_md Fill(ref SqlDataReader r) {
            binary_md rn = new binary_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.apps = r["apps"].ToString();
            rn.bntype = r["bntype"].ToString();
            rn.bncode = r["bncode"].ToString();
            rn.bnvalue = r["bnvalue"].ToString();
            rn.bndesc = r["bndesc"].ToString();
            rn.bndescalt = r["bndescalt"].ToString();
            rn.bnflex1 = r["bnflex1"].ToString();
            rn.bnflex2 = r["bnflex2"].ToString();
            rn.bnflex3 = r["bnflex3"].ToString();
            rn.bnflex4 = r["bnflex4"].ToString();
            rn.bnicon = r["bnicon"].ToString();
            rn.bnstate = r["bnstate"].ToString();
            rn.datecreate = (r.IsDBNull(15)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(15);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17);
            rn.accnmodify = r["accnmodify"].ToString();
            return rn;
        }
        public SqlCommand getcmd(binary_md o){ 
            SqlCommand cm = new SqlCommand();
            Fillpars(ref cm,o);
            return cm;
        }

        //LOV
        public lov FillLOV(ref SqlDataReader r) {
            lov rn = new lov();
            rn.desc = r["bndescalt"].ToString();
            rn.icon = r["bnicon"].ToString();
            rn.valopnfirst = r["bnflex1"].ToString();
            rn.valopnsecond = r["bnflex2"].ToString();
            rn.valopnthird = r["bnflex3"].ToString();
            rn.valopnfour = r["bnflex4"].ToString();
            rn.value = r["bnvalue"].ToString();
            return rn;
        }

        public lov FillLOVmaster(ref SqlDataReader r) {
            lov rn = new lov();
            rn.desc = r["bndesc"].ToString();
            rn.icon = r["bnicon"].ToString();
            rn.valopnfirst = r["bnflex1"].ToString();
            rn.valopnsecond = r["bnflex2"].ToString();
            rn.valopnthird = r["bnflex3"].ToString();
            rn.valopnfour = r["bnflex4"].ToString();
            rn.value = r["bnvalue"].ToString();
            return rn;
        }
    }
}