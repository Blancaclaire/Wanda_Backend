using Models;

namespace wandaAPI.Services
{
    public interface ITransactionSplitService
    {

        Task<List<TransactionSplit>> GetUserSplitsAsync(int userId, string? statusFilter);
        
        Task AcceptDebtAsync(int splitId);
    }
}