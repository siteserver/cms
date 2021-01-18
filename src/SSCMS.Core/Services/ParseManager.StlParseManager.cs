using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ParseManager
    {
        public async Task ParseTemplateContentAsync(StringBuilder parsedBuilder)
        {
            var isInnerElement = ContextInfo.IsInnerElement;
            ContextInfo.IsInnerElement = false;
            ContextInfo.ContainerClientId = string.Empty;
            await ReplaceStlElementsAsync(parsedBuilder);
            await ReplaceStlEntitiesAsync(parsedBuilder);
            ContextInfo.IsInnerElement = isInnerElement;
        }

        public async Task<string> ParseTemplateWithCodesHtmlAsync(string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;
            var parsedBuilder = new StringBuilder(template);

            await ParseTemplateContentAsync(parsedBuilder);

            return PageInfo.HeadCodesHtml + PageInfo.BodyCodesHtml + parsedBuilder + PageInfo.FootCodesHtml;
        }

        public async Task ParseInnerContentAsync(StringBuilder builder)
        {
            if (builder == null || builder.Length == 0) return;

            var isInnerElement = ContextInfo.IsInnerElement;
            ContextInfo.IsInnerElement = true;
            await ReplaceStlElementsAsync(builder);
            await ReplaceStlEntitiesAsync(builder);
            ContextInfo.IsInnerElement = isInnerElement;
        }

        public string StlEncrypt(string stlElement)
        {
            return $"<!-- {SettingsManager.Encrypt(stlElement)} -->";
        }

        public async Task ReplacePageElementsInContentPageAsync(StringBuilder parsedBuilder, List<string> labelList, int currentPageIndex, int pageCount)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInContentPageAsync(stlElement, PageInfo.PageChannelId, PageInfo.PageContentId, currentPageIndex, pageCount);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInContentPageAsync(stlElement, PageInfo.PageChannelId, PageInfo.PageContentId, currentPageIndex, pageCount, pageCount);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public async Task ReplacePageElementsInChannelPageAsync(StringBuilder parsedBuilder, List<string> labelList, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInChannelPageAsync(stlElement, PageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInChannelPageAsync(stlElement, PageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public async Task ReplacePageElementsInSearchPageAsync(StringBuilder parsedBuilder, List<string> labelList, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInSearchPageAsync(stlElement, ajaxDivId, PageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInSearchPageAsync(stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public async Task ReplacePageElementsInDynamicPageAsync(StringBuilder parsedBuilder, List<string> labelList, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInDynamicPageAsync(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (ParseUtils.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInDynamicPageAsync(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }
    }
}
