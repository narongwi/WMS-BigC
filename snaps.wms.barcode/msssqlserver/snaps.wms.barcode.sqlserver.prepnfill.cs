using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.Hash;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS.Barcode {

    public partial class barcode_ops : IDisposable { 
 private barcode_ls fillls(ref SqlDataReader r) { 
            return new barcode_ls() { 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                article = r["article"].ToString(),
                pv = r.GetInt32(5),
                lv = r.GetInt32(6),
                barops = r["barops"].ToString(),
                barcode = r["barcode"].ToString(),
                bartype = r["bartype"].ToString(),
                articledsc = r["descalt"].ToString(),
                tflow = r["tflow"].ToString(),
                thname = r["thnamealt"].ToString(),
                thcode = r["thcode"].ToString(),
                isprimary = r["isprimary"].ToString().CInt32()
            };
        }
        private barcode_ix fillix(ref SqlDataReader r) { 
            return new barcode_ix() { 
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                article = r["article"].ToString(),
                pv = r.GetInt32(4),
                lv = r.GetInt32(5),
                barops = r["barops"].ToString(),
                barcode = r["barcode"].ToString(),
                bartype = r["bartype"].ToString(),
                thcode = r["thcode"].ToString(),
                tflow = r["tflow"].ToString(),
                fileid = r["fileid"].ToString(),
                rowops = r["rowops"].ToString(),
                ermsg = r["ermsg"].ToString(),
                dateops =  r.GetDateTime(14)
            };
        }
        private barcode_md fillmdl(ref SqlDataReader r) { 
            return new barcode_md() {
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                article = r["article"].ToString(),
                pv = r.GetInt32(5),
                lv = r.GetInt32(6),
                barops = r["barops"].ToString(),
                barcode = r["barcode"].ToString(),
                bartype = r["bartype"].ToString(),
                thcode = r["thcode"].ToString(),
                tflow = r["tflow"].ToString(),
                barremarks = r["barremarks"].ToString(),
                datecreate = (r.IsDBNull(13)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(13),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(15)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(15),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
                isprimary = r["isprimary"].ToString().CInt32()
            };
        }
        private SqlCommand ixcommand(barcode_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.barops,"barops");
            cm.snapsPar(o.barcode,"barcode");
            cm.snapsPar(o.bartype,"bartype");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.fileid,"fileid");
            cm.snapsPar(o.rowops,"rowops");
            cm.snapsPar(o.ermsg,"ermsg");
            cm.snapsPar(o.dateops,"dateops");
            return cm;
        }
        private SqlCommand obcommand(barcode_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.barops,"barops");
            cm.snapsPar(o.barcode,"barcode");
            cm.snapsPar(o.bartype,"bartype");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.barremarks,"barremarks");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsParsysdateoffset();
            return cm;
        }

        public SqlCommand oucommand(barcode_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.barcode,"barcode");
            return cm;
        }
        public SqlCommand oucommand(barcode_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.barcode,"barcode");
            return cm;
        }
    }
}