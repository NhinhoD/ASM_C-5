// ComboController.cs
using ASM_C_4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Hosting;

namespace ASM_C_4.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ComboController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _context; // Sử dụng đúng DbContext

        public ComboController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Hiển thị danh sách Combo
        public async Task<IActionResult> Index()
        {
            var combos = await _context.Combos
                .Include(c => c.ComboProducts)
                .ThenInclude(cp => cp.Product)
                .ToListAsync();

            // Kiểm tra dữ liệu truy xuất
            foreach (var combo in combos)
            {
                Console.WriteLine($"Combo: {combo.Name}");
                foreach (var cp in combo.ComboProducts)
                {
                    Console.WriteLine($"- Product: {cp.Product?.Name ?? "null"}");
                }
            }

            return View(combos);
        }



        // Hiển thị form tạo Combo
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // Xử lý tạo Combo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Combo combo, long[] selectedProducts)
        {
            if (ModelState.IsValid)
            {
                if (combo.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + combo.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await combo.ImageUpload.CopyToAsync(fs);
                    }
                    combo.Image = imageName;
                }

                try
                {
                    _context.Combos.Add(combo);
                    await _context.SaveChangesAsync();
                  
                    if (selectedProducts != null && selectedProducts.Any())
                    {
                        foreach (var productId in selectedProducts)
                        {
                            var comboProduct = new ComboProduct
                            {
                                ComboId = combo.Id,

                                //for
                                ProductId = productId
                            };
                            _context.ComboProducts.Add(comboProduct);
                        }
                        await _context.SaveChangesAsync();
                    }
                    TempData["success"] = "Thêm sản phẩm thành công";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError(string.Empty, $"Có lỗi khi lưu thông tin Combo: {innerException}");
                    return View(combo);
                }
            }

            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");
            return View(combo);
        }






        // Xóa Combo
        public async Task<IActionResult> Delete(long id)
        {
            var combo = await _context.Combos
                .Include(c => c.ComboProducts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null)
            {
                return NotFound();
            }
            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            string oldfilePath = Path.Combine(uploadsDir, combo.Image);
            try
            {
                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi :(");
            }
            // Xóa các liên kết trong ComboProducts trước khi xóa Combo
            if (combo.ComboProducts != null)
            {
                _context.ComboProducts.RemoveRange(combo.ComboProducts);
            }

            _context.Combos.Remove(combo);
            await _context.SaveChangesAsync();
            TempData["success"] = "Xóa thành công";
            return RedirectToAction(nameof(Index));
        }
    }
}
