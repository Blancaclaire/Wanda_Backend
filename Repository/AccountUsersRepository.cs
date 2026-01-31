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

        public async Task<List<AccountUsers>> GetUsersByAccountIdAsync(int accountId)
        {
            var usersList = new List<AccountUsers>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT user_id, account_id, joined_at FROM ACCOUNT_USERS WHERE account_id = @account_id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", accountId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            usersList.Add(new AccountUsers
                            {
                                User_id = reader.GetInt32(0),
                                Account_id = reader.GetInt32(1),
                                Joined_at = reader.GetDateTime(2)
                            });
                        }
                    }
                }
            }
            return usersList;
        }


    }
}
