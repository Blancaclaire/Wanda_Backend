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
        public async Task<List<Transaction>> GetTransactionsByAccountAsync(int accountId)
        {
            var transactions = new List<Transaction>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT transaction_id, account_id, user_id, objective_id, category, amount, transaction_type, concept, transaction_date, isRecurring, frequency, end_date, split_type, last_execution_date FROM TRANSACTIONS WHERE account_id = @account_id;";

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
                                Transaction_type = reader.GetString(6),
                                Concept = reader.GetString(7),
                                Transaction_date = reader.GetDateTime(8),
                                IsRecurring = reader.GetBoolean(9),
                                Frequency = reader.IsDBNull(10) ? null : reader.GetString(10),
                                End_date = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                                Split_type = reader.GetString(12),
                                Last_execution_date = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13)
                            });
                        }
                    }
                }
            }
            return transactions;
        }

        public async Task<Transaction?> GetTransactionAsync(int transaction_id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT transaction_id, account_id, user_id, objective_id, category, amount, transaction_type, concept, transaction_date, isRecurring, frequency, end_date, split_type, last_execution_date FROM TRANSACTIONS WHERE transaction_id = @transactions_id;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transactions_id", transaction_id);

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
                                Transaction_type = reader.GetString(6),
                                Concept = reader.GetString(7),
                                Transaction_date = reader.GetDateTime(8),
                                IsRecurring = reader.GetBoolean(9),
                                Frequency = reader.IsDBNull(10) ? null : reader.GetString(10),
                                End_date = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                                Split_type = reader.GetString(12),
                                Last_execution_date = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"UPDATE TRANSACTIONS SET 
                                account_id = @account_id, 
                                objective_id = @objective_id, 
                                user_id = @user_id, 
                                transaction_type = @type, 
                                split_type = @split, 
                                frequency = @frequency, 
                                category = @category, 
                                amount = @amount, 
                                concept = @concept, 
                                isRecurring = @isRecurring, 
                                transaction_date = @date, 
                                end_date = @end_date 
                                WHERE transaction_id = @transaction_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transaction_id", transaction.Transaction_id);
                    command.Parameters.AddWithValue("@account_id", transaction.Account_id);
                    command.Parameters.AddWithValue("@user_id", transaction.User_id);
                    command.Parameters.AddWithValue("@objective_id", transaction.Objective_id > 0 ? transaction.Objective_id : DBNull.Value);
                    command.Parameters.AddWithValue("@type", transaction.Transaction_type);
                    command.Parameters.AddWithValue("@split", transaction.Split_type);
                    command.Parameters.AddWithValue("@frequency", (object)transaction.Frequency ?? DBNull.Value);
                    command.Parameters.AddWithValue("@category", transaction.Category);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@concept", transaction.Concept);
                    command.Parameters.AddWithValue("@isRecurring", transaction.IsRecurring);
                    command.Parameters.AddWithValue("@date", transaction.Transaction_date);
                    command.Parameters.AddWithValue("@end_date", (object)transaction.End_date ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteTransactionAsync(int transaction_id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM TRANSACTIONS WHERE transaction_id = @transaction_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transaction_id", transaction_id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> AddTransactionAsync(Transaction transaction)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Añadida la columna transaction_date para que se guarde la fecha del objeto
                string query = @"
                    INSERT INTO TRANSACTIONS (account_id, user_id, objective_id, category, amount, transaction_type, concept, isRecurring, frequency, transaction_date, end_date, split_type, last_execution_date ) 
                    VALUES (@account_id, @user_id, @objective_id, @category, @amount, @type, @concept, @isRecurring, @frequency, @date, @end_date, @split, @last_execution_date);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", transaction.Account_id);
                    command.Parameters.AddWithValue("@user_id", transaction.User_id);
                    command.Parameters.AddWithValue("@objective_id", transaction.Objective_id > 0 ? transaction.Objective_id : DBNull.Value);
                    command.Parameters.AddWithValue("@category", transaction.Category);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@type", transaction.Transaction_type);
                    command.Parameters.AddWithValue("@concept", transaction.Concept);
                    command.Parameters.AddWithValue("@isRecurring", transaction.IsRecurring);
                    command.Parameters.AddWithValue("@frequency", (object)transaction.Frequency ?? DBNull.Value);
                    command.Parameters.AddWithValue("@date", transaction.Transaction_date);
                    command.Parameters.AddWithValue("@end_date", (object)transaction.End_date ?? DBNull.Value);
                    command.Parameters.AddWithValue("@split", transaction.Split_type);
                    command.Parameters.AddWithValue("@last_execution_date", (object)transaction.Last_execution_date ?? DBNull.Value);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }


        public async Task UpdateLastExecutionAsync(int transactionId, DateTime date)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "UPDATE TRANSACTIONS SET last_execution_date = @date WHERE transaction_id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", transactionId);
                    command.Parameters.AddWithValue("@date", date);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        
        public async Task<List<Transaction>> GetRecurringTransactionsAsync()
        {
            var transactions = new List<Transaction>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                
                string query = @"
            SELECT transaction_id, account_id, user_id, objective_id, category, amount, 
                   transaction_type, concept, transaction_date, isRecurring, frequency, 
                   end_date, split_type, last_execution_date 
            FROM TRANSACTIONS 
            WHERE isRecurring = 1 
            AND (end_date IS NULL OR end_date >= GETDATE())";

                using (var command = new SqlCommand(query, connection))
                {
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
                                Transaction_type = reader.GetString(6),
                                Concept = reader.GetString(7),
                                Transaction_date = reader.GetDateTime(8),
                                IsRecurring = reader.GetBoolean(9),
                                Frequency = reader.IsDBNull(10) ? null : reader.GetString(10),
                                End_date = reader.IsDBNull(11) ? (DateTime?)null : reader.GetDateTime(11),
                                Split_type = reader.GetString(12),
                                Last_execution_date = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13)
                            });
                        }
                    }
                }
            }
            return transactions;
        }



    }
}