using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public partial class correction_ops : IDisposable { 

        //Get correctio code
        // private string sqlcorrect_valcode = "select bnflex1 mapcode, bnflex2 reqorbut from wm_binary where bntype = 'CORRECTION' and bncode = 'CODE' and bnstate = 'IO' and apps = 'WMS' " + 
        // "and orgcode = @orgcode and site = @site and depot = @depot  and bnvalue = @bnvalue";
        private string sqlcorrect_valloc = @" select isnull(( select case when count(1) = 0 then 'NF' 
        when max(l.tflow) not in ('IO') then 'NF' when max(p.tflow) not in ('IO') then 'BL' 
        when cast((max(isnull(crweight,0)) {0} (p.skuweight * @variancesku)) as decimal(20,5)) > max(lsmxweight) then 'OW'
        when cast((max(isnull(crvolume,0)) {0} (p.skuvolume * @variancesku)) as decimal(20,5)) > max(lsmxvolume) then 'OV'
        when (max(isnull(crhu,0)) {0} case when @huno = '0' then 1 else 0 end ) > max(lsmxhuno) then 'OH'
        when max(lsmixarticle) = 0 and (@article != isnull((select max(article) from wm_stock s 
        where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode and s.loccode = @loccode),@article)) then 'OA'
        when max(lsmixlotno)   = 1 and (@batchno != isnull((select max(batchno) from wm_stock s 
        where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = @loccode),@batchno)) then 'OB'
        else max(lscode) end esl from wm_locdw l, wm_product p
        where l.lscode = @loccode and l.orgcode = @orgcode and l.site = @site and l.depot = @depot
        and l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot and p.article = @article and p.pv = @pv and p.lv = @lv
        group by l.orgcode, l.site, l.depot,p.skuweight,p.skuvolume, l.lscode ),'NF') rsl";
        private string sqlcorrect_valcode = ""+
        " select b.orgcode,b.bnflex4 orbitsource, b.bnflex2 reqorbut, o.orbitsite, o.orbitdepot,o.mapcode  from wm_binary b left join ( " +
        " select  o.obitcode orbitcode, o.orgcode, o.site, o.depot, " +
        "    max(case when bncode = 'SITE' then bnflex3 else '' end) orbitsite, " +
        "    max(case when bncode = 'DEPOT' then bnflex3 else '' end) orbitdepot, " +
        "    max(case when bncode = 'CODE' and bnvalue = @bnvalue then bnflex3 else '' end) mapcode " +
        "  from wm_orbit o,wm_binary b " +
        " where apps = o.obitcode and o.orgcode = b.orgcode and o.site = b.site and o.depot = b.depot " +
        " and bncode in ('SITE','DEPOT','CODE') and o.tflow = 'IO' and orbitvld = '1' and isnull(enddate,dateadd(DAY,1,sysdatetimeoffset())) > SYSDATETIMEOFFSET() " +
        " group by o.obitcode, o.orgcode, o.site, o.depot) o on b.bnflex4 = o.orbitcode  and b.orgcode = o.orgcode and b.site = o.site and b.depot = o.depot " +
        " where b.bntype = 'CORRECTION' and bncode = 'CODE' and bnstate = 'IO' and bnvalue = @bnvalue " +
        "   and b.orgcode = @orgcode and b.site = @site and b.depot = @depot order by b.bnflex2 desc";
        // " select bnflex1 mapcode, bnflex2 reqorbut, d.orbitsite, d.orbitdepot,d.orbitsource        " + 
        // " from wm_binary b, wm_depot d where bntype = 'CORRECTION' and bncode = 'CODE' and bnstate = 'IO' " +  
        // " and apps = 'WMS' and b.orgcode = d.orgcode and b.site = d.sitecode and b.depot = d.depotcode    " + 
        // " and b.orgcode = @orgcode and b.site = @site and b.depot = @depot  and b.bnvalue = @bnvalue      " ;

        //private string sqlcorrect_getagrn = @"select isnull(isnull(max(inrefno),(select max(inrefno) from wm_stock 
        //where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv )),'99999999') inrefno,
        //isnull(isnull(max(inagrn),(select max(inagrn) from wm_stock where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv )),'999999') inagrn  
        //from wm_inboulx where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inrefno and article = @article and pv = @pv and lv = @lv";
        // private string sqlcorrect_getagrn =
        //    "select top 1 thcode,inorder,inagrn from xm_xoreceipt x where orgcode = @orgcode and site = @site and depot = @depot and (inorder = @inrefno or inagrn = @inagrn) and article = @article and pv = @pv and lv = @lv";
        private string sqlcorrect_getagrn2 =
           "select top 1 thcode ,inorder,inflag inagrn from wm_inbound where  orgcode = @orgcode and site = @site and depot = @depot and (inorder = @inrefno or inflag=@inagrn)";

        // select top 1 thcode ,inorder,inflag inagrn from wm_inbound where  orgcode = @orgcode and site = @site and depot = @depot and (inorder = @inrefno or inflag= @inagrn)
        //" select inagrn from wm_stock where huno = @huno and stockid = @stockid and orgcode = @orgcode and site = @site and depot = @depot " ;

        private string sqlcorrect_getgrno = "" +
        " select ingrno from wm_stock where huno = @huno and stockid = @stockid and orgcode = @orgcode and site = @site and depot = @depot " ;

        private string sqlcorrect_getinln ="" +
        " select inln from wm_inbouln i, wm_stock s where i.orgcode = s.orgcode and i.site = s.site and i.depot = s.depot and s.inrefno = i.inorder " + 
        " and i.article = s.article and i.pv = s.pv and i.lv = s.lv and s.huno = @huno and s.stockid = @stockid and s.orgcode = @orgcode and s.site = @site " + 
        " and s.depot = @depot ";

        //get handlerunit into 
        private string sqlcorrect_huinfo = " select p.thcode, mxsku,mxweight, mxvolume,p.hucode, @opsqty * skuweight qtyweight, @opsqty * skuvolume qtyvolume, " + 
        "         case  when p.unitmanage = 1 then @opsqty * 1  " +
        " 			    when p.unitmanage = 2 then @opsqty * rtoskuofipck " +
        "			    when p.unitmanage = 3 then @opsqty * rtoskuofpck " +
        " 			    when p.unitmanage = 4 then @opsqty * rtoskuoflayer " +
        " 			    when p.unitmanage = 5 then @opsqty * rtoskuofhu  end qtypu" +
        " from wm_handerlingunit h , wm_product p " + 
        " where h.orgcode = p.orgcode and h.site = p.site and h.depot = p.depot and h.huno = p.hucode " + 
        " and p.orgcode = @orgcode and p.site = @site and p.depot = @depot and p.article = @article and p.pv = @pv and p.lv = @lv ";
        //get product into 
        private string sqlcorrect_prodinfo = "  select  @opsqty * skuweight qtyweight, @opsqty * skuvolume qtyvolume, " +
        "         case  when p.unitmanage = 1 then @opsqty * 1  " +
        " 			    when p.unitmanage = 2 then @opsqty * rtoskuofipck " +
        "			    when p.unitmanage = 3 then @opsqty * rtoskuofpck " +
        " 			    when p.unitmanage = 4 then @opsqty * rtoskuoflayer " +
        " 			    when p.unitmanage = 5 then @opsqty * rtoskuofhu  end qtypu,thcode" +
        " from wm_product p where 1=1 " +
        " and orgcode = @orgcode and site = @site and depot = @depot and p.article = @article and pv = @pv and lv = @lv"; 

        //validate huno
        private string sqlcorrect_valhuno = "select case when @opsqty > (mxsku - crsku) then 'HUsku' when @qtyweight > (mxweight - crweight) then 'HUweight' " + 
        "            when @qtyvolume > (mxvolume - crvolume) then 'HUvolume' when tflow != 'IO' then 'HUflow' " +
        "            else 'pass' end rsl " +
        " from wm_handerlingunit where orgcode = @orgcode and site = @site and depot = @depot and huno = @huno ";

        //validate huno
        private string sqlcorrect_isbulkloc = "select top 1 count(1) isbulk from wm_locdw where spcarea ='BL' and fltype = 'BL' and spcpicking = 0 and orgcode = @orgcode and site = @site and depot = @depot  and lscode =@loccode";

        //Insert new correction
        private string sqlcorrect_new = "insert into wm_correction                           " + 
        " (orgcode,site,depot,spcarea,dateops,accnops,seqops,codeops,typeops,thcode          " +
        " ,article,pv,lv,unitops,qtysku,qtypu,qtyweight,qtyvolume,inreftype,inrefno          " + 
        " ,ingrno,inpromo,reason,stockid,huno,loccode,daterec,batchno,lotno,datemfg          " +
        " ,dateexp,serialno,tflow,datecreate,accncreate,procmodify,inagrn) VALUES                   " +
        " (@orgcode,@site,@depot,@spcarea,@dateops,@accnops,@seqops,@codeops,@typeops        " + 
        " ,@thcode,@article,@pv,@lv,@unitops,@qtysku,@qtypu,@qtyweight,@qtyvolume            " +
        " ,@inreftype,@inrefno,@ingrno,@inpromo,@reason,@stockid,@huno,@loccode              " +
        " ,@daterec,@batchno,@lotno,@datemfg,@dateexp,@serialno,'IO',sysdatetimeoffset()   " +
        " ,@accncreate,@procmodify,@inagrn)" ;
        //Send to orbit
        private string sqlcorrect_sendtorbit = "INSERT INTO dbo.xm_xocorrection             " + 
        " (orgcode,site,depot,dateops,accnops,seqops,codeops,typeops,thcode,article         " +
        " ,pv,lv,unitops,qtysku,qtyweight,inreftype,inrefno,ingrno,inpromo,reason           " + 
        " ,xaction,xcreate,orbitsite, orbitdepot, orbitsource, inagrn, rowid ) values       " + 
        " (@orgcode,@site,@depot,@dateops,@accnops,@seqops,@codeops,@typeops,@thcode        " +
        " ,@article,@pv,@lv,@unitops,@qtysku,@qtyweight,@inreftype,@inrefno,@ingrno         " + 
        " ,@inpromo,@reason,'WC',sysdatetimeoffset(),@orbitsite, @orbitdepot, @orbitsource " + 
        " ,@inagrn ,next value for dbo.seq_xocorrection ) ";

        //Get Location for correctio xxx
        private string sqlcorrect_getlocation = "" +
        " select top 1 lscode, lscodealt + ' - Reserve' lscodealt from wm_locdw where tflow in ('IO','IX')      " + 
        "    and lsloctype = 'LC' and spcpicking = 0 and orgcode = @orgcode and depot = @depot and site = @site " + 
        " union all                                                                                             " + 
        " select top 1 lscode, lscodealt + ' - Picking' lscodealt from wm_locdw where tflow in ('IO','IX')      " + 
        "    and lsloctype = 'LC' and spcpicking = 1 and orgcode = @orgcode and depot = @depot and site = @site " + 
        " union all                                                                                             " + 
        " select top 7 lscode, lscodealt + ' - Bulk' lscodealt from wm_locdw where fltype = 'BL'             " + 
        "    and tflow in ('IO','IX') and lsloctype = 'UT' and spcpicking = 0 and orgcode = @orgcode            " + 
        "    and depot = @depot and site = @site                               ";
        string sqlcorrect_stategic_step1_product = @"select spcrecvzone, 
        case when isnull(spcrecvaisle,'')   != '' then (select cast(min(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'AL' and lscode = spcrecvaisle     ) else '' end spcrecvaisle, 
        case when isnull(spcrecvaisleto,'') != '' then (select cast(max(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'AL' and lscode = spcrecvaisleto   ) else '' end spcrecvaisleto, 
        case when isnull(spcrecvbay,'')     != '' then (select cast(min(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'BA' and lscode = spcrecvbay       ) else '' end spcrecvbay, 
        case when isnull(spcrecvbayto,'')   != '' then (select cast(max(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'BA' and lscode = spcrecvbayto     ) else '' end spcrecvbayto, 
        case when isnull(spcrecvlevel,'')   != '' then (select cast(min(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'LV' and lscode = spcrecvlevel     ) else '' end spcrecvlevel, 
        case when isnull(spcrecvlevelto,'') != '' then (select cast(max(lsseq) as varchar(10)) from wm_locup where orgcode = @orgcode and site = @site and depot = @depot and spcarea = 'ST' and fltype = 'LV' and lscode = spcrecvlevelto   ) else '' end spcrecvlevelto, 
        isnull(spcrecvlocation,'') spcrecvlocation, isnull(isslowmove,'0')  isslowmove
        from wm_product where orgcode = @orgcode and site = @site and depot = @depot and article = @article and pv = @pv and lv = @lv";

        string sqlcorrect_statetic_step2_selection = @" select * from 
        (select a.lscode,'RV' + ' : '+a.lscode lscodealt,'reserve' loctype,0 priv  from ( 
        select a.orgcode, a.site, a.depot, a.lscode,selpriv, row_number() over (order by cast(selpriv as numeric(16)) {5} ) rn  
        from (select  l.*, 
                case when l.spcarea = 'ST' then '99' else '90' end + 
                case when isdynamicpick = 1 then '9' else '0' end + 
                case when isdynamicpick = 1 and l.spclasttouch = p.article then '9' else '0' end + 
                case when z.lscode = l.lscode then '9' else '0' end + 
                case when spcarticle = p.article then '9' else '0' end + 
                case when spcrecvzone = l.lszone then '9' else '0' end + 
                case when spcrecvlocation = l.lscode then '9' else '0' end + 
                case when spcrecvaisle = l.lsaisle then '9' else '0' end + 
                case when spcrecvbay = l.lsbay then '9' else '0' end + 
                case when spcrecvlevel = l.lslevel then '9' else '0' end + 
                case when l.spcthcode = p.thcode then '9' else '0' end + 
                case when spcpicking = 0 then '9' else '0' end selpriv 
        from ( select top 10 * from wm_locdw l where orgcode = @orgcode and site = @site and depot = @depot and tflow in ('IO','IX')
            and (l.spcarea = 'ST' and l.fltype = 'LC' and l.lsloctype = 'LC') 
            and lsmxhuno >= isnull(crhu,0) + 1 and lsmxweight >= isnull(crweight,0) + 0 and lsmxvolume >= isnull(crvolume,0) + 0
            and not exists (select 1 from wm_stock where wm_stock.orgcode = l.orgcode and wm_stock.site = l.site and wm_stock.depot = l.depot and wm_stock.loccode = l.lscode) 
            and not exists (select 1 from wm_taln where wm_taln.orgcode = l.orgcode and wm_taln.site = l.site and wm_taln.depot = l.depot and ( wm_taln.sourceloc = l.lscode or wm_taln.targetadv = l.lscode ) and wm_taln.tflow = 'IO')
            and not exists (select 1 from wm_loczp z where z.orgcode = l.orgcode and z.site = l.site and z.depot = l.depot and z.lscode = l.lscode)
            {0} {1} {2} {3} {4} 
        ) l 
        left join (select top 1 * from wm_product where orgcode = @orgcode and site = @site and depot = @depot and tflow = 'IO' and article = @article and pv = @pv and lv = @lv) p 
            on l.orgcode = p.orgcode and l.site = p.site and l.depot = p.depot 
        left join (select top 1 * from wm_loczp where orgcode = @orgcode and site = @site and depot = @depot and spcproduct = @article and spcpv = @pv and spclv = @lv ) z 
            on p.orgcode = z.orgcode and p.site = z.site and p.depot = z.depot and p.spcprepzone = z.przone   
        where l.orgcode = @orgcode and l.site = @site and l.depot = @depot and article = @article and pv = @pv and lv = @lv
        ) a ) a  
        union all  
        select lscode,lsloctype + ' : ' + lscodealt lscodealt,'bulk' loctype, 2 priv from wm_locdw where orgcode = @orgcode and site = @site and depot = @depot and spcarea not in ('ST','XD','RS','DS') and lsloctype not in ('RS','DS') 
        union all 
        select top 5 lscode,'PK' + ' : '+ lscodealt lscodealt,'picking' loctype,1 priv from wm_locdw l where orgcode = @orgcode and site = @site and depot = @depot and spcpicking = '1' and tflow = 'IO'
            and not exists (select 1 from wm_stock s where s.orgcode = l.orgcode and s.site = l.site and s.depot = l.depot and s.loccode = l.lscode) 
            and not exists (select 1 from wm_taln  t where t.orgcode = l.orgcode and t.site = l.site and t.depot = l.depot and ( t.sourceloc = l.lscode or t.targetadv = l.lscode ) and t.tflow = 'IO')
            and not exists (select 1 from wm_loczp z where z.orgcode = l.orgcode and z.site = l.site and z.depot = l.depot and z.lscode = l.lscode)
        ) lscode order by priv";
    }
}