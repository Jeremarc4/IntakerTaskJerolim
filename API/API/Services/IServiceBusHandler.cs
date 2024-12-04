namespace API.Services
{
    public interface IServiceBusHandler
    {
        Task SendMessageAsync<T>(T message, string correlationId) where T : class;
        Task ReceiveMessagesAsync();
        Task StopProcessingAsync();
    }
}
