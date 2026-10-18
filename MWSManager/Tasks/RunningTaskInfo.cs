using Model.ApplicationModels;


namespace MWSManager.Tasks
{
    internal sealed class RunningTaskInfo
    {
        public RabbitTasks Config { get; set; }
        public Task Task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
