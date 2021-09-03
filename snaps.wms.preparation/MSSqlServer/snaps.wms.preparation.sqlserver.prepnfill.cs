using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS.preparation {
    public partial class prep_ops : IDisposable {
        private prep_ls prep_getls(ref SqlDataReader r) { 

            prep_ls rn = new prep_ls();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.routeno = r["routeno"].ToString();
            rn.huno = r["huno"].ToString();
            rn.preptype = r["preptype"].ToString();
            rn.prepno = r["prepno"].ToString();
            rn.prepdate = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
            rn.priority = r["priority"].ToString().CInt32();//Hardcode to fix convert decimal to int32
            rn.thcode = r["thcode"].ToString();
            rn.spcorder = r["spcorder"].ToString();
            rn.spcarticle = r["spcarticle"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.thname = r["thname"].ToString();
            rn.picker = r["picker"].ToString();
            rn.preppct = r.GetDecimal(9);
            rn.preptypename = r["preptypename"].ToString();
            rn.przone = r["przone"].ToString();
            return rn;
        }
        private prep_ix prep_getix(ref SqlDataReader r) { 
            return new prep_ix() { 

            };
        }
        private prep_md prep_getmd(ref SqlDataReader r) { 
            prep_md rn = new prep_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.routeno = r["routeno"].ToString();
            rn.huno = r["huno"].ToString();
            rn.preptype = r["preptype"].ToString();
            rn.prepno = r["prepno"].ToString();
            //rn.prepdate = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
            rn.prepdate = (r.IsDBNull(r.GetOrdinal("prepdate"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("prepdate"));
            //rn.priority = (r.IsDBNull(9)) ? 0 : r.GetInt32(9);
            rn.priority = (r.IsDBNull(r.GetOrdinal("priority"))) ? 0 : r.GetInt32(r.GetOrdinal("priority"));
            rn.preppct = r.GetDecimal(r.GetOrdinal("preppct"));
            rn.thcode = r["thcode"].ToString();
            rn.spcorder = r["spcorder"].ToString();
            rn.spcarticle = r["spcarticle"].ToString();
            //rn.dateassign = (r.IsDBNull(14)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(14);
            //rn.datestart = (r.IsDBNull(15)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(15);
            //rn.dateend = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(16);
            rn.dateassign = (r.IsDBNull(r.GetOrdinal("dateassign"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("dateassign"));
            rn.datestart = (r.IsDBNull(r.GetOrdinal("datestart"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("datestart"));
            rn.dateend = (r.IsDBNull(r.GetOrdinal("dateend"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("dateend"));
            rn.tflow = r["tflow"].ToString();
            rn.deviceID = r["deviceID"].ToString();
            rn.picker = r["picker"].ToString();
            //rn.datecreate = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20);
            rn.datecreate = (r.IsDBNull(r.GetOrdinal("datecreate"))) ? (DateTimeOffset?) null : r.GetDateTimeOffset(r.GetOrdinal("datecreate"));
            rn.accncreate = r["accncreate"].ToString();
            //rn.datemodify = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22);
            rn.datemodify = (r.IsDBNull(r.GetOrdinal("datemodify"))) ? (DateTimeOffset?) null : r.GetDateTimeOffset(r.GetOrdinal("datemodify"));
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.przone = r["przone"].ToString();
            rn.loccode = r["loccode"].ToString();
            return rn;
        }
        private SqlCommand prep_setix(prep_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand prep_setmd(prep_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.routeno,"routeno");
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.preptype,"preptype");
            cm.snapsPar(o.prepno,"prepno");
            cm.snapsPar(o.prepdate,"prepdate");
            cm.snapsPar(o.priority.ToString(),"priority");
            cm.snapsPar(o.preppct,"preppct");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.spcorder,"spcorder");
            cm.snapsPar(o.spcarticle,"spcarticle");
            //cm.snapsPar(o.dateassign,"dateassign");
            //cm.snapsPar(o.datestart,"datestart");
            //cm.snapsPar(o.dateend,"dateend");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.deviceID,"deviceID");
            cm.snapsPar(o.picker,"picker");
            //cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            //cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");

            cm.snapsPar(o.hutype,"hutype");
            cm.snapsPar(o.przone,"przone");
            cm.snapsPar(o.setno,"setno");
            cm.snapsParsysdateoffset();
            return cm;
        }


        //Line 
        private prln_ls prln_getls(ref SqlDataReader r) { 
            prln_ls rn = new prln_ls(); 
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.article = r["article"].ToString();
            rn.description =  r["descalt"].ToString();
            rn.qtyskuorder = r.GetInt32(6);
            rn.qtypuorder = r.GetInt32(7);
            rn.qtypuops = r.GetInt32(9);
            rn.tflow = r["tflow"].ToString();
            rn.datemodify = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11);
            rn.unitprep = r["unitprep"].ToString();
            rn.unitname =  "";
            rn.qtyskuops = r.GetInt32(14);
            rn.prepno = r["prepno"].ToString();
            rn.prepln = r.GetInt32(16);
            return rn;
        }
        private prln_ix prln_getix(ref SqlDataReader r) { 
            return new prln_ix() { 

            };
        }

        private prln_md prln_getmd(
            string orgcode, string site, string depot, string spcarea,
            string huno, string prepno,Int32 diststockid, 
            DataRow r, Int32 qtyskuops,Int32 qtypuops, string batchno,string lotno, 
            DateTimeOffset? datemfg, DateTimeOffset? dateexp, string serialno,
            DateTimeOffset? daterec, string inagrn, string ingrno, Int32 prepln
             ) { 
            prln_md rn =  new prln_md();
            rn.orgcode = orgcode;
            rn.site = site;
            rn.depot = depot;
            rn.spcarea = spcarea;
            rn.huno = null;
            rn.hunosource = huno;
            rn.prepno = prepno;
            rn.prepln = prepln; 
            rn.loczone = r["spcdistzone"].ToString();
            rn.loccode = r["loccode"].ToString();
            rn.locseq = 1;//bug
            rn.locdigit = "";
            rn.ouorder = r["ouorder"].ToString();
            rn.ouln =  r["ouln"].ToString();
            rn.barcode = "";
            rn.article = r["article"].ToString();
            rn.pv = r["pv"].ToString().CInt32();
            rn.lv = r["lv"].ToString().CInt32();
            rn.stockid = diststockid;
            rn.unitprep = r["unitprep"].ToString();
            rn.qtyskuorder = qtyskuops;
            rn.qtypuorder = qtypuops;
            rn.qtyweightorder = qtyskuops * r["skuweight"].ToString().CDecimal();
            rn.qtyvolumeorder = qtyskuops * r["skuvolume"].ToString().CDecimal();
            rn.qtyskuops = 0;
            rn.qtypuops = 0;
            rn.qtyweightops = 0;
            rn.qtyvolumeops = 0;
            rn.batchno = batchno;
            rn.lotno = lotno;
            rn.datemfg = datemfg;
            rn.dateexp = dateexp;
            rn.serialno = serialno;
            rn.picker = "";
            rn.datepick = null;
            rn.devicecode =  null;
            rn.tflow = "IO";
            rn.rtoskuofpu =  r["ratiopu"].ToString().CInt32();
            rn.thcode = r["thcode"].ToString();
            rn.taskno = "";

            rn.daterec =  daterec;
            rn.inagrn = inagrn;
            rn.ingrno = ingrno;
            return rn;
        }

        private prln_md prln_getmd(ref SqlDataReader r) { 
            prln_md rn =  new prln_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.huno = r["huno"].ToString();
            rn.hunosource = r["hunosource"].ToString();
            rn.prepno = r["prepno"].ToString();
            rn.prepln = r["prepln"].ToString().CInt32();
            rn.loczone = r["loczone"].ToString();
            rn.loccode = r["loccode"].ToString();
            rn.locseq = r["locseq"].ToString().CInt32();
            rn.locdigit = r["locdigit"].ToString();
            rn.ouorder = r["ouorder"].ToString();
            rn.ouln =  r["ouln"].ToString();
            rn.barcode = r["barcode"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = r.GetInt32(r.GetOrdinal("pv"));
            rn.lv = r.GetInt32(r.GetOrdinal("lv"));
            rn.stockid = r.GetDecimal(r.GetOrdinal("stockid"));
            rn.unitprep = r["unitprep"].ToString();
            rn.qtyskuorder = r.GetInt32(r.GetOrdinal("qtyskuorder"));
            rn.qtypuorder = r.GetInt32(r.GetOrdinal("qtypuorder"));
            rn.qtyweightorder = r.GetDecimal(r.GetOrdinal("qtyweightorder"));
            rn.qtyvolumeorder = r.GetDecimal(r.GetOrdinal("qtyvolumeorder"));
            rn.qtyskuops = r.GetInt32(r.GetOrdinal("qtyskuops"));
            rn.qtypuops = r.GetInt32(r.GetOrdinal("qtypuops"));
            rn.qtyweightops = r.GetDecimal(r.GetOrdinal("qtyweightops"));
            rn.qtyvolumeops = r.GetDecimal(r.GetOrdinal("qtyvolumeops"));
            rn.batchno = r["batchno"].ToString();
            rn.lotno = r["lotno"].ToString();
            //rn.datemfg = (r.IsDBNull(32)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(32);
            rn.datemfg = (r.IsDBNull(r.GetOrdinal("datemfg"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("datemfg"));
            //rn.dateexp = (r.IsDBNull(33)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(33);
            rn.dateexp = (r.IsDBNull(r.GetOrdinal("dateexp"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("dateexp"));
            rn.serialno = r["serialno"].ToString();
            rn.picker = r["picker"].ToString();
            //rn.datepick = (r.IsDBNull(36)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(36);
            rn.datepick = (r.IsDBNull(r.GetOrdinal("datepick"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("datepick"));
            rn.devicecode = r["devicecode"].ToString();
            rn.tflow = r["tflow"].ToString();
            //rn.datecreate = (r.IsDBNull(39)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(39);
            rn.datecreate = (r.IsDBNull(r.GetOrdinal("datecreate"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("datecreate"));
            rn.accncreate = r["accncreate"].ToString();
            //rn.datemodify = (r.IsDBNull(41)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(41);
            rn.datemodify = (r.IsDBNull(r.GetOrdinal("datemodify"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("datemodify"));
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.description = r["descalt"].ToString(); //44 ,45
            //rn.rtoskuofpu = r.GetInt32(46);
            rn.rtoskuofpu = r.GetInt32(r.GetOrdinal("rtoskuofpu"));
            rn.thcode = r["thcode"].ToString();
            rn.taskno = r["taskno"].ToString();

            rn.daterec =  (r.IsDBNull(r.GetOrdinal("daterec"))) ? (DateTimeOffset?)null : r.GetDateTimeOffset(r.GetOrdinal("daterec"));
            rn.inagrn = r["inagrn"].ToString();
            rn.ingrno = r["ingrno"].ToString();
            rn.preptypeops = r["preptypeops"].ToString();
            rn.preplineops = r.GetInt32(r.GetOrdinal("preplineops"));
            return rn;
        }
        private SqlCommand prln_setix(prln_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand prln_setmd(prln_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.hunosource,"hunosource");
            cm.snapsPar(o.prepno,"prepno");
            cm.snapsPar(o.prepln,"prepln");
            cm.snapsPar(o.loczone,"loczone");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.locseq,"locseq");
            cm.snapsPar(o.locdigit,"locdigit");
            cm.snapsPar(o.ouorder,"ouorder");
            cm.snapsPar(o.ouln,"ouln");
            cm.snapsPar(o.barcode,"barcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(o.unitprep,"unitprep");
            cm.snapsPar(o.qtyskuorder,"qtyskuorder");
            cm.snapsPar(o.qtypuorder,"qtypuorder");
            cm.snapsPar(o.qtyweightorder,"qtyweightorder");
            cm.snapsPar(o.qtyvolumeorder,"qtyvolumeorder");
            cm.snapsPar(o.qtyskuops,"qtyskuops");
            cm.snapsPar(o.qtypuops,"qtypuops");
            cm.snapsPar(o.qtyweightops,"qtyweightops");
            cm.snapsPar(o.qtyvolumeops,"qtyvolumeops");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.picker,"picker");
            cm.snapsPar(o.datepick,"datepick");
            cm.snapsPar(o.devicecode,"devicecode");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsParsysdateoffset();
            return cm;
        }

        private SqlCommand prbc_setmd(prln_md o,String inOpstype,String hutype, String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(hutype,"hutype"); //hardcode
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.inorder,"inorder");
            cm.snapsPar(o.inln ,"inln");
            cm.snapsPar(o.ouorder,"ouorder");
            cm.snapsPar(o.ouln,"ouln");
            cm.snapsPar(inOpstype,"opstype");
            cm.snapsPar(o.prepno,"opsno");
            cm.snapsPar(o.prepln,"opsln");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.qtyskuorder,"rsvsku");
            cm.snapsPar(o.qtypuorder,"rsvpu");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.hunosource, "hunosource");
            cm.snapsParsysdateoffset();
            return cm;
        }
        private SqlCommand prsp_setmd(prln_md o,string setno,String sql) {
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(setno,"setno");
            return cm;
        }


        //For process order task  - prepset 
        public SqlCommand prepset_setcmd(prepset o){ 
            SqlCommand cm = new SqlCommand(sqlprepset_insert,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.setno,"setno");
            cm.snapsPar(o.datestart,"datestart");
            cm.snapsPar(o.datefinish,"datefinish");
            //cm.snapsPar(o.opsperform,"opsperform");
            cm.snapsPar(o.opsorder,"opsorder");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.procmodify,"procmodify");
            return cm;
        }
        public SqlCommand prepsln_setcmd(prepsln o) {
            SqlCommand cm = new SqlCommand(sqlprepset_insert,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.routeno,"routeno");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.ouorder,"ouorder");
            cm.snapsPar(o.ouln,"ouln");
            cm.snapsPar(o.barcode,"barcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.unitprep,"unitprep");
            cm.snapsPar(o.qtyskuorder,"qtyskuorder");
            cm.snapsPar(o.qtypuorder,"qtypuorder");
            cm.snapsPar(o.qtyweightorder,"qtyweightorder");
            cm.snapsPar(o.qtyvolumeorder,"qtyvolumeorder");
            cm.snapsPar(o.qtyskuops,"qtyskuops");
            cm.snapsPar(o.qtypuops,"qtypuops");
            cm.snapsPar(o.qtyweightops,"qtyweightops");
            cm.snapsPar(o.qtyvolumeops,"qtyvolumeops");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
        
            return cm;
        }

        public prepsln prepsln_fill(ref SqlDataReader r) { 
            prepsln rn = new prepsln();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.routeno = r["routeno"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.ouorder = r["ouorder"].ToString();
            rn.ouln = r["ouln"].ToString();
            rn.przone = r["przone"].ToString();
            rn.barcode = r["barcode"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = r.IsDBNull(r.GetOrdinal("pv")) ? 0 : r.GetInt32(r.GetOrdinal("pv"));//(r.IsDBNull(12)) ? 0 :  r.GetInt32(12);
            rn.lv = r.IsDBNull(r.GetOrdinal("lv")) ? 0 : r.GetInt32(r.GetOrdinal("lv"));//(r.IsDBNull(13)) ? 0 :  r.GetInt32(13);
            rn.unitprep = r.IsDBNull(r.GetOrdinal("unitprep")) ? 0 : r.GetInt32(r.GetOrdinal("unitprep"));//(r.IsDBNull(14)) ? 0 :  r.GetInt32(14);
            rn.qtyskuorder = r.IsDBNull(r.GetOrdinal("qtyskuorder")) ? 0 : r.GetInt32(r.GetOrdinal("qtyskuorder"));//(r.IsDBNull(15)) ? 0 :  r.GetInt32(15);
            rn.qtypuorder = r.IsDBNull(r.GetOrdinal("qtypuorder")) ? 0 : r.GetInt32(r.GetOrdinal("qtypuorder")); //(r.IsDBNull(16)) ? 0 :  r.GetInt32(16);
            rn.qtyweightorder = r.IsDBNull(r.GetOrdinal("qtyweightorder")) ? 0 : r.GetDecimal(r.GetOrdinal("qtyweightorder")); //(r.IsDBNull(17)) ? 0 : r.GetDecimal(17);
            rn.qtyvolumeorder = r.IsDBNull(r.GetOrdinal("qtyvolumeorder")) ? 0 : r.GetDecimal(r.GetOrdinal("qtyvolumeorder")); //(r.IsDBNull(18)) ? 0 : r.GetDecimal(18);
            rn.qtyskuops = r.IsDBNull(r.GetOrdinal("qtyskuops")) ? 0 : r.GetInt32(r.GetOrdinal("qtyskuops")); //(r.IsDBNull(19)) ? 0 :  r.GetInt32(19);
            rn.qtypuops = r.IsDBNull(r.GetOrdinal("qtypuops")) ? 0 : r.GetInt32(r.GetOrdinal("qtypuops")); //
            rn.qtyweightops = r.IsDBNull(r.GetOrdinal("qtyweightops")) ? 0 : r.GetDecimal(r.GetOrdinal("qtyweightops")); //(r.IsDBNull(21)) ? 0 : r.GetDecimal(21);
            rn.qtyvolumeops = r.IsDBNull(r.GetOrdinal("qtyvolumeops")) ? 0 : r.GetDecimal(r.GetOrdinal("qtyvolumeops")); //(r.IsDBNull(22)) ? 0 : r.GetDecimal(22);
            rn.batchno = r["batchno"].ToString();
            rn.lotno = r["lotno"].ToString();
            rn.datemfg = (r.IsDBNull(r.GetOrdinal("datemfg"))) ? (DateTimeOffset?) null : r.GetDateTimeOffset(r.GetOrdinal("datemfg"));
            rn.dateexp = (r.IsDBNull(r.GetOrdinal("dateexp"))) ? (DateTimeOffset?) null : r.GetDateTimeOffset(r.GetOrdinal("dateexp"));
            rn.serialno = r["serialno"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.errmsg = r["errmsg"].ToString();
            rn.description = r["descalt"].ToString();
            rn.loccode = r["loccode"].ToString();
            rn.dishuno = r["dishuno"].ToString();
            rn.disstockid = (r["disstockid"].ToString() == "") ? 0 : r["disstockid"].ToString().CInt32();
            rn.skuweight = r["skuweight"].ToString().CDecimal();
            rn.skuvolume = r["skuvolume"].ToString().CDecimal();
            return rn;
        }

        public prepsln prepsln_copy(prepsln ln, string unitprep,Int32 qtysku, Int32 qytpu, Decimal qtyweight, Decimal qtyvolume) { 
            prepsln rn = new prepsln();
            rn.site           = ln.site;
            rn.depot          = ln.depot;
            rn.spcarea        = ln.spcarea; 
            rn.routeno        = ln.routeno; 
            rn.thcode         = ln.thcode;
            rn.ouorder        = ln.ouorder; 
            rn.ouln           = ln.ouln;
            rn.przone         = ln.przone; 
            rn.barcode        = ln.barcode; 
            rn.article        = ln.article; 
            rn.pv             = ln.pv;
            rn.lv             = ln.lv;
            rn.unitprep       = ln.unitprep; 
            rn.qtyskuorder    = qtysku;
            rn.qtypuorder     = qytpu; 
            rn.qtyweightorder = qtyweight; 
            rn.qtyvolumeorder = qtyvolume;
            rn.qtyskuops      = 0; 
            rn.qtypuops       = 0; 
            rn.qtyweightops   = 0; 
            rn.qtyvolumeops   = 0;
            rn.batchno        = ln.batchno;
            rn.lotno          = ln.lotno;
            rn.datemfg        = ln.datemfg; 
            rn.dateexp        = ln.dateexp; 
            rn.serialno       = ln.serialno; 
            rn.tflow          = ln.tflow; 
            rn.errmsg         = ln.errmsg;
            rn.description    = ln.description; 
            rn.loccode        = ln.loccode;
            rn.dishuno        = ln.dishuno;
            rn.disstockid     = ln.disstockid;
            rn.skuweight      = ln.skuweight; 
            rn.skuvolume      = ln.skuvolume;      
            return rn;
        }

    }
}