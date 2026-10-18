using Application.ServiceContract;
using Model.PublicModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMq_Infrastructure.RabbitConnection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Application.ServiceImplement
{
    public class SendMessgaeDirectly : ISendMessgaeDirectly
    {
        private readonly RabbitConnectionManager rabbitConnectionManager;
        private IChannel channel;
        private IConnection connection;

        public SendMessgaeDirectly(RabbitConnectionManager rabbitConnectionManager)
        {
            this.rabbitConnectionManager = rabbitConnectionManager;

        }
        public async Task<OperationResult> SendMessage(string exchange, string queueName, string Message, IDictionary<string, object> headers)
        {
            connection = await rabbitConnectionManager.ConnectionGenerateAsync();
            using (channel = await connection.CreateChannelAsync())
            {
                var Body = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(Message));
                BasicProperties basicProperties = new BasicProperties();
                basicProperties.Headers = headers;
                try
                {
                    await channel.BasicPublishAsync(exchange, queueName, true, basicProperties, Body);
                }
                catch (OperationInterruptedException ex)
                {

                    return new OperationResult(false, $"صف یا اکسچنج مورد نظر یافت نشد");
                }

            }
            return new OperationResult(true,"");
        }
    }
}
