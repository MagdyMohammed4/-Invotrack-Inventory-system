using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Invotrack.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Invotrack.UI.Controllers
{
    [Authorize]
    public class TransfersController : Controller
    {
        private readonly ITransferService _transferService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;

        public TransfersController(
            ITransferService transferService,
            IProductService productService,
            IWarehouseService warehouseService)
        {
            _transferService = transferService;
            _productService = productService;
            _warehouseService = warehouseService;
        }

        
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransferDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _transferService.ProcessTransferAsync(dto, User.Identity?.Name ?? "System");
                    TempData["Success"] = "تم تسجيل وإتمام عملية نقل المنتجات بين المخازن بنجاح";
                    return RedirectToAction("Index", "Products");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            await PopulateDropdowns();
            return View(dto);
        }

        private async Task PopulateDropdowns()
        {
            var productsResult = await _productService.GetProductsPagedAsync(null, 1, 1000);
            var warehouses = await _warehouseService.GetAllWarehousesAsync();

            ViewBag.Products = new SelectList(productsResult.Items, "Id", "NameAr");
            ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
        }
    }
}
