using Models;

namespace wandaAPI.Repositories
{
    public interface IObjectiveRepository
    {
        Task<List<Objective>> GetByAccountIdAsync(int accountId);
        Task<Objective?> GetByIdAsync(int id);
        Task<int> AddAsync(Objective objective);
        Task UpdateAsync(Objective objective);
        Task DeleteAsync(int id);
    }
}