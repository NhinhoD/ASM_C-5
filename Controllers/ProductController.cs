using ASM_C_4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Controllers
{
	public class ProductController : Controller
	{
		private readonly DataContext _dataContext;
		public ProductController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Details(long id)
		{
			if(id == null) return RedirectToAction("Index");
			var productsById = _dataContext.Products.Where(c => c.Id == id).FirstOrDefault();
			return View(productsById);
		}
		[HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();
            ViewBag.Keyword = searchTerm; return View(products);
        }
    }
}
