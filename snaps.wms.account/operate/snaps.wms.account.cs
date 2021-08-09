// using System;
// using System.Linq;
// using System.Data;
// using System.Data.Common;
// using System.Data.SqlClient;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Snaps.Helpers;
// using Snaps.Helpers.StringExt;
// using Snaps.Helpers.Hash;
// using Snaps.Helpers.Json;
// using Snaps.Helpers.DbContext.SQLServer;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Converters;
// using Microsoft.IdentityModel.Tokens;
// using System.Security.Claims;
// using System.Text;

// namespace Snaps.WMS {

//     public class accn_ops : IDisposable { 
 
//         private static string tbn = "wm_accn";
//         private static string sqlmcom = " and orgcode = @orgcode and accncode = @accncode " ;
//         private String sqlins = "insert into wm_accn" + tbn + 
//         " ( orgcode, accntype, accncode, accnname, email, mobileno, accnapline, dateexpire, tflow, accnavartar, tkrqpriv, cntfailure, datelogin, datelogout, datechnpriv, datecreate, accncreate, datemodify, accnmodify, procmodify ) " + 
//         " values "  +
//         " ( @orgcode, @accntype, @accncode, @accnname, @email, @mobileno, @accnapline, @dateexpire, @tflow, @accnavartar, @tkrqpriv, @cntfailure, @datelogin, @datelogout, @datechnpriv, @datecreate, @accncreate, @datemodify, @accnmodify, @procmodify ) ";
//         private String sqlupd = " update " + tbn + " set " + 
//         " accnname = @accnname, accnsurname = @accnsurname, accsrole = @accsrole, tflow = @tflow, datemodify = @datemodify, accnmodify = @accnmodify, procmodify = @procmodify " + 
//         " where 1=1 " + sqlmcom;        
//         private String sqlrem = "delete from " + tbn + " where 1=1 " + sqlmcom; 
//         private String sqlfnd = "select * from " + tbn + " where 1=1 ";
//         private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;

//         private String sqlsignup = "insert into wm_accn (accncode, email, tflow,datecreate,accncreate,procmodify) values (@accncode, @email,'PC',@sysdate,'signup','signup')";
//         private String sqlsignpv = "insert into wm_acpriv (accncode, accnpriv) values (@accncode, @password)";
//         public accn_ops() {  }
//         public accn_ops(String cx) { cn = new SqlConnection(cx); }


//         // private accn_ls fillls(ref SqlDataReader r) { 
//         //     return new accn_ls() { 
//         //         orgcode = r["orgcode"].ToString(),
//         //         accntype = r["accntype"].ToString(),
//         //         accncode = r["accncode"].ToString(),
//         //         accnname = r["accnname"].ToString() + "  " +  r["accnsurname"].ToString(),
//         //         email = r["email"].ToString(),
//         //         dateexpire = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8),
//         //         sessionexpire = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9),
//         //         tflow = r["tflow"].ToString(),
//         //         accnavartar = r["accnavartar"].ToString(),
//         //         formatdateshort = r["formatdateshort"].ToString(),
//         //         formatdatelong = r["formatdatelong"].ToString(),
//         //         formatdate = r["formatdate"].ToString()
//         //     };
//         // }
//         // private accn_ix fillix(ref SqlDataReader r) { 
//         //     return new accn_ix() { 

//         //     };
//         // }
//         // private accn_md fillmdl(ref SqlDataReader r) { 
//         //     return new accn_md() {
//         //         orgcode = r["orgcode"].ToString(),
//         //         accntype = r["accntype"].ToString(),
//         //         accncode = r["accncode"].ToString(),
//         //         accnname = r["accnname"].ToString(),
//         //         email = r["email"].ToString(),
//         //         mobileno = r["mobileno"].ToString(),
//         //         accnapline = r["accnapline"].ToString(),
//         //         dateexpire = (r.IsDBNull(8)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(8),
//         //         tflow = r["tflow"].ToString(),
//         //         accnavartar = r["accnavartar"].ToString(),
//         //         tkrqpriv = r["tkrqpriv"].ToString(),
//         //         cntfailure = r.GetInt32(13),
//         //         datelogin = (r.IsDBNull(15)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(15),
//         //         datelogout = (r.IsDBNull(16)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(16),
//         //         datechnpriv = (r.IsDBNull(17)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17),
//         //         datecreate = (r.IsDBNull(18)) ? (DateTimeOffset?) DateTime.Now : r.GetDateTimeOffset(18),
//         //         accncreate = r["accncreate"].ToString(),
//         //         datemodify = (r.IsDBNull(19)) ? (DateTimeOffset?) DateTime.Now : r.GetDateTimeOffset(19),
//         //         accnmodify = r["accnmodify"].ToString(),
//         //         procmodify = r["procmodify"].ToString(),
//         //         accnsurname = r["accnsurname"].ToString(),
//         //         accsrole = JsonConvert.DeserializeObject<lov>(r["accsrole"].ToString()),
//         //     };
//         // }
//         // private SqlCommand ixcommand(accn_ix o,String sql) { 
//         //     SqlCommand cm = new SqlCommand(sql,cn);

//         //     return cm;
//         // }
//         // private SqlCommand obcommand(accn_md o,String sql){ 
//         //     SqlCommand cm = new SqlCommand(sql,cn);
//         //     cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//         //     cm.Parameters.Add(o.accntype.snapsPar("accntype"));
//         //     cm.Parameters.Add(o.accncode.snapsPar("accncode"));
//         //     cm.Parameters.Add(o.accnname.snapsPar("accnname"));
//         //     cm.Parameters.Add(o.accnsurname.snapsPar("accnsurname"));
//         //     cm.Parameters.Add(o.email.snapsPar("email"));
//         //     cm.Parameters.Add(o.mobileno.snapsPar("mobileno"));
//         //     cm.Parameters.Add(o.accnapline.snapsPar("accnapline"));
//         //     //cm.Parameters.Add(o.dateexpire.snapsPar("dateexpire"));
//         //     cm.Parameters.Add(o.tflow.snapsPar("tflow"));
//         //     cm.Parameters.Add(o.accnavartar.snapsPar("accnavartar"));
//         //     //cm.Parameters.Add(o.tkrqpriv.snapsPar("tkrqpriv"));
//         //     //cm.Parameters.Add(o.cntfailure.snapsPar("cntfailure"));
//         //     //cm.Parameters.Add(o.datelogin.snapsPar("datelogin"));
//         //     //cm.Parameters.Add(o.datelogout.snapsPar("datelogout"));
//         //     //cm.Parameters.Add(o.datechnpriv.snapsPar("datechnpriv"));
//         //     cm.Parameters.Add(o.datecreate.snapsPar("datecreate"));
//         //     cm.Parameters.Add(o.accncreate.snapsPar("accncreate"));
//         //     cm.Parameters.Add(o.datemodify.snapsPar("datemodify"));
//         //     cm.Parameters.Add(o.accnmodify.snapsPar("accnmodify"));
//         //     cm.Parameters.Add(o.procmodify.snapsPar("procmodify"));
//         //     cm.Parameters.Add(o.accsrole.toJson().snapsPar("accsrole"));
//         //     return cm;
//         // }

//         // public SqlCommand oucommand(accn_ls o,String sql,String accnmodify) { 
//         //     SqlCommand cm = new SqlCommand(sql,cn);
//         //     cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//         //     cm.Parameters.Add(o.accntype.snapsPar("accntype"));
//         //     cm.Parameters.Add(o.accncode.snapsPar("accncode"));
//         //     return cm;
//         // }
//         // public SqlCommand oucommand(accn_md o,String sql) { 
//         //     SqlCommand cm = new SqlCommand(sql,cn);
//         //     cm.Parameters.Add(o.orgcode.snapsPar("orgcode"));
//         //     cm.Parameters.Add(o.accntype.snapsPar("accntype"));
//         //     cm.Parameters.Add(o.accncode.snapsPar("accncode"));
//         //     return cm;
//         // }


        
//         // private async Task<accn_ls> get(String accscode) { 
//         //     SqlCommand cm = new SqlCommand("",cn); SqlDataReader r = null;
//         //     accn_ls rn = new accn_ls();
//         //     try { 
//         //         /* Vlidate parameter */
//         //         cm = (sqlfnd + " and accscode = @accscode and sessionexpire > @sysdate and dateexpire > @sysdate ").snapsCommand(cn);
//         //         cm.snapsPar(accscode,nameof(accscode));
//         //         cm.snapsParsysdateoffset();
//         //         r = await cm.snapsReadAsync();
//         //         while(await r.ReadAsync()) { rn = fillls(ref r); }
//         //         await r.CloseAsync(); await cn.CloseAsync(); 
//         //         return rn; 
//         //     }
//         //     catch (Exception ex) { throw ex; }
//         //     finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         // }
//         // public async Task<accn_md> get(accn_ls rs){ 
//         //     SqlCommand cm =  new SqlCommand(sqlfnd,cn); SqlDataReader r = null;
//         //     accn_md rn = new accn_md();
//         //     try { 
//         //         /* Vlidate parameter */
//         //         cm.snapsCdn(rs.orgcode,"orgcode");
//         //         cm.snapsCdn(rs.accntype,"accntype");
//         //         cm.snapsCdn(rs.accncode,"accncode");
//         //         r = await cm.snapsReadAsync();
//         //         while(await r.ReadAsync()) { rn = fillmdl(ref r); }
//         //         await r.CloseAsync(); await cn.CloseAsync(); 
//         //         return rn; 
//         //     }
//         //     catch (Exception ex) { throw ex; }
//         //     finally { if(cm!=null) { cm.Dispose();} if(r!=null) { await r.DisposeAsync(); } }
//         // }
//         // public async Task upsert(List<accn_md> rs){ 
//         //     List<SqlCommand> cm = new List<SqlCommand>(); 
//         //     Int32 ix=0;
//         //     String rsl = "";
//         //     try { 
//         //         foreach (accn_md ln in rs) {
//         //             cm.Add(obcommand(ln,sqlval));
//         //             rsl = cm[ix].snapsScalarAsync().GetAwaiter().GetResult().ToString();
//         //             cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//         //         }
//         //         await cm.snapsExecuteTransAsync(cn);
//         //     }catch (Exception ex) { 
//         //         throw ex;
//         //     } finally { cm.ForEach(x=>x.Dispose()); }
//         // }
//         // public async Task upsert(accn_md rs){ 
//         //     List<accn_md> ro = new List<accn_md>(); 
//         //     try { 
//         //         ro.Add(rs); await upsert(ro); 
//         //     }catch (Exception ex) { 
//         //         throw ex;
//         //     } finally { ro.Clear(); }
//         // }
//         public async Task remove(List<accn_md> rs){
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             try { 
//                 foreach (accn_md ln in rs) { cm.Add(obcommand(ln,sqlrem)); }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         public async Task remove(accn_md rs){
//             List<accn_md> ro = new List<accn_md>(); 
//             try { 
//                 ro.Add(rs); await remove(ro); 
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { ro.Clear(); }
//         }
//         public async Task import(List<accn_ix> rs) { 
//             List<SqlCommand> cm = new List<SqlCommand>(); 
//             Int32 ix=0;
//             try { 
//                 foreach (accn_ix ln in rs) {
//                     cm.Add(ixcommand(ln,sqlval)); 
//                     cm[ix].CommandText = (cm[ix].snapsScalarStrAsync().Result == "0") ? sqlins : sqlupd; 
//                 }
//                 await cm.snapsExecuteTransAsync(cn);
//             }catch (Exception ex) { 
//                 throw ex;
//             } finally { cm.ForEach(x=>x.Dispose()); }
//         }
//         private bool disposedValue = false;
//         protected virtual void Dispose(bool Disposing){ 
//             if(!disposedValue) { 
//                 if (cn != null) { cn.Dispose(); } sqlval = null; sqlins = null; sqlupd = null; sqlrem = null; sqlfnd = null; tbn = null; sqlmcom = null;
//             }
//             disposedValue = true;
//         }
//         public void Dispose() {
//             GC.SuppressFinalize(this);
//         }

//         public String getMessage(String Lang,String Ercode){ 
//             SqlCommand cm = new SqlCommand(string.Format("select ISNULL((select descmsg from wm_message where apps = 'WMS' and typemsg = 'ER'" + 
//             " and langmsg = '{0}' and codemsg = '{1}'),'{1}')",Lang,Ercode),cn);
//             try { 
//                 return cm.snapsScalarStrAsync().Result;
//             } catch (Exception ex) { throw ex; 
//             } finally { cm.Dispose();}
//         }
//         public async Task<accn_authen> authen(accn_signup o) { 
//             String sqlauth = "select count(1) rsl from wm_acpriv where accncode = @accncode and accnpriv = @password " +
//              " and tflow = 'IO' and dateexpire >= @sysdate";
//             String sqlstmp = "update wm_accn set cntfailure = 0, datelogin = @sysdate, accscode = @accscode where accncode = @accncode";
//             String sqlvaccn = "select isnull((select tflow rsl from wm_accn where accncode = @accncode),'NF') rsl";
//             SqlCommand cm = new SqlCommand(sqlauth,cn);
//             String accscode = ""; String rslcode = "";
//             accn_authen rncode = new accn_authen();;
//             try { 
//                 cm.Parameters.Add(o.accncode.ClearEmail().snapsPar(nameof(o.accncode)));
//                 cm.Parameters.Add(o.password.ClearReg().snapsPar(nameof(o.password)));
//                 cm.Parameters.Add(o.lang.ClearReg().snapsPar(nameof(o.lang)));
//                 cm.snapsParsysdateoffset();  
//                 if (cm.snapsScalarStrAsync().Result == "1") { 
//                     cm.snapsSetcmd(sqlvaccn);
//                     rslcode = cm.snapsScalarStrAsync().Result;
//                     if (rslcode != "NF") {
//                           if (rslcode == "BL")      { rncode.valcode = "AUTH-02-001"; //Account blocked
//                         } else if (rslcode == "EX") { rncode.valcode = "AUTH-02-002"; //Account expired
//                         } else if (rslcode == "XX") { rncode.valcode = "AUTH-02-003"; //Account deleted
//                         } else if (rslcode == "FG") { rncode.valcode = "AUTH-02-004"; //Account forgoting process
//                         } else if (rslcode == "PC") { rncode.valcode = "AUTH-02-005"; //Account pending for complete profile
//                         } else if (rslcode == "IO"){ //Account active
//                             accscode = "SnapsAuth:"+ (o.accncode+DateTime.Now.ToString()).ToHash();
//                             rncode.valcode = accscode;
//                             cm.Parameters.Add(accscode.snapsPar(nameof(accscode)));
//                             cm.snapsSetcmd(sqlstmp);
//                             await cm.snapsExecuteAsync();
//                             rncode.account = await get(rncode.valcode);
//                             //rncode = new accn_acs(rncode,DateTimeOffset.Now.AddDays(1)).toJson(); >> use return access code I/O access object 
//                         } else { rncode.valcode = "AUTH-02-199"; //Account invalid state
//                         }
//                     }else{ 
//                         rncode.valcode = "AUTH-01-001"; 
//                     }                    
//                 }else { 
//                     cm.snapsSetcmd(sqlvaccn);
//                     rslcode = cm.snapsScalarStrAsync().Result;
//                     if (rslcode == "NF") {
//                         rncode.valcode = "AUTH-01-001"; 
//                     } else { 
                        
//                         cm.snapsSetcmd("update wm_accn SET cntfailure = cntfailure+1,tflow = CASE WHEN " + 
//                         " cntfailure+1 >= (select maxauthfail from wm_policy where tflow = 'IO') then 'BL' " + 
//                         " else tflow end where accncode = @accncode ");
//                         await cm.snapsExecuteAsync();
//                         rncode.valcode = "AUTH-01-001"; 
//                     } 
//                 }
//                 return rncode;
//             } catch (Exception ex) { throw ex; 
//             } finally { sqlauth = null;}
            
//         }

//         public String valaccncode(accn_signup o) { 
//              String sqlvaccn = "select isnull((select tflow rsl from wm_accn where accncode = @accncode),'Ok') rsl";
//              SqlCommand cm = new SqlCommand(sqlvaccn,cn);
//              try { 
//                  cm.Parameters.Add(o.accncode.snapsPar(nameof(o.accncode)));
//                  return cm.snapsScalarStrAsync().Result.ToString();
//              }catch (Exception ex){ 
//                  throw ex; 
//              }finally { sqlvaccn = null; cm.Dispose(); }
//         }
//         public String valaccnemail(accn_signup o) { 
//              String sqlvaccn = "select isnull((select tflow rsl from wm_accn where email = @email),'Ok') rsl";
//              SqlCommand cm = new SqlCommand(sqlvaccn,cn);
//              try { 
//                  if (o.email.Contains("@") == false || o.email.Contains(".") == false) { return "ACCN-01-003"; } 
//                  else { 
//                     cm.Parameters.Add(o.accncode.snapsPar(nameof(o.accncode)));
//                     cm.Parameters.Add(o.email.snapsPar(nameof(o.email)));
//                     return cm.snapsScalarStrAsync().Result.ToString();
//                  }
//              }catch (Exception ex){ 
//                  throw ex; 
//              }finally { sqlvaccn = null; cm.Dispose(); }
//         }
//         public async Task<String> signupAsync(accn_signup o){ 
//              List<SqlCommand> cm = new List<SqlCommand>();
//              try { 
//                  if (valaccncode(o) != "Ok") { return "ACCN-01-001";
//                  }else if (valaccnemail(o) != "Ok"){ return "ACCN-01-002";
//                  }else if (o.email.Contains("@") == false || o.email.Contains(".") == false) { return "ACCN-01-003"; //Email incorrect format
//                  }else {
//                     //Prepare signup
//                     cm.Add(sqlsignup.snapsCommand(cn));
//                     cm.Last().Parameters.Add(o.accncode.snapsPar(nameof(o.accncode)));
//                     cm.Last().Parameters.Add(o.email.snapsPar(nameof(o.email)));
//                     cm.Last().snapsParsysdateoffset();
//                     //prepare password
//                     cm.Add(sqlsignpv.snapsCommand(cn));
//                     cm.Last().Parameters.Add(o.accncode.snapsPar(nameof(o.accncode)));
//                     cm.Last().Parameters.Add(o.password.snapsPar(nameof(o.password)));
//                     cm.Last().snapsParsysdateoffset();

//                     await cm.snapsExecuteTransAsync(cn);
//                     return "Ok";
//                  }
//              }catch (Exception ex){ 
//                 throw ex; 
//              }finally { cm.ForEach(x=>x.Dispose()); }
//         }

//         public async Task<String> forgotAsync(accn_signup o){ 
//             String sqlvld = "select isnull((select tflow rsl from wm_accn where email = @email),'NF') rsl";
//             String sqlfrg = "update wm_accn set tflow = 'FG', tkrqpriv = @tkrqpriv, datemodify = @sysdate, procmodify = 'forgot' where email = @email ";
//             String rncode = ""; String rscode = "";
//             SqlCommand cm = new SqlCommand(sqlvld,cn);
//             try { 
//                 if (o.email.IsValidEmail() == false) { rncode = "ACCN-01-003"; } 
//                 else { 
//                     cm.snapsSetcmd(sqlvld);
//                     cm.snapsPar(o.email,nameof(o.email));
//                     cm.snapsPar("SnapsRecover:"+ (o.email+DateTime.Now.ToString()).ToHash(),"tkrqpriv");
//                     cm.snapsParsysdateoffset();
//                     rscode = cm.snapsScalarStrAsync().Result;
//                       if (rscode == "NF") { rncode = "ACCN-01-004";
//                     } else if (rscode == "PC"){ rncode = "AUTH-02-005";
//                     } else if (rscode == "FG") { rncode = "AUTH-02-004";
//                     } else if (rscode == "IO") { 

//                         cm.snapsSetcmd(sqlfrg);
//                         await cm.snapsExecuteAsync();                   
//                         rncode = "Ok";

//                     }else { rncode = "AUTH-02-199"; }
//                 }
//                 return rncode;
//             }catch (Exception ex){ 
//                 throw ex; 
//             }finally { sqlvld = null; cm.Dispose(); }
//         }
//         public async Task<String> recoveryAsync(accn_signup o){ 
//             String sqlvld = "select isnull((select accncode rsl from wm_accn where tkrqpriv = @accstoken),'NF') rsl";
//             String sqlfrg = "update wm_accn set tflow = 'IO', tkrqpriv = null, datemodify = @sysdate, procmodify = 'receovery' where email = @email ";
//             String accncode = "";
//             String rncode = "";
//             SqlCommand vm = new SqlCommand(sqlvld,cn);
//             List<SqlCommand> cm = new List<SqlCommand>();
//             try { 
                
//                 if (o.email.IsValidEmail() == false) { rncode = "AUTH-02-005"; } 
//                 else { 

//                     vm.snapsPar(o.email,nameof(o.email));
//                     vm.snapsPar(o.accstoken,nameof(o.accstoken));
//                     vm.snapsParsysdateoffset();
                    
//                     accncode = vm.snapsScalarStrAsync().Result;
//                     if (accncode == "NF"){ 
//                         rncode = "AUTH-02-005";
//                     }else { 

//                         cm.Add(sqlfrg.snapsCommand(cn)); // update account profile
//                         cm.Last().snapsPar(o.email,nameof(o.email));
//                         cm.Last().snapsPar(o.accstoken,nameof(o.accstoken));
//                         cm.Last().snapsParsysdateoffset();
                        
//                         cm.Add(sqlsignpv.snapsCommand(cn)); //Insert new access code
//                         cm.Last().snapsPar(o.accncode,nameof(accncode));
//                         cm.Last().snapsPar(o.password,nameof(o.password));
                        
//                         await cm.snapsExecuteTransAsync(cn);

//                         rncode = "Ok";

//                     }
//                 }
//                 return rncode;
//             }catch (Exception ex){ 
//                 throw ex; 
//             }finally { sqlvld = null; sqlfrg = null; vm.Dispose(); cm.ForEach(x=>x.Dispose()); }
//         }

//         public async Task<accn_profile> GetProfileAsync(String o){ 
//             accn_ls ls;
//             accn_profile prc = new accn_profile();
            
//             String sqlget = "select roljson from wm_role where rolecode in (select cfgvalue from wm_acfg f where f.orgcode = 'bgc' and f.apcode = 'wms' and f.accncode = @accncode and f.cfgtype = 'role' and f.cfgcode = 'defrole')";
//             SqlCommand cm = new SqlCommand(sqlget,cn);
//             string rsl = "";
//             try { 
//                 ls = await get(o);
//                 if(ls.notNull()){ 
//                     prc.accnavartar = ls.accnavartar;
//                     prc.accncode = ls.accncode;
//                     prc.accnname = ls.accnname ;
//                     prc.accntype = ls.accntype;
//                     prc.dateexpire = ls.dateexpire;
//                     prc.email = ls.email;
//                     prc.orgcode = ls.orgcode;
//                     prc.roleaccess = new List<accn_roleacs>();
//                     prc.formatdatelong = ls.formatdatelong;
//                     prc.formatdateshort = ls.formatdateshort;
//                     prc.formatdate = ls.formatdate;
                    
//                     cm.snapsPar(ls.accncode,nameof(ls.accncode));
//                     cm.snapsParsysdateoffset();
//                     rsl = cm.snapsScalarStrAsync().Result.ToString();
//                     foreach(DataRow rw in cm.snapsTableAsync().Result.Rows){ 
//                         prc.roleaccess.Add(JsonConvert.DeserializeObject<accn_roleacs>(rw[0].ToString()));
//                     }                    
//                     return prc;
//                 }else { return prc; }
//             }catch (Exception ex) { 
//                 throw ex;
//             }finally { sqlget = null; ls = null; }
//         }

//     }

// }
