using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invotrack.UI.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateCategoryAsync(dto, User.Identity?.Name ?? "System");
                TempData["Success"] = "تم إضافة التصنيف بنجاح";
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(nameof(Index), categories);
        }

       
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryDto dto)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategoryAsync(dto, User.Identity?.Name ?? "System");
                TempData["Success"] = "تم تعديل بيانات التصنيف بنجاح";
                return RedirectToAction(nameof(Index));
            }

            return View(dto);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id, User.Identity?.Name ?? "System");
            TempData["Success"] = "تم حذف التصنيف بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
