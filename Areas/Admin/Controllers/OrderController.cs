using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Areas.Admin.Controllers
{
	[Area("Admin")]
	//[Authorize(Roles = "Admin, Publisher, Author")]
	public class OrderController : Controller
	{
		private readonly DataContext _dataContext;
		public OrderController(DataContext dataContext)
		{
			_dataContext = dataContext;
		}
		public async Task<IActionResult> Index(int pg = 1)
		{
            List<OrderModel> Orders = _dataContext.Orders.OrderByDescending(p => p.Id).ToList();
            const int pageSize = 5; //10 items/trang

            if (pg < 1) //page < 1;
            {
                pg = 1; //page ==1
            }
            int recsCount = Orders.Count(); //33 items;

            var pager = new Paginate(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

            //category.Skip(20).Take(10).ToList()

            var data = Orders.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
            //return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }
		public async Task<IActionResult> ViewOrder(string ordercode)
		{
			var DetailsOrder = await _dataContext.OrderDetails
				.Include(o => o.Product) // Ensure Product is a valid navigation property
				.Where(o => o.OrderCode == ordercode)
				.ToListAsync();

			return View(DetailsOrder);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateOrder(string ordercode, int status)
		{
			var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
			if (order == null)
			{
				return NotFound();
			}
			order.Status = status;
			try
			{
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật thành công" });
			
            }
			catch (Exception ex)
			{
				return StatusCode(500, "Lỗi");
			}

		}
	}
}
