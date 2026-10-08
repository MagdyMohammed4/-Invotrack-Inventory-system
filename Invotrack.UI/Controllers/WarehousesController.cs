using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invotrack.WebUI.Controllers
{
    [Authorize]
    public class WarehousesController : Controller
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // عرض قائمة المخازن
        public async Task<IActionResult> Index()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return View(warehouses);
        }

        // إضافة مخزن جديد عبر Modal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseDto dto)
        {
            if (ModelState.IsValid)
            {
                await _warehouseService.CreateWarehouseAsync(dto, User.Identity?.Name ?? "System");
                TempData["Success"] = "تم إضافة المخزن بنجاح";
                return RedirectToAction(nameof(Index));
            }

            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return View(nameof(Index), warehouses);
        }

        // صفحة تعديل بيانات المخزن
        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null) return NotFound();

            return View(warehouse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WarehouseDto dto)
        {
            if (ModelState.IsValid)
            {
                await _warehouseService.UpdateWarehouseAsync(dto, User.Identity?.Name ?? "System");
                TempData["Success"] = "تم تعديل بيانات المخزن بنجاح";
                return RedirectToAction(nameof(Index));
            }

            return View(dto);
        }

        // حذف مخزن مباشر مع Modal التأكيد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _warehouseService.DeleteWarehouseAsync(id, User.Identity?.Name ?? "System");
            TempData["Success"] = "تم حذف المخزن بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}