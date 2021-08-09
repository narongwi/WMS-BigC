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

    public class taln_ops : IDisposable { 
        private SqlConnection cn = null;
        private static string tbn = "wm_taln";
        private static string sqlmcom = " orgcode = @orgcode and site = @site  and depot = @depot  and spcarea = @spcarea  and taskno = @taskno  and taskseq = @taskseq" ;
        private String sqlins = "insert into " + tbn + 
        " ( orgcode,site, depot, spcarea, taskno, taskseq, article, pv, lv, sourceloc, sourcehuno, targetadv, targetloc, targethuno, " +
        " targetqty, collectloc, collecthuno, collectqty, accnassign, accnwork, accnfill, accncollect, dateassign, datework, " + 
        " datefill, datecollect, iopromo, ioreftype, iorefno, lotno, datemfg, dateexp, serialno, tflow, datecreate, accncreate, " + 
        " datemodify, accnmodify, procmodify ) " + 
        " values "  +
        " ( @orgcode, @site, @depot, @spcarea, @taskno, @taskseq, @article, @pv, @lv, @sourceloc, @sourcehuno, @targetadv, null, null, " +
        " null, null, null, null, null, null, null, null,null, null, " +
        " null, null, @iopromo, @ioreftype, @iorefno, @lotno, @datemfg, @dateexp, @serialno, @tflow, @sysdate, @accncreate, "+ 
        " @sysdate, @accnmodify, @procmodify ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " article = @article, pv = @pv, lv = @lv, sourceloc = @sourceloc, sourcehuno = @sourcehuno, targetadv = @targetadv, targetloc = @targetloc, " +
        " targethuno = @targethuno, targetqty = @targetqty, collectloc = @collectloc, collecthuno = @collecthuno, collectqty = @collectqty, " +
        " accnassign = @accnassign, accnwork = @accnwork, accnfill = @accnfill, accncollect = @accncollect, dateassign = @dateassign, "+ 
        " datework = @datework, datefill = @datefill, datecollect = @datecollect, iopromo = @iopromo, ioreftype = @ioreftype, iorefno = @iorefno, " +
        " lotno = @lotno, datemfg = @datemfg, dateexp = @dateexp, serialno = @serialno, tflow = @tflow,  "+ 
        "  datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " + 
        " where 1=1 " + sqlmcom;        
        private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from " + tbn + " where 1=1 ";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        public taln_ops() {  }
        public taln_ops(String cx) { cn = new SqlConnection(cx); }

        private taln_ls fillls(ref SqlDataReader r) { 
            return new taln_ls() { 
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                taskno = r["taskno"].ToString(),
                taskseq = r.GetInt32(5),
                article = r["article"].ToString(),
                sourceloc = r["sourceloc"].ToString(),
                sourcehuno = r["sourcehuno"].ToString(),
                targetadv = r["targetadv"].ToString(),
                targethuno = r["targethuno"].ToString(),
                iopromo = r["iopromo"].ToString(),
                ioreftype = r["ioreftype"].ToString(),
                iorefno = r["iorefno"].ToString(),
                datemodify = (r.IsDBNull(14)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(14),
                tflow = r["tflow"].ToString(),
                accnassign = r["accnassign"].ToString(),
                accnwork = r["accnwork"].ToString(),
            };
        }
        private taln_ix fillix(ref SqlDataReader r) { 
            return new taln_ix() { 

            };
        }
        private taln_md fillmdl(ref SqlDataReader r) { 
            return new taln_md() {
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                taskno = r["taskno"].ToString(),
                taskseq = r.GetInt32(5),
                article = r["article"].ToString(),
                pv = r.GetInt32(7),
                lv = r.GetInt32(8),
                sourceloc = r["sourceloc"].ToString(),
                sourcehuno = r["sourcehuno"].ToString(),
                targetadv = r["targetadv"].ToString(),
                targetloc = r["targetloc"].ToString(),
                targethuno = r["targethuno"].ToString(),
                targetqty = r.GetInt32(14),
                collectloc = r["collectloc"].ToString(),
                collecthuno = r["collecthuno"].ToString(),
                collectqty = r.GetInt32(17),
                accnassign = r["accnassign"].ToString(),
                accnwork = r["accnwork"].ToString(),
                accnfill = r["accnfill"].ToString(),
                accncollect = r["accncollect"].ToString(),
                dateassign = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22),
                datework = (r.IsDBNull(23)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(23),
                datefill = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24),
                datecollect = (r.IsDBNull(25)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(25),
                iopromo = r["iopromo"].ToString(),
                ioreftype = r["ioreftype"].ToString(),
                iorefno = r["iorefno"].ToString(),
                lotno = r["lotno"].ToString(),
                datemfg = (r.IsDBNull(30)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(30),
                dateexp = (r.IsDBNull(31)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(31),
                serialno = r["serialno"].ToString(),
                tflow = r["tflow"].ToString(),
                datecreate = (r.IsDBNull(34)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(34),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(36)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(36),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
                descalt = r["descalt"].ToString(),
            };
        }
        private SqlCommand ixcommand(taln_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);

            return cm;
        }
        private SqlCommand obcommand(taln_md o,String sql){ 
            SqlCommand cm = new SqlCommand(sql,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.taskno,"taskno");
            cm.snapsPar(o.taskseq,"taskseq");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.sourceloc,"sourceloc");
            cm.snapsPar(o.sourcehuno,"sourcehuno");
            cm.snapsPar(o.targetadv,"targetadv");
            cm.snapsPar(o.targetloc,"targetloc");
            cm.snapsPar(o.targethuno,"targethuno");
            cm.snapsPar(o.targetqty,"targetqty");
            cm.snapsPar(o.collectloc,"collectloc");
            cm.snapsPar(o.collecthuno,"collecthuno");
            cm.snapsPar(o.collectqty,"collectqty");
            cm.snapsPar(o.accnassign,"accnassign");
            cm.snapsPar(o.accnwork,"accnwork");
            cm.snapsPar(o.accnfill,"accnfill");
            cm.snapsPar(o.accncollect,"accncollect");
            cm.snapsPar(o.dateassign,"dateassign");
            cm.snapsPar(o.datework,"datework");
            cm.snapsPar(o.datefill,"datefill");
            cm.snapsPar(o.datecollect,"datecollect");
            cm.snapsPar(o.iopromo,"iopromo");
            cm.snapsPar(o.ioreftype,"ioreftype");
            cm.snapsPar(o.iorefno,"iorefno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.datecreate,"datecreate");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.datemodify,"datemodify");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            return cm;
        }


        public async Task<taln_md> get(taln_ls rs){ 
            SqlCommand cm = null; SqlDataReader r = null;
            taln_md rn = new taln_md();
            String sqlpam = "";
            try { 
                /* Vlidate parameter */
                cm = (sqlfnd).snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsPar(rs.spcarea,"spcarea");
                cm.snapsPar(rs.taskno,"taskno");
                cm.snapsPar(rs.taskseq,"taskseq");

                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} sqlpam = null; if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<taln_md> rs){ 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (taln_md ln in rs) {
                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(taln_md rs){ 
            List<taln_md> ro = new List<taln_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<taln_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (taln_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(taln_md rs){
            List<taln_md> ro = new List<taln_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<taln_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (taln_ix ln in rs) {
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
