using Models;
using Microsoft.Data.SqlClient;

namespace wandaAPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly string _connectionString;

        public TransactionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("wandaDb") ?? throw new ArgumentNullException("Connection string not found");
        }

        public Task<Transaction> AddTransactionAssync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTransactionAssync(int transactions_id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Transaction>> GetTransactionAccountAssync(int accountId)
        {
            var transactions = new List<Transaction>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT transaction_id, account_id, objective_id, user_id, transactions_Type, splitstype, frecuency, category, amount, concept, isRecurring, transactions_date, end_date FROM TRANSACTIONS WHERE account_id = @account_id;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", accountId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            transactions.Add(new Transaction
                            {
                                Transaction_id = reader.GetInt32(0),
                                Account_id = reader.GetInt32(1),
                                User_id = reader.GetInt32(2),
                                Objective_id = reader.GetInt32(3),
                                Category = reader.GetString(4),
                                Amount = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
                                Transaction_type = Enum.Parse<Transaction.ETransaction_type>(reader.GetString(6), ignoreCase: true),
                                Concept = reader.GetString(7),
                                Transaction_date = reader.GetDateTime(8),
                                IsRecurring = reader.GetBoolean(9),
                                Frecuency = Enum.Parse<Transaction.EFrecuency>(reader.GetString(10), ignoreCase: true),
                                End_date = reader.GetDateTime(11),
                                Splittype = Enum.Parse<Transaction.Split_type>(reader.GetString(12), ignoreCase: true),

                            });
                        }
                    }
                }
            }
         return transactions;

        }

        public Task<Transaction?> GetTransactionAssync(int transactions_id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTransactionAssync(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}

