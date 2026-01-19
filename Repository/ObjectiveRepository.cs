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


        public async Task<List<Objective>> GetAllAsync()
        {
            var Objectives = new List<Objective>();

            // using (var connection = new SqlConnection(_connectionString))
            // {
            //     await connection.OpenAsync();

            //     string query = "SELECT Objective_id, name, Objective_type, amount, weekly_budget, monthly_budget, Objective_picture_url, creation_date FROM ObjectiveS";
            //     using (var command = new SqlCommand(query, connection))
            //     {
            //         using (var reader = await command.ExecuteReaderAsync())
            //         {
            //             while (await reader.ReadAsync())
            //             {
            //                 var Objective = new Objective
            //                 {
            //                     Objective_id = reader.GetInt32(0),
            //                     Name = reader.GetString(1),
            //                     Objective_Type = Enum.Parse<Objective.ObjectiveType>(reader.GetString(2), ignoreCase: true),
            //                     Amount = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)),
            //                     Weekly_budget = reader.IsDBNull(4) ? 0 : Convert.ToDouble(reader.GetDecimal(4)),
            //                     Monthly_budget = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
            //                     Objective_picture_url = reader.IsDBNull(6) ? null : reader.GetString(6),
            //                     Creation_date = reader.GetDateTime(7)
            //                 };

            //                 Objectives.Add(Objective);
            //             }
            //         }
            //     }
            // }
            return Objectives;
        }

        public async Task<Objective?> GetByIdAsync(int id)
        {
            Objective Objective1 = null;

            // using (var connection = new SqlConnection(_connectionString))
            // {
            //     await connection.OpenAsync();

            //     string query = "SELECT Objective_id, name, Objective_type, amount, weekly_budget, monthly_budget, Objective_picture_url, creation_date FROM ObjectiveS WHERE Objective_id = @Objective_id";
            //     using (var command = new SqlCommand(query, connection))
            //     {
            //         command.Parameters.AddWithValue("@Objective_id", id);

            //         using (var reader = await command.ExecuteReaderAsync())
            //         {
            //             if (await reader.ReadAsync())
            //             {
            //                 Objective1 = new Objective
            //                 {
            //                     Objective_id = reader.GetInt32(0),
            //                     Name = reader.GetString(1),
            //                     Objective_Type = Enum.Parse<Objective.ObjectiveType>(reader.GetString(2), ignoreCase: true),
            //                     Amount = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)),
            //                     Weekly_budget = reader.IsDBNull(4) ? 0 : Convert.ToDouble(reader.GetDecimal(4)),
            //                     Monthly_budget = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
            //                     Objective_picture_url = reader.IsDBNull(6) ? null : reader.GetString(6),
            //                     Creation_date = reader.GetDateTime(7)
            //                 };
            //             }
            //         }
            //     }
            // }
            return Objective1;
        }


        public async Task<int> AddAsync(Objective Objective1)
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

        public async Task UpdateAsync(Objective Objective1)
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
