using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.ApplicationModels
{
    public class RabbitTasks
    {
        [Key]
        public int Id { get; set; }
        public string TaskName { get; set; }
        public  int RabbitConsumerRequestHubId { get; set; }
        public short IntervalHour { get; set; }
        public short IntervalMinute { get; set; }
        public short IntervalTotalMinute { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        public string? FromPersianDate { get; set; }
        public string? ToPersianDate { get; set; }

        public bool StopForce { get; set; }

        public ICollection<RabbitTaskServers>? RabbitTaskServerss { get; set; }
        public RabbitConsumerRequestHub? rabbitConsumerRequestHub { get; set; }
       
    }
}
