using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;

namespace Snaps.WMS.warehouse {

    public partial class locdw_ops : IDisposable { 
        private static string tbn = "wm_locdw";
        private static string sqlmcom = " and orgcode = @orgcode and site = @site and depot = @depot and spcarea = @spcarea and lscodeid = @lscodeid  " ;
        private String sqlins = "insert into " + tbn + 
        " ( orgcode, site,depot,spcarea,fltype,lszone,lsaisle,lsbay,lslevel,lsstack,lscode,lscodealt,lscodefull, "+ 
        " lsdmlength,lsdmwidth,lsdmheight,lsmxweight,lsmxvolume,lsmxlength,lsmxwidth,lsmxheight,lsmxhuno, "+ 
        " lsmnsafety,lsmixarticle,lsmixage,lsmixlotno,lsloctype,lsremarks,lsgaptop,lsgapleft,lsgapright, "+ 
        " lsgapbuttom,lsstackable,lsdigit,spcthcode,spchuno,spcarticle,spcblock,spctaskfnd,spcseqpath, "+ 
        " spclasttouch,spcpivot,spcpicking,spcpickunit,spcrpn,spcmnaging,spcmxaging,crweight,crvolume, "+ 
        " crfreepct,tflow,tflowcnt,lshash,datecreate,accncreate,datemodify,accnmodify, lscodeid,lsloc ) values "+ 
        " ( @orgcode, @site, @depot,@spcarea,@fltype,@lszone,@lsaisle,@lsbay,@lslevel,@lsstack,@lscode, @lscodealt, "+ 
        " @lscodefull,@lsdmlength,@lsdmwidth,@lsdmheight,@lsmxweight,@lsmxvolume,@lsmxlength,@lsmxwidth, "+ 
        " @lsmxheight,@lsmxhuno,@lsmnsafety,@lsmixarticle,@lsmixage,@lsmixlotno,@lsloctype,@lsremarks, "+ 
        " @lsgaptop,@lsgapleft,@lsgapright,@lsgapbuttom,@lsstackable,@lsdigit,@spcthcode,@spchuno, "+ 
        " @spcarticle,@spcblock,@spctaskfnd,@spcseqpath,@spclasttouch,@spcpivot,@spcpicking,@spcpickunit, "+ 
        " @spcrpn,@spcmnaging,@spcmxaging,@crweight,@crvolume,@crfreepct,'IO','IO',@lshash,@sysdate, "+ 
        " @accncreate,@sysdate,@accnmodify, NEXT VALUE FOR seq_locdw,  @lsloc ) ";
        private String sqlupd = " update " + tbn + " set " + 
        " lscodealt = @lscodealt,lscodefull = @lscodefull,lsdmlength = @lsdmlength,lsdmwidth = @lsdmwidth,lsdmheight = @lsdmheight, "+ 
        " lsmxweight = @lsmxweight,lsmxvolume = @lsmxvolume,lsmxlength = @lsmxlength,lsmxwidth = @lsmxwidth,lsmxheight = @lsmxheight, "+ 
        " lsmxhuno = @lsmxhuno,lsmnsafety = @lsmnsafety,lsmixarticle = @lsmixarticle,lsmixage = @lsmixage,lsmixlotno = @lsmixlotno, "+ 
        " lsloctype = @lsloctype,lsremarks = @lsremarks,lsgaptop = @lsgaptop,lsgapleft = @lsgapleft,lsgapright = @lsgapright, "+ 
        " lsgapbuttom = @lsgapbuttom,lsstackable = @lsstackable,lsdigit = @lsdigit,spcthcode = @spcthcode,spchuno = @spchuno, "+ 
        " spcarticle = @spcarticle,spcblock = @spcblock,spctaskfnd = @spctaskfnd,spcseqpath = @spcseqpath,spclasttouch = @spclasttouch, "+ 
        " spcpivot = @spcpivot,spcpicking = @spcpicking,spcpickunit = @spcpickunit,spcrpn = @spcrpn,spcmnaging = @spcmnaging, "+ 
        " spcmxaging = @spcmxaging,crweight = @crweight,crvolume = @crvolume,crfreepct = @crfreepct,tflow = @tflow, "+ 
        " tflowcnt = @tflowcnt,lshash = @lshash,datemodify = @sysdate,accnmodify = @accnmodify " + 
        " where 1=1 " + sqlmcom;        
        private String sqlrem = "delete from wm_locdw where 1=1 " + sqlmcom; 
        private String sqlfnd = "select * from wm_locdw l where l.orgcode = @orgcode and l.site = @site and l.depot = @depot";
        private String sqlval = "select count(1) rsl from " + tbn + " where 1=1 " + sqlmcom;  
        private string sqllocup_validate_isexists = "select isnull((select count(1) from wm_locdw where lscode = @lscode and orgcode = @orgcode and site = @site  and depot = @depot) ,0) ";



    }
}