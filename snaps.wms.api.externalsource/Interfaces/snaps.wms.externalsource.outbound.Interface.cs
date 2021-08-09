using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IexsOutboundService { 
          Task<List<exsFile>> findAsync(exsFile o);
          Task<List<exsOutbound>> lineOutboundAsync(exsFile o);
          Task<string> ImpexsOutboundAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart,  List<exsOutbound> o);
          Task<List<exsOutbouln>> lineOutboulnAsync(exsFile o);
          Task<string> ImpexsOutboulnAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart,  List<exsOutbouln> o);
     }
}