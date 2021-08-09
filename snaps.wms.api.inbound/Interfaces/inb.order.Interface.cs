using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IinboundService{ 
        Task<List<inbound_ls>> listAsync(inbound_pm o); //list
        Task<inbound_md> getAsync(inbound_ls o); // get
        Task upsertAsync(inbound_md o); //upsert
        Task<List<lov>> getstaging(String orgcode, String site,String depot, Int32 quantity); //get staging available
        Task<List<lov>> getproductratio(String orgcode, String site, String depot, String article, String pv, String lv);
        Task setremarks(String orgcode, String site,String depot, String inorder,String remarks, String accncode) ;
        Task setpriority(String orgcode, String site,String depot, String inorder,Int32 inpriority, String accncode) ;
        Task setstaging(String orgcode, String site,String depot, String inorder,String staging,String accncode);
        Task setunloadstart(String orgcode, String site,String depot, String inorder, String accncode);
        Task setunloadend(String orgcode, String site,String depot, String inorder, String accncode);
        Task setfinish(String orgcode, String site,String depot, String inorder, String accncode);
        Task setinvoice(String orgcode, String site, String depot, String inorder, String invoiceno, String accncode);
        Task setreplan(String orgcode, String site, String depot, String inorder, DateTimeOffset? datereplan, String accncode);
        Task setcancel(String orgcode, String site, String depot, String inorder, String remarks, String accncode);
        
        Task<List<inbound_hs>> history(inbound_pm o) ;

         //Line Receive 
        Task<List<inboulx>> getlx(inboulx o);
        Task<List<inboulx>> upsertlx(inboulx o);
        Task<List<inboulx>> removelx(inboulx o);
        Task<List<inboulx>> commitlx(inboulx o);
     }
}