using Microsoft.AspNetCore.Mvc;
namespace snaps.wms.api.pda.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        public IActionResult APK()
        {
            var fileName = @"snaps.wms.pda.apk";
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"downloads\" + fileName);
            return File(fileBytes, "application/force-download", fileName);
        }

        public IActionResult EMU() {
            var fileName = @"snaps.wms.pda.apk";
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"downloads\emulator\" + fileName);
            return File(fileBytes,"application/force-download",fileName);
        }
    }
}
