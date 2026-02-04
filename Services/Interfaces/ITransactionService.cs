using Models;

namespace wandaAPI.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetByAccountAsync(int accountId);
        Task<Transaction?> GetByIdAsync(int id);
        /// <summary>
        /// =========================================================================================================
        /// 1. CREACIÓN DE TRANSACCIONES 
        /// Lógica: 
        ///    a) Determina quién paga realmente (Cuenta Personal vs Conjunta).
        ///    b) Mueve el dinero físico (Actualiza saldo).
        ///    c) Genera la transacción oficial (Historial).
        ///    d) Crea "Transacción Espejo" en la cuenta personal para tracking individual.
        ///    e) Genera deudas (Splits) si es un gasto compartido.
        /// =========================================================================================================
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Transaction> CreateAsync(int accountId, TransactionCreateDTO dto);
        /// <summary>
        /// ejempl
        /// </summary>
        /// <param name="id">Este es el id</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task UpdateAsync(int id, TransactionUpdateDTO dto);
        Task DeleteAsync(int id);

        Task ProcessRecurringTransactionsAsync();
    }
}
