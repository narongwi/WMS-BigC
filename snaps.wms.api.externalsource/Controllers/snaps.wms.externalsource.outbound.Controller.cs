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
    public class exsOutboundController : ControllerBase {
        private IexsOutboundService _cf;
        private readonly ILogger<exsOutboundController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.externalsource.Outbound";
        public exsOutboundController(ILogger<exsOutboundController> logger,IOptions<AppSettings> optn, IexsOutboundService ocf ) { 
            _log = logger; _optn = optn.Value; _cf = ocf; }
        
        //Task<List<exsFile>> findAsync(exsFile o)
        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("find/{id}")]
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

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("lines/Outbound/{fileid}/{id}")]
        public async Task<IActionResult> lineOutboundAsync(String id, exsFile o,
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
                return Ok(await _cf.lineOutboundAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"lineAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
            }
        }        

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("lines/Outbouln/{fileid}/{id}")]
        public async Task<IActionResult> lineOutboulnAsync(String id, exsFile o,
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
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"lineOutboulnAsync", id, Request.getIP(), app, accncode); }
                o.orgcode = orgcode; o.site = site; o.depot = depot; 
                return Ok(await _cf.lineOutboulnAsync(o));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"lineOutboulnAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
            }
        }        

        //Header 
        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/Outbound/CSV/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportCSVOutboundAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsOutbound> ls = new List<exsOutbound>();
            Stream stream = null; StreamReader reader = null;
            string ln; Int32 rw = 0;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportCSVOutboundAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try {                             
                            while((ln = await reader.ReadLineAsync()) != null) {  
                               if (rw != 0 && ln.ToLower().Trim() !="ouorder,outype,ousubtype,thcode,dateorder,dateprep,dateexpire,oupriority,ouflag,oupromo,dropship,stocode,stoname,stoaddressln1,stoaddressln2,stoaddressln3,stosubdistict,stodistrict,stocity,stocountry,stopostcode,stomobile,stoemail,inorder,rowops"){                             
                                    if (ln.Count(e => e == ',') != 24) { throw new Exception("Data incorrect line no." + rw + " result " + ln); }
                                    else { ls.Add(new exsOutbound(orgcode,site,depot, ln)); }
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
                return Ok(await _cf.ImpexsOutboundAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportCSVOutboundAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/Outbound/excel/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportExcelOutboundAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsOutbound> ls = new List<exsOutbound>();
            Stream stream = null; StreamReader reader = null;
            ExcelPackage expck;
            DateTimeOffset decstart = DateTimeOffset.Now;
            Int32 rw = 0; Int32 rwmx = 0; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportExcelOutboundAsync", id, Request.getIP(), app, accncode); }
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
                                    ws.Cells[1,1].Value.ToString().ToLower().Trim() != "ouorder"        ||
                                    ws.Cells[1,2].Value.ToString().ToLower().Trim() != "outype"         ||
                                    ws.Cells[1,3].Value.ToString().ToLower().Trim() != "ousubtype"      ||
                                    ws.Cells[1,4].Value.ToString().ToLower().Trim() != "thcode"         ||
                                    ws.Cells[1,5].Value.ToString().ToLower().Trim() != "dateorder"      ||
                                    ws.Cells[1,6].Value.ToString().ToLower().Trim() != "dateprep"       ||
                                    ws.Cells[1,7].Value.ToString().ToLower().Trim() != "dateexpire"     ||
                                    ws.Cells[1,8].Value.ToString().ToLower().Trim() != "oupriority"     ||
                                    ws.Cells[1,9].Value.ToString().ToLower().Trim() != "ouflag"         ||
                                    ws.Cells[1,10].Value.ToString().ToLower().Trim() != "oupromo"       ||
                                    ws.Cells[1,11].Value.ToString().ToLower().Trim() != "dropship"      ||
                                    ws.Cells[1,12].Value.ToString().ToLower().Trim() != "stocode"       ||
                                    ws.Cells[1,13].Value.ToString().ToLower().Trim() != "stoname"       ||
                                    ws.Cells[1,14].Value.ToString().ToLower().Trim() != "stoaddressln1" ||
                                    ws.Cells[1,15].Value.ToString().ToLower().Trim() != "stoaddressln2" ||
                                    ws.Cells[1,16].Value.ToString().ToLower().Trim() != "stoaddressln3" ||
                                    ws.Cells[1,17].Value.ToString().ToLower().Trim() != "stosubdistict" ||
                                    ws.Cells[1,18].Value.ToString().ToLower().Trim() != "stodistrict"   ||
                                    ws.Cells[1,19].Value.ToString().ToLower().Trim() != "stocity"       ||
                                    ws.Cells[1,20].Value.ToString().ToLower().Trim() != "stocountry"    ||
                                    ws.Cells[1,21].Value.ToString().ToLower().Trim() != "stopostcode"   ||
                                    ws.Cells[1,22].Value.ToString().ToLower().Trim() != "stomobile"     ||
                                    ws.Cells[1,23].Value.ToString().ToLower().Trim() != "stoemail"      ||
                                    ws.Cells[1,24].Value.ToString().ToLower().Trim() != "inorder"       ||
                                    ws.Cells[1,25].Value.ToString().ToLower().Trim() != "rowops"
                                    ){ 
                                        throw new Exception("Excel column header incorrect format ");  
                                }
                            }else { 
                                try { 
                                    ls.Add(new exsOutbound(orgcode,site,depot, 
                                    (ws.Cells[row,1].Value == null) ? "" : ws.Cells[row,1].Value.ToString().Trim(), //ouorder 
                                    (ws.Cells[row,2].Value == null) ? "" : ws.Cells[row,2].Value.ToString().Trim(), //outype 
                                    (ws.Cells[row,3].Value == null) ? "" : ws.Cells[row,3].Value.ToString().Trim(), //ousubtype 
                                    (ws.Cells[row,4].Value == null) ? "" : ws.Cells[row,4].Value.ToString().Trim(), //thcode 
                                    (ws.Cells[row,5].Value == null) ? "" : ws.Cells[row,5].Value.ToString().Trim(), //dateorder 
                                    (ws.Cells[row,6].Value == null) ? "" : ws.Cells[row,6].Value.ToString().Trim(), //dateprep 
                                    (ws.Cells[row,7].Value == null) ? "" : ws.Cells[row,7].Value.ToString().Trim(), //dateexpire 
                                    (ws.Cells[row,8].Value == null) ? "" : ws.Cells[row,8].Value.ToString().Trim(), //oupriority 
                                    (ws.Cells[row,9].Value == null) ? "" : ws.Cells[row,9].Value.ToString().Trim(), //ouflag 
                                    (ws.Cells[row,10].Value == null) ? "" : ws.Cells[row,10].Value.ToString().Trim(), //oupromo 
                                    (ws.Cells[row,11].Value == null) ? "" : ws.Cells[row,11].Value.ToString().Trim(), //dropship 
                                    (ws.Cells[row,12].Value == null) ? "" : ws.Cells[row,12].Value.ToString().Trim(), //stocode 
                                    (ws.Cells[row,13].Value == null) ? "" : ws.Cells[row,13].Value.ToString().Trim(), //stoname 
                                    (ws.Cells[row,14].Value == null) ? "" : ws.Cells[row,14].Value.ToString().Trim(), //stoaddressln1 
                                    (ws.Cells[row,15].Value == null) ? "" : ws.Cells[row,15].Value.ToString().Trim(), //stoaddressln2 
                                    (ws.Cells[row,16].Value == null) ? "" : ws.Cells[row,16].Value.ToString().Trim(), //stoaddressln3 
                                    (ws.Cells[row,17].Value == null) ? "" : ws.Cells[row,17].Value.ToString().Trim(), //stosubdistict 
                                    (ws.Cells[row,18].Value == null) ? "" : ws.Cells[row,18].Value.ToString().Trim(), //stodistrict 
                                    (ws.Cells[row,19].Value == null) ? "" : ws.Cells[row,19].Value.ToString().Trim(), //stocity 
                                    (ws.Cells[row,20].Value == null) ? "" : ws.Cells[row,20].Value.ToString().Trim(), //stocountry 
                                    (ws.Cells[row,21].Value == null) ? "" : ws.Cells[row,21].Value.ToString().Trim(), //stopostcode 
                                    (ws.Cells[row,22].Value == null) ? "" : ws.Cells[row,22].Value.ToString().Trim(), //stomobile 
                                    (ws.Cells[row,23].Value == null) ? "" : ws.Cells[row,23].Value.ToString().Trim(), //stoemail 
                                    (ws.Cells[row,24].Value == null) ? "" : ws.Cells[row,24].Value.ToString().Trim(), //inorder 
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
                return Ok(await _cf.ImpexsOutboundAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportExcelOutboundAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/Outbound/json/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportJsonOutboundAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsOutbound> ls = new List<exsOutbound>();
            Stream stream = null; StreamReader reader = null;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportJsonOutboundAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try { 
                            ls = JsonConvert.DeserializeObject<List<exsOutbound>>(reader.ReadToEnd());
                        }catch (Exception exl) { 
                             throw new Exception("Data incorrect result " + exl.Message);
                        }                        
                        reader.Close();
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsOutboundAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {
                reader.Close();
                stream.Close();
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportJsonOutboundAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }

        //Line 
        
        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/Outbouln/CSV/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportCSVOutboulnAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsOutbouln> ls = new List<exsOutbouln>();
            Stream stream = null; StreamReader reader = null;
            string ln; Int32 rw = 0;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportCSVOutboulnAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try {                             
                            while((ln = await reader.ReadLineAsync()) != null) {  
                               if (rw != 0 && ln.ToLower().Trim() !="ouorder,ouln,ourefno,ourefln,inorder,article,pv,lv,unitops,qtysku,qtypu,qtyweight,spcselect,batchno,lotno,datemfg,dateexp,serialno,disthcode,rowops"){                             
                                    if (ln.Count(e => e == ',') != 19) { throw new Exception("Data incorrect line no." + rw + " result " + ln); }
                                    else { ls.Add(new exsOutbouln(orgcode,site,depot, ln)); }
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
                return Ok(await _cf.ImpexsOutboulnAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportCSVOutboulnAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/Outbouln/excel/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportExcelOutboulnAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsOutbouln> ls = new List<exsOutbouln>();
            Stream stream = null; StreamReader reader = null;
            ExcelPackage expck;
            DateTimeOffset decstart = DateTimeOffset.Now;
            Int32 rw = 0; Int32 rwmx = 0; 
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportExcelOutboulnAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (expck = new ExcelPackage(stream)){ 
                        ExcelWorksheet ws = expck.Workbook.Worksheets["import"];  
                        rwmx = ws.Dimension.End.Row;
                        for (int row = 1; row <= rwmx; row++) {
                            if (rw == 0){ 
                                if ( ws.Dimension.End.Column != 20 ) { throw new Exception("Excel column incorrect format "); }
                                else if (
                                    ws.Cells[1,1].Value.ToString().ToLower().Trim() != "ouorder"    ||
                                    ws.Cells[1,2].Value.ToString().ToLower().Trim() != "ouln"       ||
                                    ws.Cells[1,3].Value.ToString().ToLower().Trim() != "ourefno"    ||
                                    ws.Cells[1,4].Value.ToString().ToLower().Trim() != "ourefln"    ||
                                    ws.Cells[1,5].Value.ToString().ToLower().Trim() != "inorder"    ||
                                    ws.Cells[1,6].Value.ToString().ToLower().Trim() != "article"    ||
                                    ws.Cells[1,7].Value.ToString().ToLower().Trim() != "pv"         ||
                                    ws.Cells[1,8].Value.ToString().ToLower().Trim() != "lv"         ||
                                    ws.Cells[1,9].Value.ToString().ToLower().Trim() != "unitops"    ||
                                    ws.Cells[1,10].Value.ToString().ToLower().Trim() != "qtysku"    ||
                                    ws.Cells[1,11].Value.ToString().ToLower().Trim() != "qtypu"     ||
                                    ws.Cells[1,12].Value.ToString().ToLower().Trim() != "qtyweight" ||
                                    ws.Cells[1,13].Value.ToString().ToLower().Trim() != "spcselect" ||
                                    ws.Cells[1,14].Value.ToString().ToLower().Trim() != "batchno"   ||
                                    ws.Cells[1,15].Value.ToString().ToLower().Trim() != "lotno"     ||
                                    ws.Cells[1,16].Value.ToString().ToLower().Trim() != "datemfg"   ||
                                    ws.Cells[1,17].Value.ToString().ToLower().Trim() != "dateexp"   ||
                                    ws.Cells[1,18].Value.ToString().ToLower().Trim() != "serialno"  ||
                                    ws.Cells[1,19].Value.ToString().ToLower().Trim() != "disthcode" ||
                                    ws.Cells[1,20].Value.ToString().ToLower().Trim() != "rowops"      
                                    ){ 
                                        throw new Exception("Excel column header incorrect format ");  
                                }
                            }else { 
                                try { 
                                    ls.Add(new exsOutbouln(orgcode,site,depot, 
                                    (ws.Cells[row,1].Value == null) ? "" : ws.Cells[row,1].Value.ToString().Trim(), //ouorder 
                                    (ws.Cells[row,2].Value == null) ? "" : ws.Cells[row,2].Value.ToString().Trim(), //ouln 
                                    (ws.Cells[row,3].Value == null) ? "" : ws.Cells[row,3].Value.ToString().Trim(), //ourefno 
                                    (ws.Cells[row,4].Value == null) ? "" : ws.Cells[row,4].Value.ToString().Trim(), //ourefln 
                                    (ws.Cells[row,5].Value == null) ? "" : ws.Cells[row,5].Value.ToString().Trim(), //inorder 
                                    (ws.Cells[row,6].Value == null) ? "" : ws.Cells[row,6].Value.ToString().Trim(), //article 
                                    (ws.Cells[row,7].Value == null) ? "" : ws.Cells[row,7].Value.ToString().Trim(), //pv 
                                    (ws.Cells[row,8].Value == null) ? "" : ws.Cells[row,8].Value.ToString().Trim(), //lv 
                                    (ws.Cells[row,9].Value == null) ? "" : ws.Cells[row,9].Value.ToString().Trim(), //unitops 
                                    (ws.Cells[row,10].Value == null) ? "" : ws.Cells[row,10].Value.ToString().Trim(), //qtysku 
                                    (ws.Cells[row,11].Value == null) ? "" : ws.Cells[row,11].Value.ToString().Trim(), //qtypu 
                                    (ws.Cells[row,12].Value == null) ? "" : ws.Cells[row,12].Value.ToString().Trim(), //qtyweight 
                                    (ws.Cells[row,13].Value == null) ? "" : ws.Cells[row,13].Value.ToString().Trim(), //spcselect 
                                    (ws.Cells[row,14].Value == null) ? "" : ws.Cells[row,14].Value.ToString().Trim(), //batchno 
                                    (ws.Cells[row,15].Value == null) ? "" : ws.Cells[row,15].Value.ToString().Trim(), //lotno 
                                    (ws.Cells[row,16].Value == null) ? "" : ws.Cells[row,16].Value.ToString().Trim(), //datemfg 
                                    (ws.Cells[row,17].Value == null) ? "" : ws.Cells[row,17].Value.ToString().Trim(), //dateexp 
                                    (ws.Cells[row,18].Value == null) ? "" : ws.Cells[row,18].Value.ToString().Trim(), //serialno 
                                    (ws.Cells[row,19].Value == null) ? "" : ws.Cells[row,19].Value.ToString().Trim(), //disthcode 
                                    (ws.Cells[row,20].Value == null) ? "" : ws.Cells[row,20].Value.ToString().Trim() //rowops 
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
                return Ok(await _cf.ImpexsOutboulnAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {         
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportExcelOutboulnAsync",rqid:id,ob:"o"));       
                return BadRequest(exr.SnapsBadRequest());
            }finally { 
                if (_log.IsEnabled(LogLevel.Debug)){ p.snap(); _log.LogDebug(p.toJson()); p.Dispose(); ps.Dispose(); }
                if (stream !=null) { await stream.DisposeAsync(); }
                if (reader !=null) { reader.Dispose(); }
                ls.Clear(); ls = null; 
            }
        }

        [Authorize(Roles = "Admin,Importer,Snaps")] [HttpPost("Upload/Outbouln/json/{id}"), DisableRequestSizeLimit]
        public async Task<IActionResult> ImportJsonOutboulnAsync(String id, [FromForm]IFormFile ofile,
                    [FromHeader(Name="site")] String site,
                    [FromHeader(Name="depot")] String depot,
                    [FromHeader(Name="accncode")] String accncode,
                    [FromHeader(Name="accscode")] String accscode,
                    [FromHeader(Name="orgcode")] String orgcode, 
                    [FromHeader(Name="lang")] String lng
                    ) {
            Process ps = null;  SnapsLogDbg p = null;  
            List<exsOutbouln> ls = new List<exsOutbouln>();
            Stream stream = null; StreamReader reader = null;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportJsonOutboulnAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try { 
                            ls = JsonConvert.DeserializeObject<List<exsOutbouln>>(reader.ReadToEnd());
                        }catch (Exception exl) { 
                             throw new Exception("Data incorrect result " + exl.Message);
                        }                        
                        reader.Close();
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsOutboulnAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
            }catch (Exception exr) {
                reader.Close();
                stream.Close();
                _log.LogError(exr.SnapsLogExc(Request.getIP(),accncode,app,"ImportJsonOutboulnAsync",rqid:id,ob:"o"));       
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