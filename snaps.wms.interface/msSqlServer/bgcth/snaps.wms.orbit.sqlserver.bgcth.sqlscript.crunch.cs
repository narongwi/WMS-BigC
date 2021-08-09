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

    public partial class orbit_ops : IDisposable {
        string sqlInterface_crunching_inbound =
            @"select distinct                                                                               
               case when o.tflow not in ('IO') then '9' 
		            when t.thcode is null then '9'
		            when x.rowops = '1' and o.inorder is null then '1' 
		            when x.rowops = '1' and o.inorder is not null then '9'
                    when x.rowops = '2' and o.inorder is null then '9'
                    when x.rowops = '2' and o.inorder is not null then '2'
                    when x.rowops = '3' and o.inorder is not null then '3'
                    when x.rowops = '3' and o.inorder is null then '9'
		            else '9' end rowops,     
               case when o.tflow not in ('IO') then 'Order has process can not modify'                  
		            when t.thcode is null then 'Third party not interface' 
                    when x.rowops in('2','3') and o.inorder is null then 'Order not found.'
                    when x.rowops not in ('1','2','3') then 'Action code Not Support'
                    when x.rowops = '1' and o.inorder is not null then 'Duplicate Interface'
		            else null end ermsg, 
            x.rowid,x.site,x.depot,x.inorder                                    
            from xm_xiinbound x left join wm_inbound o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.inorder = o.inorder 
            left join wm_thparty t on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.thcode = t.thcode   
            where  x.tflow = 'WC'";

       string sqlInterface_crunching_inbound_clear = "" + 
        " update xm_xiinbound set tflow = 'ED', dateops = SYSDATETIMEOFFSET() where rowid = @rowid    ";
        string sqlInterface_crunching_inbound_error = "" + 
        " update xm_xiinbound set tflow = 'ER', ermsg = @ermsg, dateops = SYSDATETIMEOFFSET() where rowid = @rowid ";
        string sqlInterface_crunching_inbound_lnerror =
        @"update d set tflow = 'ER',ermsg = @ermsg, dateops = SYSDATETIMEOFFSET()
            from xm_xiinbouln d inner join xm_xiinbound h on d.site = h.site and d.depot = h.depot and d.inorder = h.inorder and h.rowid = @rowid and h.tflow='ER'
            where d.tflow = 'WC'";
        string sqlInterface_crunching_inbound_insert = ""+
        " insert into wm_inbound                                                                                             " +
        " ( orgcode,site,depot,spcarea,thcode,intype,subtype,inorder, dateorder,dateplan,dateexpire,                         " +
        "   inpriority,inflag, inpromo,tflow,datecreate,accncreate,procmodify,orbitsource,datereplan  )                      " +
        " select orgcode,site,depot,spcarea,thcode,intype,subtype,inorder,isnull(SYSDATETIMEOFFSET(),dateorder) dateorder,   " +
        "        dateplan,dateexpire,inpriority,next value for seq_inflag inflag,inpromo,'IO' tflow,SYSDATETIMEOFFSET() datecreate, orbitsource accncreate ," + 
        "        orbitsource procmodify, orbitsource, dateplan datereplan                                                    " + 
        " from xm_xiinbound x where rowid = @rowid                                                                           " ;

        // oracle
        string sqlInterface_crunching_inbound_cleargc =
            "DELETE FROM ICSENTOR WHERE iersite = :site and ierdepo = :depot and IERCEXCDE=:inorder and ierdmaj is null  ";

        string sqlInterface_crunching_inbound_update = "" + 
        "  update t                                                              " +
        "     set                                                                " +
        "         t.dateplan  = s.dateplan,                                      " +
        "         t.dateexpire = s.dateexpire,                                   " +
        "         t.inpriority = s.inpriority,                                   " +
        "         t.inflag = s.inflag,                                           " +
        "         t.inpromo = s.inpromo,                                         " +
        "         t.tflow = 'IO',                                             " +
        "         t.procmodify = s.orbitsource,                                  " +
        "         t.orbitsource = s.orbitsource,                                 " +
        "         t.datemodify = SYSDATETIMEOFFSET(), accnmodify = s.orbitsource " + 
        " from wm_inbound t                                                      " +
        "  inner join xm_xiinbound s                                               " + 
        "   on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot  " + 
        "  and t.inorder = s.inorder                                             " + 
        " where rowid = @rowid                                                   " ;

        string sqlInterface_crunching_inbound_delete = "" +
        " delete t from wm_inbound t inner join xm_xiinbound s                        " +
        "     on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot " +
        "    and t.inorder = s.inorder                                           " +
        " where rowid = @rowid                                                      " ;


        string sqlInterface_crunching_inbound_cancel = "" +
        " update t set t.tflow='CL', t.datemodify = SYSDATETIMEOFFSET(), t.accnmodify = s.orbitsource " +
        " from wm_inbound t join xm_xiinbound s on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.inorder = s.inorder  " +
        " where rowid = @rowid";

        //string sqlInterface_crunching_inbouln = ""+
        //" select case when ermsg is not null then '9' else  rowops end rowops,ermsg,rowid,site,depot,inorder,article " +
        ////" select rowops,ermsg,rowid                  " +
        //"  from (                                                                                          " +
        //"  select rowops,                                                                                  " +
        //"         case when o.tflow not in ('IO') then 'Order has process can not modify'                  " +
        //"              when t.article is null then 'Product not interface'                                 " +
        //"         else null end ermsg, x.rowid, x.site,x.depot,x.inorder,x.article                         " +
        //"   from xm_xiinbouln x left join wm_inbouln o                                                     " +
        //"     on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.inorder = o.inorder " +
        //"    and x.article = o.article and x.pv = o.pv and x.lv = o.lv and x.inln = o.inln                 " +
        //"   left join wm_product t                                                                         " +
        //"     on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article " +
        //"    and x.lv = t.lv and x.pv = t.pv                                                               " +
        //"  where  x.tflow = 'WC' ) a                                                                       " ;

        string sqlInterface_crunching_inbouln =
            @"select (case when o.tflow not in ('IO') then '9'  when t.article is null then '9'  when x.rowops not in ('1','2','3') then '9' else rowops end) rowops,                                                                                  
	            (case when o.tflow not in ('IO') then 'Order has process can not modify' when t.article is null then 'Product not interface'                                 
	                 when x.rowops not in ('1','2','3') then 'Action code Not Support' else null end) ermsg, x.rowid, x.site,x.depot,x.inorder,x.article                         
            from xm_xiinbouln x left join wm_inbouln o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.inorder = o.inorder 
	            and x.article = o.article and x.pv = o.pv and x.lv = o.lv and x.inln = o.inln  left join wm_product t                                                                         
	            on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article and x.lv = t.lv and x.pv = t.pv                                                               
            where  x.tflow = 'WC'";

        string sqlInterface_crunching_inbouln_clear = "" + 
        " update xm_xiinbouln set tflow = 'ED', dateops = SYSDATETIMEOFFSET() where rowid = @rowid    ";
        string sqlInterface_crunching_inbouln_error = "" + 
        " update xm_xiinbouln set tflow = 'ER', ermsg = @ermsg, dateops = SYSDATETIMEOFFSET() where rowid = @rowid ";

        string sqlInterface_crunching_outbound_lnerror =
            @"update d set tflow = 'ER',ermsg = @ermsg, dateops = SYSDATETIMEOFFSET()
                from xm_xioutbouln d inner join xm_xioutbound h on d.site = h.site and d.depot = h.depot and d.ouorder = h.ouorder and h.rowid = @rowid and h.tflow='ER'
            where d.tflow = 'WC'";

       // next value for seq_agrn inagrn
       string sqlInterface_crunching_inbouln_insert = "" + 
        " insert into wm_inbouln ( orgcode,site,depot,spcarea,inorder,inln,inrefno,inrefln,inagrn,barcode,article,pv,lv,unitops,                                      " + 
        "   qtysku,qtypu, qtyweight,lotno,expdate,serialno,qtypnd,tflow,datecreate,accncreate,procmodify,batchno,orbitsource, inseq )                                 " + 
        " select x.orgcode,x.site,x.depot,                                                                                                                            " + 
        "    (select top 1 spcarea from wm_inbound n where n.orgcode = x.orgcode and n.site = x.site and n.depot = x.depot and n.inorder = x.inorder ) spcarea,             " +
        "    inorder,inln,inrefno,inrefln,next value for seq_agrn inagrn," +
        "    (select max(b.barcode) as barcode from wm_barcode b where b.orgcode=x.orgcode and b.site=x.site and b.depot = x.depot and b.article=x.article and b.pv=x.pv and b.lv=x.lv and b.tflow = 'IO') as barcode, " + //b.barcode,
        "    x.article,x.pv, x.lv,p.unitreceipt unitops,qtysku,                                " +
        "   CEILING(x.qtysku/dbo.get_ratiopu_receive(x.orgcode, x.site, x.depot,x.article,x.pv,x.lv)) qtypu, qtysku * skuweight qtyweight,                " +
        "    lotno,expdate,serialno,CEILING(x.qtysku/dbo.get_ratiopu_receive(x.orgcode, x.site, x.depot,x.article,x.pv,x.lv)) qtypnd,'IO' tflow,SYSDATETIMEOFFSET() datecreate,                                                                          " + 
        "	x.orbitsource accncreate,x.orbitsource procmodify, batchno,x.orbitsource,                                                                                 " + 
        "    isnull((select count(1) from wm_inbouln l where l.orgcode = x.orgcode and l.site = x.site and l.depot = x.depot and l.inorder = x.inorder),0) + 1 inseq  " + 
        "   from xm_xiinbouln x                                                                                                                                       " +
        //"   left join wm_barcode b on x.orgcode = b.orgcode and x.site = b.site and x.depot = b.depot and x.article = b.article and x.pv = b.pv and x.lv = b.lv       " +
        "   left join wm_product p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.pv = p.pv and x.lv = p.lv       " +
        "  where rowid = @rowid ";

        string sqlInterface_crunching_inbouln_update = "" +
        "   update t set t.unitops = s.unitops, t.qtysku = s.qtysku,t.qtypu = CEILING(s.qtysku/dbo.get_ratiopu_receive(s.orgcode, s.site, s.depot,s.article,s.pv,s.lv)),t.qtyweight = s.qtyweight,                        " +
        "          t.lotno = s.lotno, t.expdate = s.expdate,t.serialno = s.serialno,t.qtypnd = CEILING(s.qtysku/dbo.get_ratiopu_receive(s.orgcode, s.site, s.depot,s.article,s.pv,s.lv)),t.tflow = 'IO',                 " +
        "          t.datemodify = sysdatetimeoffset(), t.accnmodify = s.orbitsource,t.procmodify = s.orbitsource,t.batchno = s.batchno, " +
        " 		 t.orbitsource = s.orbitsource                                                                                          " +
        "  from wm_inbouln t inner join xm_xiinbouln s                                                                                  " +
        "   on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.inorder = s.inorder and t.inln = s.inln           " +
        "   and t.article = s.article and t.pv = s.pv and t.lv = s.lv where rowid = @rowid                                              " ;
        string sqlInterface_crunching_inbouln_delete = "" + 
        " delete t from wm_inbouln t join xm_xiinbouln s                                                                      " + 
        "    on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.inorder = s.inorder and t.inln = s.inln " + 
        "   and t.article = s.article and t.pv = s.pv and t.lv = s.lv where rowid = @rowid                                     " ;

        string sqlInterface_crunching_inbouln_cancel = "" +
        " update t set t.tflow = 'CL',t.datemodify = sysdatetimeoffset(), t.accnmodify = s.orbitsource from wm_inbouln t join xm_xiinbouln s                                                                      " +
        "   on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.inorder = s.inorder and t.inln = s.inln " +
        "   and t.article = s.article and t.pv = s.pv and t.lv = s.lv where rowid = @rowid                                     ";

        string sqlInterface_crunching_inbouln_cleargc =
            " DELETE FROM ICSDETOR  WHERE idrsite =:site and idrdepo=:depot and idrcexcde =:inorder and idrcode=:article and idrdmaj is null  ";

        //Crunching Outbound order from orbit to earth
        //string sqlInterface_crunching_outbound = "" +
        //" select case when ermsg is not null then '9' else  rowops end rowops,ermsg,rowid,site,depot,ouorder " +
        ////" select rowops,ermsg,rowid                 " +
        //" from (                                                                                          " +
        //" select rowops,                                                                                  " +
        //"        case when x.rowops = '1' and o.ouorder is not null then 'Order is Duplicate interface' " +  
        //"             when o.tflow not in ('IO') then 'Order has process can not modify'                  " +
        //"             when t.thcode is null then 'Third party not interface'                              " +
        //"        else null end ermsg, x.rowid ,x.site, x.depot,x.ouorder                                " +
        //"  from xm_xioutbound x left join wm_outbound o                                                   " +
        //"    on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder " + 
        //"  left join wm_thparty t                                                                         " + 
        //"    on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.thcode = t.thcode   " + 
        //" where  x.tflow = 'WC' ) a                                                                       " ;

        string sqlInterface_crunching_outbound =
            @"select distinct
               case when o.tflow not in ('IO') then '9' when t.thcode is null then '9'
		            when x.rowops = '1' and o.ouorder is null then '1' 
		            when x.rowops = '1' and o.ouorder is not null then '2'
                    when x.rowops = '2' and o.ouorder is not null then '3'
		            when x.rowops = '2' and o.ouorder is null then '9'
		            else '9' end rowops , 
               case when o.tflow not in ('IO') then 'Order has process can not modify' 
		            when t.thcode is null then 'Third party not interface'
		            when x.rowops = '2' and o.ouorder is null then 'Order not found'
                    when x.rowops not in ('1','2') then 'Action code Not Support'
                else null end ermsg, x.rowid ,x.site, x.depot,x.ouorder                                
            from xm_xioutbound x left join wm_outbound o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder  
            left join wm_thparty t on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.thcode = t.thcode    
            where  x.tflow = 'WC' ";

        string sqlInterface_crunching_outbound_clear = "" + 
        " update xm_xioutbound set tflow = 'ED', dateops = SYSDATETIMEOFFSET() where rowid = @rowid    ";
        string sqlInterface_crunching_outbound_error = "" + 
        " update xm_xioutbound set tflow = 'ER', ermsg = @ermsg, dateops = SYSDATETIMEOFFSET() where rowid = @rowid ";
        //string sqlInterface_crunching_outbound_insert = "" +
        //" insert into wm_outbound                                                                                     " +
        //" ( orgcode,site,depot,spcarea,ouorder,outype,ousubtype,thcode,dateorder,dateprep,dateexpire,oupriority,      " +
        //"   ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,stoaddressln2,stoaddressln3,            " +
        //"   stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,datereqdel,tflow,             " +
        //"   datecreate,accncreate,procmodify,inorder,qtyorder, qtypnd )                                               " +
        //" select orgcode,site,depot,isnull(NULLIF(spcarea, ''),'ST') spcarea,ouorder,outype,ousubtype,thcode,dateorder,dateprep,dateexpire,oupriority, " +
        //"   ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,stoaddressln2,stoaddressln3,            " +
        //"   stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,dateprep datereqdel,'IO' tflow,    " +
        //"   SYSDATETIMEOFFSET() ,orbitsource accncreate,orbitsource procmodify,inorder,0 qtyorder,0 qtypnd               " +
        //" from xm_xioutbound x where rowid = @rowid                                                                   " ;
        string sqlInterface_crunching_outbound_insert = "" +
      " insert into wm_outbound                                                                                     " +
      " ( orgcode,site,depot,spcarea,ouorder,outype,ousubtype,thcode,dateorder,dateprep,dateexpire,oupriority,      " +
      "   ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,stoaddressln2,stoaddressln3,            " +
      "   stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,datereqdel,tflow,             " +
      "   datecreate,accncreate,procmodify,inorder,qtyorder, qtypnd ,datemodify,accnmodify)                          " +
      " select orgcode,site,depot,spcarea," +
      " ouorder,outype,ousubtype,thcode,dateorder,dateprep,dateexpire,oupriority, " +
      "   ouflag,oupromo,dropship,orbitsource,stocode,stoname,stoaddressln1,stoaddressln2,stoaddressln3,            " +
      "   stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,dateexpire datereqdel,'IO' tflow,    " +
      "   SYSDATETIMEOFFSET() ,orbitsource accncreate,orbitsource procmodify,inorder,0 qtyorder,0 qtypnd ,SYSDATETIMEOFFSET(),orbitsource  " +
      " from xm_xioutbound x where rowid = @rowid                                                                   ";
        string sqlInterface_crunching_outbound_update = "" +
        "  update t                                                                                                            " +
        "     set t.dateprep = s.dateprep,t.dateexpire = s.dateexpire , t.oupriority = s.oupriority,                           " +
        "         t.ouflag = s.ouflag, t.oupromo = s.oupromo,t.dropship = s.dropship,t.orbitsource = s.orbitsource,            " +
        "         t.stoaddressln1 = s.stoaddressln1,t.stoaddressln2 = s.stoaddressln2,t.stoaddressln3 = s.stoaddressln3,       " +
        "         t.stosubdistict = s.stosubdistict,t.stodistrict = s.stodistrict,t.stocity = s.stocity,                       " +
        "         t.stocountry = s.stocountry,t.stopostcode = s.stopostcode,t.stomobile = s.stomobile,t.stoemail = s.stoemail, " +
        "         t.datemodify = SYSDATETIMEOFFSET(),t.accnmodify = s.orbitsource                                              " +
        "  from wm_outbound t                                                                                                  " +
        "  left join xm_xioutbound s                                                                                           " +
        "    on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder                     " +
        " where rowid = @rowid                                                                                                 " ;
        string sqlInterface_crunching_outbound_delete = "" + 
        " delete t from wm_outbound t join xm_xioutbound s                                                 " +
        "    on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder " +
        " where rowid = @rowid                                                                             " ;
        string sqlInterface_crunching_outbound_cancel = "" +
     " update t set tflow = 'CL', datemodify=SYSDATETIMEOFFSET(),accnmodify='BGCTH.GC' from wm_outbound t join xm_xioutbound s                                                 " +
     "    on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder " +
     " where rowid = @rowid                                                                             ";

        string sqlInterface_crunching_outbound_oudono = "select next value for seq_dono as oudono";
        //string sqlInterface_crunching_outbouln = "" +
        //" select case when ermsg is not null then '9' else  rowops end rowops,ermsg,rowid,site,depot,ouorder,article" +
        //"  from (                                                                                          " +
        //"  select rowops,                                                                                  " +
        //"         case when o.tflow not in ('IO') then 'Order has process can not modify'                  " +
        //"              when t.article is null then 'Article not interface'                                 " +
        //"              when h.ouorder is null then 'Outbound header not interface'                         " +
        //"         else null end ermsg, x.rowid,x.site ,x.depot, x.ouorder,x.article                       " +
        //"   from xm_xioutbouln x left join wm_outbouln o                                                   " +
        //"     on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder " +
        //"    and x.ouln = o.ouln                                                                           " +
        //"   left join wm_product t                                                                         " +
        //"     on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article " +
        //"    and x.pv = t.pv and x.lv = t.lv                                                               " +
        //"   left join wm_outbound h                                                                        " +
        //"     on x.orgcode = h.orgcode and x.site = h.site and x.depot = h.depot and x.ouorder = h.ouorder " +
        //"  where  x.tflow = 'WC' ) a ";

        string sqlInterface_crunching_outbouln =
        @"select (case 
	            when o.tflow not in ('IO') then '9' 
	            when t.article is null then '9' 
	            when h.ouorder is null then '9'
	            when x.rowops not in ('1','2') then '9'
	            when x.rowops = '1' and o.ouorder is null then '1' 
	            when x.rowops = '1' and o.ouorder is not null then '2'
                when x.rowops = '2' and o.ouorder is not null then '3'
	            when x.rowops = '2' and o.ouorder is null then '9'
	            else rowops end) rowops ,                                                                                 
           (case 
	           when o.tflow not in ('IO') then 'Order has process can not modify'                  
	           when t.article is null then 'Article not interface'                                 
	           when h.ouorder is null then 'Outbound header not interface'
	           when x.rowops not in ('1','2') then 'Action code Not Support'
	           when x.rowops = '2' and o.ouorder is null then 'Order not found'
	           else null end) ermsg, x.rowid,x.site ,x.depot, x.ouorder,x.article                       
           from xm_xioutbouln x left join wm_outbouln o on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.ouorder = o.ouorder and x.ouln = o.ouln                                                                           
	           left join wm_product t on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article and x.pv = t.pv and x.lv = t.lv                                                               
	           left join wm_outbound h on x.orgcode = h.orgcode and x.site = h.site and x.depot = h.depot and x.ouorder = h.ouorder 
           where  x.tflow = 'WC' ";

        string sqlInterface_crunching_outbouln_clear = "" + 
        " update xm_xioutbouln set tflow = 'ED', dateops = SYSDATETIMEOFFSET() where rowid = @rowid    ";
        string sqlInterface_crunching_outbouln_error = "" + 
        " update xm_xioutbouln set tflow = 'ER', ermsg = @ermsg, dateops = SYSDATETIMEOFFSET() where rowid = @rowid ";
        string sqlInterface_crunching_outbouln_insert = ""+ 
        " insert into wm_outbouln                                                                           " +
        " (orgcode,site,depot,spcarea,ouorder,ouln,ourefno,ourefln,inorder,b.barcode,article,               " +
        "  pv,lv,unitops,qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,                    " +
        "  serialno,tflow,datecreate,accncreate,disthcode,qtypndsku,qtypndpu,qtyreqpu,                      " +
        "  procmodify,orbitsource,ouseq,oudono,qtyreqsku)                                         " +
        "  select x.orgcode,x.site,x.depot,x.spcarea,x.ouorder,x.ouln,x.ourefno,x.ourefln,x.inorder,b.barcode,x.article,x.pv,x.lv,x.unitops,x.qtysku," +
        "   CEILING(x.qtysku/p.rtoskuofpu) qtypu,(x.qtysku * p.skugrossweight) qtyweight,x.spcselect,x.batchno,x.lotno," +
        "   x.datemfg,x.dateexp,x.serialno,'IO' tflow,sysdatetimeoffset() datecreate,x.orbitsource accncreate,x.disthcode,x.qtysku qtypndsku,CEILING(x.qtysku/p.rtoskuofpu) qtypndpu," +
        "   CEILING(x.qtysku/p.rtoskuofpu) qtyreqpu, x.orbitsource procmodify,x.orbitsource,x.ouseq,@oudono, x.qtysku as qtyreqsku" +
        "  from xm_xioutbouln x" +
        "   left join wm_product p on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.article = p.article and x.lv = p.lv" +
        "   left join wm_barcode b on x.orgcode = b.orgcode and x.site = b.site and x.depot = b.depot and x.article = b.article and x.lv = b.lv and b.isprimary = '1'" +
        "  where x.rowid = @rowid";
        //"  and b.tflow = 'IO' and isprimary = '1' where rowid = @rowid                " ;

        //  string sqlInterface_crunching_outbouln_insert = "" +
        //" insert into wm_outbouln                                                                           " +
        //" (orgcode,site,depot,spcarea,ouorder,ouln,ourefno,ourefln,inorder,b.barcode,article,               " +
        //"  pv,lv,unitops,qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,                    " +
        //"  serialno,tflow,datecreate,accncreate,disthcode,qtypndsku,qtypndpu,qtyreqpu,                      " +
        //"  procmodify,orbitsource,ouseq,oudono,qtyreqsku,qtyreqou )                                         " +
        //" select x.orgcode,x.site,x.depot,isnull(NULLIF(x.spcarea, ''),'ST') spcarea,ouorder,ouln,ourefno,ourefln,inorder,barcode,x.article, " +
        //"  x.pv,x.lv,unitops,qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,                " +
        //"  serialno,'IO' tflow,sysdatetimeoffset() datecreate,x.orbitsource accncreate,                     " +
        //"  disthcode,qtysku qtypndsku,qtypu qtypndpu, qtypu qtyreqpu,                                       " +
        //"  x.orbitsource procmodify,x.orbitsource,ouseq,@oudono, x.qtysku, x.qtypu                              " +
        //" from xm_xioutbouln x left join wm_barcode b                                                       " +
        //"   on x.orgcode = b.orgcode and x.site = b.site and x.depot = b.depot and x.article = b.article    " +
        //"  and b.tflow = 'IO' where rowid = @rowid                                                          ";
        string sqlInterface_crunching_outbouln_update = "" +
        "  update t set t.orgcode = s.orgcode, t.unitops = s.unitops ,t.qtysku = s.qtysku,t.qtypu = CEILING(s.qtysku/dbo.get_ratiopu_prep(s.orgcode, s.site, s.depot,s.article,s.pv,s.lv)), " +	
        "		t.qtyweight = s.qtyweight, t.batchno = s.batchno,t.lotno = s.lotno,t.datemfg = s.datemfg,    " +
        "		t.dateexp = s.dateexp,t.serialno = s.serialno ,t.qtypndsku = s.qtysku,      " +
        "		t.qtypndpu = cast(cast(s.qtysku as decimal(10,2)) / cast(dbo.get_ratiopu_prep(s.orgcode, s.site, s.depot,s.article,s.pv,s.lv) as decimal(10,2)),t.orbitsource = s.orbitsource,t.datemodify = SYSDATETIMEOFFSET(),       " +
        "		t.accnmodify = s.orbitsource , t.qtyreqsku = s.qtysku, t.qtyreqpu = CEILING(s.qtysku/dbo.get_ratiopu_prep(s.orgcode, s.site, s.depot,s.article,s.pv,s.lv))                  " +
        " from wm_outbouln t                                                                                 " +
        " left join xm_xioutbouln s                                                                          " +
        "   on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder    " +
        "  and t.ouln = s.ouln and t.article = s.article and t.pv = s.pv and t.lv = s.lv where rowid = @rowid" ;
        string sqlInterface_crunching_outbouln_delete = "" + 
        " delete t from wm_outbouln t left join xm_xioutbouln s												 " +
        "    on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder    " +
        "   and t.ouln = s.ouln and t.article = s.article and t.pv = s.pv and t.lv = s.lv                    " +
        " where s.rowid = @rowid                                                                             " ;

        string sqlInterface_crunching_outbouln_cancel = "" +
       " update t set tflow='CL',datemodify=SYSDATETIMEOFFSET(),accnmodify='BGCTH.GC' from wm_outbouln t left join xm_xioutbouln s	" +
       "    on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.ouorder = s.ouorder    " +
       "   and t.ouln = s.ouln and t.article = s.article and t.pv = s.pv and t.lv = s.lv                    " +
       " where s.rowid = @rowid                                                                             ";

        string sqlInterface_crunching_outbouln_cleargc = "DELETE FROM ICSDEOL where iolsite = :site and ioldepo = :depot and iolcexcde=:ouorder and iolcodc=:article and ioldmaj is null";

        //Crunching Product from orbit to earth 
        //string sqlInterface_crunching_product = "" + 
        //" select  " +
        //         "case when p.thcode is null then '9' when (x.rowops='1' and t.article is null) then '1' when (x.rowops='1' and t.article is not null) then '2' else '3' end rowops, " +
        //"         case when p.thcode is null then 'Thirdparty not interface' else null end ermsg, x.rowid    " +
        //"  from xm_xiproduct x left join wm_product t                                                         " +
        //"    on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article     " +
        //"   and x.pv = t.pv and x.lv = t.lv                                                                   " +
        //"  left join wm_thparty p  on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.thcode = p.thcode       " +
        //" where  x.tflow = 'WC' " ;
        string sqlInterface_crunching_product =
            @"select distinct case when p.thcode is null then '9' 
	               when (x.rowops='1' and t.article is null) then '1' 
	               when (x.rowops='1' and t.article is not null) then '2' 
                   when (x.rowops='2' and t.article is not null) then '3' 
	               when (x.rowops not in ('1','2')) then '9'
	               else '3' end rowops, 
                   case when p.thcode is null then 'Thirdparty not interface'
                        when (x.rowops not in ('1','2')) then 'Action code Not Support' 
                        else null end ermsg, x.rowid    
               from xm_xiproduct x left join wm_product t on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article  and x.pv = t.pv and x.lv = t.lv                                                                   
              left join wm_thparty p  on x.orgcode = p.orgcode and x.site = p.site and x.depot = p.depot and x.thcode = p.thcode       
              where  x.tflow = 'WC'";
        string sqlInterface_crunching_product_clear = ""+
        " update xm_xiproduct set tflow = 'ED', dateops = SYSDATETIMEOFFSET() where rowid = @rowid    ";
        string sqlInterface_crunching_product_error = ""+
        " update xm_xiproduct set tflow = 'ER', ermsg = @ermsg, dateops = SYSDATETIMEOFFSET() where rowid = @rowid ";
        string sqlInterface_crunching_product_insert = "" + 
        " insert into wm_product                                                                            " +
        "     ( orgcode,site,depot,spcarea,article,articletype,pv,lv,description,descalt,thcode             " +
        "     ,dlcall,dlcfactory,dlcwarehouse,dlcshop,dlconsumer,hdivison,hdepartment,hsubdepart            " +
        "     ,hclass,hsubclass,typemanage,unitmanage,unitdesc,unitreceipt,unitprep,unitsale                " +
        "     ,unitstock,unitweight,unitdimension,unitvolume,hucode,rtoskuofpu,rtoskuofipck                 " +
        "     ,rtoskuofpck,rtoipckofpck,rtoskuoflayer,rtoskuofhu,rtopckoflayer,rtolayerofhu,innaturalloss                " +
        "     ,ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight                  " +
        "     ,skugrossweight,skuweight,skuvolume,pulength,puwidth,puheight,pugrossweight                   " +
        "     ,puweight,puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight                 " +
        "     ,ipckvolume,pcklength,pckwidth,pckheight,pckgrossweight,pckweight,pckvolume                   " +
        "     ,layerlength,layerwidth,layerheight,layergrossweight,layerweight,layervolume                  " +
        "     ,hulength,huwidth,huheight,hugrossweight,huweight,huvolume                                    " +
        "     ,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription,isdlc,ismaterial                " +
        "     ,isunique,isalcohol,istemperature,isdynamicpick,ismixingprep,isfinishgoods                    " +
        "     ,isnaturalloss,isbatchno,ismeasurement,roomtype,tempmin,tempmax,alcmanage                     " +
        "     ,alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin                 " +
        "     ,stockthresholdmax,spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation           " +
        "     ,spcprepzone,spcdistzone,spcdistshare,spczonedelv,orbitsource,tflow,datecreate,accncreate)    " +
        " select orgcode,site,depot,spcarea,article,'G' articletype,pv,lv,description,descalt,thcode        " +
        "     ,dlcall,dlcfactory,dlcwarehouse,dlcshop,dlconsumer,hdivison,hdepartment,hsubdepart            " +
        "     ,hclass,hsubclass,'PP' typemanage,unitmanage,unitdesc,unitreceipt,unitprep,unitsale                " +
        "     ,unitstock,unitweight,unitdimension,unitvolume,'PL01' hucode,rtoskuofpu,rtoskuofipck                 " +
        "     ,rtoskuofpck,rtoipckofpck,rtoskuoflayer,rtoskuofhu,rtopckoflayer,rtolayerofhu,innaturalloss   " +
        "     ,ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight                  " +
        "     ,skugrossweight,skuweight,skuvolume,pulength,puwidth,puheight,pugrossweight                   " +
        "     ,puweight,puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight                 " +
        "     ,ipckvolume,pcklength,pckwidth,pckheight,pckgrossweight,pckweight,pckvolume                   " +
        "     ,layerlength,layerwidth,layerheight,layergrossweight,layerweight,layervolume                  " +
        "     ,hulength,huwidth,huheight,hugrossweight,huweight,huvolume                                    " +
        "     ,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription,(case when dlcall > 0 then 1 else 0 end) isdlc,ismaterial" +
        "     ,isunique,isalcohol,istemperature,isdynamicpick,ismixingprep,isfinishgoods                    " +
        "     ,isnaturalloss,isbatchno,1 ismeasurement,'Normal' roomtype,tempmin,tempmax,alcmanage          " +
        "     ,alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin,stockthresholdmax," +
        "     dbo.get_default(orgcode,site,depot,'RCVZONE') spcrecvzone,spcrecvaisle,spcrecvbay,spcrecvlevel,spcrecvlocation,       " +
        "     dbo.get_default(orgcode,site,depot,'PREPZONE') spcprepzone," +
        "     dbo.get_default(orgcode,site,depot,'DISTZONE') spcdistzone," +
        "     dbo.get_default(orgcode,site,depot,'FARESHARE') spcdistshare,spczonedelv,orbitsource,'IO' tflow,SYSDATETIMEOFFSET(), " +
        "      orbitsource accncreate from xm_xiproduct where rowid = @rowid ";
        string sqlInterface_crunching_product_update = "" +
        " update t                                                                                                              " +
        "    set t.orgcode = s.orgcode,t.site = s.site,t.depot = s.depot,t.spcarea = s.spcarea,t.article = s.article,              " +
        " t.pv = s.pv,t.lv = s.lv,t.description = s.description,t.descalt = s.descalt,               " +
        " t.thcode = s.thcode,t.dlcall = s.dlcall,t.dlcfactory = s.dlcfactory,t.dlcwarehouse = s.dlcwarehouse,                     " +
        " t.dlcshop = s.dlcshop,t.dlconsumer = s.dlconsumer,t.hdivison = s.hdivison,t.hdepartment = s.hdepartment,                 " +
        " t.hsubdepart = s.hsubdepart,t.hclass = s.hclass,t.hsubclass = s.hsubclass,                " +
        " t.unitmanage = s.unitmanage,t.unitdesc = s.unitdesc,t.unitreceipt = s.unitreceipt,t.unitprep = s.unitprep,               " +
        " t.unitsale = s.unitsale,t.unitstock = s.unitstock,t.unitweight = s.unitweight,t.unitdimension = s.unitdimension,         " +
        " t.unitvolume = s.unitvolume,t.rtoskuofpu = s.rtoskuofpu,t.rtoskuofipck = s.rtoskuofipck,             " +
        " t.rtoskuofpck = s.rtoskuofpck,t.rtoskuoflayer = s.rtoskuoflayer,t.rtoskuofhu = s.rtoskuofhu,                             " +
        " t.innaturalloss = s.innaturalloss,                     " +
        " t.ounaturalloss = s.ounaturalloss,t.costinbound = s.costinbound,t.costoutbound = s.costoutbound,                         " +
        " t.costavg = s.costavg,t.skulength = s.skulength,t.skuwidth = s.skuwidth,t.skuheight = s.skuheight,                       " +
        " t.skugrossweight = s.skugrossweight,t.skuweight = s.skuweight,t.skuvolume = s.skuvolume,t.pulength = s.pulength,         " +
        " t.puwidth = s.puwidth,t.puheight = s.puheight,t.pugrossweight = s.pugrossweight,t.puweight = s.puweight,                 " +
        " t.puvolume = s.puvolume,t.ipcklength = s.ipcklength,t.ipckwidth = s.ipckwidth,t.ipckheight = s.ipckheight,               " +
        " t.ipckgrossweight = s.ipckgrossweight,t.ipckweight = s.ipckweight,t.ipckvolume = s.ipckvolume,t.pcklength = s.pcklength, " +
        " t.pckwidth = s.pckwidth,t.pckheight = s.pckheight,t.pckgrossweight = s.pckgrossweight,t.pckweight = s.pckweight,         " +
        " t.pckvolume = s.pckvolume,t.layerlength = s.layerlength,t.layerwidth = s.layerwidth,t.layerheight = s.layerheight,       " +
        " t.layergrossweight = s.layergrossweight,t.layerweight = s.layerweight,t.layervolume = s.layervolume,                     " +
        " t.hulength = s.hulength,t.huwidth = s.huwidth,t.huheight = s.huheight,t.hugrossweight = s.hugrossweight,                 " +
        " t.huweight = s.huweight,t.huvolume = s.huvolume,t.tempmin = s.tempmin,t.tempmax = s.tempmax,     " +
        " t.alcmanage = s.alcmanage,t.alccategory = s.alccategory,t.alccontent = s.alccontent,t.alccolor = s.alccolor,             " +
        " t.dangercategory = s.dangercategory,t.dangerlevel = s.dangerlevel,t.stockthresholdmin = s.stockthresholdmin,             " +
        " t.stockthresholdmax = s.stockthresholdmax,t.datemodify = SYSDATETIMEOFFSET(),t.orbitsource = s.orbitsource,              " +
        " t.procmodify = s.orbitsource, t.accnmodify = s.orbitsource                                                               " +
        "  from wm_product t                                                                                                       " +
        "  left join xm_xiproduct s                                                                                                " +
        "    on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode                            " +
        "   and t.article = s.article and t.pv = s.pv and t.lv = s.lv                                                              " +
        " where rowid = @rowid ";

        //   string sqlInterface_crunching_product_update = "" +
        //" update t                                                                                                                 " +
        //"    set t.orgcode = s.orgcode,t.site = s.site,t.depot = s.depot,t.spcarea = s.spcarea,t.article = s.article,              " +
        //" t.articletype = s.articletype,t.pv = s.pv,t.lv = s.lv,t.description = s.description,t.descalt = s.descalt,               " +
        //" t.thcode = s.thcode,t.dlcall = s.dlcall,t.dlcfactory = s.dlcfactory,t.dlcwarehouse = s.dlcwarehouse,                     " +
        //" t.dlcshop = s.dlcshop,t.dlconsumer = s.dlconsumer,t.hdivison = s.hdivison,t.hdepartment = s.hdepartment,                 " +
        //" t.hsubdepart = s.hsubdepart,t.hclass = s.hclass,t.hsubclass = s.hsubclass,t.typemanage = s.typemanage,                   " +
        //" t.unitmanage = s.unitmanage,t.unitdesc = s.unitdesc,t.unitreceipt = s.unitreceipt,t.unitprep = s.unitprep,               " +
        //" t.unitsale = s.unitsale,t.unitstock = s.unitstock,t.unitweight = s.unitweight,t.unitdimension = s.unitdimension,         " +
        //" t.unitvolume = s.unitvolume,t.hucode = s.hucode,t.rtoskuofpu = s.rtoskuofpu,t.rtoskuofipck = s.rtoskuofipck,             " +
        //" t.rtoskuofpck = s.rtoskuofpck,t.rtoskuoflayer = s.rtoskuoflayer,t.rtoskuofhu = s.rtoskuofhu,                             " +
        //" t.rtopckoflayer = s.rtopckoflayer,t.rtolayerofhu = s.rtolayerofhu,t.innaturalloss = s.innaturalloss,                     " +
        //" t.ounaturalloss = s.ounaturalloss,t.costinbound = s.costinbound,t.costoutbound = s.costoutbound,                         " +
        //" t.costavg = s.costavg,t.skulength = s.skulength,t.skuwidth = s.skuwidth,t.skuheight = s.skuheight,                       " +
        //" t.skugrossweight = s.skugrossweight,t.skuweight = s.skuweight,t.skuvolume = s.skuvolume,t.pulength = s.pulength,         " +
        //" t.puwidth = s.puwidth,t.puheight = s.puheight,t.pugrossweight = s.pugrossweight,t.puweight = s.puweight,                 " +
        //" t.puvolume = s.puvolume,t.ipcklength = s.ipcklength,t.ipckwidth = s.ipckwidth,t.ipckheight = s.ipckheight,               " +
        //" t.ipckgrossweight = s.ipckgrossweight,t.ipckweight = s.ipckweight,t.ipckvolume = s.ipckvolume,t.pcklength = s.pcklength, " +
        //" t.pckwidth = s.pckwidth,t.pckheight = s.pckheight,t.pckgrossweight = s.pckgrossweight,t.pckweight = s.pckweight,         " +
        //" t.pckvolume = s.pckvolume,t.layerlength = s.layerlength,t.layerwidth = s.layerwidth,t.layerheight = s.layerheight,       " +
        //" t.layergrossweight = s.layergrossweight,t.layerweight = s.layerweight,t.layervolume = s.layervolume,                     " +
        //" t.hulength = s.hulength,t.huwidth = s.huwidth,t.huheight = s.huheight,t.hugrossweight = s.hugrossweight,                 " +
        //" t.huweight = s.huweight,t.huvolume = s.huvolume,t.roomtype = s.roomtype,t.tempmin = s.tempmin,t.tempmax = s.tempmax,     " +
        //" t.alcmanage = s.alcmanage,t.alccategory = s.alccategory,t.alccontent = s.alccontent,t.alccolor = s.alccolor,             " +
        //" t.dangercategory = s.dangercategory,t.dangerlevel = s.dangerlevel,t.stockthresholdmin = s.stockthresholdmin,             " +
        //" t.stockthresholdmax = s.stockthresholdmax,t.datemodify = SYSDATETIMEOFFSET(),t.orbitsource = s.orbitsource,              " +
        //" t.procmodify = s.orbitsource, t.accnmodify = s.orbitsource                                                               " +
        //"  from wm_product t                                                                                                       " +
        //"  left join xm_xiproduct s                                                                                                " +
        //"    on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode                            " +
        //"   and t.article = s.article and t.pv = s.pv and t.lv = s.lv                                                              " +
        //" where rowid = @rowid ";

        string sqlInterface_crunching_product_delete = "" +
        " delete t                                                                                      " +
        "  from wm_product t                                                                            " +
        "  left join xm_xiproduct s                                                                     " +
        "    on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode " +
        "   and t.article = s.article and t.pv = s.pv and t.lv = s.lv                                   " +
        " where s.rowid = @rowid ";

        string sqlInterface_crunching_product_cancel = "" +
        " update t set tflow='CL',datemodify=SYSDATETIMEOFFSET(),accncreate='BGCTH.GC'                  " +
        "  from wm_product t                                                                            " +
        "  left join xm_xiproduct s                                                                     " +
        "    on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode " +
        "   and t.article = s.article and t.pv = s.pv and t.lv = s.lv                                   " +
        " where s.rowid = @rowid ";


        //Crunching Barcode from orbit to earth 
        //string sqlInterface_crunching_barcode = "" +
        //" select case when ermsg is not null then '9' else rowops end rowops,ermsg,rowid,site,depot,article,lv,barcode" +
        //" from (                                                                                          " +
        //" select x.rowops,                                                                                " +
        //"        case when t.article is null then 'Product not interface'                                 " +
        //"             when x.rowops = '1' and o.barcode is not null then 'Duplicate Interface'            " +
        //"             else null end ermsg, x.rowid,x.site,x.depot,x.article,x.lv,x.barcode                " +
        //"  from xm_xibarcode x left join wm_barcode o                                                     " +
        //"    on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.barcode = o.barcode " + 
        //"  left join wm_product t                                                                         " + 
        //"    on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article " + 
        //"   and x.pv = t.pv and x.lv = t.lv                                                               " + 
        //" where  x.tflow = 'WC' ) a                                                                       " ;

        string sqlInterface_crunching_barcode =
            @"select distinct case 
                     when (t.article is null) then '9'
                     when (x.rowops not in ('1','2')) then '9'
                     when (x.rowops= '1' and o.barcode is null) then '1' 
                     when (x.rowops= '1' and o.barcode is not null) then '2'
                     when (x.rowops= '2' and x.barcode is not null) then '3' 
                     when (x.barcode is null) then '4'
		             else '4' end rowops,                    
                case 
                    when t.article is null then 'Product not interface' 
                    when x.rowops not in ('1','2') then 'Action code Not Support'
                    else null end ermsg, x.rowid, x.site, x.depot, x.article, x.lv, x.barcode                
            from xm_xibarcode x left join wm_barcode o  on x.orgcode = o.orgcode and x.site = o.site and x.depot = o.depot and x.barcode = o.barcode  
            left join wm_product t   on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.article = t.article  
            and x.pv = t.pv and x.lv = t.lv                                                                
            where  x.tflow = 'WC' ";

        string sqlInterface_crunching_barcode_clear = ""+
        " update xm_xibarcode set tflow = 'ED', dateops = SYSDATETIMEOFFSET() where rowid = @rowid    ";
        string sqlInterface_crunching_barcode_error = ""+
        " update xm_xibarcode set tflow = 'ER', ermsg = @ermsg, dateops = SYSDATETIMEOFFSET() where rowid = @rowid ";
        string sqlInterface_crunching_barcode_insert = "" +
        " insert into wm_barcode                                                                            " +
        "      ( orgcode,site,depot,spcarea,article,pv,lv,barops,barcode,bartype,thcode,orbitsource,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,isprimary ) " +
        " select orgcode,site,depot,spcarea,article,pv,lv,'AL' barops,barcode,bartype,thcode,orbitsource,(case when barops = '1' then 'IO' else 'XX' end) tflow, " +
        " sysdatetimeoffset() datecreate,orbitsource accncreate,sysdatetimeoffset() datemodify,orbitsource accnmodify, orbitsource procmodify,(case when barops = '1' then 1 else 0 end) isprimary" +
        "   from xm_xibarcode x where rowid = @rowid ";
        string sqlInterface_crunching_barcode_cleargc = 
            "delete from icsarticle where iarsite = :site and iardepo = :depot and iarcexr=:article and iarcexvl=:lv and iarean =:barcode";
        string sqlInterface_crunching_article_cleargc =
          "delete from icsarticle where iarsite = :site and iardepo = :depot and iarcexr=:article and iarcexvl=:lv ";
        string sqlInterface_crunching_barcode_update = "" + 
        "   update t                                                            " + 
        "      set t.barops = s.barops, t.bartype = s.bartype,                  " + 
        "          t.thcode = s.thcode ,t.orbitsource = s.orbitsource,          " +
        "          t.tflow = (case when s.barops = '1' then 'IO' else 'XX' end), t.datemodify = sysdatetimeoffset(),       " + 
        "          t.accnmodify = s.orbitsource,t.procmodify = s.orbitsource ,  " +
        "          t.isprimary = (case when s.barops = '1' then 1 else 0 end)   " +
        "  from wm_barcode t                                                    " + 
        "  left join xm_xibarcode s                                             " + 
        "    on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot " +  
        "   and t.barcode = s.barcode                                           " + 
        "  where rowid = @rowid                                                 " ; 
        string sqlInterface_crunching_barcode_delete = "" + 
        " delete t  from wm_barcode t join xm_xibarcode s                        " +
        "     on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot " +
        "    and t.barcode = s.barcode where  rowid = @rowid  ";

        string sqlInterface_crunching_barcode_cancel = "" +
        " update t set t.tflow='XX', t.datemodify = sysdatetimeoffset(),t.accnmodify=s.orbitsource from wm_barcode t join xm_xibarcode s                        " +
        "     on t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot " +
        "    and t.barcode = s.barcode where  rowid = @rowid ";
        //Crunching Thirdparty from orbit to earth
        //string sqlInterface_crunching_thirdparty = "" + 
        //" select  case when t.thcode is null then 1 else x.rowops end rowops,null ermsg, x.rowid        " + 
        //"  from xm_xithirdparty x left join wm_thparty t                                                " + 
        //"    on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.thcode = t.thcode " + 
        //" where  x.tflow = 'WC'";
        string sqlInterface_crunching_thirdparty =
        @"select  distinct
           case when x.rowops = '1' and t.thcode is null then '1'
		        when x.rowops = '1' and t.thcode is not null then '2'
		        when x.rowops = '2' then '3' else '9' end rowops,
           case when x.rowops not in ('1','2') then 'Action code Not Support' else null end ermsg, 
	        x.rowid, x.site,x.depot,x.thcode
        from xm_xithirdparty x left join wm_thparty t on x.orgcode = t.orgcode and x.site = t.site and x.depot = t.depot and x.thcode = t.thcode
        where  x.tflow = 'WC'";

        string sqlInterface_crunching_thirdparty_cleargc = "DELETE FROM ICSTIERS WHERE itisite =:tsite AND itidepo = :tdepot AND iticod =:tcode";
        string sqlInterface_crunching_thirdparty_clear = "" + 
        " update xm_xithirdparty set tflow = 'ED', dateops = SYSDATETIMEOFFSET() where rowid = @rowid    ";
        string sqlInterface_crunching_thirdparty_error = ""+
        " update xm_xithirdparty set tflow = 'ER', ermsg = @ermsg, dateops = SYSDATETIMEOFFSET() where rowid = @rowid ";
        string sqlInterface_crunching_thirdparty_insert = ""+
        " insert into wm_thparty                                                        " + 
        " (orgcode,site,depot,spcarea,thtype,thbutype,thcode,thcodealt,vatcode,         " +
        " thname,thnamealt,thnameint,addressln1,addressln2,addressln3,subdistrict,district,       " + 
        " city,country,postcode,region,telephone,email,thgroup,thcomment,throuteformat, " + 
        " plandelivery,naturalloss,mapaddress,orbitsource,tflow)                        " + 
        " select orgcode,site,depot,spcarea,thtype,thbutype,thcode,thcodealt,vatcode,   " +
        " thname,thnamealt,thnamealt as thnameint, addressln1,addressln2,addressln3,subdistrict,district,       " + 
        " city,country,postcode,region,telephone,email,thgroup,thcomment,throuteformat, " +  
        " plandelivery,naturalloss,mapaddress,orbitsource,'IO' tflow                         " + 
        " from xm_xithirdparty where rowid = @rowid                                     " ;
        string sqlInterface_crunching_thirdparty_update = "" +
        " update t                                                                                                  " + 
        "    set t.orgcode = s.orgcode,t.site = s.site,t.depot = s.depot,t.spcarea = s.spcarea,t.thtype = s.thtype, " +
        "    t.thbutype = s.thbutype,t.thcode = s.thcode,t.thcodealt = s.thcodealt,t.vatcode = s.vatcode,           " +
        "    t.thname = s.thname,t.thnamealt = s.thnamealt,t.addressln1 = s.addressln1,t.addressln2 = s.addressln2, " +
        "    t.addressln3 = s.addressln3,t.subdistrict = s.subdistrict,t.district = s.district,t.city = s.city,     " +
        "    t.country = s.country,t.postcode = s.postcode,t.region = s.region,t.telephone = s.telephone,           " +
        "    t.email = s.email,t.thgroup = s.thgroup,t.thcomment = s.thcomment,t.throuteformat = s.throuteformat,   " +
        "    t.plandelivery = s.plandelivery,t.naturalloss = s.naturalloss,t.mapaddress = s.mapaddress,             " +
        "    t.orbitsource = s.orbitsource,t.tflow = 'IO',t.datemodify = SYSDATETIMEOFFSET(),                       " +
        "    t.accnmodify = s.orbitsource                                                                           " +
        " from wm_thparty t                                                                                         " +
        " left join xm_xithirdparty s                                                                               " +
        "   on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode             " +
        " where rowid = @rowid                                                                                      " ;
        string sqlInterface_crunching_thirdparty_delete = ""+
        " delete t from wm_thparty t left join xm_xithirdparty s                                          " +
        "     on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode " +
        " where s.rowid = @rowid                                                                          ";
        string sqlInterface_crunching_thirdparty_cancel = "" +
         " update t set tflow='XX',datemodify=SYSDATETIMEOFFSET(),accnmodify= s.orbitsource from wm_thparty t left join xm_xithirdparty s   " +
         "     on  t.orgcode = s.orgcode and t.site = s.site and t.depot = s.depot and t.thcode = s.thcode " +
         " where s.rowid = @rowid    ";

    }
}