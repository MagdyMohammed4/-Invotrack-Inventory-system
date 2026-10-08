using System.Threading.Tasks;
using Invotrack.Application.DTOs;
using Invotrack.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Invotrack.WebUI.Controllers
{
    [Authorize(Policy = "OwnerOnly")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // عرض قائمة المستخدمين
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // صفحة إنشاء مستخدم جديد
        public async Task<IActionResult> Create()
        {
            var roles = await _userService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(dto);
                if (result.Succeeded)
                {
                    TempData["Success"] = "تم إنشاء المستخدم وتحديد صلاحياته بنجاح";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            var roles = await _userService.GetAllRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Name", "Name", dto.Role);
            return View(dto);
        }

        // تعطيل/تنشيط حساب مستخدم
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            await _userService.ToggleUserStatusAsync(id);
            TempData["Success"] = "تم تحديث حالة الحساب بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}