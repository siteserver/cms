using System.Collections.Generic;

namespace SS.CMS.Data
{
    public partial class Repository : IRepository
    {
        public IDatabase Database { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; }

        public Repository(IDatabase database)
        {
            Database = database;
            TableName = null;
            TableColumns = null;
        }

        public Repository(IDatabase database, string tableName)
        {
            Database = database;
            TableName = tableName;
            TableColumns = null;
        }

        public Repository(IDatabase database, string tableName, List<TableColumn> tableColumns)
        {
            Database = database;
            TableName = tableName;
            TableColumns = tableColumns;
        }
    }
}
