using MWSMan.Core.Model.InfrastructModels;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace Model.PresentationModels
{
    public class ServerSelected
    {
        public ServerInfo serverInfo { get; set; }
        public bool isSelected { get; set; }
        public ResponseInfo responseInfo { get; set; }


        public ServerSelected(ServerInfo serverInfo, bool isSelected = false, ResponseInfo? responseInfo = null)
        {
            this.serverInfo = serverInfo;
            this.isSelected = isSelected;
            this.responseInfo = responseInfo??new ResponseInfo("") { responseState = ResponseState.None};
        }


    }
    public class ResponseInfo
    {
        public ResponseInfo(string receiverName)
        {
            this.receiverName = receiverName;
        }
        public string receiverName { get; set; }
        public ResponseState responseState { get; set; }
    }
    public enum ResponseState
    {
        Wait = 0,
        Cancel = 10,
        Finish = 20,
        FinishWithError = 30,
        None = 40
    }
}
