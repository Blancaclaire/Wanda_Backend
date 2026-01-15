using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace wandaAPI.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);
        Task InicializarDatosAsync();
    }
}