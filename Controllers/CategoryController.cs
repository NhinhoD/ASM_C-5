﻿using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Controllers
{
	public class CategoryController : Controller
	{
		private readonly DataContext _dataContext;
		public CategoryController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
		public async Task<IActionResult> Index(string Slug = "")
		{
			CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
			if(category == null) return RedirectToAction("Index");
			var productsByCategory = _dataContext.Products.Where(c => c.CategoryId == category.Id); 
			return View(await productsByCategory.OrderByDescending(c => c.Id).ToListAsync());
		}
	}
}
