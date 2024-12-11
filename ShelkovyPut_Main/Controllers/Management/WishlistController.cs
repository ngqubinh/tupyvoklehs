using Application.Interfaces.Management;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace ShelkovyPut_Main.Controllers.Management
{
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlist;
        private readonly ShelkobyPutDbContext _context;

        public WishlistController(IWishlistService wishlist, ShelkobyPutDbContext context)
        {
            _wishlist = wishlist;
            _context = context;
        }

        public async Task<IActionResult> Wishlists() 
        {
            var wishlists = await _wishlist.GetAllWishListsAsync();
            return View(wishlists);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId, string userId) 
        {
            if(userId == null)
            {
                return Unauthorized();
            }

            var existingProduct = await _context.WishLists.FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);
            if(existingProduct != null)
            {
                TempData["Notification"] = "This product is already in your wishlist!";
                var refererURL = Request.Headers["Referer"].ToString();
                return !string.IsNullOrEmpty(refererURL) ? Redirect(refererURL) : RedirectToAction(nameof(Wishlists));
            }

            var wishlistItem = new WishList()
            {
                ProductId = productId,
                UserId = userId
            };

           _context.WishLists.Add(wishlistItem);
           await _context.SaveChangesAsync();

           var currentURL = Request.Headers["Referer"].ToString();
           if(!string.IsNullOrEmpty(currentURL))
           {
                return Redirect(currentURL);
           }
           return RedirectToAction(nameof(Wishlists));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null) 
            {
                return NotFound("Khong tim thay san pham ton tai trong wishlist!");
            }

            var wishlistDetail = await _context.WishLists
                .Include(w => w.Product)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if(wishlistDetail == null)
            {
                return NotFound();
            }

            return View(wishlistDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        { 
            var wishlistItem = await _context.WishLists.FindAsync(id); 
            if (wishlistItem != null) { _context.WishLists.Remove(wishlistItem); 
            await _context.SaveChangesAsync(); } return RedirectToAction(nameof(Wishlists));
        }
    }
}
