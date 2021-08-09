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

    public partial class stock_ops : IDisposable { 

        private stock_ls fillls(ref SqlDataReader r) { 
            stock_ls rn = new stock_ls();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = r["pv"].ToString().CInt32();
            rn.lv = r["lv"].ToString().CInt32();
            rn.description = r["description"].ToString();
            rn.cronhand = r.GetDecimal(11);
            rn.tflow = r["tflow"].ToString();
            rn.unitmanage = r["unitmanage"].ToString();            
            rn.unitratio = r.GetInt32(14);
            rn.cronhandpu = (rn.cronhand == 0) ? 0 : rn.cronhand / rn.unitratio;
            rn.dlcall = r["dlcall"].ToString().CInt32();
            rn.dlcwarehouse = r["dlcwarehouse"].ToString().CInt32();
            rn.dlcfactory = r["dlcfactory"].ToString().CInt32();
            return rn;
        }
        private stock_ix fillix(ref SqlDataReader r) { 
            return new stock_ix() { 

            };
        }
        private stock_md fillmdl(ref SqlDataReader r) { 
            stock_md rn = new stock_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.stockid = (r.IsDBNull(4)) ? 0 : r.GetDecimal(4);
            rn.hutype = r["hutype"].ToString();
            rn.huno = r["huno"].ToString();
            rn.hunosource = r["hunosource"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.inrefno = r["inrefno"].ToString();
            rn.inrefln = r["inrefln"].ToString(); 
            rn.loccode = r["loccode"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = (r.IsDBNull(13)) ? 0 : r.GetInt32(13);
            rn.lv = (r.IsDBNull(14)) ? 0 : r.GetInt32(14);
            rn.qtysku = (r.IsDBNull(15)) ? 0 :  r.GetInt32(15);
            rn.qtypu = (r.IsDBNull(16)) ? 0 : r.GetDecimal(16);
            rn.qtyweight = (r.IsDBNull(17)) ? 0 : r.GetDecimal(17);
            rn.qtyvolume =(r.IsDBNull(18)) ? 0 : r.GetDecimal(18);
            rn.daterec = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19);
            rn.batchno = r["batchno"].ToString();
            rn.lotno = r["lotno"].ToString();
            rn.datemfg = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22);
            rn.dateexp = (r.IsDBNull(23)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(23);
            rn.serialno = r["serialno"].ToString();
            rn.stkremarks = r["stkremarks"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(27)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(27);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(29)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(29);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.tflowsign = r["tflowsign"].ToString();
            rn.inagrn = r["inagrn"].ToString();
            rn.ingrno = r["ingrno"].ToString();
            rn.unitmanage = r["unitops"].ToString();
            return rn;
        }
        private SqlCommand ixcommand(stock_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand obcommand(stock_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(o.hutype,"hutype");
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.hunosource,"hunosource");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.inrefno,"inrefno");
            cm.snapsPar(o.inrefln,"inrefln");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.qtysku,"qtysku");
            cm.snapsPar(o.qtypu,"qtypu");
            cm.snapsPar(o.qtyweight,"qtyweight");
            cm.snapsPar(o.qtyvolume,"qtyvolume");
            cm.snapsPar(o.daterec,"daterec");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.stkremarks,"stkremarks");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsParsysdateoffset();
            return cm;
        }
        public SqlCommand setMVPams(string cmd, stock_mvou o){
            SqlCommand m;
            m = cmd.snapsCommand(cn);
            m.snapsPar(o.orgcode,"orgcode");
            m.snapsPar(o.site,"site");
            m.snapsPar(o.depot,"depot");
            m.snapsPar(o.spcarea,"spcarea");
            m.snapsPar(o.stockid,"stockid");
            m.snapsPar(o.article,"article");
            m.snapsPar(o.pv,"pv");
            m.snapsPar(o.lv,"lv");
            m.snapsPar(o.opsunit,"opsunit");
            m.snapsPar(o.opssku,"opssku");
            m.snapsPar(o.opspu,"opspu");
            m.snapsPar(o.opsweight,"opsweight");
            m.snapsPar(o.opsvolume,"opsvolume");
            m.snapsPar(o.opstype,"opstype");
            m.snapsPar(o.opscode,"opscode");
            m.snapsPar(o.opsroute,"opsroute");
            m.snapsPar(o.opsthcode,"opsthcode");
            m.snapsPar(o.opsaccn,"opsaccn");
            m.snapsPar(o.opsrefno,"opsrefno");
            m.snapsPar(o.procmodify,"procmodify");

            m.snapsPar(o.hutype,"hutype");
            m.snapsPar(o.opshuno,"opshuno");
            m.snapsPar(0,"inrefln");
            m.snapsPar(o.opsloccode,"opsloccode");
            m.snapsPar(o.daterec,"daterec");
            m.snapsPar(o.batchno,"batchno");
            m.snapsPar(o.lotno,"lotno");
            m.snapsPar(o.datemfg,"datemfg");
            m.snapsPar(o.dateexp,"dateexp");
            m.snapsPar(o.serialno,"serialno");
            m.snapsPar("","stkremarks");
            m.snapsPar("IO","tflow");
            m.snapsPar(o.opshusource,"opshusource");
            
            m.snapsPar(o.opsreftype,"opsreftype");

            return m;
        }

        public SqlCommand setMVPams(string cmd, stock_mvin o){
            SqlCommand m;
            m = cmd.snapsCommand(cn);
            m.snapsPar(o.orgcode,"orgcode");
            m.snapsPar(o.site,"site");
            m.snapsPar(o.depot,"depot");
            m.snapsPar(o.spcarea,"spcarea");
            m.snapsPar(o.stockid,"stockid");
            m.snapsPar(o.article,"article");
            m.snapsPar(o.pv,"pv");
            m.snapsPar(o.lv,"lv");
            m.snapsPar(o.opsunit,"opsunit");
            m.snapsPar(o.opssku,"opssku");
            m.snapsPar(o.opspu,"opspu");
            m.snapsPar(o.opsweight,"opsweight");
            m.snapsPar(o.opsvolume,"opsvolume");
            m.snapsPar(o.opstype,"opstype");
            m.snapsPar(o.opscode,"opscode");
            m.snapsPar(o.opsroute,"opsroute");
            m.snapsPar(o.opsthcode,"opsthcode");
            m.snapsPar(o.opsaccn,"opsaccn");
            m.snapsPar(o.opsrefno,"opsrefno");
            m.snapsPar(o.procmodify,"procmodify");

            m.snapsPar(o.hutype,"hutype");
            m.snapsPar(o.opshuno,"opshuno");
            m.snapsPar(0,"inrefln");
            m.snapsPar(o.opsloccode,"opsloccode");
            m.snapsPar(o.daterec,"daterec");
            m.snapsPar(o.batchno,"batchno");
            m.snapsPar(o.lotno,"lotno");
            m.snapsPar(o.datemfg,"datemfg");
            m.snapsPar(o.dateexp,"dateexp");
            m.snapsPar(o.serialno,"serialno");
            m.snapsPar("","stkremarks");
            m.snapsPar(o.tflow,"tflow");
            m.snapsPar(o.opshusource,"opshusource");

            m.snapsPar(o.inagrn,"inagrn");
            m.snapsPar(o.ingrno,"ingrno");
            m.snapsPar(o.opsreftype,"opsreftype");
            m.snapsPar(o.opsunit,"unitops");
            return m;
        }

    }
}