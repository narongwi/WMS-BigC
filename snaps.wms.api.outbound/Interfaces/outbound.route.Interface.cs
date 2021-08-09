using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IourouteService{ 
         Task<List<route_thsum>> thsum(route_pm o);
         Task<List<route_ls>> listAsync(route_pm o); //list
         Task<route_md> getAsync(route_ls o); // get
         Task upsert(route_md o);
         Task allocate(route_md o);
         Task huload(string orgcode, string site, string depot, route_hu o);
         Task shipment(route_md o,string accncode);
         Task<List<lov>> getstaging(String orgcode, String site,String depot);

        Task<List<lov>> getthirdparty(String orgcode, String site, String depot);
        Task<List<lov>> getTransporter(String orgcode, String site, String depot);
        Task remove(route_md o);

     }
}