using Application.DTOs.Request.Authentication;
using Application.DTOs.Response;

namespace Application.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<GeneralResponse> LoginAsync(LoginRequest model);
    }
}