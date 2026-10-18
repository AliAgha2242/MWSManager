using Microsoft.Extensions.DependencyInjection;
using Model.ApplicationModels;
using Model.PresentationModels;
using Persistence;
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;
using RabbitSender;
using RabbitSender.Models.ApiModels;
using RabbitSender.Models.LogModels;
using RabbitSender.Models.RabbitModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.RabbitSenderRepository
{
    public class RabbitSenderRepository : IRabbitSenderRepository
    {
        private readonly Client client;
        private readonly IServiceProvider serviceProvider;
        private readonly SqlConnectionManager connectionManager;

        public RabbitSenderRepository(Client client, IServiceProvider serviceProvider,SqlConnectionManager connectionManager)
        {
            this.client = client;
            this.serviceProvider = serviceProvider;
            this.connectionManager = connectionManager;
        }

        public BaseResultModel Send(decimal ReceiverVahedeTejariSN, string MessageProccessorName, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage)
        {
            BaseOperationResult result = null;
            try
            {
                SendModel sendModel = new SendModel()
                {
                    ReceiverName = Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName,
                    MessageProcessorName = MessageProccessorName,
                    Properties = Data,
                    ReceiverVahedeTejariSn = ReceiverVahedeTejariSN,
                };

                if (requestOrMessage == RequestOrMessageEnum.Request)
                    result = client.SendTransactional(sendModel);
                else
                    result =  client.SendToQueue(sendModel);

                return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess, Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName);
            }
            catch (Exception ex)
            {
                if (result is not null)
                    return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess, Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName);
                else
                    return new BaseResultModel(result?.SendId, ex.Message, 500, false, Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName);
            }
        }

        public BaseResultModel Send(string ReceiverName, string MessageProccessorName, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage)
        {
            BaseOperationResult result = null;
            try
            {
                SendModel sendModel = new SendModel()
                {
                    ReceiverName = ReceiverName,
                    MessageProcessorName = MessageProccessorName,
                    Properties = Data,
                    ReceiverVahedeTejariSn = Client.GetServerInfo(serverName: ReceiverName).First().VahedeTejariSn
                };

                if (requestOrMessage == RequestOrMessageEnum.Request)
                    result = client.SendTransactional(sendModel);
                else
                    result = client.SendToQueue(sendModel);
                return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess, ReceiverName);
            }
            catch (Exception ex)
            {
                if (result is not null)
                    return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess, ReceiverName);
                else
                    return new BaseResultModel(result?.SendId, ex.Message, 500, false, Client.GetServerInfo(serverName:ReceiverName).First().ServerName);
            }
        }

        public async Task<BaseResultModel> SendAsync(string ReceiverName, string MessageProccessorName, decimal ReceiverVahedeTejariSN, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage)        
        {
            BaseOperationResult result = null;
            try
            {
                SendModel sendModel = new SendModel()
                {
                    ReceiverName = ReceiverName,
                    MessageProcessorName = MessageProccessorName,
                    Properties = Data,
                    ReceiverVahedeTejariSn = ReceiverVahedeTejariSN,
                };


                if (requestOrMessage == RequestOrMessageEnum.Request)
                    result = await client.SendTransactionalAsync(sendModel);
                else
                    result = await client.SendToQueueAsync(sendModel);

                return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess,ReceiverName);
            }
            catch (Exception ex)
            {
                if(result is not null)
                    return new BaseResultModel(result.SendId, result.ErrorMessage,result.StatusCode, result.IsSuccess, ReceiverName);
                else
                    return new BaseResultModel(result?.SendId, ex.Message,500,false, ReceiverName);
            }
            

        }
        public async Task<BaseResultModel> SendAsync(string ReceiverName, string MessageProccessorName, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage)
        {
           
            BaseOperationResult result = null;
            try
            {
                SendModel sendModel = new SendModel()
                {
                    ReceiverName = ReceiverName,
                    MessageProcessorName = MessageProccessorName,
                    Properties = Data,
                    ReceiverVahedeTejariSn = Client.GetServerInfo(serverName:ReceiverName).First().VahedeTejariSn,
                };

                if (requestOrMessage == RequestOrMessageEnum.Request)
                    result = await client.SendTransactionalAsync(sendModel);
                else
                    result = await client.SendToQueueAsync(sendModel);

                return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess, ReceiverName);
            }
            catch (Exception ex)
            {
                if (result is not null)
                    return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess, ReceiverName);
                else
                    return new BaseResultModel(result?.SendId, ex.Message, 500, false, ReceiverName);
            }


        }
        public async Task<BaseResultModel> SendAsync(decimal ReceiverVahedeTejariSN, string MessageProccessorName, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage)
        {
            BaseOperationResult result = null;
            try
            {
                SendModel sendModel = new SendModel()
                {
                    ReceiverName = Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName,
                    MessageProcessorName = MessageProccessorName,
                    Properties = Data,
                    ReceiverVahedeTejariSn = ReceiverVahedeTejariSN,
                };

                if (requestOrMessage == RequestOrMessageEnum.Request)
                    result = await client.SendTransactionalAsync(sendModel);
                else
                    result = await client.SendToQueueAsync(sendModel);

                return new BaseResultModel(result.SendId, result.ErrorMessage, result.StatusCode, result.IsSuccess, Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName);
            }
            catch (Exception ex)
            {
                if (result is not null)
                    return new BaseResultModel(result.SendId,result.ErrorMessage, result.StatusCode, result.IsSuccess, Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName);
                else
                    return new BaseResultModel(result?.SendId,ex.Message, 500, false, Client.GetServerInfo(vahedeTejariSN: Convert.ToDouble(ReceiverVahedeTejariSN)).First().ServerName);
            }


        }

        public IList<BaseResultModel> SendParallelism(RequestOrMessageEnum requestOrMessage, params ManagerSendModel[] sendModels)
        {
            IList<BaseOperationResult> results = null;
            IList<SendModel> sendModelsIndex = new List<SendModel>();
            try
            {
                foreach (var item in sendModels)
                {
                    sendModelsIndex.Add(new SendModel()
                    {
                        ReceiverVahedeTejariSn = item.ReceiverVahedeTejariSn,
                        MessageProcessorName = item.MessageProcessorName,
                        Properties = item.Properties,
                        ReceiverName = item.ReceiverName,
                    });
                }
                if (requestOrMessage is RequestOrMessageEnum.Message)
                    results = client.SendToQueueParallelism(sendModelsIndex);
                else
                    results =  client.SendTransactionalParallelism(sendModelsIndex);

                return results.Select(p=>new BaseResultModel() { ErrMessage = p.ErrorMessage,ReceiverName = p.ReceiverName,id = p.SendId,IsSuccsss = p.IsSuccess ,StatusCode = p.StatusCode}).ToList();

            }
            catch (Exception ex)
            {

                return new List<BaseResultModel>() { new BaseResultModel()
                {
                    ErrMessage = ex.Message,
                    id = null,
                    StatusCode = 500,
                    IsSuccsss = false,
                    ReceiverName = null
                } };
            }
        }

        public IList<BaseResultModel> SendSaga(params ManagerSendModel[] sendModels)
        {
            IList<BaseOperationResult> result = client.SagaSendWithQueue(sendModels.Select(p=>new SendModel(ReceiverVahedeTejariSn:p.ReceiverVahedeTejariSn.Value,p.MessageProcessorName,p.Properties)).ToArray());
            return result.Select(p=>new BaseResultModel(p.SendId,p.ErrorMessage,p.StatusCode,p.IsSuccess)).ToList();
        }
    }
}
