using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Management;
using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Domain.Models.Auth;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;

public class ChatHub : Hub
{
    private readonly ShelkobyPutDbContext _context;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<User> _userManager;

    public ChatHub(ShelkobyPutDbContext context, IHttpContextAccessor http, UserManager<User> userManager)
    {
        _context = context;
        _http = http;
        _userManager = userManager;
    }

    public async Task SendMessage(string receiverEmail, string message)
    {
        try
        {
            var currentUserId = _http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var receiver = await _userManager.FindByEmailAsync(receiverEmail);

            if (currentUser == null || receiver == null)
            {
                Console.WriteLine("Either currentUser or receiver is null.");
                return;
            }

            var chatMessage = new Messages
            {
                UserId = currentUserId!,
                Message = message,
                Timestamp = DateTime.Now
            };

            _context.Messages.Add(chatMessage);
            await _context.SaveChangesAsync();

            await Clients.User(receiver.Id).SendAsync("ReceiveMessage", currentUser.Email, message);
            await Clients.User(currentUserId).SendAsync("ReceiveMessage", receiver.Email, message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;
        } 
        
    }

    public async Task<List<Messages>> GetMessages()
    {
        var userId = _http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null && await _userManager.IsInRoleAsync(user, StaticUserRole.ADMIN))
        {
            return await _context.Messages.Include(m => m.User).ToListAsync();
        }

        return await _context.Messages.Include(m => m.User)
            .Where(m => m.UserId == userId || m.User.Email == user.Email)
            .ToListAsync();
    }

    public async Task<List<Messages>> GetMessagesForUser(string email)
    {
        var receiver = await _userManager.FindByEmailAsync(email);
        if (receiver != null)
        {
            return await _context.Messages.Include(m => m.User)
                .Where(m => m.UserId == receiver.Id || m.User.Email == email)
                .ToListAsync();
        }
        return new List<Messages>();
    }
}
