using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net;

namespace RabbitMq_Infrastructure.SqlConnectionManagerGroup
{
    public class SqlConnectionManager
    {

        public IDbConnection Connection { get; set; }
        private CSConfig _scConfig;
        public CSConfig SConfig
        {
            get {
                return _scConfig;
            }
            set
            {
                
                _scConfig = value;
                using (IDbConnection Conn = new SqlConnection(_scConfig.Connectionstring))
                {
                    Connection = Conn;
                }
                Connection.ConnectionString = _scConfig.Connectionstring;

            }
        }
        public SqlConnectionManager(CSConfig sConfig)
        {
            _scConfig = sConfig;
            using (IDbConnection Conn = new SqlConnection(SConfig.Connectionstring))
            {
                Connection = Conn;
            }
            Connection.ConnectionString = SConfig.Connectionstring;

        }
        public SqlConnectionManager(string cs)
        {
            using (IDbConnection Conn = new SqlConnection(cs))
            {
                Connection = Conn;
            }
            Connection.ConnectionString = cs;

        }

        /// <summary>
        /// (health checks)
        /// </summary>
        //public async Task GetRealDatabase(IDbConnection? _connection , string? ServerName)
        //{
        //    if (_connection is null)
        //        _connection = this.Connection;
        //    if (ServerName == null) ServerName = Dns.GetHostName();

        //    string Query = $"Select * from  RaVw_ServerDatabaseInfo where serverName Like '%{ServerName}%'";
        //    IEnumerable<ServerDatabaseModel> serverDatabases = await _connection.QueryAsync<ServerDatabaseModel>(Query);
        //    if (serverDatabases.Count() == 0 || serverDatabases.Count() > 1)
        //    {

        //    }








        //}

        public dynamic QuerySingle(Sql sql)
        {
            return Connection.QuerySingle(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);
        }
        public async Task<dynamic> QuerySingleAsync(Sql sql)
        {
            return await Connection.QuerySingleAsync(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }

        public T QuerySingle<T>(Sql sql)
        {
            return Connection.QuerySingle<T>(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }
        public async Task<T> QuerySingleAsync<T>(Sql sql)
        {
            return await Connection.QuerySingleAsync<T>(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }



        public dynamic? QuerySingleOrDefault(Sql sql)
        {
            return Connection.QuerySingleOrDefault(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }
        public async Task<dynamic?> QuerySingleOrDefaultAsync(Sql sql)
        {
            return await Connection.QuerySingleOrDefaultAsync(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }

        public T? QuerySingleOrDefault<T>(Sql sql)
        {
            return Connection.QuerySingleOrDefault<T>(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }
        public async Task<T?> QuerySingleOrDefaultAsync<T>(Sql sql)
        {
            return await Connection.QuerySingleOrDefaultAsync<T>(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }




        public IEnumerable<T> Query<T>(Sql sql)
        {
            return Connection.Query<T>(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }
        public IEnumerable<dynamic> Query(Sql sql)
        {
            return Connection.Query(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }





        public async Task<IEnumerable<T>> QueryAsync<T>(Sql sql)
        {
            return await Connection.QueryAsync<T>(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }

        public async Task<IEnumerable<dynamic>> QueryAsync(Sql sql)
        {
            return await Connection.QueryAsync(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }

        public int Execute(Sql sql)
        {
            return Connection.Execute(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }
        public async Task<int> ExecuteAsync(Sql sql)
        {
            return await Connection.ExecuteAsync(sql.Text, sql.Parameters, transaction: sql.transaction, commandType: sql.commandType, commandTimeout: Connection.ConnectionTimeout / 100);

        }
    }
}
