using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface ItaskService{ 
        Task<List<task_ls>> listAsync(task_pm o); //list
        Task<task_md> getAsync(task_ls o); // get
        Task upsertAsync(task_md o); //upsert
        
        Task assignAsync(task_md o); // assign task
        Task startAsync(task_md o); // start task
        Task fillAsync(task_md o); // fill task
        Task cancelAsync(task_md o); // cancel task
        Task collectAsync(task_md o); // collect task
        Task confirmAsync(task_md o); // confirm on web only
        Task UrgenReplenishment(replen_md o); // Urgen Replenishment process
    }
}