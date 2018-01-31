using System.Collections.Specialized;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlUtility
    {
        public static string GetStlCurrentUrl(SiteInfo siteInfo, int channelId, int contentId, IContentInfo contentInfo, TemplateType templateType, int templateId, bool isLocal)
        {
            var currentUrl = string.Empty;
            if (templateType == TemplateType.IndexPageTemplate)
            {
                currentUrl = siteInfo.Additional.WebUrl;
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                if (contentInfo == null)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    currentUrl = PageUtility.GetContentUrl(siteInfo, nodeInfo, contentId, isLocal);
                }
                else
                {
                    currentUrl = PageUtility.GetContentUrl(siteInfo, contentInfo, isLocal);
                }
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                currentUrl = PageUtility.GetChannelUrl(siteInfo, ChannelManager.GetChannelInfo(siteInfo.Id, channelId), isLocal);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                currentUrl = PageUtility.GetFileUrl(siteInfo, templateId, isLocal);
            }
            //currentUrl是当前页面的地址，前后台分离的时候，不允许带上protocol
            //return PageUtils.AddProtocolToUrl(currentUrl);
            return currentUrl;
        }

        public static string ParseDynamicContent(int siteId, int channelId, int contentId, int templateId, bool isPageRefresh, string templateContent, string pageUrl, int pageIndex, string ajaxDivId, NameValueCollection queryString, IUserInfo userInfo)
        {
            var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);
            //TemplateManager.GetTemplateInfo(siteID, channelID, templateType);
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var pageInfo = new PageInfo(channelId, contentId, siteInfo, templateInfo)
            {
                UniqueId = 1000,
                UserInfo = userInfo
            };
            var contextInfo = new ContextInfo(pageInfo);

            templateContent = StlRequestEntities.ParseRequestEntities(queryString, templateContent);
            var contentBuilder = new StringBuilder(templateContent);
            var stlElementList = StlParserUtility.GetStlElementList(contentBuilder.ToString());

            //如果标签中存在<stl:pageContents>
            if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlElementList);
                var stlPageContentsElement = stlElement;
                var stlPageContentsElementReplaceString = stlElement;

                var pageContentsElementParser = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                        contentBuilder.Replace(stlPageContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

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

                var pageChannelsElementParser = new StlPageChannels(stlPageChannelsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageChannelsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

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

                var pageSqlContentsElementParser = new StlPageSqlContents(stlPageSqlContentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageSqlContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }

            else if (StlParserUtility.IsStlElementExists(StlPageItems.ElementName, stlElementList))
            {
                var pageCount = TranslateUtils.ToInt(queryString["pageCount"]);
                var totalNum = TranslateUtils.ToInt(queryString["totalNum"]);
                var pageContentsAjaxDivId = queryString["pageContentsAjaxDivId"];

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum, isPageRefresh, pageContentsAjaxDivId);

                        break;
                    }
                }
            }

            StlParserManager.ParseInnerContent(contentBuilder, pageInfo, contextInfo);

            //string afterBodyScript = StlParserManager.GetPageInfoScript(pageInfo, true);
            //string beforBodyScript = StlParserManager.GetPageInfoScript(pageInfo, false);

            //return afterBodyScript + StlParserUtility.GetBackHtml(contentBuilder.ToString(), pageInfo) + beforBodyScript;

            return StlParserUtility.GetBackHtml(contentBuilder.ToString(), pageInfo);

            //return contentBuilder.ToString();
        }
    }
}
