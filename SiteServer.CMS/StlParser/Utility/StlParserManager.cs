using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.Utility
{
    public static class StlParserManager
    {
        public static async Task ParseTemplateContentAsync(StringBuilder parsedBuilder, PageInfo pageInfo, ContextInfo contextInfo)
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
                Type = templateType
            };
            var pageInfo = await PageInfo.GetPageInfoAsync(channelId, contentId, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);

            var parsedBuilder = new StringBuilder(template);
            
            await ParseTemplateContentAsync(parsedBuilder, pageInfo, contextInfo);

            return pageInfo.HeadCodesHtml + pageInfo.BodyCodesHtml + parsedBuilder + pageInfo.FootCodesHtml;
        }

        public static async Task ParseInnerContentAsync(StringBuilder builder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            if (builder == null || builder.Length == 0) return;

            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = true;
            await StlElementParser.ReplaceStlElementsAsync(builder, pageInfo, contextInfo);
            await StlEntityParser.ReplaceStlEntitiesAsync(builder, pageInfo, contextInfo);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public static async Task<string> ParseInnerContentAsync(string template, ParseContextImpl context)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var builder = new StringBuilder(template);
            var site = await DataProvider.SiteDao.GetAsync(context.SiteId);
            var templateInfo = new Template
            {
                Id = context.TemplateId,
                Type = context.TemplateType
            };
            var pageInfo = await PageInfo.GetPageInfoAsync(context.ChannelId, context.ContentId, site, templateInfo, context.PluginItems);
            var contextInfo = new ContextInfo(pageInfo);
            await ParseInnerContentAsync(builder, pageInfo, contextInfo);
            return builder.ToString();
        }

        //public static void ParseInnerContent(StringBuilder builder, int siteId, int channelId, int contentId)
        //{
        //    var site = DataProvider.SiteDao.GetSite(siteId);
        //    var pageInfo = new PageInfo(channelId, contentId, site, null, null);
        //    var contextInfo = new ContextInfo(pageInfo);
        //    ParseInnerContent(builder, pageInfo, contextInfo);
        //}

        public static string StlEncrypt(string stlElement)
        {
            return $"<!-- {WebConfigUtils.EncryptStringBySecretKey(stlElement)} -->";
        }

        public static async Task ReplacePageElementsInContentPageAsync(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, int channelId, int contentId, int currentPageIndex, int pageCount)
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

        public static async Task ReplacePageElementsInChannelPageAsync(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, int channelId, int currentPageIndex, int pageCount, int totalNum)
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

        public static async Task ReplacePageElementsInSearchPageAsync(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
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

        public static async Task ReplacePageElementsInDynamicPageAsync(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
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
