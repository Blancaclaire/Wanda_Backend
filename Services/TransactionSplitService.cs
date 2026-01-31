using Models;
using wandaAPI.Repositories;
using System.Transactions;

namespace wandaAPI.Services
{
    public class TransactionSplitService : ITransactionSplitService
    {
        private readonly ITransactionSplitRepository _splitRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;

        public TransactionSplitService(
            ITransactionSplitRepository splitRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IUserRepository userRepository)
        {
            _splitRepository = splitRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public async Task<List<TransactionSplit>> GetUserSplitsAsync(int userId, string? statusFilter)
        {
            
            var allSplits = await _splitRepository.GetAllByUserIdAsync(userId);

            
            if (!string.IsNullOrEmpty(statusFilter))
            {

                return allSplits
                    .Where(s => s.Status?.ToLower() == statusFilter.ToLower())
                    .ToList();
            }

            return allSplits;
        }


        public async Task AcceptDebtAsync(int splitId)
        {

            var (split, originalTransaction, cuentaDeudor, cuentaAcreedor, userDeudor, userAcreedor) = await ValidateDebtAcceptanceAsync(splitId);


            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                cuentaDeudor.Amount -= split.Amount_assigned;
                cuentaAcreedor.Amount += split.Amount_assigned;

                await _accountRepository.UpdateAsync(cuentaDeudor);
                await _accountRepository.UpdateAsync(cuentaAcreedor);


                var gastoDeuda = new Models.Transaction
                {
                    Account_id = cuentaDeudor.Account_id,
                    User_id = split.User_id,
                    Category = "Deuda",
                    Amount = split.Amount_assigned,
                    Transaction_type = "expense",
                    Concept = $"Pago deuda a {userAcreedor?.Name} ({originalTransaction.Concept})",
                    Transaction_date = DateTime.Now,
                    IsRecurring = false,
                    Split_type = "individual"
                };
                await _transactionRepository.AddTransactionAsync(gastoDeuda);


                var ingresoReembolso = new Models.Transaction
                {
                    Account_id = cuentaAcreedor.Account_id,
                    User_id = originalTransaction.User_id,
                    Category = "Reembolso",
                    Amount = split.Amount_assigned,
                    Transaction_type = "income",
                    Concept = $"Reembolso de {userDeudor?.Name} ({originalTransaction.Concept})",
                    Transaction_date = DateTime.Now,
                    IsRecurring = false,
                    Split_type = "individual"
                };
                await _transactionRepository.AddTransactionAsync(ingresoReembolso);


                await _splitRepository.UpdateStatusAsync(splitId, "settled", DateTime.Now);

                scope.Complete();
            }
        }


        private async Task<(TransactionSplit, Models.Transaction, Account, Account, User?, User?)>
            ValidateDebtAcceptanceAsync(int splitId)
        {

            var split = await _splitRepository.GetByIdAsync(splitId);
            if (split == null) throw new KeyNotFoundException("La deuda no existe.");
            if (split.Status == "settled") throw new InvalidOperationException("Esta deuda ya fue aceptada y pagada.");


            var originalTransaction = await _transactionRepository.GetTransactionAsync(split.Transaction_id);
            if (originalTransaction == null) throw new Exception("Error de integridad: La transacción original no existe.");


            int deudorId = split.User_id;
            int acreedorId = originalTransaction.User_id;

            var cuentaDeudor = await _accountRepository.GetPersonalAccountByUserIdAsync(deudorId);
            var cuentaAcreedor = await _accountRepository.GetPersonalAccountByUserIdAsync(acreedorId);

            if (cuentaDeudor == null || cuentaAcreedor == null)
                throw new Exception("Ambos usuarios necesitan tener una cuenta personal activa.");


            if (cuentaDeudor.Amount < split.Amount_assigned)
                throw new InvalidOperationException("No tienes saldo suficiente en tu cuenta personal para aceptar esta deuda.");


            var userDeudor = await _userRepository.GetByIdAsync(deudorId);
            var userAcreedor = await _userRepository.GetByIdAsync(acreedorId);

            return (split, originalTransaction, cuentaDeudor, cuentaAcreedor, userDeudor, userAcreedor);
        }
    }
}