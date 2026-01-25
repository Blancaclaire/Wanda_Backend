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
        private readonly IAccountService _accountService;
        private readonly IAccountUsersRepository _accountUsersRepository;

        public UserService(IUserRepository userRepository, IAccountService accountService, IAccountUsersRepository accountUsersRepository)
        {
            _userRepository = userRepository;
            _accountService = accountService;
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

            //Añade la nueva Account a partir de User a la tabla de ACCOUNTS
            int accountId = await _accountService.AddPersonalAccountAsync(user.Name);

            var accountUser = new AccountUsers
            {
                User_id = userId,
                Account_id = accountId,
                Role = AccountUsers.UserRole.admin
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

                var UserExistente = await _userRepository.GetByIdAsync(id);

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
            // 1. Verificar si el usuario existe
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("El User no existe");
            }

            // 2. Buscar y borrar la cuenta PERSONAL del usuario
            // Necesitarás un método en el repositorio para encontrar la cuenta personal asociada al user_id
            var accounts = await _accountService.GetAllAsync(); // O un método especializado
            var personalAccount = accounts.FirstOrDefault(a =>
                a.Account_Type == Account.AccountType.personal &&
                a.Name == user.Name); // O mediante una consulta a ACCOUNT_USERS para mayor precisión

            if (personalAccount != null)
            {
                // Al borrar la cuenta, el ON DELETE CASCADE del SQL limpiará ACCOUNT_USERS para esta cuenta
                await _accountService.DeleteAsync(personalAccount.Account_id);
            }

            // 3. Borrar el usuario
            // El ON DELETE CASCADE que configuramos en SQL limpiará su participación en cuentas compartidas
            await _userRepository.DeleteAsync(id);

        }

    }

}