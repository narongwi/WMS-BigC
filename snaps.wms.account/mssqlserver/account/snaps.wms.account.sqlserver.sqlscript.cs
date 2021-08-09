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

namespace Snaps.WMS {

    public partial class accn_ops : IDisposable { 
        private string sqlaccount_vldpriv = "select count(1) rsl from wm_acpriv where accncode = @accncode and accnpriv = @password " +
             " and tflow = 'IO' and dateexpire >= sysdatetimeoffset()";
        private string sqlaccount_accs = "update wm_accn set cntfailure = 0, datelogin = sysdatetimeoffset(), accscode = @accscode where accncode = @accncode";
        private string sqlaccount_get = "select * from wm_accn where accncode = @accncode and accscode = @accscode ";
        private string sqlaccount_profile = "select * from wm_accn where accncode = @accncode and orgcode = @orgcode ";

        private string sqlaccount_setvld = "select count(1) from wm_acfg where orgcode = @orgcode and site = @site and depot = @depot and accncode = @accncode";
        private string sqlaccount_setadd = @"INSERT INTO wm_acfg (orgcode, apcode, accncode, site, depot, cfgtype, cfgcode, cfgname, cfgvalue, cfghash, tflow, formatdate, 
        formatdateshort, formatdatelong, unitdimension, unitweight,unitvolume, unitcubic, pagelimit, lang, datecreate, accncreate, datemodify, accnmodify, procmodify, isdefault)
        VALUES(@orgcode, 'WMS', @accncode,@site,@depot, 'role', @rolecode,'role permission', @rolecode, null, 'IO', null, null, null, null, null, null, null,200, 'EN',sysdatetimeoffset(),
        @accnmodify,sysdatetimeoffset(),@accnmodify, 'accn.permission', 0)";
        private string sqlaccount_setupd = @"update wm_acfg set cfgcode = @rolecode ,cfgvalue = @rolecode, accnmodify = @accnmodify, datemodify = sysdatetimeoffset() where orgcode = @orgcode and site = @site and depot = @depot and accncode = @accncode";

        private string sqlaccount_setdel = @"delete from wm_acfg where orgcode = @orgcode and site = @site and depot = @depot and accncode = @accncode and cfgcode = @rolecode";
        private string sqlaccount_signup = "insert into wm_accn (accncode,accnname,accnsurname, email, tflow ) values (@accncode,@accnname,@accnsurname, @email, @tflow)";
        private string sqlaccount_insert_step1 =  "insert into wm_accn" + 
        " ( orgcode, accntype, accncode, accnname, accnsurname, email, mobileno, accnapline, dateexpire, tflow, accnavartar, " + 
        "   tkrqpriv, cntfailure, datelogin, datelogout, datechnpriv, datecreate, accncreate, datemodify, accnmodify, procmodify,site,depot ) values " + 
        " ( @orgcode, @accntype, @accncode, @accnname, @accnsurname, @email, @mobileno, @accnapline, @dateexpire, @tflow, @accnavartar, @tkrqpriv, " +
        "   @cntfailure, @datelogin, @datelogout, @datechnpriv, sysdatetimeoffset(), @accncreate, sysdatetimeoffset(), @accnmodify, @procmodify,@site, @depot ) ";
        private string sqlaccount_insert_step2 = " insert into wm_acpriv " + 
        " (orgcode,apcode,accncode,accnpriv,tflow,hashpriv,dateexpire,datecreate,accncreate,datemodify,accnmodify,procmodify) VALUES " + 
        " (@orgcode,@apcode,@accncode,@accnpriv,@tflow,@hashpriv,@dateexpire,sysdatetimeoffset(),@accncreate,sysdatetimeoffset(),@accncreate,@procmodify) " ;

        private string sqlaccount_update_passw = "update wm_acpriv set accnpriv=@accnpriv,hashpriv=@hashpriv,dateexpire=@dateexpire,datemodify=sysdatetimeoffset()," +
            " accnmodify = @accnmodify,procmodify=@procmodify  where orgcode = @orgcode and accncode = @accncode";

        private string sqlaccount_vldaccn = "select"+ 
        " (select case when count(1) = 1 then 'Account not approve for create ' else '' end from wm_accn where accncode = @accncode and tflow = 'PC') +  " + 
        " (select case when count(1) > 1 then 'Your email has registered on syste' else '' end from wm_accn where email = @accncode and tflow = 'PC') rsl ";
        private string sqlaccount_update = " update wm_accn set accnname = @accnname, accnsurname = @accnsurname, tflow = @tflow, accsrole = @accsrole ," +
        " formatdate=@formatdate,formatdatelong=@formatdatelong ,formatdateshort=@formatdateshort,datemodify = sysdatetimeoffset(), accnavartar=@accnavartar," +
        " accnmodify = @accnmodify, procmodify = @procmodify  where orgcode = @orgcode and accncode = @accncode ";

        //SqlCommand cmc = new SqlCommand("", cn);
        //cmc.CommandText = sqlacconfig_remove;
        //                cmc.snapsPar(o.orgcode, "orgcode");
        //                cmc.snapsPar(o.site, "site");
        //                cmc.snapsPar(o.depot, "depot");
        //                cmc.snapsPar(o.accnmodify, "accnmodify");
        //                cmc.snapsPar(o.accsrole, "cfgcode");
        //                cmc.snapsPar(o.accsrole, "cfgvalue");
        //                cmc.snapsPar(o.formatdate, "formatdate");
        //                cmc.snapsPar(o.formatdatelong, "formatdatelong");
        //                cmc.snapsPar(o.formatdateshort, "formatdateshort");
        //                cmc.snapsPar("accn.upsert", "procmodify");
        //                cmc.snapsPar(o.accncode, "accncode");
        //                cmc.snapsPar("role", "cfgtype");

        string sqlacconfig_remove = "update wm_acfg set cfgcode=@cfgcode,cfgvalue=@cfgvalue,formatdate=@formatdate,formatdatelong=@formatdatelong," +
            " formatdateshort=@formatdateshort,accnmodify=@accnmodify,procmodify=@procmodify,datemodify=getdate()" +
            " where orgcode = @orgcode and site = @site and depot = @depot and accncode = @accncode and cfgtype = @cfgtype ";

        private string sqlaccount_getdefpriv = "select suffixpriv from wm_policy where orgcode = @orgcode and site =  @site and depot = @depot and tflow = 'IO'";
        private string sqlaccount_getlifetime = "select dayexpire from wm_policy where orgcode = @orgcode and site =  @site and depot = @depot and tflow = 'IO'";
        private string sqlaccount_vldaccs = "select count(1) rsl from wm_accn where accscode = @accscode";
        private string sqlaccount_vldforgot = "select isnull((select tflow rsl from wm_accn where email = @email),'NF') rsl";
        private string sqlaccount_forgot = "update wm_accn set tflow = 'FG', tkrqpriv = @tkrqpriv, datemodify = sysdatetimeoffset(), procmodify = 'accn.forgor' where email = @email ";

        private string sqlaccount_vldtkreg = "select count(1) rsl from wm_accn where accscode = @accscode";
        private string sqlaccount_recovery_step1 = "update wm_accn set tflow = 'IO', tkrqpriv = null, datemodify = sysdatetimeoffset(), procmodify = 'receovery' where email = @email ";

        private string sqlaccount_privvld_his = "select count(1) rsl from wm_acpriv where orgcode = @orgcode and accncode = @accncode and accnpriv = @accnpriv";
        private string sqlaccount_privvld_old = "select count(1) rsl from wm_acpriv where orgcode = @orgcode and accncode = @accncode and accnpriv = @accnpriv and tflow = 'IO'";
        private string sqlaccount_privvld_act = "select count(1) rsl from wm_acpriv where orgcode = @orgcode and accncode = @accncode";
        private string sqlaccount_privins = "insert into wm_acpriv (orgcode,apcode,accncode,accnpriv, tflow,hashpriv,dateexpire, datecreate,accncreate,datemodify, accnmodify, procmodify) " + 
        " values ( @orgcode, @apcode, @accncode, @newpriv, @tflow, @hashpriv, @dateexpire, sysdatetimeoffset() , @accncreate, sysdatetimeoffset() , @accnmodify, @procmodify )";
        private string sqlaccount_prisend = "update wm_acpriv set tflow = 'ED', datemodify = sysdatetimeoffset(), accnmodify = @accncode, procmodify = @procmodify" + 
        " where orgcode = @orgcode and accncode = @accncode and tflow = 'IO'";
        private string sqlaccount_vldcode = "select isnull((select top 1 tflow rsl from wm_accn where accncode = @accncode),'') rsl";
        private string sqlaccount_vldemail = "select isnull((select top 1 tflow rsl from wm_accn where email = @email),'') rsl";
        //private string sqlaccoutn_find = "select * from wm_accn where orgcode = @orgcode and site = @site and depot = @depot and accntype != 'Snaps' ";
        private string sqlaccoutn_list = "select * from wm_accn where orgcode = @orgcode";
        private string cnx = "";
        private SqlConnection cn = null;
        public accn_ops() {  }
        public accn_ops(String cx) { this.cnx = cx; this.cn = new SqlConnection(cx); }
        public accn_ops(SqlConnection cx) { this.cn = cx; this.cnx = cx.ConnectionString; }

        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }

            this.sqlaccount_insert_step1 = null;
            this.sqlaccount_insert_step2 = null;
            this.sqlaccount_vldaccn = null;
            this.sqlaccount_update = null;
            
            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}