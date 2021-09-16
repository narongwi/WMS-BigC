using Snaps.Helpers.DbContext.SQLServer;
using Snaps.Helpers.StringExt;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Snaps.WMS
{
    public partial class handerlingunit_ops : IDisposable {

        public async Task<List<handerlingunit>> find(handerlingunit rs)
        {
            SqlCommand cm = new SqlCommand(hu_sqlfnd, cn);
            List<handerlingunit> rn = new List<handerlingunit>();
            SqlDataReader r = null;
            try
            {
                /* Vlidate parameter */
                cm.snapsPar(rs.orgcode, "orgcode");
                cm.snapsPar(rs.site, "site");
                cm.snapsPar(rs.depot, "depot");
                cm.snapsCdn(rs.spcarea, "spcarea"," and h.spcarea = @spcarea ");
                cm.snapsCdn(rs.hutype, "hutype"," and h.hutype = @hutype ");
                cm.snapsCdn(rs.huno, "huno"," and h.huno = @huno ");
                cm.snapsCdn(rs.thcode, "thcode", string.Format(" and ( h.thcode like '%{0}%' or t.thnameint like '%{0}%' ) ", rs.thcode.ClearReg()));
                cm.snapsCdn(rs.routeno, "routeno"," and h.routeno = @routeno ");
                cm.snapsCdn(rs.tflow, "tflow"," and h.tflow = @tflow ");
                cm.snapsCdn(rs.promo, "promo"," and h.promo = @promo ");
                cm.snapsCdn(rs.priority, "priority"," and h.priority = @priority ");
                cm.snapsCdn(rs.loccode,"loccode", " and h.loccode = @loccode");

                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn.Add(handerlingunit_get(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }

        public async Task<handerlingunit> get(handerlingunit o) { 
            SqlCommand cm = new SqlCommand(hu_sqlget, cn);
            handerlingunit rn = new handerlingunit();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");
                cm.snapsPar(o.huno, "huno");
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn = handerlingunit_get(ref r); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }

        public async Task<List<Snaps.WMS.lov>> Lov(string orgcode, string site, string depot){ 
            SqlCommand cm = new SqlCommand(sqlhu_lov, cn);
            SqlDataReader r = null;
            List<Snaps.WMS.lov> rn = new List<Snaps.WMS.lov>();
            try { 
                cm.snapsPar(orgcode, "orgcode");
                cm.snapsPar(site, "site");
                cm.snapsPar(depot, "depot");

                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { 
                    rn.Add(new Snaps.WMS.lov(r["article"].ToString(), r["descalt"].ToString()));
                }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            }catch (Exception ex){ 
                throw ex; 
            }finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }

        public async Task<List<handerlingunit_item>> lines(handerlingunit o, Boolean issummary = true)
        {
            SqlCommand cm = new SqlCommand(hu_sqlline, cn);
            List<handerlingunit_item> rn = new List<handerlingunit_item>();
            SqlDataReader r = null;
            try
            {
                if ( issummary == false) { 
                    cm.CommandText =  hu_sqlline_nonsum;
                } else if (o.tflow == "ED") { 
                    cm.CommandText = hu_sqllinehx;
                }else { 
                    cm.CommandText = hu_sqlline;
                }
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");
                cm.snapsPar(o.huno, "huno");

                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { 
                    rn.Add(new handerlingunit_item()
                    {
                
                        prepno = r["prepno"].ToString(), 
                        inorder = r["inorder"].ToString(),
                       
                        ouorder = r["ouorder"].ToString(),                     
                        article = r["article"].ToString(),
                        pv = r.IsDBNull(4) ?  0 : r["pv"].ToString().CInt32(),
                        lv = r.IsDBNull(5) ?  0 : r["lv"].ToString().CInt32(),
                        qtysku = r.IsDBNull(6) ?  0 : r["qtysku"].ToString().CInt32(),
                        qtypu =  r.IsDBNull(7) ?  0 : r["qtypu"].ToString().CDecimal(),
                        qtyweight = r.IsDBNull(8) ?  0 : r["qtyweight"].ToString().CDecimal(),
                        qtyvolume = r.IsDBNull(9) ?  0 : r["qtyvolume"].ToString().CDecimal(),
                        descalt = r["descalt"].ToString(),
                        batchno = r["batchno"].ToString(),
                        lotno = r["lotno"].ToString(), 
                        datemfg = (r.IsDBNull(13)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(13),
                        dateexp = (r.IsDBNull(14)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(14),
                        serialno = r["serialno"].ToString(),
                        unitprep = r["unitprep"].ToString(),
                        loccode = r["loccode"].ToString()
                    }); 

                    
                }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }

        public async Task upsert(List<handerlingunit> rs)
        {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                foreach (handerlingunit ln in rs) { 
                    cm.Add(handerlingunit_setcmd(ln, hu_sqlins_stp1)); 
                }
                await cm.snapsExecuteTransAsync(cn);
            }
            catch (Exception ex) {
                throw ex;
            }
            finally { 
                cm.ForEach(x => x.Dispose()); 
            }
        }
        public async Task upsert(handerlingunit rs)
        {
            List<handerlingunit> ro = new List<handerlingunit>();
            try {
                ro.Add(rs); await upsert(ro);
            }
            catch (Exception ex) {
                throw ex;
            }
            finally { ro.Clear(); }
        }
        public async Task remove(List<handerlingunit> rs)
        {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                foreach (handerlingunit ln in rs) { cm.Add(handerlingunit_setcmd(ln, hu_sqlrem_stp1)); }
                await cm.snapsExecuteTransAsync(cn);
            }
            catch (Exception ex) {
                throw ex;
            }
            finally { 
                cm.ForEach(x => x.Dispose()); 
            }
        }
        public async Task remove(handerlingunit rs)
        {
            List<handerlingunit> ro = new List<handerlingunit>();
            try
            {
                ro.Add(rs); await remove(ro);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { ro.Clear(); }
        }


        public async Task generate(handerlingunit_gen o) {
            List<handerlingunit> gn = new List<handerlingunit>();
            SqlCommand cm = new SqlCommand("", cn);
            SqlCommand ms = new SqlCommand("", cn);
            String defloc = "";
            DataTable odt = new DataTable();
            DataTable ort = new DataTable();
            Decimal mssku    = 0;
            Decimal msweight = 0;
            Decimal msvolume = 0;

            string defroute = "";
            try {
                // get max weight , volume from HU Master
                ms.snapsPar(o.orgcode,"orgcode");
                ms.snapsPar(o.site,"site");
                ms.snapsPar(o.depot,"depot");
                ms.snapsPar((o.spcarea == "XD") ? o.huno : o.hutype,"hutype");
                //ms.snapsPar((o.spcarea == "XD") ? o.huno : o.hutype,"hutype");
                ms.snapsPar(o.huno,"huno");

                ms.CommandText = "select mxsku from wm_handerlingunit where hutype = 'MS' and orgcode = @orgcode and site = @site and depot = @depot and huno = @hutype";
                var _masku = await ms.snapsScalarStrAsync();
                if(_masku == null) throw new Exception("mxsku is null on handerlingunit");
                mssku = ms.snapsScalarStrAsync().Result.ToString().CDecimal();

                ms.CommandText = "select mxweight from wm_handerlingunit where hutype = 'MS' and orgcode = @orgcode and site = @site and depot = @depot and huno = @hutype";
                var _msweight = await ms.snapsScalarStrAsync();
                if(_msweight == null) throw new Exception("mxweight is null on handerlingunit");
                msweight = ms.snapsScalarStrAsync().Result.ToString().CDecimal();

                ms.CommandText = "select mxvolume from wm_handerlingunit where hutype = 'MS' and orgcode = @orgcode and site = @site and depot = @depot and huno = @hutype";
                var _msvolume = await ms.snapsScalarStrAsync();
                if(_msvolume == null) throw new Exception("msvolume is null on handerlingunit");
                msvolume = ms.snapsScalarStrAsync().Result.ToString().CDecimal();

                if (o.spcarea == "XD") { 
                    cm.CommandText = "select lscode from wm_loczp where spcthcode = @thcode and spcarea = 'XD' and orgcode = @orgcode and site = @site and depot = @depot";
                    cm.snapsPar(o.orgcode,"orgcode");
                    cm.snapsPar(o.site,"site");
                    cm.snapsPar(o.depot,"depot");
                    cm.snapsPar(o.thcode,"thcode");
                    odt = await cm.snapsTableAsync();

                    cm.CommandText = "select top 1 routeno from wm_route where orgcode = 'bgc' and site = @site and depot = @depot and thcode = @thcode and tflow = 'IO' order by routetype desc ";
                    ort = await cm.snapsTableAsync();
                    //defloc = cm.snapsScalarStrAsync().Result;
                    if (odt.Rows.Count > 0 && ort.Rows.Count > 0 ){ 
                        defloc = odt.Rows[0][0].ToString();
                        defroute = ort.Rows[0][0].ToString();
                        cm.CommandText = string.Format(hu_seq, o.site);
                        for (int i = 0; i < o.quantity; i++) {
                            gn.Add(new handerlingunit() {
                                orgcode = o.orgcode, 
                                site = o.site,
                                depot = o.depot,
                                spcarea = o.spcarea,
                                hutype = o.hutype,
                                huno = cm.snapsScalarStrAsync().Result,
                                loccode = defloc,
                                thcode = o.thcode,
                                routeno = defroute,
                                mxsku = mssku,
                                mxweight = msweight,
                                mxvolume = msvolume,
                                crsku = 0, 
                                crweight = 0,
                                crvolume = 0,
                                crcapacity = o.crcapacity,
                                tflow = "IO",
                                datecreate = DateTimeOffset.Now,
                                datemodify = DateTimeOffset.Now,
                                accncreate = o.accncreate,
                                accnmodify = o.accnmodify,
                                promo = o.promo.nvl(),
                                procmodfiy = o.procmodfiy,
                                priority = o.priority,
                                thname = "",
                            });
                        }                        
                    } else if (odt.Rows.Count == 0) { 
                        throw new Exception("Distribution grid does not define");
                    } else if (ort.Rows.Count == 0) {
                        throw new Exception("Default route preparation not define");
                    }
                }else { 
                    cm.CommandText = string.Format(hu_seq, o.site);
                        for (int i = 0; i < o.quantity; i++) {
                            gn.Add(new handerlingunit() {
                                orgcode = o.orgcode, 
                                site = o.site,
                                depot = o.depot,
                                spcarea = o.spcarea,
                                hutype = o.hutype,
                                huno = cm.snapsScalarStrAsync().Result,
                                loccode = o.loccode,
                                thcode = o.thcode,
                                routeno = o.routeno,
                                mxsku = mssku,
                                mxweight = msweight,
                                mxvolume = msvolume,                            
                                crsku = 0, 
                                crweight = 0,
                                crvolume = 0,
                                crcapacity = o.crcapacity,
                                tflow = "IO",
                                datecreate = DateTimeOffset.Now,
                                datemodify = DateTimeOffset.Now,
                                accncreate = o.accncreate,
                                accnmodify = o.accnmodify,
                                promo = o.promo.nvl(),
                                procmodfiy = o.procmodfiy,
                                priority = o.priority,
                                thname = "",
                            });
                        }
                }

                await upsert(gn);
                if (gn.Count > 0) { 
                    o.huno = gn[0].huno; 
                }

                // Send Empty Label to Printer
                if(o.spcarea == "XD" && o.hutype == "XE") {
                    await printEmpty(o.orgcode,o.site,o.depot,gn[0].huno,o.accncreate);
                }
            }
            catch (Exception ex) { throw ex; }
            finally {
                
            }
        }
        private async Task<bool> printEmpty(string orgcode,string site,string depot,string huno,string accode) {
            try {
                // get document api config
                string command =
                    @"select top 1 bnflex1 from wm_binary where orgcode=@orgcode and site=@site and depot=@depot 
                    and apps='WMS' and bntype = 'PRINTER' and bncode ='LABEL'";

                SqlCommand pm = new SqlCommand(command,cn);
                pm.snapsPar(orgcode,"orgcode");
                pm.snapsPar(site,"site");
                pm.snapsPar(depot,"depot");
                string baseAddress = pm.snapsScalarStrAsync().Result;
                using(var httpClient = new System.Net.Http.HttpClient()) {
                    httpClient.Timeout = TimeSpan.FromSeconds(60);
                    httpClient.BaseAddress = new Uri(baseAddress);
                    var formVariables = new List<KeyValuePair<string,string>>();
                    formVariables.Add(new KeyValuePair<string,string>("orgcode",orgcode));
                    formVariables.Add(new KeyValuePair<string,string>("site",site));
                    formVariables.Add(new KeyValuePair<string,string>("depot",depot));
                    formVariables.Add(new KeyValuePair<string,string>("huno",huno));
                    var formContent = new System.Net.Http.FormUrlEncodedContent(formVariables);
                    var httpResponse = await httpClient.PostAsync("print/huempty",formContent);
                    return httpResponse.IsSuccessStatusCode;
                }
            } catch(Exception) {
                return false;
            }
        }
        public async Task<List<lov>> getmaster(String orgcode, String site, String depot)
        {
            SqlCommand cm = new SqlCommand(string.Format(hu_getmaster, orgcode, site, depot) , cn);
            List<lov> ls = new List<lov>(); SqlDataReader r = null;
            try
            {
                cm.snapsPar(orgcode, "orgcode");
                cm.snapsPar(site, "site");
                cm.snapsPar(depot, "depot");
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { 
                    ls.Add(new lov(r["huno"].ToString(), r["descalt"].ToString())); 
                }
                await r.CloseAsync(); await cn.CloseAsync();
                return ls;
            }
            catch (Exception ex) { throw ex; }
            finally { cm.Dispose();  }
        }

        public async Task close(handerlingunit o){ 
            SqlCommand cm = new SqlCommand("",cn);
            try {
                cm.CommandText = hu_sql_close;
                cm.snapsPar(o.accnmodify,"accnmodify");
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.huno,"huno");
                cm.snapsParsysdateoffset();
                await cm.snapsExecuteAsync();

                cm.CommandText = hu_sql_calhu;
                await cm.snapsExecuteAsync();

                cm.CommandText = hu_sql_calroute;
                await cm.snapsExecuteAsync();
            }
            catch (Exception ex) {
                throw ex;
            }
            finally { await cm.DisposeAsync(); }
        }
    }
}
