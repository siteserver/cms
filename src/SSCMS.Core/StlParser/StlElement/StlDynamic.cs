using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using Dynamic = SSCMS.Parse.Dynamic;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "动态显示", Description = "通过 stl:dynamic 标签在模板中实现动态显示功能")]
    public static partial class StlDynamic
    {
        public const string ElementName = "stl:dynamic";

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        [StlAttribute(Title = "是否在行内显示")]
        private const string Inline = nameof(Inline);

        [StlAttribute(Title = "动态请求发送前执行的JS代码")]
        private const string OnBeforeSend = nameof(OnBeforeSend);

        [StlAttribute(Title = "动态请求成功后执行的JS代码")]
        private const string OnSuccess = nameof(OnSuccess);

        [StlAttribute(Title = "动态请求结束后执行的JS代码")]
        private const string OnComplete = nameof(OnComplete);

        [StlAttribute(Title = "动态请求失败后执行的JS代码")]
        private const string OnError = nameof(OnError);

        internal static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var contextInfo = parseManager.ContextInfo;

            // 如果是实体标签则返回空
            if (contextInfo.IsStlEntity)
            {
                return string.Empty;
            }

            var inline = false;
            var onBeforeSend = string.Empty;
            var onSuccess = string.Empty;
            var onComplete = string.Empty;
            var onError = string.Empty;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    contextInfo.ContextType = TranslateUtils.ToEnum(value, ParseType.Undefined);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Inline))
                {
                    inline = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnBeforeSend))
                {
                    onBeforeSend = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnSuccess))
                {
                    onSuccess = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnComplete))
                {
                    onComplete = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnError))
                {
                    onError = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
            }

            StlParserUtility.GetLoading(contextInfo.InnerHtml, out var loading, out var template);
            if (!string.IsNullOrEmpty(loading))
            {
                var innerBuilder = new StringBuilder(loading);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                loading = innerBuilder.ToString();
            }

            return await ParseAsync(parseManager, contextInfo.Site, loading, template, inline, onBeforeSend, onSuccess, onComplete, onError);
        }

        internal static async Task<string> ParseAsync(string stlElement, IParseManager parseManager, Site site)
        {
            stlElement = StringUtils.ReplaceIgnoreCase(stlElement, "isdynamic=\"true\"", string.Empty);
            return await ParseAsync(parseManager, site, string.Empty, stlElement, true, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, Site site, string loading, string template, bool inline, string onBeforeSend, string onSuccess, string onComplete, string onError)
        {
            await parseManager.PageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.StlClient);

            var dynamicInfo = new Dynamic
            {
                SiteId = parseManager.PageInfo.SiteId,
                ChannelId = parseManager.ContextInfo.ChannelId,
                ContentId = parseManager.ContextInfo.ContentId,
                TemplateId = parseManager.PageInfo.Template.Id,
                ElementId = StringUtils.GetElementId(),
                LoadingTemplate = loading,
                YesTemplate = template,
                IsInline = inline,
                OnBeforeSend = onBeforeSend,
                OnSuccess = onSuccess,
                OnComplete = onComplete,
                OnError = onError
            };

            var dynamicUrl = parseManager.PathManager.GetDynamicApiUrl(site);
            return await GetScriptAsync(parseManager, dynamicUrl, dynamicInfo);
        }

        public static async Task<string> ParseDynamicAsync(IParseManager parseManager, Dynamic dynamicInfo, string template)
        {
            var databaseManager = parseManager.DatabaseManager;
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var templateInfo = await databaseManager.TemplateRepository.GetAsync(dynamicInfo.TemplateId);
            var siteInfo = await databaseManager.SiteRepository.GetAsync(dynamicInfo.SiteId);

            await parseManager.InitAsync(EditMode.Default, siteInfo, dynamicInfo.ChannelId, dynamicInfo.ContentId, templateInfo);

            parseManager.PageInfo.User = dynamicInfo.User;

            var templateContent = StlRequest.ParseRequestEntities(dynamicInfo.QueryString, template);
            var contentBuilder = new StringBuilder(templateContent);
            var stlElementList = ParseUtils.GetStlElements(contentBuilder.ToString());

            var pageIndex = dynamicInfo.Page - 1;

            //如果标签中存在<stl:pageContents>
            if (ParseUtils.IsStlElementExists(StlPageContents.ElementName, stlElementList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageContents.ElementName, stlElementList);
                var stlPageContentsElement = stlElement;
                var stlPageContentsElementReplaceString = stlElement;

                var pageContentsElementParser = await StlPageContents.GetAsync(stlPageContentsElement, parseManager, dynamicInfo.Query);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        contentBuilder.Replace(stlPageContentsElementReplaceString, pageHtml);

                        await parseManager.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, false, dynamicInfo.ElementId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (ParseUtils.IsStlElementExists(StlPageChannels.ElementName, stlElementList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageChannels.ElementName, stlElementList);
                var stlPageChannelsElement = stlElement;
                var stlPageChannelsElementReplaceString = stlElement;

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlPageChannelsElement, parseManager);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex != pageIndex) continue;

                    var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                    contentBuilder.Replace(stlPageChannelsElementReplaceString, pageHtml);

                    await parseManager.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, false, dynamicInfo.ElementId);

                    break;
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (ParseUtils.IsStlElementExists(StlPageSqlContents.ElementName, stlElementList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageSqlContents.ElementName, stlElementList);
                var stlPageSqlContentsElement = stlElement;
                var stlPageSqlContentsElementReplaceString = stlElement;

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlPageSqlContentsElement, parseManager);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex != pageIndex) continue;

                    var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                    contentBuilder.Replace(stlPageSqlContentsElementReplaceString, pageHtml);

                    await parseManager.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, false, dynamicInfo.ElementId);

                    break;
                }
            }
            else if (ParseUtils.IsStlElementExists(StlPageItems.ElementName, stlElementList))
            {
                var pageCount = TranslateUtils.ToInt(dynamicInfo.QueryString["pageCount"]);
                var totalNum = TranslateUtils.ToInt(dynamicInfo.QueryString["totalNum"]);
                var pageContentsAjaxDivId = dynamicInfo.QueryString["pageContentsAjaxDivId"];

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex != pageIndex) continue;

                    await parseManager.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, false, pageContentsAjaxDivId);

                    break;
                }
            }

            await parseManager.ParseInnerContentAsync(contentBuilder);

            //var parsedContent = StlParserUtility.GetBackHtml(contentBuilder.ToString(), pageInfo);
            //return pageInfo.HeadCodesHtml + pageInfo.BodyCodesHtml + parsedContent + pageInfo.FootCodesHtml;
            return contentBuilder.ToString();
        }
    }
}
