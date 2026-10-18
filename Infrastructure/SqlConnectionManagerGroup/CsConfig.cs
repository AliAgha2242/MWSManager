

using MWSMan.Core.Model.InfrastructModels;

namespace RabbitMq_Infrastructure.SqlConnectionManagerGroup
{
    public class CSConfig
    {
        public string Password { get; set; }
        public string UserId { get; set; }
        public string ServerName { get; set; }
        public string DbName { get; set; }
        public int TimeOutMiliSecond { get; set; }
        public bool TrustServerCertificate { get; set; }
        public bool MultipleActiveResultSets { get; set; }
        public SqlLoginType? sqlLoginType { get; set; }
        public string Connectionstring { get; set; }
        public CSConfig() { }
        public CSConfig(OwnServerInfo serverInfo)
        {
            Password = serverInfo.Password;
            UserId = serverInfo.UserId;
            DbName = serverInfo.DataBaseName;
            ServerName = serverInfo.ServerName;
            TimeOutMiliSecond = serverInfo.TimeOutMiliSecond;
            TrustServerCertificate = serverInfo.TrustServerCertificate;
            MultipleActiveResultSets = serverInfo.MultipleActiveResultSets;
            sqlLoginType = serverInfo.sqlLoginType;
            CsGenerate();
        }
        //public CSConfig(CenterServerInfo serverInfo)
        //{
        //    Password = serverInfo.SqlPassword;
        //    UserId = serverInfo.SqlUserName;
        //    DbName = serverInfo.DbName;
        //    ServerName = serverInfo.ServerName;
        //    TimeOutMiliSecond = 300;
        //    TrustServerCertificate = true;
        //    MultipleActiveResultSets = true;
        //    sqlLoginType = serverInfo.sqlLoginType;
        //    CsGenerate();
        //}
        private string CsGenerate()
        {
            string Cs;
            if (sqlLoginType == SqlLoginType.WindowsAuthentication)
            {
                Cs = @$"server={ServerName};initial catalog={DbName};Trusted_Connection={TrustServerCertificate};TrustServerCertificate={TrustServerCertificate.ToString()};Connect Timeout={TimeOutMiliSecond};";
            }
            else
            {
                Cs = @$"server={ServerName};initial catalog={DbName};user id={UserId};password={Password};
                           TrustServerCertificate={TrustServerCertificate};MultipleActiveResultSets={MultipleActiveResultSets};Connect Timeout={TimeOutMiliSecond};";
            }

            //string Cs = @$"data source={ServerName};initial catalog={DbName};user id={UserId};password={Password};Trusted_Connection={TrustServerCertificate};MultipleActiveResultSets={MultipleActiveResultSets};Connect Timeout={TimeOutMiliSecond};";
            Connectionstring = Cs;
            return Cs;
        }
    }
}
