using Base.Models;
using System.Linq.Expressions;

namespace Base.Data.Services
{
    public class IntakerTaskService
    {
        private readonly IRepository<IntakerTask> _taskRepository;
        private readonly IRepository<Status> _statusRepository;

        public IntakerTaskService(IRepository<IntakerTask> taskRepository, IRepository<Status> statusRepository)
        {
            _taskRepository = taskRepository;
            _statusRepository = statusRepository;
        }

        public async Task AddNewTaskAsync(IntakerTask newTask)
        {
            await _taskRepository.AddAsync(newTask);
        }

        public async Task<IEnumerable<TResult>> GetAllTasksAsync<TResult>(Expression<Func<IntakerTask, TResult>> expression)
        {
            return await _taskRepository.GetAllAsync(expression);
        }

        public async Task<IntakerTask?> GetSingleTaskAsync(int Id)
        {
            return await _taskRepository.GetSingleAsync(i => i.Id == Id);
        }

        public async Task UpdateTaskAsync(IntakerTask taskoToUpdate)
        {
            await _taskRepository.UpdateAsync(taskoToUpdate);
        }

        public async Task SaveChangesToDatabaseAsync()
        {
            await _taskRepository.SaveAsync();
        }

        public async Task<IEnumerable<TResult>> GetAllStatusesAsync<TResult>(Expression<Func<Status, TResult>> expression)
        {
            return await _statusRepository.GetAllAsync(expression);
        }
    }
}
