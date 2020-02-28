using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.Core;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Parsers;
using SS.CMS.StlParser.StlElement;


namespace SS.CMS.StlParser.Utility
{
    public static class StlParserManager
    {
        public static async Task ParseTemplateContentAsync(StringBuilder parsedBuilder, ParsePage pageInfo, ParseContext contextInfo)
        {
            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = false;
            contextInfo.ContainerClientId = string.Empty;
            await StlElementParser.ReplaceStlElementsAsync(parsedBuilder, pageInfo, contextInfo);
            await StlEntityParser.ReplaceStlEntitiesAsync(parsedBuilder, pageInfo, contextInfo);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public static async Task<string> ParseTemplatePreviewAsync(Site site, TemplateType templateType, int channelId, int contentId, string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var templateInfo = new Template
            {
                TemplateType = templateType
            };
            var pageInfo = await ParsePage.GetPageInfoAsync(channelId, contentId, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ParseContext(pageInfo);

            var parsedBuilder = new StringBuilder(template);
            
            await ParseTemplateContentAsync(parsedBuilder, pageInfo, contextInfo);

            return pageInfo.HeadCodesHtml + pageInfo.BodyCodesHtml + parsedBuilder + pageInfo.FootCodesHtml;
        }

        public static async Task ParseInnerContentAsync(StringBuilder builder, ParsePage pageInfo, ParseContext contextInfo)
        {
            if (builder == null || builder.Length == 0) return;

            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = true;
            await StlElementParser.ReplaceStlElementsAsync(builder, pageInfo, contextInfo);
            await StlEntityParser.ReplaceStlEntitiesAsync(builder, pageInfo, contextInfo);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public static string StlEncrypt(string stlElement)
        {
            return $"<!-- {GlobalSettings.SettingsManager.Encrypt(stlElement)} -->";
        }

        public static async Task ReplacePageElementsInContentPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int channelId, int contentId, int currentPageIndex, int pageCount)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageInContentPageAsync(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageItemInContentPageAsync(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount, pageCount);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static async Task ReplacePageElementsInChannelPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageInChannelPageAsync(stlElement, pageInfo, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageItemInChannelPageAsync(stlElement, pageInfo, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static async Task ReplacePageElementsInSearchPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageInSearchPageAsync(stlElement, pageInfo, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageItemInSearchPageAsync(stlElement, pageInfo, ajaxDivId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static async Task ReplacePageElementsInDynamicPageAsync(StringBuilder parsedBuilder, ParsePage pageInfo, List<string> labelList, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageInDynamicPageAsync(stlElement, pageInfo, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = await StlPageElementParser.ParseStlPageItemInDynamicPageAsync(stlElement, pageInfo, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }
    }
}
