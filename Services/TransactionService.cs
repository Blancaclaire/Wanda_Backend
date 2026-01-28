using Models;
using DTOs;
using wandaAPI.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using Microsoft.VisualBasic;

namespace wandaAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IObjectiveRepository _objectiveRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IObjectiveRepository objectiveRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _objectiveRepository = objectiveRepository;
        }
        private void ValidateTransactionData(TransactionCreateDTO dto)
        {
            if (dto.Amount <= 0)
                throw new ArgumentException("El monto (Amount) debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("La categoría es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.Concept))
                throw new ArgumentException("El concepto es obligatorio.");

            if (dto.User_id <= 0)
                throw new ArgumentException("El User_id es obligatorio y debe ser válido.");

            if (!Enum.IsDefined(typeof(TransactionCreateDTO.ETransaction_type), dto.Transaction_type))
                throw new ArgumentException("El tipo de transacción no es válido.");

            if (!Enum.IsDefined(typeof(TransactionCreateDTO.Split_type), dto.Splittype))
                throw new ArgumentException("El tipo de división de la transacción no es válido.");

            if (dto.Transaction_type == TransactionCreateDTO.ETransaction_type.saving && dto.Objective_id <= 0)
            {
                throw new ArgumentException("Si la transacción es de tipo 'saving', debes especificar un Objective_id válido.");
            }

            if (dto.IsRecurring)
            {
                if (!Enum.IsDefined(typeof(TransactionCreateDTO.EFrequency), dto.Frequency))
                    throw new ArgumentException("La frecuencia seleccionada no es válida.");

                if (dto.End_date == default || dto.End_date <= dto.Transaction_date)
                {
                    throw new ArgumentException("Para una transacción recurrente, la fecha de fin debe ser posterior a la fecha de la transacción.");
                }
            }
            if (dto.Transaction_date > DateTime.Now)
            {
                throw new ArgumentException("No se pueden registrar transacciones con fecha futura .");
            }
        }

        public async Task<Transaction> CreateAsync(int accountId, TransactionCreateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            ValidateTransactionData(dto);

            var personalAccount = await _accountRepository.GetByIdAsync(dto.User_id);
            if (personalAccount == null) throw new KeyNotFoundException("No se encontró la cuenta personal.");

            if (personalAccount.Amount < dto.Amount)
                throw new InvalidOperationException("Saldo insuficiente en la cuenta personal.");

            personalAccount.Amount -= dto.Amount;
            await _accountRepository.UpdateAsync(personalAccount);

            if (dto.Transaction_type == TransactionCreateDTO.ETransaction_type.saving && dto.Objective_id > 0)
            {
                var objective = await _objectiveRepository.GetByIdAsync(dto.Objective_id);
                if (objective != null)
                {
                    objective.Current_save += dto.Amount;
                    await _objectiveRepository.UpdateAsync(objective);
                }
            }

            var transaction = new Transaction
            {
                Account_id = accountId,
                User_id = dto.User_id,
                Objective_id = dto.Objective_id,
                Category = dto.Category,
                Amount = dto.Amount,
                Transaction_type = (Transaction.ETransaction_type)dto.Transaction_type,
                Concept = dto.Concept,
                Transaction_date = dto.Transaction_date,
                IsRecurring = dto.IsRecurring,
                Frequency = (Transaction.EFrequency)dto.Frequency,
                End_date = dto.End_date,
                Splittype = (Transaction.Split_type)dto.Splittype
            };

            int id = await _transactionRepository.AddTransactionAssync(transaction);
            transaction.Transaction_id = id;

            return transaction;
        }

        public async Task DeleteAsync(int id)
        {
            var transaction = await _transactionRepository.GetTransactionAssync(id);
            if (transaction == null) throw new KeyNotFoundException("El objetivo no existe.");

            await _transactionRepository.DeleteTransactionAssync(id);
        }


        public async Task<List<Transaction>> GetByAccountAsync(int accountId)
        {
            var account = await _transactionRepository.GetTransactionAccountAssync(accountId);
            if (account == null) throw new KeyNotFoundException("El account no existe.");
            return account;
        }

        public async Task<Transaction?> GetByIdAsync(int transactions_id)
        {
            var transaccion = await _transactionRepository.GetTransactionAssync(transactions_id);
            if (transaccion == null) throw new KeyNotFoundException("El transaction no existe.");
            return transaccion;
        }
        private void ValidateUpdateData(TransactionUpdateDTO dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("El monto debe ser mayor a 0.");
            if (string.IsNullOrWhiteSpace(dto.Category)) throw new ArgumentException("La categoría es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Concept)) throw new ArgumentException("El concepto es obligatorio.");

            if (dto.IsRecurring)
            {
                if (!Enum.IsDefined(typeof(TransactionUpdateDTO.EFrequency), dto.Frequency))
                    throw new ArgumentException("La frecuencia no es válida.");

                if (dto.End_date == default || dto.End_date <= dto.Transaction_date)
                    throw new ArgumentException("La fecha fin debe ser posterior a la fecha de transacción.");
            }
        }

        public async Task UpdateAsync(int transactions_id, TransactionUpdateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var transaction = await _transactionRepository.GetTransactionAssync(transactions_id);
            if (transaction == null) throw new KeyNotFoundException("La transacción no existe.");

            ValidateUpdateData(dto);
            transaction.Category = dto.Category;
            transaction.Amount = dto.Amount;
            transaction.Concept = dto.Concept;
            transaction.Transaction_date = dto.Transaction_date;
            transaction.IsRecurring = dto.IsRecurring;
            transaction.Frequency = (Transaction.EFrequency)dto.Frequency;
            transaction.End_date = dto.End_date;

            await _transactionRepository.UpdateTransactionAssync(transaction);
        }
    }
}