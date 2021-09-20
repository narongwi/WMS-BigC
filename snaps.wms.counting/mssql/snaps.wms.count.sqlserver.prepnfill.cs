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

namespace Snaps.WMS {
    public partial class counting_ops : IDisposable { 

        //Task
        public void fillCommand(ref SqlCommand cm, counttask_md o) {
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.counttype,"counttype");
            cm.snapsPar(o.countcode,"countcode");
            cm.snapsPar(o.countname,"countname");
            cm.snapsPar(o.datestart,"datestart");
            cm.snapsPar(o.dateend,"dateend");
            cm.snapsPar(o.isblock,"isblock");
            cm.snapsPar(o.remarks,"remarks");
            cm.snapsPar(o.tflow=="NW"?"IO": o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
        }
        public SqlCommand getCommand(counttask_md o,string cmd = "") {
            SqlCommand cm = new SqlCommand(cmd,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.counttype,"counttype");
            cm.snapsPar(o.countcode,"countcode");
            cm.snapsPar(o.countname,"countname");
            cm.snapsPar(o.datestart,"datestart");
            cm.snapsPar(o.dateend,"dateend");
            cm.snapsPar(o.isblock,"isblock");
            cm.snapsPar(o.remarks,"remarks");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
             return cm;
        }
        public counttask_md setCounttask(ref SqlDataReader r) {
            counttask_md rn = new counttask_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.counttype = r["counttype"].ToString();
            rn.countcode = r["countcode"].ToString();
            rn.countname = r["countname"].ToString();
            rn.datestart = (r.IsDBNull(7)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(7);
            rn.dateend = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
            rn.isblock = (r.IsDBNull(9)) ? 0 :  r.GetInt32(9);
            rn.remarks = r["remarks"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(12)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(12);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(14)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(14);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
             return rn;
        }

        //Plan
        public void fillCommand(ref SqlCommand cm, countplan_md o) {
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.countcode,"countcode");
            cm.snapsPar(o.plancode,"plancode");
            cm.snapsPar(o.planname,"planname");
            cm.snapsPar(o.accnassign,"accnassign");
            cm.snapsPar(o.accnwork,"accnwork");
            cm.snapsPar(o.szone,"szone");
            cm.snapsPar(o.ezone,"ezone");
            cm.snapsPar(o.saisle,"saisle");
            cm.snapsPar(o.eaisle,"eaisle");
            cm.snapsPar(o.sbay,"sbay");
            cm.snapsPar(o.ebay,"ebay");
            cm.snapsPar(o.slevel,"slevel");
            cm.snapsPar(o.elevel,"elevel");
            cm.snapsPar(o.isroaming,"isroaming");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.cntpercentage,"cntpercentage");
            cm.snapsPar(o.cnterror,"cnterror");
            cm.snapsPar(o.cntlines,"cntlines");
            cm.snapsPar(o.datestart,"datestart");
            cm.snapsPar(o.pctstart,"pctstart");
            cm.snapsPar(o.datevld,"datevld");
            cm.snapsPar(o.pctvld,"pctvld");
            cm.snapsPar(o.accnvld,"accnvld");
            cm.snapsPar(o.devicevld,"devicevld");
            cm.snapsPar(o.remarksvld,"remarksvld");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.isblock,"isblock");
            cm.snapsPar(o.isdatemfg,"isdatemfg");
            cm.snapsPar(o.isdateexp,"isdateexp");
            cm.snapsPar(o.isbatchno,"isbatchno");
            cm.snapsPar(o.isserialno,"isserialno"); 
            cm.snapsPar(o.allowscanhu,"allowscanhu");
        }
        public SqlCommand getCommand(countplan_md o) {
            SqlCommand cm = new SqlCommand("",cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.countcode,"countcode");
            cm.snapsPar(o.plancode,"plancode");
            cm.snapsPar(o.planname,"planname");
            cm.snapsPar(o.accnassign,"accnassign");
            cm.snapsPar(o.accnwork,"accnwork");
            cm.snapsPar(o.szone,"szone");
            cm.snapsPar(o.ezone,"ezone");
            cm.snapsPar(o.saisle,"saisle");
            cm.snapsPar(o.eaisle,"eaisle");
            cm.snapsPar(o.sbay,"sbay");
            cm.snapsPar(o.ebay,"ebay");
            cm.snapsPar(o.slevel,"slevel");
            cm.snapsPar(o.elevel,"elevel");
            cm.snapsPar(o.isroaming,"isroaming");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.cntpercentage,"cntpercentage");
            cm.snapsPar(o.cnterror,"cnterror");
            cm.snapsPar(o.cntlines,"cntlines");
            cm.snapsPar(o.datestart,"datestart");
            cm.snapsPar(o.pctstart,"pctstart");
            cm.snapsPar(o.datevld,"datevld");
            cm.snapsPar(o.pctvld,"pctvld");
            cm.snapsPar(o.accnvld,"accnvld");
            cm.snapsPar(o.devicevld,"devicevld");
            cm.snapsPar(o.remarksvld,"remarksvld");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
             return cm;
        }
        public countplan_md setCountplan(ref SqlDataReader r) {
            countplan_md rn = new countplan_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.countcode = r["countcode"].ToString();
            rn.plancode = r["plancode"].ToString();
            rn.planname = r["planname"].ToString();
            rn.accnassign = r["accnassign"].ToString();
            rn.accnwork = r["accnwork"].ToString();
            rn.szone = r["szone"].ToString();
            rn.ezone = r["ezone"].ToString();
            rn.saisle = r["saisle"].ToString();
            rn.eaisle = r["eaisle"].ToString();
            rn.sbay = r["sbay"].ToString();
            rn.ebay = r["ebay"].ToString();
            rn.slevel = r["slevel"].ToString();
            rn.elevel = r["elevel"].ToString();
            rn.isroaming = (r.IsDBNull(17)) ? 0 :  r.GetInt32(17);
            rn.tflow = r["tflow"].ToString();
            rn.cntpercentage = (r.IsDBNull(19)) ? 0 :  r.GetInt32(19);
            rn.cnterror = (r.IsDBNull(20)) ? 0 :  r.GetInt32(20);
            rn.cntlines = (r.IsDBNull(21)) ? 0 :  r.GetInt32(21);

            rn.datestart = (r.IsDBNull(23)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(23);
            rn.pctstart = (r.IsDBNull(24)) ? 0 :  r.GetInt32(24);
            rn.datevld = (r.IsDBNull(25)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(25);
            rn.pctvld = (r.IsDBNull(26)) ? 0 :  r.GetInt32(26);
            rn.accnvld = r["accnvld"].ToString();
            rn.devicevld = r["devicevld"].ToString();
            rn.remarksvld = r["remarksvld"].ToString();
            rn.datecreate = (r.IsDBNull(30)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(30);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(32)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(32);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.planorigin = r["planorigin"].ToString();
            return rn;
        }
        public countcorrection_md setCountcorrection(ref SqlDataReader r) {
            countcorrection_md rn = new countcorrection_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.counttype = r["counttype"].ToString();
            rn.countcode = r["countcode"].ToString();
            rn.countname = r["countname"].ToString();
            rn.barcode = r["cnbarcode"].ToString();
            rn.article = r["cnarticle"].ToString();
            rn.pv = r["cnpv"].ToString().CInt32();
            rn.lv = r["cnlv"].ToString().CInt32();
            rn.description = r["descalt"].ToString();
            rn.corcode = r["corcode"].ToString();
            rn.corsku = r["corsku"].ToString().CInt32();
            rn.corqty = r["corqty"].ToString().CInt32();
            rn.unitcount = r["unitcount"].ToString();
            rn.skuofpck = r["rtoipckofpck"].ToString().CInt32();
            rn.skuweight = r["skugrossweight"].ToString().CDouble();
            rn.skuvolume = r["skuvolume"].ToString().CDouble();
            rn.unitmanage = r["unitmanage"].ToString().CInt32();
            rn.unitprep = r["unitprep"].ToString().CInt32();
            rn.taskstate = r["tflow"].ToString();
            return rn;
        }
        public confirmline_md setConfirmLine(ref SqlDataReader r) {
            confirmline_md rn = new confirmline_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.counttype = r["counttype"].ToString();
            rn.countcode = r["countcode"].ToString();
            rn.countname = r["countname"].ToString();
            rn.datestart = (r.IsDBNull(r.GetOrdinal("datestart"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("datestart"));
            rn.dateend = (r.IsDBNull(r.GetOrdinal("dateend"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("dateend"));
            rn.plancode = r["plancode"].ToString();
            rn.loccode = r["loccode"].ToString();
            rn.pctvld = r["pctvld"].ToString().CInt32();
            rn.locseq = r["locseq"].ToString().CInt32();
            rn.huno = r["sthuno"].ToString();
            rn.unitcount = r["unitcount"].ToString();
            rn.barcode = r["stbarcode"].ToString();
            rn.article = r["starticle"].ToString();
            rn.description = r["descalt"].ToString();
            rn.pv = r["stpv"].ToString().CInt32();
            rn.lv = r["stlv"].ToString().CInt32();
            rn.stqtysku = r["stqtysku"].ToString().CInt32();
            rn.stqtypu = r["stqtypu"].ToString().CInt32();
            rn.cnqtypu = r["cnqtypu"].ToString().CInt32();
            rn.corcode = r["corcode"].ToString();
            rn.corqty = r["corqty"].ToString().CInt32();
            rn.skuofpck = r["rtoipckofpck"].ToString().CInt32();
            rn.skuweight = r["skugrossweight"].ToString().CDouble();
            rn.skuvolume = r["skuvolume"].ToString().CDouble();
            rn.unitmanage = r["unitmanage"].ToString().CInt32();
            rn.unitprep = r["unitprep"].ToString().CInt32();
            rn.taskstate = r["tflow"].ToString();
            rn.mfglot = r["cnlotmfg"].ToString();
            rn.serial = r["cnserialno"].ToString();
            rn.cnflow = r["cnflow"].ToString();
            rn.cnmsg = r["cnmsg"].ToString();
            rn.datemfg = (r.IsDBNull(r.GetOrdinal("cndatemfg"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("cndatemfg"));
            rn.dateexp = (r.IsDBNull(r.GetOrdinal("cndateexp"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("cndateexp"));
            rn.isbatchno = r["isbatchno"].ToString().CInt32();
            rn.isdatexp = r["isdatexp"].ToString().CInt32();
            rn.isdatemfg = r["isdatemfg"].ToString().CInt32();
            rn.isserailno = r["isserailno"].ToString().CInt32();
            rn.locctype = r["locctype"].ToString();
            return rn;
        }
        //Line
        public void fillCommand(ref SqlCommand cm, countline_md o) {
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.countcode,"countcode");
            cm.snapsPar(o.plancode,"plancode");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.locseq,"locseq");
            cm.snapsPar(o.unitcount,"unitcount");
            cm.snapsPar(o.stbarcode,"stbarcode");
            cm.snapsPar(o.starticle,"starticle");
            cm.snapsPar(o.stpv,"stpv");
            cm.snapsPar(o.stlv,"stlv");
            cm.snapsPar(o.stqtysku,"stqtysku");
            cm.snapsPar(o.stqtypu,"stqtypu");
            cm.snapsPar(o.stlotmfg,"stlotmfg");
            cm.snapsPar(o.stdatemfg,"stdatemfg");
            cm.snapsPar(o.stdateexp,"stdateexp");
            cm.snapsPar(o.stserialno,"stserialno");
            cm.snapsPar(o.sthuno,"sthuno");
            cm.snapsPar(o.cnbarcode,"cnbarcode");
            cm.snapsPar(o.cnarticle,"cnarticle");
            cm.snapsPar(o.cnpv,"cnpv");
            cm.snapsPar(o.cnlv,"cnlv");
            cm.snapsPar(o.cnqtysku,"cnqtysku");
            cm.snapsPar(o.cnqtypu,"cnqtypu");
            cm.snapsPar(o.cnlotmfg,"cnlotmfg");
            cm.snapsPar(o.cndatemfg,"cndatemfg");
            cm.snapsPar(o.cndateexp,"cndateexp");
            cm.snapsPar(o.cnserialno,"cnserialno");
            cm.snapsPar(o.cnhuno,"cnhuno");
            cm.snapsPar(o.cnflow,"cnflow");
            cm.snapsPar(o.cnmsg,"cnmsg");
            cm.snapsPar(o.isskip,"isskip");
            cm.snapsPar(o.isrgen,"isrgen");
            cm.snapsPar(o.iswrgln,"iswrgln");
            cm.snapsPar(o.countdevice,"countdevice");
            cm.snapsPar(o.countdate,"countdate");
            cm.snapsPar(o.corcode,"corcode");
            cm.snapsPar(o.corqty,"corqty");
            cm.snapsPar(o.coraccn,"coraccn");
            cm.snapsPar(o.cordevice,"cordevice");
            cm.snapsPar(o.cordate,"cordate");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
        }
        public SqlCommand getCommand(countline_md o,string cmd = "") {
            SqlCommand cm = new SqlCommand(cmd,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.countcode,"countcode");
            cm.snapsPar(o.plancode,"plancode");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.locseq,"locseq");
            cm.snapsPar(o.unitcount,"unitcount");
            cm.snapsPar(o.stbarcode,"stbarcode");
            cm.snapsPar(o.starticle,"starticle");
            cm.snapsPar(o.stpv,"stpv");
            cm.snapsPar(o.stlv,"stlv");
            cm.snapsPar(o.stqtysku,"stqtysku");
            cm.snapsPar(o.stqtypu,"stqtypu");
            cm.snapsPar(o.stlotmfg,"stlotmfg");
            cm.snapsPar(o.stdatemfg,"stdatemfg");
            cm.snapsPar(o.stdateexp,"stdateexp");
            cm.snapsPar(o.stserialno,"stserialno");
            cm.snapsPar(o.sthuno,"sthuno");
            cm.snapsPar(o.cnbarcode,"cnbarcode");
            cm.snapsPar(o.cnarticle,"cnarticle");
            cm.snapsPar(o.cnpv,"cnpv");
            cm.snapsPar(o.cnlv,"cnlv");
            cm.snapsPar(o.cnqtysku,"cnqtysku");
            cm.snapsPar(o.cnqtypu,"cnqtypu");
            cm.snapsPar(o.cnlotmfg,"cnlotmfg");
            cm.snapsPar(o.cndatemfg,"cndatemfg");
            cm.snapsPar(o.cndateexp,"cndateexp");
            cm.snapsPar(o.cnserialno,"cnserialno");
            cm.snapsPar(o.cnhuno,"cnhuno");
            cm.snapsPar(o.cnflow,"cnflow");
            cm.snapsPar(o.cnmsg,"cnmsg");
            cm.snapsPar(o.isskip,"isskip");
            cm.snapsPar(o.isrgen,"isrgen");
            cm.snapsPar(o.iswrgln,"iswrgln");
            cm.snapsPar(o.countdevice,"countdevice");
            cm.snapsPar(o.countdate,"countdate");
            cm.snapsPar(o.corcode,"corcode");
            cm.snapsPar(o.corqty,"corqty");
            cm.snapsPar(o.coraccn,"coraccn");
            cm.snapsPar(o.cordevice,"cordevice");
            cm.snapsPar(o.cordate,"cordate");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
             return cm;
        }
        public SqlCommand getZeroCommand(countline_md o,string cmd = "") {
            SqlCommand cm = new SqlCommand(cmd,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.countcode,"countcode");
            cm.snapsPar(o.plancode,"plancode");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.locseq,"locseq");
            cm.snapsPar(o.unitcount,"unitcount");
            cm.snapsPar(o.stbarcode,"stbarcode");
            cm.snapsPar(o.starticle,"starticle");
            cm.snapsPar(o.stpv,"stpv");
            cm.snapsPar(o.stlv,"stlv");
            cm.snapsPar(o.stqtysku,"stqtysku");
            cm.snapsPar(o.stqtypu,"stqtypu");
            cm.snapsPar(o.stlotmfg,"stlotmfg");
            cm.snapsPar(o.stdatemfg,"stdatemfg");
            cm.snapsPar(o.stdateexp,"stdateexp");
            cm.snapsPar(o.stserialno,"stserialno");
            cm.snapsPar(o.sthuno,"sthuno");
            cm.snapsPar(o.stbarcode,"cnbarcode");
            cm.snapsPar(o.cnarticle,"cnarticle");
            cm.snapsPar(o.cnpv,"cnpv");
            cm.snapsPar(o.cnlv,"cnlv");
            cm.snapsPar(0,"cnqtysku");
            cm.snapsPar(0,"cnqtypu");
            cm.snapsPar(o.cnlotmfg,"cnlotmfg");
            cm.snapsPar(o.cndatemfg,"cndatemfg");
            cm.snapsPar(o.cndateexp,"cndateexp");
            cm.snapsPar(o.cnserialno,"cnserialno");
            cm.snapsPar(o.cnhuno,"cnhuno");
            cm.snapsPar(o.cnflow,"cnflow");
            cm.snapsPar(o.cnmsg,"cnmsg");
            cm.snapsPar(o.isskip,"isskip");
            cm.snapsPar(o.isrgen,"isrgen");
            cm.snapsPar(o.iswrgln,"iswrgln");
            cm.snapsPar(o.countdevice,"countdevice");
            cm.snapsPar(o.countdate,"countdate");
            cm.snapsPar(o.corcode,"corcode");
            cm.snapsPar(o.corqty,"corqty");
            cm.snapsPar(o.coraccn,"coraccn");
            cm.snapsPar(o.cordevice,"cordevice");
            cm.snapsPar(o.cordate,"cordate");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            return cm;
        }
        public countline_md setCountline(ref SqlDataReader r) {
            countline_md rn = new countline_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.countcode = r["countcode"].ToString();
            rn.plancode = r["plancode"].ToString();
            rn.loccode = r["loccode"].ToString();
            rn.locseq = (r.IsDBNull(7)) ? 0 :  r.GetInt32(7);
            rn.unitcount = r["unitcount"].ToString();
            rn.stbarcode = r["stbarcode"].ToString();
            rn.starticle = r["starticle"].ToString();
            rn.stpv = (r.IsDBNull(11)) ? 0 :  r.GetInt32(11);
            rn.stlv = (r.IsDBNull(12)) ? 0 :  r.GetInt32(12);
            rn.stqtysku = (r.IsDBNull(13)) ? 0 :  r.GetInt32(13);
            rn.stqtypu = (r.IsDBNull(14)) ? 0 :  r.GetInt32(14);
            rn.stlotmfg = r["stlotmfg"].ToString();
            rn.stdatemfg = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(16);
            rn.stdateexp = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17);
            rn.stserialno = r["stserialno"].ToString();
            rn.sthuno = r["sthuno"].ToString();
            rn.cnbarcode = r["cnbarcode"].ToString();
            rn.cnarticle = r["cnarticle"].ToString();
            rn.cnpv = (r.IsDBNull(22)) ? 0 :  r.GetInt32(22);
            rn.cnlv = (r.IsDBNull(23)) ? 0 :  r.GetInt32(23);
            rn.cnqtysku = (r.IsDBNull(24)) ? 0 :  r.GetInt32(24);
            rn.cnqtypu = (r.IsDBNull(25)) ? 0 :  r.GetInt32(25);
            rn.cnlotmfg = r["cnlotmfg"].ToString();
            rn.cndatemfg = (r.IsDBNull(27)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(27);
            rn.cndateexp = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(28);
            rn.cnserialno = r["cnserialno"].ToString();
            rn.cnhuno = r["cnhuno"].ToString();
            rn.cnflow = r["cnflow"].ToString();
            rn.cnmsg = r["cnmsg"].ToString();
            rn.isskip = (r.IsDBNull(33)) ? 0 :  r.GetInt32(33);
            rn.isrgen = (r.IsDBNull(34)) ? 0 :  r.GetInt32(34);
            rn.iswrgln = (r.IsDBNull(35)) ? 0 :  r.GetInt32(35);
            rn.countdevice = r["countdevice"].ToString();
            rn.countdate = (r.IsDBNull(37)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(37);
            rn.corcode = r["corcode"].ToString();
            rn.corqty = (r.IsDBNull(39)) ? 0 :  r.GetInt32(39);
            rn.coraccn = r["coraccn"].ToString();
            rn.cordevice = r["cordevice"].ToString();
            rn.cordate = (r.IsDBNull(42)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(42);
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(44)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(44);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(46)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(46);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.productdesc = r["description"].ToString();
            rn.locctype = r["locctype"].ToString();
            rn.seqno = (r["seqno"]??0).ToString().CDouble();
            rn.addnew = (r["addnew"]??0).ToString().CInt32();
            return rn;
        }



    }
}