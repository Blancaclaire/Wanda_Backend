using Models;
using DTOs;
using wandaAPI.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;

namespace wandaAPI.Services
{
    public class ObjectiveService : IObjectiveService
    {
        private readonly IObjectiveRepository _objectiveRepository;

        public ObjectiveService(IObjectiveRepository objectiveRepository)
        {
            _objectiveRepository = objectiveRepository;
        }


        private void ValidateObjectiveData(string name, double targetAmount, double currentSave, DateTime deadline)
        {

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del objetivo es obligatorio.");


            if (targetAmount <= 0)
                throw new ArgumentException("La meta de ahorro debe ser un monto mayor a 0.");


            if (currentSave < 0)
                throw new ArgumentException("El ahorro actual no puede ser un valor negativo.");


            if (targetAmount < currentSave)
                throw new ArgumentException("La meta de ahorro no puede ser inferior al monto ya ahorrado.");


            if (deadline <= DateTime.Now)
                throw new ArgumentException("La fecha límite debe ser una fecha futura.");
        }

        public async Task<List<Objective>> GetByAccountAsync(int accountId)
        {
            if (accountId <= 0) throw new ArgumentException("El ID de cuenta no es válido.");

            var objectives = await _objectiveRepository.GetByAccountIdAsync(accountId);
            return objectives;
        }

        public async Task<Objective> CreateAsync(int accountId, ObjectiveCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            ValidateObjectiveData(dto.Name, dto.Target_amount, 0, dto.Deadline);

            var objective = new Objective
            {
                Account_id = accountId,
                Name = dto.Name,
                Target_amount = dto.Target_amount,
                Current_save = 0,
                Deadline = dto.Deadline,
                Objective_picture_url = dto.Objective_picture_url
            };

            int id = await _objectiveRepository.AddAsync(objective);
            objective.Objective_id = id;

            return objective;
        }

        public async Task<Objective?> GetByIdAsync(int id)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null) throw new KeyNotFoundException("El objetivo no existe.");
            return objective;
        }

        public async Task AddFundsAsync(int id, double amount)
        {
            if (amount <= 0) throw new ArgumentException("El monto debe ser positivo.");

            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null) throw new KeyNotFoundException("Objetivo no encontrado.");

            objective.Current_save += amount;
            await _objectiveRepository.UpdateAsync(objective);
        }

        public async Task UpdateAsync(int id, ObjectiveUpdateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = await _objectiveRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("El objetivo no existe.");

            
            ValidateObjectiveData(dto.Name, dto.Target_amount, dto.Current_save, dto.Deadline);

            existing.Name = dto.Name;
            existing.Target_amount = dto.Target_amount;
            existing.Current_save = dto.Current_save;
            existing.Deadline = dto.Deadline;
            existing.Objective_picture_url = dto.Objective_picture_url;

            await _objectiveRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null) throw new KeyNotFoundException("El objetivo no existe.");

            if (objective.Current_save > 0)
            {
                throw new InvalidOperationException("No se puede eliminar un objetivo con fondos.");
            }

            await _objectiveRepository.DeleteAsync(id);
        }
    }
}