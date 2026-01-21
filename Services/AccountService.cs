using Models;
using wandaAPI.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;

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

        private async Task ValidateJointAccountDataAsync(JointAccountCreateDto dto, int ownerId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre de la cuenta es obligatorio.");

            
            var allMemberIds = dto.Member_Ids ?? new List<int>();
            var uniqueMembers = allMemberIds.Append(ownerId).Distinct().ToList();

            if (uniqueMembers.Count < 2)
                throw new ArgumentException("Una cuenta conjunta debe tener al menos dos miembros diferentes.");

            
            var owner = await _userRepository.GetByIdAsync(ownerId);
            if (owner == null) throw new KeyNotFoundException($"El usuario creador (ID: {ownerId}) no existe.");

            
            foreach (var id in allMemberIds)
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"El usuario invitado con ID {id} no existe en el sistema.");
            }
        }

        public async Task AddJointAccountAsync(JointAccountCreateDto dto, int ownerId)
        {
            await ValidateJointAccountDataAsync(dto, ownerId);

            var jointAccount = new Account
            {
                Name = dto.Name,
                Account_Type = Account.AccountType.joint,
                Amount = 0,
                Creation_date = DateTime.Now
            };

            int accountId = await _accountRepository.AddAsync(jointAccount);

            await _accountUsersRepository.AddAsync(new AccountUsers
            {
                User_id = ownerId,
                Account_id = accountId,
                Role = AccountUsers.UserRole.admin,
                Joined_at = DateTime.Now
            });

            var otherMembers = dto.Member_Ids.Where(id => id != ownerId).Distinct();

            foreach (var memberId in otherMembers)
            {
                await _accountUsersRepository.AddAsync(new AccountUsers
                {
                    User_id = memberId,
                    Account_id = accountId,
                    Role = AccountUsers.UserRole.member,
                    Joined_at = DateTime.Now
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
                Account_Type = Account.AccountType.personal,
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
            var accounts = await _accountRepository.GetAllAsync();
            return accounts;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null || account.Account_id < 0)
            {
                throw new KeyNotFoundException("El ID debe ser mayor que cero o no existe.");
            }
            return account;
        }

        public async Task UpdateAsync(int id, AccountUpdateDto accountDto)
        {
            try
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

                if (existingAccount.Account_Type == Account.AccountType.personal)
                {
                    existingAccount.Amount = accountDto.Amount;

                }

                await _accountRepository.UpdateAsync(existingAccount);

            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }

            catch (KeyNotFoundException ex)
            {

                throw new KeyNotFoundException(ex.Message);
            }

        }
    }
}