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
                string query = "SELECT transaction_id, account_id, user_id, objective_id, category, amount, transaction_type, concept, transaction_date, isRecurring, frequency, end_date, split_type FROM TRANSACTIONS WHERE account_id = @account_id;";

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
                                Objective_id = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                Category = reader.GetString(4),
                                Amount = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
                                Transaction_type = Enum.Parse<Transaction.ETransaction_type>(reader.GetString(6), ignoreCase: true),
                                Concept = reader.GetString(7),
                                Transaction_date = reader.GetDateTime(8),
                                IsRecurring = reader.GetBoolean(9),
                                Frequency = reader.IsDBNull(10) ? Transaction.EFrequency.mouthly : Enum.Parse<Transaction.EFrequency>(reader.GetString(10), ignoreCase: true),
                                End_date = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                Splittype = Enum.Parse<Transaction.Split_type>(reader.GetString(12), ignoreCase: true)
                            });
                        }
                    }
                }
            }
            return transactions;
        }

        public async Task<Transaction?> GetTransactionAssync(int transactions_id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT transaction_id, account_id, user_id, objective_id, category, amount, transaction_type, concept, transaction_date, isRecurring, frequency, end_date, split_type FROM TRANSACTIONS WHERE transaction_id = @transactions_id;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transactions_id", transactions_id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Transaction
                            {
                                Transaction_id = reader.GetInt32(0),
                                Account_id = reader.GetInt32(1),
                                User_id = reader.GetInt32(2),
                                Objective_id = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                Category = reader.GetString(4),
                                Amount = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
                                Transaction_type = Enum.Parse<Transaction.ETransaction_type>(reader.GetString(6), ignoreCase: true),
                                Concept = reader.GetString(7),
                                Transaction_date = reader.GetDateTime(8),
                                IsRecurring = reader.GetBoolean(9),
                                Frequency = reader.IsDBNull(10) ? Transaction.EFrequency.mouthly : Enum.Parse<Transaction.EFrequency>(reader.GetString(10), ignoreCase: true),
                                End_date = reader.IsDBNull(11) ? DateTime.MinValue : reader.GetDateTime(11),
                                Splittype = Enum.Parse<Transaction.Split_type>(reader.GetString(12), ignoreCase: true)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateTransactionAssync(Transaction transaction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE TRANSACTIONS SET account_id = @account_id, objective_id = @objective_id, user_id = @user_id, transaction_Type = @type, split_type = @split, frequency = @frequency, category = @category, amount = @amount, concept = @concept, isRecurring = @isRecurring, transaction_date = @date, end_date = @end_date WHERE transaction_id = @transaction_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transaction_id", transaction.Transaction_id);
                    command.Parameters.AddWithValue("@account_id", transaction.Account_id);
                    command.Parameters.AddWithValue("@user_id", transaction.User_id);
                    command.Parameters.AddWithValue("@objective_id", transaction.Objective_id > 0 ? transaction.Objective_id : DBNull.Value);
                    command.Parameters.AddWithValue("@type", transaction.Transaction_type.ToString());
                    command.Parameters.AddWithValue("@split", transaction.Splittype.ToString());
                    command.Parameters.AddWithValue("@frequency", transaction.IsRecurring ? transaction.Frequency.ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@category", transaction.Category);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@concept", transaction.Concept);
                    command.Parameters.AddWithValue("@isRecurring", transaction.IsRecurring);
                    command.Parameters.AddWithValue("@date", transaction.Transaction_date);
                    command.Parameters.AddWithValue("@end_date", transaction.End_date == DateTime.MinValue ? DBNull.Value : transaction.End_date);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteTransactionAssync(int transactions_id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM TRANSACTIONS WHERE transaction_id = @transaction_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transaction_id", transactions_id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> AddTransactionAssync(Transaction transaction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    INSERT INTO TRANSACTIONS (account_id, user_id, objective_id, category, amount, transaction_Type, concept, isRecurring, frequency, end_date, split_type) 
                    VALUES (@account_id, @user_id, @objective_id, @category, @amount, @type, @concept, @isRecurring, @frequency, @end_date, @split);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", transaction.Account_id);
                    command.Parameters.AddWithValue("@user_id", transaction.User_id);
                    command.Parameters.AddWithValue("@objective_id", transaction.Objective_id > 0 ? transaction.Objective_id : DBNull.Value);
                    command.Parameters.AddWithValue("@category", transaction.Category);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@type", transaction.Transaction_type.ToString());
                    command.Parameters.AddWithValue("@concept", transaction.Concept);
                    command.Parameters.AddWithValue("@isRecurring", transaction.IsRecurring);
                    command.Parameters.AddWithValue("@frequency", transaction.IsRecurring ? transaction.Frequency.ToString() : DBNull.Value);
                    command.Parameters.AddWithValue("@end_date", transaction.End_date == DateTime.MinValue ? DBNull.Value : transaction.End_date);
                    command.Parameters.AddWithValue("@split", transaction.Splittype.ToString());

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }
    }
}