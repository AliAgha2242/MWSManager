
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;
using RabbitSender.Models.LogModels;

namespace MWSMan.Core.Infrastructure.LoggManager.LoggManager
{
    public interface IMinooLogReader
    {
        //Master
        Task<IEnumerable<LogModel>> GetAllAsync();
        Task<IEnumerable<LogModel>> GetAllAsync(DateTime dateTime);
        Task<IEnumerable<LogModel>> GetAllWithConditionAsync(Func<LogModel, bool> func);
        Task<LogModel> GetAsync(Guid id);

        //Detail
        Task<IEnumerable<ProgramLogModel>> GetAllDetailAsync();
        Task<IEnumerable<ProgramLogModel>> GetAllDetailAsync(DateTime dateTime);
        Task<IEnumerable<ProgramLogModel>> GetAllDetailWithConditionAsync(Func<ProgramLogModel, bool> func);
        Task<ProgramLogModel> GetDetailAsync(Guid id);
    }
    public class MinooLogReader : IMinooLogReader
    {
        public SqlConnectionManager SqlConnectionManager { get; }

        public MinooLogReader(SqlConnectionManager sqlConnectionManager)
        {
            SqlConnectionManager = sqlConnectionManager;
        }


        public Task<IEnumerable<LogModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LogModel>> GetAllAsync(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProgramLogModel>> GetAllDetailAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProgramLogModel>> GetAllDetailAsync(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProgramLogModel>> GetAllDetailWithConditionAsync(Func<ProgramLogModel, Boolean> func)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LogModel>> GetAllWithConditionAsync(Func<LogModel, Boolean> func)
        {
            throw new NotImplementedException();
        }

        public Task<LogModel> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ProgramLogModel> GetDetailAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
