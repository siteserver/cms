using System.Collections.Generic;
using System.Text;
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
        public static void ParseTemplateContent(StringBuilder parsedBuilder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = false;
            contextInfo.ContainerClientId = string.Empty;
            StlElementParser.ReplaceStlElements(parsedBuilder, pageInfo, contextInfo);
            StlEntityParser.ReplaceStlEntities(parsedBuilder, pageInfo, contextInfo);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public static string ParseTemplatePreview(SiteInfo siteInfo, TemplateType templateType, int channelId, int contentId, string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var templateInfo = new TemplateInfo
            {
                TemplateType = templateType
            };
            var pageInfo = new PageInfo(channelId, contentId, siteInfo, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);

            var parsedBuilder = new StringBuilder(template);
            
            ParseTemplateContent(parsedBuilder, pageInfo, contextInfo);

            return pageInfo.HeadCodesHtml + pageInfo.BodyCodesHtml + parsedBuilder + pageInfo.FootCodesHtml;
        }

        public static void ParseInnerContent(StringBuilder builder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            if (builder == null || builder.Length == 0) return;

            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = true;
            StlElementParser.ReplaceStlElements(builder, pageInfo, contextInfo);
            StlEntityParser.ReplaceStlEntities(builder, pageInfo, contextInfo);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public static string ParseInnerContent(string template, PageInfo pageInfo, ContextInfo contextInfo)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var builder = new StringBuilder(template);
            ParseInnerContent(builder, pageInfo, contextInfo);
            return builder.ToString();
        }

        public static string ParseInnerContent(string template, ParseContextImpl context)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var builder = new StringBuilder(template);
            var siteInfo = SiteManager.GetSiteInfo(context.SiteId);
            var templateInfo = new TemplateInfo
            {
                Id = context.TemplateId,
                TemplateType = context.TemplateType
            };
            var pageInfo = new PageInfo(context.ChannelId, context.ContentId, siteInfo, templateInfo, context.PluginItems);
            var contextInfo = new ContextInfo(pageInfo);
            ParseInnerContent(builder, pageInfo, contextInfo);
            return builder.ToString();
        }

        //public static void ParseInnerContent(StringBuilder builder, int siteId, int channelId, int contentId)
        //{
        //    var siteInfo = SiteManager.GetSiteInfo(siteId);
        //    var pageInfo = new PageInfo(channelId, contentId, siteInfo, null, null);
        //    var contextInfo = new ContextInfo(pageInfo);
        //    ParseInnerContent(builder, pageInfo, contextInfo);
        //}

        public static string StlEncrypt(string stlElement)
        {
            return $"<!-- {TranslateUtils.EncryptStringBySecretKey(stlElement)} -->";
        }

        public static void ReplacePageElementsInContentPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, int channelId, int contentId, int currentPageIndex, int pageCount)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInContentPage(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInContentPage(stlElement, pageInfo, channelId, contentId, currentPageIndex, pageCount, pageCount);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static void ReplacePageElementsInChannelPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInChannelPage(stlElement, pageInfo, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInChannelPage(stlElement, pageInfo, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static void ReplacePageElementsInSearchPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, string ajaxDivId, int channelId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInSearchPage(stlElement, pageInfo, ajaxDivId, channelId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInSearchPage(stlElement, pageInfo, ajaxDivId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static void ReplacePageElementsInDynamicPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, string pageUrl, int channelId, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInDynamicPage(stlElement, pageInfo, pageUrl, channelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInDynamicPage(stlElement, pageInfo, pageUrl, channelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }
    }
}
