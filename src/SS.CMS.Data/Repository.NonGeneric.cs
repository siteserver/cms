using System.Collections.Generic;

namespace SS.CMS.Data
{
    public partial class Repository : IRepository
    {
        public DbContext DbContext { get; }
        public string TableName { get; }
        public List<TableColumn> TableColumns { get; }

        public Repository(DbContext dbContext)
        {
            DbContext = dbContext;
            TableName = null;
            TableColumns = null;
        }

        public Repository(DbContext dbContext, string tableName)
        {
            DbContext = dbContext;
            TableName = tableName;
            TableColumns = null;
        }

        public Repository(DbContext dbContext, string tableName, List<TableColumn> tableColumns)
        {
            DbContext = dbContext;
            TableName = tableName;
            TableColumns = tableColumns;
        }
    }
}
