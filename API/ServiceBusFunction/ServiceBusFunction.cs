using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Base.Data.Services;
using Base.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServiceBusFunction
{
    public class ServiceBusFunction
    {
       private readonly ILogger<ServiceBusFunction> _logger;
        private readonly IntakerTaskService _taskService;

        public ServiceBusFunction(ILogger<ServiceBusFunction> logger, IntakerTaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        [Function("HandleTaskManagement")]
        [ServiceBusOutput("TaskManagementResponse", Connection = "ServiceBusConnection")]
        public async Task<string> Run(
              [ServiceBusTrigger("TaskManagement", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
              ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            try
            {
                var messageBody = message.Body.ToString();
                var workingTask = JsonSerializer.Deserialize<IntakerTask>(messageBody);

                var correlation = message.CorrelationId;

                if (workingTask != null)
                {
                    _logger.LogInformation($"Message : CorrelationId = {correlation}, Name = {workingTask.Name}");

                    if (workingTask.Id == 0)
                    {
                        await _taskService.AddNewTaskAsync(workingTask);
                        await _taskService.SaveChangesToDatabaseAsync();

                        return $"Added Task to database of Correlation: {correlation}";
                    }
                    else
                    {
                        await _taskService.UpdateTaskAsync(workingTask);
                        await _taskService.SaveChangesToDatabaseAsync();

                        return $"Updated Task of Correlation: {correlation}";
                    }
                }
                else
                {
                    return $"Failed to add or update task of Correlation:{correlation}";
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Error message : {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error message: {ex.Message}");
            }

            return $"Failed to add Task to database of Correlation: {message.CorrelationId}";
        }
    }
}
