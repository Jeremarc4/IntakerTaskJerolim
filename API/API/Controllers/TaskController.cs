using API.Models;
using API.Models.Dto;
using API.Services;
using Base.Data.Services;
using Base.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly IServiceBusHandler _serviceBusHandler;
        private readonly IntakerTaskService _taskService;
        public TaskController(IServiceBusHandler serviceBusHandler, IntakerTaskService taskService)
        {
            _serviceBusHandler = serviceBusHandler;
            _taskService = taskService;
        }

        [HttpPost("add-task")]
        public async Task<IActionResult> AddTask(AddTask request)
        {
            var correlationId = Guid.NewGuid().ToString();

            var newTask = new IntakerTask
            {
                Name = request.Name,
                Description = request.Description,
                StatusId = request.StatusId,
                AssignedTo = request.AssignedTo,
            };

            await _serviceBusHandler.SendMessageAsync(newTask, correlationId);

            return Ok();
        }

        [HttpPost("update-task")]
        public async Task<IActionResult> UpdateTask(UpdateTask request)
        {
            var correlationId = Guid.NewGuid().ToString();

            var taskToUpdate = await _taskService.GetSingleTaskAsync(request.Id);

            if (taskToUpdate == null)
                return NotFound("No task in database with that Id");

            taskToUpdate.StatusId = request.StatusId;

            await _serviceBusHandler.SendMessageAsync(taskToUpdate, correlationId);

            return Ok();
        }

        [HttpGet("list-tasks")]
        public async Task<IActionResult> ListTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync(DtoExpressions._GetAllTasks);

            return Ok(tasks);
        }

        [HttpGet("list-statuses")]
        public async Task<IActionResult> ListStatuses()
        {
            var statuses = await _taskService.GetAllStatusesAsync(DtoExpressions._GetAllStatuses);

            return Ok(statuses);
        }
    }
}
