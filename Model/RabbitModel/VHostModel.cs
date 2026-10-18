namespace MWSMan.Core.Model.RabbitModel
{
    public class VHostModel 
    {
        public int Messages { get; set; }
        public string Name { get; set; }
        public long RecvOct { get; set; }
        public long SendOct { get; set; }
        public string Description { get; set; }
        public MetadataModel Metadata { get; set; }
        public List<string> Tags { get; set; }
        public string DefaultQueueType { get; set; }
        public int MessagesReady { get; set; }
        public int MessagesUnacknowledged { get; set; }
        public bool ProtectedFromDeletion { get; set; }
        public bool Tracing { get; set; }
        public Dictionary<string, string> ClusterState { get; set; }
        public MessageStatsModel MessageStats { get; set; }
        public RateDetailsModel RecvOctDetails { get; set; }
        public RateDetailsModel SendOctDetails { get; set; }
        public RateDetailsModel MessagesDetails { get; set; }
        public RateDetailsModel MessagesUnacknowledgedDetails { get; set; }
        public RateDetailsModel MessagesReadyDetails { get; set; }
        public VHostModel(string Name)
        {
            this.Name = Name;
        }
    }



}
