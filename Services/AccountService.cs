using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models; // Asegúrate de que el namespace sea correcto
using wandaAPI.Repositories; // Ajustado según tus mensajes anteriores

namespace wandaAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Account account)
        {
            await _accountRepository.AddAsync(account);
        }

        public async Task UpdateAsync(Account account)
        {
            var existingAccount = await _accountRepository.GetByIdAsync(account.AccountId);
            if (existingAccount == null)
            {
                throw new KeyNotFoundException("Cuenta no encontrada");
            }
            await _accountRepository.UpdateAsync(account);
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Cuenta no encontrada");
            }
            await _accountRepository.DeleteAsync(id);
        }

        public async Task InicializarDatosAsync()
        {
            await _accountRepository.InicializarDatosAsync();
        }
    }
}