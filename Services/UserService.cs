using Models;
using wandaAPI.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;

namespace wandaAPI.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountUsersRepository _accountUsersRepository;

        public UserService(IUserRepository userRepository, IAccountRepository accountRepository, IAccountUsersRepository accountUsersRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _accountUsersRepository = accountUsersRepository;
        }

        public async Task<List<User>> GetAllAsync()
        {

            var Users = await _userRepository.GetAllAsync();
            return Users;

        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var User = await _userRepository.GetByIdAsync(id);
            if (User == null || User.User_id < 0)
            {
                throw new KeyNotFoundException("El ID debe ser mayor que cero o no existe.");
            }
            return User;

        }

        public async Task AddAsync(UserCreateDTO user1)
        {

            //Comprobaciones antes de añadir el user

            var users = await _userRepository.GetAllAsync();
            foreach (var us in users)
            {
                if (us.Email.Equals(user1.Email))
                {
                    throw new InvalidOperationException($"El User '{user1.Email}' ya existe.");
                }
            }

            if (user1.Password.Length < 5)
            {
                throw new InvalidOperationException("La contraseña no puede tener menos de 5 carácteres");
            }

            var containsMayus = false;
            if (user1.Password.Any(char.IsUpper))
            {
                containsMayus = true;
            }
            else if (!containsMayus)
            {
                throw new InvalidOperationException("La contraseña no contine al menos una mayuscula");
            }

            var user = new User
            {
                Name = user1.Name,
                Email = user1.Email,
                Password = user1.Password,

            };

            //Añade el nuevo User a la tabla de USERS
            int userId = await _userRepository.AddAsync(user);

            //Creación automatica de cuenta personal
            var personalAccount = new Account
            {
                Name= user.Name,
                Account_Type = "personal",
                Amount = 0
            };
            
            //Añade la nueva account a la tabla de ACCOUNTS
            int accountId = await _accountRepository.AddAsync(personalAccount);

            var accountUser = new AccountUsers
            {
                User_id = userId,
                Account_id = accountId,
                Role = "admin"
            };

            //Añade en la tabla ACCOUNT_USERS el nuevo usuario y su nueva cuenta
            await _accountUsersRepository.AddAsync(accountUser);

        }

        public async Task UpdateAsync(int id, UserUpdateDTO user1)
        {
            try
            {

                if (user1.Password.Length < 5)
                {
                    throw new InvalidOperationException("La contraseña no puede tener menos de 5 carácteres");
                }

                var containsMayus = false;

                if (user1.Password.Any(char.IsUpper))
                {
                    containsMayus = true;
                }
                else if (!containsMayus)
                {
                    throw new InvalidOperationException("La contraseña no contine al menos una mayuscula");
                }

                var UserExistente = await GetByIdAsync(id);

                UserExistente.Name = user1.Name;
                UserExistente.Password = user1.Password;

                await _userRepository.UpdateAsync(UserExistente);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            // Si el servicio no encontró el User para actualizar
            catch (KeyNotFoundException ex)
            {

                throw new KeyNotFoundException(ex.Message);
            }

        }

        public async Task DeleteAsync(int id)
        {
            var user1 = await GetByIdAsync(id);
            if (user1 == null)
            {
                throw new KeyNotFoundException("El User no existe");
            }

            await _userRepository.DeleteAsync(id);

        }

    }

}