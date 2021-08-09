using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Snaps.Helpers.DbContext.SQLServer;

namespace BGC.MIS.WMS.PTG.Controllers
{
    public class LovsController : ApiController
    {
        // GET: Result
        [HttpPost]
        public IHttpActionResult getlsRep(HttpRequestMessage r)
        {
            RepCtl oRc = new RepCtl(cfg.getReps(""));
            Reppam opm = null;
            try {
                opm = JsonConvert.DeserializeObject<Reppam>(r.Content.ReadAsStringAsync().Result);
                return Ok(oRc.getReps(opm.dc, opm.depot, opm.rlcode));
            } catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new PTG_Result(e: ex));
            } finally { oRc.Dispose(); }
        }
        [HttpPost]
        public IHttpActionResult getlsPar(HttpRequestMessage r)
        {
            RepCtl oRc = new RepCtl(cfg.getReps(""));
            Reppam opm = null;
            try {
                opm = JsonConvert.DeserializeObject<Reppam>(r.Content.ReadAsStringAsync().Result);
                return Ok(oRc.getExPars(opm.dc, opm.depot, opm.rpcode));
            } catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new PTG_Result(e: ex));
            } finally { oRc.Dispose(); }
        }
        [HttpPost]
        public IHttpActionResult getCategory(HttpRequestMessage r)
        {
            RepCtl oRc = new RepCtl(cfg.getInfo(""));
            try {
                oRc = new RepCtl(cfg.getInfo(""));
                return Ok(oRc.getCategory());
            } catch (Exception ex) {
                // throw ex;
                return Content(HttpStatusCode.BadRequest, new PTG_Result(e: ex));
            } finally { oRc.Dispose(); }
        }
        [HttpPost]
        public IHttpActionResult getPamType(HttpRequestMessage r)
        {
            RepCtl oRc = new RepCtl(cfg.getInfo(""));
            try {
                return Ok(oRc.getPamType());
            } catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new PTG_Result(e: ex));
            } finally { oRc.Dispose(); }
        }
        [HttpPost]
        public IHttpActionResult getRepSource(HttpRequestMessage r)
        {
            RepCtl oRc = new RepCtl(cfg.getInfo(""));
            try {
                return Ok(oRc.getRepSource());
            } catch (Exception ex) {
                return Content(HttpStatusCode.BadRequest, new PTG_Result(e: ex));
            } finally { oRc.Dispose(); }
        }
        //[HttpPost]
        //public IHttpActionResult getPCVal(HttpRequestMessage r)
        //{

        //}

        [HttpGet]
        public IHttpActionResult test()
        {
            return Ok(new { results = new PTG_Result("Test call success") });
        }

        [HttpPost]
        public IHttpActionResult getResult(HttpRequestMessage r)
        {
            RepCtl op = new RepCtl(cfg.getRepsExc());
            RepCmd cm = new RepCmd();
            List<RepExpar> pm = new List<RepExpar>();
            try {               
                pm = JsonConvert.DeserializeObject<List<RepExpar>>(r.Content.ReadAsStringAsync().Result);
               // cm = op.getInfo(pm.First().rpcode);
                return new eBookResult(new MemoryStream(op.opsExcel(pm)), Request, pm.First().rpcode + "_"+DateTime.Now.ToString("yyyy-MM-dd HHmm", CultureInfo.CreateSpecificCulture("en-GB")) + ".xlsx");
            }catch(Exception ex) {
                throw ex;
            }
           
            //Response.Clear();
            //Response.ClearContent();
            //Response.ClearHeaders();
            //Response.Cookies.Clear();
            //Response.Cache.SetCacheability(HttpCacheability.Private);
            //Response.CacheControl = "private";
            //Response.Charset = System.Text.UTF8Encoding.UTF8.WebName;
            //Response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
            //Response.AppendHeader("Pragma", "cache");
            //Response.AppendHeader("Expires", "60");
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", string.Format("attachment;filename={0}_{1}.xlsx", oRp.First().rpcode, DateTime.Now.ToString("dd-MMM-yyyy HHmm", CultureInfo.CreateSpecificCulture("en-GB"))));
            //Response.BufferOutput = false;
            //Response.BinaryWrite(oRc.opsExcel(oRp));
            //Response.End();
            //return Content("");
            //var dataBytes = File.ReadAllBytes("D:\\test.xlsx");
            //adding bytes to memory stream   
            //var dataStream = ;
            
        }



        public class eBookResult : IHttpActionResult
        {
            MemoryStream bookStuff;
            string PdfFileName;
            HttpRequestMessage httpRequestMessage;
            HttpResponseMessage httpResponseMessage;
            public eBookResult(MemoryStream data, HttpRequestMessage request, string filename)
            {
                bookStuff = data;
                httpRequestMessage = request;
                PdfFileName = filename;
            }
            public System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
            {
                httpResponseMessage = httpRequestMessage.CreateResponse(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(bookStuff);
                httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = PdfFileName;
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                return System.Threading.Tasks.Task.FromResult(httpResponseMessage);
            }
        }
    }
}