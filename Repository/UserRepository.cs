using Models;
using Microsoft.Data.SqlClient;
namespace wandaAPI.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("wandaDb") ?? "Not found";
        }


        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT user_id, name, email, password FROM USERS";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var User = new User
                            {
                                User_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3)
                            };

                            users.Add(User);
                        }
                    }
                }
            }
            return users;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            User user1 = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT user_id, name, email, password FROM USERS WHERE user_id = @user_id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user1 = new User
                            {
                                User_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return user1;
        }


        public async Task AddAsync(User user1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO USERS (name, email, password) VALUES (@name, @email, @password);";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", user1.Name);
                    command.Parameters.AddWithValue("@email", user1.Email);
                    command.Parameters.AddWithValue("@password", user1.Password);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(User user1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE USERS SET name = @name, email = @email, password = @password WHERE user_id = @user_id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", user1.User_id);
                    command.Parameters.AddWithValue("@name", user1.Name);
                    command.Parameters.AddWithValue("@email", user1.Email);
                    command.Parameters.AddWithValue("@password", user1.Password);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM USERS WHERE user_id = @user_id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


    }
}