using Microsoft.AspNetCore.Mvc;
using MovieApp.Utility;

namespace MovieAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShareController : Controller
    {
        private readonly QrCodeUtil _qrCodeUtil;

        public ShareController(QrCodeUtil qrCodeUtil)
        {
            _qrCodeUtil = qrCodeUtil;
        }

        public IActionResult GenerateQRCode()
        {
            //// Generate QR code for the provided URL
            var currentUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var qrCodeBytes = _qrCodeUtil.GenerateQRCode(currentUrl);
            // Return as image
            return File(qrCodeBytes, "image/png");
        }
    }
}
