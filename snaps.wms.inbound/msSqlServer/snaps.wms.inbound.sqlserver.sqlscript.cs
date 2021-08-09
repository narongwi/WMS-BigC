using System;
using System.Data.SqlClient;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {

    public partial class inbound_ops : IDisposable { 
        //Insert inbound order
        private String sqlinsert = "insert into wm_inbound " + 
        " ( orgcode, site, depot, spcarea, thcode, intype, subtype, inorder, dateorder, dateplan,   " + 
        "   dateexpire, slotdate, slotno, inpriority, inflag, inpromo, tflow, daterec, dockrec,     " + 
        "   invno, remarkrec, datecreate, accncreate, datemodify, accnmodify, procmodify ) values   " + 
        " ( @rogcode, @site, @depot, @spcarea, @thcode, @intype, @subtype, @inorder, @dateorder,    " + 
        " @dateplan, @dateexpire, @slotdate, @slotno, @inpriority, @inflag, @inpromo, @tflow,       " + 
        " @daterec, @dockrec, @invno, @remarkrec, @datecreate, @accncreate, @datemodify,            " + 
        " @accnmodify, @procmodify ) ";

        //Update inbound order 
        private String sqlupdate = " update wm_inbound set " + 
        " thcode = @thcode, intype = @intype, subtype = @subtype,  dateorder = @dateorder,          " + 
        " dateplan = @dateplan, dateexpire = @dateexpire, slotdate = @slotdate, slotno = @slotno,   " + 
        " inpriority = @inpriority, inflag = @inflag, inpromo = @inpromo, tflow = @tflow,           " + 
        " daterec = @daterec, dockrec = @dockrec, invno = @invno, remarkrec = @remarkrec,           " + 
        " datemodify = @datemodify,  accnmodify = @accnmodify, procmodify = @procmodify             " +
        " where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder       " ;

        //Delete inbound order 
        private String sqldelete_step1 = "delete from wm_inbound where orgcode = @orgcode           " + 
        " and site = @site and depot = @depot and inorder = @inorder                                " ;
        private String sqldelete_step2 = "delete from wm_inbouln where orgcode = @orgcode           " + 
        " and site = @site and depot = @depot and inotder = @inorder                                " ;

        //Find inbound order
        private String sqlinbound_order_find = "select i.*,t.thnameint thname, " + 
        " isnull(ismeasurement,0) ismeasurement, case when i.tflow not in ('IO') and                " + 
        " i.datemodify > GETDATE()-1 then FORMATMESSAGE(bndescalt,i.dockrec,                        " + 
        " dbo.dsc_dateshort(i.datemodify,SYSDATETIMEOFFSET()),'') else bndesc end dateremarks       " + 
        " from wm_inbound i " + 
        "        join wm_thparty t on i.orgcode = t.orgcode and i.site = t.site and                 " + 
        "                             i.depot = t.depot and i.thcode = t.thcode                     " + 
        "   left join ( select orgcode, site, depot, bnvalue, bndesc, bndescalt from wm_binary      " + 
        "                where bntype = 'INBORDER' and bncode = 'FLOW' and bnstate = 'IO' ) b       " + 
        "                   on i.orgcode = b.orgcode and i.site = b.site and i.depot = b.depot      " + 
        "                  and i.tflow = b.bnvalue                                                  " +
        "   left join (select distinct orgcode,site,depot,inorder, 1 ismeasurement from wm_inbouln  " +
        "               where orgcode = @orgcode and site = @site and depot = @depot                " + 
        " 			    and exists (Select 1 from wm_product p where wm_inbouln.orgcode = p.orgcode " + 
        "               and wm_inbouln.site = p.site and wm_inbouln.depot = p.depot                 " + 
        "               and wm_inbouln.article = p.article and wm_inbouln.pv = p.pv                 " + 
        " 				and wm_inbouln.lv = p.lv and isnull(ismeasurement,0) = 1 ) ) l              " + 
        " 	    on i.orgcode = l.orgcode and i.site = l.site and i.depot = l.depot                  " + 
        "      and i.inorder = l.inorder                                                            " + 
        "  where i.orgcode = @orgcode and i.site = @site and i.depot = @depot                       " ;

        //Find receiption history
        private String sqlhistory = "select m.*,p.descalt, t.thnamealt                              " + 
        "   from wm_inboulx m, wm_product p, wm_thparty t                                           " + 
        " where m.orgcode = p.orgcode and m.site = p.site and m.depot = p.depot                     " + 
        "   and m.article = p.article and m.pv = p.pv and m.lv = p.lv and m.tflow = 'ED'            " + 
        "   and p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot                     " + 
        "   and p.thcode = t.thcode and m.orgcode = @orgcode and m.depot = @depot and m.site = @site";

        //get staging 
        private String sqlgetstaging = "select lscode,lscodealt + ' free : ' +                   " + 
        " convert(varchar(10) ,(lsmxhuno - isnull(crhu,0))) lscodedsc from wm_locdw where tflow = 'IO'        " + 
        " and spcarea = 'RS' and orgcode = @orgcode and site = @site and depot = @depot             ";

        //get product ration
        private String sqlgetratio = " select bndescalt unit, case when bndesc = 'SKU' then 1 when  " + 
        " bndesc = 'IPCK' then rtoskuofipck when bndesc = 'PCK' then rtoskuofpck when               " + 
        " bndesc = 'LAYER' then rtoskuoflayer when bndesc = 'HU' then rtoskuofhu else 0 end rtosku, " + 
        " bnvalue from " + 
        " (select orgcode,site,depot,bndesc, bndescalt,bnvalue from wm_binary                       " + 
        "   where bntype = 'UNIT' and bncode = 'KEEP' and bnstate = 'IO' ) b,                       " +
        " (select orgcode,site,depot,rtoskuofpu, rtoskuofipck, rtoskuofpck, rtoskuoflayer, rtoskuofhu " + 
        " from wm_product where site = @site and depot = @depot and pv = @pv and lv = @lv           " + 
        " and article = @article and pv = @pv and lv = @lv ) p where b.orgcode = p.orgcode          " + 
        " and b.site = p.site and b.depot = p.depot ";

        //get Inbound order header 
        //private String sqlgetheader = "select i.*,t.thnameint thname from wm_inbound i,             " +
        //" wm_thparty t where i.orgcode = t.orgcode and i.site = t.site and i.depot = t.depot        " + 
        //" and i.thcode = t.thcode and i.orgcode = @orgcode and i.site = @site and i.depot = @depot  " + 
        //" and i.inorder = @inorder ";

        private String sqlgetheader = @"select i.*,t.thnameint thname,
            (select count(1) from  wm_inboulx x where i.orgcode = x.orgcode and i.site = x.site and i.depot =x.depot and i.inorder = x.inorder and x.tflow ='SV' ) as waitconfirm,
            (select count(1) from  xm_xoreceipt r where i.orgcode = r.orgcode and i.site = r.site and i.depot = r.depot and i.inorder = r.inorder and r.xaction ='WA') as pendingInf
            from wm_inbound i inner join wm_thparty t on i.orgcode = t.orgcode and i.site = t.site and i.depot = t.depot and i.thcode = t.thcode
            where i.orgcode = @orgcode and i.site = @site and i.depot = @depot   and i.inorder = @inorder";

        //get Inbound order line 
        private String sqlgetline = @"select i.orgcode,i.site,i.depot,i.spcarea,i.inorder,i.inln,i.inrefno,i.inrefln,i.inagrn,i.barcode,i.article,i.pv,i.lv,i.unitops,i.qtysku
        ,i.qtypu
        ,i.qtyweight,i.lotno,i.expdate,i.serialno
        ,i.qtypnd
        ,i.qtyskurec,i.qtypurec,i.qtyweightrec,i.qtynaturalloss,i.tflow,i.datecreate,i.accncreate,i.datemodify,i.accnmodify,i.procmodify,i.batchno
        ,i.qtyhurec,i.orbitsource,i.inseq ,p.descalt,b.bndescalt unitopsdesc,p.isdlc,          " +
        " p.isunique,p.ismixingprep,p.isbatchno, Ceiling((p.dlcall*dlcfactory)/100) dlcfactory,       " +
        " Ceiling((dlcall* (dlcshop + dlconsumer))/100) as dlcwarehouse,p.unitreceipt,          " + 
        " p.innaturalloss,p.skulength,p.skuwidth,p.skuheight,p.skuweight,                           " + 
        " convert(varchar(10), p.rtopckoflayer) + ' x '+ convert(varchar(10), p.rtolayerofhu) tihi,     " + 
        " p.laslotno,p.lasdatemfg,p.lasdateexp,p.lasserialno,p.rtoskuofipck,p.rtoskuofpck,          " +
        " p.rtoskuoflayer,p.rtoskuofhu,floor(isnull(qtypnd,1) / isnull(case when rtoskuofhu = 0 then 1 else rtoskuofhu end ,1)) huestimate,p.dlcall,lasbatchno    " +
        " from wm_inbouln i, wm_product p, wm_binary b where i.orgcode = p.orgcode                  " + 
        " and i.site = p.site and i.depot = p.depot and i.article = p.article and i.pv = p.pv       " + 
        " and i.lv = p.lv and p.orgcode = b.orgcode and i.site = b.site and i.depot = b.depot       " + 
        " and b.apps = 'WMS' and bntype = 'UNIT' and bncode = 'STOCKDESC' and bnstate = 'IO'        " + 
        " and p.unitdesc = b.bnvalue and i.orgcode = @orgcode and i.site = @site                    " + 
        " and i.depot = @depot and i.inorder = @inorder ";
        
        //validate inbound order is exists 
        private String sqlvalidate = "select count(1) from wm_inbound where orgcode = @orgcode and  " +
        " and site = @site and depot = @depot and inorder = @inorder                                ";
        
        //set priority
        private String sqlsetpriority = "update wm_inbound set inpriority = @inpriority,            " + 
        " accnmodify = @accncode, datemodify = @sysdate where orgcode = @orgcode and site = @site   " + 
        " and depot = @depot and inorder = @inorder";
       
        //Set staging
        private String sqlsetstaging = "update wm_inbound set tflow = 'SA', dockrec = @staging,     " + 
        " accnmodify = @accncode, datemodify = @sysdate, dateassign = @sysdate                      " + 
        " where tflow in ('SA','IO') and orgcode = @orgcode and site = @site and depot = @depot     " + 
        " and inorder = @inorder ";
        
        //set Remarks
        private String sqlsetremark = "update wm_inbound set remarkrec = @remarks,                  " + 
        " datemodify = CURRENT_TIMESTAMP, accnmodify = @accncode where orgcode = @orgcode           " + 
        " and site = @site and depot = @depot and inorder = @inorder ";
        
        //Set invoice no 
        private String sqlsetinvoice = "update wm_inbound set invno = @invno,                       " + 
        " datemodify = CURRENT_TIMESTAMP, accnmodify = @accncode where orgcode = @orgcode           " + 
        " and site = @site and depot = @depot and inorder = @inorder and tflow not in ('CL') ";
        
        //Set replan delivery date
        private String sqlsetreplan = "update wm_inbound set datereplan = @datereplan,              " + 
        " datemodify = CURRENT_TIMESTAMP, accnmodify = @accncode where  orgcode = @orgcode          " + 
        " and site = @site and depot = @depot and inorder = @inorder and tflow in ('IO')"; 
        
        //Start start unload good on staging
        private String sqlunloadstart = "update wm_inbound set tflow = 'SS',                        " + 
        " dateunloadstart = @sysdate, accnmodify = @accncode, datemodify = @sysdate where           " + 
        " orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder             " + 
        " and tflow in ('SA') ";
        
        //Set Finish unload goods on staging 
        private String sqlunloadend = "update wm_inbound set tflow = 'SE', dateunloadend = @sysdate, " + 
        " accnmodify = @accncode, datemodify = @sysdate where orgcode = @orgcode and site = @site   " + 
        " and depot = @depot and inorder = @inorder and tflow in ('SS') ";
        
        //Set Completed receipt order
        //private String sqlfinish = "update wm_inbound set tflow = 'ED',daterec = sysdatetimeoffset(), datefinish =  sysdatetimeoffset(),     " + 
        //" accnmodify = @accncode, datemodify = sysdatetimeoffset() where orgcode = @orgcode and site = @site   " + 
        //" and depot = @depot and inorder = @inorder and tflow in ('SE') ";

        //Set Completed receipt order
        private String sqlfinish = "update wm_inbound set tflow = 'ED',daterec = sysdatetimeoffset(), datefinish =  sysdatetimeoffset(),     " +
        " accnmodify = @accncode, datemodify = sysdatetimeoffset() where orgcode = @orgcode and site = @site   " +
        " and depot = @depot and inorder = @inorder and tflow in ('SE');" +
        " update wm_inbouln set tflow = 'ED', accnmodify = @accncode, datemodify = sysdatetimeoffset(), procmodify = 'inb.closepo' " +
        " where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder and tflow in ('IO');";

        //Cancel Inbound order
        //private String sqlcancel = "update wm_inbound set tflow = 'CL', remarkrec = @remarks,       " + 
        //" datemodify = CURRENT_TIMESTAMP, accnmodify = @accncode where orgcode = @orgcode           " + 
        //" and site = @site and depot = @depot and inorder = @inorder and tflow in ('IO','SA')       ";
        //Cancel Inbound order
        private String sqlcancel = "update wm_inbound set tflow = 'CL', remarkrec = @remarks,       " +
        " datemodify = CURRENT_TIMESTAMP, accnmodify = @accncode where orgcode = @orgcode           " +
        " and site = @site and depot = @depot and inorder = @inorder and tflow in ('IO','SA');" +
        " update wm_inbouln set tflow = 'CL', accnmodify = @accncode, datemodify = sysdatetimeoffset(), procmodify = 'inb.cancelpo' " +
        " where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder and tflow in ('IO'); ";


        // Get Receipt transaction 
        private String sqlgetlx =  @"select x.inlx,x.orgcode,x.site,x.depot,x.spcarea,x.inorder,x.inln,x.inrefno,x.inrefln,x.barcode, 
        x.article,x.pv,x.lv ,x.unitops,x.qtyskurec,x.qtypurec,x.qtyweightrec,x.qtynaturalloss, 
        x.daterec,x.datemfg,x.dateexp,x.batchno,x.lotno ,x.serialno,x.datecreate,x.accncreate, 
        x.datemodify,x.accnmodify,x.procmodify,x.tflow,x.qtyhurec , ingrno,0 rtoskuofpu, 
        o.dockrec, 0 rtoskuofhu,o.thcode,o.intype,o.subtype insubtype,barcode, o.inpromo, 
        o.orbitsource, 0 skuweight, 0 skuvolume, 0 skucubic,x.inagrn 
        from wm_inbound o, wm_inboulx x 
        where o.orgcode = x.orgcode and o.site = x.site and o.depot = x.depot and o.inorder = x.inorder 
        and o.orgcode = @orgcode and o.site = @site and o.depot = @depot and o.inorder = @inorder ";
        
        // Insert Receipt transaction 
        private String sqlinsertlx = "insert into wm_inboulx ( inlx,orgcode,site,depot,spcarea,inorder, " + 
        " inln,inrefno,inrefln,barcode,article,pv,lv,unitops,qtyskurec,qtypurec,qtyweightrec,qtynaturalloss, " +
        " daterec,datemfg,dateexp,batchno,lotno,serialno,datecreate,accncreate,datemodify,accnmodify,procmodify, " + 
        " tflow,qtyhurec,ingrno,inagrn,inseq ) values (next value for seq_intx, @orgcode,@site,@depot,@spcarea,@inorder, " +
        " @inln,@inrefno,@inrefln,@barcode,@article,@pv,@lv,@unitops,@qtyskurec,@qtypurec," + 
        " @qtyskurec * (select skuweight from wm_product o where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv) , " +
        " @qtynaturalloss,sysdatetimeoffset(),@datemfg,@dateexp,@batchno,@lotno,@serialno,sysdatetimeoffset(),@accncreate, " +
        " sysdatetimeoffset(),@accnmodify,@procmodify,'SV',@qtyhurec,@ingrno,@inagrn,@inseq) ";

        //validate receipt tranaction 
        private string sqlinboulx_val = ""+ 
        " select case when qtysku < isnull((select sum(qtyskurec) from wm_inboulx where orgcode = @orgcode and site = @site and depot = @depot " + 
        " and inorder = @inorder and inln = @inln and article = @article and pv = @pv and lv = @lv),0)  + @qtyskurec then 1 else 0 end rsl "+ 
        " from wm_inbouln where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder and inln = @inln " + 
        " and article = @article and pv = @pv and lv = @lv and tflow = 'IO' ";
        private string sqlinboulc_valserail = @"select isnull( ( select count(1) from wm_inboulx where serialno = @serialno and orgcode = @orgcode and site = @site and depot = @depot ), 0 ) rsl";

        private string sqlinbounlx_valmea = "select ismeasurement from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv";



        // Remove Receipt transaction 
        private String sqlremovelx = " delete from wm_inboulx where orgcode = @orgcode             " + 
        " and site = @site and depot = @depot and inorder = @inorder and tflow = 'SV'               " + 
        " and inlx = @inlx ";

        //Confirm Receipt transction 
        private String sqlconfirmlx = " update wm_inboulx set tflow = 'ED',ingrno= @ingrno,         " + 
        " datemodify = @sysdate, accnmodify = @accnmodify where orgcode = @orgcode                  " + 
        " and site = @site and depot = @depot and inorder = @inorder and tflow = 'SV'           " + 
        " and inlx = @inlx";

        //Get Receipt transaction to confirm for orbit 
        // private String sqlorbitconfirm = " SELECT	"+ 
        // " x.inlx,x.orgcode,x.site,x.depot,x.spcarea,x.inorder,x.inln,x.inrefno,x.inrefln,x.barcode, " +
        // " x.article,x.pv,x.lv ,x.unitops,x.qtyskurec,x.qtypurec,x.qtyweightrec,x.qtynaturalloss,    " +
        // " x.daterec,x.datemfg,x.dateexp,x.batchno,x.lotno ,x.serialno,x.datecreate,x.accncreate,    " + 
        // " x.datemodify,x.accnmodify,x.procmodify,x.tflow,x.qtyhurec , ingrno,                       " +
        // " case when x.unitops = 1 then 1 when x.unitops = 2 then rtoskuofipck                     " +
        // "   when x.unitops = 3 then rtoskuofpck when x.unitops = 4 then rtoskuoflayer            " +
        // "   when x.unitops = 5 then rtoskuofhu else 1 end rtoskuofpu, i.dockrec,rtoskuofhu,i.thcode," + 
        // " i.intype, i.subtype insubtype, x.barcode, inpromo, l.orbitsource, l.inagrn,               " +
        // " p.skuweight, p.skuvolume, p.skuvolume /100 skucubic, x.inagrn                             " + 
        // " from wm_inboulx x, wm_inbouln l, wm_inbound i, wm_product p                               " +
        // "   where i.orgcode = l.orgcode and i.site = l.site and i.depot = l.depot                   " + 
        // "     and i.inorder = l.inorder and l.orgcode = x.orgcode and l.site = x.site               " + 
        // "     and l.depot = x.depot and l.inorder = x.inorder and l.inln = x.inln                   " +
        // "	  and l.article = x.article and l.pv = x.pv and l.lv = x.lv                             " +
        // " 	  and not exists ( select 1 from xm_xoreceipt o where x.orgcode = o.orgcode             " + 
        // "     and x.site = o.site and x.depot = o.depot and x.inorder = o.inorder                   " + 
        // "     and x.inln = o.inln and x.article = o.article and x.pv = o.pv and x.lv = o.lv         " + 
        // "     and x.ingrno = o.ingrno ) and x.orgcode = p.orgcode and x.site = p.site               " +
        // "     and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv       " +
        // " 	  and x.ingrno is null and x.tflow = 'SV' and l.orgcode = @orgcode and l.site = @site   " +
        // "     and l.depot = @depot and l.inorder = @inorder and l.inln = @inln                      " + 
        // "     and l.article = @article and l.pv = @pv and l.lv = @lv ";
        
        //Change from receiption stock unit to preparation unit 
        private string sqlorbitconfirm = @" SELECT x.inlx,x.orgcode,x.site,x.depot,x.spcarea,x.inorder,x.inln,x.inrefno,x.inrefln,x.barcode, 
        x.article,x.pv,x.lv ,x.unitops,x.qtyskurec,x.qtypurec,x.qtyweightrec,x.qtynaturalloss, 
        x.daterec,x.datemfg,x.dateexp,x.batchno,x.lotno ,x.serialno,x.datecreate,x.accncreate, 
        x.datemodify,x.accnmodify,x.procmodify,x.tflow,x.qtyhurec , ingrno, 
        case when p.unitprep = 1 then 1            when p.unitprep = 2 then rtoskuofipck 
            when p.unitprep = 3 then rtoskuofpck  when p.unitprep = 4 then rtoskuoflayer 
            when p.unitprep = 5 then rtoskuofhu   else 1 end rtoskuofpu, 
        i.dockrec,rtoskuofhu,i.thcode,
        i.intype, i.subtype insubtype, x.barcode, inpromo, l.orbitsource, l.inagrn, 
        p.skuweight, p.skuvolume, p.skuvolume /100 skucubic, x.inagrn 
        from wm_inboulx x, wm_inbouln l, wm_inbound i, wm_product p 
        where i.orgcode = l.orgcode and i.site = l.site and i.depot = l.depot 
            and i.inorder = l.inorder and l.orgcode = x.orgcode and l.site = x.site 
            and l.depot = x.depot and l.inorder = x.inorder and l.inln = x.inln 
            and l.article = x.article and l.pv = x.pv and l.lv = x.lv 
            and not exists ( select 1 from xm_xoreceipt o where x.orgcode = o.orgcode 
            and x.site = o.site and x.depot = o.depot and x.inorder = o.inorder 
            and x.inln = o.inln and x.article = o.article and x.pv = o.pv and x.lv = o.lv 
            and x.ingrno = o.ingrno ) and x.orgcode = p.orgcode and x.site = p.site 
            and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv 
            and x.ingrno is null and x.tflow = 'SV'      
            and l.orgcode = @orgcode and l.site = @site  
            and l.depot = @depot and l.inorder = @inorder and l.inln = @inln 
            and l.article = @article and l.pv = @pv and l.lv = @lv";
    }
}