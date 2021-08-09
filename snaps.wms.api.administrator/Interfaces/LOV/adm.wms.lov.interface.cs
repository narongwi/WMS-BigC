using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces {
    public interface ILOVService {
        Task<List<lov>> lovwarehouseAsync();
        Task<List<lov>> lovwarehouseAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovdepotAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovroleAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovAsync(string orgcode,string site,string depot,string bntype,string bncode,Boolean isdesc = true);
        Task<List<lov>> lovprepzonestockAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovprepzonedistAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovsharedistAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovstoragezoneAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovhuAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovzoneAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovaisleAsync(string orgcode,string site,string depot,string zone);
        Task<List<lov>> lovbayAsync(string orgcode,string site,string depot,string zone,string aisle);
        Task<List<lov>> lovlevelAsync(string orgcode,string site,string depot,string zone,string aisle,string bay);
        Task<List<lov>> lovlocationAsync(string orgcode,string site,string depot,string zone,string aisle,string bay,string level);

        Task<List<lov>> lovstaginginbAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovstagingoubAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovbulkAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovdamageAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovsinbinAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovexchangeAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovoverflowAsync(string orgcode,string site,string depot);
        Task<List<lov>> lovreturnAsync(string orgcode,string site,string depot);

        //Validate location
        Boolean valaisle(string orgcode,string site,string depot,string loc);
        Boolean valbay(string orgcode,string site,string depot,string loc);
        Boolean vallevel(string orgcode,string site,string depot,string loc);
        Boolean vallocation(string orgcode,string site,string depot,string loc);


    }
}