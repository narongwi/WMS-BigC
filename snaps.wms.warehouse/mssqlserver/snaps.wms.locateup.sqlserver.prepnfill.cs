using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.warehouse {

    public partial class locup_ops : IDisposable { 

        private locup_ls fillls(ref SqlDataReader r) { 
            return new locup_ls() { 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                fltype = r["fltype"].ToString(),
                lslevel = r["lslevel"].ToString(),
                lscode = r["lscode"].ToString(),
                lszone = r["lszone"].ToString(),
                lsaisle = r["lsaisle"].ToString(),
                lsbay = r["lsbay"].ToString(),
                crfreepct = r.GetDecimal(18),
                tflow = r["tflow"].ToString(),
                lscodealt = r["lscodealt"].ToString(),
                lscodeid = r["lscodeid"].ToString(),
                datemodify = (r.IsDBNull(25)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(25)
            };
        }
        private locup_ix fillix(ref SqlDataReader r) { 
            return new locup_ix() { 

            };
        }
        private locup_md fillmdl(ref SqlDataReader r) { 
            locup_md rn =  new locup_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.fltype = r["fltype"].ToString();
            rn.lscode = r["lscode"].ToString();
            rn.lsformat = r["lsformat"].ToString();
            rn.lsseq = (r.IsDBNull(7)) ? 0 :  r.GetInt32(7);
            rn.lscodealt = r["lscodealt"].ToString();
            rn.lscodefull = r["lscodefull"].ToString();
            rn.lscodeid = r["lscodeid"].ToString();
            rn.lszone = r["lszone"].ToString();
            rn.lsaisle = r["lsaisle"].ToString();
            rn.lsbay = r["lsbay"].ToString();
            rn.lslevel = r["lslevel"].ToString();
            rn.crweight =  (r.IsDBNull(15)) ? 0 : r.GetDecimal(15);
            rn.crvolume =  (r.IsDBNull(16)) ? 0 : r.GetDecimal(16);
            rn.crlocation =  (r.IsDBNull(17)) ? 0 : r.GetDecimal(17);
            rn.crfreepct =  (r.IsDBNull(18)) ? 0 : r.GetDecimal(18);
            rn.tflow = r["tflow"].ToString();
            rn.tflowcnt = r["tflowcnt"].ToString();
            rn.lshash = r["lshash"].ToString();
            rn.datecreate = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.lsdesc = r["lsdesc"].ToString();
            return rn;
        }
        private SqlCommand ixcommand(locup_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand obcommand(locup_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.fltype,"fltype");
            cm.snapsPar(o.lscode,"lscode");
            cm.snapsPar(o.lsformat,"lsformat");
            cm.snapsPar(o.lsseq,"lsseq");
            cm.snapsPar(o.lscodealt,"lscodealt");
            cm.snapsPar(o.lscodefull,"lscodefull");
            cm.snapsPar(o.lscodeid,"lscodeid");
            cm.snapsPar(o.lszone,"lszone");
            cm.snapsPar(o.lsaisle,"lsaisle");
            cm.snapsPar(o.lsbay,"lsbay");
            cm.snapsPar(o.lslevel,"lslevel");
            cm.snapsPar(o.crweight,"crweight");
            cm.snapsPar(o.crvolume,"crvolume");
            cm.snapsPar(o.crlocation,"crlocation");
            cm.snapsPar(o.crfreepct,"crfreepct");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.tflowcnt,"tflowcnt");
            cm.snapsPar(o.lshash,"lshash");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.lsdesc,"lsdesc");
            return cm;
        }
        public SqlCommand oucommand(locup_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.fltype,"fltype");
            cm.snapsPar(o.lscodeid,"lscodeid");
            return cm;
        }

    }
}