using Models;

namespace wandaAPI.Repositories
{
    public interface IObjectiveRepository
    {
        Task<List<Objective>> ListByAccountIdAsync(int accountId);
        Task<Account?> ObjectiveEdit(int id);
        Task<int> AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);

    }
}