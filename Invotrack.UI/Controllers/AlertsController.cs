using Invotrack.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invotrack.UI.Controllers
{
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly IStockAlertService _alertService;

        public AlertsController(IStockAlertService alertService)
        {
            _alertService = alertService;
        }

       
        public async Task<IActionResult> LowStock()
        {
            var lowStockProducts = await _alertService.GetLowStockProductsAsync();
            return View(lowStockProducts);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetLowStockCount()
        {
            var count = await _alertService.GetLowStockCountAsync();
            return Json(new { count });
        }
    }
}
