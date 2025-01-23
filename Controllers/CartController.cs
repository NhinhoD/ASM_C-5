using ASM_C_4.Models;
using ASM_C_4.Models.ViewModels;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ASM_C_4.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;

        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }

        public IActionResult Checkout()
        {
            return View("~/Views/Checkuot/Index.cshtml");
        }

        // Thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> Add(long id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(id);
            if (product != null)
            {
                List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

                if (cartItem == null)
                {
                    cart.Add(new CartItemModel(product));
                }
                else
                {
                    cartItem.Quantity += 1;
                }

                HttpContext.Session.SetJson("Cart", cart);
                TempData["success"] = "Thêm sản phẩm vào giỏ hàng thành công";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return NotFound();
        }

        // Thêm combo vào giỏ hàng
        public async Task<IActionResult> AddCombo(long id)
        {
            Combo combo = await _dataContext.Combos.FindAsync(id);
            if (combo != null)
            {
                List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

                if (cartItem == null)
                {
                    cart.Add(new CartItemModel(combo));
                }
                else
                {
                    cartItem.Quantity += 1;
                }

                HttpContext.Session.SetJson("Cart", cart);
                TempData["success"] = "Thêm combo vào giỏ hàng thành công";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return NotFound();
        }

        // Các phương thức khác không thay đổi...

        public async Task<IActionResult> Decrease(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity -= 1;
                }
                else
                {
                    cart.RemoveAll(p => p.ProductId == id);
                }
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductId == id);

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Xóa giỏ hàng thành công";
            return RedirectToAction("Index");
        }
    }
}
