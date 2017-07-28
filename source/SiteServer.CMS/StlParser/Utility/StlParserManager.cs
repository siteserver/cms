using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlParserManager
    {
        private StlParserManager()
        {
        }

        public static string ParsePreviewContent(PublishmentSystemInfo publishmentSystemInfo, string content)
        {
            var templateInfo = new TemplateInfo();
            var pageInfo = new PageInfo(publishmentSystemInfo.PublishmentSystemId, 0, publishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);

            var parsedBuilder = new StringBuilder(content);

            StlElementParser.ReplaceStlElements(parsedBuilder, pageInfo, contextInfo);
            StlEntityParser.ReplaceStlEntities(parsedBuilder, pageInfo, contextInfo);

            var pageAfterBodyScripts = GetPageInfoScript(pageInfo, true);
            var pageBeforeBodyScripts = GetPageInfoScript(pageInfo, false);

            return pageAfterBodyScripts + parsedBuilder + pageBeforeBodyScripts;
        }

        public static void ParseTemplateContent(StringBuilder parsedBuilder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = false;
            contextInfo.ContainerClientId = string.Empty;
            StlElementParser.ReplaceStlElements(parsedBuilder, pageInfo, contextInfo);
            StlEntityParser.ReplaceStlEntities(parsedBuilder, pageInfo, contextInfo);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public static void ParseInnerContent(StringBuilder parsedBuilder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var isInnerElement = contextInfo.IsInnerElement;
            contextInfo.IsInnerElement = true;
            StlElementParser.ReplaceStlElements(parsedBuilder, pageInfo, contextInfo);
            StlEntityParser.ReplaceStlEntities(parsedBuilder, pageInfo, contextInfo);
            contextInfo.IsInnerElement = isInnerElement;
        }

        public static void ReplacePageElementsInContentPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, int nodeId, int contentId, int currentPageIndex, int pageCount)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInContentPage(stlElement, pageInfo, nodeId, contentId, currentPageIndex, pageCount);
                    parsedBuilder.Replace(StlPageItems.Translate(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInContentPage(stlElement, pageInfo, nodeId, contentId, currentPageIndex, pageCount, pageCount);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static void ReplacePageElementsInChannelPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, int nodeId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInChannelPage(stlElement, pageInfo, nodeId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(StlPageItems.Translate(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInChannelPage(stlElement, pageInfo, nodeId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static void ReplacePageElementsInSearchPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, string ajaxDivId, int nodeId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInSearchPage(stlElement, pageInfo, ajaxDivId, nodeId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInSearchPage(stlElement, pageInfo, ajaxDivId, nodeId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static void ReplacePageElementsInDynamicPage(StringBuilder parsedBuilder, PageInfo pageInfo, List<string> labelList, string pageUrl, int nodeId, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageInDynamicPage(stlElement, pageInfo, pageUrl, nodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = StlPageElementParser.ParseStlPageItemInDynamicPage(stlElement, pageInfo, pageUrl, nodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public static string GetPageInfoHeadScript(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var builder = new StringBuilder();

            builder.Append(
                $@"<script>var $pageInfo = {{publishmentSystemID : {pageInfo.PublishmentSystemId}, channelID : {pageInfo.PageNodeId}, contentID : {pageInfo.PageContentId}, siteUrl : ""{pageInfo.PublishmentSystemInfo.PublishmentSystemUrl.TrimEnd('/')}"", homeUrl : ""{pageInfo.HomeUrl.TrimEnd('/')}"", currentUrl : ""{StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelId, contextInfo.ContentId, contextInfo.ContentInfo)}"", rootUrl : ""{PageUtils.GetRootUrl(string.Empty).TrimEnd('/')}"", apiUrl : ""{pageInfo.ApiUrl.TrimEnd('/')}""}};</script>");

            foreach (string key in pageInfo.PageHeadScriptKeys)
            {
                var js = pageInfo.GetPageHeadScripts(key);
                if (!string.IsNullOrEmpty(js))
                {
                    builder.Append(js);
                }
            }

            return builder.ToString();
        }

        public static string GetPageInfoScript(PageInfo pageInfo, bool isAfterBody)
        {
            var scriptBuilder = new StringBuilder();

            if (isAfterBody)
            {
                foreach (string key in pageInfo.PageAfterBodyScriptKeys)
                {
                    scriptBuilder.Append(pageInfo.GetPageScripts(key, true));
                }
            }
            else
            {
                foreach (string key in pageInfo.PageBeforeBodyScriptKeys)
                {
                    scriptBuilder.Append(pageInfo.GetPageScripts(key, false));
                }
            }

            return scriptBuilder.ToString();
        }
    }
}
