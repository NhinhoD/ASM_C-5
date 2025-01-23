using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Controllers
{
	public class BrandController : Controller
	{
		private readonly DataContext _dataContext;
		public BrandController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
		public async Task<IActionResult> Index(string Slug = "")
		{
			BrandModel Brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
			if (Brand == null) return RedirectToAction("Index");
			var productsByBrand = _dataContext.Products.Where(c => c.BrandId == Brand.Id);
			return View(await productsByBrand.OrderByDescending(c => c.Id).ToListAsync());
		}
	}
}
