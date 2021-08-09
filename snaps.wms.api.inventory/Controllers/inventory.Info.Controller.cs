using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace snaps.wms.api.inventory.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class inveninfoController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok("Snaps WMS Inventory API \nv.1.0.0.0");
        }
    }
}
