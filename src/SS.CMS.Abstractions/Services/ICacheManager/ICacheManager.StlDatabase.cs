using System.Collections.Generic;
using System.Data;
using SS.CMS.Data;

namespace SS.CMS.Services.ICacheManager
{
    public partial interface ICacheManager
    {
        int GetPageTotalCount(string sqlString);

        string GetStlPageSqlString(string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex);

        string GetString(string connectionString, string queryString);

        DataSet GetDataSet(string connectionString, string queryString);

        List<KeyValuePair<int, Dictionary<string, object>>> GetContainerSqlList(IDb db, string queryString);

        DataTable GetDataTable(string connectionString, string queryString);
    }
}
