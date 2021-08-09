using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IexsPrepService { 
          Task<List<exsFile>> findAsync(exsFile o);
          Task<List<exsPrep>> linePrepAsync(exsFile o);
          Task<string> ImpexsPrepAsync(string orgcode, string site, string depot,string accncode, string filetype,string filename,long filelength,DateTimeOffset decstart,  List<exsPrep> o);
     }
}