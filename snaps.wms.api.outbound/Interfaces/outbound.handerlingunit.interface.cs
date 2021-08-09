using Snaps.WMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Snaps.Helpers.StringExt;

namespace Snaps.WMS.Interfaces {
    public interface IouhanderlingunitService
    {
        Task<List<handerlingunit>> listAsync(handerlingunit o); //list
        Task<handerlingunit> getAsync(handerlingunit o); // get
        Task upsert(handerlingunit o);

        Task generate(handerlingunit_gen o);
        Task<List<lov>> getmaster(String orgcode, String site, String depot);
        Task<List<handerlingunit_item>> lines(handerlingunit o);
        Task<List<handerlingunit_item>> linesnonsum(handerlingunit o);
        Task close(handerlingunit o); //Close / Base close hu

    }
}
