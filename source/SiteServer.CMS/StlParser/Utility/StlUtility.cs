using System;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Advertisement;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlUtility
    {
        public static string GetStlCurrentUrl(PublishmentSystemInfo publishmentSystemInfo, int channelId, int contentId, IContentInfo contentInfo, ETemplateType templateType, int templateId)
        {
            var currentUrl = string.Empty;
            if (templateType == ETemplateType.IndexPageTemplate)
            {
                currentUrl = publishmentSystemInfo.PublishmentSystemUrl;
            }
            else if (templateType == ETemplateType.ContentTemplate)
            {
                if (contentInfo == null)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId);
                    currentUrl = PageUtility.GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, false);
                }
                else
                {
                    currentUrl = PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo);
                }
            }
            else if (templateType == ETemplateType.ChannelTemplate)
            {
                currentUrl = PageUtility.GetChannelUrl(publishmentSystemInfo, NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelId));
            }
            else if (templateType == ETemplateType.FileTemplate)
            {
                currentUrl = PageUtility.GetFileUrl(publishmentSystemInfo, templateId);
            }
            //currentUrl是当前页面的地址，前后台分离的时候，不允许带上protocol
            //return PageUtils.AddProtocolToUrl(currentUrl);
            return currentUrl;
        }

        public static bool IsAdvertisementExists(PageInfo pageInfo)
        {
            var arraylists = AdvertisementManager.GetAdvertisementArrayLists(pageInfo.PublishmentSystemId);
            if (pageInfo.TemplateInfo.TemplateType == ETemplateType.IndexPageTemplate || pageInfo.TemplateInfo.TemplateType == ETemplateType.ChannelTemplate)
            {
                var arraylist = arraylists[0];
                return arraylist.Contains(pageInfo.PageNodeId);
            }
            else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.ContentTemplate)
            {
                var arraylist = arraylists[1];
                return arraylist.Contains(pageInfo.PageContentId);
            }
            else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.FileTemplate)
            {
                var arraylist = arraylists[2];
                return arraylist.Contains(pageInfo.TemplateInfo.TemplateId);
            }
            return false;
        }

        public static bool IsSeoMetaExists(PageInfo pageInfo)
        {
            var arraylists = SeoManager.GetSeoMetaArrayLists(pageInfo.PublishmentSystemId);
            if (pageInfo.PageContentId != 0)
            {
                var arraylist = arraylists[1];
                return arraylist.Contains(pageInfo.PageNodeId);
            }
            else
            {
                var arraylist = arraylists[0];
                return arraylist.Contains(pageInfo.PageNodeId);
            }
        }

        public static void AddSeoMetaToContent(PageInfo pageInfo, StringBuilder contentBuilder)
        {
            if (IsSeoMetaExists(pageInfo))
            {
                int seoMetaId;
                if (pageInfo.PageContentId != 0)
                {
                    seoMetaId = SeoMeta.GetSeoMetaIdByNodeId(pageInfo.PageNodeId, false);
                    if (seoMetaId == 0)
                    {
                        seoMetaId = SeoMeta.GetDefaultSeoMetaId(pageInfo.PublishmentSystemId);
                    }
                }
                else
                {
                    seoMetaId = SeoMeta.GetSeoMetaIdByNodeId(pageInfo.PageNodeId, true);
                    if (seoMetaId == 0)
                    {
                        seoMetaId = SeoMeta.GetDefaultSeoMetaId(pageInfo.PublishmentSystemId);
                    }
                }
                if (seoMetaId != 0)
                {
                    var seoMetaInfo = SeoMeta.GetSeoMetaInfo(seoMetaId);
                    var seoMetaInfoFromTemplate = SeoManager.GetSeoMetaInfo(contentBuilder.ToString());
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.PageTitle)) seoMetaInfo.PageTitle = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Keywords)) seoMetaInfo.Keywords = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Description)) seoMetaInfo.Description = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Copyright)) seoMetaInfo.Copyright = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Author)) seoMetaInfo.Author = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Email)) seoMetaInfo.Email = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Language)) seoMetaInfo.Language = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Charset)) seoMetaInfo.Charset = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Distribution)) seoMetaInfo.Distribution = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Rating)) seoMetaInfo.Rating = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Robots)) seoMetaInfo.Robots = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.RevisitAfter)) seoMetaInfo.RevisitAfter = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Expires)) seoMetaInfo.Expires = string.Empty;

                    var metaContent = SeoManager.GetMetaContent(seoMetaInfo);

                    if (!string.IsNullOrEmpty(metaContent))
                    {
                        StringUtils.InsertBefore("</head>", contentBuilder, metaContent);
                    }
                }
            }
        }

        public static void AddAdvertisementsToContent(PageInfo pageInfo)
        {
            if (!IsAdvertisementExists(pageInfo)) return;

            var advertisementNameList = Advertisement.GetAdvertisementNameList(pageInfo.PublishmentSystemId);

            foreach (var advertisementName in advertisementNameList)
            {
                var adInfo = Advertisement.GetAdvertisementInfo(advertisementName, pageInfo.PublishmentSystemId);
                if (adInfo.IsDateLimited)
                {
                    if (DateTime.Now < adInfo.StartDate || DateTime.Now > adInfo.EndDate)
                    {
                        continue;
                    }
                }
                var isToDo = false;
                if (pageInfo.TemplateInfo.TemplateType == ETemplateType.IndexPageTemplate || pageInfo.TemplateInfo.TemplateType == ETemplateType.ChannelTemplate)
                {
                    if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToChannel))
                    {
                        var nodeIdArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToChannel);
                        if (nodeIdArrayList.Contains(pageInfo.PageNodeId))
                        {
                            isToDo = true;
                        }
                    }
                }
                else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.ContentTemplate)
                {
                    if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToContent))
                    {
                        var nodeIdArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToContent);
                        if (nodeIdArrayList.Contains(pageInfo.PageContentId))
                        {
                            isToDo = true;
                        }
                    }
                }
                else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.FileTemplate)
                {
                    if (!string.IsNullOrEmpty(adInfo.FileTemplateIDCollection))
                    {
                        var fileTemplateIdArrayList = TranslateUtils.StringCollectionToIntList(adInfo.FileTemplateIDCollection);
                        if (fileTemplateIdArrayList.Contains(pageInfo.TemplateInfo.TemplateId))
                        {
                            isToDo = true;
                        }
                    }
                }

                if (isToDo)
                {
                    var scripts = string.Empty;
                    if (adInfo.AdvertisementType == EAdvertisementType.FloatImage)
                    {
                        pageInfo.AddPageScriptsIfNotExists(PageInfo.JsStaticAdFloating);

                        var floatScript = new FloatingScript(pageInfo.PublishmentSystemInfo, pageInfo.UniqueId, adInfo);
                        scripts = floatScript.GetScript();
                    }
                    else if (adInfo.AdvertisementType == EAdvertisementType.ScreenDown)
                    {
                        pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

                        var screenDownScript = new ScreenDownScript(pageInfo.PublishmentSystemInfo, pageInfo.UniqueId, adInfo);
                        scripts = screenDownScript.GetScript();
                    }
                    else if (adInfo.AdvertisementType == EAdvertisementType.OpenWindow)
                    {
                        var openWindowScript = new OpenWindowScript(pageInfo.PublishmentSystemInfo, pageInfo.UniqueId, adInfo);
                        scripts = openWindowScript.GetScript();
                    }

                    pageInfo.AddPageEndScriptsIfNotExists(EAdvertisementTypeUtils.GetValue(adInfo.AdvertisementType) + "_" + adInfo.AdvertisementName, scripts);
                }
            }
        }

        public static string ParseDynamicContent(int publishmentSystemId, int channelId, int contentId, int templateId, bool isPageRefresh, string templateContent, string pageUrl, int pageIndex, string ajaxDivId, NameValueCollection queryString, UserInfo userInfo)
        {
            var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemId, templateId);
            //TemplateManager.GetTemplateInfo(publishmentSystemID, channelID, templateType);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var pageInfo = new PageInfo(channelId, contentId, publishmentSystemInfo, templateInfo, userInfo);
            pageInfo.SetUniqueId(1000);
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

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

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

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageComments>
            else if (StlParserUtility.IsStlElementExists(StlPageComments.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageComments.ElementName, stlElementList);
                var stlPageCommentsElement = stlElement;
                var stlPageCommentsElementReplaceString = stlElement;

                var pageCommentsElementParser = new StlPageComments(stlPageCommentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageCommentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageCommentsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageCommentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageinputContents>
            else if (StlParserUtility.IsStlElementExists(StlPageInputContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageInputContents.ElementName, stlElementList);
                var stlPageInputContentsElement = stlElement;
                var stlPageInputContentsElementReplaceString = stlElement;

                var pageInputContentsElementParser = new StlPageInputContents(stlPageInputContentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageInputContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageInputContentsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageInputContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

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

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }

            else if (StlParserUtility.IsStlElementExists(StlPageItems.ElementName, stlElementList))
            {
                var pageCount = TranslateUtils.ToInt(queryString["pageCount"]);
                var totalNum = TranslateUtils.ToInt(queryString["totalNum"]);
                var pageContentsAjaxDivId = queryString["pageContentsAjaxDivID"];

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, pageContentsAjaxDivId);

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
