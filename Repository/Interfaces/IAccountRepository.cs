using Models;

namespace wandaAPI.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<int> AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);

        Task<Account?> GetPersonalAccountByUserIdAsync(int userId);
        
    }
}