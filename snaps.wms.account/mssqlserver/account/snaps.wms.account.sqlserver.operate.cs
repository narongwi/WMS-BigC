using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.Hash;
using Snaps.Helpers.Json;
using Snaps.Helpers.DbContext.SQLServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;


namespace Snaps.WMS {

    public partial class accn_ops : IDisposable { 
        //public async Task<List<accn_ls>> findAsync(accn_pm rs) { 
        //    SqlCommand cm =  new SqlCommand(sqlaccoutn_find,cn);
        //    List<accn_ls> rn = new List<accn_ls>();
        //    SqlDataReader r = null;
        //    try { 
        //        cm.snapsPar(rs.orgcode,"orgcode");
        //        cm.snapsPar(rs.site,"site");
        //        cm.snapsPar(rs.depot,"depot");
        //        cm.snapsCdn(rs.accnname, "product", " and (accncode=@product or accnname like '%' + @product + '%')");
        //        cm.snapsCdnlikeend(rs.email, "email");
        //        cm.snapsCdn(rs.tflow,"tflow");
        //        r = await cm.snapsReadAsync();
        //        while(await r.ReadAsync()) { rn.Add(setls(ref r)); }
        //        await r.CloseAsync(); await cn.CloseAsync(); 
        //        return rn;
        //    }
        //    catch (Exception ex) { throw ex; }
        //    finally { if(cm!=null) { await cm.DisposeAsync();} if(r!=null) { await r.DisposeAsync(); } }
        //}

        public async Task<List<accn_ls>> findAsync(accn_pm rs) {
            SqlCommand cm = new SqlCommand(sqlaccoutn_list,cn);
            List<accn_ls> rn = new List<accn_ls>();
            SqlDataReader r = null;
            try {
                cm.snapsPar(rs.orgcode,"orgcode");
                cm.snapsCdn(rs.accnname,"accnname"," and (accncode=@accnname or accnname like '%' + @accnname + '%')");
                //cm.snapsCdnlikeend(rs.email,"email");
                cm.snapsPar(rs.tflow,"tflow");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn.Add(setls(ref r)); }
                await r.CloseAsync(); await cn.CloseAsync();
                return rn;
            } catch(Exception ex) { throw ex; } finally { if(cm != null) { await cm.DisposeAsync(); } if(r != null) { await r.DisposeAsync(); } }
        }
        public async Task<accn_ls> getAsycn(string accncode, string accscode){ 
            SqlCommand cm = new SqlCommand(sqlaccount_get,cn);
            SqlDataReader r = null;
            accn_ls rn = new accn_ls();
            try { 
                cm.snapsPar(accncode,"accncode");
                cm.snapsPar(accscode,"accscode");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = setls(ref r); }
                await r.CloseAsync(); await cn.CloseAsync(); 
                return rn;
            }catch (Exception ex){ 
                throw ex; 
            } finally { if(cm!=null) { await cm.DisposeAsync();} if(r!=null) { await r.DisposeAsync(); } }
        }
        public async Task<Boolean> vldSession( string accscode){ 
            SqlCommand cm = new SqlCommand(sqlaccount_vldaccs,cn);
            try { 
                cm.snapsPar(accscode.ClearReg(),"accscode");
                return (cm.snapsScalarStrAsync().Result == "1") ? true : false;
            }catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync();   }
        }
        public async Task<accn_authen> authen(accn_signup o) { 
            SqlCommand cm = new SqlCommand(sqlaccount_vldpriv,cn);
            String accscode = ""; String rslcode = "";
            accn_authen rncode = new accn_authen();
            try { 
                cm.snapsPar(o.accncode.ClearReg(),"accncode");
                cm.snapsPar(o.password.ClearReg().ToHash(),"password");
                //Console.WriteLine(o.password.ClearReg().ToHash());
                if (cm.snapsScalarStrAsync().Result == "1") { 
                    cm.snapsSetcmd(sqlaccount_vldcode);
                    rslcode = cm.snapsScalarStrAsync().Result;
                    if (rslcode != "NF") {
                          if (rslcode == "BL")      { rncode.valcode = "Account has blocked"; //Account blocked
                        } else if (rslcode == "EX") { rncode.valcode = "Account has expired"; //Account expired
                        } else if (rslcode == "XX") { rncode.valcode = "Account has deleted"; //Account deleted
                        } else if (rslcode == "FG") { rncode.valcode = "Account has forgoting process"; //Account forgoting process
                        } else if (rslcode == "PC") { rncode.valcode = "Account has pending approve"; //Account pending for complete profile
                        } else if (rslcode == "IO"){ //Account active
                            accscode = "SnapsAuth:"+ (o.accncode+DateTimeOffset.Now.ToString()).ToHash();
                            rncode.valcode = accscode;
                            cm.Parameters.Add(accscode.snapsPar(nameof(accscode)));
                            cm.snapsSetcmd(sqlaccount_accs);
                            await cm.snapsExecuteAsync();
                            rncode.account = await getAsycn(o.accncode, rncode.valcode);
                            //rncode = new accn_acs(rncode,DateTimeOffset.Now.AddDays(1)).toJson(); >> use return access code I/O access object 
                        } else { rncode.valcode = "Account has incorrect state"; //Account invalid state
                        }
                    }else{ 
                        rncode.valcode = "Account code or password incorrect"; 
                    }                    
                }else { 
                    cm.snapsSetcmd(sqlaccount_vldaccn);
                    rslcode = cm.snapsScalarStrAsync().Result;
                    if (rslcode == "NF") {
                        rncode.valcode = "Account code or password incorrect"; 
                    } else {                         
                        cm.snapsSetcmd("update wm_accn SET cntfailure = cntfailure+1,tflow = CASE WHEN " + 
                        " cntfailure+1 >= (select maxauthfail from wm_policy where tflow = 'IO') then 'BL' " + 
                        " else tflow end where accncode = @accncode ");
                        await cm.snapsExecuteAsync();
                        rncode.valcode = "Account code or password incorrect"; 
                    } 
                }
                return rncode;
            } catch (Exception ex) { throw ex; 
            } finally { await cm.DisposeAsync(); accscode = null; rslcode = null; }
        }

        public async Task<string> signupAsync(accn_signup o){ 
             List<SqlCommand> cm = new List<SqlCommand>();
             string rslaccn = "";
             string rslemail = "";
             try { 
                 rslaccn = vldAccount(o.accncode);
                 rslemail = vldEmail(o.email);
                 if (rslaccn != "") { return rslaccn;
                 }else if (o.email.Contains("@") == false || o.email.Contains(".") == false) { return "Email incorrect format";
                 }else if (rslemail != ""){ return rslemail;
                 }else {
                    //Prepare signup
                    cm.Add(sqlaccount_signup.snapsCommand(cn));
                    cm.Last().snapsPar(o.accncode,"accncode");
                    cm.Last().snapsPar(o.accnname,"accnname");
                    cm.Last().snapsPar(o.accnsurname,"accnsurname");
                    cm.Last().snapsPar(o.email,"email");
                    cm.Last().snapsPar("PC","tflow");
                    await cm.snapsExecuteTransAsync(cn);
                    return "";
                 }
             }catch (Exception ex){ 
                throw ex; 
             }finally { cm.ForEach(x=>x.Dispose()); }
        }
        public async Task upsertAsync(accn_md o) { 
            SqlCommand cm = new SqlCommand(sqlaccount_vldaccn,cn);
            SqlCommand cmg = new SqlCommand("",cn);
            SqlCommand cmp = new SqlCommand("",cn);
           

            List<SqlCommand> lm = new List<SqlCommand>();
            config_ops co = new config_ops(cnx);
            config_md cg = new config_md();
            accn_priv ap = new accn_priv();
            string rsl = "";
            try { 
                if (o.email.Contains("@") == false || o.email.Contains(".") == false) { throw new Exception("Email address incorrect format"); }
                else { 
                   
                    if (o.tflow == "NW") { 
                        o.tflow = "IO";
                        if (!o.accnavartar.notNull()) { 
                                o.accnavartar = "assets/images/logobgc_small_back.jpg"; //Wait for fix
                        }
                        fillCommand(ref cm,o);
                        rsl= cm.snapsScalarStrAsync().Result;
                        if (rsl != "") { throw new Exception(rsl); }
                        else {
                            cg.accncode = o.accncode;
                            cg.accncreate = o.accnmodify;
                            cg.accnmodify = o.accnmodify;
                            cg.apcode = "WMS";
                            cg.cfgcode = o.accsrole;
                            cg.site = o.site;
                            cg.depot = o.depot;
                            cg.cfgtype = "role";
                            cg.cfgvalue = o.accsrole;
                            cg.formatdate = o.formatdate;
                            cg.formatdatelong = o.formatdatelong;
                            cg.formatdateshort = o.formatdateshort;
                            cg.unitdimension = "2";
                            cg.unitweight = "7";
                            cg.unitvolume = "2";
                            cg.unitcubic = "4";
                            cg.pagelimit = 200;
                            cg.lang = "EN";
                            cg.isdefault = 1;
                            cg.orgcode = o.orgcode;
                            cg.cfghash = (cg.orgcode + cg.apcode + DateTimeOffset.Now.ToString()).ToHash();
                            cg.tflow = "IO";
                            cg.isdefault = 1;
                            cg.procmodify = "accn.upsert";
                            cg.cfgname = "Default setup configuration";

                            cm.CommandText = sqlaccount_getdefpriv;
                            ap.newpriv = cm.snapsScalarStrAsync().Result;
                            cm.CommandText = sqlaccount_getlifetime;
                            ap.lifetime = cm.snapsScalarStrAsync().Result.CInt32();  
                            ap.accncode = o.accncode;
                            ap.orgcode = o.orgcode;

                            // insert account
                            cm.CommandText = sqlaccount_insert_step1;
                            lm.Add(cm);
                            // insert config
                            cmg = co.getCommand(cg);
                            lm.Add(cmg);
                            // insert password 
                            cmp =  setPriv(ap,o.accnmodify);
                            lm.Add(cmp);

                            // process
                            await lm.snapsExecuteTransAsync(cn);

                            //// insert account
                            //await cm.snapsExecuteAsync();
                            //// insert config
                            //await cmg.snapsExecuteAsync();
                            //// insert password 
                            //await cmp.snapsExecuteAsync();

                        }
                    }
                    else { 

                        // update wm_accn
                        fillCommand(ref cm,o);
                        cm.CommandText = sqlaccount_update;
                        lm.Add(cm);

                        // update config
                        SqlCommand cmc = new SqlCommand("", cn);
                        cmc.CommandText = sqlacconfig_remove;
                        cmc.snapsPar(o.orgcode, "orgcode");
                        cmc.snapsPar(o.site, "site");
                        cmc.snapsPar(o.depot, "depot");
                        cmc.snapsPar(o.accnmodify, "accnmodify");
                        cmc.snapsPar(o.accsrole, "cfgcode");
                        cmc.snapsPar(o.accsrole, "cfgvalue");
                        cmc.snapsPar(o.formatdate, "formatdate");
                        cmc.snapsPar(o.formatdatelong, "formatdatelong");
                        cmc.snapsPar(o.formatdateshort, "formatdateshort");
                        cmc.snapsPar("accn.upsert", "procmodify");
                        cmc.snapsPar(o.accncode, "accncode");
                        cmc.snapsPar("role", "cfgtype");
                        lm.Add(cmc);

                        // process
                        await lm.snapsExecuteTransAsync(cn);
                    }
                } 
            }catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync();   }
        }
        public async Task<accn_md> getModify(string orgcode,string accncode) {
            SqlCommand cm = new SqlCommand(sqlaccount_profile,cn);
            SqlDataReader r = null;
            accn_md rn = new accn_md();
            try {
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(accncode,"accncode");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = setAccn(ref r); }
                await r.CloseAsync(); await cn.CloseAsync();

                cm.CommandText = @"select c.orgcode,c.site,c.depot,r.rolecode ,r.rolename ,c.accncode,c.accncreate,c.datecreate
                 from wm_acfg c join wm_role r on c.orgcode = r.orgcode  and c.cfgcode = r.rolecode
                 where c.orgcode = @orgcode and c.accncode = @accncode";

                var cfgdt = cm.snapsTableAsync().Result;
                rn.accncfg  = new List<accn_cfg>();
                foreach(DataRow rw in cfgdt.Rows) {
                    rn.accncfg.Add(new accn_cfg(rw["orgcode"],rw["site"],rw["depot"],rw["accncode"],rw["rolecode"],rw["rolename"],rw["accncreate"],rw["datecreate"]));
                }

                if(rn.accncfg.Count > 0)
                    rn.accsrole = rn.accncfg.First().rolecode;

                return rn;
            } catch(Exception ex) {
                throw ex;
            } finally {
                cm.Dispose();
                if(r != null) { await r.DisposeAsync(); }
            }
        }
        public async Task addCfgAsync(accn_cfg cfg,string accnmodify) {
            SqlCommand cm = new SqlCommand(sqlaccount_setvld,cn);
            accn_md rn = new accn_md();
            try {
                cm.snapsPar(cfg.orgcode,"orgcode");
                cm.snapsPar(cfg.site,"site");
                cm.snapsPar(cfg.depot,"depot");
                cm.snapsPar(cfg.accncode,"accncode");
                cm.snapsPar(cfg.rolecode,"rolecode");
                cm.snapsPar(accnmodify,"accnmodify");
                if(cm.snapsScalarStrAsync().Result == "0") {
                    cm.CommandText = sqlaccount_setadd;
                    await cm.snapsExecuteAsync();
                } else {
                    cm.CommandText = sqlaccount_setupd;
                    await cm.snapsExecuteAsync();
                }
            } catch(Exception ex) {
                throw ex;
            } finally {
                cm.Dispose();
            }
        }
        public async Task delCfgAsync(accn_cfg cfg,string accnmodify) {
            SqlCommand cm = new SqlCommand(sqlaccount_setvld,cn);
            accn_md rn = new accn_md();
            try {
                cm.snapsPar(cfg.orgcode,"orgcode");
                cm.snapsPar(cfg.site,"site");
                cm.snapsPar(cfg.depot,"depot");
                cm.snapsPar(cfg.accncode,"accncode");
                cm.snapsPar(cfg.rolecode,"rolecode");
                cm.snapsPar(accnmodify,"accnmodify");

                if(cm.snapsScalarStrAsync().Result == "0") {
                    throw new Exception("role data not found");
                } else {
                    cm.CommandText = sqlaccount_setdel;
                    await cm.snapsExecuteAsync();
                }
            } catch(Exception ex) {
                throw ex;
            } finally {
                cm.Dispose();
            }
        }
        public async Task<accn_md> getProfile(string orgcode, string site, string depot, string accncode){ 
            SqlCommand cm = new SqlCommand(sqlaccount_profile,cn);
            SqlDataReader r = null; 
            accn_md rn = new accn_md();
            try { 
                //cm.snapsPar(orgcode,"orgcode");
                //cm.snapsPar(site,"site");
                //cm.snapsPar(depot,"depot");
                cm.snapsPar(accncode,"accncode");
                r = await cm.snapsReadAsync();
                while(await r.ReadAsync()) { rn = setAccn(ref r); }
                await r.CloseAsync(); await cn.CloseAsync();

                cm.CommandText = "select top 1 cfgcode from wm_acfg where orgcode = @orgcode and accncode = @accncode and site = @site";
                rn.accsrole = cm.snapsScalarStrAsync().Result;
                return rn;
            }catch (Exception ex) { 
                throw ex;
            }finally { 
                cm.Dispose(); 
                if (r != null) { await r.DisposeAsync(); }
            }
        }
        public async Task<string> forgotAsync(accn_signup o){ 
            SqlCommand cm = new SqlCommand(sqlaccount_vldforgot,cn);
            string rsl = "";
            try { 
                cm.snapsPar(o.email.ClearReg(),"email");
                rsl = cm.snapsScalarStrAsync().Result;
                if (rsl == "NF") { return "Email incorrect"; }
                else if (rsl == "PC") { return"Email under approval process";  }
                else if (rsl == "FG") { return"Email under forgoting process"; }
                else if (rsl == "XX") { return"Email has deleted "; }
                else if (rsl == "BL") { return"Email has blocked"; }
                else if (rsl == "IO") { 
                    cm.CommandText = sqlaccount_forgot;
                    await cm.snapsExecuteAsync();
                    return "";
                }else { 
                    return "Email state invalid";
                }
                
            }catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync();   }
        }

        public async Task valdRecoveryAsync(string tkreqid){ 
            SqlCommand cm = new SqlCommand(sqlaccount_vldforgot,cn);
            string rsl = "";
            try { 
                cm.snapsPar(tkreqid.ClearReg(),"tkreqid");
                rsl = cm.snapsScalarStrAsync().Result;
                if (rsl == "NF") {  throw new Exception("Email incorrect"); }
                else if (rsl == "PC") { throw new Exception("Email under approval process");  }
                else if (rsl == "FG") { throw new Exception("Email under forgoting process"); }
                else if (rsl == "XX") { throw new Exception("Email has deleted "); }
                else if (rsl == "BL") { throw new Exception("Email has blocked"); }
                else if (rsl == "IO") { 
                    cm.CommandText = sqlaccount_forgot;
                    await cm.snapsExecuteAsync();
                }                
            }catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync();   }
        }

        public async Task changePrivAsync(accn_priv o) { 
            SqlCommand cm = new SqlCommand(sqlaccount_privvld_old,cn);
            policy_ops po = new policy_ops(cnx);
            SqlCommand cmp = new SqlCommand("", cn);
            policy_md pm = new policy_md();
            List<policy_md>  lm = new List<policy_md>();
            string rsl = "";
            try{ 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.accncode,"accncode");
                cm.snapsPar(o.oldpriv,"oldpriv");
                cm.snapsPar(o.newpriv,"newpriv");
                cm.snapsPar(o.oldpriv.ClearReg().ToHash(),"accnpriv");
                cm.snapsPar("accn.change", "procmodify");

                // check old password
                if (cm.snapsScalarStrAsync().Result == "0"){ 
                    throw new Exception("Your old password incorrect");
                }else {
                    // check matching new password
                    cm.Parameters["accnpriv"].Value = o.newpriv.ClearReg().ToHash();
                    cm.CommandText = sqlaccount_privvld_his;
                    if(cm.snapsScalarStrAsync().Result == "1") { 
                        throw new Exception("Your new password that matching the old password");
                    }else { 
                        pm.orgcode = o.orgcode;
                        pm.site = o.site;
                        pm.depot = o.depot;
                        lm = await po.findAsync(pm);

                        // validate password
                        rsl = privvld(lm.FirstOrDefault(), o.newpriv);
                        if (rsl == "") {
                            cm.snapsPar(o.site, "site");
                            cm.snapsPar(o.depot, "depot");
                            cm.CommandText = sqlaccount_getlifetime;// password lifetime
                            o.lifetime = cm.snapsScalarStrAsync().Result.CInt32();

                            cmp = updPriv(o,o.accncode,"accn.change");
                            await cmp.snapsExecuteAsync();
                        }else { 
                            throw new Exception(rsl);
                        }
                    }
                }
            }catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync(); cmp.Dispose();   }
        }
        public async Task resetPrivAsync(accn_priv o,string usermodify)
        {
            SqlCommand cm = new SqlCommand("", cn);
            SqlCommand cmp = new SqlCommand("", cn);

            try
            {
                cm = new SqlCommand(sqlaccount_privvld_act, cn);
                cm.snapsPar(o.orgcode, "orgcode");
                cm.snapsPar(o.accncode, "accncode");
                cm.snapsPar(o.site, "site");
                cm.snapsPar(o.depot, "depot");

                if (cm.snapsScalarStrAsync().Result == "0")
                {
                    throw new Exception("Account not found.");
                }
                else
                {
                    // get default password
                    cm.CommandText = sqlaccount_getdefpriv;// password suffix
                    o.newpriv = o.accncode.ClearReg() + cm.snapsScalarStrAsync().Result;
                    cm.CommandText = sqlaccount_getlifetime;// password lifetime
                    o.lifetime = cm.snapsScalarStrAsync().Result.CInt32();
                    cmp = updPriv(o, usermodify,"accn.reset");
                    await cmp.snapsExecuteAsync();
                }
            }
            catch (Exception ex) { throw ex; }
            finally { await cm.DisposeAsync(); cmp.Dispose(); }
        }
        public string privvld(policy_md ms, string Password) {
           string ermsg = "";
           try { 
                ermsg = "";
                if (Password.Length < ms.minlength) 
                    ermsg += "Password length must more than " + ms.minlength + Environment.NewLine;
                    //ermsg += "รหัสผ่านต้องมีมากกว่า " + ms.minlength + " ตัวอักษร" +  Environment.NewLine;
                if (UpperCaseCount(Password) < ms.requppercase && ms.requppercase > 0)
                    ermsg += "Upper case chcracter length must more than " + ms.requppercase + Environment.NewLine;
                    //ermsg += "ตัวพิมพ์ใหญ่ต้องมีมากกว่า " + ms.requppercase + " ตัวอักษร" + Environment.NewLine;
                if (LowerCaseCount(Password) < ms.reqlowercase && ms.reqlowercase > 0)
                    ermsg += "Lower case chcracter length must more than " + ms.reqlowercase + Environment.NewLine;
                    //ermsg += "ตัวพิมพ์เล็กต้องมีมากกว่า " + ms.requppercase + " ตัวอักษร" + Environment.NewLine;
                if (NumericCount(Password) < ms.reqnumeric && ms.reqnumeric > 0 )
                    ermsg += "Numeric chcracter length must more than " + ms.reqnumeric + Environment.NewLine;
                    //ermsg += "ตัวเลขต้องมีมากกว่า " + ms.requppercase + " ตัวอักษร" + Environment.NewLine;
                if (NonAlphaCount(Password) < ms.reqspecialchar && ms.reqspecialchar > 0)
                    ermsg += "Non alphabet chcracter length must more than " + ms.reqnumeric + Environment.NewLine;
                    //ermsg += "ตัวอักษรหรือสัญลักษณ์ต้องมีมากกว่า " + ms.reqnumeric + " ตัวอักษร" + Environment.NewLine;
                if (SpecificCount(ms.spcchar, Password) < 1 && ms.spcchar != "") 
                    ermsg += "Specific charactor " +ms.spcchar+ " is request ";
                    //ermsg += "ต้องมีตัวอักขระ " +ms.spcchar;
                return ermsg;
           }catch (Exception ex) { 
               throw ex;
           } finally { ermsg = null;}     
        }

        private static int UpperCaseCount(string Password) { return Regex.Matches(Password, "[A-Z]").Count; }
        private static int LowerCaseCount(string Password) { return Regex.Matches(Password, "[a-z]").Count; }
        private static int NumericCount(string Password) { return Regex.Matches(Password, "[0-9]").Count; }
        private static int NonAlphaCount(string Password) { return Regex.Matches(Password, @"[^0-9a-zA-Z\._]").Count; }
        private static int SpecificCount(string spcchar, string Password) { return Regex.Matches(Password, spcchar).Count; }

        public string vldAccount(string accncode){
             SqlCommand cm = new SqlCommand(sqlaccount_vldcode,cn);
             try { 
                 cm.snapsPar(accncode.ClearReg(),"accncode");
                 if (cm.snapsScalarStrAsync().Result != "") { 
                     return "Account is not available";
                 }else { 
                     return "";
                 }
             }catch (Exception ex){ 
                 throw ex; 
             }finally { cm.Dispose(); }
        }
        public string vldEmail(string email) { 
             SqlCommand cm = new SqlCommand(sqlaccount_vldemail,cn);
             try { 
                 cm.snapsPar(email.ClearReg(),"email");
                 if (cm.snapsScalarStrAsync().Result != "") { 
                    return "Email is not available";
                 }else { 
                     return "";
                 }
             }catch (Exception ex){ 
                 throw ex; 
             }finally { cm.Dispose(); }
        }
    
    
    }
}