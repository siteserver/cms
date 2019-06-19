namespace SS.CMS.Services.ICacheManager
{
    public partial interface ICacheManager
    {
        string GetSelectSqlStringByQueryString(string connectionString, string queryString, int startNum, int totalNum, string orderByString);
    }
}
