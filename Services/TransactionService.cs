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

        // Tipos de transacciones:
        // Personal - ingreso
        // Personal - gasto
        // Conjunta - gasto contribucion
        // Conjunta - gasto dividido
        // Aportaciion Objetivo - gasto saving



        // =========================================================================================================
        // 1. CREACIÓN DE TRANSACCIONES 
        // Lógica: 
        //    a) Determina quién paga realmente (Cuenta Personal vs Conjunta).
        //    b) Mueve el dinero físico (Actualiza saldo).
        //    c) Genera la transacción oficial (Historial).
        //    d) Crea "Transacción Espejo" en la cuenta personal para tracking individual.
        //    e) Genera deudas (Splits) si es un gasto compartido.
        // =========================================================================================================
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
                // 1. Ejecución del Movimiento Bancario
                if (dto.Transaction_type.ToLower() == "income")
                    fundingAccount.Amount += dto.Amount;
                else
                    fundingAccount.Amount -= dto.Amount;

                await _accountRepository.UpdateAsync(fundingAccount);

                // 2. Registro de la Transacción Principal (La que ven todos en el grupo)
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

                // 3. Generación de "Transacción Espejo" (Visibilidad Personal)
                // Si pagaste algo para el grupo, se crea una copia en tu cuenta personal para que veas el gasto.

                if (targetAccount.Account_id != fundingAccount.Account_id)
                {
                    string espejoConcepto = dto.Split_type.ToLower() == "divided"
                        ? $"(Gasto Compartido) {dto.Concept}" : $"(Aportación Conjunta) {dto.Concept}";

                    var mirrorTransaction = new Models.Transaction
                    {
                        Account_id = fundingAccount.Account_id,
                        User_id = dto.User_id,
                        Category = dto.Category,
                        Amount = dto.Amount,
                        Transaction_type = "expense",
                        Concept = espejoConcepto,
                        Transaction_date = dto.Transaction_date,
                        IsRecurring = dto.IsRecurring,
                        Frequency = dto.Frequency?.ToLower(),
                        End_date = dto.End_date,
                        Split_type = "individual"
                    };

                    await _transactionRepository.AddTransactionAsync(mirrorTransaction);
                }

                // 4. Procesamiento de Deudas y Ahorros
                await ProcessJointSplitsAsync(transaction, targetAccount, transId, dto.CustomSplits);

                await ProcessObjectiveContributionAsync(transaction);


                scope.Complete();

                return transaction;
            }
        }

        // =========================================================================================================
        // 2. EDICIÓN (UPDATE)
        // Lógica: 
        //    a) PROTECCIÓN: Bloquea la edición de gastos compartidos ("divided") para mantener integridad de deudas.
        //    b) Calcula diferencias de saldo y ajusta la cuenta financiadora.
        //    c) Sincroniza la "Transacción Espejo" (busca la antigua y la actualiza).
        // =========================================================================================================

        public async Task UpdateAsync(int id, TransactionUpdateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var originalTx = await _transactionRepository.GetTransactionAsync(id);
            if (originalTx == null) throw new KeyNotFoundException("La transacción no existe.");

            // Mantenemos tu protección para no editar gastos divididos
            if (originalTx.Split_type.ToLower() == "divided")
            {
                throw new InvalidOperationException("No es posible editar un gasto compartido. Por favor, elimínalo y créalo de nuevo con los datos correctos.");
            }

            ValidateUpdateData(dto, originalTx);

            var targetAccount = await _accountRepository.GetByIdAsync(originalTx.Account_id);
            var fundingAccount = await ResolveFundingAccountAsync(targetAccount, originalTx.User_id);

            // Cálculos de saldo
            double amountDifference = dto.Amount - originalTx.Amount;
            bool hasAmountChanged = amountDifference != 0;

            if (hasAmountChanged && amountDifference > 0 &&
               (originalTx.Transaction_type == "expense" || originalTx.Transaction_type == "saving"))
            {
                ValidateSufficientFunds(fundingAccount, amountDifference, originalTx.Transaction_type);
            }

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // 1. Ajustar Saldos y Objetivos
                if (hasAmountChanged)
                {
                    await AdjustBalanceForUpdateAsync(fundingAccount, originalTx.Transaction_type, amountDifference);
                    await AdjustObjectiveForUpdateAsync(originalTx, amountDifference);
                }

                //Actualiza transaccion espejo en cuenta personal
                if (targetAccount.Account_id != fundingAccount.Account_id)
                {

                    string oldEspejoConcepto = originalTx.Split_type.ToLower() == "divided"
                        ? $"(Gasto Compartido) {originalTx.Concept}"
                        : $"(Aportación Conjunta) {originalTx.Concept}";

                    // Buscamos transaccioin que coincida con los datos antiguos
                    var personalTxs = await _transactionRepository.GetTransactionsByAccountAsync(fundingAccount.Account_id);

                    var mirrorTx = personalTxs.FirstOrDefault(t =>
                        t.Amount == originalTx.Amount &&
                        t.Transaction_date == originalTx.Transaction_date &&
                        t.Concept == oldEspejoConcepto
                    );


                    if (mirrorTx != null)
                    {
                        string newEspejoConcepto = originalTx.Split_type.ToLower() == "divided"
                            ? $"(Gasto Compartido) {dto.Concept}"
                            : $"(Aportación Conjunta) {dto.Concept}";

                        mirrorTx.Amount = dto.Amount;
                        mirrorTx.Category = dto.Category;
                        mirrorTx.Concept = newEspejoConcepto;
                        mirrorTx.Transaction_date = dto.Transaction_date;
                        mirrorTx.IsRecurring = dto.IsRecurring;
                        mirrorTx.Frequency = dto.IsRecurring ? dto.Frequency?.ToLower() : null;
                        mirrorTx.End_date = dto.End_date;

                        await _transactionRepository.UpdateTransactionAsync(mirrorTx);
                    }
                }


                originalTx.Amount = dto.Amount;
                originalTx.Category = dto.Category;
                originalTx.Concept = dto.Concept;
                originalTx.Transaction_date = dto.Transaction_date;
                originalTx.Objective_id = dto.Objective_id;
                originalTx.IsRecurring = dto.IsRecurring;
                originalTx.Frequency = dto.IsRecurring ? dto.Frequency?.ToLower() : null;
                originalTx.End_date = dto.End_date;

                await _transactionRepository.UpdateTransactionAsync(originalTx);

                scope.Complete();
            }
        }

        // =========================================================================================================
        // 3. ELIMINACIÓN (DELETE)
        // Lógica: 
        //    a) PROTECCIÓN: Impide borrar si hay deudas asociadas ya pagadas (settled).
        //    b) Reembolso: Devuelve el dinero al saldo de la cuenta financiadora.
        //    c) Limpieza: Elimina la transacción principal y su espejo personal.
        //    d) Cascada: Las deudas pendientes se borran solas por BBDD (ON DELETE CASCADE).
        // =========================================================================================================

        public async Task DeleteAsync(int id)
        {

            var tx = await _transactionRepository.GetTransactionAsync(id);
            if (tx == null) throw new KeyNotFoundException("La transacción no existe.");

            // Verifica las deudas pagadas
            if (tx.Split_type.ToLower() == "divided")
            {

                var splits = await _splitRepository.GetByTransactionIdAsync(id);

                // Si alguno está pagado, prohibimos borrar
                if (splits.Any(s => s.Status.ToLower() == "settled"))
                {
                    throw new InvalidOperationException("No se puede eliminar este gasto porque uno o más usuarios ya han pagado su parte. Deben revertirse los pagos primero.");
                }

            }

            var targetAccount = await _accountRepository.GetByIdAsync(tx.Account_id);
            var fundingAccount = await ResolveFundingAccountAsync(targetAccount, tx.User_id);

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                await RevertBalanceEffectAsync(fundingAccount, tx);


                await RevertObjectiveEffectAsync(tx);


                await _transactionRepository.DeleteTransactionAsync(id);

                //Borra la transaccion Espejo
                if (targetAccount.Account_id != fundingAccount.Account_id)
                {
                    string espejoConcepto = tx.Split_type.ToLower() == "divided"
                        ? $"(Gasto Compartido) {tx.Concept}"
                        : $"(Aportación Conjunta) {tx.Concept}";


                    var personalTxs = await _transactionRepository.GetTransactionsByAccountAsync(fundingAccount.Account_id);

                    var mirrorTx = personalTxs.FirstOrDefault(t =>
                        t.Amount == tx.Amount &&
                        t.Transaction_date == tx.Transaction_date &&
                        t.Concept == espejoConcepto
                    );

                    if (mirrorTx != null)
                    {
                        await _transactionRepository.DeleteTransactionAsync(mirrorTx.Transaction_id);
                    }
                }

                scope.Complete();
            }
        }

        // =========================================================================================================
        // 4. LÓGICA DE NEGOCIO AUXILIAR
        // =========================================================================================================        

        // Redirecciona el cobro: Si la cuenta destino es conjunta, busca y devuelve la cuenta personal del usuario.
        private async Task<Account> ResolveFundingAccountAsync(Account targetAccount, int userId)
        {

            if (targetAccount.Account_type?.Trim().ToLower() == "joint")
            {
                var personalAccount = await _accountRepository.GetPersonalAccountByUserIdAsync(userId);
                if (personalAccount == null)
                    throw new InvalidOperationException("El usuario no tiene una cuenta personal para afrontar pagos conjuntos.");

                return personalAccount;
            }


            return targetAccount;
        }

        // Genera los registros de deuda (Splits) para los miembros del grupo.
        // Soporta modo Manual (CustomSplits) o Automático (Equitativo).
        private async Task ProcessJointSplitsAsync(Models.Transaction tx, Account targetAccount, int transId, List<TransactionSplitDetailDTO>? customSplits)
        {

            if (targetAccount.Account_type?.Trim().ToLower() == "joint" &&
                tx.Transaction_type == "expense" &&
                tx.Split_type == "divided")
            {
                // DIVISIÓN MANUAL (El usuario especificó cantidades)
                if (customSplits != null && customSplits.Count > 0)
                {
                    foreach (var splitDto in customSplits)
                    {
                        // No creamos deuda para el que pagó la cuenta completa (tx.User_id)
                        // Solo registramos lo que le deben los DEMÁS
                        if (splitDto.User_id != tx.User_id)
                        {
                            var split = new TransactionSplit
                            {
                                Transaction_id = transId,
                                User_id = splitDto.User_id,
                                Amount_assigned = splitDto.Amount,
                                Status = "pending"
                            };
                            await _splitRepository.AddAsync(split);
                        }
                    }
                }
                // CASO B: DIVISIÓN AUTOMÁTICA 
                else
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


        // =========================================================================================================
        // 5. VALIDACIONES Y ESTADOS
        // =========================================================================================================


        //Valida que haya dinero suficiente en la cuenta en caso de ser un gasto o ahorro
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

         private void ValidateUpdateData(TransactionUpdateDTO dto, Models.Transaction originalTx)
        {

            if (dto.Amount <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("La categoría es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.Concept))
                throw new ArgumentException("El concepto es obligatorio.");

            if (dto.Transaction_date == default)
                throw new ArgumentException("La fecha de la transacción no es válida.");


            if (dto.IsRecurring)
            {
                var validFrequencies = new[] { "monthly", "weekly", "annual" };
                if (string.IsNullOrEmpty(dto.Frequency) || !validFrequencies.Contains(dto.Frequency.ToLower()))
                    throw new ArgumentException("La frecuencia debe ser: monthly, weekly o annual.");

                if (dto.End_date.HasValue && dto.End_date.Value <= dto.Transaction_date)
                    throw new ArgumentException("La fecha fin debe ser posterior a la fecha de transacción.");
            }

            if (originalTx.Transaction_type.ToLower() == "saving")
            {
                if (dto.Objective_id <= 0)
                {
                    throw new ArgumentException("No puedes dejar una transacción de ahorro sin un objetivo válido.");
                }
            }
        }


        //Deshace el movimiento de dinero 
        private async Task RevertBalanceEffectAsync(Account account, Models.Transaction tx)
        {

            if (tx.Transaction_type.ToLower() == "income")
            {
                account.Amount -= tx.Amount;
            }

            else
            {
                account.Amount += tx.Amount;
            }
            await _accountRepository.UpdateAsync(account);
        }

        //Deshace el movimiento de ahorro
        private async Task RevertObjectiveEffectAsync(Models.Transaction tx)
        {
            if (tx.Transaction_type.ToLower() == "saving" && tx.Objective_id > 0)
            {
                var objective = await _objectiveRepository.GetByIdAsync(tx.Objective_id);
                if (objective != null)
                {
                    objective.Current_save -= tx.Amount;

                    if (objective.Current_save < 0) objective.Current_save = 0;

                    await _objectiveRepository.UpdateAsync(objective);
                }
            }
        }



        //Ajusta el saldo. Comprueba si se trata de una diferencia positiva o negativa
        private async Task AdjustBalanceForUpdateAsync(Account account, string type, double difference)
        {

            if (type == "expense" || type == "saving")
            {
                account.Amount -= difference;
            }
            else if (type == "income")
            {
                account.Amount += difference;
            }

            await _accountRepository.UpdateAsync(account);
        }

        // Ajusta el objetivo
        private async Task AdjustObjectiveForUpdateAsync(Models.Transaction tx, double difference)
        {
            if (tx.Transaction_type == "saving" && tx.Objective_id > 0)
            {
                var objective = await _objectiveRepository.GetByIdAsync(tx.Objective_id);
                if (objective != null)
                {
                    objective.Current_save += difference;
                    await _objectiveRepository.UpdateAsync(objective);
                }
            }
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
    }
}