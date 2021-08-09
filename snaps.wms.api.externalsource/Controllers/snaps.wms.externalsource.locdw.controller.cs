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
    public class exsLocdwController : ControllerBase {
        private IexsLocdwService _cf;
        private readonly ILogger<exsLocdwController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.externalsource.locdw";
        public exsLocdwController(ILogger<exsLocdwController> logger,IOptions<AppSettings> optn, IexsLocdwService ocf ) { 
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
                return Ok(await _cf.lineLocdwAsync(o));                                
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
            List<exsLocdw> ls = new List<exsLocdw>();
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
                               if (rw != 0 && ln.ToLower().Trim() !="spcarea,fltype,lszone,lsaisle,lsbay,lslevel,lsloc,lsstack,lscode,lscodealt,lscodefull,lsdmlength,lsdmwidth,lsdmheight,lsmxweight,lsmxvolume,lsmxlength,lsmxwidth,lsmxheight,lsmxhuno,lsmnsafety,lsmixarticle,lsmixage,lsmixlotno,lsloctype,lsgaptop,lsgapleft,lsgapright,lsgapbuttom,lsstackable,lsdigit,spcthcode,spchuno,spcarticle,spcmnaging,spcmxaging,lsdirection,spcpathrecv,spcpathpick,spcpathdist,rowops"){                             
                                    if (ln.Count(e => e == ',') != 40) { throw new Exception("Data incorrect line no." + rw + " result " + ln); }
                                    else { ls.Add(new exsLocdw(orgcode,site,depot, ln)); }
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
                return Ok(await _cf.ImpexsLocdwAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsLocdw> ls = new List<exsLocdw>();
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
                                    ws.Cells[1,2].Value.ToString().ToLower().Trim() != "lszone"||
                                    ws.Cells[1,3].Value.ToString().ToLower().Trim() != "lsaisle"||
                                    ws.Cells[1,4].Value.ToString().ToLower().Trim() != "lsbay"||
                                    ws.Cells[1,5].Value.ToString().ToLower().Trim() != "lslevel"||
                                    ws.Cells[1,6].Value.ToString().ToLower().Trim() != "lsloc"||
                                    ws.Cells[1,7].Value.ToString().ToLower().Trim() != "lsstack"||
                                    ws.Cells[1,8].Value.ToString().ToLower().Trim() != "lscode"||
                                    ws.Cells[1,9].Value.ToString().ToLower().Trim() != "lscodealt"||
                                    ws.Cells[1,10].Value.ToString().ToLower().Trim() != "lscodefull"||
                                    ws.Cells[1,11].Value.ToString().ToLower().Trim() != "lsdmlength"||
                                    ws.Cells[1,12].Value.ToString().ToLower().Trim() != "lsdmwidth"||
                                    ws.Cells[1,13].Value.ToString().ToLower().Trim() != "lsdmheight"||
                                    ws.Cells[1,14].Value.ToString().ToLower().Trim() != "lsmxweight"||
                                    ws.Cells[1,15].Value.ToString().ToLower().Trim() != "lsmxvolume"||
                                    ws.Cells[1,16].Value.ToString().ToLower().Trim() != "lsmxlength"||
                                    ws.Cells[1,17].Value.ToString().ToLower().Trim() != "lsmxwidth"||
                                    ws.Cells[1,18].Value.ToString().ToLower().Trim() != "lsmxheight"||
                                    ws.Cells[1,19].Value.ToString().ToLower().Trim() != "lsmxhuno"||
                                    ws.Cells[1,20].Value.ToString().ToLower().Trim() != "lsmnsafety"||
                                    ws.Cells[1,21].Value.ToString().ToLower().Trim() != "lsmixarticle"||
                                    ws.Cells[1,22].Value.ToString().ToLower().Trim() != "lsmixage"||
                                    ws.Cells[1,23].Value.ToString().ToLower().Trim() != "lsmixlotno"||
                                    ws.Cells[1,24].Value.ToString().ToLower().Trim() != "lsloctype"||
                                    ws.Cells[1,25].Value.ToString().ToLower().Trim() != "lsgaptop"||
                                    ws.Cells[1,26].Value.ToString().ToLower().Trim() != "lsgapleft"||
                                    ws.Cells[1,27].Value.ToString().ToLower().Trim() != "lsgapright"||
                                    ws.Cells[1,28].Value.ToString().ToLower().Trim() != "lsgapbuttom"||
                                    ws.Cells[1,29].Value.ToString().ToLower().Trim() != "lsstackable"||
                                    ws.Cells[1,30].Value.ToString().ToLower().Trim() != "lsdigit"||
                                    ws.Cells[1,31].Value.ToString().ToLower().Trim() != "spcthcode"||
                                    ws.Cells[1,32].Value.ToString().ToLower().Trim() != "spchuno"||
                                    ws.Cells[1,33].Value.ToString().ToLower().Trim() != "spcarticle"||
                                    ws.Cells[1,34].Value.ToString().ToLower().Trim() != "spcmnaging"||
                                    ws.Cells[1,35].Value.ToString().ToLower().Trim() != "spcmxaging"||
                                    ws.Cells[1,36].Value.ToString().ToLower().Trim() != "lsdirection"||
                                    ws.Cells[1,37].Value.ToString().ToLower().Trim() != "spcpathrecv"||
                                    ws.Cells[1,38].Value.ToString().ToLower().Trim() != "spcpathpick"||
                                    ws.Cells[1,39].Value.ToString().ToLower().Trim() != "spcpathdist"||
                                    ws.Cells[1,40].Value.ToString().ToLower().Trim() != "rowops"
                                    ){ 
                                        throw new Exception("Excel column header incorrect format ");  
                                }
                            }else { 
                                try { 
                                    ls.Add(new exsLocdw(orgcode,site,depot, 
                                    (ws.Cells[row,0].Value == null) ? "" : ws.Cells[row,0].Value.ToString().Trim(), //spcarea 
                                    (ws.Cells[row,1].Value == null) ? "" : ws.Cells[row,1].Value.ToString().Trim(), //fltype 
                                    (ws.Cells[row,2].Value == null) ? "" : ws.Cells[row,2].Value.ToString().Trim(), //lszone 
                                    (ws.Cells[row,3].Value == null) ? "" : ws.Cells[row,3].Value.ToString().Trim(), //lsaisle 
                                    (ws.Cells[row,4].Value == null) ? "" : ws.Cells[row,4].Value.ToString().Trim(), //lsbay 
                                    (ws.Cells[row,5].Value == null) ? "" : ws.Cells[row,5].Value.ToString().Trim(), //lslevel 
                                    (ws.Cells[row,6].Value == null) ? "" : ws.Cells[row,6].Value.ToString().Trim(), //lsloc 
                                    (ws.Cells[row,7].Value == null) ? "" : ws.Cells[row,7].Value.ToString().Trim(), //lsstack 
                                    (ws.Cells[row,8].Value == null) ? "" : ws.Cells[row,8].Value.ToString().Trim(), //lscode 
                                    (ws.Cells[row,9].Value == null) ? "" : ws.Cells[row,9].Value.ToString().Trim(), //lscodealt 
                                    (ws.Cells[row,10].Value == null) ? "" : ws.Cells[row,10].Value.ToString().Trim(), //lscodefull 
                                    (ws.Cells[row,11].Value == null) ? "" : ws.Cells[row,11].Value.ToString().Trim(), //lsdmlength 
                                    (ws.Cells[row,12].Value == null) ? "" : ws.Cells[row,12].Value.ToString().Trim(), //lsdmwidth 
                                    (ws.Cells[row,13].Value == null) ? "" : ws.Cells[row,13].Value.ToString().Trim(), //lsdmheight 
                                    (ws.Cells[row,14].Value == null) ? "" : ws.Cells[row,14].Value.ToString().Trim(), //lsmxweight 
                                    (ws.Cells[row,15].Value == null) ? "" : ws.Cells[row,15].Value.ToString().Trim(), //lsmxvolume 
                                    (ws.Cells[row,16].Value == null) ? "" : ws.Cells[row,16].Value.ToString().Trim(), //lsmxlength 
                                    (ws.Cells[row,17].Value == null) ? "" : ws.Cells[row,17].Value.ToString().Trim(), //lsmxwidth 
                                    (ws.Cells[row,18].Value == null) ? "" : ws.Cells[row,18].Value.ToString().Trim(), //lsmxheight 
                                    (ws.Cells[row,19].Value == null) ? "" : ws.Cells[row,19].Value.ToString().Trim(), //lsmxhuno 
                                    (ws.Cells[row,20].Value == null) ? "" : ws.Cells[row,20].Value.ToString().Trim(), //lsmnsafety 
                                    (ws.Cells[row,21].Value == null) ? "" : ws.Cells[row,21].Value.ToString().Trim(), //lsmixarticle 
                                    (ws.Cells[row,22].Value == null) ? "" : ws.Cells[row,22].Value.ToString().Trim(), //lsmixage 
                                    (ws.Cells[row,23].Value == null) ? "" : ws.Cells[row,23].Value.ToString().Trim(), //lsmixlotno 
                                    (ws.Cells[row,24].Value == null) ? "" : ws.Cells[row,24].Value.ToString().Trim(), //lsloctype 
                                    (ws.Cells[row,25].Value == null) ? "" : ws.Cells[row,25].Value.ToString().Trim(), //lsgaptop 
                                    (ws.Cells[row,26].Value == null) ? "" : ws.Cells[row,26].Value.ToString().Trim(), //lsgapleft 
                                    (ws.Cells[row,27].Value == null) ? "" : ws.Cells[row,27].Value.ToString().Trim(), //lsgapright 
                                    (ws.Cells[row,28].Value == null) ? "" : ws.Cells[row,28].Value.ToString().Trim(), //lsgapbuttom 
                                    (ws.Cells[row,29].Value == null) ? "" : ws.Cells[row,29].Value.ToString().Trim(), //lsstackable 
                                    (ws.Cells[row,30].Value == null) ? "" : ws.Cells[row,30].Value.ToString().Trim(), //lsdigit 
                                    (ws.Cells[row,31].Value == null) ? "" : ws.Cells[row,31].Value.ToString().Trim(), //spcthcode 
                                    (ws.Cells[row,32].Value == null) ? "" : ws.Cells[row,32].Value.ToString().Trim(), //spchuno 
                                    (ws.Cells[row,33].Value == null) ? "" : ws.Cells[row,33].Value.ToString().Trim(), //spcarticle 
                                    (ws.Cells[row,34].Value == null) ? "" : ws.Cells[row,34].Value.ToString().Trim(), //spcmnaging 
                                    (ws.Cells[row,35].Value == null) ? "" : ws.Cells[row,35].Value.ToString().Trim(), //spcmxaging 
                                    (ws.Cells[row,36].Value == null) ? "" : ws.Cells[row,36].Value.ToString().Trim(), //lsdirection 
                                    (ws.Cells[row,37].Value == null) ? "" : ws.Cells[row,37].Value.ToString().Trim(), //spcpathrecv 
                                    (ws.Cells[row,38].Value == null) ? "" : ws.Cells[row,38].Value.ToString().Trim(), //spcpathpick 
                                    (ws.Cells[row,39].Value == null) ? "" : ws.Cells[row,39].Value.ToString().Trim(), //spcpathdist                                   
                                    (ws.Cells[row,40].Value == null) ? "" : ws.Cells[row,40].Value.ToString().Trim()  //rowops
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
                return Ok(await _cf.ImpexsLocdwAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsLocdw> ls = new List<exsLocdw>();
            Stream stream = null; StreamReader reader = null;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportJsonAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try { 
                            ls = JsonConvert.DeserializeObject<List<exsLocdw>>(reader.ReadToEnd());
                        }catch (Exception exl) { 
                             throw new Exception("Data incorrect result " + exl.Message);
                        }                        
                        reader.Close();
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsLocdwAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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