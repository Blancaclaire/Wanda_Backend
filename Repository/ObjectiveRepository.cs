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
                string query = "SELECT objective_id, account_id, name, target_amount, current_save, deadline, objective_picture_url FROM OBJECTIVES WHERE account_id = @accountId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@accountId", accountId);

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
                                Objective_picture_url = reader.GetString(6)
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
                string query = "SELECT objective_id, account_id, name, target_amount, current_save, deadline, objective_picture_url FROM OBJECTIVES WHERE objective_id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

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
                                Objective_picture_url = reader.GetString(6)
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
                                 VALUES (@accId, @name, @target, @current, @deadline, @url);
                                 SELECT SCOPE_IDENTITY();";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@accId", objective.Account_id);
                    command.Parameters.AddWithValue("@name", objective.Name);
                    command.Parameters.AddWithValue("@target", objective.Target_amount);
                    command.Parameters.AddWithValue("@current", objective.Current_save);
                    command.Parameters.AddWithValue("@deadline", objective.Deadline);
                    command.Parameters.AddWithValue("@url", objective.Objective_picture_url);

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
                string query = @"UPDATE OBJECTIVES SET name = @name, target_amount = @target, current_save = @current, 
                                 deadline = @deadline, objective_picture_url = @url WHERE objective_id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", objective.Objective_id);
                    command.Parameters.AddWithValue("@name", objective.Name);
                    command.Parameters.AddWithValue("@target", objective.Target_amount);
                    command.Parameters.AddWithValue("@current", objective.Current_save);
                    command.Parameters.AddWithValue("@deadline", objective.Deadline);
                    command.Parameters.AddWithValue("@url", objective.Objective_picture_url);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "DELETE FROM OBJECTIVES WHERE objective_id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}