using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Invotrack.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Invotrack.UI.Controllers
{
    [Authorize]
    public class PartnersController : Controller
    {
        private readonly IPartnerService _partnerService;

        public PartnersController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        // 1️⃣ شاشة عرض الموردين
        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await _partnerService.GetPartnersByTypeAsync(PartnerType.Supplier);
            return View(suppliers);
        }

        // 2️⃣ شاشة عرض الموزعين والعملاء
        public async Task<IActionResult> Distributors()
        {
            var distributors = await _partnerService.GetPartnersByTypeAsync(PartnerType.Distributor);
            return View(distributors);
        }

        // 3️⃣ إضافة مورد جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier(BusinessPartnerDto dto)
        {
            dto.Type = PartnerType.Supplier;
            if (ModelState.IsValid)
            {
                try
                {
                    await _partnerService.CreatePartnerAsync(dto, User.Identity?.Name ?? "System");
                    TempData["Success"] = "تم تسجيل المورد بنجاح";
                    return RedirectToAction(nameof(Suppliers));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "حدث خطأ أثناء الحفظ: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "يرجى التأكد من إدخال البيانات المطلوبة (الاسم ورقم الهاتف)";
            }

            return RedirectToAction(nameof(Suppliers));
        }

        // 4️⃣ إضافة موزع/عميل جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDistributor(BusinessPartnerDto dto)
        {
            dto.Type = PartnerType.Distributor;
            if (ModelState.IsValid)
            {
                try
                {
                    await _partnerService.CreatePartnerAsync(dto, User.Identity?.Name ?? "System");
                    TempData["Success"] = "تم تسجيل الموزع/العميل بنجاح";
                    return RedirectToAction(nameof(Distributors));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "حدث خطأ أثناء الحفظ: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "يرجى التأكد من إدخال البيانات المطلوبة (الاسم ورقم الهاتف)";
            }

            return RedirectToAction(nameof(Distributors));
        }

        // 5️⃣ صفحة التعديل
        public async Task<IActionResult> Edit(int id)
        {
            var partner = await _partnerService.GetPartnerByIdAsync(id);
            if (partner == null) return NotFound();

            return View(partner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BusinessPartnerDto dto)
        {
            if (ModelState.IsValid)
            {
                await _partnerService.UpdatePartnerAsync(dto, User.Identity?.Name ?? "System");
                TempData["Success"] = "تم تعديل البيانات بنجاح";

                if (dto.Type == PartnerType.Supplier)
                    return RedirectToAction(nameof(Suppliers));

                return RedirectToAction(nameof(Distributors));
            }

            return View(dto);
        }

        // 6️⃣ حذف الشريك
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, PartnerType type)
        {
            await _partnerService.DeletePartnerAsync(id, User.Identity?.Name ?? "System");
            TempData["Success"] = "تم الحذف بنجاح";

            if (type == PartnerType.Supplier)
                return RedirectToAction(nameof(Suppliers));

            return RedirectToAction(nameof(Distributors));
        }
    }
}