using Domain.Models.Auth;

namespace Application.Interfaces.Authentication
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(User user);
		RefreshToken CreateRefreshToken(User user);
    }
}