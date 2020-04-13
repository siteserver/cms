using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

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
        public ListInfo ListInfo { get; set; }
        private List<KeyValuePair<int, Content>> DataSource { get; set; }

        public static async Task<StlPageContents> GetAsync(string stlPageContentsElement, IParseManager parseManager)
        {
            var stlPageContents = new StlPageContents
            {
                StlPageContentsElement = stlPageContentsElement,
                ParseManager = parseManager
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement);

            stlPageContents.ParseManager.ContextInfo = parseManager.ContextInfo.Clone(stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            stlPageContents.ListInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Content);

            //stlPageContents.SqlString = await StlDataUtility.GetStlPageContentsSqlStringAsync(stlPageContents.PageInfo.Site, channelId, stlPageContents.ListInfo);
            stlPageContents.DataSource = await GetContentsDataSourceAsync(parseManager, stlPageContents.ListInfo);

            return stlPageContents;
        }

        //API StlActionsSearchController调用
        public static async Task<StlPageContents> GetAsync(string stlPageContentsElement, IParseManager parseManager, int pageNum, string tableName, string where)
        {
            var stlPageContents = new StlPageContents
            {
                ParseManager = parseManager
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement);
            parseManager.ContextInfo = parseManager.ContextInfo.Clone(stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            stlPageContents.ListInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Content);

            stlPageContents.ListInfo.Scope = ScopeType.All;

            if (pageNum > 0)
            {
                stlPageContents.ListInfo.PageNum = pageNum;
            }

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

                parsedContent = await ParseElementAsync(ParseManager, ListInfo, pageChannelList);
            }

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            ParseManager.ContextInfo.PageItemIndex = 0;

            return parsedContent;
        }

        //public async Task<(int PageCount, int TotalNum)> GetPageCountAsync()
        //{
        //    var pageCount = 1;
        //    var totalNum = 0;
        //    try
        //    {
        //        //totalNum = GlobalSettings.DatabaseRepository.GetPageTotalCount(SqlString);
        //        totalNum = GlobalSettings.DatabaseRepository.GetPageTotalCount(SqlString);
        //        if (ListInfo.PageNum != 0 && ListInfo.PageNum < totalNum)//需要翻页
        //        {
        //            pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(ListInfo.PageNum)));//需要生成的总页数
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        await LogUtils.AddStlErrorLogAsync(PageInfo, ElementName, StlPageContentsElement, ex);
        //    }
        //    return (pageCount, totalNum);
        //}

        //public async Task<string> ParseAsync(int totalNum, int currentPageIndex, int pageCount, bool isStatic)
        //{
        //    if (isStatic)
        //    {
        //        var maxPage = ListInfo.MaxPage;
        //        if (maxPage == 0)
        //        {
        //            maxPage = PageInfo.Site.CreateStaticMaxPage;
        //        }
        //        if (maxPage > 0 && currentPageIndex + 1 > maxPage)
        //        {
        //            return await ParseDynamicAsync(totalNum, currentPageIndex, pageCount);
        //        }
        //    }

        //    var parsedContent = string.Empty;

        //    ContextInfo.PageItemIndex = currentPageIndex * ListInfo.PageNum;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(SqlString))
        //        {
        //            //var pageSqlString = GlobalSettings.DatabaseRepository.GetPageSqlString(SqlString, ListInfo.OrderByString, totalNum, ListInfo.PageNum, currentPageIndex);
        //            var pageSqlString = GlobalSettings.DatabaseRepository.GetStlPageSqlString(SqlString, ListInfo.Order, totalNum, ListInfo.PageNum, currentPageIndex);

        //            var datasource = GlobalSettings.DatabaseRepository.GetDataSource(pageSqlString);

        //            if (ListInfo.Layout == Model.Layout.None)
        //            {
        //                var rptContents = new Repeater();

        //                if (!string.IsNullOrEmpty(ListInfo.HeaderTemplate))
        //                {
        //                    rptContents.HeaderTemplate = new SeparatorTemplate(ListInfo.HeaderTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(ListInfo.FooterTemplate))
        //                {
        //                    rptContents.FooterTemplate = new SeparatorTemplate(ListInfo.FooterTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(ListInfo.SeparatorTemplate))
        //                {
        //                    rptContents.SeparatorTemplate = new SeparatorTemplate(ListInfo.SeparatorTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(ListInfo.AlternatingItemTemplate))
        //                {
        //                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, ContextType.Content, ContextInfo);
        //                }

        //                rptContents.ItemTemplate = new RepeaterTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, ContextType.Content, ContextInfo);

        //                rptContents.DataSource = datasource;
        //                rptContents.DataBind();

        //                if (rptContents.Items.Count > 0)
        //                {
        //                    parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
        //                }
        //            }
        //            else
        //            {
        //                var pdlContents = new ParsedDataList();

        //                //设置显示属性
        //                TemplateUtility.PutListInfoToMyDataList(pdlContents, ListInfo);

        //                pdlContents.ItemTemplate = new DataListTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, ContextType.Content, ContextInfo);
        //                if (!string.IsNullOrEmpty(ListInfo.HeaderTemplate))
        //                {
        //                    pdlContents.HeaderTemplate = new SeparatorTemplate(ListInfo.HeaderTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(ListInfo.FooterTemplate))
        //                {
        //                    pdlContents.FooterTemplate = new SeparatorTemplate(ListInfo.FooterTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(ListInfo.SeparatorTemplate))
        //                {
        //                    pdlContents.SeparatorTemplate = new SeparatorTemplate(ListInfo.SeparatorTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(ListInfo.AlternatingItemTemplate))
        //                {
        //                    pdlContents.AlternatingItemTemplate = new DataListTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, ContextType.Content, ContextInfo);
        //                }

        //                pdlContents.DataSource = datasource;
        //                pdlContents.DataKeyField = ContentAttribute.Id;
        //                pdlContents.DataBind();

        //                if (pdlContents.Items.Count > 0)
        //                {
        //                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        parsedContent = await LogUtils.AddStlErrorLogAsync(PageInfo, ElementName, StlPageContentsElement, ex);
        //    }

        //    //还原翻页为0，使得其他列表能够正确解析ItemIndex
        //    ContextInfo.PageItemIndex = 0;
        //    return parsedContent;
        //}

        private async Task<string> ParseDynamicAsync(int totalNum, int currentPageIndex, int pageCount)
        {
            var pageInfo = ParseManager.PageInfo;
            var contextInfo = ParseManager.ContextInfo;

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

            await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.Jquery);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
            var apiUrl = ParseManager.PathManager.GetPageContentsApiUrl(pageInfo.ApiUrl);
            var apiParameters = ParseManager.PathManager.GetPageContentsApiParameters(pageInfo.SiteId, pageInfo.PageChannelId, pageInfo.Template.Id, totalNum, pageCount, currentPageIndex, StlPageContentsElement);

            var builder = new StringBuilder();
            builder.Append($@"<div id=""{ajaxDivId}"">");
            builder.Append($@"<div class=""loading"">{loading}</div>");
            builder.Append($@"<div class=""yes"">{string.Empty}</div>");
            builder.Append("</div>");

            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
$(document).ready(function(){{
    $(""#{ajaxDivId} .loading"").show();
    $(""#{ajaxDivId} .yes"").hide();

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
            $(""#{ajaxDivId} .loading"").hide();
            $(""#{ajaxDivId} .yes"").show();
            $(""#{ajaxDivId} .yes"").html(res);
        }},
        error: function(e) {{
            $(""#{ajaxDivId} .loading"").hide();
            $(""#{ajaxDivId} .yes"").hide();
        }}
    }});
}});
</script>
");

            return builder.ToString();
        }
    }
}