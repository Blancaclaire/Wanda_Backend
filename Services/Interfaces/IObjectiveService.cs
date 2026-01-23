using Models;
using DTOs;
namespace wandaAPI.Services
{
    public interface IObjectiveService
    {
        Task<List<Objective>> GetByAccountAsync(int accountId);
        Task<Objective> CreateAsync(int accountId, ObjectiveCreateDto dto);
        Task<Objective?> GetByIdAsync(int id);
        Task UpdateAsync(int id, ObjectiveUpdateDto dto);
        Task DeleteAsync(int id);
        Task AddFundsAsync(int id, double amount);
    }
}
