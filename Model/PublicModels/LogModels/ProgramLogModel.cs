using Model.PublicModels;

namespace MWSMan.Core.Model.PublicModels.LogModels
{
    public class ProgramLogModel : BaseLogModel
    {

        public long id { get; set; }

        public Guid? ProducerConsumerLogId { get; set; }
        public int MessageId { get; set; }
        public MinooLogLevel Loglevel { get; set; }
        public bool IsStopProgram { get; set; }
        public DateTime LogDate { get; set; }
        public string SourcePath { get; set; }
        public bool IsFinishInCurrentOperation { get; set; }
        public short SendLevel { get; set; }
        public bool IsTotalFinish { get; set; }

        public ProgramLogModel()
        {
            
        }
        public ProgramLogModel(Guid? ProducerConsumerLogId, MinooLogLevel loglevel, bool isStopProgram, int messageId, short sendLevel, CallerModel? callerModel = null, bool isFinishInCurrentOperation = false, bool isTotalFinish = false)
        {

            this.ProducerConsumerLogId = ProducerConsumerLogId;
            SourcePath = (callerModel ?? new CallerModel()).ReturnSource();
            Loglevel = loglevel;
            MessageId = messageId;
            IsStopProgram = isStopProgram;
            LogDate = DockerStaticDateTime.NowDateTime;
            IsFinishInCurrentOperation = isFinishInCurrentOperation;
            SendLevel = sendLevel;
            IsTotalFinish = isTotalFinish;
        }
    }
}
