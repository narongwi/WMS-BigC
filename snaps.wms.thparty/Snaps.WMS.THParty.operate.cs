using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.THParty {
   
    public class thparty_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_thparty";
        private static string sqlmcom = "  " ;
        private String sqlins = "insert into " + tbn + " " +
        " ( orgcode, site, depot, spcarea, thtype, thbutype, thcode, thcodealt, vatcode, thname, thnamealt, thnameint, addressln1, addressln2, " +
        " addressln3, subdistrict, district, city, country, postcode, region, telephone, email, thgroup, thcomment, throuteformat, " + 
        " plandelivery, naturalloss, mapaddress, tflow, datecreate, accncreate, datemodify, accnmodify, procmodify, indock, oudock ) values "  +
        " ( @orgcode, @site, @depot, @spcarea, @thtype, @thbutype, @thcode, @thcodealt, @vatcode, @thname, @thnamealt, @thnameint, @addressln1, " + 
        " @addressln2, @addressln3, @subdistrict, @district, @city, @country, @postcode, @region, @telephone, @email, @thgroup, @thcomment, " + 
        " @throuteformat, @plandelivery, @naturalloss, @mapaddress, @tflow, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify "+ 
        " @indock, @oudock )";
        private String sqlupd = " update " + tbn + " set " +
        " thtype = @thtype, thbutype = @thbutype,  thcodealt = @thcodealt, vatcode = @vatcode, thname = @thname, thnamealt = @thnamealt, "+ 
        " thnameint = @thnameint, addressln1 = @addressln1, addressln2 = @addressln2, addressln3 = @addressln3, subdistrict = @subdistrict, "+ 
        " district = @district, city = @city, country = @country, postcode = @postcode, region = @region, telephone = @telephone, email = @email, " +
        " thgroup = @thgroup, thcomment = @thcomment, throuteformat = @throuteformat, plandelivery = @plandelivery, naturalloss = @naturalloss, " +
        " mapaddress = @mapaddress, tflow = @tflow,   datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify," +
        " indock = @indock, oudock = @oudock,traveltime = @traveltime where orgcode = @orgcode and site = @site and depot = @depot and thcode = @thcode ";        
        private String sqlrem = " delete wm_thparty from where orgcode = @orgcode and site = @site and depot = @depot and thcode = @thcode "; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 "; 
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public thparty_ops() {  }
        public thparty_ops(String cx) { cn = new SqlConnection(cx); }

        private thparty_ls fillls(ref SqlDataReader r) { 
            return new thparty_ls() { 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                thtype = r["thtype"].ToString(),
                thbutype = r["thbutype"].ToString(),
                thcode = r["thcode"].ToString(),
                thname = r["thname"].ToString(),
                thgroup = r["thgroup"].ToString(),
                tflow = r["tflow"].ToString(),
                thnamealt =  r["thnamealt"].ToString(),
                thcodealt = r["thcodealt"].ToString(),
            };
        }
        private thparty_ix fillix(ref SqlDataReader r) { 
            thparty_ix rn = new thparty_ix(); 
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.thbutype = r["thbutype"].ToString();
            rn.thtype = r["thtype"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.thcodealt = r["thcodealt"].ToString();
            rn.vatcode = r["vatcode"].ToString();
            rn.thname = r["thname"].ToString();
            rn.thnamealt = r["thnamealt"].ToString();
            rn.thnameint = r["thnameint"].ToString();
            rn.addressln1 = r["addressln1"].ToString();
            rn.addressln2 = r["addressln2"].ToString();
            rn.addressln3 = r["addressln3"].ToString();
            rn.subdistrict = r["subdistrict"].ToString();
            rn.district = r["district"].ToString();
            rn.city = r["city"].ToString();
            rn.country = r["country"].ToString();
            rn.postcode = r["postcode"].ToString();
            rn.region = r["region"].ToString();
            rn.telephone = r["telephone"].ToString();
            rn.email = r["email"].ToString();
            rn.thgroup = r["thgroup"].ToString();
            rn.thcomment = r["thcomment"].ToString();
            rn.throuteformat = r["throuteformat"].ToString();
            rn.plandelivery = (r.IsDBNull(26)) ? 0 :  r.GetInt32(26);
            rn.naturalloss = (r.IsDBNull(27)) ? 0 :  r.GetInt32(27);
            rn.mapaddress = r["mapaddress"].ToString();
            rn.carriercode = r["carriercode"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.timeslotday = r["timeslotday"].ToString();
            rn.timeslothourmin = r["timeslothourmin"].ToString();
            rn.timeslothourmax = r["timeslothourmax"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.indock = r["indock"].ToString();
            rn.oudock = r["oudock"].ToString();
            rn.fileid =  r["fileid"].ToString();
            rn.rowops =  r["rowops"].ToString();
            rn.ermsg = r["ermsg"].ToString();
            return rn;    
        }
        private thparty_md fillmdl(ref SqlDataReader r) { 
            thparty_md rn = new thparty_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.thbutype = r["thbutype"].ToString();
            rn.thtype = r["thtype"].ToString();
            rn.thcode = r["thcode"].ToString();
            rn.thcodealt = r["thcodealt"].ToString();
            rn.vatcode = r["vatcode"].ToString();
            rn.thname = r["thname"].ToString();
            rn.thnamealt = r["thnamealt"].ToString();
            rn.thnameint = r["thnameint"].ToString();
            rn.addressln1 = r["addressln1"].ToString();
            rn.addressln2 = r["addressln2"].ToString();
            rn.addressln3 = r["addressln3"].ToString();
            rn.subdistrict = r["subdistrict"].ToString();
            rn.district = r["district"].ToString();
            rn.city = r["city"].ToString();
            rn.country = r["country"].ToString();
            rn.postcode = r["postcode"].ToString();
            rn.region = r["region"].ToString();
            rn.telephone = r["telephone"].ToString();
            rn.email = r["email"].ToString();
            rn.thgroup = r["thgroup"].ToString();
            rn.thcomment = r["thcomment"].ToString();
            rn.throuteformat = r["throuteformat"].ToString();
            rn.plandelivery = r["plandelivery"].ToString().CInt32();
            rn.naturalloss = r["naturalloss"].ToString().CInt32();
            rn.mapaddress = r["mapaddress"].ToString();
            rn.carriercode = r["carriercode"].ToString();
            rn.orbitsource = r["orbitsource"].ToString();
            rn.timeslotday = r["timeslotday"].ToString();
            rn.timeslothourmin = r["timeslothourmin"].ToString();
            rn.timeslothourmax = r["timeslothourmax"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(35)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(35);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(37)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(37);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.indock = r["indock"].ToString();
            rn.oudock = r["oudock"].ToString();
            rn.traveltime = r["traveltime"].ToString().CInt32();
            return rn;
        }
        private SqlCommand ixcommand(thparty_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.thbutype,"thbutype");
            cm.snapsPar(o.thtype,"thtype");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.thcodealt,"thcodealt");
            cm.snapsPar(o.vatcode,"vatcode");
            cm.snapsPar(o.thname,"thname");
            cm.snapsPar(o.thnamealt,"thnamealt");
            cm.snapsPar(o.thnameint,"thnameint");
            cm.snapsPar(o.addressln1,"addressln1");
            cm.snapsPar(o.addressln2,"addressln2");
            cm.snapsPar(o.addressln3,"addressln3");
            cm.snapsPar(o.subdistrict,"subdistrict");
            cm.snapsPar(o.district,"district");
            cm.snapsPar(o.city,"city");
            cm.snapsPar(o.country,"country");
            cm.snapsPar(o.postcode,"postcode");
            cm.snapsPar(o.region,"region");
            cm.snapsPar(o.telephone,"telephone");
            cm.snapsPar(o.email,"email");
            cm.snapsPar(o.thgroup,"thgroup");
            cm.snapsPar(o.thcomment,"thcomment");
            cm.snapsPar(o.throuteformat,"throuteformat");
            cm.snapsPar(o.plandelivery,"plandelivery");
            cm.snapsPar(o.naturalloss,"naturalloss");
            cm.snapsPar(o.mapaddress,"mapaddress");
            cm.snapsPar(o.carriercode,"carriercode");
            cm.snapsPar(o.orbitsource,"orbitsource");
            cm.snapsPar(o.timeslotday,"timeslotday");
            cm.snapsPar(o.timeslothourmin,"timeslothourmin");
            cm.snapsPar(o.timeslothourmax,"timeslothourmax");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.indock,"indock");
            cm.snapsPar(o.oudock,"oudock");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.fileid,"fileid");
            cm.snapsPar(o.rowops,"rowops");
            cm.snapsPar(o.ermsg,"ermsg");
            cm.snapsPar(o.dateops,"dateops");
            
            return cm;
        }
        private SqlCommand obcommand(thparty_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.thbutype,"thbutype");
            cm.snapsPar(o.thtype,"thtype");
            cm.snapsPar(o.thcode,"thcode");
            cm.snapsPar(o.thcodealt,"thcodealt");
            cm.snapsPar(o.vatcode,"vatcode");
            cm.snapsPar(o.thname,"thname");
            cm.snapsPar(o.thnamealt,"thnamealt");
            cm.snapsPar(o.thnameint,"thnameint");
            cm.snapsPar(o.addressln1,"addressln1");
            cm.snapsPar(o.addressln2,"addressln2");
            cm.snapsPar(o.addressln3,"addressln3");
            cm.snapsPar(o.subdistrict,"subdistrict");
            cm.snapsPar(o.district,"district");
            cm.snapsPar(o.city,"city");
            cm.snapsPar(o.country,"country");
            cm.snapsPar(o.postcode,"postcode");
            cm.snapsPar(o.region,"region");
            cm.snapsPar(o.telephone,"telephone");
            cm.snapsPar(o.email,"email");
            cm.snapsPar(o.thgroup,"thgroup");
            cm.snapsPar(o.thcomment,"thcomment");
            cm.snapsPar(o.throuteformat,"throuteformat");
            cm.snapsPar(o.plandelivery,"plandelivery");
            cm.snapsPar(o.naturalloss,"naturalloss");
            cm.snapsPar(o.mapaddress,"mapaddress");
            cm.snapsPar(o.carriercode,"carriercode");
            cm.snapsPar(o.orbitsource,"orbitsource");
            cm.snapsPar(o.timeslotday,"timeslotday");
            cm.snapsPar(o.timeslothourmin,"timeslothourmin");
            cm.snapsPar(o.timeslothourmax,"timeslothourmax");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.indock,"indock");
            cm.snapsPar(o.oudock,"oudock");
            cm.snapsPar(o.traveltime,"traveltime");
            return cm;
        }
        private SqlCommand oucommand(thparty_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.thcode,"thcode");
            return cm;
        }
        private SqlCommand oucommand(thparty_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.thcode,"thcode");
            return cm;
        }

        public async Task<List<thparty_ls>> find(thparty_pm rs) { 
            SqlCommand cm = null;
            List<thparty_ls> rn = new List<thparty_ls>();
            SqlDataReader r = null;
            try { 
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.thtype,"thtype");
                cm.snapsCdn(rs.thbutype,"thbutype");
                cm.snapsCdn(rs.thcode,"thcode");
                cm.snapsCdn(rs.thname,"thname");
                cm.snapsCdn(rs.telephone,"telephone");
                cm.snapsCdn(rs.email,"email");
                cm.snapsCdn(rs.mapaddress,"mapaddress");
                cm.snapsCdn(rs.tflow,"tflow");
                cm.snapsCdn(rs.searchall,"searchall",string.Format(" and (thcode like '%{0}%' or thname like '%{0}%' ) ", rs.searchall.ClearReg()) );
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<thparty_md> get(thparty_ls rs){ 
            SqlCommand cm = null; SqlDataReader r = null;
            thparty_md rn = new thparty_md();
            try { 
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsCdn(rs.orgcode,"orgcode");
                cm.snapsCdn(rs.site,"site");
                cm.snapsCdn(rs.depot,"depot");
                cm.snapsCdn(rs.thcode,"thcode");
                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<thparty_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (thparty_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(thparty_md rs){ 
            List<thparty_md> ro = new List<thparty_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<thparty_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (thparty_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(thparty_md rs){
            List<thparty_md> ro = new List<thparty_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<thparty_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (thparty_ix ln in rs) {
                    cm.Add(ixcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { 
                if (cn != null) { cn.Dispose(); } sqlval = null; sqlins = null; sqlupd = null; sqlrem = null; sqlfnd = null; tbn = null; sqlmcom = null;
            }
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }

}
