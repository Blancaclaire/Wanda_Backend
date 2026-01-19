using Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

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
            var Objectives = new List<Objective>();

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
                            var Objective = new Objective
                            {
                                Objective_id = reader.GetInt32(0),
                                Account_id = reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Target_amount = reader.GetDouble(3),
                                Current_save = reader.GetDouble(4),
                                Deadline = reader.GetDateTime(5),
                                Objective_picture_url = reader.GetString(6)
                            };
                            Objectives.Add(Objective);
                        }
                    }
                }
            }
            return Objectives;
        }

        public async Task ObjectiveEdit(Objective objective)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Objectives SET name = @name, target_amount = @target_amount, current_save = @current_save, deadline = @deadline, objective_picture_url = @url WHERE Objective_id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", objective.Objective_id);
                    command.Parameters.AddWithValue("@name", objective.Name);
                    command.Parameters.AddWithValue("@target_amount", objective.Target_amount);
                    command.Parameters.AddWithValue("@current_save", objective.Current_save);
                    command.Parameters.AddWithValue("@deadline", objective.Deadline);
                    command.Parameters.AddWithValue("@url", (object)objective.Objective_picture_url ?? DBNull.Value);//puede ser null

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<int> AddAsync(Account account)
        {
            // using (var connection = new SqlConnection(_connectionString))
            // {
            //     await connection.OpenAsync();

            //     string query = "INSERT INTO ObjectiveS (name, Objective_type, amount, weekly_budget, monthly_budget, Objective_picture_url) VALUES (@name, @Objective_type, @amount, @weekly_budget, @monthly_budget, @Objective_picture_url); SELECT SCOPE_IDENTITY();";
            //     using (var command = new SqlCommand(query, connection))
            //     {
            //         command.Parameters.AddWithValue("@name", Objective1.Name);
            //         command.Parameters.AddWithValue("@Objective_type", Objective1.Objective_Type.ToString());
            //         command.Parameters.AddWithValue("@amount", Objective1.Amount);
            //         command.Parameters.AddWithValue("@weekly_budget", Objective1.Weekly_budget);
            //         command.Parameters.AddWithValue("@monthly_budget", Objective1.Monthly_budget);
            //         command.Parameters.AddWithValue("@Objective_picture_url", (object)Objective1.Objective_picture_url ?? DBNull.Value);


            // ExecuteScalar devuelve la primera columna de la primera fila->id
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
            //     }
            // }
        }

        public async Task UpdateAsync(Account account)
        {
            // using (var connection = new SqlConnection(_connectionString))
            // {
            //     await connection.OpenAsync();

            //     string query = "UPDATE ObjectiveS SET name = @name, Objective_type = @Objective_type, amount = @amount, weekly_budget = @weekly_budget, monthly_budget = @monthly_budget, Objective_picture_url = @Objective_picture_url  WHERE Objective_id = @Objective_id";
            //     using (var command = new SqlCommand(query, connection))
            //     {
            //         command.Parameters.AddWithValue("@name", Objective1.Name);
            //         command.Parameters.AddWithValue("@Objective_type", Objective1.Objective_Type.ToString());
            //         command.Parameters.AddWithValue("@amount", Objective1.Amount);
            //         command.Parameters.AddWithValue("@weekly_budget", Objective1.Weekly_budget);
            //         command.Parameters.AddWithValue("@monthly_budget", Objective1.Monthly_budget);
            //         command.Parameters.AddWithValue("@Objective_picture_url", (object)Objective1.Objective_picture_url ?? DBNull.Value);

            //         command.Parameters.AddWithValue("@Objective_id", Objective1.Objective_id);

            //         await command.ExecuteNonQueryAsync();
            //     }
            // }
        }

        public async Task DeleteAsync(int id)
        {
            // using (var connection = new SqlConnection(_connectionString))
            // {
            //     await connection.OpenAsync();

            //     string query = "DELETE FROM ObjectiveS WHERE Objective_id = @Objective_id";
            //     using (var command = new SqlCommand(query, connection))
            //     {
            //         command.Parameters.AddWithValue("@Objective_id", id);

            //         await command.ExecuteNonQueryAsync();
            //     }
            // }
        }


    }
}
