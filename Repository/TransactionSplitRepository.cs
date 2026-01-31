using Models;
using Microsoft.Data.SqlClient;

namespace wandaAPI.Repositories
{
    public class TransactionSplitRepository : ITransactionSplitRepository
    {
        private readonly string _connectionString;

        public TransactionSplitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("wandaDb") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<int> AddAsync(TransactionSplit split)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                    INSERT INTO TRANSACTION_SPLITS (user_id, transaction_id, amount_assigned, status, paid_at) 
                    VALUES (@user_id, @transaction_id, @amount_assigned, @status, @paid_at);
                    SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", split.User_id);
                    command.Parameters.AddWithValue("@transaction_id", split.Transaction_id);
                    command.Parameters.AddWithValue("@amount_assigned", split.Amount_assigned);
                    command.Parameters.AddWithValue("@status", split.Status ?? "pending"); // Por defecto pending


                    command.Parameters.AddWithValue("@paid_at", (object)split.Paid_at ?? DBNull.Value);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<List<TransactionSplit>> GetAllByUserIdAsync(int userId)
        {
            var splits = new List<TransactionSplit>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                
                string query = @"SELECT split_id, user_id, transaction_id, amount_assigned, status, paid_at 
                         FROM TRANSACTION_SPLITS 
                         WHERE user_id = @user_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            splits.Add(MapReaderToSplit(reader));
                        }
                    }
                }
            }
            return splits;
        }

        public async Task<TransactionSplit?> GetByIdAsync(int splitId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT split_id, user_id, transaction_id, amount_assigned, status, paid_at FROM TRANSACTION_SPLITS WHERE split_id = @split_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@split_id", splitId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapReaderToSplit(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateStatusAsync(int splitId, string status, DateTime? paidAt)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"UPDATE TRANSACTION_SPLITS 
                                 SET status = @status, paid_at = @paid_at 
                                 WHERE split_id = @split_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@split_id", splitId);
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@paid_at", (object)paidAt ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<TransactionSplit>> GetByTransactionIdAsync(int transactionId)
        {
            var splits = new List<TransactionSplit>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT split_id, user_id, transaction_id, amount_assigned, status, paid_at FROM TRANSACTION_SPLITS WHERE transaction_id = @transaction_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@transaction_id", transactionId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            splits.Add(MapReaderToSplit(reader));
                        }
                    }
                }
            }
            return splits;
        }

        // Helper privado para no repetir el mapeo en cada método
        private TransactionSplit MapReaderToSplit(SqlDataReader reader)
        {
            return new TransactionSplit
            {
                Split_id = reader.GetInt32(0),
                User_id = reader.GetInt32(1),
                Transaction_id = reader.GetInt32(2),
                Amount_assigned = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)),
                Status = reader.GetString(4),
                Paid_at = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
            };
        }
    }
}