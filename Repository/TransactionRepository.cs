using Models;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

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

        public async Task<Transaction?> GetTransactionAssync(int transactions_id)
        {
            var transaction = new Transaction();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT transaction_id, account_id, objective_id, user_id, transactions_Type, splitstype, frecuency, category, amount, concept, isRecurring, transactions_date, end_date FROM TRANSACTIONS WHERE transactions_id = @transactions_id;";

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

                string query = "UPDATE TRANSACTIONS SET account_id = @account_id, objective_id = @objective_id, user_id = @user_id, transactions_Type = @type, splitstype = @split, frecuency = @frecuency, category = @category, amount = @amount, concept = @concept, isRecurring = @isRecurring, transactions_date = @date, end_date = @end_date WHERE transaction_id = @transaction_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transaction_id", transaction.Transaction_id);
                    command.Parameters.AddWithValue("@account_id", transaction.Account_id);
                    command.Parameters.AddWithValue("@objective_id", transaction.Objective_id);
                    command.Parameters.AddWithValue("@user_id", transaction.User_id);
                    command.Parameters.AddWithValue("@type", transaction.Transaction_type.ToString());//enum to string para obtener el valor
                    command.Parameters.AddWithValue("@split", transaction.Splittype.ToString());
                    command.Parameters.AddWithValue("@frecuency", transaction.Frecuency.ToString());
                    command.Parameters.AddWithValue("@category", transaction.Category);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@concept", transaction.Concept);
                    command.Parameters.AddWithValue("@isRecurring", transaction.IsRecurring);
                    command.Parameters.AddWithValue("@date", transaction.Transaction_date);
                    command.Parameters.AddWithValue("@end_date", transaction.End_date);

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

                string query = "INSERT INTO TRANSACTIONS (transaction_id, account_id, user_id, objective_id, category, amount, transactions_Type, concept, isRecurring, frecuency, end_date, splitstype) VALUES (@transaction_id, @account_id, @user_id, @objective_id, @category, @amount, @type, @concept, @isRecurring, @frecuency, @end_date, @split);";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transaction_id", transaction.Transaction_id);
                    command.Parameters.AddWithValue("@account_id", transaction.Account_id);
                    command.Parameters.AddWithValue("@user_id", transaction.User_id);
                    command.Parameters.AddWithValue("@objective_id", transaction.Objective_id);
                    command.Parameters.AddWithValue("@category", transaction.Category);
                    command.Parameters.AddWithValue("@amount", transaction.Amount);
                    command.Parameters.AddWithValue("@type", transaction.Transaction_type.ToString());
                    command.Parameters.AddWithValue("@transaction", transaction.Concept);
                    command.Parameters.AddWithValue("@isRecurring", transaction.IsRecurring);
                    command.Parameters.AddWithValue("@frecuency", transaction.Frecuency.ToString());
                    command.Parameters.AddWithValue("@end_date", transaction.End_date);
                    command.Parameters.AddWithValue("@split", transaction.Splittype.ToString());

                    var result = await command.ExecuteNonQueryAsync();
                    return Convert.ToInt32(result);
                }
            }


        }




    }
}

