using Models;

namespace wandaAPI.Repositories
{
    public interface ITransactionSplitRepository
    {

        Task<int> AddAsync(TransactionSplit split);

        Task<List<TransactionSplit>> GetAllByUserIdAsync(int userId);


        Task<TransactionSplit?> GetByIdAsync(int splitId);


        Task UpdateStatusAsync(int splitId, string status, DateTime? paidAt);
        

        Task<List<TransactionSplit>> GetByTransactionIdAsync(int transactionId);
    }
}