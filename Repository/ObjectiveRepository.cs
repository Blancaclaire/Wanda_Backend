using Models;
using Microsoft.Data.SqlClient;

namespace wandaAPI.Repositories
{
    public class ObjectiveRepository : IObjectiveRepository
    {
        private readonly string _connectionString;

        public ObjectiveRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("wandaDb") ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<List<Objective>> GetByAccountIdAsync(int accountId)
        {
            var objectives = new List<Objective>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT objective_id, account_id, name, target_amount, current_save, deadline, objective_picture_url FROM OBJECTIVES WHERE account_id = @account_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", accountId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            objectives.Add(new Objective
                            {
                                Objective_id = reader.GetInt32(0),
                                Account_id = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Target_amount = reader.GetDouble(3),
                                Current_save = reader.GetDouble(4),
                                Deadline = reader.GetDateTime(5),
                                Objective_picture_url = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return objectives;
        }

        public async Task<Objective?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT objective_id, account_id, name, target_amount, current_save, deadline, objective_picture_url FROM OBJECTIVES WHERE objective_id = @objective_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@objective_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Objective
                            {
                                Objective_id = reader.GetInt32(0),
                                Account_id = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Target_amount = reader.GetDouble(3),
                                Current_save = reader.GetDouble(4),
                                Deadline = reader.GetDateTime(5),
                                Objective_picture_url = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<int> AddAsync(Objective objective)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO OBJECTIVES (account_id, name, target_amount, current_save, deadline, objective_picture_url) 
                                 VALUES (@account_id, @name, @target_amount, @current_save, @deadline, @objective_picture_url);
                                 SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", objective.Account_id);
                    command.Parameters.AddWithValue("@name", objective.Name);
                    command.Parameters.AddWithValue("@target_amount", objective.Target_amount);
                    command.Parameters.AddWithValue("@current_save", objective.Current_save);
                    command.Parameters.AddWithValue("@deadline", objective.Deadline);
                    command.Parameters.AddWithValue("@objective_picture_url", (object)objective.Objective_picture_url ?? DBNull.Value);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task UpdateAsync(Objective objective)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"UPDATE OBJECTIVES SET name = @name, target_amount = @target_amount, current_save = @current_save, 
                                 deadline = @deadline, objective_picture_url = @objective_picture_url WHERE objective_id = @objective_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@objective_id", objective.Objective_id);
                    command.Parameters.AddWithValue("@name", objective.Name);
                    command.Parameters.AddWithValue("@target_amount", objective.Target_amount);
                    command.Parameters.AddWithValue("@current_save", objective.Current_save);
                    command.Parameters.AddWithValue("@deadline", objective.Deadline);
                    command.Parameters.AddWithValue("@objective_picture_url", (object)objective.Objective_picture_url ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE objective_id = @objective_id FROM TRANSACTIONS WHERE objective_id = @objective_id ";

                using (var command = new SqlCommand(query,connection))

                {
                    command.Parameters.AddWithValue("@objective_id", id);
                    await command.ExecuteNonQueryAsync();
                    command.CommandText = "DELETE objective_id = @objective_id FROM OBJECTIVES WHERE objective_id = @objective_id";
                    command.ExecuteNonQuery();
                }
                await connection.OpenAsync();

            }
        }
        
    }
}