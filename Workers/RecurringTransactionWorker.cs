using wandaAPI.Services;

namespace wandaAPI.Workers
{
    public class RecurringTransactionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecurringTransactionWorker> _logger;
        
        // Ejecución cada 24 horas. Para pruebas ponlo en TimeSpan.FromMinutes(1)
        private readonly TimeSpan _period = TimeSpan.FromSeconds(24); 

        public RecurringTransactionWorker(IServiceProvider serviceProvider, ILogger<RecurringTransactionWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Iniciando proceso de transacciones recurrentes: {DateTime.Now}");

                    // IMPORTANTE: Los servicios (TransactionService) son "Scoped" (por petición),
                    // pero este Worker es "Singleton" (vive siempre). 
                    // Debemos crear un Scope manual para obtener el servicio.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
                        
                        await transactionService.ProcessRecurringTransactionsAsync();
                    }

                    _logger.LogInformation("Proceso de recurrencia finalizado.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error crítico en el worker de recurrencia.");
                }
            }
        }
    }
}