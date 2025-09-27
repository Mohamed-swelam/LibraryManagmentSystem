using Core.Entites;

namespace Core.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserById(string userId);
        Task SaveChangesAsync();
        Task<ApplicationUser?> GetUserByRefreshtoken(string refreshtoken);
    }
}
