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
    public class docs_ls {
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String iotype { get; set; } 
        public String ioorder { get; set; } 
        public String iobarcode { get; set; } 
        public String ioopno { get; set; } 
        public String doctype { get; set; } 
        public String docno { get; set; } 
        public DateTime? issuedate { get; set; } 
        public DateTime? datemodify { get; set; } 
    }
    public class docs_pm : docs_ls { 
         public String docname { get; set; } 
    }
    public class docs_ix : docs_ls {

    }
    public class docs_md : docs_ls  {
        public String docname { get; set; } 
        public String owner { get; set; } 
        public String docremarks { get; set; } 
        public DateTime? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
    }
    public class docs_prc { 
        public String opsaccn { get; set; }
        public docs_md opsobj { get; set; } 
    }
    public class docs_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_document";
        private static string sqlmcom = " and site = @site and depot = @depot  and spcarea = @spcarea  and iotype = @iotype  and ioorder = @ioorder   and ioopno = @ioopno  and doctype = @doctype  and docno = @docno " ;
        private String sqlins = "insert into " + tbn + 
        " ( site, depot, spcarea, iotype, ioorder, iobarcode, ioopno, doctype, docno, docname, owner, issuedate, docremarks, datecreate, accncreate, datemodify, accnmodify, procmodify, ) " + 
        " values "  +
        " ( @site, @depot, @spcarea, @iotype, @ioorder, @iobarcode, @ioopno, @doctype, @docno, @docname, @owner, @issuedate, @docremarks, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify, ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " iobarcode = @iobarcode,    docname = @docname, owner = @owner, issuedate = @issuedate, docremarks = @docremarks,   datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify, " + 
        " where 1=1 " + sqlmcom;        
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 + ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public docs_ops() {  }
        public docs_ops(String cx) { cn = new SqlConnection(cx); }

        private docs_ls fillls(ref DbDataReader r) { 
            return new docs_ls() { 
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                iotype = r["iotype"].ToString(),
                ioorder = r["ioorder"].ToString(),
                iobarcode = r["iobarcode"].ToString(),
                ioopno = r["ioopno"].ToString(),
                doctype = r["doctype"].ToString(),
                docno = r["docno"].ToString(),
                issuedate = (r.IsDBNull(9)) ? (DateTime?) null : r.GetDateTime(9),
                datemodify = (r.IsDBNull(10)) ? (DateTime?) null : r.GetDateTime(10),
            };
        }
        private docs_ix fillix(ref DbDataReader r) { 
            return new docs_ix() { 

            };
        }
        private docs_md fillmdl(ref DbDataReader r) { 
            return new docs_md() {
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                iotype = r["iotype"].ToString(),
                ioorder = r["ioorder"].ToString(),
                iobarcode = r["iobarcode"].ToString(),
                ioopno = r["ioopno"].ToString(),
                doctype = r["doctype"].ToString(),
                docno = r["docno"].ToString(),
                docname = r["docname"].ToString(),
                owner = r["owner"].ToString(),
                issuedate = (r.IsDBNull(11)) ? (DateTime?) null : r.GetDateTime(11),
                docremarks = r["docremarks"].ToString(),
                datecreate = (r.IsDBNull(13)) ? (DateTime?) null : r.GetDateTime(13),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(15)) ? (DateTime?) null : r.GetDateTime(15),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
            };
        }
        private SqlCommand ixcommand(docs_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand obcommand(docs_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.spcarea.snapsPar());
            cm.Parameters.Add(o.iotype.snapsPar());
            cm.Parameters.Add(o.ioorder.snapsPar());
            cm.Parameters.Add(o.iobarcode.snapsPar());
            cm.Parameters.Add(o.ioopno.snapsPar());
            cm.Parameters.Add(o.doctype.snapsPar());
            cm.Parameters.Add(o.docno.snapsPar());
            cm.Parameters.Add(o.docname.snapsPar());
            cm.Parameters.Add(o.owner.snapsPar());
            cm.Parameters.Add(o.issuedate.snapsPar());
            cm.Parameters.Add(o.docremarks.snapsPar());
            cm.Parameters.Add(o.datecreate.snapsPar());
            cm.Parameters.Add(o.accncreate.snapsPar());
            cm.Parameters.Add(o.datemodify.snapsPar());
            cm.Parameters.Add(o.accnmodify.snapsPar());
            cm.Parameters.Add(o.procmodify.snapsPar());
            return cm;
        }

        public SqlCommand oucommand(docs_ls o,String sql,String accnmodify) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.spcarea.snapsPar());
            cm.Parameters.Add(o.iotype.snapsPar());
            cm.Parameters.Add(o.ioorder.snapsPar());
            cm.Parameters.Add(o.ioopno.snapsPar());
            cm.Parameters.Add(o.doctype.snapsPar());
            cm.Parameters.Add(o.docno.snapsPar());
            return cm;
        }
        public SqlCommand oucommand(docs_md o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.Parameters.Add(o.site.snapsPar());
            cm.Parameters.Add(o.depot.snapsPar());
            cm.Parameters.Add(o.spcarea.snapsPar());
            cm.Parameters.Add(o.iotype.snapsPar());
            cm.Parameters.Add(o.ioorder.snapsPar());
            cm.Parameters.Add(o.ioopno.snapsPar());
            cm.Parameters.Add(o.doctype.snapsPar());
            cm.Parameters.Add(o.docno.snapsPar());
            return cm;
        }

        public async Task<List<docs_ls>> find(docs_pm rs) { 
            SqlCommand cm = null;
            String sqlpam = "";
            List<docs_ls> rn = new List<docs_ls>();
            DbDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm.snapsCdn(rs.site);
                cm.snapsCdn(rs.depot);
                cm.snapsCdn(rs.spcarea);
                cm.snapsCdn(rs.ioorder);
                cm.snapsCdn(rs.iobarcode);
                cm.snapsCdn(rs.ioopno);
                cm.snapsCdn(rs.doctype);
                cm.snapsCdn(rs.docno);
                cm.snapsCdn(rs.docname);

                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<docs_md> get(docs_ls rs){ 
            SqlCommand cm = null; DbDataReader r = null;
            docs_md rn = new docs_md();
            String sqlpam = "";
            try { 
                /* Vlidate parameter */
                cm.snapsCdn(rs.site);
                cm.snapsCdn(rs.depot);
                cm.snapsCdn(rs.spcarea);
                cm.snapsCdn(rs.iotype);
                cm.snapsCdn(rs.ioorder);
                cm.snapsCdn(rs.ioopno);
                cm.snapsCdn(rs.doctype);
                cm.snapsCdn(rs.docno);

                cm = (sqlfnd).snapsCommand(cn);
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<docs_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (docs_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(docs_md rs){ 
            List<docs_md> ro = new List<docs_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<docs_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (docs_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(docs_md rs){
            List<docs_md> ro = new List<docs_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<docs_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (docs_ix ln in rs) {
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
