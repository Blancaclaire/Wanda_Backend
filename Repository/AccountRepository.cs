using Models;
using Microsoft.Data.SqlClient;

namespace wandaAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("wandaDb") ?? throw new ArgumentNullException("Connection string not found");
        }


        public async Task<List<Account>> GetAllAsync()
        {
            var Accounts = new List<Account>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT account_id, name, account_type, amount, weekly_budget, monthly_budget, account_picture_url, creation_date FROM ACCOUNTS";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var Account = new Account
                            {
                                Account_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Account_Type = Enum.Parse<Account.AccountType>(reader.GetString(2), ignoreCase: true),
                                Amount = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)),
                                Weekly_budget = reader.IsDBNull(4) ? 0 : Convert.ToDouble(reader.GetDecimal(4)),
                                Monthly_budget = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
                                Account_picture_url = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Creation_date = reader.GetDateTime(7)
                            };

                            Accounts.Add(Account);
                        }
                    }
                }
            }
            return Accounts;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            Account Account1 = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT account_id, name, account_type, amount, weekly_budget, monthly_budget, account_picture_url, creation_date FROM ACCOUNTS WHERE account_id = @account_id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Account1 = new Account
                            {
                                Account_id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Account_Type = Enum.Parse<Account.AccountType>(reader.GetString(2), ignoreCase: true),
                                Amount = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)),
                                Weekly_budget = reader.IsDBNull(4) ? 0 : Convert.ToDouble(reader.GetDecimal(4)),
                                Monthly_budget = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
                                Account_picture_url = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Creation_date = reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return Account1;
        }


        public async Task<int> AddAsync(Account Account1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget, account_picture_url) VALUES (@name, @account_type, @amount, @weekly_budget, @monthly_budget, @account_picture_url); SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", Account1.Name);
                    command.Parameters.AddWithValue("@account_type", Account1.Account_Type.ToString());
                    command.Parameters.AddWithValue("@amount", Account1.Amount);
                    command.Parameters.AddWithValue("@weekly_budget", Account1.Weekly_budget);
                    command.Parameters.AddWithValue("@monthly_budget", Account1.Monthly_budget);
                    command.Parameters.AddWithValue("@account_picture_url", (object)Account1.Account_picture_url ?? DBNull.Value);


                    // ExecuteScalar devuelve la primera columna de la primera fila->id
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task UpdateAsync(Account Account1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE ACCOUNTS SET name = @name, account_type = @account_type, amount = @amount, weekly_budget = @weekly_budget, monthly_budget = @monthly_budget, account_picture_url = @account_picture_url  WHERE account_id = @account_id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", Account1.Name);
                    command.Parameters.AddWithValue("@account_type", Account1.Account_Type.ToString());
                    command.Parameters.AddWithValue("@amount", Account1.Amount);
                    command.Parameters.AddWithValue("@weekly_budget", Account1.Weekly_budget);
                    command.Parameters.AddWithValue("@monthly_budget", Account1.Monthly_budget);
                    command.Parameters.AddWithValue("@account_picture_url", (object)Account1.Account_picture_url ?? DBNull.Value);

                    command.Parameters.AddWithValue("@account_id", Account1.Account_id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM ACCOUNTS WHERE account_id = @account_id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@account_id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


    }
}
