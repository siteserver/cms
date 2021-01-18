using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlKata;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "翻页内容列表", Description = "通过 stl:pageContents 标签在模板中显示翻页内容列表")]
    public class StlPageContents : StlContents
    {
        public new const string ElementName = "stl:pageContents";

        [StlAttribute(Title = "每页显示的内容数目")]
        public const string PageNum = nameof(PageNum);
        
        [StlAttribute(Title = "翻页中生成的静态页面最大数，剩余页面将动态获取")]
        public const string MaxPage = nameof(MaxPage);

        private string StlPageContentsElement { get; set; }
        private IParseManager ParseManager { get; set; }
        private ListInfo ListInfo { get; set; }
        private List<KeyValuePair<int, Content>> DataSource { get; set; }

        public static async Task<StlPageContents> GetAsync(string stlPageContentsElement, IParseManager parseManager, Query query = null)
        {
            var stlPageContents = new StlPageContents
            {
                StlPageContentsElement = stlPageContentsElement,
                ParseManager = parseManager
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement, -1);

            stlPageContents.ParseManager.ContextInfo = parseManager.ContextInfo.Clone(ElementName, stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes, stlElementInfo.StartIndex);

            stlPageContents.ListInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Content, query);

            //stlPageContents.SqlString = await StlDataUtility.GetStlPageContentsSqlStringAsync(stlPageContents.PageInfo.Site, channelId, stlPageContents.ListInfo);
            stlPageContents.DataSource = await GetContentsDataSourceAsync(parseManager, stlPageContents.ListInfo);

            return stlPageContents;
        }

        //API StlActionsSearchController调用
        public static async Task<StlPageContents> GetByStlSearchAsync(string stlPageContentsElement, IParseManager parseManager, int pageNum, Query query)
        {
            var stlPageContents = new StlPageContents
            {
                ParseManager = parseManager
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement, -1);
            parseManager.ContextInfo = parseManager.ContextInfo.Clone(ElementName, stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes, stlElementInfo.StartIndex);

            stlPageContents.ListInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Content);

            stlPageContents.ListInfo.Scope = ScopeType.All;

            if (pageNum > 0)
            {
                stlPageContents.ListInfo.PageNum = pageNum;
            }

            stlPageContents.ListInfo.Query = query;

            stlPageContents.DataSource = await GetContentsDataSourceAsync(parseManager, stlPageContents.ListInfo);

            //stlPageContents.SqlString = StlDataUtility.GetPageContentsSqlStringBySearch(tableName, stlPageContents.ListInfo.GroupContent, stlPageContents.ListInfo.GroupContentNot, stlPageContents.ListInfo.Tags, stlPageContents.ListInfo.IsImageExists, stlPageContents.ListInfo.IsImage, stlPageContents.ListInfo.IsVideoExists, stlPageContents.ListInfo.IsVideo, stlPageContents.ListInfo.IsFileExists, stlPageContents.ListInfo.IsFile, stlPageContents.ListInfo.StartNum, stlPageContents.ListInfo.TotalNum, stlPageContents.ListInfo.Order, stlPageContents.ListInfo.IsTopExists, stlPageContents.ListInfo.IsTop, stlPageContents.ListInfo.IsRecommendExists, stlPageContents.ListInfo.IsRecommend, stlPageContents.ListInfo.IsHotExists, stlPageContents.ListInfo.IsHot, stlPageContents.ListInfo.IsColorExists, stlPageContents.ListInfo.IsColor, where);

            return stlPageContents;
        }

        public (int PageCount, int TotalNum) GetPageCount()
        {
            var pageCount = 1;
            var totalNum = 0;//数据库中实际的内容数目
            if (DataSource == null || DataSource.Count == 0) return (pageCount, totalNum);

            totalNum = DataSource.Count;
            if (ListInfo.PageNum != 0 && ListInfo.PageNum < totalNum)//需要翻页
            {
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(ListInfo.PageNum)));//需要生成的总页数
            }
            return (pageCount, totalNum);
        }

        public async Task<string> ParseAsync(int totalNum, int currentPageIndex, int pageCount, bool isStatic)
        {
            if (isStatic)
            {
                var maxPage = ListInfo.MaxPage;
                if (maxPage == 0)
                {
                    maxPage = ParseManager.PageInfo.Site.CreateStaticMaxPage;
                }
                if (maxPage > 0 && currentPageIndex + 1 > maxPage)
                {
                    return await ParseDynamicAsync(totalNum, currentPageIndex, pageCount);
                }
            }

            var parsedContent = string.Empty;

            ParseManager.ContextInfo.PageItemIndex = currentPageIndex * ListInfo.PageNum;

            if (DataSource != null && DataSource.Count > 0)
            {
                var pageChannelList = pageCount > 1
                    ? DataSource.Skip(ParseManager.ContextInfo.PageItemIndex).Take(ListInfo.PageNum).ToList()
                    : DataSource;

                parsedContent = await ParseAsync(ParseManager, ListInfo, pageChannelList);
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
            var apiParameters = ParseManager.PathManager.GetPageContentsApiParameters(pageInfo.SiteId, pageInfo.PageChannelId, pageInfo.Template.Id, totalNum, pageCount, currentPageIndex, StlPageContentsElement);

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
            $(""#{elementId} .yes"").html(res.html);
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