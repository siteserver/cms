using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Api.Sys.Stl;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "翻页内容列表", Description = "通过 stl:pageContents 标签在模板中显示翻页内容列表")]
    public class StlPageContents : StlContents
    {
        public new const string ElementName = "stl:pageContents";

        [StlAttribute(Title = "每页显示的内容数目")]
        public const string PageNum = nameof(PageNum);

        [StlAttribute(Title = "翻页中生成的静态页面最大数，剩余页面将动态获取")]
        public const string MaxPage = nameof(MaxPage);

        private readonly string _stlPageContentsElement;

        private readonly List<ContentInfo> _contentInfoList;
        private readonly ParseContext _parseContext;
        private readonly ListInfo _listInfo;

        public StlPageContents(string stlPageContentsElement, ParseContext parseContext)
        {
            _stlPageContentsElement = stlPageContentsElement;
            _parseContext = parseContext;

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement);

            _parseContext = parseContext.Clone(stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            parseContext.ContextType = EContextType.Content;
            _listInfo = ListInfo.GetListInfo(_parseContext);

            var channelId = StlDataUtility.GetChannelIdByLevel(parseContext.SiteId, _parseContext.ChannelId, _listInfo.UpLevel, _listInfo.TopLevel);

            channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(parseContext.SiteId, channelId, _listInfo.ChannelIndex, _listInfo.ChannelName);

            _contentInfoList = StlDataUtility.GetStlPageContentsSqlString(_parseContext.PluginManager, parseContext.SiteInfo, channelId, _listInfo);
        }

        //API StlActionsSearchController调用
        public StlPageContents(string stlPageContentsElement, ParseContext parseContext, int pageNum, ChannelInfo channelInfo)
        {
            _parseContext = parseContext;

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageContentsElement);
            _parseContext = parseContext.Clone(stlPageContentsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            _parseContext.ContextType = EContextType.Content;
            _listInfo = ListInfo.GetListInfo(_parseContext);

            _listInfo.Scope = ScopeType.All;

            if (pageNum > 0)
            {
                _listInfo.PageNum = pageNum;
            }

            _contentInfoList = StlDataUtility.GetPageContentsSqlStringBySearch(channelInfo, _listInfo.GroupContent, _listInfo.GroupContentNot, _listInfo.Tags, _listInfo.IsImage, _listInfo.IsVideo, _listInfo.IsFile, _listInfo.StartNum, _listInfo.TotalNum, _listInfo.Order, _listInfo.IsTop, _listInfo.IsRecommend, _listInfo.IsHot, _listInfo.IsColor);
        }

        public int GetPageCount(out int totalNum)
        {
            totalNum = 0;
            var pageCount = 1;
            try
            {
                //totalNum = DatabaseUtils.GetPageTotalCount(SqlString);
                // totalNum = StlDatabaseCache.GetPageTotalCount(_sqlString);
                if (_listInfo.PageNum != 0 && _listInfo.PageNum < totalNum)//需要翻页
                {
                    pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(_listInfo.PageNum)));//需要生成的总页数
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddStlErrorLog(_parseContext.PageInfo, ElementName, _stlPageContentsElement, ex);
            }
            return pageCount;
        }

        public string Parse(int totalNum, int currentPageIndex, int pageCount, bool isStatic)
        {
            if (isStatic)
            {
                var maxPage = _listInfo.MaxPage;
                if (maxPage == 0)
                {
                    maxPage = _parseContext.SiteInfo.CreateStaticMaxPage;
                }
                if (maxPage > 0 && currentPageIndex + 1 > maxPage)
                {
                    return ParseDynamic(totalNum, currentPageIndex, pageCount);
                }
            }

            var parsedContent = string.Empty;

            _parseContext.PageItemIndex = currentPageIndex * _listInfo.PageNum;

            try
            {
                // if (!string.IsNullOrEmpty(_sqlString))
                // {
                //     var contentList = _parseContext.ChannelInfo.ContentRepository.StlGetContainerContentListBySqlString(_parseContext.ChannelInfo, _sqlString, _listInfo.Order, totalNum, _listInfo.PageNum, currentPageIndex);
                //     parsedContent = StlContents.ParseElement(_parseContext, _listInfo, contentList);
                // }
            }
            catch (Exception ex)
            {
                parsedContent = LogUtils.AddStlErrorLog(_parseContext.PageInfo, ElementName, _stlPageContentsElement, ex);
            }

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            _parseContext.PageItemIndex = 0;
            return parsedContent;
        }

        private string ParseDynamic(int totalNum, int currentPageIndex, int pageCount)
        {
            var loading = _listInfo.LoadingTemplate;
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

            _parseContext.PageInfo.AddPageBodyCodeIfNotExists(_parseContext.UrlManager, PageInfo.Const.Jquery);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(_parseContext.UniqueId);
            var apiUrl = ApiRouteActionsPageContents.GetUrl(_parseContext.ApiUrl);
            var apiParameters = ApiRouteActionsPageContents.GetParameters(_parseContext.SettingsManager, _parseContext.SiteId, _parseContext.PageChannelId, _parseContext.TemplateInfo.Id, totalNum, pageCount, currentPageIndex, _stlPageContentsElement);

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