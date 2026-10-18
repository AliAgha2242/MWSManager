namespace MWSMan.Core.Model.PresentationModels
{

    public class ErrorModel
    {
    }
    public class ErrorModel<TOut> : ErrorModel
    {
        public string error { get; set; }
        public string Stat { get; set; }
        public TOut data { get; set; }
    }
}
