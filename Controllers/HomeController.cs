using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ASM_C_4.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _dataContext = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách Combo
            var combos = await _dataContext.Combos
                                       .Include(c => c.ComboProducts)
                                       .ThenInclude(cp => cp.Product)
                                       .ToListAsync();

            // Lấy danh sách sản phẩm (hoặc bất kỳ dữ liệu khác mà bạn muốn hiển thị)
            var products = await _dataContext.Products.Include(p => p.Category).Include(p => p.Brand).ToListAsync();

            ViewBag.Combos = combos;

            return View(products); // Trả về danh sách sản phẩm chính
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if(statuscode == 404)
            {
                return View("NotFound");
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
