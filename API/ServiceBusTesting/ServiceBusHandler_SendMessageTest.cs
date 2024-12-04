using API.Services;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Base.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;

namespace ServiceBusTesting
{
    public class ServiceBusHandler_SendMessageTest
    {
        private readonly Mock<ServiceBusClient> _mockClient;
        private readonly Mock<ServiceBusSender> _mockSender;

        private readonly Mock<ILogger<ServiceBusHandler>> _mockLogger;
        private readonly Mock<IAsyncPolicy> _mockRetryPolicy;
        private readonly Mock<IConfiguration> _mockConfiguration;

        private readonly ServiceBusHandler _serviceBusHandler;

        public ServiceBusHandler_SendMessageTest()
        {
            _mockLogger = new Mock<ILogger<ServiceBusHandler>>();

            _mockClient = new Mock<ServiceBusClient>();
            _mockSender = new Mock<ServiceBusSender>();
            _mockClient
                .Setup(client => client.CreateSender(It.IsAny<string>()))
                .Returns(_mockSender.Object);
            _mockSender
               .Setup(sender => sender.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
               .Returns(Task.CompletedTask);

            _mockRetryPolicy = new Mock<IAsyncPolicy>();
            _mockRetryPolicy
                .Setup(policy => policy.ExecuteAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(func => func());

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["Queue:Sender"]).Returns("MockTest");

            _serviceBusHandler = new ServiceBusHandler(
                _mockLogger.Object,
                _mockClient.Object,
                _mockRetryPolicy.Object,
                _mockConfiguration.Object
            );
        }

        [Fact]
        public async Task SendMessageAsync_SendsMessageToServiceBusWithCorrelationId()
        {
            // Arrange
            var message = new IntakerTask
            {
                Name = "TestingName",
                Description = "First description",
                StatusId = 1,
                AssignedTo = "John Edwards",
            };

            var correlationId = "MyTestCorrelation1";

            // Act
            await _serviceBusHandler.SendMessageAsync(message, correlationId);

            // Assert
            _mockSender.Verify(
                sender => sender.SendMessageAsync(It.Is<ServiceBusMessage>(
                    msg => msg.CorrelationId == correlationId
                ), It.IsAny<CancellationToken>()),
                Times.Once
            );

            //Assert after sending message
            _mockLogger.Verify(
               logger => logger.Log(
                   It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information), 
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((baseObject, type) => baseObject.ToString().Contains($"The message with correlationId: {correlationId} has been sent successfully")),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception?, string>>() 
               ),
               Times.Once
           );
        }
    }
}