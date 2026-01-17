using Models;

namespace wandaAPI.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task AddPersonalAccountAsync(string userName); //Implementacion en UserService
        Task AddJointAccountAsync(JointAccountCreateDto dto, int ownerId); //Implementaciion en AccountController
        Task UpdateAsync(int id, AccountUpdateDto accountDto);
        Task DeleteAsync(int id);

    }
}
