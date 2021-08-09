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
    public class exsBarcodeController : ControllerBase {
        private IexsBarcodeService _cf;
        private readonly ILogger<exsBarcodeController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.externalsource.barcode";
        public exsBarcodeController(ILogger<exsBarcodeController> logger,IOptions<AppSettings> optn, IexsBarcodeService ocf ) { 
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
                return Ok(await _cf.lineBarcodeAsync(o));                                
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
            List<exsBarcode> ls = new List<exsBarcode>();
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
                               if (rw != 0 && ln.ToLower().Trim() !="barcode,thcode,article,pv,lv,rowops,barops"){                             
                                    if (ln.Count(e => e == ',') != 6) { throw new Exception("Data incorrect line no." + rw + " result " + ln); }
                                    else { ls.Add(new exsBarcode(orgcode,site,depot, ln)); }
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
                return Ok(await _cf.ImpexsBarcodeAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsBarcode> ls = new List<exsBarcode>();
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
                                    ws.Cells[1,1].Value.ToString().ToLower().Trim() != "barcode"||
                                    ws.Cells[1,2].Value.ToString().ToLower().Trim() != "thcode" ||
                                    ws.Cells[1,3].Value.ToString().ToLower().Trim() != "article"||
                                    ws.Cells[1,4].Value.ToString().ToLower().Trim() != "pv"     ||
                                    ws.Cells[1,5].Value.ToString().ToLower().Trim() != "lv"     ||
                                    ws.Cells[1,6].Value.ToString().ToLower().Trim() != "rowops" ||
                                    ws.Cells[1,7].Value.ToString().ToLower().Trim() != "barops"
                                    ){ 
                                        throw new Exception("Excel column header incorrect format ");  
                                }
                            }else { 
                                try { 
                                    ls.Add(new exsBarcode(orgcode,site,depot, 
                                        (ws.Cells[row,1].Value == null) ? "" : ws.Cells[row,1].Value.ToString().Trim(), //Barcode 
                                        (ws.Cells[row,2].Value == null) ? "" : ws.Cells[row,2].Value.ToString().Trim(), //THCode
                                        (ws.Cells[row,3].Value == null) ? "" : ws.Cells[row,3].Value.ToString().Trim(), //article 
                                        (ws.Cells[row,4].Value == null) ? "" : ws.Cells[row,4].Value.ToString().Trim(), //pv 
                                        (ws.Cells[row,5].Value == null) ? "" : ws.Cells[row,5].Value.ToString().Trim(), //lv, 
                                        (ws.Cells[row,6].Value == null) ? "" : ws.Cells[row,6].Value.ToString().Trim(), //rowops
                                        (ws.Cells[row,7].Value == null) ? "" : ws.Cells[row,7].Value.ToString().Trim()  //barops
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
                return Ok(await _cf.ImpexsBarcodeAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsBarcode> ls = new List<exsBarcode>();
            Stream stream = null; StreamReader reader = null;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportJsonAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try { 
                            ls = JsonConvert.DeserializeObject<List<exsBarcode>>(reader.ReadToEnd());
                        }catch (Exception exl) { 
                             throw new Exception("Data incorrect result " + exl.Message);
                        }                        
                        reader.Close();
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsBarcodeAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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