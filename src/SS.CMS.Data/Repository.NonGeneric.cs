using System.Collections.Generic;

namespace SS.CMS.Data
{
    public partial class Repository : IRepository
    {
        public IDb Db { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; }

        public Repository(IDb db)
        {
            Db = db;
            TableName = null;
            TableColumns = null;
        }

        public Repository(IDb db, string tableName)
        {
            Db = db;
            TableName = tableName;
            TableColumns = null;
        }

        public Repository(IDb db, string tableName, List<TableColumn> tableColumns)
        {
            Db = db;
            TableName = tableName;
            TableColumns = tableColumns;
        }
    }
}
