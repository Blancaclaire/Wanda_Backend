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

        public async Task<List<Transaction>> GetTransactionAccountAssync(int accountId)
        {
            var transactions = new List<Transaction>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Transaction_id, User_id, Transaction_id, Transactions_Type, Splitstype, Frecuency, Category, Amount, Concept, IsRecurring, Transactions_date, End_date FROM Transactions WHERE Transaction_id = @Transaction_id;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", accountId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var transaction = new Transaction
                            {
                                Transaction_id = reader.GetInt32(0),
                                User_id = reader.GetInt32(1),
                                Objective_id = reader.GetInt32(2),
                                Transactions_Type = Enum.Parse<Transaction.Transaction_type>(reader.GetString(3), ignoreCase: true),
                                Splitstype = Enum.Parse<Transaction.Splits_type>(reader.GetString(4), ignoreCase: true),
                                Frecuency = Enum.Parse<Transaction.EFrecuency>(reader.GetString(5), ignoreCase: true),
                                Category = reader.GetString(6),
                                Amount = reader.IsDBNull(7) ? 0 : Convert.ToDouble(reader.GetDecimal(7)),
                                Concept = reader.GetString(8),
                                IsRecurring = reader.GetBoolean(9),
                                Transactions_date = reader.GetDateTime(10),
                                End_date = reader.GetDateTime(11)
                               
                            };
                        }
                        transactions.Add(transaction);
                    }
                }
            }
         return transactions;

        }
        




    }
}

