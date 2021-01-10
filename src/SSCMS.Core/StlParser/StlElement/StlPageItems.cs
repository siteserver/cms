using System;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "翻页项容器", Description = "通过 stl:pageItems 标签在模板中插入翻页项的容器，当不需要翻页时容器内的内容不显示")]
    public static class StlPageItems
    {
        public const string ElementName = "stl:pageItems";

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        //对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public static async Task<string> ParseAsync(IParseManager parseManager, string stlElement, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum, ParseType contextType)
        {
            var pageInfo = parseManager.PageInfo;

            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);
            string parsedContent;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement, -1);
                if (stlElementInfo.Attributes[Context] != null)
                {
                    contextType = TranslateUtils.ToEnum(stlElementInfo.Attributes[Context], ParseType.Undefined);
                }

                if (pageCount <= 1)
                {
                    return string.Empty;
                }

                bool isXmlContent;
                var index = stlElement.IndexOf(">", StringComparison.Ordinal) + 1;
                var length = stlElement.LastIndexOf("<", StringComparison.Ordinal) - index;
                if (index <= 0 || length <= 0)
                {
                    stlElement = stlElementInfo.InnerHtml;
                    isXmlContent = true;
                }
                else
                {
                    stlElement = stlElement.Substring(index, length);
                    isXmlContent = false;
                }

                parsedContent = await parseManager.ParseStlPageItemsAsync(stlElement, channelId, contentId, currentPageIndex, pageCount, totalNum, isXmlContent, contextType);

                var innerBuilder = new StringBuilder(parsedContent);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                parsedContent = innerBuilder.ToString();
            }
            catch (Exception ex)
            {
                parsedContent =
                    await parseManager.AddStlErrorLogAsync(ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static async Task<string> ParseInSearchPageAsync(IParseManager parseManager, string stlElement, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            string parsedContent;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement, -1);

                if (pageCount <= 1)
                {
                    return string.Empty;
                }

                //bool isXmlContent;
                var index = stlElement.IndexOf(">", StringComparison.Ordinal) + 1;
                var length = stlElement.LastIndexOf("<", StringComparison.Ordinal) - index;
                if (index <= 0 || length <= 0)
                {
                    stlElement = stlElementInfo.InnerHtml;
                    //isXmlContent = true;
                }
                else
                {
                    stlElement = stlElement.Substring(index, length);
                    //isXmlContent = false;
                }

                parsedContent = await parseManager.ParseStlPageItemsInSearchPageAsync(stlElement, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
            }
            catch (Exception ex)
            {
                parsedContent = await parseManager.AddStlErrorLogAsync(ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static async Task<string> ParseInDynamicPageAsync(IParseManager parseManager, string stlElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            string parsedContent;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement, -1);

                if (pageCount <= 1)
                {
                    return string.Empty;
                }

                var index = stlElement.IndexOf(">", StringComparison.Ordinal) + 1;
                var length = stlElement.LastIndexOf("<", StringComparison.Ordinal) - index;
                if (index <= 0 || length <= 0)
                {
                    stlElement = stlElementInfo.InnerHtml;
                }
                else
                {
                    stlElement = stlElement.Substring(index, length);
                }

                parsedContent = await parseManager.ParseStlPageItemsInDynamicPageAsync(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
            }
            catch (Exception ex)
            {
                parsedContent = await parseManager.AddStlErrorLogAsync(ElementName, stlElement, ex);
            }

            return parsedContent;
        }
    }
}
