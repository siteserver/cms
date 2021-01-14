using System.Collections.Generic;

namespace Datory
{
    public partial class Repository : IRepository
    {
        public IDatabase Database { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; }
        public IRedis Redis { get; }

        public Repository(IDatabase database)
        {
            Database = database;
            TableName = null;
            TableColumns = null;
        }

        public Repository(IDatabase database, IRedis redis)
        {
            Database = database;
            TableName = null;
            TableColumns = null;
            Redis = redis;
        }

        public Repository(IDatabase database, string tableName)
        {
            Database = database;
            TableName = tableName;
            TableColumns = null;
        }

        public Repository(IDatabase database, string tableName, IRedis redis)
        {
            Database = database;
            TableName = tableName;
            TableColumns = null;
            Redis = redis;
        }

        public Repository(IDatabase database, string tableName, List<TableColumn> tableColumns)
        {
            Database = database;
            TableName = tableName;
            TableColumns = tableColumns;
        }

        public Repository(IDatabase database, string tableName, List<TableColumn> tableColumns, IRedis redis)
        {
            Database = database;
            TableName = tableName;
            TableColumns = tableColumns;
            Redis = redis;
        }
    }
}
