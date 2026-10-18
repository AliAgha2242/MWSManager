using Model.ApplicationModels;
using Model.PresentationModels;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServiceContract
{
    public interface ISend
    {
        Task<BaseResultModel> SendToQueueAsync(string receiver, string messageProccessorName, IDictionary<string, object?> body, bool serverNavSideEffect = true);
        Task<BaseResultModel> SendTransactionalAsync(string receiver, string messageProccessorName, IDictionary<string, object?> body, bool serverNavSideEffect = true);
        Task<BaseResultModel> SendToQueueAsync(decimal receiver, string messageProccessorName, IDictionary<string, object?> body, bool serverNavSideEffect = true);

        Task<BaseResultModel> SendTransactionalAsync(decimal receiver, string messageProccessorName, IDictionary<string, object?> body);
        BaseResultModel SendTransactional(decimal receiver, string messageProccessorName, IDictionary<string, object?> body);
        BaseResultModel SendTransactional(string receiver, string messageProccessorName, IDictionary<string, object?> body);

        IAsyncEnumerable<BaseResultModel> SendMessageForMultiServers(string MessageProccessorName, IEnumerable<decimal> ReceiverVahedeTejariSN, IDictionary<string, object?> Data);
        IAsyncEnumerable<BaseResultModel> SendMessageForMultiServers(string MessageProccessorName, IEnumerable<string> ReceiverName, IDictionary<string, object?> Data);
        IAsyncEnumerable<BaseResultModel> SendRequestForMultiServers(string MessageProccessorName, IEnumerable<decimal> ReceiverVahedeTejariSN, IDictionary<string, object?> Data);
        IAsyncEnumerable<BaseResultModel> SendRequestForMultiServers(string MessageProccessorName, IEnumerable<string> ReceiverName, IDictionary<string, object?> Data);
        IAsyncEnumerable<BaseResultModel> SendAuto(IEnumerable<string> ReceiverName, string ProccessorName, IDictionary<string, object?> data, RequestOrMessageEnum requestOrMessageEnum);
        IList<Task> SendAuto((IEnumerable<string> ReceiverName, string ProccessorName, IDictionary<string, object?> data, RequestOrMessageEnum requestOrMessageEnum) parametr);

        IList<BaseResultModel> SendAutoParallelism(IEnumerable<string> ReceiverName, string ProccessorName, IDictionary<string, object?> data, RequestOrMessageEnum requestOrMessageEnum);


        IList<BaseResultModel> SendSaga(params ManagerSendModel[] sendModels);
        Task<IList<BaseResultModel>> RunTask(RabbitTasks tasks, bool serverNavSideEffect = true);

    }



}
