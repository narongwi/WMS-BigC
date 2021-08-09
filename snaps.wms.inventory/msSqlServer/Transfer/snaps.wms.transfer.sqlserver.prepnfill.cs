using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public partial class transfer_ops : IDisposable { 
        private transfer_md fillmdl(ref DbDataReader r) { 
            return new transfer_md() {
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                dateops = (r.IsDBNull(3)) ? (DateTimeOffset?) null : r.GetDateTime(3),
                codeops = r["codeops"].ToString(),
                typeops = r["typeops"].ToString(),
                stockid = r.GetInt32(10),
                huno = r["huno"].ToString(),
                thcode = r["thcode"].ToString(),
                inrefno = r["inrefno"].ToString(),
                loccode = r["loccode"].ToString(),
                article = r["article"].ToString(),
                pv = r.GetInt32(13),
                lv = r.GetInt32(14),
                unitops = r["unitcor"].ToString(),
                qtysku = r.GetInt32(16),
                qtypu = r.GetDecimal(17),
                qtyweight = r.GetDecimal(18),
                qtyvolume = r.GetDecimal(19),
                daterec = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTime(20),
                batchno = r["batchno"].ToString(),
                lotno = r["lotno"].ToString(),
                datemfg = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTime(24),
                dateexp = (r.IsDBNull(25)) ? (DateTimeOffset?) null : r.GetDateTime(25),
                serialno = r["serialno"].ToString(),
                tflow = r["tflow"].ToString(),
                datecreate = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTime(28),
                accncreate = r["accncreate"].ToString()
            };
        }
        private SqlCommand transferCommand(transfer_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.dateops,"dateops");
            cm.snapsPar(o.accnops,"accnops");
            cm.snapsPar(o.seqops,"seqops");        
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.unitops,"unitops");
            cm.snapsPar(o.variancesku,"qtysku");
            cm.snapsPar(o.variancepu,"qtypu");
            cm.snapsPar(o.qtyweight,"qtyweight");
            cm.snapsPar(o.qtyvolume,"qtyvolume");
            cm.snapsPar(o.inreftype,"inreftype");
            cm.snapsPar(o.inrefno,"inrefno");
            cm.snapsPar(o.ingrno,"ingrno");
            cm.snapsPar(o.inpromo,"inpromo");
            cm.snapsPar(o.reason,"reason");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.sourcelocation,"sourcelocation");
            cm.snapsPar(o.targetlocation,"targetlocation");
            cm.snapsPar(o.daterec,"daterec");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar("","opstype");
            cm.snapsPar("","opscode");
            cm.snapsPar(o.rslhuno,"rslhuno");
            cm.snapsPar(o.rslstockid,"rslstockid");
            cm.snapsPar(o.rsltaskno,"rsltaskno");
            cm.snapsPar("","routeno");
            cm.snapsPar("","routethcode");
            cm.snapsParsysdateoffset();
            return cm;
        }

        private stock_mvin tomoveIn(transfer_md o) {             
            stock_mvin rn = new stock_mvin();
            rn.orgcode = o.orgcode;
            rn.site = o.site;
            rn.depot = o.depot;
            rn.spcarea = o.spcarea;
            rn.stockid = 0;//force to create a new stock id transactive I/O update the old one
            rn.article = o.article;
            rn.batchno = o.batchno;
            rn.dateexp = o.dateexp;
            rn.datemfg = o.datemfg;
            rn.daterec = o.daterec;
            rn.depot = o.depot;
            rn.lotno = o.lotno;
            rn.lv = o.lv;
            rn.opsaccn = o.accnops;
            rn.opsunit = o.unitops;
            rn.opssku = o.variancesku;
            rn.opspu = o.variancepu;
            rn.opsweight = o.qtyweight; 
            rn.opsvolume = o.qtyvolume;
            rn.opsnaturalloss = 0;
            rn.opsdate = DateTimeOffset.Now;
            rn.opstype = "";
            rn.opscode = "";
            rn.opsroute = "";
            rn.opsthcode = o.thcode;
            rn.stockhash = "";
            rn.opsaccn = o.accnops;
            rn.opshuno = o.rslhuno;
            rn.opsloccode = o.loccode;
            rn.opsrefno = o.seqops;
            rn.procmodify = o.procmodify;
            rn.batchno = o.batchno;
            rn.lotno = o.lotno;
            rn.serialno = o.serialno;
            rn.opshusource = o.huno;
            rn.inagrn = o.inagrn;
            rn.ingrno = o.ingrno;
            rn.stocktflow = o.stocktflow;
            return rn;
        }

        private stock_mvou tomoveOut(transfer_md o){
            stock_mvou rn = new stock_mvou();
            rn.orgcode = o.orgcode;
            rn.site = o.site;
            rn.depot = o.depot;
            rn.spcarea = o.spcarea;
            rn.stockid = o.stockid;
            rn.article = o.article;
            rn.batchno = o.batchno;
            rn.dateexp = o.dateexp;
            rn.datemfg = o.datemfg;
            rn.daterec = o.daterec;
            rn.depot = o.depot;
            rn.lotno = o.lotno;
            rn.lv = o.lv;
            rn.opsaccn = o.accnops;
            rn.opsunit = o.unitops;
            rn.opssku = o.variancesku;
            rn.opspu = o.variancepu;
            rn.opsweight = o.qtyweight; 
            rn.opsvolume = o.qtyvolume;
            rn.opsnaturalloss = 0;
            rn.opsdate = DateTimeOffset.Now;
            rn.opstype = "";
            rn.opscode = "";
            rn.opsroute = "";
            rn.opsthcode = o.thcode;
            rn.stockhash = "";
            rn.opsaccn = o.accnops;
            rn.opshuno = o.huno;
            rn.opsloccode = o.targetlocation;
            rn.opsrefno = o.seqops;
            rn.procmodify = o.procmodify;
            rn.batchno = o.batchno;
            rn.lotno = o.lotno;
            rn.serialno = o.serialno;
            rn.opshusource = "";
            rn.hutype = "";
            rn.inagrn = o.inagrn;
            rn.ingrno = o.ingrno;
            return rn;
        }

        private task_md gettaskmove(transfer_md o){
            task_md rn = new task_md();
            
            rn.orgcode = o.orgcode;
            rn.site = o.site;
            rn.depot = o.depot;
            rn.spcarea = o.spcarea;
            rn.accncreate = o.accncreate;
            rn.accnmodify = o.accncreate;
            rn.datecreate = DateTimeOffset.Now;
            rn.iopromo = "";
            rn.iorefno = o.inrefno;
            rn.lines = new List<taln_md>();
            rn.priority = "0" ;
            rn.taskdate = DateTimeOffset.Now;
            rn.tasktype = "T";
            rn.tflow = "IO";
            rn.taskname = "-Transfer";
            rn.procmodify ="wms.transfer";

            rn.lines.Add(new taln_md());
            rn.lines.Last().orgcode = o.orgcode;
            rn.lines.Last().site = o.site;
            rn.lines.Last().depot = o.depot;
            rn.lines.Last().spcarea = o.spcarea;
            rn.lines.Last().accncreate = o.accncreate;
            rn.lines.Last().accnmodify = o.accncreate;
            rn.lines.Last().article = o.article;
            rn.lines.Last().pv = o.pv;
            rn.lines.Last().lv = o.lv;
            rn.lines.Last().datecreate = DateTimeOffset.Now;
            rn.lines.Last().dateexp = o.dateexp;
            rn.lines.Last().datemfg = o.datemfg;
            rn.lines.Last().iopromo = o.inpromo;
            rn.lines.Last().iorefno = o.inrefno;
            rn.lines.Last().lotno = o.lotno;
            rn.lines.Last().serialno = o.serialno;
            rn.lines.Last().sourcehuno = o.rslhuno;
            //rn.lines.Last().targethuno = o.targethuno;
            rn.lines.Last().targetadv = o.targetlocation;
            rn.lines.Last().taskno = "";
            rn.lines.Last().taskseq = 1;
            rn.lines.Last().tflow = "IO";
            rn.lines.Last().sourceloc = o.sourcelocation;
            rn.lines.Last().sourceqty = o.variancesku;
            rn.lines.Last().sourcevolume = 0;//bug
            rn.lines.Last().stockid = o.stockid;
            rn.lines.Last().targetloc = o.targetlocation;
            rn.lines.Last().targethuno = o.rslhuno;
            rn.lines.Last().collectloc = "";
            rn.lines.Last().collecthuno = "";
            rn.lines.Last().accnassign = "";
            rn.lines.Last().accnwork = "";
            rn.lines.Last().accnfill = "";
            rn.lines.Last().accncollect = "";
            rn.lines.Last().ioreftype = "Transfer";
            rn.lines.Last().procmodify="wms.transfer";
            rn.lines.Last().iopromo = "";
            rn.lines.Last().targetqty = o.variancesku;
            rn.routeno = "";
            rn.routethcode = "";
            return rn;
        }
    }
}