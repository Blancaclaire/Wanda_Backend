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
            var accounts = new List<Account>();
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Accounts";
            using var command = new SqlCommand(sql, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                accounts.Add(MapReaderToAccount(reader));
            }
            return accounts;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT * FROM Accounts WHERE AccountId = @id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapReaderToAccount(reader);
            return null;
        }

        public async Task AddAsync(Account account)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"INSERT INTO Accounts (Name, AccountType, Balance, WeeklyBudget, MonthlyBudget, AccountPictureUrl, Password) 
                                 VALUES (@Name, @Type, @Balance, @Weekly, @Monthly, @Pic, @Pass)";
            using var command = new SqlCommand(sql, connection);
            AddParameters(command, account);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = @"UPDATE Accounts SET Name=@Name, AccountType=@Type, Balance=@Balance, 
                                 WeeklyBudget=@Weekly, MonthlyBudget=@Monthly, AccountPictureUrl=@Pic 
                                 WHERE AccountId=@id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", account.AccountId);
            AddParameters(command, account);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "DELETE FROM Accounts WHERE AccountId = @id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // Helpers para evitar repetición
        private static void AddParameters(SqlCommand cmd, Account acc)
        {
            cmd.Parameters.AddWithValue("@Name", acc.Name);
            cmd.Parameters.AddWithValue("@Type", acc.AccountType);
            cmd.Parameters.AddWithValue("@Balance", acc.Balance);
            cmd.Parameters.AddWithValue("@Weekly", acc.WeeklyBudget);
            cmd.Parameters.AddWithValue("@Monthly", acc.MonthlyBudget);
            cmd.Parameters.AddWithValue("@Pic", acc.AccountPictureUrl);
            cmd.Parameters.AddWithValue("@Pass", acc.Password ?? (object)DBNull.Value);
        }

        private static Account MapReaderToAccount(SqlDataReader reader) => new()
        {
            AccountId = (int)reader["AccountId"],
            Name = reader["Name"].ToString()!,
            AccountType = reader["AccountType"].ToString()!,
            Balance = Convert.ToDouble(reader["Balance"]),
            WeeklyBudget = Convert.ToDouble(reader["WeeklyBudget"]),
            MonthlyBudget = Convert.ToDouble(reader["MonthlyBudget"]),
            AccountPictureUrl = reader["AccountPictureUrl"].ToString()!,
            Password = reader["Password"].ToString()
        };
    }
}