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
    public partial class correction_ops : IDisposable { 

        private correction_ls fillls(ref DbDataReader r) { 
            return new correction_ls() { 
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                dateops = (r.IsDBNull(3)) ? (DateTimeOffset?) null : r.GetDateTime(3),
                codeops = r["codeops"].ToString(),
                typeops = r["typeops"].ToString(),
                article = r["article"].ToString(),
                unitops = r["unitcor"].ToString(),
                qtypu = r.GetInt32(8),
                tflow = r["tflow"].ToString(),
                description =  r["description"].ToString(),
            };
        }
        private correction_ix fillix(ref DbDataReader r) { 
            return new correction_ix() { 

            };
        }
        private correction_md fillmdl(ref DbDataReader r) { 
            return new correction_md() {
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
                qtypu = r.GetInt32(17),
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
        private SqlCommand ixcommand(correction_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand correctCommand(correction_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.dateops,"dateops");
            cm.snapsPar(o.accnops,"accnops");
            cm.snapsPar(o.seqops,"seqops");
            cm.snapsPar(o.codeops,"codeops");
            cm.snapsPar(o.typeops,"typeops");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.unitops,"unitops");
            cm.snapsPar(Math.Abs(o.variancesku),"qtysku");
            cm.snapsPar(Math.Abs(o.variancepu),"qtypu");
            cm.snapsPar(Math.Abs(o.qtyweight),"qtyweight");
            cm.snapsPar(Math.Abs(o.qtyvolume),"qtyvolume");
            cm.snapsPar(o.inreftype,"inreftype");
            cm.snapsPar(o.inrefno,"inrefno");
            cm.snapsPar(o.ingrno,"ingrno");
            cm.snapsPar(o.inpromo,"inpromo");
            cm.snapsPar(o.reason,"reason");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.daterec,"daterec");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.orbitsource,"orbitsource");
            cm.snapsPar(o.orbitsite,"orbitsite");
            cm.snapsPar(o.orbitdepot,"orbitdepot");
            cm.snapsPar(o.inagrn,"inagrn");
            return cm;
        }

        private SqlCommand orbitCommand(correction_md o,string mapcode) {
            SqlCommand cm = new SqlCommand(sqlcorrect_sendtorbit,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.dateops,"dateops");
            cm.snapsPar(o.accnops,"accnops");
            cm.snapsPar(o.seqops,"seqops");
            cm.snapsPar(mapcode,"codeops");
            cm.snapsPar(o.typeops,"typeops");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.unitops,"unitops");
            if (o.typeops == "-") { 
                cm.snapsPar(("-"+o.qtysku.ToString().Replace("-","")).CInt32(),"qtysku");
                cm.snapsPar(("-"+o.qtypu.ToString().Replace("-","")).CInt32(),"qtypu");
                cm.snapsPar(("-"+o.qtyweight.ToString().Replace("-","")).CDecimal(),"qtyweight");
                cm.snapsPar(("-"+o.qtyvolume.ToString().Replace("-","")).CDecimal(),"qtyvolume");
            }else { 
                cm.snapsPar(Math.Abs(o.qtysku),"qtysku");
                cm.snapsPar(Math.Abs(o.qtypu),"qtypu");
                cm.snapsPar(Math.Abs(o.qtyweight),"qtyweight");
                cm.snapsPar(Math.Abs(o.qtyvolume),"qtyvolume");
            }            
            cm.snapsPar(o.inreftype,"inreftype");
            cm.snapsPar(o.inrefno,"inrefno");
            cm.snapsPar(o.ingrno,"ingrno");
            cm.snapsPar(o.inpromo,"inpromo");
            cm.snapsPar(o.reason,"reason");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(o.huno,"huno");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.daterec,"daterec");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.orbitsource,"orbitsource");
            cm.snapsPar(o.orbitsite,"orbitsite");
            cm.snapsPar(o.orbitdepot,"orbitdepot");
            cm.snapsPar(o.inagrn,"inagrn");
            return cm;
        }

        private stock_mvin tomoveIn(correction_md o) {             
            stock_mvin rn = new stock_mvin();
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
            rn.opstype = o.typeops;
            rn.opscode = o.codeops;
            rn.opsroute = "";
            rn.opsthcode = o.thcode;
            rn.stockhash = "";
            rn.opsaccn = o.accnops;
            rn.opshuno = o.huno;
            rn.opsloccode = o.loccode;
            rn.opsrefno = o.seqops;
            rn.procmodify = o.procmodify;
            rn.batchno = o.batchno;
            rn.lotno = o.lotno;
            rn.serialno = o.serialno;
            rn.opshusource = "";
            rn.inagrn = o.inagrn;
            rn.ingrno = o.ingrno;
            return rn;
        }

        private stock_mvou tomoveOut(correction_md o) {             
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
            rn.opssku = Math.Abs(o.qtysku);
            rn.opspu = Math.Abs(o.qtypu);
            rn.opsweight = Math.Abs(o.qtyweight); 
            rn.opsvolume = Math.Abs(o.qtyvolume);
            rn.opsnaturalloss = 0;
            rn.opsdate = DateTimeOffset.Now;
            rn.opstype = o.typeops;
            rn.opscode = o.codeops;
            rn.opsroute = "";
            rn.opsthcode = o.thcode;
            rn.stockhash = "";
            rn.opsaccn = o.accnops;
            rn.opshuno = o.huno;
            rn.opsloccode = o.loccode;
            rn.opsrefno = o.seqops;
            rn.procmodify = o.procmodify;
            rn.batchno = o.batchno;
            rn.lotno = o.lotno;
            rn.serialno = o.serialno;
            rn.opshusource = "";
            rn.inagrn = o.inagrn;
            rn.ingrno = o.ingrno;
            return rn;
        }
    }
}