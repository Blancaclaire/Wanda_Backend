using Models;
using wandaAPI.Repositories;
using DTOs; // Asegúrate de incluir el namespace de tus DTOs

namespace wandaAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountUsersRepository _accountUsersRepository;
        private readonly IUserRepository _userRepository;

        public AccountService(IAccountRepository accountRepository, IAccountUsersRepository accountUsersRepository, IUserRepository userRepository)
        {
            _accountRepository = accountRepository;
            _accountUsersRepository = accountUsersRepository;
            _userRepository = userRepository;
        }

        private async Task ValidateJointAccountDataAsync(JointAccountCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre de la cuenta es obligatorio.");

            var allMemberIds = dto.Member_Ids ?? new List<int>();
            var uniqueMembers = allMemberIds.Distinct().ToList();

            if (uniqueMembers.Count < 2)
                throw new ArgumentException("Una cuenta conjunta debe tener al menos dos miembros diferentes.");

            foreach (var id in uniqueMembers)
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"El usuario con ID {id} no existe en el sistema.");
            }
        }

        public async Task AddJointAccountAsync(JointAccountCreateDto dto)
        {
            await ValidateJointAccountDataAsync(dto);

            var jointAccount = new Account
            {
                Name = dto.Name,
                Account_type = "joint", 
                Amount = 0,
                Creation_date = DateTime.Now
            };

            int accountId = await _accountRepository.AddAsync(jointAccount);

            foreach (var userId in dto.Member_Ids.Distinct())
            {
                await _accountUsersRepository.AddAsync(new AccountUsers
                {
                    User_id = userId,
                    Account_id = accountId
                });
            }
        }

        public async Task<int> AddPersonalAccountAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(userName));
            }

            var personalAccount = new Account
            {
                Name = userName,
                Account_type = "personal", 
                Amount = 0
            };

            int accountId = await _accountRepository.AddAsync(personalAccount);

            if (accountId <= 0)
            {
                throw new Exception("No se pudo crear la cuenta en la base de datos.");
            }

            return accountId;
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("La account no existe");
            }

            await _accountRepository.DeleteAsync(id);
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) // Simplificado: si es null, no existe
            {
                throw new KeyNotFoundException("La cuenta no existe.");
            }
            return account;
        }

        public async Task UpdateAsync(int id, AccountUpdateDto accountDto)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(id);

            if (existingAccount == null)
            {
                throw new KeyNotFoundException("La cuenta que se desea actualizar no existe.");
            }

            existingAccount.Name = accountDto.Name;
            existingAccount.Weekly_budget = accountDto.Weekly_budget;
            existingAccount.Monthly_budget = accountDto.Monthly_budget;
            existingAccount.Account_picture_url = accountDto.Account_picture_url;

            if (existingAccount.Account_type == "personal")
            {
                existingAccount.Amount = accountDto.Amount;
            }

            await _accountRepository.UpdateAsync(existingAccount);
        }
    }
}