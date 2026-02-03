using Models;

namespace wandaAPI.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetByAccountAsync(int accountId);
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction> CreateAsync(int accountId, TransactionCreateDTO dto);
        Task UpdateAsync(int id, TransactionUpdateDTO dto);
        Task DeleteAsync(int id);

        Task ProcessRecurringTransactionsAsync();
    }
}
