using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Snaps.Helpers.DbContext.SQLServer;
using System.Threading.Tasks;
namespace Snaps.WMS {
    public partial class exSource_ops : IDisposable {
        private string sqlexssource_find = "select * from xm_xiexsource where imptype = @imptype and orgcode = @orgcode and site = @site and depot = @depot  ";
        private string sqlexssource_barcode = @"SELECT * FROM xm_xibarcode where orgcode = @orgcode and depot = @depot and site = @site and fileid = @fileid  ";
        private string sqlexssource_getid = "select next value for seq_exsid";
        private string sqlexssource_start = "insert into xm_xiexsource ( orgcode,site,depot,fileid,filename,filetype,filelength,datestart,tflow,datecreate,accncreate,opsdecstart,opsimpstart, imptype ) " + 
        " values ( @orgcode, @site, @depot,@fileid,@filename,@filetype,@filelength,sysdatetimeoffset(),'IO',sysdatetimeoffset(),@accncreate, @opsdecstart, @opsimpstart, @imptype) ";
        private string sqlexsource_error = "update xm_xiexsource set tflow = 'ER',ermsg = @ermsg,dateend = sysdatetimeoffset() where orgcode = @orgcode and site = @site and depot = @depot and fileid = @fileid";
        private string sqlexsbarcode_insert_one = "insert into xm_xibarcode ( orgcode,site,depot,article,pv,lv,barops,barcode,bartype,thcode,orbitsource,tflow,fileid,rowops,dateops,rowid ) " +
        "values ( @orgcode,@site,@depot,@article,@pv,@lv,@barops,@barcode,@bartype,@thcode,@orbitsource,'WC',@fileid,@rowops,sysdatetimeoffset(),next value for seq_ixbarcode)";
        private string sqlexsbarcode_import_step1 = @"update t set tflow = case  when p.article is null then 'ER'
                            when isnull(h.thcode,'') != '' and h.thcode is null then 'ER'
                            when t.rowops not in ('N','E','R') then 'ER'
                            when t.barops not in ('A','I','O','R') then 'ER'
                            when b.bartype is null then 'ER'
                            when o.barcode is null and t.rowops = 'R' then 'ER'
                            else 'IM' end,
            ermsg = case when p.article is null then 'Article not found'
                            when isnull(h.thcode,'') != '' and h.thcode is null then 'Thirdparty is not found'
                            when t.rowops not in ('N','E','R') then 'Row operate code incorrect'
                            when t.barops not in ('A','I','O','R') then 'Barcode opreate type incorrect'
                            when b.bartype is null then 'Type of barcode incorrect'
                            when o.barcode is null and t.rowops = 'E' then 'Auto convert update to insert'
                            when o.barcode is null and t.rowops = 'R' then 'Barcode not found for remove'
                            when o.barcode is not null and t.rowops = 'N' then 'Auto convert from insert to update'
                            else '' end,
            rowops = case when o.barcode is null and t.rowops = 'E' then 'N' when o.barcode is not null and t.rowops = 'N' then 'E' else t.rowops end
        from xm_xibarcode t 
        left join wm_product p on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.article = p.article and t.pv = p.pv and t.lv = p.lv
        left join wm_thparty h on t.orgcode = h.orgcode and t.site = h.site and t.depot = h.depot and t.thcode = h.thcode
        left join wm_barcode o on t.orgcode = o.orgcode and t.site = o.site and t.depot = o.depot and t.barcode = o.barcode 
        left join (select orgcode,site,depot,bnvalue bartype from wm_binary where bntype = 'BARCODE' and bncode = 'TYPE') b on 
        t.orgcode = b.orgcode and t.site = b.site and t.depot = b.depot and t.bartype = b.bartype
        where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.fileid = @fileid and t.tflow = 'WC'";
        private string sqlexsbarcode_import_step2 = @" insert into wm_barcode ( orgcode,site,depot,spcarea,article,pv,lv,barops,barcode,
        bartype,thcode,orbitsource,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify ) 
        select orgcode,site,depot,spcarea,article,pv,lv, barops,barcode,bartype,thcode,orbitsource,
        'IO' tflow, sysdatetimeoffset() datecreate,orbitsource accncreate,sysdatetimeoffset() 
        datemodify,orbitsource accnmodify, orbitsource procmodify 
        from xm_xibarcode t where rowops = 'N' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot 
        and t.fileid = @fileid and t.tflow = 'IM' ";
        private string sqlexsbarcode_import_step3 = @"update t set t.barops = s.barops, t.bartype = s.bartype, 
        t.thcode = s.thcode ,t.orbitsource = s.orbitsource, 
        t.tflow = s.tflow, t.datemodify = sysdatetimeoffset(), 
        t.accnmodify = s.orbitsource,t.procmodify = s.orbitsource 
        from wm_barcode t join xm_xibarcode s on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.barcode = s.barcode 
        where rowops = 'E' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsbarcode_import_step4 = @"delete t from wm_barcode t join xm_xibarcode s on t.orgcode = s.orgcode and t.site = s.site 
        and t.depot = s.depot and t.barcode = s.barcode  where s.rowops = 'R' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsbarcode_import_step5 = @"update xm_xibarcode set tflow = 'ED' where tflow = 'IM' and orgcode = @orgcode 
        and site = @site and depot = @depot and fileid = @fileid ";
        private string sqlexsbarcode_end = " update x set x.dateend = sysdatetimeoffset(), x.opsins = isnull(o.opsins,0), " +
        " x.opsupd = isnull(o.opsupd,0), x.opsrem = isnull(o.opsrem,0), x.opserr = isnull(o.opserr,0), " +
        " x.tflow = 'ED', opsperform = CAST(CONVERT(varchar(12), DATEADD(SECOND, DATEDIFF(SECOND, x.datestart,  sysdatetimeoffset()), 0), 114) AS time(7) ) " +
        " from xm_xiexsource x left join " +
        " (select orgcode,site, depot, fileid, " +
        " sum(case when tflow = 'ED' and rowops in ('N','1') then 1 else 0 end) opsins, " +
        " sum(case when tflow = 'ED' and rowops in ('E','2') then 1 else 0 end) opsupd, " +
        " sum(case when tflow = 'ED' and rowops in ('R','3','9') then 1 else 0 end) opsrem, " +
        " sum(case when tflow in ('ER','XX') then 1 else 0 end) opserr " +
        " from xm_xibarcode  where orgcode = @orgcode and site = @site and depot = @depot and isnull(fileid,'') != '' " +
        " and fileid = @fileid group by orgcode,site, depot, fileid) o " +
        " on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.fileid = o.fileid " +
        " where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid " ;
        
        private string sqlexsbarcode_insert_err = "insert into xm_xibarcode ( orgcode,site,depot,barcode,tflow,ermsg,fileid ) values ( @orgcode,@site,@depot,@barcode,@tflow,@ermsg,@fileid) ";

        //Third party
        private string sqlexssource_thparty = @"SELECT * FROM xm_xithirdparty where orgcode = @orgcode and depot = @depot and site = @site and fileid = @fileid  ";
        private string sqlexsthparty_insert = " insert into xm_xithirdparty ( orgcode,site,depot,thtype,thbutype,thcode,thcodealt,vatcode,thname,thnamealt,addressln1,addressln2,addressln3, " + 
        " subdistrict,district,city,country,postcode,region,telephone,email,thgroup,thcomment,throuteformat,plandelivery,naturalloss,mapaddress,orbitsource, " +
        " tflow,fileid,rowops,dateops,rowid ) values ( @orgcode,@site,@depot,@thtype,@thbutype,@thcode,@thcodealt,@vatcode,@thname,@thnamealt,@addressln1, " +
        " @addressln2,@addressln3,@subdistrict,@district,@city,@country,@postcode,@region,@telephone,@email,@thgroup,@thcomment,@throuteformat,@plandelivery, " +
        " @naturalloss,@mapaddress,@orbitsource,'WC',@fileid,@rowops,sysdatetimeoffset(),next value for seq_ixthirdparty) " ;
        private string sqlexsthparty_import_step1 = @"update t set tflow = case when t.thtype is null then  'ER'
                when t.thbutype is null then 'ER'
                when t.thcode is null then 'ER'
                when h.thcode is null and t.rowops = 'R' then 'ER'
                else 'IM' end,
        ermsg = case when t.thtype is null then  'Thirdparty type is blank'
                when t.thbutype is null then 'Business type is blank'
                when t.thcode is null then 'Thirdparty code is blank'
                when h.thcode is null and t.rowops = 'R' then 'Thparty not found for remove'
                when h.thcode is null and t.rowops = 'E' then 'Auto convert update to insert'
                when h.thcode is not null and t.rowops = 'N' then 'Auto convert insert to update'
                else '' end,
        rowops = case when h.thcode is null and t.rowops = 'E' then 'N'
                    when h.thcode is not null and t.rowops = 'N' then 'E' else t.rowops end
        from  xm_xithirdparty t left join wm_thparty h on t.orgcode = h.orgcode and t.site = h.site and t.depot = h.depot and t.thcode = h.thcode
        where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.fileid = @fileid and t.tflow = 'WC'";

        private string sqlexsthparty_import_step2 = @"insert into wm_thpartyhx select t.* from wm_thparty t join xm_xithirdparty s 
        on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.thcode = s.thcode  where s.rowops in ('E','R') and t.orgcode = @orgcode 
        and t.site = @site and t.depot = @depot and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsthparty_import_step3 =  @" insert into wm_thparty (orgcode,site,depot,spcarea,thtype,thbutype,thcode,thcodealt,vatcode, thname,thnamealt,thnameint,addressln1,addressln2,
        addressln3,subdistrict,district, city,country,postcode,region,telephone,email,thgroup,thcomment,throuteformat, plandelivery,naturalloss,mapaddress,
        orbitsource,tflow) select orgcode,site,depot,spcarea,thtype,thbutype,thcode,thcodealt,vatcode, thname,thnamealt,thnamealt as thnameint, addressln1,
        addressln2,addressln3,subdistrict,district, city,country,postcode,region,telephone,email,thgroup,thcomment,throuteformat, plandelivery,naturalloss,
        mapaddress,orbitsource,'IO' tflow from xm_xithirdparty t where t.rowops = 'N' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot 
        and t.fileid = @fileid and t.tflow = 'IM'";
        private string sqlexsthparty_import_step4 = @"update t  set t.thtype = s.thtype, t.thbutype = s.thbutype,t.thcodealt = s.thcodealt,t.vatcode = s.vatcode,
        t.thname = s.thname,t.thnamealt = s.thnamealt,t.addressln1 = s.addressln1,t.addressln2 = s.addressln2, 
        t.addressln3 = s.addressln3,t.subdistrict = s.subdistrict,t.district = s.district,t.city = s.city, 
        t.country = s.country,t.postcode = s.postcode,t.region = s.region,t.telephone = s.telephone, 
        t.email = s.email,t.thgroup = s.thgroup,t.thcomment = s.thcomment,t.throuteformat = s.throuteformat, 
        t.plandelivery = s.plandelivery,t.naturalloss = s.naturalloss,t.mapaddress = s.mapaddress, 
        t.orbitsource = s.orbitsource,t.tflow = 'IO',t.datemodify = SYSDATETIMEOFFSET(), t.accnmodify = s.orbitsource 
        from wm_thparty t join xm_xithirdparty s on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.thcode = s.thcode where s.rowops = 'E' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsthparty_import_step5 =  @"delete t from wm_thparty t join xm_xithirdparty s on t.orgcode = s.orgcode and t.site = s.site 
        and t.depot = s.depot and t.thcode = s.thcode  where s.rowops = 'R' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsthparty_import_step6 = @"update xm_xithirdparty set tflow = 'ED' where tflow = 'IM' and orgcode = @orgcode 
        and site = @site and depot = @depot and fileid = @fileid ";
        private string sqlexsthparty_end =  @"update x set x.dateend = sysdatetimeoffset(), x.opsins = isnull(o.opsins,0),
        x.opsupd = isnull(o.opsupd,0), x.opsrem = isnull(o.opsrem,0), x.opserr = isnull(o.opserr,0), 
        x.tflow = 'ED', opsperform = CAST(CONVERT(varchar(12), DATEADD(SECOND, DATEDIFF(SECOND, x.datestart,  sysdatetimeoffset()), 0), 114) AS time(7) ) 
        from xm_xiexsource x left join (select orgcode,site, depot, fileid, 
        sum(case when tflow = 'ED' and rowops in ('N','1') then 1 else 0 end) opsins, 
        sum(case when tflow = 'ED' and rowops in ('E','2') then 1 else 0 end) opsupd, 
        sum(case when tflow = 'ED' and rowops in ('R','3','9') then 1 else 0 end) opsrem, 
        sum(case when tflow in ('ER','XX') then 1 else 0 end) opserr 
        from xm_xithirdparty  where orgcode = @orgcode and site = @site and depot = @depot and isnull(fileid,'') != '' 
        and fileid = @fileid group by orgcode,site, depot, fileid) o 
        on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.fileid = o.fileid 
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid ";
        private string sqlexsthparty_insert_err = "insert into xm_xibarcode ( orgcode,site,depot,thcode,tflow,ermsg,fileid ) values ( @orgcode,@site,@depot,@thcode,@tflow,@ermsg,@fileid) ";

        //Inbound
        private string sqlexssource_inbound =  @"SELECT * FROM xm_xiinbound where orgcode = @orgcode and depot = @depot and site = @site and fileid = @fileid ";
        private string sqlexsinbound_insert = " insert into xm_xiinbound ( orgcode,site,depot,spcarea,thcode,intype,subtype,inorder,dateorder,dateplan, " + 
        " dateexpire,inpriority,inflag,inpromo,tflow,orbitsource,fileid,rowops,ermsg,dateops,rowid ) values ( " + 
        " @orgcode,@site,@depot,@spcarea,@thcode,@intype,@subtype,@inorder,@dateorder,@dateplan, " + 
        " @dateexpire,@inpriority,@inflag,@inpromo,@tflow,@orbitsource,@fileid,@rowops,@ermsg,@dateops,next value for seq_ixinbound ) " ;
        //validate 
        private string sqlexsinbound_import_step1 = @"update t set tflow = case when t.thcode is null then 'ER'
                        when h.thcode is null then 'ER'
                        when y.bnvalue is null then 'ER'
                        when s.bnvalue is null then 'ER'
                        when o.dateplan < sysdatetimeoffset() then 'ER'
                        when o.dateexpire < sysdatetimeoffset() then 'ER'
                        when o.inorder is not null and isnull(o.tflow,'IO') != 'IO' and isnull(t.rowops,'N') in ('N','E') then 'ER'
                        else 'IM' end,
           ermsg = case when o.inorder is null and t.rowops = 'E' and o.dateplan > sysdatetimeoffset() and o.dateexpire > sysdatetimeoffset() then 'Auto convert update to insert'
                        when o.inorder is not null and t.rowops = 'N' and o.dateplan > sysdatetimeoffset() and o.dateexpire > sysdatetimeoffset() then 'Auto convert insert to update' 
                        when t.thcode is null then 'Thirdparty code is blank'
                        when h.thcode is null then 'Thirdaprty not found'
                        when y.bnvalue is null then 'Order type incorrect'
                        when s.bnvalue is null then 'Order subtype incorrect'
                        when o.dateplan < sysdatetimeoffset() then 'Plan date is backlog'
                        when o.dateexpire < sysdatetimeoffset() then 'Expire date is backlog'
                        when o.inorder is not null and isnull(o.tflow,'IO') != 'IO' and isnull(t.rowops,'N') in ('N','E') then 'Order has under operate'
                        else '' end,
            rowops = case when o.inorder is null and t.rowops = 'E' then 'N'
                        when o.inorder is not null and t.rowops = 'N' then 'E' else t.rowops end
            from  xm_xiinbound t left join wm_inbound o on
            t.orgcode = o.orgcode and t.site = o.site and t.depot = o.depot and t.inorder = o.inorder 
            left join wm_thparty h on t.orgcode = h.orgcode and t.site = h.site and t.depot = h.depot and t.thcode = h.thcode
            left join (select orgcode,site,depot,bnvalue from wm_binary where bntype = 'ORDER' and bncode = 'TYPE') y
            on t.orgcode = y.orgcode and t.site = y.site and t.depot = y.depot and t.intype = y.bnvalue
            left join (select orgcode,site,depot,bnvalue from wm_binary where bntype = 'ORDER' and bncode = 'SUBTYPE') s
            on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.intype = s.bnvalue
            where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.fileid = @fileid and t.tflow = 'WC'";
        //Insert 
        private string sqlexsinbound_import_step2 =  @"insert into wm_inbound ( orgcode,site,depot,spcarea,thcode,intype,subtype,inorder, dateorder,dateplan,dateexpire, 
        inpriority,inflag, inpromo,tflow,datecreate,accncreate,procmodify,orbitsource,datereplan ) 
        select orgcode,site,depot,spcarea,thcode,intype,subtype,inorder,isnull(SYSDATETIMEOFFSET(),dateorder) dateorder, 
        dateplan,dateexpire,inpriority,next value for seq_inflag inflag,inpromo,'IO' tflow,SYSDATETIMEOFFSET() datecreate, 
        orbitsource accncreate ,  orbitsource procmodify, orbitsource, dateplan datereplan 
        from xm_xiinbound x where x.rowops = 'N' and x.orgcode = @orgcode and x.site = @site and x.depot = @depot 
        and x.fileid = @fileid and x.tflow = 'IM'";
        //Update
        private string sqlexsinbound_import_step3 = @"update t set t.dateplan  = s.dateplan, t.dateexpire = s.dateexpire, t.inpriority = s.inpriority, t.inflag = s.inflag, 
        t.inpromo = s.inpromo, t.tflow = 'IO', t.procmodify = s.orbitsource, t.orbitsource = s.orbitsource, 
        t.datemodify = SYSDATETIMEOFFSET(), accnmodify = s.orbitsource  from wm_inbound t join xm_xiinbound s 
        on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.inorder = s.inorder 
        where s.rowops = 'E' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.fileid = @fileid and s.tflow = 'IM' ";
        //update
        private string sqlexsinbound_import_step4 = @"delete t from wm_inbound t join xm_xiinbound s on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.inorder = s.inorder  where s.rowops = 'R' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM'";
        //setcompleted
        private string sqlexsinbound_import_step5 =  @"update xm_xiinbound set tflow = 'ED' where tflow = 'IM' and orgcode = @orgcode 
        and site = @site and depot = @depot and fileid = @fileid ";
        //Calculdate statistic
        private string sqlexsinbound_end = @"update x set x.dateend = sysdatetimeoffset(), x.opsins = isnull(o.opsins,0),
        x.opsupd = isnull(o.opsupd,0), x.opsrem = isnull(o.opsrem,0), x.opserr = isnull(o.opserr,0), 
        x.tflow = 'ED', opsperform = CAST(CONVERT(varchar(12), DATEADD(SECOND, DATEDIFF(SECOND, x.datestart,  sysdatetimeoffset()), 0), 114) AS time(7) ) 
        from xm_xiexsource x left join (select orgcode,site, depot, fileid, 
        sum(case when tflow = 'ED' and rowops in ('N','1') then 1 else 0 end) opsins, 
        sum(case when tflow = 'ED' and rowops in ('E','2') then 1 else 0 end) opsupd, 
        sum(case when tflow = 'ED' and rowops in ('R','3','9') then 1 else 0 end) opsrem, 
        sum(case when tflow in ('ER','XX') then 1 else 0 end) opserr 
        from xm_xiinbound  where orgcode = @orgcode and site = @site and depot = @depot and isnull(fileid,'') != '' 
        and fileid = @fileid group by orgcode,site, depot, fileid) o 
        on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.fileid = o.fileid 
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid ";
        
        private string sqlexsinbound_insert_err = "insert into xm_xiinbound ( orgcode,site,depot,inorder,tflow,ermsg,fileid ) values ( @orgcode,@site,@depot,@inorder,@tflow,@ermsg,@fileid) ";

        //Inbouln
        private string sqlexssource_inbouln = @"SELECT * FROM xm_xiinbouln where orgcode = @orgcode and depot = @depot and site = @site and fileid = @fileid ";
        private string sqlexsinbouln_insert = " insert into xm_xiinbouln ( orgcode,site,depot,inorder,inln,inrefno,inrefln,article,pv,lv, " +
        " unitops,qtysku,qtypu,qtyweight,batchno,lotno,expdate,serialno,orbitsource,tflow,fileid,rowops, " +
        " ermsg,dateops,rowid ) values ( @orgcode,@site,@depot,@inorder,@inln,@inrefno,@inrefln,@article,@pv,@lv, " +
        " @unitops,@qtysku,@qtypu,@qtyweight,@batchno,@lotno,@expdate,@serialno,@orbitsource,@tflow,@fileid,@rowops, " +
        " @ermsg,sysdatetimeoffset(),next value for seq_ixinbouln ) ";
        private string sqlexsinbouln_import_step1 = @"update t set
            t.tflow = case  when r.inorder is null then 'ER' 
            when isnull(o.tflow,'IO') != 'IO' then 'ER' 
            when p.article is null then 'ER' 
            when h.thcode is null then 'ER' 
            when a.barcode is null then 'ER' 
            when isnull(t.rowops,'') not in ('N','E','R') then 'ER' 
            when t.rowops not in ('N','E','R') then 'ER' 
            when isnull(o.tflow,'IO') != 'IO' then 'ER' 
            else 'IM' end,
            t.ermsg = case  when r.inorder is null then 'Header inbond missing' 
            when isnull(o.tflow,'IO') != 'IO' then 'Order has operated'
            when p.article is null then 'Product not found'
            when h.thcode is null then 'Thirdparty not found'
            when a.barcode is null then 'Primary barcode not found'
            when isnull(t.rowops,'') not in ('N','E','R') then 'Row operate code incorrect'
            when t.rowops not in ('N','E','R') then 'Row operate code incorrect'
            when isnull(o.tflow,'IO') != 'IO' then 'Line has operated'
            when o.inorder is null and t.rowops = 'E' then 'Auto convert update to insert'
            when o.inorder is not null and t.rowops = 'N' then 'Auto convert insert to update'
            else '' end,    
            t.qtypu = t.qtysku / case when p.unitreceipt = 1 then 1 when p.unitreceipt = 2 then p.rtoskuofipck when p.unitreceipt = 3 then p.rtoskuofpck when p.unitreceipt = 4 then p.rtoskuoflayer when p.unitreceipt = 5 then p.rtoskuofhu else  1 end ,
            t.qtyweight = t.qtysku * p.skuweight,t.unitops = p.unitreceipt, t.spcarea = o.spcarea
        from xm_xiinbouln t 
        left join wm_inbouln o on t.orgcode = o.orgcode and t.site = o.site and t.depot = o.depot and t.inorder = o.inorder and t.inln = o.inln
        left join (select * from wm_inbound where orgcode = @orgcode and site = @site and depot = @depot ) r on t.inorder = r.inorder
        left join (select * from wm_product where orgcode = @orgcode and site = @site and depot = @depot ) p on t.article = p.article and t.pv = p.pv and t.lv = p.lv
        left join (select * from wm_thparty where orgcode = @orgcode and site = @site and depot= @depot  ) h on p.thcode = h.thcode
        left join (select * from wm_barcode where site = @site and depot = @depot and orgcode = @orgcode and isprimary = '1') a 
        on t.article = a.article and t.pv = a.pv and t.lv = a.lv
        where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.fileid = @fileid and t.tflow = 'WC'";
        private string sqlexsinbouln_import_step2 = @"insert into wm_inbouln ( orgcode,site,depot,spcarea,inorder,inln,inrefno,inrefln,inagrn,barcode,article,pv,lv,unitops, 
        qtysku,qtypu, qtyweight,lotno,expdate,serialno,qtypnd,tflow,datecreate,accncreate,procmodify,batchno,orbitsource, inseq ) 
        select x.orgcode,x.site,x.depot,x.spcarea, inorder,inln,inrefno,inrefln,next value for seq_agrn over (order by rowid) inagrn,
        p.barcode, x.article,x.pv, x.lv, unitops,qtysku,qtypu qtypu, qtyweight, lotno,expdate,serialno,qtysku qtypnd,'IO' tflow,
        sysdatetimeoffset() datecreate,  x.orbitsource accncreate,x.orbitsource procmodify, batchno,x.orbitsource, 
        row_number() over (partition by x.orgcode,x.site,x.depot, x.inorder order by x.orgcode,x.site,x.depot, x.inorder, x.inln) inseq 
        from xm_xiinbouln x left join (select * from wm_barcode where isprimary = 1 and orgcode = @orgcode and site = @site 
        and depot = @depot ) p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article 
        and x.pv = p.pv and x.lv = p.lv where x.rowops = 'N' and x.orgcode = @orgcode and x.site = @site and x.depot = @depot 
        and x.fileid = @fileid and x.tflow = 'IM' ";
        private string sqlexsinbouln_import_step3 = @"update t set t.unitops = s.unitops, t.qtysku = s.qtysku,t.qtypu = s.qtypu,t.qtyweight = s.qtyweight, 
            t.lotno = s.lotno, t.expdate = s.expdate,t.serialno = s.serialno,t.qtypnd = s.qtysku,t.tflow = 'IO', 
            t.datemodify = sysdatetimeoffset(), t.accnmodify = s.orbitsource,t.procmodify = s.orbitsource,t.batchno = s.batchno, 
            t.orbitsource = s.orbitsource 
        from wm_inbouln t join xm_xiinbouln s on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.inorder = s.inorder and t.inln = s.inln and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
        where s.rowops = 'E' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM' ";
        private string sqlexsinbouln_import_step4 = @"delete t from wm_inbouln t join xm_xiinbouln s on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.inorder = s.inorder and t.inln = s.inln and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
        where s.rowops = 'R' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM' ";
        private string sqlexsinbouln_import_step5 = @"update xm_xiinbouln set tflow = 'ED' where tflow = 'IM' and orgcode = @orgcode 
        and site = @site and depot = @depot and fileid = @fileid ";
        private string sqlexsinbouln_end = @"update x set x.dateend = sysdatetimeoffset(), x.opsins = isnull(o.opsins,0),
        x.opsupd = isnull(o.opsupd,0), x.opsrem = isnull(o.opsrem,0), x.opserr = isnull(o.opserr,0), 
        x.tflow = 'ED', opsperform = CAST(CONVERT(varchar(12), DATEADD(SECOND, DATEDIFF(SECOND, x.datestart,  sysdatetimeoffset()), 0), 114) AS time(7) ) 
        from xm_xiexsource x left join (select orgcode,site, depot, fileid, 
        sum(case when tflow = 'ED' and rowops in ('N','1') then 1 else 0 end) opsins, 
        sum(case when tflow = 'ED' and rowops in ('E','2') then 1 else 0 end) opsupd, 
        sum(case when tflow = 'ED' and rowops in ('R','3','9') then 1 else 0 end) opsrem, 
        sum(case when tflow in ('ER','XX') then 1 else 0 end) opserr 
        from xm_xiinbouln  where orgcode = @orgcode and site = @site and depot = @depot and isnull(fileid,'') != '' 
        and fileid = @fileid group by orgcode,site, depot, fileid) o 
        on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.fileid = o.fileid 
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid ";
        private string sqlexsinbouln_insert_err = "insert into xm_xiinbouln ( orgcode,site,depot,inorder,tflow,ermsg,fileid ) values ( @orgcode,@site,@depot,@inorder,@tflow,@ermsg,@fileid)";

        //Outbound
        private string sqlexssource_outbound = @"SELECT * FROM xm_xioutbound where orgcode = @orgcode and depot = @depot and site = @site and fileid = @fileid ";
        private string sqlexsoutbound_insert = " insert into xm_xioutbound (orgcode,site,depot,ouorder,outype,ousubtype,thcode,dateorder,dateprep, " +
        " dateexpire,oupriority,ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,stoaddressln2, " +
        " stoaddressln3,stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,tflow,inorder, " +
        " fileid,rowops,ermsg,dateops,rowid) values  " +
        " (@orgcode,@site,@depot,@ouorder,@outype,@ousubtype,@thcode,@dateorder,@dateprep, " +
        " @dateexpire,@oupriority,@ouflag,@oupromo,@dropship,@orbitsource,@stocode,@stoname,@stoaddressln1,@stoaddressln2, " +
        " @stoaddressln3,@stosubdistict,@stodistrict,@stocity,@stocountry,@stopostcode,@stomobile,@stoemail,@tflow,@inorder, " +
        " @fileid,@rowops,@ermsg,sysdatetimeoffset(),next value for seq_ixoutbound ) ";
        private string sqlexsoutbound_import_step1 = @"update t set tflow = case when t.thcode is null then 'ER'
        when h.thcode is null then 'ER'
        when y.bnvalue is null then 'ER'
        when s.bnvalue is null then 'ER'
        when o.dateprep < sysdatetimeoffset() then 'ER'
        when o.dateexpire < sysdatetimeoffset() then 'ER'
        else 'IM' end,
        ermsg = case when t.thcode is null then 'Thirdparty code is blank'
        when h.thcode is null then 'Thirdaprty not found'
        when o.ouorder is null and t.rowops = 'E' then 'Auto convert update to insert'
        when o.ouorder is not null and t.rowops = 'N' then 'Auto convert insert to update'
        when y.bnvalue is null then 'Order type incorrect'
        when s.bnvalue is null then 'Order subtype incorrect'
        when o.dateprep < sysdatetimeoffset() then 'Preparation date is backlog'
        when o.dateexpire < sysdatetimeoffset() then 'Expire date is backlog'
        else '' end,
        rowops = case when o.ouorder is null and t.rowops = 'E' then 'N'
        when o.ouorder is not null and t.rowops = 'N' then 'E' else t.rowops end
        from xm_xioutbound t left join wm_outbound o 
        on t.orgcode = o.orgcode and t.site = o.site and t.depot = o.depot and t.ouorder = o.ouorder 
        left join wm_thparty h on t.orgcode = h.orgcode and t.site = h.site and t.depot = h.depot and t.thcode = h.thcode
        left join (select orgcode,site,depot,bnvalue from wm_binary where bntype = 'ORDER' and bncode = 'TYPE') y
        on t.orgcode = y.orgcode and t.site = y.site and t.depot = y.depot and t.outype = y.bnvalue
        left join (select orgcode,site,depot,bnvalue from wm_binary where bntype = 'ORDER' and bncode = 'SUBTYPE') s
        on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.outype = s.bnvalue
        where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.fileid = @fileid and t.tflow = 'WC'";

        private string sqlexsoutbound_import_step2 = @"insert into wm_outbound ( orgcode,site,depot,spcarea,ouorder,outype,ousubtype,thcode,dateorder,dateprep,
        dateexpire,oupriority, ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,stoaddressln2,
        stoaddressln3, stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,datereqdel,
        tflow, datecreate,accncreate,procmodify,inorder,qtyorder, qtypnd ) 
        select orgcode,site,depot,isnull(NULLIF(spcarea, ''),'ST') spcarea,ouorder,outype,ousubtype,thcode,
        dateorder,dateprep,dateexpire,oupriority, ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,
        stoaddressln2,stoaddressln3, stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,
        dateprep datereqdel,'IO' tflow, SYSDATETIMEOFFSET() ,orbitsource accncreate,orbitsource procmodify,inorder,
        0 qtyorder,0 qtypnd from xm_xioutbound x where x.rowops = 'N' and x.orgcode = @orgcode and x.site = @site 
        and x.depot = @depot and x.fileid = @fileid and x.tflow = 'IM'";
        private string sqlexsoutbound_import_step3 = @"update t set t.dateprep = s.dateprep,t.dateexpire = s.dateexpire , t.oupriority = s.oupriority, 
        t.ouflag = s.ouflag, t.oupromo = s.oupromo,t.dropship = s.dropship,t.orbitsource = s.orbitsource, 
        t.stoaddressln1 = s.stoaddressln1,t.stoaddressln2 = s.stoaddressln2,t.stoaddressln3 = s.stoaddressln3, 
        t.stosubdistict = s.stosubdistict,t.stodistrict = s.stodistrict,t.stocity = s.stocity, 
        t.stocountry = s.stocountry,t.stopostcode = s.stopostcode,t.stomobile = s.stomobile,t.stoemail = s.stoemail, 
        t.datemodify = SYSDATETIMEOFFSET(),t.accnmodify = s.orbitsource from wm_outbound t join xm_xioutbound s 
        on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder 
        where s.rowops = 'E' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsoutbound_import_step4 = @"delete t from wm_outbound t join xm_xioutbound s on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.ouorder = s.ouorder where s.rowops = 'R' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsoutbound_import_step5 = @"update xm_xioutbound set tflow = 'ED' where tflow = 'IM' and orgcode = @orgcode 
        and site = @site and depot = @depot and fileid = @fileid ";
        private string sqlexsoutbound_end = @"update x set x.dateend = sysdatetimeoffset(), x.opsins = isnull(o.opsins,0),
        x.opsupd = isnull(o.opsupd,0), x.opsrem = isnull(o.opsrem,0), x.opserr = isnull(o.opserr,0), 
        x.tflow = 'ED', opsperform = CAST(CONVERT(varchar(12), DATEADD(SECOND, DATEDIFF(SECOND, x.datestart,  sysdatetimeoffset()), 0), 114) AS time(7) ) 
        from xm_xiexsource x left join (select orgcode,site, depot, fileid, 
        sum(case when tflow = 'ED' and rowops in ('N','1') then 1 else 0 end) opsins, 
        sum(case when tflow = 'ED' and rowops in ('E','2') then 1 else 0 end) opsupd, 
        sum(case when tflow = 'ED' and rowops in ('R','3','9') then 1 else 0 end) opsrem, 
        sum(case when tflow in ('ER','XX') then 1 else 0 end) opserr 
        from xm_xioutbound  where orgcode = @orgcode and site = @site and depot = @depot and isnull(fileid,'') != '' 
        and fileid = @fileid group by orgcode,site, depot, fileid) o 
        on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.fileid = o.fileid 
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid ";
        private string sqlexsoutbound_insert_err = "insert into xm_xioutbound ( orgcode,site,depot,ouorder,tflow,ermsg,fileid ) values ( @orgcode,@site,@depot,@ouorder,@tflow,@ermsg,@fileid)";

        //Outbouln 
        private string sqlexssource_outbouln = @"SELECT * FROM xm_xioutbouln where orgcode = @orgcode and depot = @depot and site = @site and fileid = @fileid ";
        private string sqlexsoutbouln_insert = " insert into xm_xioutbouln ( orgcode,site,depot,ouorder,ouln,ourefno,ourefln,inorder, " + 
        " article,pv,lv,unitops,qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,serialno, " + 
        " orbitsource,tflow,disthcode,fileid,rowops,ermsg,dateops,rowid,ouseq ) values " + 
        " ( @orgcode,@site,@depot,@ouorder,@ouln,@ourefno,@ourefln,@inorder, " + 
        "   @article,@pv,@lv,@unitops,@qtysku,@qtypu,@qtyweight,@spcselect,@batchno,@lotno,@datemfg," + 
        "   @dateexp,@serialno,@orbitsource,@tflow,@disthcode,@fileid,@rowops,@ermsg,sysdatetimeoffset(),@rowid,next value for seq_ixoutbouln ) ";
        private string sqlexsoutbouln_import_step1 = @"update t set
            t.tflow = case  when r.ouorder is null then 'ER' 
            when isnull(o.tflow,'IO') != 'IO' then 'ER' 
            when p.article is null then 'ER' 
            when h.thcode is null then 'ER' 
            when a.barcode is null then 'ER' 
            when isnull(t.rowops,'') not in ('N','E','R') then 'ER' 
            when t.rowops not in ('N','E','R') then 'ER' 
            when isnull(o.tflow,'IO') != 'IO' then 'ER' 
            else 'IM' end,
            t.ermsg = case  when r.ouorder is null then 'Header outbound missing' 
            when isnull(o.tflow,'IO') != 'IO' then 'Order has operated'
            when p.article is null then 'Product not found'
            when h.thcode is null then 'Thirdparty not found'
            when a.barcode is null then 'Primary barcode not found'
            when isnull(t.rowops,'') not in ('N','E','R') then 'Row operate code incorrect'
            when t.rowops not in ('N','E','R') then 'Row operate code incorrect'
            when isnull(o.tflow,'IO') != 'IO' then 'Line has operated'
            when o.ouorder is null and t.rowops = 'E' then 'Auto convert update to insert'
            when o.ouorder is not null and t.rowops = 'N' then 'Auto convert insert to update'
            else '' end,    
            t.qtypu = t.qtysku / case when p.unitprep = 1 then 1 when p.unitprep = 2 then p.rtoskuofipck when p.unitprep = 3 then p.rtoskuofpck when p.unitprep = 4 then p.rtoskuoflayer when p.unitprep = 5 then p.rtoskuofhu else  1 end ,
            t.qtyweight = t.qtysku * p.skuweight,t.unitops = p.unitprep, t.spcarea = o.spcarea
        from xm_xioutbouln t 
        left join wm_outbouln o on t.orgcode = o.orgcode and t.site = o.site and t.depot = o.depot and t.ouorder = o.ouorder and t.ouln = o.ouln
        left join (select * from wm_outbound where orgcode = @orgcode and site = @site and depot = @depot ) r on t.ouorder = r.ouorder
        left join (select * from wm_product where orgcode = @orgcode and site = @site and depot = @depot ) p on t.article = p.article and t.pv = p.pv and t.lv = p.lv
        left join (select * from wm_thparty where orgcode = @orgcode and site = @site and depot= @depot  ) h on p.thcode = h.thcode
        left join (select * from wm_barcode where site = @site and depot = @depot and orgcode = @orgcode and isprimary = '1') a 
        on t.article = a.article and t.pv = a.pv and t.lv = a.lv
        where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.fileid = @fileid and t.tflow = 'WC'";

        private string sqlexsoutbouln_import_step2 = @"insert into wm_outbouln 
        (orgcode,site,depot,spcarea,ouorder,ouln,ourefno,ourefln,inorder,b.barcode,article, pv,lv,unitops,qtysku,qtypu,qtyweight,spcselect,
        batchno,lotno,datemfg,dateexp, serialno,tflow,datecreate,accncreate,disthcode,qtypndsku,qtypndpu,qtyreqpu,  procmodify,orbitsource,
        ouseq,oudono,qtyreqsku ) 
        select x.orgcode,x.site,x.depot,isnull(NULLIF(x.spcarea, ''),'ST') spcarea,ouorder,ouln,ourefno,ourefln,inorder,barcode,x.article, 
        x.pv,x.lv,unitops,qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,serialno,'IO' tflow,sysdatetimeoffset() datecreate,
        x.orbitsource accncreate,disthcode,qtysku qtypndsku,qtypu qtypndpu, qtypu qtyreqpu, x.orbitsource procmodify,x.orbitsource,
        row_number() over (partition by x.orgcode,x.site,x.depot, x.ouorder order by x.orgcode,x.site,x.depot, x.ouorder, x.ouln) ouseq,
        next value for seq_dono over (order by rowid) , x.qtysku from xm_xioutbouln x left join wm_barcode b on x.orgcode = b.orgcode 
        and x.site = b.site and x.depot = b.depot and x.article = b.article and b.tflow = 'IO' where x.rowops = 'N' and x.orgcode = @orgcode 
        and x.site = @site and x.depot = @depot and x.fileid = @fileid and x.tflow = 'IM'";
        private string sqlexsoutbouln_import_step3 =  @"update t set t.unitops = s.unitops ,t.qtysku = s.qtysku,t.qtypu = s.qtypu, 
        t.qtyweight = s.qtyweight, t.batchno = s.batchno,t.lotno = s.lotno,t.datemfg = s.datemfg, 
        t.dateexp = s.dateexp,t.serialno = s.serialno ,t.qtypndsku = s.qtysku, 
        t.qtypndpu = s.qtypu,t.orbitsource = s.orbitsource,t.datemodify = sysdatetimeoffset(), 
        t.accnmodify = s.orbitsource , t.qtyreqsku = s.qtysku, t.qtyreqpu = s.qtypu 
        from wm_outbouln t join xm_xioutbouln s on  t.orgcode = s.orgcode and t.site = s.site 
        and t.depot = s.depot and t.ouorder = s.ouorder and t.ouln = s.ouln and t.article = s.article 
        and t.pv = s.pv and t.lv = s.lv where s.rowops = 'E' and s.orgcode = @orgcode and s.site = @site 
        and s.depot = @depot and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsoutbouln_import_step4 = @"delete t from wm_outbouln t join xm_xioutbouln s on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.ouorder = s.ouorder and t.ouln = s.ouln where s.rowops = 'R' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot 
        and s.fileid = @fileid and s.tflow = 'IM'";
        private string sqlexsoutbouln_import_step5 = @"update xm_xioutbouln set tflow = 'ED' where tflow = 'IM' and orgcode = @orgcode 
        and site = @site and depot = @depot and fileid = @fileid ";
        private string sqlexsoutbouln_end = @"update x set x.dateend = sysdatetimeoffset(), x.opsins = isnull(o.opsins,0),
        x.opsupd = isnull(o.opsupd,0), x.opsrem = isnull(o.opsrem,0), x.opserr = isnull(o.opserr,0), 
        x.tflow = 'ED', opsperform = CAST(CONVERT(varchar(12), DATEADD(SECOND, DATEDIFF(SECOND, x.datestart,  sysdatetimeoffset()), 0), 114) AS time(7) ) 
        from xm_xiexsource x left join (select orgcode,site, depot, fileid, 
        sum(case when tflow = 'ED' and rowops in ('N','1') then 1 else 0 end) opsins, 
        sum(case when tflow = 'ED' and rowops in ('E','2') then 1 else 0 end) opsupd, 
        sum(case when tflow = 'ED' and rowops in ('R','3','9') then 1 else 0 end) opsrem, 
        sum(case when tflow in ('ER','XX') then 1 else 0 end) opserr 
        from xm_xioutbouln  where orgcode = @orgcode and site = @site and depot = @depot and isnull(fileid,'') != '' 
        and fileid = @fileid group by orgcode,site, depot, fileid) o 
        on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.fileid = o.fileid 
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid ";
        private string sqlexsoutbouln_insert_err = "insert into xm_xioutbouln ( orgcode,site,depot,ouorder,tflow,ermsg,fileid ) values ( @orgcode,@site,@depot,@ouorder,@tflow,@ermsg,@fileid)";

        //Product 
        private string sqlexssource_product = @"SELECT * FROM xm_xiproduct where orgcode = @orgcode and depot = @depot and site = @site and fileid = @fileid ";
        private string sqlexsproduct_insert = " insert into xm_xiproduct ( orgcode,site,depot,article,articletype,pv,lv,description,descalt,thcode,dlcall, " + 
        " dlcfactory,dlcwarehouse,dlcshop,dlconsumer,hdivison,hdepartment,hsubdepart,hclass,hsubclass,typemanage,unitmanage, " + 
        " unitdesc,unitreceipt,unitprep,unitsale,unitstock,unitweight,unitdimension,unitvolume,hucode,rtoskuofpu,rtoskuofipck, " + 
        " rtoskuofpck,rtoskuoflayer,rtoskuofhu,rtopckoflayer,rtolayerofhu,innaturalloss,ounaturalloss,costinbound,costoutbound, " + 
        " costavg,skulength,skuwidth,skuheight,skugrossweight,skuweight,skuvolume,pulength,puwidth,puheight,pugrossweight,puweight, " + 
        " puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight,ipckvolume,pcklength,pckwidth,pckheight,pckgrossweight, " + 
        " pckweight,pckvolume,layerlength,layerwidth,layerheight,layergrossweight,layerweight,layervolume,hulength,huwidth, " + 
        " huheight,hugrossweight,huweight,huvolume,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription,isdlc,ismaterial, " + 
        " isunique,isalcohol,istemperature,isdynamicpick,ismixingprep,isfinishgoods,isnaturalloss,isbatchno,ismeasurement,roomtype, " + 
        " tempmin,tempmax,alcmanage,alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin,stockthresholdmax, " + 
        " spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation,spcprepzone,spcdistzone,spcdistshare,spczonedelv, " + 
        " orbitsource,tflow,fileid,rowops,datecreate,dateops,ermsg,rowid) values ( " + 
        " @orgcode,@site,@depot,@article,@articletype,@pv,@lv,@description,@descalt,@thcode,@dlcall, " + 
        " @dlcfactory,@dlcwarehouse,@dlcshop,@dlconsumer,@hdivison,@hdepartment,@hsubdepart,@hclass,@hsubclass,@typemanage,@unitmanage, " + 
        " @unitdesc,@unitreceipt,@unitprep,@unitsale,@unitstock,@unitweight,@unitdimension,@unitvolume,@hucode,@rtoskuofpu,@rtoskuofipck, " + 
        " @rtoskuofpck,@rtoskuoflayer,@rtoskuofhu,@rtopckoflayer,@rtolayerofhu,@innaturalloss,@ounaturalloss,@costinbound,@costoutbound, " + 
        " @costavg,@skulength,@skuwidth,@skuheight,@skugrossweight,@skuweight,@skuvolume,@pulength,@puwidth,@puheight,@pugrossweight,@puweight, " + 
        " @puvolume,@ipcklength,@ipckwidth,@ipckheight,@ipckgrossweight,@ipckweight,@ipckvolume,@pcklength,@pckwidth,@pckheight,@pckgrossweight, " + 
        " @pckweight,@pckvolume,@layerlength,@layerwidth,@layerheight,@layergrossweight,@layerweight,@layervolume,@hulength,@huwidth, " + 
        " @huheight,@hugrossweight,@huweight,@huvolume,@isdangerous,@ishighvalue,@isfastmove,@isslowmove,@isprescription,@isdlc,@ismaterial, " + 
        " @isunique,@isalcohol,@istemperature,@isdynamicpick,@ismixingprep,@isfinishgoods,@isnaturalloss,@isbatchno,@ismeasurement,@roomtype, " + 
        " @tempmin,@tempmax,@alcmanage,@alccategory,@alccontent,@alccolor,@dangercategory,@dangerlevel,@stockthresholdmin,@stockthresholdmax, " + 
        " @spcrecvzone,@spcrecvaisle,@spcrecvbay,@spcrecvlevel,@spcrecvlocation,@spcprepzone,@spcdistzone,@spcdistshare,@spczonedelv, " + 
        " @orbitsource,@tflow,@fileid,@rowops,sysdatetimeoffset(),sysdatetimeoffset(),@ermsg,next value for seq_ixproduct ) " ;
        private string sqlexsproduct_import_step1 = @"update t set t.tflow = case when h.thcode is null then 'ER'
        when isnull(t.rowops,'') not in ('N','E','R') then 'ER'
        when o.article is null and t.rowops = 'E' then 'ER'
        when o.article is not null and t.rowops = 'N' then 'ER'
        else 'IM' end,
        t.ermsg = case when h.thcode is null then 'Thirdparty not found'
        when isnull(t.rowops,'') not in ('N','E','R') then 'Row operate code incorrect'
        when o.article is null and t.rowops = 'E' then 'Auto convert update to insert'
        when o.article is not null and t.rowops = 'N' then 'Auto convert insert to update'
        else '' end
        from xm_xiproduct t 
        left join wm_product o on t.orgcode = o.orgcode and t.site = o.site and t.depot = o.depot and t.article = o.article and t.pv = o.pv and t.lv = o.lv
        left join (select * from wm_thparty where orgcode = @orgcode and site = @site and depot= @depot  ) h on t.thcode = h.thcode
        where t.orgcode = @orgcode and t.site = @site and t.depot = @depot and t.fileid = @fileid and t.tflow = 'WC'";
        private string sqlexsproduct_import_step2 = @"insert into wm_product 
        ( orgcode,site,depot,spcarea,article,articletype,pv,lv,description,descalt,thcode 
        ,dlcall,dlcfactory,dlcwarehouse,dlcshop,dlconsumer,hdivison,hdepartment,hsubdepart 
        ,hclass,hsubclass,typemanage,unitmanage,unitdesc,unitreceipt,unitprep,unitsale 
        ,unitstock,unitweight,unitdimension,unitvolume,hucode,rtoskuofpu,rtoskuofipck 
        ,rtoskuofpck,rtoipckofpck,rtoskuoflayer,rtoskuofhu,rtopckoflayer,rtolayerofhu,innaturalloss 
        ,ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight 
        ,skugrossweight,skuweight,skuvolume,pulength,puwidth,puheight,pugrossweight 
        ,puweight,puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight 
        ,ipckvolume,pcklength,pckwidth,pckheight,pckgrossweight,pckweight,pckvolume 
        ,layerlength,layerwidth,layerheight,layergrossweight,layerweight,layervolume 
        ,hulength,huwidth,huheight,hugrossweight,huweight,huvolume 
        ,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription,isdlc,ismaterial 
        ,isunique,isalcohol,istemperature,isdynamicpick,ismixingprep,isfinishgoods 
        ,isnaturalloss,isbatchno,ismeasurement,roomtype,tempmin,tempmax,alcmanage 
        ,alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin 
        ,stockthresholdmax,spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation 
        ,spcprepzone,spcdistzone,spcdistshare,spczonedelv,orbitsource,tflow,datecreate,accncreate) 
        select x.orgcode,x.site,x.depot,spcarea,article,articletype,pv,lv,description,descalt,thcode 
        ,dlcall,dlcfactory,dlcwarehouse,dlcshop,dlconsumer,hdivison,hdepartment,hsubdepart 
        ,hclass,hsubclass,'PP' typemanage,unitmanage,unitdesc,unitreceipt,unitprep,unitsale 
        ,unitstock,unitweight,unitdimension,unitvolume,hucode,rtoskuofpu,rtoskuofipck 
        ,rtoskuofpck,rtoipckofpck,rtoskuoflayer,rtoskuofhu,rtopckoflayer,rtolayerofhu,innaturalloss 
        ,ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight 
        ,skugrossweight,skuweight,skuvolume,pulength,puwidth,puheight,pugrossweight 
        ,puweight,puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight 
        ,ipckvolume,pcklength,pckwidth,pckheight,pckgrossweight,pckweight,pckvolume 
        ,layerlength,layerwidth,layerheight,layergrossweight,layerweight,layervolume 
        ,hulength,huwidth,huheight,hugrossweight,huweight,huvolume 
        ,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription,isdlc,ismaterial 
        ,isunique,isalcohol,istemperature,isdynamicpick,ismixingprep,isfinishgoods 
        ,isnaturalloss,isbatchno,1 ismeasurement, roomtype,tempmin,tempmax,alcmanage 
        ,alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin 
        ,stockthresholdmax,'A' spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation 
        ,przone spcprepzone,dbzone spcdistzone,spcdistshare,spczonedelv,orbitsource,'IO' tflow,SYSDATETIMEOFFSET(), 
        orbitsource accncreate from xm_xiproduct x
        left join (select orgcode, site, depot, przone from wm_loczn where isprimary = 1 and spcarea = 'ST') dfprep 
        on x.orgcode = dfprep.orgcode and x.site = dfprep.site and x.depot = dfprep.depot 
        left join (select orgcode, site, depot, przone dbzone from wm_loczn where isprimary = 1 and spcarea = 'XD') dfdist 
        on x.orgcode = dfdist.orgcode and x.site = dfdist.site and x.depot = dfdist.depot 
        where x.rowops = 'N' and x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid 
        and x.tflow = 'IM' ";

        private string sqlexsproduct_import_step3 = @"insert into wm_producthx 
        select t.* from wm_product t join xm_xiproduct s 
            on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode 
        and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
        where s.rowops in ('R','E') and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.fileid = @fileid 
        and s.tflow = 'IM'";
        private string sqlexsproduct_import_step4 = @" update t set t.orgcode = s.orgcode,t.site = s.site,t.depot = s.depot,t.spcarea = s.spcarea,t.article = s.article, 
        t.pv = s.pv,t.lv = s.lv,t.description = s.description,t.descalt = s.descalt, t.thcode = s.thcode,t.dlcall = s.dlcall,
        t.dlcfactory = s.dlcfactory,t.dlcwarehouse = s.dlcwarehouse, t.dlcshop = s.dlcshop,t.dlconsumer = s.dlconsumer,
        t.hdivison = s.hdivison,t.hdepartment = s.hdepartment, t.hsubdepart = s.hsubdepart,t.hclass = s.hclass,t.hsubclass = s.hsubclass, 
        t.unitmanage = s.unitmanage,t.unitdesc = s.unitdesc,t.unitreceipt = s.unitreceipt,t.unitprep = s.unitprep, 
        t.unitsale = s.unitsale,t.unitstock = s.unitstock,t.unitweight = s.unitweight,t.unitdimension = s.unitdimension, 
        t.unitvolume = s.unitvolume,t.rtoskuofpu = s.rtoskuofpu,t.rtoskuofipck = s.rtoskuofipck, t.rtoskuofpck = s.rtoskuofpck,
        t.rtoskuoflayer = s.rtoskuoflayer,t.rtoskuofhu = s.rtoskuofhu, t.innaturalloss = s.innaturalloss, t.ounaturalloss = s.ounaturalloss,
        t.costinbound = s.costinbound,t.costoutbound = s.costoutbound, t.costavg = s.costavg,t.skulength = s.skulength,
        t.skuwidth = s.skuwidth,t.skuheight = s.skuheight, t.skugrossweight = s.skugrossweight,t.skuweight = s.skuweight,
        t.skuvolume = s.skuvolume,t.pulength = s.pulength, t.puwidth = s.puwidth,t.puheight = s.puheight,t.pugrossweight = s.pugrossweight,
        t.puweight = s.puweight, t.puvolume = s.puvolume,t.ipcklength = s.ipcklength,t.ipckwidth = s.ipckwidth,t.ipckheight = s.ipckheight, 
        t.ipckgrossweight = s.ipckgrossweight,t.ipckweight = s.ipckweight,t.ipckvolume = s.ipckvolume,t.pcklength = s.pcklength,
        t.pckwidth = s.pckwidth,t.pckheight = s.pckheight,t.pckgrossweight = s.pckgrossweight,t.pckweight = s.pckweight, 
        t.pckvolume = s.pckvolume,t.layerlength = s.layerlength,t.layerwidth = s.layerwidth,t.layerheight = s.layerheight, 
        t.layergrossweight = s.layergrossweight,t.layerweight = s.layerweight,t.layervolume = s.layervolume, 
        t.hulength = s.hulength,t.huwidth = s.huwidth,t.huheight = s.huheight,t.hugrossweight = s.hugrossweight, 
        t.huweight = s.huweight,t.huvolume = s.huvolume,t.tempmin = s.tempmin,t.tempmax = s.tempmax, 
        t.alcmanage = s.alcmanage,t.alccategory = s.alccategory,t.alccontent = s.alccontent,t.alccolor = s.alccolor, 
        t.dangercategory = s.dangercategory,t.dangerlevel = s.dangerlevel,t.stockthresholdmin = s.stockthresholdmin, 
        t.stockthresholdmax = s.stockthresholdmax,t.datemodify = sysdatetimeoffset(),t.orbitsource = s.orbitsource, 
        t.procmodify = s.orbitsource, t.accnmodify = s.orbitsource from wm_product t join xm_xiproduct s 
            on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode 
        and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
        where s.rowops = 'E' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.fileid = @fileid 
        and s.tflow = 'IM'";
        private string sqlexsproduct_import_step5 = @"delete t from  wm_product t join xm_xiproduct s   on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot 
        and t.thcode = s.thcode and t.article = s.article and t.pv = s.pv and t.lv = s.lv 
        where s.rowops = 'R' and s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.fileid = @fileid 
        and s.tflow = 'IM'";
        private string sqlexsproduct_import_step6 = @"update xm_xiproduct set tflow = 'ED' where tflow = 'IM' and orgcode = @orgcode 
        and site = @site and depot = @depot and fileid = @fileid ";
        private string sqlexsproduct_end = @"update x set x.dateend = sysdatetimeoffset(), x.opsins = isnull(o.opsins,0),
        x.opsupd = isnull(o.opsupd,0), x.opsrem = isnull(o.opsrem,0), x.opserr = isnull(o.opserr,0), 
        x.tflow = 'ED', opsperform = CAST(CONVERT(varchar(12), DATEADD(SECOND, DATEDIFF(SECOND, x.datestart,  sysdatetimeoffset()), 0), 114) AS time(7) ) 
        from xm_xiexsource x left join (select orgcode,site, depot, fileid, 
        sum(case when tflow = 'ED' and rowops in ('N','1') then 1 else 0 end) opsins, 
        sum(case when tflow = 'ED' and rowops in ('E','2') then 1 else 0 end) opsupd, 
        sum(case when tflow = 'ED' and rowops in ('R','3','9') then 1 else 0 end) opsrem, 
        sum(case when tflow in ('ER','XX') then 1 else 0 end) opserr 
        from xm_xiproduct  where orgcode = @orgcode and site = @site and depot = @depot and isnull(fileid,'') != '' 
        and fileid = @fileid group by orgcode,site, depot, fileid) o 
        on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.fileid = o.fileid 
        where x.orgcode = @orgcode and x.site = @site and x.depot = @depot and x.fileid = @fileid ";
        private string sqlexsproduct_insert_err = "insert into xm_xioutbouln ( orgcode,site,depot,ouorder,tflow,ermsg,fileid ) values ( @orgcode,@site,@depot,@ouorder,@tflow,@ermsg,@fileid)";
        private SqlConnection cn = new SqlConnection();
        public exSource_ops() {}
        public exSource_ops(string _cnx) { this.cn = new SqlConnection(_cnx); }
        public exSource_ops(SqlConnection ocn) { this.cn = ocn; }

        
        private bool disposedValue = false;
        protected virtual void Dispose(bool Disposing){ 
            if(!disposedValue) { }

            this.sqlexsoutbouln_insert = null;

            SqlConnection.ClearPool(cn);
            cn.Dispose();
            disposedValue = true;
        }
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

    }
}
