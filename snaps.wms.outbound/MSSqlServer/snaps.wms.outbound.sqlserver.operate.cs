using System;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS
{

    //Header 
    public partial class outbound_ops : IDisposable
    {
        private int getRowlimit(string orgcode,string site,string depot)
        {
            SqlCommand rm = new SqlCommand(pagerowlimit,cn);
            try{rm.snapsPar(orgcode, "orgcode");
                rm.snapsPar(site, "site");
                rm.snapsPar(depot, "depot");
                return rm.snapsScalarStrAsync().Result.CInt32();
            }catch (Exception ex) {
                logger.Error(orgcode, site, depot,ex.Message);
                return 200;
            }
            finally { rm.Dispose(); }
        }
        public async Task<List<outbound_ls>> find(outbound_pm rs)
        {
            SqlCommand cm = new SqlCommand();
            List<outbound_ls> rn = new List<outbound_ls>();
            SqlDataReader r = null;
            try
            {
                //fix slow with limit row perpage default 200
                int rowlimt = getRowlimit(rs.orgcode, rs.site, rs.depot); 

                /* Vlidate parameter */
                cm = outbound_sqlfnd.snapsCommand(cn);
                cm.snapsPar(rowlimt, "rowlimit");
                cm.snapsPar(rs.orgcode, "orgcode");
                cm.snapsPar(rs.site, "site");
                cm.snapsPar(rs.depot, "depot");
                cm.snapsCdn(rs.spcarea, "spcarea", " and o.spcarea = @spcarea ");
                cm.snapsCdn(rs.outype, "outype", " and outype = @outype ");
                cm.snapsCdn(rs.ousubtype, "ousubtype", " and ousubtype = @ousubtype ");
                cm.snapsCdn(rs.tflow, "tflow", " and o.tflow = @tflow ");
                cm.snapsCdn(rs.dropship, "dropship", " and o.dropship = @dropship ");
                cm.snapsCdn(rs.thcode, "thcode", " and o.thcode = @thcode ");
                cm.snapsCdn(rs.ouorder, "ouorder", " and ouorder = @ouorder ");
                cm.snapsCdn(rs.disinbound, "disinbound", " and inorder = @disinbound ");
                if (rs.oupriority != 100) { cm.snapsCdn(rs.oupriority.ToString(), "oupriority", " and o.oupriority " + ((rs.oupriority == 1) ? " > 0 " : " = 0")); }
                if (rs.article.notNull())
                {
                    cm.snapsCdn(rs.article, "article", string.Format(" and exists (select 1 from wm_outbouln l where exists (select 1 from wm_product p where l.orgcode = p.orgcode " +
                    " and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv and (p.article like '%{0}%'  " +
                    "  or p.description like '%{0}%' or p.descalt like '%{0}%')) and o.orgcode = l.orgcode and o.site = l.site and o.depot = l.depot  " +
                    "  and o.ouorder = l.ouorder) ", rs.article.ClearReg()));
                }
                if (rs.oupromo.notNull()) { cm.snapsCdn(rs.oupromo, "oupromo", string.Format(" and o.oupromo like '%{0}%' ", rs.oupromo.ClearReg())); }
                if (rs.ouflag.notNull()) { cm.snapsCdn(rs.ouflag, "ouflag", string.Format(" and ( o.ouflag like '%{0}%' or o.ouremarks like '%{0}%' ) ", rs.ouflag.ClearReg())); }

                //Date req
                if (!rs.datereqfrom.Equals(null) && !rs.datereqto.Equals(null))
                {
                    cm.snapsPar(rs.datereqto, "datereqto");
                    cm.snapsCdn(rs.datereqfrom, "datereqfrom", " and cast(o.datereqdel as date) between cast(@datereqfrom as date) and cast(@datereqto as date) ");
                }
                else if (!rs.datereqfrom.Equals(null) && rs.datereqto.Equals(null))
                {
                    cm.snapsCdn(rs.datereqfrom, "datereqfrom", " and cast(o.datereqdel as date) = cast(@datereqfrom as date) ");
                }
                else if (!rs.datereqto.Equals(null) && rs.datereqfrom.Equals(null))
                {
                    cm.snapsCdn(rs.datereqto, "datereqto", " and cast(o.datereqdel as date) <= cast( @datereqto as date) ");
                }

                //Date order 
                if (!rs.dateorderfrom.Equals(null) && !rs.dateorderto.Equals(null))
                {
                    cm.snapsPar(rs.dateorderto, "dateorderto");
                    cm.snapsCdn(rs.dateorderfrom, "dateorderfrom", " and cast(o.dateorder as date) between cast(@dateorderfrom as date) and cast(@dateorderto as date) ");
                }
                else if (!rs.dateorderfrom.Equals(null) && rs.dateorderto.Equals(null))
                {
                    cm.snapsCdn(rs.dateorderfrom, "dateorderfrom", " and cast(o.dateorder as date) = cast(@dateorderfrom as date) ");
                }
                else if (!rs.dateorderto.Equals(null) && rs.dateorderfrom.Equals(null))
                {
                    cm.snapsCdn(rs.dateorderto, "dateorderto", " and cast(o.dateorder as date) <= cast( @dateorderto as date) ");
                }

                //Date prep
                if (!rs.dateprepfrom.Equals(null) && !rs.dateprepto.Equals(null))
                {
                    cm.snapsPar(rs.dateprepto, "dateprepto");
                    cm.snapsCdn(rs.dateprepfrom, "dateprepfrom", " and cast(o.dateprep as date) between cast(@dateprepfrom as date) and cast(@dateprepto as date) ");
                }
                else if (!rs.dateprepfrom.Equals(null) && rs.dateprepto.Equals(null))
                {
                    cm.snapsCdn(rs.dateprepfrom, "dateprepfrom", " and cast(o.dateprep as date) = cast(@dateprepfrom as date) ");
                }
                else if (!rs.dateprepto.Equals(null) && rs.dateprepfrom.Equals(null))
                {
                    cm.snapsCdn(rs.dateprepto, "dateprepto", " and cast(o.dateprep as date) <= cast( @dateprepto as date) ");
                }

                cm.snapsCdn(rs.ispending, "", " and exists (select 1 from wm_outbouln l where o.orgcode = l.orgcode and o.site = l.site and o.depot = l.depot and o.ouorder = l.ouorder and isnull(qtypndsku,0) > 0 ) ");

                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn.Add(outbound_getls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) {
                logger.Error(rs.orgcode, rs.site, rs.depot, ex, ex.Message);
                throw ex;
            }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }

        public async Task<List<outbound_ls>> finddist(outbound_pm rs)
        {
            //outbound_sqlfnddist
            SqlCommand cm = new SqlCommand();
            List<outbound_ls> rn = new List<outbound_ls>();
            SqlDataReader r = null;
            try
            {
                /* Vlidate parameter */
                cm = outbound_sqlfnddist.snapsCommand(cn);
                cm.snapsPar(rs.orgcode, "orgcode");
                cm.snapsPar(rs.site, "site");
                cm.snapsPar(rs.depot, "depot");
                cm.snapsCdn(rs.spcarea, "spcarea", " and o.spcarea = @spcarea ");
                cm.snapsCdn(rs.outype, "outype", " and outype = @outype ");
                cm.snapsCdn(rs.ousubtype, "ousubtype", " and ousubtype = @ousubtype ");
                cm.snapsCdn(rs.tflow, "tflow", " and o.tflow = @tflow ");
                cm.snapsCdn(rs.ouorder, "ouorder", " and o.ouorder = @ouorder ");
                cm.snapsCdn(rs.thcode, "thcode", " and o.thcode = @thcode ");
                cm.snapsCdn(rs.inorder, "inorder", " and inorder = @inorder ");
                //cm.snapsCdn(rs.ouorder,"ouorder"," and ouorder = @ouorder ");
                cm.snapsCdn(rs.huno, "huno", " and o.dishuno = @huno");
                if (rs.thcode.notNull()) { cm.snapsCdn(rs.thcode, "thcode", string.Format(" and ( disthcode like '%{0}%' or thname like '%{0}%' )", rs.thcode.ClearReg())); }
                if (rs.article.notNull()) { cm.snapsCdn(rs.thcode, "thcode", string.Format(" and ( disproduct like '%{0}%' or disproductdsc like '%{0}%' )", rs.thcode.ClearReg())); }
                cm.snapsCdn(rs.oupromo, "oupromo", " and oupromo = @oupromo");
                if (rs.ouremarks.notNull()) { cm.snapsCdn(rs.ouremarks, "ouremarks", string.Format(" and remarks like '%{0}%' ", rs.ouremarks.ClearReg())); }
                //if (rs.oupriority != 100){ cm.snapsCdn(rs.oupriority.ToString(),"oupriority"," and o.oupriority " + ((rs.oupriority == 1) ? " > 0 ": " = 0")); }
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn.Add(outbound_getls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) {
                logger.Error(rs.orgcode, rs.site, rs.depot, ex, ex.Message);
                throw ex;
            }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }
        public async Task<outbound_md> get(outbound_ls rs)
        {
            SqlCommand cm = new SqlCommand(sqloutbound_orderline, cn);
            SqlDataReader r = null;
            outbound_md rn = new outbound_md();
            try
            {
                /* Vlidate parameter */
                cm.snapsPar(rs.orgcode, "orgcode");
                cm.snapsPar(rs.site, "site");
                cm.snapsPar(rs.depot, "depot");
                cm.snapsPar(rs.spcarea, "spcarea");
                cm.snapsCdn(rs.ouorder, "ouorder", " and o.ouorder = @ouorder ");

                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn = outbound_getmd(ref r); }
                await r.CloseAsync();
                rn.lines = new List<outbouln_md>();
                cm.CommandText = outbouln_sqlfnd + " and o.ouorder = @ouorder order by ouln";
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn.lines.Add(outbouln_getmd(ref r)); }
                await r.CloseAsync();

                await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) {
                logger.Error(rs.orgcode, rs.site, rs.depot, ex, ex.Message);
                throw ex; 
            }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }
        public async Task<outbound_md> getdist(outbound_ls rs)
        {
            SqlCommand cm = new SqlCommand(outbound_sqlfnddist, cn);
            SqlDataReader r = null;
            outbound_md rn = new outbound_md();
            try
            {
                /* Vlidate parameter */
                cm.snapsPar(rs.orgcode, "orgcode");
                cm.snapsPar(rs.site, "site");
                cm.snapsPar(rs.depot, "depot");
                cm.snapsPar(rs.spcarea, "spcarea");
                cm.snapsCdn(rs.ouorder, "ouorder", " and o.ouorder = @ouorder ");
                cm.snapsPar(rs.ouorder, "inorder");
                cm.snapsPar(rs.disproduct, "article");

                // r = await cm.snapsReadAsync(); 
                // while(await r.ReadAsync()) { rn = outbound_getmd(ref r); }
                // await r.CloseAsync(); 

                rn.lines = new List<outbouln_md>();
                cm.CommandText = outbound_sqlgetdist;
                r = await cm.snapsReadAsync();
                while (await r.ReadAsync()) { rn.lines.Add(outbouln_getmd(ref r)); }
                await r.CloseAsync();

                await cn.CloseAsync();
                return rn;
            }
            catch (Exception ex) {
                logger.Error(rs.orgcode, rs.site, rs.depot, ex, ex.Message); 
                throw ex;
            }
            finally { if (cm != null) { cm.Dispose(); } if (r != null) { await r.DisposeAsync(); } }
        }

        public async Task upsert(List<outbound_md> rs)
        {
            List<SqlCommand> cm = new List<SqlCommand>();
            Int32 ix = 0;
            try
            {
                foreach (outbound_md ln in rs)
                {
                    cm.Add(outbound_setmd(ln, outbound_sqlvld));
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? outbound_sqlins : outbound_sqlupd;
                }
                await cm.snapsExecuteTransAsync(cn);
            }
            catch (Exception ex)
            {
                logger.Error(rs.FirstOrDefault().orgcode, rs.FirstOrDefault().site, rs.FirstOrDefault().accnmodify, ex, ex.Message);
                throw ex;
            }
            finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task upsert(outbound_md rs)
        {
            List<outbound_md> ro = new List<outbound_md>();
            try
            {
                ro.Add(rs); await upsert(ro);
            }
            catch (Exception ex)
            {
                logger.Error(rs.orgcode, rs.site, rs.accnmodify,ex, ex.Message);
                throw ex;
            }
            finally { ro.Clear(); }
        }
        public async Task remove(List<outbound_md> rs)
        {
            List<SqlCommand> cm = new List<SqlCommand>();
            try
            {
                foreach (outbound_md ln in rs)
                {
                    cm.Add(outbound_setmd(ln, outbound_sqlrem));
                }
                await cm.snapsExecuteTransAsync(cn);
            }
            catch (Exception ex)
            {
                logger.Error(rs.FirstOrDefault().orgcode, rs.FirstOrDefault().site, rs.FirstOrDefault().accnmodify, ex.Message);
                throw ex;
            }
            finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task remove(outbound_md rs)
        {
            List<outbound_md> ro = new List<outbound_md>();
            try
            {
                ro.Add(rs); await remove(ro);
            }
            catch (Exception ex)
            {
                logger.Error(rs.orgcode, rs.site, rs.accnmodify,ex, ex.Message);
                throw ex;
            }
            finally { ro.Clear(); }
        }
        public async Task import(List<outbound_ix> rs)
        {
            List<SqlCommand> cm = new List<SqlCommand>();
            Int32 ix = 0;
            try
            {
                foreach (outbound_ix ln in rs)
                {
                    cm.Add(outbound_setix(ln, outbound_sqlvld));
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? outbound_sqlinx : outbound_sqlupd;
                }
                await cm.snapsExecuteTransAsync(cn);
            }
            catch (Exception ex)
            {
                logger.Error(rs.FirstOrDefault().orgcode, rs.FirstOrDefault().site, "",ex, ex.Message);
                throw ex;
            }
            finally { cm.ForEach(x => x.Dispose()); }
        }
        public async Task setPriority(outbound_md o)
        {
            SqlCommand cm = new SqlCommand(sqloutbound_setpriority, cn);
            try
            {
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");
                cm.snapsPar(o.spcarea, "spcarea");
                cm.snapsPar(o.ouorder, "ouorder");
                cm.snapsPar(o.accnmodify, "accnmodify");
                cm.snapsPar("wms.outbound.setpriority", "procmodify");
                await cm.snapsExecuteAsync();
            }
            catch (Exception ex) {
                logger.Error(o.orgcode, o.site, o.accnmodify,ex, ex.Message);
                throw ex; 
            }
            finally { cm.Dispose(); }
        }

        public async Task changeRequest(outbound_md o)
        {
            SqlCommand cm = new SqlCommand(sqloutbound_changedelivery, cn);
            try
            {
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");
                cm.snapsPar(o.spcarea, "spcarea");
                cm.snapsPar(o.ouorder, "ouorder");
                cm.snapsPar(o.datereqdel, "datereqdel");
                cm.snapsPar(o.accnmodify, "accnmodify");
                cm.snapsPar("wms.outbound.changerequest", "procmodify");
                await cm.snapsExecuteAsync();
            }
            catch (Exception ex) {
                logger.Error(o.orgcode, o.site, o.accnmodify, ex, ex.Message);
                throw ex; 
            }
            finally { cm.Dispose(); }
        }

        public async Task setremarks(outbound_md o)
        {
            SqlCommand cm = new SqlCommand(sqloutbound_setremarks, cn);
            try
            {
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");
                cm.snapsPar(o.spcarea, "spcarea");
                cm.snapsPar(o.ouorder, "ouorder");
                cm.snapsPar(o.ouremarks, "ouremarks");
                cm.snapsPar(o.accnmodify, "accnmodify");
                cm.snapsPar("wms.outbound.setremarks", "procmodify");
                await cm.snapsExecuteAsync();
            }
            catch (Exception ex) {
                logger.Error(o.orgcode, o.site, o.accnmodify, ex, ex.Message);
                throw ex; 
            }
            finally { cm.Dispose(); }
        }

        public async Task setlineorder(outbouln_md o)
        {
            SqlCommand cm = new SqlCommand(sqloutbound_setlinebatch, cn);
            try
            {
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");
                cm.snapsPar(o.spcarea, "spcarea");
                cm.snapsPar(o.ouorder, "ouorder");
                cm.snapsPar(o.ouln, "ouln");
                cm.snapsPar(o.qtyreqpu, "qtyreqpu");
                cm.snapsPar(o.dateexp, "dateexp");
                cm.snapsPar(o.batchno, "batchno");
                cm.snapsPar(o.serialno, "serialno");
                cm.snapsPar(o.accnmodify, "accnmodify");
                cm.snapsPar("wms.outbound.setlineorder", "procmodify");
                await cm.snapsExecuteAsync();
            }
            catch (Exception ex) { 
                logger.Error(o.orgcode, o.site, o.accnmodify, ex, ex.Message);
                throw ex; 
            }
            finally { cm.Dispose(); }
        }

        //cancel an order - 
        public async Task setcancel(outbound_md o)
        {
            SqlCommand cm = new SqlCommand(sqloutbound_ordercancel, cn);
            try
            {
                if (string.IsNullOrEmpty(o.ouremarks))
                {
                    throw new Exception("Remarks is request");
                }
                else
                {
                    cm.snapsPar(o.orgcode, "orgcode");
                    cm.snapsPar(o.site, "site");
                    cm.snapsPar(o.depot, "depot");
                    cm.snapsPar(o.spcarea, "spcarea");
                    cm.snapsPar(o.ouorder, "ouorder");
                    cm.snapsPar(o.ouremarks, "ouremarks");
                    cm.snapsPar(o.accnmodify, "accnmodify");
                    cm.snapsPar("wms.outbound.cancel", "procmodify");
                    await cm.snapsExecuteAsync();
                }
            }
            catch (Exception ex) {
                logger.Error(o.orgcode, o.site, o.accnmodify, ex, ex.Message);
                throw ex; 
            }
            finally { cm.Dispose(); }
        }

    }

}