using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWSMan.Core.Model.Utilitis
{
    public class BaseProp
    {
        public byte? tryCount;
        public string? UserId { get; set; }
        public Guid? CurrentId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? AppName { get; set; }
        public string? SenderIp { get; set; }
        public string? Sendername { get; set; }
        public string? ReceiverName { get; set; }
        public string? MessageBody { get; set; }
        public string? QueueName { get; set; }
        public string? RoutingKey { get; set; }
    }
}

