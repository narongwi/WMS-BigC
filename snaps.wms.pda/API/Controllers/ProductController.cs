using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using snaps.wms.api.pda.Data;
using snaps.wms.api.pda.Manager;
using snaps.wms.api.pda.Utils;
using System;

namespace snaps.wms.api.pda.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly Products product;
        private readonly Account account;
        private readonly IConfiguration config;
        private readonly Log log = new Log();
        public ProductController(IConfiguration configuration)
        {
            config = configuration;
            account = new Account(config["ConnectionString"]);
            product = new Products(config["ConnectionString"]);
        }

        [Authorize]
        [HttpGet("{productCode}")]
        public ActionResult Info(string productCode)
        {
            try
            {
                var tok = User.Identity.Decode();
                var usr = account.profile(tok.Uname);
                var productActive = product.GetActive(usr.orgcode, usr.site, usr.depot, productCode);
                return Ok(productActive);
            }
            catch (Exception ex)
            {
                log.Error("Product", "GetActive", ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("{article}/{lv}")]
        public ActionResult article(string article,string lv)
        {
            try
            {
                var tok = User.Identity.Decode();
                var usr = account.profile(tok.Uname);
                var productActive = product.GetActive(usr.orgcode, usr.site, usr.depot, article, lv);
                return Ok(productActive);
            }
            catch (Exception ex)
            {
                log.Error("Product", "GetActive", ex.Message);
                return BadRequest(ex.Message);
            }
        }
        //[Authorize]
        //[HttpPost]
        //public ActionResult getexp([FromBody] DateRequaseModel model)
        //{
        //    try
        //    {
        //        var tok = User.Identity.Decode();
        //        var usr = account.profile(tok.Uname);
        //        var mfg = model.dateX.toDate("dd/MM/yyyy");
        //        if (mfg == null)throw new Exception("invalid date");
        //        return Ok(product.GetExp(usr.orgcode, usr.site, usr.depot, model.article, model.lv, mfg).toSuccess());
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Product", "getexp", ex.Message);
        //        return BadRequest(ex.Message.toError());
        //    }
        //}
        //[Authorize]
        //[HttpPost]
        //public ActionResult getmfg([FromBody] DateRequaseModel model)
        //{
        //    try
        //    {
        //        var tok = User.Identity.Decode();
        //        var usr = account.profile(tok.Uname);
        //        var exp = model.dateX.toDate("dd/MM/yyyy");
        //        if (exp == null)throw new Exception("invalid date");
        //        return Ok(product.GetMfg(usr.orgcode, usr.site, usr.depot, model.article, model.lv, exp).toSuccess());
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Product", "getmfg", ex.Message);
        //        return BadRequest(ex.Message.toError());
        //    }
        //}
    }
}
