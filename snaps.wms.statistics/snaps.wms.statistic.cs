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

    public partial class statisic_ops : IDisposable {

        private String _cnx = "";
        public statisic_ops() { }
        public statisic_ops(ref SqlConnection cn) { this._cnx = cn.ConnectionString; }

        public SqlCommand inbound(String orgcode,String site,String depot,String inorder,String accncode) {
            SqlCommand cm = new SqlCommand();
            try {
                cm.CommandText = sqlinbound;
                cm.Parameters.Add(orgcode.snapsPar("orgcode"));
                cm.Parameters.Add(site.snapsPar("site"));
                cm.Parameters.Add(depot.snapsPar("depot"));
                cm.Parameters.Add(inorder.snapsPar("inorder"));
                cm.Parameters.Add(accncode.snapsPar("accnmodify"));
                cm.snapsParsysdateoffset();
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }

        //Update transction of receipt to order line 
        public SqlCommand inboundLine(String orgcode,String site,String depot,String inorder,string inln,String accncode) {
            SqlCommand cm = new SqlCommand();
            try {
                cm.CommandText = sqlinboundline;
                cm.Parameters.Add(orgcode.snapsPar("orgcode"));
                cm.Parameters.Add(site.snapsPar("site"));
                cm.Parameters.Add(depot.snapsPar("depot"));
                cm.Parameters.Add(inorder.snapsPar("inorder"));
                cm.Parameters.Add(inln.snapsPar("inln"));
                cm.Parameters.Add(accncode.snapsPar("accnmodify"));
                cm.snapsParsysdateoffset();
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }
        //update stock 
        public List<SqlCommand> inboundstock(String orgcode,String site,String depot,String article,string pv,string lv) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                cm.Add(new SqlCommand(sqlcalculate_stock_clear));
                cm.Add(new SqlCommand(sqlcalculate_stock_info));
                cm.ForEach(e => {
                    e.snapsPar(orgcode,"orgcode");
                    e.snapsPar(site,"site");
                    e.snapsPar(depot,"depot");
                    e.snapsPar(article,"article");
                    e.snapsPar(pv,"pv");
                    e.snapsPar(lv,"lv");
                });
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }


        //Task movement 
        public List<SqlCommand> taskmovement(string orgcode,string site,string depot,string sourcelocation,string targetlocation,string article,string pv,string lv) {
            List<SqlCommand> cm = new List<SqlCommand>();
            try {
                cm.Add(sqllocation.snapsCommand());
                cm.Add(sqllocation.snapsCommand());
                cm.Add(sqlcalculate_stock_clear.snapsCommand());
                cm.Add(sqlcalculate_stock_info.snapsCommand());

                foreach(SqlCommand e in cm) {
                    e.snapsPar(orgcode,"orgcode");
                    e.snapsPar(site,"site");
                    e.snapsPar(depot,"depot");
                    e.snapsPar(article,"article");
                    e.snapsPar(pv,"pv");
                    e.snapsPar(lv,"lv");
                }
                cm[0].snapsPar(sourcelocation,"loccode");
                cm[1].snapsPar(targetlocation,"loccode");
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }


        //Stock on Hand 
        //for inbound
        public SqlCommand stockonhand(string orgcode,string site,string depot,string article,Int32 pv,Int32 lv) {
            return stockcalulate(sqlcalonhand,orgcode,site,depot,article,pv,lv);
        }

        //Stock available 
        //public SqlCommand stockavailable(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalonavai,orgcode,site,depot,article,pv,lv);
        //}

        ////Stock Bulk n Return 
        //public SqlCommand stockbulknreturn(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalbulknrtn,orgcode,site,depot,article,pv,lv);
        //}

        ////Stock overflow
        //public SqlCommand stockoverflow(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcaloverflow,orgcode,site,depot,article,pv,lv);
        //}

        ////Stock Preparation
        //public SqlCommand stockprep(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalprep,orgcode,site,depot,article,pv,lv);
        //}
        ////Stock staging
        //public SqlCommand stockstaging(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalstaging,orgcode,site,depot,article,pv,lv);
        //}
        ////Stock rtv
        //public SqlCommand stockrtv(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalrtv,orgcode,site,depot,article,pv,lv);
        //}
        ////Stock sinbin
        //public SqlCommand stocksinbin(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalsinbin,orgcode,site,depot,article,pv,lv);
        //}
        ////Stock damage
        //public SqlCommand stockdamage(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcaldamage,orgcode,site,depot,article,pv,lv);
        //} 
        ////Stock block
        //public SqlCommand stockblock(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalblock,orgcode,site,depot,article,pv,lv);
        //}
        ////Stock exchange
        //public SqlCommand stockexchange(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcalexchange,orgcode,site,depot,article,pv,lv);
        //}
        ////Stock task
        //public SqlCommand stocktask(string orgcode, string site, string depot, string article, Int32 pv, Int32 lv){ 
        //    return stockcalulate(sqlcaltask,orgcode,site,depot,article,pv,lv);
        //}        

        private SqlCommand stockcalulate(string sqlcmd,string orgcode,string site,string depot,string article,Int32 pv,Int32 lv) {
            SqlCommand cm = new SqlCommand();
            try {
                cm.CommandText = sqlcmd;
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(article,"article");
                cm.snapsPar(pv,"pv");
                cm.snapsPar(lv,"lv");
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }


        public List<SqlCommand> snapsstock(string orgcode,string site,string depot,string article,Int32 pv,Int32 lv) {
            List<SqlCommand> rn = new List<SqlCommand>();
            rn.Add(stockcalulate(sqlcalculate_stock_clear,orgcode,site,depot,article,pv,lv));
            rn.Add(stockcalulate(sqlcalculate_stock_info,orgcode,site,depot,article,pv,lv));
            return rn;
        }
        public List<SqlCommand> correctionstock(string orgcode,string site,string depot,string article,Int32 pv,Int32 lv) {
            List<SqlCommand> rn = new List<SqlCommand>();
            rn.Add(stockcalulate(sqlcalculate_stock_clear,orgcode,site,depot,article,pv,lv));
            rn.Add(stockcalulate(sqlcalculate_stock_info,orgcode,site,depot,article,pv,lv));
            return rn;
        }

        //Location 
        public SqlCommand location(String orgcode,String site,String depot,String loccode) {
            SqlCommand cm = new SqlCommand();
            try {
                cm.CommandText = sqllocation;
                cm.Parameters.Add(orgcode.snapsPar("orgcode"));
                cm.Parameters.Add(site.snapsPar("site"));
                cm.Parameters.Add(depot.snapsPar("depot"));
                cm.Parameters.Add(loccode.snapsPar("loccode"));
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }

        //route
        public SqlCommand route(string orgcode,string site,string depot,string routeno,string accnmodify) {
            SqlCommand cm = new SqlCommand();
            try {
                cm.CommandText = sqlroute;
                cm.Parameters.Add(orgcode.snapsPar("orgcode"));
                cm.Parameters.Add(site.snapsPar("site"));
                cm.Parameters.Add(depot.snapsPar("depot"));
                cm.Parameters.Add(routeno.snapsPar("routeno"));
                cm.Parameters.Add(accnmodify.snapsPar("accnmodify"));
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }
        public SqlCommand routeforcast(string orgcode,string site,string depot,string routeno,string accnmodify,SqlConnection cn = null) {
            SqlCommand cm = new SqlCommand();
            try {
                if(cn != null) { cm.Connection = cn; }
                cm.CommandText = sqlcal_routeforcast;
                cm.Parameters.Add(orgcode.snapsPar("orgcode"));
                cm.Parameters.Add(site.snapsPar("site"));
                cm.Parameters.Add(depot.snapsPar("depot"));
                cm.Parameters.Add(routeno.snapsPar("routeno"));
                cm.Parameters.Add(accnmodify.snapsPar("accnmodify"));
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }

        //HU 
        public SqlCommand HUforcast(string orgcode,string site,string depot,string huno,string accnmodify) {
            SqlCommand cm = new SqlCommand();
            try {
                cm.CommandText = sqlcal_huforcast;
                cm.Parameters.Add(orgcode.snapsPar("orgcode"));
                cm.Parameters.Add(site.snapsPar("site"));
                cm.Parameters.Add(depot.snapsPar("depot"));
                cm.Parameters.Add(huno.snapsPar("huno"));
                cm.Parameters.Add(accnmodify.snapsPar("accnmodify"));
                return cm;
            } catch(Exception ex) { throw ex; } finally { }
        }

        public void Hourly(string ocn) {
            //SqlConnection cn = new SqlConnection(ocn);
            //SqlCommand cm = new SqlCommand();
            using(SqlConnection cn = new SqlConnection(ocn))
            using(SqlCommand cm = new SqlCommand()) {
                cn.Open();
                cm.Connection = cn;
                cm.Parameters.AddWithValue("orgcode","bgc");
                cm.Parameters.AddWithValue("site","91917");
                cm.Parameters.AddWithValue("depot","01");

                // history backup
                cm.CommandText = sqlstocksnapshot_step1;
                cm.ExecuteNonQuery();

                // truncate table
                cm.CommandText = sqlstocksnapshot_step2;
                cm.ExecuteNonQuery();

                // insert data 
                cm.CommandText = sqlstocksnapshot_step3;
                cm.ExecuteNonQuery();

                cn.Close();
            }

        }
        public void Purgecurrent(string ocn) {
            using(SqlConnection cn = new SqlConnection(ocn))
            using(SqlCommand cm = new SqlCommand()) {
                cn.Open();
                cm.Connection = cn;
                cm.CommandType = System.Data.CommandType.StoredProcedure;
                cm.CommandText = "[dbo].[snaps_purge]";
                cm.ExecuteNonQuery();
                cn.Close();
            }

        }
        public void Purgehistory(string ocn) {
            using(SqlConnection cn = new SqlConnection(ocn))
            using(SqlCommand cm = new SqlCommand()) {
                cn.Open();
                cm.Connection = cn;
                cm.CommandType = System.Data.CommandType.StoredProcedure;
                cm.CommandText = "[dbo].[snaps_purge_history]";
                cm.ExecuteNonQuery();
                cn.Close();
            }

        }

        public void GeneratePrepRoute(string ocn) {
            using(SqlConnection cn = new SqlConnection(ocn))
            using(SqlCommand cm = new SqlCommand()) {
                cn.Open();   
                cm.Connection = cn;
                cm.CommandType = System.Data.CommandType.StoredProcedure;
                cm.CommandText = "[dbo].[snaps_gen_preproute]";
                cm.Parameters.AddWithValue("inorgcode","bgc");
                cm.Parameters.AddWithValue("insite","91918");
                cm.Parameters.AddWithValue("indepot","08");
                cm.ExecuteNonQuery();
                cn.Close();
            }

        }
    }

}