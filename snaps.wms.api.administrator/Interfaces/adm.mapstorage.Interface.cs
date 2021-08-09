using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS.warehouse;
namespace Snaps.WMS.Interfaces { 
     public interface ImapstorageService { 
        Task<List<locup_md>> lstloczone(locup_pm o);
        Task<List<locup_md>> lstlocup(locup_pm o);
        Task upsertlocup(locup_md o);
        Task removelocup(locup_md o);
        Task<List<locdw_ls>> lstlocdw(locdw_pm o);
        Task<locdw_md> getlocdw(locdw_ls o);
        Task upsertlocdw(locdw_md o);
        Task removelocdw(locdw_md o);
        Task generatelocdw(locdw_gn o);
        Task generatelocgd(locdw_gngrid o);
        Task<List<locdw_pivot>> getpivot(locdw_pm o);
        Task setpivot(locdw_pivot o,String accncode);
        Task<List<locdw_picking>> getpicking(locdw_pm o);
        Task setpicking(locdw_picking o,String accncode);

     }
}