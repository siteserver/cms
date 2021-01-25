using System;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "翻页数据库列表", Description = "通过 stl:pageSqlContents 标签在模板中显示能够翻页的数据库列表")]
    public class StlPageSqlContents : StlSqlContents
    {
        public new const string ElementName = "stl:pageSqlContents";

        [StlAttribute(Title = "每页显示的内容数目")]
        public const string PageNum = nameof(PageNum);

        [StlAttribute(Title = "翻页中生成的静态页面最大数，剩余页面将动态获取")]
        public const string MaxPage = nameof(MaxPage);

        private string StlPageSqlContentsElement { get; set; }
        private IParseManager ParseManager { get; set; }
        private ListInfo ListInfo { get; set; }
        private string SqlString { get; set; }
        //private readonly DataSet _dataSet;

        public static async Task<StlPageSqlContents> GetAsync(string stlPageSqlContentsElement, IParseManager parseManager)
        {
            var stlPageSqlContents = new StlPageSqlContents
            {
                StlPageSqlContentsElement = stlPageSqlContentsElement,
                ParseManager = parseManager
            };

            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlPageSqlContentsElement, -1);

                parseManager.ContextInfo = parseManager.ContextInfo.Clone(ElementName, stlPageSqlContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes, stlElementInfo.StartIndex);

                stlPageSqlContents.ListInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.SqlContent);

                stlPageSqlContents.SqlString = stlPageSqlContents.ListInfo.QueryString;
                if (string.IsNullOrWhiteSpace(stlPageSqlContents.ListInfo.Order))
                {
                    var pos = stlPageSqlContents.SqlString.LastIndexOf(" ORDER BY ", StringComparison.OrdinalIgnoreCase);
                    if (pos > -1)
                    {
                        stlPageSqlContents.SqlString = stlPageSqlContents.SqlString.Substring(0, pos);
                        stlPageSqlContents.ListInfo.Order = stlPageSqlContents.SqlString.Substring(pos);
                    }
                }
                else
                {
                    if (stlPageSqlContents.ListInfo.Order.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        stlPageSqlContents.ListInfo.Order = $"ORDER BY {stlPageSqlContents.ListInfo.Order}";
                    }
                }

                //_dataSet = StlDataUtility.GetPageSqlContentsDataSet(_listInfo.ConnectionString, _listInfo.QueryString, _listInfo.StartNum, _listInfo.PageNum, _listInfo.OrderByString);
            }
            catch (Exception ex)
            {
                await parseManager.AddStlErrorLogAsync(ElementName, stlPageSqlContentsElement, ex);
                stlPageSqlContents.ListInfo = new ListInfo();
            }

            return stlPageSqlContents;
        }

        public int GetPageCount(out int totalNum)
        {
            totalNum = 0;
            var pageCount = 1;
            try
            {
                //totalNum = DatabaseApi.Instance.GetPageTotalCount(SqlString);
                totalNum = ParseManager.DatabaseManager.GetPageTotalCount(SqlString);
                if (ListInfo.PageNum != 0 && ListInfo.PageNum < totalNum)//需要翻页
                {
                    pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(ListInfo.PageNum)));//需要生成的总页数
                }
            }
            catch
            {
                // ignored
            }
            return pageCount;
        }

        public async Task<string> ParseAsync(int totalNum, int currentPageIndex, int pageCount, bool isStatic)
        {
            var pageInfo = ParseManager.PageInfo;

            if (isStatic)
            {
                var maxPage = ListInfo.MaxPage;
                if (maxPage == 0)
                {
                    maxPage = pageInfo.Site.CreateStaticMaxPage;
                }
                if (maxPage > 0 && currentPageIndex + 1 > maxPage)
                {
                    return await ParseDynamicAsync(totalNum, currentPageIndex, pageCount);
                }
            }

            var parsedContent = string.Empty;

            ParseManager.ContextInfo.PageItemIndex = currentPageIndex * ListInfo.PageNum;

            try
            {
                if (!string.IsNullOrEmpty(SqlString))
                {
                    //var pageSqlString = DatabaseApi.Instance.GetPageSqlString(SqlString, ListInfo.OrderByString, totalNum, ListInfo.PageNum, currentPageIndex);
                    var pageSqlString = ParseManager.DatabaseManager.GetStlPageSqlString(SqlString, ListInfo.Order, totalNum, ListInfo.PageNum, currentPageIndex);

                    var dataSource = await ParseManager.DatabaseManager.ParserGetSqlDataSourceAsync(ListInfo.DatabaseType, ListInfo.ConnectionString, pageSqlString);

                    parsedContent = await ParseElementAsync(ParseManager, ListInfo, dataSource);
                }

                //if (_dataSet != null)
                //{
                //    var dataSource = new PagedDataSource { DataSource = _dataSet.Tables[0].DefaultView }; //分页类

                //    if (pageCount > 1)
                //    {
                //        dataSource.AllowPaging = true;
                //        dataSource.PageSize = _listInfo.PageNum;//每页显示的项数
                //    }
                //    else
                //    {
                //        dataSource.AllowPaging = false;
                //    }

                //    dataSource.CurrentPageIndex = currentPageIndex;//当前页的索引

                //    if (_listInfo.Layout == ELayout.None)
                //    {
                //        var rptContents = new Repeater
                //        {
                //            ItemTemplate =
                //                new RepeaterTemplate(_listInfo.ItemTemplate, _listInfo.SelectedItems,
                //                    _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate,
                //                    _listInfo.SeparatorRepeat, _pageInfo, EContextType.SqlContent, _contextInfo)
                //        };

                //        if (!string.IsNullOrEmpty(_listInfo.HeaderTemplate))
                //        {
                //            rptContents.HeaderTemplate = new SeparatorTemplate(_listInfo.HeaderTemplate);
                //        }
                //        if (!string.IsNullOrEmpty(_listInfo.FooterTemplate))
                //        {
                //            rptContents.FooterTemplate = new SeparatorTemplate(_listInfo.FooterTemplate);
                //        }
                //        if (!string.IsNullOrEmpty(_listInfo.SeparatorTemplate))
                //        {
                //            rptContents.SeparatorTemplate = new SeparatorTemplate(_listInfo.SeparatorTemplate);
                //        }
                //        if (!string.IsNullOrEmpty(_listInfo.AlternatingItemTemplate))
                //        {
                //            rptContents.AlternatingItemTemplate = new RepeaterTemplate(_listInfo.AlternatingItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, EContextType.SqlContent, _contextInfo);
                //        }

                //        rptContents.DataSource = dataSource;
                //        rptContents.DataBind();

                //        if (rptContents.Items.Count > 0)
                //        {
                //            parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                //        }
                //    }
                //    else
                //    {
                //        var pdlContents = new ParsedDataList();

                //        //设置显示属性
                //        TemplateUtility.PutListInfoToMyDataList(pdlContents, _listInfo);

                //        pdlContents.ItemTemplate = new DataListTemplate(_listInfo.ItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, EContextType.SqlContent, _contextInfo);
                //        if (!string.IsNullOrEmpty(_listInfo.HeaderTemplate))
                //        {
                //            pdlContents.HeaderTemplate = new SeparatorTemplate(_listInfo.HeaderTemplate);
                //        }
                //        if (!string.IsNullOrEmpty(_listInfo.FooterTemplate))
                //        {
                //            pdlContents.FooterTemplate = new SeparatorTemplate(_listInfo.FooterTemplate);
                //        }
                //        if (!string.IsNullOrEmpty(_listInfo.SeparatorTemplate))
                //        {
                //            pdlContents.SeparatorTemplate = new SeparatorTemplate(_listInfo.SeparatorTemplate);
                //        }
                //        if (!string.IsNullOrEmpty(_listInfo.AlternatingItemTemplate))
                //        {
                //            pdlContents.AlternatingItemTemplate = new DataListTemplate(_listInfo.AlternatingItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, EContextType.SqlContent, _contextInfo);
                //        }

                //        pdlContents.DataSource = dataSource;
                //        pdlContents.DataBind();

                //        if (pdlContents.Items.Count > 0)
                //        {
                //            parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                parsedContent = await ParseManager.AddStlErrorLogAsync(ElementName, StlPageSqlContentsElement, ex);
            }

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            ParseManager.ContextInfo.PageItemIndex = 0;

            return parsedContent;
        }

        private async Task<string> ParseDynamicAsync(int totalNum, int currentPageIndex, int pageCount)
        {
            var pageInfo = ParseManager.PageInfo;

            var loading = ListInfo.LoadingTemplate;
            if (string.IsNullOrEmpty(loading))
            {
                loading = @"<div style=""margin: 0 auto;
    padding: 40px 0;
    font-size: 14px;
    font-family: 'Microsoft YaHei';
    text-align: center;
    font-weight: 400;"">
        载入中，请稍后...
</div>";
            }

            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);

            var elementId = StringUtils.GetElementId();
            var apiUrl = ParseManager.PathManager.GetPageContentsApiUrl(pageInfo.Site);
            var apiParameters = ParseManager.PathManager.GetPageContentsApiParameters(pageInfo.SiteId, pageInfo.PageChannelId, pageInfo.Template.Id, totalNum, pageCount, currentPageIndex, StlPageSqlContentsElement);

            var builder = new StringBuilder();
            builder.Append($@"<div id=""{elementId}"">");
            builder.Append($@"<div class=""loading"">{loading}</div>");
            builder.Append($@"<div class=""yes"">{string.Empty}</div>");
            builder.Append("</div>");

            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
$(document).ready(function(){{
    $(""#{elementId} .loading"").show();
    $(""#{elementId} .yes"").hide();

    var url = '{apiUrl}';
    var parameters = {apiParameters};

    $.support.cors = true;
    $.ajax({{
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(parameters),
        dataType: 'json',
        success: function(res) {{
            $(""#{elementId} .loading"").hide();
            $(""#{elementId} .yes"").show();
            $(""#{elementId} .yes"").html(res);
        }},
        error: function(e) {{
            $(""#{elementId} .loading"").hide();
            $(""#{elementId} .yes"").hide();
        }}
    }});
}});
</script>
");

            return builder.ToString();
        }
    }

}
