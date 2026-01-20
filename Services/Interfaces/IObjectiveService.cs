using Models;

namespace wandaAPI.Services
{
    public interface IObjectiveService
    {   
        Task<List<Objective>> GetByAccountAsync(IObjectiveService ObjectiveDto ,int accountId);
        Task<Objective> CreateObjectiveAsync(Objective objective);
        Task<Objective?> AddFundsAsync(int objectiveId, double amount);//amount se crea al vuelo
        Task<int> GetProgressAsync(int acountId);//devuelve un %
        Task DeleteAsync(int id);

    }
}
