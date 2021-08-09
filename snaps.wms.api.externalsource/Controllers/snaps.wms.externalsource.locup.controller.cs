using System;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Snaps.Helpers;
using Snaps.Helpers.Json;
using Snaps.WMS.Interfaces;
using Snaps.WMS;
using Snaps.Helpers.Logging;
using System.IO;
using OfficeOpenXml;
namespace Snaps.WMS.Controllers {

    [ApiController] [Route("[controller]")]
    public class exsLocupController : ControllerBase {
        private IexsLocupService _cf;
        private readonly ILogger<exsLocupController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.externalsource.Locup";
        public exsLocupController(ILogger<exsLocupController> logger,IOptions<AppSettings> optn, IexsLocupService ocf ) { 
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
                return Ok(await _cf.lineLocupAsync(o));                                
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
            List<exsLocup> ls = new List<exsLocup>();
            Stream stream = null; StreamReader reader = null;
            string ln; Int32 rw = 0;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportCSVAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try {                             
                            while((ln = await reader.ReadLineAsync()) != null) {  
                               if (rw != 0 && ln.ToLower().Trim() !="spcarea,fltype,lscode,lsseq,lscodealt,lscodefull,lscodeid,lszone,lsaisle,lsbay,lslevel,tflow,lsdesc,lsloctype,rowops"){                             
                                    if (ln.Count(e => e == ',') != 14) { throw new Exception("Data incorrect line no." + rw + " result " + ln); }
                                    else { ls.Add(new exsLocup(orgcode,site,depot, ln)); }
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
                return Ok(await _cf.ImpexsLocupAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsLocup> ls = new List<exsLocup>();
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
                                if ( ws.Dimension.End.Column != 7 ) { throw new Exception("Excel column incorrect format "); }
                                else if (
                                    ws.Cells[1,0].Value.ToString().ToLower().Trim() != "spcarea"||
                                    ws.Cells[1,1].Value.ToString().ToLower().Trim() != "fltype"||
                                    ws.Cells[1,2].Value.ToString().ToLower().Trim() != "lscode"||
                                    ws.Cells[1,3].Value.ToString().ToLower().Trim() != "lsseq"||
                                    ws.Cells[1,4].Value.ToString().ToLower().Trim() != "lscodealt"||
                                    ws.Cells[1,5].Value.ToString().ToLower().Trim() != "lscodefull"||
                                    ws.Cells[1,6].Value.ToString().ToLower().Trim() != "lscodeid"||
                                    ws.Cells[1,7].Value.ToString().ToLower().Trim() != "lszone"||
                                    ws.Cells[1,8].Value.ToString().ToLower().Trim() != "lsaisle"||
                                    ws.Cells[1,9].Value.ToString().ToLower().Trim() != "lsbay"||
                                    ws.Cells[1,10].Value.ToString().ToLower().Trim() != "lslevel"||
                                    ws.Cells[1,11].Value.ToString().ToLower().Trim() != "tflow"||
                                    ws.Cells[1,12].Value.ToString().ToLower().Trim() != "lsdesc"||
                                    ws.Cells[1,13].Value.ToString().ToLower().Trim() != "lsloctype"
                                    ){ 
                                        throw new Exception("Excel column header incorrect format ");  
                                }
                            }else { 
                                try { 
                                    ls.Add(new exsLocup(orgcode,site,depot, 
                                    (ws.Cells[row,0].Value == null) ? "" : ws.Cells[row,0].Value.ToString().Trim(), //spcarea 
                                    (ws.Cells[row,1].Value == null) ? "" : ws.Cells[row,1].Value.ToString().Trim(), //fltype 
                                    (ws.Cells[row,2].Value == null) ? "" : ws.Cells[row,2].Value.ToString().Trim(), //lscode 
                                    (ws.Cells[row,3].Value == null) ? "" : ws.Cells[row,3].Value.ToString().Trim(), //lsseq 
                                    (ws.Cells[row,4].Value == null) ? "" : ws.Cells[row,4].Value.ToString().Trim(), //lscodealt 
                                    (ws.Cells[row,5].Value == null) ? "" : ws.Cells[row,5].Value.ToString().Trim(), //lscodefull 
                                    (ws.Cells[row,6].Value == null) ? "" : ws.Cells[row,6].Value.ToString().Trim(), //lscodeid 
                                    (ws.Cells[row,7].Value == null) ? "" : ws.Cells[row,7].Value.ToString().Trim(), //lszone 
                                    (ws.Cells[row,8].Value == null) ? "" : ws.Cells[row,8].Value.ToString().Trim(), //lsaisle 
                                    (ws.Cells[row,9].Value == null) ? "" : ws.Cells[row,9].Value.ToString().Trim(), //lsbay 
                                    (ws.Cells[row,10].Value == null) ? "" : ws.Cells[row,10].Value.ToString().Trim(), //lslevel 
                                    (ws.Cells[row,11].Value == null) ? "" : ws.Cells[row,11].Value.ToString().Trim(), //tflow 
                                    (ws.Cells[row,12].Value == null) ? "" : ws.Cells[row,12].Value.ToString().Trim(), //lsdesc 
                                    (ws.Cells[row,13].Value == null) ? "" : ws.Cells[row,13].Value.ToString().Trim() //lsloctype 
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
                return Ok(await _cf.ImpexsLocupAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsLocup> ls = new List<exsLocup>();
            Stream stream = null; StreamReader reader = null;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportJsonAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try { 
                            ls = JsonConvert.DeserializeObject<List<exsLocup>>(reader.ReadToEnd());
                        }catch (Exception exl) { 
                             throw new Exception("Data incorrect result " + exl.Message);
                        }                        
                        reader.Close();
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsLocupAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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