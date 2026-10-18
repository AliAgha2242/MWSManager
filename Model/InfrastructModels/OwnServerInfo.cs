using System.Net;

namespace MWSMan.Core.Model.InfrastructModels
{
    public class OwnServerInfo
    {
        public string? ServerName { get; set; }
        public string? DataBaseName { get; set; }
        public string ServerIp { get; set; }
        public int TimeOutMiliSecond { get; set; }
        public bool TrustServerCertificate { get; set; }
        public bool MultipleActiveResultSets { get; set; }
        public SqlLoginType? sqlLoginType { get; set; }
        public string Password { get; set; }
        public string UserId { get; set; }
        public OwnServerInfo()
        {
            ServerName = Dns.GetHostName();
            ServerIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Last().ToString();
        }
    }
}
