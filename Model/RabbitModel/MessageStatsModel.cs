namespace MWSMan.Core.Model.RabbitModel
{
    public class MessageStatsModel
    {
        public int Ack { get; set; }
        public RateDetailsModel AckDetails { get; set; }
        public int Confirm { get; set; }
        public RateDetailsModel ConfirmDetails { get; set; }
        public int Deliver { get; set; }
        public RateDetailsModel DeliverDetails { get; set; }
        public int DeliverGet { get; set; }
        public RateDetailsModel DeliverGetDetails { get; set; }
        public int DeliverNoAck { get; set; }
        public RateDetailsModel DeliverNoAckDetails { get; set; }
        public int DropUnroutable { get; set; }
        public RateDetailsModel DropUnroutableDetails { get; set; }
        public int Get { get; set; }
        public RateDetailsModel GetDetails { get; set; }
        public int GetEmpty { get; set; }
        public RateDetailsModel GetEmptyDetails { get; set; }
        public int GetNoAck { get; set; }
        public RateDetailsModel GetNoAckDetails { get; set; }
        public int Publish { get; set; }
        public RateDetailsModel PublishDetails { get; set; }
        public int Redeliver { get; set; }
        public RateDetailsModel RedeliverDetails { get; set; }
        public int ReturnUnroutable { get; set; }
        public RateDetailsModel ReturnUnroutableDetails { get; set; }
    }



}
