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
using Snaps.Helpers.Logger;

namespace Snaps.WMS {
    public partial class correction_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        public correction_ops() {  }
        public correction_ops(String cx) { 
            cn = new SqlConnection(cx);
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<correction_ops>();
        }

        public async Task<List<correction_ls>> find(correction_pm rs) { 
            SqlCommand cm = null;
            List<correction_ls> rn = new List<correction_ls>();
            DbDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm = ("").snapsCommand(cn);
                cm.snapsCdn(rs.orgcode,"orgcode");
                cm.snapsCdn(rs.site,"site");
                cm.snapsCdn(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.dateops,"dateops");
                cm.snapsCdn(rs.article,"article");
                cm.snapsCdn(rs.qtypu,"qtypu");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) {
                logger.Error(rs.orgcode, rs.site, rs.accnops,ex, ex.Message);
                throw ex;
            }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<correction_md> get(correction_ls rs){ 
            SqlCommand cm = null; DbDataReader r = null;
            correction_md rn = new correction_md();
            try { 
                /* Vlidate parameter */
                cm = ("").snapsCommand(cn);
                cm.snapsCdn(rs.orgcode,"orgcode");
                cm.snapsCdn(rs.site,"site");
                cm.snapsCdn(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.dateops,"dateops");
                cm.snapsCdn(rs.article,"article");
                cm.snapsCdn(rs.qtypu,"qtypu");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) {
                logger.Error(rs.orgcode, rs.site, rs.accnops, ex, ex.Message);
                throw ex; 
            }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }

        public async Task<List<lov>> getLocation(stock_ls o) { 
            string spcrecvzone  = "";
            string spcrecvaisle = "";
            string spcrecvaisleto = "";
            string spcrecvbay   = "";
            string spcrecvbayto = "";
            string spcrecvlevel = "";
            string spcrecvlevelto = "";
            string spcrecvlocation = "";
            string isslowmove = "";

            SqlCommand cm = new SqlCommand(sqlcorrect_getlocation,cn);
            SqlDataReader r = null;
            List<lov> rn = new List<lov>();
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");
                cm.snapsPar(o.thcode,"thcode");
                cm.snapsPar(o.crweight,"crvolume");
                cm.snapsPar(o.crvolume,"crweight");
              

                cm.CommandText = sqlcorrect_stategic_step1_product;
                foreach(System.Data.DataRow ln in cm.snapsTableAsync().Result.Rows) {
                    spcrecvzone     = ln["spcrecvzone"].ToString();
                    spcrecvaisle    = ln["spcrecvaisle"].ToString();
                    spcrecvaisleto  = ln["spcrecvaisleto"].ToString();
                    spcrecvbay      = ln["spcrecvbay"].ToString(); 
                    spcrecvbayto    = ln["spcrecvbayto"].ToString();
                    spcrecvlevel    = ln["spcrecvlevel"].ToString(); 
                    spcrecvlevelto  = ln["spcrecvlevelto"].ToString();
                    spcrecvlocation = ln["spcrecvlocation"].ToString();
                    isslowmove      = ln["isslowmove"].ToString();
                }

                    cm.CommandText = string.Format(
                    sqlcorrect_statetic_step2_selection,
                    string.Format("and lszone ='{0}' ", spcrecvzone), // specific zone 

                    (spcrecvaisle == "") ? "" : " and exists ( select 1 from wm_locup al where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'AL' and lsseq >= " + spcrecvaisle +
                    ((spcrecvaisleto == "") ? "" : " and lsseq <=  " + spcrecvaisleto) + " and l.orgcode = al.orgcode and l.site = al.site and l.depot = al.depot and l.lsaisle = al.lscode)", // specific aisle
                    
                    (spcrecvbay == "") ? "" : " and exists ( select 1 from wm_locup ab where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'BA' and lsseq >= " + spcrecvbay + 
                    ((spcrecvbayto == "") ? "" : " and lsseq <=  " + spcrecvbayto) + " and l.orgcode = ab.orgcode and l.site = ab.site and l.depot = ab.depot and l.lsbay = ab.lscode )", // specific bay 
                    
                    (spcrecvlevel == "") ? "" : " and exists ( select 1 from wm_locup ab where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'LV' and lsseq >= " + spcrecvlevel +
                    ((spcrecvlevelto == "") ? "" : " and lsseq <=  " + spcrecvlevelto) + " and l.orgcode = ab.orgcode and l.site = ab.site and l.depot = ab.depot and l.lslevel = ab.lscode ) ", // spceic level

                    (spcrecvlocation == "" ) ? "" : " and lscode = '"+spcrecvlocation+"' ", // specific location 

                    (isslowmove == "1") ? "asc" : "desc"  // specific slow move selection
                    
                );
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { 
                    rn.Add(new lov() { 
                        value = r["lscode"].ToString(),
                        desc = r["lscodealt"].ToString(),
                        icon = "",
                        valopnfirst =  r["loctype"].ToString(),
                        valopnfour = "",
                        valopnsecond = "",
                        valopnthird = ""
                    }); 
                }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) {
                logger.Error(o.orgcode, o.site, "", ex, ex.Message); 
                throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }

        public async Task<correction_md> CorrectionStock(correction_md o) {
            SqlCommand cmv = new SqlCommand("",cn);
            SqlTransaction tx;
            handerlingunit_ops hu = new handerlingunit_ops(cn);
            stock_ops oiv = new stock_ops(cn.ConnectionString );    //Stock operation
            statisic_ops so = new statisic_ops(ref cn);             //Statistic operation
            handerlingunit_gen hg = new handerlingunit_gen();       //Handerlingunit object
            List<stock_mvin> miv = new List<stock_mvin>();          //Stock In movement object
            List<stock_mvou> mov = new List<stock_mvou>();          //Stock In movement object
            List<SqlCommand> cm = new List<SqlCommand>();           //Correction command list
            List<SqlCommand> sm = new List<SqlCommand>();           //Statistic command list
            sequence_ops sq = new sequence_ops(cn);                 //Sequence generator
            String isorbit = "0";
            String mapcode = "";
            DataTable dt = new DataTable();
            Decimal outstockid = 0; //bug'
            bool isnewhu = false;

            string errsql = "";
            try { 
                //validate correction code is request send to orbit 
                isorbit = "0";
                cmv.CommandText = string.Format(sqlcorrect_valloc,o.typeops);
                o.qtysku = o.variancesku;
                o.qtypu = o.variancepu;

                cmv.snapsPar(o.orgcode,"orgcode");
                cmv.snapsPar(o.site,"site");
                cmv.snapsPar(o.depot,"depot");
                cmv.snapsPar(o.codeops,"bnvalue");
                cmv.snapsPar(o.loccode.ClearReg(),"loccode");
                cmv.snapsPar(o.huno.ToString(),"huno");
                cmv.snapsPar(o.variancesku,"variancesku");
                cmv.snapsPar(o.article,"article");
                cmv.snapsPar(o.pv,"pv");
                cmv.snapsPar(o.lv,"lv");
                cmv.snapsPar(o.variancepu,"opsqty");
                cmv.snapsPar(o.batchno,"batchno");
                cmv.snapsPar("","setno");
                

                o.loccode = cmv.snapsScalarStrAsync().Result;
                if (o.loccode == "NF") {
                    throw new Exception("Location incorrect ");
                }else if (o.loccode == "BL") {
                    throw new Exception("Article status inactive");
                } else if (o.loccode == "OW") { 
                    throw new Exception("Weight is over capacity");
                } else if (o.loccode == "OV") { 
                    throw new Exception("Volume is over capacity");
                } else if (o.loccode == "OH") { 
                    throw new Exception("HU is over capacity");
                } else if (o.loccode == "OA") { 
                    throw new Exception("Article can not mixing");
                } else if (o.loccode == "OB") { 
                    throw new Exception("Batchno can not mixing");
                }else { 
                    cmv.CommandText = sqlcorrect_valcode;
                    dt = cmv.snapsTableAsync().Result;
                    if (dt.Rows.Count == 0) throw new Exception("adjust code is invalid");
                    mapcode = dt.Rows[0]["mapcode"].ToString();
                    isorbit = dt.Rows[0]["reqorbut"].ToString();
                    o.orbitsite = dt.Rows[0]["orbitsite"].ToString();
                    o.orbitdepot = dt.Rows[0]["orbitdepot"].ToString();
                    o.orbitsource = dt.Rows[0]["orbitsource"].ToString();
                    //o.qtysku = Math.Abs(o.variancesku);
                    //cmv.snapsPar(o.variancesku,"opsqty");

                    //Validate is request a HU 
                    if (o.huno == "0")
                    {
                        cmv.CommandText = sqlcorrect_huinfo;
                        dt = cmv.snapsTableAsync().Result;

                        if (dt.Rows.Count == 0) throw new Exception("invalid hu info.");

                        hg.accncreate = o.accncreate;
                        hg.accnmodify = o.accncreate;
                        hg.crcapacity = 0;
                        hg.crsku = 0;
                        hg.crvolume = 0;
                        hg.crweight = 0;
                        hg.depot = o.depot;
                        hg.hutype = dt.Rows[0]["hucode"].ToString();
                        //hg.loccode = o.loccode;
                        hg.mxsku = dt.Rows[0][1].ToString().CInt32();
                        hg.mxweight = dt.Rows[0][2].ToString().CDecimal();
                        hg.mxvolume = dt.Rows[0][3].ToString().CDecimal();
                        hg.orgcode = o.orgcode;
                        hg.priority = 0;
                        hg.procmodfiy = "wms.correction";
                        hg.promo = "";
                        hg.quantity = 1;
                        hg.routeno = "";
                        hg.site = o.site;
                        hg.spcarea = "ST";
                        hg.tflow = "IO";
                        hg.thcode = dt.Rows[0]["thcode"].ToString();
                        hg.loccode = o.loccode;
                        await hu.generate(hg);
                        o.huno = hg.huno;
                        o.thcode = dt.Rows[0]["thcode"].ToString();
                        o.qtyweight = dt.Rows[0][5].ToString().CDecimal();
                        o.qtyvolume = dt.Rows[0][6].ToString().CDecimal();
                        o.qtypu = dt.Rows[0][7].ToString().CInt32();
                        o.daterec = DateTimeOffset.Now;

                        //generate stock id 
                        o.stockid = sq.stockNext(ref cn).ToString().CInt32();
                        o.ingrno = sq.grnoNext(ref cn).ToString();
                        isnewhu = true;
                        if (isorbit == "1")
                        {
                            o.inagrn = o.inagrn.ClearReg();
                        }
                        else
                        {
                            o.inagrn = sq.agrnNext(ref cn).ToString();
                        }
                    }
                    else
                    {

                        //Recalculate weight, volume, PU quantity 
                        cmv.CommandText = sqlcorrect_prodinfo;
                        dt = cmv.snapsTableAsync().Result;

                        if (dt.Rows.Count == 0) throw new Exception("product infomation not found.");
                        o.qtyweight = dt.Rows[0][0].ToString().CDecimal();
                        o.qtyvolume = dt.Rows[0][1].ToString().CDecimal();
                        o.qtypu = dt.Rows[0][2].ToString().CInt32();
                        o.thcode = dt.Rows[0]["thcode"].ToString();

                        //validate HU still active

                        cmv.snapsPar(o.qtyweight, "qtyweight");
                        cmv.snapsPar(o.qtyvolume, "qtyvolume");

                        cmv.CommandText = sqlcorrect_valhuno;
                        dt = cmv.snapsTableAsync().Result;

                        if (dt.Rows.Count == 0)
                        {   //validate HU is not missing
                            throw new Exception("HU no " + o.huno + " not found");
                        }
                        else if (dt.Rows[0][0].ToString() == "HUsku")
                        {           //validate 
                            throw new Exception("HU no " + o.huno + " SKU quantity is over capacity");
                        }
                        else if (dt.Rows[0][0].ToString() == "HUweight")
                        {        //validate weight
                            throw new Exception("Hu no " + o.huno + " Weight is over capacity ");
                        }
                        else if (dt.Rows[0][0].ToString() == "HUvolume")
                        {        //validate volume
                            throw new Exception("Hu no " + o.huno + " Volume is over capacity ");
                        }
                        else if (dt.Rows[0][0].ToString() == "HUflow")
                        {        //validate state
                            throw new Exception("Hu no " + o.huno + " state is not active ");
                        }
                    }// end check hu

                  

                    o.spcarea = "ST";
                    o.inrefno = string.IsNullOrEmpty(o.inrefno.ClearReg()) ? o.inagrn.ClearReg() : o.inrefno ;
                    //For BULK Set MI3STASTK= 0 and Other Set MI3STASTK= 1 
                    cmv.CommandText = sqlcorrect_isbulkloc;
                    string isbulkloc = cmv.snapsScalarStrAsync().Result;
                    o.inreftype = isbulkloc == "1"? "BULK" : "";
                    o.inpromo = "";
                    o.ingrno = o.ingrno.ClearReg();
                    o.inagrn = o.inagrn.ClearReg();
                    o.tflow = "IO";
                    o.procmodify = "wms.correction";
                    o.dateops =  DateTimeOffset.Now;
                    o.datemfg =  o.datemfg; // fix
                    o.dateexp =  o.dateexp; // fix

                    //Generate correction sequnce no 
                    o.seqops = sq.corrNext(ref cn);

                    //Create new correction transaction
                    cm.Add(correctCommand(o,sqlcorrect_new));

                    //Correction to orbit

                    if (isorbit == "1" )
                    {
                        if (o.inagrn == "") { 
                            throw new Exception("System can not track agrn no"); 
                        }

                        //Get agrn no, grno]
                        cmv.Parameters.Clear();
                        cmv.CommandText = sqlcorrect_getagrn2;
                        cmv.snapsPar(o.orgcode, "orgcode");
                        cmv.snapsPar(o.site, "site");
                        cmv.snapsPar(o.depot, "depot");
                        cmv.snapsPar(o.inrefno, "inrefno");
                        cmv.snapsPar(o.inagrn, "inagrn");
                        dt = cmv.snapsTableAsync().Result;

                        if (dt.Rows.Count > 0)
                        {
                            o.inagrn = dt.Rows[0]["inagrn"].ToString();
                            o.thcode = dt.Rows[0]["thcode"].ToString();
                        }
                        else
                        {
                            throw new Exception($"Can not matching AGRN {o.inagrn} with Inbound order");
                        }

                        if (o.inagrn == "")
                        {
                            throw new Exception("AGRN required!!");
                        }

                    }

                    // initial interface  data
                    cm.Add(orbitCommand(o, mapcode));

                    //Parse transaction 
                    if (cn.State == ConnectionState.Closed) { 
                        cn.Open(); 
                    }


                    tx = cn.BeginTransaction();
                    
                    //Correction stock movement 
                    if (o.variancesku == 0) { 
                            throw new Exception("Quantity must more than 0");
                    } else if (o.typeops == "+") {
                        if (o.stockbeforesku > o.aftersku) {
                            throw new Exception("Correction type is not matchting with variance quantity");
                        } else { 
                            miv.Add(tomoveIn(o));
                            oiv.stockIncommand(miv,ref cn,ref tx, ref outstockid).ForEach(x=> { cm.Add(x); }); 
                        }
                    }else { 
                        if (o.stockbeforesku < o.aftersku){
                            throw new Exception("Correction type is not matchting with variance quantity");
                        } else { 
                            o.qtysku = Math.Abs(o.variancesku);                        
                            o.variancepu = Math.Abs(o.variancepu);
                            o.variancesku = Math.Abs(o.variancesku);
                            mov.Add(tomoveOut(o));                        
                            oiv.stockOutcommand(mov,ref cn,ref tx, ref outstockid).ForEach(x=> { cm.Add(x); });
                        }
                    }
                    

                    //Correction transaction                
                    cm.snapsExecuteTransRef(ref cn,ref tx,ref errsql,true); //debuging

                    //Commit transaction
                    tx.Commit();
                    cn.Close();
                    

                    //Recal stock capacity 
                    sm = so.correctionstock(o.orgcode, o.site, o.depot,o.article,o.pv,o.lv);
                    //Recal location capacity 
                    sm.Add(so.location(o.orgcode,o.site,o.depot,o.loccode));

                    //await sm.snapsExecuteTransAsync(cn,true,errsql); 
                    sm.snapsExecuteTrans(cn,ref errsql,true);

                    /* PRINT LABEL */
                    if(isnewhu) {
                        await printCorrection(o.orgcode,o.site,o.depot,o.seqops,o.huno,o.accnops);
                    }

                    return o;
                }
            }catch (Exception ex) {
                logger.Error(o.orgcode, o.site, o.accnops, ex, ex.Message);
                throw ex; 
            }
            finally { 

            }

        }

        private async Task<bool> printCorrection(string orgcode,string site,string depot,string seqops,string huno,string accode) {
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
                    formVariables.Add(new KeyValuePair<string,string>("seqops",seqops));
                    formVariables.Add(new KeyValuePair<string,string>("huno",huno));

                    var formContent = new System.Net.Http.FormUrlEncodedContent(formVariables);
                    var httpResponse = await httpClient.PostAsync("print/printCorrection",formContent);
                    return httpResponse.IsSuccessStatusCode;
                }
            } catch(Exception ex) {
                logger.Error(orgcode,site,accode,ex,ex.Message,"HU:",ex.Message);
                return false;
            }
        }

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                if (cn != null) { cn.Dispose(); }
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}
