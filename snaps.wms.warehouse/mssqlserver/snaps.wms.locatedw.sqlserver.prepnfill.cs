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

namespace Snaps.WMS.warehouse {

    public partial class locdw_ops : IDisposable { 
        private locdw_ls fillls(ref SqlDataReader r) { 
            locdw_ls rn = new locdw_ls();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.fltype = r["fltype"].ToString();
            rn.lszone = r["lszone"].ToString();
            rn.lsaisle = r["lsaisle"].ToString();
            rn.lsbay = r["lsbay"].ToString();
            rn.lslevel = r["lslevel"].ToString();
            rn.lsloc = r["lsloc"].ToString();
            rn.lsstack = r["lsstack"].ToString();
            rn.lscode = r["lscode"].ToString();
            rn.lscodealt = r["lscodealt"].ToString();
            rn.crweight = (r.IsDBNull(49)) ? 0 :  r.GetDecimal(49);
            rn.crvolume = (r.IsDBNull(50)) ? 0 :  r.GetDecimal(50);
            rn.crfreepct = (r.IsDBNull(51)) ? 0 :  r.GetDecimal(51);
            rn.tflow = r["tflow"].ToString();
            rn.spcpicking = (r.IsDBNull(44)) ? 0 :  r.GetInt32(44);
            return rn;
        }
        private locdw_pivot fillpivotls(ref SqlDataReader r) { 
            locdw_pivot rn = new locdw_pivot();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.fltype = r["fltype"].ToString();
            rn.lszone = r["lszone"].ToString();
            rn.lsaisle = r["lsaisle"].ToString();
            rn.lsbay = r["lsbay"].ToString();
            rn.lslevel = r["lslevel"].ToString();
            rn.lsloc = r["lsloc"].ToString();
            rn.lsstack = r["lsstack"].ToString();
            rn.lscode = r["lscode"].ToString();
            rn.lscodealt = r["lscodealt"].ToString();
            rn.crweight = (r.IsDBNull(49)) ? 0 :  r.GetDecimal(49);
            rn.crvolume = (r.IsDBNull(50)) ? 0 :  r.GetDecimal(50);
            rn.crfreepct = (r.IsDBNull(51)) ? 0 :  r.GetDecimal(51);
            rn.tflow = r["tflow"].ToString();
            rn.spcpicking = (r.IsDBNull(44)) ? 0 :  r.GetInt32(44);
            rn.spcpivot = r["spcpivot"].ToString();
            return rn;
        }
        private locdw_picking fillpickingls(ref SqlDataReader r) { 
            locdw_picking rn = new locdw_picking();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.fltype = r["fltype"].ToString();
            rn.lszone = r["lszone"].ToString();
            rn.lsaisle = r["lsaisle"].ToString();
            rn.lsbay = r["lsbay"].ToString();
            rn.lslevel = r["lslevel"].ToString();
            rn.lsloc = r["lsloc"].ToString();
            rn.lsstack = r["lsstack"].ToString();
            rn.lscode = r["lscode"].ToString();
            rn.lscodealt = r["lscodealt"].ToString();
            rn.crweight = (r.IsDBNull(49)) ? 0 :  r.GetDecimal(49);
            rn.crvolume = (r.IsDBNull(50)) ? 0 :  r.GetDecimal(50);
            rn.crfreepct = (r.IsDBNull(51)) ? 0 :  r.GetDecimal(51);
            rn.tflow = r["tflow"].ToString();
            rn.spcpicking = (r.IsDBNull(44)) ? 0 :  r.GetInt32(44);
            rn.spcseqpath = (r.IsDBNull(41)) ? 0 :  r.GetInt32(41); 
            rn.spcpickunit = r["spcpickunit"].ToString();
            rn.spcrpn = r["spcrpn"].ToString();            
            rn.lsmnsafety = (r.IsDBNull(24)) ? 0 :  r.GetInt32(24);
            rn.spcarticle = r["spcarticle"].ToString();
            return rn;
        }
        private locdw_ix fillix(ref SqlDataReader r) { 
            return new locdw_ix() { 

            };
        }
        private locdw_md fillmdl(ref SqlDataReader r) { 
            locdw_md rn = new locdw_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.fltype = r["fltype"].ToString();
            rn.lszone = r["lszone"].ToString();
            rn.lsaisle = r["lsaisle"].ToString();
            rn.lsbay = r["lsbay"].ToString();
            rn.lslevel = r["lslevel"].ToString();
            rn.lsloc = r["lsloc"].ToString();
            rn.lsstack = r["lsstack"].ToString();
            rn.lscode = r["lscode"].ToString();
            rn.lscodealt = r["lscodealt"].ToString();
            rn.lscodefull = r["lscodefull"].ToString();
            rn.lscodeid = r["lscodeid"].ToString();
            rn.lsdmlength =  (r.IsDBNull(15)) ? 0 : r.GetDecimal(15);
            rn.lsdmwidth =  (r.IsDBNull(16)) ? 0 : r.GetDecimal(16);
            rn.lsdmheight =  (r.IsDBNull(17)) ? 0 : r.GetDecimal(17);
            rn.lsmxweight =  (r.IsDBNull(18)) ? 0 : r.GetDecimal(18);
            rn.lsmxvolume =  (r.IsDBNull(19)) ? 0 : r.GetDecimal(19);
            rn.lsmxlength =  (r.IsDBNull(20)) ? 0 : r.GetDecimal(20);
            rn.lsmxwidth =  (r.IsDBNull(21)) ? 0 : r.GetDecimal(21);
            rn.lsmxheight =  (r.IsDBNull(22)) ? 0 : r.GetDecimal(22);
            rn.lsmxhuno = (r.IsDBNull(23)) ? 0 :  r.GetInt32(23);
            rn.lsmnsafety = (r.IsDBNull(24)) ? 0 :  r.GetInt32(24);
            rn.lsmixarticle = (r.IsDBNull(25)) ? 0 :  r.GetInt32(25);
            rn.lsmixage = (r.IsDBNull(26)) ? 0 :  r.GetInt32(26);
            rn.lsmixlotno = (r.IsDBNull(27)) ? 0 :  r.GetInt32(27);
            rn.lsloctype = r["lsloctype"].ToString();
            rn.lsremarks = r["lsremarks"].ToString();
            rn.lsgaptop =  (r.IsDBNull(30)) ? 0 : r.GetDecimal(30);
            rn.lsgapleft =  (r.IsDBNull(31)) ? 0 : r.GetDecimal(31);
            rn.lsgapright =  (r.IsDBNull(32)) ? 0 : r.GetDecimal(32);
            rn.lsgapbuttom =  (r.IsDBNull(33)) ? 0 : r.GetDecimal(33);
            rn.lsstackable = (r.IsDBNull(34)) ? 0 :  r.GetInt32(34);
            rn.lsdigit = r["lsdigit"].ToString();
            rn.spcthcode = r["spcthcode"].ToString();
            rn.spchuno = r["spchuno"].ToString();
            rn.spcarticle = r["spcarticle"].ToString();
            rn.spcblock = (r.IsDBNull(39)) ? 0 :  r.GetInt32(39);
            rn.spctaskfnd = r["spctaskfnd"].ToString();
            rn.spcseqpath = (r.IsDBNull(41)) ? 0 :  r.GetInt32(41);
            rn.spclasttouch = r["spclasttouch"].ToString();
            rn.spcpivot = r["spcpivot"].ToString();
            rn.spcpicking = (r.IsDBNull(44)) ? 0 :  r.GetInt32(44);
            rn.spcpickunit = r["spcpickunit"].ToString();
            rn.spcrpn = r["spcrpn"].ToString();
            rn.spcmnaging = (r.IsDBNull(47)) ? 0 :  r.GetInt32(47);
            rn.spcmxaging = (r.IsDBNull(48)) ? 0 :  r.GetInt32(48);
            rn.crweight =  (r.IsDBNull(49)) ? 0 : r.GetDecimal(49);
            rn.crvolume =  (r.IsDBNull(50)) ? 0 : r.GetDecimal(50);
            rn.crfreepct =  (r.IsDBNull(51)) ? 0 : r.GetDecimal(51);
            rn.tflow = r["tflow"].ToString();
            rn.tflowcnt = r["tflowcnt"].ToString();
            rn.lshash = r["lshash"].ToString();
            rn.datecreate = (r.IsDBNull(55)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(55);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(57)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(57);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.crhu = (r.IsDBNull(60)) ? 0 :  r.GetInt32(60);
            rn.lsdirection = r["lsdirection"].ToString();
            rn.spcpathrecv = (r.IsDBNull(62)) ? 0 :  r.GetInt32(62);
            rn.spcpathpick = (r.IsDBNull(63)) ? 0 :  r.GetInt32(63);
            rn.spcpathdist = (r.IsDBNull(64)) ? 0 :  r.GetInt32(64);
            return rn;
        }
        private SqlCommand ixcommand(locdw_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand  obcommand(locdw_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.orgcode.snapsPar("orgcode" ));
            cm.Parameters.Add(o.site.snapsPar("site" ));
            cm.Parameters.Add(o.depot.snapsPar("depot" ));
            cm.Parameters.Add(o.spcarea.snapsPar("spcarea" ));
            cm.Parameters.Add(o.fltype.snapsPar("fltype" ));
            cm.Parameters.Add(o.lszone.snapsPar("lszone" ));
            cm.Parameters.Add(o.lsaisle.snapsPar("lsaisle" ));
            cm.Parameters.Add(o.lsbay.snapsPar("lsbay" ));
            cm.Parameters.Add(o.lslevel.snapsPar("lslevel" ));
            cm.Parameters.Add(o.lsloc.snapsPar("lsloc"));
            cm.Parameters.Add(o.lsstack.snapsPar("lsstack" ));
            cm.Parameters.Add(o.lscode.snapsPar("lscode" ));
            cm.Parameters.Add(o.lscodealt.snapsPar("lscodealt" ));
            cm.Parameters.Add(o.lscodefull.snapsPar("lscodefull" ));
            cm.Parameters.Add(o.lscodeid.snapsPar("lscodeid"));
            cm.Parameters.Add(o.lsdmlength.snapsPar("lsdmlength" ));
            cm.Parameters.Add(o.lsdmwidth.snapsPar("lsdmwidth" ));
            cm.Parameters.Add(o.lsdmheight.snapsPar("lsdmheight" ));
            cm.Parameters.Add(o.lsmxweight.snapsPar("lsmxweight" ));
            cm.Parameters.Add(o.lsmxvolume.snapsPar("lsmxvolume" ));
            cm.Parameters.Add(o.lsmxlength.snapsPar("lsmxlength" ));
            cm.Parameters.Add(o.lsmxwidth.snapsPar("lsmxwidth" ));
            cm.Parameters.Add(o.lsmxheight.snapsPar("lsmxheight" ));
            cm.Parameters.Add(o.lsmxhuno.snapsPar("lsmxhuno" ));
            cm.Parameters.Add(o.lsmnsafety.snapsPar("lsmnsafety" ));
            cm.Parameters.Add(o.lsmixarticle.snapsPar("lsmixarticle" ));
            cm.Parameters.Add(o.lsmixage.snapsPar("lsmixage" ));
            cm.Parameters.Add(o.lsmixlotno.snapsPar("lsmixlotno" ));
            cm.Parameters.Add(o.lsloctype.snapsPar("lsloctype" ));
            cm.Parameters.Add(o.lsremarks.snapsPar("lsremarks" ));
            cm.Parameters.Add(o.lsgaptop.snapsPar("lsgaptop" ));
            cm.Parameters.Add(o.lsgapleft.snapsPar("lsgapleft" ));
            cm.Parameters.Add(o.lsgapright.snapsPar("lsgapright" ));
            cm.Parameters.Add(o.lsgapbuttom.snapsPar("lsgapbuttom" ));
            cm.Parameters.Add(o.lsstackable.snapsPar("lsstackable" ));
            cm.Parameters.Add(o.lsdigit.snapsPar("lsdigit" ));
            cm.Parameters.Add(o.spcthcode.snapsPar("spcthcode" ));
            cm.Parameters.Add(o.spchuno.snapsPar("spchuno" ));
            cm.Parameters.Add(o.spcarticle.snapsPar("spcarticle" ));
            cm.Parameters.Add(o.spcblock.snapsPar("spcblock" ));
            cm.Parameters.Add(o.spctaskfnd.snapsPar("spctaskfnd" ));
            cm.Parameters.Add(o.spcseqpath.snapsPar("spcseqpath" ));
            cm.Parameters.Add(o.spclasttouch.snapsPar("spclasttouch" ));
            cm.Parameters.Add(o.spcpivot.snapsPar("spcpivot" ));
            cm.Parameters.Add(o.spcpicking.snapsPar("spcpicking" ));
            cm.Parameters.Add(o.spcpickunit.snapsPar("spcpickunit" ));
            cm.Parameters.Add(o.spcrpn.snapsPar("spcrpn" ));
            cm.Parameters.Add(o.spcmnaging.snapsPar("spcmnaging" ));
            cm.Parameters.Add(o.spcmxaging.snapsPar("spcmxaging" ));
            cm.Parameters.Add(o.crweight.snapsPar("crweight" ));
            cm.Parameters.Add(o.crvolume.snapsPar("crvolume" ));
            cm.Parameters.Add(o.crfreepct.snapsPar("crfreepct" ));
            cm.Parameters.Add(o.tflow.snapsPar("tflow" ));
            cm.Parameters.Add(o.tflowcnt.snapsPar("tflowcnt" ));
            cm.Parameters.Add(o.lshash.snapsPar("lshash" ));            
            cm.Parameters.Add(o.accncreate.snapsPar("accncreate" ));
            cm.Parameters.Add(o.accnmodify.snapsPar("accnmodify" ));
            cm.snapsParsysdateoffset();
            return cm;
        }

        public SqlCommand oucommand(locdw_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar(nameof(o.site)));
            cm.Parameters.Add(o.depot.snapsPar(nameof(o.depot)));
            cm.Parameters.Add(o.spcarea.snapsPar(nameof(o.spcarea)));
            cm.Parameters.Add(o.fltype.snapsPar(nameof(o.fltype)));
            cm.Parameters.Add(o.lscode.snapsPar(nameof(o.lscode)));
            return cm;
        }
        public SqlCommand oucommand(locdw_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar(nameof(o.site)));
            cm.Parameters.Add(o.depot.snapsPar(nameof(o.depot)));
            cm.Parameters.Add(o.spcarea.snapsPar(nameof(o.spcarea)));
            cm.Parameters.Add(o.fltype.snapsPar(nameof(o.fltype)));
            cm.Parameters.Add(o.lscode.snapsPar(nameof(o.lscode)));
            return cm;
        }


    }
}