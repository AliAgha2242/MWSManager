using Model.ApplicationModels;
using Model.PresentationModels;
using Persistence;
using RabbitSender.Models.RabbitModel;

namespace Infrastructure.RabbitSenderRepository
{
    public interface IRabbitSenderRepository
    {
        Task<BaseResultModel> SendAsync(string ReceiverName, string MessageProccessorName, decimal ReceiverVahedeTejariSN, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage);
        Task<BaseResultModel> SendAsync(string ReceiverName, string MessageProccessorName,  IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage);
        Task<BaseResultModel> SendAsync(decimal ReceiverVahedeTejariSN,string MessageProccessorName, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage);
        BaseResultModel Send(decimal ReceiverVahedeTejariSN, string MessageProccessorName, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage);
        BaseResultModel Send(string ReceiverName, string MessageProccessorName, IDictionary<string, object?> Data, RequestOrMessageEnum requestOrMessage);

        IList<BaseResultModel> SendSaga(params ManagerSendModel[] sendModels);
        IList<BaseResultModel> SendParallelism(RequestOrMessageEnum requestOrMessage , params ManagerSendModel[] sendModels );

    }
}
