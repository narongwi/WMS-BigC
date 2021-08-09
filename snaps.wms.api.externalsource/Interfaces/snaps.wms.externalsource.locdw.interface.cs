using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IexsLocdwService { 
          Task<List<exsFile>> findAsync(exsFile o);
          Task<List<exsLocdw>> lineLocdwAsync(exsFile o);
          Task<string> ImpexsLocdwAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart,  List<exsLocdw> o);
     }
}