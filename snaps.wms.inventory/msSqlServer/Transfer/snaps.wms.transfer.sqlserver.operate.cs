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
    public partial class transfer_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        public transfer_ops() {  }
        public transfer_ops(String cx) { 
            cn = new SqlConnection(cx);
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<transfer_ops>();
        }

        public lov checklocation(string orgcode, string site, string depot, string loccode) { 
            SqlCommand cm = new SqlCommand(sqltransfer_checklocation,cn);
            lov rn = new lov();
            try{ 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(loccode,"loccode");
                foreach(DataRow ln in cm.snapsTableAsync().Result.Rows) {
                    rn.value = ln["lscode"].ToString();
                    rn.desc = ln["lscodealt"].ToString();
                    rn.icon = "";
                    rn.valopnfirst = ln["loctype"].ToString();
                    rn.valopnfour = "";
                    rn.valopnsecond = "";
                    rn.valopnthird = "";
                }
                if (rn.value == null) { 
                    throw new Exception("Location not found");
                }else {
                    return rn;
                }
            }catch (Exception ex) {
                logger.Error(orgcode,site,"",ex,ex.Message);
                throw ex; 
            }
            finally { if(cm!=null) { cm.Dispose(); } }
        }
        public void validatelocation(transfer_md o) { 
            SqlCommand cm = new SqlCommand(sqltransfer_validlocation,cn);
            string rsl = "";
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                //cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");
                cm.snapsPar(o.variancesku,"opssku");
                cm.snapsPar(o.targetlocation,"opsloccode");

                rsl = cm.snapsScalarStrAsync().Result;
                switch(rsl){
                    case "ATflow"   :   throw new Exception("Article state does not active");
                    case "ATrqm"    :   throw new Exception("Article still require to measurement");
                    case "LCflow"   :   throw new Exception("Location state does not active");
                    case "LCweight" :   throw new Exception("Weight on location is over capacity");
                    case "LCvolume" :   throw new Exception("Volume on location is over capacity");
                    case "LChu"     :   throw new Exception("HU on location is over capacity");
                    case "HUweight" :   throw new Exception("Weight on HU is over capacity");
                    case "HUvolume" :   throw new Exception("Volume on HU is over capaciry");
                    case "HUsku"    :   throw new Exception("Quantity on HU is over capacity");
                    case "HUflow"   :   throw new Exception("HU state does not active");
                    default : break;
                }
            }
            catch (Exception ex) {
                logger.Error(o.orgcode,o.site,"",ex,ex.Message);
                throw ex; 
            }
            finally { if(cm!=null) { cm.Dispose();} }
        }
        public void validateMixlocation(transfer_md o) {
            SqlCommand cm = new SqlCommand("",cn);
            string rsl = "";
            try {
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");
                cm.snapsPar(o.variancesku,"opssku");
                cm.snapsPar(o.daterec,"daterec");
                cm.snapsPar(o.datemfg,"datemfg");
                cm.snapsPar(o.targetlocation,"opsloccode");

                // validate existing task transfer 
                DataTable dt = new DataTable();
                cm.CommandText = sqltransfer_validexisting;
                dt = cm.snapsTableAsync().Result;
                if(dt.Rows.Count > 0)  throw new Exception("Location is waiting for confirmation [Task No." + dt.Rows[0]["taskno"].ToString() + "] ");
               

                // get product
                cm.CommandText = sqltransfer_validproduct;
                dt = cm.snapsTableAsync().Result;
                if(dt.Rows.Count == 0) throw new Exception("Article state does not active");
                if(dt.Rows[0]["ismeasurement"].ToString() == "1") throw new Exception("Article still require to measurement");
                cm.snapsPar(dt.Rows[0]["skuweight"].ToString().CDouble(),"skuweight");
                cm.snapsPar(dt.Rows[0]["skuvolume"].ToString().CDouble(),"skuvolume");

                // validate capacity
                cm.CommandText = sqltransfer_validmixlocation; 
                rsl = cm.snapsScalarStrAsync().Result;

                if(string.IsNullOrEmpty(rsl)) {
                    throw new Exception("Location state does not active");
                }else if(rsl != "pass") {
                    throw new Exception(rsl);
                }
            } catch(Exception ex) {
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } }
        }

        public async Task<transfer_md> transferstock(transfer_md o) {
            SqlCommand cmv = new SqlCommand("",cn);
            //SqlCommand vllc = new SqlCommand(sqltransfer_reqblock,cn);
            SqlTransaction tx = null;
            handerlingunit_ops hu = new handerlingunit_ops(cn);
            stock_ops oiv = new stock_ops(cn.ConnectionString );    //Stock operation
            statisic_ops so = new statisic_ops(ref cn);             //Statistic operation
            handerlingunit_gen hg = new handerlingunit_gen();       //Handerlingunit object
            List<stock_mvin> miv = new List<stock_mvin>();          //Stock movement In object
            List<stock_mvou> mou = new List<stock_mvou>();         //Stock movement Out object
            List<SqlCommand> cm = new List<SqlCommand>();           //Correction command list
            List<SqlCommand> sm = new List<SqlCommand>();           //Statistic command list
            sequence_ops sq = new sequence_ops();                 //Sequence generator
            task_ops to = new task_ops(cn);
            task_md ts = new task_md();
            DataTable dt = new DataTable();
            Decimal outstockid = 0; //bug
            string opstaskno = "";
            //string targetblock = "";
            string errsql = "";
            try {

                /* Validate or Check Location */
                validateMixlocation(o);

                //validate correction code is request send to orbit 
                cmv.snapsPar(o.orgcode,"orgcode");
                cmv.snapsPar(o.site,"site");
                cmv.snapsPar(o.depot,"depot");
                cmv.snapsPar(o.huno.ToString(),"huno");
                cmv.snapsPar(o.article,"article");
                cmv.snapsPar(o.pv,"pv");
                cmv.snapsPar(o.lv,"lv");
                cmv.snapsPar(o.variancesku,"opsqty");
                cmv.snapsPar("","setno");

                cmv.CommandText = sqltransfer_huinfo;
                dt = cmv.snapsTableAsync().Result;
                if (o.aftersku != 0) { 
                    hg.accncreate= o.accncreate;
                    hg.accnmodify = o.accncreate;
                    hg.crcapacity = 0;
                    hg.crsku = 0;
                    hg.crvolume = 0;
                    hg.crweight = 0;
                    hg.depot = o.depot;
                    hg.hutype = dt.Rows[0]["hucode"].ToString();
                    hg.loccode = o.loccode;
                    hg.mxsku = dt.Rows[0][1].ToString().CInt32();
                    hg.mxweight = dt.Rows[0][2].ToString().CDecimal();
                    hg.mxvolume = dt.Rows[0][3].ToString().CDecimal();
                    hg.orgcode = o.orgcode;
                    hg.priority = 0;
                    hg.procmodfiy = "wms.transfer";
                    hg.promo = "";
                    hg.quantity = 1;
                    hg.routeno = "";
                    hg.site = o.site;
                    hg.spcarea = "ST";
                    hg.tflow = "IO";
                    hg.thcode = dt.Rows[0][0].ToString();
                    hg.loccode = o.loccode;
                    await hu.generate(hg);
                    o.rslhuno = hg.huno;
                }else { 
                    o.rslhuno = o.huno;
                }

                o.thcode =  dt.Rows[0][0].ToString();
                o.qtyweight =  dt.Rows[0][5].ToString().CDecimal();
                o.qtyvolume =  dt.Rows[0][6].ToString().CDecimal();
                o.qtypu = dt.Rows[0][7].ToString().CInt32();
                o.daterec = DateTimeOffset.Now;

                //Generate transfer sequnce no 
                o.seqops = sq.transferNext(ref cn);

                //fix
                o.spcarea = "ST";
                o.inrefno = o.seqops;
                o.inreftype = "Transfer";
                o.inpromo = "";
                //o.ingrno = "";
                o.tflow = "IO";
                o.procmodify = "wms.transfer";
                o.dateops =  DateTimeOffset.Now;
                //o.datemfg =  DateTimeOffset.Now; //bug
                //o.dateexp =  DateTimeOffset.Now; //bug  

                ////If location has blocked must be send to GC               
                //vllc.snapsPar(o.orgcode, "orgcode");
                //vllc.snapsPar(o.site,"site");
                //vllc.snapsPar(o.depot,"depot");
                //vllc.snapsPar(o.targetlocation,"lscode");
                //targetblock = vllc.snapsScalarStrAsync().Result.ToString();
               
                if (o.aftersku == 0) { 
                    //Move full pallet 

                    //Generate Task movment 
                    ts = gettaskmove(o);
                    to.generateTaskCommand(ts,ref cn,ref opstaskno).ForEach(x=> { cm.Add(x); });

                    //Create new transfer transaction
                    o.rsltaskno = ts.taskno; 
                    cm.Add(transferCommand(o,sqltransfer_new));

                    //Set command to sending block location stock flow active 
                    //if ( (targetblock == "1" && o.stocktflow == "IO")||
                    //     (targetblock == "0" && o.stocktflow == "XX")){ 
                    //    cm.Add(oiv.sqlstock_block_step2.snapsCommand());
                    //    cm.Last().snapsPar(o.orgcode, "orgcode");
                    //    cm.Last().snapsPar(o.site,"site");
                    //    cm.Last().snapsPar(o.depot,"depot");
                    //    cm.Last().snapsPar(o.stockid,"stockid");
                    //    cm.Last().snapsPar(o.article,"article");
                    //    cm.Last().snapsPar(o.pv,"pv");
                    //    cm.Last().snapsPar(o.lv,"lv");
                    //    cm.Last().snapsPar(o.accncreate,"accnmodify");
                    //    cm.Last().snapsPar((targetblock == "1" && o.stocktflow == "IO") ? "B" : "U","opstype");
                    //}
                    //if (targetblock == "1" ||targetblock == "0")
                    //{
                    //    cm.Add(oiv.sqlstock_block_step2.snapsCommand());
                    //    cm.Last().snapsPar(o.orgcode, "orgcode");
                    //    cm.Last().snapsPar(o.site, "site");
                    //    cm.Last().snapsPar(o.depot, "depot");
                    //    cm.Last().snapsPar(o.stockid, "stockid");
                    //    cm.Last().snapsPar(o.article, "article");
                    //    cm.Last().snapsPar(o.pv, "pv");
                    //    cm.Last().snapsPar(o.lv, "lv");
                    //    cm.Last().snapsPar(o.accncreate, "accnmodify");
                    //    cm.Last().snapsPar(targetblock == "1" ? "B" : "U", "opstype");
                    //}
                    //Transfer transaction -- Pendin f
                    await cm.snapsExecuteTransAsync(cn); 


                } else { 

                    //Parse transaction -
                    if (cn.State == ConnectionState.Closed) { cn.Open(); }
                    tx = cn.BeginTransaction();
                    
                    //Absolute stock 
                    o.variancesku = Math.Abs(o.variancesku);
                    o.variancepu = Math.Abs(o.variancepu);

                    //Set stock status before generate command 
                    //if ( (targetblock == "1" ||targetblock == "0")){ 
                    //    //set new stock to revert status
                    //    o.stocktflow = (targetblock == "1" && o.stocktflow == "IO") ? "XX" : "IO";
                    //}

                    //Transfer stock In
                    miv.Add(tomoveIn(o));               
                    oiv.stockIncommand(miv,ref cn,ref tx, ref outstockid).ForEach(x=> { cm.Add(x); });

                    o.rslstockid = outstockid;

                    //Set command to sending block location stock flow active 
                    //if ( (targetblock == "1" && o.stocktflow == "IO")||
                    //     (targetblock == "0" && o.stocktflow == "XX")){ 
                    //    cm.Add(oiv.sqlstock_block_step2.snapsCommand());
                    //    cm.Last().snapsPar(o.orgcode, "orgcode");
                    //    cm.Last().snapsPar(o.site,"site");
                    //    cm.Last().snapsPar(o.depot,"depot");
                    //    cm.Last().snapsPar(o.rslstockid,"stockid");
                    //    cm.Last().snapsPar(o.article,"article");
                    //    cm.Last().snapsPar(o.pv,"pv");
                    //    cm.Last().snapsPar(o.lv,"lv");
                    //    cm.Last().snapsPar(o.accncreate,"accnmodify");
                    //    cm.Last().snapsPar(targetblock == "1" ? "B" : "U","opstype");
                    //}

                    //Transfer stock Out
                    mou.Add(tomoveOut(o));
                    oiv.stockOutcommand(mou,ref cn,ref tx, ref outstockid).ForEach(x=> { cm.Add(x); });   

                    //Generate Task movment 
                    ts = gettaskmove(o);
                    to.generateTaskCommand(ts,ref cn,ref opstaskno).ForEach(x=> { cm.Add(x); });

                    //Create new transfer transaction
                    o.rsltaskno = ts.taskno; 
                    cm.Add(transferCommand(o,sqltransfer_new));

                    //Transfer transaction -- Pendin f
                    cm.snapsExecuteTransRef(ref cn,ref tx,ref errsql,true); //debuging

                    //Commit transaction
                    tx.Commit();
                    cn.Close();                 
                }

                //Recal stock capacity 
                sm = so.correctionstock(o.orgcode, o.site, o.depot,o.article,o.pv,o.lv);
                 //Recal location capacity 
                sm.Add(so.location(o.orgcode,o.site,o.depot,o.loccode));

                //Calculate execute
                //await sm.snapsExecuteTransAsync(cn,true,errsql); 
                sm.snapsExecuteTrans(cn,ref errsql,true);

                /* PRINT LABEL */
                if(!string.IsNullOrEmpty(opstaskno)){
                    await printPallet(o.orgcode,o.site,o.depot,opstaskno,o.accnops);
                }

                return o;
            }catch (Exception ex) { 
                if (tx!=null) { try { tx.Rollback(); } catch(Exception exl) {} }
                cn.Close();
                //Console.WriteLine(errsql);
                logger.Error(o.orgcode,o.site,o.accnops,ex,ex.Message);
                throw ex; 
            }
            finally { }
        }
        private async Task<bool> printPallet(string orgcode,string site,string depot,string taskno,string accode) {
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
                    formVariables.Add(new KeyValuePair<string,string>("taskno",taskno));
                    var formContent = new System.Net.Http.FormUrlEncodedContent(formVariables);
                    var httpResponse = await httpClient.PostAsync("print/printputaway",formContent);
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
