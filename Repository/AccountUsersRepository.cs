using Models;
using Microsoft.Data.SqlClient;
namespace wandaAPI.Repositories
{
    public class AccountUsersRepository : IAccountUsersRepository

    {

        private readonly string _connectionString;

        public AccountUsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("wandaDb") ?? "Not found";
        }


        public async Task AddAsync(AccountUsers accountUsers)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO ACCOUNT_USERS (user_id, account_id) VALUES (@user_id, @account_id);";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", accountUsers.User_id);
                    command.Parameters.AddWithValue("@account_id", accountUsers.Account_id);
                    await command.ExecuteNonQueryAsync();

                }
            }
        }



    }
}
