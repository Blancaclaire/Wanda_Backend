using Models;

namespace wandaAPI.Repositories
{
    public interface IObjectiveRepository
    {
        Task<List<Objective>> GetByAccountIdAsync(int accountId);
        Task<Account?> ObjectiveEdit(Account account);
        Task<int> AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);

    }
}