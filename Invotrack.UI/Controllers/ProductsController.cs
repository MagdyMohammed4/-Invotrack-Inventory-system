using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Invotrack.UI.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

       
        public async Task<IActionResult> Index(string search, int pageIndex = 1)
        {
            ViewBag.Search = search;
            var result = await _productService.GetProductsPagedAsync(search, pageIndex, 10);
            return View(result);
        }

       
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "NameAr");
            return View(new ProductDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            
            ModelState.Remove("SKU");

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.CreateProductAsync(dto, User.Identity?.Name ?? "System");
                    TempData["Success"] = "تم إضافة المنتج بنجاح برمز SKU: " + dto.SKU;
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "NameAr", dto.CategoryId);
            return View(dto);
        }

       
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "NameAr", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductDto dto)
        {
            ModelState.Remove("SKU");

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(dto, User.Identity?.Name ?? "System");
                    TempData["Success"] = "تم تعديل بيانات المنتج بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "NameAr", dto.CategoryId);
            return View(dto);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id, User.Identity?.Name ?? "System");
                TempData["Success"] = "تم حذف المنتج بنجاح";
            }
            catch (Exception)
            {
                TempData["Error"] = "تعذر حذف المنتج لارتباطه بحركات مخزنية أو بيانات أخرى.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}