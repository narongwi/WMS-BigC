
using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers.DbContext.SQLServer;
namespace Snaps.WMS
{
    public partial class sequence_ops : IDisposable {
        public sequence_ops(SqlConnection cn) { }
        public sequence_ops(ref SqlConnection cn){ this._cnx = cn.ConnectionString; }

        //Goods receipt number sequence
        public SqlCommand grnoNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqgrno_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string grnoNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = grnoNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string grnoSql() { return string.Format(sqlseq_msql,sqlseqgrno_msql); }

        //Goods receipt transaction sequence number 
        public SqlCommand grtxNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqgrtx_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string grtxNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = grtxNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string grtxSql() { return string.Format(sqlseq_msql,sqlseqgrtx_msql); }

        //Advance goods receive number sequence
        public SqlCommand agrnNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqagrn_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string agrnNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = agrnNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string agrnSql() { return string.Format(sqlseq_msql,sqlseqagrn_msql); }

        //Correction sequence number
        public SqlCommand corrNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqcorr_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string corrNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = corrNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string corrSql() { return string.Format(sqlseq_msql,sqlseqcorr_msql); }

        //Delivery note sequence number
        public SqlCommand dnnoNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqdnno_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string dnnoNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = dnnoNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string dnnoSql() { return string.Format(sqlseq_msql,sqlseqdnno_msql); }

        //Handerling unit sequence number
        public SqlCommand hunoNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqhuno_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string hunoNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = hunoNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string hunoSql() { return string.Format(sqlseq_msql,sqlseqhuno_msql); }

        //Task seqeunce number
        public SqlCommand taskNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqtask_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string taskNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = taskNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string taskSql() { return string.Format(sqlseq_msql,sqlseqtask_msql); }

        //Transport Note sequence number 
        public SqlCommand trnoNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqtrno_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string trnoNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = trnoNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string trnoSql() { return string.Format(sqlseq_msql,sqlseqtrno_msql); }

        //Location sequence no
        public SqlCommand locNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqloc_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string locNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = locNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string locSql() { return string.Format(sqlseq_msql,sqlseqloc_msql); }

        //Preparation sequence number 
        public SqlCommand prepNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqprep_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string prepNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = prepNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string prepSql() { return string.Format(sqlseq_msql,sqlseqprep_msql); }

        //Stock sequence number
        public SqlCommand stockNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqstock_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string stockNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = stockNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string stockSql() { return string.Format(sqlseq_msql,sqlseqstock_msql); }

        //Count task number 
        public SqlCommand cntkNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqcntk_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string cntkNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = cntkNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string cntkSql() { return string.Format(sqlseq_msql,sqlseqcntk_msql); }

        //Count task plan number
        public SqlCommand cnpnNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqcnpn_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string cnpnNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = cnpnNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string cnpnSql() { return string.Format(sqlseq_msql,sqlseqcnpn_msql); }

        //Route sequence number
        public SqlCommand routeNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqroute_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string routeNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = routeNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string routeSql() { return string.Format(sqlseq_msql,sqlseqroute_msql); }

        //Transfer no
        public SqlCommand transferNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqtransfer_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string transferNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = transferNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string transferSql() { return string.Format(sqlseq_msql,sqlseqtransfer_msql); }

        //Preparartion set        
        public SqlCommand prepsetNext_command(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(string.Format(sqlseq_msql,sqlseqpreset_msql),cn);
            try { return cm; }
            catch (Exception ex) { throw ex;} 
        }
        public string prepsetNext(ref SqlConnection cn){ 
            SqlCommand cm = new SqlCommand(); 
            try { 
                cm = transferNext_command(ref cn);
                return cm.snapsScalarStrAsync().Result; 
            } catch (Exception ex) { throw ex; 
            } finally { cm.Dispose(); }
        }
        public string prepsetSql() { return string.Format(sqlseq_msql,sqlseqpreset_msql); }
    }
}