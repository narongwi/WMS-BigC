using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS {
    public class task_ls {

        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String tasktype { get; set; } 
        public String taskno { get; set; } 
        public String iopromo { get; set; } 
        public String iorefno { get; set; } 
        public string priority { get; set; } 
        public DateTimeOffset? taskdate { get; set; } 
        public String tflow { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public DateTimeOffset? datestart { get; set; } 
        public DateTimeOffset? dateend { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public String taskname { get; set; } 
        public String article { get; set; }
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public String sourceloc { get; set; }
        public String sourcehuno { get; set; }
        public String targetadv { get; set; }
        public String descalt { get; set; }
        public String accnwork { get; set; } 
        public String dateremarks { get; set ;}
        public string routeno { get; set; }
    }
    public class task_pm : task_ls { 
        public DateTimeOffset? taskdatefrom { get; set; } 
        public DateTimeOffset? taskdateto { get; set; } 
    }
    public class task_ix : task_ls { 
    }
    public class task_md   {

        public String orgcode { get; set; }
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String tasktype { get; set; } 
        public String taskno { get; set; } 
        public String iopromo { get; set; } 
        public String iorefno { get; set; } 
        public string priority { get; set; } 
        public DateTimeOffset? taskdate { get; set; } 
        public String tflow { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public DateTimeOffset? datestart { get; set; } 
        public DateTimeOffset? dateend { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public String taskname { get; set; } 
        public List<taln_md> lines { get; set; }


        public String confirmdigit { get; set; }
        public String devid { get; set; }
        public string routeno { get; set;}
        public string routethcode { get; set; }
        public string opscode { get; set ;}
        public string intype { get; set; } // Stock, Bulk , Damage, Interchange , Overflow, Return 

        public string setno { get; set; } // preparation setno
    }

    public class taln_ls {
        public String orgcode { get; set; } 
        public String site { get; set; } 
        public String depot { get; set; } 
        public String spcarea { get; set; } 
        public String taskno { get; set; }  
        public Int32 taskseq { get; set; } 
        public String article { get; set; } 
        public String sourceloc { get; set; } 
        public String sourcehuno { get; set; } 
        public String targetadv { get; set; } 
        public String targethuno { get; set; } 
        public String iopromo { get; set; } 
        public String ioreftype { get; set; } 
        public String iorefno { get; set; } 
        public DateTimeOffset? datemodify { get; set; } 
        public String tflow { get; set; } 
        public String accnassign { get; set; } 
        public String accnwork { get; set; } 
    }
    public class taln_pm : taln_ls { 
    }
    public class taln_ix : taln_ls { 
    }
    public class taln_md : taln_ls  {
        public Int32 pv { get; set; } 
        public Int32 lv { get; set; } 
        public String targetloc { get; set; } 
        public Int32 targetqty { get; set; } 
        public String collectloc { get; set; } 
        public String collecthuno { get; set; } 
        public Int32 collectqty { get; set; }
        public String accnfill { get; set; } 
        public String accncollect { get; set; } 
        public DateTimeOffset? dateassign { get; set; } 
        public DateTimeOffset? datework { get; set; } 
        public DateTimeOffset? datefill { get; set; } 
        public DateTimeOffset? datecollect { get; set; } 
        public String lotno { get; set; } 
        public DateTimeOffset? datemfg { get; set; } 
        public DateTimeOffset? dateexp { get; set; } 
        public String serialno { get; set; } 
        public DateTimeOffset? datecreate { get; set; } 
        public String accncreate { get; set; } 
        public String accnmodify { get; set; } 
        public String procmodify { get; set; } 
        public String descalt { get; set;}
        public Decimal sourceqty { get; set; }
        public Decimal sourcevolume { get; set; }
        public Decimal sourceweight { get; set; }
        public Decimal stockid { get; set ;}

        public string ouorder   { get; set; }
        public string ouln      { get; set; }
        public string ourefno   { get; set; }
        public string ourefln   { get; set; }

        public string thcode { get; set; }
        public string skipdigit { get; set; }

    }

    public class replen_md {
        public string orgcode { get; set; }
        public string site { get; set; }
        public string depot { get; set; }
        public string spcarea { get; set; }
        public string zone { get; set; }
        public string aisle { get; set; }
        public string level { get; set; }
        public string location { get; set; }
        public string article { get; set; }
        public string pv { get; set; }
        public string lv { get; set; }
        public string accncode { get; set; }
    }
}