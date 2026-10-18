namespace MWSMan.Core.Model.PublicModels.LogModels
{
    public class BaseLogModel
    {
        public string QueryOrCommand { get; set; }
        public string LogOwner { get; set; } = "Receiver";

    }
}
