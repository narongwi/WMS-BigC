using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using snaps.wms.api.pda.Data;
using snaps.wms.api.pda.Manager;
using snaps.wms.api.pda.Utils;

namespace snaps.wms.api.pda.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AppinfoController : ControllerBase
    {
        private readonly AppInfo appinfo;
        private readonly IConfiguration config;
        private readonly Account account;
        public AppinfoController(IConfiguration configuration) {
            config = configuration;
            appinfo = new AppInfo(config["ConnectionString"]);
            account = new Account(config["ConnectionString"]);
        }
        [HttpGet("{accncode}")]
        public IActionResult Version(string accncode)
        {
            var acc = account.profile(accncode);
            var inf = appinfo.getInfo(acc.orgcode,acc.site,acc.depot);
            return Ok(inf);
        }
    }
}
