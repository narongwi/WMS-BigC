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

namespace Snaps.WMS.warehouse {

    public partial class zoneprep_ops : IDisposable { 
        //Header
        public void fillPRpar(ref SqlCommand cm, zoneprep_md o) {
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.przone,"przone");
            cm.snapsPar(o.przonename,"przonename");
            cm.snapsPar(o.przonedesc,"przonedesc");
            cm.snapsPar(o.hutype,"hutype");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");

            cm.snapsPar(o.huvalweight,"huvalweight");
            cm.snapsPar(o.huvalvolume,"huvalvolume");
            cm.snapsPar(o.hucapweight,"hucapweight");
            cm.snapsPar(o.hucapvolume,"hucapvolume");
    

        }
        public void fillPRcmd(ref SqlCommand x, zoneprep_md o){
            x.Parameters["orgcode"].Value = o.orgcode;
            x.Parameters["site"].Value = o.site;
            x.Parameters["depot"].Value = o.depot;
            x.Parameters["spcarea"].Value = o.spcarea;
            x.Parameters["przone"].Value = o.przone;
            x.Parameters["przonename"].Value = o.przonename;
            x.Parameters["przonedesc"].Value = o.przonedesc;
            x.Parameters["hutype"].Value = o.hutype;
            x.Parameters["tflow"].Value = o.tflow;
            x.Parameters["datecreate"].Value = o.datecreate;
            x.Parameters["accncreate"].Value = o.accncreate;
            x.Parameters["datemodify"].Value = o.datemodify;
            x.Parameters["accnmodify"].Value = o.accnmodify;
            x.Parameters["procmodify"].Value = o.procmodify;

            
            x.Parameters["huvalweight"].Value = o.huvalweight;
            x.Parameters["huvalvolume"].Value = o.huvalvolume;
            x.Parameters["hucapweight"].Value = o.hucapweight;
            x.Parameters["hucapvolume"].Value = o.hucapvolume;
        }
        public zoneprep_md fillPR(ref SqlDataReader r) {
            zoneprep_md rn = new zoneprep_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.przone = r["przone"].ToString();
            rn.przonename = r["przonename"].ToString();
            rn.przonedesc = r["przonedesc"].ToString();
            rn.hutype = r["hutype"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();

            rn.huvalweight = r["huvalweight"].ToString().CDecimal();
            rn.huvalvolume = r["huvalvolume"].ToString().CDecimal();
            rn.hucapweight = r["hucapweight"].ToString().CDecimal();
            rn.hucapvolume = r["hucapvolume"].ToString().CDecimal();
            return rn;
        }
        public SqlCommand getPRcmd(zoneprep_md o){ 
            SqlCommand cm = new SqlCommand();
            fillPRpar(ref cm,o);
            return cm;
        }

        //Line
        public void fillPLpar(ref SqlCommand cm, zoneprln_md o){ 
            o.lshash = (o.orgcode+o.site+o.depot+o.spcarea+o.przone+o.lscode+o.spcproduct+o.spcpv+o.spclv+o.lspath.ToString()).ToHash();
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.fltype,"fltype");
            cm.snapsPar(o.przone,"przone");
            cm.snapsPar(o.lszone,"lszone");
            cm.snapsPar(o.lsaisle,"lsaisle");
            cm.snapsPar(o.lsbay,"lsbay");
            cm.snapsPar(o.lslevel,"lslevel");
            cm.snapsPar(o.lsloc,"lsloc");
            cm.snapsPar(o.lsstack,"lsstack");
            cm.snapsPar(o.lscode,"lscode");
            cm.snapsPar(string.IsNullOrEmpty(o.spcproduct) ? null : o.spcproduct, "spcproduct");
            cm.snapsPar(string.IsNullOrEmpty(o.spcpv) ? null : o.spcpv, "spcpv");
            cm.snapsPar(string.IsNullOrEmpty(o.spclv) ? null : o.spclv, "spclv");
            cm.snapsPar(string.IsNullOrEmpty(o.spcunit) ? null : o.spcunit, "spcunit");           
            cm.snapsPar(o.spcthcode,"spcthcode");
            cm.snapsPar(o.lsdirection,"lsdirection");
            cm.snapsPar(o.lspath,"lspath");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.lshash,"lshash");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.rtoskuofpu,"rtoskuofpu");
            
        }
        public void fillPLcmd(ref SqlCommand x, zoneprln_md o){
            x.Parameters["orgcode"].Value = o.orgcode;
            x.Parameters["site"].Value = o.site;
            x.Parameters["depot"].Value = o.depot;
            x.Parameters["spcarea"].Value = o.spcarea;
            x.Parameters["fltype"].Value = o.fltype;
            x.Parameters["przone"].Value = o.przone;
            x.Parameters["lszone"].Value = o.lszone;
            x.Parameters["lsaisle"].Value = o.lsaisle;
            x.Parameters["lsbay"].Value = o.lsbay;
            x.Parameters["lslevel"].Value = o.lslevel;
            x.Parameters["lsloc"].Value = o.lsloc;
            x.Parameters["lsstack"].Value = o.lsstack;
            x.Parameters["lscode"].Value = o.lscode;
            x.Parameters["spcproduct"].Value = o.spcproduct;
            x.Parameters["spcpv"].Value = o.spcpv;
            x.Parameters["spclv"].Value = o.spclv;
            x.Parameters["spcunit"].Value = o.spcunit;
            x.Parameters["spcthcode"].Value = o.spcthcode;
            x.Parameters["lsdirection"].Value = o.lsdirection;
            x.Parameters["lspath"].Value = o.lspath;
            x.Parameters["tflow"].Value = o.tflow;
            x.Parameters["lshash"].Value = o.lshash;
            x.Parameters["datecreate"].Value = o.datecreate;
            x.Parameters["accncreate"].Value = o.accncreate;
            x.Parameters["datemodify"].Value = o.datemodify;
            x.Parameters["accnmodify"].Value = o.accnmodify;
            x.Parameters["procmodify"].Value = o.procmodify;
        }
        public zoneprln_md fillPL(ref SqlDataReader r){ 
            zoneprln_md rn = new zoneprln_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.fltype = r["fltype"].ToString();
            rn.przone = r["przone"].ToString();
            rn.lszone = r["lszone"].ToString();
            rn.lsaisle = r["lsaisle"].ToString();
            rn.lsbay = r["lsbay"].ToString();
            rn.lslevel = r["lslevel"].ToString();
            rn.lsloc = r["lsloc"].ToString();
            rn.lsstack = r["lsstack"].ToString();
            rn.lscode = r["lscode"].ToString();
            rn.spcproduct = r["spcproduct"].ToString();
            rn.spcpv = r["spcpv"].ToString();
            rn.spclv = r["spclv"].ToString();
            rn.spcunit = r["spcunit"].ToString();
            rn.spcthcode = r["spcthcode"].ToString();
            rn.lsdirection = r["lsdirection"].ToString();
            rn.lspath = (r.IsDBNull(19)) ? 0 :  r.GetInt32(19);
            rn.tflow = r["tflow"].ToString();
            rn.lshash = r["lshash"].ToString();
            rn.datecreate = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            return rn;
        }
        public SqlCommand getPLcmd(zoneprln_md o){ 
            SqlCommand cm = new SqlCommand();
            fillPLpar(ref cm,o);
            return cm;
        }
        
    }
}