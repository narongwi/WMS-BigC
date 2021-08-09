using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
namespace Snaps.WMS.Interfaces { 
     public interface ItransferService{ 
        lov checklocation(string orgcode, string site, string depot, string loccode);
        void validatelocation(transfer_md o) ;
        Task<transfer_md> transferstock(transfer_md o);
    }
}