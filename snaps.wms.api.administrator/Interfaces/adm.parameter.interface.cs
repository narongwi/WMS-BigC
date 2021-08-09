using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers;
using Snaps.WMS;

namespace Snaps.WMS.Interfaces { 
     public interface IadmparameterService { 
         Task<List<pam_parameter>> getParameterListAsync(String orgcode, String site, String depot, String pmmodule,String type ); //list
         Task updateParameterAsync(pam_parameter o); //update
     }
}