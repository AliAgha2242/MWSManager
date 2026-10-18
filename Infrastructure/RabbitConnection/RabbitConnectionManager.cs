//using MWSMan.Core.Infrastructure.LoggManager.LoggManager;
using MWSMan.Core.Infrastructure.LoggManager.LoggManager;
using MWSMan.Core.Model.RabbitModel;
using MWSMan.Core.Persistence.StaticConfiguration;
using RabbitMQ.Client;

namespace RabbitMq_Infrastructure.RabbitConnection
{
    public class RabbitConnectionManager : ConnectionFactoryBase, IDisposable
    {
        private VHostModel _vHostModel { get; set; }
        private UserModel _userModel { get; set; }
        private ConnectionFactory connectionFactory;
        private byte _maxRetries;
        private byte _WaitTimeRetriesSecond;
        private IConnection? connection;
        //private readonly IMinooLogger minoologger;
        private readonly IMinooConfiguration configuration;

        public RabbitConnectionManager(VHostModel vHostModel, UserModel userModel, IMinooConfiguration configuration)
        {

            _vHostModel = vHostModel;
            _userModel = userModel;
            connectionFactory = new ConnectionFactory()
            {
                HostName = configuration.Get<string>("Rabbit:ServerIp"),
                VirtualHost = vHostModel.Name,
                UserName = userModel.UserName,
                Password = userModel.Password,
                Port = configuration.Get<int>("Rabbit:ServerPort"),
                AutomaticRecoveryEnabled = configuration.Get<bool>("Rabbit:AutoRecoveryConnection"),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(configuration.Get<int>("Rabbit:IntervalRecoveryConnectionSecond"))
            };
            _maxRetries = configuration.Get<byte>("Rabbit:MaxRetries");
            _WaitTimeRetriesSecond = configuration.Get<byte>("Rabbit:WaitSecondRetries");
            this.configuration = configuration;

            ConnectionGenerateAsync();


        }

        public async Task<IConnection> ConnectionGenerateAsync()
        {
            if (connection is not null)
                return connection;

            byte attempt = 1;
            while (true)
            {
                try
                {
                    connection = await connectionFactory.CreateConnectionAsync();
                    //minoologger.ProgramLog(new ProgramLogModel(null, MinooLogLevel.Success,$"Connection To Rabbit Service Is Success Your Config = ( {JsonSerializer.Serialize(_userModel)} ) --- ( {JsonSerializer.Serialize(_vHostModel)} ) "));
                    break;
                }
                catch (Exception)
                {
                    if (attempt == _maxRetries)
                    {

                        //minoologger.ProgramLog(new ProgramLogModel(null,MinooLogLevel.Error, true, "Connection To The Rabbit Is Failed", 3,true), false);
                        throw new Exception("Connection To The Rabbit Is Failed");
                    }

                    //minoologger.ProgramLog(new ProgramLogModel(null,MinooLogLevel.Warning,false, $"Retry For {attempt++}"));
                    attempt++;
                    Thread.Sleep(_WaitTimeRetriesSecond * 10000);
                }

            }
            return connection;
        }

        void IDisposable.Dispose()
        {

            connection?.Dispose();

        }
    }

}
