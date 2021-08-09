using MGTH;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BGC.MIS.WMS.PTG.Controllers
{
    public class QryController : ApiController
    {
        protected Configure cfg = new Configure();

        [HttpGet]
        public IHttpActionResult get()
        {
            RepCtl orc = new RepCtl(cfg.getInfo());
            try
            {
                orc.dd();
                return Ok("");
            }
            catch (Exception ex) { throw ex; }
            finally { orc.Dispose(); }
        }

        [HttpPost]
        public IHttpActionResult get(HttpRequestMessage r)
        {
            Reppam p;
            RepCtl orc = new RepCtl(cfg.getInfo());
            try
            {
                p = JsonConvert.DeserializeObject<Reppam>(r.Content.ReadAsStringAsync().Result);
                return Ok(new
                {
                    results = new PTG_Result()
                    {
                        state = mResult.OK,
                        message = "",
                        rawdata = orc.getInfo(p.rpcode)
                    }
                });
            }
            catch (Exception ex) { throw ex; }
            finally { orc.Dispose(); }
        }

        [HttpPost]
        public IHttpActionResult Ops(HttpRequestMessage r)
        {
            RepCtl oRc = new RepCtl(cfg.getInfo(""));
            RepCmd orp;
            try {
               
                orp = JsonConvert.DeserializeObject<RepCmd>(r.Content.ReadAsStringAsync().Result);
                //return Ok(new { r.Content.ReadAsStringAsync().Result });
                return Ok(new {
                    results = new PTG_Result() {
                        state = mResult.OK, message = "",
                        rawdata = oRc.save(orp)
                    }
                });

            } catch (Exception ex) { throw ex; }
        }
    }
}