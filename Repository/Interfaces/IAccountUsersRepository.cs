using Models;

namespace wandaAPI.Repositories
{
    public interface IAccountUsersRepository
    {
        Task AddAsync(AccountUsers accountUser);

        Task<List<AccountUsers>> GetUsersByAccountIdAsync(int accountId);
    
    }

}
