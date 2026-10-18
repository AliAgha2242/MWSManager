using System.Runtime.CompilerServices;

namespace MWSMan.Core.Model.PublicModels.LogModels
{
    public class CallerModel
    {
        private string? CallerFilePath { get; set; }
        private string? CallerMemberName { get; set; }
        private int? CallerLineNumber { get; set; }
        public CallerModel([CallerFilePath] string? CallerFilePath = "",[CallerMemberName] string? CallerMemberName = "" , [CallerLineNumber] int? CallerLineNumber = 0)
        {
            this.CallerLineNumber = CallerLineNumber;
            this.CallerMemberName = CallerMemberName;
            this.CallerFilePath = CallerFilePath?.Replace(nameof(CallerModel),"");
        }
        public string ReturnSource()
        {
            return $"{CallerFilePath ?? ""} - {CallerMemberName ?? ""} - LineNUmber = {CallerLineNumber ?? 0}";
        }
    }
}
