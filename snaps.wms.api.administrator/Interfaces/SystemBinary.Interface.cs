using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.warehouse;
using Snaps.Helpers.StringExt;
namespace Snaps.WMS.Interfaces { 
     public interface ISystemBinaryService { 

       resultRequest getMessage(String lang, String ercode,resultState result,String reqid, String opt1 = "",String opt2 = "");
     //   Task<List<lov>> LOVWarehouse(String valorg, String valsite, String valdepot);
     //   Task<List<lov>> LOVDepot(String valorg, String valsite, String valdepot);
     //   Task<List<lov>> LOVRole(String valorg, String valsite, String valdepot);
     //   Task<List<lov>> LOVState(String valorg, String valsite, String valdepot, String valtype,String valcode);
     //   Task<List<lov>> LOVConfig(String valorg, String valsite, String valdepot, String valtype,String valcode);


     //  //Location 
     //  Task<List<lov>> LOVLocupper(locup_pm o);
     //  Task<List<lov>> LOVLoclower(locdw_pm o);
     //  Task<List<lov>> LOVLocdist(locup_pm o);

     //  //Handerling unit 
     //  Task<List<lov>> LOVHU(string orgcode, string site, string depot);

     //  //Zone
     //  Task<List<lov>> LOVZone(string orgcode, string site, string depot);

     }
}