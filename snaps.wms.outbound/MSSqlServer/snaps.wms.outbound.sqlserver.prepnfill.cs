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
        public partial class outbound_ops : IDisposable {
            //Header
            private outbound_ls outbound_getls(ref SqlDataReader r) { 
                outbound_ls rn = new outbound_ls();
                try { 
                    rn.orgcode = r["orgcode"].ToString();
                    rn.site = r["site"].ToString();
                    rn.depot = r["depot"].ToString();
                    rn.spcarea = r["spcarea"].ToString();
                    rn.ouorder = r["ouorder"].ToString(); 
                    rn.outype = r["outype"].ToString();
                    rn.ousubtype = r["ousubtype"].ToString();
                    //rn.thcode = r["disthcode"].ToString();
                    rn.thcode = r["thcode"].ToString();
                    rn.dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
                    rn.dateprep = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
                    rn.dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10);
                    rn.oupriority = r.GetInt32(11);
                    rn.oupromo = r["oupromo"].ToString();
                    rn.orbitsource = r["orbitsource"].ToString();
                    rn.datereqdel = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(28);
                    rn.ouremarks = r["ouremarks"].ToString();
                    rn.tflow = r["tflow"].ToString();
                    rn.datedelivery = (r.IsDBNull(30)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(30);
                    rn.thname = r["thname"].ToString();
                    rn.disinbound = r["inorder"].ToString();
                    rn.disproduct = r["disproduct"].ToString();
                    rn.disproductdesc = r["disproductdsc"].ToString();
                    rn.dateremarks = r["dateremarks"].ToString();
                    rn.disunitops = r["disunitops"].ToString();
                    rn.disqtypnd = r["qtypnd"].ToString().CInt32();
                    rn.dishuno = r["dishuno"].ToString();
                    rn.disstockid = r["disstockid"].ToString().CInt32();
                    rn.dislv = r["dislv"].ToString().CInt32();
                    rn.dispv = r["dispv"].ToString().CInt32();
                    rn.disloccode = r["disloccode"].ToString();
                    return rn; 
                }catch (Exception ex){
                    throw ex;
                }

            }
            private outbound_ix outbound_getix(ref SqlDataReader r) { 
                outbound_ix rn = new outbound_ix();
                rn.orgcode = r["orgcode"].ToString();
                rn.site = r["site"].ToString();
                rn.depot = r["depot"].ToString();
                rn.spcarea = r["spcarea"].ToString();
                rn.ouorder = r["ouorder"].ToString();
                rn.outype = r["outype"].ToString();
                rn.ousubtype = r["ousubtype"].ToString();
                rn.thcode = r["thcode"].ToString();
                rn.dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
                rn.dateprep = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
                rn.dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10);
                rn.oupriority = r.GetInt32(11);
                rn.ouflag = r["ouflag"].ToString();
                rn.oupromo = r["oupromo"].ToString();
                rn.orbitsource = r["orbitsource"].ToString();
                rn.dropship = r["dropship"].ToString();
                rn.stocode = r["stocode"].ToString();
                rn.stoname = r["stoname"].ToString();
                rn.stoaddressln1 = r["stoaddressln1"].ToString();
                rn.stoaddressln2 = r["stoaddressln2"].ToString();
                rn.stoaddressln3 = r["stoaddressln3"].ToString();
                rn.stosubdistict = r["stosubdistict"].ToString();
                rn.stodistrict = r["stodistrict"].ToString();
                rn.stocity = r["stocity"].ToString();
                rn.stocountry = r["stocountry"].ToString();
                rn.stopostcode = r["stopostcode"].ToString();
                rn.stomobile = r["stomobile"].ToString();
                rn.stoemail = r["stoemail"].ToString();
                rn.fileid =  r["fileid"].ToString();
                rn.rowops =  r["rowops"].ToString();
                rn.tflow = r["tflow"].ToString();
                rn.ermsg =   r["ermsg"].ToString();
                rn.opsdate =  (r.IsDBNull(32)) ? (DateTime?) null : r.GetDateTime(32);

                return rn;
            }
            
            private outbound_md outbound_getmd(ref SqlDataReader r) { 
                outbound_md rn = new outbound_md();
                rn.orgcode = r["orgcode"].ToString();
                rn.site = r["site"].ToString();
                rn.depot = r["depot"].ToString();
                rn.spcarea = r["spcarea"].ToString();
                rn.ouorder = r["ouorder"].ToString();
                rn.outype = r["outype"].ToString();
                rn.ousubtype = r["ousubtype"].ToString();
                rn.thcode = r["thcode"].ToString();
                rn.dateorder = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8);
                rn.dateprep = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
                rn.dateexpire = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10);
                rn.oupriority = r.GetInt32(11);
                rn.ouflag = r["ouflag"].ToString();
                rn.oupromo = r["oupromo"].ToString();
                rn.orbitsource = r["orbitsource"].ToString();
                rn.dropship = r["dropship"].ToString();
                rn.stocode = r["stocode"].ToString();
                rn.stoname = r["stoname"].ToString();
                rn.stoaddressln1 = r["stoaddressln1"].ToString();
                rn.stoaddressln2 = r["stoaddressln2"].ToString();
                rn.stoaddressln3 = r["stoaddressln3"].ToString();
                rn.stosubdistict = r["stosubdistict"].ToString();
                rn.stodistrict = r["stodistrict"].ToString();
                rn.stocity = r["stocity"].ToString();
                rn.stocountry = r["stocountry"].ToString();
                rn.stopostcode = r["stopostcode"].ToString();
                rn.stomobile = r["stomobile"].ToString();
                rn.stoemail = r["stoemail"].ToString();
                rn.datereqdel = (r.IsDBNull(28)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(28);
                rn.dateprocess = (r.IsDBNull(29)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(29);
                rn.datedelivery = (r.IsDBNull(30)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(30);
                rn.qtyorder = r.GetDecimal(31);
                rn.qtypnd = r.GetDecimal(32);
                rn.ouremarks = r["ouremarks"].ToString();
                rn.tflow = r["tflow"].ToString();
                rn.datecreate = (r.IsDBNull(35)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(35);
                rn.accncreate = r["accncreate"].ToString();
                rn.datemodify = (r.IsDBNull(37)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(37);
                rn.accnmodify = r["accnmodify"].ToString();
                rn.procmodify = r["procmodify"].ToString();
                rn.thname = r["thname"].ToString();
                return rn;
            }
            private SqlCommand outbound_setix(outbound_ix o,String sql) { 
                SqlCommand cm = new SqlCommand(sql,cn);
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.ouorder,"ouorder");
                cm.snapsPar(o.outype,"outype");
                cm.snapsPar(o.ousubtype,"ousubtype");
                cm.snapsPar(o.thcode,"thcode");
                cm.snapsPar(o.dateorder,"dateorder");
                cm.snapsPar(o.dateprep,"dateprep");
                cm.snapsPar(o.dateexpire,"dateexpire");
                cm.snapsPar(o.oupriority,"oupriority");
                cm.snapsPar(o.ouflag,"ouflag");
                cm.snapsPar(o.oupromo,"oupromo");
                cm.snapsPar(o.orbitsource,"orbitsource");
                cm.snapsPar(o.dropship,"dropship");
                cm.snapsPar(o.stocode,"stocode");
                cm.snapsPar(o.stoname,"stoname");
                cm.snapsPar(o.stoaddressln1,"stoaddressln1");
                cm.snapsPar(o.stoaddressln2,"stoaddressln2");
                cm.snapsPar(o.stoaddressln3,"stoaddressln3");
                cm.snapsPar(o.stosubdistict,"stosubdistict");
                cm.snapsPar(o.stodistrict,"stodistrict");
                cm.snapsPar(o.stocity,"stocity");
                cm.snapsPar(o.stocountry,"stocountry");
                cm.snapsPar(o.stopostcode,"stopostcode");
                cm.snapsPar(o.stomobile,"stomobile");
                cm.snapsPar(o.stoemail,"stoemail");
                cm.snapsPar(o.fileid,"fileid");
                cm.snapsPar(o.rowops,"rowops");
                cm.snapsPar(o.tflow,"tflow");
                cm.snapsPar(o.ermsg,"ermsg");
                cm.snapsPar(o.opsdate,"opsdate");
                return cm;
            }
            private SqlCommand outbound_setmd(outbound_md o,String sql){ 
                SqlCommand cm = new SqlCommand(sql,cn);
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.ouorder,"ouorder");
                cm.snapsPar(o.outype,"outype");
                cm.snapsPar(o.ousubtype,"ousubtype");
                cm.snapsPar(o.thcode,"thcode");
                cm.snapsPar(o.dateorder,"dateorder");
                cm.snapsPar(o.dateprep,"dateprep");
                cm.snapsPar(o.dateexpire,"dateexpire");
                cm.snapsPar(o.oupriority,"oupriority");
                cm.snapsPar(o.ouflag,"ouflag");
                cm.snapsPar(o.oupromo,"oupromo");
                cm.snapsPar(o.orbitsource,"orbitsource");
                cm.snapsPar(o.dropship,"dropship");
                cm.snapsPar(o.stocode,"stocode");
                cm.snapsPar(o.stoname,"stoname");
                cm.snapsPar(o.stoaddressln1,"stoaddressln1");
                cm.snapsPar(o.stoaddressln2,"stoaddressln2");
                cm.snapsPar(o.stoaddressln3,"stoaddressln3");
                cm.snapsPar(o.stosubdistict,"stosubdistict");
                cm.snapsPar(o.stodistrict,"stodistrict");
                cm.snapsPar(o.stocity,"stocity");
                cm.snapsPar(o.stocountry,"stocountry");
                cm.snapsPar(o.stopostcode,"stopostcode");
                cm.snapsPar(o.stomobile,"stomobile");
                cm.snapsPar(o.stoemail,"stoemail");
                cm.snapsPar(o.datereqdel,"datereqdel");
                cm.snapsPar(o.dateprocess,"dateprocess");
                cm.snapsPar(o.datedelivery,"datedelivery");
                cm.snapsPar(o.qtyorder,"qtyorder");
                cm.snapsPar(o.qtypnd,"qtypnd");
                cm.snapsPar(o.ouremarks,"ouremarks");
                cm.snapsPar(o.tflow,"tflow");
                cm.snapsPar(o.datecreate,"datecreate");
                cm.snapsPar(o.accncreate,"accncreate");
                cm.snapsPar(o.datemodify,"datemodify");
                cm.snapsPar(o.accnmodify,"accnmodify");
                cm.snapsPar(o.procmodify,"procmodify");
                return cm;
            }

            //Line 
            private outbouln_ls outbouln_getls(ref SqlDataReader r) { 
                return new outbouln_ls() { 
                    orgcode = r["orgcode"].ToString(),
                    site = r["site"].ToString(),
                    depot = r["depot"].ToString(),
                    spcarea = r["spcarea"].ToString(),
                    ouorder = r["ouorder"].ToString(),
                    ouln = r["ouln"].ToString(),
                    ourefno = r["ourefno"].ToString(),
                    barcode = r["barcode"].ToString(),
                    article = r["article"].ToString(),
                    pv = r.GetInt32(8),
                    lv = r.GetInt32(9),
                    unitops = r["unitops"].ToString(),
                    qtysku = r.GetInt32(11),
                    qtypu = r.GetInt32(12),
                    tflow = r["tflow"].ToString(),
                    articledsc =  r["articledsc"].ToString(),

                    
                };
            }
            private outbouln_ix outbouln_getix(ref SqlDataReader r) { 
                return new outbouln_ix() { 
                    site = r["site"].ToString(),
                    depot = r["depot"].ToString(),
                    spcarea = r["spcarea"].ToString(),
                    ouorder = r["ouorder"].ToString(),
                    ouln = r["ouln"].ToString(),
                    ourefno = r["ourefno"].ToString(),
                    ourefln =r["ourefln"].ToString(), 
                    inorder = r["inorder"].ToString(),
                    barcode = r["barcode"].ToString(),
                    article = r["article"].ToString(),
                    pv = r.GetInt32(10),
                    lv = r.GetInt32(11),
                    unitops = r["unitops"].ToString(),
                    qtysku = r.GetInt32(13),
                    qtypu = r.GetInt32(14),
                    qtyweight = r.GetDecimal(15),
                    lotno = r["lotno"].ToString(),
                    expdate = (r.IsDBNull(17)) ? (DateTime?) null : r.GetDateTime(17),
                    serialno = r["serialno"].ToString(),
                    tflow = r["tflow"].ToString(),
                    fileid = r["lotno"].ToString(),
                    rowops = r["lotno"].ToString(),
                    ermsg = r["lotno"].ToString(),
                    dateops = (r.IsDBNull(22)) ? (DateTime?) null : r.GetDateTime(22),
                };
            }
            private outbouln_md outbouln_getmd(ref SqlDataReader r) { 
                outbouln_md rn = new outbouln_md();
                rn.orgcode = r["orgcode"].ToString();
                rn.site = r["site"].ToString();
                rn.depot = r["depot"].ToString();
                rn.spcarea = r["spcarea"].ToString();
                rn.ouorder = r["ouorder"].ToString();
                rn.ouln = r["ouln"].ToString();
                rn.ourefno = r["ourefno"].ToString();
                rn.ourefln =r["inorder"].ToString();
                rn.inorder = r["inorder"].ToString();
                rn.barcode = r["barcode"].ToString();
                rn.article = r["article"].ToString();
                rn.pv = r.GetInt32(11);
                rn.lv = r.GetInt32(12);
                rn.unitops = r["unitops"].ToString();
                rn.qtysku = r.GetInt32(14);
                rn.qtypu = r.GetInt32(15);
                rn.qtyweight = r.GetDecimal(16);
                rn.spcselect = r["spcselect"].ToString();
                rn.batchno = r["batchno"].ToString();
                rn.lotno =  r["lotno"].ToString();
                rn.datemfg = (r.IsDBNull(20)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(20);
                rn.dateexp = (r.IsDBNull(21)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(21);
                rn.serialno = r["serialno"].ToString();
                rn.tflow = r["tflow"].ToString();
                rn.qtypnd = r.GetInt32(24);
                rn.datedelivery = (r.IsDBNull(25)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(25);
                rn.qtyskudel = r.GetInt32(26);
                rn.qtypudel = r.GetInt32(27);
                rn.qtyweightdel = r.GetDecimal(28);
                rn.oudnno = r["oudnno"].ToString();
                rn.datecreate = (r.IsDBNull(30)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(30);
                rn.accncreate = r["accncreate"].ToString();
                rn.datemodify = (r.IsDBNull(32)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(32);
                rn.accnmodify = r["accnmodify"].ToString();
                rn.procmodify = r["procmodify"].ToString();
                rn.articledsc = r["descalt"].ToString();
                rn.qtypndpu = r.GetInt32(36);
                rn.qtyreqpu = r.GetInt32(37);
                rn.disthcode = r["disthcode"].ToString();
                rn.disthname = r["distthname"].ToString();
                
                return rn;
            }
            private SqlCommand outbouln_setix(outbouln_ix o,String sql) { 
                SqlCommand cm = new SqlCommand(sql,cn);
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.ouorder,"ouorder");
                cm.snapsPar(o.ouln,"ouln");
                cm.snapsPar(o.ourefno,"ourefno");
                cm.snapsPar(o.ourefln,"ourefln");
                cm.snapsPar(o.inorder,"inorder");
                cm.snapsPar(o.barcode,"barcode");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");
                cm.snapsPar(o.unitops,"unitops");
                cm.snapsPar(o.qtysku,"qtysku");
                cm.snapsPar(o.qtypu,"qtypu");
                cm.snapsPar(o.qtyweight,"qtyweight");
                cm.snapsPar(o.lotno,"lotno");
                cm.snapsPar(o.expdate,"expdate");
                cm.snapsPar(o.serialno,"serialno");
                cm.snapsPar(o.tflow,"tflow");
                cm.snapsPar(o.fileid,"fileid");
                cm.snapsPar(o.rowops,"rowops");
                cm.snapsPar(o.ermsg,"ermsg");
                cm.snapsPar(o.dateops,"dateops");
                return cm;
            }
            private SqlCommand outbouln_setmd(outbouln_md o,String sql){ 
                SqlCommand cm = new SqlCommand(sql,cn);
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.ouorder,"ouorder");
                cm.snapsPar(o.ouln,"ouln");
                cm.snapsPar(o.ourefno,"ourefno");
                cm.snapsPar(o.ourefln,"ourefln");
                cm.snapsPar(o.inorder,"inorder");
                cm.snapsPar(o.barcode,"barcode");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");
                cm.snapsPar(o.unitops,"unitops");
                cm.snapsPar(o.qtysku,"qtysku");
                cm.snapsPar(o.qtypu,"qtypu");
                cm.snapsPar(o.qtyweight,"qtyweight");
                cm.snapsPar(o.spcselect,"spcselect");
                cm.snapsPar(o.lotno,"lotno");
                cm.snapsPar(o.datemfg,"datemfg");
                cm.snapsPar(o.dateexp,"dateexp");
                cm.snapsPar(o.serialno,"serialno");
                cm.snapsPar(o.tflow,"tflow");
                cm.snapsPar(o.qtypnd,"qtypnd");
                cm.snapsPar(o.datedelivery,"datedelivery");
                cm.snapsPar(o.qtyskudel,"qtyskudel");
                cm.snapsPar(o.qtypudel,"qtypudel");
                cm.snapsPar(o.qtyweightdel,"qtyweightdel");
                cm.snapsPar(o.oudnno,"oudnno");
                cm.snapsPar(o.datecreate,"datecreate");
                cm.snapsPar(o.accncreate,"accncreate");
                cm.snapsPar(o.datemodify,"datemodify");
                cm.snapsPar(o.accnmodify,"accnmodify");
                cm.snapsPar(o.procmodify,"procmodify");
                return cm;
            }
            
        }



}