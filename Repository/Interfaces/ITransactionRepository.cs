using Models;

namespace wandaAPI.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetTransactionAccountAssync(int accountId);
        Task<Transaction?> GetTransactionAssync(int transactions_id);
        Task<int> AddTransactionAssync(Transaction transaction);
        Task UpdateTransactionAssync(Transaction transaction);
        Task DeleteTransactionAssync(int transactions_id);
    }
}