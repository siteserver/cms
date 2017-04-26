using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Controls
{
    [DefaultProperty("SelectCommand")]
    [DefaultEvent("PageIndexChanged")]
    [ToolboxData("<{0}:SqlCountPager runat=\"server\" />")]
    public class SqlCountPager : Table, INamingContainer
    {
        private PagedDataSource _dataSource;
        private Control _controlToPaginate;
        private string CacheKeyName => Page.Request.FilePath + "_" + UniqueID + "_Data";
        public const string ParmPage = "page";
        public ArrayList RemoveQueryString = new ArrayList();

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
                else
                {
                    return $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {ItemsPerPage * (CurrentPageIndex + 1)} * FROM ({SelectCommand}) AS t0 {OrderByString}
    ) AS t1 {orderByString2}
) AS t2 {OrderByString}";
                }
            }
            else
            {
                if (WebConfigUtils.IsMySql)
                {
                    return $@"
SELECT * FROM (
    SELECT * FROM (
        SELECT * FROM ({SelectCommand}) AS t0 ORDER BY {SortField} {SortMode} LIMIT {ItemsPerPage * (CurrentPageIndex + 1)}
    ) AS t1 ORDER BY {SortField} {AlterSortMode(SortMode)} LIMIT {recsToRetrieve}
) AS t2 ORDER BY {SortField} {SortMode}";
                }
                else
                {
                    return $@"
SELECT * FROM (
    SELECT TOP {recsToRetrieve} * FROM (
        SELECT TOP {ItemsPerPage * (CurrentPageIndex + 1)} * FROM ({SelectCommand}) AS t0 ORDER BY {SortField} {SortMode}
    ) AS t1 ORDER BY {SortField} {AlterSortMode(SortMode)}
) AS t2 ORDER BY {SortField} {SortMode}";
                }
            }
        }

        public SqlCountPager()
        {
            _dataSource = null;
            _controlToPaginate = null;

            PagingMode = PagingMode.NonCached;
            PagerStyle = PagerStyle.NextPrev;
            CurrentPageIndex = 0;
            SelectCommand = "";
            ItemsPerPage = 10;
            TotalPages = -1;
            CacheDuration = 60;
            SortMode = SortMode.DESC;
        }

        /// <summary>
        /// Removes any data cached for paging
        /// </summary>
        public void ClearCache()
        {
            if (PagingMode == PagingMode.Cached)
                Page.Cache.Remove(CacheKeyName);
        }

        [Description("Gets and sets for how many seconds the data should stay in the cache")]
        public int CacheDuration
        {
            get { return Convert.ToInt32(ViewState["CacheDuration"]); }
            set { ViewState["CacheDuration"] = value; }
        }

        [Description("Indicates whether the data are retrieved page by page or can be cached")]
        public PagingMode PagingMode
        {
            get { return (PagingMode)ViewState["PagingMode"]; }
            set { ViewState["PagingMode"] = value; }
        }

        [Description("Indicates the style of the pager's navigation bar")]
        public PagerStyle PagerStyle
        {
            get { return (PagerStyle)ViewState["PagerStyle"]; }
            set { ViewState["PagerStyle"] = value; }
        }

        [Description("Gets and sets the name of the control to paginate")]
        public Control ControlToPaginate
        {
            get { return _controlToPaginate; }
            set { _controlToPaginate = value; }
        }

        [Description("Gets and sets the number of items to display per page")]
        public int ItemsPerPage
        {
            get { return Convert.ToInt32(ViewState["ItemsPerPage"]); }
            set { ViewState["ItemsPerPage"] = value; }
        }

        [Description("Gets and sets the index of the currently displayed page")]
        public int CurrentPageIndex
        {
            get { return Convert.ToInt32(ViewState["CurrentPageIndex"]); }
            set { ViewState["CurrentPageIndex"] = value; }
        }

        [Description("Gets and sets the SQL query to get data")]
        public string SelectCommand
        {
            get { return Convert.ToString(ViewState["SelectCommand"]); }
            set { ViewState["SelectCommand"] = value; }
        }

        public string OrderByString
        {
            get { return Convert.ToString(ViewState["OrderByString"]); }
            set { ViewState["OrderByString"] = value; }
        }

        [Description("Gets and sets the sort-by field. It is mandatory in NonCached mode.)")]
        public string SortField
        {
            get { return Convert.ToString(ViewState["SortKeyField"]); }
            set { ViewState["SortKeyField"] = value; }
        }

        [Description("Gets and sets the Unit.)")]
        public string Unit
        {
            get { return Convert.ToString(ViewState["Unit"]); }
            set { ViewState["Unit"] = value; }
        }

        [Description("取得设置排序模式")]
        public SortMode SortMode
        {
            get { return (SortMode)ViewState["SortMode"]; }
            set { ViewState["SortMode"] = value; }
        }

        public string FirstText
        {
            get
            {
                var text = ViewState["FirstText"] as string;
                if (string.IsNullOrEmpty(text))
                {
                    text = "首页";
                }
                return text;
            }
            set { ViewState["FirstText"] = value; }
        }

        public string LastText
        {
            get
            {
                var text = ViewState["LastText"] as string;
                if (string.IsNullOrEmpty(text))
                {
                    text = "末页";
                }
                return text;
            }
            set { ViewState["LastText"] = value; }
        }

        public string PrevText
        {
            get
            {
                var text = ViewState["PrevText"] as string;
                if (string.IsNullOrEmpty(text))
                {
                    text = "上一页";
                }
                return text;
            }
            set { ViewState["PrevText"] = value; }
        }

        public string NextText
        {
            get
            {
                var text = ViewState["NextText"] as string;
                if (string.IsNullOrEmpty(text))
                {
                    text = "下一页";
                }
                return text;
            }
            set { ViewState["NextText"] = value; }
        }

        public string CurrentPageText
        {
            get { return ViewState["CurrentPageText"] as string; }
            set { ViewState["CurrentPageText"] = value; }
        }

        public string EnabledCssClass
        {
            get { return ViewState["EnabledCssClass"] as string; }
            set { ViewState["EnabledCssClass"] = value; }
        }

        public new string DisabledCssClass
        {
            get { return ViewState["DisabledCssClass"] as string; }
            set { ViewState["DisabledCssClass"] = value; }
        }

        public string TextCssClass
        {
            get { return ViewState["TextCssClass"] as string; }
            set { ViewState["TextCssClass"] = value; }
        }

        /// <summary>
        /// Gets the number of displayable pages 
        /// </summary>
        [Browsable(false)]
        public int PageCount => TotalPages;

        /// <summary>
        /// Gets and sets the number of pages to display 
        /// </summary>
        protected int TotalPages
        {
            get { return Convert.ToInt32(ViewState["TotalPages"]); }
            set { ViewState["TotalPages"] = value; }
        }

        /// <summary>
        /// Gets and sets the number of pages to display 
        /// </summary>
        public int TotalCount
        {
            get { return Convert.ToInt32(ViewState["TotalCount"]); }
            set { ViewState["TotalCount"] = value; }
        }

        /// <summary>
        /// Fetches and stores the data
        /// </summary>
        public override void DataBind()
        {
            CurrentPageIndex = TranslateUtils.ToInt(Page.Request.QueryString[ParmPage], 1) - 1;

            base.DataBind();

            // Controls must be recreated after data binding 
            ChildControlsCreated = false;

            // Ensures the control exists and is a list control
            if (_controlToPaginate == null)
                return;
            if (!(_controlToPaginate is BaseDataList || _controlToPaginate is Repeater || _controlToPaginate is ListControl))
                return;

            // Ensures enough info to connect and query is specified
            if (SelectCommand == "")
                return;

            // Fetch data
            if (PagingMode == PagingMode.Cached)
            {
                FetchAllData();
            }
            else
            {
                FetchPageData();
            }

            // Bind data to the buddy control
            if (_controlToPaginate is BaseDataList)
            {
                var baseDataListControl = (BaseDataList)_controlToPaginate;
                baseDataListControl.DataSource = _dataSource;
                baseDataListControl.DataBind();
            }
            else if (_controlToPaginate is Repeater)
            {
                var baseRepeaterControl = (Repeater)_controlToPaginate;
                baseRepeaterControl.DataSource = _dataSource;
                baseRepeaterControl.DataBind();
            }
            else if (_controlToPaginate is ListControl)
            {
                var listControl = (ListControl)_controlToPaginate;
                listControl.Items.Clear();
                listControl.DataSource = _dataSource;
                listControl.DataBind();
            }
        }

        /// <summary>
        /// Writes the content to be rendered on the client
        /// </summary>
        protected override void Render(HtmlTextWriter output)
        {
            // If in design-mode ensure that child controls have been created.
            // Child controls are not created at this time in design-mode because
            // there's no pre-render stage. Do so for composite controls like this 
            if (Site != null && Site.DesignMode)
                CreateChildControls();

            base.Render(output);
        }

        /// <summary>
        /// Outputs the HTML markup for the control
        /// </summary>
        protected override void CreateChildControls()
        {
            Controls.Clear();
            ClearChildViewState();

            BuildControlHierarchy();
        }

        /// <summary>
        /// Control the building of the control's hierarchy
        /// </summary>
        private void BuildControlHierarchy()
        {
            if (TotalPages > 1)
            {
                // Build the surrounding table (one row, two cells)

                // Build the table row
                var row = new TableRow {Height = 25};
                Rows.Add(row);
                //t.Rows.Add(row);

                // Build the cell with navigation bar
                var cellNavBar = new TableCell {VerticalAlign = VerticalAlign.Middle};
                if (PagerStyle == PagerStyle.NextPrev)
                {
                    BuildNextPrevUi(cellNavBar);
                    row.Cells.Add(cellNavBar);
                    // Build the cell with the page index
                    var cellPageDesc = new TableCell();
                    if (!string.IsNullOrEmpty(TextCssClass))
                    {
                        cellPageDesc.CssClass = TextCssClass;
                    }
                    cellPageDesc.HorizontalAlign = HorizontalAlign.Right;
                    cellPageDesc.VerticalAlign = VerticalAlign.Top;
                    BuildCurrentPage(cellPageDesc);
                    row.Cells.Add(cellPageDesc);
                }
                else
                {
                    row.Cells.Add(cellNavBar);
                }
            }
        }

        private string GetNavigationUrl(int page)
        {
            var queryString = new NameValueCollection(Page.Request.QueryString);
            if (page > 1)
            {
                queryString[ParmPage] = page.ToString();
            }
            else
            {
                queryString.Remove(ParmPage);
            }
            if (RemoveQueryString.Count > 0)
            {
                foreach (string name in RemoveQueryString)
                {
                    queryString.Remove(name);
                }
            }
            return PageUtils.AddQueryString(PageUtils.GetUrlWithoutQueryString(Page.Request.RawUrl), queryString);
        }

        /// <summary>
        /// Generates the HTML markup for the Next/Prev navigation bar
        /// </summary>
        /// <param name="cell"></param>
        private void BuildNextPrevUi(TableCell cell)
        {
            var isValidPage = (CurrentPageIndex >= 0 && CurrentPageIndex <= TotalPages - 1);
            var canMoveBack = (CurrentPageIndex > 0);
            var canMoveForward = (CurrentPageIndex < TotalPages - 1);

            // 首页
            var enabled = isValidPage && canMoveBack;
            var firstImage = new Image {ToolTip = FirstText};
            var firstText = new Label {Text = FirstText};

            if (enabled)
            {
                firstImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.First);

                var link = new HyperLink();
                link.Style.Add("text-decoration", "none");
                link.NavigateUrl = GetNavigationUrl(1);
                if (!string.IsNullOrEmpty(EnabledCssClass))
                {
                    link.CssClass = EnabledCssClass;
                }
                link.Controls.Add(firstImage);
                link.Controls.Add(new LiteralControl("&nbsp;"));
                link.Controls.Add(firstText);
                cell.Controls.Add(link);
            }
            else
            {
                firstImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.FirstDisabled);
                cell.Controls.Add(firstImage);
                cell.Controls.Add(new LiteralControl("&nbsp;"));
                if (!string.IsNullOrEmpty(DisabledCssClass))
                {
                    firstText.CssClass = DisabledCssClass;
                }
                else
                {
                    firstText.Style.Add("color", "gray");
                }
                cell.Controls.Add(firstText);
            }

            cell.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

            // 上一页
            var prevImage = new Image {ToolTip = PrevText};
            var prevText = new Label {Text = PrevText};

            if (enabled)
            {
                prevImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.Previous);

                var link = new HyperLink();
                link.Style.Add("text-decoration", "none");
                link.NavigateUrl = GetNavigationUrl(CurrentPageIndex);
                if (!string.IsNullOrEmpty(EnabledCssClass))
                {
                    link.CssClass = EnabledCssClass;
                }
                link.Controls.Add(prevImage);
                link.Controls.Add(new LiteralControl("&nbsp;"));
                link.Controls.Add(prevText);
                cell.Controls.Add(link);
            }
            else
            {
                prevImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.PreviousDisabled);
                cell.Controls.Add(prevImage);
                cell.Controls.Add(new LiteralControl("&nbsp;"));
                if (!string.IsNullOrEmpty(DisabledCssClass))
                {
                    prevText.CssClass = DisabledCssClass;
                }
                else
                {
                    prevText.Style.Add("color", "gray");
                }
                cell.Controls.Add(prevText);
            }

            cell.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

            // 下一页
            enabled = isValidPage && canMoveForward;
            var nextImage = new Image {ToolTip = NextText};
            var nextText = new Label {Text = NextText};

            if (enabled)
            {
                nextImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.Next);

                var link = new HyperLink();
                link.Style.Add("text-decoration", "none");
                link.NavigateUrl = GetNavigationUrl(CurrentPageIndex + 2);
                if (!string.IsNullOrEmpty(EnabledCssClass))
                {
                    link.CssClass = EnabledCssClass;
                }
                link.Controls.Add(nextImage);
                link.Controls.Add(new LiteralControl("&nbsp;"));
                link.Controls.Add(nextText);
                cell.Controls.Add(link);
            }
            else
            {
                nextImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.NextDisabled);

                cell.Controls.Add(nextImage);
                cell.Controls.Add(new LiteralControl("&nbsp;"));
                if (!string.IsNullOrEmpty(DisabledCssClass))
                {
                    nextText.CssClass = DisabledCssClass;
                }
                else
                {
                    nextText.Style.Add("color", "gray");
                }
                cell.Controls.Add(nextText);
            }

            cell.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

            // 末页
            var lastImage = new Image {ToolTip = LastText};
            var lastText = new Label {Text = LastText};

            if (enabled)
            {
                lastImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.Last);

                var link = new HyperLink();
                link.Style.Add("text-decoration", "none");
                link.NavigateUrl = GetNavigationUrl(TotalPages);
                if (!string.IsNullOrEmpty(EnabledCssClass))
                {
                    link.CssClass = EnabledCssClass;
                }
                link.Controls.Add(lastImage);
                link.Controls.Add(new LiteralControl("&nbsp;"));
                link.Controls.Add(lastText);
                cell.Controls.Add(link);
            }
            else
            {
                lastImage.ImageUrl = SiteServerAssets.GetUrl(SiteServerAssets.Arrow.LastDisabled);

                cell.Controls.Add(lastImage);
                cell.Controls.Add(new LiteralControl("&nbsp;"));
                if (!string.IsNullOrEmpty(DisabledCssClass))
                {
                    lastText.CssClass = DisabledCssClass;
                }
                else
                {
                    lastText.Style.Add("color", "gray");
                }
                cell.Controls.Add(lastText);
            }

            //显示总记录数
            if (TotalPages > 0)
            {

                cell.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                var totalPagesText = new Label();
                lastText.Text = $@" ( 共{TotalCount}条 )";
                cell.Controls.Add(totalPagesText);
            }
        }

        // ***********************************************************************

        /// <summary>
        /// Generates the HTML markup to describe the current page (0-based)
        /// </summary>
        private void BuildCurrentPage(TableCell cell)
        {
            var text = new NoTagText
            {
                ID = "Text",
                Text = CurrentPageText
            };
            cell.Controls.Add(text);
            // Render a drop-down list  
            var pageList = new DropDownList
            {
                ID = "PageList",
                AutoPostBack = true
            };
            pageList.SelectedIndexChanged += PageList_Click;
            pageList.Font.Name = Font.Name;
            pageList.Font.Size = Font.Size;
            pageList.ForeColor = ForeColor;
            pageList.CssClass = "input-medium";

            // Embellish the list when there are no pages to list 
            if (TotalPages <= 0 || CurrentPageIndex == -1)
            {
                pageList.Items.Add("1 / 1");
                pageList.Enabled = false;
                pageList.SelectedIndex = 0;
            }
            else // Populate the list
            {
                for (var i = 1; i <= TotalPages; i++)
                {
                    var item = new ListItem($"{i} / {TotalPages}", (i - 1).ToString());
                    pageList.Items.Add(item);
                }
                pageList.SelectedIndex = CurrentPageIndex;
            }
            cell.CssClass = "align-right";
            cell.Controls.Add(pageList);
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
        /// Runs the query for all data to be paged and caches the resulting data
        /// </summary>
        private void FetchAllData()
        {
            // Looks for data in the ASP.NET Cache
            var data = (DataTable)Page.Cache[CacheKeyName];
            if (data == null)
            {
                // Fix SelectCommand with order-by info
                AdjustSelectCommand(true);

                // If data expired or has never been fetched, go to the database
                //SqlDataAdapter adapter = new SqlDataAdapter(SelectCommand, ConnectionString);
                var adapter = SqlUtils.GetIDbDataAdapter(SelectCommand, WebConfigUtils.ConnectionString);
                data = new DataTable();
                //adapter.Fill(data);
                SqlUtils.FillDataAdapterWithDataTable(adapter, data);
                Page.Cache.Insert(CacheKeyName, data, null,
                    DateTime.Now.AddSeconds(CacheDuration),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }

            // Configures the paged data source component
            if (_dataSource == null)
                _dataSource = new PagedDataSource();
            _dataSource.DataSource = data.DefaultView; // must be IEnumerable!
            _dataSource.AllowPaging = true;
            _dataSource.PageSize = ItemsPerPage;
            TotalPages = _dataSource.PageCount;

            // Ensures the page index is valid 
            ValidatePageIndex();
            if (CurrentPageIndex == -1)
            {
                _dataSource = null;
                return;
            }

            // Selects the page to view
            _dataSource.CurrentPageIndex = CurrentPageIndex;
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
                return;

            // Prepare and run the command
            var cmd = PrepareCommand(countInfo);
            if (cmd == null)
                return;
            //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            var adapter = SqlUtils.GetIDbDataAdapter();
            adapter.SelectCommand = cmd;
            var data = new DataTable();
            //adapter.Fill(data);
            SqlUtils.FillDataAdapterWithDataTable(adapter, data);

            // Configures the paged data source component
            if (_dataSource == null)
                _dataSource = new PagedDataSource();
            _dataSource.AllowCustomPaging = true;
            _dataSource.AllowPaging = true;
            _dataSource.CurrentPageIndex = 0;
            _dataSource.PageSize = ItemsPerPage;
            _dataSource.VirtualCount = countInfo.RecordCount;
            _dataSource.DataSource = data.DefaultView;
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
                RecordCount = GetQueryVirtualCount(),
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

        /// <summary>
        /// 方法 反转排序模式
        /// </summary>
        /// <param name="mode">排序模式</param>
        /// <returns>相反的排序模式</returns>
        private static SortMode AlterSortMode(SortMode mode)
        {
            return mode == SortMode.DESC ? SortMode.ASC : SortMode.DESC;
        }

        /// <summary>
        /// Run a query to get the record count
        /// </summary>
        private int GetQueryVirtualCount()
        {
            var cmdText = GetQueryCountCommandText();

            var recCount = BaiRongDataProvider.DatabaseDao.GetIntResult(WebConfigUtils.ConnectionString, cmdText);
            //            SqlConnection conn = new SqlConnection(ConnectionString);
            //            SqlCommand cmd = new SqlCommand(cmdText, conn);
            //IDbConnection conn = SqlUtils.GetIDbConnection(BaiRongDataProvider.ADOType, ConnectionString);
            //IDbCommand cmd = SqlUtils.GetIDbCommand(BaiRongDataProvider.ADOType);
            //cmd.Connection = conn;
            //cmd.CommandText = cmdText;

            //cmd.Connection.Open();
            //int recCount = (int)cmd.ExecuteScalar();
            //cmd.Connection.Close();

            return recCount;
        }

        /// <summary>
        /// Sets the current page index
        /// </summary>
        //private void GoToPage(int pageIndex)
        //{
        //    // Prepares event data
        //    PageChangedEventArgs e = new PageChangedEventArgs();
        //    e.OldPageIndex = CurrentPageIndex;
        //    e.NewPageIndex = pageIndex;

        //    // Updates the current index
        //    CurrentPageIndex = pageIndex;

        //    // Fires the page changed event
        //    OnPageIndexChanged(e);

        //    // Binds new data
        //    DataBind();
        //}

        /// <summary>
        /// Event handler for any page selected from the drop-down page list 
        /// </summary>
        private void PageList_Click(object sender, EventArgs e)
        {
            var pageList = (DropDownList)sender;
            var pageIndex = Convert.ToInt32(pageList.SelectedValue);
            PageUtils.Redirect(GetNavigationUrl(pageIndex + 1));
        }

        ///// <summary>
        ///// Event handler for any page selected from the drop-down page list 
        ///// </summary>
        //private void Numeric_Click(object sender, EventArgs e)
        //{
        //    LinkButton linkButton = (LinkButton)sender;
        //    int pageIndex = Convert.ToInt32(linkButton.CommandArgument);
        //    GoToPage(pageIndex);
        //}
    }
}
