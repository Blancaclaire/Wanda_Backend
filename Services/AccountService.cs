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

        public AccountService(IAccountRepository accountRepository, IAccountUsersRepository accountUsersRepository)
        {
            _accountRepository = accountRepository;
            _accountUsersRepository = accountUsersRepository;
        }

        public async Task AddJointAccountAsync(JointAccountCreateDto dto, int ownerId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("El nombre de la cuenta es obligatorio.");
            if (dto.Member_Ids == null || dto.Member_Ids.Count == 0) throw new ArgumentException("Debes añadir al menos un miembro.");

            var jointAccount = new Account
            {
                Name = dto.Name,
                Account_Type = Account.AccountType.joint,
                Amount = 0
            };

            int accountId = await _accountRepository.AddAsync(jointAccount);

            var accountAdmin = new AccountUsers
            {
                User_id = ownerId,
                Account_id = accountId,
                Role = AccountUsers.UserRole.admin
            };

            //establece el dueño como admin y lo mete en ACCOUNTS_USERS
            await _accountUsersRepository.AddAsync(accountAdmin);


            //mete al resto de users que no son el dueño como members en ACCOUNTS_USERS
            foreach (var memberId in dto.Member_Ids)
            {
                if (memberId != ownerId)
                {
                    var accountMember = new AccountUsers
                    {
                        User_id = memberId,
                        Account_id = accountId,
                        Role = AccountUsers.UserRole.member
                    };

                    await _accountUsersRepository.AddAsync(accountMember);
                }
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