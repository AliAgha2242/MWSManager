using System;
using System.Collections.Generic;
using System.Text;

namespace Model.PresentationModels
{
    public class BaseResultModel
    {
        public string ErrMessage { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccsss { get; set; }
        public string? ReceiverName { get; set; }
        public Guid? id { get; set; }
        public BaseResultModel(Guid? id = null,string errMessage = "", int statusCode = 200, bool isSuccsss = true,string? receiverName = null)
        {
            ErrMessage = errMessage;
            StatusCode = statusCode;
            IsSuccsss = isSuccsss;
            ReceiverName = receiverName;
            this.id = id;
        }
    }
    public class BasResult<T> : BaseResultModel
    {
        public T Data { get; set; }
        public BasResult(T data)
        {
            Data = data;
        }
    }

    
}
