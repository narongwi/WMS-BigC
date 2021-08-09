using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using snaps.wms.api.report.Manager;
using snaps.wms.api.report.Models;
using snaps.wms.api.report.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace snaps.wms.api.report.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Log logger = new Log();
        public ReportController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// List Report is Active
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get()
        {
            ResponseMessages response = new ResponseMessages();
            ReportManager manager = new ReportManager(_configuration["ConnectionString"]);
            try
            {
                response.success = true;
                response.message = string.Empty;
                response.data = manager.ReportList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.Error("REPORT", "LIST", ex.Message);
                response.success = false;
                response.message = ex.Message;
                response.data = new List<object>();
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Export Excel with Forms
        /// </summary>
        /// <param name="jsonForm"></param>
        /// <returns>File Excel</returns>
        [HttpPost]
        public IActionResult Post([FromBody] JObject jsonForm)
        {
            try
            {
                if (jsonForm.Count == 0)
                    return BadRequest("parameter incorrect!");

                var manager = new ReportManager(_configuration["ConnectionString"]);
                var reh_id = jsonForm["reh_id"].toInt();
                var reh_no = jsonForm["reh_code"].ToString();
                var reh_na = jsonForm["reh_name"].ToString();
                var commandText = manager.ReportCommand(reh_id);
                var controlForm = manager.ReportControl(reh_no);

                foreach (var c in controlForm)
                {
                    string controlValue = "";
                    string controlName = $"re{c.Typ.ToLower()}_{c.Name}";
                    if (c.Typ.ToLower() == "c")
                    {
                        var options = jsonForm[controlName].ToObject<OptionsModel>();
                        if (options != null) controlValue = options.Id;
                    }
                    else if (c.Typ.ToLower() == "d")
                    {
                        string textValue = jsonForm[controlName].ToObject<string>();
                        if (!string.IsNullOrEmpty(textValue))
                        {
                            var dateModel = jsonForm[controlName].ToObject<DateTime>();
                            if (dateModel != null) controlValue = dateModel.ToString("dd/MM/yyyy");
                        }
                    }
                    else
                    {
                        string textValue = jsonForm[controlName].ToObject<string>();
                        if (textValue != null) controlValue = textValue;
                    }
                    commandText = commandText.Replace("{" + c.Param + "}", controlValue);
                }
                byte[] _response = manager.ReportExport(commandText);
                return File(_response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reh_no + ".xlsx");
            }
            catch (Exception ex)
            {
                logger.Error("REPORT", "EXPORT", ex.Message);
                return BadRequest(new ResponseMessages(false, ex.Message, new object()));
            }
        }
    }
}
