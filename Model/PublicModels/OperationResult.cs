using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace Model.PublicModels
{
    public class OperationResult<T> : OperationResult
    {
        public OperationResult(bool isSuccess = true, string errMsg = "")
            : base(isSuccess, errMsg)
        {
        }
        public OperationResult(T Data,bool isSuccess = true, string errMsg = "")
        {
            this.Data = Data;
            ErrMsg = errMsg;
            IsSuccess = isSuccess;
        }
        public T Data { get; set; }
    }

    public class OperationResult
    {
        public OperationResult(bool isSuccess = true, string errMsg = "")
        {
            IsSuccess = isSuccess;
            ErrMsg = errMsg;
        }
        public bool IsSuccess { get; set; }
        public string ErrMsg { get; set; }
    }

}
