using Application.Interfaces.Management;
using Application.ViewModels.Order;
using Application.ViewModels.User;
using Domain.Constants;
using Domain.Models.Auth;
using Domain.Models.Management;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ShelkovyPut_Main.Controllers.Admin
{
    [Authorize(Roles = StaticUserRole.ADMIN)]
    public class AdminController : Controller
    {
        private readonly IOrderService _order;
        private readonly UserManager<Domain.Models.Auth.User> _userManager;
        private readonly ShelkobyPutDbContext _context;
        public AdminController(IOrderService order, UserManager<Domain.Models.Auth.User> userManager, ShelkobyPutDbContext context)
        {
            _order = order;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _order.AdminOrders(true);
            return View(orders);
        }

        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _order.TogglePaymentStatus(orderId);
            }
            catch (Exception ex)
            {
                // log exception here
            }
            return RedirectToAction(nameof(AllOrders));
        }

        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _order.GetOrderById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with id:{orderId} does not found.");
            }
            var orderStatusList = (await _order.GetOrderStatuses()).Select(orderStatus =>
            {
                return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = order.OrderStatusId == orderStatus.Id };
            });
            var data = new UpdateOrderStatusVM
            {
                OrderId = orderId,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusVM data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    data.OrderStatusList = (await _order.GetOrderStatuses()).Select(orderStatus =>
                    {
                        return new SelectListItem { Value = orderStatus.Id.ToString(), Text = orderStatus.StatusName, Selected = orderStatus.Id == data.OrderStatusId };
                    });

                    return View(data);
                }
                await _order.ChangeOrderStatus(data);
                TempData["msg"] = "Updated successfully";
            }
            catch (Exception ex)
            {
                // catch exception here
                TempData["msg"] = "Something went wrong";
            }
            return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> AssignOrders()
        {
            var shippers = await _userManager.GetUsersInRoleAsync(StaticUserRole.STAFF);
            var orders = await _order.GetUnshippedOrdersAsync();
            orders.ForEach(o => o.AvailableShippers = shippers.ToList());
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> AssignOrders(List<UnshippedOrderVM> model)
        {
            await _order.AssignShipperToOrdersAsync(model);
            return RedirectToAction(nameof(AssignOrders));
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                return Redirect("https://localhost:7235/identity/account/login");
            }

            var profileVM = new ProfileVM()
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = user.CreatedDate,
            };
            return View(profileVM);
        }
    
        public async Task<IActionResult> Chat()
        {
            var messages = await _context.Messages.Include(m => m.User).ToListAsync();
            return View(messages);
        }
    
        public async Task<IActionResult> GetMessagesByUser(string email)
        {
            var messages = await _context.Messages
                .Include(m => m.User)
                    .Where(m => m.User.Email == email)
                    .Select(m => new { m.User.Email, m.Message})
                    .ToListAsync();
            return Json(messages);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var nonAdminUsers = new List<Domain.Models.Auth.User>();
            foreach(var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);
                if(!role.Contains(StaticUserRole.ADMIN))
                {
                    nonAdminUsers.Add(user);
                }
            }
            return View(nonAdminUsers);
        }

        [HttpGet]
        public async Task<IActionResult> EditUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRole = await _userManager.GetRolesAsync(user);
            var model = new EditUserRoleVM()
            {
                UserId = user.Id,
                CurrentRole = currentRole.FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRole(EditUserRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeRolesResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove user roles");
                return View(model);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (!addRoleResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to add user role");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            return View();
        }
    }
}
