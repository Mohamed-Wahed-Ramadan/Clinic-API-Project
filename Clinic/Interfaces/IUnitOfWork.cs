using Clinic.Interfaces.Repositories;

namespace Clinic.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IOrderRepository Orders { get; }
        Task<int> CompleteAsync();
    }
}