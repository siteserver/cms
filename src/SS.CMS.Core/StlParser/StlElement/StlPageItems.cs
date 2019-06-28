using System;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "翻页项容器", Description = "通过 stl:pageItems 标签在模板中插入翻页项的容器，当不需要翻页时容器内的内容不显示")]
    public class StlPageItems
    {
        private StlPageItems() { }
        public const string ElementName = "stl:pageItems";

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        //对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public static async Task<string> ParseAsync(ParseContext parseContext, string stlElement, int currentPageIndex, int pageCount, int totalNum)
        {
            parseContext.PageInfo.AddPageBodyCodeIfNotExists(parseContext.UrlManager, PageInfo.Const.Jquery);
            string parsedContent;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);
                if (stlElementInfo.Attributes[Context] != null)
                {
                    parseContext.ContextType = EContextTypeUtils.GetEnumType(stlElementInfo.Attributes[Context]);
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

                parsedContent = await parseContext.ParseStlPageItemsAsync(stlElement, currentPageIndex, pageCount, totalNum, isXmlContent);
            }
            catch (Exception ex)
            {
                parsedContent =
                    await parseContext.GetErrorMessageAsync(ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static async Task<string> ParseInSearchPageAsync(ParseContext parseContext, string stlElement, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            string parsedContent;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);

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

                parsedContent = await parseContext.ParseStlPageItemsInSearchPageAsync(stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
            }
            catch (Exception ex)
            {
                parsedContent = await parseContext.GetErrorMessageAsync(ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static async Task<string> ParseInDynamicPageAsync(ParseContext parseContext, string stlElement, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            string parsedContent;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);

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

                parsedContent = await parseContext.ParseStlPageItemsInDynamicPageAsync(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
            }
            catch (Exception ex)
            {
                parsedContent = await parseContext.GetErrorMessageAsync(ElementName, stlElement, ex);
            }

            return parsedContent;
        }
    }
}
