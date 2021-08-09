using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IexsInboundService { 
          Task<List<exsFile>> findAsync(exsFile o);
          Task<List<exsInbound>> lineInboundAsync(exsFile o);
          Task<string> ImpexsInboundAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart,  List<exsInbound> o);
          Task<List<exsInbouln>> lineInboulnAsync(exsFile o);
          Task<string> ImpexsInboulnAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart,  List<exsInbouln> o);
     }
}