using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestauranteAPI.Data;
using RestauranteAPI.Services;

namespace RestauranteAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _AccountRepository;
        public AccountService(IAccountRepository AccountRepository)
        {
            _AccountRepository = AccountRepository;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _AccountRepository.GetAllAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _AccountRepository.GetByIdAsync(id);
        }


        public async Task AddAsync(Account account)
        {
            await _AccountRepository.AddAsync(account);
        }

        public async Task UpdateAsync(Account account)
        {
            await _AccountRepository.UpdateAsync(account);
        }

        public async Task DeleteAsync(int id)
        {
           var account = await _AccountRepository.GetByIdAsync(id);
           if (account == null)
           {
               throw KeyNotFoundException("Producto no encontrado");
           }
           await _AccountRepository.DeleteAsync(id);
        }

        private Exception KeyNotFoundException(string account)
        {
            throw new NotImplementedException();
        }

        public async Task InicializarDatosAsync()
        {
            await _AccountRepository.InicializarDatosAsync();
        }

    }
}


