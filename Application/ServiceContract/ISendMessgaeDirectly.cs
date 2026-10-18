using Model.PublicModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServiceContract
{
    public interface ISendMessgaeDirectly
    {
        Task<OperationResult> SendMessage(string exchange, string queueName, string Message, IDictionary<string, object> headers);
    }
}
