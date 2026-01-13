using Models;
using Microsoft.Data.SqlClient;
namespace wandaAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {

        private readonly string _connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("wandaDb") ?? "Not found";
        }


        public async Task<List<Account>> GetAllAsync()
        {
            var Accounts = new List<Account>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
            }
            return ;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            Account Account1 = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
            }
            return ;
        }


        public async Task AddAsync(Account Account1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
            }
        }

        public async Task UpdateAsync(Account Account1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
            }
        }


    }
}