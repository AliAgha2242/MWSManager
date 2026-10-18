namespace MWSMan.Core.Model.RabbitModel
{
    public class QueueModel
    {
        public Dictionary<string, object> arguments { get; set; }
        public bool autoDelete { get; set; }
        public double consumerCapacity { get; set; }
        public double consumerUtilisation { get; set; }
        public int consumers { get; set; }
        public int deliveryLimit { get; set; }
        public bool durable { get; set; }
        public Dictionary<string, object> effectivePolicyDefinition { get; set; }
        public bool exclusive { get; set; }
        public GarbageCollectionModel garbageCollection { get; set; }
        public string leader { get; set; }
        public List<string> members { get; set; }
        public long memory { get; set; }
        public long messageBytes { get; set; }
        public long messageBytesDlx { get; set; }
        public long messageBytesPersistent { get; set; }
        public long messageBytesRam { get; set; }
        public long messageBytesReady { get; set; }
        public long messageBytesUnacknowledged { get; set; }
        public int messages { get; set; }
        public RateDetailsModel messagesDetails { get; set; }
        public int messagesDlx { get; set; }
        public int messagesPersistent { get; set; }
        public int messagesRam { get; set; }
        public int messagesReady { get; set; }
        public RateDetailsModel messagesReadyDetails { get; set; }
        public int messagesReadyHigh { get; set; }
        public int messagesReadyNormal { get; set; }
        public int messagesReadyReturned { get; set; }
        public int messagesUnacknowledged { get; set; }
        public RateDetailsModel messagesUnacknowledgedDetails { get; set; }
        public string name { get; set; }
    }
}
