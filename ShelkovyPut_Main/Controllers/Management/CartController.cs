using System.IO;
using System.Security.Claims;
using Application.DTOs.Request.Management;
using Application.Interfaces.Management;
using Application.ViewModels;
using Application.ViewModels.Page;
using Domain.Models.Enum;
using Domain.Models.Management;
using Infrastructure.Data;
using Infrastructure.Repositories.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class CartController : Controller
    {
        private readonly ShelkobyPutDbContext _context;
        private readonly ICartService _cart;
        private readonly IVnPayService _vnPay;
        private readonly IHomeService _home;
        public CartController(ShelkobyPutDbContext context, ICartService cart, IVnPayService vnPay, IHomeService home)
        {
            _context = context;
            _cart = cart;
            _vnPay = vnPay;
            _home = home;
        }

        public async Task<ActionResult> AddItem(int productId, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cart.AddItem(productId, qty);
            if (redirect == 0)
            {
                return Ok(cartCount);
            }
            return RedirectToAction(nameof(GetUserCart));
        }

        [HttpPost]
        public async Task<IActionResult> SyncCart([FromBody] List<CartDetails> items)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _cart.AddMultipleItemsAsync(userId, items);

            return Ok();
        }


        public async Task<ActionResult> RemoveItem(int productId)
        {
            var cartCount = await _cart.RemoveItem(productId);
            return RedirectToAction(nameof(GetUserCart));
        }

        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var result = await _cart.RemoveFromCart(productId);
            if(!result)
            {
                return BadRequest("Failed to remove the product from the cart.");
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("Cart/UserCart")]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                // User is not logged in, return view for client-side rendering
                IEnumerable<Category> categoriesForSearchs = await _home.Categories();
                var vms = new HeaderVM()
                {
                    Carts = null, // No server-side cart
                    CategoryForSearch = categoriesForSearchs
                };

                return View(vms);
            }

            var cart = await _cart.GetUserCart();
            if (cart == null || !cart.CartDetails.Any())
            {
                return RedirectToAction(nameof(CartEmpty));
            }

            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            var vm = new HeaderVM()
            {
                Carts = cart,
                CategoryForSearch = categoriesForSearch
            };

            return View(vm);
        }


        public async Task<IActionResult> CartEmpty(string sTerm = "", int categoryId = 0)
        {
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            var viewModel = new SEOProduct()
            {
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
            };

            return viewModel == null ? NotFound() : View(viewModel);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItems = await _cart.GetCartItemCount();
            return Ok(cartItems);
        }

        public IActionResult Checkout()
        {           
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Checkout(CheckoutRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            bool isCheckedOut = await _cart.DoCheckout(model);
            if (!isCheckedOut)
            {
                return RedirectToAction(nameof(OrderFailure));
            }

            return RedirectToAction(nameof(OrderSuccess));
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var res = _vnPay.PaymentExecute(Request.Query);
            if(res == null || res.VnPayResponseCode != "00")
            {
                Console.WriteLine(res.VnPayResponseCode);
                return RedirectToAction(nameof(OrderFailure));
            }
            return RedirectToAction(nameof(OrderSuccess));
        }
        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }
    }
}
