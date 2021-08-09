using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface IinboundparameterService{ 
        Task<pam_inbound> getInbound(string orgcode,string site ,string depot );
     }
}