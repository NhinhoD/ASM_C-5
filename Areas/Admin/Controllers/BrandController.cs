using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    /*[Route("Admin/Brand")]*/
    //[Authorize(Roles = "Admin, Publisher, Author")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index(int pg = 1)
        {
            List<BrandModel> Brand = _dataContext.Brands.ToList();
            const int pageSize = 10; //10 items/trang

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = Brand.Count(); //33 items;

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = Brand.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(x => x.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại");
                    return View(brand);
                }

                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Tạo thương hiệu thành công";
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
                return View(brand);
            }
            return View(brand);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            if (brand == null)
            {
                return NotFound();
            }

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa thương hiệu thành công";
            return RedirectToAction("Index");
        }
        //Edit Brand
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }
        //Edit Brand
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                // Tạo slug từ tên danh mục
                brand.Slug = brand.Name.Replace(" ", "-").ToLower();

                // Kiểm tra xem slug đã tồn tại cho danh mục khác chưa
                var existingCategory = await _dataContext.Categories
                    .FirstOrDefaultAsync(x => x.Slug == brand.Slug && x.Id != brand.Id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Slug", "Thương hiệu đã tồn tại.");
                    return View(brand);
                }

                // Cập nhật danh mục và lưu thay đổi
                try
                {
                    _dataContext.Update(brand);
                    await _dataContext.SaveChangesAsync();
                    TempData["success"] = "Cập nhật thương hiệu thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Ghi lại ngoại lệ (ex) nếu cần
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật thương hiệu.");
                }
            }

            // Thu thập các lỗi ModelState và lưu trữ chúng để hiển thị
            TempData["error"] = "lỗi :(";
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();
            TempData["error"] = errors;

            return View(brand);
        }
    }
}
