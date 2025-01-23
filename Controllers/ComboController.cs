using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Controllers
{
    public class ComboController : Controller
    {
        private readonly DataContext _context;

        public ComboController(DataContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách Combo
        public async Task<IActionResult> Index()
        {
            var combos = await _context.Combos
                .Include(c => c.ComboProducts)
                .ThenInclude(cp => cp.Product)
                .ToListAsync();

            return View(combos);
        }

        // Hiển thị form tạo Combo
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
                // Lưu Combo vào database
                _context.Combos.Add(combo);
                await _context.SaveChangesAsync();

                // Thêm sản phẩm vào Combo
                if (selectedProducts != null && selectedProducts.Any())
                {
                    foreach (var productId in selectedProducts)
                    {
                        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
                        if (productExists)
                        {
                            _context.ComboProducts.Add(new ComboProduct
                            {
                                ComboId = combo.Id,
                                ProductId = productId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");
            return View(combo);
        }


        // Xóa Combo
        public async Task<IActionResult> Delete(long id)
        {
            var combo = await _context.Combos.FindAsync(id);
            if (combo == null)
            {
                return NotFound();
            }

            _context.Combos.Remove(combo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
		public async Task<IActionResult> Details(long id)
		{
			if (id == null)
			{
				return RedirectToAction("Index");
			}

			var combo = await _context.Combos
				.Include(c => c.ComboProducts)
				.ThenInclude(cp => cp.Product)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (combo == null)
			{
				return NotFound();
			}

			return View(combo);
		}

	}
}

