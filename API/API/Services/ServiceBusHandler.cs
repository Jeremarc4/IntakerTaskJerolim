using Azure.Messaging.ServiceBus;
using Polly;
using System.Text.Json;

namespace API.Services
{
    public class ServiceBusHandler : IAsyncDisposable, IServiceBusHandler
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly ILogger<ServiceBusHandler> _logger;
        private ServiceBusProcessor? _processor;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly IConfiguration _configuration;

        public ServiceBusHandler(ILogger<ServiceBusHandler> logger, ServiceBusClient client, IAsyncPolicy retryPolicy, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            _sender = _client.CreateSender(_configuration["Queue:Sender"]);
            _logger = logger;
            _retryPolicy = retryPolicy;
        }

        public async Task SendMessageAsync<T>(T message, string correlationId) where T : class
        {
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    string messageBody = JsonSerializer.Serialize(message);

                    var serviceBusMessage = new ServiceBusMessage(messageBody);
                    serviceBusMessage.CorrelationId = correlationId;

                    await _sender.SendMessageAsync(serviceBusMessage);

                    _logger.LogInformation($"The message with correlationId: {correlationId} has been sent successfully");
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send message: {ex.Message}");
            }
        }

        public async Task ReceiveMessagesAsync()
        {
            if (_processor == null)
            {
                _processor = _client.CreateProcessor(_configuration["Queue:Receiver"]);

                _processor.ProcessMessageAsync += MessageHandler;
                _processor.ProcessErrorAsync += ErrorHandler;
            }

            await _processor.StartProcessingAsync();
        }

        public async Task StopProcessingAsync()
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync();
                await DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_processor != null)
                await _processor.DisposeAsync();

            if (_sender != null)
                await _sender.DisposeAsync();

            if (_client != null)
                await _client.DisposeAsync();
        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            _logger.LogInformation($"{body}");

            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError($"An error has occured. Error messaage: {args.Exception.Message}");

            return Task.CompletedTask;
        }
    }
}
