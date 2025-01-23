using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public RoleController(DataContext dataContext, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _dataContext = dataContext;
        }

        // Danh sách các vai trò
        public async Task<IActionResult> Index()
        {
            var roles = await _dataContext.Roles.OrderByDescending(p => p.Id).ToListAsync();
            return View(roles);
        }

        // Hiển thị form tạo vai trò mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Tạo vai trò mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role.Name));
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Tạo vai trò thành công";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Vai trò đã tồn tại");
                }
            }
            return View(role);
        }

        // Xóa vai trò
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["success"] = "Xóa vai trò thành công";
                }
                else
                {
                    TempData["error"] = "Không thể xóa vai trò";
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Đã xảy ra lỗi trong quá trình xóa";
            }

            return RedirectToAction("Index");
        }

        // Hiển thị form chỉnh sửa vai trò
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // Chỉnh sửa vai trò
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityRole model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = model.Name;

                try
                {
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Cập nhật vai trò thành công";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật");
                }
            }

            return View(model);
        }
    }
}
