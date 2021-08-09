using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IstockService{ 
        Task<List<stock_ls>> listProductAsync(stock_pm o); //list
        Task<stock_info> getstockInfo(stock_ls o);
        Task<List<stock_md>> getstockLine(string typesel, stock_ls o);
        Task setstatus(stock_md o);
        Task<List<lov>> getproductratio(string orgcode, string site, string depot, string article, string pv, string lv);
     }
}