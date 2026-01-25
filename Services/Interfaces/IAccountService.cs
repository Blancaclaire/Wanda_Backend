using Models;

namespace wandaAPI.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task <int> AddPersonalAccountAsync(string userName); //Implementacion en UserService
        Task AddJointAccountAsync(JointAccountCreateDto dto); //Implementacion en AccountController
        Task UpdateAsync(int id, AccountUpdateDto accountDto);
        Task DeleteAsync(int id);

    }
}
