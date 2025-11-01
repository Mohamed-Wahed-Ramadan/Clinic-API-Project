using Clinic.Models;

namespace Clinic.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByNameAsync(string name);
        Task<bool> NameExistsAsync(string name);
    }
}