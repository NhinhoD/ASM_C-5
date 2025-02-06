using ASM_C_4.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM_C_4.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASM_C_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    /*[Authorize(Roles = "Admin, Publisher, Author")]*/
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            List<CategoryModel> category = _dataContext.Categories.ToList(); //33 datas


            const int pageSize = 10; //10 items/trang                                                                                                               

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = category.Count(); //33 items;

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = category.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        /*public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Categories.OrderByDescending(p => p.Id).ToListAsync());
        }*/

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại");
                    return View(category);
                }

                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
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
                return View(category);
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa danh mục thành công";
            return RedirectToAction("Index");
        }

        // Edit Danh Muc
        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            return View(category);
        }
        // Edit Danh Muc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug từ tên danh mục
                category.Slug = category.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem slug đã tồn tại cho danh mục khác chưa
                var existingCategory = await _dataContext.Categories
                    .FirstOrDefaultAsync(x => x.Slug == category.Slug && x.Id != category.Id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Slug", "Danh mục đã tồn tại.");
                    return View(category);
                }

                // Cập nhật danh mục và lưu thay đổi
                try
                {
                    _dataContext.Update(category);
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật danh mục thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Ghi lại ngoại lệ (ex) nếu cần
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật danh mục.");
                }
            }

            // Thu thập các lỗi ModelState và lưu trữ chúng để hiển thị
            TempData["error"] = "lỗi :(";
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();
            TempData["error"] = errors;

            return View(category);
        }

    }
}
