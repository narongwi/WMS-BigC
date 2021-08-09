using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces
{
    public interface IcountService
    {
        Task<List<counttask_md>> listTaskAsync(counttask_md o);
        Task<counttask_md> getTaskAsync(counttask_md o);
        Task upsertTaskAsnc(counttask_md o);
        Task removeTaskAsync(counttask_md o);

        Task<List<countplan_md>> listPlanAsync(counttask_md o);
        Task<countplan_md> getPlanAsync(countplan_md o);
        Task upsertPlanAysnc(countplan_md o);
        Task removePlanAsync(countplan_md o);
        Task validatePlanAsync(countplan_md o);

        Task<List<countline_md>> listLineAsync(countplan_md o);
        Task<List<countline_md>> countLineAsync(countplan_md o);
        Task<List<countline_md>> getLineAsync(findcountline_md o);
        Task upsertLineAsync(List<countline_md> o);
        
        // confirm stock take
        Task<List<countcorrection_md>> getConfrimLineAsync(counttask_md o);
        Task countConfirmAsync(counttask_md o);


    }
}