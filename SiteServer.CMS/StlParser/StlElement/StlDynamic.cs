using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "动态显示", Description = "通过 stl:dynamic 标签在模板中实现动态显示功能")]
    public class StlDynamic
    {
        private StlDynamic() { }
        public const string ElementName = "stl:dynamic";

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        [StlAttribute(Title = "翻页时是否刷新页面")]
        private const string IsPageRefresh = nameof(IsPageRefresh);

        [StlAttribute(Title = "动态请求发送前执行的JS代码")]
        private const string OnBeforeSend = nameof(OnBeforeSend);

        [StlAttribute(Title = "动态请求成功后执行的JS代码")]
        private const string OnSuccess = nameof(OnSuccess);

        [StlAttribute(Title = "动态请求结束后执行的JS代码")]
        private const string OnComplete = nameof(OnComplete);

        [StlAttribute(Title = "动态请求失败后执行的JS代码")]
        private const string OnError = nameof(OnError);

        internal static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            // 如果是实体标签则返回空
            if (contextInfo.IsStlEntity)
            {
                return string.Empty;
            }

            var isPageRefresh = false;
            var onBeforeSend = string.Empty;
            var onSuccess = string.Empty;
            var onComplete = string.Empty;
            var onError = string.Empty;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsPageRefresh))
                {
                    isPageRefresh = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnBeforeSend))
                {
                    onBeforeSend = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnSuccess))
                {
                    onSuccess = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnComplete))
                {
                    onComplete = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnError))
                {
                    onError = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
            }

            return ParseImpl(pageInfo, contextInfo, contextInfo.InnerHtml, isPageRefresh, onBeforeSend, onSuccess, onComplete, onError);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string templateContent, bool isPageRefresh, string onBeforeSend, string onSuccess, string onComplete, string onError)
        {
            pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.StlClient);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);

            var functionName = $"stlDynamic_{ajaxDivId}";

            if (string.IsNullOrEmpty(templateContent))
            {
                return string.Empty;
            }

            StlParserManager.ParseInnerContent(new StringBuilder(templateContent), pageInfo, contextInfo);

            var apiUrl = ApiRouteActionsDynamic.GetUrl(pageInfo.ApiUrl);
            var currentPageUrl = StlParserUtility.GetStlCurrentUrl(pageInfo.SiteInfo, contextInfo.ChannelId, contextInfo.ContentId, contextInfo.ContentInfo, pageInfo.TemplateInfo.TemplateType, pageInfo.TemplateInfo.Id, pageInfo.IsLocal);
            currentPageUrl = PageUtils.AddQuestionOrAndToUrl(currentPageUrl);
            var apiParameters = ApiRouteActionsDynamic.GetParameters(pageInfo.SiteId, contextInfo.ChannelId, contextInfo.ContentId, pageInfo.TemplateInfo.Id, currentPageUrl, ajaxDivId, isPageRefresh, templateContent);

            var builder = new StringBuilder();
            builder.Append($@"<span id=""{ajaxDivId}""></span>");

            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
function {functionName}(pageNum)
{{
    var url = ""{apiUrl}?"" + StlClient.getQueryString();
    var data = {apiParameters};
    if (pageNum && pageNum > 0)
    {{
        data.pageNum = pageNum;
    }}
    var containerId = '{ajaxDivId}';

    {onBeforeSend}

    stlClient.post(url, data, function (err, data, status) {{
        if (!err) {{
            {onSuccess}
            document.getElementById(containerId).innerHTML = data.html;
        }} else {{
            {onError}
        }}
        {onComplete}
    }});
}}
{functionName}(0);
</script>
");

            return builder.ToString();
        }

        internal static string ParseDynamicElement(string stlElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            stlElement = StringUtils.ReplaceIgnoreCase(stlElement, "isdynamic=\"true\"", string.Empty);
            return ParseImpl(pageInfo, contextInfo, stlElement, false, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public static string ParseDynamicContent(int siteId, int channelId, int contentId, int templateId, bool isPageRefresh, string templateContent, string pageUrl, int pageIndex, string ajaxDivId, NameValueCollection queryString, IUserInfo userInfo)
        {
            StlCacheManager.ClearAll();

            var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);
            //TemplateManager.GetTemplateInfo(siteID, channelID, templateType);
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var pageInfo = new PageInfo(channelId, contentId, siteInfo, templateInfo, new Dictionary<string, object>())
            {
                UniqueId = 1000,
                UserInfo = userInfo
            };
            var contextInfo = new ContextInfo(pageInfo);

            templateContent = StlRequestEntities.ParseRequestEntities(queryString, templateContent);
            var contentBuilder = new StringBuilder(templateContent);
            var stlElementList = StlParserUtility.GetStlElementList(contentBuilder.ToString());

            //如果标签中存在<stl:pageContents>
            if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlElementList);
                var stlPageContentsElement = stlElement;
                var stlPageContentsElementReplaceString = stlElement;

                var pageContentsElementParser = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo);
                var pageCount = pageContentsElementParser.GetPageCount(out var totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                        contentBuilder.Replace(stlPageContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlElementList);
                var stlPageChannelsElement = stlElement;
                var stlPageChannelsElementReplaceString = stlElement;

                var pageChannelsElementParser = new StlPageChannels(stlPageChannelsElement, pageInfo, contextInfo);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageChannelsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlElementList);
                var stlPageSqlContentsElement = stlElement;
                var stlPageSqlContentsElementReplaceString = stlElement;

                var pageSqlContentsElementParser = new StlPageSqlContents(stlPageSqlContentsElement, pageInfo, contextInfo);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageSqlContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                        contentBuilder.Replace(stlPageSqlContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }

            else if (StlParserUtility.IsStlElementExists(StlPageItems.ElementName, stlElementList))
            {
                var pageCount = TranslateUtils.ToInt(queryString["pageCount"]);
                var totalNum = TranslateUtils.ToInt(queryString["totalNum"]);
                var pageContentsAjaxDivId = queryString["pageContentsAjaxDivId"];

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, pageContentsAjaxDivId);

                        break;
                    }
                }
            }

            StlParserManager.ParseInnerContent(contentBuilder, pageInfo, contextInfo);

            //var parsedContent = StlParserUtility.GetBackHtml(contentBuilder.ToString(), pageInfo);
            //return pageInfo.HeadCodesHtml + pageInfo.BodyCodesHtml + parsedContent + pageInfo.FootCodesHtml;
            return contentBuilder.ToString();
        }
    }
}
