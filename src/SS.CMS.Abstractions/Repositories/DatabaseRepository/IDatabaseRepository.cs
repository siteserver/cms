using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public partial interface IDatabaseRepository
    {
        List<IRepository> GetAllRepositories();

        Database GetDatabase(string connectionString = null);

        int GetIntResult(string connectionString, string sqlString);

        int GetIntResult(string sqlString);

        string GetString(string connectionString, string sqlString);

        IEnumerable<IDictionary<string, object>> GetRows(string connectionString, string sqlString);

        int GetPageTotalCount(string sqlString);

        string GetStlPageSqlString(string sqlString, string orderString, int totalCount, int itemsPerPage,
            int currentPageIndex);

        string GetSelectSqlString(string tableName, int totalNum, string columns, string whereString,
            string orderByString);

        int GetCount(string tableName);

        IEnumerable<dynamic> GetObjects(string tableName);

        IEnumerable<dynamic> GetPageObjects(string tableName, string identityColumnName, int offset, int limit);

        string GetPageSqlString(string tableName, string columnNames, string whereSqlString, string orderSqlString,
            int offset, int limit);
    }
}