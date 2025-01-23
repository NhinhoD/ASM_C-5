using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Sockets;

namespace ASM_C_4.Areas.Admin.Controllers
{
	[Area("Admin")]
   /* [Authorize(Roles = "Admin")]*/
    public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
			_dataContext = dataContext;
		}
		public async Task<IActionResult> Index()
		{
			
			return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());
		}
		[HttpGet]
        public IActionResult Create()
        {
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
			return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
			if (ModelState.IsValid)
			{
				product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "Sản phẩm đã tồn tại");
					return View(product);
				}	
				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath,"media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath,FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					product.Image = imageName;
				}
				_dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
				return RedirectToAction("Index");
            }
			else
			{
				TempData["error"] = "Lỗi :(";
				List<string> errors = new List<string>();
				foreach(var value in ModelState.Values)
				{
					foreach (var error in value.Errors) {
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				/*return BadRequest(errorMessage);*/
                return View(product);
            }
            return View(product);
		}
		public async Task<IActionResult> Edit(long Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View(product);
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            // Lấy danh sách các danh mục và nhãn hiệu để hiển thị trên view
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            // Tìm sản phẩm hiện có trong database theo Id của sản phẩm
            var existed_product = await _dataContext.Products.FindAsync(product.Id);

            if (ModelState.IsValid) // Kiểm tra xem dữ liệu đầu vào có hợp lệ hay không
            {
                // Tạo "slug" từ tên sản phẩm bằng cách thay khoảng trắng bằng dấu gạch ngang
                product.Slug = product.Name.Replace(" ", "-");

                // Kiểm tra xem slug có trùng với sản phẩm nào khác không
                var slug = await _dataContext.Products.FirstOrDefaultAsync(x => x.Slug == product.Slug && x.Id != product.Id);
                if (slug != null) // Nếu slug đã tồn tại, hiển thị lỗi và trả về view với dữ liệu hiện tại
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    // Tạo tên file ảnh mới và lưu vào thư mục media/products
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(fs);
                    }

                    // Cập nhật thuộc tính Image của existed_product với tên ảnh mới
                    string oldImage = existed_product.Image; // Lưu lại ảnh cũ để xóa sau
                    existed_product.Image = imageName;

                    // Xóa ảnh cũ nếu có
                    string oldFilePath = Path.Combine(uploadsDir, oldImage);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Xảy ra lỗi khi xóa ảnh cũ");
                        }
                    }
                }


                // Cập nhật các thuộc tính khác của sản phẩm
                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CategoryId = product.CategoryId;
                existed_product.BrandId = product.BrandId;

                // Cập nhật sản phẩm vào database
                _dataContext.Update(existed_product);
                await _dataContext.SaveChangesAsync();

                // Thông báo thành công và chuyển hướng về trang danh sách sản phẩm
                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                // Nếu ModelState không hợp lệ, hiển thị lỗi
                TempData["error"] = "Dữ liệu không hợp lệ";

                // Lấy danh sách các lỗi từ ModelState
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                // Kết hợp các lỗi thành một chuỗi và trả về kết quả lỗi
                string errorMessage = string.Join("\n", errors);
                return View(product);
            }
        }

		public async Task<IActionResult> Delete(long Id)
		{
			var product = await _dataContext.Products.FindAsync(Id);
			if (product == null)
			{
				return NotFound();
			}

			string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
			string oldfilePath = Path.Combine(uploadsDir, product.Image);

			// Xóa file hình ảnh nếu tồn tại
			if (System.IO.File.Exists(oldfilePath))
			{
				try
				{
					System.IO.File.Delete(oldfilePath);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("", "Không thể xóa tệp hình ảnh.");
				}
			}

			// Xóa các bản ghi liên quan trong OrderDetails
			var relatedOrderDetails = _dataContext.OrderDetails.Where(od => od.ProductId == Id);
			_dataContext.OrderDetails.RemoveRange(relatedOrderDetails);

			// Xóa sản phẩm
			_dataContext.Products.Remove(product);

			// Lưu thay đổi
			await _dataContext.SaveChangesAsync();

			TempData["success"] = "Xóa sản phẩm thành công!";
			return RedirectToAction("Index");
		}

	}
}
