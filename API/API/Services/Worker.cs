namespace API.Services
{
     public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IServiceBusHandler _serviceBusHandler;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IServiceBusHandler serviceBusHandler)
        {
            _logger = logger;
            _serviceBusHandler = serviceBusHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _serviceBusHandler.ReceiveMessagesAsync();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_serviceBusHandler != null)
            {
                await _serviceBusHandler.StopProcessingAsync();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
