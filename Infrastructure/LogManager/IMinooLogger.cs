

using MWSMan.Core.Model.PublicModels.LogModels;
using MWSMan.Core.Persistence.StaticConfiguration;
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;
using RabbitSender.Models.LogModels;
using System.Text;

namespace MWSMan.Core.Infrastructure.LoggManager.LoggManager
{
    public interface IMinooLogger
    {
        public void Logg(LogModel model, bool asynchronous = false);
        public void LoggUpdate(LogModel model, bool asynchronous = false);
        public void ProgramLogg(RabbitSender.Models.LogModels.ProgramLogModel model, bool asynchronous = false);
        public bool LogIsExists(LogModel model);
    }
    public class MinooLogger : IMinooLogger
    {

        private CSConfig config;
        private SqlConnectionManager connectionManager;
        //private ILogQueue _Queue;
        private IMinooConfiguration Configuration;
        public MinooLogger(CSConfig sConfig, IMinooConfiguration configuration)
        {
            config = sConfig;
            connectionManager = new SqlConnectionManager(config);
            //_Queue = queue;
            this.Configuration = configuration;
        }


        public void Logg(LogModel model, bool asynchronous = false)
        {
            if (Encoding.UTF8.GetBytes(model.Body ?? "").Length > (Configuration.Get<int>("TransactionalSend:BodyLimitInByte")))
            {
                model.Body = "Body In Request Is Very Big ";
            }
            Sql sql = new Sql()
            {
                Text = "Sp_Ra_ProducerConsumerLogg_Insert",
                commandType = System.Data.CommandType.StoredProcedure,
                Parameters = new
                {
                    Id = model.id,
                    SenderName = model.SenderName,
                    SenderIp = model.SenderIp,
                    ReciverName = model.ReceiverName,
                    ReciverIp = model.ReceiverIp,
                    QueueName = model.QueueName,
                    RoutingKey = model.RoutingKey,
                    Body = model.Body,
                    ErrMessageId = model.ErrorMessageId,
                    LastUpdateDate = model.LastUpdateDate,
                    CreateDate = model.CreateDate,
                    ProccessDate = model.ProccessDate,
                    LogLevel = model.LogLevel.ToString(),
                    UserHostName = model.UserHostName,
                    UserID = model.UserID,
                    AppName = model.AppName,
                    Attemp = model.Attemp,
                    IsTransactional = model.IsTransacional

                }
            };


            if (!asynchronous)
            {
                try
                {
                    connectionManager.Execute(sql);
                }
                catch (Exception e)
                {

                    throw;
                }
                return;
            }
            //_Queue.Enqueue(model);
        }

        public void LoggUpdate(LogModel model, bool asynchronous = false)
        {
            if (Encoding.UTF8.GetBytes(model.Body ?? "").Length > (Configuration.Get<int>("TransactionalSend:BodyLimitInByte")))
            {
                model.Body = "Body In Request Is Very Big ";
            }
            if (LogIsExists(model))
            {
                this.Logg(model, asynchronous);
                return;
            }


            Sql sql = new Sql()
            {
                Text = "Sp_Ra_ProducerConsumerLogg_Update",
                commandType = System.Data.CommandType.StoredProcedure,
                Parameters = new
                {
                    Id = model.id,
                    SenderName = model.SenderName,
                    SenderIp = model.SenderIp,
                    ReciverName = model.ReceiverName,
                    ReciverIp = model.ReceiverIp,
                    QueueName = model.QueueName,
                    RoutingKey = model.RoutingKey,
                    Body = model.Body,
                    ErrMessageId = model.ErrorMessageId,
                    LastUpdateDate = model.LastUpdateDate,
                    //CreateDate = model.CreateDate,
                    ProccessDate = model.ProccessDate,
                    LogLevel = model.LogLevel.ToString(),
                    UserHostName = model.UserHostName,
                    UserID = model.UserID,
                    AppName = model.AppName,
                    Attemp = model.Attemp,
                    IsTransactional = model.IsTransacional
                }
            };

            if (!asynchronous)
            {
                connectionManager.Execute(sql);
                return;
            }
            //_Queue.Enqueue(model);
        }

        public bool LogIsExists(LogModel model)
        {
            if (model.id == null)
                return false;
            Sql sql = new Sql()
            {
                Text = $"Select 1 from {Configuration.Get<string>("Rabbit:LogTableName") ?? throw new Exception("The Config File Not Contains LogTableName")}" +
                $" where id = '{model.id}'",
                commandType = System.Data.CommandType.Text

            };
            byte[]? result = connectionManager.Query<byte>(sql).ToArray();
            return (result.Count() ==  0 );
        }

        public void ProgramLogg(RabbitSender.Models.LogModels.ProgramLogModel model, bool asynchronous = false)
        {

            short CrisisLevel = Configuration.Get<short>("Rabbit:CrisisLevel");
            if ((model.Loglevel == LogLevel.Information || model.Loglevel == LogLevel.Warning) && CrisisLevel == 1)//خوشبینانه 
            {
                return;
            }
            if (model.Loglevel == LogLevel.Information && CrisisLevel == 3)
                return;


            Sql sql = new Sql()
            {
                Text = "Sp_Ra_ProducerConsumerProgramLogg_Insert",
                commandType = System.Data.CommandType.StoredProcedure,
                Parameters = new
                {
                    ProducerConsumerLogg = model.ProducerConsumerLogId,
                    MessageId = model.MessageId,
                    Logglevel = model.Loglevel.ToString(),
                    IsStopProgram = model.IsStopProgram,
                    LoggDate = model.LogDate,
                    SourcePath = model.SourcePath,
                    IsFinishInCurrentOperation = model.IsFinishInCurrentOperation,
                    CurrentOperation = model.LogOwner,
                    SendLevel = model.SendLevel,
                    IsTotalFinish = model.IsTotalFinish
                }
            };
            if (!asynchronous)
            {
                try
                {
                    connectionManager.Execute(sql);
                }
                catch (Exception ex)
                {

                    throw;
                }
                return;
            }

            //_Queue.Enqueue(model);
        }

    }
}
