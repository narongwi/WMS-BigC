using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Snaps.Helpers.StringExt;
using Snaps.WMS;
using Snaps.WMS.preparation;

namespace Snaps.WMS.Interfaces { 
     public interface IouprepService{ 
        Task<List<prep_ls>> listAsync(prep_pm o); //list
        Task<prep_md> getAsync(prep_ls o); // get
        Task setPriority(prep_md o);
        Task setStart(prep_md o);
        Task setEnd(prep_md o);
        Task opsPick(prln_md o);
        Task opsPut(prln_md o);
        Task opsCancel(prep_md o);
        Task opsSelect(ouselect o);
        Task opsUnselect(ouselect o);
        Task<prepset> procsetup(prepset o);       //Setup process
        Task<prepset> procstock(string orgcode, string site, string depot,string spcarea, string setno,string accncode); // process stocking 
        //Task procdistb(List<outbound_ls> o); // process distribute

        Task<prepset> distsetup(prepset o);     //Setup process for distribute 
        Task<prepset> procdistb(string orgcode,string site,string depot,string spcarea, string setno,string accncode) ;


    }
}