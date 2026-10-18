using Model.PublicModels;
using MWSMan.Core.Model.Utilitis;
using System.Net;
using System.Reflection;

//using System.Runtime.CompilerServices;

namespace MWSMan.Core.Model.PublicModels.LogModels
{
    public class RabbitLogModel : BaseLogModel
    {
        public Guid? id { get; set; }
        public string SenderName { get; set; }
        public string SenderIp { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverIp { get; set; }
        public string? QueueName { get; set; }
        public string? RoutingKey { get; set; }
        public string? Body { get; set; }
        public bool IsTransactional { get; }
        public int? ErrorMessageId { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ProccessDate { get; set; }
        public MinooLogLevel? LogLevel { get; set; }
        public string? UserID { get; set; }
        public string? AppName { get; set; }
        public byte? Attemp { get; set; }
        public string UserHostName { get; set; }
        //public AppDynamicSetting appDynamicSetting { get; set; }
        public RabbitLogModel()
        {
            
        }
        public RabbitLogModel(BaseProp prop,string ReceiverName, string senderIp, byte? tryCount = 1, string? userID = null, MinooLogLevel? logLevel = default, DateTime? proccessDate = default,
           int? ErrorMessageId = null, string? body = null, string? routingKey = null,bool IsTransactional = false)
        {
            IPAddress[] iPAddresses;
            try
            {
                iPAddresses = Dns.GetHostEntry(ReceiverName ?? Dns.GetHostName()).AddressList;

            }
            catch (Exception e)
            {
                throw;
            }

            //if (appDynamicSetting == null)
            //    appDynamicSetting = new AppDynamicSetting();
            LastUpdateDate = DockerStaticDateTime.NowDateTime;
            this.ReceiverName = ReceiverName ?? Dns.GetHostName();
            Attemp = 1;
            ReceiverIp = (string.IsNullOrWhiteSpace(ReceiverName ?? Dns.GetHostName()) ? null : iPAddresses[^1].ToString()) ?? "Unknown Receiver";
            Attemp = tryCount;
            AppName = Assembly.GetEntryAssembly().FullName.Split(',').FirstOrDefault();
            UserID = userID;
            LogLevel = logLevel;
            ProccessDate = proccessDate;
            CreateDate = DockerStaticDateTime.NowDateTime;
            this.ErrorMessageId = ErrorMessageId;
            Body = body;
            this.IsTransactional = IsTransactional;
            RoutingKey = routingKey ?? null;
            QueueName = !IsTransactional ? ReceiverName ?? Dns.GetHostName() : null;
            SenderIp = senderIp;
            SenderName = prop.Sendername;
            this.id = prop.CurrentId;
            UserHostName = prop.SenderIp;
        }
    }
}
