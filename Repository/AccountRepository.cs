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

                string query = "SELECT account_id, name, account_type, amount, weekly_budget, monthly_budget, account_picture_url FROM ACCOUNTS";
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
                                Account_Type = reader.GetString(2),
                                Amount = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)),
                                Weekly_budget = reader.IsDBNull(4) ? 0 : Convert.ToDouble(reader.GetDecimal(4)),
                                Monthly_budget = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
                                Account_picture_url =  reader.IsDBNull(6) ? null : reader.GetString(6)
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

                string query = "SELECT account_id, name, account_type, amount, weekly_budget, monthly_budget, account_picture_url FROM ACCOUNTS WHERE account_id = @account_id";
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
                                Account_Type = reader.GetString(2),
                                Amount = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)),
                                Weekly_budget = reader.IsDBNull(4) ? 0 : Convert.ToDouble(reader.GetDecimal(4)),
                                Monthly_budget = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)),
                                Account_picture_url =  reader.IsDBNull(6) ? null : reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return Account1;
        }


        public async Task AddAsync(Account Account1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO ACCOUNTS (name, account_type, amount, weekly_budget, monthly_budget, account_picture_url) VALUES (@name, @account_type, @amount, @weekly_budget, @monthly_budget, @account_picture_url);";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", Account1.Name);
                    command.Parameters.AddWithValue("@account_type", Account1.Account_Type);
                    command.Parameters.AddWithValue("@amount", Account1.Amount);
                    command.Parameters.AddWithValue("@weekly_budget", Account1.Weekly_budget);
                    command.Parameters.AddWithValue("@monthly_budget", Account1.Monthly_budget);
                    command.Parameters.AddWithValue("@account_picture_url", Account1.Account_picture_url);

                    await command.ExecuteNonQueryAsync();
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
                    command.Parameters.AddWithValue("@account_type", Account1.Account_Type);
                    command.Parameters.AddWithValue("@amount", Account1.Amount);
                    command.Parameters.AddWithValue("@weekly_budget", Account1.Weekly_budget);
                    command.Parameters.AddWithValue("@monthly_budget", Account1.Monthly_budget);
                    command.Parameters.AddWithValue("@account_picture_url", Account1.Account_picture_url);

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

        // public async Task<List<Account>> GetAllAsync()
        // {
        //     var accounts = new List<Account>();
        //     using var connection = new SqlConnection(_connectionString);
        //     const string sql = "SELECT * FROM Accounts";
        //     using var command = new SqlCommand(sql, connection);

        //     await connection.OpenAsync();
        //     using var reader = await command.ExecuteReaderAsync();
        //     while (await reader.ReadAsync())
        //     {
        //         accounts.Add(MapReaderToAccount(reader));
        //     }
        //     return accounts;
        // }

        // public async Task<Account?> GetByIdAsync(int id)
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     const string sql = "SELECT * FROM Accounts WHERE AccountId = @id";
        //     using var command = new SqlCommand(sql, connection);
        //     command.Parameters.AddWithValue("@id", id);

        //     await connection.OpenAsync();
        //     using var reader = await command.ExecuteReaderAsync();
        //     if (await reader.ReadAsync()) return MapReaderToAccount(reader);
        //     return null;
        // }

        // public async Task AddAsync(Account account)
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     const string sql = @"INSERT INTO Accounts (Name, AccountType, Balance, WeeklyBudget, MonthlyBudget, AccountPictureUrl, Password) 
        //                          VALUES (@Name, @Type, @Balance, @Weekly, @Monthly, @Pic, @Pass)";
        //     using var command = new SqlCommand(sql, connection);
        //     AddParameters(command, account);

        //     await connection.OpenAsync();
        //     await command.ExecuteNonQueryAsync();
        // }

        // public async Task UpdateAsync(Account account)
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     const string sql = @"UPDATE Accounts SET Name=@Name, AccountType=@Type, Balance=@Balance, 
        //                          WeeklyBudget=@Weekly, MonthlyBudget=@Monthly, AccountPictureUrl=@Pic 
        //                          WHERE AccountId=@id";
        //     using var command = new SqlCommand(sql, connection);
        //     command.Parameters.AddWithValue("@id", account.AccountId);
        //     AddParameters(command, account);

        //     await connection.OpenAsync();
        //     await command.ExecuteNonQueryAsync();
        // }

        // public async Task DeleteAsync(int id)
        // {
        //     using var connection = new SqlConnection(_connectionString);
        //     const string sql = "DELETE FROM Accounts WHERE AccountId = @id";
        //     using var command = new SqlCommand(sql, connection);
        //     command.Parameters.AddWithValue("@id", id);

        //     await connection.OpenAsync();
        //     await command.ExecuteNonQueryAsync();
        // }

        // Helpers para evitar repetición
        // private static void AddParameters(SqlCommand cmd, Account acc)
        // {
        //     cmd.Parameters.AddWithValue("@Name", acc.Name);
        //     cmd.Parameters.AddWithValue("@Type", acc.AccountType);
        //     cmd.Parameters.AddWithValue("@Balance", acc.Balance);
        //     cmd.Parameters.AddWithValue("@Weekly", acc.WeeklyBudget);
        //     cmd.Parameters.AddWithValue("@Monthly", acc.MonthlyBudget);
        //     cmd.Parameters.AddWithValue("@Pic", acc.AccountPictureUrl);
        //     cmd.Parameters.AddWithValue("@Pass", acc.Password ?? (object)DBNull.Value);
        // }

        // private static Account MapReaderToAccount(SqlDataReader reader) => new()
        // {
        //     AccountId = (int)reader["AccountId"],
        //     Name = reader["Name"].ToString()!,
        //     AccountType = reader["AccountType"].ToString()!,
        //     Balance = Convert.ToDouble(reader["Balance"]),
        //     WeeklyBudget = Convert.ToDouble(reader["WeeklyBudget"]),
        //     MonthlyBudget = Convert.ToDouble(reader["MonthlyBudget"]),
        //     AccountPictureUrl = reader["AccountPictureUrl"].ToString()!,
        //     Password = reader["Password"].ToString()
        // };
    }
}