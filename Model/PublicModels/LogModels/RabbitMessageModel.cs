using System.ComponentModel.DataAnnotations;

namespace Model.PublicModels.LogModels
{
    public class RabbitMessage
    {
        [Key]
        public int MessageId { get; set; }
        public byte MessgaeTypeId { get; set; }
        public string MessageDescription { get; set; }
    }

    public class RabbitMessageType
    {
        [Key]
        public short MessageTypeId { get; set; }
        public string MessageTypeDescription { get; set; }
    }
}
