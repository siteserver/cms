using System;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "翻页项容器", Description = "通过 stl:pageItems 标签在模板中插入翻页项的容器，当不需要翻页时容器内的内容不显示")]
    public class StlPageItems
    {
        private StlPageItems() { }
        public const string ElementName = "stl:pageItems";

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        //对“翻页项容器”（stl:pageItems）元素进行解析，此元素在生成页面时单独解析，不包含在ParseStlElement方法中。
        public static async Task<string> ParseAsync(string stlElement, PageInfo pageInfo, int channelId, int contentId, int currentPageIndex, int pageCount, int totalNum, EContextType contextType)
        {
            pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
            string parsedContent;
            try
            {
                var stlElementInfo = StlParserUtility.ParseStlElement(stlElement);
                if (stlElementInfo.Attributes[Context] != null)
                {
                    contextType = EContextTypeUtils.GetEnumType(stlElementInfo.Attributes[Context]);
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

                parsedContent = await StlPageElementParser.ParseStlPageItemsAsync(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount, totalNum, isXmlContent, contextType);

                ContextInfo contextInfo = new ContextInfo(pageInfo)
                {
                    ContextType = contextType
                };
                var innerBuilder = new StringBuilder(parsedContent);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                parsedContent = innerBuilder.ToString();

            }
            catch (Exception ex)
            {
                parsedContent =
                    await LogUtils.AddStlErrorLogAsync(pageInfo, ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static async Task<string> ParseInSearchPageAsync(string stlElement, PageInfo pageInfo, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
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

                parsedContent = await StlPageElementParser.ParseStlPageItemsInSearchPageAsync(stlElement, pageInfo, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
            }
            catch (Exception ex)
            {
                parsedContent = await LogUtils.AddStlErrorLogAsync(pageInfo, ElementName, stlElement, ex);
            }

            return parsedContent;
        }

        public static async Task<string> ParseInDynamicPageAsync(string stlElement, PageInfo pageInfo, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
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

                parsedContent = await StlPageElementParser.ParseStlPageItemsInDynamicPageAsync(stlElement, pageInfo, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
            }
            catch (Exception ex)
            {
                parsedContent = await LogUtils.AddStlErrorLogAsync(pageInfo, ElementName, stlElement, ex);
            }

            return parsedContent;
        }
    }
}
