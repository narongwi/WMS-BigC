using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace snaps.wms.api.document.Models
{
    public static class SqlReportModel
    {
        public static string ReceiptionConfirmSql =
            " select p.thcode supp_code, thname supp_name, x.site dc, x.depot depot, x.inorder supp_po,CONVERT(varchar, w.dateplan, 23) plan_date,  " +
            " 		p.article, p.description,x.qtysku poqty,p.rtoskuofipck rtosku, p.rtoipckofpck rtoipck, p.rtopckoflayer rtopck, p.rtolayerofhu rtolayer, p.skuweight,   " +
            " 		p.skulength dmlength,p.skuwidth dmwidth,p.skuheight dmheight, p.spcrecvlocation spclocation, p.dlcwarehouse,p.dlcwarehouse whshelflife, " +
            " (select max(lscodealt) adrdef from wm_locdw l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.article = l.spcarticle) adrdef, " +
            " (select max(barcode) barcode from wm_barcode l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.article = l.article) barcode,  " +
            " invno refno, qtyskurec rcqty,qtyweightrec rcweight,0 rccubic,dateunloadstart receipt_date " +
            " from wm_inbound w, wm_inbouln x,wm_thparty t ,wm_product p  " +
            " where w.orgcode = x.orgcode and w.site = x.site and w.depot = x.depot and w.inorder = x.inorder " +
            " and x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv  " +
            " and p.orgcode = t.orgcode and p.site = t.site and p.depot = t.depot and p.thcode = t.thcode and w.inorder = {0}";

        public static string ReceiptStateSql =
            " select  w.thcode supp_code, t.thnamealt supp_name, x.site dc, x.depot depot, x.inorder supp_po,CONVERT(varchar, w.dateplan, 23) plan_date,  " +
            " 		p.article + '-' + cast(p.lv as varchar(2)) article, p.description,x.qtypu poqty,p.rtoskuofipck rtosku, p.rtoipckofpck rtoipck, p.rtopckoflayer rtopck, " +
            " 		p.rtolayerofhu rtolayer, cast(p.skuweight as decimal(10,2)) weight , cast(p.skulength as decimal(10,2)) dmlength, " +
            "		cast(p.skuwidth as decimal(10,2)) dmwidth,cast(p.skuheight as decimal(10,2)) dmheight, p.spcrecvlocation spclocation, " +
            "		 p.dlcwarehouse,p.dlcwarehouse whshelflife, w.dockrec dock_no,b.bndesc manageType, " +
            " (select max(lscodealt) adrdef from wm_locdw l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.article = l.spcarticle) adrdef, " +
            " (select max(barcode) barcode from wm_barcode l where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.article = l.article) barcode " +
            " from wm_inbound w join wm_inbouln x on w.orgcode = x.orgcode and w.site = x.site and w.depot = x.depot and w.inorder = x.inorder " +
            " join wm_thparty t on w.orgcode = t.orgcode and w.site = t.site and w.depot = t.depot and w.thcode = t.thcode " +
            " join wm_product p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv  " +
            " left join wm_loczp z on x.orgcode = z.orgcode and x.site = z.site and x.depot = z.depot and x.article = z.spcproduct and x.pv = z.spcpv and x.lv = z.spclv " +
            " left join (select orgcode,site,depot, bnvalue,bndesc from wm_binary where bncode = 'SUBTYPE' and bntype = 'ORDER'  and apps = 'WMS' ) b " +
            "   on w.orgcode = b.orgcode and w.site = b.site and w.depot = b.depot and w.subtype = b.bnvalue " +
            " where w.orgcode = '{0}' and w.site = '{1}' and w.depot = '{2}' and w.inorder = '{3}' ";
        
        public static string PutawaySql =
            " select t.article + '-' + cast(t.lv as varchar(5)) article, t.lv, t.pv, targetadv location, p.description," +
            " convert(varchar,  t.datecreate, 103)  daterecipt, t.iorefno inorder, s.qtysku quantitysku, s.qtypu quantitypu, " +
            "        s.qtyweight weight, convert(varchar, s.dateexp, 103) dateexp,convert(varchar, s.datemfg, 103)  datemfg, cast(rtopckoflayer as varchar(10)) + ' x ' + cast(rtolayerofhu as varchar(20)) tihi, " +
            " 	   rtoskuofhu skuofpallet, rtoskuofipck skuofipck, rtoskuofpck skuofpck,rtoipckofpck ipckofpck, rtopckoflayer * rtolayerofhu pckofpallet, s.huno, b.barcode,k.tasktype " +
            "   from wm_task k,wm_taln t, wm_product p, wm_stock s, wm_barcode b " +
            "  where k.orgcode = t.orgcode and k.site = t.site and k.depot = t.depot and k.taskno  = t.taskno" +
            "    and t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.article = p.article and t.pv = p.pv and t.lv = p.lv " +
            "    and t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.stockid = s.stockid " +
            "    and t.orgcode = b.orgcode and t.site = b.site and t.depot = b.depot and t.article = b.article and t.pv = b.pv and t.lv = b.lv " +
            "    and t.orgcode = '{0}' and t.site = '{1}' and t.depot = '{2}' and t.taskno = '{3}'  and b.isprimary = '1' ";

        public static string InboundSql =
            @"select s.article, s.lv, s.pv, s.loccode location, p.description,convert(varchar, s.datecreate, 103)  daterecipt, inrefno inorder, qtysku quantitysku, qtypu quantitypu,
             s.qtyweight weight, convert(varchar, s.dateexp, 103) dateexp,convert(varchar, s.datemfg, 103)  datemfg, cast(rtopckoflayer as varchar(10)) + ' x ' + cast(rtolayerofhu as varchar(20)) tihi,
 	         rtoskuofhu skuofpallet, rtoskuofipck skuofipck, rtoskuofpck skuofpck,0 ipckofpck, 0 pckofpallet, s.huno, b.barcode
             from wm_product p, wm_stock s, wm_barcode b
             where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
             and s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.article = b.article and s.pv = b.pv and s.lv = b.lv
             and s.orgcode = '{0}' and s.site = '{1}' and s.depot = '{2}' and s.inrefno = '{3}'  and b.isprimary = '1' and b.tflow = 'IO' ";

        public static string MergeHuLabel =
             @"select 
	            concat(l.article,'-',l.lv) as article, l.lv as lv,l.pv as pv,h.loccode location,
	            SUBSTRING(p.description,0,40) description,convert(varchar,min(l.daterec), 103) daterecipt,min(l.inrefno) as inorder,
	            sum(l.skuops) as quantitysku, sum(l.puops) as quantitypu, sum(l.weightops) as weight, 
	            convert(varchar,min(l.dateexp), 103) as dateexp, convert(varchar,min(l.datemfg), 103) as datemfg, 
	            concat(p.rtopckoflayer,' x ', p.rtolayerofhu) tihi, p.rtoskuofhu as skuofpallet, 
	            p.rtoskuofipck as skuofipck, p.rtoskuofpck as skuofpck,p.rtoipckofpck as ipckofpck,
	            p.rtopckoflayer * p.rtolayerofhu as pckofpallet, h.hutarget huno,
	            dbo.get_barcode(l.orgcode,l.site,l.depot,l.article,l.pv,l.lv) barcode,NULL as tasktype 
             from wm_mergehu h left join wm_mergeln l on h.mergeno = l.mergeno left join wm_product p on l.orgcode = p.orgcode 
 	            and l.site = p.site and l.depot = p.depot and l.article = p.article  and l.pv = p.pv and l.lv = p.lv 	
             where h.orgcode = @orgcode and h.site = @site and h.depot = @depot and h.hutarget =  @huno 
 	            and 1 =  (select count(DISTINCT t.article) from wm_mergehu x join wm_mergeln t on x.mergeno = t.mergeno 
 	            where x.orgcode = h.orgcode and x.site = h.site and x.depot = h.depot and x.hutarget = h.hutarget)
             group by l.orgcode,l.site,l.depot,l.article,l.pv,l.lv,SUBSTRING(p.description,0,40),h.loccode,h.hutarget,p.rtopckoflayer, 
 	            p.rtolayerofhu,p.rtoskuofhu,p.rtoskuofipck,p.rtoskuofpck,p.rtoipckofpck
             UNION ALL 
             select NULL as article, NULL as lv,NULL as pv, h.loccode location,NULL description,NULL daterecipt,
		            NULL as inorder,SUM(l.skuops) as quantitysku, SUM(l.puops) as quantitypu, SUM(l.weightops) as weight, 
		            NULL as dateexp,NULL as datemfg,NULL tihi,NULL as skuofpallet,NULL as skuofipck,NULL as skuofpck,
		            NULL as ipckofpck,NULL as pckofpallet,h.hutarget huno,'Multiple Product' barcode, NULL tasktype 
             from wm_mergehu h left join wm_mergeln l on h.mergeno = l.mergeno	
             where h.orgcode = @orgcode and h.site = @site and h.depot = @depot and h.hutarget = @huno
 	            and 1 < (select count(DISTINCT t.article) from wm_mergehu x join wm_mergeln t on x.mergeno = t.mergeno  
             where x.orgcode = h.orgcode and x.site = h.site and x.depot = h.depot and x.hutarget = h.hutarget)
             group by  h.loccode,h.hutarget";

        public static string StockHuLabel =
            @"select 
	            concat(s.article,'-',s.lv) as article, s.lv, s.pv,s.loccode as location, 
	            SUBSTRING(p.description,0,40) AS description,convert(varchar,s.daterec, 103) as daterecipt,s.inrefno as inorder,
	            s.qtysku as quantitysku, s.qtypu as quantitypu,s.qtyweight as weight,
	            convert(varchar, s.dateexp, 103) as dateexp,
	            convert(varchar, s.datemfg, 103) as datemfg, 
	            concat(p.rtopckoflayer,' x ',p.rtolayerofhu) as tihi, 
	            p.rtoskuofhu as skuofpallet, p.rtoskuofipck as skuofipck, p.rtoskuofpck as skuofpck,p.rtoipckofpck as ipckofpck, 
	            p.rtopckoflayer * p.rtolayerofhu as pckofpallet,s.huno, dbo.get_barcode(s.orgcode,s.site,s.depot,s.article,s.pv,s.lv) barcode,NULL as tasktype 
            from wm_stock s join wm_product p on s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv 
            where s.orgcode = @orgcode and s.site = @site and s.depot = @depot and s.huno = @huno
	            and 1 = (select count(x.article) from wm_stock x where x.orgcode = s.orgcode and x.site = s.site and x.depot = s.depot and x.huno = s.huno)
            UNION ALL 
            select  NULL as article, NULL as lv,NULL as pv, s.loccode location,NULL description,NULL daterecipt,NULL as inorder,
		            SUM(s.qtysku) as quantitysku, SUM(s.qtypu) as quantitypu, SUM(s.qtyweight) as weight, NULL as dateexp,
		            NULL as datemfg, NULL tihi,NULL as skuofpallet,NULL as skuofipck, NULL as skuofpck,NULL as ipckofpck,NULL as pckofpallet,
		            s.huno,'Multiple Product' barcode, NULL tasktype 
             from wm_stock s where s.orgcode = @orgcode and s.site =@site and s.depot = @depot and s.huno = @huno
             and 1 < (select count(distinct x.article) from wm_stock x where x.orgcode = s.orgcode and x.site = s.site and x.depot = s.depot and x.huno = s.huno)
             group by s.loccode,s.huno ";

        //public static string HUSql =
        //    @"select s.article, s.lv, s.pv, s.loccode location, p.description, convert(varchar, s.datecreate, 103)  daterecipt, 
        //      inrefno inorder, qtysku quantitysku, qtypu as quantitypu, s.qtyweight weight, 
        //      convert(varchar, s.dateexp, 103) dateexp, convert(varchar, s.datemfg, 103)  datemfg, 
        //      cast(rtopckoflayer as varchar(10)) + ' x ' + cast(rtolayerofhu as varchar(20)) tihi,
        //       rtoskuofhu skuofpallet, rtoskuofipck skuofipck, rtoskuofpck skuofpck, rtoipckofpck ipckofpck, 
        //      rtopckoflayer*rtolayerofhu pckofpallet, s.huno, b.barcode
        //    from wm_product p, wm_stock s, wm_barcode b
        //    where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
        //    and s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.article = b.article and s.pv = b.pv and s.lv = b.lv
        //    and s.orgcode = '{0}' and s.site = '{1}' and s.depot = '{2}' and s.huno = '{3}'  and b.isprimary = '1' and b.tflow = 'IO' ";

        public static string HUSql =
            @"select s.article, s.lv, s.pv, s.loccode location, p.description, convert(varchar, s.datecreate, 103)  daterecipt, 
		            inrefno inorder, qtysku quantitysku, qtypu as quantitypu, s.qtyweight weight, 
		            convert(varchar, s.dateexp, 103) dateexp, convert(varchar, s.datemfg, 103)  datemfg, 
		            cast(rtopckoflayer as varchar(10)) + ' x ' + cast(rtolayerofhu as varchar(20)) tihi,
 		            rtoskuofhu skuofpallet, rtoskuofipck skuofipck, rtoskuofpck skuofpck, rtoipckofpck ipckofpck, 
		            rtopckoflayer*rtolayerofhu pckofpallet, s.huno, b.barcode
            from wm_product p, wm_stock s, wm_barcode b
            where s.orgcode = p.orgcode and s.site = p.site and s.depot = p.depot and s.article = p.article and s.pv = p.pv and s.lv = p.lv
            and s.orgcode = b.orgcode and s.site = b.site and s.depot = b.depot and s.article = b.article and s.pv = b.pv and s.lv = b.lv
            and s.orgcode = '{0}' and s.site = '{1}' and s.depot = '{2}' and s.huno = '{3}' and b.isprimary = '1' and b.tflow = 'IO'
            and 1 = (select count(distinct x.article) from wm_stock x where x.orgcode = s.orgcode and x.site = s.site and x.depot = s.depot and x.huno = s.huno)
            UNION ALL 
            select  null article, null lv, null pv, s.loccode location, null description, null daterecipt, 
		            null inorder, sum(qtysku) quantitysku, sum(qtypu) as quantitypu, sum(s.qtyweight) weight, 
		            null dateexp, null datemfg, 
		            null tihi,
 		            null skuofpallet, null skuofipck, null skuofpck, null ipckofpck, 
		            null pckofpallet, s.huno, 'Multi Product' barcode
            from wm_stock s where s.orgcode = '{0}' and s.site = '{1}' and s.depot = '{2}' and s.huno = '{3}'
            and 1 < (select count(distinct x.article) from wm_stock x where x.orgcode = s.orgcode and x.site = s.site and x.depot = s.depot and x.huno = s.huno)
            group by s.loccode,s.huno ";

        public static string HUEmptySql =
          @"select (select top 1 zp.przone from wm_loczp zp where zp.orgcode = '{0}' and zp.site = '{1}' and zp.depot = '{2}' and zp.lscode = loccode) worksite,
                loccode, h.thcode, huno, SUBSTRING(huno,len(huno)-3,4) hunolast, SUBSTRING(huno,0,len(huno)-3) hunowolast,t.thnameint thname,
                    convert(varchar,  sysdatetimeoffset(), 103) + ' ' +  replace(convert(varchar,  sysdatetimeoffset(), 8),'+07:00','')  printdate                    
             from wm_handerlingunit h, wm_thparty t where hutype = 'XE' and h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.thcode = t.thcode
            and h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.huno = '{3}'";

        public static string CountSheetSql =
            " select countcode taskid, plancode, c.orgcode, c.site , c.depot ,countcode,loccode,stbarcode ohbar, p.descalt, stlotmfg, stdatemfg, stdateexp, cnqtypu, cnflow, p.descalt ohdesc,locseq " +
            " from wm_couln c left join wm_product p on c.orgcode = p.orgcode and c.site = p.site and c.depot = p.depot and c.starticle = p.article " +
            " where c.orgcode = '{0}' and c.site = '{1}' and c.depot = '{2}' and c.countcode = '{3}' and c.plancode =  '{4}'";

             public static string CountSheetSql2 = @"select c.orgcode, c.site, c.depot, c.spcarea, c.countcode, c.plancode, c.planname, c.accnassign,c.szone,c.saisle,c.sbay,c.slevel,l.loccode,l.locseq,l.starticle,l.stbarcode,
            l.stpv, l.stlv, p.description,l.stqtysku, l.stqtypu, l.stlotmfg, l.stdatemfg, l.stdateexp, l.stserialno, l.sthuno, l.cnbarcode, l.cnarticle, l.cnpv, l.cnlv, l.cnqtysku, l.cnqtypu, l.cnlotmfg, l.cndatemfg, l.cndateexp, l.cnserialno,
            l.cnhuno, l.cnflow, l.cnmsg, l.isskip, l.isrgen, l.iswrgln, l.countdevice, l.countdate, l.corcode,l.corqty, l.coraccn, l.cordevice, l.cordate, l.tflow, l.datecreate, l.accncreate, l.datemodify, l.accnmodify, l.procmodify
            from wm_coupn c join wm_couln l on c.orgcode = l.orgcode and c.site = l.site and c.depot = l.depot and c.countcode = l.countcode and c.plancode = l.plancode	
            left join wm_product p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot  and l.starticle = p.article  and l.stpv= p.pv and l.stlv = p.lv 
            where c.orgcode = '{0}' and c.site ='{1}' and c.depot = '{2}' and c.countcode = '{3}' and c.plancode = '{4}' order by l.locseq asc";

        public static string PickListSql =
            " select p.orgcode,p.site,p.depot,p.prepno, p.przone przone, FORMAT (p.prepdate, 'dd/MM/yyyy hh:mm:ss') prepdate, o.ourefno,                                          " +
            "        p.routeno, l.ouorder, l.loccode, l.huno,  l.article, l.pv,l.lv, b.barcode,                                             " +
            "		r.descalt description, l.unitprep pickunit, l.qtyskuorder orderqtysku, l.qtypuorder orderqtypu,l.batchno , convert(varchar, l.dateexp, 103) dateexp     " +
            "  from wm_prep p, wm_prln l, wm_outbouln o, wm_product r left join wm_barcode b on r.orgcode = b.orgcode and r.site = b.site and r.depot = b.depot and r.article = b.article and r.pv = b.pv and r.lv = b.lv and b.isprimary = 1" +
            "  where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno and p.spcarea = 'ST'             " +
            "  and l.orgcode = r.orgcode and l.site = r.site and l.depot = r.depot and l.article = r.article and l.pv = r.pv and l.lv = r.lv" +
            "  and l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder and l.ouln = o.ouln             " +
            "  and l.article = o.article and l.pv = o.pv and l.lv = o.lv and p.orgcode = '{0}' and p.site = '{1}' and p.depot = '{2}' and p.prepno = '{3}' ";

        public static string DistrListSql =
           @"select p.prepno lotno, FORMAT (p.prepdate, 'dd/MM/yyyy hh:mm:ss') prepdate, l.loccode, l.ouorder,  l.unitprep pickunit, l.qtypuorder ordpu, l.qtypuorder picpu, 
                l.article product, r.descalt productname, l.thcode article, t.thnameint description, p.spcorder inborder,l.ouorder ordno,
                p.huno,dbo.get_barcode_active(l.orgcode, l.site, l.depot, l.article, l.pv, l.lv) barcode
            from wm_prep p, wm_prln l, wm_locdw d , wm_product r, wm_thparty t  
            where p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and p.prepno = l.prepno and p.spcarea = 'XD'
            and l.orgcode = d.orgcode and l.site = d.site and l.depot = d.depot and l.loccode = d.lscode 
            and l.orgcode = r.orgcode and l.site = r.site and l.depot = r.depot and l.article = r.article and l.pv = r.pv and l.lv = r.lv 
            and l.orgcode = t.orgcode and l.site = t.site and l.depot = t.depot and l.thcode = t.thcode 
            and p.orgcode = '{0}' and p.site = '{1}' and p.depot = '{2}' and p.prepno = '{3}'";
        //public static string LabelShippedSql =
        //     @"select h.orgcode, h.site, h.depot, h.thcode, t.thnameint thname, sum(qtyskuops) quantity, 
        //        format(r.plandate,'dd/MM/yyyy')  routedate, h.huno,h.routeno,
        //     case when count(h.prepno) > 1 then 'Multi-Task' else max(h.prepno) end taskno,
        //        case when count(l.ouorder) > 1 then 'Multi-Order' else max(l.ouorder) end ouorder,
        //        case when count(l.ouorder) > 1 then 'Multi-Product' else max(l.article) end article,
        //        case when count(l.ouorder) > 1 then 'Multi-Barcode' else max(l.barcode) end barcode,
        //        case when count(l.ouorder) > 1 then 'Multi-Product' else max(p.descalt) end description,
        //        case when count(l.ouorder) > 1 then 'Multi-line' else max(o.ourefno) end ourefno,
        //        case when count(l.ouorder) > 1 then 'Multi-Date' else format(max(b.dateprep),'dd/MM/yyyy') end dateorder,
        //        format(datereqdelivery,'dd/MM/yyyy hh:mm') datedelivery
        //    from wm_handerlingunit u, wm_prep h, wm_prln l, wm_thparty t, wm_route r, wm_product p,wm_outbouln o, wm_outbound b

        //    left join wm_barcode x on p.orgcode = x.orgcode and p.site = x.site and p.depot = x.depot and p.article = x.article and p.pv = x.pv and p.lv = x.lv and x.isprimary = 1
        //    where  h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.prepno = l.prepno 
        //    and  h.orgcode = u.orgcode and h.site = u.site and h.depot = u.depot and h.huno = u.huno 
        //    and u.orgcode = r.orgcode and u.site = r.site and u.depot = r.depot and u.routeno = r.routeno 
        //    and l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
        //    and l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder and l.ouln = o.ouln 
        //    and l.article = o.article and l.pv = o.pv and l.lv = o.lv
        //    and b.orgcode = o.orgcode and b.site = o.site and b.depot = o.depot and b.ouorder = o.ouorder
        //    and h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.thcode = t.thcode
        //    and h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.huno = '{3}' 
        //    group by h.orgcode, h.site, h.depot, h.thcode, t.thnameint,h.huno,h.routeno,r.plandate,datereqdelivery ";
        public static string LabelShippedSql =
             @"select h.orgcode, h.site, h.depot, h.thcode, t.thnameint thname, sum(qtyskuops) quantity, 
                format(r.plandate,'dd/MM/yyyy')  routedate, h.huno,h.routeno,
                case when count(h.prepno) > 1  then 'Multi-Task' else max(h.prepno) end taskno,
                case when count(l.ouorder) > 1 then 'Multi-Order' else max(l.ouorder) end ouorder,
                case when count(l.ouorder) > 1 then 'Multi-Product' else max(l.article) end article,
                case when count(l.ouorder) > 1 then 'Multi-Barcode' else max(x.barcode) end barcode,
                case when count(l.ouorder) > 1 then 'Multi-Product' else max(p.descalt) end description,
                case when count(l.ouorder) > 1 then 'Multi-line' else max(o.ourefno) end ourefno,
                case when count(l.ouorder) > 1 then 'Multi-Date' else format(max(b.dateprep),'dd/MM/yyyy') end dateorder,
                format(datereqdelivery,'dd/MM/yyyy hh:mm') datedelivery
            from wm_handerlingunit u
	            join wm_prep h on u.orgcode = h.orgcode and u.site = h.site  and u.depot = h.depot and u.huno = h.huno 
	            join wm_prln l on h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.prepno = l.prepno 
	            join wm_product p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
	            join wm_thparty t on  h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.thcode = t.thcode
	            join wm_route r on u.orgcode = r.orgcode and u.site = r.site and u.depot = r.depot and u.routeno = r.routeno 
	            join wm_outbouln o on l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder and l.ouln = o.ouln and l.article = o.article and l.pv = o.pv and l.lv = o.lv
	            join wm_outbound b on o.orgcode = b.orgcode and o.site = b.site and o.depot = b.depot and o.ouorder = b.ouorder
	            left join wm_barcode x on p.orgcode = x.orgcode and p.site = x.site and p.depot = x.depot and p.article = x.article and p.pv = x.pv and p.lv = x.lv and x.isprimary = 1
            where u.orgcode = '{0}' and u.site = '{1}' and u.depot = '{2}' and u.huno = '{3}' 
            group by h.orgcode, h.site, h.depot, h.thcode, t.thnameint,h.huno,h.routeno,r.plandate,datereqdelivery";
        //public static string LabelShippedSql =
        //    @"select h.orgcode, h.site, h.depot, h.thcode, t.thname, h.prepno taskno, sum(qtyskuops) quantity, 
        //        format(r.plandate,'dd/MM/yyyy')  routedate, h.huno,h.routeno,
        //        case when count(l.ouorder) > 1 then 'Multi-Order' else max(l.ouorder) end ouorder,
        //        case when count(l.ouorder) > 1 then 'Multi-Product' else max(l.article) end article,
        //        case when count(l.ouorder) > 1 then 'Multi-Barcode' else max(l.barcode) end barcode,
        //        case when count(l.ouorder) > 1 then 'Multi-Product' else max(p.descalt) end description,
        //        case when count(l.ouorder) > 1 then 'Multi-line' else max(o.ourefno) end ourefno,
        //        case when count(l.ouorder) > 1 then 'Multi-Date' else format(max(b.dateprep),'dd/MM/yyyy') end dateorder,
        //        format(datereqdelivery,'dd/MM/yyyy hh:mm') datedelivery
        //    from wm_prep h, wm_prln l, wm_thparty t, wm_route r, wm_product p,wm_outbouln o, wm_outbound b
        //    where h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.thcode = t.thcode --and preptype = 'P'
        //    and h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.prepno = l.prepno 
        //    and h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno 
        //    and l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
        //    and l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder and l.ouln = o.ouln 
        //    and l.article = o.article and l.pv = o.pv and l.lv = o.lv
        //    and b.orgcode = o.orgcode and b.site = o.site and b.depot = o.depot and b.ouorder = o.ouorder
        //    and h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.huno = '{3}' 
        //    group by h.orgcode, h.site, h.depot, h.thcode, t.thname, h.prepno, h.huno,h.routeno,r.plandate,datereqdelivery ";

        //public static string LabelShippedPalletSql =
        //   @"select h.orgcode, h.site, h.depot, h.thcode, t.thname, h.prepno taskno, sum(qtyskuops) quantity, 
        //        format(r.plandate,'dd/MM/yyyy')  routedate, h.huno,h.routeno,
        //        case when count(l.ouorder) > 1 then 'Multi-Order' else max(l.ouorder) end ouorder,
        //        case when count(l.ouorder) > 1 then 'Multi-Product' else max(l.article) end article,
        //        case when count(l.ouorder) > 1 then 'Multi-Barcode' else max(l.barcode) end barcode,
        //        case when count(l.ouorder) > 1 then 'Multi-Product' else max(p.descalt) end description,
        //        case when count(l.ouorder) > 1 then 'Multi-line' else max(o.ourefno) end ourefno,
        //        case when count(l.ouorder) > 1 then 'Multi-Date' else format(max(b.dateprep),'dd/MM/yyyy') end dateorder,
        //        format(datereqdelivery,'dd/MM/yyyy hh:mm') datedelivery
        //    from wm_prep h, wm_prln l, wm_thparty t, wm_route r, wm_product p,wm_outbouln o, wm_outbound b
        //    where h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.thcode = t.thcode
        //    and h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.prepno = l.prepno 
        //    and h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno 
        //    and l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
        //    and l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder and l.ouln = o.ouln 
        //    and l.article = o.article and l.pv = o.pv and l.lv = o.lv
        //    and b.orgcode = o.orgcode and b.site = o.site and b.depot = o.depot and b.ouorder = o.ouorder
        //    and h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.huno = '{3}' 
        //    group by h.orgcode, h.site, h.depot, h.thcode, t.thname, h.prepno, h.huno,h.routeno,r.plandate,datereqdelivery ";

        public static string LabelShippedPalletSql =
           @"select h.orgcode, h.site, h.depot, t.thcode, t.thnameint thname,
	            sum(l.qtypuops) quantity, format(r.plandate,'dd/MM/yyyy') routedate, u.huno,u.routeno,
	            case when count(h.prepno)  > 1 then 'Multi-Task' else max(h.prepno) end taskno,
                case when count(l.ouorder) > 1 then 'Multi-Order' else max(l.ouorder) end ouorder,
                case when count(l.ouorder) > 1 then 'Multi-Product' else max(l.article) end article,
                case when count(l.ouorder) > 1 then 'Multi-Barcode' else max(o.barcode) end barcode,
                case when count(l.ouorder) > 1 then 'Multi-Product' else max(p.descalt) end description,
                case when count(l.ouorder) > 1 then 'Multi-line' else max(o.ourefno) end ourefno,
                case when count(l.ouorder) > 1 then 'Multi-Date' else format(max(b.dateprep),'dd/MM/yyyy') end dateorder,
                format(datereqdelivery,'dd/MM/yyyy hh:mm') datedelivery
            from wm_handerlingunit u,wm_prep h, wm_prln l, wm_thparty t, wm_route r, wm_product p,wm_outbouln o, wm_outbound b
            where h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.prepno = l.prepno 
            and l.orgcode = u.orgcode and l.site = u.site and l.depot = u.depot and l.huno = u.huno 
            and u.orgcode = r.orgcode and u.site = r.site and u.depot = r.depot and u.routeno = r.routeno  
            and l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
            and l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder and l.ouln = o.ouln and l.article = o.article and l.pv = o.pv and l.lv = o.lv
            and b.orgcode = o.orgcode and b.site = o.site and b.depot = o.depot and b.ouorder = o.ouorder
            and l.orgcode = t.orgcode and l.site = t.site and l.depot = t.depot and l.thcode = t.thcode
            and h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and l.huno = '{3}' 
            group by h.orgcode, h.site, h.depot, t.thcode, t.thnameint, /*h.prepno,*/ u.huno,u.routeno,r.plandate,datereqdelivery";



        //public static string LoadingDraftSql =
        //@"select  h.orgcode, h.site, h.depot, r.thcode, t.thname thname, r.loadtype, lt.bndesc loadtypename, r.routeno,convert(varchar, r.plandate, 103) plandate, 
        //        r.plandate, r.loccode,r.outrno docno, '' ordertype, h.crpu puqty, opscode, case when opstype = 'P' then 'Loose' else 'Pallet' end opstype,
        //        (h.crvolume) volume, h.crweight weight,(h.crvolume/1000) cubic, r.plateNo,r.driver, convert(varchar, r.datereqdelivery, 103)   datedelivery,
        //        r.transportor, tp.thname transportername,
        //        (select distinct CONCAT('',T1.results) AS oudnno FROM(
        //        select REPLACE(STUFF(CAST(( SELECT ' ,' + cast(oudnno as varchar(10)) FROM ( 
        //        select oudnno from wm_outboulx x 
        //        where  x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}'
        //    ) c order by oudnno FOR XML PATH(''), TYPE) AS VARCHAR(MAX)), 1, 2, ''),' ','') AS results from wm_outboulx t) T1 ) ouddno, r.outrno,h.huno
        //from (
        //        select * from wm_handerlingunit h where spcarea = 'ST' and h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.routeno = '{3}' 
        //            and ( exists (select 1 from wm_task t, wm_taln l where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A' 
        //                    and l.orgcode = h.orgcode and l.site = h.site and l.depot = h.depot and l.sourcehuno = h.huno) 
        //                or exists (select 1 from wm_prep p where p.orgcode = h.orgcode and p.site = h.site and p.depot = h.depot and p.huno = h.huno) )
        //        union 
        //        select * from wm_handerlingunit where spcarea = 'XD' and hutype = 'XE' and tflow in ('IO','PE','LD')
        //        and orgcode = '{0}' and site = '{1}' and depot = '{2}' and routeno = '{3}' 
        //    ) h join wm_route r 
        //    on h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno 
        //    left join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and r.thcode = t.thcode
        //    left join ( select orgcode, site, depot, thcode, thname from wm_thparty where thtype = 'TP' ) tp 
        //    on r.orgcode = tp.orgcode and r.site = tp.site and r.depot= tp.depot and r.transportor = tp.thcode
        //    left join ( select orgcode,site,depot,bnvalue, bndesc  from wm_binary where bncode= 'LOADTYPE' and apps = 'WMS' and bntype = 'TRANSPORT') lt
        //    on r.orgcode = lt.orgcode and r.site = lt.site and r.depot = lt.depot and r.loadtype = lt.bnvalue
        //where h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.routeno = '{3}'  ";
        public static string LoadingDraftSql =
           @"select  h.orgcode, h.site, h.depot, r.thcode, t.thnameint thname, r.loadtype,lt.bndesc loadtypename,r.routeno,convert(varchar, r.plandate, 103) plandate, 
                r.plandate, r.loccode,r.outrno docno, '' ordertype, sum(h.crsku / a.rtoskuofpu) puqty, opscode, case when opstype = 'P' then 'Loose' else 'Pallet' end opstype,
                sum(h.crvolume) volume, sum(h.crweight) weight,sum(h.crvolume/1000) cubic, r.plateNo,r.driver, convert(varchar, r.datereqdelivery, 103) datedelivery,
                r.transportor , tp.thname transportername,r.outrno,h.huno
            from (
            select h.*,l.article,l.pv,l.lv from wm_handerlingunit h 
             join wm_taln l on h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot and h.huno = l.sourcehuno  and l.tflow = 'ED'
             where EXISTS (select 1 from wm_task t where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and t.taskno = l.taskno and t.tasktype = 'A'  and t.tflow = 'ED')
             union all
             select h.*,l.article,l.pv,l.lv from wm_handerlingunit h 
             join wm_prep p on p.orgcode = h.orgcode and p.site = h.site and p.depot = h.depot and p.huno = h.huno and h.opstype = 'P' and h.routeno = p.routeno 
             join wm_prln l on p.orgcode = l.orgcode and p.site = l.site and p.depot =l.depot and p.prepno = l.prepno
             union all
             select h.*, l.article, l.pv, l.lv from wm_handerlingunit h, wm_prln l where h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.spcarea = 'XD' 
             	and h.hutype = 'XE' and h.routeno = '{3}' and h.tflow in ('IO','PE','LD') and h.orgcode = l.orgcode and h.site = l.site and h.depot = l.depot
                and h.huno = l.huno and h.opstype is null
            ) h
            join wm_product a on h.orgcode =a.orgcode and h.site = a.site and h.depot = a.depot  and h.article = a.article and h.pv = a.pv and h.lv = a.lv
            join wm_route r  on h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno 
            left join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and r.thcode = t.thcode
            left join ( select orgcode, site, depot, thcode, thname from wm_thparty where thtype = 'TP' ) tp  on r.orgcode = tp.orgcode and r.site = tp.site and r.depot= tp.depot and r.transportor = tp.thcode
            left join ( select orgcode,site,depot,bnvalue, bndesc  from wm_binary where bncode= 'LOADTYPE' and apps = 'WMS' and bntype = 'TRANSPORT') lt  
	            on r.orgcode = lt.orgcode and r.site = lt.site and r.depot = lt.depot and r.loadtype = lt.bnvalue
            where h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.routeno = '{3}'
            group by h.orgcode, h.site, h.depot, r.thcode, t.thnameint, r.loadtype,lt.bndesc,r.routeno,convert(varchar, r.plandate, 103),  r.plandate, r.loccode,r.outrno, 
            opscode,opstype,r.plateNo,r.driver, convert(varchar, r.datereqdelivery, 103) ,r.transportor , tp.thname,r.outrno,h.huno";

        //public static string TransportnoteSql =
        //      @"select  h.orgcode, h.site, h.depot, r.thcode, case when t.thcode = 'CMB' then 'Combine Route' else h.thcode + ' : ' + t.thnamealt end thname, r.loadtype, lt.bndesc loadtypename, r.routeno,FORMAT( r.plandate, 'dd/MM/yyyy hh:mm:ss') plandate, 
        //            r.plandate, r.loccode,r.outrno docno, '' ordertype, opspu puqty, huqty,
        //            (h.crvolume/1000) volume, h.crweight weight,0 cubic, r.plateNo,r.driver, FORMAT( r.dateshipment, 'dd/MM/yyyy hh:mm:ss' )  datedelivery,
        //            r.transportor, tp.thname transportername,
        //            (select distinct CONCAT('',T1.results) AS oudnno FROM(
        //            select REPLACE(STUFF(CAST(( SELECT ' ,' + cast(oudnno as varchar(10)) FROM ( 
        //            select oudnno from wm_outboulx x where  x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and x.outrno = '{4}'
        //        ) c order by oudnno FOR XML PATH(''), TYPE) AS VARCHAR(MAX)), 1, 2, ''),' ','') AS results from wm_outboulx t) T1 ) oudnno, r.outrno
        //    from (select x.orgcode, x.site, x.depot, x.routenodel routeno, o.thcode, x.outrno, count(x.opshuno) huqty, sum(x.opspu) opspu, 
	       //         sum(u.crweight) crweight, sum(u.crvolume) crvolume	from wm_outboulx x, wm_outbound o , wm_handerlingunitlx u
	       //         where x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
        //            and x.orgcode = u.orgcode and x.site = u.site and x.depot = u.depot and x.routenodel = u.routeno and x.opshuno = u.huno
        //            and x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and x.outrno = '{4}'
        //            group by x.orgcode, x.site, x.depot, x.routenodel, o.thcode, x.outrno ) h 
        //        join wm_route r on h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno 
        //        left join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and h.thcode = t.thcode
        //        left join ( select orgcode, site, depot, thcode,  thname from wm_thparty where thtype = 'TP' ) tp 
        //        on r.orgcode = tp.orgcode and r.site = tp.site and r.depot= tp.depot and r.transportor = tp.thcode
        //        left join ( select orgcode, site, depot, bnvalue, bndesc from wm_binary where bncode= 'LOADTYPE' and apps = 'WMS' and bntype = 'TRANSPORT') lt
        //        on r.orgcode = lt.orgcode and r.site = lt.site and r.depot = lt.depot and r.loadtype = lt.bnvalue
        //    where h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.routeno = '{3}' and h.outrno = '{4}' ";


        public static string TransportnoteSql2 =
             @"SELECT *, STUFF((SELECT distinct ',' + xo.oudnno FROM wm_outboulx xo where xo.orgcode = xx.orgcode 
	            AND xo.site = xx.site and xo.depot = xx.depot and xo.outrno = xx.docno FOR XML PATH('') ), 1, 1, '') as oudnno
                FROM (
                SELECT x.orgcode,x.site,x.depot, r.thcode, (case when r.thcode = 'CMB' then 'Combine Route for multi store' else r.thcode + ' : ' + s.thnamealt end) thname, 
                	(case when r.thcode = 'CMB' then 'Combine Route for multi store' else s.thnamealt end) thnamealt,
	                r.loadtype, b.bndesc loadtypename,x.routenodel as routeno,FORMAT(r.plandate, 'dd/MM/yyyy HH:mm:ss') plandate,r.loccode,r.outrno docno,'' ordertype, 
	                sum(x.opspu) puqty, count(distinct x.opshuno) huqty,( sum(x.opspu * p.skuvolume)/1000) volume, sum((x.opspu * p.rtoskuofpu) * p.skugrossweight) weight,0 cubic, r.plateNo, r.driver,
	                format(r.dateshipment, 'dd/MM/yyyy HH:mm:ss' ) as datedelivery,r.transportor, t.thname transportername,r.outrno 
                FROM wm_outboulx x
	                JOIN wm_outbound o ON  x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
	                JOIN wm_product p ON x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv=p.pv and x.lv=p.lv 
	                JOIN wm_route r on x.orgcode = r.orgcode and x.site = r.site and x.depot = r.depot and x.routenodel = r.routeno and x.outrno = r.outrno
	                LEFT JOIN wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot= t.depot and (r.transportor = t.thcode or r.transportor = t.thnameint) and t.thtype = 'TP'
	                LEFT JOIN wm_thparty s on r.orgcode = s.orgcode and r.site = s.site and r.depot= s.depot and r.thcode = s.thcode and s.thtype = 'CS'
	                LEFT JOIN wm_binary b on r.orgcode = b.orgcode and r.site = b.site and r.depot = b.depot and r.loadtype = b.bnvalue and b.bncode= 'LOADTYPE' and b.apps = 'WMS' and b.bntype = 'TRANSPORT'
                where x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and x.outrno = '{4}'
                group by x.orgcode, x.site, x.depot, r.thcode,s.thnamealt,r.loadtype, b.bndesc, x.routenodel,format(r.plandate, 'dd/MM/yyyy HH:mm:ss'),
	                r.loccode,r.plateNo,r.driver,format(r.dateshipment, 'dd/MM/yyyy HH:mm:ss' ),r.transportor,t.thname,r.outrno 
                ) xx";

        //public static string TransportnoteSql =
        //       @"select  h.orgcode, h.site, h.depot, r.thcode, case when t.thcode = 'CMB' then 'Combine Route' else h.thcode + ' : ' + t.thnamealt end thname, r.loadtype, lt.bndesc loadtypename, r.routeno,FORMAT( r.plandate, 'dd/MM/yyyy hh:mm:ss') plandate, 
        //            r.plandate, r.loccode,r.outrno docno, '' ordertype, opspu puqty, huqty,
        //            (h.crvolume/1000) volume, h.crweight weight,0 cubic, r.plateNo,r.driver, FORMAT( r.dateshipment, 'dd/MM/yyyy hh:mm:ss' )  datedelivery,
        //            r.transportor, tp.thname transportername,
        //            (select distinct CONCAT('',T1.results) AS oudnno FROM(
        //            select REPLACE(STUFF(CAST(( SELECT ' ,' + cast(oudnno as varchar(10)) FROM ( 
        //            select oudnno from wm_outboulx x where  x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and x.outrno = '{4}'
        //        ) c order by oudnno FOR XML PATH(''), TYPE) AS VARCHAR(MAX)), 1, 2, ''),' ','') AS results from wm_outboulx t) T1 ) oudnno, r.outrno
        //    from (select x.orgcode, x.site, x.depot, x.routenodel routeno, o.thcode, x.outrno, count(opshuno) huqty, sum(opspu) opspu, sum(opsweight) crweight, sum(opsvolume) crvolume
        //        from wm_outboulx x, wm_outbound o
        //        where x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
        //            and x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and x.outrno = '{4}'
        //            group by x.orgcode, x.site, x.depot, x.routenodel, o.thcode, x.outrno ) h 
        //        join wm_route r on h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno 
        //        left join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and h.thcode = t.thcode
        //        left join ( select orgcode, site, depot, thcode,  thname from wm_thparty where thtype = 'TP' ) tp 
        //        on r.orgcode = tp.orgcode and r.site = tp.site and r.depot= tp.depot and r.transportor = tp.thcode
        //        left join ( select orgcode, site, depot, bnvalue, bndesc from wm_binary where bncode= 'LOADTYPE' and apps = 'WMS' and bntype = 'TRANSPORT') lt
        //        on r.orgcode = lt.orgcode and r.site = lt.site and r.depot = lt.depot and r.loadtype = lt.bnvalue
        //    where h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.routeno = '{3}' and h.outrno = '{4}' ";

        //public static string PackinglistSql =
        //  @"select  x.orgcode, x.site, x.depot,r.routeno, datedel,r.thcode, t.thnameint thname, x.opshuno huno,  x.ouorder, 
        //        (select bndesc from wm_binary b where b.orgcode = x.orgcode and b.site = x.site and b.depot = x.depot and b.bntype = 'PRODUCT' and bncode = 'DEP' 
        //     and RIGHT(REPLICATE('0', 10) + bnvalue , 10) = RIGHT(REPLICATE('0', 10) + hdepartment , 10)) productdept,''productcate, 
        //        x.article product, x.pv, x.lv, b.barcode,p.description, x.opssku skuqty, x.opspu puqty,convert(varchar, dateshipment, 103) shipdate, 
        //     substring(convert(varchar, dateshipment, 108),0,9) shiptime,max(x.oudnno) over (partition by x.ouorder order by  x.ouorder) oudnno,r.outrno,
        //        (x.opssku * p.skugrossweight) opsweight  
        //    from wm_route r join wm_outboulx x on r.orgcode = x.orgcode and r.site = x.site and r.depot = x.depot and r.routeno = x.routenodel 
        //     join wm_outbound o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
        //     join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and o.thcode = t.thcode  
        //     join wm_product p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv 
        //     left join wm_barcode b on x.orgcode = b.orgcode and x.site = b.site and x.depot = b.depot and x.article = b.article and x.pv = b.pv and x.lv = b.lv and b.isprimary = 1
        //    where x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and r.outrno = '{4}' and x.opssku > 0 ";

        //public static string PackinglistSql =
        //  @"select  x.orgcode, x.site, x.depot,r.routeno, datedel,r.thcode, (case when r.thcode = 'CMB' then 'Combine Route' else t.thnameint end) thname, 
        //(case when r.thcode = 'CMB' then x.opshuno+' - '+(select distinct orh.thcode from wm_outbound orh where orh.orgcode = x.orgcode 
        //	and orh.site = x.site and orh.depot = x.depot and orh.ouorder = x.ouorder) else x.opshuno end) huno,  x.ouorder, 
        //            (select bndesc from wm_binary b where b.orgcode = x.orgcode and b.site = x.site and b.depot = x.depot and b.bntype = 'PRODUCT' and bncode = 'DEP' 
        //         and RIGHT(REPLICATE('0', 10) + bnvalue , 10) = RIGHT(REPLICATE('0', 10) + hdepartment , 10)) productdept,''productcate, 
        //            x.article product, x.pv, x.lv, b.barcode,p.description, x.opssku skuqty, x.opspu puqty,convert(varchar, dateshipment, 103) shipdate, 
        //         substring(convert(varchar, dateshipment, 108),0,9) shiptime,max(x.oudnno) over (partition by x.ouorder order by  x.ouorder) oudnno,r.outrno,
        //            (x.opssku * p.skugrossweight) opsweight
        //        from wm_route r join wm_outboulx x on r.orgcode = x.orgcode and r.site = x.site and r.depot = x.depot and r.routeno = x.routenodel 
        //         join wm_outbound o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
        //         join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and o.thcode = t.thcode  
        //         join wm_product p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv 
        //         left join wm_barcode b on x.orgcode = b.orgcode and x.site = b.site and x.depot = b.depot and x.article = b.article and x.pv = b.pv and x.lv = b.lv and b.isprimary = 1
        //        where x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and r.outrno = '{4}' and x.opssku > 0 ";

        //public static string PackinglistSql =
        //      @" select  x.orgcode, x.site, x.depot,r.routeno, datedel,r.thcode, t.thnameint thname, x.opshuno huno,  x.ouorder, 
        //            (select bndesc from wm_binary b where b.orgcode = x.orgcode and b.site = x.site and b.depot = x.depot  
        //                and b.bntype = 'PRODUCT' and bncode = 'DEP' and bnvalue = hdepartment) productdept,  
        //            (select bndesc from wm_binary b where b.orgcode = x.orgcode and b.site = x.site and b.depot = x.depot  
        //                and b.bntype = 'PRODUCT' and bncode = 'SCL' and bnvalue = hsubclass) productcate, 
        //            x.article product, x.pv, x.lv, x.barcode,  
        //            p.description, x.opssku skuqty, x.opspu puqty,convert(varchar, dateshipment, 103) shipdate, substring(convert(varchar, dateshipment, 108),0,9) shiptime, oudnno, r.outrno, x.opsweight 
        //      from wm_route r join wm_outboulx x on r.orgcode = x.orgcode and r.site = x.site and r.depot = x.depot and r.routeno = x.routenodel 
        //      join wm_outbound o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
        //      join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and o.thcode = t.thcode  
        //      join wm_product p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv  
        //     where x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and r.outrno = '{4}' and x.opssku > 0";

        public static string PackinglistSql =
            @"select z.orgcode, z.site, z.depot,z.routeno, format(z.datedel, 'dd/MM/yyyy HH:mm:ss') datedel,z.thcode, z.thname, z.huno, 
	            z.ouorder, z.productdept, z.productcate, z.product, z.pv, z.lv, z.barcode,z.description, sum(z.skuqty) skuqty,  
	            sum(z.puqty) puqty ,z.shipdate, z.shiptime,z.oudnno, z.outrno, sum(z.opsweight) opsweight 
            from (select  x.orgcode, x.site, x.depot,r.routeno, datedel,r.thcode, (case when r.thcode = 'CMB' then 'Combine Route' else t.thnameint end) thname, 
        (case when r.thcode = 'CMB' then x.opshuno+' - '+(select distinct orh.thcode from wm_outbound orh where orh.orgcode = x.orgcode 
        	and orh.site = x.site and orh.depot = x.depot and orh.ouorder = x.ouorder) else x.opshuno end) huno,  x.ouorder, 
                    (select bndesc from wm_binary b where b.orgcode = x.orgcode and b.site = x.site and b.depot = x.depot and b.bntype = 'PRODUCT' and bncode = 'DEP' 
                 and RIGHT(REPLICATE('0', 10) + bnvalue , 10) = RIGHT(REPLICATE('0', 10) + hdepartment , 10)) productdept,''productcate, 
                    x.article product, x.pv, x.lv, b.barcode,p.description, x.opssku skuqty, x.opspu puqty,convert(varchar, dateshipment, 103) shipdate, 
                 substring(convert(varchar, dateshipment, 108),0,9) shiptime,max(x.oudnno) over (partition by x.ouorder order by  x.ouorder) oudnno,r.outrno,
                    (x.opssku * p.skugrossweight) opsweight
                from wm_route r join wm_outboulx x on r.orgcode = x.orgcode and r.site = x.site and r.depot = x.depot and r.routeno = x.routenodel 
                 join wm_outbound o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder
                 join wm_thparty t on r.orgcode = t.orgcode and r.site = t.site and r.depot = t.depot and o.thcode = t.thcode  
                 join wm_product p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv 
                 left join wm_barcode b on x.orgcode = b.orgcode and x.site = b.site and x.depot = b.depot and x.article = b.article and x.pv = b.pv and x.lv = b.lv and b.isprimary = 1
                where x.orgcode = '{0}' and x.site = '{1}' and x.depot = '{2}' and x.routenodel = '{3}' and r.outrno = '{4}' and x.opssku > 0 
            ) z group by z.orgcode, z.site, z.depot,z.routeno, format(z.datedel, 'dd/MM/yyyy HH:mm:ss'),z.thcode, z.thname, z.huno, z.ouorder, 
	        z.productdept, z.productcate, z.product, z.pv, z.lv, z.barcode,z.description, z.shipdate, z.shiptime,z.oudnno, z.outrno ";

        public static string FullpalletSql = @"select  h.orgcode, h.site, h.depot,t.thcode, t.thnameint thname,l.taskno,l.article,l.pv,l.lv,p.description,o.barcode,
	        l.sourceloc loccode,h.routeno,o.ouorder,o.ourefno,format(b.dateprep,'dd/MM/yyyy') dateorder,format(r.datereqdelivery,'dd/MM/yyyy hh:mm') datedelivery,
	        format(r.plandate ,'dd/MM/yyyy hh:mm') datetimeslot,l.sourcehuno huno,l.sourceqty as quantity 
	        from wm_task h join wm_taln l on h.orgcode = l.orgcode  and h.site = l.site and h.depot = l.depot and  h.taskno = l.taskno and h.tasktype = 'A'
	        join wm_route r on h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno   
	        join wm_thparty t on h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.routethcode = t.thcode
	        join wm_outbouln o on l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder 
	        and l.ouln = o.ouln and l.article = o.article and l.pv = o.pv and l.lv = o.lv
	        join wm_outbound b on o.orgcode = b.orgcode  and o.site = b.site and o.depot = b.depot and o.ouorder = b.ouorder
            join wm_product p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
        where  h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and h.taskno = '{3}' ";

        public static string FullpalletByHUSql = @"select  h.orgcode, h.site, h.depot,t.thcode, t.thnameint thname,l.taskno,l.article,l.pv,l.lv,p.description,o.barcode,
	        l.sourceloc loccode,h.routeno,o.ouorder,o.ourefno,format(b.dateprep,'dd/MM/yyyy') dateorder,format(r.datereqdelivery,'dd/MM/yyyy hh:mm') datedelivery,
	        format(r.plandate ,'dd/MM/yyyy hh:mm') datetimeslot,l.sourcehuno huno,l.sourceqty as quantity 
	        from wm_task h join wm_taln l on h.orgcode = l.orgcode  and h.site = l.site and h.depot = l.depot and  h.taskno = l.taskno and h.tasktype = 'A'
	        join wm_route r on h.orgcode = r.orgcode and h.site = r.site and h.depot = r.depot and h.routeno = r.routeno   
	        join wm_thparty t on h.orgcode = t.orgcode and h.site = t.site and h.depot = t.depot and h.routethcode = t.thcode
	        join wm_outbouln o on l.orgcode = o.orgcode and l.site = o.site and l.depot = o.depot and l.ouorder = o.ouorder 
	        and l.ouln = o.ouln and l.article = o.article and l.pv = o.pv and l.lv = o.lv
	        join wm_outbound b on o.orgcode = b.orgcode  and o.site = b.site and o.depot = b.depot and o.ouorder = b.ouorder
            join wm_product p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.article = p.article and l.pv = p.pv and l.lv = p.lv
        where  h.orgcode = '{0}' and h.site = '{1}' and h.depot = '{2}' and l.sourcehuno = '{3}' ";

        public static string CorrectionSql = @"select t.article + '-' + cast(t.lv as varchar(5)) article, t.lv, t.pv,t.loccode location, p.description,
 		        convert(varchar,t.datecreate, 103) daterecipt, '' inorder, t.qtysku quantitysku, t.qtypu quantitypu, 
                t.qtyweight weight, convert(varchar, t.dateexp, 103) dateexp,convert(varchar, t.datemfg, 103)  datemfg, 
                cast(p.rtopckoflayer as varchar(10)) + ' x ' + cast(p.rtolayerofhu as varchar(20)) tihi, 
 	            p.rtoskuofhu skuofpallet, p.rtoskuofipck skuofipck, p.rtoskuofpck skuofpck,p.rtoipckofpck ipckofpck, 
 	            p.rtopckoflayer * p.rtolayerofhu pckofpallet, t.huno, b.barcode  from wm_correction t 
        inner join wm_product p on t.orgcode = p.orgcode and t.site = p.site and t.depot = p.depot and t.article = p.article and t.pv = p.pv and t.lv = p.lv
        left  join wm_barcode b on t.orgcode = b.orgcode and t.site = b.site and t.depot = b.depot and t.article = b.article and t.pv = b.pv and t.lv = b.lv and b.isprimary = '1'
        where t.orgcode = '{0}' and t.site = '{1}' and t.depot = '{2}' and t.seqops = '{3}' and huno ='{4}'";

        public static string PrintSettingSql = "select top 1 bnvalue,bndesc,bndescalt,bnflex1 from wm_binary where orgcode = '{0}' and site = '{1}' and depot = '{2}' and apps = 'WMS' and bntype ='PRINTER' and bncode ='{3}'";
    }
}