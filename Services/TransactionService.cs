using Models;
using DTOs;
using wandaAPI.Repositories;

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

            var validTypes = new[] { "income", "expense", "saving" };
            if (!validTypes.Contains(dto.Transaction_type.ToLower()))
                throw new ArgumentException("El tipo de transacción debe ser: income, expense o saving.");


            var validSplits = new[] { "individual", "contribution", "divided" };
            if (!validSplits.Contains(dto.Split_type.ToLower()))
                throw new ArgumentException("El tipo de división debe ser: individual, contribution o divided.");

            if (dto.IsRecurring)
            {
                var validFrequencies = new[] { "monthly", "weekly", "annual" };
                if (string.IsNullOrEmpty(dto.Frequency) || !validFrequencies.Contains(dto.Frequency.ToLower()))
                    throw new ArgumentException("La frecuencia debe ser: monthly, weekly o annual.");
            }
        }



        public async Task<Transaction> CreateAsync(int accountId, TransactionCreateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            ValidateTransactionData(dto);

            var personalAccount = await _accountRepository.GetByIdAsync(accountId);
            if (personalAccount == null) throw new KeyNotFoundException("No se encontró la cuenta.");



            var transaction = new Transaction
            {
                Account_id = accountId,
                User_id = dto.User_id,
                Objective_id = dto.Objective_id,
                Category = dto.Category,
                Amount = dto.Amount,
                Transaction_type = dto.Transaction_type.ToLower(),
                Concept = dto.Concept,
                Transaction_date = dto.Transaction_date,
                IsRecurring = dto.IsRecurring,
                Frequency = dto.Frequency?.ToLower(),
                End_date = dto.End_date,
                Split_type = dto.Split_type.ToLower()
            };


            int id = await _transactionRepository.AddTransactionAsync(transaction);
            transaction.Transaction_id = id;

            return transaction;
        }

        public async Task DeleteAsync(int id)
        {

            var transaction = await _transactionRepository.GetTransactionAsync(id);
            if (transaction == null) throw new KeyNotFoundException("La transacción no existe.");


            await _transactionRepository.DeleteTransactionAsync(id);
        }

        public async Task<List<Transaction>> GetByAccountAsync(int accountId)
        {

            var transactions = await _transactionRepository.GetTransactionsByAccountAsync(accountId);
            return transactions;
        }

        public async Task<Transaction?> GetByIdAsync(int transactions_id)
        {

            var transaccion = await _transactionRepository.GetTransactionAsync(transactions_id);
            if (transaccion == null) throw new KeyNotFoundException("La transacción no existe.");
            return transaccion;
        }

        public async Task UpdateAsync(int transactions_id, TransactionUpdateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var transaction = await _transactionRepository.GetTransactionAsync(transactions_id);
            if (transaction == null) throw new KeyNotFoundException("La transacción no existe.");

            ValidateUpdateData(dto);

            transaction.Category = dto.Category;
            transaction.Amount = dto.Amount;
            transaction.Concept = dto.Concept;
            transaction.Transaction_date = dto.Transaction_date;
            transaction.IsRecurring = dto.IsRecurring;

            
            transaction.Frequency = dto.IsRecurring ? dto.Frequency?.ToLower() : null;

            transaction.End_date = dto.End_date;

            await _transactionRepository.UpdateTransactionAsync(transaction);
        }

        private void ValidateUpdateData(TransactionUpdateDTO dto)
        {
            if (dto.Amount <= 0) throw new ArgumentException("El monto debe ser mayor a 0.");
            if (string.IsNullOrWhiteSpace(dto.Category)) throw new ArgumentException("La categoría es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Concept)) throw new ArgumentException("El concepto es obligatorio.");

            if (dto.IsRecurring)
            {
                
                var validFrequencies = new[] { "monthly", "weekly", "annual" };

                if (string.IsNullOrWhiteSpace(dto.Frequency) || !validFrequencies.Contains(dto.Frequency.ToLower()))
                {
                    throw new ArgumentException("La frecuencia no es válida. Use: monthly, weekly o annual.");
                }

                if (dto.End_date == default || dto.End_date <= dto.Transaction_date)
                    throw new ArgumentException("La fecha fin debe ser posterior a la fecha de transacción.");
            }
        }
    }
}