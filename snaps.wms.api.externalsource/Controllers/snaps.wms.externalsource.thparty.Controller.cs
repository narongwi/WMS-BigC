using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Snaps.Helpers;
using Snaps.Helpers.Json;
using Snaps.WMS.Interfaces;
using Snaps.Helpers.Logging;
using System.IO;
using OfficeOpenXml;
namespace Snaps.WMS.Controllers {

    [ApiController] [Route("[controller]")]
    public class exsTHPartyController : ControllerBase {
        private IexsTHPartyService _cf;
        private readonly ILogger<exsTHPartyController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.externalsource.thparty";
        public exsTHPartyController(ILogger<exsTHPartyController> logger,IOptions<AppSettings> optn, IexsTHPartyService ocf ) { 
            _log = logger; _optn = optn.Value; _cf = ocf; }
        
        //Task<List<exsFile>> findAsync(exsFile o)
        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("find/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> findAsync(String id, exsFile o,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"findAsync", id, Request.getIP(), app, accncode); }
                o.orgcode = orgcode; o.site = site; o.depot = depot; 
                return Ok(await _cf.findAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"findAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
            }
        } 

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("lines/{fileid}/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> lineAsync(String id, exsFile o,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lineAsync", id, Request.getIP(), app, accncode); }
                o.orgcode = orgcode; o.site = site; o.depot = depot; 
                return Ok(await _cf.lineTHPartyAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"lineAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
            }
        }        

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/CSV/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportCSVAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsThirdparty> ls = new List<exsThirdparty>();
            Stream stream = null; StreamReader reader = null;
            string ln; Int32 rw = 0;
            DateTimeOffset decstart = DateTimeOffset.Now;
            string headformat = "thtype,thbutype,thcode,thcodealt,vatcode,thname,thnamealt,addressln1,addressln2,addressln3,subdistrict,"+
            "district,city,country,postcode,region,telephone,email,thgroup,thcomment,throuteformat,plandelivery,naturalloss,mapaddress,rowops";
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportCSVAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try {                             
                            while((ln = await reader.ReadLineAsync()) != null) {  
                               if (rw != 0 && ln.ToLower().Trim() != headformat){                             
                                    if (ln.Count(e => e == ',') != 24) { throw new Exception("Data incorrect line no." + rw + " result " + ln); }
                                    else { ls.Add(new exsThirdparty(orgcode,site,depot, ln)); }
                                }
                                rw++;
                            }
                        }catch (Exception ex){ 
                            throw ex;
                        }
                        reader.Close();                       
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsTHPartyAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportCSVAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/excel/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportExcelAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsThirdparty> ls = new List<exsThirdparty>();
            Stream stream = null; StreamReader reader = null;
            ExcelPackage expck;
            DateTimeOffset decstart = DateTimeOffset.Now;
            Int32 rw = 0; Int32 rwmx = 0; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportExcelAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (expck = new ExcelPackage(stream)){ 
                        ExcelWorksheet ws = expck.Workbook.Worksheets["import"];  
                        rwmx = ws.Dimension.End.Row;
                        for (int row = 1; row <= rwmx; row++) {
                            if (rw == 0){ 
                                if ( ws.Dimension.End.Column != 25 ) { throw new Exception("Excel column incorrect format "); }
                                else if (
                                    ws.Cells[1,1].Value.ToString().ToLower().Trim() != "thtype"||
                                    ws.Cells[1,2].Value.ToString().ToLower().Trim() != "thbutype"||
                                    ws.Cells[1,3].Value.ToString().ToLower().Trim() != "thcode"||
                                    ws.Cells[1,4].Value.ToString().ToLower().Trim() != "thcodealt"||
                                    ws.Cells[1,5].Value.ToString().ToLower().Trim() != "vatcode"||
                                    ws.Cells[1,6].Value.ToString().ToLower().Trim() != "thname"||
                                    ws.Cells[1,7].Value.ToString().ToLower().Trim() != "thnamealt"||
                                    ws.Cells[1,8].Value.ToString().ToLower().Trim() != "addressln1"||
                                    ws.Cells[1,9].Value.ToString().ToLower().Trim() != "addressln2"||
                                    ws.Cells[1,10].Value.ToString().ToLower().Trim() != "addressln3"||
                                    ws.Cells[1,11].Value.ToString().ToLower().Trim() != "subdistrict"||
                                    ws.Cells[1,12].Value.ToString().ToLower().Trim() != "district"||
                                    ws.Cells[1,13].Value.ToString().ToLower().Trim() != "city"||
                                    ws.Cells[1,14].Value.ToString().ToLower().Trim() != "country"||
                                    ws.Cells[1,15].Value.ToString().ToLower().Trim() != "postcode"||
                                    ws.Cells[1,16].Value.ToString().ToLower().Trim() != "region"||
                                    ws.Cells[1,17].Value.ToString().ToLower().Trim() != "telephone"||
                                    ws.Cells[1,18].Value.ToString().ToLower().Trim() != "email"||
                                    ws.Cells[1,19].Value.ToString().ToLower().Trim() != "thgroup"||
                                    ws.Cells[1,20].Value.ToString().ToLower().Trim() != "thcomment"||
                                    ws.Cells[1,21].Value.ToString().ToLower().Trim() != "throuteformat"||
                                    ws.Cells[1,22].Value.ToString().ToLower().Trim() != "plandelivery"||
                                    ws.Cells[1,23].Value.ToString().ToLower().Trim() != "naturalloss"||
                                    ws.Cells[1,24].Value.ToString().ToLower().Trim() != "mapaddress"||
                                    ws.Cells[1,25].Value.ToString().ToLower().Trim() != "rowops"
                                    ){ 
                                        throw new Exception("Excel column header incorrect format ");  
                                }
                            }else { 
                                try { 
                                    ls.Add(new exsThirdparty(orgcode,site,depot, 
                                    (ws.Cells[row,1].Value == null) ? "" : ws.Cells[row,1].Value.ToString().Trim(), //thtype 
                                    (ws.Cells[row,2].Value == null) ? "" : ws.Cells[row,2].Value.ToString().Trim(), //thbutype 
                                    (ws.Cells[row,3].Value == null) ? "" : ws.Cells[row,3].Value.ToString().Trim(), //thcode 
                                    (ws.Cells[row,4].Value == null) ? "" : ws.Cells[row,4].Value.ToString().Trim(), //thcodealt 
                                    (ws.Cells[row,5].Value == null) ? "" : ws.Cells[row,5].Value.ToString().Trim(), //vatcode 
                                    (ws.Cells[row,6].Value == null) ? "" : ws.Cells[row,6].Value.ToString().Trim(), //thname 
                                    (ws.Cells[row,7].Value == null) ? "" : ws.Cells[row,7].Value.ToString().Trim(), //thnamealt 
                                    (ws.Cells[row,8].Value == null) ? "" : ws.Cells[row,8].Value.ToString().Trim(), //addressln1 
                                    (ws.Cells[row,9].Value == null) ? "" : ws.Cells[row,9].Value.ToString().Trim(), //addressln2 
                                    (ws.Cells[row,10].Value == null) ? "" : ws.Cells[row,10].Value.ToString().Trim(), //addressln3 
                                    (ws.Cells[row,11].Value == null) ? "" : ws.Cells[row,11].Value.ToString().Trim(), //subdistrict 
                                    (ws.Cells[row,12].Value == null) ? "" : ws.Cells[row,12].Value.ToString().Trim(), //district 
                                    (ws.Cells[row,13].Value == null) ? "" : ws.Cells[row,13].Value.ToString().Trim(), //city 
                                    (ws.Cells[row,14].Value == null) ? "" : ws.Cells[row,14].Value.ToString().Trim(), //country 
                                    (ws.Cells[row,15].Value == null) ? "" : ws.Cells[row,15].Value.ToString().Trim(), //postcode 
                                    (ws.Cells[row,16].Value == null) ? "" : ws.Cells[row,16].Value.ToString().Trim(), //region 
                                    (ws.Cells[row,17].Value == null) ? "" : ws.Cells[row,17].Value.ToString().Trim(), //telephone 
                                    (ws.Cells[row,18].Value == null) ? "" : ws.Cells[row,18].Value.ToString().Trim(), //email 
                                    (ws.Cells[row,19].Value == null) ? "" : ws.Cells[row,19].Value.ToString().Trim(), //thgroup 
                                    (ws.Cells[row,20].Value == null) ? "" : ws.Cells[row,20].Value.ToString().Trim(), //thcomment 
                                    (ws.Cells[row,21].Value == null) ? "" : ws.Cells[row,21].Value.ToString().Trim(), //throuteformat 
                                    (ws.Cells[row,22].Value == null) ? "" : ws.Cells[row,22].Value.ToString().Trim(), //plandelivery 
                                    (ws.Cells[row,23].Value == null) ? "" : ws.Cells[row,23].Value.ToString().Trim(), //naturalloss 
                                    (ws.Cells[row,24].Value == null) ? "" : ws.Cells[row,24].Value.ToString().Trim(), //mapaddress 
                                    (ws.Cells[row,25].Value == null) ? "" : ws.Cells[row,25].Value.ToString().Trim() //rowops 
                                    ));
                                }catch(Exception exl){ 
                                        throw new Exception("Data incorrect line no. " + row + " result " + exl.Message);
                                }
                            }
                            rw++;
                        }   
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsTHPartyAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportExcelAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/json/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportJsonAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsThirdparty> ls = new List<exsThirdparty>();
            Stream stream = null; StreamReader reader = null;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportJsonAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try { 
                            ls = JsonConvert.DeserializeObject<List<exsThirdparty>>(reader.ReadToEnd());
                        }catch (Exception exl) { 
                             throw new Exception("Data incorrect result " + exl.Message);
                        }                        
                        reader.Close();
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsTHPartyAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {
                reader.Close();
                stream.Close();
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportJsonAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }
    }
}