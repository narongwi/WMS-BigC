using System;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snaps.Helpers;
using Snaps.Helpers.StringExt;
using Snaps.Helpers.DbContext.SQLServer;
using System.Runtime.CompilerServices;

namespace Snaps.WMS {

    public partial class route_ops : IDisposable {       

        public async Task<route_md> bgcth_getroute(route_md o,string isnew = "NW"){ 
            SqlCommand cm = new SqlCommand("select count(routeno) routeno from wm_route where orgcode = @orgcode and site = @site and depot = @depot " +
            " and thcode = @thcode and cast(plandate as date) = cast(@datereqdelivery as date) and tflow = 'IO'",cn);
            route_md ro = new route_md();
            string rn = "";
            string nroute = "";
            string ndock = "";
            try { 
                cm.snapsPar(o.orgcode,"orgcode");
                cm.snapsPar(o.site,"site");
                cm.snapsPar(o.depot,"depot");
                cm.snapsPar(o.thcode,"thcode");
                o.datereqdelivery = (o.datereqdelivery.notNUll()) ? o.datereqdelivery : DateTimeOffset.Now;
                cm.snapsPar(o.datereqdelivery,"datereqdelivery");
                rn = cm.snapsScalarStrAsync().Result;
                if (rn == "0" && isnew == "NW" && o.routetype != "C") { 
                    cm.CommandText = "select indock from wm_thparty where orgcode = @orgcode and site = @site and depot = @depot and thcode = @thcode ";
                    ndock = cm.snapsScalarStrAsync().Result;
                    cm.CommandText =  " select cast(datepart(dw, @datereqdelivery) as varchar(1)) + t.thcode+ cast(RIGHT('00'+CAST(count(r.routeno)+1 AS VARCHAR(3)),2) as varchar(4))  " +
                    "   from wm_thparty t left join wm_route r  on t.orgcode = r.orgcode and t.site = r.site and t.depot = r.depot and t.thcode = r.thcode  " +
                    "    and cast(plandate as date) = cast(@datereqdelivery as date) where t.tflow = 'IO' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot " +
                    "    and t.thcode = @thcode  group by t.thcode ";
                    nroute = cm.snapsScalarStrAsync().Result;
                    ro.orgcode = o.orgcode;
                    ro.site = o.site;
                    ro.depot = o.depot;
                    ro.accncreate = o.accnmodify;
                    ro.accnmodify = o.accnmodify;
                    ro.contactno = "";
                    ro.crhu = 0;
                    ro.crvolume = 0;
                    ro.crweight = 0;
                    ro.datereqdelivery = o.datereqdelivery;
                    ro.dateshipment = null;
                    ro.loccode = ndock;
                    ro.mxvolume = o.mxvolume;
                    ro.mxweight = o.mxweight;
                    ro.mxhu = o.mxhu;
                    ro.oupriority = 0;
                    ro.plandate = o.datereqdelivery;
                    ro.routetype = o.routetype;
                    ro.routeno = nroute;
                    ro.routename = nroute;
                    ro.thcode = o.thcode;
                    ro.tflow = "IO";
                    ro.trtmode = o.trtmode;
                    ro.transportor = o.transportor;
                    ro.trucktype = o.trucktype;
                    ro.loadtype = o.loadtype;
                    ro.paytype = o.paytype;
                    ro.remarks = o.remarks;
                    ro.oupromo = o.oupromo;
                    ro.driver = o.driver;
                    ro.plateNo = o.plateNo; 
                    await upsert(ro);
                    return ro;
                } else if (isnew == "NW" && o.routetype == "C") {
                    //cm.CommandText =  " select '8' + format(sysdatetimeoffset(),'yyMMdd') + cast(RIGHT('0'+CAST(count(r.routeno)+1 AS VARCHAR(3)),2) as varchar(4)) rsl " + 
                    //" from wm_route r where cast(plandate as date) = cast(getdate() as date) and routetype = 'C'";
                    // GC Request 8 digit 
                    cm.CommandText = " select '8' + format(sysdatetimeoffset(),'MMdd') + cast(RIGHT('00'+CAST(count(r.routeno)+1 AS VARCHAR(3)),2) as varchar(4)) rsl " +
                    " from wm_route r where cast(plandate as date) = cast(getdate() as date) and routetype = 'C'";
                    nroute = cm.snapsScalarStrAsync().Result;
                    ro.orgcode = o.orgcode;
                    ro.site = o.site;
                    ro.depot = o.depot;
                    ro.accncreate = o.accnmodify;
                    ro.accnmodify = o.accnmodify;
                    ro.contactno = "";
                    ro.crhu = 0;
                    ro.crvolume = 0;
                    ro.crweight = 0;
                    ro.datereqdelivery = DateTimeOffset.Now;
                    ro.dateshipment = null;
                    ro.loccode = o.loccode;
                    ro.mxvolume = o.mxvolume< 99999 ? 99999: o.mxvolume;
                    ro.mxweight = o.mxweight< 9999? 9999: o.mxweight;
                    ro.mxhu = o.mxhu < 999 ? 999: o.mxhu;
                    ro.oupriority = 0;
                    ro.plandate = o.datereqdelivery;
                    ro.routetype = o.routetype;
                    ro.routeno = nroute;
                    ro.routename = nroute;
                    ro.thcode = "";
                    ro.tflow = "IO";
                    ro.trtmode = o.trtmode;
                    ro.transportor = o.transportor;
                    ro.trucktype = o.trucktype;
                    ro.loadtype = o.loadtype;
                    ro.paytype = o.paytype;
                    ro.remarks = o.remarks;
                    ro.oupromo = o.oupromo;
                    ro.driver = o.driver;
                    ro.plateNo = o.plateNo;
                    await upsert(ro);
                    return ro;
                } else {
                    cm.CommandText = "select max(routeno) routeno from wm_route where orgcode = @orgcode and site = @site and depot = @depot " +
                    " and thcode = @thcode and cast(plandate as date) = cast(@datereqdelivery as date) and tflow = 'IO'";
                    rn = cm.snapsScalarStrAsync().Result;
                    cm.snapsPar(rn,"routeno");
                    cm.CommandText = "select loccode from wm_route where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno ";
                    ndock = cm.snapsScalarStrAsync().Result;
                    ro.orgcode = o.orgcode;
                    ro.site = o.site;
                    ro.depot = o.depot;
                    ro.routeno = rn;
                    ro.routename = rn;
                    ro.loccode = ndock;
                    ro.accncreate = o.accnmodify;
                    ro.accnmodify = o.accnmodify;
                    ro.contactno = "";
                    ro.crhu = 0;
                    ro.crvolume = 0;
                    ro.crweight = 0;
                    ro.datereqdelivery = DateTimeOffset.Now;
                    ro.dateshipment = null;
                    ro.mxvolume = 999;
                    ro.mxweight = 9999;
                    ro.mxhu = 99999;
                    ro.oupriority = 0;
                    ro.plandate = DateTimeOffset.Now;
                    ro.routetype = o.routetype;
                    ro.thcode = o.thcode;
                    ro.tflow = "IO";
                    return ro;
                }                
            }
            catch (Exception ex) { throw ex; }
            finally  { await cm.DisposeAsync(); ro = null; nroute = null; ndock = null; }
        }  

        public async Task<route_md> getCombineroute(string orgcode, string site, string depot, string thcode)
        {
            try
            {
                SqlDataReader r = null;
                route_md ro = new route_md();
                SqlCommand cm = new SqlCommand("select top 1 * from wm_route where orgcode = @orgcode and site = @site and depot = @depot " +
                " and cast(plandate as date) = cast(getdate() as date) and routetype = 'C' and tflow='IO' order by routeno desc", cn);
                cm.snapsPar(orgcode, "orgcode");
                cm.snapsPar(site, "site");
                cm.snapsPar(depot, "depot");
                cm.snapsPar(thcode, "thcode");

                r = await cm.snapsReadAsync();
                if (!r.HasRows)
                {
                    await r.CloseAsync();
                    return null;
                }else
                {
                    while (await r.ReadAsync()) { ro = fillmdl(ref r); }
                    await r.CloseAsync();
                    cm.CommandText = "select oudock from wm_thparty where orgcode = @orgcode and site = @site and depot = @depot and thcode = @thcode ";
                    string ndock = cm.snapsScalarStrAsync().Result;
                    ro.loccode = ndock;
                    ro.thcode = thcode;
                    return ro;
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
        public async Task<route_md> bgcth_getroute(string orgcode,string site, string depot, string thcode,string accncode,DateTimeOffset plandate, string routetype  = "P",string isnew = "NW") {
            //SqlCommand cm = new SqlCommand("select count(routeno) routeno from wm_route where orgcode = @orgcode and site = @site and depot = @depot " +
            //" and thcode = @thcode and cast(plandate as date) = cast(@plandate as date) and tflow = 'IO'",cn);

            // check exists route 
            string sqlcm = @"select count(1)
	        from (select max(routeno) routeno, 0 priv from wm_route where orgcode = @orgcode and site = @site and depot = @depot 
            and tflow = 'IO' and routetype = 'C' and cast(plandate as date) = cast(@plandate as date) union all
            select max(routeno) routeno, 1 priv from wm_route where orgcode = @orgcode and site = @site and depot = @depot 
            and thcode = @thcode and cast(plandate as date) = cast(@plandate as date) and tflow = 'IO' and routetype = 'P'
            ) a where routeno is not null";

            SqlCommand cm = new SqlCommand(sqlcm,cn);
            route_md ro = new route_md();
            string rn = "";
            string nroute = "";
            string ndock = "";
            try { 
                cm.snapsPar(orgcode,"orgcode");
                cm.snapsPar(site,"site");
                cm.snapsPar(depot,"depot");
                cm.snapsPar(thcode,"thcode");
                cm.snapsPar(plandate,"plandate");

                // get route active with store code 
                rn = cm.snapsScalarStrAsync().Result;

                // request new route if not acive
                isnew = rn == "0" ? "NW" : isnew;

                // check condition
                if (rn == "0" || (isnew == "NW" && routetype == "P")) { 
                    cm.CommandText = "select oudock from wm_thparty where orgcode = @orgcode and site = @site and depot = @depot and thcode = @thcode ";
                    ndock = cm.snapsScalarStrAsync().Result;

                    // gen route format
                    //cm.CommandText =  "select cast(datepart(dw, @plandate) as varchar(1)) + t.thcode+ cast(RIGHT('00'+CAST(count(r.routeno)+1 AS VARCHAR(3)),2) as varchar(4))  " +
                    //"   from wm_thparty t left join wm_route r  on t.orgcode = r.orgcode and t.site = r.site and t.depot = r.depot and t.thcode = r.thcode  " +
                    //"   and cast(plandate as date) = cast(@plandate as date) where t.tflow = 'IO' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot " +
                    //"   and t.thcode = @thcode  group by t.thcode ";
                    cm.CommandText = "select (case when t.thtype = 'CS' then cast(datepart(dw, getdate()) as varchar(1)) + t.thcode+ cast(RIGHT('00'+CAST(count(r.routeno)+1 AS VARCHAR(3)),2) as varchar(4)) " +
                    "          else cast(datepart(dw, getdate()) as varchar(1)) + t.thcode + format(sysdatetimeoffset(), 'MMdd') + cast(RIGHT('00' + CAST(count(r.routeno) + 1 AS VARCHAR(3)), 2) as varchar(4))  end) " +
                    "   from wm_thparty t left join wm_route r  on t.orgcode = r.orgcode and t.site = r.site and t.depot = r.depot and t.thcode = r.thcode  " +
                    "   and cast(plandate as date) = cast(@plandate as date) where t.tflow = 'IO' and t.orgcode = @orgcode and t.site = @site and t.depot = @depot " +
                    "   and t.thcode = @thcode group by t.thtype,t.thcode ";

                    // new route number
                    nroute = cm.snapsScalarStrAsync().Result;

                    // binding model
                    ro.orgcode = orgcode;
                    ro.site = site;
                    ro.depot = depot;
                    ro.accncreate = accncode;
                    ro.accnmodify = accncode;
                    ro.contactno = "";
                    ro.crhu = 0;
                    ro.crvolume = 0;
                    ro.crweight = 0;
                    ro.datereqdelivery = DateTimeOffset.Now;
                    ro.dateshipment = null;
                    ro.loccode = ndock;
                    ro.mxvolume = 999999;
                    ro.mxweight = 999999;
                    ro.mxhu = 99;
                    ro.oupriority = 0;
                    ro.plandate = DateTimeOffset.Now;
                    ro.routetype = routetype;
                    ro.routeno = nroute;
                    ro.routename = nroute;
                    ro.thcode = thcode;
                    ro.tflow = "IO";
                    await upsert(ro);
                    return ro;
                } else if (isnew == "NW" && routetype == "C") { 
                    cm.CommandText = "select oudock from wm_thparty where orgcode = @orgcode and site = @site and depot = @depot and thcode = @thcode ";
                    ndock = cm.snapsScalarStrAsync().Result;

                    cm.CommandText =  "select '8' + format(sysdatetimeoffset(),'MMdd') + cast(RIGHT('00'+CAST(count(r.routeno)+1 AS VARCHAR(3)),2) as varchar(4)) rsl " + 
                    " from wm_route r where cast(plandate as date) = cast(getdate() as date) and routetype = 'C'";

                    ro.orgcode = orgcode;
                    ro.site = site;
                    ro.depot = depot;
                    ro.accncreate = accncode;
                    ro.accnmodify = accncode;
                    ro.contactno = "";
                    ro.crhu = 0;
                    ro.crvolume = 0;
                    ro.crweight = 0;
                    ro.datereqdelivery = DateTimeOffset.Now;
                    ro.dateshipment = null;
                    ro.loccode = ndock;
                    ro.mxvolume = 999999;
                    ro.mxweight = 999999;
                    ro.mxhu = 99;
                    ro.oupriority = 0;
                    ro.plandate = DateTimeOffset.Now;
                    ro.routetype = routetype;
                    ro.routeno = nroute;
                    ro.routename = nroute;
                    ro.thcode = thcode;
                    ro.tflow = "IO";
                    await upsert(ro);
                    return ro;
                } else {
                    //cm.CommandText = "select max(routeno) routeno from wm_route where orgcode = @orgcode and site = @site and depot = @depot " +
                    //" and thcode = @thcode and cast(plandate as date) = cast(@plandate as date) and tflow = 'IO' and routetype = 'P'";
                    cm.CommandText = @"select top 1 routeno from (
                    select max(routeno) routeno, 0 priv from wm_route where orgcode = @orgcode and site = @site and depot = @depot 
                    and tflow = 'IO' and routetype = 'C' and cast(plandate as date) = cast(@plandate as date) union all
                    select max(routeno) routeno, 1 priv from wm_route where orgcode = @orgcode and site = @site and depot = @depot 
                    and thcode = @thcode and cast(plandate as date) = cast(@plandate as date) and tflow = 'IO' and routetype = 'P'
                    ) a where routeno is not null order by priv asc";

                    // get route
                    rn = cm.snapsScalarStrAsync().Result;

                    cm.snapsPar(rn,"routeno");
                    cm.CommandText = "select loccode from wm_route where orgcode = @orgcode and site = @site and depot = @depot and routeno = @routeno ";
                    ndock = cm.snapsScalarStrAsync().Result;

                    ro.orgcode = orgcode;
                    ro.site = site;
                    ro.depot = depot;
                    ro.routeno = rn;
                    ro.routename = rn;
                    ro.loccode = ndock;
                    ro.accncreate = accncode;
                    ro.accnmodify = accncode;
                    ro.contactno = "";
                    ro.crhu = 0;
                    ro.crvolume = 0;
                    ro.crweight = 0;
                    ro.datereqdelivery = DateTimeOffset.Now;
                    ro.dateshipment = null;
                    ro.mxvolume = 999;
                    ro.mxweight = 9999;
                    ro.mxhu = 99999;
                    ro.oupriority = 0;
                    ro.plandate = DateTimeOffset.Now;
                    ro.routetype = routetype;
                    ro.thcode = thcode;
                    ro.tflow = "IO";
                    return ro;
                }                
            }
            catch (Exception ex) { throw ex; }
            finally  { await cm.DisposeAsync(); ro = null; nroute = null; ndock = null; }
        }
    }
}