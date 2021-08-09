using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IshareprepService { 
        Task<List<shareprep_md>> listAsync(shareprep_md o);
        Task<shareprep_md> getAsync(shareprep_md o);
        Task upsertAsnc(shareprep_md o);
        Task removeAsync(shareprep_md o);
        Task uplineAsync(shareprln_md o);
        Task rmlineAsync(shareprln_md o);
     }
}