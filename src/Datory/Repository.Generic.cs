using System.Collections.Generic;
using Datory.Utils;

namespace Datory
{
    public partial class Repository<T> : IRepository<T> where T : Entity, new()
    {
        public IDatabase Database { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; }
        public IRedis Redis { get; }

        public Repository(IDatabase database)
        {
            Database = database;
            TableName = ReflectionUtils.GetTableName(typeof(T));
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
        }

        public Repository(IDatabase database, IRedis redis)
        {
            Database = database;
            TableName = ReflectionUtils.GetTableName(typeof(T));
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
            Redis = redis;
        }

        public Repository(IDatabase database, string tableName)
        {
            Database = database;
            TableName = tableName;
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
        }

        public Repository(IDatabase database, string tableName, IRedis redis)
        {
            Database = database;
            TableName = tableName;
            TableColumns = ReflectionUtils.GetTableColumns(typeof(T));
            Redis = redis;
        }
    }
}
