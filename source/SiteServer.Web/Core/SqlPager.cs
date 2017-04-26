using System;
using System.Data;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;

namespace SiteServer.API.Core
{
    public enum SortMode
    {
        Asc,
        Desc
    }

    public class VirtualRecordCount
    {
        public int RecordCount;
        public int PageCount;
        public int RecordsInLastPage;
    }

    public class SqlPager
    {
        public PagedDataSource PagedDataSource { get; private set; }

        private string GetQueryCountCommandText()
        {
            return $"SELECT COUNT(*) FROM ({SelectCommand}) AS t0";
        }

        private string GetQueryPageCommandText(int recsToRetrieve)
        {
            if (!string.IsNullOrEmpty(OrderByString))
            {
                var orderByString2 = OrderByString.Replace(" DESC", " DESC2");
                orderByString2 = orderByString2.Replace(" ASC", " DESC");
                orderByString2 = orderByString2.Replace(" DESC2", " ASC");

                if (WebConfigUtils.IsMySql)
                {
                    return $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({SelectCommand}) AS t0 {OrderByString} LIMIT {ItemsPerPage * (CurrentPageIndex + 1)}
    ) AS t1 {orderByString2} LIMIT {recsToRetrieve}
) AS t2 {OrderByString}";
                }
                return $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {ItemsPerPage * (CurrentPageIndex + 1)} * FROM ({SelectCommand}) AS t0 {OrderByString}
    ) AS t1 {orderByString2}
) AS t2 {OrderByString}";
            }
            if (WebConfigUtils.IsMySql)
            {
                return $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({SelectCommand}) AS t0 ORDER BY {SortField} {SortMode} LIMIT {ItemsPerPage * (CurrentPageIndex + 1)}
    ) AS t1 ORDER BY {SortField} {AlterSortMode(SortMode)} LIMIT {recsToRetrieve}
) AS t2 ORDER BY {SortField} {SortMode}";
            }
            return $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {ItemsPerPage * (CurrentPageIndex + 1)} * FROM ({SelectCommand}) AS t0 ORDER BY {SortField} {SortMode}
    ) AS t1 ORDER BY {SortField} {AlterSortMode(SortMode)}
) AS t2 ORDER BY {SortField} {SortMode}";
        }

        public SqlPager()
        {
            PagedDataSource = null;

            CurrentPageIndex = 0;
            SelectCommand = "";
            ItemsPerPage = 10;
            TotalPages = -1;
            SortMode = SortMode.Desc;
            IsQueryTotalCount = true;
        }

        public int ItemsPerPage { get; set; }

        public int CurrentPageIndex { get; set; }

        public string SelectCommand { get; set; }

        public string OrderByString { get; set; } = "";

        public string SortField { get; set; } = "";

        public SortMode SortMode { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public bool IsQueryTotalCount { get; set; }

        /// <summary>
        /// Fetches and stores the data
        /// </summary>
        public void DataBind(int currentPage)
        {
            CurrentPageIndex = currentPage - 1;

            // Ensures enough info to connect and query is specified
            if (SelectCommand == "")
                return;

            // Fetch data
            FetchPageData();
        }

        /// <summary>
        /// Ensures the CurrentPageIndex is either valid [0,TotalPages)
        /// </summary>
        private void ValidatePageIndex()
        {
            if (CurrentPageIndex < 0)
            {
                CurrentPageIndex = 0;
            }
            else if (CurrentPageIndex > TotalPages - 1)
            {
                CurrentPageIndex = TotalPages - 1;
            }
        }

        /// <summary>
        /// Runs the query to get only the data that fit into the current page
        /// </summary>
        private void FetchPageData()
        {
            // Need a validated page index to fetch data.
            // Also need the virtual page count to validate the page index
            AdjustSelectCommand(false);
            var countInfo = CalculateVirtualRecordCount();
            TotalPages = countInfo.PageCount;
            TotalCount = countInfo.RecordCount;

            // Validate the page number (ensures CurrentPageIndex is valid or -1)
            ValidatePageIndex();
            if (CurrentPageIndex == -1)
            {
                return;
            }

            // Prepare and run the command
            var cmd = PrepareCommand(countInfo);
            if (cmd == null)
            {
                return;
            }
            var adapter = SqlUtils.GetIDbDataAdapter();
            adapter.SelectCommand = cmd;
            //IDbDataAdapter adapter = SqlUtils.GetIDbDataAdapter(ADOType);
            var data = new DataTable();
            //adapter.Fill(data);
            SqlUtils.FillDataAdapterWithDataTable(adapter, data);

            // Configures the paged data source component
            if (PagedDataSource == null)
            {
                PagedDataSource = new PagedDataSource();
            }
            PagedDataSource.AllowCustomPaging = true;
            PagedDataSource.AllowPaging = true;
            PagedDataSource.CurrentPageIndex = 0;
            PagedDataSource.PageSize = ItemsPerPage;
            PagedDataSource.VirtualCount = countInfo.RecordCount;
            PagedDataSource.DataSource = data.DefaultView;
        }

        /// <summary>
        /// Strips ORDER-BY clauses from SelectCommand and adds a new one based
        /// on SortKeyField
        /// </summary>
        private void AdjustSelectCommand(bool addCustomSortInfo)
        {
            // Truncate where ORDER BY is found
            var temp = SelectCommand.ToLower();
            var pos = temp.IndexOf("order by", StringComparison.Ordinal);
            if (pos > -1)
                SelectCommand = SelectCommand.Substring(0, pos);

            // Add new ORDER BY info if SortKeyField is specified
            if (SortField != "" && addCustomSortInfo)
                SelectCommand += " ORDER BY " + SortField;
        }

        /// <summary>
        /// Calculates record and page count for the specified query
        /// </summary>
        private VirtualRecordCount CalculateVirtualRecordCount()
        {
            var count = new VirtualRecordCount
            {
                RecordCount = IsQueryTotalCount ? GetQueryVirtualCount() : TotalCount,
                RecordsInLastPage = ItemsPerPage
            };

            // Calculate the virtual number of records from the query

            // Calculate the correspondent number of pages
            var lastPage = count.RecordCount / ItemsPerPage;
            var remainder = count.RecordCount % ItemsPerPage;
            if (remainder > 0)
                lastPage++;
            count.PageCount = lastPage;

            // Calculate the number of items in the last page
            if (remainder > 0)
                count.RecordsInLastPage = remainder;
            return count;
        }

        /// <summary>
        /// Prepares and returns the command object for the reader-based query
        /// </summary>
        private IDbCommand PrepareCommand(VirtualRecordCount countInfo)
        {
            // Determines how many records are to be retrieved.
            // The last page could require less than other pages
            var recsToRetrieve = ItemsPerPage;
            if (CurrentPageIndex == countInfo.PageCount - 1)
                recsToRetrieve = countInfo.RecordsInLastPage;

            var cmdText = GetQueryPageCommandText(recsToRetrieve);

            var conn = SqlUtils.GetIDbConnection(WebConfigUtils.IsMySql, WebConfigUtils.ConnectionString);
            var cmd = SqlUtils.GetIDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            return cmd;
        }

        private static SortMode AlterSortMode(SortMode mode)
        {
            mode = mode == SortMode.Desc ? SortMode.Asc : SortMode.Desc;
            return mode;
        }

        /// <summary>
        /// Run a query to get the record count
        /// </summary>
        private int GetQueryVirtualCount()
        {
            var sqlString = GetQueryCountCommandText();

            var recCount = BaiRongDataProvider.DatabaseDao.GetIntResult(WebConfigUtils.ConnectionString, sqlString);

            return recCount;
        }
    }
}