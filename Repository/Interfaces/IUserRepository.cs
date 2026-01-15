using Models;

namespace wandaAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user1);
        Task UpdateAsync(User user1);
        Task DeleteAsync(int id);
        
    }
}