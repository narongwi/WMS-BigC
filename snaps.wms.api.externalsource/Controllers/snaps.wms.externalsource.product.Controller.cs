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
    public class exsProductController : ControllerBase {
        private IexsProductService _cf;
        private readonly ILogger<exsProductController> _log;
        private readonly AppSettings _optn;
        private readonly String app = "api.externalsource.Product";
        public exsProductController(ILogger<exsProductController> logger,IOptions<AppSettings> optn, IexsProductService ocf ) { 
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
                return Ok(await _cf.lineProductAsync(o));                                
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
            List<exsProduct> ls = new List<exsProduct>();
            Stream stream = null; StreamReader reader = null;
            string ln; Int32 rw = 0;
            DateTimeOffset decstart = DateTimeOffset.Now;
            string headformat = "article,articletype,pv,lv,description,descalt,thcode,dlcall,dlcfactory,dlcwarehouse,dlcshop,dlconsumer,hdivison,"+ 
            "hdepartment,hsubdepart,hclass,hsubclass,typemanage,unitmanage,unitdesc,unitreceipt,unitprep,unitsale,unitstock,unitweight,unitdimension,"+ 
            "unitvolume,hucode,rtoskuofpu,rtoskuofipck,rtoskuofpck,rtoskuoflayer,rtoskuofhu,rtoipckofpck,rtopckoflayer,rtolayerofhu,innaturalloss,"+
            "ounaturalloss,costinbound,costoutbound,costavg,skulength,skuwidth,skuheight,skugrossweight,skuweight,skuvolume,pulength,puwidth,puheight,"+
            "pugrossweight,puweight,puvolume,ipcklength,ipckwidth,ipckheight,ipckgrossweight,ipckweight,ipckvolume,pcklength,pckwidth,pckheight,"+
            "pckgrossweight,pckweight,pckvolume,layerlength,layerwidth,layerheight,layergrossweight,layerweight,layervolume,hulength,huwidth,huheight,"+
            "hugrossweight,huweight,huvolume,isdangerous,ishighvalue,isfastmove,isslowmove,isprescription,isdlc,ismaterial,isunique,isalcohol,"+
            "istemperature,isdynamicpick,ismixingprep,isfinishgoods,isnaturalloss,isbatchno,ismeasurement,roomtype,tempmin,tempmax,alcmanage,"+
            "alccategory,alccontent,alccolor,dangercategory,dangerlevel,stockthresholdmin,stockthresholdmax,spcrecvzone,spcrecvaisle,spcrecvbay,"+
            "spcrecvlevel,spcrecvlocation,spcprepzone,spcdistzone,spcdistshare,spczonedelv,rowops";
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportCSVAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try {                             
                            while((ln = await reader.ReadLineAsync()) != null) {  
                               if (rw != 0 && ln.ToLower().Trim() != headformat){                             
                                    if (ln.Count(e => e == ',') != 113) { throw new Exception("Data incorrect line no." + rw + " result " + ln); }
                                    else { ls.Add(new exsProduct(orgcode,site,depot, ln)); }
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
                return Ok(await _cf.ImpexsProductAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsProduct> ls = new List<exsProduct>();
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
                                if ( ws.Dimension.End.Column != 114 ) { throw new Exception("Excel column incorrect format "); }
                                else if (
                                    ws.Cells[1,1].Value.ToString().ToLower().Trim() != "article"||
                                    ws.Cells[1,2].Value.ToString().ToLower().Trim() != "articletype"||
                                    ws.Cells[1,3].Value.ToString().ToLower().Trim() != "pv"||
                                    ws.Cells[1,4].Value.ToString().ToLower().Trim() != "lv"||
                                    ws.Cells[1,5].Value.ToString().ToLower().Trim() != "description"||
                                    ws.Cells[1,6].Value.ToString().ToLower().Trim() != "descalt"||
                                    ws.Cells[1,7].Value.ToString().ToLower().Trim() != "thcode"||
                                    ws.Cells[1,8].Value.ToString().ToLower().Trim() != "dlcall"||
                                    ws.Cells[1,9].Value.ToString().ToLower().Trim() != "dlcfactory"||
                                    ws.Cells[1,10].Value.ToString().ToLower().Trim() != "dlcwarehouse"||
                                    ws.Cells[1,11].Value.ToString().ToLower().Trim() != "dlcshop"||
                                    ws.Cells[1,12].Value.ToString().ToLower().Trim() != "dlconsumer"||
                                    ws.Cells[1,13].Value.ToString().ToLower().Trim() != "hdivison"||
                                    ws.Cells[1,14].Value.ToString().ToLower().Trim() != "hdepartment"||
                                    ws.Cells[1,15].Value.ToString().ToLower().Trim() != "hsubdepart"||
                                    ws.Cells[1,16].Value.ToString().ToLower().Trim() != "hclass"||
                                    ws.Cells[1,17].Value.ToString().ToLower().Trim() != "hsubclass"||
                                    ws.Cells[1,18].Value.ToString().ToLower().Trim() != "typemanage"||
                                    ws.Cells[1,19].Value.ToString().ToLower().Trim() != "unitmanage"||
                                    ws.Cells[1,20].Value.ToString().ToLower().Trim() != "unitdesc"||
                                    ws.Cells[1,21].Value.ToString().ToLower().Trim() != "unitreceipt"||
                                    ws.Cells[1,22].Value.ToString().ToLower().Trim() != "unitprep"||
                                    ws.Cells[1,23].Value.ToString().ToLower().Trim() != "unitsale"||
                                    ws.Cells[1,24].Value.ToString().ToLower().Trim() != "unitstock"||
                                    ws.Cells[1,25].Value.ToString().ToLower().Trim() != "unitweight"||
                                    ws.Cells[1,26].Value.ToString().ToLower().Trim() != "unitdimension"||
                                    ws.Cells[1,27].Value.ToString().ToLower().Trim() != "unitvolume"||
                                    ws.Cells[1,28].Value.ToString().ToLower().Trim() != "hucode"||
                                    ws.Cells[1,29].Value.ToString().ToLower().Trim() != "rtoskuofpu"||
                                    ws.Cells[1,30].Value.ToString().ToLower().Trim() != "rtoskuofipck"||
                                    ws.Cells[1,31].Value.ToString().ToLower().Trim() != "rtoskuofpck"||
                                    ws.Cells[1,32].Value.ToString().ToLower().Trim() != "rtoskuoflayer"||
                                    ws.Cells[1,33].Value.ToString().ToLower().Trim() != "rtoskuofhu"||
                                    ws.Cells[1,34].Value.ToString().ToLower().Trim() != "rtoipckofpck"||
                                    ws.Cells[1,35].Value.ToString().ToLower().Trim() != "rtopckoflayer"||
                                    ws.Cells[1,36].Value.ToString().ToLower().Trim() != "rtolayerofhu"||
                                    ws.Cells[1,37].Value.ToString().ToLower().Trim() != "innaturalloss"||
                                    ws.Cells[1,38].Value.ToString().ToLower().Trim() != "ounaturalloss"||
                                    ws.Cells[1,39].Value.ToString().ToLower().Trim() != "costinbound"||
                                    ws.Cells[1,40].Value.ToString().ToLower().Trim() != "costoutbound"||
                                    ws.Cells[1,41].Value.ToString().ToLower().Trim() != "costavg"||
                                    ws.Cells[1,42].Value.ToString().ToLower().Trim() != "skulength"||
                                    ws.Cells[1,43].Value.ToString().ToLower().Trim() != "skuwidth"||
                                    ws.Cells[1,44].Value.ToString().ToLower().Trim() != "skuheight"||
                                    ws.Cells[1,45].Value.ToString().ToLower().Trim() != "skugrossweight"||
                                    ws.Cells[1,46].Value.ToString().ToLower().Trim() != "skuweight"||
                                    ws.Cells[1,47].Value.ToString().ToLower().Trim() != "skuvolume"||
                                    ws.Cells[1,48].Value.ToString().ToLower().Trim() != "pulength"||
                                    ws.Cells[1,49].Value.ToString().ToLower().Trim() != "puwidth"||
                                    ws.Cells[1,50].Value.ToString().ToLower().Trim() != "puheight"||
                                    ws.Cells[1,51].Value.ToString().ToLower().Trim() != "pugrossweight"||
                                    ws.Cells[1,52].Value.ToString().ToLower().Trim() != "puweight"||
                                    ws.Cells[1,53].Value.ToString().ToLower().Trim() != "puvolume"||
                                    ws.Cells[1,54].Value.ToString().ToLower().Trim() != "ipcklength"||
                                    ws.Cells[1,55].Value.ToString().ToLower().Trim() != "ipckwidth"||
                                    ws.Cells[1,56].Value.ToString().ToLower().Trim() != "ipckheight"||
                                    ws.Cells[1,57].Value.ToString().ToLower().Trim() != "ipckgrossweight"||
                                    ws.Cells[1,58].Value.ToString().ToLower().Trim() != "ipckweight"||
                                    ws.Cells[1,59].Value.ToString().ToLower().Trim() != "ipckvolume"||
                                    ws.Cells[1,60].Value.ToString().ToLower().Trim() != "pcklength"||
                                    ws.Cells[1,61].Value.ToString().ToLower().Trim() != "pckwidth"||
                                    ws.Cells[1,62].Value.ToString().ToLower().Trim() != "pckheight"||
                                    ws.Cells[1,63].Value.ToString().ToLower().Trim() != "pckgrossweight"||
                                    ws.Cells[1,64].Value.ToString().ToLower().Trim() != "pckweight"||
                                    ws.Cells[1,65].Value.ToString().ToLower().Trim() != "pckvolume"||
                                    ws.Cells[1,66].Value.ToString().ToLower().Trim() != "layerlength"||
                                    ws.Cells[1,67].Value.ToString().ToLower().Trim() != "layerwidth"||
                                    ws.Cells[1,68].Value.ToString().ToLower().Trim() != "layerheight"||
                                    ws.Cells[1,69].Value.ToString().ToLower().Trim() != "layergrossweight"||
                                    ws.Cells[1,70].Value.ToString().ToLower().Trim() != "layerweight"||
                                    ws.Cells[1,71].Value.ToString().ToLower().Trim() != "layervolume"||
                                    ws.Cells[1,72].Value.ToString().ToLower().Trim() != "hulength"||
                                    ws.Cells[1,73].Value.ToString().ToLower().Trim() != "huwidth"||
                                    ws.Cells[1,74].Value.ToString().ToLower().Trim() != "huheight"||
                                    ws.Cells[1,75].Value.ToString().ToLower().Trim() != "hugrossweight"||
                                    ws.Cells[1,76].Value.ToString().ToLower().Trim() != "huweight"||
                                    ws.Cells[1,77].Value.ToString().ToLower().Trim() != "huvolume"||
                                    ws.Cells[1,78].Value.ToString().ToLower().Trim() != "isdangerous"||
                                    ws.Cells[1,79].Value.ToString().ToLower().Trim() != "ishighvalue"||
                                    ws.Cells[1,80].Value.ToString().ToLower().Trim() != "isfastmove"||
                                    ws.Cells[1,81].Value.ToString().ToLower().Trim() != "isslowmove"||
                                    ws.Cells[1,82].Value.ToString().ToLower().Trim() != "isprescription"||
                                    ws.Cells[1,83].Value.ToString().ToLower().Trim() != "isdlc"||
                                    ws.Cells[1,84].Value.ToString().ToLower().Trim() != "ismaterial"||
                                    ws.Cells[1,85].Value.ToString().ToLower().Trim() != "isunique"||
                                    ws.Cells[1,86].Value.ToString().ToLower().Trim() != "isalcohol"||
                                    ws.Cells[1,87].Value.ToString().ToLower().Trim() != "istemperature"||
                                    ws.Cells[1,88].Value.ToString().ToLower().Trim() != "isdynamicpick"||
                                    ws.Cells[1,89].Value.ToString().ToLower().Trim() != "ismixingprep"||
                                    ws.Cells[1,90].Value.ToString().ToLower().Trim() != "isfinishgoods"||
                                    ws.Cells[1,91].Value.ToString().ToLower().Trim() != "isnaturalloss"||
                                    ws.Cells[1,92].Value.ToString().ToLower().Trim() != "isbatchno"||
                                    ws.Cells[1,93].Value.ToString().ToLower().Trim() != "ismeasurement"||
                                    ws.Cells[1,94].Value.ToString().ToLower().Trim() != "roomtype"||
                                    ws.Cells[1,95].Value.ToString().ToLower().Trim() != "tempmin"||
                                    ws.Cells[1,96].Value.ToString().ToLower().Trim() != "tempmax"||
                                    ws.Cells[1,97].Value.ToString().ToLower().Trim() != "alcmanage"||
                                    ws.Cells[1,98].Value.ToString().ToLower().Trim() != "alccategory"||
                                    ws.Cells[1,99].Value.ToString().ToLower().Trim() != "alccontent"||
                                    ws.Cells[1,100].Value.ToString().ToLower().Trim() != "alccolor"||
                                    ws.Cells[1,101].Value.ToString().ToLower().Trim() != "dangercategory"||
                                    ws.Cells[1,102].Value.ToString().ToLower().Trim() != "dangerlevel"||
                                    ws.Cells[1,103].Value.ToString().ToLower().Trim() != "stockthresholdmin"||
                                    ws.Cells[1,104].Value.ToString().ToLower().Trim() != "stockthresholdmax"||
                                    ws.Cells[1,105].Value.ToString().ToLower().Trim() != "spcrecvzone"||
                                    ws.Cells[1,106].Value.ToString().ToLower().Trim() != "spcrecvaisle"||
                                    ws.Cells[1,107].Value.ToString().ToLower().Trim() != "spcrecvbay"||
                                    ws.Cells[1,108].Value.ToString().ToLower().Trim() != "spcrecvlevel"||
                                    ws.Cells[1,109].Value.ToString().ToLower().Trim() != "spcrecvlocation"||
                                    ws.Cells[1,110].Value.ToString().ToLower().Trim() != "spcprepzone"||
                                    ws.Cells[1,111].Value.ToString().ToLower().Trim() != "spcdistzone"||
                                    ws.Cells[1,112].Value.ToString().ToLower().Trim() != "spcdistshare"||
                                    ws.Cells[1,113].Value.ToString().ToLower().Trim() != "spczonedelv"||
                                    ws.Cells[1,114].Value.ToString().ToLower().Trim() != "rowops"
                                ){ 
                                        throw new Exception("Excel column header incorrect format ");  
                                }
                            }else { 
                                try { 
                                    ls.Add(new exsProduct(orgcode,site,depot, 
                                    (ws.Cells[row,1].Value == null) ? "" : ws.Cells[row,1].Value.ToString().Trim(), //article 
                                    (ws.Cells[row,2].Value == null) ? "" : ws.Cells[row,2].Value.ToString().Trim(), //articletype 
                                    (ws.Cells[row,3].Value == null) ? "" : ws.Cells[row,3].Value.ToString().Trim(), //pv 
                                    (ws.Cells[row,4].Value == null) ? "" : ws.Cells[row,4].Value.ToString().Trim(), //lv 
                                    (ws.Cells[row,5].Value == null) ? "" : ws.Cells[row,5].Value.ToString().Trim(), //description 
                                    (ws.Cells[row,6].Value == null) ? "" : ws.Cells[row,6].Value.ToString().Trim(), //descalt 
                                    (ws.Cells[row,7].Value == null) ? "" : ws.Cells[row,7].Value.ToString().Trim(), //thcode 
                                    (ws.Cells[row,8].Value == null) ? "" : ws.Cells[row,8].Value.ToString().Trim(), //dlcall 
                                    (ws.Cells[row,9].Value == null) ? "" : ws.Cells[row,9].Value.ToString().Trim(), //dlcfactory 
                                    (ws.Cells[row,10].Value == null) ? "" : ws.Cells[row,10].Value.ToString().Trim(), //dlcwarehouse 
                                    (ws.Cells[row,11].Value == null) ? "" : ws.Cells[row,11].Value.ToString().Trim(), //dlcshop 
                                    (ws.Cells[row,12].Value == null) ? "" : ws.Cells[row,12].Value.ToString().Trim(), //dlconsumer 
                                    (ws.Cells[row,13].Value == null) ? "" : ws.Cells[row,13].Value.ToString().Trim(), //hdivison 
                                    (ws.Cells[row,14].Value == null) ? "" : ws.Cells[row,14].Value.ToString().Trim(), //hdepartment 
                                    (ws.Cells[row,15].Value == null) ? "" : ws.Cells[row,15].Value.ToString().Trim(), //hsubdepart 
                                    (ws.Cells[row,16].Value == null) ? "" : ws.Cells[row,16].Value.ToString().Trim(), //hclass 
                                    (ws.Cells[row,17].Value == null) ? "" : ws.Cells[row,17].Value.ToString().Trim(), //hsubclass 
                                    (ws.Cells[row,18].Value == null) ? "" : ws.Cells[row,18].Value.ToString().Trim(), //typemanage 
                                    (ws.Cells[row,19].Value == null) ? "" : ws.Cells[row,19].Value.ToString().Trim(), //unitmanage 
                                    (ws.Cells[row,20].Value == null) ? "" : ws.Cells[row,20].Value.ToString().Trim(), //unitdesc 
                                    (ws.Cells[row,21].Value == null) ? "" : ws.Cells[row,21].Value.ToString().Trim(), //unitreceipt 
                                    (ws.Cells[row,22].Value == null) ? "" : ws.Cells[row,22].Value.ToString().Trim(), //unitprep 
                                    (ws.Cells[row,23].Value == null) ? "" : ws.Cells[row,23].Value.ToString().Trim(), //unitsale 
                                    (ws.Cells[row,24].Value == null) ? "" : ws.Cells[row,24].Value.ToString().Trim(), //unitstock 
                                    (ws.Cells[row,25].Value == null) ? "" : ws.Cells[row,25].Value.ToString().Trim(), //unitweight 
                                    (ws.Cells[row,26].Value == null) ? "" : ws.Cells[row,26].Value.ToString().Trim(), //unitdimension 
                                    (ws.Cells[row,27].Value == null) ? "" : ws.Cells[row,27].Value.ToString().Trim(), //unitvolume 
                                    (ws.Cells[row,28].Value == null) ? "" : ws.Cells[row,28].Value.ToString().Trim(), //hucode 
                                    (ws.Cells[row,29].Value == null) ? "" : ws.Cells[row,29].Value.ToString().Trim(), //rtoskuofpu 
                                    (ws.Cells[row,30].Value == null) ? "" : ws.Cells[row,30].Value.ToString().Trim(), //rtoskuofipck 
                                    (ws.Cells[row,31].Value == null) ? "" : ws.Cells[row,31].Value.ToString().Trim(), //rtoskuofpck 
                                    (ws.Cells[row,32].Value == null) ? "" : ws.Cells[row,32].Value.ToString().Trim(), //rtoskuoflayer 
                                    (ws.Cells[row,33].Value == null) ? "" : ws.Cells[row,33].Value.ToString().Trim(), //rtoskuofhu 
                                    (ws.Cells[row,34].Value == null) ? "" : ws.Cells[row,34].Value.ToString().Trim(), //rtoipckofpck 
                                    (ws.Cells[row,35].Value == null) ? "" : ws.Cells[row,35].Value.ToString().Trim(), //rtopckoflayer 
                                    (ws.Cells[row,36].Value == null) ? "" : ws.Cells[row,36].Value.ToString().Trim(), //rtolayerofhu 
                                    (ws.Cells[row,37].Value == null) ? "" : ws.Cells[row,37].Value.ToString().Trim(), //innaturalloss 
                                    (ws.Cells[row,38].Value == null) ? "" : ws.Cells[row,38].Value.ToString().Trim(), //ounaturalloss 
                                    (ws.Cells[row,39].Value == null) ? "" : ws.Cells[row,39].Value.ToString().Trim(), //costinbound 
                                    (ws.Cells[row,40].Value == null) ? "" : ws.Cells[row,40].Value.ToString().Trim(), //costoutbound 
                                    (ws.Cells[row,41].Value == null) ? "" : ws.Cells[row,41].Value.ToString().Trim(), //costavg 
                                    (ws.Cells[row,42].Value == null) ? "" : ws.Cells[row,42].Value.ToString().Trim(), //skulength 
                                    (ws.Cells[row,43].Value == null) ? "" : ws.Cells[row,43].Value.ToString().Trim(), //skuwidth 
                                    (ws.Cells[row,44].Value == null) ? "" : ws.Cells[row,44].Value.ToString().Trim(), //skuheight 
                                    (ws.Cells[row,45].Value == null) ? "" : ws.Cells[row,45].Value.ToString().Trim(), //skugrossweight 
                                    (ws.Cells[row,46].Value == null) ? "" : ws.Cells[row,46].Value.ToString().Trim(), //skuweight 
                                    (ws.Cells[row,47].Value == null) ? "" : ws.Cells[row,47].Value.ToString().Trim(), //skuvolume 
                                    (ws.Cells[row,48].Value == null) ? "" : ws.Cells[row,48].Value.ToString().Trim(), //pulength 
                                    (ws.Cells[row,49].Value == null) ? "" : ws.Cells[row,49].Value.ToString().Trim(), //puwidth 
                                    (ws.Cells[row,50].Value == null) ? "" : ws.Cells[row,50].Value.ToString().Trim(), //puheight 
                                    (ws.Cells[row,51].Value == null) ? "" : ws.Cells[row,51].Value.ToString().Trim(), //pugrossweight 
                                    (ws.Cells[row,52].Value == null) ? "" : ws.Cells[row,52].Value.ToString().Trim(), //puweight 
                                    (ws.Cells[row,53].Value == null) ? "" : ws.Cells[row,53].Value.ToString().Trim(), //puvolume 
                                    (ws.Cells[row,54].Value == null) ? "" : ws.Cells[row,54].Value.ToString().Trim(), //ipcklength 
                                    (ws.Cells[row,55].Value == null) ? "" : ws.Cells[row,55].Value.ToString().Trim(), //ipckwidth 
                                    (ws.Cells[row,56].Value == null) ? "" : ws.Cells[row,56].Value.ToString().Trim(), //ipckheight 
                                    (ws.Cells[row,57].Value == null) ? "" : ws.Cells[row,57].Value.ToString().Trim(), //ipckgrossweight 
                                    (ws.Cells[row,58].Value == null) ? "" : ws.Cells[row,58].Value.ToString().Trim(), //ipckweight 
                                    (ws.Cells[row,59].Value == null) ? "" : ws.Cells[row,59].Value.ToString().Trim(), //ipckvolume 
                                    (ws.Cells[row,60].Value == null) ? "" : ws.Cells[row,60].Value.ToString().Trim(), //pcklength 
                                    (ws.Cells[row,61].Value == null) ? "" : ws.Cells[row,61].Value.ToString().Trim(), //pckwidth 
                                    (ws.Cells[row,62].Value == null) ? "" : ws.Cells[row,62].Value.ToString().Trim(), //pckheight 
                                    (ws.Cells[row,63].Value == null) ? "" : ws.Cells[row,63].Value.ToString().Trim(), //pckgrossweight 
                                    (ws.Cells[row,64].Value == null) ? "" : ws.Cells[row,64].Value.ToString().Trim(), //pckweight 
                                    (ws.Cells[row,65].Value == null) ? "" : ws.Cells[row,65].Value.ToString().Trim(), //pckvolume 
                                    (ws.Cells[row,66].Value == null) ? "" : ws.Cells[row,66].Value.ToString().Trim(), //layerlength 
                                    (ws.Cells[row,67].Value == null) ? "" : ws.Cells[row,67].Value.ToString().Trim(), //layerwidth 
                                    (ws.Cells[row,68].Value == null) ? "" : ws.Cells[row,68].Value.ToString().Trim(), //layerheight 
                                    (ws.Cells[row,69].Value == null) ? "" : ws.Cells[row,69].Value.ToString().Trim(), //layergrossweight 
                                    (ws.Cells[row,70].Value == null) ? "" : ws.Cells[row,70].Value.ToString().Trim(), //layerweight 
                                    (ws.Cells[row,71].Value == null) ? "" : ws.Cells[row,71].Value.ToString().Trim(), //layervolume 
                                    (ws.Cells[row,72].Value == null) ? "" : ws.Cells[row,72].Value.ToString().Trim(), //hulength 
                                    (ws.Cells[row,73].Value == null) ? "" : ws.Cells[row,73].Value.ToString().Trim(), //huwidth 
                                    (ws.Cells[row,74].Value == null) ? "" : ws.Cells[row,74].Value.ToString().Trim(), //huheight 
                                    (ws.Cells[row,75].Value == null) ? "" : ws.Cells[row,75].Value.ToString().Trim(), //hugrossweight 
                                    (ws.Cells[row,76].Value == null) ? "" : ws.Cells[row,76].Value.ToString().Trim(), //huweight 
                                    (ws.Cells[row,77].Value == null) ? "" : ws.Cells[row,77].Value.ToString().Trim(), //huvolume 
                                    (ws.Cells[row,78].Value == null) ? "" : ws.Cells[row,78].Value.ToString().Trim(), //isdangerous 
                                    (ws.Cells[row,79].Value == null) ? "" : ws.Cells[row,79].Value.ToString().Trim(), //ishighvalue 
                                    (ws.Cells[row,80].Value == null) ? "" : ws.Cells[row,80].Value.ToString().Trim(), //isfastmove 
                                    (ws.Cells[row,81].Value == null) ? "" : ws.Cells[row,81].Value.ToString().Trim(), //isslowmove 
                                    (ws.Cells[row,82].Value == null) ? "" : ws.Cells[row,82].Value.ToString().Trim(), //isprescription 
                                    (ws.Cells[row,83].Value == null) ? "" : ws.Cells[row,83].Value.ToString().Trim(), //isdlc 
                                    (ws.Cells[row,84].Value == null) ? "" : ws.Cells[row,84].Value.ToString().Trim(), //ismaterial 
                                    (ws.Cells[row,85].Value == null) ? "" : ws.Cells[row,85].Value.ToString().Trim(), //isunique 
                                    (ws.Cells[row,86].Value == null) ? "" : ws.Cells[row,86].Value.ToString().Trim(), //isalcohol 
                                    (ws.Cells[row,87].Value == null) ? "" : ws.Cells[row,87].Value.ToString().Trim(), //istemperature 
                                    (ws.Cells[row,88].Value == null) ? "" : ws.Cells[row,88].Value.ToString().Trim(), //isdynamicpick 
                                    (ws.Cells[row,89].Value == null) ? "" : ws.Cells[row,89].Value.ToString().Trim(), //ismixingprep 
                                    (ws.Cells[row,90].Value == null) ? "" : ws.Cells[row,90].Value.ToString().Trim(), //isfinishgoods 
                                    (ws.Cells[row,91].Value == null) ? "" : ws.Cells[row,91].Value.ToString().Trim(), //isnaturalloss 
                                    (ws.Cells[row,92].Value == null) ? "" : ws.Cells[row,92].Value.ToString().Trim(), //isbatchno 
                                    (ws.Cells[row,93].Value == null) ? "" : ws.Cells[row,93].Value.ToString().Trim(), //ismeasurement 
                                    (ws.Cells[row,94].Value == null) ? "" : ws.Cells[row,94].Value.ToString().Trim(), //roomtype 
                                    (ws.Cells[row,95].Value == null) ? "" : ws.Cells[row,95].Value.ToString().Trim(), //tempmin 
                                    (ws.Cells[row,96].Value == null) ? "" : ws.Cells[row,96].Value.ToString().Trim(), //tempmax 
                                    (ws.Cells[row,97].Value == null) ? "" : ws.Cells[row,97].Value.ToString().Trim(), //alcmanage 
                                    (ws.Cells[row,98].Value == null) ? "" : ws.Cells[row,98].Value.ToString().Trim(), //alccategory 
                                    (ws.Cells[row,99].Value == null) ? "" : ws.Cells[row,99].Value.ToString().Trim(), //alccontent 
                                    (ws.Cells[row,100].Value == null) ? "" : ws.Cells[row,100].Value.ToString().Trim(), //alccolor 
                                    (ws.Cells[row,101].Value == null) ? "" : ws.Cells[row,101].Value.ToString().Trim(), //dangercategory 
                                    (ws.Cells[row,102].Value == null) ? "" : ws.Cells[row,102].Value.ToString().Trim(), //dangerlevel 
                                    (ws.Cells[row,103].Value == null) ? "" : ws.Cells[row,103].Value.ToString().Trim(), //stockthresholdmin 
                                    (ws.Cells[row,104].Value == null) ? "" : ws.Cells[row,104].Value.ToString().Trim(), //stockthresholdmax 
                                    (ws.Cells[row,105].Value == null) ? "" : ws.Cells[row,105].Value.ToString().Trim(), //spcrecvzone 
                                    (ws.Cells[row,106].Value == null) ? "" : ws.Cells[row,106].Value.ToString().Trim(), //spcrecvaisle 
                                    (ws.Cells[row,107].Value == null) ? "" : ws.Cells[row,107].Value.ToString().Trim(), //spcrecvbay 
                                    (ws.Cells[row,108].Value == null) ? "" : ws.Cells[row,108].Value.ToString().Trim(), //spcrecvlevel 
                                    (ws.Cells[row,109].Value == null) ? "" : ws.Cells[row,109].Value.ToString().Trim(), //spcrecvlocation 
                                    (ws.Cells[row,110].Value == null) ? "" : ws.Cells[row,110].Value.ToString().Trim(), //spcprepzone 
                                    (ws.Cells[row,111].Value == null) ? "" : ws.Cells[row,111].Value.ToString().Trim(), //spcdistzone 
                                    (ws.Cells[row,112].Value == null) ? "" : ws.Cells[row,112].Value.ToString().Trim(), //spcdistshare 
                                    (ws.Cells[row,113].Value == null) ? "" : ws.Cells[row,113].Value.ToString().Trim(), //spczonedelv 
                                    (ws.Cells[row,117].Value == null) ? "" : ws.Cells[row,117].Value.ToString().Trim() //rowops 
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
                return Ok(await _cf.ImpexsProductAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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
            List<exsProduct> ls = new List<exsProduct>();
            Stream stream = null; StreamReader reader = null;
            DateTimeOffset decstart = DateTimeOffset.Now;
            try { 
                if (_log.IsEnabled(LogLevel.Debug)){  ps = Process.GetCurrentProcess(); p = new SnapsLogDbg(ps,"ImportJsonAsync", id, Request.getIP(), app, accncode); }
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                using (stream = file.OpenReadStream()) {
                    using (reader = new StreamReader(stream)) {
                        try { 
                            ls = JsonConvert.DeserializeObject<List<exsProduct>>(reader.ReadToEnd());
                        }catch (Exception exl) { 
                             throw new Exception("Data incorrect result " + exl.Message);
                        }                        
                        reader.Close();
                    }
                    stream.Close();
                }
                return Ok(await _cf.ImpexsProductAsync(orgcode,site,depot,accncode,file.ContentType,file.FileName,file.Length,decstart,ls));                                
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