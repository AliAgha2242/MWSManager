using System.Data;

namespace RabbitMq_Infrastructure.SqlConnectionManagerGroup
{
    public class Sql
    {
        private string _Text;
        public required string Text { get { return _Text; } set { _Text = value; commandType = CommandType.Text;Parameters = null; } }

        public object? Parameters { get; set; }
        public CommandType? commandType{ get; set; }
        public IDbTransaction? transaction { get; set; }

        public Sql() { }
        public Sql(string text, object? parameters, IDbTransaction? transaction, CommandType? commandType = CommandType.Text)
        {
            Text = text;
            Parameters = parameters;
            this.commandType = commandType;
            this.transaction = transaction;
        }
    }
}
