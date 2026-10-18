using Model.ApplicationModels;
using Model.PublicModels;

namespace Application.ServiceContract
{
    public interface IRequestHubService
    {
        Task<OperationResult> CreateAsync(RabbitConsumerRequestHub requestHub);
        Task<OperationResult> UpdateAsync(RabbitConsumerRequestHub requestHub);
        Task<OperationResult> DeleteAsync(RabbitConsumerRequestHub requestHub);
        Task<OperationResult> DeleteAsync(int Id);
        Task<OperationResult<RabbitConsumerRequestHub>> ReadAsync(int Id);
        Task<OperationResult<IEnumerable<RabbitConsumerRequestHub>>> ReadAllAsync();
        Task<OperationResult<IEnumerable<RabbitConsumerRequestHub>>> ReadAllAsync(Func<RabbitConsumerRequestHub,bool> func);

        OperationResult<IEnumerable<RabbitConsumerRequestHub>> ReadAll();

    }
}
