using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
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
        private PageInfo PageInfo { get; set; }
        private ContextInfo ContextInfo { get; set; }

        public static async Task<StlPageContents> GetAsync(string stlPageContentsElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var stlPageContents = new StlPageContents
            {
                StlPageContentsElement = stlPageContentsElement, 
                PageInfo = pageInfo, 
                ContextInfo = contextInfo
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement);

            stlPageContents.ContextInfo = contextInfo.Clone(stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            stlPageContents.ListInfo = await ListInfo.GetListInfoAsync(stlPageContents.PageInfo, stlPageContents.ContextInfo, EContextType.Content);

            var channelId = await StlDataUtility.GetChannelIdByLevelAsync(stlPageContents.PageInfo.SiteId, stlPageContents.ContextInfo.ChannelId, stlPageContents.ListInfo.UpLevel, stlPageContents.ListInfo.TopLevel);

            channelId = await StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(stlPageContents.PageInfo.SiteId, channelId, stlPageContents.ListInfo.ChannelIndex, stlPageContents.ListInfo.ChannelName);

            stlPageContents.SqlString = await StlDataUtility.GetStlPageContentsSqlStringAsync(stlPageContents.PageInfo.Site, channelId, stlPageContents.ListInfo);

            return stlPageContents;
        }

        //API StlActionsSearchController调用
        public static async Task<StlPageContents> GetAsync(string stlPageContentsElement, PageInfo pageInfo, ContextInfo contextInfo, int pageNum, string tableName, string whereString)
        {
            var stlPageContents = new StlPageContents
            {
                PageInfo = pageInfo, 
                ContextInfo = contextInfo
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement);
            stlPageContents.ContextInfo = contextInfo.Clone(stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            stlPageContents.ListInfo = await ListInfo.GetListInfoAsync(stlPageContents.PageInfo, stlPageContents.ContextInfo, EContextType.Content);

            stlPageContents.ListInfo.Scope = EScopeType.All;

            stlPageContents.ListInfo.Where += whereString;
            if (pageNum > 0)
            {
                stlPageContents.ListInfo.PageNum = pageNum;
            }

            stlPageContents.SqlString = StlDataUtility.GetPageContentsSqlStringBySearch(tableName, stlPageContents.ListInfo.GroupContent, stlPageContents.ListInfo.GroupContentNot, stlPageContents.ListInfo.Tags, stlPageContents.ListInfo.IsImageExists, stlPageContents.ListInfo.IsImage, stlPageContents.ListInfo.IsVideoExists, stlPageContents.ListInfo.IsVideo, stlPageContents.ListInfo.IsFileExists, stlPageContents.ListInfo.IsFile, stlPageContents.ListInfo.StartNum, stlPageContents.ListInfo.TotalNum, stlPageContents.ListInfo.OrderByString, stlPageContents.ListInfo.IsTopExists, stlPageContents.ListInfo.IsTop, stlPageContents.ListInfo.IsRecommendExists, stlPageContents.ListInfo.IsRecommend, stlPageContents.ListInfo.IsHotExists, stlPageContents.ListInfo.IsHot, stlPageContents.ListInfo.IsColorExists, stlPageContents.ListInfo.IsColor, stlPageContents.ListInfo.Where);

            return stlPageContents;
        }

        public async Task<(int PageCount, int TotalNum)> GetPageCountAsync()
        {
            var pageCount = 1;
            var totalNum = 0;
            try
            {
                //totalNum = DataProvider.DatabaseRepository.GetPageTotalCount(SqlString);
                totalNum = DataProvider.DatabaseRepository.GetPageTotalCount(SqlString);
                if (ListInfo.PageNum != 0 && ListInfo.PageNum < totalNum)//需要翻页
                {
                    pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(ListInfo.PageNum)));//需要生成的总页数
                }
            }
            catch(Exception ex)
            {
                await LogUtils.AddStlErrorLogAsync(PageInfo, ElementName, StlPageContentsElement, ex);
            }
            return (pageCount, totalNum);
        }

        public string SqlString { get; set; }

        public ListInfo ListInfo { get; set; }

        public async Task<string> ParseAsync(int totalNum, int currentPageIndex, int pageCount, bool isStatic)
        {
            if (isStatic)
            {
                var maxPage = ListInfo.MaxPage;
                if (maxPage == 0)
                {
                    maxPage = PageInfo.Site.CreateStaticMaxPage;
                }
                if (maxPage > 0 && currentPageIndex + 1 > maxPage)
                {
                    return ParseDynamic(totalNum, currentPageIndex, pageCount);
                }
            }

            var parsedContent = string.Empty;

            ContextInfo.PageItemIndex = currentPageIndex * ListInfo.PageNum;

            try
            {
                if (!string.IsNullOrEmpty(SqlString))
                {
                    //var pageSqlString = DataProvider.DatabaseRepository.GetPageSqlString(SqlString, ListInfo.OrderByString, totalNum, ListInfo.PageNum, currentPageIndex);
                    var pageSqlString = DataProvider.DatabaseRepository.GetStlPageSqlString(SqlString, ListInfo.OrderByString, totalNum, ListInfo.PageNum, currentPageIndex);

                    var datasource = DataProvider.DatabaseRepository.GetDataSource(pageSqlString);

                    if (ListInfo.Layout == ELayout.None)
                    {
                        var rptContents = new Repeater();

                        if (!string.IsNullOrEmpty(ListInfo.HeaderTemplate))
                        {
                            rptContents.HeaderTemplate = new SeparatorTemplate(ListInfo.HeaderTemplate);
                        }
                        if (!string.IsNullOrEmpty(ListInfo.FooterTemplate))
                        {
                            rptContents.FooterTemplate = new SeparatorTemplate(ListInfo.FooterTemplate);
                        }
                        if (!string.IsNullOrEmpty(ListInfo.SeparatorTemplate))
                        {
                            rptContents.SeparatorTemplate = new SeparatorTemplate(ListInfo.SeparatorTemplate);
                        }
                        if (!string.IsNullOrEmpty(ListInfo.AlternatingItemTemplate))
                        {
                            rptContents.AlternatingItemTemplate = new RepeaterTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, EContextType.Content, ContextInfo);
                        }

                        rptContents.ItemTemplate = new RepeaterTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, EContextType.Content, ContextInfo);

                        rptContents.DataSource = datasource;
                        rptContents.DataBind();

                        if (rptContents.Items.Count > 0)
                        {
                            parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                        }
                    }
                    else
                    {
                        var pdlContents = new ParsedDataList();

                        //设置显示属性
                        TemplateUtility.PutListInfoToMyDataList(pdlContents, ListInfo);

                        pdlContents.ItemTemplate = new DataListTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, EContextType.Content, ContextInfo);
                        if (!string.IsNullOrEmpty(ListInfo.HeaderTemplate))
                        {
                            pdlContents.HeaderTemplate = new SeparatorTemplate(ListInfo.HeaderTemplate);
                        }
                        if (!string.IsNullOrEmpty(ListInfo.FooterTemplate))
                        {
                            pdlContents.FooterTemplate = new SeparatorTemplate(ListInfo.FooterTemplate);
                        }
                        if (!string.IsNullOrEmpty(ListInfo.SeparatorTemplate))
                        {
                            pdlContents.SeparatorTemplate = new SeparatorTemplate(ListInfo.SeparatorTemplate);
                        }
                        if (!string.IsNullOrEmpty(ListInfo.AlternatingItemTemplate))
                        {
                            pdlContents.AlternatingItemTemplate = new DataListTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, PageInfo, EContextType.Content, ContextInfo);
                        }

                        pdlContents.DataSource = datasource;
                        pdlContents.DataKeyField = ContentAttribute.Id;
                        pdlContents.DataBind();

                        if (pdlContents.Items.Count > 0)
                        {
                            parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                parsedContent = await LogUtils.AddStlErrorLogAsync(PageInfo, ElementName, StlPageContentsElement, ex);
            }

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            ContextInfo.PageItemIndex = 0;
            return parsedContent;
        }

        private string ParseDynamic(int totalNum, int currentPageIndex, int pageCount)
        {
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

            PageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(PageInfo.UniqueId);
            var apiUrl = ApiRouteActionsPageContents.GetUrl(PageInfo.ApiUrl);
            var apiParameters = ApiRouteActionsPageContents.GetParameters(PageInfo.SiteId, PageInfo.PageChannelId, PageInfo.Template.Id, totalNum, pageCount, currentPageIndex, StlPageContentsElement);

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