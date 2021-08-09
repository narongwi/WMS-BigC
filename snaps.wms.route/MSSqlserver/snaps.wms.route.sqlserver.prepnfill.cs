using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using System.Runtime.CompilerServices;

namespace Snaps.WMS {
    public partial class route_ops : IDisposable { 
        private route_thsum fillthsum(ref SqlDataReader r) { 
            route_thsum rn = new route_thsum();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.thname =  r["thname"].ToString();
            rn.crroute = (r.IsDBNull(5)) ? 0 :  r.GetInt32(5);
            rn.crhu = (r.IsDBNull(6)) ? 0 :  r.GetInt32(6);
            rn.crweight =  (r.IsDBNull(7)) ? 0 : r.GetDecimal(7);
            rn.crvolume =  (r.IsDBNull(8)) ? 0 : r.GetDecimal(8);
            rn.crcapacity =  (r.IsDBNull(9)) ? 0 : r.GetDecimal(9);
            rn.crophu  =  (r.IsDBNull(10)) ? 0 : r.GetDecimal(10);
            return rn;
        }
        private route_ls fillls(ref SqlDataReader r) {  //GetDateTimeOffset
            route_ls rn = new route_ls();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.routetype = r["routetype"].ToString();
            rn.routeno = r["routeno"].ToString();
            rn.oupromo = r["oupromo"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.plandate = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(16);
            rn.utlzcode = r["utlzcode"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.driver = r["driver"].ToString();
            rn.thname =  r["thname"].ToString();
            rn.oupriority = r.GetInt32(15);
            rn.datereqdelivery = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19);
            rn.crhu = (r.IsDBNull(26)) ? 0 :  r.GetInt32(26);
            rn.crweight =  (r.IsDBNull(27)) ? 0 : r.GetDecimal(27);
            rn.crvolume =  (r.IsDBNull(28)) ? 0 : r.GetDecimal(28);
            rn.crcapacity =  (r.IsDBNull(43)) ? 0 : r.GetDecimal(43);
            rn.crophu  =  (r.IsDBNull(44)) ? 0 : r.GetDecimal(44);
            rn.remarks = r["remarks"].ToString();
            return rn;
        }
        private route_ix fillix(ref SqlDataReader r) { 
            return new route_ix() { 

            };
        }
        private route_md fillmdl(ref SqlDataReader r) { 
            route_md rn = new route_md() ;
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.routetype = r["routetype"].ToString();
            rn.routeno = r["routeno"].ToString();
            rn.routename = r["routename"].ToString();
            rn.oupromo = r["oupromo"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.trttype = r["trttype"].ToString();
            rn.loadtype = r["loadtype"].ToString();
            rn.trucktype = r["trucktype"].ToString();
            rn.trtmode = r["trtmode"].ToString();
            rn.loccode = r["loccode"].ToString();
            rn.paytype = r["paytype"].ToString();
            rn.oupriority = (r.IsDBNull(15)) ? 0 :  r.GetInt32(15);
            rn.plandate = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(16);
            rn.loaddate = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17);
            rn.dateshipment = (r.IsDBNull(18)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(18);
            rn.datereqdelivery = (r.IsDBNull(19)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19);
            rn.relocationto = r["relocationto"].ToString();
            rn.relocationtask = r["relocationtask"].ToString();
            rn.shipper = r["shipper"].ToString();
            rn.mxhu = (r.IsDBNull(23)) ? 0 :  r.GetInt32(23);
            rn.mxweight =  (r.IsDBNull(24)) ? 0 : r.GetDecimal(24);
            rn.mxvolume =  (r.IsDBNull(25)) ? 0 : r.GetDecimal(25);
            rn.crhu = (r.IsDBNull(26)) ? 0 :  r.GetInt32(26);
            rn.crweight =  (r.IsDBNull(27)) ? 0 : r.GetDecimal(27);
            rn.crvolume =  (r.IsDBNull(28)) ? 0 : r.GetDecimal(28);
            rn.driver = r["driver"].ToString();
            rn.plateNo = r["plateNo"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.contactno = r["contactno"].ToString();
            rn.utlzcode = r["utlzcode"].ToString();
            rn.datecreate = (r.IsDBNull(34)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(34);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(36)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(36);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.transportor = r["transportor"].ToString();
            rn.remarks = r["remarks"].ToString();
            rn.outrno = r["outrno"].ToString();
            rn.sealno = r["sealno"].ToString();
            return rn;
        }
        private SqlCommand ixcommand(route_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand obcommand(route_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.routetype,"routetype");
            cm.snapsPar(o.routeno,"routeno");
            cm.snapsPar(o.routename,"routename");
            cm.snapsPar(o.oupromo,"oupromo");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.trttype,"trttype");
            cm.snapsPar(o.loadtype,"loadtype");
            cm.snapsPar(o.trucktype,"trucktype");
            cm.snapsPar(o.trtmode,"trtmode");
            cm.snapsPar(o.loccode,"loccode");
            cm.snapsPar(o.paytype,"paytype");
            cm.snapsPar(o.oupriority,"oupriority");
            cm.snapsPar(o.plandate,"plandate");
            cm.snapsPar(o.loaddate,"loaddate");
            cm.snapsPar(o.dateshipment,"dateshipment");
            cm.snapsPar(o.datereqdelivery,"datereqdelivery");
            cm.snapsPar(o.relocationto,"relocationto");
            cm.snapsPar(o.relocationtask,"relocationtask");
            cm.snapsPar(o.shipper,"shipper");
            cm.snapsPar(o.mxhu,"mxhu");
            cm.snapsPar(o.mxweight,"mxweight");
            cm.snapsPar(o.mxvolume,"mxvolume");
            cm.snapsPar(o.crhu,"crhu");
            cm.snapsPar(o.crweight,"crweight");
            cm.snapsPar(o.crvolume,"crvolume");
            cm.snapsPar(o.driver,"driver");
            cm.snapsPar(o.plateNo,"plateNo");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.contactno,"contactno");
            cm.snapsPar(o.utlzcode,"utlzcode");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.transportor,"transportor");
            cm.snapsPar(o.remarks,"remarks");
            cm.snapsPar(o.sealno,"sealno");
            return cm;
        }

        private SqlCommand lxcommand(outboulx_md o,string command) { 
            SqlCommand cm = new SqlCommand(command,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.ouorder,"ouorder");
            cm.snapsPar(o.ouln,"ouln");
            cm.snapsPar(o.ourefno,"ourefno");
            cm.snapsPar(o.ourefln,"ourefln");
            cm.snapsPar(o.ouseq,"ouseq");
            cm.snapsPar(o.inorder,"inorder");
            cm.snapsPar(o.barcode,"barcode");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.unitops,"unitops");
            cm.snapsPar(o.opssku,"opssku");
            cm.snapsPar(o.opspu,"opspu");
            cm.snapsPar(o.opsweight,"opsweight");
            cm.snapsPar(o.opsvolume,"opsvolume");
            cm.snapsPar(o.oudono,"oudono");
            cm.snapsPar(o.oudnno,"oudnno");
            cm.snapsPar(o.outrno,"outrno");
            cm.snapsPar(o.batchno,"batchno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.routenodel,"routenodel");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(o.opshuno,"opshuno");
            cm.snapsPar(o.opshusource,"opshusource");
            cm.snapsPar(o.datedel,"datedel");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.loccode,"loccode");
            return cm;
        }

    }
}