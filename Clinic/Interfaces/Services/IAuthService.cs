using Clinic.DTOs;

namespace Clinic.Interfaces.Services
{
    public interface IAuthService
    {
        Task<object> RegisterAsync(UserRegisterDTO dto);
        Task<object> LoginAsync(UserLoginDTO dto);
    }
}