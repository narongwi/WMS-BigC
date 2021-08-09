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

    public partial class task_ops : IDisposable {

        //Additional 
        private SqlCommand prepInb(task_md o,string taskno){ 
            SqlCommand cm = new SqlCommand(sqlinsert_task,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.tasktype,"tasktype");
            cm.snapsPar(taskno,"taskno");
            cm.snapsPar(o.iopromo,"iopromo");
            cm.snapsPar(o.iorefno,"iorefno");
            cm.snapsPar(o.priority,"priority");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(taskno + " : " + o.taskname ,"taskname");
            cm.snapsPar(o.routeno,"routeno");
            cm.snapsPar(o.routethcode,"routethcode");
            cm.snapsPar(o.setno,"setno");
            cm.snapsParsysdateoffset();
            return cm;
        }

        private SqlCommand prepInl(taln_md o,string taskno,string routeno){ 
            SqlCommand cm = new SqlCommand(sqlinsert_line,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(taskno,"taskno");
            cm.snapsPar(o.taskseq,"taskseq");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.sourceloc,"sourceloc");
            cm.snapsPar(o.sourcehuno,"sourcehuno");
            cm.snapsPar(o.targetadv,"targetadv");
            cm.snapsPar(o.targetloc,"targetloc");
            cm.snapsPar(o.targethuno,"targethuno");
            cm.snapsPar(o.targetqty,"targetqty");
            cm.snapsPar(o.collectloc,"collectloc"); 
            cm.snapsPar(o.collecthuno,"collecthuno");
            cm.snapsPar(o.collectqty,"collectqty");
            cm.snapsPar(o.accnassign,"accnassign");
            cm.snapsPar(o.accnwork,"accnwork");
            cm.snapsPar(o.accnfill,"accnfill");
            cm.snapsPar(o.accncollect,"accncollect");
            cm.snapsPar(o.dateassign,"dateassign");
            cm.snapsPar(o.datework,"datework");
            cm.snapsPar(o.datefill,"datefill");
            cm.snapsPar(o.datecollect,"datecollect");
            cm.snapsPar(o.iopromo,"iopromo");
            cm.snapsPar(o.ioreftype,"ioreftype");
            cm.snapsPar(o.iorefno,"iorefno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.sourceqty,"sourceqty");
            cm.snapsPar(o.sourcevolume,"sourcevolume");
            cm.snapsPar(o.stockid,"stockid");
            cm.snapsPar(routeno,"routeno");
            cm.snapsPar(taskno,"opscode");

            cm.snapsPar(o.ouorder,"ouorder");
            cm.snapsPar(o.ouln,"ouln");
            cm.snapsPar(o.ourefno,"ourefno");
            cm.snapsPar(o.ourefln,"ourefln");
            
            cm.snapsParsysdateoffset();
            return cm; 
        }
        private task_ls fillls(ref SqlDataReader r) { 
            task_ls rn =  new task_ls();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.tasktype = r["tasktype"].ToString();
            rn.taskno = r["taskno"].ToString();
            rn.iopromo = r["iopromo"].ToString();
            rn.iorefno = r["iorefno"].ToString();
            rn.priority =  r["priority"].ToString();
            rn.taskdate = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9);
            rn.datestart = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10);
            rn.dateend = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11);
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(13)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(13);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(15)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(15);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.taskname = r["taskname"].ToString();
            rn.article = r["article"].ToString();
            rn.pv = r.GetInt32(28);
            rn.lv = r.GetInt32(29);
            rn.sourceloc = r["sourceloc"].ToString();
            rn.sourcehuno = r["sourcehuno"].ToString();
            rn.targetadv = r["targetadv"].ToString();
            rn.descalt = r["descalt"].ToString();
            rn.accnwork = r["accnwork"].ToString();
            rn.dateremarks = r["dateremarks"].ToString();
            rn.routeno = r["routeno"].ToString();
            return rn;
        }
        private task_ix fillix(ref SqlDataReader r) { 
            return new task_ix() { 

            };
        }
        private taln_md fillln(ref SqlDataReader r){
            taln_md rn = new taln_md();
            rn.orgcode = r["orgcode"].ToString();
            rn.site = r["site"].ToString();
            rn.depot = r["depot"].ToString();
            rn.spcarea = r["spcarea"].ToString();
            rn.taskno = r["taskno"].ToString();
            rn.taskseq = r.GetInt32(5);
            rn.article = r["article"].ToString();
            rn.pv = r.GetInt32(7);
            rn.lv = r.GetInt32(8);
            rn.sourceloc = r["sourceloc"].ToString();
            rn.sourcehuno = r["sourcehuno"].ToString();
            rn.targetadv = r["targetadv"].ToString();
            rn.targetloc = r["targetloc"].ToString();
            rn.targethuno = r["targethuno"].ToString();
            rn.targetqty = r.GetInt32(14);
            rn.collectloc = r["collectloc"].ToString();
            rn.collecthuno = r["collecthuno"].ToString();
            rn.collectqty = r.GetInt32(17);
            rn.accnassign = r["accnassign"].ToString();
            rn.accnwork = r["accnwork"].ToString();
            rn.accnfill = r["accnfill"].ToString();
            rn.accncollect = r["accncollect"].ToString();
            rn.dateassign = (r.IsDBNull(22)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(22);
            rn.datework = (r.IsDBNull(23)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(23);
            rn.datefill = (r.IsDBNull(24)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(24);
            rn.datecollect = (r.IsDBNull(25)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(25);
            rn.iopromo = r["iopromo"].ToString();
            rn.ioreftype = r["ioreftype"].ToString();
            rn.iorefno = r["iorefno"].ToString();
            rn.lotno = r["lotno"].ToString();
            rn.datemfg = (r.IsDBNull(31)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(31);
            rn.dateexp = (r.IsDBNull(32)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(32);
            rn.serialno = r["serialno"].ToString();
            rn.tflow = r["tflow"].ToString();
            rn.datecreate = (r.IsDBNull(35)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(35);
            rn.accncreate = r["accncreate"].ToString();
            rn.datemodify = (r.IsDBNull(37)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(37);
            rn.accnmodify = r["accnmodify"].ToString();
            rn.procmodify = r["procmodify"].ToString();
            rn.descalt = r["descalt"].ToString();
            return rn;
        }
        private task_md fillmdl(ref SqlDataReader r) { 
            return new task_md() {
                orgcode = r["orgcode"].ToString(),
                site = r["site"].ToString(),
                depot = r["depot"].ToString(),
                spcarea = r["spcarea"].ToString(),
                tasktype = r["tasktype"].ToString(),
                taskno = r["taskno"].ToString(),
                iopromo = r["iopromo"].ToString(),
                iorefno = r["iorefno"].ToString(),
                priority = r["priority"].ToString(),
                taskdate = (r.IsDBNull(9)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(9),
                datestart = (r.IsDBNull(10)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(10),
                dateend = (r.IsDBNull(11)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(11),
                tflow = r["tflow"].ToString(),
                datecreate = (r.IsDBNull(13)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(13),
                accncreate = r["accncreate"].ToString(),
                datemodify = (r.IsDBNull(15)) ? (DateTimeOffset?) null : r.GetDateTimeOffset(15),
                accnmodify = r["accnmodify"].ToString(),
                procmodify = r["procmodify"].ToString(),
                taskname = r["taskname"].ToString(),
            };
        }
        private SqlCommand ixcommand(task_ix o,String sql) { 
            SqlCommand cm = new SqlCommand(sql,cn);
            return cm;
        }

        private List<SqlCommand> getcommand(task_md o){ 
            List<SqlCommand> rn = new List<SqlCommand>();
            rn.Add(obcommand(o,"sqlinsert_task"));
            rn.Add(lbcommand(o.lines[0],o.taskno));
            return rn;
        }
        private SqlCommand obcommand(task_md o,string cmd){ 
            SqlCommand cm = new SqlCommand(sqlinsert_task,cn);
            cm.snapsPar(o.orgcode, "orgcode");
            cm.snapsPar(o.site, "site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(o.tasktype,"tasktype");
            cm.snapsPar(o.taskno,"taskno");
            cm.snapsPar(o.iopromo,"iopromo");
            cm.snapsPar(o.iorefno,"iorefno");
            cm.snapsPar(o.priority,"priority");
            cm.snapsPar(o.taskdate,"taskdate");
            //cm.snapsPar(o.datestart,"datestart");
            //cm.snapsPar(o.dateend,"dateend");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.taskno + "-"+ o.taskname,"taskname");
            cm.snapsPar(o.routeno,"routeno");
            cm.snapsPar(o.routethcode,"routethcode");
            cm.snapsPar("","setno");
            cm.snapsParsysdateoffset();
            return cm;
        }

        private SqlCommand lbcommand(taln_md o,string taskno) {
            SqlCommand cm = new SqlCommand(sqlinsert_line,cn);
            cm.snapsPar(o.orgcode,"orgcode");
            cm.snapsPar(o.site,"site");
            cm.snapsPar(o.depot,"depot");
            cm.snapsPar(o.spcarea,"spcarea");
            cm.snapsPar(taskno,"taskno");
            cm.snapsPar(o.taskseq,"taskseq");
            cm.snapsPar(o.article,"article");
            cm.snapsPar(o.pv,"pv");
            cm.snapsPar(o.lv,"lv");
            cm.snapsPar(o.sourceloc,"sourceloc");
            cm.snapsPar(o.sourcehuno,"sourcehuno");
            cm.snapsPar(o.targetadv,"targetadv");
            cm.snapsPar(o.targetloc,"targetloc");
            cm.snapsPar(o.targethuno,"targethuno");
            cm.snapsPar(o.targetqty,"targetqty");
            cm.snapsPar(o.collectloc,"collectloc"); 
            cm.snapsPar(o.collecthuno,"collecthuno");
            cm.snapsPar(o.collectqty,"collectqty");
            cm.snapsPar(o.accnassign,"accnassign");
            cm.snapsPar(o.accnwork,"accnwork");
            cm.snapsPar(o.accnfill,"accnfill");
            cm.snapsPar(o.accncollect,"accncollect");
            //cm.snapsPar(o.dateassign,"dateassign");
           // cm.snapsPar(o.datework,"datework");
            //cm.snapsPar(o.datefill,"datefill");
            //cm.snapsPar(o.datecollect,"datecollect");
            cm.snapsPar(o.iopromo,"iopromo");
            cm.snapsPar(o.ioreftype,"ioreftype");
            cm.snapsPar(o.iorefno,"iorefno");
            cm.snapsPar(o.lotno,"lotno");
            cm.snapsPar(o.datemfg,"datemfg");
            cm.snapsPar(o.dateexp,"dateexp");
            cm.snapsPar(o.serialno,"serialno");
            cm.snapsPar(o.tflow,"tflow");
            cm.snapsPar(o.accncreate,"accncreate");
            cm.snapsPar(o.accnmodify,"accnmodify");
            cm.snapsPar(o.procmodify,"procmodify");
            cm.snapsPar(o.sourceqty,"sourceqty");
            cm.snapsPar(o.sourcevolume,"sourcevolume");
            cm.snapsPar(o.stockid,"stockid");

            cm.snapsPar(o.ouorder,"ouorder");
            cm.snapsPar(o.ouln,"ouln");
            cm.snapsPar(o.ourefno,"ourefno");
            cm.snapsPar(o.ourefln,"ourefln");
            
            cm.snapsParsysdateoffset();
            return cm; 
        }

    }
}