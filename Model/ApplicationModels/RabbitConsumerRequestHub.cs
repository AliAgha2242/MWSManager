using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Model.ApplicationModels
{
    [Table("RabbitConsumerRequestHub")]
    public class RabbitConsumerRequestHub
    {
        [Key]
        public int Id { get; set; }
        public string? RequestName { get; set; }
        public bool IsTransactional { get; set; }
        public string RoutingKey { get; set; }
        public string? Body { get; set; }
        public string ProcessorName { get; set; }
        public ICollection<RabbitTasks>?  RabbitTasks { get; set; }
        public RabbitConsumerRequestHub()
        {
            
        }
        public RabbitConsumerRequestHub(string body, string routingKey, bool isTransactional, string requestName, string processorName)
        {
            Body = body;
            RoutingKey = routingKey;
            IsTransactional = isTransactional;
            RequestName = requestName;
            ProcessorName = processorName;
        }
    }
    public class RabbitTaskServers
    {
        [Key]
        public int Id { get; set; }
        public int RabbitTaskId { get; set; }
        public string ServerName { get; set; }
        public DateTime? LastStartDate { get; set; }
        public RabbitTasks RabbitTasks { get; set; }
        public Guid? LastCorrectSendId { get; set; }
        public Guid? LastId { get; set; }
        public Guid? LastSendWithErrorId { get; set; }
    }

}
