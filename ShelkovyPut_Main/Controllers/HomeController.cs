using Application.Interfaces.Management;
using Application.ViewModels;
using Domain.Models.Management;
using Microsoft.AspNetCore.Mvc;
using ShelkovyPut_Main.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ShelkovyPut_Main.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _home;

        public HomeController(ILogger<HomeController> logger, IHomeService home)
        {
            _logger = logger;
            _home = home;
        }

        public async Task<IActionResult> Index(string sTerm = "", int categoryId = 0 )
        {
            IEnumerable<Product> products = await _home.GetFeaturedProducts();
            IEnumerable<Product> newProducts = await _home.GetNewProducts();
            IEnumerable<Product> productsSoldThisWeek = await _home.GetProductsSoldThisWeek();
            IEnumerable<Product> saleProducts = await _home.GetSaleProducts();
            IEnumerable<Product> brandProducts = await _home.GetBrandProducts();
            IEnumerable<Category> categoriesForSearch = await _home.Categories();

            //if (!products.Any())
            //{
            //    return NotFound("No products found");
            //}

            //if (!brandProducts.Any())
            //{
            //    return NotFound("No products found");
            //}

            //if (!newProducts.Any())
            //{
            //    return NotFound("No new products found");
            //}

            //if (!saleProducts.Any())
            //{
            //    return NotFound("No quan products found");
            //}

            var viewModel = new SEOProduct()
            {
                FeaturedProducts = products,
                STerm = sTerm,
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
                NewProducts = newProducts,
                ProductsSoldThisWeek = productsSoldThisWeek, 
                ProductsSale = saleProducts,
                ProductsBrand = brandProducts,
            };

            foreach (var rp in viewModel.FeaturedProducts)
            {
                Console.WriteLine($"Featured Product: {rp.ProductName}");
            }

            foreach (var rp in viewModel.FeaturedProducts)
            {
                Console.WriteLine($"New Product: {rp.ProductName}");
            }

            foreach (var rp in viewModel.ProductsBrand)
            {
                Console.WriteLine($"New Product: {rp.ProductName}");
            }

            return viewModel == null ? NotFound() : View(viewModel); 
        }

        [HttpGet]
        [Route("Home/Contract")]
        public async Task<IActionResult> Contract(int categoryId = 0)
        {
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            var viewModel = new SEOProduct()
            {
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
            };
            return viewModel == null ? NotFound() : View(viewModel);
        }

        [HttpGet]
        [Route("Home/AboutUs")]
        public async Task<IActionResult> AboutUs(int categoryId = 0)
        {
            IEnumerable<Category> categoriesForSearch = await _home.Categories();
            var viewModel = new SEOProduct()
            {
                CategoryId = categoryId,
                CategoryForSearch = categoriesForSearch,
            };
            return viewModel == null ? NotFound() : View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
