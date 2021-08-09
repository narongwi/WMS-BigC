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

namespace Snaps.WMS.warehouse {

    public partial class locdw_ops : IDisposable { 
        private SqlConnection cn = null;

        public locdw_ops() {  }
        public locdw_ops(String cx) { cn = new SqlConnection(cx); }

        public async Task<List<locdw_ls>> find(locdw_pm rs) { 
            SqlCommand cm = new SqlCommand("",cn);
            List<locdw_ls> rn = new List<locdw_ls>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                 cm = sqlfnd.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");

                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.fltype,"fltype");
                cm.snapsCdn(rs.lszone,"lszone");
                //cm.snapsCdn(rs.lsaisle,"lsaisle");
                cm.snapsCdn(rs.lsbay,"lsbay");
                cm.snapsCdn(rs.lslevel,"lslevel");
                cm.snapsCdn(rs.lsloc,"lsloc");
                if (rs.lscode.notNull()) { cm.snapsCdn(rs.lscode.Replace("-",""),"lscode"); }
                cm.snapsCdn(rs.lsloctype,"lsloctype");
                cm.snapsCdn(rs.spcthcode,"spcthcode");
                cm.snapsCdn(rs.spchuno,"spchuno");
                cm.snapsCdn(rs.spcarticle,"spcarticle");
                //cm.snapsCdn(rs.spcblock,nameof(rs.spcblock));
                cm.snapsCdn(rs.spctaskfnd,"spctaskfnd");
                //cm.snapsCdn(rs.spcseqpath,nameof(rs.spcseqpath));
                //cm.snapsCdn(rs.spclasttouch,"spclasttouch");
                cm.snapsCdn(rs.spcpivot,"spcpivot");
                cm.snapsCdn(rs.spcpickunit,"spcpickunit");
                cm.snapsCdn(rs.tflow,"tflow");

                if (rs.aislefrom.notNull() && rs.aisleto.notNull()) { 
                    cm.snapsCdn(rs.aislefrom,"aislefrom",string.Format(" and lsaisle between '{0}' and '{1}'", rs.aislefrom,rs.aisleto)); }
                else if (rs.aislefrom.notNull()) { 
                    cm.snapsCdn(rs.aislefrom,"aislefrom"," and lsaisle = @aislefrom");
                }
                if (rs.bayfrom.notNull() && rs.bayto.notNull()) { 
                    cm.snapsCdn(rs.bayfrom,"bayfrom",string.Format(" and lsbay between '{0}' and '{1}'", rs.bayfrom,rs.bayto)); }
                else if (rs.aislefrom.notNull()) { 
                    cm.snapsCdn(rs.bayfrom,"bayfrom"," and lsbay = @bayfrom");
                }
                if (rs.levelfrom.notNull() && rs.levelto.notNull()) { 
                    cm.snapsCdn(rs.levelfrom,"levelfrom",string.Format(" and lslevel between '{0}' and '{1}'", rs.levelfrom,rs.levelto)); }
                else if (rs.levelfrom.notNull()) { 
                    cm.snapsCdn(rs.levelfrom,"levelfrom"," and lslevel = @levelfrom");
                }
                
                cm.snapsCdn(rs.mixproduct,"lsmixarticle");
                cm.snapsCdn(rs.mixaging,"lsmixage");
                cm.snapsCdn(rs.mixlot,"lsmixlotno");
                if (rs.ispicking.notNull() && rs.ispicking == "1") { 
                    cm.snapsCdn("1","ispicking","and ( isnull(spcpicking,'0') = 1 or exists (select 1 from wm_loczp z where l.orgcode = z.orgcode and l.site = z.site and l.depot = z.depot and l.lscode = z.lscode) )");
                }
                if (rs.isreserve.notNull() && rs.isreserve == "1") { 
                    cm.snapsCdn("1","isreserve","and ( isnull(spcpicking,'0') = 0 or not exists (select 1 from wm_loczp z where l.orgcode = z.orgcode and l.site = z.site and l.depot = z.depot and l.lscode = z.lscode) )");
                }
                cm.snapsCdn(rs.spcthcode,"spcthcode");
                cm.snapsCdn(rs.spcproduct,"spcproduct");
                cm.snapsCdn(rs.lasttourch,"spclasttouch");
                
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<List<locdw_pivot>> findpivot(locdw_pm rs) { 
            SqlCommand cm = new SqlCommand("",cn);
            List<locdw_pivot> rn = new List<locdw_pivot>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn("ST","spcarea");
                cm.snapsCdn("LC","fltype");
                cm.snapsCdn(rs.lszone,"lszone");
                cm.snapsCdn(rs.lsaisle,"lsaisle");
                cm.snapsCdn(rs.lsbay,"lsbay");
                cm.snapsCdn(rs.lslevel,"lslevel");
                cm.snapsCdn(rs.lsloc,"lsloc");
                cm.snapsCdn(rs.tflow,"tflow");               
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillpivotls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<List<locdw_picking>> findpicking(locdw_pm rs) { 
            SqlCommand cm = new SqlCommand("",cn);
            List<locdw_picking> rn = new List<locdw_picking>();
            SqlDataReader r = null;
            try { 
                 /* Vlidate parameter */
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn("ST",nameof(rs.spcarea));
                cm.snapsCdn("LC",nameof(rs.fltype));
                cm.snapsCdn(rs.lszone,"lszone");
                cm.snapsCdn(rs.lsaisle,"lsaisle");
                cm.snapsCdn(rs.lsbay,"lsbay");
                cm.snapsCdn(rs.lslevel,"lslevel");
                cm.snapsCdn(rs.lsloc,"lsloc");
                cm.snapsCdn(rs.tflow,"tflow");    
                cm.snapsCdn("1","spcpicking");                    
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(fillpickingls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
        }

        public async Task<locdw_md> get(locdw_ls rs){ 
            SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
            locdw_md rn = new locdw_md();
            try { 
                /* Vlidate parameter */
                cm = sqlfnd.snapsCommand(cn);
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsPar(rs.site,"site");
                cm.snapsPar(rs.depot,"depot");
                cm.snapsCdn(rs.spcarea,"spcarea");
                cm.snapsCdn(rs.fltype,"fltype");
                cm.snapsCdn(rs.lscode,"lscode");
                cm.snapsCdn(rs.lscodealt,"lscodealt");
                cm.snapsCdn(rs.lszone,"lszone");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = fillmdl(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn; 
            }
            catch (Exception ex) { throw ex; }
            finally { if(cm!=null) { cm.Dispose();}  if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task upsert(List<locdw_md> rs){ 
            SqlCommand lm = new SqlCommand(sqllocup_validate_isexists,cn);
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (locdw_md ln in rs) {

                    if (ln.tflow == "NW") {
                        //ln.lscodefull = ln.orgcode + "-" + ln.site + "-" + ln.depot + 
                        //((ln.lszone.notNull()) ? "-" + ln.lszone : "") + 
                        //((ln.lsaisle.notNull()) ? "-" + ln.lsaisle : "") + 
                        //((ln.lsbay.notNull()) ? "-" + ln.lsbay : "") + 
                        //((ln.lslevel.notNull()) ? "-" + ln.lslevel : "");
                        lm.Parameters.Clear();
                        lm.snapsPar(ln.orgcode,"orgcode");
                        lm.snapsPar(ln.site,"site");
                        lm.snapsPar(ln.depot,"depot");
                        lm.snapsPar(ln.lscode,"lscode");
                        if (lm.snapsScalarStrAsync().Result != "0"){
                            throw new Exception("location has exists !");
                        }
                    }

                    cm.Add(obcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd;
                    ix++; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsert(locdw_md rs){ 
            List<locdw_md> ro = new List<locdw_md>(); 
            try { 
                ro.Add(rs); await upsert(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task remove(List<locdw_md> rs){
            List<SqlCommand> cm = new List<SqlCommand>(); 
            try { 
                foreach (locdw_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task remove(locdw_md rs){
            List<locdw_md> ro = new List<locdw_md>(); 
            try { 
                ro.Add(rs); await remove(ro); 
            }catch (Exception ex) { 
                throw ex;
            } finally { ro.Clear(); }
        }
        public async Task import(List<locdw_ix> rs) { 
            List<SqlCommand> cm = new List<SqlCommand>(); 
            Int32 ix=0;
            try { 
                foreach (locdw_ix ln in rs) {
                    cm.Add(ixcommand(ln,sqlval)); 
                    cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
                }
                await cm.snapsExecuteTransAsync(cn);
            }catch (Exception ex) { 
                throw ex;
            } finally { cm.ForEach(x=>x.Dispose()); }
        }

        public async Task setpivot(locdw_pivot o,String accncode) { 
            SqlCommand cm = new SqlCommand("",cn); 
            String sql = "update wm_locdw set spcpivot = @spcpivot, accnmodify = @accncode, datemodify = @sysdate, procmodify = 'setpivot' " + 
            " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and lscode = @lscode " ;
            try { 
                cm.CommandText = sql;
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcpivot,"spcpivot");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.lscode,"lscode");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();                
                await cm.snapsExecuteAsync(true);
            }catch (Exception ex) { 
                throw ex;
            } finally { }
        }

        public async Task setpicking(locdw_picking o,String accncode) { 
            SqlCommand cm = new SqlCommand("",cn); 
            String sql = "update wm_locdw set spcpickunit = @spcpickunit, spcrpn = @spcrpn, spcarticle = @spcarticle, lsmnsafety = @lsmnsafety, " +
            " accnmodify = @accncode, datemodify = @sysdate, procmodify = 'setpicking', spcseqpath = @spcseqpath" + 
            " where orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and lscode = @lscode " ;
            try { 
                cm.CommandText = sql;
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.spcpickunit,"spcpickunit");
                cm.snapsPar(o.spcrpn,"spcrpn");
                cm.snapsPar(o.spcarticle,"spcarticle");
                cm.snapsPar(o.lsmnsafety,"lsmnsafety");
                cm.snapsPar(o.spcseqpath,"spcseqpath");
                cm.snapsPar(o.spcarea,"spcarea");
                cm.snapsPar(o.lscode,"lscode");
                cm.snapsPar(accncode,"accncode");
                cm.snapsParsysdateoffset();  
                await cm.snapsExecuteAsync(true);
            }catch (Exception ex) { 
                throw ex;
            } finally { }
        }

        /* Generate location */
        public async Task generate(locdw_gn o){ 
            //List<SqlCommand> cm = new List<SqlCommand>();
            locdw_ops op = new locdw_ops(cn.ConnectionString);
            List<locdw_md> lm = new List<locdw_md>();
            SqlCommand om = new SqlCommand("",cn);
            DataTable dt = new DataTable();
            string sqlfnd = "SELECT orgcode,site,depot,spcarea,fltype,lscode,lsformat,lsseq,lszone,lsaisle,lsbay,lslevel,tflow " + 
            " FROM wm_locup where lszone = @zone and lsaisle between @aislefr and @aisleto and lsbay between @bayfr and @bayto and lslevel between @levelfr and @levelto";
            string[] lsstacks = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", };

            try
            { 
                om.CommandText = sqlfnd;
                om.Parameters.Add(o.zone.snapsPar("zone"));
                om.Parameters.Add(o.aislefr.snapsPar("aislefr"));
                om.Parameters.Add(o.aisleto.snapsPar("aisleto"));
                om.Parameters.Add(o.bayfr.snapsPar("bayfr"));
                om.Parameters.Add(o.bayto.snapsPar("bayto"));                
                om.Parameters.Add(o.levelfr.snapsPar("levelfr"));
                om.Parameters.Add(o.levelto.snapsPar("levelto"));

                dt = await om.snapsTableAsync();
                o.lsremarks = (o.lsremarks==null) ? "" : o.lsremarks;
                o.spcarea = (o.spcarea==null) ? "" : o.spcarea;
                o.spchuno = (o.spchuno==null) ? "" : o.spchuno;
                o.spclasttouch = (o.spclasttouch==null) ? "" : o.spclasttouch;
                o.spcpickunit = (o.spcpickunit==null) ? "" : o.spcpickunit;
                o.spcpivot = (o.spcpivot==null) ? "" : o.spcpivot;
                o.spctaskfnd = (o.spctaskfnd==null) ? "" : o.spctaskfnd;
                o.spcthcode = (o.spcthcode==null) ? "" : o.spcthcode;
                o.tflow = (o.tflow==null) ? "" : o.tflow;
                o.spcarticle = (o.spcarticle==null) ? "" : o.spcarticle;
                o.spcrpn = (o.spcrpn==null) ? "" : o.spcrpn;
                o.location = (o.location == 0 ? 1 : o.location);
                foreach (DataRow r in dt.Rows) {
                    for (int i = 0; i < o.location; i++){
                        string stacklabel = o.lsstacklabel.ToUpper() == "AZ" 
                            ? lsstacks[i]: (i + 1).ToString();

                        lm.Add(new locdw_md());  
                        lm.Last().accncreate = o.accncode;
                        lm.Last().accnmodify = o.accncode;
                        lm.Last().crfreepct = 100;
                        lm.Last().crvolume = 0;
                        lm.Last().crweight = 0;
                        lm.Last().datecreate = new DateTimeOffset();
                        lm.Last().datemodify = new DateTimeOffset();
                        lm.Last().depot = o.depot;
                        lm.Last().fltype = "LC";
                        lm.Last().lsaisle = r["lsaisle"].ToString();
                        lm.Last().lsbay = r["lsbay"].ToString();
                        lm.Last().lsloc = "1";
                        lm.Last().lscode = r["depot"].ToString().PadLeft(2, '0') + r["lszone"].ToString() + r["lsaisle"].ToString() + r["lsbay"].ToString() + r["lslevel"].ToString() + stacklabel;;
                        lm.Last().lscodealt = r["depot"].ToString().PadLeft(2, '0') + r["lszone"].ToString() + "-" + r["lsaisle"].ToString() + "-" + r["lsbay"].ToString() + "-" + r["lslevel"].ToString() + "-" + stacklabel;
                        lm.Last().lscodefull = r["site"].ToString() + "-" + r["depot"].ToString().PadLeft(2, '0') + r["lszone"].ToString() + r["lsaisle"].ToString() + r["lsbay"].ToString() + r["lslevel"].ToString() + stacklabel;
                        lm.Last().lscodeid = o.orgcode + "-" + o.site + "-" + o.depot + "-" + o.zone + "-" + r["lsaisle"].ToString() + "-"+ r["lsaisle"].ToString() + "-"+r["lsbay"].ToString()+"-"+r["lslevel"].ToString()+"-" + stacklabel;
                        lm.Last().lsdesc = "";
                        lm.Last().lsdigit = "";
                        lm.Last().lsgapbuttom = o.lsgapbuttom;
                        lm.Last().lsgapleft = o.lsgapleft;
                        lm.Last().lsgapright = o.lsgapright;
                        lm.Last().lsgaptop = o.lsgaptop;
                        lm.Last().lshash = "000";
                        lm.Last().lslevel = r["lslevel"].ToString();
                        lm.Last().lsloctype = "LC";
                        lm.Last().lsdmlength = o.lsmxlength + o.lsgapleft;
                        lm.Last().lsdmwidth = o.lsmxwidth + o.lsgapright;
                        lm.Last().lsdmheight = o.lsmxheight + o.lsgaptop;
                        lm.Last().lsmixage = o.lsmixage;
                        lm.Last().lsmixarticle = o.lsmixarticle;
                        lm.Last().lsmixlotno = o.lsmixlotno;
                        lm.Last().lsmnsafety = o.lsmnsafety;
                        lm.Last().lsmxheight = o.lsmxheight;
                        lm.Last().lsmxhuno = o.lsmxhuno; 
                        lm.Last().lsmxlength = o.lsmxlength;
                        lm.Last().lsmxvolume = o.lsmxvolume;
                        lm.Last().lsmxweight = o.lsmxweight;
                        lm.Last().lsmxwidth = o.lsmxwidth;
                        lm.Last().lsremarks = o.lsremarks; 
                        lm.Last().lsstack = "1";
                        lm.Last().lsstackable = o.lsstackable;
                        lm.Last().lszone = r["lszone"].ToString();
                        lm.Last().orgcode = o.orgcode;
                        lm.Last().procmodify = "locgenerate";
                        lm.Last().site = o.site.ToString();
                        lm.Last().spcarea = o.spcarea.ToString(); 
                        lm.Last().spcarticle = o.spcarticle.ToString();
                        lm.Last().spcblock = o.spcblock; 
                        lm.Last().spchuno = o.spchuno.ToString();
                        lm.Last().spclasttouch = o.spclasttouch.ToString(); 
                        lm.Last().spcpickunit = o.spcpickunit.ToString(); 
                        lm.Last().spcpicking = o.spcpicking; 
                        lm.Last().spcpivot = o.spcpivot.ToString(); 
                        lm.Last().spcseqpath = o.spcseqpath;
                        lm.Last().spctaskfnd = o.spctaskfnd.ToString();
                        lm.Last().spcthcode = o.spcthcode.ToString();
                        lm.Last().tflow = "NW";// o.tflow.ToString();  
                        lm.Last().tflowcnt = o.tflow.ToString();
                        lm.Last().spcrpn = o.spcrpn;
                    }// end generate stack

                }
                await op.upsert(lm);
            }catch(Exception ex) { throw ex; }
            finally { }
        }
        public async Task generate(locdw_gngrid o) {
            locdw_ops op = new locdw_ops(cn.ConnectionString);
            List<locdw_md> lm = new List<locdw_md>();
            SqlCommand cm = new SqlCommand(" select ",cn);
            Int32 lastsq  = 0;
            try { 
                lastsq = o.lsseq;
                for(int i = 0; i <= o.location; ++i ) { 
                    lastsq += 1;
                    lm.Add(new locdw_md());  
                    lm.Last().accncreate = o.accncode;
                        lm.Last().accnmodify = o.accncode;
                        lm.Last().crfreepct = 100;
                        lm.Last().crvolume = 0;
                        lm.Last().crweight = 0;
                        lm.Last().datecreate = new DateTimeOffset();
                        lm.Last().datemodify = new DateTimeOffset();
                        lm.Last().depot = o.depot;
                        lm.Last().fltype = "LC";
                        lm.Last().lsaisle = "0";
                        lm.Last().lsbay = "0";
                        lm.Last().lscode = o.zone + lastsq.ToString().PadLeft(5,'0');
                        lm.Last().lsloc = "1";
                        //lm.Last().lscodealt = r["depot"].ToString().PadLeft(2, '0') + "-" + r["lszone"].ToString() + "-" + r["lsaisle"].ToString() + "-" + r["lsbay"].ToString() + "-" + r["lslevel"].ToString() + "-A";
                        lm.Last().lscodealt = o.zone + lastsq.ToString().PadLeft(5,'0');
                        lm.Last().lscodefull = o.orgcode + "-" + o.site + "-" + o.depot + "-" + o.zone + i.ToString().PadLeft(5,'0');
                        lm.Last().lscodeid = o.orgcode + "-" + o.site + "-" + o.depot + "-" + o.zone + i.ToString().PadLeft(5,'0');
                        lm.Last().lsdesc = "";
                        lm.Last().lsdigit = "";
                        lm.Last().lsgapbuttom = 0;
                        lm.Last().lsgapleft = 0;
                        lm.Last().lsgapright = 0;
                        lm.Last().lsgaptop = 0;
                        lm.Last().lshash = "000";
                        lm.Last().lslevel = "0";
                        lm.Last().lsloctype = "LC";
                        lm.Last().lsdmlength = 0;
                        lm.Last().lsdmwidth = 0;
                        lm.Last().lsdmheight = 0;
                        lm.Last().lsmixage = 1;
                        lm.Last().lsmixarticle = o.lsmixarticle;
                        lm.Last().lsmixlotno = 1;
                        lm.Last().lsmnsafety = 99999999;
                        lm.Last().lsmxheight = 99999999;
                        lm.Last().lsmxhuno = o.lsmxhuno; 
                        lm.Last().lsmxlength = 99999999;
                        lm.Last().lsmxvolume = o.lsmxvolume;
                        lm.Last().lsmxweight = o.lsmxweight;
                        lm.Last().lsmxwidth = 99999999;
                        lm.Last().lsremarks = ""; 
                        lm.Last().lsstackable = 0;
                        lm.Last().lsstacklimit = 99999999;
                        lm.Last().lszone = o.zone;
                        lm.Last().orgcode = o.orgcode;
                        lm.Last().procmodify = "locgenerate";
                        lm.Last().site = o.site.ToString();
                        lm.Last().spcarea = o.spcarea.ToString(); 
                        lm.Last().spcarticle = ""; 
                        lm.Last().spcblock = 0; 
                        lm.Last().spchuno = ""; 
                        lm.Last().spclasttouch = ""; 
                        lm.Last().spcpickunit = ""; 
                        lm.Last().spcpicking = 1; 
                        lm.Last().spcpivot = ""; 
                        lm.Last().spcseqpath = i;
                        lm.Last().spctaskfnd = "";
                        lm.Last().spcthcode = (o.spcthcode.notNull()) ? o.spcthcode.ToString() : null;  
                        lm.Last().tflow = o.tflow.ToString();  
                        lm.Last().tflowcnt = o.tflow.ToString();
                        lm.Last().spcrpn = "";
                        lm.Last().lsstack = "0";
                }
                await op.upsert(lm);
            }catch(Exception ex) { throw ex; }
            finally { }
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
