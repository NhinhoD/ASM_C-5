using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ASM_C_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    /*[Route("Admin/User")]*/
    //[Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext )
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Route("Index")]
		public async Task<IActionResult> Index()
		{

			var usersWithRoles = await (from u in _dataContext.Users
										join ur in _dataContext.UserRoles on u.Id equals ur.UserId into userRoleGroup
										from ur in userRoleGroup.DefaultIfEmpty()
										join r in _dataContext.Roles on ur.RoleId equals r.Id into roleGroup
										from r in roleGroup.DefaultIfEmpty()
										select new
										{
											User = u,
											RoleName = r != null ? r.Name : "Khách hàng"
										}).ToListAsync();

			return View(usersWithRoles);
		}


		[HttpGet]
		[Route("Create")]
		public async Task<IActionResult> Create()
		{
			var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
		}

        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email); // tim user theo email
                    var userId = createUser.Id; // lay userId
                    var role = _roleManager.FindByIdAsync(user.RoleId); // lay RoleId
                    // gan quyen
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    TempData["success"] = "Tạo Khách hàng thành công";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(createUserResult);
                    return View(user);
                }
            }
            else
            {
                TempData["error"] = "Lỗi :(";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                /*return BadRequest(errorMessage);*/
                return View(user);
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Xóa khách hàng thành công";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
		public async Task<IActionResult> Edit(string Id, AppUserModel user)
		{
			var existingUser = await _userManager.FindByIdAsync(Id);
			if (existingUser == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				// Cập nhật thông tin người dùng
				existingUser.UserName = user.UserName;
				existingUser.Email = user.Email;
				existingUser.PhoneNumber = user.PhoneNumber;

				// Cập nhật thông tin người dùng
				var updateResult = await _userManager.UpdateAsync(existingUser);

				if (updateResult.Succeeded)
				{
					// Lấy danh sách các vai trò hiện tại của người dùng
					var currentRoles = await _userManager.GetRolesAsync(existingUser);

					// Xóa tất cả các vai trò hiện tại
					await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);

					// Thêm vai trò mới nếu `RoleId` không null
					if (!string.IsNullOrEmpty(user.RoleId))
					{
						var role = await _roleManager.FindByIdAsync(user.RoleId);
						if (role != null)
						{
							await _userManager.AddToRoleAsync(existingUser, role.Name);
						}
					}

					TempData["success"] = "Chỉnh sửa khách hàng thành công";
					return RedirectToAction("Index", "User");
				}
				else
				{
					AddIdentityErrors(updateResult);
				}
			}

			// Lấy danh sách vai trò để hiển thị lại trong dropdown nếu có lỗi
			var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");

			TempData["error"] = "Lỗi dữ liệu";
			var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
			string errorMessage = string.Join("\n", errors);
			return View(existingUser);
		}

	}
}
