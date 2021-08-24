using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Snaps.Helpers;
using Snaps.Helpers.Logger;
using Snaps.Helpers.Logging;
using Snaps.WMS.Interfaces;
using System;
using System.Threading.Tasks;

namespace Snaps.WMS.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class mergeController : ControllerBase {
        private ImergeService _sv;
        private readonly ISnapsLogger _snaplog;
        private readonly AppSettings _optn;
        public mergeController(IOptions<AppSettings> optn,ImergeService osv) {
            
            ISnapsLogFactory snapsLogFactory = new SnapsLogFactory();
            _snaplog = snapsLogFactory.Create<countController>();
            _sv = osv;
            _optn = optn.Value;
        }

        [Authorize]
        [HttpPost("find/{id}")]
        public async Task<IActionResult> findAsync(String id,[FromBody] merge_find o,
                [FromHeader(Name = "site")] string osite,
                [FromHeader(Name = "depot")] string odepot,
                [FromHeader(Name = "accncode")] string oaccn,
                [FromHeader(Name = "orgcode")] string oorg,
                [FromHeader(Name = "lang")] string lng
                 ) {
            try {
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncode = oaccn;
                return Ok(await _sv.Find(o));
            } catch(Exception exr) {
                _snaplog.Error(osite,oaccn,id,exr,exr.Message);
                return BadRequest(exr.SnapsBadRequest());
            }
        }
        [Authorize]
        [HttpPost("generate/{id}")]
        public async Task<IActionResult> GenerateAsync(String id,[FromBody] merge_set o,
                [FromHeader(Name = "site")] string osite,
                [FromHeader(Name = "depot")] string odepot,
                [FromHeader(Name = "accncode")] string oaccn,
                [FromHeader(Name = "orgcode")] string oorg,
                [FromHeader(Name = "lang")] string lng
                   ) {
            try {
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncode = oaccn;
                return Ok(await _sv.Generate(o));
            } catch(Exception exr) {
                _snaplog.Error(osite,oaccn,id,exr,exr.Message);
                return BadRequest(exr.SnapsBadRequest());
            }
        }

        [Authorize]
        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelAsync(String id,[FromBody] merge_set o,
                [FromHeader(Name = "site")] string osite,
                [FromHeader(Name = "depot")] string odepot,
                [FromHeader(Name = "accncode")] string oaccn,
                [FromHeader(Name = "orgcode")] string oorg,
                [FromHeader(Name = "lang")] string lng
                 ) {
            try {
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncode = oaccn;
                return Ok(await _sv.Cancel(o));
            } catch(Exception exr) {
                _snaplog.Error(osite,oaccn,id,exr,exr.Message);
                return BadRequest(exr.SnapsBadRequest());
            }
        }

        [Authorize]
        [HttpPost("mergehu/{id}")]
        public async Task<IActionResult> MergeAsync(String id,[FromBody] merge_md o,
                [FromHeader(Name = "site")] string osite,
                [FromHeader(Name = "depot")] string odepot,
                [FromHeader(Name = "accncode")] string oaccn,
                [FromHeader(Name = "orgcode")] string oorg,
                [FromHeader(Name = "lang")] string lng
                ) {
            try {
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn; o.accncreate = oaccn;
                return Ok(await _sv.Mergehu(o));
            } catch(Exception exr) {
                _snaplog.Error(osite,oaccn,id,exr,exr.Message);
                return BadRequest(exr.SnapsBadRequest());
            }
        }
        [Authorize]
        [HttpPost("list/{id}")]
        public async Task<IActionResult> MergelistAsync(String id,[FromBody] merge_find o,
                [FromHeader(Name = "site")] string osite,
                [FromHeader(Name = "depot")] string odepot,
                [FromHeader(Name = "accncode")] string oaccn,
                [FromHeader(Name = "orgcode")] string oorg,
                [FromHeader(Name = "lang")] string lng
                ) {
            try {
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncode = oaccn; o.accncode = oaccn;
                return Ok(await _sv.Mergelist(o));
            } catch(Exception exr) {
                _snaplog.Error(osite,oaccn,id,exr,exr.Message);
                return BadRequest(exr.SnapsBadRequest());
            }
        }
        [Authorize]
        [HttpPost("line/{id}")]
        public async Task<IActionResult> MergelineAsync(String id,[FromBody] mergehu_md o,
        [FromHeader(Name = "site")] string osite,
        [FromHeader(Name = "depot")] string odepot,
        [FromHeader(Name = "accncode")] string oaccn,
        [FromHeader(Name = "orgcode")] string oorg,
        [FromHeader(Name = "lang")] string lng
        ) {
            try {
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accnmodify = oaccn;
                return Ok(await _sv.Mergeline(o));
            } catch(Exception exr) {
                _snaplog.Error(osite,oaccn,id,exr,exr.Message);
                return BadRequest(exr.SnapsBadRequest());
            }
        }
        [Authorize]
        [HttpPost("label/{id}")]
        public async Task<IActionResult> LabelAsync(string id,[FromBody] merge_set o,
                [FromHeader(Name = "site")] string osite,
                [FromHeader(Name = "depot")] string odepot,
                [FromHeader(Name = "accncode")] string oaccn,
                [FromHeader(Name = "orgcode")] string oorg,
                [FromHeader(Name = "lang")] string lng
                ) {
            try {
                o.orgcode = oorg; o.site = osite; o.depot = odepot; o.accncode = oaccn;
                await _sv.Label(o);
                return Ok();
            } catch(Exception exr) {
                _snaplog.Error(osite,oaccn,id,exr,exr.Message);
                return BadRequest(exr.SnapsBadRequest());
            }
        }
    }
}