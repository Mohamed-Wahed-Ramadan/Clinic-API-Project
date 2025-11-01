using Clinic.Models;

namespace Clinic.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}