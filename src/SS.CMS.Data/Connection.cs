using System.Data;

namespace SS.CMS.Data
{
    public class Connection : IDbConnection
    {
        private readonly IDbConnection _dbConnection;
        public DatabaseType DatabaseType { get; }
        public string ConnectionString { get; set; }
        public int ConnectionTimeout => _dbConnection.ConnectionTimeout;
        public string Database => _dbConnection.Database;
        public ConnectionState State => _dbConnection.State;

        public Connection(DatabaseType databaseType, string connectionString)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            _dbConnection = DatoryUtils.GetConnection(databaseType, connectionString);
        }

        public IDbTransaction BeginTransaction()
        {
            return _dbConnection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _dbConnection.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            _dbConnection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            _dbConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _dbConnection.CreateCommand();
        }

        public void Open()
        {
            _dbConnection.Open();
        }

        public void Dispose()
        {
            _dbConnection.Dispose();
        }
    }
}
