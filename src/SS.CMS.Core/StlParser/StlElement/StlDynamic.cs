using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Api.Sys.Stl;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlEntity;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "动态显示", Description = "通过 stl:dynamic 标签在模板中实现动态显示功能")]
    public class StlDynamic
    {
        private StlDynamic() { }
        public const string ElementName = "stl:dynamic";

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        [StlAttribute(Title = "动态请求发送前执行的JS代码")]
        private const string OnBeforeSend = nameof(OnBeforeSend);

        [StlAttribute(Title = "动态请求成功后执行的JS代码")]
        private const string OnSuccess = nameof(OnSuccess);

        [StlAttribute(Title = "动态请求结束后执行的JS代码")]
        private const string OnComplete = nameof(OnComplete);

        [StlAttribute(Title = "动态请求失败后执行的JS代码")]
        private const string OnError = nameof(OnError);

        internal static async Task<object> ParseAsync(ParseContext parseContext)
        {
            // 如果是实体标签则返回空
            if (parseContext.IsStlEntity)
            {
                return string.Empty;
            }

            var onBeforeSend = string.Empty;
            var onSuccess = string.Empty;
            var onComplete = string.Empty;
            var onError = string.Empty;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseContext.ContextType = EContextTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnBeforeSend))
                {
                    onBeforeSend = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnSuccess))
                {
                    onSuccess = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnComplete))
                {
                    onComplete = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnError))
                {
                    onError = await parseContext.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
            }

            StlParserUtility.GetLoading(parseContext.InnerHtml, out var loading, out var template);
            if (!string.IsNullOrEmpty(loading))
            {
                var innerBuilder = new StringBuilder(loading);
                await parseContext.ParseInnerContentAsync(innerBuilder);
                loading = innerBuilder.ToString();
            }

            return await ParseImplAsync(parseContext, loading, template, onBeforeSend, onSuccess, onComplete, onError);
        }

        private static async Task<string> ParseImplAsync(ParseContext parseContext, string loading, string template, string onBeforeSend, string onSuccess, string onComplete, string onError)
        {
            parseContext.PageInfo.AddPageBodyCodeIfNotExists(parseContext.UrlManager, PageInfo.Const.StlClient);

            //运行解析以便为页面生成所需JS引用
            if (!string.IsNullOrEmpty(template))
            {
                await parseContext.ParseInnerContentAsync(new StringBuilder(template));
            }

            var dynamicInfo = new DynamicInfo
            {
                ElementName = ElementName,
                SiteId = parseContext.SiteId,
                ChannelId = parseContext.ChannelId,
                ContentId = parseContext.ContentId,
                TemplateId = parseContext.TemplateInfo.Id,
                AjaxDivId = StlParserUtility.GetAjaxDivId(parseContext.UniqueId),
                LoadingTemplate = loading,
                SuccessTemplate = template,
                OnBeforeSend = onBeforeSend,
                OnSuccess = onSuccess,
                OnComplete = onComplete,
                OnError = onError
            };

            return dynamicInfo.GetScript(parseContext.SettingsManager, ApiRouteActionsDynamic.GetUrl());
        }

        internal static async Task<string> ParseDynamicElementAsync(string stlElement, ParseContext parseContext)
        {
            stlElement = StringUtils.ReplaceIgnoreCase(stlElement, "isdynamic=\"true\"", string.Empty);
            return await ParseImplAsync(parseContext, string.Empty, stlElement, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public static async Task<string> ParseDynamicContentAsync(ParseContext parseContext, DynamicInfo dynamicInfo, string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            // var templateInfo = TemplateManager.GetTemplateInfo(dynamicInfo.SiteId, dynamicInfo.TemplateId);
            // var siteInfo = SiteManager.GetSiteInfo(dynamicInfo.SiteId);
            // var pageInfo = new PageInfo(dynamicInfo.ChannelId, dynamicInfo.ContentId, siteInfo, templateInfo, new Dictionary<string, object>())
            // {
            //     UniqueId = 1000,
            //     UserInfo = dynamicInfo.UserInfo
            // };
            // var stlParserManager = new StlParserManager(pluginManager);
            // var contextInfo = new ParseContext(pageInfo, pluginManager, stlParserManager);

            var templateContent = StlRequestEntities.ParseRequestEntities(dynamicInfo.QueryString, template);
            var contentBuilder = new StringBuilder(templateContent);
            var stlElementList = StlParserUtility.GetStlElementList(contentBuilder.ToString());

            var pageIndex = dynamicInfo.Page - 1;

            //如果标签中存在<stl:pageContents>
            if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlElementList);
                var stlPageContentsElement = stlElement;
                var stlPageContentsElementReplaceString = stlElement;

                var pageContentsElementParser = new StlPageContents();
                await pageContentsElementParser.LoadAsync(stlPageContentsElement, parseContext);
                var (pageCount, totalNum) = await pageContentsElementParser.GetPageCountAsync();

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        contentBuilder.Replace(stlPageContentsElementReplaceString, pageHtml);

                        await parseContext.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, true, dynamicInfo.AjaxDivId);

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

                var pageChannelsElementParser = new StlPageChannels();
                await pageChannelsElementParser.LoadAsync(stlPageChannelsElement, parseContext);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageChannelsElementReplaceString, pageHtml);

                        await parseContext.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, true, dynamicInfo.AjaxDivId);

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

                var pageSqlContentsElementParser = new StlPageSqlContents();
                await pageSqlContentsElementParser.LoadAsync(stlPageSqlContentsElement, parseContext);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        contentBuilder.Replace(stlPageSqlContentsElementReplaceString, pageHtml);

                        await parseContext.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, true, dynamicInfo.AjaxDivId);

                        break;
                    }
                }
            }

            else if (StlParserUtility.IsStlElementExists(StlPageItems.ElementName, stlElementList))
            {
                var pageCount = TranslateUtils.ToInt(dynamicInfo.QueryString["pageCount"]);
                var totalNum = TranslateUtils.ToInt(dynamicInfo.QueryString["totalNum"]);
                var pageContentsAjaxDivId = dynamicInfo.QueryString["pageContentsAjaxDivId"];

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        await parseContext.ReplacePageElementsInDynamicPageAsync(contentBuilder, stlElementList, currentPageIndex, pageCount, totalNum, true, pageContentsAjaxDivId);

                        break;
                    }
                }
            }

            await parseContext.ParseInnerContentAsync(contentBuilder);

            //var parsedContent = StlParserUtility.GetBackHtml(contentBuilder.ToString(), pageInfo);
            //return pageInfo.HeadCodesHtml + pageInfo.BodyCodesHtml + parsedContent + pageInfo.FootCodesHtml;
            return contentBuilder.ToString();
        }
    }
}
