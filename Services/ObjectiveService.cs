using Models;
using wandaAPI.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;

namespace wandaAPI.Services
{   

    public class ObjetiveService : IObjectiveService
    {

        private readonly IObjectiveRepository _objectiveRepository;
        private readonly IAccountService _accountService;
        private readonly IAccountUsersRepository _accountUsersRepository;

        public ObjetiveService(IObjectiveRepository objectiveRepository, IAccountService accountService, IAccountUsersRepository accountUsersRepository)
        {
            _objectiveRepository = objectiveRepository;
            _accountService = accountService;
            _accountUsersRepository = accountUsersRepository;
        }

        public async Task<List<Objective>> GetByAccountAsync(int accountId)
        {
            if (accountId <= 0) throw new ArgumentException("Por favor indique un numero valido"); ;
            var objectives = await _objectiveRepository.GetByAccountIdAsync(accountId);
            return objectives;
        }
        public async Task<Objective> CreateObjectiveAsync(ObjetiveCreateDto ObjectiveDto, Objective objective){
            if (ObjectiveDto == null) throw new ArgumentNullException(nameof(ObjectiveDto));
            if (string.IsNullOrWhiteSpace(ObjectiveDto.Name)) throw new ArgumentException("El nombre del objetivo es obligatorio.");
            var newObjective = new Objective
            {
            Account_id = ObjectiveDto.Account_id,
            Name = ObjectiveDto.Name,
            Target_amount = ObjectiveDto.Target_amount,
            Deadline = ObjectiveDto.Deadline,
            Current_save = 0,
            };
            await _objectiveRepository.AddAsync(newObjective);
        }
        
        public async Task<Objective?> AddFundsAsync(int objectiveId, double amount){

        }
        public async Task<int> GetProgressAsync(int acountId){

        }
        public async Task<Objective> DeleteAsync(int id){

        }

}
}