using System;
using System.Collections.Generic;
using System.Text;

namespace Snaps.WMS{
    public partial class mergehu_ops {
        public readonly string sqlmergehu_insert = @"INSERT INTO wm_mergehu ( orgcode,site,depot,spcarea,hutype,hutarget,loccode,tflow,datecreate,accncreate,datemodify,accnmodify,procmodify,remarks ) 
         VALUES (@orgcode,@site,@depot,@spcarea,@hutype,@hutarget,@loccode,@tflow,@datecreate,@accncreate,@datemodify,@accnmodify,@procmodify,@remarks); select SCOPE_IDENTITY() as mergeno";

        public readonly string sqlmergehu_update = @"UPDATE wm_mergehu  SET tflow = @tflow, datemodify = SYSDATETIMEOFFSET(), accnmodify = @accnmodify,remarks = @remarks 
        WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND spcarea = @spcarea  AND mergeno = @mergeno";

        public readonly string sqlmergehu_delete = 
            @"DELETE FROM wm_mergeln WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND spcarea = @spcarea  AND mergeno = @mergeno;
              DELETE FROM wm_mergehu WHERE orgcode = @orgcode AND site = @site AND depot = @depot AND spcarea = @spcarea  AND mergeno = @mergeno";
    }
}
