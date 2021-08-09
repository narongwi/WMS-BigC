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
        string sqlInterface_landing_orbitsouce = "" + 
        " select o.orbitsite,o.orbitdepot from wm_inbound i left join (select o.obitcode orbitcode, o.orgcode, o.site, o.depot, max(case when bncode = 'SITE' then bnflex3 else '' end) orbitsite, " +
        " max(case when bncode = 'DEPOT' then bnflex3 else '' end) orbitdepot from wm_orbit o,wm_binary b where apps = o.obitcode and o.orgcode = b.orgcode and o.site = b.site and o.depot = b.depot " +
        " and bncode in ('SITE','DEPOT') and o.tflow = 'IO' and orbitvld = '1' and isnull(enddate,dateadd(DAY,1,sysdatetimeoffset())) > SYSDATETIMEOFFSET() group by o.obitcode, o.orgcode, o.site, o.depot ) o " +
        " on i.orgcode = o.orgcode and i.depot = o.depot and i.site = o.site and i.orbitsource = o.orbitcode where i.orgcode = @orgcode and i.site = @site and o.depot = @depot and i.inorder = @inorder ";
        //Confirm Receipt
        public string sqlInterface_landing_receipt_confirm = @"
        update xm_xoreceipt set xmodify = sysdatetimeoffset(),ingrno=@ingrno , xaction = 'WC' where orgcode = @orgcode and site = @site and depot = @depot and inorder = @inorder and xaction = 'WA' ";
        string sqlInterface_landing_receipt = "" + 
        " insert into xm_xoreceipt (orgcode,site,depot,thcode,intype,insubtype,inorder,inln,ingrno,      " +
        " inrefno,inrefln,barcode,article,pv,lv,unitops,qtysku,qtypu,qtyweight,qtyvolume,qtynaturalloss, " +
        " dateops,accnops,dateexp,datemfg,batchno,lotmfg,serialno,huno,inpromo,orbitsource,xaction,      " +
        " xcreate,rowid,orbitsite,orbitdepot,inagrn ) values ( @orgcode,@site,@depot,@thcode,@intype,@insubtype,@inorder,@inln,      " +
        " @ingrno,@inrefno,@inrefln,@barcode,@article,@pv,@lv,@unitops,@qtysku,@qtypu,@qtyweight,        " +
        " @qtyvolume,@qtynaturalloss,@dateops,@accnops,@dateexp,@datemfg,@batchno,@lotmfg,@serialno,     " +
        " @huno,@inpromo,@orbitsource,'WA',sysdatetimeoffset(), next value for seq_xoreceipt,@orbitsite,@orbitdepot,@inagrn )       " ;

        //Correction 
        string sqlInterface_landing_correction = "" + 
        " insert into xm_correction (orgcode,site,depot,dateops,accnops,seqops,codeops,typeops,thcode,     " +
        " article,pv,lv,unitops,qtysku,qtyweight,inreftype,inrefno,ingrno,inpromo,reason,xaction,xcreate,  " +
        " rowid,orbitsite,orbitdepot) values (orgcode,@site,@depot,@dateops,@accnops,@seqops,@codeops,@typeops,@thcode,@article," +
        " @pv,@lv,@unitops,@qtysku,@qtyweight,@inreftype,@inrefno,@ingrno,@inpromo,@reason,'WC'," + 
        " sysdatetimeoffset(),next value for seq_xocorrection,@orbitsite,@orbitdepot  ) ";
        
        //Delivery 
        string sqlInterface_landing_delivery = "" + 
        " insert into xm_xodelivery (orgcode,site,depot,dateops,routeops,transportno,thcode,thtype,        " +
        " dropship,ouorder,ouline,ourefno,ourefln,oudnno,inorder,ingrno,article,pv,lv,unitops,qtysku,      " +
        " qtypu,qtyweight,qtyvolume,dateexp,datemfg,lotmfg,serialno,huno,accnops,oupromo,xaction,          " +
        " xcreate,rowid ,orbitsite,orbitdepot) values ( @orgcode,@site,@depot,@dateops,@routeops,@transportno,@thcode,@thtype,  " +
        " dropship,@ouorder,@ouline,@ourefno,@ourefln,@oudnno,@inorder,@ingrno,@article,@pv,@lv,@unitops,  " +
        " @qtysku,qtypu,@qtyweight,@qtyvolume,@dateexp,@datemfg,@lotmfg,@serialno,@huno,@accnops,@oupromo, " +
        " 'WC',sysdatetimeoffset(), next value for seq_xodelivery,@orbitsite,@orbitdepot ) ";


    }
}