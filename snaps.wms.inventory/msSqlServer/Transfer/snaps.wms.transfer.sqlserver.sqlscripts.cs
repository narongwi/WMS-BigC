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

namespace Snaps.WMS {
    public partial class transfer_ops : IDisposable { 

        public string sqltransfer_checklocation = @" 
        select l.lscode, 'SP : '+ lscodealt lscodealt, case when l.spcarea in ('ST','XD') and spcpicking = '1' then 'picking'
                    when l.spcarea in ('ST','XD') and l.lscode = p.lscode then 'picking'
                    when l.spcarea in ('ST','XD') and spcpicking = '0' then 'reserve' else 'bulk' end loctype, 3  priv
            from wm_locdw l left join wm_loczp p on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and l.lscode = p.lscode
        where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.lscode = @loccode and l.tflow = 'IO' ";

        public string sqltransfer_validlocation = ""+ 
        " select  case                                                                                              " +
        "             when p.tflow not in ('IO','IX') then 'ATflow'                                                 " +
        "             when ismeasurement = 1 then 'ATrqm'                                                           " +
        "             when p.tflow not in ('IO') then 'LCflow'                                                      " +
        "             when skuweight > (lsmxweight - l.crweight) then 'LCweight'                                    " +
        "             when skuvolume > (lsmxvolume - l.crvolume) then 'LCvolume'                                    " +
        "             when 1 > (lsmxhuno - crhu) then 'LChu'                                                        " +
        "             when skuweight > (mxweight - h.crweight) then 'HUweight'                                      " +
        "             when skuvolume > (mxvolume - h.crvolume) then 'HUvolume'                                      " +
        "             when @opssku > (mxsku - crsku)  then 'HUsku'                                                  " +
        "             when h.tflow != 'IO' then 'HUflow'                                                            " +
        "             else 'pass' end rsl                                                                           " +
        " from                                                                                                      " +
        " (select orgcode, site, depot, hucode, @opssku * skuweight skuweight, @opssku * skuvolume skuvolume,       " +
        "         tflow,ismeasurement                                                                               " +
        "    from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article    " + 
        "     and pv = @pv and lv = @lv ) p,                                                                        " + 
        "    wm_handerlingunit h, wm_locdw l                                                                        " + 
        " where p.orgcode = h.orgcode and p.site = h.site and p.depot = h.depot and p.hucode = h.huno               " + 
        " and p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot and l.lscode = @opsloccode            " ;

        public string sqltransfer_validflowlocation =
         @"select  case  when p.tflow not in ('IO','IX') then 'Article state does not active'                                                 
             when p.ismeasurement = 1 then 'Article still require to measurement'                                                           
             when l.tflow != 'IO' then 'Location state does not active'                                                      
             when (@opssku * p.skuweight) > (l.lsmxweight - l.crweight) then 'Weight on location is over capacity'                                    
             when (@opssku * p.skuvolume) > (l.lsmxvolume - l.crvolume) then 'Volume on location is over capacity'
             else 'pass' end rsl from  wm_product p , wm_locdw l 
         where p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.article = @article and p.pv = @pv and p.lv = @lv
         and p.orgcode = l.orgcode and p.site = l.site and p.depot = l.depot  and l.lscode =  @opsloccode";

        // check target location is existing transfer
        public string sqltransfer_validexisting =
            @"select tl.taskno,tl.targetadv,tl.sourcehuno,tl.accncreate, th.tflow , tl.tflow from wm_task th join wm_taln tl on  th.orgcode = tl.orgcode 
            and th.site = tl.site and th.depot = tl.depot  and th.taskno = tl.taskno 
            where th.orgcode = @orgcode and th.site = @site and th.depot = @depot and th.tasktype ='T' and th.tflow = 'IO' and tl.targetadv = @opsloccode
            and exists (select 1 from wm_locdw ld where tl.orgcode = ld.orgcode and tl.site = ld.site and tl.depot = ld.depot and ld.lscode = tl.targetloc
            and ld.spcarea = 'ST' and ld.fltype ='LC' and ld.lsloctype='LC')";

        public string sqltransfer_validproduct = @"select top 1 p.ismeasurement,(@opssku * p.skuweight) skuweight , (@opssku * p.skuvolume) skuvolume 
                from wm_product p where p.orgcode = @orgcode and p.site = @site and p.depot = @depot 
                and p.article = @article and p.pv = @pv and p.lv = @lv and  p.tflow in('IO','IX')";
        public string sqltransfer_validmixlocation =
            @"select case when l.tflow != 'IO' then 'Location state does not active' 
            when l.crhu + 1 > l.lsmxhuno then 'Location is over Maximum HU'
		    when @skuweight > (l.lsmxweight - l.crweight) THEN 'Weight on location is over capacity'  
		    when @skuvolume > (l.lsmxvolume - l.crvolume) then 'Volume on location is over capacity'
            when l.lsmixarticle = 0 and sum(case when @article != s.article then 1 else 0 end) > 0 then 'Location is not allow Mixing Article'
            when l.lsmixage = 0 and sum(case when @daterec != s.daterec then 1 else 0 end) > 0 then 'Location is not allow Mixing Aging'
            when l.lsmixlotno = 0 and sum(case when @datemfg != s.datemfg then 1 else 0 end) > 0 then 'Location is not Mixing Lot mfg'
        else 'pass' end from  wm_locdw l left join wm_stock s  on l.orgcode = s.orgcode and l.site =s.site and l.depot = s.depot and l.lscode  = s.loccode 
        where l.lscode = @opsloccode group by l.tflow,l.lsmixarticle,l.lsmixage,l.lsmixlotno,l.lsmxweight,l.crweight,l.lsmxvolume,l.crvolume,l.crhu, l.lsmxhuno";


        private string sqltransfer_new = " INSERT INTO wm_transfer "+
        " (orgcode,site,depot,spcarea,dateops,accnops,seqops,thcode,article,pv,lv       " +
        " ,unitops,qtysku,qtypu,qtyweight,qtyvolume,inreftype,inrefno,ingrno,inpromo    " +
        " ,reason,stockid,huno,sourcelocation,targetlocation,daterec,batchno,lotno      " +
        " ,datemfg,dateexp,serialno,rsltaskno,rslhuno,rslstockid,tflow,datecreate       " +
        " ,accncreate,procmodify) VALUES  " +
        " (@orgcode,@site,@depot,@spcarea,@dateops,@accnops,@seqops,@thcode,@article    " +
        " ,@pv,@lv,@unitops,@qtysku,@qtypu,@qtyweight,@qtyvolume,@inreftype,@inrefno    " +
        " ,@ingrno,@inpromo,@reason,@stockid,@huno,@sourcelocation,@targetlocation      " +
        " ,@daterec,@batchno,@lotno,@datemfg,@dateexp,@serialno,@rsltaskno,@rslhuno     " +
        " ,@rslstockid,@tflow,SYSDATETIMEOFFSET(),@accncreate,@procmodify) " ;

        //get handlerunit into 
        private string sqltransfer_huinfo = " select p.thcode, mxsku,mxweight, mxvolume,p.hucode, @opsqty * skuweight qtyweight, @opsqty * skuvolume qtyvolume, " + 
        "         case  when p.unitmanage = 1 then @opsqty * 1  " +
        " 			    when p.unitmanage = 2 then @opsqty * rtoskuofipck " +
        "			    when p.unitmanage = 3 then @opsqty * rtoskuofpck " +
        " 			    when p.unitmanage = 4 then @opsqty * rtoskuoflayer " +
        " 			    when p.unitmanage = 5 then @opsqty * rtoskuofhu  end qtypu" +
        " from wm_handerlingunit h , wm_product p " + 
        " where h.orgcode = p.orgcode and h.site = p.site and h.depot = p.depot and h.huno = p.hucode " + 
        " and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.article = @article and p.pv = @pv and p.lv = @lv ";

        private string sqltransfer_reqblock = @"select b.bnflex from wm_locdw l, ( select orgcode, site, depot, bnvalue,isnull(bnflex1,0) bnflex from wm_binary 
        where bntype = 'LOCATION' and bncode = 'AREA' ) b where l.orgcode = b.orgcode and l.site = b.site and l.depot = b.depot and l.spcarea = b.bnvalue 
        and l.orgcode = @orgcode and l.site = @site and l.depot = @depot and l.lscode = @lscode";
    }
}