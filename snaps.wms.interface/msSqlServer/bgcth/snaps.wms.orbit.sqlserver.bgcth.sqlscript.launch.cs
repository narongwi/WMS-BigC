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
        //Receipt
        // string sqlInterface_launch_receipt = "" + 
        // " select orbitsite	MR3CODSIT,dateops MR3DATMVT,accnops	MR3CODREC,'USER DEFAULT' MR3LIBREC,dateops	MR3DATREC, " + 
        // " 		inorder	MR3NCDEFO,'1' MR3BDLFOU,'0' MR3NUMGPR,inagrn MR3NUMROC,article	MR3CPROIN,qtysku	       " + 
        // " 		MR3UVCREC,qtyweight	MR3PDSREC,intype MR3TYPOR,''	MR3CODCLI,dbo.lpad(pv,2,'0')  MR3ARPROM,dbo.lpad(lv,3,'0') MR3ILOGIS,           " + 
        // " 		thcode MR3FOURN,'0'	MR3STASTK,site	MR3DONORD,inpromo	MR3NDOS,''	MR3DABL,'1'	MR3TYPMVT,         " + 
        // " 		orbitdepot	MR3DEPOT,inrefno MR3CINCDE, inrefln	MR3NOLIGN,'' MR3PRVE,'0' MR3NLIGP,ingrno MR3NSEQOR,    " + 
        // " 		dateops MR3DATACT, ROWID ,qtysku MR3UMCREC                                                                       " + 
        // " from xm_xoreceipt where xaction = 'WC'                                                                      " ;

        //string sqlInterface_launch_receipt = ""+
        //"  select orbitsite	MR3CODSIT,getdate() MR3DATMVT,accnops	MR3CODREC, ac.accnname MR3LIBREC,dateops MR3DATREC,                " + 
        //" ix.inorder MR3NCDEFO,ib.invno MR3BDLFOU,'0' MR3NUMGPR,inagrn MR3NUMROC,article	MR3CPROIN,qtysku MR3UVCREC,                " + 
        //" qtyweight	MR3PDSREC,ix.intype MR3TYPOR,(case when ix.intype = '2' then ix.thcode else '' end) MR3CODCLI,                 " + 
        //" dbo.lpad(pv,2,'0')  MR3ARPROM,dbo.lpad(lv,3,'0') MR3ILOGIS,ix.thcode MR3FOURN,                                             " + 
        //" (case when ix.intype = '2' then '1' else '0' end)	MR3STASTK,ix.site	MR3DONORD,ix.inpromo	MR3NDOS,                   " + 
        //" ''	MR3DABL,'1'	MR3TYPMVT,ix.orbitdepot	MR3DEPOT,ix.inrefno MR3CINCDE, ix.inln	MR3NOLIGN,'' MR3PRVE,                      " + 
        //" '0' MR3NLIGP,ingrno MR3NSEQOR, dateops MR3DATACT, ROWID ,qtysku MR3UMCREC from xm_xoreceipt ix, wm_inbound ib, wm_accn ac  " + 
        //" where xaction = 'WC' and ix.orgcode = ib.orgcode and ix.site = ib.site and ix.depot = ib.depot and ix.inorder = ib.inorder " + 
        //" and ix.accnops = ac.accncode and orbitsite is not null";
        // 

        // $$$ ingrno MR3NSEQOR

        string sqlInterface_launch_receipt =
            @"select ix.orbitsite MR3CODSIT,getdate() MR3DATMVT,'000' MR3CODREC,'USER DEFAULT' MR3LIBREC,CAST(ix.dateops AS datetime) MR3DATREC,
                ix.inorder MR3NCDEFO,ib.invno MR3BDLFOU,'0' MR3NUMGPR,ib.inflag MR3NUMROC,ix.article MR3CPROIN,sum(ix.qtysku) MR3UVCREC,
                sum(ix.qtyweight) MR3PDSREC, (case when ix.intype = 'RC' and ix.insubtype='RC' then '1'when ix.intype = 'RC' and ix.insubtype='RW' then '2'
           when ix.intype = 'FW' and ix.insubtype='FW' then '3' when ix.intype = 'XD' and ix.insubtype='XD' then '4' else '0' end) MR3TYPOR,
                (case when ix.intype = 'RC' and ix.insubtype='RW' then ix.thcode else NULL end) MR3CODCLI,
                dbo.lpad(pv,2,'0') MR3ARPROM,dbo.lpad(lv,3,'0') MR3ILOGIS, ix.thcode MR3FOURN,
                 (case when ix.intype = 'RC' and ix.insubtype='RW' then '1' else '0' end) MR3STASTK,ix.site	MR3DONORD,ix.inpromo MR3NDOS,
                null MR3DABL,'1' MR3TYPMVT,ix.orbitdepot MR3DEPOT,ix.inrefno MR3CINCDE, ix.inln	MR3NOLIGN,null MR3PRVE,
                '0' MR3NLIGP,ix.ingrno MR3NSEQOR, CAST(ix.dateops AS datetime) MR3DATACT, ix.ROWID ,sum(ix.qtysku) MR3UMCREC
            from xm_xoreceipt ix, wm_inbound ib  where xaction = 'WC'
            and ix.orgcode = ib.orgcode and ix.site = ib.site and ix.depot = ib.depot and ix.inorder = ib.inorder  and orbitsite is not null
            group by ix.orbitsite, CAST(ix.dateops AS datetime), ix.accnops,ib.inflag,
                     ix.inorder, ib.invno, ix.ingrno, ix.article, ix.intype,ix.insubtype,
                     ix.intype, ix.pv, ix.lv, ix.thcode, ix.site, ix.inpromo,
                     ix.orbitdepot, ix.inrefno, ix.inln,ix.ingrno, ix.ROWID order by ib.inflag";

        string sqlInterface_launch_receipt_insert = "" + 
        "  INSERT INTO N3_MVTRE (                                                                                                       " + 
        "  MR3CODSIT,MR3DATMVT,MR3CODREC,MR3LIBREC,MR3DATREC,MR3NCDEFO,MR3BDLFOU,MR3NUMGPR,MR3NUMROC,MR3CPROIN,MR3UVCREC,               " + 
        "  MR3PDSREC,MR3TYPOR,MR3CODCLI,MR3ARPROM,MR3ILOGIS,MR3FOURN,MR3STASTK,MR3DONORD,MR3NDOS,MR3DABL,MR3TYPMVT,MR3DEPOT,            " + 
        "  MR3CINCDE,MR3NOLIGN,MR3PRVE,MR3NLIGP,MR3NSEQOR,MR3DATACT,MR3UMCREC ) VALUES (                                                " + 
        " :MR3CODSIT,:MR3DATMVT,:MR3CODREC,:MR3LIBREC,:MR3DATREC,:MR3NCDEFO,:MR3BDLFOU,:MR3NUMGPR,:MR3NUMROC,:MR3CPROIN,:MR3UVCREC,     " + 
        " :MR3PDSREC,:MR3TYPOR,:MR3CODCLI,:MR3ARPROM,:MR3ILOGIS,:MR3FOURN,:MR3STASTK,:MR3DONORD,:MR3NDOS,:MR3DABL,:MR3TYPMVT,:MR3DEPOT, " +  
        " :MR3CINCDE,:MR3NOLIGN,:MR3PRVE,:MR3NLIGP,:MR3NSEQOR,:MR3DATACT,:MR3UMCREC )                                                   " ;
        string sqlInterface_launch_receipt_grno = "select next value for seq_recno recno";
        string sqlInterface_launch_receipt_clear = "" +
        " update xm_xoreceipt set xaction = 1,xmsg = 'Successed', xmodify = sysdatetimeoffset() where rowid = @rowid ";
        string sqlInterface_launch_receipt_error = "" + 
        " update xm_xoreceipt set xaction = 2, xmodify = sysdatetimeoffset(), xmsg = @xmsg where rowid = @rowid ";

        //Correction 
        string sqlInterface_launch_correction = "" +
        // " select orbitsite MI3CODSIT,dateops MI3DATMVT,article MI3CPROIN,codeops MI3CECART,qtysku MI3QTEUVC,accnops MI3CODINV," + 
        // " ingrno MI3NUMORC, thcode	MI3CODCLI,qtyweight	MI3POIDS,null MI3SECTIO," + 
        // " case when codeops = '00'and qtysku > 0 then 1 else 0 end MI3STASTK,site MI3DONORD,inpromo MI3NDOS, " +
        // " null MI3DABL,orbitdepot MI3DEPOT,dbo.lpad(lv,3,'0')  MI3ILOGIS,null MI3NOLIGN,null MI3NLIGP,null MI3DATACT,ROWID,qtysku MI3QTEUMC " +
        // " from xm_xocorrection where xaction = 'WC' and orbitsite is not null ";
        //" select orbitsite MI3CODSIT,cast(x.dateops as date) MI3DATMVT,article MI3CPROIN,codeops MI3CECART,qtysku MI3QTEUVC,accnops MI3CODINV, " + 
        //" (select top 1 inorder from xm_xoreceipt r where x.orgcode = r.orgcode and x.site = r.site and x.depot = r.depot and x.inagrn = r.inagrn) MI3NUMORC, " + 
        //" thcode	MI3CODCLI,qtyweight	MI3POIDS,null MI3SECTIO," + 
        //" case when codeops = '00'and qtysku > 0 then 1 else 0 end MI3STASTK,site MI3DONORD,inpromo MI3NDOS," +
        //" null MI3DABL,orbitdepot MI3DEPOT,dbo.lpad(lv,3,'0')  MI3ILOGIS,ingrno MI3NOLIGN,null MI3NLIGP,cast(x.dateops as date) MI3DATACT,ROWID,qtysku MI3QTEUMC" + 
        //" from xm_xocorrection x where x.xaction = 'WC' and isnull(orbitsite,'') != '' " ;
        @"select orbitsite MI3CODSIT,convert(datetime2,x.dateops) MI3DATMVT,article MI3CPROIN,codeops MI3CECART,qtysku MI3QTEUVC,accnops MI3CODINV,  
        (case when x.codeops not in ('78', '79') then null else x.inagrn end) MI3NUMORC,(case when x.codeops not in ('78', '79') then null else x.thcode end) MI3CODCLI,
        qtyweight MI3POIDS,null MI3SECTIO, 
        case when (codeops = '00' and qtysku > 0) then 1 /* block   */  
             when (codeops = '00' and qtysku < 0) then 0 /* Unblock */
             when (inreftype='BULK'             ) then 1 /* block   */
             else 0 end MI3STASTK,
        site MI3DONORD,inpromo MI3NDOS,
        null MI3DABL,orbitdepot MI3DEPOT,dbo.lpad(lv,3,'0') MI3ILOGIS,(case when x.codeops not in ('78', '79') then null else (select top 1 inln from xm_xoreceipt r 
        where x.orgcode = r.orgcode and x.site = r.site and x.depot = r.depot and (x.inagrn = r.inorder or x.inrefno = r.inorder)
        and x.article = r.article and x.lv = r.lv) end) MI3NOLIGN,null MI3NLIGP,cast(x.dateops as date) MI3DATACT,ROWID,qtysku MI3QTEUMC
        from xm_xocorrection x where x.xaction = 'WC' and isnull(orbitsite,'') != ''";
        
        string sqlInterface_launch_correction_insert = "" +
        " INSERT INTO N3_MVTIN ( MI3CODSIT,MI3DATMVT,MI3CPROIN,MI3CECART,MI3QTEUVC,MI3CODINV,MI3NUMORC,MI3CODCLI,MI3POIDS "+ 
        " ,MI3SECTIO,MI3STASTK,MI3DONORD,MI3NDOS,MI3DABL,MI3DEPOT,MI3ILOGIS,MI3NOLIGN,MI3NLIGP,MI3DATACT,MI3QTEUMC ) VALUES ( 	  " +
        "  :MI3CODSIT,:MI3DATMVT,:MI3CPROIN,:MI3CECART,:MI3QTEUVC,:MI3CODINV,:MI3NUMORC,:MI3CODCLI,:MI3POIDS,:MI3SECTIO    "+ 
        " ,:MI3STASTK,:MI3DONORD,:MI3NDOS,:MI3DABL,:MI3DEPOT,:MI3ILOGIS,:MI3NOLIGN,:MI3NLIGP,:MI3DATACT, :MI3QTEUMC ) ";
        string sqlInterface_launch_correction_clear = "" + 
        " update xm_xocorrection set xaction = 1, xmodify = sysdatetimeoffset() where rowid = @rowid ";
        string sqlInterface_launch_correction_error = "" +
        " update xm_xocorrection set xaction = 2, xmodify = sysdatetimeoffset(), xmsg = @xmsg where rowid = @rowid ";

        //Delivery 
        //string sqlInterface_launch_delivery = "" + 
        //" select orbitsource ME3CODSIT,ouorder ME3NUMCDE,0 ME3NUMCTL,dateops ME3DATLIV,article ME3CPROIN,qtysku ME3QTEEXP " + 
        //" 		,qtysku ME3QTEALIV, qtysku ME3QTECDE,thtype ME3TYPOL,thcode ME3CODCLI,ingrno ME3NUMORL,null	ME3CPROCD  " + 
        //" 		,accnops ME3CODEXP,oudnno ME3NSEQBL,qtyweight ME3PDSEXP,null ME3STASTK,oupromo	ME3NDOS,site ME3DONORD " + 
        //" 		,'1' ME3TYPMVT,'1' ME3FINCDE,transportno ME3NSETRA,orbitdepot ME3DEPOT,dbo.lpad(lv,3,'0')  ME3ILOGIS,routeops	ME3CTOURN,     " +
        //" 		ouline	ME3NLIGOL,null ME3NCOLIS,ourefno ME3CINCDE,ourefln	ME3NOLIGN,huno ME3SSCC,null	ME3ILOGCD,     " +
        //" 		'0'	ME3NLIGP,null ME3DATEXP,null ME3DATACT,ROWID                                                       " +
        //" from xm_xodelivery where xaction = '0'                                                                         " ;
        //string sqlInterface_launch_delivery =
        //@"select ox.orbitsite ME3CODSIT, ox.ouorder ME3NUMCDE,ox.ourefno ME3NUMCTL, ox.dateops ME3DATLIV,
        //    ox.article ME3CPROIN, ox.qtysku ME3QTEEXP, ox.qtysku ME3QTEALIV, ox.qtysku ME3QTECDE,
        //    ox.thtype ME3TYPOL, ox.thcode ME3CODCLI, ob.oudono ME3NUMORL, null ME3CPROCD,
        //    ox.accnops ME3CODEXP, ox.oudnno ME3NSEQBL, CAST(ox.qtyweight as decimal(9,3)) ME3PDSEXP, null ME3STASTK,
        //    ox.oupromo ME3NDOS, ox.site ME3DONORD, '1' ME3TYPMVT, '1' ME3FINCDE,
        //    ox.transportno ME3NSETRA, ox.orbitdepot ME3DEPOT, dbo.lpad(ox.lv,3,'0') ME3ILOGIS,
        //    ox.routeops ME3CTOURN,ob.ouseq ME3NLIGOL, ox.qtysku ME3NCOLIS,ob.ouln ME3CINCDE, ox.ourefln ME3NOLIGN,
        //    ox.huno ME3SSCC, null ME3ILOGCD, '0' ME3NLIGP,FORMAT(ox.dateops, 'dd/MM/yyyy') ME3DATEXP,
        //    FORMAT(ox.dateops, 'dd/MM/yyyy') ME3DATACT,'4001' ME3CODTRA, null ME3TIEEMB, null ME3UMCEXP, '1' ME3NLIGMAG,ROWID
        //from xm_xodelivery ox , wm_outbouln ob where ox.xaction = 'WC'
        //and ox.orgcode = ob.orgcode and ox.site = ob.site and ox.depot = ob.depot and ox.ouorder = ob.ouorder";
        // string sqlInterface_launch_delivery =
        //@"select ox.orbitsite ME3CODSIT, ox.ouorder ME3NUMCDE,ob.ourefno ME3NUMCTL, CAST(ox.dateops AS datetime) ME3DATLIV,
        //     ox.article ME3CPROIN, ox.qtysku ME3QTEEXP, ox.qtysku ME3QTEALIV, ox.qtysku ME3QTECDE,
        //     (case when oh.outype = 'DL' and oh.ousubtype ='DV' then -1 when oh.outype = 'DL' and oh.ousubtype ='DC' then 2 else null end) ME3TYPOL,
        //  ox.thcode ME3CODCLI, ob.oudono ME3NUMORL, null ME3CPROCD,ox.accnops ME3CODEXP,
        //     max(ox.oudnno) over (partition by ox.ouorder order by ox.ouorder) ME3NSEQBL,
        //     CAST(ox.qtyweight as decimal(9,3)) ME3PDSEXP, null ME3STASTK,
        //     0 ME3NDOS, ox.site ME3DONORD, '1' ME3TYPMVT, '1' ME3FINCDE,
        //     isnull(ox.transportno,0) ME3NSETRA, ox.orbitdepot ME3DEPOT, dbo.lpad(ox.lv,3,'0') ME3ILOGIS,
        //     ox.routeops ME3CTOURN, ob.ouseq ME3NLIGOL, ox.qtysku ME3NCOLIS,ob.ouln ME3CINCDE,ob.ourefln ME3NOLIGN,
        //     ox.huno ME3SSCC, null ME3ILOGCD, '0' ME3NLIGP,CAST(ox.dateops AS datetime) ME3DATEXP,
        //     CAST(ox.dateops AS datetime) ME3DATACT,'4001' ME3CODTRA, null ME3TIEEMB, ox.qtysku ME3UMCEXP, '1' ME3NLIGMAG,ROWID
        // from xm_xodelivery ox ,wm_outbound oh, wm_outbouln ob  where ox.xaction = 'WC'
        // and ox.orgcode = oh.orgcode and ox.site = oh.site and ox.depot = oh.depot and ox.ouorder = oh.ouorder
        // and ox.orgcode = ob.orgcode and ox.site = ob.site and ox.depot = ob.depot and ox.ouorder = ob.ouorder";

        //isnull(ob.ourefno,'0') ME3NUMCTL
        //string sqlInterface_launch_delivery =
        //   @"select ox.orbitsite ME3CODSIT, ox.ouorder ME3NUMCDE,(case when isnull(ob.ourefno,'')='' then '0' else ob.ourefno end) ME3NUMCTL, 
        //         CAST(ox.dateops AS datetime) ME3DATLIV,ox.article ME3CPROIN, ox.qtysku ME3QTEEXP, ob.qtysku ME3QTEALIV,ob.qtysku ME3QTECDE,
        //         (case when oh.outype = 'DL' and oh.ousubtype ='DV' then '1' when oh.outype = 'DL' and oh.ousubtype ='DC' then '2' else null end) ME3TYPOL,
        //         ox.thcode ME3CODCLI, ob.oudono ME3NUMORL, null ME3CPROCD,ox.accnops ME3CODEXP, max(ox.oudnno) over (partition by ox.ouorder order by ox.ouorder) ME3NSEQBL,
        //         CAST(ox.qtyweight as decimal(9,3)) ME3PDSEXP, null ME3STASTK, 0 ME3NDOS, ox.site ME3DONORD, '1' ME3TYPMVT, 
        //         CASE WHEN ob.qtysku = ob.qtyskudel THEN '1' ELSE '0' END AS ME3FINCDE,
        //         isnull(ox.transportno,0) ME3NSETRA, ox.orbitdepot ME3DEPOT, dbo.lpad(ox.lv,3,'0') ME3ILOGIS,
        //         CASE WHEN oh.outype = 'DL' and oh.ousubtype ='DV' THEN '-1' ELSE ox.routeops END AS ME3CTOURN, ob.ouseq ME3NLIGOL, ox.qtysku ME3NCOLIS,ob.ouln ME3CINCDE,ob.ourefln ME3NOLIGN,
        //         ox.huno ME3SSCC, null ME3ILOGCD, '0' ME3NLIGP,CAST(ox.dateops AS datetime) ME3DATEXP,
        //         CAST(ox.dateops AS datetime) ME3DATACT,'4001' ME3CODTRA, null ME3TIEEMB, ox.qtysku ME3UMCEXP, '1' ME3NLIGMAG,ROWID
        //     from xm_xodelivery ox ,wm_outbound oh, wm_outbouln ob  where ox.xaction = 'WC'
        //     and ox.orgcode = oh.orgcode and ox.site = oh.site and ox.depot = oh.depot and ox.ouorder = oh.ouorder
        //     and ox.orgcode = ob.orgcode and ox.site = ob.site and ox.depot = ob.depot and ox.ouorder = ob.ouorder and ox.article = ob.article and ox.pv = ob.pv and ox.lv = ob.lv";

        // summary interface 
        string sqlInterface_launch_delivery =
          @"SELECT ME3CODSIT,ME3NUMCDE,ME3NUMCTL,
	        MAX(ME3DATLIV) as ME3DATLIV,ME3CPROIN,
	        SUM(ME3QTEEXP) as ME3QTEEXP,
	        SUM(ME3QTEALIV)as ME3QTEALIV,
	        SUM(ME3QTECDE) as ME3QTECDE,ME3TYPOL,ME3CODCLI,ME3NUMORL,ME3CPROCD,ME3CODEXP,ME3NSEQBL,
	        SUM(ME3PDSEXP) as ME3PDSEXP,ME3STASTK,ME3NDOS,ME3DONORD,ME3TYPMVT,ME3FINCDE,ME3NSETRA,ME3DEPOT,ME3ILOGIS,ME3CTOURN,ME3NLIGOL,
	        SUM(ME3NCOLIS) AS ME3NCOLIS,ME3CINCDE,ME3NOLIGN,ME3SSCC,ME3ILOGCD,ME3NLIGP,
	        MAX(ME3DATEXP) AS ME3DATEXP,
	        MAX(ME3DATACT) AS ME3DATACT,ME3CODTRA,ME3TIEEMB,
	        SUM(ME3UMCEXP) AS ME3UMCEXP,ME3NLIGMAG,	0 ROWID
            FROM (
            select ox.orbitsite ME3CODSIT, ox.ouorder ME3NUMCDE,(case when isnull(ob.ourefno,'')='' then '0' else ob.ourefno end) ME3NUMCTL, 
                 CAST(ox.dateops AS datetime) ME3DATLIV,ox.article ME3CPROIN, ox.qtysku ME3QTEEXP, ob.qtysku ME3QTEALIV,ob.qtysku ME3QTECDE,
                 (case when oh.outype = 'DL' and oh.ousubtype ='DV' then '1' when oh.outype = 'DL' and oh.ousubtype ='DC' then '2' else null end) ME3TYPOL,
                 ox.thcode ME3CODCLI, ob.oudono ME3NUMORL, null ME3CPROCD,ox.accnops ME3CODEXP, max(ox.oudnno) over (partition by ox.ouorder order by ox.ouorder) ME3NSEQBL,
                 CAST(ox.qtyweight as decimal(9,3)) ME3PDSEXP, null ME3STASTK, 0 ME3NDOS, ox.site ME3DONORD, '1' ME3TYPMVT, 
                 CASE WHEN ob.qtysku = ob.qtyskudel THEN '1' ELSE '0' END AS ME3FINCDE,
                 isnull(ox.transportno,0) ME3NSETRA, ox.orbitdepot ME3DEPOT, dbo.lpad(ox.lv,3,'0') ME3ILOGIS,
                 CASE WHEN oh.outype = 'DL' and oh.ousubtype ='DV' THEN '-1' ELSE ox.routeops END AS ME3CTOURN, ob.ouseq ME3NLIGOL, ox.qtysku ME3NCOLIS,ob.ouln ME3CINCDE,ob.ourefln ME3NOLIGN,
                 ox.huno ME3SSCC, null ME3ILOGCD, '0' ME3NLIGP,CAST(ox.dateops AS datetime) ME3DATEXP,
                 CAST(ox.dateops AS datetime) ME3DATACT,'4001' ME3CODTRA, null ME3TIEEMB, ox.qtysku ME3UMCEXP, '1' ME3NLIGMAG,ROWID
             from xm_xodelivery ox ,wm_outbound oh, wm_outbouln ob  where ox.xaction = 'WC'
             and ox.orgcode = oh.orgcode and ox.site = oh.site and ox.depot = oh.depot and ox.ouorder = oh.ouorder
             and ox.orgcode = ob.orgcode and ox.site = ob.site and ox.depot = ob.depot and ox.ouorder = ob.ouorder and ox.article = ob.article and ox.pv = ob.pv and ox.lv = ob.lv
            ) s GROUP BY ME3CODSIT,ME3NUMCDE,ME3NUMCTL,ME3CPROIN,ME3TYPOL,ME3CODCLI,ME3NUMORL,ME3CPROCD,ME3CODEXP,ME3NSEQBL,ME3STASTK,ME3NDOS,ME3DONORD,ME3TYPMVT,ME3FINCDE,ME3NSETRA,
	            ME3DEPOT,ME3ILOGIS,ME3CTOURN,ME3NLIGOL,ME3CINCDE,ME3NOLIGN,ME3SSCC,ME3ILOGCD,ME3NLIGP,ME3CODTRA,ME3TIEEMB,ME3NLIGMAG";

        string sqlInterface_launch_delivery_insert = "" + 
        " INSERT INTO N3_MVTEX ( ME3CODSIT,ME3NUMCDE,ME3NUMCTL,ME3DATLIV,ME3CPROIN,ME3QTEEXP,ME3QTEALIV,ME3QTECDE	        " +
        " ,ME3TYPOL,ME3CODCLI,ME3NUMORL,ME3CPROCD,ME3CODEXP,ME3NSEQBL,ME3PDSEXP,ME3STASTK,ME3NDOS,ME3DONORD,ME3TYPMVT       " +
        " ,ME3FINCDE,ME3NSETRA,ME3DEPOT,ME3ILOGIS,ME3CTOURN,ME3NLIGOL,ME3NCOLIS,ME3CINCDE,ME3NOLIGN,ME3SSCC,ME3ILOGCD       " +
        " ,ME3NLIGP,ME3DATEXP,ME3DATACT,ME3CODTRA,ME3NLIGMAG,ME3UMCEXP ) VALUES ( :ME3CODSIT,:ME3NUMCDE,:ME3NUMCTL,:ME3DATLIV,:ME3CPROIN,:ME3QTEEXP, " + 
        "  :ME3QTEALIV,:ME3QTECDE,:ME3TYPOL,:ME3CODCLI,:ME3NUMORL,:ME3CPROCD,:ME3CODEXP,:ME3NSEQBL,:ME3PDSEXP,:ME3STASTK	" +
        " ,:ME3NDOS,:ME3DONORD,:ME3TYPMVT,:ME3FINCDE,:ME3NSETRA,:ME3DEPOT,:ME3ILOGIS,:ME3CTOURN,:ME3NLIGOL,:ME3NCOLIS		" +
        " ,:ME3CINCDE,:ME3NOLIGN,:ME3SSCC,:ME3ILOGCD,:ME3NLIGP,:ME3DATEXP,:ME3DATACT,:ME3CODTRA,:ME3NLIGMAG,:ME3UMCEXP )    ";

        //string sqlInterface_launch_delivery_clear = "" + 
        //" update xm_xodelivery set xaction = 1, xmodify = sysdatetimeoffset() where rowid = @rowid ";
        //string sqlInterface_launch_delivery_error = "" +
        //" update xm_xodelivery set xaction = 2, xmodify = sysdatetimeoffset(), xmsg = @xmsg where rowid = @rowid ";

        string sqlInterface_launch_delivery_clear = @"update xm_xodelivery set xaction = 1, xmodify = sysdatetimeoffset()  where site = @site and oudnno = @oudnno and ouorder =@ouorder and ouln =@ouln and huno = @huno and article = @article and lv = cast(@lv as int) and xaction = 'WC'";
        string sqlInterface_launch_delivery_error = @"update xm_xodelivery set xaction = 2, xmodify = sysdatetimeoffset(), xmsg = @xmsg  where site = @site and oudnno = @oudnno and ouorder =@ouorder and ouln =@ouln and huno = @huno and article = @article and lv = cast(@lv as int) and xaction = 'WC'";

        string sqlinterface_launch_block = @" select orbitsite MI3CODSIT,cast(x.xcreate as datetime) MI3DATMVT,article MI3CPROIN,
                '00' MI3CECART,qtysku MI3QTEUVC,x.accnmodify MI3CODINV, 
                null MI3NUMORC, thcode	MI3CODCLI,qtyweight	MI3POIDS,null MI3SECTIO, 1 MI3STASTK,x.site MI3DONORD,null MI3NDOS,
                null MI3DABL,orbitdepot MI3DEPOT,dbo.lpad(lv,3,'0')  MI3ILOGIS,null  MI3NOLIGN,null MI3NLIGP,cast(x.xcreate as date) MI3DATACT,ROWID,qtysku MI3QTEUMC, x.opstype,rowid
        from xm_xoblock x left join (
            select  o.obitcode orbitcode, o.orgcode, o.site, o.depot, 
                max(case when bncode = 'SITE' then bnflex3 else '' end) orbitsite, 
                max(case when bncode = 'DEPOT' then bnflex3 else '' end) orbitdepot
            from wm_orbit o,wm_binary b 
            where apps = o.obitcode and o.orgcode = b.orgcode and o.site = b.site and o.depot = b.depot 
            and bncode in ('SITE','DEPOT','CODE') and o.tflow = 'IO' and orbitvld = '1' 
            and isnull(enddate,dateadd(DAY,1,sysdatetimeoffset())) > SYSDATETIMEOFFSET() 
            group by o.obitcode, o.orgcode, o.site, o.depot ) o
        on o.orgcode = x.orgcode and o.site = x.site and o.depot = x.depot
        where x.xaction = 'WC'";
        string sqlinterface_launch_block_insert = "" + 
        " INSERT INTO N3_MVTIN ( MI3CODSIT,MI3DATMVT,MI3CPROIN,MI3CECART,MI3QTEUVC,MI3CODINV,MI3NUMORC,MI3CODCLI,MI3POIDS "+ 
        " ,MI3SECTIO,MI3STASTK,MI3DONORD,MI3NDOS,MI3DABL,MI3DEPOT,MI3ILOGIS,MI3NOLIGN,MI3NLIGP,MI3DATACT,MI3QTEUMC ) VALUES ( 	  " +
        "  :MI3CODSIT,:MI3DATMVT,:MI3CPROIN,:MI3CECART,:MI3QTEUVC,:MI3CODINV,:MI3NUMORC,:MI3CODCLI,:MI3POIDS,:MI3SECTIO    "+ 
        " ,:MI3STASTK,:MI3DONORD,:MI3NDOS,:MI3DABL,:MI3DEPOT,:MI3ILOGIS,:MI3NOLIGN,:MI3NLIGP,:MI3DATACT,:MI3QTEUMC ) ";

        string sqlinterface_launch_block_clear = " update xm_xoblock set xaction = 1, xmodify = sysdatetimeoffset() where rowid = @rowid ";
        string sqlinterface_launch_block_error = " update xm_xoblock set xaction = 2, xmodify = sysdatetimeoffset(), xmsg = @xmsg where rowid = @rowid ";


        string sqlinterface_launch_imstock =
            @"select wd.orbitsite IS3CODSIT, ps.article IS3CPROIN, sum(ps.qtysku) IS3STKUVC, 
		            (sum(ps.qtysku)*pm.skugrossweight) IS3STKPOI,
       	            ps.site IS3DONORD, 0 IS3STPALE, ps.depot IS3DEPOT, REPLACE(STR(ps.lv, 3), ' ','0') IS3ILOGIS
            from vi_stock ps, wm_product pm, wm_depot wd
            where ps.orgcode = wd.orgcode and ps.site = wd.sitecode and ps.depot = wd.depotcode 
            and wd.tflow = 'IO' and wd.orbitoperate = '1'
            and ps.orgcode = pm.orgcode and ps.site = pm.site and ps.depot = pm.depot and ps.article = pm.article and ps.lv = pm.lv 
            and not exists (select 1 from vi_stock_block sb where sb.orgcode = ps.orgcode and sb.site = ps.site 
					            and sb.depot = ps.depot and sb.article = ps.article and sb.lv = ps.lv and sb.loccode = ps.loccode)
            group by wd.orbitsite, ps.article, ps.site, ps.depot, ps.lv, pm.skugrossweight
            union
            select wd.orbitsite IS3CODSIT, ps.article IS3CPROIN, sum(ps.qtysku) IS3STKUVC, 
		            (sum(ps.qtysku)*pm.skugrossweight) IS3STKPOI,
       	            ps.site IS3DONORD, 1 IS3STPALE, ps.depot IS3DEPOT, REPLACE(STR(ps.lv, 3), ' ','0') IS3ILOGIS
            from vi_stock_block ps, wm_product pm, wm_depot wd
            where ps.orgcode = wd.orgcode and ps.site = wd.sitecode and ps.depot = wd.depotcode 
            and wd.tflow = 'IO' and wd.orbitoperate = '1'
            and ps.orgcode = pm.orgcode and ps.site = pm.site and ps.depot = pm.depot and ps.article = pm.article and ps.lv = pm.lv 
            group by wd.orbitsite, ps.article, ps.site, ps.depot, ps.lv, pm.skugrossweight";

        string sqlinterface_launch_imstock_insert =
            @"INSERT INTO n3_imstk (IS3CODSIT, IS3CPROIN, IS3STKUVC, IS3STKPOI, IS3DONORD, IS3STPALE, IS3DEPOT, IS3ILOGIS) VALUES(:1,:2,:3,:4,:5,:6,:7,:8)";
    }
}