using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.Core;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;
using SS.CMS.StlParser.Utility;

namespace SS.CMS.Services
{
    public partial class ParseManager
    {
        public async Task ParseTemplateContentAsync(StringBuilder parsedBuilder, ParsePage pageInfo, ParseContext contextInfo)
        {
            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = false;
            contextInfo.ContainerClientId = string.Empty;
            await ReplaceStlElementsAsync(parsedBuilder);
            await ReplaceStlEntitiesAsync(parsedBuilder);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public async Task<string> ParseTemplatePreviewAsync(Site site, TemplateType templateType, int channelId, int contentId, string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var templateInfo = new Template
            {
                TemplateType = templateType
            };
            var config = await DatabaseManager.ConfigRepository.GetAsync();
            var pageInfo = ParsePage.GetPageInfo(PathManager, config, channelId, contentId, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ParseContext(pageInfo);

            var parsedBuilder = new StringBuilder(template);
            
            await ParseTemplateContentAsync(parsedBuilder, pageInfo, contextInfo);

            return pageInfo.HeadCodesHtml + pageInfo.BodyCodesHtml + parsedBuilder + pageInfo.FootCodesHtml;
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

        public async Task ReplacePageElementsInContentPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int channelId, int contentId, int currentPageIndex, int pageCount)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInContentPageAsync(stlElement, channelId, contentId, currentPageIndex, pageCount);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInContentPageAsync(stlElement, channelId, contentId, currentPageIndex, pageCount, pageCount);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public async Task ReplacePageElementsInChannelPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInChannelPageAsync(stlElement, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInChannelPageAsync(stlElement, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public async Task ReplacePageElementsInSearchPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInSearchPageAsync(stlElement, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInSearchPageAsync(stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public async Task ReplacePageElementsInDynamicPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageInDynamicPageAsync(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await ParseStlPageItemInDynamicPageAsync(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }
    }
}
