using Models;
using wandaAPI.Repositories;
using System.Transactions;

namespace wandaAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IObjectiveRepository _objectiveRepository;

        private readonly ITransactionSplitRepository _splitRepository;

        private readonly IAccountUsersRepository _accountUsersRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IObjectiveRepository objectiveRepository, ITransactionSplitRepository splitRepository, IAccountUsersRepository accountUsersRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _objectiveRepository = objectiveRepository;
            _splitRepository = splitRepository;
            _accountUsersRepository = accountUsersRepository;

        }


        // Metodo para crear una transaccion. Tipos de transacciones:
        // Personal - ingreso
        // Personal - gasto
        // Conjunta - gasto contribucion
        // Conjunta - gasto dividido
        // Aportaciion Objetivo - gasto saving

        
        public async Task<Models.Transaction> CreateAsync(int accountId, TransactionCreateDTO dto)
        {

            if (dto == null) throw new ArgumentNullException(nameof(dto));
            

            var targetAccount = await _accountRepository.GetByIdAsync(accountId);
            if (targetAccount == null) throw new KeyNotFoundException("La cuenta destino no existe.");


            ValidateTransactionData(dto, targetAccount);

            var fundingAccount = await ResolveFundingAccountAsync(targetAccount, dto.User_id);


            ValidateSufficientFunds(fundingAccount, dto.Amount, dto.Transaction_type.ToLower());


            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                
                if (dto.Transaction_type.ToLower() == "income")
                    fundingAccount.Amount += dto.Amount;
                else
                    fundingAccount.Amount -= dto.Amount;

                await _accountRepository.UpdateAsync(fundingAccount);


                var transaction = new Models.Transaction
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

                int transId = await _transactionRepository.AddTransactionAsync(transaction);
                transaction.Transaction_id = transId;


                await ProcessJointSplitsAsync(transaction, targetAccount, transId);

                await ProcessObjectiveContributionAsync(transaction);


                scope.Complete();

                return transaction;
            }
        }


        //Determina quien paga la transaccion en caso de  que la cuenta sea conjunta o personal.
        private async Task<Account> ResolveFundingAccountAsync(Account targetAccount, int userId)
        {

            if (targetAccount.Account_type == "joint")
            {
                var personalAccount = await _accountRepository.GetPersonalAccountByUserIdAsync(userId);
                if (personalAccount == null)
                    throw new InvalidOperationException("El usuario no tiene una cuenta personal para afrontar pagos conjuntos.");

                return personalAccount;
            }


            return targetAccount;
        }

        //Validamos que haya dinero suficiente en la cuenta en caso de ser un gasto o ahorro
        private void ValidateSufficientFunds(Account account, double amount, string type)
        {

            if (type == "expense" || type == "saving")
            {
                if (account.Amount < amount)
                {
                    throw new InvalidOperationException($"Saldo insuficiente en la cuenta '{account.Name}'. Tienes {account.Amount}€ e intentas mover {amount}€.");
                }
            }
        }

        //Gestion de splits para la cuenta conjunta
        private async Task ProcessJointSplitsAsync(Models.Transaction tx, Account targetAccount, int transId)
        {

            if (targetAccount.Account_type == "joint" &&
                tx.Transaction_type == "expense" &&
                tx.Split_type == "divided")
            {

                var members = await _accountUsersRepository.GetUsersByAccountIdAsync(targetAccount.Account_id);

                if (members != null && members.Count > 0)
                {
                    double amountPerPerson = tx.Amount / members.Count;

                    foreach (var member in members)
                    {
                        if (member.User_id != tx.User_id)
                        {
                            var split = new TransactionSplit
                            {
                                Transaction_id = transId,
                                User_id = member.User_id,
                                Amount_assigned = amountPerPerson,
                                Status = "pending"
                            };
                            await _splitRepository.AddAsync(split);
                        }
                    }
                }
            }
        }


        //Gestion de Objetivos

        private async Task ProcessObjectiveContributionAsync(Models.Transaction tx)
        {

            if (tx.Transaction_type == "saving" && tx.Objective_id > 0)
            {
                var objective = await _objectiveRepository.GetByIdAsync(tx.Objective_id);
                if (objective != null)
                {
                    objective.Current_save += tx.Amount;
                    await _objectiveRepository.UpdateAsync(objective);
                }
            }
        }


        private void ValidateTransactionData(TransactionCreateDTO dto)
        {


        }


        private void ValidateTransactionData(TransactionCreateDTO dto, Account targetAccount)
        {

            // Validaciones ENUMS

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


            //Validaciones básicas

            if (dto.Amount <= 0)
                throw new ArgumentException("El monto de la transacción debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(dto.Concept))
                throw new ArgumentException("El concepto es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("La categoría es obligatoria.");

            if (dto.Transaction_date == default)
                throw new ArgumentException("La fecha de la transacción no es válida.");


            // Validaciones logica de negocio

            if (dto.Transaction_type.ToLower() == "saving" && dto.Objective_id <= 0)
            {
                throw new ArgumentException("Las transacciones de tipo 'saving' deben tener un Objective_id válido asociado.");
            }

            if (dto.IsRecurring && dto.End_date.HasValue)
            {
                if (dto.End_date.Value <= dto.Transaction_date)
                    throw new ArgumentException("La fecha de fin de la recurrencia debe ser posterior a la fecha de la transacción.");
            }

            if (targetAccount.Account_type == "personal" && dto.Split_type.ToLower() != "individual")
            {
                throw new ArgumentException("En una cuenta personal, el tipo de división (split) solo puede ser 'individual'.");
            }

        }

        public async Task DeleteAsync(int id)
        {

            var transaction = await _transactionRepository.GetTransactionAsync(id);
            if (transaction == null) throw new KeyNotFoundException("La transacción no existe.");


            await _transactionRepository.DeleteTransactionAsync(id);
        }

        public async Task<List<Models.Transaction>> GetByAccountAsync(int accountId)
        {

            var transactions = await _transactionRepository.GetTransactionsByAccountAsync(accountId);
            return transactions;
        }

        public async Task<Models.Transaction?> GetByIdAsync(int transactions_id)
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