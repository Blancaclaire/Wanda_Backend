using Models;

namespace wandaAPI.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetTransactionsByAccountAsync(int accountId);
        Task<Transaction?> GetTransactionAsync(int transaction_id);
        Task<int> AddTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int transaction_id);
    }
}