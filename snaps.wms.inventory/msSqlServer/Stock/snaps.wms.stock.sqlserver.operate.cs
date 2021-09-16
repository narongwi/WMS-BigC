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

    public partial class stock_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        public stock_ops() { }
        public stock_ops(String cx) {
            cn = new SqlConnection(cx);
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<stock_ops>();
        }
        //Search product list
        public async Task<List<stock_ls>> findProduct(stock_pm rs) {
            SqlCommand cm = new SqlCommand();
            List<stock_ls> rn = new List<stock_ls>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm = (sqlstock_fnd).snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.thcode,"thcode"," and p.thcode = @thcode ");
                cm.snapsCdn(rs.article,"article"," and p.article = @article ");
                // 
                if(rs.searchall.notNull()) {
                    //cm.snapsCdn(rs.searchall, "searchall", string.Format(" and (p.article like '%{0}%' or p.thcode like '%{0}%' or p.descalt like '%{0}%' )", rs.searchall.ClearReg()));
                    cm.snapsCdn(rs.searchall,"searchall"," and EXISTS (select 1 from wm_barcode b where b.orgcode= p.orgcode and b.site= p.site and b.depot=p.depot and b.article=p.article and b.pv = p.pv and b.lv = p.lv and ( b.article=@searchall or b.barcode=@searchall)) ");

                }

                if(rs.huno.notNull()) {
                    cm.snapsCdn(rs.huno,"huno",string.Format(" and exists ( select 1 from wm_stock s where huno like '%{0}%' and s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv )  ",rs.huno.ClearReg()));
                }
                if(rs.inrefno.notNull()) {
                    cm.snapsCdn(rs.inrefno,"inrefno",string.Format(" and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and s.inrefno like '%{0}%' )  ",rs.inrefno.ClearReg()));
                }
                if(rs.loccode.notNull()) {
                    cm.snapsCdn(rs.loccode,"loccode",string.Format(" and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and s.loccode like '%{0}%' )  ",rs.loccode.ClearReg()));
                }
                if(rs.batchno.notNull()) {
                    cm.snapsCdn(rs.batchno,"batchno",string.Format(" and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and batchno like '%{0}%' )  ",rs.batchno.ClearReg()));
                }
                if(rs.daterec.notNull()) {
                    cm.snapsCdn(rs.daterec,"daterec"," and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and cast(daterec as date) = cast(@daterec as date) )  ");
                }
                if(rs.datemfg.notNull()) {
                    cm.snapsCdn(rs.datemfg,"datemfg"," and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and cast(datemfg as date) = cast(@datemfg as date) )  ");
                }
                if(rs.dateexp.notNull()) {
                    cm.snapsCdn(rs.dateexp,"dateexp"," and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and cast(dateexp as date) = cast(@dateexp as date) )  ");
                }
                if(rs.stkremarks.notNull()) {
                    cm.snapsCdn(rs.stkremarks,"stkremarks",string.Format(" and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and s.stkremarks like '%{0}%' ) ",rs.stkremarks.ClearReg()));
                }
                if(rs.serialno.notNull()) {
                    cm.snapsCdn(rs.serialno,"serialno",string.Format(" and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and s.serialno like '%{0}%' ) ",rs.serialno.ClearReg()));
                }
                if(rs.isblock.notNull()) {
                    cm.snapsCdn(rs.isblock,"isblock",string.Format(" and exists ( select 1 from wm_stock s where s.orgcode = p.orgcode and s.site = p.site " +
                    " and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv and s.tflow = @isblock ) ",rs.isblock.ClearReg()));
                }

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.article,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }

        public async Task<List<stock_md>> getlist(stock_ls rs) {
            SqlCommand cm = new SqlCommand(); SqlDataReader r = null;
            List<stock_md> rn = new List<stock_md>();
            try {
                /* Vlidate parameter */
                cm = "select * from wm_stock where 1=1 ".snapsCommand(cn);
                cm.snapsCdn(rs.orgcode,"orgcode");
                cm.snapsCdn(rs.site,"site");
                cm.snapsCdn(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.article,"article");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillmdl(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(rs.orgcode,rs.site,rs.article,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }

        //get stock info - summary by location 
        public async Task<stock_info> getstockInfo(stock_ls o) {
            SqlCommand cm = new SqlCommand(); SqlDataReader r = null;
            stock_info rn = new stock_info();

            try {
                /* Vlidate parameter */
                cm = (sqlstock_info).snapsCommand(cn);
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) {
                    rn.orgcode = r["orgcode"].ToString();
                    rn.site = r["site"].ToString();
                    rn.depot = r["depot"].ToString();
                    rn.article = r["article"].ToString();
                    rn.description = r["descalt"].ToString();
                    rn.pv = r.GetInt32(5);
                    rn.lv = r.GetInt32(6);
                    rn.tihi = r["tihi"].ToString();
                    rn.dimension = r["dimension"].ToString();
                    rn.cronhand = r.GetDecimal(8);
                    rn.cravailable = r.GetDecimal(9);

                    rn.crbulknrtn = r.GetDecimal(10);
                    rn.croverflow = r.GetDecimal(11);
                    rn.crprep = r.GetDecimal(12);
                    rn.crstaging = r.GetDecimal(13);
                    rn.crrtv = r.GetDecimal(14);
                    rn.crsinbin = r.GetDecimal(15);
                    rn.crdamage = r.GetDecimal(16);
                    rn.crblock = r.GetDecimal(17);
                    rn.crincoming = r.GetDecimal(18);

                    rn.crplanship = r.GetDecimal(19);
                    rn.crexchange = r.GetDecimal(20);
                    rn.crtask = r.GetDecimal(21);
                    rn.unitmanage = r["unitmanage"].ToString();

                    rn.unitratio = r.GetInt32(23);
                    rn.thcode = r["thcode"].ToString();
                    rn.thname = r["thnameint"].ToString();
                    rn.skuweight = r.GetDecimal(27);
                    rn.barcode = r["barcode"].ToString();

                    rn.isbatchno = r["isbatchno"].ToString().CInt32();
                    rn.isdlc = r["isdlc"].ToString().CInt32();
                    rn.isunique = r["isunique"].ToString().CInt32();
                    rn.dlcall = r["dlcall"].ToString().CInt32();
                    rn.dlcwarehouse = r["dlcwarehouse"].ToString().CInt32();
                    rn.dlcfactory = r["dlcfactory"].ToString().CInt32();

                }
                await r.CloseAsync();
                await cn.CloseAsync();

                //rn.lines = await getlist(o);
                rn.lines = new List<stock_md>();
                return rn;
            } catch(Exception ex) {
                logger.Error(rn.orgcode,rn.site,rn.article,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }

        //get stock line with detail 
        public async Task<List<stock_md>> getstockLine(string typesel,stock_ls o) {
            SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
            List<stock_md> rn = new List<stock_md>();
            try {
                if(typesel == "crincoming") {
                    cm.CommandText = sqlstock_detail_incoming;
                } else if(typesel == "crplanship") {
                    cm.CommandText = sqlstock_detail_planship;
                } else if(typesel == "crtask") {
                    cm.CommandText = sqlstock_detail_task;
                } else if(typesel == "crprep") {
                    cm.CommandText = sqlstock_detail_prep;
                } else if(typesel == "crblock") {
                    cm.CommandText = sqlstock_detail_block;
                } else if(typesel == "crstaging") {
                    cm.CommandText = sqlstock_detail_staging;
                } else if(typesel == "crsinbin") {
                    cm.CommandText = sqlstock_detail_sinbin;
                } else if(typesel == "cravailable") {
                    cm.CommandText = sqlstock_detail_available;
                } else if(typesel == "crwomovement") {
                    cm.CommandText = sqlstock_detail_womovement;
                } else {
                    cm.CommandText = sqlstock_detail_all;
                }
                cm.CommandText = "select * from ( " + cm.CommandText + " ) s where 1 = 1 ";
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.article,"article");
                cm.snapsPar(o.pv,"pv");
                cm.snapsPar(o.lv,"lv");

                cm.snapsCdn(o.huno,"huno"," and s.huno = @huno ");
                cm.snapsCdn(o.loccode,"loccode"," and s.loccode = @loccode ");
                cm.snapsCdn(o.inrefno,"inrefno"," and s.inrefno = @inrefno ");
                cm.snapsCdn(o.serialno,"serialno"," and s.serialno = @serialno");
                cm.snapsCdn(o.isblock,"isblock"," and s.tflow = @isblock");

                if(typesel != "cractive" && typesel != "cravailable" && typesel != "crall" && typesel != "crprep" && typesel != "crwomovement") { 
                    cm.snapsCdn(typesel,"typesel"," and s.tflowsign = @typesel"); 
                }
                cm.CommandText += " order by tflowsign, s.huno";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillmdl(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.article,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.Dispose(); } }
        }

        //set stock remark 
        public async Task setstatus(stock_md o) {
            List<SqlCommand> cm = new List<SqlCommand>();
            SqlCommand vm = new SqlCommand(sqlstock_block_valid,cn);
            stock_ls on = new stock_ls();
            statisic_ops so = new statisic_ops();
            string rslflow = "";
            try {
                /* Vlidate parameter */
                vm.snapsPar(o.orgcode,"orgcode");
                vm.snapsPar(o.site,"site");
                vm.snapsPar(o.depot,"depot");
                vm.snapsPar(o.article,"article");
                vm.snapsPar(o.pv,"pv");
                vm.snapsPar(o.lv,"lv");
                vm.snapsPar(o.stockid,"stockid");
                vm.snapsPar(o.tflow,"tflow");
                rslflow = vm.snapsScalarStrAsync().Result;
                if(rslflow == "NF") {
                    throw new Exception("Stock is not found");
                } else {
                    if(rslflow != o.tflow) {
                        cm.Add(sqlstock_block_step1.snapsCommand());
                        cm.Add(sqlstock_block_step2.snapsCommand());
                        foreach(SqlCommand lm in cm) {
                            lm.snapsPar(o.orgcode,"orgcode");
                            lm.snapsPar(o.site,"site");
                            lm.snapsPar(o.depot,"depot");
                            lm.snapsPar(o.article,"article");
                            lm.snapsPar(o.pv,"pv");
                            lm.snapsPar(o.lv,"lv");
                            lm.snapsPar(o.tflow,"tflow");
                            lm.snapsPar(o.stkremarks,"stkremarks");
                            lm.snapsPar(o.accnmodify,"accnmodify");
                            lm.snapsPar(o.stockid,"stockid");
                            lm.snapsPar((rslflow == "IO" && o.tflow == "XX") ? "B" : "U","opstype");
                            lm.snapsPar("stock.setstatus","procmodify");
                        }
                        foreach(SqlCommand em in so.correctionstock(o.orgcode,o.site,o.depot,o.article,o.pv,o.lv)) { cm.Add(em); }
                        await cm.snapsExecuteTransAsync(cn);
                    } else {
                        vm.CommandText = sqlstock_remarks;
                        vm.snapsPar(o.stkremarks,"stkremarks");
                        vm.snapsPar(o.accnmodify,"accnmodify");
                        vm.snapsPar("stock.setstatus","procmodify");
                        await vm.snapsExecuteAsync();
                    }
                }
            } catch(Exception ex) {
                logger.Error(o.orgcode,o.site,o.article,ex,ex.Message);
                throw ex;
            } finally { if(cm != null) { cm.ForEach(x => x.Dispose()); } }
        }

        //Increase stock
        public void stockIn(List<stock_mvin> o,ref SqlConnection ocn,ref SqlTransaction otx,ref Decimal outstockid) {
            List<SqlCommand> cm = new List<SqlCommand>();
            String errsql = "";
            try {
                cm = stockIncommand(o,ref ocn,ref otx,ref outstockid);

                //Commit stock 
                cm.snapsExecuteTransRef(ref ocn,ref otx,ref errsql,true);
            } catch(Exception ex) {
                throw ex;
            }
        }

        public List<SqlCommand> stockIncommand(List<stock_mvin> o,ref SqlConnection ocn,ref SqlTransaction otx,ref Decimal outstockid) {
            return stockIncommand(ref o,ref ocn,ref otx,ref outstockid);
        }
        public List<SqlCommand> stockIncommand(ref List<stock_mvin> o,ref SqlConnection ocn,ref SqlTransaction otx,ref Decimal outstockid) {
            SqlCommand vm = new SqlCommand(sqlstock_increase_valid,ocn);
            sequence_ops sq = new sequence_ops(ocn);
            List<SqlCommand> cm = new List<SqlCommand>();
            String rsl = "";
            String errsql = "";
            DataTable dt = new DataTable();
            try {
                if(otx != null) { 
                    vm.Transaction = otx;
                }
                vm.snapsPar(o.FirstOrDefault().orgcode,"orgcode");
                vm.snapsPar(o.FirstOrDefault().site,"site");
                vm.snapsPar(o.FirstOrDefault().depot,"depot");
                vm.snapsPar(o.FirstOrDefault().stockid,"stockid");
                vm.snapsPar(o.FirstOrDefault().opssku,"opssku");
                vm.snapsPar(o.FirstOrDefault().opspu,"opspu");
                vm.snapsPar(o.FirstOrDefault().opsaccn,"opsaccn");
                vm.snapsPar(o.FirstOrDefault().article,"article");
                vm.snapsPar(o.FirstOrDefault().pv,"pv");
                vm.snapsPar(o.FirstOrDefault().lv,"lv");
                vm.snapsPar(o.FirstOrDefault().opshuno,"opshuno");
                vm.snapsPar(o.FirstOrDefault().opsthcode,"opsthcode");
                vm.snapsPar(o.FirstOrDefault().opsloccode,"opsloccode");
                vm.snapsPar(o.FirstOrDefault().opsweight,"opsweight");
                vm.snapsPar(o.FirstOrDefault().opsvolume,"opsvolume");
                vm.snapsPar(o.FirstOrDefault().opstype,"opstype");
                vm.snapsPar(o.FirstOrDefault().opscode,"opscode");
                vm.snapsPar(o.FirstOrDefault().inagrn,"inagrn");
                vm.snapsPar(o.FirstOrDefault().ingrno,"ingrno");
                vm.snapsPar(o.FirstOrDefault().opsunit,"opsunit");

                // vm.Parameters.Add("dateexp", SqlDbType.DateTimeOffset);
                // vm.Parameters["dateexp"].IsNullable = true;
                // vm.Parameters["dateexp"].SqlValue = DBNull.Value;

                // vm.Parameters.Add("datemfg", SqlDbType.DateTimeOffset);
                // vm.Parameters["datemfg"].IsNullable = true;
                // vm.Parameters["datemfg"].SqlValue = DBNull.Value;

                //vm.snapsPar(o.FirstOrDefault().dateexp,"dateexp");
                //vm.snapsPar(o.FirstOrDefault().datemfg,"datemfg");
                vm.snapsPar(o.FirstOrDefault().batchno,"batchno");
                vm.snapsPar(o.FirstOrDefault().newhu,"newhu");
                vm.snapsPar(o.FirstOrDefault().stocktflow,"stocktflow");

                //Allow to null value for some product did not control 


                vm.Parameters["batchno"].IsNullable = true;

                foreach(stock_mvin ln in o) {

                    vm.CommandText = sqlstock_increase_valid;
                    vm.Parameters["orgcode"].Value = ln.orgcode;
                    vm.Parameters["site"].Value = ln.site;
                    vm.Parameters["depot"].Value = ln.depot;
                    vm.Parameters["stockid"].Value = ln.stockid;
                    vm.Parameters["opssku"].Value = ln.opssku;
                    vm.Parameters["opspu"].Value = ln.opspu;
                    vm.Parameters["opsaccn"].Value = ln.opsaccn;
                    vm.Parameters["article"].Value = ln.article;
                    vm.Parameters["pv"].Value = ln.pv;
                    vm.Parameters["lv"].Value = ln.lv;
                    vm.Parameters["opshuno"].Value = ln.opshuno;
                    vm.Parameters["opsthcode"].Value = ln.opsthcode;
                    vm.Parameters["opsloccode"].Value = ln.opsloccode;
                    vm.Parameters["opsweight"].Value = ln.opsweight;
                    vm.Parameters["opsvolume"].Value = ln.opsvolume;
                    vm.Parameters["inagrn"].Value = ln.inagrn;
                    vm.Parameters["ingrno"].Value = ln.ingrno;
                    vm.Parameters["opsunit"].Value = ln.opsunit;
                    //if (ln.dateexp == null) { vm.Parameters["dateexp"].Value =  (DateTimeOffset?) null ; } else { vm.Parameters["dateexp"].Value = ln.dateexp; }
                    //vm.Parameters["datemfg"].Value = ln.datemfg;
                    vm.Parameters["batchno"].Value = ln.batchno;
                    vm.Parameters["newhu"].Value = string.IsNullOrEmpty(ln.newhu) ? 0 : 1;
                    vm.Parameters["stocktflow"].Value = (ln.stocktflow == null) ? "IO" : ln.stocktflow;

                    rsl = vm.snapsScalarStrAsync().Result;
                    // inrefno
                    if(ln.opssku <= 0) { throw new Exception("Quantity must more than 0 "); } 
                    else if(rsl == "HUweight") { throw new Exception(string.Format("Weight quantity on HU {0} is over capacity",ln.opshuno)); }                   //Check summary weight on HU is over
                    else if(rsl == "HUvolume") { throw new Exception(string.Format("Volume quantity on HU {0} is over capacity",ln.opshuno)); }                   //Check summary volume on HU is over 
                    else if(rsl == "HUsku") { throw new Exception(string.Format("SKU quantity on HU {0} is over capacity",ln.opshuno)); } 
                    else if(rsl == "HUflow") { throw new Exception(string.Format("HU {0} does not active",ln.opshuno)); }                   //Check summary sku on HU is over 
                    else if(rsl == "ATflow") { throw new Exception(string.Format("Article {0}-{1}-{2} is not active",ln.article,ln.pv,ln.lv)); }                   //Check Article is active
                    else if(rsl == "ATrqm") { throw new Exception(string.Format("Article {0}-{1}-{2} is requirement to measurement",ln.article,ln.pv,ln.lv)); }     //Check Article is require to measurement
                    else if(rsl == "THflow") { throw new Exception(string.Format("Third party {0} is not active",ln.opsthcode)); }                   //Check third party is active
                    else if(rsl == "LCflow") { throw new Exception(string.Format("Location {0} is not allow for stock In",ln.opsloccode)); }                   //Check Location is allow to stock In
                    else if(rsl == "LCweight") { throw new Exception(string.Format("Location {0} weight is over capacity",ln.opsloccode)); }                   //Check weight capacity of location is enough
                    else if(rsl == "LCvolume") { throw new Exception(string.Format("Location {0} volume is over capacity",ln.opsloccode)); }                   //Check volume capacity on location is enough
                    else if(rsl == "LCcount") { throw new Exception(string.Format("Location {0} has counting process",ln.opsloccode)); }                   //Check location has block for counting
                    else if(rsl == "LCOH") { throw new Exception(string.Format("Location {0} has over HU capacity",ln.opsloccode)); }                   //Check location has block for HU capacity
                    else if(rsl == "LCMA") { throw new Exception(string.Format("Location {0} has block for mix product",ln.opsloccode)); }                   //Check location has block for mix product
                    else if(rsl == "LCMB") { throw new Exception(string.Format("Location {0} has block for mix batch",ln.opsloccode)); }                   //Check location has block for mix batcno
                    else {
                        //Recalculate pu, weight, volume
                        vm.CommandText = sqlstock_decrease_skuinfo;
                        dt = vm.snapsTableAsync().Result;
                        //ln.opspu = dt.Rows[0]["qtypu"].ToString().CInt32();
                        ln.opsweight = dt.Rows[0]["qtyweight"].ToString().CDecimal();
                        ln.opsvolume = dt.Rows[0]["qtyvolume"].ToString().CDecimal();
                        ln.hutype = dt.Rows[0]["hucode"].ToString();
                        //Validate stock line is exists 
                        vm.CommandText = sqlstock_validate_exists;
                        if("0" == vm.snapsScalarStrAsync().Result) {
                            //increase stock with new line
                            ln.stockid = sq.stockNext(ref ocn).ToString().CDecimal();
                            outstockid = ln.stockid;
                            ln.tflow = (ln.stocktflow == null) ? "IO" : ln.stocktflow;
                            cm.Add(setMVPams(sqlstock_increase_step1_new,ln));
                        } else {
                            //Increase stock with old line 
                            cm.Add(setMVPams(sqlstock_increase_step1_upd,ln));
                        }

                        //Generate log movement 
                        cm.Add(setMVPams(sqlstock_increase_step2,ln));
                    }
                }
                return cm;

            } catch(Exception ex) {
                logger.Error(o.FirstOrDefault().orgcode,o.FirstOrDefault().site,o.FirstOrDefault().article,ex,ex.Message);
                throw new Exception(ex.Message.ToString());
            } finally {
                vm.Dispose();
                cm.ForEach(x => x.Dispose());
                rsl = null;
                dt.Dispose();
            }
        }

        //Decrease stock
        public void stockOut(List<stock_mvou> o,ref SqlConnection ocn,ref SqlTransaction otx,ref Decimal outstockid) {
            List<SqlCommand> cm = new List<SqlCommand>();
            String errsql = "";
            try {
                cm = stockOutcommand(o,ref ocn,ref otx,ref outstockid);
                //Commit stock 
                cm.snapsExecuteTransRef(ref ocn,ref otx,ref errsql,true);
            } catch(Exception ex) { throw ex; }
        }

        public List<SqlCommand> stockOutcommand(List<stock_mvou> o,ref SqlConnection ocn,ref SqlTransaction otx,ref Decimal outstockid) {
            SqlCommand vm = new SqlCommand(sqlstock_decrease_valid,ocn);
            sequence_ops sq = new sequence_ops(ocn);
            List<SqlCommand> cm = new List<SqlCommand>();
            String rsl = "";
            String errsql = "";
            DataTable dt = new DataTable();
            try {
                //Parse stock
                if(otx != null) { vm.Transaction = otx; }
                vm.snapsPar(o.FirstOrDefault().orgcode,"orgcode");
                vm.snapsPar(o.FirstOrDefault().site,"site");
                vm.snapsPar(o.FirstOrDefault().depot,"depot");
                vm.snapsPar(o.FirstOrDefault().stockid,"stockid");
                vm.snapsPar(o.FirstOrDefault().opssku,"opssku");
                vm.snapsPar(o.FirstOrDefault().opsaccn,"opsaccn");
                vm.snapsPar(o.FirstOrDefault().article,"article");
                vm.snapsPar(o.FirstOrDefault().pv,"pv");
                vm.snapsPar(o.FirstOrDefault().lv,"lv");
                vm.snapsPar(o.FirstOrDefault().opshuno,"opshuno");
                vm.snapsPar(o.FirstOrDefault().opsthcode,"opsthcode");
                vm.snapsPar(o.FirstOrDefault().opsloccode,"opsloccode");
                vm.snapsPar(o.FirstOrDefault().opsweight,"opsweight");
                vm.snapsPar(o.FirstOrDefault().opsvolume,"opsvolume");
                vm.snapsPar(o.FirstOrDefault().opstype,"opstype");
                vm.snapsPar(o.FirstOrDefault().opscode,"opscode");

                foreach(stock_mvou ln in o) {

                    vm.CommandText = sqlstock_decrease_valid;
                    vm.Parameters["orgcode"].Value = ln.orgcode;
                    vm.Parameters["site"].Value = ln.site;
                    vm.Parameters["depot"].Value = ln.depot;
                    vm.Parameters["stockid"].Value = ln.stockid;
                    vm.Parameters["opssku"].Value = ln.opssku;
                    vm.Parameters["opsaccn"].Value = ln.opsaccn;
                    vm.Parameters["article"].Value = ln.article;
                    vm.Parameters["pv"].Value = ln.pv;
                    vm.Parameters["lv"].Value = ln.lv;
                    vm.Parameters["opshuno"].Value = ln.opshuno;
                    vm.Parameters["opsthcode"].Value = ln.opsthcode;
                    vm.Parameters["opsloccode"].Value = ln.opsloccode;
                    vm.Parameters["opsweight"].Value = ln.opsweight;
                    vm.Parameters["opsvolume"].Value = ln.opsvolume;


                    rsl = vm.snapsScalarStrAsync().Result;
                    if(ln.opssku <= 0) { throw new Exception("Quantity must more than 0 "); } 
                    else if(rsl == "HUweight") { throw new Exception(string.Format("Weight quantity on HU {0} is over capacity",ln.opshuno)); }                    //Check summary weight on HU is over
                                                            else if(rsl == "HUvolume") { throw new Exception(string.Format("Volume quantity on HU {0} is over capacity",ln.opshuno)); }                    //Check summary volume on HU is over 
                                                            else if(rsl == "HUsku") { throw new Exception(string.Format("SKU quantity on HU {0} is over capacity",ln.opshuno)); } else if(rsl == "HUflow") { throw new Exception(string.Format("HU {0} does not active",ln.opshuno)); }                    //Check summary sku on HU is over 
                                                                                                                                                                                else if(rsl == "ATflow") { throw new Exception(string.Format("Article {0}-{1}-{2} is not active",ln.article,ln.pv,ln.lv)); }                    //Check Article is active
                                                                                                                                                                                else if(rsl == "ATrqm") { throw new Exception(string.Format("Article {0}-{1}-{2} is requirement to measurement",ln.article,ln.pv,ln.lv)); }     //Check Article is require to measurement
                                                                                                                                                                                else if(rsl == "THflow") { throw new Exception(string.Format("Third party {0}-{1}-{2} is not active",ln.opsthcode)); }                    //Check third party is active
                                                                                                                                                                                                                    else if(rsl == "LCflow") { throw new Exception(string.Format("Location {0} is not allow for stock In",ln.opsloccode)); }                    //Check Location is allow to stock In
                                                                                                                                                                                                                    else if(rsl == "LCweight") { throw new Exception(string.Format("Location {0} weight is over capacity",ln.opsloccode)); }                    //Check weight capacity of location is enough
                                                                                                                                                                                                                    else if(rsl == "LCvolume") { throw new Exception(string.Format("Location {0} volume is over capacity",ln.opsloccode)); }                    //Check volume capacity on location is enough
                                                                                                                                                                                                                    else if(rsl == "LCcount") { throw new Exception(string.Format("Location {0} has counting process",ln.opsloccode)); }                    //Check Counting process
                                                                                                                                                                                                                    else {
                        //Recalculate pu, weight, volume
                        vm.CommandText = sqlstock_decrease_skuinfo;
                        dt = vm.snapsTableAsync().Result;
                        ln.opspu = dt.Rows[0]["qtypu"].ToString().CInt32();
                        ln.opsweight = dt.Rows[0]["qtyweight"].ToString().CDecimal();
                        ln.opsvolume = dt.Rows[0]["qtyvolume"].ToString().CDecimal();
                        //Decrase stock
                        cm.Add(setMVPams(sqlstock_decrease_step1,ln));
                        //Generate log movement 
                        cm.Add(setMVPams(sqlstock_decrease_step2,ln));
                    }
                }

                return cm;

            } catch(Exception ex) {
                logger.Error(o.FirstOrDefault().orgcode,o.FirstOrDefault().site,o.FirstOrDefault().article,ex,ex.Message);
                throw ex;
            } finally {
                vm.Dispose();
                cm.ForEach(x => { x.Dispose(); });
                rsl = null;
                dt.Dispose();
            }
        }

        public async Task<List<lov>> getproductratio(String orgcode,String site,String depot,String article,String pv,String lv) {
            SqlCommand cm = null;
            List<lov> rn = new List<lov>();
            SqlDataReader r = null;
            try {
                /* Vlidate parameter */
                cm = sqlgetratio.snapsCommand(cn);
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(article,"article");
                cm.snapsPar(pv,"pv");
                cm.snapsPar(lv,"lv");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new lov(r["rtosku"].ToString(),r["unit"].ToString(),r["bnvalue"].ToString(),"","","","")); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { throw ex; } finally { if(cm != null) { cm.Dispose(); } if(r != null) { await r.DisposeAsync(); } }
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing) {
            if(!disposedValue) {
                if(cn != null) { cn.Dispose(); }
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}
