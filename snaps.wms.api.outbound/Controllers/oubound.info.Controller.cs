using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace snaps.wms.api.outbound.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class oubinfoController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("Snaps WMS Outbounb API \nv.1.0.0.0");
        }
    }
}
