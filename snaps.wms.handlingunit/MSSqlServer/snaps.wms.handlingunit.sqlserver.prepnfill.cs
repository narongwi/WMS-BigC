using Snaps.Helpers.DbContext.SQLServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Snaps.WMS
{
    public partial class handerlingunit_ops : IDisposable
    {
        //Header
        private handerlingunit handerlingunit_get(ref SqlDataReader r)
        {
            handerlingunit rn = new handerlingunit();
            try {
                rn.orgcode = r["orgcode"].ToString();
                rn.site = r["site"].ToString();
                rn.depot = r["depot"].ToString();
                rn.spcarea = r["spcarea"].ToString();
                rn.hutype = r["hutype"].ToString();
                rn.huno = r["huno"].ToString();
                rn.loccode = r["loccode"].ToString();
                rn.thcode = r["thcode"].ToString();
                rn.routeno = r["routeno"].ToString();
                rn.mxsku = r.IsDBNull(9) ? 0 : r.GetInt32(9);
                rn.mxweight = r.IsDBNull(10) ? 0 : r.GetDecimal(10);
                rn.mxvolume = r.IsDBNull(11) ? 0 : r.GetDecimal(11);
                rn.crsku = r.IsDBNull(12) ? 0 : r.GetInt32(12);
                rn.crweight = r.IsDBNull(13) ? 0 : r.GetDecimal(13);
                rn.crvolume = r.IsDBNull(14) ? 0 : r.GetDecimal(14);
                rn.crcapacity = r.IsDBNull(15) ? 0 : r.GetDecimal(15);
                rn.tflow = r["tflow"].ToString();
                rn.datecreate = r.IsDBNull(17) ? (DateTimeOffset?) null : r.GetDateTimeOffset(17);
                rn.accncreate = r["accncreate"].ToString();
                rn.datemodify =  r.IsDBNull(19) ? (DateTimeOffset?) null : r.GetDateTimeOffset(19);
                rn.accnmodify = r["accnmodify"].ToString();
                rn.procmodfiy = r["procmodify"].ToString();
                rn.thname = r["thnameint"].ToString();
                rn.priority = r.IsDBNull(22) ? 0 :r.GetInt32(22);
                rn.promo = r["promo"].ToString();
                return rn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SqlCommand handerlingunit_setcmd(handerlingunit o, String sql)
        {
            SqlCommand cm = new SqlCommand(sql, cn);
            cm.snapsPar(o.orgcode, "orgcode");
            cm.snapsPar(o.site, "site");
            cm.snapsPar(o.depot, "depot");
            cm.snapsPar(o.spcarea, "spcarea");
            cm.snapsPar(o.hutype, "hutype");
            cm.snapsPar(o.huno, "huno");
            cm.snapsPar(o.loccode, "loccode");
            cm.snapsPar(o.thcode, "thcode");
            cm.snapsPar(o.routeno, "routeno");
            cm.snapsPar(o.tflow, "tflow");
            cm.snapsPar(o.accnmodify, "accnmodify");
            cm.snapsPar(o.procmodfiy, "procmodify");
            cm.snapsPar(o.priority, "priority");
            cm.snapsPar(o.promo, "promo");
            cm.snapsPar(o.mxsku, "mxsku");
            cm.snapsPar(o.mxweight, "mxweight");
            cm.snapsPar(o.mxvolume,"mxvolume");
            cm.snapsPar(o.crsku, "crsku");
            cm.snapsPar(o.crweight, "crweight");
            cm.snapsPar(o.crvolume, "crvolume");
            cm.snapsPar(o.crcapacity, "crcapacity");
            cm.snapsPar(o.accncreate, "accncreate");
            //cm.snapsPar(o.accnmodify, "accnmodify");
            //cm.snapsPar(o.priority, "priority");
            cm.snapsParsysdateoffset();
            return cm;
        }
    }
}
