using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IexsProductService { 
          Task<List<exsFile>> findAsync(exsFile o);
          Task<List<exsProduct>> lineProductAsync(exsFile o);
          Task<string> ImpexsProductAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart,  List<exsProduct> o);
     }
}