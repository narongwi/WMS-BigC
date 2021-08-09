using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IouborderService{ 
         Task<List<outbound_ls>> listAsync(outbound_pm o); //list
         Task<List<outbound_ls>> listdistAsync(outbound_pm o); //list dist
         Task<outbound_md> getAsync(outbound_ls o); // get
         Task<outbound_md> getdistAsync(outbound_ls o);
         Task setPriority(outbound_md o);
         Task setremarks(outbound_md o);
         Task changeRequest(outbound_md o);
         Task setlineorder(outbouln_md o);

     }
}