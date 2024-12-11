using Application.DTOs.Request.Authentication;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Authentication;
using Application.ViewModels.Auth;
using Domain.Models.Auth;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories.Auth
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IConfiguration _config;
		private readonly ShelkobyPutDbContext _context;
		private readonly ITokenService _token;
		private readonly IHttpContextAccessor _http;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, ShelkobyPutDbContext context, ITokenService token, IHttpContextAccessor http)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _context = context;
            _token = token;
            _http = http;
        }

        public Task<GeneralResponse> LoginAsync(LoginRequest model)
        {
            throw new NotImplementedException();
        }

        // public async Task<GeneralResponse> LoginAsync(LoginRequest model)
        // {
        //     try
        // 	{
        // 		var user = await FindUserByEmailAsync(model.Email);
        // 		if (user == null)
        // 		{
        // 			return new UserResponse()
        // 			{
        // 				Message = $"{user!.Email} register yet"
        // 			};
        // 		}

        // 		SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        // 		if (!result.Succeeded)
        // 		{
        // 			return new UserResponse()
        // 			{
        // 				Message = "Sai mat khau"
        // 			};
        // 		}

        // 		return await DisplayUserResponse(user);
        // 	}
        // 	catch (Exception ex)
        // 	{
        // 		throw new Exception(ex.Message);
        // 	}
        // }
    }
}
