using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.warehouse {

    public partial class warehouse_ops : IDisposable { 
        private string sqlwarehouse_ins = "insert into wm_warehouse ( orgcode,sitecode,sitename,sitenamealt,datestart,sitekey,tflow,datecreate,accncreate,      "+ 
        " datemodify,accnmodify,procmodify,sitetype,sitehash) values ( @orgcode,@sitecode,@sitename,@sitenamealt,@datestart,@sitekey,@tflow,sysdatetimeoffset() " + 
        " ,'snaps',sysdatetimeoffset(),'snaps','snaps',@sitetype,@sitehash) ";
        private string sqlwarehouse_upd = "update wm_warehouse set sitename = @sitename, sitenamealt = @sitenamealt, datestart = @datestart, tflow = @tflow, " + 
        " datemodify = sysdatetimeoffset(),accnmodify = @accnmodify,procmodify = @procmodify, sitetype = @sitetype, sitehash = @sitehash  " + 
        " where orgcode = @orgcode and site = @site ";
        private string sqlwarehouse_rem = "";

        private string sqldepot_gen_step1 = "insert into wm_binary (orgcode, site, depot, apps,bntype,bncode,bnvalue,bndesc,bndescalt,bnflex1,bnflex2,    " +
        " bnflex3,bnflex4,bnicon,bnstate,datecreate, accncreate, datemodify, modifyaccn) select @orgcode orgcode,@site site,@depot depot, apps,bntype,bncode, " + 
        " bnvalue,bndesc,bndescalt,bnflex1,bnflex2,bnflex3,bnflex4,bnicon,bnstate,sysdatetimeoffset(), accncreate, sysdatetimeoffset(), accnmodify from       " + 
        " wm_binary where orgcode = 'Snaps' and site = 0 and depot = 0 and apps = 'WMS'";
        private string sqldepot_gen_step2 = "insert into wm_parameters (orgcode, site, depot,apps,pmmodule,pmtype,pmcode, pmvalue,pmdesc,pmdescalt,pmstate" +
        " ,datecreate,accncreate,datemodify,accnmodify,pmseq,pmoption) select @orgcode, @site, @depot,apps,pmmodule,pmtype,pmcode, pmvalue,pmdesc,pmdescalt   " + 
        " ,pmstate,datecreate,accncreate,datemodify,accnmodify,pmseq,pmoption from wm_parameters where orgcode = 'Snaps' and site = 0 and depot = 0 ";
        private string sqldepot_gen_step3 = "insert into wm_role (orgcode,apcode,site,depot,rolecode,rolename,roledesc,tflow,hashrol,datecreate,accncreate" +
        " ,datemodify,accnmodify,procmodify,roljson) select @orgcode ,apcode,@site,@depot,rolecode,rolename,roledesc,tflow,hashrol,sysdatetimeoffset(),accncreate"+
        " ,sysdatetimeoffset(),accnmodify,procmodify,roljson from wm_role where orgcode = 'Snaps' and apcode = 'WMS' and site = 0 and depot = 0 " + 
        " and rolecode = 'snapsme'";
        private string sqldepot_gen_step4 = "insert into wm_accn ( orgcode,accntype,accncode,accnname,accnsurname,email,dateexpire,tflow,datecreate,accncreate, "+ 
        " procmodify,formatdateshort,formatdatelong,formatdate )      values ( @orgcode,'creator',accncode,'Snaps','Solution','contact@snapssolution.com',     " + 
        " dateadd(YEAR, 10, sysdatetimeoffset()),'IO',sysdatetimeoffset(),'Snaps','Snaps','dd/MM/yy hh:mm','dd/MMM/yyyy hh:mm','dd/MM/yy hh:mm' )";
        private string sqldepot_gen_step5 = "insert into dbo.wm_acpriv (orgcode,apcode,accncode,accnpriv,tflow,hashpriv,dateexpire,datecreate,accncreate,procmodify)" + 
        " values (@orgcode,'WMS',@accncode,@accnpriv,'IO',@hashpriv, dateadd(YEAR, 10, sysdatetimeoffset()) ,sysdatetimeoffset() ,'Snaps' ,'Snaps')";
        private string sqldepot_gen_step6 = "insert into wm_thparty (orgcode,site,depot,spcarea,thbutype,thtype,thcode,thcodealt,thname,thnamealt,orbitsource " + 
        " ,tflow,datecreate,accncreate,procmodify) values (@orgcode,@site,@depot,@spcarea,'WH','WH',@depot,@depotname,@depotname,@depotname,'Snaps','IO'      " +
        " ,sysdatetimeoffset(),'snaps','snaps')";

    }
}