using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWSMan.Core.Model.RabbitModel
{
    using System.Text.Json.Serialization;

    public class BindingModel
    {
        /// <summary>
        /// اسم exchange که پیام‌ها از آن می‌آید
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        public string VHost { get; set; }

        /// <summary>
        /// مقصد binding (queue یا exchange)
        /// </summary>
        [JsonPropertyName("destination")]
        public string Destination { get; set; }

        /// <summary>
        /// نوع مقصد: queue یا exchange
        /// </summary>
        [JsonPropertyName("destination_type")]
        public string DestinationType { get; set; }

        /// <summary>
        /// کلید روتینگ (routing key) این binding
        /// </summary>
        [JsonPropertyName("routing_key")]
        public string RoutingKey { get; set; }

        /// <summary>
        /// کلید property استفاده شده در binding
        /// </summary>
        [JsonPropertyName("properties_key")]
        public string PropertiesKey { get; set; }

        /// <summary>
        /// آرگومان‌های اضافی binding (در صورت وجود)
        /// </summary>
        [JsonPropertyName("arguments")]
        public Dictionary<string, object> Arguments { get; set; }
    }

}
