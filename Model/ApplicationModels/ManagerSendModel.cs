using System;
using System.Collections.Generic;
using System.Text;

namespace Model.ApplicationModels
{
    public class ManagerSendModel
    {
        private string _receiverName;

        private string _messageProcessorName;
        public string ReceiverName
        {
            get
            {
                return _receiverName;
            }
            set
            {
                _receiverName = value?.ToLower();
            }
        }

        public decimal? ReceiverVahedeTejariSn { get; set; }

        public string MessageProcessorName
        {
            get
            {
                return _messageProcessorName;
            }
            set
            {
                _messageProcessorName = value;
            }
        }

        public IDictionary<string, object> Properties { get; set; }

        public ManagerSendModel(IDictionary<string, object> properties, string messageProcessorName, decimal? receiverVahedeTejariSn, string receiverName)
        {
            Properties = properties;
            MessageProcessorName = messageProcessorName;
            ReceiverVahedeTejariSn = receiverVahedeTejariSn;
            ReceiverName = receiverName;
        }

    }
}
