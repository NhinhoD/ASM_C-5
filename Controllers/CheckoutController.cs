using ASM_C_4.Areas.Admin.Repository;
using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASM_C_4.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly IEmailSender _emailSender;
		public CheckoutController(DataContext context, IEmailSender emailSender)
		{
			_dataContext = context;
			_emailSender = emailSender;
		}
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel
                {
                    OrderCode = ordercode,
                    UserName = userEmail,
                    Status = 1,
                    CreatedDate = DateTime.Now
                };
                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();

                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    if (cart.IsCombo)
                    {
                        // Xử lý nếu là combo
                        var combo = await _dataContext.Combos
                            .Include(c => c.ComboProducts)
                            .ThenInclude(cp => cp.Product)
                            .FirstOrDefaultAsync(c => c.Id == cart.ProductId);

                        if (combo != null)
                        {
                            foreach (var comboProduct in combo.ComboProducts)
                            {
                                var orderdetails = new OrderDetails
                                {
                                    UserName = userEmail,
                                    OrderCode = ordercode,
                                    ProductId = comboProduct.ProductId,
                                    Price = comboProduct.Product.Price, // Sử dụng giá sản phẩm thực tế
                                    Quantity = cart.Quantity,
                                    IsCombo = true
                                };
                                _dataContext.Add(orderdetails);
                            }
                        }
                    }
                    else
                    {
                        var orderdetails = new OrderDetails
                        {
                            UserName = userEmail,
                            OrderCode = ordercode,
                            ProductId = cart.ProductId,
                            Price = cart.Price,
                            Quantity = cart.Quantity,
                            IsCombo = false
                        };
                        _dataContext.Add(orderdetails);
                    }
                    await _dataContext.SaveChangesAsync();
                }
                HttpContext.Session.Remove("Cart");

                // Gửi email khi order thành công
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = $"Đơn hàng của bạn đã được đặt thành công. Mã đơn hàng: {ordercode}";
                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["success"] = "Tạo đơn hàng thành công! Vui lòng đợi duyệt đơn hàng!";
                return RedirectToAction("Index", "Cart");
            }
        }


    }
}
