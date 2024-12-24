using Application.DTOs.Request.Authentication;
using Application.Interfaces.Authentication;
using Application.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShelkovyPut_Main.Controllers.Auth
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _auth;

		public AuthController(IAuthService auth)
		{
			_auth = auth;
		}

		// [HttpPost("Register")]
		// public async Task<ActionResult<GeneralResponse>> Register(CreateRequest model)
		// {
		// 	var registerUser = await _auth.CreateAsync(model);
		// 	return Ok(registerUser);
		// }

		[HttpPost("Login")]
		public async Task<ActionResult<UserResponse>> Login(LoginRequest model)
		{
			var loginUser = await _auth.LoginAsync(model);
			return Ok(loginUser);
		}

		[Authorize]
		[HttpGet("refresh-page")]
		public async Task<ActionResult<UserResponse>> RefreshPage()
		{
			var user = await _auth.RefreshPage();
			return Ok(user);
		}

		[Authorize]
		[HttpPost("refresh-token")]
		public async Task<ActionResult<UserResponse>> RefreshToken()
		{
			return await _auth.RefreshToken();
		}
	}
}