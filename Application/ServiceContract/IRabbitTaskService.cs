using Model.ApplicationModels;
using Model.PublicModels;
using System.Linq.Expressions;

namespace Application.ServiceContract
{
    public interface IRabbitTaskService
    {
        Task<OperationResult> CreateAsync(RabbitTasks taskModels);
        Task<OperationResult> UpdateAsync(RabbitTasks taskModels);
        Task<OperationResult> DeleteAsync(RabbitTasks taskModels);
        Task<OperationResult> DeleteAsync(int Id);
        Task<OperationResult<RabbitTasks>> ReadAsync(int Id, params Expression<Func<RabbitTasks, object>>[] includes);
        Task<OperationResult<IEnumerable<RabbitTasks>>> ReadAllAsync();
        OperationResult<IEnumerable<RabbitTasks>> ReadAll(params Expression<Func<RabbitTasks,object>>[] includes);
        Task<OperationResult<IEnumerable<RabbitTasks>>> ReadAllAsync(params Expression<Func<RabbitTasks, object>>[] includes);

    }
}
