using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWSMan.Core.Model.RabbitModel
{
    public class ExchangeModel
    {
        public Dictionary<string, object> Arguments { get; set; }
        public bool AutoDelete { get; set; }
        public bool Durable { get; set; }
        public bool Internal { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string UserWhoPerformedAction { get; set; }
        public string Vhost { get; set; }
    }
}
