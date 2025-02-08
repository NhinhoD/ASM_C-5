using ASM_C_4.Models;
using ASM_C_4.Models.ViewModels;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
		public async Task<IActionResult> Details(long Id)
		{
			if (Id == null)
			{
				return RedirectToAction("Index");
			}
			var productById = _dataContext.Products.
				Include(p => p.Rating).Where(p => p.Id == Id).FirstOrDefault(); // category = 4
																							 //related product
			var relatedProducts = await _dataContext.Products
				.Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
				.Take(4)
				.ToListAsync();
			ViewBag.RelatedProducts = relatedProducts;
			// Lấy danh sách đánh giá của sản phẩm
			ViewBag.Reviews = productById.Rating != null ? productById.Rating.ToList() : new List<RatingModel>();

			var viewModel = new ProductDetailsViewModel
			{
				ProductDetails = productById,
				RatingDetails = productById.Rating ?? new List<RatingModel>()
			};
			return View(viewModel);
		}
		[HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();
            ViewBag.Keyword = searchTerm; return View(products);
        }
	
		[Authorize]
		[HttpPost]
		public IActionResult AddReview(int productId, int rating, string comment)
		{
			Console.WriteLine($"Received review: ProductId={productId}, Rating={rating}, Comment={comment}");

			if (string.IsNullOrWhiteSpace(comment) || rating < 1 || rating > 5)
			{
				TempData["error"] = "Dữ liệu không hợp lệ.";
				return RedirectToAction("Details", "Product", new { id = productId });
			}

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userName = User.Identity.Name;
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var existingReview = _dataContext.Ratings.FirstOrDefault(r => r.ProductId == productId && r.Email == userEmail);
			if (existingReview != null)
			{
				TempData["error"] = "Bạn đã đánh giá sản phẩm này rồi!";
				return RedirectToAction("Details", "Product", new { id = productId });
			}

			var review = new RatingModel
			{
				ProductId = productId,
				Comment = comment,
				Name = userName,
				Email = userEmail,
				Star = rating.ToString()
			};

			_dataContext.Ratings.Add(review);
			_dataContext.SaveChanges();

			TempData["success"] = "Đánh giá của bạn đã được lưu thành công!";
			return RedirectToAction("Details", "Product", new { id = productId });
		}




	}
}
