using Application.ServiceContract;
using Infrastructure.RabbitSenderRepository;
using Model.ApplicationModels;
using Model.PresentationModels;
using Model.PublicModels;
using MWSMan.Core.Infrastructure.Extenssion;
using MWSMan.Core.Model.Events;
using MWSMan.Core.Persistence.StaticConfiguration;
using Persistence;

namespace Application.ServiceImplement
{
    public class Send : ISend
    {
        private readonly DockerDateTime dockerDateTime;
        private readonly IRabbitSenderRepository rabbitSenderRepository;
        private readonly IList<ServerSelected> serverSelecteds;
        private readonly CenterEvent centerEvent;
        private readonly IRabbitTaskServerService serverService;
        private readonly IMinooConfiguration minooConfiguration;
        private readonly IUiOperationLog operationLog;

        public Send(DockerDateTime dockerDateTime, IRabbitSenderRepository rabbitSenderRepository, IList<ServerSelected> serverSelecteds, CenterEvent centerEvent, IRabbitTaskServerService serverService, IMinooConfiguration minooConfiguration, IUiOperationLog operationLog)
        {
            this.dockerDateTime = dockerDateTime;
            this.rabbitSenderRepository = rabbitSenderRepository;
            this.serverSelecteds = serverSelecteds;
            this.centerEvent = centerEvent;
            this.serverService = serverService;
            this.minooConfiguration = minooConfiguration;
            this.operationLog = operationLog;
        }

        public async Task<IList<BaseResultModel>> RunTask(RabbitTasks tasks, bool serverNavSideEffect = true)
        {
            if (serverNavSideEffect)
            {
                serverSelecteds.Select(s => s.isSelected = (tasks.RabbitTaskServerss.Select(p => p.ServerName.ToLower()).Contains(s.serverInfo.ServerName.ToLower())));
                serverSelecteds.TakeAllServerOn(ResponseState.None, s => !tasks.RabbitTaskServerss.Select(p => p.ServerName.ToLower()).Contains(s.serverInfo.ServerName.ToLower()));
                serverSelecteds.TakeAllServerOn(ResponseState.Wait, s => tasks.RabbitTaskServerss.Select(p => p.ServerName.ToLower()).Contains(s.serverInfo.ServerName.ToLower()));
                centerEvent.ServerEventCaller();
            }
            IList<BaseResultModel> resultModels = new List<BaseResultModel>();
            if (tasks.rabbitConsumerRequestHub.IsTransactional)
            {
                foreach (var item in tasks.RabbitTaskServerss)
                {
                    var result = await SendTransactionalAsync(item.ServerName, tasks.rabbitConsumerRequestHub.ProcessorName, tasks.rabbitConsumerRequestHub.Body.TooDictionary(), serverNavSideEffect);
                    RabbitTaskServers rabbitTaskServers = new RabbitTaskServers()
                    {
                        Id = item.Id,
                        LastStartDate = dockerDateTime.NowDateTime,
                        RabbitTaskId = tasks.Id,
                        ServerName = item.ServerName,
                        LastCorrectSendId = result.IsSuccsss ? result.id : item.LastCorrectSendId,
                        LastSendWithErrorId = !result.IsSuccsss ? result.id : null,
                        LastId = result.id
                    };

                    resultModels.Add(result);
                    await serverService.Update(rabbitTaskServers, true);
                    await UpdateLogAsync(item.ServerName, result.id.Value);


                }

            }
            else
            {
                foreach (var item in tasks.RabbitTaskServerss)
                {
                    var result = await SendToQueueAsync(item.ServerName, tasks.rabbitConsumerRequestHub.ProcessorName, tasks.rabbitConsumerRequestHub.Body.TooDictionary(), serverNavSideEffect);

                    if (result is null || result.id is null)
                        Console.WriteLine($"Error In Result Result.id = {result?.id} result = {result ?? null}");

                    RabbitTaskServers rabbitTaskServers = new RabbitTaskServers()
                    {
                        Id = item.Id,
                        LastStartDate = dockerDateTime.NowDateTime,
                        RabbitTaskId = tasks.Id,
                        ServerName = item.ServerName,
                        LastCorrectSendId = result.IsSuccsss ? result.id : item.LastCorrectSendId,
                        LastSendWithErrorId = !result.IsSuccsss ? result.id : null
                    };

                    await serverService.Update(rabbitTaskServers, true);
                    resultModels.Add(result);
                    await UpdateLogAsync(item.ServerName, result.id.Value);

                }
            }
            return resultModels;
        }

        public async IAsyncEnumerable<BaseResultModel> SendAuto(IEnumerable<string> ReceiverName, string ProccessorName, IDictionary<string, object?> data, RequestOrMessageEnum requestOrMessageEnum)
        {
            serverSelecteds.TakeAllServerOn(ResponseState.None, p => p.isSelected == false);
            serverSelecteds.TakeAllServerOn(ResponseState.Wait, p => p.isSelected);
            centerEvent.ServerEventCaller();
            if (requestOrMessageEnum is RequestOrMessageEnum.Message)
            {
                foreach (var item in ReceiverName.Distinct())
                {
                    var result = await this.SendToQueueAsync(item, ProccessorName, data);
                    if (result.IsSuccsss)
                        serverSelecteds.TakeAllServerOn(ResponseState.Finish, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                    else
                        serverSelecteds.TakeAllServerOn(ResponseState.FinishWithError, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                    centerEvent.ServerEventCaller();

                    yield return result;
                }
            }
            else
            {
                foreach (var item in ReceiverName.Distinct())
                {
                    BaseResultModel result = await this.SendTransactionalAsync(item, ProccessorName, data);
                    if (result.IsSuccsss)
                        serverSelecteds.TakeAllServerOn(ResponseState.Finish, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                    else
                        serverSelecteds.TakeAllServerOn(ResponseState.FinishWithError, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                    centerEvent.ServerEventCaller();

                    yield return result;

                }
            }
        }




        public IList<Task> SendAuto((IEnumerable<string> ReceiverName, string ProccessorName, IDictionary<string, object?> data, RequestOrMessageEnum requestOrMessageEnum) parametr)
        {
            serverSelecteds.TakeAllServerOn(ResponseState.None, p => p.isSelected == false);
            serverSelecteds.TakeAllServerOn(ResponseState.Wait, p => p.isSelected);
            centerEvent.ServerEventCaller();
            IList<Task> result = new List<Task>();


            foreach (var item in parametr.ReceiverName.Distinct())
            {
                if (parametr.requestOrMessageEnum is RequestOrMessageEnum.Request)
                {
                    result.Add(Task<BaseResultModel>.Run(async () =>
                    {
                        var result = await this.SendTransactionalAsync(item, parametr.ProccessorName, parametr.data);
                        if (result.IsSuccsss)
                            serverSelecteds.TakeAllServerOn(ResponseState.Finish, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                        else
                            serverSelecteds.TakeAllServerOn(ResponseState.FinishWithError, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                        centerEvent.ServerEventCaller();
                    }));
                }
                else
                {
                    result.Add(Task<BaseResultModel>.Run(async () =>
                    {
                        var result = await this.SendToQueueAsync(item, parametr.ProccessorName, parametr.data);
                        if (result.IsSuccsss)
                            serverSelecteds.TakeAllServerOn(ResponseState.Finish, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                        else
                            serverSelecteds.TakeAllServerOn(ResponseState.FinishWithError, p => p.serverInfo.ServerName.ToLower() == item.ToLower());
                        centerEvent.ServerEventCaller();
                    }));
                }

            }
            return result;
        }



        public IList<BaseResultModel> SendAutoParallelism(IEnumerable<string> ReceiverName, string ProccessorName, IDictionary<string, object?> data, RequestOrMessageEnum requestOrMessageEnum)
        {
            serverSelecteds.TakeAllServerOn(ResponseState.None, p => p.isSelected == false);
            serverSelecteds.TakeAllServerOn(ResponseState.Wait, p => p.isSelected);
            centerEvent.ServerEventCaller();

            IList<BaseResultModel> resultModels = new List<BaseResultModel>();


            List<ManagerSendModel> sendModels = new List<ManagerSendModel>();
            foreach (var item in ReceiverName)
            {
                sendModels.Add(new ManagerSendModel(data, ProccessorName, null, item));
            }
            var res = rabbitSenderRepository.SendParallelism(requestOrMessageEnum, sendModels.ToArray());
            foreach (var item in res)
            {
                if (item.IsSuccsss)
                    serverSelecteds.TakeAllServerOn(ResponseState.Finish, p => p.serverInfo.ServerName.ToLower() == item.ReceiverName.ToLower());
                else
                    serverSelecteds.TakeAllServerOn(ResponseState.FinishWithError, p => p.serverInfo.ServerName.ToLower() == item.ReceiverName.ToLower());

            }
            centerEvent.ServerEventCaller();

            return resultModels;
        }

        public async IAsyncEnumerable<BaseResultModel> SendMessageForMultiServers(string MessageProccessorName, IEnumerable<decimal> ReceiverVahedeTejariSN, IDictionary<string, object?> Data)
        {
            foreach (var item in ReceiverVahedeTejariSN.Distinct())
            {
                yield return await this.SendToQueueAsync(item, MessageProccessorName, Data);
            }
        }

        public async IAsyncEnumerable<BaseResultModel> SendMessageForMultiServers(string MessageProccessorName, IEnumerable<string> ReceiverName, IDictionary<string, object?> Data)
        {
            foreach (var item in ReceiverName.Distinct())
            {
                yield return await this.SendToQueueAsync(item, MessageProccessorName, Data);
            }
        }


        public async IAsyncEnumerable<BaseResultModel> SendRequestForMultiServers(string MessageProccessorName, IEnumerable<decimal> ReceiverVahedeTejariSN, IDictionary<string, object?> Data)
        {
            foreach (var item in ReceiverVahedeTejariSN.Distinct())
            {
                yield return await this.SendTransactionalAsync(item, MessageProccessorName, Data);
            }
        }

        public async IAsyncEnumerable<BaseResultModel> SendRequestForMultiServers(string MessageProccessorName, IEnumerable<string> ReceiverName, IDictionary<string, object?> Data)
        {
            foreach (var item in ReceiverName.Distinct())
            {
                yield return await this.SendTransactionalAsync(item, MessageProccessorName, Data);
            }
        }

        public IList<BaseResultModel> SendSaga(params ManagerSendModel[] sendModels)
        {
            return rabbitSenderRepository.SendSaga(sendModels);
        }

        public async Task<BaseResultModel> SendToQueueAsync(string receiver, string messageProccessorName, IDictionary<string, object?> body, bool serverNavSideEffect = true)
        {
            BaseResultModel result;
            try
            {
                result = await rabbitSenderRepository.SendAsync(receiver, messageProccessorName, body, RequestOrMessageEnum.Message);
                operationLog.Add("ارسال پیام", receiver, result.IsSuccsss, result.StatusCode, result.IsSuccsss ? "پاسخ موفق دریافت شد." : result.ErrMessage, result.id);
            }
            catch (Exception ex)
            {
                operationLog.Add("ارسال پیام", receiver, false, 500, ex.Message);
                result = new BaseResultModel(null, ex.Message, 500, false, receiver);
            }
            if (result.IsSuccsss && serverNavSideEffect)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.ServerName.ToLower() == receiver.ToLower())?.responseInfo.responseState = ResponseState.Finish;
            else if (serverNavSideEffect)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.ServerName.ToLower() == receiver.ToLower())?.responseInfo.responseState = ResponseState.FinishWithError;

            if (serverNavSideEffect)
                centerEvent.ServerEventCaller();
            return result;
        }

        public async Task<BaseResultModel> SendToQueueAsync(decimal receiver, string messageProccessorName, IDictionary<string, object?> body, bool serverNavSideEffect = true)
        {
            BaseResultModel result;
            try
            {
                result = await rabbitSenderRepository.SendAsync(receiver, messageProccessorName, body, RequestOrMessageEnum.Message);
                operationLog.Add("ارسال پیام", receiver.ToString(), result.IsSuccsss, result.StatusCode, result.IsSuccsss ? "پاسخ موفق دریافت شد." : result.ErrMessage, result.id);
            }
            catch (Exception ex)
            {
                operationLog.Add("ارسال پیام", receiver.ToString(), false, 500, ex.Message);
                result = new BaseResultModel(null, ex.Message, 500, false, receiver.ToString());
            }
            if (result.IsSuccsss && serverNavSideEffect)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.VahedeTejariSN == receiver)?.responseInfo.responseState = ResponseState.Finish;
            else if (serverNavSideEffect)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.VahedeTejariSN == receiver)?.responseInfo.responseState = ResponseState.FinishWithError;
            if (serverNavSideEffect)
                centerEvent.ServerEventCaller();
            return result;
        }

        public BaseResultModel SendTransactional(decimal receiver, string messageProccessorName, IDictionary<string, object?> body)
        {
            BaseResultModel result = rabbitSenderRepository.Send(receiver, messageProccessorName, body, RequestOrMessageEnum.Request);
            if (result.IsSuccsss)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.VahedeTejariSN == receiver)?.responseInfo.responseState = ResponseState.Finish;
            else
                serverSelecteds.FirstOrDefault(p => p.serverInfo.VahedeTejariSN == receiver)?.responseInfo.responseState = ResponseState.FinishWithError;
            return result;
        }
        public BaseResultModel SendTransactional(string receiver, string messageProccessorName, IDictionary<string, object?> body)
        {
            BaseResultModel result = rabbitSenderRepository.Send(receiver, messageProccessorName, body, RequestOrMessageEnum.Request);
            return result;
        }

        public async Task<BaseResultModel> SendTransactionalAsync(string receiver, string messageProccessorName, IDictionary<string, object?> body, bool serverNavSideEffect = true)
        {
            BaseResultModel result;
            try
            {
                result = await rabbitSenderRepository.SendAsync(receiver, messageProccessorName, body, RequestOrMessageEnum.Request);
                operationLog.Add("ارسال تراکنشی", receiver, result.IsSuccsss, result.StatusCode, result.IsSuccsss ? "پاسخ موفق دریافت شد." : result.ErrMessage, result.id);
            }
            catch (Exception ex)
            {
                operationLog.Add("ارسال تراکنشی", receiver, false, 500, ex.Message);
                result = new BaseResultModel(null, ex.Message, 500, false, receiver);
            }
            if (result.IsSuccsss && serverNavSideEffect)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.ServerName.ToLower() == receiver.ToLower())?.responseInfo.responseState = ResponseState.Finish;
            else if (serverNavSideEffect)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.ServerName.ToLower() == receiver.ToLower())?.responseInfo.responseState = ResponseState.FinishWithError;
            if (serverNavSideEffect)
                centerEvent.ServerEventCaller();
            return result;
        }

        public async Task<BaseResultModel> SendTransactionalAsync(decimal receiver, string messageProccessorName, IDictionary<string, object?> body)
        {
            BaseResultModel result = await rabbitSenderRepository.SendAsync(receiver, messageProccessorName, body, RequestOrMessageEnum.Request);
            if (result.IsSuccsss)
                serverSelecteds.FirstOrDefault(p => p.serverInfo.VahedeTejariSN == receiver)?.responseInfo.responseState = ResponseState.Finish;
            else
                serverSelecteds.FirstOrDefault(p => p.serverInfo.VahedeTejariSN == receiver)?.responseInfo.responseState = ResponseState.FinishWithError;
            return result;
        }
        private async Task UpdateLogAsync(string receiverName, Guid id, IDictionary<string, object?>? data = null)
        {
            if (data == null)
            {

                data = new Dictionary<string, object?>()
                {
                    {"logid",$"{id.ToString()}"},
                    {"IsInTaskRun","1"}
                };
            }
            try
            {
                await SendToQueueAsync(receiverName, "UpdateLog", data, false);
            }
            catch (Exception ex)
            {

                throw;
            }


        }
    }
}
