using Base.Models;
using System.Linq.Expressions;

namespace API.Models.Dto
{
    public class DtoExpressions
    {
        public static Expression<Func<IntakerTask, GetAllTasks>> _GetAllTasks = i => new GetAllTasks
        {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            Status = new BaseHelper
            {
                Id = i.StatusId,
                Name = i.Status.Name
            },
            AssignedTo = i.AssignedTo
        };

        public static Expression<Func<Status, BaseHelper>> _GetAllStatuses = i => new BaseHelper
        {
            Id = i.Id,
            Name = i.Name
        };
    }
}
