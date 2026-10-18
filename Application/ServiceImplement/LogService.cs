using Application.ServiceContract;
using Dapper;
using Infrastructure.DataBaseRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Model.PresentationModels;
using Model.PublicModels;
using MWSMan.Core.Model.InfrastructModels;
using MWSMan.Core.Model.PublicModels.LogModels;
using MWSMan.Core.Persistence.StaticConfiguration;
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Application.ServiceImplement
{
    public class LogService : ILogService
    {
        public IGenericRepository GenericRepository { get; }
        private IList<ServerSelected> Servers { get; set; }
        private SqlConnectionManager ownConnection;
        private IMinooConfiguration configuration;

        public LogService(IGenericRepository genericRepository, IList<ServerSelected> servers, SqlConnectionManager ownConnection, IMinooConfiguration configuration)
        {
            GenericRepository = genericRepository;
            Servers = servers;
            this.ownConnection = ownConnection;
            this.configuration = configuration;
        }

        public async Task<OperationResult<RabbitLogModel?>> GetLogAsync(Guid id)
        {
            RabbitLogModel? res = await GenericRepository.GetAsync<RabbitLogModel>(p => p.id == id);
            throw new NotImplementedException();

        }

        public Task<OperationResult<RabbitLogModel?>> GetLogAsync(string id)
        {
            throw new NotImplementedException();
        }

        public OperationResult<RabbitLogModel?> GetLog(Guid id)
        {
            throw new NotImplementedException();
        }

        public OperationResult<RabbitLogModel?> GetLog(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Memory<OperationResult<RabbitLogModel?>>> GetAllLogAsync(Expression<Func<RabbitLogModel, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<ProgramLogModel?>> GetProgramLogAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<ProgramLogModel?>> GetProgramLogAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public OperationResult<ProgramLogModel?> GetProgramLog(string id)
        {
            throw new NotImplementedException();
        }

        public OperationResult<ProgramLogModel?> GetProgramLog(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Memory<OperationResult<ProgramLogModel?>>> GetAllProgramLogAsync(Expression<Func<ProgramLogModel, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<RabbitLogModel?>> GetLogInAllServer(string id)
        {

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            string query = $"Select * from RabbitConsumerLog where id  like '%{id}%'";
            RabbitLogModel logModel = new RabbitLogModel();

            Parallel.ForEach<ServerSelected>(Servers.Where(p=>!p.serverInfo.ServerName.ToLower().Contains("rabbit")).AsEnumerable(), (p, d) =>
            {
                CSConfig config = new CSConfig(new MWSMan.Core.Model.InfrastructModels.OwnServerInfo()
                {

                    MultipleActiveResultSets = true,
                    Password = "11",
                    UserId = "Tavakoli",
                    sqlLoginType = SqlLoginType.SqlAuthentication,
                    TimeOutMiliSecond = 3,
                    TrustServerCertificate = true,
                    ServerName = p.serverInfo.ServerIp,
                    DataBaseName = p.serverInfo.DatabaseName,
                });
                try
                {

                    using (SqlConnection cn = new SqlConnection(config.Connectionstring))
                    {

                        var re = cn.Query<RabbitLogModel>(query, commandTimeout: 2000).FirstOrDefault();
                        if (re is not null)
                        {
                            logModel = re;
                            tokenSource.Cancel();
                            d.Break();
                        }

                    }
                }
                catch (Exception ex) { }
            });

            return new OperationResult<RabbitLogModel?>(logModel);





            //foreach (ServerSelected server in Servers)
            //{
            //    try
            //    {
            //        CSConfig config = new CSConfig(new MWSMan.Core.Model.InfrastructModels.OwnServerInfo()
            //        {

            //            MultipleActiveResultSets = true,
            //            Password = "11",
            //            UserId = "Tavakoli",
            //            sqlLoginType = SqlLoginType.SqlAuthentication,
            //            TimeOutMiliSecond = 3,
            //            TrustServerCertificate = true,
            //            ServerName = server.serverInfo.ServerName,
            //            DataBaseName = server.serverInfo.DatabaseName,
            //        });
            //        using (SqlConnection cn = new SqlConnection(config.Connectionstring))
            //        {

            //            var re = (await cn.QueryAsync<RabbitLogModel>(query, commandTimeout: 2000)).FirstOrDefault();
            //            if (re is not null)
            //            {
            //                logModel = re;
            //                break;
            //            }

            //        }
            //    }
            //    catch (Exception ex) { }

            //}
            //return new OperationResult<RabbitLogModel?>(logModel);
            ////catch (Exception ex)
            ////{
            ////    return new OperationResult<RabbitLogModel?>(false, ex.Message);
            ////}



        }
    }
}
