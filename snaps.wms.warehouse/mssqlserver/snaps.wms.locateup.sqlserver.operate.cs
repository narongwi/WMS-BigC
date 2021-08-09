using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers.Hash;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using Snaps.WMS;
using Snaps.Helpers.Logger;

namespace Snaps.WMS.warehouse {

    public partial class locup_ops : IDisposable {
        private readonly ISnapsLogger logger;
        private SqlConnection cn = null;
        private static string tbn = "wm_locup";
        private static string sqlmcom = " and orgcode = @orgcode and site = @site  and depot = @depot and fltype = @fltype and lscodeid = @lscodeid " ;

        private string sqllocup_validate_isexists = " select isnull((select count(1) from wm_locup where lscode = @lscode and orgcode = @orgcode and site = @site  and depot = @depot ) ,0) ";

        private string sqllocup_validate_isexists_zone = "select isnull((select count(1) from wm_locup where fltype = 'ZN' and lscode = @lscode and orgcode = @orgcode and site = @site  and depot = @depot) ,0)";
        private string sqllocup_validate_isexists_aisle = "select isnull((select count(1) from wm_locup where fltype = 'AL' and lszone = @lszone and lscode = @lscode and orgcode = @orgcode and site = @site  and depot = @depot) ,0)";
        private string sqllocup_validate_isexists_bay = "select isnull((select count(1) from wm_locup where fltype = 'BA' and lszone = @lszone and lsaisle = @lsaisle and lscode = @lscode and orgcode = @orgcode and site = @site  and depot = @depot) ,0)";
        private string sqllocup_validate_isexists_level = "select isnull((select count(1) from wm_locup where fltype = 'LV' and lszone = @lszone and lsaisle = @lsaisle and lsbay = @lsbay and lscode = @lscode and orgcode = @orgcode and site = @site  and depot = @depot) ,0)";
        private String sqlins = "insert into " + tbn + 
        " ( orgcode,site, depot, spcarea, fltype,  lscode, lsformat, lsseq, lscodealt, lscodefull, lszone, lsaisle, lsbay, lslevel, " + 
        "   crweight, crvolume, crlocation, crfreepct, tflow, tflowcnt, lshash, datecreate, accncreate, datemodify, accnmodify, procmodify,lsdesc,lscodeid ) " + 
        " values "  +
        " ( @orgcode, @site, @depot, @spcarea, @fltype,  @lscode, @lsformat, @lsseq, @lscodealt, @lscodefull, @lszone, @lsaisle, @lsbay, " + 
        "   @lslevel, @crweight, @crvolume, @crlocation, @crfreepct, 'IO', 'IO', @lshash, SYSDATETIMEOFFSET(), @accncreate, SYSDATETIMEOFFSET(), @accnmodify, @procmodify,@lsdesc, NEXT VALUE FOR seq_locdw ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " lsformat = @lsformat, lsseq = @lsseq, lscodealt = @lscodealt, lscodefull = @lscodefull, lszone = @lszone, lsaisle = @lsaisle, " + 
        " lsbay = @lsbay, lslevel = @lslevel, crweight = @crweight, crvolume = @crvolume, crlocation = @crlocation, crfreepct = @crfreepct, tflow = @tflow, " + 
        " tflowcnt = @tflowcnt, lshash = @lshash, datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify, " + 
        " procmodify = @procmodify, lsdesc = @lsdesc " + 
        " where 1=1 " + sqlmcom;        
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where orgcode = @orgcode and site = @site and depot = @depot ";
        private String sqlfndzone = "select * from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and fltype = 'ZN' and spcarea in ('ST','XD') ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public locup_ops() {  } 
        public locup_ops(String cx) {
            cn = new SqlConnection(cx);
            // logger
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            logger = snapsLogFactory.Create<locup_ops>();
        }

        //Lov zone 
        public async Task<List<lov>> LovLocdw(string type, string orgcode,string site,string depot,string lszone ="",string lsaisle = "", string lsbay = "", string lslevel = "") { 
            SqlCommand cm = new SqlCommand("",cn);
            List<lov> rn = new List<lov>();
            SqlDataReader r = null;
            try { 
                type = type.ToLower();
                if (type == "zone")       { cm.CommandText = sqllov_zone;   }
                else if (type == "aisle") { cm.CommandText = sqllov_aisle;  }
                else if (type == "bay")   { cm.CommandText = sqllov_bay;    }
                else if (type == "level") { cm.CommandText = sqllov_level;  }
                else if (type == "location") { cm.CommandText = sqllov_location; }    
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(lszone,"lszone");
                cm.snapsPar(lsaisle,"lsaisle");
                cm.snapsPar(lsbay,"lsbay");
                cm.snapsPar(lslevel,"lsleve");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { 
                    rn.Add(new lov(r["lscode"].ToString(), r["bndescalt"].ToString(),r["spcarea"].ToString(),"")); 
                }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }

        public async Task<List<locup_ls>> finddist(locup_pm rs) { 
            SqlCommand cm = new SqlCommand("",cn);
            List<locup_ls> rn = new List<locup_ls>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsPar(rs.fltype,"fltype");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(new locup_ls() { 
                    lscode = r["lscode"].ToString(),
                    lscodealt = r["lscodealt"].ToString()
                }); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }

        public async Task<List<locup_md>> findzone(locup_pm rs) { 
            SqlCommand cm = new SqlCommand(sqlfndzone,cn);
            List<locup_md> rn = new List<locup_md>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                //cm.snapsCdn(rs.spcarea,"spcarea");
                //cm.snapsCdn(rs.fltype,"fltype");
                cm.snapsCdn(rs.tflow,"tflow");
                cm.snapsCdn(rs.tflowcnt,"tflowcnt");
                cm.CommandText += " order by spcarea, lszone, lsaisle, lsbay, lslevel , lscode";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillmdl(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<List<locup_md>> find(locup_pm rs) { 
            SqlCommand cm = new SqlCommand("",cn);
            List<locup_md> rn = new List<locup_md>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                 cm = sqlfnd.snapsCommand(cn);

                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.fltype,"fltype");
                cm.snapsCdn(rs.lscode,"lscode");
                cm.snapsCdn(rs.lszone,"lszone");
                cm.snapsCdn(rs.lsaisle,"lsaisle");
                cm.snapsCdn(rs.lsbay,"lsbay");
                cm.snapsCdn(rs.lslevel,"lslevel");
                cm.snapsCdn(rs.tflow,"tflow");
                cm.snapsCdn(rs.tflowcnt,"tflowcnt");
                cm.CommandText += " order by lszone, lsaisle, lsbay, lslevel , lscode";
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillmdl(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<locup_md> get(locup_ls rs){ 
            SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
            locup_md rn = new locup_md();
            try { 
                /* Vlidate parameter */
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                
                cm.snapsCdn(rs.spcarea,"rs.spcarea");
                cm.snapsCdn(rs.fltype,"rs.fltype");
                cm.snapsCdn(rs.lscode,"rs.lscode");
                cm.snapsCdn(rs.lscodeid,"rs.lscodeid");

                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<locup_md> rs){ 
            SqlCommand lm = new SqlCommand(sqllocup_validate_isexists,cn);
            List<SqlCommand> cm = new List<SqlCommand>(); 
            sequence_ops so = new sequence_ops();
            Int32 ix=0;
            try { 
                foreach (locup_md ln in rs) {
                    if (ln.tflow == "NW") { 
                        ln.lscodefull = ln.orgcode + "-" + ln.site + "-" + ln.depot + 
                        ((ln.lszone.notNull()) ? "-" + ln.lszone : "") + 
                        ((ln.lsaisle.notNull()) ? "-" + ln.lsaisle : "") + 
                        ((ln.lsbay.notNull()) ? "-" + ln.lsbay : "") + 
                        ((ln.lslevel.notNull()) ? "-" + ln.lslevel : "");
                        lm.snapsPar(ln.orgcode,"orgcode");
                        lm.snapsPar(ln.site,"site");
                        lm.snapsPar(ln.depot,"depot");
                        lm.snapsPar(ln.lscode,"lscode");
                        lm.snapsPar(ln.fltype,"fltype");

                        if (ln.fltype == "ZN") { 
                            lm.CommandText = sqllocup_validate_isexists_zone;
                            ln.lszone = ln.lscode;
                        }else if (ln.fltype == "AL") { 
                            lm.CommandText = sqllocup_validate_isexists_aisle;
                            ln.lsaisle = ln.lscode;
                        }else if (ln.fltype == "BA") { 
                            lm.CommandText = sqllocup_validate_isexists_bay;
                            ln.lsbay = ln.lscode;
                        }else if (ln.fltype == "LV") { 
                            lm.CommandText = sqllocup_validate_isexists_level;
                            ln.lslevel = ln.lscode;
                        }
                        lm.snapsPar(ln.lszone,"lszone");
                        lm.snapsPar(ln.lsaisle,"lsaisle");
                        lm.snapsPar(ln.lsbay,"lsbay");
                        lm.snapsPar(ln.lslevel,"lslevel");

                        if (lm.snapsScalarStrAsync().Result != "0"){
                            throw new Exception("location has exists !");
                        }
                    }
                    ln.lshash = ln.lscodefull.ToHash();
                    cm.Add(obcommand(ln,sqlval)); 
                    //cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                    cm[ix].CommandText = ln.tflow != "NW" ? sqlupd : sqlins; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(locup_md rs){ 
            List<locup_md> ro = new List<locup_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<locup_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (locup_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(locup_md rs){
            List<locup_md> ro = new List<locup_md>();
            SqlCommand cm = new SqlCommand("",cn);
            try { 
                cm.CommandText = valRemcmd(rs.spcarea, rs.fltype);
                cm.snapsPar(rs.orgcode, "orgcode"  );
                cm.snapsPar(rs.site,    "site"     );
                cm.snapsPar(rs.depot,   "depot"    );
                cm.snapsPar(rs.spcarea, "spcarea"  );
                cm.snapsPar(rs.fltype,  "fltype"   );
                cm.snapsPar(rs.lszone,  "lszone"   );
                cm.snapsPar(rs.lsaisle, "lsaisle"  );
                cm.snapsPar(rs.lsbay,   "lsbay"    );
                cm.snapsPar(rs.lslevel, "lslevel"  );
                cm.snapsPar(rs.lscode,  "lscode"   );
                if (cm.snapsScalarStrAsync().Result != "0") { 
                    throw new Exception("Current location still using");
                }else { 
                    ro.Add(rs); 
                    await remove(ro); 
                }
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }

        string valRemcmd(string spcarea, string fltype){ 
            if (spcarea == "ST") {
                if      (fltype == "ZN")    { return sqlvalzonefree;  }
                else if (fltype == "AL")    { return sqlvalaislefree; }
                else if (fltype == "BA")    { return sqlvalbayfree;   }
                else    { return sqlvallevelfree; }
            }else { 
                return sqlvalzonefree;
            }   
        }
        public async Task import(List<locup_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (locup_ix ln in rs) {
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

        public String getMessage(String Lang,String Ercode){ 
            SqlCommand cm = new SqlCommand(string.Format("select ISNULL((select descmsg from wm_message where apps = 'WMS' and typemsg = 'ER'" + 
            " and langmsg = '{0}' and codemsg = '{1}'),'{1}')",Lang,Ercode),cn);
            try { 
                return cm.snapsScalarStrAsync().Result;
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose();}
        }

    }

}
