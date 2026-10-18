using Model.PublicModels;
using MWSMan.Core.Model.PublicModels.LogModels;
using System.Linq.Expressions;

namespace Application.ServiceContract
{
    public interface ILogService
    {
        public Task<OperationResult<RabbitLogModel?>> GetLogAsync(Guid id);
        public Task<OperationResult<RabbitLogModel?>> GetLogAsync(string id);
        public OperationResult<RabbitLogModel?> GetLog(Guid id);
        public OperationResult<RabbitLogModel?> GetLog(string id);
        public Task<Memory<OperationResult<RabbitLogModel?>>> GetAllLogAsync(Expression<Func<RabbitLogModel,bool>> expression);


        public Task<OperationResult<ProgramLogModel?>> GetProgramLogAsync(string id);
        public Task<OperationResult<ProgramLogModel?>> GetProgramLogAsync(Guid id);

        public OperationResult<ProgramLogModel?> GetProgramLog(string id);
        public OperationResult<ProgramLogModel?> GetProgramLog(Guid id);
        public Task<Memory<OperationResult<ProgramLogModel?>>> GetAllProgramLogAsync(Expression<Func<ProgramLogModel, bool>> expression);
        public Task<OperationResult<RabbitLogModel?>> GetLogInAllServer(string id);

    }

}
